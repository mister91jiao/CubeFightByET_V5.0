using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_GetPlayerInfoHandler : AMRpcHandler<C2G_GetPlayerInfo, G2C_GetPlayerInfo>
    {
        protected override async ETTask Run(Session session, C2G_GetPlayerInfo request, G2C_GetPlayerInfo response, Action reply)
        {
            DBProxyComponent dBProxy = Game.Scene.GetComponent<DBProxyComponent>();

            List<ComponentWithId> count = await dBProxy.Query<PlayerInfoDB>(PlayerInfoDB => PlayerInfoDB.account == request.Account);
            if (count.Count == 1)
            {
                PlayerInfoDB playerInfoDB = count[0] as PlayerInfoDB;

                Log.Info("查询玩家：" + request.Account);

                response.KillCount = playerInfoDB.KillCount;
                response.DeathCount = playerInfoDB.DeathCount;
                response.MoneyCount = playerInfoDB.MoneyCount;
            }
            else
            {
                Log.Error("查找数据不正确：" + request.Account);

                response.KillCount = 0;
                response.DeathCount = 0;
                response.MoneyCount = 0;
            }

            reply();
            await ETTask.CompletedTask;
        }
    }
}
