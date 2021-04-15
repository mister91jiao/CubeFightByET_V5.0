using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_OtherPlayerPositionHandler : AMHandler<G2C_OtherPlayerPosition>
    {
        private OtherCubeManagerComponent otherCubeManagerComponent = null;

        //本地时间标记
        private long serverTime;

        protected override async ETTask Run(ETModel.Session session, G2C_OtherPlayerPosition message)
        {
            if (otherCubeManagerComponent == null)
            {
                otherCubeManagerComponent = Game.Scene.GetComponent<OtherCubeManagerComponent>();
            }

            if (message.ServerTime > serverTime)
            {
                serverTime = message.ServerTime;

                int[] DirAccount = message.DirAccount.array;
                float[] PositionX = message.PositionX.array;
                float[] PositionY = message.PositionY.array;
                float[] PositionZ = message.PositionZ.array;

                float[] RotationX = message.RotationX.array;
                float[] RotationY = message.RotationY.array;
                float[] RotationZ = message.RotationZ.array;
                float[] RotationW = message.RotationW.array;

                float[] VelocityX = message.VelocityX.array;
                float[] VelocityY = message.VelocityY.array;
                float[] VelocityZ = message.VelocityZ.array;

                bool[] Fire = message.Fire.array;

                for (int i = 0; i < DirAccount.Length; i++)
                {
                    OtherCubeNetSyncComponent otherCubeNetSyncComponent = otherCubeManagerComponent.GetNetSyncComponentByOtherCubeAccount(DirAccount[i]);
                    if (otherCubeNetSyncComponent != null)
                    {
                        otherCubeNetSyncComponent.NetWorkAsyncPosition(new Vector3(PositionX[i], PositionY[i], PositionZ[i]), new Quaternion(RotationX[i], RotationY[i], RotationZ[i], RotationW[i]), new Vector3(VelocityX[i], VelocityY[i], VelocityZ[i]));
                        //Log.Info("同步一次位置：" + DirAccount[i]);

                        otherCubeNetSyncComponent.NetWorkAsyncFire(Fire[i]);
                    }
                }

                PlayerInfoComponent playerInfoComponent = Game.Scene.GetComponent<PlayerInfoComponent>();


                //if (message.Bullets.array.Length != 0)
                //{
                //    Debug.LogError("子弹数量：" + message.Bullets.count);
                //}

                //同步子弹数量
                int count = message.Bullets.count;
                for (int i = 0; i < count; i++)
                {
                    //不是自己的子弹才需要创建同步
                    if (message.Bullets[i].Account != playerInfoComponent.account)
                    {
                        CubeBullet cubeBullet = CubeBulletFactory.CreateCubeBullet();
                        cubeBullet.SyncBullet(new Vector3(message.Bullets[i].PositionX, message.Bullets[i].PositionY, message.Bullets[i].PositionZ),
                            new Quaternion(message.Bullets[i].RotationX, message.Bullets[i].RotationY, message.Bullets[i].RotationZ, message.Bullets[i].RotationW),
                            new Vector3(message.Bullets[i].VelocityX, message.Bullets[i].VelocityY, message.Bullets[i].VelocityZ));

                        //Debug.LogError("创建一颗子弹");
                    }
                }

            }
            else
            {
                Debug.LogError("丢包了: " + message.ServerTime + " || " + serverTime);
            }


            await ETTask.CompletedTask;
        }
    }
}
