using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_PlayerBackLoginHandler : AMHandler<G2C_PlayerBackLogin>
    {
        protected override async ETTask Run(ETModel.Session session, G2C_PlayerBackLogin message)
        {
            Debug.LogError("断线原因：" + message.NetMessage);

            Application.Quit();

            await ETTask.CompletedTask;
        }
    }
}