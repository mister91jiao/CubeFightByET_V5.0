using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class BulletManagerComponentAwakeSystem : AwakeSystem<BulletManagerComponent>
    {
        public override void Awake(BulletManagerComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class BulletManagerComponentStartSystem : StartSystem<BulletManagerComponent>
    {
        public override void Start(BulletManagerComponent self)
        {
            self.Start();
        }
    }
    public class BulletManagerComponent : Component
    {
        /// <summary>
        /// 每个账号和其对应的需要同步的子弹
        /// </summary>
        private Dictionary<int, Queue<Bullet>> accountToBullet = new Dictionary<int, Queue<Bullet>>();


        public void Awake()
        {
            Log.Info("Map服子弹管理组件初始化");
        }

        public void Start()
        {
           
        }


        /// <summary>
        /// 添加子弹到待同步队列
        /// </summary>
        public void AddBulletToQueue(Bullet bullet)
        {
            if (!accountToBullet.TryGetValue(bullet.account, out Queue<Bullet> needSyncQueue))
            {
                needSyncQueue = new Queue<Bullet>();
                accountToBullet.Add(bullet.account, needSyncQueue);
            }

            needSyncQueue.Enqueue(bullet);
        }

        /// <summary>
        /// 获取所有需要发射同步的子弹
        /// </summary>
        public List<BulletInfo> GetAllNeedSyncBullet()
        {
            List<BulletInfo> bulletInfos = new List<BulletInfo>();
            foreach (Queue<Bullet> needSyncQueue in accountToBullet.Values)
            {
                int count = needSyncQueue.Count;
                for (int i = 0; i < count; i++)
                {
                    Bullet bullet = needSyncQueue.Dequeue();
                    BulletInfo bulletInfo = new BulletInfo();

                    bulletInfo.Account = bullet.account;

                    bulletInfo.PositionX = bullet.PositionX;
                    bulletInfo.PositionY = bullet.PositionY;
                    bulletInfo.PositionZ = bullet.PositionZ;

                    bulletInfo.RotationX = bullet.RotationX;
                    bulletInfo.RotationY = bullet.RotationY;
                    bulletInfo.RotationZ = bullet.RotationZ;
                    bulletInfo.RotationW = bullet.RotationW;

                    bulletInfo.VelocityX = bullet.VelocityX;
                    bulletInfo.VelocityY = bullet.VelocityY;
                    bulletInfo.VelocityZ = bullet.VelocityZ;

                    bulletInfos.Add(bulletInfo);
                }

                //Log.Error("出队完成后数量：" + needSyncQueue.Count);
            }

            //Log.Error("出队完成后数量：" + bulletInfos.Count);
            return bulletInfos;
        }

    }
}
