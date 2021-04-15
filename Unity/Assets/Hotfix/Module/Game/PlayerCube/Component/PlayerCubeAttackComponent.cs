using ETModel;
using UnityEngine;

namespace ETHotfix
{

    [ObjectSystem]
    public class PlayerCubeAttackComponentAwakeSystem : AwakeSystem<PlayerCubeAttackComponent>
    {
        public override void Awake(PlayerCubeAttackComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class PlayerCubeAttackComponentStartSystem : StartSystem<PlayerCubeAttackComponent>
    {
        public override void Start(PlayerCubeAttackComponent self)
        {
            self.Start();
        }
    }

    [ObjectSystem]
    public class PlayerCubeAttackComponentUpDateSystem : UpdateSystem<PlayerCubeAttackComponent>
    {
        public override void Update(PlayerCubeAttackComponent self)
        {
            self.UpDate();
        }
    }

    public class PlayerCubeAttackComponent : Component
    {
        /// <summary>
        /// cube角色的Transform
        /// </summary>
        public Transform cubePlayer_Transform = null;

        /// <summary>
        /// cube角色的CharacterController组件
        /// </summary>
        private CharacterController cubePlayer_Controller = null;

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

        /// <summary>
        /// 准星的Object
        /// </summary>
        private GameObject attackObject = null;

        /// <summary>
        /// 玩家cube的同步组件
        /// </summary>
        private PlayerCubeNetComponent playerCubeNetComponent = null;

        public void Awake()
        {
            //查找相关引用
            cubePlayer_Transform = this.GetParent<PlayerCube>().cube_GameObject.GetComponent<Transform>();
            cubePlayer_Controller = cubePlayer_Transform.GetComponent<CharacterController>();
            cubeGun_Transform = cubePlayer_Transform.Find("CubeBody/Gun");
            attackObject = this.GetParent<PlayerCube>().GetComponent<PlayerCubeControllerComponent>().targetArrow.targetArrow_GameObject;
            

        }

        public void Start()
        {
            playerCubeNetComponent = this.GetParent<PlayerCube>().GetComponent<PlayerCubeNetComponent>();
        }

        public void UpDate()
        {
            if (attackObject != null)
            {
                if (attackObject.activeSelf)
                {
                    activeTime += Time.deltaTime;
                    if (activeTime >= shootSpeedTime)
                    {
                        cubeAttack();
                        activeTime = 0.0f;
                    }
                }
                else
                {
                    activeTime = 0.0f;
                }
            }
        }

        /// <summary>
        /// 玩家控制的方块攻击了一次
        /// </summary>
        public void cubeAttack()
        {
            //Debug.LogError("需要攻击");
            CubeBullet cubeBullet = CubeBulletFactory.CreateCubeBullet();

            cubeBullet.Attack(cubeGun_Transform.gameObject, attackObject, cubePlayer_Controller.velocity);

            playerCubeNetComponent.needSyncBullet.Enqueue(cubeBullet);
        }

        /// <summary>
        /// 是否正在攻击
        /// </summary>
        public bool isAttacking()
        {
            return attackObject.activeSelf;
        }

    }
}