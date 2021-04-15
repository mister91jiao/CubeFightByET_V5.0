namespace ETModel
{
    [ObjectSystem]
    public class PlayerAwakeSystem : AwakeSystem<Player, int>
    {
        public override void Awake(Player self, int account)
        {
            self.Awake(account);
        }
    }

	public sealed class Player : Entity
    {
        public int Account { get; private set; }

        /// <summary>
        /// 客户端链接的Session
        /// </summary>
        public Session session;

        public MailBoxComponent mailBoxComponent;

        public long MapInstanceId;

        public void Awake(int account)
        {
            this.Account = account;
            //添加MailBoxComponent组件
            mailBoxComponent = this.AddComponent<MailBoxComponent>();

            //mailBoxComponent = this.AddComponent<MailBoxComponent, string>(MailboxType.GateSession);
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
