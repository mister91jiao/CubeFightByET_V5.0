using ETModel;
namespace ETModel
{
	[Message(OuterOpcode.C2R_Ping)]
	public partial class C2R_Ping : IRequest {}

	[Message(OuterOpcode.R2C_Ping)]
	public partial class R2C_Ping : IResponse {}

	[Message(OuterOpcode.G2C_PlayerPlaying)]
	public partial class G2C_PlayerPlaying : IMessage {}

}
namespace ETModel
{
	public static partial class OuterOpcode
	{
		 public const ushort C2R_Ping = 101;
		 public const ushort R2C_Ping = 102;
		 public const ushort G2C_PlayerPlaying = 103;
	}
}
