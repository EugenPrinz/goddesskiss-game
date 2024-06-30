using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Encryption_Rijndae;
using Facebook.Unity;
using UnityEngine;

public class UIUserDetail : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public UIUserProfile profile;

	public GameObject notice;

	public UIGameSetting setting;

	public GameObject info;

	public UIFlipSwitch profileTab;

	public UIFlipSwitch noticeTab;

	public UIFlipSwitch settingTab;

	public UIFlipSwitch infoTab;

	public UILabel gameVersion;

	public UILabel dbVersion;

	public GameObject fbBtn;

	public GameObject googleBtn;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public List<UIFlipSwitch> languageBtnList;

	public UIButton googleAchievement;

	public GameObject chanceDeviceBtn;

	private string preLanguage;

	private bool preNotification;

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		switch (text)
		{
		case "Close":
			if (setting.patch.activeSelf)
			{
				return;
			}
			ClosePopup();
			break;
		case "Profile":
			InitAndOpenUserDetailProfile();
			break;
		case "Setting":
			InitAndOpenUserDetailSetting();
			break;
		case "Notice":
			InitAndOpenUserDetailNotice();
			break;
		case "BtnComm":
			if (base.localUser.channel == 1)
			{
				string text2 = "?user_app_id=" + base.localUser.uno;
				string text3 = "&user_name=" + base.localUser.nickname;
				string text4 = "&device_info=" + SystemInfo.deviceModel;
				string text5 = "&opt1=version^" + Application.version;
				string text6 = "&opt2=server^" + base.localUser.world;
				string text7 = "&opt3=os^" + SystemInfo.operatingSystem;
				string stringToEscape = "https://gkcs.dbros.co.kr/" + text2 + text3 + text4 + text5 + text6 + text7;
				stringToEscape = Uri.EscapeUriString(stringToEscape);
				Application.OpenURL(stringToEscape);
			}
			else
			{
				string session = JsonRpcClient.session;
				string s = Program.Encrypt_Session(session);
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				string text8 = Convert.ToBase64String(bytes);
				string gameServerUrl = RemoteObjectManager.instance.GameServerUrl;
				gameServerUrl = gameServerUrl.Replace("http://", string.Empty);
				gameServerUrl = gameServerUrl.Replace("https://", string.Empty);
				gameServerUrl = gameServerUrl.Replace("/game/server.php", string.Empty);
				gameServerUrl = gameServerUrl.Replace("/server.php", string.Empty);
				int num = gameServerUrl.IndexOf(":");
				if (num >= 0)
				{
					gameServerUrl = gameServerUrl.Substring(0, num);
				}
				string url = "http://" + gameServerUrl + ":8080/?key=" + text8;
				Application.OpenURL(url);
			}
			break;
		case "BtnTerms":
		{
			string url2 = ((base.localUser.channel != 1) ? "http://gkcdn.dbros.co.kr/legal/policy_en.html" : "http://gkcdn.dbros.co.kr/legal/policy_ko.html");
			Application.OpenURL(url2);
			break;
		}
		case "BtnChangeDevice":
			base.network.RequestGetChangeDeviceCode();
			break;
		case "BtnDeleteDB":
			UISimplePopup.CreateBool(localization: true, "28156", "28157", null, "1001", "1000").onClick = delegate(GameObject obj)
			{
				if (obj.name == "OK")
				{
					PatchManager.Instance.RemoveAllDBFile();
					Application.Quit();
				}
			};
			break;
		case "Info":
			InitAndOpenUserDetailInfo();
			break;
		default:
			if (text.StartsWith("Lang-"))
			{
				string key = text.Substring(text.IndexOf("-") + 1);
				_SetLanguage(key);
				break;
			}
			switch (text)
			{
			case "GoogleAchievement":
				if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
				{
					SA_Singleton<GooglePlayManager>.Instance.ShowAchievementsUI();
				}
				break;
			case "BtnGoogle":
			{
				if (GooglePlayConnection.State != GPConnectionState.STATE_CONNECTED)
				{
					break;
				}
				UISimplePopup uISimplePopup2 = UISimplePopup.CreateBool(localization: true, "5686", "5687", null, "1001", "1000");
				uISimplePopup2.onClick = delegate(GameObject go)
				{
					string text9 = go.name;
					if (text9 == "OK")
					{
						SA_Singleton<GooglePlusAPI>.Instance.ClearDefaultAccount();
						Application.Quit();
					}
				};
				break;
			}
			case "BtnFB":
			{
				if (!FB.IsLoggedIn)
				{
					break;
				}
				UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: true, "5686", "5687", null, "1001", "1000");
				uISimplePopup.onClick = delegate(GameObject go)
				{
					string text10 = go.name;
					if (text10 == "OK")
					{
						FB.LogOut();
						Application.Quit();
					}
				};
				break;
			}
			}
			break;
		}
		base.OnClick(sender);
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		if (profile.root.activeSelf)
		{
			profile.Set(base.localUser);
		}
		else if (setting.root.activeSelf)
		{
			setting.Set(GameSetting.instance);
		}
	}

	public void Set()
	{
		RoStatistics statistics = base.localUser.statistics;
		List<RoMission> achievementList = base.localUser.achievementList;
		profile.Set(base.localUser);
		setting.Set(GameSetting.instance);
		UISetter.SetLabel(gameVersion, Application.version);
		UISetter.SetLabel(dbVersion, PlayerPrefs.GetString("DBVersion", "0"));
		UISetter.SetActive(googleBtn, Application.platform == RuntimePlatform.Android);
		UISetter.SetButtonEnable(googleBtn, GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED);
		UISetter.SetButtonEnable(fbBtn, FB.IsLoggedIn);
		UISetter.SetActive(chanceDeviceBtn, base.localUser.platform != Platform.Guest);
		_SetLanguage(Localization.language);
		preLanguage = PlayerPrefs.GetString("Language");
		_SetPage(profileState: true, settingState: false, noticeState: false, infoState: false);
	}

	public void InitAndOpenUserDetailProfile()
	{
		Set();
		_SetPage(profileState: true, settingState: false, noticeState: false, infoState: false);
	}

	public void InitAndOpenUserDetailSetting()
	{
		Set();
		_SetPage(profileState: false, settingState: true, noticeState: false, infoState: false);
	}

	public void InitAndOpenUserDetailNotice()
	{
		Set();
		_SetPage(profileState: false, settingState: false, noticeState: true, infoState: false);
	}

	public void InitAndOpenUserDetailInfo()
	{
		Set();
		_SetPage(profileState: false, settingState: false, noticeState: false, infoState: true);
	}

	private void _SetPage(bool profileState, bool settingState, bool noticeState, bool infoState)
	{
		int num = 0;
		num += (profileState ? 1 : 0);
		num += (noticeState ? 1 : 0);
		num += (settingState ? 1 : 0);
		num += (infoState ? 1 : 0);
		if (num > 1 || num == 0)
		{
			profileState = true;
			noticeState = false;
			settingState = false;
			infoState = false;
		}
		UISetter.SetActive(profile, profileState);
		UISetter.SetActive(notice, noticeState);
		UISetter.SetActive(setting, settingState);
		UISetter.SetActive(info, infoState);
		UISetter.SetFlipSwitch(profileTab, profileState);
		UISetter.SetFlipSwitch(noticeTab, noticeState);
		UISetter.SetFlipSwitch(settingTab, settingState);
		UISetter.SetFlipSwitch(infoTab, infoState);
	}

	private void _SetLanguage(string key)
	{
		for (int i = 0; i < Localization.knownLanguages.Length; i++)
		{
			if (string.Equals(Localization.knownLanguages[i], key))
			{
				languageBtnList[i].Set(SwitchStatus.ON);
			}
			else
			{
				languageBtnList[i].Set(SwitchStatus.OFF);
			}
		}
		if (Localization.language != key)
		{
			Localization.language = key;
		}
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		if (!bBackKeyEnable)
		{
			if (PlayerPrefs.GetString("Language") != preLanguage)
			{
				base.network.RequestChangeLanguage();
			}
			if (GameSetting.instance.Notification != preNotification)
			{
				base.network.RequestSetPushOnOff(GameSetting.instance.Notification);
			}
			bBackKeyEnable = true;
			HidePopup();
		}
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
		AnimBG.Reset();
		AnimBlock.Reset();
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public void InitOpenPopup()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			InitAndOpenUserDetailProfile();
			OpenPopup();
			preNotification = GameSetting.instance.Notification;
		}
	}
}
