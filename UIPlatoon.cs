using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIPlatoon : UIItemBase
{
	public UILabel number;

	public UILabel nickname;

	public UIStatus status;

	public UICommander commanderSlot;

	public UIUnit supporterSlot;

	public List<UIUnit> unitSlotList;

	public GameObject unitSlotsRoot;

	public GameObject organizationButton;

	public GameObject changeNicknameButton;

	public GameObject unitPositionRoot;

	public UIUnit unitPositionItem;

	private List<UIUnit> _unitPositionList;

	[HideInInspector]
	public string organizationButtonPrefix = "Organization-";

	[HideInInspector]
	public string changeNicknameButtonPrefix = "ChangeNickname-";

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

	public void Set(RoTroop troop)
	{
		if (troop == null)
		{
			return;
		}
		UISetter.SetGameObjectName(organizationButton, organizationButtonPrefix + troop.id);
		UISetter.SetGameObjectName(changeNicknameButton, changeNicknameButtonPrefix + troop.id);
		UISetter.SetLabel(number, Localization.Format("Troop.NumberingFormat", troop.number));
		UISetter.SetLabel(nickname, troop.nickname);
		if (commanderSlot != null)
		{
			commanderSlot.Set(troop.commanderId);
		}
		if (unitSlotList == null || unitSlotList.Count <= 0)
		{
			unitSlotList = new List<UIUnit>();
			UIUnit[] componentsInChildren = unitSlotsRoot.GetComponentsInChildren<UIUnit>(includeInactive: true);
			unitSlotList.AddRange(componentsInChildren);
		}
		if (unitSlotList != null)
		{
			DataTable<UnitDataRow> unitDtbl = RemoteObjectManager.instance.regulation.unitDtbl;
			for (int i = 0; i < unitSlotList.Count; i++)
			{
				UIUnit uIUnit = unitSlotList[i];
				if (!(uIUnit == null))
				{
					RoTroop.Slot slot = troop.slots[i + 1];
					slot.Set(slot);
					uIUnit.SetSelection(selected: false);
				}
			}
		}
		UISetter.SetStatus(status, troop);
	}
}
