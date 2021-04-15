namespace ETModel
{
    [ObjectSystem]
    public class UnitAwakeSystem : AwakeSystem<Unit, int>
    {
        public override void Awake(Unit self, int account)
        {
            self.Awake(account);
        }
    }

    public sealed class Unit : Entity
    {
        public int Account { get; private set; }

        /// <summary>
        /// 血量
        /// </summary>
        private int Health = 100;

        /// <summary>
        /// 玩家是否死亡
        /// </summary>
        public bool Die = false;

        public MailBoxComponent mailBoxComponent;

        public long GateInstanceId;

        public float InitPositionX = 0.0f;
        public float InitPositionY = 0.0f;
        public float InitPositionZ = 0.0f;

        public float RotationX = 0;
        public float RotationY = 0;
        public float RotationZ = 0;
        public float RotationW = 1;

        public float VelocityX;
        public float VelocityY;
        public float VelocityZ;

        /// <summary>
        /// 是否在开火
        /// </summary>
        public bool Fire = false;

        public void Awake(int account)
        {
            this.Account = account;
            //添加MailBoxComponent组件
            mailBoxComponent = this.AddComponent<MailBoxComponent>();

            //mailBoxComponent = this.AddComponent<MailBoxComponent, string>(MailboxType.GateSession);

        }

        /// <summary>
        /// 扣血
        /// </summary>
        /// <param name="health">需要扣的血</param>
        /// <returns>扣完后剩余血量</returns>
        public int SubHealth(int health)
        {
            Health -= health;
            if (Health <= 0)
            {
                Health = 0;
                Die = true;
            }

            return Health;
        }

        /// <summary>
        /// 恢复血量
        /// </summary>
        public void ReviveHealth()
        {
            Health = 100;
            Die = false;
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
        }
    }
}
