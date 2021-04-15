using System;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiLoginComponentSystem : AwakeSystem<UILoginComponent>
	{
		public override void Awake(UILoginComponent self)
		{
			self.Awake();
		}
	}
	
	public class UILoginComponent: Component
	{
		private GameObject loginPanel;
		private GameObject account;
        private GameObject password;
		private GameObject loginBtn;
		private GameObject goRegBtn;
        private GameObject loginFail;

		private GameObject regPanel;
        private GameObject regAccount;
        private GameObject regPassword;
        private GameObject regBtn;
        private GameObject goLoginBtn;
        private GameObject regFail;

        //用于发包的Session
        private Session loginSession;

        private GameObject failPanel;

        public void Awake()
		{
			ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

			//注册面板查找
            this.loginPanel = rc.Get<GameObject>("Panel");
            this.account = rc.Get<GameObject>("Account");
            this.password = rc.Get<GameObject>("Password");
			this.loginBtn = rc.Get<GameObject>("LoginBtn");
            this.goRegBtn = rc.Get<GameObject>("GoRegBtn");
            this.loginFail = rc.Get<GameObject>("LoginFail");

            this.regPanel = rc.Get<GameObject>("RegPanel");
            this.regAccount = rc.Get<GameObject>("RegAccount");
            this.regPassword = rc.Get<GameObject>("RegPassword");
            this.regBtn = rc.Get<GameObject>("RegBtn");
            this.goLoginBtn = rc.Get<GameObject>("GoLoginBtn");
            this.regFail = rc.Get<GameObject>("RegFail");

            this.failPanel = rc.Get<GameObject>("FailPanel");

            Log.Info("登录的服务器地址：" + GlobalConfigComponent.Instance.GlobalProto.Address);

            // 创建一个ETModel层的Session
            ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>()
                .Create(GlobalConfigComponent.Instance.GlobalProto.Address);
            ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>().Session = session;
            // 创建一个ETHotfix层的Session, ETHotfix的Session会通过ETModel层的Session发送消息
            Session hotfixSession = ComponentFactory.Create<Session, ETModel.Session>(session);
            Game.Scene.AddComponent<SessionComponent>().Session = hotfixSession;

            loginSession = Game.Scene.GetComponent<SessionComponent>().Session;

            failPanel.SetActive(false);

            GoLogin();

            //注册登录按钮
            loginBtn.GetComponent<Button>().onClick.Add(() =>
            {
                OnLogin().Coroutine();
            });

            //注册打开注册面板按钮
            goRegBtn.GetComponent<Button>().onClick.Add(GoReg);

            //注册注册按钮
            regBtn.GetComponent<Button>().onClick.Add(() =>
            {
                OnReg().Coroutine();
            });

            //注册打开登录面板按钮
            goLoginBtn.GetComponent<Button>().onClick.Add(GoLogin);

        }

        /// <summary>
        /// 打开登录面板
        /// </summary>
        private void GoLogin()
        {
            loginFail.SetActive(false);
            regFail.SetActive(false);
            regPanel.SetActive(false);
            loginPanel.SetActive(true);
        }

        private async ETVoid OnReg()
        {
            //获取账号
            if (!int.TryParse(this.regAccount.GetComponent<InputField>().text, out int account))
            {
                Log.Error("错误，账号不是数字");
                regFail.SetActive(true);
                return;
            }
            string password = this.regPassword.GetComponent<InputField>().text;

            //发送请求注册的包
            try
            {
                G2C_Reg g2CReg = (G2C_Reg)await loginSession.Call(new C2G_Reg() { Account = account, Password = password });

                if (g2CReg.RegFail)
                {
                    //注册成功
                    loginFail.SetActive(false);
                    GoLogin();
                }
                else
                {
                    loginFail.SetActive(true);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("链接服务器失败: " + e);

                ServerFail();

            }
        }

        /// <summary>
        /// 打开注册面板
        /// </summary>
        private void GoReg()
        {
            loginFail.SetActive(false);
            loginPanel.SetActive(false);
            regPanel.SetActive(true);
            regFail.SetActive(false);
        }

        /// <summary>
        /// 等待按钮回包
        /// </summary>
        private bool waitPackeg = false;

        /// <summary>
        /// 登录按钮
        /// </summary>
		private async ETVoid OnLogin()
        {
            //获取账号
            if (!int.TryParse(this.account.GetComponent<InputField>().text, out int account))
            {
                Log.Error("错误，账号不是数字");
                loginFail.SetActive(true);
                return;
            }
            string password = this.password.GetComponent<InputField>().text;


            if (waitPackeg)
                return;
            waitPackeg = true;
            try
            {
                //发送请求登录的包
                G2C_Login g2CLogin = (G2C_Login)await loginSession.Call(new C2G_Login() { Account = account, Password = password });

                if (g2CLogin.LoginFail)
                {
                    //登录成功
                    loginFail.SetActive(false);
                    LoginHelper.OnLoginAsync(account);
                }
                else
                {
                    loginFail.SetActive(true);
                    waitPackeg = false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("链接服务器失败: " + e);

                ServerFail();

            }

        }

        /// <summary>
        /// 链接服务器失败
        /// </summary>
        private void ServerFail()
        {
            failPanel.SetActive(true);
            loginPanel.SetActive(false);
            regPanel.SetActive(false);
        }
	}
}
