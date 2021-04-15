using System;
using System.Collections.Generic;
using System.Net;
using ETHotfix;

namespace ETModel
{
    [MessageHandler(AppType.Map)]
    public class G2M_GetAllMapUnitHandler : AMRpcHandler<G2M_GetAllMapUnitExcept, M2G_GetAllMapUnitExcept>
    {
        protected override async ETTask Run(Session session, G2M_GetAllMapUnitExcept request, M2G_GetAllMapUnitExcept response, Action reply)
        {
            UnitComponent unitComponent = Game.Scene.GetComponent<UnitComponent>();

            List<int> allExceptUnit = new List<int>();

            List<Unit> units = unitComponent.getCountUnits(0);
            if (units != null)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].Account != request.Account)
                    {
                        allExceptUnit.Add(units[i].Account);
                    }
                }
            }

            response.Accounts = allExceptUnit;

            reply();

            await ETTask.CompletedTask;
        }

    }
}
