using ETModel;
using ETHotfix;
using System.Collections.Generic;
namespace ETModel
{
	[Message(InnerOpcode.G2G_LockRequest)]
	public partial class G2G_LockRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Id { get; set; }

		public string Address { get; set; }

	}

	[Message(InnerOpcode.G2G_LockResponse)]
	public partial class G2G_LockResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.G2G_LockReleaseRequest)]
	public partial class G2G_LockReleaseRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Id { get; set; }

		public string Address { get; set; }

	}

	[Message(InnerOpcode.G2G_LockReleaseResponse)]
	public partial class G2G_LockReleaseResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.DBSaveRequest)]
	public partial class DBSaveRequest: IRequest
	{
		public int RpcId { get; set; }

		public string CollectionName { get; set; }

		public ComponentWithId Component { get; set; }

	}

	[Message(InnerOpcode.DBSaveBatchResponse)]
	public partial class DBSaveBatchResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.DBSaveBatchRequest)]
	public partial class DBSaveBatchRequest: IRequest
	{
		public int RpcId { get; set; }

		public string CollectionName { get; set; }

		public List<ComponentWithId> Components = new List<ComponentWithId>();

	}

	[Message(InnerOpcode.DBSaveResponse)]
	public partial class DBSaveResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.DBQueryRequest)]
	public partial class DBQueryRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Id { get; set; }

		public string CollectionName { get; set; }

	}

	[Message(InnerOpcode.DBQueryResponse)]
	public partial class DBQueryResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public ComponentWithId Component { get; set; }

	}

	[Message(InnerOpcode.DBQueryBatchRequest)]
	public partial class DBQueryBatchRequest: IRequest
	{
		public int RpcId { get; set; }

		public string CollectionName { get; set; }

		public List<long> IdList = new List<long>();

	}

	[Message(InnerOpcode.DBQueryBatchResponse)]
	public partial class DBQueryBatchResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public List<ComponentWithId> Components = new List<ComponentWithId>();

	}

	[Message(InnerOpcode.DBQueryJsonRequest)]
	public partial class DBQueryJsonRequest: IRequest
	{
		public int RpcId { get; set; }

		public string CollectionName { get; set; }

		public string Json { get; set; }

	}

	[Message(InnerOpcode.DBQueryJsonResponse)]
	public partial class DBQueryJsonResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public List<ComponentWithId> Components = new List<ComponentWithId>();

	}

	[Message(InnerOpcode.ObjectAddRequest)]
	public partial class ObjectAddRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Key { get; set; }

		public long InstanceId { get; set; }

	}

	[Message(InnerOpcode.ObjectAddResponse)]
	public partial class ObjectAddResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.ObjectRemoveRequest)]
	public partial class ObjectRemoveRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Key { get; set; }

	}

	[Message(InnerOpcode.ObjectRemoveResponse)]
	public partial class ObjectRemoveResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.ObjectLockRequest)]
	public partial class ObjectLockRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Key { get; set; }

		public long InstanceId { get; set; }

		public int Time { get; set; }

	}

	[Message(InnerOpcode.ObjectLockResponse)]
	public partial class ObjectLockResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.ObjectUnLockRequest)]
	public partial class ObjectUnLockRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Key { get; set; }

		public long OldInstanceId { get; set; }

		public long InstanceId { get; set; }

	}

	[Message(InnerOpcode.ObjectUnLockResponse)]
	public partial class ObjectUnLockResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

	}

	[Message(InnerOpcode.ObjectGetRequest)]
	public partial class ObjectGetRequest: IRequest
	{
		public int RpcId { get; set; }

		public long Key { get; set; }

	}

	[Message(InnerOpcode.ObjectGetResponse)]
	public partial class ObjectGetResponse: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public long InstanceId { get; set; }

	}

	[Message(InnerOpcode.G2M_SessionDisconnect)]
	public partial class G2M_SessionDisconnect: IActorLocationMessage
	{
		public int RpcId { get; set; }

		public long ActorId { get; set; }

	}

	[Message(InnerOpcode.G2M_EnterWorld)]
	public partial class G2M_EnterWorld: IRequest
	{
		public int RpcId { get; set; }

		public long ActorId { get; set; }

		public int Account { get; set; }

		public long PlayerGateInstanceId { get; set; }

	}

	[Message(InnerOpcode.M2G_EnterWorld)]
	public partial class M2G_EnterWorld: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public int Account { get; set; }

		public long PlayerMapInstanceId { get; set; }

	}

	[Message(InnerOpcode.Actor_PlayerInitPositionRequest)]
	public partial class Actor_PlayerInitPositionRequest: IActorRequest
	{
		public int RpcId { get; set; }

		public long ActorId { get; set; }

	}

	[Message(InnerOpcode.Actor_PlayerInitPositionResponse)]
	public partial class Actor_PlayerInitPositionResponse: IActorResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public float PositionX { get; set; }

		public float PositionY { get; set; }

		public float PositionZ { get; set; }

	}

	[Message(InnerOpcode.Actor_PlayerInitPositionUpDate)]
	public partial class Actor_PlayerInitPositionUpDate: IActorMessage
	{
		public long ActorId { get; set; }

		public float PositionX { get; set; }

		public float PositionY { get; set; }

		public float PositionZ { get; set; }

		public float RotationX { get; set; }

		public float RotationY { get; set; }

		public float RotationZ { get; set; }

		public float RotationW { get; set; }

		public float VelocityX { get; set; }

		public float VelocityY { get; set; }

		public float VelocityZ { get; set; }

		public bool Fire { get; set; }

		public List<BulletInfo> Bullets = new List<BulletInfo>();

	}

	[Message(InnerOpcode.Actor_PlayerNetSyncToCline)]
	public partial class Actor_PlayerNetSyncToCline: IActorMessage
	{
		public long ActorId { get; set; }

		public List<int> DirAccount = new List<int>();

		public List<float> PositionX = new List<float>();

		public List<float> PositionY = new List<float>();

		public List<float> PositionZ = new List<float>();

		public List<float> RotationX = new List<float>();

		public List<float> RotationY = new List<float>();

		public List<float> RotationZ = new List<float>();

		public List<float> RotationW = new List<float>();

		public List<float> VelocityX = new List<float>();

		public List<float> VelocityY = new List<float>();

		public List<float> VelocityZ = new List<float>();

		public List<bool> Fire = new List<bool>();

		public List<BulletInfo> Bullets = new List<BulletInfo>();

	}

	[Message(InnerOpcode.G2M_RemoveUnitByMap)]
	public partial class G2M_RemoveUnitByMap: IRequest
	{
		public int RpcId { get; set; }

		public int Account { get; set; }

	}

	[Message(InnerOpcode.M2G_RemoveUnitByMap)]
	public partial class M2G_RemoveUnitByMap: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public List<int> Accounts = new List<int>();

	}

	[Message(InnerOpcode.Actor_PlayerToUnitSubHealthRequest)]
	public partial class Actor_PlayerToUnitSubHealthRequest: IActorRequest
	{
		public int RpcId { get; set; }

		public long ActorId { get; set; }

		public int SubHealth { get; set; }

		public int KillerAccount { get; set; }

	}

	[Message(InnerOpcode.Actor_PlayerToUnitSubHealthResponse)]
	public partial class Actor_PlayerToUnitSubHealthResponse: IActorResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public int UnitHealth { get; set; }

		public bool Die { get; set; }

		public bool AttackDiePlayer { get; set; }

	}

	[Message(InnerOpcode.Actor_OtherPlayerDie)]
	public partial class Actor_OtherPlayerDie: IActorMessage
	{
		public long ActorId { get; set; }

		public int DiePlayerAccount { get; set; }

	}

	[Message(InnerOpcode.Actor_PlayerResurrection)]
	public partial class Actor_PlayerResurrection: IActorMessage
	{
		public long ActorId { get; set; }

		public int ResurrectionPlayerAccount { get; set; }

		public float PositionX { get; set; }

		public float PositionY { get; set; }

		public float PositionZ { get; set; }

	}

	[Message(InnerOpcode.G2M_GetAllMapUnitExcept)]
	public partial class G2M_GetAllMapUnitExcept: IRequest
	{
		public int RpcId { get; set; }

		public long ActorId { get; set; }

		public int Account { get; set; }

	}

	[Message(InnerOpcode.M2G_GetAllMapUnitExcept)]
	public partial class M2G_GetAllMapUnitExcept: IResponse
	{
		public int RpcId { get; set; }

		public int Error { get; set; }

		public string Message { get; set; }

		public List<int> Accounts = new List<int>();

	}

	[Message(InnerOpcode.M2G_RecordKillData)]
	public partial class M2G_RecordKillData: IMessage
	{
		public int RpcId { get; set; }

		public int KillerAccount { get; set; }

		public int DeathAccount { get; set; }

	}

}
