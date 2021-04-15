using System;
using ETModel;
using libx;
using UnityEngine;

namespace ETHotfix
{
    public static class TargetArrowFactory
    {
        public static ETTask<TargetArrow> Create(Vector3 InitPos, PlayerCube playerCube, UI AttackUI)
        {
            ETTaskCompletionSource<TargetArrow> tcs = new ETTaskCompletionSource<TargetArrow>();

            CreateTargetArrow(InitPos, playerCube, AttackUI.GetComponent<VariableJoystickComponent>(), tcs).Coroutine();

            return tcs.Task;
        }

        private static async ETVoid CreateTargetArrow(Vector3 InitPos, PlayerCube playerCube, VariableJoystickComponent AttackUI, ETTaskCompletionSource<TargetArrow> tcs)
        {
            try
            {
                //创建一个TargetArrow的3D物体
                GameObject resObj = await ETModel.Game.Scene.GetComponent<ResourcesAsyncComponent>().LoadAssetAsync<GameObject>(Assets.LoadAssetAsync("Assets/Bundles/Prefab/TargetArrow.prefab", typeof(GameObject)));
                ReferenceCollector rc = resObj.GetComponent<ReferenceCollector>();
                GameObject targetArrowObj = GameObject.Instantiate<GameObject>(rc.Get<GameObject>("TargetArrow"));
                targetArrowObj.transform.position = InitPos;

                //创建TargetArrow实体脚本
                TargetArrow targetArrow = ComponentFactory.Create<TargetArrow, GameObject>(targetArrowObj, false);

                //添加控制组件
                targetArrow.AddComponent<TargetArrowComponent, PlayerCube, VariableJoystickComponent>(playerCube, AttackUI);


                tcs.SetResult(targetArrow);
            }
            catch (Exception e)
            {
                Log.Error(e);
                tcs.SetResult(null);
            }
        }
    }
}