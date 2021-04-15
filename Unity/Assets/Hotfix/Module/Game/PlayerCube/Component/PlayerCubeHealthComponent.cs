using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class PlayerCubeHealthComponentAwakeSystem : AwakeSystem<PlayerCubeHealthComponent>
    {
        public override void Awake(PlayerCubeHealthComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class PlayerCubeHealthComponentStartSystem : StartSystem<PlayerCubeHealthComponent>
    {
        public override void Start(PlayerCubeHealthComponent self)
        {
            self.Start();
        }
    }

    public class PlayerCubeHealthComponent : Component
    {
        /// <summary>
        /// 玩家cube实体
        /// </summary>
        private PlayerCube playerCube;

        /// <summary>
        /// 玩家生命值信息UI
        /// </summary>
        private UI playerInfoUI;

        /// <summary>
        /// 玩家生命值信息UI组件
        /// </summary>
        private UIPlayerInfoComponent playerInfoUIComponent;

        /// <summary>
        /// 玩家死亡UI
        /// </summary>
        private UI playerDieUI;

        /// <summary>
        /// 玩家死亡UI组件
        /// </summary>
        private UIPlayerDieComponent playerDieUIComponent;

        public void Awake()
        {
            playerInfoUI = UIPlayerInfoFactory.Create();
            Game.Scene.GetComponent<UIComponent>().Add(playerInfoUI);

            playerDieUI = UIPlayerDieFactory.Create();
            Game.Scene.GetComponent<UIComponent>().Add(playerDieUI);
        }

        public void Start()
        {
            //查找引用
            playerInfoUIComponent = playerInfoUI.GetComponent<UIPlayerInfoComponent>();

            playerDieUIComponent = playerDieUI.GetComponent<UIPlayerDieComponent>();

            playerCube = this.GetParent<PlayerCube>();
        }

        /// <summary>
        /// 设置生命值UI的显示
        /// </summary>
        public void SetPlayerCubeHealth(int health)
        {
            playerInfoUIComponent.Health.fillAmount = (health / 100.0f);
        }

        /// <summary>
        /// 设置玩家死亡
        /// </summary>
        public void SetPlayerDie()
        {
            playerCube.PlayerDie = true;

            //设置死亡面板显示
            playerDieUIComponent.DiePanel.SetActive(true);

            PlayerCubeControllerComponent playerCubeControllerComponent = playerCube.GetComponent<PlayerCubeControllerComponent>();

            //设置自己隐藏
            playerCubeControllerComponent.cubePlayerBody_Transform.gameObject.SetActive(false);

            //隐藏攻击箭头
            playerCubeControllerComponent.targetArrow.targetArrow_GameObject.SetActive(false);

            //隐藏控制UI
            VariableJoystickComponent variableJoystickController = playerCubeControllerComponent.cubePlayer_ControllerUI;
            variableJoystickController.OnPointerUp();
            variableJoystickController.GetParent<UI>().GameObject.SetActive(false);

            //隐藏攻击UI
            VariableJoystickComponent variableJoystickAttack = playerCubeControllerComponent.targetArrow.GetComponent<TargetArrowComponent>().AttackUI;
            variableJoystickAttack.OnPointerUp();
            variableJoystickAttack.GetParent<UI>().GameObject.SetActive(false);

        }

        /// <summary>
        /// 设置玩家复活
        /// </summary>
        public void SetPlayerResurrection(Vector3 ResurrectionPos)
        {
            playerCube.PlayerDie = false;

            //设置死亡面板显示
            playerDieUIComponent.DiePanel.SetActive(false);

            //设置血条
            SetPlayerCubeHealth(100);

            PlayerCubeControllerComponent playerCubeControllerComponent = playerCube.GetComponent<PlayerCubeControllerComponent>();

            //设置自己隐藏
            playerCube.cube_GameObject.transform.position = ResurrectionPos;
            playerCubeControllerComponent.cubePlayerBody_Transform.gameObject.SetActive(true);

            //隐藏攻击箭头
            playerCubeControllerComponent.targetArrow.targetArrow_GameObject.SetActive(true);

            //隐藏控制UI
            VariableJoystickComponent variableJoystickController = playerCubeControllerComponent.cubePlayer_ControllerUI;
            variableJoystickController.GetParent<UI>().GameObject.SetActive(true);

            //隐藏攻击UI
            VariableJoystickComponent variableJoystickAttack = playerCubeControllerComponent.targetArrow.GetComponent<TargetArrowComponent>().AttackUI;
            variableJoystickAttack.GetParent<UI>().GameObject.SetActive(true);

        }

    }
}