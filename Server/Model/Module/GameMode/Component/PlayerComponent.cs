using System;
using System.Collections.Generic;
using System.Net;
using ETHotfix;

namespace ETModel
{
    [ObjectSystem]
    public class PlayerComponentAwakeSystem : AwakeSystem<PlayerComponent>
    {
        public override void Awake(PlayerComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class PlayerComponentStartSystem : StartSystem<PlayerComponent>
    {
        public override void Start(PlayerComponent self)
        {
            self.Start();
        }
    }

    public class PlayerComponent: Component
    {

        private Dictionary<int, Player> AccountToPlayer = new Dictionary<int, Player>();

        public void Awake()
        {
            Log.Info("玩家管理组件初始化");
        }

        public void Start()
        {
            playerOnlineTest(3000).Coroutine();
        }

        /// <summary>
        /// 添加玩家实体到字典管理
        /// </summary>
        public void addPlayerToDict(int account, Player player)
        {
            if (!AccountToPlayer.ContainsKey(account))
            {
                AccountToPlayer.Add(account, player);
            }
            else
            {
                Log.Error("相同账号重复添加：" + account);
            }

        }

        /// <summary>
        /// 玩家重复登录
        /// </summary>
        public async ETVoid PlayerBackLogin(int account)
        {
            if (AccountToPlayer.ContainsKey(account))
            {
                //获取内网发送组件
                IPEndPoint mapAddress = StartConfigComponent.Instance.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
                Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapAddress);

                //给Map服发送移除unit的信息, 并收到需要广播的player的账号
                M2G_RemoveUnitByMap m2GRemoveUnitByMap = (M2G_RemoveUnitByMap)await mapSession.Call(new G2M_RemoveUnitByMap() { Account = account });

                if (m2GRemoveUnitByMap.Accounts.Count > 0)
                {
                    for (int i = 0; i < m2GRemoveUnitByMap.Accounts.Count; i++)
                    {
                        if (AccountToPlayer.TryGetValue(m2GRemoveUnitByMap.Accounts[i], out Player player))
                        {
                            player.session.Send(new G2C_PlayerDisCatenate()
                            {
                                Account = account
                            });
                        }
                        else
                        {
                            Log.Error("所有player找不到这个需要发送其它玩家的断线信息的玩家");
                        }
                    }
                }

            }
            else
            {
                Log.Error("这个玩家本来就不存在: " + account);
            }
        }

        /// <summary>
        /// 检测玩家是否在线
        /// </summary>
        public async ETVoid playerOnlineTest(long time)
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();
            while (true)
            {
                await timerComponent.WaitAsync(time);
                foreach (Player player in AccountToPlayer.Values)
                {
                    try
                    {
                        player.session.Send(new G2C_PlayerPlaying());
                    }
                    catch (Exception e)
                    {
                        //给其它玩家广播这个玩家掉线的信息
                        removeOnePlayerLink(player.Account).Coroutine();
                        Log.Error("一名玩家无应答离线了: " + player.Account);
                    }
                }
            }

        }

        /// <summary>
        /// 断开一颗玩家的链接，包括它在Map服中的实体
        /// </summary>
        /// <param name="account"></param>
        public async ETVoid removeOnePlayerLink(int account)
        {
            if (AccountToPlayer.ContainsKey(account))
            {
                AccountToPlayer.Remove(account);

                //获取内网发送组件
                IPEndPoint mapAddress = StartConfigComponent.Instance.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
                Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapAddress);

                //给Map服发送移除unit的信息, 并收到需要广播的player的账号
                M2G_RemoveUnitByMap m2GRemoveUnitByMap = (M2G_RemoveUnitByMap) await mapSession.Call(new G2M_RemoveUnitByMap(){ Account = account });

                if (m2GRemoveUnitByMap.Accounts.Count > 0)
                {
                    for (int i = 0; i < m2GRemoveUnitByMap.Accounts.Count; i++)
                    {
                        if (AccountToPlayer.TryGetValue(m2GRemoveUnitByMap.Accounts[i], out Player player))
                        {
                            player.session.Send(new G2C_PlayerDisCatenate()
                            {
                                Account = account
                            });
                        }
                        else
                        {
                            Log.Error("所有player找不到这个需要发送其它玩家的断线信息的玩家");
                        }
                    }
                }

            }
            else
            {
                Log.Error("这个玩家本来就不存在: " + account);
            }
        }

        /// <summary>
        /// 通过账号来获取玩家
        /// </summary>
        public Player getPlayerByAccount(int account)
        {
            if (AccountToPlayer.TryGetValue(account, out Player player))
            {
                return player;
            }
            else
            {
                Log.Error("错误，没有获取到玩家：" + account);
                return null;
            }
        }

        /// <summary>
        /// 忽略自己的account获取到其它玩家的account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public List<Player> getOtherPlayerIgnoreAccount(int account)
        {
            if (AccountToPlayer.Count > 1)
            {
                List<Player> data = new List<Player>();
                foreach (int otherAccount in AccountToPlayer.Keys)
                {
                    if (account != otherAccount)
                    {
                        data.Add(AccountToPlayer[otherAccount]);
                    }
                }
                return data;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 账号已经被创建过了
        /// </summary>
        public bool AccountHaveBeCreated(int account)
        {
            return AccountToPlayer.ContainsKey(account);
        }
    }
}

