using System;
using ETModel;
using libx;
using UnityEngine;

namespace ETHotfix
{
	public static class ConfigHelper
	{
		public static string GetText(string key)
		{
			try
			{
                AssetRequest assetRequest = Assets.LoadAsset("Assets/Bundles/Independent/config.prefab", typeof(GameObject));
                GameObject config = assetRequest.asset as GameObject;

				string configStr = config.Get<TextAsset>(key).text;
				return configStr;
			}
			catch (Exception e)
			{
				throw new Exception($"load config file fail, key: {key}", e);
			}
		}

		public static T ToObject<T>(string str)
		{
			return JsonHelper.FromJson<T>(str);
		}
	}
}