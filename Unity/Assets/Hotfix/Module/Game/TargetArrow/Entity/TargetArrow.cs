using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class TargetArrowAwakeSystem : AwakeSystem<TargetArrow, GameObject>
    {
        public override void Awake(TargetArrow self, GameObject gameObject)
        {
            self.Awake(gameObject);
        }
    }

    public class TargetArrow : Entity
    {
        /// <summary>
        /// 箭头模型的引用
        /// </summary>
        public GameObject targetArrow_GameObject;

        public void Awake(GameObject gameObject)
        {
            this.targetArrow_GameObject = gameObject;

        }
    }
}


