using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

public class UIDefaultListView : UIListViewBase
{
	public void Init(List<RoUnit> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoUnit roUnit = list[i];
			UIItemBase uIItemBase = base.itemList[i];
			if (uIItemBase == null)
			{
				continue;
			}
			uIItemBase.gameObject.name = idPrefix + roUnit.unitReg.key;
			UIDefaultItem uIDefaultItem = uIItemBase as UIDefaultItem;
			if (uIDefaultItem != null)
			{
				UnitDataRow unitReg = roUnit.unitReg;
				uIDefaultItem.SetMainSprite(unitReg.key);
				UISetter.SetLabel(uIDefaultItem.cost, unitReg.leadership.ToString());
				uIDefaultItem.SetSelectedMark(selected: false);
				uIDefaultItem.SetBranch(unitReg.branch);
				uIDefaultItem.SetLockMark(!roUnit.unlocked);
				uIDefaultItem.SetLevel(roUnit.level);
				bool flag = roUnit.trainingTime.IsValid();
				uIDefaultItem.SetResearchMark(flag);
				if (flag)
				{
					uIDefaultItem.SetTimer(roUnit.trainingTime.start, roUnit.trainingTime.end);
					uIDefaultItem.SetNickname(Localization.Get("Building.Headquarters.UnitResearch.Researching"));
				}
				else
				{
					UISetter.SetLabel(uIDefaultItem.nickname, unitReg.key);
				}
			}
			UIUnit uIUnit = uIItemBase as UIUnit;
			if (uIUnit != null)
			{
				uIUnit.SetSelection(selected: false);
				uIUnit.SetLock(locked: false);
				uIUnit.Set(roUnit);
			}
		}
	}

	public void InitUnit(List<RoUnit> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoUnit roUnit = list[i];
			UIUnit uIUnit = base.itemList[i] as UIUnit;
			if (!(uIUnit == null))
			{
				UISetter.SetGameObjectName(uIUnit.gameObject, idPrefix + roUnit.id);
				uIUnit.SetSelection(selected: false);
				uIUnit.Set(roUnit);
			}
		}
	}

	public void Init(List<RoCommander> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoCommander roCommander = list[i];
			UIItemBase uIItemBase = base.itemList[i];
			if (!(uIItemBase == null))
			{
				UISetter.SetGameObjectName(uIItemBase.gameObject, idPrefix + roCommander.id);
				uIItemBase.SetSelection(selected: false);
				UICommander uICommander = uIItemBase as UICommander;
				if (!(uICommander == null))
				{
					uICommander.Set(roCommander);
				}
			}
		}
	}

	public void Init(List<CommanderCostumeDataRow> list, string selectId, string idPrefix = null, Dictionary<int, int> eventCostume = null, RoCommander commander = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			CommanderCostumeDataRow commanderCostumeDataRow = list[i];
			CommanderCostumeListItem commanderCostumeListItem = base.itemList[i] as CommanderCostumeListItem;
			if (!(commanderCostumeListItem == null))
			{
				UISetter.SetGameObjectName(commanderCostumeListItem.gameObject, idPrefix + commanderCostumeDataRow.ctid);
				commanderCostumeListItem.SetSelection(selectId != null && commanderCostumeDataRow.ctid == (int)commander.currentCostume);
				bool isNew;
				bool isHot;
				bool isSale;
				if (eventCostume != null && eventCostume.ContainsKey(commanderCostumeDataRow.ctid))
				{
					int num = eventCostume[commanderCostumeDataRow.ctid];
					isNew = num == 1;
					isHot = num == 2;
					isSale = num == 3;
				}
				else
				{
					isNew = false;
					isHot = false;
					isSale = false;
				}
				commanderCostumeListItem.Set(commanderCostumeDataRow, selectId != null, isNew, isHot, isSale);
			}
		}
	}

	public void Init(List<FavorDataRow> list, string cid, int favorStep, int favorRewarStep, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			FavorDataRow favorDataRow = list[i];
			FavorListItem favorListItem = base.itemList[i] as FavorListItem;
			if (!(favorListItem == null))
			{
				UISetter.SetGameObjectName(favorListItem.gameObject, idPrefix + favorDataRow.step);
				favorListItem.SetSelection(selected: false);
				favorListItem.Set(favorDataRow, cid, favorStep, favorRewarStep);
			}
		}
	}

	public void InitGiftList(List<CommanderGiftDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			CommanderGiftDataRow commanderGiftDataRow = list[i];
			CommanderGiftListItem commanderGiftListItem = base.itemList[i] as CommanderGiftListItem;
			if (!(commanderGiftListItem == null))
			{
				UISetter.SetGameObjectName(commanderGiftListItem.gameObject, idPrefix + commanderGiftDataRow.idx);
				commanderGiftListItem.SetSelection(selected: false);
				commanderGiftListItem.Set(commanderGiftDataRow);
			}
		}
	}

	public void InitVoiceInteraction(List<InteractionDataRow> list, int favorStep, int marry, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			InteractionDataRow interactionDataRow = list[i];
			VoiceInteractionListItem voiceInteractionListItem = base.itemList[i] as VoiceInteractionListItem;
			if (voiceInteractionListItem == null)
			{
				continue;
			}
			UISetter.SetGameObjectName(voiceInteractionListItem.gameObject, idPrefix + interactionDataRow.favorup);
			voiceInteractionListItem.SetSelection(selected: false);
			bool overFavor = false;
			int favorStep2 = 0;
			bool marryMark = false;
			if (favorStep <= 4)
			{
				if (interactionDataRow.type == InteractionType.HEAD_FAVOR2 || interactionDataRow.type == InteractionType.BODY_FAVOR2 || interactionDataRow.type == InteractionType.LEG_FAVOR2 || interactionDataRow.type == InteractionType.START_FAVOR2 || interactionDataRow.type == InteractionType.HEAD_FAVOR3 || interactionDataRow.type == InteractionType.BODY_FAVOR3 || interactionDataRow.type == InteractionType.LEG_FAVOR3 || interactionDataRow.type == InteractionType.START_FAVOR3 || interactionDataRow.type == InteractionType.HEAD_FAVOR4 || interactionDataRow.type == InteractionType.BODY_FAVOR4 || interactionDataRow.type == InteractionType.LEG_FAVOR4 || interactionDataRow.type == InteractionType.START_FAVOR4 || interactionDataRow.type == InteractionType.HEAD_FAVOR5 || interactionDataRow.type == InteractionType.BODY_FAVOR5 || interactionDataRow.type == InteractionType.LEG_FAVOR5 || interactionDataRow.type == InteractionType.START_FAVOR5 || interactionDataRow.type == InteractionType.IDLE_MARRY)
				{
					overFavor = true;
				}
			}
			else if (favorStep >= 5 && favorStep <= 8)
			{
				if (interactionDataRow.type == InteractionType.HEAD_FAVOR3 || interactionDataRow.type == InteractionType.BODY_FAVOR3 || interactionDataRow.type == InteractionType.LEG_FAVOR3 || interactionDataRow.type == InteractionType.START_FAVOR3 || interactionDataRow.type == InteractionType.HEAD_FAVOR4 || interactionDataRow.type == InteractionType.BODY_FAVOR4 || interactionDataRow.type == InteractionType.LEG_FAVOR4 || interactionDataRow.type == InteractionType.START_FAVOR4 || interactionDataRow.type == InteractionType.HEAD_FAVOR5 || interactionDataRow.type == InteractionType.BODY_FAVOR5 || interactionDataRow.type == InteractionType.LEG_FAVOR5 || interactionDataRow.type == InteractionType.START_FAVOR5 || interactionDataRow.type == InteractionType.IDLE_MARRY)
				{
					overFavor = true;
				}
			}
			else if (favorStep >= 9 && favorStep <= 12)
			{
				if (interactionDataRow.type == InteractionType.HEAD_FAVOR4 || interactionDataRow.type == InteractionType.BODY_FAVOR4 || interactionDataRow.type == InteractionType.LEG_FAVOR4 || interactionDataRow.type == InteractionType.START_FAVOR4 || interactionDataRow.type == InteractionType.HEAD_FAVOR5 || interactionDataRow.type == InteractionType.BODY_FAVOR5 || interactionDataRow.type == InteractionType.LEG_FAVOR5 || interactionDataRow.type == InteractionType.START_FAVOR5 || interactionDataRow.type == InteractionType.IDLE_MARRY)
				{
					overFavor = true;
				}
			}
			else if (marry == 0 && (interactionDataRow.type == InteractionType.HEAD_FAVOR5 || interactionDataRow.type == InteractionType.BODY_FAVOR5 || interactionDataRow.type == InteractionType.LEG_FAVOR5 || interactionDataRow.type == InteractionType.START_FAVOR5 || interactionDataRow.type == InteractionType.IDLE_MARRY))
			{
				overFavor = true;
			}
			if (interactionDataRow.type == InteractionType.HEAD_FAVOR2 || interactionDataRow.type == InteractionType.BODY_FAVOR2 || interactionDataRow.type == InteractionType.LEG_FAVOR2 || interactionDataRow.type == InteractionType.START_FAVOR2)
			{
				favorStep2 = 5;
			}
			else if (interactionDataRow.type == InteractionType.HEAD_FAVOR3 || interactionDataRow.type == InteractionType.BODY_FAVOR3 || interactionDataRow.type == InteractionType.LEG_FAVOR3 || interactionDataRow.type == InteractionType.START_FAVOR3)
			{
				favorStep2 = 9;
			}
			else if (interactionDataRow.type == InteractionType.HEAD_FAVOR4 || interactionDataRow.type == InteractionType.BODY_FAVOR4 || interactionDataRow.type == InteractionType.LEG_FAVOR4 || interactionDataRow.type == InteractionType.START_FAVOR4)
			{
				favorStep2 = 13;
			}
			else if (interactionDataRow.type == InteractionType.HEAD_FAVOR5 || interactionDataRow.type == InteractionType.BODY_FAVOR5 || interactionDataRow.type == InteractionType.LEG_FAVOR5 || interactionDataRow.type == InteractionType.START_FAVOR5 || interactionDataRow.type == InteractionType.IDLE_MARRY)
			{
				marryMark = true;
			}
			voiceInteractionListItem.Set(Localization.Format("10079", interactionDataRow.favorup), overFavor, favorStep2, marryMark);
		}
	}

	public void InitVoiceBattle(List<CommanderVoiceDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			CommanderVoiceDataRow commanderVoiceDataRow = list[i];
			VoiceInteractionListItem voiceInteractionListItem = base.itemList[i] as VoiceInteractionListItem;
			if (!(voiceInteractionListItem == null))
			{
				UISetter.SetGameObjectName(voiceInteractionListItem.gameObject, idPrefix + (int)commanderVoiceDataRow.type);
				voiceInteractionListItem.SetSelection(selected: false);
				voiceInteractionListItem.Set(Localization.Format("10079", i + 1), overFavor: false, 1, commanderVoiceDataRow.type == ECommanderVoiceEventType.Fatal || commanderVoiceDataRow.type == ECommanderVoiceEventType.WinFatal || commanderVoiceDataRow.type == ECommanderVoiceEventType.Lose);
			}
		}
	}

	public void InitGroupTab(Dictionary<string, string> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<string, string> item in list)
		{
			GroupTapListItem groupTapListItem = base.itemList[num] as GroupTapListItem;
			if (!(groupTapListItem == null))
			{
				UISetter.SetGameObjectName(groupTapListItem.gameObject, idPrefix + item.Key);
				groupTapListItem.SetSelection(selected: false);
				groupTapListItem.Set(item.Key);
				num++;
			}
		}
	}

	public void InitGroup(List<GroupInfoDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			GroupInfoDataRow groupInfoDataRow = list[i];
			GroupListItem groupListItem = base.itemList[i] as GroupListItem;
			if (!(groupListItem == null))
			{
				UISetter.SetGameObjectName(groupListItem.gameObject, idPrefix + groupInfoDataRow.groupIdx);
				groupListItem.SetSelection(selected: false);
				groupListItem.Set(groupInfoDataRow);
			}
		}
	}

	public void InitGroupMember(List<GroupMemberDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			GroupMemberDataRow groupMemberDataRow = list[i];
			GroupMemberItem groupMemberItem = base.itemList[i] as GroupMemberItem;
			if (!(groupMemberItem == null))
			{
				UISetter.SetGameObjectName(groupMemberItem.gameObject, idPrefix + groupMemberDataRow.idx);
				groupMemberItem.SetSelection(selected: false);
				groupMemberItem.Set(groupMemberDataRow);
			}
		}
	}

	public void InitRaidList(List<RoCommander> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoCommander roCommander = list[i];
			UIItemBase uIItemBase = base.itemList[i];
			if (!(uIItemBase == null))
			{
				UISetter.SetGameObjectName(uIItemBase.gameObject, idPrefix + roCommander.id);
				uIItemBase.SetSelection(selected: false);
				RaidItem raidItem = uIItemBase as RaidItem;
				if (!(raidItem == null))
				{
					raidItem.Set(roCommander, i);
				}
			}
		}
	}

	public void InitWaveBattleList(List<Protocols.WaveBattleInfoList.WaveBattleInfo> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			UIItemBase uIItemBase = base.itemList[i];
			if (!(uIItemBase == null))
			{
				UISetter.SetGameObjectName(uIItemBase.gameObject, idPrefix + list[i].battleIdx);
				uIItemBase.SetSelection(selected: false);
				WaveBattleItem waveBattleItem = uIItemBase as WaveBattleItem;
				if (!(waveBattleItem == null))
				{
					waveBattleItem.SetWaveItem(list[i]);
				}
			}
		}
	}

	public void Init(List<RoCommander> list, RoTroop troop, BattleData battleData, string idPrefix = null, ECharacterType charType = ECharacterType.None)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoCommander roCommander = list[i];
			UICommanderSelectItem uICommanderSelectItem = base.itemList[i] as UICommanderSelectItem;
			if (!(uICommanderSelectItem == null))
			{
				UISetter.SetGameObjectName(uICommanderSelectItem.gameObject, idPrefix + roCommander.id);
				uICommanderSelectItem.SetSelection(selected: false);
				if (charType == ECharacterType.Mercenary)
				{
					uICommanderSelectItem.Set(roCommander, troop, battleData, charType, i);
				}
				else
				{
					uICommanderSelectItem.Set(roCommander, troop, battleData, charType);
				}
			}
		}
	}

	public void Init(List<RoCommander> list, List<RoTroop> troops, BattleData battleData, string idPrefix = null, ECharacterType charType = ECharacterType.None)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoCommander roCommander = list[i];
			UICommanderSelectItem uICommanderSelectItem = base.itemList[i] as UICommanderSelectItem;
			if (!(uICommanderSelectItem == null))
			{
				UISetter.SetGameObjectName(uICommanderSelectItem.gameObject, idPrefix + roCommander.id);
				uICommanderSelectItem.SetSelection(selected: false);
				if (charType == ECharacterType.Mercenary)
				{
					uICommanderSelectItem.Set(roCommander, troops, battleData, charType, i);
				}
				else
				{
					uICommanderSelectItem.Set(roCommander, troops, battleData, charType);
				}
			}
		}
	}

	public void Init(List<RoRecruit.Entry> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoRecruit.Entry entry = list[i];
			UIItemBase uIItemBase = base.itemList[i];
			if (!(uIItemBase == null))
			{
				UISetter.SetGameObjectName(uIItemBase.gameObject, idPrefix + entry.commander.id);
				uIItemBase.SetSelection(selected: false);
				UICommander uICommander = uIItemBase as UICommander;
				if (!(uICommander == null))
				{
					uICommander.Set(entry);
				}
			}
		}
	}

	public void Init(List<RoTroop> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoTroop roTroop = list[i];
			UIItemBase uIItemBase = base.itemList[i];
			if (!(uIItemBase == null))
			{
				UISetter.SetGameObjectName(uIItemBase.gameObject, idPrefix + roTroop.id);
				uIItemBase.SetSelection(selected: false);
				if (uIItemBase is UITroop)
				{
					UITroop uITroop = uIItemBase as UITroop;
					uITroop.Set(roTroop);
				}
			}
		}
	}

	public void InitPlatoon(List<RoTroop> troopList, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(troopList.Count);
		for (int i = 0; i < troopList.Count; i++)
		{
			RoTroop roTroop = troopList[i];
			UIItemBase uIItemBase = base.itemList[i];
			if (!(uIItemBase == null) && uIItemBase is UIPlatoon)
			{
				uIItemBase.gameObject.name = idPrefix + roTroop.id;
				UIPlatoon uIPlatoon = uIItemBase as UIPlatoon;
				uIPlatoon.Set(roTroop);
			}
		}
		string value = "AddPlatoon-";
		int num = 0;
		while (true)
		{
			Transform child = grid.GetChild(num++);
			if (child == null)
			{
				break;
			}
			if (child.name.StartsWith(value))
			{
				child.SetAsLastSibling();
				break;
			}
		}
		grid.sorting = UIGrid.Sorting.None;
		grid.Reposition();
	}

	public void InitTimeMachineResult(List<List<Protocols.RewardInfo.RewardData>> list, ETimeMachineType type, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		float num2 = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			List<Protocols.RewardInfo.RewardData> list2 = list[i];
			TimeMachineResultListItem timeMachineResultListItem = base.itemList[num] as TimeMachineResultListItem;
			if (!(timeMachineResultListItem == null))
			{
				timeMachineResultListItem.gameObject.transform.localPosition = new Vector3(timeMachineResultListItem.gameObject.transform.localPosition.x, 0f - num2, timeMachineResultListItem.gameObject.transform.localPosition.z);
				timeMachineResultListItem.gameObject.name = (num + 1).ToString();
				switch (type)
				{
				case ETimeMachineType.Stage:
					timeMachineResultListItem.Set(list2, i == list.Count - 1);
					break;
				case ETimeMachineType.Sweep:
				case ETimeMachineType.AdvanceParty:
					timeMachineResultListItem.Set(list2, type);
					break;
				}
				num2 += (float)(timeMachineResultListItem.bg.height + 10);
				UISetter.SetActive(timeMachineResultListItem, active: false);
				num++;
			}
		}
	}

	public void InitTimeMachineRewardItem(List<Protocols.RewardInfo.RewardData> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.RewardInfo.RewardData rewardData = list[i];
			UIGoods uIGoods = base.itemList[i] as UIGoods;
			if (!(uIGoods == null))
			{
				uIGoods.Set(rewardData);
				UISetter.SetGameObjectName(uIGoods.gameObject, idPrefix + rewardData.rewardId);
				UISetter.SetActive(uIGoods, active: false);
				uIGoods.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<RoReward> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoReward roReward = list[i];
			UIRewardListItem uIRewardListItem = base.itemList[i] as UIRewardListItem;
			if (!(uIRewardListItem == null))
			{
				uIRewardListItem.Set(roReward);
				UISetter.SetGameObjectName(uIRewardListItem.gameObject, idPrefix + roReward.id);
				uIRewardListItem.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<Protocols.MailInfo.MailData> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.MailInfo.MailData mailData = list[i];
			UIRewardListItem uIRewardListItem = base.itemList[i] as UIRewardListItem;
			if (!(uIRewardListItem == null))
			{
				uIRewardListItem.Set(mailData);
				UISetter.SetGameObjectName(uIRewardListItem.gameObject, idPrefix + mailData.idx);
				uIRewardListItem.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<RoMission> list, string idPrefix = null, bool bComplete = false)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		List<RoMission> list2 = new List<RoMission>();
		for (int i = 0; i < list.Count; i++)
		{
			RoMission roMission = list[i];
			if (bComplete)
			{
				if (roMission.received)
				{
					list2.Add(roMission);
				}
			}
			else if (!roMission.received && roMission.bListShow && !list2.Contains(roMission))
			{
				list2.Add(roMission);
			}
		}
		list2.Sort(delegate(RoMission row, RoMission row1)
		{
			if (row.combleted && !row1.combleted)
			{
				return -1;
			}
			if (!row.combleted && row1.combleted)
			{
				return 1;
			}
			if (int.Parse(row.idx) < int.Parse(row1.idx))
			{
				return -1;
			}
			return (int.Parse(row.idx) > int.Parse(row1.idx)) ? 1 : row.sort.CompareTo(row1.sort);
		});
		ResizeItemList(list2.Count);
		for (int j = 0; j < list2.Count; j++)
		{
			RoMission roMission2 = list2[j];
			UIRewardListItem uIRewardListItem = base.itemList[j] as UIRewardListItem;
			if (!(uIRewardListItem == null))
			{
				uIRewardListItem.Set(roMission2);
				UISetter.SetGameObjectName(uIRewardListItem.gameObject, idPrefix + roMission2.idx);
				uIRewardListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitRewardList(List<Protocols.RewardInfo.RewardData> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.RewardInfo.RewardData rewardData = list[i];
			UIGoods uIGoods = base.itemList[i] as UIGoods;
			if (!(uIGoods == null))
			{
				uIGoods.Set(rewardData);
				UISetter.SetGameObjectName(uIGoods.gameObject, idPrefix + rewardData.rewardId);
				uIGoods.SetSelection(selected: false);
			}
		}
	}

	public void InitBoxRewardList(List<RandomBoxRewardDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RandomBoxRewardDataRow randomBoxRewardDataRow = list[i];
			UIGoods uIGoods = base.itemList[i] as UIGoods;
			if (!(uIGoods == null))
			{
				uIGoods.Set(randomBoxRewardDataRow);
				UISetter.SetGameObjectName(uIGoods.gameObject, $"{idPrefix}{(int)randomBoxRewardDataRow.rewardType}_{randomBoxRewardDataRow.rewardIdx}");
				uIGoods.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<RoUser> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoUser roUser = list[i];
			UIUser uIUser = base.itemList[i] as UIUser;
			if (!(uIUser == null))
			{
				uIUser.Set(roUser);
				UISetter.SetGameObjectName(uIUser.gameObject, idPrefix + roUser.id);
				uIUser.SetSelection(selected: false);
			}
		}
	}

	public void InitRankingList(EBattleType type, List<RoUser> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoUser roUser = list[i];
			RecordListItem recordListItem = base.itemList[i] as RecordListItem;
			if (!(recordListItem == null))
			{
				UISetter.SetGameObjectName(recordListItem.gameObject, idPrefix + roUser.id);
				recordListItem.Set(type, roUser);
				recordListItem.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<BuildingLevelDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			BuildingLevelDataRow buildingLevelDataRow = list[i];
			UIDefaultItem uIDefaultItem = base.itemList[i] as UIDefaultItem;
			if (!(uIDefaultItem == null))
			{
				UISetter.SetActive(uIDefaultItem.count, active: false);
				RemoteObjectManager instance = RemoteObjectManager.instance;
				RoBuilding roBuilding = instance.localUser.buildingDict[buildingLevelDataRow.type];
				uIDefaultItem.SetNickname(Localization.Get(roBuilding.reg.locNameKey));
				uIDefaultItem.SetBuildingAtlas();
				uIDefaultItem.SetIcon(buildingLevelDataRow.resourceId);
				if (buildingLevelDataRow.level == 1)
				{
					UISetter.SetActive(uIDefaultItem.newMark, active: true);
					UISetter.SetActive(uIDefaultItem.level, active: false);
				}
				else
				{
					UISetter.SetActive(uIDefaultItem.newMark, active: false);
					UISetter.SetActive(uIDefaultItem.level, active: true);
					uIDefaultItem.SetLevel(buildingLevelDataRow.level);
				}
				UISetter.SetGameObjectName(uIDefaultItem.gameObject, idPrefix + i);
				uIDefaultItem.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<UnitDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			UnitDataRow unitDataRow = list[i];
			UIDefaultItem uIDefaultItem = base.itemList[i] as UIDefaultItem;
			if (!(uIDefaultItem == null))
			{
				UISetter.SetActive(uIDefaultItem.newMark, active: false);
				UISetter.SetActive(uIDefaultItem.count, active: false);
				UISetter.SetActive(uIDefaultItem.level, active: true);
				RemoteObjectManager instance = RemoteObjectManager.instance;
				uIDefaultItem.SetNickname(Localization.Format(unitDataRow.nameKey));
				uIDefaultItem.SetCommonAtlas();
				uIDefaultItem.SetIcon(unitDataRow.resourceName + "_Front");
				uIDefaultItem.SetLevel(1);
				UISetter.SetGameObjectName(uIDefaultItem.gameObject, idPrefix + i);
				uIDefaultItem.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<RoWorldMap.Stage> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoWorldMap.Stage stage = list[i];
			UIStage uIStage = base.itemList[i] as UIStage;
			if (!(uIStage == null))
			{
				uIStage.Set(stage);
				UISetter.SetGameObjectName(uIStage.gameObject, idPrefix + stage.id);
				uIStage.SetSelection(selected: false);
				Vector3 position = stage.data.position;
				position.y *= -1f;
				uIStage.transform.localPosition = position;
			}
		}
	}

	public void Init(List<GuildOccupyDataRow> list, List<UIConquestStage> stageList, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			GuildOccupyDataRow guildOccupyDataRow = list[i];
			UIConquestStage uIConquestStage = base.itemList[i] as UIConquestStage;
			if (!(uIConquestStage == null))
			{
				uIConquestStage.Set(guildOccupyDataRow);
				stageList.Add(uIConquestStage);
				UISetter.SetGameObjectName(uIConquestStage.gameObject, idPrefix + guildOccupyDataRow.idx);
				uIConquestStage.SetSelection(selected: false);
				Vector3 zero = Vector3.zero;
				zero.x = guildOccupyDataRow.positionx;
				zero.y = guildOccupyDataRow.positiony;
				zero.y *= -1f;
				uIConquestStage.transform.localPosition = zero;
			}
		}
	}

	public void Init(List<RoScramble.Stage> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoScramble.Stage stage = list[i];
			UIStage uIStage = base.itemList[i] as UIStage;
			if (!(uIStage == null))
			{
				uIStage.Set(stage);
				UISetter.SetGameObjectName(uIStage.gameObject, idPrefix + stage.id);
				uIStage.SetSelection(selected: false);
				Vector3 position = stage.data.position;
				position.y *= -1f;
				uIStage.transform.localPosition = position;
			}
		}
	}

	public void Init(Dictionary<string, int> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<string, int> item in list)
		{
			UIGoods uIGoods = base.itemList[num] as UIGoods;
			if (!(uIGoods == null))
			{
				uIGoods.Set(item.Key, item.Value);
				uIGoods.SetSelection(selected: false);
				num++;
			}
		}
	}

	public void InitGoodsList(Dictionary<string, int> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<string, int> item in list)
		{
			UIGoods uIGoods = base.itemList[num] as UIGoods;
			if (!(uIGoods == null))
			{
				uIGoods.SetGoodsId(item.Key, item.Value);
				uIGoods.SetSelection(selected: false);
				num++;
			}
		}
	}

	public void Init(List<SweepDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			SweepDataRow sweepDataRow = list[i];
			SweepListItem sweepListItem = base.itemList[i] as SweepListItem;
			if (!(sweepListItem == null))
			{
				UISetter.SetGameObjectName(sweepListItem.gameObject, idPrefix + sweepDataRow.level);
				sweepListItem.Set(sweepDataRow);
				sweepListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitBattleReward(Dictionary<string, int> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<string, int> item in list)
		{
			UIGoods uIGoods = base.itemList[num] as UIGoods;
			if (!(uIGoods == null))
			{
				uIGoods.Set(item.Key, item.Value);
				uIGoods.SetSelection(selected: false);
				num++;
			}
		}
	}

	public void InitDuelList(EBattleType battleType, Dictionary<int, RoUser> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<int, RoUser> item in list)
		{
			RemoteObjectManager.instance.localUser.duelTargetIdx = num + 1;
			RoUser pvpUser = list[item.Key];
			PvPListItem pvPListItem = base.itemList[num] as PvPListItem;
			if (!(pvPListItem == null))
			{
				pvPListItem.Set(battleType, pvpUser);
				UISetter.SetGameObjectName(pvPListItem.battleBtn.gameObject, idPrefix + item.Key);
				UISetter.SetGameObjectName(pvPListItem.gameObject, idPrefix + item.Key);
				pvPListItem.SetSelection(selected: false);
				num++;
			}
		}
	}

	public void InitDuelList(Dictionary<int, RoUser> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<int, RoUser> item in list)
		{
			RemoteObjectManager.instance.localUser.duelTargetIdx = num + 1;
			RoUser pvpUser = list[item.Key];
			PvPListItem pvPListItem = base.itemList[num] as PvPListItem;
			if (!(pvPListItem == null))
			{
				pvPListItem.Set(pvpUser);
				UISetter.SetGameObjectName(pvPListItem.battleBtn.gameObject, idPrefix + item.Key);
				UISetter.SetGameObjectName(pvPListItem.gameObject, idPrefix + item.Key);
				pvPListItem.SetSelection(selected: false);
				num++;
			}
		}
	}

	public void Init(List<GuildSkillState> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			UIBattleItem uIBattleItem = base.itemList[i] as UIBattleItem;
			if (!(uIBattleItem == null))
			{
				GuildSkillState guildSkillState = list[i];
				uIBattleItem.Set(guildSkillState.dr);
				uIBattleItem.gameObject.SetActive(value: true);
			}
		}
	}

	public IEnumerator _PlayBuffAnimation()
	{
		if (base.itemList != null)
		{
			yield return new WaitForSeconds(0.6f);
			for (int i = 0; i < base.itemList.Count; i++)
			{
				base.itemList[i].gameObject.SetActive(value: true);
				yield return new WaitForSeconds(0.6f);
			}
		}
		yield return null;
	}

	public void InitAdvancePossibleCommanderList(List<RoCommander> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoCommander roCommander = list[i];
			AdvanceCommanderListItem advanceCommanderListItem = base.itemList[i] as AdvanceCommanderListItem;
			if (!(advanceCommanderListItem == null))
			{
				advanceCommanderListItem.SetItem(roCommander);
				UISetter.SetGameObjectName(advanceCommanderListItem.gameObject, idPrefix + roCommander.id);
				advanceCommanderListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitDispatchPossibleCommanderList(List<RoCommander> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoCommander roCommander = list[i];
			AdvanceCommanderListItem advanceCommanderListItem = base.itemList[i] as AdvanceCommanderListItem;
			if (!(advanceCommanderListItem == null))
			{
				advanceCommanderListItem.DispatchSetItem(roCommander);
				UISetter.SetGameObjectName(advanceCommanderListItem.gameObject, idPrefix + roCommander.id);
				advanceCommanderListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitOpenList(List<BuildingLevelDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			BuildingLevelDataRow buildingLevelDataRow = list[i];
			UIBuilding uIBuilding = base.itemList[i] as UIBuilding;
			if (!(uIBuilding == null))
			{
				uIBuilding.Set(buildingLevelDataRow);
				UISetter.SetLabel(uIBuilding.nickname, Localization.Get(buildingLevelDataRow.locNameKey));
				UISetter.SetGameObjectName(uIBuilding.gameObject, idPrefix + buildingLevelDataRow.type);
				uIBuilding.SetSelection(selected: false);
			}
		}
	}

	public void InitSkillList(List<SkillDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			SkillDataRow skillDataRow = list[i];
			UISkill uISkill = base.itemList[i] as UISkill;
			if (!(uISkill == null))
			{
				uISkill.Set(skillDataRow);
				string empty = string.Empty;
				UISetter.SetGameObjectName(name: (skillDataRow != null) ? (idPrefix + skillDataRow.key) : (idPrefix + "0"), go: uISkill.gameObject);
				uISkill.SetSelection(selected: false);
			}
		}
	}

	public void InitCommanderSkillList(List<SkillDataRow> list, RoCommander commander, string idPrefix = null)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			SkillDataRow skillDataRow = list[i];
			UICommanderSkill uICommanderSkill = base.itemList[i] as UICommanderSkill;
			if (!(uICommanderSkill == null))
			{
				int skillCost = 0;
				int num = 0;
				bool isUpgradeLevel = false;
				int skillLevel = commander.GetSkillLevel(i + 1);
				SkillCostDataRow skillCostDataRow = regulation.skillCostDtbl[skillLevel.ToString()];
				if (i < 4)
				{
					skillCost = skillCostDataRow.typeCost[i];
					num = skillCostDataRow.typeLimitLevel[i];
					isUpgradeLevel = num != 0 && (int)commander.level >= num;
				}
				uICommanderSkill.SetSkill(skillDataRow, skillLevel, skillCost, skillDataRow.openGrade, skillDataRow.openGrade <= (int)commander.cls, i, isUpgradeLevel);
				string empty = string.Empty;
				UISetter.SetGameObjectName(name: (skillDataRow != null) ? (idPrefix + skillDataRow.key) : (idPrefix + "0"), go: uICommanderSkill.gameObject);
				uICommanderSkill.SetSelection(selected: false);
			}
		}
	}

	public void InitGrade(int grade, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(grade);
	}

	public void Init(List<RoPart> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		List<RoPart> list2 = new List<RoPart>();
		for (int i = 0; i < list.Count; i++)
		{
			RoPart roPart = list[i];
			if (roPart.count > 0)
			{
				list2.Add(roPart);
			}
		}
		ResizeItemList(list2.Count);
		for (int j = 0; j < list2.Count; j++)
		{
			RoPart roPart2 = list2[j];
			UIPartListItem uIPartListItem = base.itemList[j] as UIPartListItem;
			if (!(uIPartListItem == null))
			{
				uIPartListItem.Set(roPart2);
				UISetter.SetGameObjectName(uIPartListItem.gameObject, idPrefix + roPart2.id);
				uIPartListItem.SetSelection(selected: false);
			}
		}
	}

	public void Init(ERePlayType rePlayType, List<Protocols.RecordInfo> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		list.Sort((Protocols.RecordInfo a, Protocols.RecordInfo b) => (int)(b.date - a.date));
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.RecordInfo recordInfo = list[i];
			if (recordInfo.simulationVer == 1000000)
			{
				RecordListItem recordListItem = base.itemList[i] as RecordListItem;
				if (!(recordListItem == null))
				{
					recordListItem.Set(rePlayType, recordInfo);
					UISetter.SetGameObjectName(recordListItem.replayBtn, "RePlay-" + recordInfo.id);
					recordListItem.SetSelection(selected: false);
				}
			}
		}
	}

	public void Init(List<Protocols.NavigatorInfo> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.NavigatorInfo navigatorInfo = list[i];
			NaviListItem naviListItem = base.itemList[i] as NaviListItem;
			if (!(naviListItem == null))
			{
				naviListItem.Set(navigatorInfo);
				UISetter.SetGameObjectName(naviListItem.gameObject, string.Concat(idPrefix, navigatorInfo.type, "_", navigatorInfo.stageIdx));
				naviListItem.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<Protocols.UserInformationResponse.PreDeck> list, BattleData battleData, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.UserInformationResponse.PreDeck preDeck = list[i];
			DeckListItem deckListItem = base.itemList[i] as DeckListItem;
			if (!(deckListItem == null))
			{
				deckListItem.Set(preDeck, battleData);
				UISetter.SetGameObjectName(deckListItem.gameObject, idPrefix + preDeck.idx);
				deckListItem.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<Protocols.ServerData.ServerInfo> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.ServerData.ServerInfo serverInfo = list[i];
			UIServerListItem uIServerListItem = base.itemList[i] as UIServerListItem;
			if (!(uIServerListItem == null))
			{
				uIServerListItem.Set(serverInfo);
				UISetter.SetGameObjectName(uIServerListItem.gameObject, idPrefix + serverInfo.idx);
				uIServerListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitRewardList(List<RewardDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RewardDataRow rewardDataRow = list[i];
			UIGoods uIGoods = base.itemList[i] as UIGoods;
			if (!(uIGoods == null))
			{
				uIGoods.Set(rewardDataRow);
				UISetter.SetGameObjectName(uIGoods.gameObject, idPrefix + rewardDataRow.type);
				uIGoods.SetSelection(selected: false);
			}
		}
	}

	public void InitRandomCarnivalRewardList(List<RewardDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RewardDataRow rewardDataRow = list[i];
			UIGoods uIGoods = base.itemList[i] as UIGoods;
			if (!(uIGoods == null))
			{
				uIGoods.SetRandomCarnivalReward(rewardDataRow);
				UISetter.SetGameObjectName(uIGoods.gameObject, idPrefix + rewardDataRow.type);
				uIGoods.SetSelection(selected: false);
			}
		}
	}

	public void InitCarivalSelectRewardList(List<RewardDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RewardDataRow item = list[i];
			UICarnivalSelectRewardListItem uICarnivalSelectRewardListItem = base.itemList[i] as UICarnivalSelectRewardListItem;
			if (!(uICarnivalSelectRewardListItem == null))
			{
				uICarnivalSelectRewardListItem.SetItem(item);
				UISetter.SetGameObjectName(uICarnivalSelectRewardListItem.gameObject, idPrefix + (i + 1));
				uICarnivalSelectRewardListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitCarnivalRewardList(List<CurCarnivalItemInfo> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			CurCarnivalItemInfo carnivalItem = list[i];
			UIGoods uIGoods = base.itemList[i] as UIGoods;
			if (!(uIGoods == null))
			{
				uIGoods.SetCarnivalItem(carnivalItem);
				UISetter.SetGameObjectName(uIGoods.gameObject, idPrefix + carnivalItem.type);
				uIGoods.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<RoGuild> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoGuild roGuild = list[i];
			UIGuildListItem uIGuildListItem = base.itemList[i] as UIGuildListItem;
			if (!(uIGuildListItem == null))
			{
				UISetter.SetGameObjectName(uIGuildListItem.gameObject, idPrefix + roGuild.gidx);
				uIGuildListItem.SetSelection(selected: false);
				uIGuildListItem.Set(roGuild);
			}
		}
	}

	public void Init(List<Protocols.GuildMember.MemberData> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.GuildMember.MemberData memberData = list[i];
			UIGuildMemberListItem uIGuildMemberListItem = base.itemList[i] as UIGuildMemberListItem;
			if (!(uIGuildMemberListItem == null))
			{
				UISetter.SetGameObjectName(uIGuildMemberListItem.gameObject, idPrefix + memberData.uno);
				uIGuildMemberListItem.SetSelection(selected: false);
				uIGuildMemberListItem.Set(memberData);
			}
		}
	}

	public void Init(List<RoGuildSkill> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoGuildSkill roGuildSkill = list[i];
			UIGuildSkillListItem uIGuildSkillListItem = base.itemList[i] as UIGuildSkillListItem;
			if (!(uIGuildSkillListItem == null))
			{
				UISetter.SetGameObjectName(uIGuildSkillListItem.gameObject, idPrefix + roGuildSkill.idx);
				uIGuildSkillListItem.SetSelection(selected: false);
				uIGuildSkillListItem.Set(roGuildSkill);
			}
		}
	}

	public void InitTranscendenceSlot(RoCommander comm, int slotIdx, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(RemoteObjectManager.instance.regulation.transcendenceSlotDtbl.length);
		int num = 0;
		foreach (TranscendenceSlotDataRow item in RemoteObjectManager.instance.regulation.transcendenceSlotDtbl)
		{
			TranscendenceSlotDataRow transcendenceSlotDataRow = item;
			UITranscendenceListItem uITranscendenceListItem = base.itemList[num] as UITranscendenceListItem;
			if (!(uITranscendenceListItem == null))
			{
				UISetter.SetGameObjectName(uITranscendenceListItem.gameObject, idPrefix + transcendenceSlotDataRow.slot);
				if (slotIdx != 0 && slotIdx == transcendenceSlotDataRow.slot)
				{
					uITranscendenceListItem.SetSelection(selected: true);
				}
				else
				{
					uITranscendenceListItem.SetSelection(selected: false);
				}
				uITranscendenceListItem.Set(comm, transcendenceSlotDataRow);
				num++;
			}
		}
	}

	public void InitTicket(List<CommanderTrainingTicketDataRow> list, RoCommander commander, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			CommanderTrainingTicketDataRow commanderTrainingTicketDataRow = list[i];
			UIGoods uIGoods = base.itemList[i] as UIGoods;
			if (!(uIGoods == null))
			{
				UISetter.SetGameObjectName(uIGoods.gameObject, idPrefix + commanderTrainingTicketDataRow.type);
				uIGoods.SetSelection(selected: false);
				uIGoods.Set(commanderTrainingTicketDataRow, commander);
			}
		}
	}

	public void Init(List<Protocols.SecretShop.ShopData> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.SecretShop.ShopData data = list[i];
			UIShopListItem uIShopListItem = base.itemList[i] as UIShopListItem;
			if (!(uIShopListItem == null))
			{
				UISetter.SetGameObjectName(uIShopListItem.gameObject, idPrefix + i);
				uIShopListItem.SetSelection(selected: false);
				uIShopListItem.Set(data);
			}
		}
	}

	public void Init(List<Protocols.ScrambleMapHistory> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.ScrambleMapHistory scrambleMapHistory = list[i];
			ScrambleHistoryItem scrambleHistoryItem = base.itemList[i] as ScrambleHistoryItem;
			if (!(scrambleHistoryItem == null))
			{
				UISetter.SetGameObjectName(scrambleHistoryItem.gameObject, idPrefix + scrambleMapHistory.result);
				scrambleHistoryItem.SetSelection(selected: false);
				scrambleHistoryItem.Set(scrambleMapHistory);
			}
		}
	}

	public void Init(List<Protocols.ScrambleRankingData> list, int maxScore, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.ScrambleRankingData scrambleRankingData = list[i];
			ScrambleRankingItem scrambleRankingItem = base.itemList[i] as ScrambleRankingItem;
			if (!(scrambleRankingItem == null))
			{
				scrambleRankingItem.SetMaxScore(maxScore);
				scrambleRankingItem.Set(scrambleRankingData);
				UISetter.SetGameObjectName(scrambleRankingItem.gameObject, idPrefix + scrambleRankingData.name);
				scrambleRankingItem.SetSelection(selected: false);
			}
		}
	}

	public void InitFavorItem(List<RewardDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RewardDataRow rewardDataRow = list[i];
			UIFavorRewardListItem uIFavorRewardListItem = base.itemList[i] as UIFavorRewardListItem;
			if (!(uIFavorRewardListItem == null))
			{
				UISetter.SetGameObjectName(uIFavorRewardListItem.gameObject, idPrefix + rewardDataRow.typeIndex);
				uIFavorRewardListItem.SetSelection(selected: false);
				uIFavorRewardListItem.Set(rewardDataRow);
			}
		}
	}

	public void InitAlarm(string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(RemoteObjectManager.instance.regulation.reportDtbl.length);
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		int num = 0;
		foreach (AlarmDataRow item in RemoteObjectManager.instance.regulation.reportDtbl)
		{
			AlarmDataRow alarmDataRow = RemoteObjectManager.instance.regulation.reportDtbl[item.key];
			AlarmListItem alarmListItem = base.itemList[num] as AlarmListItem;
			if (alarmListItem == null)
			{
				continue;
			}
			UISetter.SetGameObjectName(alarmListItem.gameObject, idPrefix + alarmDataRow.key);
			alarmListItem.SetSelection(selected: false);
			if (string.Equals(item.key, "dcnt"))
			{
				int dcnt = localUser.alarmData.dcnt;
				if (dcnt == 0)
				{
					UISetter.SetActive(alarmListItem, active: false);
				}
				else
				{
					UISetter.SetActive(alarmListItem, active: true);
				}
				alarmListItem.SetMissonCount(alarmDataRow, dcnt);
			}
			else if (string.Equals(item.key, "shop"))
			{
				if (localUser.alarmData.shop.ContainsKey(EShopType.BasicShop))
				{
					int num2 = localUser.alarmData.shop[EShopType.BasicShop];
					if (num2 > 0)
					{
						UISetter.SetActive(alarmListItem, active: false);
					}
					else
					{
						UISetter.SetActive(alarmListItem, active: true);
					}
					alarmListItem.SetShopRefresh(alarmDataRow, num2);
				}
				else
				{
					UISetter.SetActive(alarmListItem, active: false);
				}
			}
			else if (string.Equals(item.key, "srgs"))
			{
				int srgs = localUser.alarmData.srgs;
				if (srgs > 0)
				{
					UISetter.SetActive(alarmListItem, active: true);
				}
				else
				{
					UISetter.SetActive(alarmListItem, active: false);
				}
				alarmListItem.SetScrambleStart(alarmDataRow, srgs);
			}
			else if (string.Equals(item.key, "srge"))
			{
				int srge = localUser.alarmData.srge;
				if (srge > 0)
				{
					UISetter.SetActive(alarmListItem, active: true);
				}
				else
				{
					UISetter.SetActive(alarmListItem, active: false);
				}
				alarmListItem.SetScrambleEnd(alarmDataRow, srge);
			}
			else
			{
				UISetter.SetActive(alarmListItem, active: false);
			}
			num++;
		}
	}

	public void InitWhisperNickNameList(Dictionary<int, NickNameType> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			KeyValuePair<int, NickNameType> keyValuePair = list.ElementAt(i);
			UIUser uIUser = base.itemList[i] as UIUser;
			if (!(uIUser == null))
			{
				uIUser.Set(keyValuePair.Value.nickname);
				UISetter.SetGameObjectName(uIUser.gameObject, idPrefix + keyValuePair.Key);
				uIUser.SetSelection(selected: false);
			}
		}
	}

	public void InitStoragePartList(List<RoPart> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoPart roPart = list[i];
			UIStorageListItem uIStorageListItem = base.itemList[i] as UIStorageListItem;
			if (!(uIStorageListItem == null))
			{
				uIStorageListItem.Set(roPart);
				UISetter.SetGameObjectName(uIStorageListItem.gameObject, idPrefix + roPart.id);
				uIStorageListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitStorageList(Dictionary<string, int> list, EStorageType type, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<string, int> item in list)
		{
			UIStorageListItem uIStorageListItem = base.itemList[num] as UIStorageListItem;
			if (!(uIStorageListItem == null))
			{
				uIStorageListItem.Set(item.Key, item.Value, type);
				UISetter.SetGameObjectName(uIStorageListItem.gameObject, idPrefix + item.Key);
				uIStorageListItem.SetSelection(selected: false);
				num++;
			}
		}
	}

	public void InitStorageItemList(List<RoItem> itmeList, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(itmeList.Count);
		for (int i = 0; i < itmeList.Count; i++)
		{
			RoItem roItem = itmeList[i];
			UIStorageListItem uIStorageListItem = base.itemList[i] as UIStorageListItem;
			if (!(uIStorageListItem == null))
			{
				uIStorageListItem.Set(roItem);
				UISetter.SetGameObjectName(uIStorageListItem.gameObject, idPrefix + roItem.id + "-" + roItem.level);
				uIStorageListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitCommanderList(string commanderId, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		string[] array = commanderId.Split(',');
		ResizeItemList(array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			UICommander uICommander = base.itemList[i] as UICommander;
			uICommander.Set(text);
			UISetter.SetGameObjectName(uICommander.gameObject, idPrefix + text);
			uICommander.SetSelection(selected: false);
		}
	}

	public void InitMaterial(Dictionary<int, int[]> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<int, int[]> item in list)
		{
			CommanderClassMaterialItem commanderClassMaterialItem = base.itemList[num] as CommanderClassMaterialItem;
			if (!(commanderClassMaterialItem == null))
			{
				commanderClassMaterialItem.SetIcon(item.Key, item.Value);
				UISetter.SetGameObjectName(commanderClassMaterialItem.gameObject, idPrefix + item.Value[0]);
				commanderClassMaterialItem.SetSelection(selected: false);
				num++;
			}
		}
	}

	public void InitScoreRewardList(Dictionary<int, List<RewardDataRow>> list, bool duel, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<int, List<RewardDataRow>> item in list)
		{
			UIScoreRewardItem uIScoreRewardItem = base.itemList[num] as UIScoreRewardItem;
			if (!(uIScoreRewardItem == null))
			{
				if (duel)
				{
					uIScoreRewardItem.Set(item.Key, item.Value);
				}
				else
				{
					uIScoreRewardItem.SetRaidScore(item.Key, item.Value);
				}
				UISetter.SetGameObjectName(uIScoreRewardItem.gameObject, idPrefix + item.Key);
				UISetter.SetGameObjectName(uIScoreRewardItem.getBtn, idPrefix + item.Key);
				num++;
			}
		}
	}

	public void InitCommanderViewList(RoTroop troop, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		List<RoTroop.Slot> list = new List<RoTroop.Slot>();
		for (int i = 0; i < troop.slots.Length; i++)
		{
			RoTroop.Slot slot = troop.slots[i];
			if (slot.position >= 0)
			{
				if (list.Count == 0)
				{
					list.Add(slot);
				}
				else if (slot.position < list[0].position)
				{
					list.Insert(0, slot);
				}
				else
				{
					list.Add(slot);
				}
			}
		}
		ResizeItemList(list.Count);
		for (int j = 0; j < list.Count; j++)
		{
			RoTroop.Slot slot2 = list[j];
			UIBattleUnitStatus uIBattleUnitStatus = base.itemList[j] as UIBattleUnitStatus;
			if (!(uIBattleUnitStatus == null))
			{
				uIBattleUnitStatus.Set(slot2);
				UISetter.SetGameObjectName(uIBattleUnitStatus.gameObject, idPrefix + slot2.commanderId);
				uIBattleUnitStatus.SetSelection(selected: false);
				uIBattleUnitStatus.statusView.SetEnable(enable: false);
				uIBattleUnitStatus.commanderView.SetEnable(enable: true);
				uIBattleUnitStatus.expView.SetEnable(enable: true);
			}
		}
	}

	public void InitRoulletRewardList(List<int> list, int cost = 0, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			int num = list[i];
			RoulletListItem roulletListItem = base.itemList[i] as RoulletListItem;
			if (!(roulletListItem == null))
			{
				if (cost == 0)
				{
					roulletListItem.Set(num);
				}
				else
				{
					roulletListItem.Set(num, cost);
					UISetter.SetActive(roulletListItem.gameObject, active: false);
				}
				UISetter.SetGameObjectName(roulletListItem.gameObject, idPrefix + num);
				roulletListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitCashShop(List<Protocols.CashShopData> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.CashShopData cashShopData = list[i];
			UICashShopListItem uICashShopListItem = base.itemList[i] as UICashShopListItem;
			uICashShopListItem.Set(cashShopData);
			UISetter.SetGameObjectName(uICashShopListItem.gameObject, idPrefix + cashShopData.priceId);
			uICashShopListItem.SetSelection(selected: false);
		}
	}

	public void InitRankingReward(List<RankingDataRow> list, ERankingContentsType type, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RankingDataRow rankingDataRow = list[i];
			UIRankingRewardItem uIRankingRewardItem = base.itemList[i] as UIRankingRewardItem;
			if (type == ERankingContentsType.ChallengeGrade || type == ERankingContentsType.WorldDuelGrade)
			{
				uIRankingRewardItem.SetReward(rankingDataRow, type);
			}
			else
			{
				uIRankingRewardItem.Set(rankingDataRow, i);
			}
			UISetter.SetGameObjectName(uIRankingRewardItem.gameObject, idPrefix + rankingDataRow.r_idx);
			UISetter.SetGameObjectName(uIRankingRewardItem.receiveBtn, idPrefix + rankingDataRow.r_idx);
			uIRankingRewardItem.SetSelection(selected: false);
		}
	}

	public void Init(List<Protocols.VipGacha.VipGachaInfo> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.VipGacha.VipGachaInfo gachaInfo = list[i];
			UIGoods uIGoods = base.itemList[i] as UIGoods;
			if (!(uIGoods == null))
			{
				UISetter.SetGameObjectName(uIGoods.gameObject, idPrefix + i);
				uIGoods.SetSelection(selected: false);
				uIGoods.Set(gachaInfo);
			}
		}
	}

	public void SetDialougeText(List<string> _txt, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(_txt.Count);
		string empty = string.Empty;
		for (int i = 0; i < _txt.Count; i++)
		{
			empty = _txt[i];
			UIDialogueChoice uIDialogueChoice = base.itemList[i] as UIDialogueChoice;
			UISetter.SetGameObjectName(uIDialogueChoice.gameObject, i.ToString());
			uIDialogueChoice.SetSelection(selected: false);
			uIDialogueChoice.setText(empty, i);
		}
	}

	public void Init(Dictionary<string, Protocols.CarnivalList.CarnivaTime> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		List<string> list2 = list.Keys.ToList();
		list2.Sort(delegate(string a, string b)
		{
			CarnivalTypeDataRow carnivalTypeDataRow = RemoteObjectManager.instance.regulation.carnivalTypeDtbl[a];
			CarnivalTypeDataRow carnivalTypeDataRow2 = RemoteObjectManager.instance.regulation.carnivalTypeDtbl[b];
			return carnivalTypeDataRow.sort.CompareTo(carnivalTypeDataRow2.sort);
		});
		int num = 0;
		foreach (string item in list2)
		{
			CarnivalTabListItem carnivalTabListItem = base.itemList[num] as CarnivalTabListItem;
			if (!(carnivalTabListItem == null))
			{
				carnivalTabListItem.Set(item);
				UISetter.SetGameObjectName(carnivalTabListItem.gameObject, $"{idPrefix}{item}");
				carnivalTabListItem.SetSelection(selected: false);
				num++;
			}
		}
	}

	public void Init(List<CarnivalDataRow> list, CarnivalTypeDataRow carType, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			CarnivalDataRow row = list[i];
			UICarnivalListItem uICarnivalListItem = base.itemList[i] as UICarnivalListItem;
			if (!(uICarnivalListItem == null))
			{
				UISetter.SetGameObjectName(uICarnivalListItem.gameObject, $"{idPrefix}{i}");
				uICarnivalListItem.SetSelection(selected: false);
				if (carType.Type != ECarnivalType.ConnectTimeReward)
				{
					uICarnivalListItem.Set(carType.idx, row);
				}
			}
		}
	}

	public void InitGiftList(List<CarnivalDataRow> list, int process, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			CarnivalDataRow carnivalDataRow = list[i];
			UICarnivalgiftListItem uICarnivalgiftListItem = base.itemList[i] as UICarnivalgiftListItem;
			if (!(uICarnivalgiftListItem == null))
			{
				UISetter.SetGameObjectName(uICarnivalgiftListItem.gameObject, $"{idPrefix}{carnivalDataRow.idx}");
				uICarnivalgiftListItem.SetSelection(selected: false);
				uICarnivalgiftListItem.Set(i <= process);
			}
		}
	}

	public void InitDailyPackageList(List<CarnivalDataRow> list, CarnivalTypeDataRow carType, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			CarnivalDataRow carnivalDataRow = list[i];
			UICarnivalDailyPackageListItem uICarnivalDailyPackageListItem = base.itemList[i] as UICarnivalDailyPackageListItem;
			if (!(uICarnivalDailyPackageListItem == null))
			{
				UISetter.SetGameObjectName(uICarnivalDailyPackageListItem.gameObject, $"{idPrefix}{carnivalDataRow.idx}");
				uICarnivalDailyPackageListItem.SetSelection(selected: false);
				uICarnivalDailyPackageListItem.Set(carType.idx, carnivalDataRow);
			}
		}
	}

	public void InitVipList(List<CarnivalDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			CarnivalDataRow carnivalDataRow = list[i];
			UICarnivalVipListItem uICarnivalVipListItem = base.itemList[i] as UICarnivalVipListItem;
			if (!(uICarnivalVipListItem == null))
			{
				UISetter.SetGameObjectName(uICarnivalVipListItem.gameObject, $"{idPrefix}{carnivalDataRow.idx}");
				uICarnivalVipListItem.SetSelection(selected: false);
				uICarnivalVipListItem.Set(carnivalDataRow);
			}
		}
	}

	public void InitDevelopmentList(List<CarnivalDataRow> list, string cTidx, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			CarnivalDataRow carnivalDataRow = list[i];
			UICarnivalDevelopmentListItem uICarnivalDevelopmentListItem = base.itemList[i] as UICarnivalDevelopmentListItem;
			if (!(uICarnivalDevelopmentListItem == null))
			{
				UISetter.SetGameObjectName(uICarnivalDevelopmentListItem.gameObject, $"{idPrefix}{carnivalDataRow.idx}");
				uICarnivalDevelopmentListItem.SetSelection(selected: false);
				uICarnivalDevelopmentListItem.Set(cTidx, carnivalDataRow);
			}
		}
	}

	public void InitAttandRewardList(List<RewardDataRow> list, string vipLevel, string vipMultiply, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RewardDataRow reward = list[i];
			UIGoods uIGoods = base.itemList[i] as UIGoods;
			if (!(uIGoods == null))
			{
				uIGoods.Set(reward, vipLevel, vipMultiply);
			}
		}
	}

	public void InitChannel(Dictionary<string, Protocols.ChannelData> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<string, Protocols.ChannelData> item in list)
		{
			ChannelListItem channelListItem = base.itemList[num] as ChannelListItem;
			if (!(channelListItem == null))
			{
				UISetter.SetGameObjectName(channelListItem.gameObject, $"{idPrefix}{item.Key}");
				channelListItem.SetSelection(selected: false);
				channelListItem.Set(item);
				num++;
			}
		}
	}

	private void OnDestroy()
	{
		if (itemPrefab != null)
		{
			itemPrefab = null;
		}
	}

	public void Init(List<CommanderScenarioDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			UIItemBase uIItemBase = base.itemList[i];
			if (uIItemBase == null)
			{
				continue;
			}
			UISetter.SetGameObjectName(uIItemBase.gameObject, idPrefix + list[i].csid);
			uIItemBase.SetSelection(selected: false);
			ScenarioListItem scenarioListItem = uIItemBase as ScenarioListItem;
			if (!(scenarioListItem == null))
			{
				scenarioListItem.Set(list[i]);
				if (i == 0)
				{
					scenarioListItem.OnClick();
				}
			}
		}
	}

	public void Init(List<Protocols.GuildBoardData> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.GuildBoardData guildBoardData = list[i];
			GuildBoardListItem guildBoardListItem = base.itemList[i] as GuildBoardListItem;
			if (!(guildBoardListItem == null))
			{
				guildBoardListItem.Set(guildBoardData);
				UISetter.SetGameObjectName(guildBoardListItem.gameObject, idPrefix + guildBoardData.idx);
				guildBoardListItem.SetSelection(selected: false);
			}
		}
	}

	public void Init(Dictionary<int, Protocols.ConquestTroopInfo.Troop> list, EConquestStageInfoType type, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<int, Protocols.ConquestTroopInfo.Troop> item in list)
		{
			Protocols.ConquestTroopInfo.Troop troop = list[item.Key];
			ConquestDeckListItem conquestDeckListItem = base.itemList[num] as ConquestDeckListItem;
			if (!(conquestDeckListItem == null))
			{
				conquestDeckListItem.Set(item.Key, type, troop);
				UISetter.SetGameObjectName(conquestDeckListItem.gameObject, idPrefix + item.Key);
				conquestDeckListItem.SetSelection(selected: false);
				num++;
			}
		}
	}

	public void Init(List<Protocols.ConquestStageInfo.User> list, bool isAlie, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.ConquestStageInfo.User user = list[i];
			UIConquestStageUserListItem uIConquestStageUserListItem = base.itemList[i] as UIConquestStageUserListItem;
			if (!(uIConquestStageUserListItem == null))
			{
				uIConquestStageUserListItem.Set(user, isAlie);
				UISetter.SetGameObjectName(uIConquestStageUserListItem.gameObject, idPrefix + user.uno);
				uIConquestStageUserListItem.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<Protocols.GuildMember.MemberData> list, Dictionary<string, int> userList, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.GuildMember.MemberData memberData = list[i];
			UIConquestCurrentStateListItem uIConquestCurrentStateListItem = base.itemList[i] as UIConquestCurrentStateListItem;
			if (!(uIConquestCurrentStateListItem == null))
			{
				int count = 0;
				if (userList.Count > 0 && userList.ContainsKey(memberData.uno.ToString()))
				{
					count = userList[memberData.uno.ToString()];
				}
				uIConquestCurrentStateListItem.Set(memberData, count);
				UISetter.SetGameObjectName(uIConquestCurrentStateListItem.gameObject, idPrefix + memberData.uno);
				uIConquestCurrentStateListItem.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<Protocols.GuildMember.MemberData> list, List<Protocols.ConquestStageInfo.User> dataList, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.GuildMember.MemberData user = list[i];
			Protocols.ConquestStageInfo.User data = dataList.Find((Protocols.ConquestStageInfo.User row) => row.uno == user.uno.ToString());
			UIConquestCurrentStateListItem uIConquestCurrentStateListItem = base.itemList[i] as UIConquestCurrentStateListItem;
			if (!(uIConquestCurrentStateListItem == null))
			{
				uIConquestCurrentStateListItem.Set(user, data);
				UISetter.SetGameObjectName(uIConquestCurrentStateListItem.gameObject, idPrefix + user.uno);
				uIConquestCurrentStateListItem.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<Protocols.ConquestStageUser> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.ConquestStageUser conquestStageUser = list[i];
			ConquestAlieListItem conquestAlieListItem = base.itemList[i] as ConquestAlieListItem;
			if (!(conquestAlieListItem == null))
			{
				conquestAlieListItem.Set(conquestStageUser);
				UISetter.SetGameObjectName(conquestAlieListItem.gameObject, idPrefix + conquestStageUser.slot);
				conquestAlieListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitConquestResultStageList(List<int> list, Protocols.ConquestInfo.PrevState.Point data, List<int> standbyList, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			int num = 0;
			int num2 = list[i];
			UIConquestBattleResultStageListItem uIConquestBattleResultStageListItem = base.itemList[i] as UIConquestBattleResultStageListItem;
			if (uIConquestBattleResultStageListItem == null)
			{
				continue;
			}
			if (data.lose.Count > 0)
			{
				for (int j = 0; j < data.lose.Count; j++)
				{
					if (num2 == data.lose[j])
					{
						num = 1;
					}
				}
			}
			if (data.win.Count > 0)
			{
				for (int k = 0; k < data.win.Count; k++)
				{
					if (num2 == data.win[k])
					{
						num = 2;
					}
				}
			}
			bool standby = false;
			if (standbyList != null)
			{
				for (int l = 0; l < standbyList.Count; l++)
				{
					if (standbyList[l] == num2)
					{
						standby = true;
					}
				}
			}
			uIConquestBattleResultStageListItem.Set(num2, num, standby);
			UISetter.SetGameObjectName(uIConquestBattleResultStageListItem.gameObject, (num != 0) ? (idPrefix + num2) : ("Empty-" + num2));
			uIConquestBattleResultStageListItem.SetSelection(selected: false);
		}
	}

	public void Init(Protocols.GetConquestBattle data, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		int num = data.entry.blue.Count + data.entry.red.Count + 1;
		ResizeItemList(num);
		UIConquestBattleResultPopup popup = UIManager.instance.world.guild.historyPopup.popup;
		int num2 = 0;
		int num3 = 0;
		int prevState = 0;
		int num4 = 0;
		for (int i = 0; i < num; i++)
		{
			UIConquestBattleResultUserListItem uIConquestBattleResultUserListItem = base.itemList[i] as UIConquestBattleResultUserListItem;
			if (uIConquestBattleResultUserListItem == null)
			{
				continue;
			}
			UISetter.SetActive(uIConquestBattleResultUserListItem, active: false);
			Protocols.GetConquestBattle.EntryInfo entryInfo = null;
			Protocols.GetConquestBattle.EntryInfo entryInfo2 = null;
			Protocols.GetConquestBattle.Result result = null;
			Protocols.GetConquestBattle.BattleEntry battleEntry = null;
			string replayId = string.Empty;
			if (data.entry.blue.Count > num2)
			{
				entryInfo = data.entry.blue[num2];
			}
			if (data.entry.red.Count > num3)
			{
				entryInfo2 = data.entry.red[num3];
			}
			if (entryInfo == null && entryInfo2 == null)
			{
				continue;
			}
			UISetter.SetActive(uIConquestBattleResultUserListItem, active: true);
			uIConquestBattleResultUserListItem.panel.depth = uIConquestBattleResultUserListItem.panel.depth + num4;
			if (data.battle.Count > i)
			{
				result = data.battle[i].result;
				battleEntry = data.battle[i].entry;
				replayId = data.battle[i].replayId;
			}
			UISetter.SetGameObjectName(uIConquestBattleResultUserListItem.gameObject, $"{num2}/{num3}");
			int num5 = uIConquestBattleResultUserListItem.Set(entryInfo, entryInfo2, battleEntry, result, replayId, prevState);
			popup.blueState.spriteName = ((num5 != 1) ? "alliance_banner_lose" : "alliance_banner_win");
			popup.redState.spriteName = ((num5 != 2) ? "alliance_banner_lose" : "alliance_banner_win");
			if (num5 == 0)
			{
				num2++;
				num3++;
			}
			if (num5 == 1)
			{
				num3++;
				if (data.entry.red.Count <= num3)
				{
					num2++;
					num5 = 3;
				}
			}
			if (num5 == 2)
			{
				num2++;
				if (data.entry.blue.Count <= num2)
				{
					num3++;
					num5 = 3;
				}
			}
			num4++;
			prevState = num5;
			uIConquestBattleResultUserListItem.SetSelection(selected: false);
		}
	}

	public void InitHaveItemList(List<RoItem> list)
	{
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			UIItemBase uIItemBase = base.itemList[i];
			if (!(uIItemBase == null))
			{
				UISetter.SetGameObjectName(uIItemBase.gameObject, list[i].id + "_" + i);
				uIItemBase.SetSelection(selected: false);
				UIHaveListItem uIHaveListItem = uIItemBase as UIHaveListItem;
				uIHaveListItem.Set(list[i]);
			}
		}
	}

	public void InitGuildRankingList(List<Protocols.GuildRankingInfo> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.GuildRankingInfo data = list[i];
			UIGuildRankingListItem uIGuildRankingListItem = base.itemList[i] as UIGuildRankingListItem;
			if (!(uIGuildRankingListItem == null))
			{
				uIGuildRankingListItem.Set(data);
				UISetter.SetGameObjectName(uIGuildRankingListItem.gameObject, idPrefix);
				uIGuildRankingListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitGachaBannerList(List<GachaDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			GachaDataRow gachaDataRow = list[i];
			UIGachaBannerListItem uIGachaBannerListItem = base.itemList[i] as UIGachaBannerListItem;
			if (!(uIGachaBannerListItem == null))
			{
				uIGachaBannerListItem.Set(gachaDataRow);
				UISetter.SetGameObjectName(uIGachaBannerListItem.gameObject, $"{idPrefix}{gachaDataRow.type}");
				uIGachaBannerListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitGachaProbabilityList(Dictionary<ERewardType, float> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<ERewardType, float> item in list)
		{
			ProbabilityListItem probabilityListItem = base.itemList[num] as ProbabilityListItem;
			if (!(probabilityListItem == null))
			{
				probabilityListItem.Set(item.Key, item.Value);
				UISetter.SetGameObjectName(probabilityListItem.gameObject, $"{idPrefix}{item.Key}");
				probabilityListItem.SetSelection(selected: false);
				num++;
			}
		}
	}

	public void InitScenarioLogList(List<ScenarioLogInfo> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			UIItemBase uIItemBase = base.itemList[i];
			if (!(uIItemBase == null))
			{
				UISetter.SetGameObjectName(uIItemBase.gameObject, i.ToString());
				uIItemBase.SetSelection(selected: false);
				ScenarioLogItem scenarioLogItem = uIItemBase as ScenarioLogItem;
				if (!(scenarioLogItem == null))
				{
					scenarioLogItem.Set(list[i]);
				}
			}
		}
	}

	public void InitCoopPointGuildRankList(List<Protocols.CooperateBattlePointGuildRankingInfo> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.CooperateBattlePointGuildRankingInfo data = list[i];
			UICooperateBattlePointGuildRankListItem uICooperateBattlePointGuildRankListItem = base.itemList[i] as UICooperateBattlePointGuildRankListItem;
			if (!(uICooperateBattlePointGuildRankListItem == null))
			{
				uICooperateBattlePointGuildRankListItem.Set(data);
				UISetter.SetGameObjectName(uICooperateBattlePointGuildRankListItem.gameObject, idPrefix);
				uICooperateBattlePointGuildRankListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitCoopPointUserRankList(List<Protocols.CooperateBattlePointUserRankingInfo> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.CooperateBattlePointUserRankingInfo data = list[i];
			UICooperateBattlePointUserRankListItem uICooperateBattlePointUserRankListItem = base.itemList[i] as UICooperateBattlePointUserRankListItem;
			if (!(uICooperateBattlePointUserRankListItem == null))
			{
				uICooperateBattlePointUserRankListItem.Set(data);
				UISetter.SetGameObjectName(uICooperateBattlePointUserRankListItem.gameObject, idPrefix);
				uICooperateBattlePointUserRankListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitBannerPorintList(List<BannerInfo> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			BannerInfo bannerInfo = list[i];
			MainBannerOrderIcon mainBannerOrderIcon = base.itemList[i] as MainBannerOrderIcon;
			if (!(mainBannerOrderIcon == null))
			{
				if (i == 0)
				{
					mainBannerOrderIcon.SetSelection(selected: true);
				}
				else
				{
					mainBannerOrderIcon.SetSelection(selected: false);
				}
				UISetter.SetGameObjectName(mainBannerOrderIcon.gameObject, i.ToString());
			}
		}
	}

	public void InitChattingBlockUserList(List<Protocols.BlockUser> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.BlockUser blockUser = list[i];
			UIChattingBlockUser uIChattingBlockUser = base.itemList[i] as UIChattingBlockUser;
			if (!(uIChattingBlockUser == null))
			{
				uIChattingBlockUser.Set(blockUser);
				UISetter.SetGameObjectName(uIChattingBlockUser.gameObject, idPrefix + blockUser.uno);
				uIChattingBlockUser.SetSelection(selected: false);
			}
		}
	}

	public void Init(List<Protocols.EventBattleInfo> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.EventBattleInfo eventBattleInfo = list[i];
			EventListItem eventListItem = base.itemList[i] as EventListItem;
			if (!(eventListItem == null))
			{
				eventListItem.Set(eventBattleInfo);
				UISetter.SetGameObjectName(eventListItem.gameObject, $"{idPrefix}{eventBattleInfo.idx}");
			}
		}
	}

	public void Init(List<EventBattleFieldDataRow> list, string lastClearId, bool eventEnd, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			EventBattleFieldDataRow eventBattleFieldDataRow = list[i];
			UIEventBattleItem uIEventBattleItem = base.itemList[i] as UIEventBattleItem;
			if (!(uIEventBattleItem == null))
			{
				uIEventBattleItem.Set(eventBattleFieldDataRow, lastClearId, eventEnd);
				UISetter.SetGameObjectName(uIEventBattleItem.gameObject, $"{idPrefix}{eventBattleFieldDataRow.idx}");
			}
		}
	}

	public void InitClearCondition(WorldMapStageDataRow mapStageDr, string idPrefix = null)
	{
		int num = 1;
		if (mapStageDr.clearCondition1 != 0)
		{
			num++;
		}
		if (mapStageDr.clearCondition2 != 0)
		{
			num++;
		}
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(num);
		int num2 = 0;
		((UIClearConditionItem)base.itemList[num2]).Set(EBattleClearCondition.None, string.Empty);
		if (mapStageDr.clearCondition1 != 0)
		{
			((UIClearConditionItem)base.itemList[++num2]).Set(mapStageDr.clearCondition1, mapStageDr.clearCondition1_Value);
		}
		if (mapStageDr.clearCondition2 != 0)
		{
			((UIClearConditionItem)base.itemList[++num2]).Set(mapStageDr.clearCondition2, mapStageDr.clearCondition2_Value);
		}
	}

	public void InitClearCondition(EventBattleFieldDataRow eventBattleDr, string idPrefix = null)
	{
		int num = 1;
		if (eventBattleDr.clearCondition1 != 0)
		{
			num++;
		}
		if (eventBattleDr.clearCondition2 != 0)
		{
			num++;
		}
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(num);
		int num2 = 0;
		((UIClearConditionItem)base.itemList[num2]).Set(EBattleClearCondition.None, string.Empty);
		if (eventBattleDr.clearCondition1 != 0)
		{
			((UIClearConditionItem)base.itemList[++num2]).Set(eventBattleDr.clearCondition1, eventBattleDr.clearCondition1_Value);
		}
		if (eventBattleDr.clearCondition2 != 0)
		{
			((UIClearConditionItem)base.itemList[++num2]).Set(eventBattleDr.clearCondition2, eventBattleDr.clearCondition2_Value);
		}
	}

	public void Init(EventRaidTabType type, List<Protocols.EventRaidData> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.EventRaidData eventRaidData = list[i];
			UIEventRaidListItem uIEventRaidListItem = base.itemList[i] as UIEventRaidListItem;
			if (!(uIEventRaidListItem == null))
			{
				uIEventRaidListItem.Set(type, eventRaidData);
				UISetter.SetGameObjectName(uIEventRaidListItem.gameObject, $"{idPrefix}{eventRaidData.bossId}");
			}
		}
	}

	public void Init(List<Protocols.EventRaidRankingData> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.EventRaidRankingData data = list[i];
			UIEventRaidJoinListItem uIEventRaidJoinListItem = base.itemList[i] as UIEventRaidJoinListItem;
			if (!(uIEventRaidJoinListItem == null))
			{
				uIEventRaidJoinListItem.Set(data);
			}
		}
	}

	public void Init(List<EventBattleScenarioDataRow> list, int lastClearIdx, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			EventBattleScenarioDataRow eventBattleScenarioDataRow = list[i];
			EventScenarioItem eventScenarioItem = base.itemList[i] as EventScenarioItem;
			if (!(eventScenarioItem == null))
			{
				eventScenarioItem.Set(eventBattleScenarioDataRow, lastClearIdx);
				UISetter.SetGameObjectName(eventScenarioItem.gameObject, $"{idPrefix}{eventBattleScenarioDataRow.sort}");
			}
		}
	}

	public void Init(List<WorldMapDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			WorldMapDataRow worldMapDataRow = list[i];
			UIWorldMapNavigationItem uIWorldMapNavigationItem = base.itemList[i] as UIWorldMapNavigationItem;
			if (!(uIWorldMapNavigationItem == null))
			{
				uIWorldMapNavigationItem.Set(worldMapDataRow);
				UISetter.SetGameObjectName(uIWorldMapNavigationItem.gameObject, $"{idPrefix}{worldMapDataRow.id}");
			}
		}
	}

	public void InitAttendanceDayList(List<string> list, Dictionary<string, CarnivalDataRow> carnivalList, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		for (int i = 0; i < list.Count; i++)
		{
			string text = list[i];
			UICarnivalDayListItem uICarnivalDayListItem = base.itemList[i] as UICarnivalDayListItem;
			if (uICarnivalDayListItem == null)
			{
				continue;
			}
			CarnivalDataRow carnivalDataRow = carnivalList[text];
			Protocols.CarnivalList.ProcessData processData = null;
			if (localUser.carnivalList.carnivalProcessList.ContainsKey(carnivalDataRow.cTidx))
			{
				if (localUser.carnivalList.carnivalProcessList[carnivalDataRow.cTidx].ContainsKey(carnivalDataRow.idx))
				{
					processData = localUser.carnivalList.carnivalProcessList[carnivalDataRow.cTidx][carnivalDataRow.idx];
				}
				else
				{
					processData.complete = 0;
					processData.receive = 0;
				}
			}
			uICarnivalDayListItem.Set(text, processData.complete == 1 && processData.receive == 0);
			uICarnivalDayListItem.SetSelection(selected: false);
			UISetter.SetGameObjectName(uICarnivalDayListItem.gameObject, $"{idPrefix}{text}");
		}
	}

	public void InitEventRemaing(List<EventRemaingTimeDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			EventRemaingTimeDataRow eventRemaingTimeDataRow = list[i];
			UIEventRemaingItem uIEventRemaingItem = base.itemList[i] as UIEventRemaingItem;
			if (!(uIEventRemaingItem == null))
			{
				uIEventRemaingItem.Set(eventRemaingTimeDataRow);
				UISetter.SetGameObjectName(uIEventRemaingItem.gameObject, $"{idPrefix}{eventRemaingTimeDataRow.idx}");
			}
		}
	}

	public void InitPredeckSlot(RoTroop[] list, int count, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(count);
		for (int i = 0; i < count; i++)
		{
			RoTroop troop = list[i];
			PreDeckListItem preDeckListItem = base.itemList[i] as PreDeckListItem;
			if (!(preDeckListItem == null))
			{
				preDeckListItem.Set(troop, i);
				UISetter.SetGameObjectName(preDeckListItem.gameObject, idPrefix + i);
				preDeckListItem.SetSelection(selected: false);
			}
		}
	}

	public void InitMercenaryList(List<RoLocalUser.MercynaryUserList> list, RoTroop troop, BattleData battleData, string idPrefix = null, ECharacterType charType = ECharacterType.None)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			UIMercenaryListItem uIMercenaryListItem = base.itemList[i] as UIMercenaryListItem;
			uIMercenaryListItem.Init(list[i], troop, battleData);
			UISetter.SetActive(uIMercenaryListItem, active: true);
		}
	}

	public void InitWeapon(List<RoWeapon> list, string unitId, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoWeapon roWeapon = list[i];
			UICommanderWeaponItem uICommanderWeaponItem = base.itemList[i] as UICommanderWeaponItem;
			if (!(uICommanderWeaponItem == null))
			{
				uICommanderWeaponItem.Set(roWeapon, unitId, roWeapon.data.slotType);
				uICommanderWeaponItem.SetSelection(selected: false);
				UISetter.SetGameObjectName(uICommanderWeaponItem.gameObject, $"{idPrefix}{roWeapon.idx}");
			}
		}
	}

	public void InitWeapon(List<RoWeapon> list, List<int> decompositionList, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			RoWeapon roWeapon = list[i];
			UICommanderWeaponItem uICommanderWeaponItem = base.itemList[i] as UICommanderWeaponItem;
			if (!(uICommanderWeaponItem == null))
			{
				uICommanderWeaponItem.Set(roWeapon, decompositionList.Contains(int.Parse(roWeapon.idx)));
				uICommanderWeaponItem.SetSelection(selected: false);
				UISetter.SetGameObjectName(uICommanderWeaponItem.gameObject, $"{idPrefix}{roWeapon.idx}");
			}
		}
	}

	public void InitWeaponProgress(Dictionary<int, int> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<int, int> item in list)
		{
			UIWeaponProgressListItem uIWeaponProgressListItem = base.itemList[num] as UIWeaponProgressListItem;
			if (!(uIWeaponProgressListItem == null))
			{
				uIWeaponProgressListItem.Set(item.Key, item.Value);
				UISetter.SetGameObjectName(uIWeaponProgressListItem.gameObject, $"{idPrefix}{item.Key}");
				num++;
			}
		}
	}

	public void InitBoxList(List<GoodsDataRow> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			GoodsDataRow goodsDataRow = list[i];
			UIBoxListItem uIBoxListItem = base.itemList[i] as UIBoxListItem;
			if (!(uIBoxListItem == null))
			{
				uIBoxListItem.Set(goodsDataRow.type);
				uIBoxListItem.SetSelection(selected: false);
				UISetter.SetGameObjectName(uIBoxListItem.gameObject, $"{idPrefix}{goodsDataRow.type}");
			}
		}
	}

	public void InitWeaponHistoryList(List<List<string>> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			List<string> list2 = list[i];
			UIWeaponProgressHistoryItem uIWeaponProgressHistoryItem = base.itemList[i] as UIWeaponProgressHistoryItem;
			if (!(uIWeaponProgressHistoryItem == null))
			{
				UISetter.SetActive(uIWeaponProgressHistoryItem.gameObject, active: true);
				uIWeaponProgressHistoryItem.Set(list2);
				uIWeaponProgressHistoryItem.SetSelection(selected: false);
				UISetter.SetGameObjectName(uIWeaponProgressHistoryItem.gameObject, $"{idPrefix}{$"{list2[4]}_{list2[5]}_{list2[6]}_{list2[7]}"}");
			}
		}
	}

	public void InitInfinityList(Dictionary<string, Dictionary<int, int>> list, string idPrefix = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		base.itemIdPrefix = idPrefix;
		ResizeItemList(list.Count);
		int num = 0;
		foreach (KeyValuePair<string, Dictionary<int, int>> item in list)
		{
			InfinityTabListItem infinityTabListItem = base.itemList[num] as InfinityTabListItem;
			if (!(infinityTabListItem == null))
			{
				infinityTabListItem.Set(item.Key, item.Value);
				infinityTabListItem.SetSelection(selected: false);
				UISetter.SetGameObjectName(infinityTabListItem.gameObject, $"{idPrefix}{item.Key}");
				num++;
			}
		}
	}
}
