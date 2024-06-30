using UnityEngine;

public class M98_Prologue : MonoBehaviour
{
	public UISprite s00_00;

	public UISprite s00_01;

	public UISprite s01_02;

	public UISprite s01_03;

	public UISprite s01_04_1;

	public UISprite s01_04_2;

	public UISprite s02_021;

	public UISprite s02_031;

	public UISprite s02_041;

	public UISprite s02_03;

	public UISprite s02_035;

	public UISprite s02_05;

	public UISprite s03_021;

	public UISprite s03_022;

	public UISprite s04_04;

	public UISprite s04_05;

	public UISprite s04_06;

	public UISprite s05_02;

	public UISprite s05_03;

	public UISprite s06_06;

	public UISprite s06_04;

	public UISprite s06_02;

	public UISprite s06_03;

	public UISprite s06_04_2;

	public UISprite s06_05;

	public UISprite s07_02;

	public UISprite s08_01;

	public UISprite s08_02;

	public UISprite s08_03;

	public UITexture s01_01;

	public UITexture s02_01;

	public UITexture s02_04;

	public UITexture s03_01;

	public UITexture s04_01;

	public UITexture s04_02;

	public UITexture s05_01;

	public UITexture s06_01;

	public UITexture s07_01;

	private void Start()
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		if (localUser.tutorialStep == 0)
		{
			Loading.Load(Loading.Tutorial);
			return;
		}
		GameObject gameObject = GameObject.Find("Loading");
		if (gameObject != null)
		{
			gameObject.GetComponent<UILoading>().Out();
		}
		Loading.Load(Loading.WorldMap);
	}

	private void OnDestroy()
	{
		s00_00 = null;
		s00_01 = null;
		s01_02 = null;
		s01_03 = null;
		s01_04_1 = null;
		s01_04_2 = null;
		s02_021 = null;
		s02_031 = null;
		s02_041 = null;
		s02_03 = null;
		s02_035 = null;
		s02_05 = null;
		s03_021 = null;
		s03_022 = null;
		s04_04 = null;
		s04_05 = null;
		s04_06 = null;
		s05_02 = null;
		s05_03 = null;
		s06_06 = null;
		s06_04 = null;
		s06_02 = null;
		s06_03 = null;
		s06_04_2 = null;
		s06_05 = null;
		s07_02 = null;
		s08_01 = null;
		s08_02 = null;
		s08_03 = null;
		if (s01_01 != null)
		{
			Object.DestroyImmediate(s01_01);
			s01_01 = null;
		}
		if (s02_01 != null)
		{
			Object.DestroyImmediate(s02_01);
			s02_01 = null;
		}
		if (s02_04 != null)
		{
			Object.DestroyImmediate(s02_04);
			s02_04 = null;
		}
		if (s03_01 != null)
		{
			Object.DestroyImmediate(s03_01);
			s03_01 = null;
		}
		if (s04_01 != null)
		{
			Object.DestroyImmediate(s04_01);
			s04_01 = null;
		}
		if (s04_02 != null)
		{
			Object.DestroyImmediate(s04_02);
			s04_02 = null;
		}
		if (s05_01 != null)
		{
			Object.DestroyImmediate(s05_01);
			s05_01 = null;
		}
		if (s06_01 != null)
		{
			Object.DestroyImmediate(s06_01);
			s06_01 = null;
		}
		if (s07_01 != null)
		{
			Object.DestroyImmediate(s07_01);
			s07_01 = null;
		}
	}
}
