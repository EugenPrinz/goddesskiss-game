using System.Collections;
using System.Collections.Generic;
using Shared;
using Shared.Regulation;
using UnityEngine;

public class UICommanderList : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public UIDefaultListView commanderListView;

	private List<RoCommander> EngageCommander_JobClass = new List<RoCommander>();

	public UIFlipSwitch commanderTab;

	public UIFlipSwitch mercenaryTab;

	public UIFlipSwitch basicTab;

	public UIFlipSwitch attackTab;

	public UIFlipSwitch defenseTab;

	public UIFlipSwitch supportTab;

	public UIFlipSwitch rankSortTab;

	public UIFlipSwitch levelSortTab;

	public UIFlipSwitch classSortTab;

	public GameObject EmptyEngage;

	public UIGrid commanderGrid;

	public GameObject sortTab;

	private BattleData battleData;

	private EJob selectJob;

	private ESortType sortType;

	private ECharacterType currCharacterType;

	private RoTroop _currentTroop;

	private List<RoTroop> troopList;

	private List<RoCommander> list = new List<RoCommander>();

	private string selectedMerceynaryCommanderId;

	public UIDefaultListView mercenaryList;

	public GameObject commanderList;

	public GameObject NoMercenaryListLabel;

	public void Init(List<RoTroop> troopList, int selectTroop, BattleData battleData)
	{
		this.troopList = troopList;
		_currentTroop = troopList[selectTroop];
		this.battleData = battleData;
		InitTab();
		OnRefresh();
		OpenPopup();
	}

	private void InitTab()
	{
		basicTab.Lock(_isLock: false);
		attackTab.Lock(_isLock: false);
		defenseTab.Lock(_isLock: false);
		supportTab.Lock(_isLock: false);
		UISetter.SetFlipSwitch(commanderTab, state: true);
		UISetter.SetFlipSwitch(mercenaryTab, state: false);
		UISetter.SetFlipSwitch(basicTab, state: true);
		UISetter.SetFlipSwitch(attackTab, state: false);
		UISetter.SetFlipSwitch(defenseTab, state: false);
		UISetter.SetFlipSwitch(supportTab, state: false);
		UISetter.SetFlipSwitch(rankSortTab, state: true);
		UISetter.SetFlipSwitch(levelSortTab, state: false);
		UISetter.SetFlipSwitch(classSortTab, state: false);
		currCharacterType = ECharacterType.Commander;
		selectJob = EJob.All;
		sortType = ESortType.Rank;
		commanderGrid.enabled = true;
		commanderGrid.cellHeight = ((battleData.type != EBattleType.Annihilation) ? 240 : 275);
		commanderGrid.Reposition();
		commanderGrid.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
		if (battleData.type == EBattleType.Guerrilla)
		{
			SweepDataRow sweepDataRow = base.regulation.FindSweepRow(battleData.sweepType, battleData.sweepLevel);
			selectJob = (EJob)sweepDataRow.type;
			basicTab.Lock(_isLock: true);
			attackTab.Lock(selectJob != EJob.Attack);
			defenseTab.Lock(selectJob != EJob.Defense);
			supportTab.Lock(selectJob != EJob.Support);
			UISetter.SetFlipSwitch(basicTab, state: false);
			UISetter.SetFlipSwitch(attackTab, selectJob == EJob.Attack);
			UISetter.SetFlipSwitch(defenseTab, selectJob == EJob.Defense);
			UISetter.SetFlipSwitch(supportTab, selectJob == EJob.Support);
		}
		else if (battleData.type == EBattleType.EventBattle)
		{
			EventBattleFieldDataRow eventBattleFieldDataRow = base.regulation.FindEventBattle(battleData.eventId, battleData.eventLevel);
			EJob job = eventBattleFieldDataRow.job;
			if (job != 0 && job != EJob.All)
			{
				if (job <= EJob.Support)
				{
					selectJob = job;
				}
				else
				{
					switch (job)
					{
					case EJob.Attack_x:
						selectJob = EJob.Defense;
						break;
					case EJob.Defense_x:
						selectJob = EJob.Attack;
						break;
					case EJob.Support_x:
						selectJob = EJob.Attack;
						break;
					}
				}
				basicTab.Lock(_isLock: true);
				attackTab.Lock(job == EJob.Attack_x || job == EJob.Defense || job == EJob.Support);
				defenseTab.Lock(job == EJob.Defense_x || job == EJob.Attack || job == EJob.Support);
				supportTab.Lock(job == EJob.Support_x || job == EJob.Attack || job == EJob.Defense);
				UISetter.SetFlipSwitch(basicTab, state: false);
				UISetter.SetFlipSwitch(attackTab, selectJob == EJob.Attack);
				UISetter.SetFlipSwitch(defenseTab, selectJob == EJob.Defense);
				UISetter.SetFlipSwitch(supportTab, selectJob == EJob.Support);
			}
		}
		switch (battleData.type)
		{
		case EBattleType.Plunder:
		case EBattleType.Raid:
		case EBattleType.WaveBattle:
		case EBattleType.EventBattle:
			mercenaryTab.Lock(_isLock: false);
			break;
		case EBattleType.Annihilation:
			mercenaryTab.Lock(_isLock: true);
			break;
		default:
			mercenaryTab.Lock(_isLock: true);
			break;
		}
		SetTab(isMercenary: false);
	}

	private void SetTab(bool isMercenary)
	{
		UISetter.SetActive(attackTab, !isMercenary);
		UISetter.SetActive(defenseTab, !isMercenary);
		UISetter.SetActive(supportTab, !isMercenary);
		UISetter.SetActive(sortTab, !isMercenary);
		UISetter.SetActive(commanderList, !isMercenary);
		UISetter.SetActive(mercenaryList, isMercenary);
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		if (currCharacterType == ECharacterType.Commander)
		{
			SetCommanderSelectList();
		}
		else
		{
			SetEngageCommanderList();
		}
		commanderListView.ResetPosition();
	}

	private void SetCommanderSelectList()
	{
		UISetter.SetActive(EmptyEngage, active: false);
		if (battleData.type != EBattleType.Annihilation)
		{
			list = base.localUser.GetCommanderList(selectJob);
		}
		else
		{
			list = base.localUser.GetAdvancePossibleCommanderList(selectJob);
		}
		list = ListSort(list);
		commanderListView.Init(list, troopList, battleData, "Commander_", ECharacterType.Commander);
	}

	private List<RoCommander> ListSort(List<RoCommander> list)
	{
		if (sortType == ESortType.Rank)
		{
			list.Sort(delegate(RoCommander row, RoCommander row1)
			{
				RoTroop.Slot slot5 = null;
				RoTroop.Slot slot6 = null;
				int num5 = -1;
				int num6 = -1;
				for (int k = 0; k < troopList.Count; k++)
				{
					if (slot5 == null)
					{
						RoTroop.Slot slotByCommanderId5 = troopList[k].GetSlotByCommanderId(row.id, currCharacterType);
						if (slotByCommanderId5 != null && slotByCommanderId5.charType == ECharacterType.Commander)
						{
							num5 = k;
							slot5 = slotByCommanderId5;
						}
					}
					if (slot6 == null)
					{
						RoTroop.Slot slotByCommanderId6 = troopList[k].GetSlotByCommanderId(row1.id, currCharacterType);
						if (slotByCommanderId6 != null && slotByCommanderId6.charType == ECharacterType.Commander)
						{
							num6 = k;
							slot6 = slotByCommanderId6;
						}
					}
				}
				if (slot5 != null && slot6 == null)
				{
					return -1;
				}
				if (slot5 == null && slot6 != null)
				{
					return 1;
				}
				if (num5 > num6)
				{
					return 1;
				}
				if (num5 < num6)
				{
					return -1;
				}
				if ((int)row.rank > (int)row1.rank)
				{
					return -1;
				}
				if ((int)row.rank < (int)row1.rank)
				{
					return 1;
				}
				if ((int)row.level > (int)row1.level)
				{
					return -1;
				}
				if ((int)row.level < (int)row1.level)
				{
					return 1;
				}
				if ((int)row.cls > (int)row1.cls)
				{
					return -1;
				}
				if ((int)row.cls < (int)row1.cls)
				{
					return 1;
				}
				if (int.Parse(row.id) > int.Parse(row1.id))
				{
					return 1;
				}
				return (int.Parse(row.id) < int.Parse(row1.id)) ? (-1) : 0;
			});
		}
		else if (sortType == ESortType.Level)
		{
			list.Sort(delegate(RoCommander row, RoCommander row1)
			{
				RoTroop.Slot slot3 = null;
				RoTroop.Slot slot4 = null;
				int num3 = -1;
				int num4 = -1;
				for (int j = 0; j < troopList.Count; j++)
				{
					if (slot3 == null)
					{
						RoTroop.Slot slotByCommanderId3 = troopList[j].GetSlotByCommanderId(row.id, currCharacterType);
						if (slotByCommanderId3 != null && slotByCommanderId3.charType == ECharacterType.Commander)
						{
							num3 = j;
							slot3 = slotByCommanderId3;
						}
					}
					if (slot4 == null)
					{
						RoTroop.Slot slotByCommanderId4 = troopList[j].GetSlotByCommanderId(row1.id, currCharacterType);
						if (slotByCommanderId4 != null && slotByCommanderId4.charType == ECharacterType.Commander)
						{
							num4 = j;
							slot4 = slotByCommanderId4;
						}
					}
				}
				if (slot3 != null && slot4 == null)
				{
					return -1;
				}
				if (slot3 == null && slot4 != null)
				{
					return 1;
				}
				if (num3 > num4)
				{
					return 1;
				}
				if (num3 < num4)
				{
					return -1;
				}
				if ((int)row.level > (int)row1.level)
				{
					return -1;
				}
				if ((int)row.level < (int)row1.level)
				{
					return 1;
				}
				if ((int)row.rank > (int)row1.rank)
				{
					return -1;
				}
				if ((int)row.rank < (int)row1.rank)
				{
					return 1;
				}
				if ((int)row.cls > (int)row1.cls)
				{
					return -1;
				}
				if ((int)row.cls < (int)row1.cls)
				{
					return 1;
				}
				if (int.Parse(row.id) > int.Parse(row1.id))
				{
					return 1;
				}
				return (int.Parse(row.id) < int.Parse(row1.id)) ? (-1) : 0;
			});
		}
		else if (sortType == ESortType.Cls)
		{
			list.Sort(delegate(RoCommander row, RoCommander row1)
			{
				RoTroop.Slot slot = null;
				RoTroop.Slot slot2 = null;
				int num = -1;
				int num2 = -1;
				for (int i = 0; i < troopList.Count; i++)
				{
					if (slot == null)
					{
						RoTroop.Slot slotByCommanderId = troopList[i].GetSlotByCommanderId(row.id, currCharacterType);
						if (slotByCommanderId != null && slotByCommanderId.charType == ECharacterType.Commander)
						{
							num = i;
							slot = slotByCommanderId;
						}
					}
					if (slot2 == null)
					{
						RoTroop.Slot slotByCommanderId2 = troopList[i].GetSlotByCommanderId(row1.id, currCharacterType);
						if (slotByCommanderId2 != null && slotByCommanderId2.charType == ECharacterType.Commander)
						{
							num2 = i;
							slot2 = slotByCommanderId2;
						}
					}
				}
				if (slot != null && slot2 == null)
				{
					return -1;
				}
				if (slot == null && slot2 != null)
				{
					return 1;
				}
				if (num > num2)
				{
					return 1;
				}
				if (num < num2)
				{
					return -1;
				}
				if ((int)row.cls > (int)row1.cls)
				{
					return -1;
				}
				if ((int)row.cls < (int)row1.cls)
				{
					return 1;
				}
				if ((int)row.rank > (int)row1.rank)
				{
					return -1;
				}
				if ((int)row.rank < (int)row1.rank)
				{
					return 1;
				}
				if ((int)row.level > (int)row1.level)
				{
					return -1;
				}
				if ((int)row.level < (int)row1.level)
				{
					return 1;
				}
				if (int.Parse(row.id) > int.Parse(row1.id))
				{
					return 1;
				}
				return (int.Parse(row.id) < int.Parse(row1.id)) ? (-1) : 0;
			});
		}
		return list;
	}

	public void SetEngageCommanderList()
	{
		UISetter.SetActive(NoMercenaryListLabel, active: false);
		if (base.localUser.EngageCommander != null)
		{
			if (base.localUser.EngageCommander.Count == 0)
			{
				UISetter.SetActive(NoMercenaryListLabel, active: true);
			}
			else
			{
				mercenaryList.InitMercenaryList(base.localUser.allMercynaryList, _currentTroop, battleData, "Mercenary_");
			}
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close")
		{
			ClosePopup();
			return;
		}
		if (text.StartsWith("AddButton-"))
		{
			string commanderId = text.Substring(text.IndexOf("-") + 1);
			AddCommander(commanderId);
			return;
		}
		if (text.StartsWith("RemoveButton-"))
		{
			string commanderId2 = text.Substring(text.IndexOf("-") + 1);
			RemoveCommander(commanderId2);
			return;
		}
		if (text.StartsWith("DieButton-"))
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("80012"));
			return;
		}
		switch (text)
		{
		case "AllTab":
			selectJob = EJob.All;
			OnRefresh();
			break;
		case "AttackTab":
			selectJob = EJob.Attack;
			OnRefresh();
			break;
		case "DefenseTab":
			selectJob = EJob.Defense;
			OnRefresh();
			break;
		case "SupportTab":
			selectJob = EJob.Support;
			OnRefresh();
			break;
		case "RankSortTab":
			sortType = ESortType.Rank;
			OnRefresh();
			break;
		case "LevelSortTab":
			sortType = ESortType.Level;
			OnRefresh();
			break;
		case "ClassSortTab":
			sortType = ESortType.Cls;
			OnRefresh();
			break;
		case "MercenaryTab":
			currCharacterType = ECharacterType.Mercenary;
			if (battleData.type == EBattleType.Plunder || battleData.type == EBattleType.Raid || battleData.type == EBattleType.WaveBattle || battleData.type == EBattleType.EventBattle)
			{
				if (base.localUser.EngageCommander == null || base.localUser.EngageCommander.Count == 0)
				{
					base.network.RequestGuildDispatchCommanderList((int)battleData.type);
				}
				else
				{
					SetEngageCommanderList();
				}
			}
			else
			{
				SetEngageCommanderList();
			}
			UISetter.SetFlipSwitch(mercenaryTab, state: true);
			UISetter.SetFlipSwitch(commanderTab, state: false);
			SetTab(isMercenary: true);
			commanderListView.ResetPosition();
			break;
		case "MercenaryLock":
			if (battleData.type == EBattleType.Guerrilla)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("16029"));
			}
			else if (battleData.type == EBattleType.Duel || battleData.type == EBattleType.WaveDuel || battleData.type == EBattleType.Defender || battleData.type == EBattleType.WaveDuelDefender)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("17089"));
			}
			else if (battleData.type == EBattleType.Conquest)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110078"));
			}
			else if (battleData.type == EBattleType.CooperateBattle)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5090036"));
			}
			else if (battleData.type == EBattleType.EventRaid)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("17099"));
			}
			break;
		case "CommanderTab":
			currCharacterType = ECharacterType.Commander;
			SetCommanderSelectList();
			UISetter.SetFlipSwitch(mercenaryTab, state: false);
			UISetter.SetFlipSwitch(commanderTab, state: true);
			UISetter.SetActive(NoMercenaryListLabel, active: false);
			SetTab(isMercenary: false);
			commanderListView.ResetPosition();
			break;
		default:
			if (text.StartsWith("EngageAddButton-"))
			{
				string s = text.Substring(text.IndexOf("-") + 1);
				int index = int.Parse(s);
				AddCommander(list[index].id, list[index].userIdx);
			}
			break;
		}
	}

	private List<RoCommander> MercenaryList(List<RoCommander> _commanderList, EJob _job)
	{
		if (_commanderList == null)
		{
			return null;
		}
		if (_job == EJob.All)
		{
			return _commanderList;
		}
		List<RoCommander> list = new List<RoCommander>();
		for (int i = 0; i < _commanderList.Count; i++)
		{
			if (_commanderList[i].job == _job)
			{
				list.Add(_commanderList[i]);
			}
		}
		return list;
	}

	private void AddCommander(string commanderId, int userIdx = -1)
	{
		if (!_currentTroop.IsAddSlotPossible())
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("1329"));
			return;
		}
		RoCommander roCommander = new RoCommander();
		if (currCharacterType == ECharacterType.Commander)
		{
			roCommander = base.localUser.FindCommander(commanderId);
		}
		for (int i = 0; i < troopList.Count; i++)
		{
			if (troopList[i].GetSlotByCommanderId(commanderId) != null)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("1366"));
				return;
			}
		}
		RoTroop.Slot nextEmptySlot = _currentTroop.GetNextEmptySlot();
		nextEmptySlot.unitId = roCommander.unitId;
		nextEmptySlot.unitLevel = roCommander.level;
		nextEmptySlot.exp = roCommander.aExp;
		nextEmptySlot.health = roCommander.hp;
		nextEmptySlot.unitCls = roCommander.cls;
		nextEmptySlot.unitCostume = roCommander.currentCostume;
		nextEmptySlot.favorRewardStep = roCommander.favorRewardStep;
		nextEmptySlot.marry = roCommander.marry;
		nextEmptySlot.transcendence = roCommander.transcendence;
		nextEmptySlot.unitRank = roCommander.rank;
		nextEmptySlot.commanderId = roCommander.id;
		nextEmptySlot.charType = roCommander.charType;
		nextEmptySlot.userName = roCommander.userName;
		nextEmptySlot.userIdx = roCommander.userIdx;
		nextEmptySlot.existEngage = roCommander.existEngaged;
		nextEmptySlot.equipItem = roCommander.GetEquipItemList();
		nextEmptySlot.weaponItem = roCommander.WeaponItem;
		for (int j = 0; j < roCommander.unitReg.skillDrks.Count; j++)
		{
			Troop.Slot.Skill skill = new Troop.Slot.Skill();
			SkillDataRow skillDataRow = base.regulation.skillDtbl[roCommander.unitReg.skillDrks[j]];
			if (skillDataRow.isActiveSkillType)
			{
				skill.sp = roCommander.sp;
			}
			skill.id = roCommander.unitReg.skillDrks[j];
			skill.lv = roCommander.skillList[j];
			nextEmptySlot.skills.Add(skill);
		}
		for (int k = 0; k < _currentTroop.slots.Length; k++)
		{
			if (_currentTroop.GetSlotIndexByPosition(k) < 0)
			{
				nextEmptySlot.position = k;
				break;
			}
		}
		SoundManager.PlaySFX("BTN_Formation_001");
		if (currCharacterType == ECharacterType.Commander)
		{
			commanderListView.Init(list, troopList, battleData, "Commander_", ECharacterType.Commander);
		}
		else
		{
			commanderListView.Init(list, _currentTroop, battleData, "EngageCommander_", ECharacterType.Mercenary);
		}
	}

	private void RemoveCommander(string commanderId)
	{
		int num = -1;
		RoTroop roTroop = null;
		for (int i = 0; i < troopList.Count; i++)
		{
			num = troopList[i].GetSlotIndexByCommanderId(commanderId);
			if (num >= 0)
			{
				roTroop = troopList[i];
				break;
			}
		}
		if (roTroop != null && num >= 0)
		{
			RoTroop.Slot[] slots = roTroop.slots;
			int position = slots[num].position;
			slots[num].ResetSlot();
			SoundManager.PlaySFX("BTN_Norma_001");
			roTroop.TrimSlot();
			if (currCharacterType == ECharacterType.Commander)
			{
				commanderListView.Init(list, troopList, battleData, "Commander_", ECharacterType.Commander);
			}
			else
			{
				commanderListView.Init(list, _currentTroop, battleData, "EngageCommander_", ECharacterType.Mercenary);
			}
		}
	}

	private bool CheckAlreadyMercenary()
	{
		if (_currentTroop != null && _currentTroop.slots != null)
		{
			int num = _currentTroop.slots.Length;
			for (int i = 0; i < num; i++)
			{
				if (_currentTroop.slots[i].charType == ECharacterType.Mercenary || _currentTroop.slots[i].charType == ECharacterType.NPCMercenary)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsPossibleEngage(string commanderId)
	{
		if (!_currentTroop.IsAddSlotPossible())
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("1329"));
			return false;
		}
		for (int i = 0; i < troopList.Count; i++)
		{
			RoTroop.Slot slotByCommanderId = troopList[i].GetSlotByCommanderId(commanderId);
			if (slotByCommanderId != null && slotByCommanderId.charType != ECharacterType.Mercenary && slotByCommanderId.charType != ECharacterType.NPCMercenary)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("1366"));
				return false;
			}
		}
		return true;
	}

	public bool IsAlreadyEngageMercenary()
	{
		if (CheckAlreadyMercenary())
		{
			return true;
		}
		return false;
	}

	public void RemoveMercenary(string commanderId)
	{
		int num = -1;
		RoTroop roTroop = null;
		for (int i = 0; i < troopList.Count; i++)
		{
			num = troopList[i].GetSlotIndexByCommanderId(commanderId);
			if (num >= 0)
			{
				roTroop = troopList[i];
				break;
			}
		}
		if (roTroop != null && num >= 0)
		{
			RoTroop.Slot[] slots = roTroop.slots;
			int position = slots[num].position;
			slots[num].ResetSlot();
			SoundManager.PlaySFX("BTN_Norma_001");
			roTroop.TrimSlot();
		}
	}

	public void AddMercenary(RoCommander commander)
	{
		RoTroop.Slot nextEmptySlot = _currentTroop.GetNextEmptySlot();
		nextEmptySlot.unitId = commander.unitId;
		nextEmptySlot.unitLevel = commander.level;
		nextEmptySlot.exp = commander.aExp;
		nextEmptySlot.health = commander.hp;
		nextEmptySlot.unitCls = commander.cls;
		nextEmptySlot.unitCostume = commander.currentCostume;
		nextEmptySlot.favorRewardStep = commander.favorRewardStep;
		nextEmptySlot.marry = commander.marry;
		nextEmptySlot.transcendence = commander.transcendence;
		nextEmptySlot.unitRank = commander.rank;
		nextEmptySlot.commanderId = commander.id;
		nextEmptySlot.charType = commander.charType;
		nextEmptySlot.userName = commander.userName;
		nextEmptySlot.userIdx = commander.userIdx;
		nextEmptySlot.existEngage = commander.existEngaged;
		nextEmptySlot.equipItem = commander.GetEquipItemList();
		nextEmptySlot.weaponItem = commander.WeaponItem;
		for (int i = 0; i < commander.unitReg.skillDrks.Count; i++)
		{
			Troop.Slot.Skill skill = new Troop.Slot.Skill();
			SkillDataRow skillDataRow = base.regulation.skillDtbl[commander.unitReg.skillDrks[i]];
			if (skillDataRow.isActiveSkillType)
			{
				skill.sp = commander.sp;
			}
			skill.id = commander.unitReg.skillDrks[i];
			skill.lv = commander.skillList[i];
			nextEmptySlot.skills.Add(skill);
		}
		for (int j = 0; j < _currentTroop.slots.Length; j++)
		{
			if (_currentTroop.GetSlotIndexByPosition(j) < 0)
			{
				nextEmptySlot.position = j;
				break;
			}
		}
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		if (base.uiWorld.existRankingBattle && base.uiWorld.rankingBattle.isActive)
		{
			base.uiWorld.readyBattle.OnRefresh();
		}
		else
		{
			UIManager.instance.RefreshOpenedUI();
		}
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0f);
		base.Close();
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
	}

	private void HidePopup()
	{
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
