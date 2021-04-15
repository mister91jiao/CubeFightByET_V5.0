using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class TargetArrowComponentAwakeSystem : AwakeSystem<TargetArrowComponent, PlayerCube, VariableJoystickComponent>
    {
        public override void Awake(TargetArrowComponent self, PlayerCube playerCube, VariableJoystickComponent AttackUI)
        {
            self.Awake(playerCube, AttackUI);
        }
    }

    [ObjectSystem]
    public class TargetArrowComponentUpDateSystem : UpdateSystem<TargetArrowComponent>
    {
        public override void Update(TargetArrowComponent self)
        {
            self.UpDate();
        }
    }

    public class TargetArrowComponent : Component
    {
        /// <summary>
        /// 箭头模型的Transform引用
        /// </summary>
        private Transform targetArrow_Transform = null;

        /// <summary>
        /// 角色cube引用脚本
        /// </summary>
        private GameObject cube_GameObject;

        private Transform cubePlayer_CameraBaseTransform;

        /// <summary>
        /// 准星控制摇滚组件
        /// </summary>
        public VariableJoystickComponent AttackUI;

        /// <summary>
        /// 准星最大范围
        /// </summary>
        private float TargetArrowDistance = 7.5f;

        public void Awake(PlayerCube playerCube, VariableJoystickComponent AttackUI)
        {
            this.cube_GameObject = playerCube.cube_GameObject;
            this.AttackUI = AttackUI;

            //查找相关引用
            targetArrow_Transform = this.GetParent<TargetArrow>().targetArrow_GameObject.GetComponent<Transform>();
            SetTargetArrowPosition(cube_GameObject.transform.position);
            cubePlayer_CameraBaseTransform = cube_GameObject.transform.Find("PlayerCameraBase");
        }

        public void UpDate()
        {
            if (AttackUI.Direction != Vector2.zero)
            {
                targetArrow_Transform.gameObject.SetActive(true);
                Vector3 arrowPosition = cube_GameObject.transform.position + (cubePlayer_CameraBaseTransform.right * AttackUI.Horizontal + cubePlayer_CameraBaseTransform.forward * AttackUI.Vertical) * TargetArrowDistance;
                SetTargetArrowPosition(arrowPosition);

            }
            else
            {
                targetArrow_Transform.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 设置准星的位置
        /// </summary>
        /// <param name="position">准星的二维位置</param>
        public void SetTargetArrowPosition(Vector3 position)
        {
            Vector3 originPos = new Vector3(position.x, 15, position.z);

            if (Physics.Raycast(originPos, Vector3.down, out RaycastHit hit, 25, 14336))
            {
                //Debug.LogError(hit.point.ToString());
                targetArrow_Transform.position = hit.point + new Vector3(0, 0.5f, 0);
            }
            else
            {
                Debug.LogError("错误，射线没有打到东西");
            }
        }

    }
}