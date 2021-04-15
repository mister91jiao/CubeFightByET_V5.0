using System;
using System.Threading;
using ETModel;
using NLog;

namespace App
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			// 异步方法全部会回掉到主线程
			SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

			try
			{
				Game.EventSystem.Add(DLLType.Model, typeof(Game).Assembly);
				Game.EventSystem.Add(DLLType.Hotfix, DllHelper.GetHotfixAssembly());

				for (int i = 0; i < args.Length; i++)
				{
					Log.Info("启动信息：" + args[i]);
				}

				//初始化选项
				Options options = Game.Scene.AddComponent<OptionComponent, string[]>(args).Options;
				//Console.WriteLine("打印日志：" + options.Config + " || " + options.AppId);
				//得到初始化的配置
				StartConfig startConfig = Game.Scene.AddComponent<StartConfigComponent, string, int>(options.Config, options.AppId).StartConfig;

				if (!options.AppType.Is(startConfig.AppType))
				{
					Log.Error("命令行参数apptype与配置不一致");
					return;
				}

				IdGenerater.AppId = options.AppId;

				LogManager.Configuration.Variables["appType"] = $"{startConfig.AppType}";
				LogManager.Configuration.Variables["appId"] = $"{startConfig.AppId}";
				LogManager.Configuration.Variables["appTypeFormat"] = $"{startConfig.AppType,-8}";
				LogManager.Configuration.Variables["appIdFormat"] = $"{startConfig.AppId:0000}";

				Log.Info($"服务器运行开始........................ {startConfig.AppId} {startConfig.AppType}");

				//添加计时组件
				Game.Scene.AddComponent<TimerComponent>();
				//添加安全码，通过特性遍历获取的
				Game.Scene.AddComponent<OpcodeTypeComponent>();
				//添加消息分发组件
				Game.Scene.AddComponent<MessageDispatcherComponent>();

				//获取外网配置IP
				OuterConfig outerConfig = startConfig.GetComponent<OuterConfig>();
				//获取内外配置IP
				InnerConfig innerConfig = startConfig.GetComponent<InnerConfig>();
				//获取客户端链接地址IP
				ClientConfig clientConfig = startConfig.GetComponent<ClientConfig>();

				switch (startConfig.AppType)
				{

					case AppType.Manager:
						//这个组件用于启动其它服务器
						Game.Scene.AddComponent<AppManagerComponent>();

						Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
						Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
						Log.Info("Manager服务器启动");
						break;
					case AppType.Map:

						//协程锁组件
						Game.Scene.AddComponent<CoroutineLockComponent>();

						// 内网消息组件
						Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);

						// 发送普通actor消息
						Game.Scene.AddComponent<ActorMessageSenderComponent>();

						// 这两个组件是处理actor消息使用的
						Game.Scene.AddComponent<MailboxDispatcherComponent>();
						Game.Scene.AddComponent<ActorMessageDispatcherComponent>();

						//添加单位管理组件
						Game.Scene.AddComponent<UnitComponent>();

						//添加热更层的一些组件
                        Game.Scene.AddComponent<AddHotfixComponent>();

						//控制台组件
						Game.Scene.AddComponent<ConsoleComponent>();

						Log.Info("Map服务器启动");
						break;
					case AppType.DB:
						// 内网消息组件
						Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);

						//数据服务器需要的组件
						Game.Scene.AddComponent<DBComponent>();

						//注册数据序列化实体类
                        Game.Scene.AddComponent<RegisterMapComponent>();

						//控制台组件
						Game.Scene.AddComponent<ConsoleComponent>();

						Log.Info("数据库服务器启动");
						break;
					case AppType.Location:

						// 内网消息组件
						Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);

						//location server需要的组件
						Game.Scene.AddComponent<LocationComponent>();

						Log.Info("Location服务器启动");
						break;
					case AppType.Gate:

						//协程锁组件
						Game.Scene.AddComponent<CoroutineLockComponent>();

						// 内网消息组件
						Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);

						// 外网消息组件
						Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);

						// 发送普通actor消息
						Game.Scene.AddComponent<ActorMessageSenderComponent>();

						// 这两个组件是处理actor消息使用的
						Game.Scene.AddComponent<MailboxDispatcherComponent>();
						Game.Scene.AddComponent<ActorMessageDispatcherComponent>();

						//访问location server的组件
						Game.Scene.AddComponent<LocationProxyComponent>();

						//发送location actor消息
						Game.Scene.AddComponent<ActorLocationSenderComponent>();

						//操作数据库的组件
						Game.Scene.AddComponent<DBProxyComponent>();

						// 配置管理
						Game.Scene.AddComponent<ConfigComponent>();

						//控制台组件
						Game.Scene.AddComponent<ConsoleComponent>();

						//添加玩家管理组件
						Game.Scene.AddComponent<PlayerComponent>();

						Log.Info("Gate服务器启动");

						break;
					case AppType.AllServer:

						//协程锁组件
						Game.Scene.AddComponent<CoroutineLockComponent>();

						// 内网消息组件
						Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);

						// 外网消息组件
						Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);

						// 发送普通actor消息
						Game.Scene.AddComponent<ActorMessageSenderComponent>();

						// 这两个组件是处理actor消息使用的
						Game.Scene.AddComponent<MailboxDispatcherComponent>();
						Game.Scene.AddComponent<ActorMessageDispatcherComponent>();

						//location server需要的组件
						Game.Scene.AddComponent<LocationComponent>();

						//访问location server的组件
						Game.Scene.AddComponent<LocationProxyComponent>();

						//发送location actor消息
						Game.Scene.AddComponent<ActorLocationSenderComponent>();

						//数据服务器需要的组件
						Game.Scene.AddComponent<DBComponent>();

						//操作数据库的组件
						Game.Scene.AddComponent<DBProxyComponent>();

						// 配置管理
						Game.Scene.AddComponent<ConfigComponent>();

						//控制台组件
						Game.Scene.AddComponent<ConsoleComponent>();

						//添加单位管理组件
						Game.Scene.AddComponent<UnitComponent>();

						//添加玩家管理组件
						Game.Scene.AddComponent<PlayerComponent>();

						//添加热更层的一些组件
                        Game.Scene.AddComponent<AddHotfixComponent>();

						Log.Info("AllServer服务器启动");

						break;
					default:
						Log.Error("启动了没有定义的服务器：" + startConfig.AppType);
						Console.ReadKey();
						return;
				}

				//这里就开始循环监听信息了
				while (true)
				{
					try
					{
						Thread.Sleep(1);
						OneThreadSynchronizationContext.Instance.Update();
						Game.EventSystem.Update();
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}
	}
}
