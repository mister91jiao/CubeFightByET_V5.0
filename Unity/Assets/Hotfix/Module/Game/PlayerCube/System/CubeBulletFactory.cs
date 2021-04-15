using System;
using System.Collections.Generic;
using ETModel;
using libx;
using UnityEngine;

namespace ETHotfix
{
    public static class CubeBulletFactory
    {
        private static AssetRequest assetRequest;
        private static ReferenceCollector rc;

        private static Queue<CubeBullet> cubeBulletPool = new Queue<CubeBullet>();

        private static HurtSyncComponent hurtSyncComponent = null;

        public static CubeBullet CreateCubeBullet()
        {
            if (rc == null)
            {
                assetRequest = Assets.LoadAsset("Assets/Bundles/Prefab/BulletFX.prefab", typeof(GameObject));
                GameObject bundleGameObject = assetRequest.asset as GameObject;
                rc = bundleGameObject.GetComponent<ReferenceCollector>();
            }

            if (hurtSyncComponent == null)
            {
                hurtSyncComponent = MapHelper.nowPlayerCube.GetComponent<HurtSyncComponent>();
            }

            //池里至少有5个才取
            if (cubeBulletPool.Count > 5)
            {
                CubeBullet cubeBullet = cubeBulletPool.Dequeue();
                return cubeBullet;
            }
            else
            {
                GameObject[] bulletObj = new GameObject[3] { GameObject.Instantiate<GameObject>(rc.Get<GameObject>("gunFire")),
                    GameObject.Instantiate<GameObject>(rc.Get<GameObject>("bullet")),
                    GameObject.Instantiate<GameObject>(rc.Get<GameObject>("hitEffect"))};

                CubeBullet cubeBullet = ComponentFactory.Create<CubeBullet, GameObject[]>(bulletObj, false);

                cubeBullet.hurtSyncComponent = hurtSyncComponent;

                for (int i = 0; i < bulletObj.Length; i++)
                {
                    bulletObj[i].transform.parent = cubeBullet.GameObject.transform;
                }

                //设置实体的管理组件
                cubeBullet.Parent = Game.Scene.GetComponent<CubeBulletManagerComponent>();

                return cubeBullet;
            }
        }

        /// <summary>
        /// CubeBullet回收进池
        /// </summary>
        public static void CubeBulletEnPool(CubeBullet cubeBullet)
        {
            cubeBulletPool.Enqueue(cubeBullet);
        }
    }
}


