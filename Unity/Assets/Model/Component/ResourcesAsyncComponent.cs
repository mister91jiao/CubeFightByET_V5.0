using libx;
using UnityEngine;

namespace ETModel
{
    public class ResourcesAsyncComponent : Component
    {
        /// <summary>
        /// 异步加载资源
        /// </summary>
        public ETTask<T> LoadAssetAsync<T>(AssetRequest assetRequest) where T : UnityEngine.Object
        {
            ETTaskCompletionSource<T> tcs = new ETTaskCompletionSource<T>();

            //如果已经加载完成则直接返回结果（适用于编辑器模式下的异步写法和重复加载）
            if (assetRequest.isDone)
            {
                tcs.SetResult((T) assetRequest.asset);
                return tcs.Task;
            }

            //+=委托链，否则会导致前面完成委托被覆盖
            assetRequest.completed += (arq) => { tcs.SetResult((T) arq.asset); };
            return tcs.Task;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        public ETTask<SceneAssetRequest> LoadSceneAsync(SceneAssetRequest sceneAssetRequest)
        {
            ETTaskCompletionSource<SceneAssetRequest> tcs = new ETTaskCompletionSource<SceneAssetRequest>();
            sceneAssetRequest.completed = (arq) => { tcs.SetResult(sceneAssetRequest); };
            return tcs.Task;
        }
    }
}