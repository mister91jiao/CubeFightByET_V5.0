using ETModel;
using System;
using System.Collections.Generic;
using System.Net;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class Actor_PlayerToUnitSubHealthRequestHandler : AMActorRpcHandler<Unit, Actor_PlayerToUnitSubHealthRequest, Actor_PlayerToUnitSubHealthResponse>
    {
        protected override async ETTask Run(Unit unit, Actor_PlayerToUnitSubHealthRequest request, Actor_PlayerToUnitSubHealthResponse response, Action reply)
        {
            //是否攻击了已经死亡的玩家
            response.AttackDiePlayer = false;

            if (unit.Die)
            {
                response.AttackDiePlayer = true;
            }
            else
            {
                int newHealth = unit.SubHealth(request.SubHealth);

                response.UnitHealth = newHealth;
                response.Die = unit.Die;

                //这个玩家被打死了需要广播给其它玩家
                if (unit.Die)
                {
                    List<Unit> units = Game.Scene.GetComponent<UnitComponent>().getCountUnits(0);
                    if (units != null)
                    {
                        ActorMessageSenderComponent actorSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
                        for (int i = 0; i < units.Count; i++)
                        {
                            if (units[i].Account != unit.Account)
                            {
                                ActorMessageSender actorMessageSender = actorSenderComponent.Get(units[i].GateInstanceId);
                                actorMessageSender.Send(new Actor_OtherPlayerDie()
                                {
                                    DiePlayerAccount = unit.Account
                                });

                            }
                        }
                    }

                    //需要开启复活倒计时
                    UpDateNetSync(3000, unit.Account).Coroutine();

                    Log.Info("玩家：" + request.KillerAccount + " 打死了玩家 " + unit.Account);

                    RecordKillDataSendPackge(request.KillerAccount, unit.Account);

                }
            }

            reply();
            await ETTask.CompletedTask;
        }

        /// <summary>
        /// 记录击杀数据
        /// </summary>
        public void RecordKillDataSendPackge(int KillerAccount, int DeathAccount)
        {

            //获取内网发送组件
            IPEndPoint gateAddress = StartConfigComponent.Instance.GateConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
            Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gateAddress);
            gateSession.Send(new M2G_RecordKillData() { KillerAccount = KillerAccount, DeathAccount = DeathAccount });
        }

        /// <summary>
        /// 开启等待复活
        /// </summary>
        public async ETVoid UpDateNetSync(long ResurrectionTime, int ResurrectionAccount)
        {
            int Account = ResurrectionAccount;
            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(ResurrectionTime);

            UnitComponent unitComponent = Game.Scene.GetComponent<UnitComponent>();
            if (unitComponent.UnitHaveBeCreated(Account))
            {
                Unit ResurrectionUnit = unitComponent.getUnitByAccount(Account);
                ResurrectionUnit.ReviveHealth();
                List<Unit> units = unitComponent.getCountUnits(0);
                ActorMessageSenderComponent actorSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
                for (int i = 0; i < units.Count; i++)
                {
                    ActorMessageSender actorMessageSender = actorSenderComponent.Get(units[i].GateInstanceId);
                    actorMessageSender.Send(new Actor_PlayerResurrection()
                    {
                        ResurrectionPlayerAccount = ResurrectionUnit.Account,
                        PositionX = 0,
                        PositionY = 0,
                        PositionZ = 0,
                    });
                }
            }
            else
            {
                Log.Info("玩家：" + Account + " 可能已经离线了");
            }

        }
    }
}
