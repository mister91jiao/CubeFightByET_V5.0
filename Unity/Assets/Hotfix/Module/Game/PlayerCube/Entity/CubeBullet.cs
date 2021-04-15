using ETModel;
using System;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class CubeBulletAwakeSystem : AwakeSystem<CubeBullet, GameObject[]>
    {
        public override void Awake(CubeBullet self, GameObject[] bulletObj)
        {
            self.Awake(bulletObj);
        }
    }

    [ObjectSystem]
    public class CubeBulletUpdateSystem : UpdateSystem<CubeBullet>
    {

        public override void Update(CubeBullet self)
        {
            self.Update();
        }
    }

    public class CubeBullet : Entity
    {
        /// <summary>
        /// 这个玩家的子弹唯一ID
        /// </summary>
        public long onlyBulletID;

        /// <summary>
        /// 只有主角自己发射出来的子弹才同步
        /// </summary>
        public bool bulletCanAttack = false;

        /// <summary>
        /// 伤害同步脚本
        /// </summary>
        public HurtSyncComponent hurtSyncComponent;

        /// <summary>
        /// 子弹的三部分, 枪口特效，子弹特效，命中特效
        /// </summary>
        public GameObject[] bulletObj;

        /// <summary>
        /// 子弹速度
        /// </summary>
        private float bulletSpeed = 40.0f;

        /// <summary>
        /// 子弹正在飞
        /// </summary>
        public bool bulletFlying = false;

        /// <summary>
        /// 以前的位置
        /// </summary>
        private Vector3 previousPosition;

        /// <summary>
        /// 子弹的生命
        /// </summary>
        private float bulletLife = 5.0f;

        /// <summary>
        /// 子弹生命开始计时
        /// </summary>
        private bool bulletLifeKey = false;

        public void Awake(GameObject[] bulletObj)
        {
            this.bulletObj = bulletObj;
            //初始化全部隐藏
            for (int i = 0; i < bulletObj.Length; i++)
            {
                bulletObj[i].SetActive(false);
            }

            onlyBulletID = this.InstanceId;
        }

        public void Update()
        {
            if (bulletFlying)
            {
                CheckCollision(previousPosition);
                previousPosition = bulletObj[1].transform.position;
            }

            if (bulletLifeKey)
            {
                bulletLife = bulletLife - Time.deltaTime;
                if (bulletLife <= 0)
                {
                    BulletRecovery();
                }
            }
        }

        /// <summary>
        /// 子弹被用于攻击
        /// </summary>
        public void Attack(GameObject gunParent, GameObject dirObj, Vector3 baseVelocity)
        {
            //设置枪口火光
            bulletObj[0].SetActive(true);
            bulletObj[0].transform.position = gunParent.transform.position;
            bulletObj[0].transform.rotation = gunParent.transform.rotation;
            bulletObj[0].transform.parent = gunParent.transform;

            //设置子弹
            bulletObj[1].SetActive(true);
            bulletObj[1].transform.position = gunParent.transform.position;
            bulletObj[1].transform.LookAt(dirObj.transform.position);
            bulletObj[1].GetComponent<Rigidbody>().velocity = bulletObj[1].transform.forward * bulletSpeed + baseVelocity;

            //子弹第一次发射需要对此进行赋值
            previousPosition = bulletObj[1].transform.position;

            bulletFlying = true;

            bulletLifeKey = true;

            bulletCanAttack = true;
        }

        /// <summary>
        /// 同步子弹
        /// </summary>
        public void SyncBullet(Vector3 Position, Quaternion Rotation, Vector3 Velocity)
        {
            //设置子弹
            bulletObj[1].SetActive(true);
            bulletObj[1].transform.position = Position;
            bulletObj[1].transform.rotation = Rotation;
            bulletObj[1].GetComponent<Rigidbody>().velocity = Velocity;

            //子弹第一次发射需要对此进行赋值
            previousPosition = bulletObj[1].transform.position;

            bulletFlying = true;

            bulletLifeKey = true;

            bulletCanAttack = false;
        }

        /// <summary>
        /// 传入以前的位置进行检测
        /// </summary>
        /// <param name="prevPos">以前的位置</param>
        private void CheckCollision(Vector3 prevPos)
        {
            RaycastHit hit;
            //获取方向
            Vector3 direction = bulletObj[1].transform.position - prevPos;
            //发射射线
            Ray ray = new Ray(prevPos, direction);
            //获取射线长度
            float dist = Vector3.Distance(bulletObj[1].transform.position, prevPos);
            //进行射线检测, 如果射线打到物体了说明命中了
            if (Physics.Raycast(ray, out hit, dist, (1 << 14) | (1 << 16) | (1 << 17)))
            {
                bulletObj[1].transform.position = hit.point;
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                Vector3 pos = hit.point;
                //根据计算出来的旋转和位置展示命中特效
                BulletHit(rot, pos, hit);

                if (hit.transform.gameObject.layer == 17 && bulletCanAttack)
                {
                    //Debug.LogError("打中了敌人: " + GetHitOtherCubeEntity(hit.transform.gameObject).account);
                    hurtSyncComponent.HitToAccount(GetHitOtherCubeEntity(hit.transform.gameObject).account, 15);
                }

            }
        }

        /// <summary>
        /// 得到命中的其它cube的脚本实体
        /// </summary>
        private OtherCube GetHitOtherCubeEntity(GameObject hitObj)
        {
            ComponentView componentView = hitObj.GetComponent<ComponentView>();
            if (componentView != null)
            {
                return componentView.Component as OtherCube;
            }
            else
            {
                return GetHitOtherCubeEntity(hitObj.transform.parent.gameObject);
            }
        }

        /// <summary>
        /// 子弹命中
        /// </summary>
        private void BulletHit(Quaternion rot, Vector3 pos, RaycastHit hit)
        {
            bulletFlying = false;

            //子弹特效需要隐藏归位
            bulletObj[1].SetActive(false);
            bulletObj[1].transform.position = Vector3.zero;
            bulletObj[1].GetComponent<Rigidbody>().velocity = Vector3.zero;

            //显示命中特效
            bulletObj[2].SetActive(true);
            bulletObj[2].transform.rotation = rot;
            bulletObj[2].transform.position = pos;
            bulletObj[2].transform.parent = hit.transform;

            bulletLife = 1.0f;
        }

        /// <summary>
        /// 子弹回收
        /// </summary>
        public void BulletRecovery()
        {
            bulletFlying = false;

            bulletLifeKey = false;
            bulletLife = 5.0f;

            for (int i = 0; i < bulletObj.Length; i++)
            {
                bulletObj[i].transform.parent = this.GameObject.transform;
                bulletObj[i].transform.position = Vector3.zero;
                bulletObj[i].transform.rotation = Quaternion.identity;
                bulletObj[i].SetActive(false);
            }

            bulletCanAttack = false;

            CubeBulletFactory.CubeBulletEnPool(this);
        }

        
    }

}