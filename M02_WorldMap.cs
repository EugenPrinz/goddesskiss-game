using System.Collections;
using System.Collections.Generic;
using Cache;
using DialoguerCore;
using Facebook.Unity;
using Shared.Regulation;
using UnityEngine;

public class M02_WorldMap : MonoBehaviour
{
	private static RoLocalUser _roUser;

	private static Regulation _reg;

	private static bool noticeState;

	public bool shouldShowFps = true;

	public GameObject tutorial;

	public GameObject block;

	private UIManager _uiManager;

	private UIManager.World _uiWorld;

	private RoLocalUser _localUser;

	private Regulation _regulation;

	private EventBattleScenarioDataRow scenarioData;

	private UIQuitPopup quitPopup;

	public UIAtlas commanderAtlas;

	public UIAtlas commanderAtlas_2;

	public UIAtlas skillIconAtals;

	public UIAtlas unitAtlas;

	public UIAtlas unitAtlas_2;

	public UIAtlas battleCommanderUnitAtlas;

	public UIAtlas costumeIcon_1Atlas;

	public UIAtlas costumeIcon_2Atlas;

	public UIAtlas costumeIcon_3Atlas;

	public UIAtlas costumeIcon_4Atlas;

	public UIAtlas costumeIcon_5Atlas;

	public UIAtlas iconAtlas;

	public UIAtlas gachaBannerAtlas;

	public UIAtlas equipmentAtlas;

	public UIAtlas dormitoryTheme_1Atlas;

	public UIAtlas dormitoryCostume_1Atlas;

	private int VipShopRemainTime;

	public static string KEY => "|GK[";

	private IEnumerator Start()
	{
		if (!AssetBundleManager.HasAssetBundle("CommanderAtlas.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("CommanderAtlas.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("CommanderAtlas_2.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("CommanderAtlas_2.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("SkillIcon.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("SkillIcon.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("Unit.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("Unit.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("Unit_2.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("Unit_2.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("BattleCommanderUnit.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("BattleCommanderUnit.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("CostumeIcon_1.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("CostumeIcon_1.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("CostumeIcon_2.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("CostumeIcon_2.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("CostumeIcon_3.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("CostumeIcon_3.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("CostumeIcon_4.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("CostumeIcon_4.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("CostumeIcon_5.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("CostumeIcon_5.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("Icon.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("Icon.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("GachaBannerAtlas.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("GachaBannerAtlas.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("EquipmentAtlas.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("EquipmentAtlas.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("DormitoryTheme_1.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("DormitoryTheme_1.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("DormitoryCostume_1.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("DormitoryCostume_1.assetbundle"));
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
		if (AssetBundleManager.HasAssetBundle("SkillIcon.assetbundle"))
		{
			skillIconAtals.replacement = AssetBundleManager.GetObjectFromAssetBundle("SkillIcon.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			skillIconAtals.spriteMaterial = Resources.Load<Material>("Atlas/SkillIcon");
		}
		if (AssetBundleManager.HasAssetBundle("Unit.assetbundle"))
		{
			unitAtlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("Unit.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			unitAtlas.spriteMaterial = Resources.Load<Material>("Atlas/Unit");
		}
		if (AssetBundleManager.HasAssetBundle("Unit_2.assetbundle"))
		{
			unitAtlas_2.replacement = AssetBundleManager.GetObjectFromAssetBundle("Unit_2.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			unitAtlas_2.spriteMaterial = Resources.Load<Material>("Atlas/Unit_2");
		}
		if (AssetBundleManager.HasAssetBundle("BattleCommanderUnit.assetbundle"))
		{
			battleCommanderUnitAtlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("BattleCommanderUnit.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			battleCommanderUnitAtlas.spriteMaterial = Resources.Load<Material>("Atlas/BattleCommanderUnit");
		}
		if (AssetBundleManager.HasAssetBundle("CostumeIcon_1.assetbundle"))
		{
			costumeIcon_1Atlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("CostumeIcon_1.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			costumeIcon_1Atlas.spriteMaterial = Resources.Load<Material>("Atlas/CostumeIcon_1");
		}
		if (AssetBundleManager.HasAssetBundle("CostumeIcon_2.assetbundle"))
		{
			costumeIcon_2Atlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("CostumeIcon_2.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			costumeIcon_2Atlas.spriteMaterial = Resources.Load<Material>("Atlas/CostumeIcon_2");
		}
		if (AssetBundleManager.HasAssetBundle("CostumeIcon_3.assetbundle"))
		{
			costumeIcon_3Atlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("CostumeIcon_3.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			costumeIcon_3Atlas.spriteMaterial = Resources.Load<Material>("Atlas/CostumeIcon_3");
		}
		if (AssetBundleManager.HasAssetBundle("CostumeIcon_4.assetbundle"))
		{
			costumeIcon_4Atlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("CostumeIcon_4.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			costumeIcon_4Atlas.spriteMaterial = Resources.Load<Material>("Atlas/CostumeIcon_4");
		}
		if (AssetBundleManager.HasAssetBundle("CostumeIcon_5.assetbundle"))
		{
			costumeIcon_5Atlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("CostumeIcon_5.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			costumeIcon_5Atlas.spriteMaterial = Resources.Load<Material>("Atlas/CostumeIcon_5");
		}
		if (AssetBundleManager.HasAssetBundle("Icon.assetbundle"))
		{
			iconAtlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("Icon.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			iconAtlas.spriteMaterial = Resources.Load<Material>("Atlas/Icon");
		}
		if (AssetBundleManager.HasAssetBundle("GachaBannerAtlas.assetbundle"))
		{
			gachaBannerAtlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("GachaBannerAtlas.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			gachaBannerAtlas.spriteMaterial = Resources.Load<Material>("Atlas/GachaBannerAtlas");
		}
		if (AssetBundleManager.HasAssetBundle("EquipmentAtlas.assetbundle"))
		{
			equipmentAtlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("EquipmentAtlas.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			equipmentAtlas.spriteMaterial = Resources.Load<Material>("Atlas/EquipmentAtlas");
		}
		if (AssetBundleManager.HasAssetBundle("DormitoryTheme_1.assetbundle"))
		{
			dormitoryTheme_1Atlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("DormitoryTheme_1.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			dormitoryTheme_1Atlas.spriteMaterial = Resources.Load<Material>("Atlas/DormitoryTheme_1");
		}
		if (AssetBundleManager.HasAssetBundle("DormitoryCostume_1.assetbundle"))
		{
			dormitoryCostume_1Atlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("DormitoryCostume_1.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			dormitoryCostume_1Atlas.spriteMaterial = Resources.Load<Material>("Atlas/DormitoryCostume_1");
		}
		UISetter.SetActive(block, active: false);
		Utility.SetBlindNoticeResetCheck();
		UIPopup.InitUIPopup();
		CacheManager.instance.StartUp();
		yield return null;
		ForwardBackKeyEvent.Lock();
		UIFade.Out(0f);
		_uiManager = UIManager.instance;
		_uiWorld = _uiManager.world;
		_localUser = RemoteObjectManager.instance.localUser;
		_regulation = RemoteObjectManager.instance.regulation;
		VipShopRemainTime = _localUser.statistics.vipShopResetTime;
		if (!RemoteObjectManager.instance.bLogin)
		{
			yield return StartCoroutine(RemoteObjectManager.instance.RequestDBVersionCheck());
			yield return StartCoroutine(PatchManager.Instance.FileDownLoad(null, null, bUpdate: false));
			RemoteObjectManager.instance.regulation = Regulation.FromLocalResources();
			new GameObject("GEAnimSystem").AddComponent<GEAnimSystemNGUI>();
			if (!string.IsNullOrEmpty(PlayerPrefs.GetString("MemberID")) && !string.IsNullOrEmpty(PlayerPrefs.GetString("MemberPW")))
			{
				RemoteObjectManager.instance.RequestSignIn(PlayerPrefs.GetString("MemberID"), PlayerPrefs.GetString("MemberPW"));
			}
			else if (string.IsNullOrEmpty(PlayerPrefs.GetString("GuestID")))
			{
				RemoteObjectManager.instance.RequestGuestSignUp();
			}
			else
			{
				RemoteObjectManager.instance.RequestGuestSignIn(PlayerPrefs.GetString("GuestID"));
			}
			yield return new WaitForSeconds(1f);
			RemoteObjectManager.instance.RequestLogin();
		}
		yield return StartCoroutine(_uiWorld.InitCoroutine());
		UIManager.RegisterOnClickDelegate(_uiWorld, this, "_OnClick");
		ForwardBackKeyEvent.Unlock();
		_roUser = RemoteObjectManager.instance.localUser;
		_reg = RemoteObjectManager.instance.regulation;
		GameBillingManager.init();
		_uiWorld.CloseAll();
		_uiWorld.mainCommand.Set();
		yield return null;
		_InitLink();
		StartCoroutine(_UpdateLogic());
		GameObject loading = GameObject.Find("Loading");
		if (loading != null)
		{
			loading.GetComponent<UILoading>().Out();
		}
		UIFade.In(1.5f);
		BattleData battleData = BattleData.Get();
		if (battleData != null)
		{
			_InitFromBattleData(battleData);
		}
		else
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
		}
		if (tutorial != null)
		{
			UISetter.SetActive(tutorial, active: true);
			_localUser.isEnableTutorial = true;
		}
		StartCoroutine(NoticeCheckRoutine());
		UIManager.instance.world.onStart = true;
	}

	private IEnumerator NoticeCheckRoutine()
	{
		yield return null;
		while (tutorial != null && tutorial.activeSelf)
		{
			yield return null;
		}
		_localUser.isEnableTutorial = false;
		UIInteraction.playVoice = true;
		if (!noticeState)
		{
			noticeState = true;
			UISetter.SetActive(block, active: true);
			string[] blindList = Utility.GetBlineNoticeIdxList();
			List<Protocols.NoticeData> eventList = _localUser.FindNoticeList(ENoticeType.Event);
			for (int i = 0; i < eventList.Count; i++)
			{
				bool blind = false;
				for (int k = 0; k < blindList.Length; k++)
				{
					if (!string.IsNullOrEmpty(blindList[k]) && int.Parse(blindList[k]) == eventList[i].idx)
					{
						blind = true;
					}
				}
				if (!blind)
				{
					UINotice uINotice = UIPopup.Create<UINotice>("UINotice");
					uINotice.Init(eventList[i]);
					_uiManager.world.noticePopUp = uINotice.gameObject;
				}
				while (_uiManager.world.noticePopUp != null)
				{
					yield return null;
				}
			}
			RemoteObjectManager.instance.RequestGetPlugEventInfo();
			List<Protocols.NoticeData> webViewList = _localUser.FindNoticeList(ENoticeType.WebView);
			for (int j = 0; j < webViewList.Count; j++)
			{
				UIWebviewPopup webviewPopUp = UIPopup.Create<UIWebviewPopup>("UIWebView");
				webviewPopUp.Init(webViewList[j].link);
				_uiManager.world.noticePopUp = webviewPopUp.gameObject;
				while (_uiManager.world.noticePopUp != null)
				{
					yield return null;
				}
			}
			if (RemoteObjectManager.instance.GetLanguageCode() == "ko")
			{
				GLink.sharedInstance().setChannelCode("ko");
			}
			else if (RemoteObjectManager.instance.GetLanguageCode() == "tw")
			{
				GLink.sharedInstance().setChannelCode("zh_TW");
			}
			else
			{
				GLink.sharedInstance().setChannelCode("en");
			}
			GLink.sharedInstance().executeHome();
			GLink.sharedInstance().syncGameUserId((_localUser.channel != 1) ? ("G_" + _localUser.uno) : ("K_" + _localUser.uno));
			_localUser.isShowGLink = true;
			while (_localUser.isShowGLink)
			{
				yield return null;
			}
			UISetter.SetActive(block, active: false);
			RemoteObjectManager.instance.RequestDailyBonusCheck();
		}
		if (UIPopup.openedPopups[0] is UICamp)
		{
			UIManager.instance.world.mainCommand.EnableMainCommanderSpine(isEnable: true);
		}
		yield return true;
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

	private void _InitFromBattleData(BattleData battleData)
	{
		EBattleType type = battleData.type;
		if (type == EBattleType.Plunder && !battleData.isReplayMode && battleData.isWin)
		{
			WorldMapStageDataRow worldMapStageDataRow = _reg.worldMapStageDtbl[battleData.stageId];
			RoWorldMap roWorldMap = _roUser.FindWorldMap(worldMapStageDataRow.worldMapId);
			RoWorldMap.Stage stage = roWorldMap.FindStage(battleData.stageId);
			roWorldMap.StageClear(battleData.stageId);
		}
		if (battleData.move == EBattleResultMove.Challenge)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			_uiWorld.camp.GoNavigation("Duel");
		}
		else if (battleData.move == EBattleResultMove.MyTown)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			if (battleData.type == EBattleType.EventBattle && IsShowFirstEventScenario(battleData.eventId, battleData.eventLevel))
			{
				StartEventBattleScenario();
			}
		}
		else if (battleData.move == EBattleResultMove.WorldMap)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp(isLastStageReset: false);
			_uiWorld.worldMap.InitAndOpenLastStageWorldMap();
		}
		else if (battleData.move == EBattleResultMove.NextStage)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp(isLastStageReset: false);
			string text = null;
			RoWorldMap roWorldMap2 = _localUser.FindWorldMapByStage(battleData.stageId);
			RoWorldMap.Stage stage2 = roWorldMap2.FindNextStage(battleData.stageId);
			if (stage2 == null)
			{
				int num = int.Parse(battleData.stageId);
				if (num < _localUser.lastClearStage)
				{
					num++;
					roWorldMap2 = _localUser.FindWorldMapByStage(num.ToString());
					stage2 = roWorldMap2.FindStage(num.ToString());
				}
			}
			if (stage2 != null)
			{
				text = stage2.id;
				_localUser.lastPlayStage = int.Parse(text);
			}
			_uiWorld.worldMap.InitAndOpenWorldMap(roWorldMap2.id, text);
		}
		else if (battleData.move == EBattleResultMove.Guild)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			_uiWorld.camp.GoNavigation("Guild");
		}
		else if (battleData.move == EBattleResultMove.Raid)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			_uiWorld.camp.GoNavigation("Raid");
		}
		else if (battleData.move == EBattleResultMove.Situation)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			_uiWorld.camp.GoNavigation("Situation");
		}
		else if (battleData.move == EBattleResultMove.Annihilation)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			_uiWorld.camp.GoNavigation("Loot");
		}
		else if (battleData.move == EBattleResultMove.NextAnnihilation)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			RemoteObjectManager.instance.RequestGetAnnihilationMapInfo(1);
		}
		else if (battleData.move == EBattleResultMove.WaveBattle)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			_uiWorld.camp.GoNavigation("WaveBattle");
		}
		else if (battleData.move == EBattleResultMove.WaveDuel)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			_uiWorld.camp.GoNavigation("WaveDuel");
		}
		else if (battleData.move == EBattleResultMove.WorldDuel)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			_uiWorld.camp.GoNavigation("WorldChallenge");
		}
		else if (battleData.move == EBattleResultMove.Conquest)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			_uiWorld.camp.GoNavigation("Guild");
		}
		else if (battleData.move == EBattleResultMove.CooperateBattle)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			_uiWorld.camp.GoNavigation("Guild");
		}
		else if (battleData.move == EBattleResultMove.EventBattle)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			RemoteObjectManager.instance.RequestGetEventBattleData(battleData.eventId);
			if (IsShowFirstEventScenario(battleData.eventId, battleData.eventLevel))
			{
				StartEventBattleScenario();
			}
		}
		else if (battleData.move == EBattleResultMove.EventBattleRetry)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			RemoteObjectManager.instance.RequestGetEventBattleData(battleData.eventId, battleData.eventLevel);
		}
		else if (battleData.move == EBattleResultMove.EventRaid)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			RemoteObjectManager.instance.RequestGetEventBattleData(battleData.returnEventIdx);
		}
		else if (battleData.move == EBattleResultMove.EventRaidRetry)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			RemoteObjectManager.instance.RequestGetEventBattleData(battleData.returnEventIdx, 0);
		}
		else if (battleData.move == EBattleResultMove.CommanderDetailScenarioInfo)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			_uiWorld.camp.GoNavigation("CommanderDetail", _localUser.currScenario.commanderId.ToString(), "Scenario");
		}
		else if (battleData.move == EBattleResultMove.InfinityBattle)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			_uiWorld.camp.GoNavigation("InfinityBattle");
		}
		else if (battleData.move == EBattleResultMove.InfinityBattleRetry)
		{
			_uiWorld.camp.Init();
			_uiWorld.camp.OpenCamp();
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
			RemoteObjectManager.instance.RequestInfinityBattleInformation(0, battleData.stageId);
		}
		UIManager.instance.world.onStart = true;
	}

	public void OnPressedQuitGame()
	{
		if (UIManager.instance.world.onStart && (!(tutorial != null) || !tutorial.activeSelf) && quitPopup == null)
		{
			quitPopup = UIPopup.Create<UIQuitPopup>("QuitPopup");
		}
	}

	private IEnumerator _UpdateLogic()
	{
		List<RoBuilding> buildingList = new List<RoBuilding>(_roUser.buildingDict.Values);
		RemoteObjectManager rom = RemoteObjectManager.instance;
		while (true)
		{
			if (_localUser.statistics.vipShopResetTime == 1)
			{
				_localUser.statistics.vipShop = 0;
				UIManager.instance.RefreshOpenedUI();
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void _InitLink()
	{
		_uiManager.RegisterLinkDelegate(this, "_InitAndOpen", string.Empty);
		_uiWorld.panelList.ForEach(delegate(UIPanelBase panel)
		{
			_uiManager.RegisterLinkDelegate(panel, "InitAndOpen", string.Empty);
		});
	}

	private void _InitAndOpenAchievement()
	{
		UIReward achievement = _uiWorld.achievement;
		achievement.Set(EReward.Achievement);
		achievement.OpenPopup();
	}

	private void _InitAndOpenMission()
	{
		UIReward mission = _uiWorld.mission;
		mission.Set(EReward.DailyMission);
		mission.OpenPopup();
	}

	private void _InitAndOpenMail()
	{
		UIReward mail = _uiWorld.mail;
		mail.Set(EReward.Mail);
		mail.OpenPopup();
	}

	private void OnDestroy()
	{
	}

	private bool IsShowFirstEventScenario(int eventId, int eventLevel)
	{
		scenarioData = _regulation.eventBattleScenarioDtbl.Find((EventBattleScenarioDataRow row) => row.eventIdx == eventId.ToString() && row.timing == EventScenarioTimingType.AfterBattleResult && row.eventType == eventLevel.ToString());
		if (scenarioData == null)
		{
			return false;
		}
		int playTurn = scenarioData.playTurn;
		int lastShowEventScenarioPlayTurn = RemoteObjectManager.instance.localUser.lastShowEventScenarioPlayTurn;
		if (playTurn > lastShowEventScenarioPlayTurn)
		{
			return true;
		}
		return false;
	}

	private void StartEventBattleScenario()
	{
		if (scenarioData != null)
		{
			ClassicRpgManager dialogMrg = UIManager.instance.world.dialogMrg;
			if (dialogMrg != null)
			{
				dialogMrg.StartEventScenario();
				dialogMrg.InitScenarioDialogue(scenarioData.scenarioIdx, DialogueType.Event);
			}
		}
	}
}
