using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_OtherPlayerDieHandler : AMHandler<G2C_OtherPlayerDie>
    {
        protected override async ETTask Run(ETModel.Session session, G2C_OtherPlayerDie message)
        {
            //使一个cube死亡
            Game.Scene.GetComponent<OtherCubeManagerComponent>().OtherCubeDie(message.DiePlayerAccount);

            await ETTask.CompletedTask;
        }
    }
}