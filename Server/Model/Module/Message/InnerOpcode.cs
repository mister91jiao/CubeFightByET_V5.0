namespace ETModel
{
	public static partial class InnerOpcode
	{
		 public const ushort G2G_LockRequest = 1001;
		 public const ushort G2G_LockResponse = 1002;
		 public const ushort G2G_LockReleaseRequest = 1003;
		 public const ushort G2G_LockReleaseResponse = 1004;
		 public const ushort DBSaveRequest = 1005;
		 public const ushort DBSaveBatchResponse = 1006;
		 public const ushort DBSaveBatchRequest = 1007;
		 public const ushort DBSaveResponse = 1008;
		 public const ushort DBQueryRequest = 1009;
		 public const ushort DBQueryResponse = 1010;
		 public const ushort DBQueryBatchRequest = 1011;
		 public const ushort DBQueryBatchResponse = 1012;
		 public const ushort DBQueryJsonRequest = 1013;
		 public const ushort DBQueryJsonResponse = 1014;
		 public const ushort ObjectAddRequest = 1015;
		 public const ushort ObjectAddResponse = 1016;
		 public const ushort ObjectRemoveRequest = 1017;
		 public const ushort ObjectRemoveResponse = 1018;
		 public const ushort ObjectLockRequest = 1019;
		 public const ushort ObjectLockResponse = 1020;
		 public const ushort ObjectUnLockRequest = 1021;
		 public const ushort ObjectUnLockResponse = 1022;
		 public const ushort ObjectGetRequest = 1023;
		 public const ushort ObjectGetResponse = 1024;
		 public const ushort G2M_SessionDisconnect = 1025;
		 public const ushort G2M_EnterWorld = 1026;
		 public const ushort M2G_EnterWorld = 1027;
		 public const ushort Actor_PlayerInitPositionRequest = 1028;
		 public const ushort Actor_PlayerInitPositionResponse = 1029;
		 public const ushort Actor_PlayerInitPositionUpDate = 1030;
		 public const ushort Actor_PlayerNetSyncToCline = 1031;
		 public const ushort G2M_RemoveUnitByMap = 1032;
		 public const ushort M2G_RemoveUnitByMap = 1033;
		 public const ushort Actor_PlayerToUnitSubHealthRequest = 1034;
		 public const ushort Actor_PlayerToUnitSubHealthResponse = 1035;
		 public const ushort Actor_OtherPlayerDie = 1036;
		 public const ushort Actor_PlayerResurrection = 1037;
		 public const ushort G2M_GetAllMapUnitExcept = 1038;
		 public const ushort M2G_GetAllMapUnitExcept = 1039;
		 public const ushort M2G_RecordKillData = 1040;
	}
}
