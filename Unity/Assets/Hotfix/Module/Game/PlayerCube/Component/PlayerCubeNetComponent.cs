using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class PlayerCubeNetComponentAwakeSystem : AwakeSystem<PlayerCubeNetComponent, int>
    {
        public override void Awake(PlayerCubeNetComponent self, int Account)
        {
            self.Awake(Account);
        }
    }

    [ObjectSystem]
    public class PlayerCubeNetComponentStartSystem : StartSystem<PlayerCubeNetComponent>
    {
        public override void Start(PlayerCubeNetComponent self)
        {
            self.Start();
        }
    }

    [ObjectSystem]
    public class PlayerCubeNetComponentUpDateSystem : UpdateSystem<PlayerCubeNetComponent>
    {
        public override void Update(PlayerCubeNetComponent self)
        {
            self.UpDate();
        }
    }

    public class PlayerCubeNetComponent : Component
    {
        /// <summary>
        /// 当前玩家账号
        /// </summary>
        private int playerAccount;

        /// <summary>
        /// 玩家cube实体
        /// </summary>
        private PlayerCube playerCube;

        /// <summary>
        /// 角色cube的Transform组件
        /// </summary>
        private Transform cube_Transform = null;

        /// <summary>
        /// cubeBody的Transform组件
        /// </summary>
        private Transform body_Transform = null;

        /// <summary>
        /// 玩家的控制组件
        /// </summary>
        private CharacterController cube_CharacterController = null;

        /// <summary>
        /// 计时器
        /// </summary>
        private float timer = 0.0f;

        /// <summary>
        /// 数据包发送器
        /// </summary>
        private Session hotfixSession = null;

        /// <summary>
        /// 发送的数据包
        /// </summary>
        private C2G_PlayerRoleNetwork NetPackge = null;

        /// <summary>
        /// 玩家的攻击组件
        /// </summary>
        private PlayerCubeAttackComponent playerCubeAttackComponent = null;

        /// <summary>
        /// 需要同步的子弹
        /// </summary>
        public Queue<CubeBullet> needSyncBullet = new Queue<CubeBullet>();

        public void Awake(int Account)
        {
            playerAccount = Account;
        }

        public void Start()
        {
            playerCube = this.GetParent<PlayerCube>();
            cube_Transform = playerCube.cube_GameObject.GetComponent<Transform>();
            body_Transform = cube_Transform.Find("CubeBody");
            cube_CharacterController = cube_Transform.GetComponent<CharacterController>();

            hotfixSession = Game.Scene.GetComponent<SessionComponent>().Session;
            NetPackge = new C2G_PlayerRoleNetwork();
            NetPackge.Account = playerAccount;

            playerCubeAttackComponent = this.GetParent<PlayerCube>().GetComponent<PlayerCubeAttackComponent>();
        }

        public void UpDate()
        {
            timer += Time.deltaTime;
            if (timer >= GloabConfigHelper.tick)
            {
                timer = 0;
                sendNetPostion();
            }
        }

        /// <summary>
        /// 发送一次网络包
        /// </summary>
        public void sendNetPostion()
        {
            NetPackge.PositionX = cube_Transform.position.x;
            NetPackge.PositionY = cube_Transform.position.y;
            NetPackge.PositionZ = cube_Transform.position.z;

            NetPackge.RotationX = body_Transform.rotation.x;
            NetPackge.RotationY = body_Transform.rotation.y;
            NetPackge.RotationZ = body_Transform.rotation.z;
            NetPackge.RotationW = body_Transform.rotation.w;

            NetPackge.VelocityX = cube_CharacterController.velocity.x;
            NetPackge.VelocityY = cube_CharacterController.velocity.y;
            NetPackge.VelocityZ = cube_CharacterController.velocity.z;

            NetPackge.Fire = playerCubeAttackComponent.isAttacking();

            int count = needSyncBullet.Count;
            for (int i = 0; i < count; i++)
            {
                
                CubeBullet cubeBullet = needSyncBullet.Dequeue();
                if (cubeBullet.bulletFlying)
                {
                    BulletInfo bulletInfo = new BulletInfo();

                    bulletInfo.Account = playerAccount;

                    bulletInfo.PositionX = cubeBullet.bulletObj[1].transform.position.x;
                    bulletInfo.PositionY = cubeBullet.bulletObj[1].transform.position.y;
                    bulletInfo.PositionZ = cubeBullet.bulletObj[1].transform.position.z;

                    bulletInfo.RotationX = cubeBullet.bulletObj[1].transform.rotation.x;
                    bulletInfo.RotationY = cubeBullet.bulletObj[1].transform.rotation.y;
                    bulletInfo.RotationZ = cubeBullet.bulletObj[1].transform.rotation.z;
                    bulletInfo.RotationW = cubeBullet.bulletObj[1].transform.rotation.w;

                    Vector3 velocity = cubeBullet.bulletObj[1].GetComponent<Rigidbody>().velocity;
                    bulletInfo.VelocityX = velocity.x;
                    bulletInfo.VelocityY = velocity.y;
                    bulletInfo.VelocityZ = velocity.z;

                    NetPackge.Bullets.Add(bulletInfo);
                }
            }

            //玩家活着才发包
            if (!playerCube.PlayerDie)
            {
                hotfixSession.Send(NetPackge);
            }

            NetPackge.Bullets.Clear();
        }

    }
}