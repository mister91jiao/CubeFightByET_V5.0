using ETModel;
using System.Collections.Generic;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_PlayerRoleNetworkHandler : AMHandler<C2G_PlayerRoleNetwork>
    {
        protected override async ETTask Run(Session session, C2G_PlayerRoleNetwork message)
        {
            //Log.Info("玩家[" + message.Account + "]传来坐标信息：" + message.RotationX + " | " + message.RotationY + " | " + message.RotationZ + " | " + message.RotationW);

            //获取玩家
            Player player = Game.Scene.GetComponent<PlayerComponent>().getPlayerByAccount(message.Account);
            if (player != null)
            {
                //Actor发送组件
                ActorMessageSenderComponent actorSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
                ActorMessageSender actorMessageSender = actorSenderComponent.Get(player.MapInstanceId);

                List<BulletInfo> bulletInfos = new List<BulletInfo>();
                for (int i = 0; i < message.Bullets.Count; i++)
                {
                    BulletInfo bulletInfo = new BulletInfo();
                    bulletInfo.Account = message.Bullets[i].Account;

                    bulletInfo.PositionX = message.Bullets[i].PositionX;
                    bulletInfo.PositionY = message.Bullets[i].PositionY;
                    bulletInfo.PositionZ = message.Bullets[i].PositionZ;

                    bulletInfo.RotationX = message.Bullets[i].RotationX;
                    bulletInfo.RotationY = message.Bullets[i].RotationY;
                    bulletInfo.RotationZ = message.Bullets[i].RotationZ;
                    bulletInfo.RotationW = message.Bullets[i].RotationW;

                    bulletInfo.VelocityX = message.Bullets[i].VelocityX;
                    bulletInfo.VelocityY = message.Bullets[i].VelocityY;
                    bulletInfo.VelocityZ = message.Bullets[i].VelocityZ;

                    bulletInfos.Add(bulletInfo);
                }

                actorMessageSender.Send(new Actor_PlayerInitPositionUpDate()
                {
                    PositionX = message.PositionX,
                    PositionY = message.PositionY,
                    PositionZ = message.PositionZ,
                    RotationX = message.RotationX,
                    RotationY = message.RotationY,
                    RotationZ = message.RotationZ,
                    RotationW = message.RotationW,
                    VelocityX = message.VelocityX,
                    VelocityY = message.VelocityY,
                    VelocityZ = message.VelocityZ,
                    Fire = message.Fire,
                    Bullets = bulletInfos,
                });
            }

            await ETTask.CompletedTask;
        }
    }
}
