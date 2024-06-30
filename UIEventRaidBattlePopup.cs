using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Regulation;
using UnityEngine;

public class UIEventRaidBattlePopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIFlipSwitch summonTab;

	public UIFlipSwitch joinTab;

	public UIFlipSwitch historyTab;

	public GameObject emptyRoot;

	public UILabel emptyLabel;

	public UIDefaultListView raidListView;

	public GameObject bottomRoot;

	public GameObject totalRewardBtn;

	public UIScrollView raidScrollView;

	public GameObject rewardBadge;

	private List<Protocols.EventRaidData> raidList;

	private UIEventRaidJoinPopup joinPopup;

	private EventRaidTabType tabType;

	public void Init(int type, int rewardCount, List<Protocols.EventRaidData> list, bool eventEnd)
	{
		SetAutoDestroy(autoDestory: true);
		tabType = (EventRaidTabType)type;
		SetTabState(eventEnd);
		if (list == null)
		{
			list = new List<Protocols.EventRaidData>();
		}
		raidList = list;
		UISetter.SetActive(raidListView, list.Count > 0);
		UISetter.SetActive(emptyRoot, list.Count == 0);
		UISetter.SetActive(bottomRoot, tabType == EventRaidTabType.History && list.Count > 0);
		if (bottomRoot.activeSelf)
		{
			raidScrollView.panel.baseClipRegion = new Vector4(103f, 10f, 915f, 475f);
		}
		else
		{
			raidScrollView.panel.baseClipRegion = new Vector4(103f, -33f, 915f, 555f);
		}
		if (tabType == EventRaidTabType.Discovery)
		{
			UISetter.SetActive(rewardBadge, rewardCount > 0);
			UISetter.SetLabel(emptyLabel, Localization.Get("6997"));
		}
		else if (tabType == EventRaidTabType.Join)
		{
			UISetter.SetActive(rewardBadge, rewardCount > 0);
			UISetter.SetLabel(emptyLabel, Localization.Get("6998"));
		}
		else if (tabType == EventRaidTabType.History)
		{
			UISetter.SetLabel(emptyLabel, Localization.Get("6999"));
			List<Protocols.EventRaidData> list2 = list.FindAll((Protocols.EventRaidData row) => row.clear == 1 && row.receive == 0);
			UISetter.SetButtonGray(totalRewardBtn, list2.Count > 0);
			UISetter.SetActive(rewardBadge, list2.Count > 0);
		}
		if (tabType == EventRaidTabType.Discovery || tabType == EventRaidTabType.Join)
		{
			list.Sort(delegate(Protocols.EventRaidData row, Protocols.EventRaidData row1)
			{
				bool value = row.isOwn == 1;
				return (row1.isOwn == 1).CompareTo(value) switch
				{
					1 => 1, 
					-1 => -1, 
					_ => row.remain.CompareTo(row1.remain), 
				};
			});
		}
		else
		{
			list.Sort(delegate(Protocols.EventRaidData row, Protocols.EventRaidData row1)
			{
				bool flag = row.clear == 1 && row.receive == 0;
				switch ((row1.clear == 1 && row1.receive == 0).CompareTo(flag))
				{
				case 1:
					return 1;
				case -1:
					return -1;
				default:
					if (flag)
					{
						return row.remain.CompareTo(row1.remain);
					}
					return row1.remain.CompareTo(row.remain);
				}
			});
		}
		raidListView.Init(tabType, list, "Boss-");
		raidListView.ResetPosition();
	}

	private void SetTabState(bool eventEnd)
	{
		UISetter.SetActive(summonTab, !eventEnd);
		UISetter.SetActive(joinTab, !eventEnd);
		UISetter.SetFlipSwitch(summonTab, tabType == EventRaidTabType.Discovery);
		UISetter.SetFlipSwitch(joinTab, tabType == EventRaidTabType.Join);
		UISetter.SetFlipSwitch(historyTab, tabType == EventRaidTabType.History);
	}

	public void EventRaidShared(string bid)
	{
		Protocols.EventRaidData raidData = raidList.Find((Protocols.EventRaidData row) => row.bossId == bid);
		if (raidData != null)
		{
			raidData.isShare = 1;
			if (base.localUser.IsExistGuild())
			{
				Formatting formatting = Formatting.None;
				JsonSerializerSettings serializerSettings = Regulation.SerializerSettings;
				Protocols.ChattingMsgData chattingMsgData = new Protocols.ChattingMsgData();
				EnemyCommanderDataRow enemyCommanderDataRow = base.regulation.enemyCommanderDtbl.Find((EnemyCommanderDataRow row) => row.id == raidData.enemy);
				chattingMsgData.data = string.Format(Localization.Get("6539"), raidData.level, Localization.Get(enemyCommanderDataRow.name));
				Protocols.ChattingInfo.ChattingData chattingData = new Protocols.ChattingInfo.ChattingData();
				chattingData.sendChannel = base.localUser.channel;
				chattingData.sendWorld = base.localUser.world;
				chattingData.sendUno = base.localUser.uno;
				chattingData.nickname = base.localUser.nickname;
				chattingData.guildName = base.localUser.guildName;
				chattingData.level = base.localUser.level;
				chattingData.thumbnail = base.localUser.thumbnailId;
				CommanderCostumeDataRow costume = RemoteObjectManager.instance.regulation.FindCostumeData(int.Parse(base.localUser.thumbnailId));
				if (costume != null)
				{
					RoCommander roCommander = base.localUser.FindCommander(costume.cid.ToString());
					if (roCommander != null && roCommander.isBasicCostume)
					{
						CommanderCostumeDataRow commanderCostumeDataRow = RemoteObjectManager.instance.regulation.commanderCostumeDtbl.Find((CommanderCostumeDataRow row) => row.cid == costume.cid && row.skinName == roCommander.currentViewCostume);
						if (commanderCostumeDataRow != null)
						{
							chattingData.thumbnail = commanderCostumeDataRow.ctid.ToString();
						}
					}
				}
				chattingData.date = RemoteObjectManager.instance.GetCurrentTime();
				chattingData.message = chattingMsgData.ToString();
				string msg = JsonConvert.SerializeObject(chattingData, formatting, serializerSettings);
				SocketChatting.instance.SendChatMessage(2, msg);
			}
		}
		UIEventRaidListItem raidListItem = GetRaidListItem(bid);
		if (raidListItem != null)
		{
			raidListItem.raidShared();
		}
	}

	public void EventRaidRewardReceive(string bid)
	{
		Protocols.EventRaidData eventRaidData = raidList.Find((Protocols.EventRaidData row) => row.bossId == bid);
		if (eventRaidData != null)
		{
			eventRaidData.receive = 1;
		}
		UIEventRaidListItem raidListItem = GetRaidListItem(bid);
		if (raidListItem != null)
		{
			raidListItem.rewardReceived();
		}
	}

	public void EventRaidRewardReceive(List<string> bidList)
	{
		if (bidList.Count != 0 && bidList.Count != 1)
		{
			List<Protocols.EventRaidData> list = raidList.FindAll((Protocols.EventRaidData row) => row.clear == 1 && row.receive == 0);
			if (list.Count > 0)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -180f, 0f), Localization.Format("6613", list.Count));
			}
		}
	}

	public void UpdateEventRaid(string bid, Protocols.EventRaidData data)
	{
		Protocols.EventRaidData eventRaidData = raidList.Find((Protocols.EventRaidData row) => row.bossId == bid);
		eventRaidData.damage = data.damage;
		eventRaidData.remain = data.remain;
	}

	public void CreateRaidRankingPopup(List<Protocols.EventRaidRankingData> list)
	{
		if (joinPopup == null)
		{
			joinPopup = UIPopup.Create<UIEventRaidJoinPopup>("EventRaidJoinPopup");
			joinPopup.Init(list);
		}
	}

	private UIEventRaidListItem GetRaidListItem(string bid)
	{
		UIItemBase uIItemBase = raidListView.FindItem(bid);
		if (uIItemBase != null)
		{
			return uIItemBase as UIEventRaidListItem;
		}
		return null;
	}

	private void RemoveRaidListItem(string bid)
	{
		UIItemBase uIItemBase = raidListView.FindItem(bid);
		if (uIItemBase != null)
		{
			raidListView.RemoveItem(uIItemBase);
		}
	}

	public void EventRaidShare(string bid)
	{
		if (!base.localUser.IsExistGuild())
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6607"));
			return;
		}
		UIEventRaidListItem raidListItem = GetRaidListItem(bid);
		if (raidListItem == null)
		{
			return;
		}
		if (raidListItem.RemainTime() <= 0)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6606"));
			RemoveRaidListItem(bid);
			return;
		}
		UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: true, "1303", "6608", string.Empty, "1304", "1305");
		uISimplePopup.onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				base.network.RequestEventRaidShare(int.Parse(bid));
			}
		};
	}

	public void StartRaidReadyBattle(string bid)
	{
		Protocols.EventRaidData eventRaidData = raidList.Find((Protocols.EventRaidData row) => row.bossId == bid);
		if (eventRaidData.remain <= 0 || eventRaidData.clear == 1)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6606"));
			RemoveRaidListItem(bid);
			return;
		}
		string key = $"{eventRaidData.eIdx}_{eventRaidData.enemy}";
		if (base.regulation.eventRaidDtbl.ContainsKey(key))
		{
			EventRaidDataRow eventRaidDataRow = base.regulation.eventRaidDtbl[key];
			BattleData battleData = BattleData.Create(EBattleType.EventRaid);
			battleData.eventId = int.Parse(eventRaidData.eIdx);
			battleData.eventRaidBossId = bid;
			battleData.eventRaidIdx = eventRaidData.enemy;
			battleData.returnEventIdx = base.uiWorld.eventBattle.GetEventId();
			battleData.eventRaidAttendCount = eventRaidData.attendCount;
			battleData.defender = RoUser.CreateEventBattleEnemy("EventEnemy-", string.Empty, RoTroop.CreateEventBoss(eventRaidData));
			UIManager.instance.world.readyBattle.InitAndOpenReadyBattle(battleData);
		}
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "Close")
		{
			ClosePopup();
		}
		else if (text.StartsWith("Tab-"))
		{
			string s = text.Substring(text.IndexOf("-") + 1);
			base.network.RequestEventRaidList(int.Parse(s));
		}
		else if (text.StartsWith("Share-"))
		{
			string bid = text.Substring(text.IndexOf("-") + 1);
			EventRaidShare(bid);
		}
		else if (text.StartsWith("Shared-"))
		{
			string s2 = text.Substring(text.IndexOf("-") + 1);
			base.network.RequestEventRaidRankingData(int.Parse(s2));
		}
		else if (text.StartsWith("Ready-"))
		{
			string s3 = text.Substring(text.IndexOf("-") + 1);
			base.network.RequestEventRaidData(int.Parse(s3));
		}
		else if (text.StartsWith("Reward-"))
		{
			string s4 = text.Substring(text.IndexOf("-") + 1);
			base.network.RequestGetEventRaidReward(int.Parse(s4));
		}
		else if (text == "TotalRewardBtn")
		{
			if (!totalRewardBtn.GetComponent<UIButton>().isGray)
			{
				base.network.RequestGetEventRaidReward(0);
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6621"));
			}
		}
	}

	public new void OnRefresh()
	{
		base.network.RequestEventRaidList((int)tabType);
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
			bBackKeyEnable = true;
			if (joinPopup != null)
			{
				joinPopup.ClosePopUp();
			}
			if (base.uiWorld.existReadyBattle && base.uiWorld.readyBattle.isActive)
			{
				base.uiWorld.readyBattle.CloseAnimation();
			}
			HidePopup();
		}
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0f);
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
		base.network.RequestGetEventBattleData(base.uiWorld.eventBattle.GetEventId());
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
	}

	private void OnAnimClose()
	{
	}
}
