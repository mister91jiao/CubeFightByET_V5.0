using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    public class OtherCubeManagerComponent : Component
    {
        /// <summary>
        /// 其它cube玩家的账号以及其对应的网络同步组件
        /// </summary>
        private Dictionary<int, OtherCubeNetSyncComponent> otherCubeAccountToNetSyncComponent =
            new Dictionary<int, OtherCubeNetSyncComponent>();

        /// <summary>
        /// 通过账号获取其它cube玩家的网络同步组件
        /// </summary>
        public OtherCubeNetSyncComponent GetNetSyncComponentByOtherCubeAccount(int Account)
        {
            if (otherCubeAccountToNetSyncComponent.TryGetValue(Account,
                out OtherCubeNetSyncComponent netSyncComponent))
            {
                return netSyncComponent;
            }
            return null;
        }

        /// <summary>
        /// 添加其它cube玩家的网络同步组件
        /// </summary>
        public void AddNetSyncComponentByOtherCubeAccount(int Account, OtherCubeNetSyncComponent netSyncComponent)
        {
            if (!otherCubeAccountToNetSyncComponent.ContainsKey(Account))
            {
                otherCubeAccountToNetSyncComponent.Add(Account, netSyncComponent);
            }
            else
            {
                Log.Info("其它玩家已经存在：" + Account);
            }
        }

        /// <summary>
        /// 一个cube死亡
        /// </summary>
        public void OtherCubeDie(int DieAccount)
        {
            if (otherCubeAccountToNetSyncComponent.TryGetValue(DieAccount, out OtherCubeNetSyncComponent otherCubeNetSync))
            {
                otherCubeNetSync.otherCube.Die = true;
                otherCubeNetSync.otherCube.otherCube_GameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("错误，需要同步死亡的cube里找不到这个：" + DieAccount);
            }
        }

        /// <summary>
        /// 一个cube复活
        /// </summary>
        public void OtherCubeResurrection(int ResurrectionAccount, Vector3 ResurrectionPosition)
        {
            if (otherCubeAccountToNetSyncComponent.TryGetValue(ResurrectionAccount, out OtherCubeNetSyncComponent otherCubeNetSync))
            {
                otherCubeNetSync.otherCube.Die = false;
                otherCubeNetSync.otherCube.otherCube_GameObject.SetActive(true);
                otherCubeNetSync.NetWorkAsyncPosition(ResurrectionPosition, Quaternion.identity, Vector3.zero);
            }
            else
            {
                Debug.LogError("错误，需要同步复活的cube里找不到这个：" + ResurrectionAccount);
            }
        }

        /// <summary>
        /// 移除一个其它Cube
        /// </summary>
        public void RemoveOneOtherCube(int Account)
        {
            if (otherCubeAccountToNetSyncComponent.TryGetValue(Account, out OtherCubeNetSyncComponent otherCubeNet))
            {
                otherCubeAccountToNetSyncComponent.Remove(Account);
                OtherCube otherCube = otherCubeNet.otherCube;

                otherCube.Dispose();
            }
            else
            {
                Debug.LogError("错误，没有找到需要移除的OtherCube");
            }
        }

    }
}