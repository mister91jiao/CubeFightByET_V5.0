using ETModel;
using System;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class Actor_PlayerInitPositionHandler : AMActorRpcHandler<Unit, Actor_PlayerInitPositionRequest, Actor_PlayerInitPositionResponse>
    {
        protected override async ETTask Run(Unit unit, Actor_PlayerInitPositionRequest request, Actor_PlayerInitPositionResponse response, Action reply)
        {
            Log.Info("收到请求初始位置的信息");

            response.PositionX = unit.InitPositionX;
            response.PositionY = unit.InitPositionY;
            response.PositionZ = unit.InitPositionZ;

            reply();
            await ETTask.CompletedTask;
        }
    }

}
