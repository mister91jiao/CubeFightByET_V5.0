using System;
using System.Collections.Generic;
using System.Net;
using ETHotfix;

namespace ETModel
{
    [MessageHandler(AppType.Map)]
    public class G2M_RemoveUnitByMapHandler : AMRpcHandler<G2M_RemoveUnitByMap, M2G_RemoveUnitByMap>
    {
        protected override async ETTask Run(Session session, G2M_RemoveUnitByMap request, M2G_RemoveUnitByMap response, Action reply)
        {
            //需要从map服中一处一个Unit
            UnitComponent unitComponent = Game.Scene.GetComponent<UnitComponent>();

            //返回所有需要广播掉线消息的Player
            List<int> needSendPlayer = new List<int>();

            if (unitComponent.UnitHaveBeCreated(request.Account))
            {
                unitComponent.removeOneUnit(request.Account);


                List<Unit> units = unitComponent.getCountUnits(0);
                if (units != null)
                {
                    for (int i = 0; i < units.Count; i++)
                    {
                        needSendPlayer.Add(units[i].Account);
                    }
                }
            }

            response.Accounts = needSendPlayer;

            reply();

            await ETTask.CompletedTask;
        }

    }
}
