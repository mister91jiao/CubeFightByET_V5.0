using System.Collections;
using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class HurtSyncComponentAwakeSystem : AwakeSystem<HurtSyncComponent>
    {
        public override void Awake(HurtSyncComponent self)
        {
            self.Awake();
        }
    }

    /// <summary>
    /// 攻击造成的伤害同步脚本
    /// </summary>
    public class HurtSyncComponent : Component
    {
        private Session hotfixSession;

        private int selfAccount;

        public void Awake()
        {
            Log.Info("攻击造成的伤害同步脚本初始化");

            hotfixSession = Game.Scene.GetComponent<SessionComponent>().Session;

            selfAccount = Game.Scene.GetComponent<PlayerInfoComponent>().account;

        }

        /// <summary>
        /// 攻击了某个玩家发送网络包
        /// </summary>
        public void HitToAccount(int account, int hurtHealth)
        {
            hotfixSession.Send(new C2G_PlayerHitOtherPlayer()
            {
                SelfAccount = selfAccount,
                HitPlayerAccount = account,
                SubHealth = hurtHealth
            });
        }
    }

}