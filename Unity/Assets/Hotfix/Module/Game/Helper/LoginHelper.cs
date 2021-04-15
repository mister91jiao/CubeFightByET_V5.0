using System;
using ETModel;

namespace ETHotfix
{
    public static class LoginHelper
    {
        public static void OnLoginAsync(int account)
        {
            Game.EventSystem.Run(EventIdType.LoginFinish);

            //添加玩家信息管理组件
            Game.Scene.AddComponent<PlayerInfoComponent, int>(account);

            //添加其它cube玩家的管理组件
            Game.Scene.AddComponent<OtherCubeManagerComponent>();

            //创建大厅UI
            UI ui = UILobbyFactory.Create();
            Game.Scene.GetComponent<UIComponent>().Add(ui);
        }

    }
}