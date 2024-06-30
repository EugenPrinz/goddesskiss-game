using System.Collections;
using System.Collections.Generic;
using Cache;
using Facebook.Unity;
using Shared.Regulation;
using UnityEngine;
using Util;

public class M00_Init : MonoBehaviour
{
	private enum CHECK_PERMISSION_RESULT
	{
		OK,
		DENIED,
		NEVER_ASK_AGAIN
	}

	public GameObject PermissionCheck;

	public GameObject SelectLanguage;

	public GameObject SelectChannel;

	public GameObject Pathch;

	public GameObject logotexture;

	public GameObject patchTexture;

	public GameObject googleConnect;

	private Dictionary<string, Protocols.ChannelData> dicChannel;

	public UIDefaultListView channelListView;

	public UILabel tipLabel;

	public UILabel label;

	public UIProgressBar progressBar;

	public UITexture m_Fade;

	public float m_fDuration = 1f;

	private AudioSource ostAudiosource;

	private int touchCount;

	private UISimplePopup quitPopup;

	private UISimplePopup permissionPopup;

	private bool successPermission;

	private AndroidJavaClass _class;

	public static string KEY => "W-$&";

	private AndroidJavaObject instance => _class.GetStatic<AndroidJavaObject>("permissionPluginInstance");

	private void Awake()
	{
		SoundManager.Instance.showDebug = false;
		if (FB.IsInitialized)
		{
			FB.ActivateApp();
		}
		else
		{
			FB.Init(delegate
			{
				FB.ActivateApp();
			});
		}
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Screen.sleepTimeout = -1;
		}
		SA_Singleton<GoogleCloudMessageService>.instance.RgisterDevice();
	}

	private IEnumerator Start()
	{
		if (string.IsNullOrEmpty(PlayerPrefs.GetString("Language")))
		{
			switch (Application.systemLanguage)
			{
			case SystemLanguage.ChineseTraditional:
				PlayerPrefs.SetString("Language", "S_Beon");
				break;
			case SystemLanguage.ChineseSimplified:
				PlayerPrefs.SetString("Language", "S_Gan");
				break;
			case SystemLanguage.German:
				PlayerPrefs.SetString("Language", "S_Deu");
				break;
			case SystemLanguage.Spanish:
				PlayerPrefs.SetString("Language", "S_Esp");
				break;
			case SystemLanguage.Indonesian:
				PlayerPrefs.SetString("Language", "S_Idn");
				break;
			case SystemLanguage.Thai:
				PlayerPrefs.SetString("Language", "S_Tha");
				break;
			case SystemLanguage.French:
				PlayerPrefs.SetString("Language", "S_Fr");
				break;
			case SystemLanguage.Korean:
				PlayerPrefs.SetString("Language", "S_Kr");
				break;
			case SystemLanguage.Russian:
				PlayerPrefs.SetString("Language", "S_Rus");
				break;
			default:
				PlayerPrefs.SetString("Language", "S_En");
				break;
			}
		}
		PlayerPrefs.SetString("PermissionCheck", "true");
		yield return null;
		Setup();
		while (!successPermission)
		{
			CheckPermissionProcess();
			yield return null;
		}
		Input.multiTouchEnabled = false;
		GooglePlayConnection.ActionConnectionResultReceived += ActionConnectionResultReceived;
		GameCenterManager.OnAuthFinished += OnAuthFinished;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Application.targetFrameRate = 30;
		}
		LoggerHelper.CurrentLogLevels = LogLevel.CRITICAL;
		RemoteObjectManager.instance.regulation = Regulation.Create();
		m_Fade.alpha = 0f;
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", m_fDuration, "onupdate", "SetAlpha", "oncomplete", "OpenEnd", "oncompletetarget", base.gameObject));
		AdjustManager.Instance.SimpleEvent("9hoylh");
	}

	private void OpenEnd()
	{
		RemoteObjectManager.instance.RequestGetRegion();
	}

	private void ActionConnectionResultReceived(GooglePlayConnectionResult result)
	{
		if (result.IsSuccess)
		{
		}
		UISetter.SetActive(googleConnect, active: false);
	}

	private void OnAuthFinished(ISN_Result res)
	{
		if (!res.IsSucceeded)
		{
			IOSNativePopUpManager.showMessage("Game Center ", "Player auth failed");
		}
	}

	public void SelectLangeAndVersinonCheck(Dictionary<string, Protocols.ChannelData> channel)
	{
		dicChannel = channel;
		if (string.IsNullOrEmpty(PlayerPrefs.GetString("Language")))
		{
			SelectLanguage.SetActive(value: true);
		}
		else if (string.IsNullOrEmpty(PlayerPrefs.GetString("PermissionCheck")))
		{
			PermissionCheck.SetActive(value: true);
		}
		else if (!PlayerPrefs.HasKey("Channel"))
		{
			channelListView.InitChannel(dicChannel, "Channel-");
			SelectChannel.SetActive(value: true);
		}
		else
		{
			StartCoroutine(RemoteObjectManager.instance.GameVersionInfo());
		}
	}

	public void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "OK")
		{
			instance.Call("ShowApplicationSettingWindow");
		}
		else if (text == "Cancel")
		{
			GameQuit();
		}
		else if (text.StartsWith("Lang-"))
		{
			string text2 = text.Substring(text.IndexOf("-") + 1);
			PlayerPrefs.SetString("Language", text2);
			Localization.language = text2;
			SelectLanguage.SetActive(value: false);
			AdjustManager.Instance.SimpleEvent("2qjhq6");
			if (string.IsNullOrEmpty(PlayerPrefs.GetString("PermissionCheck")))
			{
				PermissionCheck.SetActive(value: true);
			}
			else if (!PlayerPrefs.HasKey("Channel"))
			{
				channelListView.InitChannel(dicChannel, "Channel-");
				SelectChannel.SetActive(value: true);
			}
			else
			{
				StartCoroutine(RemoteObjectManager.instance.GameVersionInfo());
			}
		}
		else if (text.StartsWith("Channel-"))
		{
			int value = int.Parse(text.Substring(text.IndexOf("-") + 1));
			PlayerPrefs.SetInt("Channel", value);
			SelectChannel.SetActive(value: false);
			if (GameObject.Find("PatchManager") != null)
			{
				Object.Destroy(GameObject.Find("PatchManager"));
			}
			AdjustManager.Instance.SimpleEvent("c1vgff");
			StartCoroutine(RemoteObjectManager.instance.GameVersionInfo());
		}
		else if (text == "Background")
		{
			ShowTip(first: false);
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus && !successPermission)
		{
			UISetter.SetActive(PermissionCheck, active: false);
			CheckPermissionProcess();
		}
	}

	private void SetAlpha(float value)
	{
		m_Fade.alpha = value;
	}

	private IEnumerator FadeOut()
	{
		TweenAlpha.Begin(m_Fade.gameObject, m_fDuration, 0f);
		yield return new WaitForSeconds(m_fDuration);
		TweenAlpha.Begin(m_Fade.gameObject, m_fDuration, 1f);
		yield return new WaitForSeconds(m_fDuration);
	}

	public void OnPatchStart()
	{
		if (GameSetting.instance.bgm)
		{
			string path = ((!(Localization.language == "S_Kr")) ? "OST/BGM_OST_EN" : "OST/BGM_OST_KR");
			AudioClip clip = Resources.Load(path) as AudioClip;
			ostAudiosource = SoundManager.PlaySFX(clip, looping: true);
			if (ostAudiosource != null)
			{
				if (PlayerPrefs.HasKey("bgm_Volume"))
				{
					ostAudiosource.volume = PlayerPrefs.GetFloat("bgm_Volume");
				}
				else
				{
					ostAudiosource.volume = 1f;
				}
			}
		}
		StartCoroutine(PatchStart());
	}

	private IEnumerator PatchStart()
	{
		yield return null;
		bool bStopCoroutine = false;
		if (RemoteObjectManager.instance.localUser.gameVersionState > 0)
		{
			bStopCoroutine = true;
			UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: true, "19015", "19016", null, "1001", "1000");
			uISimplePopup.onClick = delegate(GameObject go)
			{
				string text = go.name;
				if (text == "OK")
				{
					if (RemoteObjectManager.instance.localUser.gameVersionState == 1)
					{
						bStopCoroutine = false;
					}
					else
					{
						Application.OpenURL("https://play.google.com/store/apps/details?id=com.flerogames.GK");
					}
				}
			};
			uISimplePopup.onClose = delegate
			{
				if (bStopCoroutine)
				{
					Application.Quit();
				}
			};
		}
		while (bStopCoroutine)
		{
			yield return null;
		}
		if (GooglePlayConnection.State != GPConnectionState.STATE_CONNECTED)
		{
			UISetter.SetActive(googleConnect, active: true);
			SA_Singleton<GooglePlayConnection>.Instance.Connect();
		}
		ShowTip(first: true);
		Pathch.SetActive(value: true);
		label.text = string.Empty;
		yield return StartCoroutine(PatchManager.Instance.FileDownLoad(label, progressBar, bUpdate: true));
		PlayerPrefs.SetString("DBVersion", RemoteObjectManager.instance.localUser.serverDBVersion.ToString());
		Localization.localizationHasBeenSet = false;
		yield return StartCoroutine(PatchManager.Instance.RunBadWordPatch());
		Pathch.SetActive(value: true);
		yield return StartCoroutine(PatchManager.Instance.RunPatch(label, progressBar));
		if (ostAudiosource != null)
		{
			ostAudiosource.Stop();
		}
		CacheManager.instance.RemoveSoundCache();
		yield return null;
		GameSetting setting = GameSetting.instance;
		CacheManager.instance.SoundPocketCache.Create("Pocket_BGM_Title");
		SoundPlayer.SetBGMVolume((!setting.bgm) ? 0f : PlayerPrefs.GetFloat("bgm_Volume", 1f));
		SoundManager.SetVolumeMusic((!setting.bgm) ? 0f : PlayerPrefs.GetFloat("bgm_Volume", 1f));
		Application.LoadLevel(Loading.Title);
	}

	private IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		if (www.error != null)
		{
		}
	}

	public void test(string path)
	{
		StartCoroutine(FileUpload(path));
	}

	public IEnumerator FileUpload(string path)
	{
		string strUrl = "http://118.36.245.241:9000/crash/index.php";
		WWW localFile = new WWW("file:///" + path);
		yield return localFile;
		if (localFile.error == null)
		{
			WWWForm postForm = new WWWForm();
			postForm.AddBinaryData("elog", localFile.bytes, path, "text/plain");
			WWW upload = new WWW(strUrl, postForm);
			yield return upload;
			if (upload.error != null)
			{
			}
		}
	}

	private void OnDestroy()
	{
		if (logotexture != null)
		{
			logotexture = null;
		}
		if (patchTexture != null)
		{
			patchTexture = null;
		}
		if (!SA_Singleton<GooglePlayConnection>.IsDestroyed)
		{
			GooglePlayConnection.ActionConnectionResultReceived -= ActionConnectionResultReceived;
		}
		GameCenterManager.OnAuthFinished -= OnAuthFinished;
	}

	private void Update()
	{
		if ((!SelectLanguage.activeSelf && !SelectChannel.activeSelf) || ForwardBackKeyEvent.stack.Count != 0 || !Input.GetKeyDown(KeyCode.Escape) || !(quitPopup == null))
		{
			return;
		}
		quitPopup = UISimplePopup.CreateBool(localization: true, "5133", "5689", null, "1001", "1000");
		quitPopup.onClick = delegate(GameObject popupSender)
		{
			string text = popupSender.name;
			if (text == "OK")
			{
				GameQuit();
			}
		};
	}

	private void GameQuit()
	{
		Application.Quit();
	}

	private void ShowTip(bool first)
	{
		if (first)
		{
			touchCount = Random.Range(0, 9);
		}
		else
		{
			touchCount++;
		}
		if (touchCount > 8)
		{
			touchCount = 0;
		}
		UISetter.SetLabel(tipLabel, Localization.Get((1501 + touchCount).ToString()).Replace('\n', ' '));
	}

	private void CheckPermissionProcess()
	{
		switch ((CHECK_PERMISSION_RESULT)instance.Call<int>("CheckPermission", new object[0]))
		{
		case CHECK_PERMISSION_RESULT.OK:
			successPermission = true;
			UISetter.SetActive(PermissionCheck, active: false);
			break;
		case CHECK_PERMISSION_RESULT.DENIED:
			if (permissionPopup == null)
			{
				permissionPopup = UISimplePopup.CreateOK("Single_Ex").Set(localization: false, Localization.Get("1310"), Localization.Get("7906"), null, Localization.Get("1001"), null, null);
				if (permissionPopup != null)
				{
					permissionPopup.onClose = delegate
					{
						instance.Call("ShowPermissionPopup");
					};
				}
			}
			UISetter.SetActive(PermissionCheck, active: false);
			break;
		case CHECK_PERMISSION_RESULT.NEVER_ASK_AGAIN:
			UISetter.SetActive(PermissionCheck, active: true);
			break;
		}
	}

	private void Setup()
	{
		_class = new AndroidJavaClass("net.sanukin.OverrideUnityActivity");
		_class.CallStatic("start");
	}
}
