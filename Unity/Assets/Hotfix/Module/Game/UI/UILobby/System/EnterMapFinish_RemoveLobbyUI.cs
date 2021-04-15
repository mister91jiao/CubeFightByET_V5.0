using ETModel;
using libx;

namespace ETHotfix
{
	[Event(EventIdType.EnterMapFinish)]
	public class EnterMapFinish_RemoveLobbyUI: AEvent
	{
		public override void Run()
		{
            Assets.UnloadAsset(Game.Scene.GetComponent<UIComponent>().Get(UIType.UILobby).assetRequest);
			Game.Scene.GetComponent<UIComponent>().Remove(UIType.UILobby);
		}
	}
}
