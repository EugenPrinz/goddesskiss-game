using System;
using System.Collections;
using Cache;
using UnityEngine;

public class UICamp : UIPopup
{
	public UIDefaultListView buildingListView;

	public UIScrollView mapScrollView;

	public string selectedBuildingId { get; private set; }

	public EBuilding selectedBuilding => (EBuilding)Enum.Parse(typeof(EBuilding), selectedBuildingId);

	private void OnPress()
	{
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		string id = (selectedBuildingId = buildingListView.GetPureId(text));
		EBuildingState eBuildingState = IsBuildingState(id);
		sender.GetComponent<UIBuilding>().PlayAnimation();
		if (eBuildingState == EBuildingState.Lock || eBuildingState == EBuildingState.UpgradeComplete || eBuildingState == EBuildingState.Open)
		{
			switch (eBuildingState)
			{
			case EBuildingState.Lock:
				if (text == "Building-VipShop")
				{
					GoOpenPopup();
				}
				else
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5897"));
				}
				break;
			case EBuildingState.UpgradeComplete:
				base.network.RequestBuildingUpgradeCompleteOut(selectedBuilding);
				break;
			case EBuildingState.Open:
				StartCoroutine(sender.GetComponent<UIBuilding>().PlayOpenEffect());
				break;
			}
			return;
		}
		switch (text)
		{
		case "Building-Challenge":
			GoChallenge();
			return;
		case "Building-Raid":
			GoRaid();
			return;
		case "Building-WarMemorial":
			GoWarMemorial();
			return;
		case "Building-Gacha":
			GoGacha();
			return;
		case "Building-Storage":
			GoStorage();
			return;
		case "Building-Headquarters":
			GoHeadQuarter();
			return;
		case "Building-MetroBank":
			GoMetrkBank();
			return;
		case "Building-SituationRoom":
			GoSituationRoom();
			return;
		case "Building-Guild":
			GoGuild();
			return;
		case "Building-BestChallenge":
			return;
		case "Building-BlackMarket":
			GoSecretShop();
			return;
		case "Building-Loot":
			GoAnnihilationMap();
			return;
		case "Building-VipShop":
			GoVipShop();
			return;
		case "Building-VipGacha":
			GoVipGacha();
			return;
		case "Building-EventBattle":
			GoEventList();
			return;
		case "Building-WaveBattle":
			GoWaveBattle();
			return;
		case "Building-Laboratory":
			OpenLaboratoryBuilding();
			return;
		case "Building-WorldChallenge":
			GoWorldChallenge();
			return;
		case "Building-InfinityBattle":
			GoInfinityBattle();
			return;
		}
		if (buildingListView.Contains(text))
		{
			buildingListView.SetSelection(id, selected: true);
		}
	}

	private void GoDuel()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.Challenge);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (uIBuilding.state == EBuildingState.Lock)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("16049", roBuilding.firstLevelReg.userLevel));
		}
		else
		{
			uIBuilding.DestroyOpenEffect();
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
			base.network.DuelRankingList();
		}
	}

	private void GoChallenge()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.Challenge);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (uIBuilding.state == EBuildingState.Lock)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("16049", roBuilding.firstLevelReg.userLevel));
		}
		else
		{
			uIBuilding.DestroyOpenEffect();
			UIPopup.Create<UISelectChallengePopup>("SelectChallengePopup").OpenPopup();
		}
	}

	private void GoWorldChallenge()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.WorldChallenge);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (uIBuilding.state == EBuildingState.Lock)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("16049", roBuilding.firstLevelReg.userLevel));
		}
		else
		{
			uIBuilding.DestroyOpenEffect();
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
			base.network.RequestWorldDuelInformation();
		}
	}

	private void GoWaveDuel()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.Challenge);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (uIBuilding.state == EBuildingState.Lock)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("16049", roBuilding.firstLevelReg.userLevel));
		}
		else
		{
			uIBuilding.DestroyOpenEffect();
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
			base.network.WaveDuelRankingList();
		}
	}

	private void GoRaid()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.Raid);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (uIBuilding.state == EBuildingState.Lock)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("16049", roBuilding.firstLevelReg.userLevel));
		}
		else
		{
			uIBuilding.DestroyOpenEffect();
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
			base.network.RaidRankingList();
		}
	}

	private void GoWarMemorial()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: false);
		base.network.RequestMission();
	}

	private void GoGacha()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: false);
		base.network.RequestGachaInformation();
	}

	private void GoHeadQuarter()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: false);
		base.uiWorld.headQuarters.InitAndOpenHeadQuarters();
	}

	private void GoMetrkBank()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.MetroBank);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (uIBuilding.state == EBuildingState.Lock)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("16049", roBuilding.firstLevelReg.userLevel));
		}
		else
		{
			uIBuilding.DestroyOpenEffect();
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
			UIManager.instance.world.metroBank.InitAndOpenMetroBank();
		}
	}

	private void GoStorage()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: false);
		base.uiWorld.storage.InitAndOpenStorage();
	}

	private void GoAlarm()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: false);
		base.network.RequestCheckAlarm();
	}

	private void GoSituationRoom()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.SituationRoom);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (uIBuilding.state == EBuildingState.Lock)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("16049", roBuilding.firstLevelReg.userLevel));
		}
		else
		{
			uIBuilding.DestroyOpenEffect();
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
			base.network.RequestSituationInformation();
		}
	}

	public void GoBlackMarket(EShopType type)
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.BlackMarket);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (uIBuilding.state == EBuildingState.Lock)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("16049", roBuilding.firstLevelReg.userLevel));
		}
		else
		{
			uIBuilding.DestroyOpenEffect();
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
			base.uiWorld.secretShop.Init(type);
		}
	}

	private void GoGuild()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.Guild);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (uIBuilding.state == EBuildingState.Lock)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("16049", roBuilding.firstLevelReg.userLevel));
			return;
		}
		uIBuilding.DestroyOpenEffect();
		UIManager.instance.EnableCameraTouchEvent(isEnable: false);
		if (base.localUser.IsExistGuild())
		{
			base.network.RequestGuildInfoAndMember();
		}
		else
		{
			base.network.RequestGuildList();
		}
	}

	private void GoSecretShop()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.BlackMarket);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (uIBuilding.state == EBuildingState.Lock)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("16049", roBuilding.firstLevelReg.userLevel));
		}
		else
		{
			uIBuilding.DestroyOpenEffect();
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
			base.uiWorld.secretShop.Init(EShopType.BasicShop);
		}
	}

	private void GoAnnihilationMap()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.Loot);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (uIBuilding.state == EBuildingState.Lock)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("16049", roBuilding.firstLevelReg.userLevel));
		}
		else
		{
			uIBuilding.DestroyOpenEffect();
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
			base.network.RequestGetAnnihilationMapInfo();
		}
	}

	private void GoVipShop()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.VipShop);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (roBuilding != null && !(uIBuilding == null))
		{
			uIBuilding.DestroyOpenEffect();
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
			base.uiWorld.vipShop.Init(EShopType.VipShop);
		}
	}

	private void GoInfinityBattle()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: false);
		base.network.RequestInfinityBattleInformation(0, string.Empty);
	}

	private void GoOpenPopup()
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		if (localUser == null)
		{
			return;
		}
		int purchase_value = int.Parse(base.regulation.defineDtbl["SKY_SHOP_CREATE_CASH"].value);
		UISimplePopup.CreateBool(localization: false, Localization.Get("22004"), Localization.Format("22005", purchase_value), null, Localization.Get("1002"), Localization.Get("1000")).onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (localUser.cash < purchase_value)
				{
					OpenPopup_GotoDiamondShop();
				}
				else
				{
					base.network.RequestBuyVipShop();
				}
			}
		};
	}

	private void OpenPopup_GotoDiamondShop()
	{
		UISimplePopup.CreateBool(localization: true, "5735", "5736", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				base.uiWorld.mainCommand.OpenDiamonShop();
			}
		};
	}

	private void GoVipGacha()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.VipShop);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (roBuilding != null && !(uIBuilding == null))
		{
			uIBuilding.DestroyOpenEffect();
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
			base.uiWorld.vipGacha.Init();
		}
	}

	private void GoWaveBattle()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.WaveBattle);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (roBuilding != null && !(uIBuilding == null))
		{
			if (uIBuilding.state == EBuildingState.Lock)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Format("16049", roBuilding.firstLevelReg.userLevel));
			}
			else
			{
				uIBuilding.DestroyOpenEffect();
				UIManager.instance.EnableCameraTouchEvent(isEnable: false);
				base.network.RequestWaveBattleList();
			}
		}
	}

	private void GoLaboratory()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.Laboratory);
		if (roBuilding == null)
		{
			return;
		}
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (!(uIBuilding == null))
		{
			if (uIBuilding.state == EBuildingState.Lock)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Format("16049", roBuilding.firstLevelReg.userLevel));
			}
			else
			{
				uIBuilding.DestroyOpenEffect();
				base.uiWorld.laboratory.InitAndOpen();
			}
		}
	}

	private void OpenLaboratoryBuilding()
	{
		UIPopup.Create<UISelectResearchPopup>("SelectResearchPopup").OpenPopup();
	}

	private void GoEventList()
	{
		RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.EventBattle);
		UIBuilding uIBuilding = roBuilding.GetUIBuilding();
		if (roBuilding != null && !(uIBuilding == null))
		{
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
			base.network.RequestGetEventBattleList();
		}
	}

	private IEnumerator GoCommanderDetail(string commanderID, string type)
	{
		if (UICamera.current != null)
		{
			UICamera.current.eventReceiverMask = LayerMask.NameToLayer("Nothing");
		}
		bool prevIsPaused = CacheManager.instance.SoundCache.isPaused;
		CacheManager.instance.SoundCache.isPaused = true;
		GoHeadQuarter();
		yield return null;
		GameObject evnt = new GameObject
		{
			name = "Commander_" + commanderID
		};
		base.uiWorld.headQuarters.OnClick(evnt);
		if (type == "Oparts")
		{
			evnt.name = "Toggle_item";
		}
		else
		{
			evnt.name = "ScenarioBtn";
		}
		base.uiWorld.commanderDetail.OnClick(evnt);
		UnityEngine.Object.DestroyImmediate(evnt);
		yield return null;
		CacheManager.instance.SoundCache.isPaused = prevIsPaused;
		if (UICamera.current != null)
		{
			UICamera.current.eventReceiverMask = LayerMask.NameToLayer("Everything");
		}
	}

	public void Set()
	{
		buildingListView.CollectExistItem("Building-");
		buildingListView.itemList.ForEach(delegate(UIItemBase item)
		{
			UIBuilding uIBuilding = item as UIBuilding;
			uIBuilding.Set(base.localUser.FindBuilding(buildingListView.GetPureId(item.name)));
		});
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		Set();
	}

	public void Init()
	{
		Set();
		Open();
	}

	public void OpenCamp(bool isLastStageReset = true)
	{
		if (isLastStageReset)
		{
			base.localUser.lastPlayStage = -1;
		}
		base.uiWorld.mainCommand.OnCamp();
	}

	private EBuildingState IsBuildingState(string _id)
	{
		UIItemBase uIItemBase = buildingListView.FindItem(_id);
		UIBuilding uIBuilding = uIItemBase as UIBuilding;
		return uIBuilding.state;
	}

	private RoBuilding GetBuilding(EBuilding type)
	{
		return base.localUser.FindBuilding(type);
	}

	public void GoNavigation(string destination)
	{
		if (destination.StartsWith("World-"))
		{
			string text = destination.Replace("World-", string.Empty);
			if (base.localUser.lastClearStage < int.Parse(text) - 1)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5767"));
				return;
			}
			UISetter.SetActive(base.uiWorld.mainCommand.sideMenu, active: false);
			RoWorldMap roWorldMap = base.localUser.FindWorldMapByStage(text);
			base.uiWorld.worldMap.InitAndOpenWorldMap(roWorldMap.id, text);
			return;
		}
		switch (destination)
		{
		case "LastStage":
			UISetter.SetActive(base.uiWorld.mainCommand.sideMenu, active: false);
			base.uiWorld.worldMap.InitAndOpenWorldMap();
			break;
		case "Camp":
			break;
		case "Duel":
			GoDuel();
			break;
		case "Gacha":
			GoGacha();
			break;
		case "HeadQuarter":
			GoHeadQuarter();
			break;
		case "MetroBank":
			GoMetrkBank();
			break;
		case "Raid":
			GoRaid();
			break;
		case "Situation":
			GoSituationRoom();
			break;
		case "WarMemorial":
			GoWarMemorial();
			break;
		case "BlackMarket":
			GoBlackMarket(EShopType.BasicShop);
			break;
		case "Guild":
			GoGuild();
			break;
		case "Storage":
			GoStorage();
			break;
		case "Loot":
			GoAnnihilationMap();
			break;
		case "Shop_Bullet":
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Bullet);
			break;
		case "Shop_Diamond":
			base.uiWorld.mainCommand.OpenDiamonShop();
			break;
		case "WaveBattle":
			GoWaveBattle();
			break;
		case "WorldChallenge":
			GoWorldChallenge();
			break;
		case "WaveDuel":
			GoWaveDuel();
			break;
		case "Laboratory":
			GoLaboratory();
			break;
		case "Carnival":
			break;
		case "InfinityBattle":
			GoInfinityBattle();
			break;
		}
	}

	public void GoNavigation(string destination, string param1, string param2)
	{
		if (destination == "CommanderDetail")
		{
			StartCoroutine(GoCommanderDetail(param1, param2));
		}
	}

	protected override void OnEnablePopup()
	{
		UIManager.instance.world.mainCommand.EnableMainCommanderVoice(enable: true);
	}

	protected override void OnDisablePopup()
	{
		UIManager.instance.world.mainCommand.EnableMainCommanderVoice(enable: false);
	}
}
