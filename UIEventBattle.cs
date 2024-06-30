using System.Collections;
using System.Collections.Generic;
using DialoguerCore;
using Shared.Regulation;
using UnityEngine;

public class UIEventBattle : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GameObject AnimInfo;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UITexture goBG;

	public UIGrid gridTab;

	public UIFlipSwitch storyTab;

	public UIFlipSwitch gachaTab;

	public UIFlipSwitch battleTab;

	public GameObject commanderRoot;

	public GameObject itemRoot;

	public GameObject storyRoot;

	public GameObject gachaRoot;

	public GameObject battleRoot;

	public GameObject summonBtn;

	public GameObject summonFailed;

	public GameObject summonRoot;

	public GameObject rewardNameRoot;

	public GameObject commanderName;

	public GameObject itemName;

	public GameObject warning;

	public GameObject eventResource;

	public GameObject badge;

	public UISprite summonIcon1;

	public UISprite summonIcon2;

	public UISprite eventIcon;

	public UISprite eventLogo;

	public UIEventBattleGacha eventGacha;

	public UILabel summonResourceCount;

	public UILabel summonResourceNeedCount;

	public UILabel summonCount;

	public UILabel eventCount;

	public UILabel bossCount;

	public UICommander rewardCommander;

	public UIGoods rewardItem;

	public UIDefaultListView eventListView;

	public UISpineAnimation spineAnimation;

	public UITimer eventTimer;

	private Vector3 namePosition1 = new Vector3(-404f, -172f, 0f);

	private Vector3 namePosition2 = new Vector3(-404f, -275f, 0f);

	private UIEventRaidBattlePopup eventRaidBattle;

	private GameObject infoPopUp;

	private Dictionary<int, int> clearList;

	private int eventId;

	private int eventLevel;

	public bool eventEnd;

	private string lastClearId;

	private List<EventBattleScenarioDataRow> eventScenarioSortList = new List<EventBattleScenarioDataRow>();

	[SerializeField]
	private UIDefaultListView scenarioListView;

	private int EventType;

	private void Start()
	{
	}

	public void Init()
	{
		InitTab();
		OpenPopupShow();
	}

	private void InitTab()
	{
		eventEnd = false;
		UISetter.SetFlipSwitch(storyTab, state: false);
		UISetter.SetFlipSwitch(gachaTab, state: false);
		UISetter.SetFlipSwitch(battleTab, state: true);
		UISetter.SetActive(storyRoot, active: false);
		UISetter.SetActive(gachaRoot, active: false);
		UISetter.SetActive(battleRoot, active: true);
		UISetter.SetActive(summonBtn, active: true);
		UISetter.SetActive(summonRoot, active: false);
		UISetter.SetActive(summonFailed, active: false);
		gridTab.Reposition();
		eventListView.ResetPosition();
		base.localUser.CommanderStatusReset();
	}

	public int GetEventId()
	{
		return eventId;
	}

	public void SetEventBattle(int eventId, Protocols.EventBattleData data)
	{
		this.eventId = eventId;
		base.localUser.badgeEventRaidReward = data.rewardCntAll > 0;
		UISetter.SetActive(badge, data.rewardCnt > 0);
		clearList = data.clearList;
		SetEventData(data.eventData);
		SetRaidData(data.raidData, data.bossCnt);
		SetEventScenarioData();
		if (IsShowFirstEventScenario())
		{
			StartEventBattleScenario();
		}
	}

	private void SetEventData(Protocols.EventBattleData.EventData eventData)
	{
		eventEnd = eventData.remain < 1.0;
		base.localUser.lastShowEventScenarioPlayTurn = int.Parse(eventData.esid);
		lastClearId = eventData.efid;
		UISetter.SetActive(eventTimer, eventData.type > 0);
		EventType = eventData.type;
		if (eventData.type > 0)
		{
			TimeData timeData = TimeData.Create();
			timeData.SetByDuration(eventData.remain);
			eventTimer.Set(timeData);
			eventTimer.SetFinishString(Localization.Get("2001"));
			eventTimer.RegisterOnFinished(delegate
			{
				eventEnd = true;
				Set();
			});
		}
	}

	public void SetRaidData(Protocols.EventBattleData.RaidData raidData, int bossCount)
	{
		UISetter.SetLabel(this.bossCount, bossCount);
		if (raidData != null)
		{
			UISetter.SetActive(summonBtn, raidData.remain == 0.0);
			UISetter.SetActive(summonRoot, raidData.remain > 0.0);
		}
	}

	public void SetEventScenarioData()
	{
		List<EventBattleScenarioDataRow> list = base.regulation.FindEventScenarioList(eventId.ToString());
		if (list == null || eventScenarioSortList == null)
		{
			return;
		}
		eventScenarioSortList.Clear();
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			if (list[i].sort != 0)
			{
				eventScenarioSortList.Add(list[i]);
			}
		}
		scenarioListView.Init(eventScenarioSortList, int.Parse(lastClearId), "EventScenario-");
	}

	public void SetEventGachaData(Protocols.EventBattleGachaInfo data)
	{
		if (data != null && data.info != null)
		{
			eventGacha.Set(eventId, data);
		}
	}

	public Coroutine OpenGacha(Protocols.EventBattleGachaInfo data)
	{
		if (eventGacha == null || data == null)
		{
			return null;
		}
		return eventGacha.OpenGacha(data);
	}

	public void StartWarningEffect()
	{
		StartCoroutine(WarningEffect());
	}

	private IEnumerator WarningEffect()
	{
		UISetter.SetActive(warning, active: true);
		yield return new WaitForSeconds(2.3f);
		UISetter.SetActive(warning, active: false);
		CreateEventRaidListPopup(1);
	}

	public void Set()
	{
		List<EventBattleFieldDataRow> list = base.regulation.FindEventBattleList(eventId);
		EventBattleDataRow eventBattleDataRow = base.regulation.eventBattleDtbl[eventId.ToString()];
		eventListView.Init(list, lastClearId, eventEnd, "EventBattle-");
		UISetter.SetActive(eventLogo, eventBattleDataRow.eventLogo != "0");
		if (eventBattleDataRow.eventLogo != "0")
		{
			UISetter.SetSprite(eventLogo, eventBattleDataRow.eventLogo);
		}
		UISetter.SetActive(AnimInfo, int.Parse(eventBattleDataRow.goodsIdx) != 0);
		if (int.Parse(eventBattleDataRow.goodsIdx) != 0)
		{
			GoodsDataRow goodsDataRow = base.regulation.goodsDtbl[eventBattleDataRow.goodsIdx];
			UISetter.SetSprite(summonIcon1, goodsDataRow.iconId);
			UISetter.SetSprite(summonIcon2, goodsDataRow.iconId);
			UISetter.SetLabel(summonResourceCount, $"x{base.localUser.resourceList[goodsDataRow.serverFieldName]}");
			UISetter.SetLabel(summonResourceNeedCount, eventBattleDataRow.goodsAmount);
		}
		rewardNameRoot.transform.localPosition = ((int.Parse(eventBattleDataRow.goodsIdx) == 0) ? namePosition2 : namePosition1);
		UISetter.SetActive(commanderRoot, eventBattleDataRow.mainRewardType == ERewardType.Commander || eventBattleDataRow.mainRewardType == ERewardType.Costume);
		UISetter.SetActive(itemRoot, eventBattleDataRow.mainRewardType != ERewardType.Commander && eventBattleDataRow.mainRewardType != ERewardType.Costume);
		UISetter.SetActive(commanderName, eventBattleDataRow.mainRewardType == ERewardType.Commander || eventBattleDataRow.mainRewardType == ERewardType.Costume);
		UISetter.SetActive(itemName, eventBattleDataRow.mainRewardType != ERewardType.Commander && eventBattleDataRow.mainRewardType != ERewardType.Costume);
		if (eventBattleDataRow.mainRewardType == ERewardType.Commander)
		{
			CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[eventBattleDataRow.mainRewardIdx];
			rewardCommander.SetCommander_ForEventBattle(int.Parse(eventBattleDataRow.mainRewardIdx));
			UISetter.SetLabel(rewardCommander.nickname, commanderDataRow.nickname);
			UISetter.SetActive(rewardCommander.rankGrid, active: true);
			UISetter.SetActive(rewardCommander.thumbGroupBackground, active: true);
		}
		else if (eventBattleDataRow.mainRewardType == ERewardType.Costume)
		{
			CommanderCostumeDataRow commanderCostumeDataRow = RemoteObjectManager.instance.regulation.FindCostumeData(int.Parse(eventBattleDataRow.mainRewardIdx));
			CommanderDataRow commanderDataRow2 = RemoteObjectManager.instance.regulation.commanderDtbl[commanderCostumeDataRow.cid.ToString()];
			rewardCommander.SetCommander_ForEventBattle(commanderCostumeDataRow.cid, int.Parse(commanderCostumeDataRow.skinName));
			UISetter.SetLabel(rewardCommander.nickname, Localization.Get(commanderCostumeDataRow.name));
			UISetter.SetActive(rewardCommander.rankGrid, active: false);
			UISetter.SetActive(rewardCommander.thumbGroupBackground, active: false);
		}
		else
		{
			rewardItem.Set(eventBattleDataRow.mainRewardType, eventBattleDataRow.mainRewardIdx, eventBattleDataRow.mainRewardAmount);
		}
		if (int.Parse(eventBattleDataRow.eventPointIdx) <= 0)
		{
			UISetter.SetActive(eventResource, active: false);
		}
		else
		{
			UISetter.SetActive(eventResource, active: true);
			GoodsDataRow goodsDataRow2 = base.regulation.goodsDtbl[eventBattleDataRow.eventPointIdx];
			UISetter.SetSprite(eventIcon, goodsDataRow2.iconId);
			UISetter.SetLabel(eventCount, $"{base.localUser.resourceList[goodsDataRow2.serverFieldName]}/{goodsDataRow2.max}");
		}
		if (eventEnd)
		{
			UISetter.SetActive(summonBtn, active: false);
			UISetter.SetActive(summonRoot, active: false);
		}
		UISetter.SetActive(summonFailed, eventEnd);
		if (eventScenarioSortList == null || (eventScenarioSortList != null && eventScenarioSortList.Count == 0))
		{
			UISetter.SetActive(storyTab, active: false);
		}
		else if (eventScenarioSortList != null && eventScenarioSortList.Count > 0)
		{
			UISetter.SetActive(storyTab, active: true);
		}
		UISetter.SetActive(gachaTab, int.Parse(eventBattleDataRow.eventPointIdx) > 0 && eventBattleDataRow.gachaOneTimeAmount != 0);
		gridTab.Reposition();
		base.uiWorld.mainCommand.SetEventBattleResourceView(int.Parse(eventBattleDataRow.goodsIdx) != 0, eventEnd);
	}

	public void StartEventReadyBattle(int eventLevel)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		EventBattleFieldDataRow eventBattleFieldDataRow = base.regulation.FindEventBattle(eventId, eventLevel);
		int eventClear = 0;
		if (clearList.ContainsKey(eventLevel))
		{
			eventClear = clearList[eventLevel];
		}
		BattleData battleData = BattleData.Create(EBattleType.EventBattle);
		battleData.eventId = eventId;
		battleData.eventLevel = eventLevel;
		battleData.eventClear = eventClear;
		battleData.defender = RoUser.CreateEventBattleEnemy("EventEnemy-", string.Empty, RoTroop.CreateEnemy(eventBattleFieldDataRow.enemy));
		UIManager.instance.world.readyBattle.InitAndOpenReadyBattle(battleData);
	}

	private bool EventRaidSummonCheck()
	{
		if (eventEnd)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6601"));
			return false;
		}
		EventBattleDataRow eventBattleDataRow = base.regulation.eventBattleDtbl[eventId.ToString()];
		GoodsDataRow goodsDataRow = base.regulation.goodsDtbl[eventBattleDataRow.goodsIdx];
		int num = base.localUser.resourceList[goodsDataRow.serverFieldName];
		if (eventBattleDataRow.goodsAmount > num)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("6602", Localization.Get(goodsDataRow.name)));
			return false;
		}
		return true;
	}

	private void EventRaidSummon()
	{
		EventBattleDataRow eventBattleDataRow = base.regulation.eventBattleDtbl[eventId.ToString()];
		GoodsDataRow goodsDataRow = base.regulation.goodsDtbl[eventBattleDataRow.goodsIdx];
		int num = base.localUser.resourceList[goodsDataRow.serverFieldName];
		UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), Localization.Format("6604", Localization.Get(goodsDataRow.name), eventBattleDataRow.goodsAmount), Localization.Format("6605", num, num - eventBattleDataRow.goodsAmount), Localization.Get("1304"), Localization.Get("1305")).onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				base.network.RequestEventRaidSummon(eventId);
			}
		};
	}

	public void UpdateEventRaid(string bid, Protocols.EventRaidData data)
	{
		if (eventRaidBattle != null)
		{
			eventRaidBattle.UpdateEventRaid(bid, data);
		}
	}

	public void RefreshEventRaid()
	{
		if (eventRaidBattle != null)
		{
			eventRaidBattle.OnRefresh();
		}
	}

	public void CloseEventRaid()
	{
		if (eventRaidBattle != null)
		{
			eventRaidBattle.ClosePopup();
		}
	}

	public void StartRaidReadyBattle(string bid)
	{
		if (eventRaidBattle != null)
		{
			eventRaidBattle.StartRaidReadyBattle(bid);
		}
	}

	public void CreateRaidRankingPopup(List<Protocols.EventRaidRankingData> list)
	{
		if (eventRaidBattle != null)
		{
			eventRaidBattle.CreateRaidRankingPopup(list);
		}
	}

	public void CreateEventRaidPopup(int type, int rewardCount, List<Protocols.EventRaidData> list)
	{
		if (eventRaidBattle == null)
		{
			eventRaidBattle = UIPopup.Create<UIEventRaidBattlePopup>("EventRaidBattlePopup");
		}
		if (eventEnd && type != 3)
		{
			CreateEventRaidListPopup(3);
		}
		else
		{
			eventRaidBattle.Init(type, rewardCount, list, eventEnd);
		}
	}

	public void EventRaidShared(string bid)
	{
		if (eventRaidBattle != null)
		{
			eventRaidBattle.EventRaidShared(bid);
		}
	}

	public void EventRaidRewardReceive(List<string> bidList)
	{
		if (eventRaidBattle != null)
		{
			eventRaidBattle.EventRaidRewardReceive(bidList);
		}
	}

	public void EventRaidRewardReceive(string bid)
	{
		if (eventRaidBattle != null)
		{
			eventRaidBattle.EventRaidRewardReceive(bid);
		}
	}

	public void CreateEventRaidListPopup(int type)
	{
		if (eventEnd)
		{
			type = 3;
		}
		base.network.RequestEventRaidList(type);
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close")
		{
			if (!bBackKeyEnable)
			{
				ClosePopUp();
			}
			return;
		}
		if (text.StartsWith("EventBattle-"))
		{
			if (!eventEnd)
			{
				int num = int.Parse(text.Substring(text.IndexOf("-") + 1));
				StartEventReadyBattle(num);
			}
			return;
		}
		switch (text)
		{
		case "RaidSummonBtn":
			if (!eventEnd && EventRaidSummonCheck())
			{
				EventRaidSummon();
			}
			break;
		case "RaidListBtn":
			CreateEventRaidListPopup(1);
			break;
		case "RaidSummonFailed":
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6600"));
			break;
		case "EventBattleLock":
			if (eventEnd)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6600"));
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5802"));
			}
			break;
		case "CarnivalBtn":
			base.network.RequestGetCarnivalList(ECarnivalCategory.Special, eventId);
			break;
		case "BattleTab":
			UISetter.SetFlipSwitch(storyTab, state: false);
			UISetter.SetFlipSwitch(gachaTab, state: false);
			UISetter.SetFlipSwitch(battleTab, state: true);
			UISetter.SetActive(storyRoot, active: false);
			UISetter.SetActive(gachaRoot, active: false);
			UISetter.SetActive(battleRoot, active: true);
			break;
		case "GachaTab":
			UISetter.SetFlipSwitch(storyTab, state: false);
			UISetter.SetFlipSwitch(gachaTab, state: true);
			UISetter.SetFlipSwitch(battleTab, state: false);
			UISetter.SetActive(storyRoot, active: false);
			UISetter.SetActive(gachaRoot, active: true);
			UISetter.SetActive(battleRoot, active: false);
			break;
		case "StoryTab":
			UISetter.SetFlipSwitch(storyTab, state: true);
			UISetter.SetFlipSwitch(gachaTab, state: false);
			UISetter.SetFlipSwitch(battleTab, state: false);
			UISetter.SetActive(storyRoot, active: true);
			UISetter.SetActive(gachaRoot, active: false);
			UISetter.SetActive(battleRoot, active: false);
			break;
		case "InfoBtn":
			if (infoPopUp == null)
			{
				UISimplePopup uISimplePopup = UISimplePopup.CreateOK("InformationPopup");
				uISimplePopup.Set(localization: true, "6500", "10000100", string.Empty, "1001", string.Empty, string.Empty);
				infoPopUp = uISimplePopup.gameObject;
			}
			break;
		}
	}

	private void OnDestroy()
	{
		if (AnimBlock != null)
		{
			AnimBlock = null;
		}
		if (goBG != null)
		{
			goBG = null;
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		Set();
		eventGacha.Refresh();
	}

	private void OpenPopupShow()
	{
		OnAnimOpen();
		base.Open();
	}

	public void ClosePopUp()
	{
		bBackKeyEnable = true;
		UIManager.instance.world.mainCommand.SetResourceView(EGoods.Bullet);
		if (base.uiWorld.existReadyBattle && base.uiWorld.readyBattle.isActive)
		{
			base.uiWorld.readyBattle.CloseAnimation();
		}
		if (eventRaidBattle != null)
		{
			eventRaidBattle.ClosePopup();
		}
		if (infoPopUp != null)
		{
			infoPopUp.GetComponent<UISimplePopup>().ClosePopup();
		}
		HidePopup();
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
		UISetter.SetActive(warning, active: false);
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
		AnimBG.Reset();
		AnimNpc.Reset();
		AnimTitle.Reset();
		AnimBlock.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public void AnimOpenFinish()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
	}

	protected override void OnEnablePopup()
	{
	}

	protected override void OnDisablePopup()
	{
	}

	private bool IsShowFirstEventScenario()
	{
		EventBattleScenarioDataRow eventBattleScenarioDataRow = base.regulation.eventBattleScenarioDtbl.Find((EventBattleScenarioDataRow row) => row.eventIdx == eventId.ToString() && row.timing == EventScenarioTimingType.EnterMain);
		if (eventBattleScenarioDataRow == null)
		{
			return false;
		}
		if (base.localUser.lastShowEventScenarioPlayTurn < eventBattleScenarioDataRow.playTurn)
		{
			return true;
		}
		return false;
	}

	private void StartEventBattleScenario()
	{
		EventBattleScenarioDataRow eventBattleScenarioDataRow = base.regulation.eventBattleScenarioDtbl.Find((EventBattleScenarioDataRow row) => row.eventIdx == eventId.ToString() && row.timing == EventScenarioTimingType.EnterMain);
		if (eventBattleScenarioDataRow != null)
		{
			ClassicRpgManager dialogMrg = UIManager.instance.world.dialogMrg;
			if (dialogMrg != null)
			{
				dialogMrg.StartEventScenario();
				dialogMrg.InitScenarioDialogue(eventBattleScenarioDataRow.scenarioIdx, DialogueType.Event);
			}
		}
	}

	public int GetEventType()
	{
		return EventType;
	}
}
