using ETModel;

namespace ETHotfix
{

    [ObjectSystem]
    public class AddHotfixComponentEXAwakeSystem : AwakeSystem<AddHotfixComponent>
    {
        public override void Awake(AddHotfixComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class AddHotfixComponentEXStartSystem : StartSystem<AddHotfixComponent>
    {
        public override void Start(AddHotfixComponent self)
        {
            self.Start();
        }
    }

    public static class AddHotfixComponentEX
    {

        public static void Awake(this AddHotfixComponent self)
        {
            Log.Info("添加热更层的Component Awake");

            //添加玩家网络同步组件
            Game.Scene.AddComponent<PlayerNetSyncComponent>();

            Game.Scene.AddComponent<BulletManagerComponent>();

        }

        public static void Start(this AddHotfixComponent self)
        {
            Log.Info("添加热更层的Component Start");

        }
    }
}
