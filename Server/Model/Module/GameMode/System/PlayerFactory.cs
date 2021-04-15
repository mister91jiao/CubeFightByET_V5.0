using System;

namespace ETModel
{
    public static class PlayerFactory
    {
        public static Player Create(int account)
        {
            try
            {
                Player player = ComponentFactory.Create<Player, int>(account);


                return player;
            }
            catch (Exception e)
            {
                Log.Error("创建玩家失败：" + e);
                return null;
            }
        }

	}
}
