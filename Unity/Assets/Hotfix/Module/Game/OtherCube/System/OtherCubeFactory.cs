using System;
using ETModel;
using libx;
using UnityEngine;

namespace ETHotfix
{
    public static class OtherCubeFactory
    {
        public static ETTask<OtherCube> Create(int account, Vector3 InitPosition)
        {
            ETTaskCompletionSource<OtherCube> tcs = new ETTaskCompletionSource<OtherCube>();

            CreateOtherCube(account, InitPosition, tcs).Coroutine();


            return tcs.Task;
        }

        private static async ETVoid CreateOtherCube(int account, Vector3 InitPosition, ETTaskCompletionSource<OtherCube> tcs)
        {
            try
            {
                GameObject resObj = await ETModel.Game.Scene.GetComponent<ResourcesAsyncComponent>().LoadAssetAsync<GameObject>(Assets.LoadAssetAsync("Assets/Bundles/Prefab/OtherCube.prefab", typeof(GameObject)));
                ReferenceCollector rc = resObj.GetComponent<ReferenceCollector>();
                GameObject otherCubeObj = GameObject.Instantiate<GameObject>(rc.Get<GameObject>("OtherCube"));
                GameObject otherDirCubeObj = GameObject.Instantiate<GameObject>(rc.Get<GameObject>("OtherDirCube"));
                GameObject ganFire = GameObject.Instantiate<GameObject>(rc.Get<GameObject>("gunFire"));

                OtherCube otherCube = ComponentFactory.Create<OtherCube, int, GameObject, GameObject>(account, otherCubeObj, otherDirCubeObj, false);

                //添加攻击脚本
                OtherCubeAttackComponent otherCubeAttackComponent = otherCube.AddComponent<OtherCubeAttackComponent, GameObject>(ganFire);

                //添加网络同步组件
                OtherCubeNetSyncComponent otherCubeNetSyncComponent = otherCube.AddComponent<OtherCubeNetSyncComponent, int, Vector3>(account, InitPosition);

                tcs.SetResult(otherCube);
            }
            catch (Exception e)
            {
                Log.Error(e);
                tcs.SetResult(null);
            }
        }
    }
}