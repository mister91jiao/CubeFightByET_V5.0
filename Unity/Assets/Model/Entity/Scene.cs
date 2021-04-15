namespace ETModel
{
	public static class SceneType
	{
		public const string Init = "Init";
        public const string Map = "Map";
        public const string Arena = "Arena";
	}
	
	public sealed class Scene: Entity
	{
		public string Name { get; set; }

		public Scene()
		{
		}

		public Scene(long id): base(id)
		{
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();
		}
	}
}