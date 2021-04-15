using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_OtherPlayerEnterMapHandler : AMHandler<G2C_OtherPlayerEnterMap>
    {
        protected override async ETTask Run(ETModel.Session session, G2C_OtherPlayerEnterMap message)
        {
            Log.Info("其它玩家进入Map: " + message.Account);
            OtherCubeNetSyncComponent NetSyncComponent = Game.Scene.GetComponent<OtherCubeManagerComponent>()
                .GetNetSyncComponentByOtherCubeAccount(message.Account);

            if (NetSyncComponent != null)
            {
                NetSyncComponent.NetWorkAsyncPosition(new Vector3(message.PositionX, message.PositionY, message.PositionZ), Quaternion.identity, Vector3.zero);
            }
            else
            {
                OtherCube otherCube = await OtherCubeFactory.Create(message.Account,
                    new Vector3(message.PositionX, message.PositionY, message.PositionZ));
            }

            await ETTask.CompletedTask;
        }
    }
}