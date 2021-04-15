using ETModel;
namespace ETHotfix
{
	[Message(HotfixOpcode.C2G_Login)]
	public partial class C2G_Login : IRequest {}

	[Message(HotfixOpcode.G2C_Login)]
	public partial class G2C_Login : IResponse {}

	[Message(HotfixOpcode.C2G_Reg)]
	public partial class C2G_Reg : IRequest {}

	[Message(HotfixOpcode.G2C_Reg)]
	public partial class G2C_Reg : IResponse {}

	[Message(HotfixOpcode.G2C_PlayerBackLogin)]
	public partial class G2C_PlayerBackLogin : IMessage {}

	[Message(HotfixOpcode.C2G_GetPlayerInfo)]
	public partial class C2G_GetPlayerInfo : IRequest {}

	[Message(HotfixOpcode.G2C_GetPlayerInfo)]
	public partial class G2C_GetPlayerInfo : IResponse {}

	[Message(HotfixOpcode.C2G_RequestEnterMap)]
	public partial class C2G_RequestEnterMap : IRequest {}

	[Message(HotfixOpcode.G2C_RequestEnterMap)]
	public partial class G2C_RequestEnterMap : IResponse {}

	[Message(HotfixOpcode.C2G_PlayerRoleNetwork)]
	public partial class C2G_PlayerRoleNetwork : IMessage {}

	[Message(HotfixOpcode.C2G_GetOtherPlayer)]
	public partial class C2G_GetOtherPlayer : IMessage {}

	[Message(HotfixOpcode.G2C_OtherPlayerEnterMap)]
	public partial class G2C_OtherPlayerEnterMap : IMessage {}

	[Message(HotfixOpcode.G2C_OtherPlayerPosition)]
	public partial class G2C_OtherPlayerPosition : IMessage {}

	[Message(HotfixOpcode.BulletInfo)]
	public partial class BulletInfo : IMessage {}

	[Message(HotfixOpcode.G2C_PlayerDisCatenate)]
	public partial class G2C_PlayerDisCatenate : IMessage {}

	[Message(HotfixOpcode.C2G_PlayerHitOtherPlayer)]
	public partial class C2G_PlayerHitOtherPlayer : IMessage {}

	[Message(HotfixOpcode.G2C_PlayerHealthUpuate)]
	public partial class G2C_PlayerHealthUpuate : IMessage {}

	[Message(HotfixOpcode.G2C_OtherPlayerDie)]
	public partial class G2C_OtherPlayerDie : IMessage {}

	[Message(HotfixOpcode.G2C_PlayerResurrection)]
	public partial class G2C_PlayerResurrection : IMessage {}

}
namespace ETHotfix
{
	public static partial class HotfixOpcode
	{
		 public const ushort C2G_Login = 10001;
		 public const ushort G2C_Login = 10002;
		 public const ushort C2G_Reg = 10003;
		 public const ushort G2C_Reg = 10004;
		 public const ushort G2C_PlayerBackLogin = 10005;
		 public const ushort C2G_GetPlayerInfo = 10006;
		 public const ushort G2C_GetPlayerInfo = 10007;
		 public const ushort C2G_RequestEnterMap = 10008;
		 public const ushort G2C_RequestEnterMap = 10009;
		 public const ushort C2G_PlayerRoleNetwork = 10010;
		 public const ushort C2G_GetOtherPlayer = 10011;
		 public const ushort G2C_OtherPlayerEnterMap = 10012;
		 public const ushort G2C_OtherPlayerPosition = 10013;
		 public const ushort BulletInfo = 10014;
		 public const ushort G2C_PlayerDisCatenate = 10015;
		 public const ushort C2G_PlayerHitOtherPlayer = 10016;
		 public const ushort G2C_PlayerHealthUpuate = 10017;
		 public const ushort G2C_OtherPlayerDie = 10018;
		 public const ushort G2C_PlayerResurrection = 10019;
	}
}
