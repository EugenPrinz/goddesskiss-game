using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UITroop : UIItemBase
{
	public UILabel number;

	public UILabel nickname;

	public UIStatus status;

	public GameObject commanderSlotsRoot;

	public List<UICommander> commanderSlotList;

	public UICommander commanderSlot;

	public UIUnit supporterSlot;

	public GameObject unitSlotsRoot;

	public List<UIUnit> unitSlotList;

	public GameObject unitFormationRoot;

	public List<UIUnit> unitPositionList;

	public List<UIPositionPlate> positionPlateList;

	public GameObject organizationButton;

	public GameObject changeNicknameButton;

	public UILabel defenderBtnLabel;

	public GameObject defenderButton;

	public GameObject notEmpty;

	public static readonly string organizationButtonPrefix = "Organization-";

	public static readonly string changeNicknameButtonPrefix = "ChangeNickname-";

	public static readonly string defenderButtonPrefix = "Defender-";

	public GameObject ClearMakr;

	public void SetUnitSlotSelection(int slotIdx, bool selected)
	{
		if (slotIdx >= 0)
		{
			if (slotIdx == 0)
			{
				commanderSlot.SetSelection(selected);
			}
			else if (slotIdx - 1 == unitSlotList.Count)
			{
				supporterSlot.SetSelection(selected);
			}
			else if (slotIdx - 1 <= unitSlotList.Count)
			{
				unitSlotList[slotIdx - 1].SetSelection(selected);
			}
		}
	}

	public void Set(RoTroop troop, EBattleType battleType = EBattleType.Undefined)
	{
		if (troop == null)
		{
			UISetter.SetActive(notEmpty, active: false);
			return;
		}
		UISetter.SetActive(notEmpty, active: true);
		UISetter.SetGameObjectName(organizationButton, organizationButtonPrefix + troop.id);
		UISetter.SetGameObjectName(changeNicknameButton, changeNicknameButtonPrefix + troop.id);
		UISetter.SetGameObjectName(defenderButton, defenderButtonPrefix + troop.id);
		UISetter.SetLabel(number, Localization.Format("5931", troop.number));
		bool flag = AnnihilationWaveClear(troop, battleType);
		UISetter.SetLabel(nickname, troop.nickname);
		UISetter.SetStatus(status, troop);
		SetSlots(troop);
		SetFormation(troop, !flag);
		UISetter.SetActive(ClearMakr, flag);
	}

	public void SetSlots(RoTroop troop)
	{
		if (commanderSlotsRoot != null && (commanderSlotList == null || commanderSlotList.Count <= 0))
		{
			UICommander[] componentsInChildren = commanderSlotsRoot.GetComponentsInChildren<UICommander>(includeInactive: true);
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				commanderSlotList = new List<UICommander>();
				commanderSlotList.AddRange(componentsInChildren);
			}
		}
		if (commanderSlotList != null)
		{
			for (int i = 0; i < commanderSlotList.Count; i++)
			{
				UICommander uICommander = commanderSlotList[i];
				if (!(uICommander == null))
				{
					RoTroop.Slot slot = troop.slots[i];
					uICommander.SetCommanderSlot(slot);
					UISetter.SetGameObjectName(uICommander.gameObject, $"CommanderSlot-{slot.commanderId}");
					uICommander.SetSelection(selected: false);
				}
			}
		}
		if (unitSlotsRoot != null && (unitSlotList == null || unitSlotList.Count <= 0))
		{
			UIUnit[] componentsInChildren2 = unitSlotsRoot.GetComponentsInChildren<UIUnit>(includeInactive: true);
			if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
			{
				unitSlotList = new List<UIUnit>();
				unitSlotList.AddRange(componentsInChildren2);
			}
		}
		if (unitSlotList == null)
		{
			return;
		}
		for (int j = 0; j < unitSlotList.Count; j++)
		{
			UIUnit uIUnit = unitSlotList[j];
			UISetter.SetActive(uIUnit.positionRoot, active: true);
			if (!(uIUnit == null))
			{
				RoTroop.Slot slot2 = troop.slots[j];
				uIUnit.Set(slot2);
				uIUnit.SetSelection(selected: false);
			}
		}
	}

	public void SetFormation(RoTroop troop, bool isActive = true)
	{
		if (unitFormationRoot != null && (unitPositionList == null || unitPositionList.Count <= 0))
		{
			UIUnit[] componentsInChildren = unitFormationRoot.GetComponentsInChildren<UIUnit>(includeInactive: true);
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				unitPositionList = new List<UIUnit>();
				unitPositionList.AddRange(componentsInChildren);
			}
		}
		if (unitPositionList == null || unitPositionList.Count <= 0)
		{
			return;
		}
		DataTable<UnitDataRow> unitDtbl = RemoteObjectManager.instance.regulation.unitDtbl;
		for (int i = 0; i < unitPositionList.Count; i++)
		{
			UIUnit uIUnit = unitPositionList[i];
			UISetter.SetActive(uIUnit.positionRoot, active: true);
			if (!(uIUnit == null))
			{
				RoTroop.Slot slotByPosition = troop.GetSlotByPosition(i);
				uIUnit.Set(slotByPosition);
				uIUnit.SetPositionPlate(isActive);
			}
		}
	}

	private bool AnnihilationWaveClear(RoTroop troop, EBattleType battleType)
	{
		if (troop == null || battleType != EBattleType.Annihilation)
		{
			return false;
		}
		if (troop.slots != null && troop.slots.Length > 0)
		{
			int num = 0;
			for (int i = 0; i < troop.slots.Length; i++)
			{
				if (string.IsNullOrEmpty(troop.slots[i].unitId))
				{
					num++;
				}
			}
			if (num == troop.slots.Length)
			{
				return true;
			}
		}
		return false;
	}
}
