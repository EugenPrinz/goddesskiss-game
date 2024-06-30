public static class ConstValue
{
	public static class Prefix
	{
		public static readonly string regulationPath = "Regulation/";

		public static readonly string spriteThumbnailPrefix;
	}

	public static readonly string defaultLanguage = "S_Kr";

	public static readonly int pubCommanderMaxCount = 4;

	public static readonly float battleTerrainScrollSpeed = -1.4f;

	public static readonly int duelListRefreshCost = 10;

	public static readonly int tutorialMaximumStage = 3;

	public static readonly int tutorialEndStep = 12;

	public static readonly int fireEffectSortingOrder = 2000;

	public static readonly int globalTimeGroupID = 1;

	public static readonly int cutInTimeGroupID = 2;

	public static readonly int chatLimitCount = 30;

	public static readonly int waveDuelTroopCount = 3;

	public static readonly int eventBattleSaveDeckCount = 20;

	public static readonly int commanderMaxRank = 6;

	public static readonly int maxConnectTime = 7200;

	public static readonly string AtlasPath = "Atlas/";

	public static int NpcStartUno => int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["NPC_START_UNO"].value);
}
