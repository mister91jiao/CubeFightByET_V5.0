using System.Collections;
using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class PlayerInfoComponentAwakeSystem : AwakeSystem<PlayerInfoComponent, int>
    {
        public override void Awake(PlayerInfoComponent self, int account)
        {
            self.Awake(account);
        }
    }

    public class PlayerInfoComponent : Component
    {
        public int account;

        public void Awake(int account)
        {
            Log.Info("玩家信息管理组件初始化");
            this.account = account;
        }
        
    }
}