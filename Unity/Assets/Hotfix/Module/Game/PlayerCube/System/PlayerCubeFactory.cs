using System;
using ETModel;
using libx;
using UnityEngine;

namespace ETHotfix
{
    public static class PlayerCubeFactory
    {
        public static ETTask<PlayerCube> Create(Vector3 InitPos)
        {
            ETTaskCompletionSource<PlayerCube> tcs = new ETTaskCompletionSource<PlayerCube>();

            CreatePlayerCube(InitPos, tcs).Coroutine();


            return tcs.Task;
        }

        private static async ETVoid CreatePlayerCube(Vector3 InitPos, ETTaskCompletionSource<PlayerCube> tcs)
        {
            try
            {
                //创建一个cube角色的3D物体
                GameObject resObj = await ETModel.Game.Scene.GetComponent<ResourcesAsyncComponent>().LoadAssetAsync<GameObject>(Assets.LoadAssetAsync("Assets/Bundles/Prefab/PlayerCube.prefab", typeof(GameObject)));
                ReferenceCollector rc = resObj.GetComponent<ReferenceCollector>();
                GameObject playerCubeObj = GameObject.Instantiate<GameObject>(rc.Get<GameObject>("PlayerCube"));
                playerCubeObj.transform.position = InitPos;

                //创建cube角色实体脚本
                PlayerCube playerCube = ComponentFactory.Create<PlayerCube, GameObject>(playerCubeObj, false);

                //创建控制UI
                UI ui = await UIJoystickFactory.Create();
                Game.Scene.GetComponent<UIComponent>().Add(ui);

                //添加控制组件
                PlayerCubeControllerComponent playerCubeControllerComponent = playerCube.AddComponent<PlayerCubeControllerComponent, UI>(ui);

                //创建准星控制UI
                UI AttackUI = await UIAttackDirstickFactory.Create();
                Game.Scene.GetComponent<UIComponent>().Add(AttackUI);

                //创建准星3D物品
                TargetArrow targetArrow = await TargetArrowFactory.Create(Vector3.zero, playerCube, AttackUI);

                //管理准星与cube
                playerCubeControllerComponent.targetArrow = targetArrow;

                //添加攻击脚本
                playerCube.AddComponent<PlayerCubeAttackComponent>();

                //添加生命值信息UI
                playerCube.AddComponent<PlayerCubeHealthComponent>();

                //添加攻击伤害同步脚本
                playerCube.AddComponent<HurtSyncComponent>();

                tcs.SetResult(playerCube);
            }
            catch (Exception e)
            {
                Log.Error(e);
                tcs.SetResult(null);
            }
        }
    }
}

