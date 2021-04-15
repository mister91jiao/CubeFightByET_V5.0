using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_PlayerDisCatenateHandler : AMHandler<G2C_PlayerDisCatenate>
    {
        protected override async ETTask Run(ETModel.Session session, G2C_PlayerDisCatenate message)
        {
            //移除已经掉线的玩家
            Game.Scene.GetComponent<OtherCubeManagerComponent>().RemoveOneOtherCube(message.Account);

            await ETTask.CompletedTask;
        }
    }
}