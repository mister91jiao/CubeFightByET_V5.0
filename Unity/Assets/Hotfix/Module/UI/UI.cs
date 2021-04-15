using System.Collections.Generic;
using ETModel;
using libx;
using UnityEngine;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiAwakeSystem : AwakeSystem<UI, string, GameObject, AssetRequest>
	{
		public override void Awake(UI self, string name, GameObject gameObject, AssetRequest assetRequest)
		{
			self.Awake(name, gameObject, assetRequest);
		}
	}
	
	[HideInHierarchy]
	public sealed class UI: Entity
	{
		public string Name { get; private set; }

		public Dictionary<string, UI> children = new Dictionary<string, UI>();

		/// <summary>
		/// 资源加载请求，用于卸载的时候调用
		/// </summary>
        public AssetRequest assetRequest = null;

		public void Awake(string name, GameObject gameObject, AssetRequest assetRequest)
		{
			this.children.Clear();
			gameObject.AddComponent<ComponentView>().Component = this;
			gameObject.layer = LayerMask.NameToLayer(LayerNames.UI);
			this.Name = name;
			this.GameObject = gameObject;
            this.assetRequest = assetRequest;
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
			
			base.Dispose();

			foreach (UI ui in this.children.Values)
			{
				ui.Dispose();
			}
			
			UnityEngine.Object.Destroy(GameObject);
			children.Clear();
		}

		public void SetAsFirstSibling()
		{
			this.GameObject.transform.SetAsFirstSibling();
		}

		public void Add(UI ui)
		{
			this.children.Add(ui.Name, ui);
			ui.Parent = this;
		}

		public void Remove(string name)
		{
			UI ui;
			if (!this.children.TryGetValue(name, out ui))
			{
				return;
			}
			this.children.Remove(name);
			ui.Dispose();
		}

		public UI Get(string name)
		{
			UI child;
			if (this.children.TryGetValue(name, out child))
			{
				return child;
			}
			GameObject childGameObject = this.GameObject.transform.Find(name)?.gameObject;
			if (childGameObject == null)
			{
				return null;
			}
			child = ComponentFactory.Create<UI, string, GameObject>(name, childGameObject);
			this.Add(child);
			return child;
		}
	}
}