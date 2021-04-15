using System;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIPlayerInfoComponentSystem : AwakeSystem<UIPlayerInfoComponent>
    {
        public override void Awake(UIPlayerInfoComponent self)
        {
            self.Awake();
        }
    }

	public class UIPlayerInfoComponent : Component
    {
        /// <summary>
        /// 血条UI
        /// </summary>
        public Image Health;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Health = rc.Get<GameObject>("Image").GetComponent<Image>();

        }
    }
}