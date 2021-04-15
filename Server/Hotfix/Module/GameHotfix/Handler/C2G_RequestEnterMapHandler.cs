using System;
using System.Collections.Generic;
using System.Net;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_RequestEnterMapHandler : AMRpcHandler<C2G_RequestEnterMap, G2C_RequestEnterMap>
    {
        protected override async ETTask Run(Session session, C2G_RequestEnterMap request, G2C_RequestEnterMap response, Action reply)
        {
            Log.Info("玩家：" + request.Account + "请求进入Map");

            //获取玩家
            PlayerComponent playerComponent = Game.Scene.GetComponent<PlayerComponent>();
            Player player = playerComponent.getPlayerByAccount(request.Account);

            if (player != null)
            {
                //获取内网发送组件
                IPEndPoint mapAddress = StartConfigComponent.Instance.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
                Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapAddress);

                M2G_EnterWorld m2GEnterWorld = (M2G_EnterWorld)await mapSession.Call(new G2M_EnterWorld() { Account = request.Account, PlayerGateInstanceId = player.InstanceId });

                player.MapInstanceId = m2GEnterWorld.PlayerMapInstanceId;

                //获取角色出生位置
                ActorMessageSenderComponent actorSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
                ActorMessageSender actorMessageSender = actorSenderComponent.Get(player.MapInstanceId);

                Actor_PlayerInitPositionResponse actor_PlayerInitPositionResponse = (Actor_PlayerInitPositionResponse)await actorMessageSender.Call(new Actor_PlayerInitPositionRequest());

                Log.Info("获取角色初始位置：" + actor_PlayerInitPositionResponse.PositionX + " || " +
                         actor_PlayerInitPositionResponse.PositionY + " || " + actor_PlayerInitPositionResponse.PositionZ);

                response.PositionX = actor_PlayerInitPositionResponse.PositionX;
                response.PositionY = actor_PlayerInitPositionResponse.PositionY;
                response.PositionZ = actor_PlayerInitPositionResponse.PositionZ;

                reply();

                Log.Info("同意进入Map");

                //向其它玩家发送自己登录的信息
                List<Player> players = playerComponent.getOtherPlayerIgnoreAccount(request.Account);
                if (players != null)
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        players[i].session.Send(new G2C_OtherPlayerEnterMap()
                        {
                            Account = request.Account,
                            PositionX = actor_PlayerInitPositionResponse.PositionX,
                            PositionY = actor_PlayerInitPositionResponse.PositionY,
                            PositionZ = actor_PlayerInitPositionResponse.PositionZ
                        });
                    }
                }
            }
            else
            {
                Log.Info("玩家超时：" + request.Account);
                session.Send(new G2C_PlayerBackLogin()
                {
                    NetMessage = "游戏超时: " + request.Account
                });
            }

            await ETTask.CompletedTask;
        }
    }
}
