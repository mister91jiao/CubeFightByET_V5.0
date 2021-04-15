using ETModel;
using UnityEngine;

namespace ETHotfix
{

    [ObjectSystem]
    public class OtherCubeAwakeSystem : AwakeSystem<OtherCube, int, GameObject, GameObject>
    {
        public override void Awake(OtherCube self, int account, GameObject gameObject, GameObject gameObjectDir)
        {
            self.Awake(account, gameObject, gameObjectDir);
        }
    }

    public sealed class OtherCube : Entity
    {
        /// <summary>
        /// 这个cube角色是哪个账号的
        /// </summary>
        public int account;

        /// <summary>
        /// 这个cube是否死亡
        /// </summary>
        public bool Die = false;

        /// <summary>
        /// cube角色的Gameobject
        /// </summary>
        public GameObject otherCube_GameObject;

        /// <summary>
        /// cube角色的同步目标Gameobject
        /// </summary>
        public GameObject otherDirCube_GameObject;

        public void Awake(int account, GameObject gameObject, GameObject otherDirCube)
        {
            this.account = account;
            this.otherCube_GameObject = gameObject;
            this.otherDirCube_GameObject = otherDirCube;

            gameObject.transform.parent = this.GameObject.transform;
            otherDirCube.transform.parent = this.GameObject.transform;
        }


        
    }
}