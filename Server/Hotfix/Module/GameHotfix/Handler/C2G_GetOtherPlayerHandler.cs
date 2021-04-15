using System.Collections.Generic;
using System.Net;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_GetOtherPlayerHandler : AMHandler<C2G_GetOtherPlayer>
    {
        protected override async ETTask Run(Session session, C2G_GetOtherPlayer message)
        {
            //获取内网发送组件
            IPEndPoint mapAddress = StartConfigComponent.Instance.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
            Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapAddress);

            M2G_GetAllMapUnitExcept m2GGetAllMapUnitExcept = (M2G_GetAllMapUnitExcept)await mapSession.Call(new G2M_GetAllMapUnitExcept() { Account = message.Account });

            if (m2GGetAllMapUnitExcept.Accounts.Count > 0)
            {
                PlayerComponent playerComponent = Game.Scene.GetComponent<PlayerComponent>();
                ActorMessageSenderComponent actorSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();

                for (int i = 0; i < m2GGetAllMapUnitExcept.Accounts.Count; i++)
                {
                    if (playerComponent.AccountHaveBeCreated(m2GGetAllMapUnitExcept.Accounts[i]))
                    {
                        Player player = playerComponent.getPlayerByAccount(m2GGetAllMapUnitExcept.Accounts[i]);

                        ActorMessageSender actorMessageSender = actorSenderComponent.Get(player.MapInstanceId);
                        Actor_PlayerInitPositionResponse actor_PlayerInitPositionResponse = (Actor_PlayerInitPositionResponse)await actorMessageSender.Call(new Actor_PlayerInitPositionRequest());
                        session.Send(new G2C_OtherPlayerEnterMap()
                        {
                            Account = player.Account,
                            PositionX = actor_PlayerInitPositionResponse.PositionX,
                            PositionY = actor_PlayerInitPositionResponse.PositionY,
                            PositionZ = actor_PlayerInitPositionResponse.PositionZ
                        });
                    }
                }
            }

            await ETTask.CompletedTask;
        }
    }
}
