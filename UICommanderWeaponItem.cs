using System;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICommanderWeaponItem : UIItemBase
{
	[Serializable]
	public class WeaponStatus
	{
		public UILabel name;

		public UILabel value;

		public GameObject emptyRoot;

		public void WeaponEmpty()
		{
			UISetter.SetActive(emptyRoot, active: false);
			UISetter.SetActive(name, active: false);
			UISetter.SetActive(value, active: false);
		}

		public void StatusEmpty()
		{
			UISetter.SetActive(emptyRoot, active: true);
			UISetter.SetActive(name, active: false);
			UISetter.SetActive(value, active: false);
		}

		public void SetLabel(string name, float value)
		{
			UISetter.SetActive(emptyRoot, active: false);
			UISetter.SetActive(this.name, active: true);
			UISetter.SetActive(this.value, active: true);
			UISetter.SetLabel(this.name, name);
			if (value < 0f)
			{
				UISetter.SetColor(this.value, new Color(0.972549f, 0.1254902f, 8f / 51f));
				UISetter.SetLabel(this.value, $"{(int)value}");
			}
			else
			{
				UISetter.SetColor(this.value, new Color(0.41568628f, 0.6509804f, 0.20784314f));
				UISetter.SetLabel(this.value, $"+{(int)value}");
			}
		}
	}

	public UILabel lbName;

	public UILabel level;

	public UILabel levelNickname;

	public UILabel useCommander;

	public UISprite iconBG;

	public UISprite icon;

	public UIGrid grade;

	public GameObject newRoot;

	public List<WeaponStatus> statusList;

	public GameObject emptyRoot;

	public GameObject weaponRoot;

	public GameObject selectRoot;

	public GameObject decompositionRoot;

	public GameObject equipRoot;

	public GameObject lockRoot;

	public UILabel percent;

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(selectRoot, selected);
	}

	public void Set(RoWeapon weapon, string unitId, int slotType)
	{
		UISetter.SetGameObjectName(base.gameObject, $"Weapon-{slotType}");
		UISetter.SetActive(emptyRoot, weapon == null);
		UISetter.SetActive(weaponRoot, weapon != null);
		UISetter.SetActive(lockRoot, active: false);
		UISetter.SetLabel(lbName, string.Empty);
		UISetter.SetLabel(levelNickname, string.Empty);
		int num = 100;
		UnitDataRow unitDataRow = RemoteObjectManager.instance.regulation.unitDtbl[unitId];
		switch (slotType)
		{
		case 1:
			UISetter.SetLabel(percent, $"{unitDataRow.partHead}%");
			num = unitDataRow.partHead;
			break;
		case 2:
			UISetter.SetLabel(percent, $"{unitDataRow.partRightHand}%");
			num = unitDataRow.partRightHand;
			break;
		case 3:
			UISetter.SetLabel(percent, $"{unitDataRow.partLeftHand}%");
			num = unitDataRow.partLeftHand;
			break;
		case 4:
			UISetter.SetLabel(percent, $"{unitDataRow.partBody}%");
			num = unitDataRow.partBody;
			break;
		case 5:
			UISetter.SetLabel(percent, $"{unitDataRow.partSpecial}%");
			num = unitDataRow.partSpecial;
			break;
		}
		WeaponEmpty(weapon == null);
		if (weapon == null)
		{
			return;
		}
		UISetter.SetActive(newRoot, RemoteObjectManager.instance.localUser.newWeaponList.Contains(weapon.idx));
		UISetter.SetLabel(lbName, Localization.Get(weapon.data.weaponName));
		UISetter.SetLabel(levelNickname, Localization.Format("1021", weapon.level + "  " + Localization.Get(weapon.data.weaponName)));
		UISetter.SetSprite(iconBG, $"weaponback_{weapon.data.weaponGrade}");
		UISetter.SetSprite(icon, weapon.data.weaponIcon);
		UISetter.SetActive(level, weapon.level != 0);
		UISetter.SetLabel(level, $"+{weapon.level}");
		UISetter.SetRank(grade, weapon.data.weaponGrade);
		UISetter.SetActive(equipRoot, weapon.currEquipCommanderId > 0);
		for (int i = 0; i < statusList.Count; i++)
		{
			WeaponStatus weaponStatus = statusList[i];
			if (weapon.data.statType[i] == EItemStatType.EQUIPED)
			{
				weaponStatus.StatusEmpty();
			}
			else
			{
				weaponStatus.SetLabel(Localization.Get(GetWeaponStatusString(weapon.data.statType[i])), weapon.GetAddStatPoint(weapon.data.statType[i]) * num / 100);
			}
		}
		UISetter.SetActive(useCommander, RemoteObjectManager.instance.regulation.commanderDtbl.ContainsKey(weapon.currEquipCommanderId.ToString()));
		if (RemoteObjectManager.instance.regulation.commanderDtbl.ContainsKey(weapon.currEquipCommanderId.ToString()))
		{
			CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[weapon.currEquipCommanderId.ToString()];
			UISetter.SetLabel(useCommander, Localization.Format("70933", Localization.Get(commanderDataRow.nickname)));
		}
	}

	public void Lock(string unitId, int slotType)
	{
		UISetter.SetGameObjectName(base.gameObject, "Lock");
		UISetter.SetActive(emptyRoot, active: false);
		UISetter.SetActive(weaponRoot, active: false);
		UISetter.SetActive(lockRoot, active: true);
		int num = 100;
		UnitDataRow unitDataRow = RemoteObjectManager.instance.regulation.unitDtbl[unitId];
		switch (slotType)
		{
		case 1:
			UISetter.SetLabel(percent, $"{unitDataRow.partHead}%");
			num = unitDataRow.partHead;
			break;
		case 2:
			UISetter.SetLabel(percent, $"{unitDataRow.partRightHand}%");
			num = unitDataRow.partRightHand;
			break;
		case 3:
			UISetter.SetLabel(percent, $"{unitDataRow.partLeftHand}%");
			num = unitDataRow.partLeftHand;
			break;
		case 4:
			UISetter.SetLabel(percent, $"{unitDataRow.partBody}%");
			num = unitDataRow.partBody;
			break;
		case 5:
			UISetter.SetLabel(percent, $"{unitDataRow.partSpecial}%");
			num = unitDataRow.partSpecial;
			break;
		}
	}

	public void Set(RoWeapon weapon, bool decomposition)
	{
		UISetter.SetActive(emptyRoot, weapon == null);
		UISetter.SetActive(weaponRoot, weapon != null);
		UISetter.SetActive(lockRoot, active: false);
		WeaponEmpty(weapon == null);
		if (weapon != null)
		{
			UISetter.SetActive(decompositionRoot, decomposition);
			Set(weapon);
		}
	}

	public void Set(RoWeapon weapon)
	{
		UISetter.SetActive(emptyRoot, weapon == null);
		UISetter.SetActive(weaponRoot, weapon != null);
		UISetter.SetActive(lockRoot, active: false);
		WeaponEmpty(weapon == null);
		if (weapon == null)
		{
			return;
		}
		UISetter.SetActive(newRoot, RemoteObjectManager.instance.localUser.newWeaponList.Contains(weapon.idx));
		UISetter.SetLabel(lbName, Localization.Get(weapon.data.weaponName));
		UISetter.SetLabel(levelNickname, Localization.Format("1021", weapon.level + "  " + Localization.Get(weapon.data.weaponName)));
		UISetter.SetSprite(iconBG, $"weaponback_{weapon.data.weaponGrade}");
		UISetter.SetSprite(icon, weapon.data.weaponIcon);
		UISetter.SetActive(level, weapon.level != 0);
		UISetter.SetLabel(level, $"+{weapon.level}");
		UISetter.SetRank(grade, weapon.data.weaponGrade);
		UISetter.SetActive(equipRoot, weapon.currEquipCommanderId > 0);
		for (int i = 0; i < statusList.Count; i++)
		{
			WeaponStatus weaponStatus = statusList[i];
			if (weapon.data.statType[i] == EItemStatType.EQUIPED)
			{
				weaponStatus.StatusEmpty();
			}
			else
			{
				weaponStatus.SetLabel(Localization.Get(GetWeaponStatusString(weapon.data.statType[i])), weapon.GetAddStatPoint(weapon.data.statType[i]));
			}
		}
		UISetter.SetActive(useCommander, RemoteObjectManager.instance.regulation.commanderDtbl.ContainsKey(weapon.currEquipCommanderId.ToString()));
		if (RemoteObjectManager.instance.regulation.commanderDtbl.ContainsKey(weapon.currEquipCommanderId.ToString()))
		{
			CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[weapon.currEquipCommanderId.ToString()];
			UISetter.SetLabel(useCommander, Localization.Format("70933", Localization.Get(commanderDataRow.nickname)));
		}
	}

	private string GetWeaponStatusString(EItemStatType type)
	{
		string result = string.Empty;
		switch (type)
		{
		case EItemStatType.ATK:
			result = "1";
			break;
		case EItemStatType.DEF:
			result = "2";
			break;
		case EItemStatType.ACCUR:
			result = "5";
			break;
		case EItemStatType.LUCK:
			result = "3";
			break;
		case EItemStatType.CRITR:
			result = "6";
			break;
		case EItemStatType.CRITDMG:
			result = "8";
			break;
		case EItemStatType.MOB:
			result = "7";
			break;
		}
		return result;
	}

	public void WeaponEmpty(bool empty)
	{
		UISetter.SetLabel(lbName, string.Empty);
		UISetter.SetLabel(levelNickname, string.Empty);
		if (statusList.Count <= 0)
		{
			return;
		}
		statusList.ForEach(delegate(WeaponStatus row)
		{
			if (empty)
			{
				row.WeaponEmpty();
			}
			else
			{
				row.StatusEmpty();
			}
		});
	}
}
