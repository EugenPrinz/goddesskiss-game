using System.Collections;
using Facebook.Unity;
using UnityEngine;

public class M01_Title : MonoBehaviour
{
	public SpriteSheet sheet;

	public UI2DSprite targetSprite;

	public UIButton touchMark;

	public GameObject root_GameStart;

	public GameObject obj_Join;

	public GameObject obj_Login;

	public GameObject obj_Agreement;

	public UISelectSever obj_SelectSever;

	public UILabel serverName;

	public UILabel gameVersion;

	public UILabel dbVersion;

	private bool bLogin;

	private bool bRequestUserTerm;

	private UIQuitPopup quitPopup;

	public GameObject goTitleLogo;

	public GameObject goLoginBG;

	public GameObject goKorRating;

	public UIAtlas commanderAtlas;

	public UIAtlas commanderAtlas_2;

	public UIAtlas skillIconAtals;

	public UIAtlas unitAtlas;

	public UIAtlas battleCommanderUnitAtlas;

	public static string KEY => "TA6m";

	private IEnumerator Start()
	{
		if (PlayerPrefs.GetString("Language") == "S_Kr")
		{
			UISetter.SetActive(goKorRating, active: true);
		}
		else
		{
			UISetter.SetActive(goKorRating, active: false);
		}
		UISetter.SetLabel(gameVersion, Application.version);
		UISetter.SetLabel(dbVersion, PlayerPrefs.GetString("DBVersion", "0"));
		if (touchMark != null)
		{
			touchMark.gameObject.SetActive(value: false);
		}
		if (root_GameStart != null)
		{
			root_GameStart.gameObject.SetActive(value: false);
		}
		GameObject loading = GameObject.Find("Loading");
		if (loading != null)
		{
			loading.GetComponent<UILoading>().Out();
		}
		if (!AssetBundleManager.HasAssetBundle("CommanderAtlas.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("CommanderAtlas.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("CommanderAtlas_2.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("CommanderAtlas_2.assetbundle"));
		}
		if (AssetBundleManager.HasAssetBundle("CommanderAtlas.assetbundle"))
		{
			commanderAtlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("CommanderAtlas.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			commanderAtlas.spriteMaterial = Resources.Load<Material>("Atlas/CommanderAtlas");
		}
		if (AssetBundleManager.HasAssetBundle("CommanderAtlas_2.assetbundle"))
		{
			commanderAtlas_2.replacement = AssetBundleManager.GetObjectFromAssetBundle("CommanderAtlas_2.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			commanderAtlas_2.spriteMaterial = Resources.Load<Material>("Atlas/CommanderAtlas_2");
		}
		yield return null;
	}

	private void OnDestroy()
	{
		if (goTitleLogo != null)
		{
			goTitleLogo = null;
		}
		if (goLoginBG != null)
		{
			goLoginBG = null;
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			return;
		}
		if (FB.IsInitialized)
		{
			FB.ActivateApp();
			return;
		}
		FB.Init(delegate
		{
			FB.ActivateApp();
		});
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && ForwardBackKeyEvent.stack.Count == 0 && (obj_Login.activeSelf || obj_Agreement.activeSelf || (!obj_Login.activeSelf && !obj_Join.activeSelf && !obj_SelectSever.gameObject.activeSelf && !obj_Agreement.activeSelf)))
		{
			OnPressedQuitGame();
		}
		if (RemoteObjectManager.instance.localUser.bShowUserTerm)
		{
			if (!bRequestUserTerm)
			{
				RemoteObjectManager.instance.RequestUserTerm();
				if (obj_Join != null)
				{
					obj_Join.SetActive(value: false);
				}
				if (obj_Login != null)
				{
					obj_Login.SetActive(value: false);
				}
				bRequestUserTerm = true;
			}
		}
		else if (RemoteObjectManager.instance.bLogin && !bLogin)
		{
			if (touchMark != null)
			{
				touchMark.gameObject.SetActive(value: true);
			}
			if (root_GameStart != null)
			{
				root_GameStart.gameObject.SetActive(value: true);
				UISetter.SetLabel(serverName, Localization.Format("19067", RemoteObjectManager.instance.localUser.world));
			}
			if (obj_Join != null)
			{
				obj_Join.SetActive(value: false);
			}
			if (obj_Login != null)
			{
				obj_Login.SetActive(value: false);
			}
			bLogin = true;
		}
	}

	public void StartGame()
	{
		SoundManager.PlaySFX("BTN_Norma_001");
		RemoteObjectManager.instance.RequestLogin();
	}

	public void ServerChange()
	{
		SoundManager.PlaySFX("BTN_Norma_001");
		RemoteObjectManager.instance.RequestServerStatus();
	}

	public void ChangeAcount()
	{
		SoundManager.PlaySFX("BTN_Norma_001");
		if (bLogin)
		{
			RemoteObjectManager.instance.bLogin = false;
			if (touchMark != null)
			{
				touchMark.gameObject.SetActive(value: false);
			}
			if (root_GameStart != null)
			{
				root_GameStart.gameObject.SetActive(value: false);
				UISetter.SetLabel(serverName, Localization.Format("19067", RemoteObjectManager.instance.localUser.world));
			}
			if (obj_Login != null)
			{
				obj_Login.SetActive(value: true);
			}
			bLogin = false;
		}
	}

	public void DeleteDB()
	{
		SoundManager.PlaySFX("BTN_Norma_001");
		UISimplePopup.CreateBool(localization: true, "28156", "28157", null, "1001", "1000").onClick = delegate(GameObject obj)
		{
			if (obj.name == "OK")
			{
				PatchManager.Instance.RemoveAllDBFile();
				Application.Quit();
			}
		};
	}

	public void SetServerStatus(Protocols.ServerData serverData)
	{
		if (root_GameStart != null)
		{
			root_GameStart.gameObject.SetActive(value: false);
		}
		if (touchMark != null)
		{
			touchMark.gameObject.SetActive(value: false);
		}
		if (obj_SelectSever != null)
		{
			obj_SelectSever.Init(serverData);
		}
	}

	private IEnumerator PlayAnimation()
	{
		if (sheet == null)
		{
			yield break;
		}
		sheet.ClearMeshList();
		Sprite[] sprite = sheet.GetUnitySprites("Army");
		if (sprite.Length <= 0)
		{
			yield break;
		}
		int idx = 0;
		float interval = 1f / 30f;
		while (true)
		{
			targetSprite.sprite2D = sprite[idx];
			idx = (idx + 1) % sprite.Length;
			yield return new WaitForSeconds(interval);
		}
	}

	private void OnPressedQuitGame()
	{
		if (quitPopup == null)
		{
			quitPopup = UIPopup.Create<UIQuitPopup>("QuitPopup");
		}
	}
}
