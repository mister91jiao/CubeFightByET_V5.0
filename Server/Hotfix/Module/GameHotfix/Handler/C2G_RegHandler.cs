using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_RegHandler : AMRpcHandler<C2G_Reg, G2C_Reg>
    {
        protected override async ETTask Run(Session session, C2G_Reg request, G2C_Reg response, Action reply)
        {
            Log.Info("玩家请求注册：" + request.Account + "玩家输入密码：" + request.Password);

            DBProxyComponent dBProxy = Game.Scene.GetComponent<DBProxyComponent>();

            List<ComponentWithId> count = await dBProxy.Query<PlayerInfoDB>(PlayerInfoDB => PlayerInfoDB.account == request.Account);

            if (count.Count == 0)
            {
                //如果运行到这里说明是注册的新账号
                PlayerInfoDB playerData = ComponentFactory.Create<PlayerInfoDB>();
                playerData.account = request.Account;
                playerData.pwd = request.Password;
                await dBProxy.Save(playerData);

                response.RegFail = true;

                Log.Info("注册新账号成功");
            }
            else
            {
                response.RegFail = false;
                Log.Info("注册新账号失败");

                if (count.Count != 1)
                {
                    Log.Error("账号：" + request.Account + " 数量异常：" + count.Count);
                }
            }

            
            reply();
            await ETTask.CompletedTask;
        }
    }
}
