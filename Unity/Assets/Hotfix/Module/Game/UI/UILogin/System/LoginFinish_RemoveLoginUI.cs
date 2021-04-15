using ETModel;
using libx;

namespace ETHotfix
{
	[Event(EventIdType.LoginFinish)]
	public class LoginFinish_RemoveLoginUI: AEvent
	{
		public override void Run()
        {
            Assets.UnloadAsset(Game.Scene.GetComponent<UIComponent>().Get(UIType.UILogin).assetRequest);
			Game.Scene.GetComponent<UIComponent>().Remove(UIType.UILogin);
		}
	}
}
