using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_PlayerResurrectionHandler : AMHandler<G2C_PlayerResurrection>
    {
        protected override async ETTask Run(ETModel.Session session, G2C_PlayerResurrection message)
        {
            //一个玩家复活了
            if (message.ResurrectionPlayerAccount == Game.Scene.GetComponent<PlayerInfoComponent>().account)
            {
                //复活的是自己
                MapHelper.nowPlayerCube.GetComponent<PlayerCubeHealthComponent>()
                    .SetPlayerResurrection(new Vector3(message.PositionX, message.PositionY, message.PositionZ));
            }
            else
            {
                //复活的是其他人
                Game.Scene.GetComponent<OtherCubeManagerComponent>()
                    .OtherCubeResurrection(message.ResurrectionPlayerAccount, new Vector3(message.PositionX, message.PositionY, message.PositionZ));
            }

            await ETTask.CompletedTask;
        }
    }
}