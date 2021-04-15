using System;
using ETModel;
using libx;
using UnityEngine;

namespace ETHotfix
{
    public static class UIPlayerDieFactory
    {
        public static UI Create()
        {
            try
            {
                AssetRequest assetRequest = Assets.LoadAsset("Assets/Bundles/UI/" + UIType.UIPlayerDie + ".prefab", typeof(GameObject));
                GameObject bundleGameObject = assetRequest.asset as GameObject;
                GameObject gameObject = UnityEngine.Object.Instantiate(bundleGameObject);

                UI ui = ComponentFactory.Create<UI, string, GameObject, AssetRequest>(UIType.UIPlayerDie, gameObject, assetRequest, false);

                ui.AddComponent<UIPlayerDieComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
	}
}