using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiLobbyComponentSystem : AwakeSystem<UILobbyComponent>
	{
		public override void Awake(UILobbyComponent self)
		{
			self.Awake();
		}
	}
	
	public class UILobbyComponent : Component
	{
		private GameObject enterMap;

        private GameObject Account;
        private GameObject KillCount;
        private GameObject DeathCount;
        private GameObject MoneyCount;

		public void Awake()
		{
			ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
			
			enterMap = rc.Get<GameObject>("EnterMap");

			this.Account = rc.Get<GameObject>("AccountChild");
            this.KillCount = rc.Get<GameObject>("KillCountChild");
            this.DeathCount = rc.Get<GameObject>("DeathCountChild");
            this.MoneyCount = rc.Get<GameObject>("MoneyCountChild");

            GetPlayerInfo().Coroutine();

        }

		/// <summary>
		/// 得到玩家的所有信息
		/// </summary>
		/// <returns></returns>
        private async ETVoid GetPlayerInfo()
        {
            int account = Game.Scene.GetComponent<PlayerInfoComponent>().account;
			Account.GetComponent<Text>().text = account.ToString();

            G2C_GetPlayerInfo g2CGetPlayerInfo = (G2C_GetPlayerInfo)await Game.Scene.GetComponent<SessionComponent>().Session.Call(new C2G_GetPlayerInfo() { Account = account });

            KillCount.GetComponent<Text>().text = g2CGetPlayerInfo.KillCount.ToString();
            DeathCount.GetComponent<Text>().text = g2CGetPlayerInfo.DeathCount.ToString();
            MoneyCount.GetComponent<Text>().text = g2CGetPlayerInfo.MoneyCount.ToString();

			enterMap.GetComponent<Button>().onClick.Add(this.EnterMap);
		}

		private void EnterMap()
		{
			MapHelper.EnterMapAsync().Coroutine();
		}
		

	}
}
