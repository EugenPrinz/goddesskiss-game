using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Regulation;

[JsonObject]
public class RoWeapon
{
	public string idx;

	public string wIdx;

	private int _level;

	public bool isEquip;

	public int statPoint;

	public int currEquipCommanderId;

	public WeaponDataRow data;

	public Dictionary<EItemStatType, int> addStatPoint = new Dictionary<EItemStatType, int>();

	public int level
	{
		get
		{
			return _level;
		}
		set
		{
			_level = value;
			RefreshAddStat();
		}
	}

	public static RoWeapon Create(string idx, string wIdx, int lv, int commanderId = 0)
	{
		RoWeapon roWeapon = new RoWeapon();
		WeaponDataRow weaponDataRow = RemoteObjectManager.instance.regulation.weaponDtbl.Find((WeaponDataRow row) => row.idx == wIdx);
		roWeapon.data = weaponDataRow;
		roWeapon.idx = idx;
		roWeapon.wIdx = wIdx;
		roWeapon.level = lv;
		roWeapon.currEquipCommanderId = commanderId;
		if (commanderId > 0)
		{
			roWeapon.isEquip = true;
		}
		else
		{
			roWeapon.isEquip = false;
		}
		return roWeapon;
	}

	public void RefreshAddStat()
	{
		addStatPoint.Clear();
		for (int i = 0; i < data.statType.Count; i++)
		{
			if (data.statType[i] != 0)
			{
				addStatPoint.Add(data.statType[i], data.statBasePoint[i] + level * data.statAddPoint[i]);
			}
		}
	}

	public int GetAddStatPoint(EItemStatType type)
	{
		int result = 0;
		if (addStatPoint.ContainsKey(type))
		{
			result = addStatPoint[type];
		}
		return result;
	}

	public int GetTotalAddStatPoint()
	{
		int num = 0;
		foreach (KeyValuePair<EItemStatType, int> item in addStatPoint)
		{
			num += item.Value;
		}
		return num;
	}

	public void EquipWeapon(string commanderId)
	{
		currEquipCommanderId = int.Parse(commanderId);
		isEquip = true;
	}

	public void DeleteWeapon()
	{
		currEquipCommanderId = 0;
		isEquip = false;
	}

	public void SetItemLevel(string id, int curLevel)
	{
	}
}
