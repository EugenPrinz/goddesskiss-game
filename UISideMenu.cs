using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UISideMenu : MonoBehaviour
{
	private enum UIType
	{
		NONE,
		EVENT,
		MENU
	}

	public GameObject[] btnQuick;

	public GameObject eventTabBtn;

	public GameObject menuArrow;

	public UIGrid tabGrid;

	public GameObject googlePlay;

	public GameObject facebook;

	public UISprite badge;

	public UISprite pilotBadge;

	public UISprite achievementBadge;

	public UISprite achievement;

	public UILabel achievementLabel;

	public UILabel gameCenterLabel;

	public GameObject animationRoot;

	public UIWidget eventRoot;

	public UIWidget buttonRoot;

	public UISprite eventBg;

	public UISprite buttonBg;

	public UIDefaultListView eventListView;

	public UIGrid buttonGrid;

	public UIGrid eventGrid;

	public GEAnimNGUI eventAnim;

	public GEAnimNGUI buttonAnim;

	public UISprite eventTabBg;

	public UISprite buttonTabBg;

	private UIType type;

	private bool isPlaying;

	private GameObject uiRoot;

	private GameObject notiBlock;

	private void Start()
	{
		uiRoot = base.gameObject.transform.parent.gameObject;
		notiBlock = uiRoot.transform.Find("Block").gameObject;
		isPlaying = false;
		type = UIType.NONE;
		SetButton();
		SetBadge();
		SetEvent();
		UISetter.SetAlpha(eventRoot, 0f);
		UISetter.SetAlpha(buttonRoot, 0f);
		SetButtonEnable(UIType.EVENT, state: false);
		SetButtonEnable(UIType.MENU, state: false);
	}

	private void SetButton()
	{
		UISetter.SetSprite(achievement, "qm-ggplay-icon", snap: true);
		UISetter.SetActive(achievementLabel, active: true);
		UISetter.SetActive(gameCenterLabel, active: false);
		UISetter.SetActive(googlePlay, GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED || GameCenterManager.IsPlayerAuthenticated);
		UISetter.SetActive(facebook, PlayerPrefs.GetString("Language") != "S_Kr");
		UISetter.SetColor(eventTabBg, Color.gray);
		UISetter.SetColor(buttonTabBg, Color.gray);
		buttonBg.width = BgSize(buttonGrid.GetChildList().Count);
		buttonGrid.transform.localPosition = new Vector3(GridPositionX(buttonGrid.GetChildList().Count), buttonGrid.transform.localPosition.y, buttonGrid.transform.localPosition.z);
		buttonGrid.Reposition();
	}

	private void SetEvent()
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		List<EventRemaingTimeDataRow> list = new List<EventRemaingTimeDataRow>();
		foreach (KeyValuePair<string, TimeData> item in localUser.eventRemaingTime)
		{
			if (item.Value.GetRemain() > 0.0)
			{
				list.Add(regulation.eventRemaingTimeDtbl[item.Key]);
			}
		}
		list.Sort((EventRemaingTimeDataRow data1, EventRemaingTimeDataRow data2) => data1.sort.CompareTo(data2.sort));
		eventListView.InitEventRemaing(list, "event-");
		eventGrid.transform.localPosition = new Vector3(GridPositionX(eventGrid.GetChildList().Count), eventGrid.transform.localPosition.y, eventGrid.transform.localPosition.z);
		eventBg.width = BgSize(list.Count);
	}

	public void OnClick(GameObject sender)
	{
		SoundManager.PlaySFX("BTN_Positive_001");
		string text = sender.name;
		if (!eventListView.Contains(text))
		{
			CloseMenu();
		}
		switch (text)
		{
		case "Notice":
			StartCoroutine(NoticeCheckRoutine());
			return;
		case "BtnGooglePlay":
			if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
			{
				SA_Singleton<GooglePlayManager>.Instance.ShowAchievementsUI();
			}
			else if (GameCenterManager.IsPlayerAuthenticated)
			{
				GameCenterManager.ShowAchievements();
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("1349"));
			}
			return;
		case "BtnFacebook":
		{
			string language = Localization.language;
			if (language == "S_Beon")
			{
				Application.OpenURL("https://www.facebook.com/goddesskissTW/");
			}
			else
			{
				Application.OpenURL("https://www.facebook.com/goddesskissgame/?fref=ts");
			}
			return;
		}
		case "BtnComm":
			RemoteObjectManager.instance.RequestGetPlugEventInfo();
			GLink.sharedInstance().executeHome();
			return;
		}
		if (eventListView.Contains(text))
		{
			string pureId = eventListView.GetPureId(text);
			EventRemaingTimeDataRow eventRemaingTimeDataRow = RemoteObjectManager.instance.regulation.eventRemaingTimeDtbl[pureId];
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get(eventRemaingTimeDataRow.name));
		}
		else
		{
			UICamp camp = UIManager.instance.world.camp;
			CloseUI();
			camp.GoNavigation(text);
		}
	}

	public void OnRefresh()
	{
		SetBadge();
		SetEvent();
	}

	public void TabBtnClick(GameObject sender)
	{
		SoundManager.PlaySFX("BTN_Positive_001");
		string text = sender.name;
		UIType uIType = (UIType)int.Parse(text.Substring(text.IndexOf("-") + 1));
		if (!isPlaying)
		{
			if (type != uIType)
			{
				OpenMenu(uIType);
			}
			else
			{
				CloseMenu();
			}
		}
	}

	public void FirstOpenMenu()
	{
		if (EventRemaingState())
		{
			OpenMenu(UIType.EVENT);
		}
	}

	private bool EventRemaingState()
	{
		bool result = false;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		foreach (KeyValuePair<string, TimeData> item in localUser.eventRemaingTime)
		{
			if (item.Value.GetRemain() > 0.0)
			{
				result = true;
			}
		}
		return result;
	}

	private void OpenMenu(UIType type)
	{
		if (type == UIType.EVENT && !EventRemaingState())
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("27859"));
			return;
		}
		UIManager.instance.world.mainCommand.CloseWorldMapNavigation();
		UISetter.SetColor(eventTabBg, (type != UIType.EVENT) ? Color.gray : Color.white);
		UISetter.SetColor(buttonTabBg, (type != UIType.MENU) ? Color.gray : Color.white);
		StartCoroutine(OpenMenuAnimation(type));
	}

	private IEnumerator OpenMenuAnimation(UIType type)
	{
		if (this.type != 0)
		{
			yield return StartCoroutine(CloseMenuAnimation());
		}
		UIGrid grid = ((type != UIType.EVENT) ? buttonGrid : eventGrid);
		GUIAnimNGUI anim = ((type != UIType.EVENT) ? buttonAnim : eventAnim);
		this.type = type;
		MoveAnimation(anim, BgSize(grid.GetChildList().Count), open: true);
	}

	private void SetButtonEnable(UIType type, bool state)
	{
		UIGrid uIGrid = ((type != UIType.MENU) ? eventGrid : buttonGrid);
		for (int i = 0; i < uIGrid.GetChildList().Count; i++)
		{
			Transform transform = uIGrid.GetChildList()[i];
			UISetter.SetButtonEnable(transform.gameObject, state);
		}
	}

	private void MoveAnimation(GUIAnimNGUI anim, int positionX, bool open)
	{
		isPlaying = true;
		if (open)
		{
			UISetter.SetAlpha(buttonRoot, (type != UIType.MENU) ? 0f : 1f);
			UISetter.SetAlpha(eventRoot, (type != UIType.EVENT) ? 0f : 1f);
			anim.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		}
		else
		{
			anim.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		}
		SetButtonEnable(UIType.EVENT, type == UIType.EVENT);
		SetButtonEnable(UIType.MENU, type == UIType.MENU);
	}

	public void CloseMenu()
	{
		UISetter.SetColor(eventTabBg, Color.gray);
		UISetter.SetColor(buttonTabBg, Color.gray);
		if (type != 0)
		{
			StartCoroutine(CloseMenuAnimation());
		}
	}

	private IEnumerator CloseMenuAnimation()
	{
		UIGrid grid = ((type != UIType.EVENT) ? buttonGrid : eventGrid);
		GUIAnimNGUI anim = ((type != UIType.EVENT) ? buttonAnim : eventAnim);
		type = UIType.NONE;
		MoveAnimation(anim, BgSize(grid.GetChildList().Count), open: false);
		while (isPlaying)
		{
			yield return null;
		}
	}

	public void AnimationEnd()
	{
		isPlaying = false;
	}

	public void SetBadge()
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetActive(achievementBadge, localUser.badgeMissionCount + localUser.badgeAchievementCount > 0);
		UISetter.SetActive(pilotBadge, localUser.IsBadgeHeadQuarter());
		UISetter.SetActive(badge, localUser.IsBadgeHeadQuarter() || localUser.badgeMissionCount + localUser.badgeAchievementCount > 0);
	}

	public void CloseUI()
	{
		UIManager.World world = UIManager.instance.world;
		if (world.worldMap != null && world.worldMap.isActive)
		{
			world.worldMap.CloseWorldMap();
		}
	}

	private IEnumerator NoticeCheckRoutine()
	{
		UISetter.SetActive(notiBlock, active: true);
		List<Protocols.NoticeData> eventList = RemoteObjectManager.instance.localUser.FindNoticeList(ENoticeType.Event);
		for (int i = 0; i < eventList.Count; i++)
		{
			UINotice noticePopUp = UIPopup.Create<UINotice>("UINotice");
			noticePopUp.Init(eventList[i]);
			UIManager.instance.world.noticePopUp = noticePopUp.gameObject;
			while (UIManager.instance.world.noticePopUp != null)
			{
				yield return null;
			}
		}
		List<Protocols.NoticeData> webViewList = RemoteObjectManager.instance.localUser.FindNoticeList(ENoticeType.WebView);
		for (int j = 0; j < webViewList.Count; j++)
		{
			UIWebviewPopup webviewPopUp = UIPopup.Create<UIWebviewPopup>("UIWebView");
			webviewPopUp.Init(webViewList[j].link);
			UIManager.instance.world.noticePopUp = webviewPopUp.gameObject;
			while (UIManager.instance.world.noticePopUp != null)
			{
				yield return null;
			}
		}
		UISetter.SetActive(notiBlock, active: false);
		yield return true;
	}

	private int BgSize(int cnt)
	{
		int num = (int)eventGrid.cellWidth;
		return Mathf.Max(1, Mathf.CeilToInt((float)cnt / (float)eventGrid.maxPerLine)) * num;
	}

	private int GridPositionX(int cnt)
	{
		int num = (int)eventGrid.cellWidth;
		return -((Mathf.Max(1, Mathf.CeilToInt((float)cnt / (float)eventGrid.maxPerLine)) - 1) * num) + -50;
	}
}
