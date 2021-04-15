using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class PlayerNetSyncComponentAwakeSystem : AwakeSystem<PlayerNetSyncComponent>
    {
        public override void Awake(PlayerNetSyncComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class PlayerNetSyncComponentStartSystem : StartSystem<PlayerNetSyncComponent>
    {
        public override void Start(PlayerNetSyncComponent self)
        {
            self.Start();
        }
    }

    public class PlayerNetSyncComponent : Component
    {
        /// <summary>
        /// 计时器组件
        /// </summary>
        private TimerComponent timerComponent = null;

        /// <summary>
        /// Map服中单位管理组件
        /// </summary>
        private UnitComponent unitComponent = null;

        /// <summary>
        /// Map服中的子弹管理器
        /// </summary>
        private BulletManagerComponent bulletManagerComponent = null;

        /// <summary>
        /// 每秒发包次数，号码
        /// </summary>
        private long tick = 1000 / 32;

        public void Awake()
        {
            Log.Info("玩家角色同步组件初始化");
        }

        public void Start()
        {
            unitComponent = Game.Scene.GetComponent<UnitComponent>();
            timerComponent = Game.Scene.GetComponent<TimerComponent>();
            bulletManagerComponent = Game.Scene.GetComponent<BulletManagerComponent>();

            UpDateNetSync(tick).Coroutine();
        }

        public async ETVoid UpDateNetSync(long ttk)
        {
            while (true)
            {
                await timerComponent.WaitAsync(ttk);

                List<Unit> units = unitComponent.getCountUnits(0);

                if (units != null)
                {
                    ActorMessageSenderComponent actorSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();

                    List<BulletInfo> bulletInfos = bulletManagerComponent.GetAllNeedSyncBullet();
                    for (int i = 0; i < units.Count; i++)
                    {
                        ActorMessageSender actorMessageSender = actorSenderComponent.Get(units[i].GateInstanceId);
                        List<int> accounts = new List<int>();
                        List<float> positionX = new List<float>();
                        List<float> positionY = new List<float>();
                        List<float> positionZ = new List<float>();

                        List<float> rotationX = new List<float>();
                        List<float> rotationY = new List<float>();
                        List<float> rotationZ = new List<float>();
                        List<float> rotationW = new List<float>();

                        List<float> velocityX = new List<float>();
                        List<float> velocityY = new List<float>();
                        List<float> velocityZ = new List<float>();

                        List<bool> isFire = new List<bool>();

                        for (int j = 0; j < units.Count; j++)
                        {
                            //这个单位没有死亡
                            if (!units[j].Die)
                            {
                                //并且不是自己
                                if (units[i].Account != units[j].Account)
                                {
                                    accounts.Add(units[j].Account);
                                    positionX.Add(units[j].InitPositionX);
                                    positionY.Add(units[j].InitPositionY);
                                    positionZ.Add(units[j].InitPositionZ);

                                    rotationX.Add(units[j].RotationX);
                                    rotationY.Add(units[j].RotationY);
                                    rotationZ.Add(units[j].RotationZ);
                                    rotationW.Add(units[j].RotationW);

                                    velocityX.Add(units[j].VelocityX);
                                    velocityY.Add(units[j].VelocityY);
                                    velocityZ.Add(units[j].VelocityZ);

                                    isFire.Add(units[j].Fire);
                                }
                            }
                        }

                        actorMessageSender.Send(new Actor_PlayerNetSyncToCline()
                        {
                            DirAccount = accounts,
                            PositionX = positionX,
                            PositionY = positionY,
                            PositionZ = positionZ,

                            RotationX = rotationX,
                            RotationY = rotationY,
                            RotationZ = rotationZ,
                            RotationW = rotationW,

                            VelocityX = velocityX,
                            VelocityY = velocityY,
                            VelocityZ = velocityZ,

                            Fire = isFire,

                            Bullets = bulletInfos,
                        });
                    }
                }

            }
        }

    }
}
