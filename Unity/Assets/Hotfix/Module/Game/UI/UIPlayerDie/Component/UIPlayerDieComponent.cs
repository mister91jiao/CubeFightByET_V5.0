using System;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIPlayerDieComponentSystem : AwakeSystem<UIPlayerDieComponent>
    {
        public override void Awake(UIPlayerDieComponent self)
        {
            self.Awake();
        }
    }

    public class UIPlayerDieComponent : Component
    {

        /// <summary>
        /// 死亡面板
        /// </summary>
        public GameObject DiePanel;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            DiePanel = rc.Get<GameObject>("DiePanel");

            DiePanel.SetActive(false);

        }
    }
}