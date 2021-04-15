using ETModel;
using System;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class Actor_PlayerInitPositionUpDateHandler : AMActorHandler<Unit, Actor_PlayerInitPositionUpDate>
    {
        private UnitComponent unitComponent = null;

        protected override async ETTask Run(Unit entity, Actor_PlayerInitPositionUpDate message)
        {
            entity.InitPositionX = message.PositionX;
            entity.InitPositionY = message.PositionY;
            entity.InitPositionZ = message.PositionZ;

            entity.RotationX = message.RotationX;
            entity.RotationY = message.RotationY;
            entity.RotationZ = message.RotationZ;
            entity.RotationW = message.RotationW;

            entity.VelocityX = message.VelocityX;
            entity.VelocityY = message.VelocityY;
            entity.VelocityZ = message.VelocityZ;

            entity.Fire = message.Fire;
            //Log.Info("单位[" + entity.Id + "]位置更新：" + message.RotationX + " | " + message.RotationY + " | " + message.RotationZ + " | " + message.RotationW);

            if (unitComponent == null)
            {
                unitComponent = Game.Scene.GetComponent<UnitComponent>();
            }

            if (message.Bullets.Count > 0)
            {
                BulletManagerComponent bulletManagerComponent = Game.Scene.GetComponent<BulletManagerComponent>();
                for (int i = 0; i < message.Bullets.Count; i++)
                {

                    Bullet bullet = ComponentFactory.Create<Bullet, int>(message.Bullets[i].Account);

                    bullet.PositionX = message.Bullets[i].PositionX;
                    bullet.PositionY = message.Bullets[i].PositionY;
                    bullet.PositionZ = message.Bullets[i].PositionZ;

                    bullet.RotationX = message.Bullets[i].RotationX;
                    bullet.RotationY = message.Bullets[i].RotationY;
                    bullet.RotationZ = message.Bullets[i].RotationZ;
                    bullet.RotationW = message.Bullets[i].RotationW;

                    bullet.VelocityX = message.Bullets[i].VelocityX;
                    bullet.VelocityY = message.Bullets[i].VelocityY;
                    bullet.VelocityZ = message.Bullets[i].VelocityZ;

                    bulletManagerComponent.AddBulletToQueue(bullet);
                }
            }

            await ETTask.CompletedTask;
        }
    }
}
