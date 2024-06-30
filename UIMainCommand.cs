using System.Collections;
using System.Collections.Generic;
using Cache;
using RoomDecorator;
using RoomDecorator.Data;
using Shared.Regulation;
using UnityEngine;

public class UIMainCommand : UIPopup
{
	public UIUser user;

	public UIChatting chat;

	public GameObject uiRoot;

	public GameObject purchasePopup;

	public GameObject Top;

	public GameObject LeftMainSpine;

	public GameObject Right;

	public GameObject BottomLeft;

	public GameObject BottomRight;

	public GameObject campBtn;

	public GameObject worldMapBtn;

	public GameObject StageInfo;

	public MessageTextUp2D floatingText;

	private int campDepth;

	private int worldMapDepth;

	public UITimer bulletTimer;

	public UITimer oilTimer;

	public UILabel bullet;

	public UILabel key;

	public UILabel challenge;

	public UILabel worldChallenge;

	public UILabel sweep;

	public UILabel waveDuelTicket;

	public UILabel cooperateBattleTicket;

	public UILabel eventRaidTicket;

	public UILabel oil;

	public GameObject goldBtn;

	public GameObject CallBtn;

	public UICallMessage callMessage;

	public UIPanel bulletRoot;

	public GameObject keyRoot;

	public GameObject challengeRoot;

	public GameObject worldChallengeRoot;

	public GameObject sweepRoot;

	public GameObject otherRoot;

	public GameObject eventRaidRoot;

	public UIPanel oilRoot;

	public GameObject goldRoot;

	public UISprite otherIcon;

	public UILabel otherCount;

	public GameObject waveDuelTicketRoot;

	public GameObject cooperateBattleTicketRoot;

	public GameObject badgeMail;

	public UILabel mailCount;

	public GameObject badgeChat;

	public GameObject badgeWorldMap;

	public GameObject badgeCarnival1;

	public GameObject badgeCarnival2;

	public GameObject badgeCarnival3;

	public GameObject badgeGroup;

	public GameObject badgeFirstPayment;

	public GameObject campRoot;

	public GameObject worldMapRoot;

	public GameObject commmonRoot;

	public UILabel worldName;

	public GameObject contentsMenu;

	public GameObject firstPayment;

	public UISprite achievement;

	public UILabel achievementLabel;

	public UILabel gameCenterLabel;

	public GameObject joinAttendCarnivalBtn;

	public UISprite bottomLeftBg;

	public UIGrid bottomLeftGrid;

	public bool enableUIAnimation = true;

	private TimeData bulletReChargeTime;

	private TimeData oilReChargeTime;

	private UIDiamondShop diamonShop;

	private UIVipInfo vipInfo;

	private UIPanel campPanel;

	private UIPanel worldMapPanel;

	private UIPanel rootPanel;

	private UIPanel commonRootPanel;

	private UIPanel campRootPanel;

	private UIPanel chatRootPanel;

	private UIPanel sideMenuPanel;

	private UIPanel contentsMenuRootPanel;

	private UIPanel worldMapRootPanel;

	private UIPanel callMessagePanel;

	private GameObject notiBlock;

	private UIFirstPaymentPopup firstPaymentPopup;

	public UIPanel naviRootPanel;

	public UIPanel naviListPanel;

	public GameObject webEvent;

	public GameObject badgeWebEvent;

	[HideInInspector]
	public GameObject sideMenu;

	private UISideMenu uiSideMenu;

	public UIMainbanner banner;

	public UISpineAnimationTest spineTest;

	private bool _enableMainCommander;

	private int _openDiaShopType = 1;

	public UISprite worldMapNaviBtn;

	public GameObject worldMapNaviRoot;

	public GameObject worldMapNaviParent;

	public UIDefaultListView worldMapNaviListView;

	public GUIAnimNGUI m_SecondaryButton;

	public GameObject dormitoryBtn;

	private void Start()
	{
		uiRoot = base.gameObject.transform.parent.gameObject;
		sideMenu = uiRoot.transform.Find("SideMenu").gameObject;
		uiSideMenu = sideMenu.GetComponent<UISideMenu>();
		notiBlock = uiRoot.transform.Find("Block").gameObject;
		campDepth = 0;
		worldMapDepth = 0;
		bulletReChargeTime = TimeData.Create();
		oilReChargeTime = TimeData.Create();
		callMessage.Init();
		SettingDepth();
		UISetter.SetActive(webEvent, active: false);
		bottomLeftBg.width = 120 + bottomLeftGrid.GetChildList().Count * 110;
		uiSideMenu.FirstOpenMenu();
	}

	private void SettingDepth()
	{
		campPanel = base.uiWorld.camp.GetComponent<UIPanel>();
		worldMapPanel = base.uiWorld.worldMap.GetComponent<UIPanel>();
		rootPanel = base.gameObject.GetComponent<UIPanel>();
		commonRootPanel = commmonRoot.GetComponent<UIPanel>();
		campRootPanel = campRoot.GetComponent<UIPanel>();
		chatRootPanel = chat.GetComponent<UIPanel>();
		sideMenuPanel = sideMenu.GetComponent<UIPanel>();
		contentsMenuRootPanel = contentsMenu.GetComponent<UIPanel>();
		worldMapRootPanel = worldMapRoot.GetComponent<UIPanel>();
		callMessagePanel = callMessage.scrollView.GetComponent<UIPanel>();
	}

	private IEnumerator TutorialPressChecker()
	{
		yield return new WaitForSeconds(1.5f);
	}

	public void OnPress(GameObject sender)
	{
		if (sender.name == "CallBtn")
		{
			StopCoroutine("TutorialPressChecker");
			StartCoroutine("TutorialPressChecker");
		}
	}

	public void OnRelease(GameObject sender)
	{
		if (sender.name == "CallBtn")
		{
			StopCoroutine("TutorialPressChecker");
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		switch (text)
		{
		case "UserDetail":
			base.uiWorld.userDetail.InitOpenPopup();
			return;
		case "Achievement":
			base.uiWorld.achievement.Set(EReward.Achievement);
			base.uiWorld.achievement.Open();
			return;
		case "Mail":
			base.network.RequestMailList();
			return;
		case "Mission":
			return;
		case "CallBtn":
			return;
		case "Link-CashShop":
			OpenDiamonShop();
			return;
		case "Link-ChallengeShop":
			return;
		case "Link-GoldShop":
			OpenMetroBank();
			return;
		case "BulletRecharge":
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Bullet);
			return;
		case "KeyRecharge":
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Key);
			return;
		case "ChallengeRecharge":
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Challenge);
			return;
		case "WorldChallengeRecharge":
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.WorldDuelTicket);
			return;
		case "SweepRecharge":
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.SweepTicket);
			return;
		case "EventRaidRecharge":
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.EventRaidTicket);
			return;
		case "OilRecharge":
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Oil);
			return;
		case "ToCamp":
			base.uiWorld.worldMap.Close();
			return;
		case "ToWorldMap":
			base.uiWorld.worldMap.InitAndOpenWorldMap();
			return;
		case "BtnCalendar":
			base.uiWorld.dailyBonus.InitAndOpenDailyBonus();
			return;
		case "BtnCarnival":
			base.network.RequestGetCarnivalList(ECarnivalCategory.Basic);
			return;
		case "BtnGroup":
			base.uiWorld.group.InitAndOpenGroup();
			return;
		case "BtnDate":
			base.uiWorld.dateMode.InitAndOpenDateMode();
			return;
		case "BtnWebEvent":
			RemoteObjectManager.instance.RequestStartWebEvent();
			return;
		case "BtnReward":
			base.network.RequestGetCarnivalList(ECarnivalCategory.Reward);
			return;
		case "BtnNewUser":
			base.network.RequestGetCarnivalList(ECarnivalCategory.JoinAttend);
			return;
		case "WorldMapNaviOpenBtn":
			OpenWorldMapNavigation();
			return;
		case "BtnDormitory":
		{
			base.localUser.dormitory.InitCommanders();
			RoDormitory.User user = new RoDormitory.User();
			user.name = base.localUser.nickname;
			user.level = base.localUser.level;
			user.uno = base.localUser.uno;
			user.isMaster = true;
			DormitoryInitData.Instance.Set(user);
			base.network.RequestGetDormitoryFloorInfo();
			return;
		}
		}
		if (worldMapNaviListView.Contains(text))
		{
			string pureId = worldMapNaviListView.GetPureId(text);
			if (!base.uiWorld.worldMap.currentWorldMapId.Equals(pureId))
			{
				base.uiWorld.worldMap.MoveWorldMap(pureId);
			}
		}
		else if (text == "FirstPayment")
		{
			OpenAndInitFirstPaymentPopup();
		}
	}

	private void OpenMetroBank()
	{
		base.uiWorld.camp.GoNavigation("MetroBank");
	}

	public void Set()
	{
		if (user != null)
		{
			user.Set(base.localUser);
			UISetter.SetLabel(bullet, $"{base.localUser.bullet}/{base.regulation.GetUserLevelDataRow(base.localUser.level).maxBullet}");
			UISetter.SetLabel(key, $"{base.localUser.opener}/{base.regulation.FindGoodsServerFieldName(10).rechargeMax}");
			UISetter.SetLabel(challenge, $"{base.localUser.challenge}/{base.regulation.FindGoodsServerFieldName(9).rechargeMax}");
			UISetter.SetLabel(worldChallenge, $"{base.localUser.worldDuelTicket}/{base.regulation.FindGoodsServerFieldName(8001).rechargeMax}");
			UISetter.SetLabel(sweep, $"{base.localUser.sweepTicket}/{base.regulation.FindGoodsServerFieldName(7).rechargeMax}");
			UISetter.SetLabel(waveDuelTicket, $"{base.localUser.waveDuelTicket}/{base.regulation.FindGoodsServerFieldName(57).rechargeMax}");
			UISetter.SetLabel(cooperateBattleTicket, string.Format("{0}/{1}", base.localUser.cooperateBattleTicket, base.regulation.defineDtbl["COOPERATE_BATTLE_COUNT"].value));
			UISetter.SetLabel(eventRaidTicket, $"{base.localUser.eventRaidTicket}/{base.regulation.FindGoodsServerFieldName(6).rechargeMax}");
			UISetter.SetLabel(oil, $"{base.localUser.oil}/{base.regulation.FindGoodsServerFieldName(24).rechargeMax}");
		}
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.MetroBank);
		UISetter.SetButtonEnable(goldBtn, base.localUser.level >= roBuilding.firstLevelReg.userLevel);
		BadgeControl();
		SetPaymentControl();
		if (webEvent != null || base.localUser.badgeCarnivalTabList[3].Count != 0)
		{
			UISetter.SetActive(webEvent, base.localUser.webEventUrls != null && base.localUser.webEventUrls.Count > 0);
			UISetter.SetActive(joinAttendCarnivalBtn, base.localUser.badgeCarnivalTabList[3].Count != 0);
			bottomLeftBg.width = 120 + bottomLeftGrid.GetChildList().Count * 110;
			bottomLeftGrid.Reposition();
			UISetter.SetActive(badgeWebEvent, base.localUser.badgeWebEvent);
		}
		UISetter.SetActive(dormitoryBtn, base.localUser.level >= base.localUser.dormitory.config.openLevel);
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		Set();
		chat.OnRefresh();
		if (diamonShop != null)
		{
			diamonShop.SetUserData();
		}
		if (sideMenu != null)
		{
			uiSideMenu.SetBadge();
		}
		if (firstPaymentPopup != null)
		{
			firstPaymentPopup.OnRefresh();
		}
	}

	public void OnWorldMap()
	{
		SetResourceView(EGoods.Bullet);
		base.network.RequestBulletCharge();
		UISetter.SetActive(worldMapBtn, active: false);
		UISetter.SetActive(campBtn, active: true);
		UISetter.SetActive(chat, active: false);
		UISetter.SetActive(StageInfo, active: true);
		UISetter.SetActive(worldMapRoot, active: true);
		UISetter.SetActive(campRoot, active: false);
		UISetter.SetActive(banner, active: false);
		UISetter.SetActive(worldMapNaviParent, active: true);
		Set();
		SetWorldMapNaviList();
		Open();
		DepthControl(camp: false);
		base.localUser.UserLevelUpCheck();
		if (callMessage != null)
		{
			callMessage.transform.localPosition = new Vector2(10f, -100f);
		}
	}

	public void OnCamp()
	{
		commmonRoot.GetComponent<PanelDepthControl>().OnDisable();
		chat.GetComponent<PanelDepthControl>().OnDisable();
		SetResourceView(EGoods.Bullet);
		UISetter.SetActive(campBtn, active: false);
		UISetter.SetActive(worldMapBtn, active: true);
		UISetter.SetActive(chat, active: true);
		UISetter.SetActive(StageInfo, active: false);
		UISetter.SetActive(worldMapRoot, active: false);
		UISetter.SetActive(campRoot, active: true);
		UISetter.SetActive(sideMenu, active: true);
		UISetter.SetActive(contentsMenu, active: true);
		UISetter.SetActive(banner, active: false);
		UISetter.SetActive(worldMapNaviParent, active: false);
		EnableMainCommanderSpine(!RemoteObjectManager.instance.localUser.isEnableTutorial);
		base.network.RequestOnCamp();
		Set();
		Open();
		DepthControl(camp: true);
		base.localUser.UserLevelUpCheck();
		RemoteObjectManager.instance.RequestGetRotationBannerInfo();
		CacheManager.instance.SoundPocketCache.Create("Pocket_BGM_World");
		if (callMessage != null)
		{
			callMessage.transform.localPosition = new Vector2(10f, -230f);
		}
	}

	public void EnableMainCommanderSpine(bool isEnable)
	{
		UISetter.SetActive(spineTest, isEnable);
		if (isEnable && enableUIAnimation)
		{
			StartAnimation();
		}
	}

	private void DepthControl(bool camp)
	{
		campDepth = campPanel.depth + 1;
		worldMapDepth = worldMapPanel.depth + 1;
		if (camp)
		{
			rootPanel.depth = campDepth;
			commonRootPanel.depth = campDepth;
			bulletRoot.depth = campDepth + 1;
			campRootPanel.depth = campDepth;
			chatRootPanel.depth = campDepth;
			sideMenuPanel.depth = campDepth;
			callMessagePanel.depth = campDepth + 1;
			contentsMenuRootPanel.depth = campDepth;
		}
		else
		{
			rootPanel.depth = worldMapDepth;
			commonRootPanel.depth = worldMapDepth + 1;
			bulletRoot.depth = worldMapDepth + 2;
			worldMapRootPanel.depth = worldMapDepth + 2;
			sideMenuPanel.depth = worldMapDepth + 3;
			naviRootPanel.depth = worldMapDepth + 4;
			naviListPanel.depth = worldMapDepth + 5;
			callMessagePanel.depth = worldMapDepth + 4;
		}
	}

	public void StartLeftMainSpine()
	{
		iTween.MoveTo(LeftMainSpine, iTween.Hash("x", 0, "time", 1.0, "delay", 0.0, "islocal", true, "oncomplete", "OnStartLeftMainSpine", "oncompletetarget", base.gameObject));
	}

	public void OnStartLeftMainSpine()
	{
		_enableMainCommander = true;
		EnableMainCommanderVoice(enable: true);
	}

	public void EndAnimation()
	{
		_enableMainCommander = true;
		GameObject target = spineTest.spine.target;
		if (target != null)
		{
			target.GetComponent<UIInteraction>().StartInteraction();
			EnableMainCommanderVoice(enable: true);
		}
	}

	public void EnableMainCommanderVoice(bool enable)
	{
		if (UIPopup.openedPopups.Count != 1 || !(UIPopup.openedPopups[0] is UICamp))
		{
			enable = false;
		}
		if (!_enableMainCommander)
		{
			enable = false;
		}
		spineTest.enableVoice = enable;
	}

	public void CloseLeftMainSpine()
	{
		iTween.MoveTo(LeftMainSpine, iTween.Hash("x", -550, "time", 1.8, "delay", 0.0, "islocal", true, "oncomplete", "OnCloseLeftMainSpine", "oncompletetarget", base.gameObject));
	}

	public void OnCloseLeftMainSpine()
	{
		_enableMainCommander = false;
		EnableMainCommanderVoice(enable: false);
	}

	public void StartAnimation()
	{
		enableUIAnimation = false;
		iTween.MoveFrom(Top, iTween.Hash("y", 5, "time", 1.0, "delay", 0.5));
		iTween.MoveFrom(LeftMainSpine, iTween.Hash("x", -5, "time", 1.0, "delay", 1.1, "oncomplete", "EndAnimation", "oncompletetarget", base.gameObject));
		iTween.MoveFrom(BottomLeft, iTween.Hash("x", -5, "time", 1.0, "delay", 0.9));
		iTween.MoveFrom(BottomRight, iTween.Hash("x", 5, "time", 1.0, "delay", 0.9));
	}

	public void TopAnChorAnimation()
	{
		iTween.MoveFrom(Top, iTween.Hash("y", 5, "time", 1.0, "delay", 0));
	}

	public void BulletControl()
	{
		UserLevelDataRow userLevelDataRow = RemoteObjectManager.instance.regulation.GetUserLevelDataRow(base.localUser.level);
		UISetter.SetLabel(bullet, $"{base.localUser.bullet}/{userLevelDataRow.maxBullet}");
		if (base.localUser.bullet < userLevelDataRow.maxBullet && base.localUser.bulletRemain > 0)
		{
			UISetter.SetActive(bulletTimer, active: true);
			bulletReChargeTime.SetByDuration(base.localUser.bulletRemain);
			UISetter.SetTimer(bulletTimer, bulletReChargeTime);
			bulletTimer.RegisterOnFinished(delegate
			{
				base.localUser.BulletCharge();
			});
		}
		else
		{
			UISetter.SetActive(bulletTimer, active: false);
		}
	}

	public void MaxBulletCheck()
	{
		UserLevelDataRow userLevelDataRow = RemoteObjectManager.instance.regulation.GetUserLevelDataRow(base.localUser.level);
		if (base.localUser.bullet >= userLevelDataRow.maxBullet)
		{
			UISetter.SetActive(bulletTimer, active: false);
			base.network.CancelLocalPush(ELocalPushType.BulletFullCharge);
		}
		else if (!base.localUser.useBullet && base.localUser.tempBullet != base.localUser.bullet)
		{
			base.network.ScheduleLocalPush(ELocalPushType.BulletFullCharge, (int)bulletReChargeTime.GetRemain());
		}
		base.localUser.tempBullet = base.localUser.bullet;
	}

	public void OilControl()
	{
		GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.FindGoodsServerFieldName(24.ToString());
		UISetter.SetLabel(oil, $"{base.localUser.oil}/{goodsDataRow.rechargeMax}");
		if (base.localUser.oil < goodsDataRow.rechargeMax && base.localUser.oilRemain > 0)
		{
			UISetter.SetActive(oilTimer, active: true);
			oilReChargeTime.SetByDuration(base.localUser.oilRemain);
			UISetter.SetTimer(oilTimer, oilReChargeTime);
			oilTimer.RegisterOnFinished(delegate
			{
				base.localUser.OilCharge();
			});
		}
		else
		{
			UISetter.SetActive(oilTimer, active: false);
		}
	}

	public void SetPaymentControl()
	{
		UISetter.SetActive(badgeFirstPayment, base.localUser.statistics.firstPayment == 1);
		UISetter.SetActive(firstPayment, base.localUser.statistics.firstPayment < 2 && !worldMapRoot.activeSelf);
	}

	public void BadgeControl()
	{
		UISetter.SetActive(badgeWorldMap, base.localUser.badgeWorldMap || base.localUser.explorationDtbl.hasCompleteState);
		UISetter.SetActive(badgeMail, base.localUser.badgeNewMailCount > 0);
		UISetter.SetLabel(mailCount, base.localUser.badgeNewMailCount);
		UISetter.SetActive(badgeCarnival1, base.localUser.badgeCarnivalComplete[1] || base.localUser.badgeCarnival(ECarnivalCategory.Basic));
		UISetter.SetActive(badgeCarnival2, base.localUser.badgeCarnivalComplete[2] || base.localUser.badgeCarnival(ECarnivalCategory.Reward));
		UISetter.SetActive(badgeCarnival3, base.localUser.badgeCarnivalComplete[3]);
		UISetter.SetActive(badgeGroup, base.localUser.badgeGroupCount > 0);
	}

	public void SetResourceView(EGoods goods)
	{
		UISetter.SetAlpha(bulletRoot, (goods != EGoods.Bullet && goods != EGoods.FreeCash && goods != EGoods.FreeGold) ? 0f : 1f);
		UISetter.SetAlpha(oilRoot, (goods != EGoods.EventRaidChallenge) ? 0f : 1f);
		UISetter.SetActive(keyRoot, goods == EGoods.Key);
		UISetter.SetActive(challengeRoot, goods == EGoods.Challenge);
		UISetter.SetActive(worldChallengeRoot, goods == EGoods.WorldDuelTicket);
		UISetter.SetActive(sweepRoot, goods == EGoods.SweepTicket);
		UISetter.SetActive(waveDuelTicketRoot, goods == EGoods.WaveDuelTicket);
		UISetter.SetActive(cooperateBattleTicketRoot, goods == EGoods.CooperateBattleTicket);
		UISetter.SetActive(eventRaidRoot, goods == EGoods.EventRaidChallenge);
		UISetter.SetActive(goldRoot, goods != EGoods.EventRaidChallenge);
		UISetter.SetActive(otherRoot, active: false);
		if (goods != EGoods.Bullet && goods != EGoods.Key && goods != EGoods.Challenge && goods != EGoods.WorldDuelTicket && goods != EGoods.SweepTicket && goods != EGoods.WaveDuelTicket && goods != EGoods.CooperateBattleTicket && goods != EGoods.EventRaidChallenge && goods != EGoods.FreeCash && goods != EGoods.FreeGold)
		{
			UISetter.SetActive(otherRoot, active: true);
			Regulation obj = base.regulation;
			int num = (int)goods;
			GoodsDataRow goodsDataRow = obj.FindGoodsServerFieldName(num.ToString());
			UISetter.SetSprite(otherIcon, goodsDataRow.iconId);
			UISetter.SetLabel(otherCount, base.localUser.resourceList[goodsDataRow.serverFieldName]);
		}
	}

	public void SetResourceView(string id)
	{
		UISetter.SetAlpha(bulletRoot, (!(id == 5.ToString()) && !(id == 2.ToString()) && !(id == 4.ToString())) ? 0f : 1f);
		UISetter.SetAlpha(oilRoot, (!(id == 6.ToString())) ? 0f : 1f);
		UISetter.SetActive(keyRoot, id == 10.ToString());
		UISetter.SetActive(challengeRoot, id == 9.ToString());
		UISetter.SetActive(worldChallengeRoot, id == 8001.ToString());
		UISetter.SetActive(sweepRoot, id == 7.ToString());
		UISetter.SetActive(waveDuelTicketRoot, id == 57.ToString());
		UISetter.SetActive(cooperateBattleTicketRoot, id == 10000.ToString());
		UISetter.SetActive(otherRoot, active: false);
		if (id != 5.ToString() && id != 10.ToString() && id != 9.ToString() && id != 8001.ToString() && id != 7.ToString() && id != 57.ToString() && id != 10000.ToString() && id != 6.ToString() && id != 2.ToString() && id != 4.ToString())
		{
			UISetter.SetActive(otherRoot, active: true);
			GoodsDataRow goodsDataRow = base.regulation.FindGoodsServerFieldName(id);
			UISetter.SetSprite(otherIcon, goodsDataRow.iconId);
			UISetter.SetLabel(otherCount, base.localUser.resourceList[goodsDataRow.serverFieldName]);
		}
	}

	public void SetEventBattleResourceView(bool eventRaidChallenge, bool eventEnd)
	{
		UISetter.SetAlpha(bulletRoot, (!eventEnd) ? 0f : 1f);
		UISetter.SetAlpha(oilRoot, eventEnd ? 0f : 1f);
		UISetter.SetActive(keyRoot, active: false);
		UISetter.SetActive(challengeRoot, active: false);
		UISetter.SetActive(sweepRoot, active: false);
		UISetter.SetActive(waveDuelTicketRoot, active: false);
		UISetter.SetActive(cooperateBattleTicketRoot, active: false);
		UISetter.SetActive(eventRaidRoot, eventRaidChallenge && !eventEnd);
		UISetter.SetActive(goldRoot, !eventRaidChallenge || eventEnd);
		UISetter.SetActive(otherRoot, active: false);
	}

	public void SetEnableButton(EGoods goods, bool state)
	{
		if (goods == EGoods.FreeGold)
		{
			UISetter.SetButtonEnable(goldBtn, state);
		}
	}

	public void CreateFloatingText(Vector3 _position, string _text)
	{
		floatingText.initAndStartMoving(_position, _text);
	}

	public void OpenVipInfo(bool isShop)
	{
		if (vipInfo == null)
		{
			vipInfo = UIPopup.Create<UIVipInfo>("vipInfo");
			vipInfo.Init(isShop);
		}
	}

	public void OpenDiamonShop(int type = 1)
	{
		_openDiaShopType = type;
		RemoteObjectManager.instance.RequestGetCashShopList();
	}

	public void OpenAndInitDiamondShop()
	{
		if (diamonShop == null)
		{
			diamonShop = UIPopup.Create<UIDiamondShop>("DiamondShop");
		}
		diamonShop.initTabIdx = _openDiaShopType;
		diamonShop.InitAndOpenCashShop();
	}

	public void OpenAndInitFirstPaymentPopup()
	{
		if (firstPaymentPopup == null)
		{
			firstPaymentPopup = UIPopup.Create<UIFirstPaymentPopup>("FirstPaymentPopup");
			firstPaymentPopup.Init();
		}
		else
		{
			firstPaymentPopup.OnRefresh();
		}
	}

	public void OpenVipRechargePopUp(EVipRechargeType type)
	{
		if (base.localUser.resetTimeData != null && base.localUser.resetTimeData.GetRemain() <= 0.0)
		{
			base.network.RequestVipBuyCount(type);
			return;
		}
		int num = (int)type;
		VipRechargeDataRow vipRechargeDataRow = base.regulation.vipRechargeDtbl[num.ToString()];
		int idx = base.regulation.vipExpDtbl[base.regulation.vipExpDtbl.length - 1].Idx;
		int num2 = 0;
		int vipLevel = base.localUser.vipLevel;
		int maxRechargeCount = vipRechargeDataRow.GetMaxRechargeCount(vipLevel);
		if (type == EVipRechargeType.StageType1 || type == EVipRechargeType.StageType2)
		{
			string stageId = base.uiWorld.readyBattle.battleData.stageId;
			if (base.localUser.stageRechargeList.ContainsKey(stageId))
			{
				num2 = base.localUser.stageRechargeList[stageId];
			}
		}
		else if (base.localUser.resourceRechargeList.ContainsKey(num.ToString()))
		{
			num2 = base.localUser.resourceRechargeList[num.ToString()];
		}
		int num3 = maxRechargeCount - num2;
		if (base.localUser.vipLevel == idx && num3 == 0)
		{
			UISimplePopup.CreateOK(localization: true, "5635", "12014", null, "1001");
		}
		else if (base.localUser.vipLevel < vipRechargeDataRow.startVip || num3 == 0)
		{
			UISimplePopup.CreateBool(localization: true, "5635", "12006", null, "5348", "1000").onClick = delegate(GameObject sender)
			{
				string text = sender.name;
				if (text == "OK")
				{
					UIManager.instance.world.mainCommand.OpenDiamonShop();
				}
			};
		}
		else if (purchasePopup == null)
		{
			UIResourcePurchasePopup uIResourcePurchasePopup = UIPopup.Create<UIResourcePurchasePopup>("ResourcePurchasePopup");
			uIResourcePurchasePopup.initData(vipRechargeDataRow);
			purchasePopup = uIResourcePurchasePopup.gameObject;
		}
	}

	private IEnumerator NoticeCheckRoutine()
	{
		UISetter.SetActive(notiBlock, active: true);
		string[] blindList = Utility.GetBlineNoticeIdxList();
		List<Protocols.NoticeData> eventList = base.localUser.FindNoticeList(ENoticeType.Event);
		for (int i = 0; i < eventList.Count; i++)
		{
			UINotice noticePopUp = UIPopup.Create<UINotice>("UINotice");
			noticePopUp.Init(eventList[i]);
			base.uiWorld.noticePopUp = noticePopUp.gameObject;
			while (base.uiWorld.noticePopUp != null)
			{
				yield return null;
			}
		}
		List<Protocols.NoticeData> webViewList = base.localUser.FindNoticeList(ENoticeType.WebView);
		for (int j = 0; j < webViewList.Count; j++)
		{
			UIWebviewPopup webviewPopUp = UIPopup.Create<UIWebviewPopup>("UIWebView");
			webviewPopUp.Init(webViewList[j].link);
			base.uiWorld.noticePopUp = webviewPopUp.gameObject;
			while (base.uiWorld.noticePopUp != null)
			{
				yield return null;
			}
		}
		UISetter.SetActive(notiBlock, active: false);
		yield return true;
	}

	public void SetChatPanelDepth(int depth)
	{
		if (!(chatRootPanel == null))
		{
			chatRootPanel.depth = depth;
		}
	}

	public void ResetChatPanelDepth()
	{
		if (!(chatRootPanel == null))
		{
			chatRootPanel.depth = campDepth;
		}
	}

	public void ResetGuildName()
	{
		if (!(user == null))
		{
			UISetter.SetLabel(user.guildName, (!GameSetting.instance.guildName) ? string.Empty : (string.IsNullOrEmpty(base.localUser.guildName) ? Localization.Get("7180") : base.localUser.guildName));
		}
	}

	private void OpenWorldMapNavigation()
	{
		UISetter.SetActive(worldMapNaviRoot, !worldMapNaviRoot.activeSelf);
		UISetter.SetSprite(worldMapNaviBtn, (!worldMapNaviRoot.activeSelf) ? "qm-up-button" : "qm-down-button");
		uiSideMenu.CloseMenu();
		if (worldMapNaviRoot.activeSelf)
		{
			m_SecondaryButton.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
		}
		else
		{
			m_SecondaryButton.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
		}
	}

	public void CloseWorldMapNavigation()
	{
		if (worldMapNaviRoot.activeSelf)
		{
			UISetter.SetActive(worldMapNaviRoot, active: false);
			UISetter.SetSprite(worldMapNaviBtn, "qm-up-button");
			m_SecondaryButton.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
		}
	}

	private void SetWorldMapNaviList()
	{
		List<WorldMapDataRow> list = base.regulation.worldMapDtbl.FindAll((WorldMapDataRow row) => row.id != "0");
		worldMapNaviListView.Init(list, "WorldMapNavi_");
		UISetter.SetActive(worldMapNaviRoot, active: false);
		UISetter.SetSprite(worldMapNaviBtn, "qm-up-button");
	}
}
