using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class OtherCubeNetSyncComponentAwakeSystem : AwakeSystem<OtherCubeNetSyncComponent, int, Vector3>
    {
        public override void Awake(OtherCubeNetSyncComponent self, int Account, Vector3 InitPostion)
        {
            self.Awake(Account, InitPostion);
        }
    }

    [ObjectSystem]
    public class OtherCubeNetSyncComponentStartSystem : StartSystem<OtherCubeNetSyncComponent>
    {
        public override void Start(OtherCubeNetSyncComponent self)
        {
            self.Start();
        }
    }

    [ObjectSystem]
    public class OtherCubeNetSyncComponentUpdateSystem : UpdateSystem<OtherCubeNetSyncComponent>
    {
        public override void Update(OtherCubeNetSyncComponent self)
        {
            self.Update();
        }
    }

    public class OtherCubeNetSyncComponent : Component
    {
        /// <summary>
        /// 其它cube玩家的账号
        /// </summary>
        private int Account;

        /// <summary>
        /// OtherCube的实体
        /// </summary>
        public OtherCube otherCube;

        /// <summary>
        /// 其它cube玩家的初始位置
        /// </summary>
        private Vector3 InitPosition;

        /// <summary>
        /// 其它cube玩家的Transform组件
        /// </summary>
        private Transform OtherCube_Transform;

        /// <summary>
        /// 其它cube玩家的Transform组件
        /// </summary>
        private Transform OtherDirCube_Transform;

        /// <summary>
        /// 其它cube玩家的Rigidbody组件
        /// </summary>
        private Rigidbody OtherDirCube_Rigidbody;

        /// <summary>
        /// 攻击控制组件
        /// </summary>
        private OtherCubeAttackComponent otherCubeAttackComponent = null;

        public void Awake(int Account, Vector3 InitPostion)
        {
            Log.Info("其它玩家初始位置：" + InitPostion.ToString());

            this.Account = Account;
            this.InitPosition = InitPostion;
        }

        public void Start()
        {
            otherCube = this.GetParent<OtherCube>();

            //获取Transform
            OtherCube_Transform = otherCube.otherCube_GameObject.GetComponent<Transform>();
            OtherCube_Transform.position = InitPosition;

            OtherDirCube_Transform = otherCube.otherDirCube_GameObject.GetComponent<Transform>();
            OtherDirCube_Transform.position = InitPosition;

            //获取Rigidbody
            OtherDirCube_Rigidbody = OtherDirCube_Transform.GetComponent<Rigidbody>();

            //添加网络组件到集中管理
            Game.Scene.GetComponent<OtherCubeManagerComponent>()
                .AddNetSyncComponentByOtherCubeAccount(Account, this);

            //获取攻击控制组件
            otherCubeAttackComponent = otherCube.GetComponent<OtherCubeAttackComponent>();
        }

        /// <summary>
        /// 同步位置
        /// </summary>
        public void NetWorkAsyncPosition(Vector3 Position, Quaternion Rotation, Vector3 Velocity)
        {
            //OtherDirCube_Rigidbody.position = Position;
            OtherDirCube_Rigidbody.position = Position + Velocity * GloabConfigHelper.tick;

            OtherDirCube_Rigidbody.rotation = Rotation;

            OtherDirCube_Rigidbody.velocity = Velocity;
        }

        /// <summary>
        /// 同步是否开火
        /// </summary>
        public void NetWorkAsyncFire(bool isFire)
        {
            otherCubeAttackComponent.Fire = isFire;
        }

        /// <summary>
        /// 保持显示和方块的同步
        /// </summary>
        public void Update()
        {
            //OtherCube_Transform.position = Vector3.Lerp(OtherDirCube_Transform.position, OtherCube_Transform.position, 60.0f * Time.deltaTime);
            //OtherCube_Transform.rotation = Quaternion.Lerp(OtherDirCube_Transform.rotation, OtherCube_Transform.rotation, 60.0f * Time.deltaTime);

            OtherCube_Transform.position = Vector3.Lerp(OtherDirCube_Transform.position, OtherCube_Transform.position, 0.5f);
            OtherCube_Transform.rotation = Quaternion.Lerp(OtherDirCube_Transform.rotation, OtherCube_Transform.rotation, 0.5f);

            //OtherCube_Transform.position = OtherDirCube_Transform.position;
            //OtherCube_Transform.rotation = OtherDirCube_Transform.rotation;
        }
    }
}