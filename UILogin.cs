using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

public class UILogin : MonoBehaviour
{
	public UIInput ID;

	public UIInput PW;

	public GameObject obj_LoginList;

	public GameObject listItem;

	public UISprite enableBox;

	private List<LocalStorage.UserLoginData> userLoginData;

	private bool isGoogleLogin;

	private bool isGoogleBtnPress;

	public UILabel guestLogin;

	private bool isSelectGuest;

	private int loginPlatform;

	public UISprite imgPlatform;

	public GameObject googleButton;

	private string fb_AccessToken;

	private void Start()
	{
		SetItemUserLoginData();
		GooglePlayConnection.ActionPlayerConnected += OnPlayerConnected;
		GooglePlayConnection.ActionPlayerDisconnected += OnPlayerDisconnected;
		GooglePlayManager.ActionOAuthTokenLoaded += ActionOAuthTokenLoaded;
		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
		{
			OnPlayerConnected();
		}
		LoadLocaleInfo();
		if (!string.IsNullOrEmpty(PlayerPrefs.GetString("MemberID")))
		{
			AutoLogin();
		}
		else if (!RemoteObjectManager.instance.localUser.bShowUserTerm)
		{
			UIPopup.Create<UISelectPlatformPopup>("SelectPlatformPopup").InitAndOpen();
		}
		UISetter.SetActive(googleButton, Application.platform != RuntimePlatform.IPhonePlayer && RemoteObjectManager.instance.localUser.bEnableGoogleAccount);
	}

	private void Update()
	{
		if (isGoogleLogin)
		{
			RetrieveToken();
			isGoogleLogin = false;
		}
	}

	private void OnEnable()
	{
		ID.value = PlayerPrefs.GetString("MemberID");
		PW.value = PlayerPrefs.GetString("MemberPW");
		loginPlatform = PlayerPrefs.GetInt("MemberPlatform");
		if (loginPlatform == 2)
		{
			UISetter.SetSprite(imgPlatform, "login_btn_fb");
		}
		else if (loginPlatform == 3)
		{
			UISetter.SetSprite(imgPlatform, "login_btn_gp");
		}
		else
		{
			UISetter.SetSprite(imgPlatform, "login_btn_flerogames");
		}
		if (!string.IsNullOrEmpty(PlayerPrefs.GetString("GuestID")))
		{
			isSelectGuest = PlayerPrefs.GetString("MemberID") == PlayerPrefs.GetString("GuestID");
		}
		else
		{
			isSelectGuest = false;
		}
		if (isSelectGuest)
		{
			UISetter.SetLabel(guestLogin, Localization.Get("19040"));
		}
		else
		{
			UISetter.SetLabel(guestLogin, Localization.Get("19021"));
		}
		UISetter.SetActive(enableBox, !string.IsNullOrEmpty(ID.value));
		UISetter.SetActive(imgPlatform, !string.IsNullOrEmpty(ID.value));
	}

	private void AutoLogin()
	{
		if (!string.IsNullOrEmpty(PlayerPrefs.GetString("MemberID")))
		{
			if (isSelectGuest)
			{
				RemoteObjectManager.instance.RequestGuestSignIn(PlayerPrefs.GetString("GuestID"));
			}
			else if (loginPlatform == 2)
			{
				FaceBookBtn();
			}
			else if (loginPlatform == 3)
			{
				GoogleBtn();
			}
			else
			{
				RemoteObjectManager.instance.RequestSignIn(ID.value, PW.value);
			}
		}
	}

	public void SetItemUserLoginData()
	{
		userLoginData = LocalStorage.LoadLoginData();
		foreach (LocalStorage.UserLoginData userLoginDatum in userLoginData)
		{
		}
		foreach (LocalStorage.UserLoginData userLoginDatum2 in userLoginData)
		{
			GameObject gameObject = Object.Instantiate(listItem);
			gameObject.transform.SetParent(listItem.transform.parent);
			gameObject.name = "User-" + userLoginDatum2.id;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.Find("Id").GetComponent<UILabel>().text = userLoginDatum2.id;
			gameObject.transform.Find("Pw").GetComponent<UILabel>().text = userLoginDatum2.pw;
			gameObject.transform.Find("plfm").tag = userLoginDatum2.platform.ToString();
			if (userLoginDatum2.platform == 2)
			{
				gameObject.transform.Find("plfm").GetComponent<UISprite>().spriteName = "login_btn_fb";
			}
			else if (userLoginDatum2.platform == 3)
			{
				gameObject.transform.Find("plfm").GetComponent<UISprite>().spriteName = "login_btn_gp";
			}
			else
			{
				gameObject.transform.Find("plfm").GetComponent<UISprite>().spriteName = "login_btn_flerogames";
			}
		}
		UISetter.SetActive(listItem, active: false);
	}

	public void RemoveUserLoginData(string id)
	{
		Transform parent = listItem.transform.parent;
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			if (child != null && child.name == "User-" + id)
			{
				Object.Destroy(child.gameObject);
				LocalStorage.RemoveLoginData(id);
				break;
			}
		}
	}

	public void OnChange(GameObject sender)
	{
		string text = sender.name;
		if (text == "Input_ID")
		{
			UISetter.SetActive(enableBox, active: false);
			UISetter.SetActive(obj_LoginList, active: false);
			loginPlatform = 1;
			UISetter.SetSprite(imgPlatform, "login_btn_flerogames");
			if (isSelectGuest)
			{
				isSelectGuest = false;
				UISetter.SetLabel(guestLogin, Localization.Get("19021"));
			}
		}
	}

	public void SetUserData(GameObject item)
	{
		ID.value = item.transform.Find("Id").GetComponent<UILabel>().text;
		PW.value = item.transform.Find("Pw").GetComponent<UILabel>().text;
		loginPlatform = int.Parse(item.transform.Find("plfm").tag);
		if (loginPlatform == 2)
		{
			UISetter.SetSprite(imgPlatform, "login_btn_fb");
		}
		else if (loginPlatform == 3)
		{
			UISetter.SetSprite(imgPlatform, "login_btn_gp");
		}
		else
		{
			UISetter.SetSprite(imgPlatform, "login_btn_flerogames");
		}
		if (!string.IsNullOrEmpty(PlayerPrefs.GetString("GuestID")))
		{
			isSelectGuest = item.transform.Find("Id").GetComponent<UILabel>().text == PlayerPrefs.GetString("GuestID");
		}
		else
		{
			isSelectGuest = false;
		}
		if (isSelectGuest)
		{
			UISetter.SetLabel(guestLogin, Localization.Get("19040"));
		}
		else
		{
			UISetter.SetLabel(guestLogin, Localization.Get("19021"));
		}
		UISetter.SetActive(enableBox, active: true);
		UISetter.SetActive(obj_LoginList, active: false);
	}

	public void OnClick(GameObject sender)
	{
		string text = sender.name;
		switch (text)
		{
		case "Btn_Join":
			SoundManager.PlaySFX("BTN_Norma_001");
			CreateMember();
			return;
		case "Btn_Login":
			SoundManager.PlaySFX("BTN_Norma_001");
			if (isSelectGuest)
			{
				RemoteObjectManager.instance.RequestGuestSignIn(PlayerPrefs.GetString("GuestID"));
			}
			else if (loginPlatform == 2)
			{
				FaceBookBtn();
			}
			else if (loginPlatform == 3)
			{
				GoogleBtn();
			}
			else if (ID.value.Length < 6 || ID.value.Length > 16 || string.IsNullOrEmpty(ID.value))
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19051"));
			}
			else if (PW.value.Length < 6 || PW.value.Length > 16 || string.IsNullOrEmpty(PW.value))
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19056"));
			}
			else
			{
				RemoteObjectManager.instance.RequestSignIn(ID.value, PW.value);
			}
			return;
		case "Btn_Guest":
			SoundManager.PlaySFX("BTN_Norma_001");
			if (isSelectGuest)
			{
				RemoteObjectManager.instance.bChangeFullMember = true;
				Protocols.OSCode osType = Protocols.OSCode.Android;
				UIPopup.Create<UISelectPlatformPopup>("SelectPlatformPopup").InitAndOpen(osType, Platform.Guest);
			}
			else
			{
				GuestSignUp();
			}
			return;
		case "Btn_OpenList":
			SoundManager.PlaySFX("BTN_Norma_001");
			UISetter.SetActive(obj_LoginList, !obj_LoginList.activeSelf);
			return;
		}
		if (text.StartsWith("User-"))
		{
			SoundManager.PlaySFX("BTN_Norma_001");
			SetUserData(sender);
			return;
		}
		switch (text)
		{
		case "Btn_Facebook":
			SoundManager.PlaySFX("BTN_Norma_001");
			FaceBookBtn();
			break;
		case "Btn_Google":
			SoundManager.PlaySFX("BTN_Norma_001");
			GoogleBtn();
			break;
		case "Btn_FindPass":
			break;
		case "Btn_ChangeDevice":
		{
			UIReceiveUserString recv = UIPopup.Create<UIReceiveUserString>("InputUserString");
			recv.SetDefault(string.Empty);
			recv.SetLimitLength(12);
			recv.Set(localization: true, "19521", "19523", null, "1001", "1000", null);
			recv.onClick = delegate(GameObject popupSender)
			{
				string text2 = popupSender.name;
				if (text2 == "OK")
				{
					RemoteObjectManager.instance.RequestCheckChangeDeviceCode(recv.inputLabel.text);
				}
			};
			break;
		}
		}
	}

	public void GuestSignUp()
	{
		UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: true, "19021", "19044", null, "1001", "1000");
		uISimplePopup.onClick = delegate(GameObject popupSender)
		{
			string text = popupSender.name;
			if (text == "OK")
			{
				if (string.IsNullOrEmpty(PlayerPrefs.GetString("GuestID")))
				{
					RemoteObjectManager.instance.RequestGuestSignUp();
				}
				else
				{
					RemoteObjectManager.instance.RequestGuestSignIn(PlayerPrefs.GetString("GuestID"));
				}
			}
		};
	}

	public void CreateMember(int type = 0)
	{
		RemoteObjectManager.instance.localUser.loginType = type;
		if (type != 2)
		{
			RemoteObjectManager.instance.bChangeFullMember = false;
		}
		M01_Title m01_Title = Object.FindObjectOfType(typeof(M01_Title)) as M01_Title;
		m01_Title.obj_Login.SetActive(value: false);
		m01_Title.obj_Join.SetActive(value: true);
	}

	public void FaceBookBtn(int type = 0)
	{
		RemoteObjectManager.instance.localUser.loginType = type;
		CallFBInit();
	}

	private void CallFBInit()
	{
		if (FB.IsInitialized)
		{
			CallFBLogin();
		}
		else
		{
			FB.Init(onInitComplete, OnHideUnity);
		}
	}

	private void onInitComplete()
	{
		CallFBLogin();
	}

	private void OnHideUnity(bool isGameShown)
	{
	}

	private void CallFBLogin()
	{
		if (FB.IsLoggedIn)
		{
			FB.API("/me?fields=id,first_name,last_name,", HttpMethod.GET, FBUserName);
			return;
		}
		List<string> list = new List<string>();
		list.Add("public_profile");
		list.Add("email");
		list.Add("user_friends");
		FB.LogInWithReadPermissions(list, LoginCallback);
	}

	private void LoginCallback(ILoginResult result)
	{
		if (result.Error == null && FB.IsLoggedIn)
		{
			FB.API("/me?fields=id,first_name,last_name,", HttpMethod.GET, FBUserName);
		}
	}

	private void FBAccessToken(IResult result)
	{
		if (result.Error == null)
		{
			RemoteObjectManager.instance.RequestFBSignIn(result.ResultDictionary["access_token"].ToString());
		}
	}

	private void FBUserName(IResult result)
	{
		if (result.Error != null)
		{
			FB.API("/me?field=id,first_name,last_name", HttpMethod.GET, FBUserName);
			return;
		}
		fb_AccessToken = AccessToken.CurrentAccessToken.TokenString;
		RemoteObjectManager.instance.localUser.platformUserInfo = result.ResultDictionary["name"].ToString();
		if (RemoteObjectManager.instance.localUser.loginType == 0)
		{
			RemoteObjectManager.instance.RequestFBSignIn(fb_AccessToken);
		}
		else
		{
			RemoteObjectManager.instance.RequestCheckPlatformExist(Platform.FaceBook, fb_AccessToken);
		}
	}

	private void CallFBLogout()
	{
		FB.LogOut();
	}

	public void GoogleBtn(int type = 0)
	{
		GoogleStart(type);
	}

	private void GoogleStart(int type = 0)
	{
		RemoteObjectManager.instance.localUser.loginType = type;
		isGoogleBtnPress = true;
		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
		{
			isGoogleLogin = true;
		}
		else
		{
			GoogleLogin();
		}
	}

	public void GoogleLogin()
	{
		SA_Singleton<GooglePlayConnection>.Instance.Connect();
	}

	public void GoogleLogOut()
	{
		SA_Singleton<GooglePlayConnection>.Instance.Disconnect();
	}

	private void OnDestroy()
	{
		if (!SA_Singleton<GooglePlayConnection>.IsDestroyed)
		{
			GooglePlayConnection.ActionPlayerConnected -= OnPlayerConnected;
			GooglePlayConnection.ActionPlayerDisconnected -= OnPlayerDisconnected;
		}
		if (!SA_Singleton<GooglePlayManager>.IsDestroyed)
		{
			GooglePlayManager.ActionOAuthTokenLoaded -= ActionOAuthTokenLoaded;
		}
	}

	private void RetrieveToken()
	{
		SA_Singleton<GooglePlayManager>.Instance.LoadToken();
	}

	private void OnPlayerDisconnected()
	{
	}

	private void OnPlayerConnected()
	{
		isGoogleLogin = true;
	}

	private void ActionOAuthTokenLoaded(string token)
	{
		if (isGoogleBtnPress)
		{
			RemoteObjectManager.instance.localUser.platformUserInfo = SA_Singleton<GooglePlayManager>.Instance.player.name;
			if (RemoteObjectManager.instance.localUser.loginType == 0)
			{
				RemoteObjectManager.instance.RequestGoogleSignIn(SA_Singleton<GooglePlayManager>.Instance.loadedAuthToken);
			}
			else
			{
				RemoteObjectManager.instance.RequestCheckPlatformExist(Platform.Google, SA_Singleton<GooglePlayManager>.Instance.loadedAuthToken);
			}
		}
		isGoogleBtnPress = false;
	}

	public void LoadLocaleInfo()
	{
		AndroidNativeUtility.LocaleInfoLoaded += LocaleInfoLoaded;
		SA_Singleton<AndroidNativeUtility>.instance.LoadLocaleInfo();
	}

	private void LocaleInfoLoaded(AN_Locale locale)
	{
		RemoteObjectManager.instance.localUser.localeCountryCode = locale.CountryCode;
	}
}
