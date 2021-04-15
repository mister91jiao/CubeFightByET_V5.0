using System;
using System.Net;

namespace ETModel
{
    [MessageHandler(AppType.Map)]
    public class G2M_EnterWorldHandler : AMRpcHandler<G2M_EnterWorld, M2G_EnterWorld>
    {
        protected override async ETTask Run(Session session, G2M_EnterWorld request, M2G_EnterWorld response, Action reply)
        {
            Log.Info("Map服收到玩家请求进入的信息: " + request.Account);

            UnitComponent unitComponent = Game.Scene.GetComponent<UnitComponent>();
            Unit enterUnit;
            //查看玩家是否已经登录创建过
            if (unitComponent.UnitHaveBeCreated(request.Account))
            {
                enterUnit = unitComponent.getUnitByAccount(request.Account);
            }
            else
            {
                //添加一个单位
                enterUnit = UnitFactory.Create(request.Account);
                unitComponent.addUnitToDict(request.Account, enterUnit);

                enterUnit.InitPositionX = 0.0f;
                enterUnit.InitPositionY = 0.0f;
                enterUnit.InitPositionZ = 0.0f;
            }

            //Unit EmmpyUnit = UnitFactory.Create("EMMPY");
            //unitComponent.addUnitToDict("EMMPY", EmmpyUnit);
            //EmmpyUnit.InitPositionX = 20;
            //EmmpyUnit.InitPositionY = 200;
            //EmmpyUnit.InitPositionZ = 2000;

            //指定InstanceId
            enterUnit.GateInstanceId = request.PlayerGateInstanceId;

            response.Account = request.Account;

            response.PlayerMapInstanceId = enterUnit.InstanceId;

            reply();
            Log.Info("Map服创建实例完成，同意进入");

            await ETTask.CompletedTask;
        }
    }
}
