using System;
using System.Collections.Generic;
using System.Net;
using ETHotfix;

namespace ETModel
{
    [MessageHandler(AppType.Gate)]
    public class C2G_PlayerHitOtherPlayerHandler : AMHandler<C2G_PlayerHitOtherPlayer>
    {
        protected override async ETTask Run(Session session, C2G_PlayerHitOtherPlayer message)
        {
            //获取玩家
            Player player = Game.Scene.GetComponent<PlayerComponent>().getPlayerByAccount(message.HitPlayerAccount);
            //Actor发送组件
            ActorMessageSenderComponent actorSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
            ActorMessageSender actorMessageSender = actorSenderComponent.Get(player.MapInstanceId);

            Actor_PlayerToUnitSubHealthResponse actor_PlayerToUnitSubHealthResponse = (Actor_PlayerToUnitSubHealthResponse)await actorMessageSender.Call(new Actor_PlayerToUnitSubHealthRequest()
            {
                SubHealth = message.SubHealth,
                KillerAccount = message.SelfAccount
            });

            //只有攻击了没有死亡的角色才会发送信息
            if (!actor_PlayerToUnitSubHealthResponse.AttackDiePlayer)
            {
                //给对应角色发扣血的信息
                player.session.Send(new G2C_PlayerHealthUpuate()
                {
                    NewHealth = actor_PlayerToUnitSubHealthResponse.UnitHealth,
                    Die = actor_PlayerToUnitSubHealthResponse.Die
                });
            }
            

            await ETTask.CompletedTask;
        }

    }
}
