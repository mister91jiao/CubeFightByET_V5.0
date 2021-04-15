using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class OtherCubeAttackComponentAwakeSystem : AwakeSystem<OtherCubeAttackComponent, GameObject>
    {
        public override void Awake(OtherCubeAttackComponent self, GameObject ganFire)
        {
            self.Awake(ganFire);
        }
    }

    [ObjectSystem]
    public class OtherCubeAttackComponentStartSystem : StartSystem<OtherCubeAttackComponent>
    {
        public override void Start(OtherCubeAttackComponent self)
        {
            self.Start();
        }
    }

    [ObjectSystem]
    public class OtherCubeAttackComponentUpdateSystem : UpdateSystem<OtherCubeAttackComponent>
    {
        public override void Update(OtherCubeAttackComponent self)
        {
            self.Update();
        }
    }

    /// <summary>
    /// 其它cube开火脚本，目前主要是枪口火焰
    /// </summary>
    public class OtherCubeAttackComponent : Component
    {
        /// <summary>
        /// 是否在开火
        /// </summary>
        private bool fire = false;

        public bool Fire
        {
            set
            {
                // ReSharper disable once RedundantCheckBeforeAssignment
                if (fire != value) fire = value;
            }
        }


        /// <summary>
        /// 枪口火焰obj
        /// </summary>
        private GameObject ganFire;

        /// <summary>
        /// 其它cube玩家的Transform组件
        /// </summary>
        private Transform OtherCube_Transform;

        /// <summary>
        /// 武器点
        /// </summary>
        public Transform cubeGun_Transform = null;

        /// <summary>
        /// 射速
        /// </summary>
        private float shootSpeedTime = 1.0f / 10.0f;

        /// <summary>
        /// 计时
        /// </summary>
        private float activeTime = 0.0f;

        public void Awake(GameObject ganFire)
        {
            this.ganFire = ganFire;

        }

        public void Start()
        {
            //获取Transform
            OtherCube_Transform = this.GetParent<OtherCube>().otherCube_GameObject.GetComponent<Transform>();
            cubeGun_Transform = OtherCube_Transform.Find("CubeBody/Gun");

            //设置枪口火焰的父物体
            ganFire.transform.parent = cubeGun_Transform;
            ganFire.SetActive(false);
            ganFire.transform.localPosition = Vector3.zero;
            ganFire.transform.localRotation = Quaternion.identity;
        }

        public void Update()
        {
            if (fire)
            {
                activeTime += Time.deltaTime;
                if (activeTime >= shootSpeedTime)
                {
                    showAttackGunFire();
                    activeTime = 0.0f;
                }
            }
            else
            {
                activeTime = 0.0f;
            }
        }

        /// <summary>
        /// 显示一次开火枪口火焰特效
        /// </summary>
        private void showAttackGunFire()
        {
            ganFire.SetActive(false);
            ganFire.SetActive(true);
        }
    }
}