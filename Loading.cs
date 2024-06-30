using UnityEngine;

public static class Loading
{
	public static readonly string Init = "M00_Init";

	public static readonly string Title = "M01_Title";

	public static readonly string WorldMap = "M02_WorldMap";

	public static readonly string Camp = "M03_Camp";

	public static readonly string Battle = "M03_Battle";

	public static readonly string BattleTest = "T04_BattleTest";

	public static readonly string Tutorial = "M04_Tutorial";

	public static readonly string Scenario = "M05_Scenario";

	public static readonly string Dormitory = "M06_Dormitory";

	public static readonly string Prologue = "M98_Prologue";

	public static readonly string EditProjectileEffect = "T03_ProjectileEffect";

	public static void Load(string nextSceneName = null)
	{
		UIManager.instance.Release();
		string text = nextSceneName;
		if (string.IsNullOrEmpty(text))
		{
			text = Application.loadedLevelName;
		}
		M99_Loading.Load(text);
	}
}
