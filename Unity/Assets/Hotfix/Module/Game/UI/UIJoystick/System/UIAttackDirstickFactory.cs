using System;
using ETModel;
using libx;
using UnityEngine;

namespace ETHotfix
{
    public static class UIAttackDirstickFactory
    {
        public static ETTask<UI> Create()
        {
            ETTaskCompletionSource<UI> tcs = new ETTaskCompletionSource<UI>();

            CreateJoystickUI(tcs).Coroutine();


            return tcs.Task;
        }

        public static async ETVoid CreateJoystickUI(ETTaskCompletionSource<UI> tcs)
        {
            try
            {
                AssetRequest assetRequest = Assets.LoadAssetAsync("Assets/Bundles/UI/" + UIType.UIAttackDirstick + ".prefab", typeof(GameObject));

                //创建一个角色的3D物体
                GameObject bundleGameObject = await ETModel.Game.Scene.GetComponent<ResourcesAsyncComponent>().LoadAssetAsync<GameObject>(assetRequest);
                GameObject gameObject = UnityEngine.Object.Instantiate(bundleGameObject);

                UI ui = ComponentFactory.Create<UI, string, GameObject, AssetRequest>(UIType.UIAttackDirstick, gameObject, assetRequest, false);

                VariableJoystickComponent variableJoystick = ui.AddComponent<VariableJoystickComponent>();
                variableJoystick.joystickType = JoystickType.Floating;

                tcs.SetResult(ui);
            }
            catch (Exception e)
            {
                Log.Error(e);
                tcs.SetResult(null);
            }
        }


    }
}

