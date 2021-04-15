using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class BulletAwakeSystem : AwakeSystem<Bullet, int>
    {
        public override void Awake(Bullet self, int account)
        {
            self.Awake(account);
        }
    }

    public class Bullet : Entity
    {
        /// <summary>
        /// 这颗子弹是哪个账号发射的
        /// </summary>
        public int account;

        public float PositionX = 0.0f;
        public float PositionY = 0.0f;
        public float PositionZ = 0.0f;

        public float RotationX = 0;
        public float RotationY = 0;
        public float RotationZ = 0;
        public float RotationW = 1;

        public float VelocityX;
        public float VelocityY;
        public float VelocityZ;

        public void Awake(int account)
        {
            this.account = account;

        }



    }
}
