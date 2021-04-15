using System;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.DB)]
	public class DBSaveRequestHandler : AMRpcHandler<DBSaveRequest, DBSaveResponse>
	{
		protected override async ETTask Run(Session session, DBSaveRequest request, DBSaveResponse response, Action reply)
        {
            Log.Info("收到请求保存的信息");

			DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
			if (string.IsNullOrEmpty(request.CollectionName))
			{
				request.CollectionName = request.Component.GetType().Name;
			}

			await dbComponent.Add(request.Component, request.CollectionName);
			reply();
		}
	}
}