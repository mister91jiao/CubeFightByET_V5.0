using ETModel;
using UnityEngine;

namespace ETHotfix
{

    [ObjectSystem]
    public class PlayerCubeControllerComponentAwakeSystem : AwakeSystem<PlayerCubeControllerComponent, UI>
    {
        public override void Awake(PlayerCubeControllerComponent self, UI ui)
        {
            self.Awake(ui);
        }
    }

    [ObjectSystem]
    public class PlayerCubeControllerComponentUpDateSystem : UpdateSystem<PlayerCubeControllerComponent>
    {
        public override void Update(PlayerCubeControllerComponent self)
        {
            self.UpDate();
        }
    }

    public class PlayerCubeControllerComponent : Component
    {
        /// <summary>
        /// 玩家cube实体
        /// </summary>
        private PlayerCube playerCube;

        /// <summary>
        /// cube角色的Transform
        /// </summary>
        public Transform cubePlayer_Transform = null;

        /// <summary>
        /// cube角色身体的Transform
        /// </summary>
        public Transform cubePlayerBody_Transform = null;

        /// <summary>
        /// cube角色的摄像机父物体Transform
        /// </summary>
        public Transform cubePlayer_CameraBaseTransform = null;

        /// <summary>
        /// 地面检测点
        /// </summary>
        private Transform groundCheck = null;

        /// <summary>
        /// cube角色的CharacterController组件
        /// </summary>
        private CharacterController cubePlayer_Controller = null;

        /// <summary>
        /// cube角色的UI控制组件
        /// </summary>
        public VariableJoystickComponent cubePlayer_ControllerUI = null;

        /// <summary>
        /// 关联准星物体
        /// </summary>
        public TargetArrow targetArrow = null;

        /// <summary>
        /// 移动速度
        /// </summary>
        private float speed = 10.0f;

        /// <summary>
        /// 速率
        /// </summary>
        private Vector3 velocity = Vector3.zero;

        /// <summary>
        /// 重力加速度
        /// </summary>
        private float gravity = -9.81f * 4.0f;

        /// <summary>
        /// 检测地面距离
        /// </summary>
        public float groundDistance = 0.15f;

        /// <summary>
        /// 跳跃高度
        /// </summary>
        private float jumpHeight = 1.0f;

        /// <summary>
        /// 是否在地面
        /// </summary>
        private bool isGrounded = true;


        public void Awake(UI ui)
        {
            //查找相关引用
            playerCube = this.GetParent<PlayerCube>();
            cubePlayer_Transform = playerCube.cube_GameObject.GetComponent<Transform>();
            cubePlayerBody_Transform = cubePlayer_Transform.Find("CubeBody");
            cubePlayer_CameraBaseTransform = cubePlayer_Transform.Find("PlayerCameraBase");
            groundCheck = cubePlayerBody_Transform.Find("GroundCheck");
            cubePlayer_Controller = cubePlayer_Transform.GetComponent<CharacterController>();


            cubePlayer_ControllerUI = ui.GetComponent<VariableJoystickComponent>();
        }

        public void UpDate()
        {
            //检测角色是否在地面
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, 14336);


            //旋转body
            if (targetArrow != null)
            {
                if (targetArrow.targetArrow_GameObject.activeSelf)
                {
                    cubePlayerBody_Transform.LookAt(
                        new Vector3(targetArrow.targetArrow_GameObject.transform.position.x,
                            cubePlayerBody_Transform.position.y, targetArrow.targetArrow_GameObject.transform.position.z),
                        Vector3.up);
                }
            }
            else
            {
                cubePlayerBody_Transform.rotation = cubePlayer_CameraBaseTransform.rotation;
            }
            
            

            CalcJumpVelocity();

            //角色活着才能移动
            if (!playerCube.PlayerDie)
            {
                if (GloabConfigHelper.controllerType == ControllerType.PC)
                {
                    keyboardMove();
                }
                else
                {
                    joyStakeMove();
                }
            }
            
        }

        /// <summary>
        /// 计算跳跃速率
        /// </summary>
        public void CalcJumpVelocity()
        {
            if (isGrounded)
            {
                if (velocity.y < 0)
                {
                    velocity.y = -2f;
                }
                if (Input.GetButtonDown("Jump"))
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }
            velocity.y += gravity * Time.deltaTime;
        }

        /// <summary>
        /// 摇滚控制移动
        /// </summary>
        public void joyStakeMove()
        {
            Vector3 move = cubePlayer_CameraBaseTransform.right * cubePlayer_ControllerUI.Horizontal + cubePlayer_CameraBaseTransform.forward * cubePlayer_ControllerUI.Vertical;

            //进行移动
            cubePlayer_Controller.Move(move * Time.deltaTime * speed + velocity * Time.deltaTime);
        }

        /// <summary>
        /// 键盘控制移动
        /// </summary>
        public void keyboardMove()
        {
            Vector3 move = (cubePlayer_CameraBaseTransform.right * Input.GetAxis("Horizontal") + cubePlayer_CameraBaseTransform.forward * Input.GetAxis("Vertical")).normalized;

            //进行移动
            cubePlayer_Controller.Move(move * Time.deltaTime * speed + velocity * Time.deltaTime);
        }
    }

    

}
