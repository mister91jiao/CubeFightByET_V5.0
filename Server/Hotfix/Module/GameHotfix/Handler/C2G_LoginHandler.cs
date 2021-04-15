using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_LoginHandler : AMRpcHandler<C2G_Login, G2C_Login>
    {
        protected override async ETTask Run(Session session, C2G_Login request, G2C_Login response, Action reply)
        {
            Log.Info("玩家请求登录：" + request.Account + "玩家输入密码：" + request.Password);

            DBProxyComponent dBProxy = Game.Scene.GetComponent<DBProxyComponent>();

            List<ComponentWithId> count = await dBProxy.Query<PlayerInfoDB>(PlayerInfoDB => PlayerInfoDB.account == request.Account);
            if (count.Count > 0)
            {
                if (count.Count == 1)
                {
                    //取得该条数据
                    PlayerInfoDB info = await dBProxy.Query<PlayerInfoDB>(count[0].Id);
                    //验证密码
                    if (info.pwd == request.Password)
                    {
                        Log.Info("密码正确，允许登录");

                        response.LoginFail = true;

                        PlayerComponent playerComponent = Game.Scene.GetComponent<PlayerComponent>();

                        Player loginPlayer;
                        //查看玩家是否已经登录创建过
                        if (playerComponent.AccountHaveBeCreated(request.Account))
                        {
                            Log.Info("玩家被顶号: " + request.Account);

                            //获取之前已经创建好的Player实体
                            loginPlayer = playerComponent.getPlayerByAccount(request.Account);

                            try
                            {
                                //给被顶号的人发送被顶号的信息
                                loginPlayer.session.Send(new G2C_PlayerBackLogin()
                                {
                                    NetMessage = "此账号在其它地方被登录"
                                });
                            }
                            catch
                            {
                                Log.Info("发送顶号信息失败：" + request.Account);
                            }

                            //给其它玩家广播这个玩家掉线的信息
                            playerComponent.PlayerBackLogin(request.Account).Coroutine();
                        }
                        else
                        {
                            //创建登录玩家的实体
                            loginPlayer = PlayerFactory.Create(request.Account);
                            //向玩家管理组件里添加玩家的信息
                            playerComponent.addPlayerToDict(request.Account, loginPlayer);

                        }

                        //对玩家的session进行记录
                        loginPlayer.session = session;
                        session.AddComponent<SessionPlayerComponent>().Player = loginPlayer;
                    }
                    else
                    {
                        Log.Info("密码错误");

                        response.LoginFail = false;
                    }
                }
                else
                {
                    Log.Error("账号重复了: " + count.Count);
                    response.LoginFail = false;
                }
            }
            else
            {

                response.LoginFail = false;
            }


            reply();
            await ETTask.CompletedTask;
        }
    }
}