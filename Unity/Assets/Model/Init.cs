using System;
using System.IO;
using System.Threading;
using libx;
using UnityEngine;

namespace ETModel
{
	public class Init : MonoBehaviour
    {
        public bool development = true;
        public bool updateUnusedAssetsImmediate = true;
		private void Start()
        {
			//设置目标帧
            //Application.targetFrameRate = 60;

			//开发模式
			Assets.development = development;
            //是否输出日志
            Assets.loggable = false;
            //开启后会进行整包资源更新
            Assets.updateAll = true;
			//验证算法
			Assets.verifyBy = VerifyBy.CRC;
			//更新的文件下载路径
            if (GloabConfigHelper.controllerType == ControllerType.PC)
            {
                Assets.updatePath = Application.dataPath + "/../updatePath/";
                if (!Directory.Exists(Assets.updatePath))
                {
                    Debug.Log("创建更新文件夹");
                    Directory.CreateDirectory(Assets.updatePath);
                }
                else
                {
                    Debug.Log("更新文件夹已经存在");
                }
			}

            
            //资源是否立即回收
            Assets.updateUnusedAssetsImmediate = updateUnusedAssetsImmediate;

            Assets.Initialize(InitError =>
            {
                if (!string.IsNullOrEmpty(InitError))
                {
                    Log.Error("初始化失败：" + InitError);
                    return;
                }
                else
                {
					this.StartAsync().Coroutine();

                }

            });
			
		}
		
		private async ETVoid StartAsync()
		{
			try
			{
				SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

				DontDestroyOnLoad(gameObject);
				Game.EventSystem.Add(DLLType.Model, typeof(Init).Assembly);

                Game.Scene.AddComponent<ResourcesAsyncComponent>();
				Game.Scene.AddComponent<TimerComponent>();
				Game.Scene.AddComponent<GlobalConfigComponent>();
				Game.Scene.AddComponent<NetOuterComponent>();
				Game.Scene.AddComponent<UIComponent>();


				if (!Assets.development)
                {
                    //下载地址
                    //Assets.downloadURL = @"http://192.168.50.2:9191/";
                    Assets.downloadURL = GlobalConfigComponent.Instance.GlobalProto.AssetBundleServerUrl;
                    //Assets.downloadURL = @"file://" + Application.dataPath + "/../../LoaclServer/";
					
					// 下载ab包
					await BundleHelper.DownloadBundle();
				}

				Game.Hotfix.LoadHotfixAssembly();

				// 加载配置
				//Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
				//Game.Scene.AddComponent<ConfigComponent>();
				//Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");

				Game.Scene.AddComponent<OpcodeTypeComponent>();
				Game.Scene.AddComponent<MessageDispatcherComponent>();

				Game.Hotfix.GotoHotfix();

			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		private void Update()
		{
			OneThreadSynchronizationContext.Instance.Update();
			Game.Hotfix.Update?.Invoke();
			Game.EventSystem.Update();
		}

		private void LateUpdate()
		{
			Game.Hotfix.LateUpdate?.Invoke();
			Game.EventSystem.LateUpdate();
		}

		private void OnApplicationQuit()
		{
			Game.Hotfix.OnApplicationQuit?.Invoke();
			Game.Close();
		}
	}
}