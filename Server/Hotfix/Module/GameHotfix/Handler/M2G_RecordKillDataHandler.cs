using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class M2G_RecordKillDataHandler : AMHandler<M2G_RecordKillData>
    {

        protected override async ETTask Run(Session session, M2G_RecordKillData message)
        {
            DBProxyComponent dBProxy = Game.Scene.GetComponent<DBProxyComponent>();

            {
                List<ComponentWithId> count = await dBProxy.Query<PlayerInfoDB>(PlayerInfoDB => PlayerInfoDB.account == message.KillerAccount);
                if (count.Count == 1)
                {
                    PlayerInfoDB playerInfoDB = count[0] as PlayerInfoDB;

                    Log.Info("查询击杀者：" + message.KillerAccount);

                    playerInfoDB.KillCount++;
                    playerInfoDB.MoneyCount += 20;

                    await dBProxy.Save(playerInfoDB);
                }
                else
                {
                    Log.Error("查找数据不正确：" + message.KillerAccount);
                    

                }
            }
            {
                List<ComponentWithId> count = await dBProxy.Query<PlayerInfoDB>(PlayerInfoDB => PlayerInfoDB.account == message.DeathAccount);
                if (count.Count == 1)
                {
                    PlayerInfoDB playerInfoDB = count[0] as PlayerInfoDB;

                    Log.Info("查询被杀者：" + message.DeathAccount);

                    playerInfoDB.DeathCount++;

                    await dBProxy.Save(playerInfoDB);
                }
                else
                {
                    Log.Error("查找数据不正确：" + message.KillerAccount);
                }
            }

            await ETTask.CompletedTask;
        }
    }
}
