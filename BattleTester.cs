using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleTester : Manager<BattleTester>
{
	public enum E_SIDE
	{
		ATTACKER,
		DEFENDER
	}

	[Serializable]
	public class UnitData
	{
		public int unitId;

		public int unitLevel;

		public int unitRank;

		public int position;

		public UnitData()
		{
			unitId = 0;
			unitLevel = 1;
			unitRank = 1;
			position = -1;
		}
	}

	[Serializable]
	public class TroopData
	{
		public int commandId;

		public int commandLevel;

		public int commandRank;

		public int commandPosition;

		public const int RowCount = 3;

		public const int ColCount = 3;

		public const int SlotCount = 9;

		public List<UnitData> units = new List<UnitData>();

		public TroopData()
		{
			commandId = 0;
			commandLevel = 1;
			commandRank = 1;
			commandPosition = -1;
		}
	}

	[Serializable]
	public class UserData
	{
		public string nickName;

		public int level;

		public bool isShowUnitList;

		public List<bool> showTroop;

		public List<TroopData> troops;

		protected E_SIDE side;

		public int TroopCount => troops.Count;

		public UserData(E_SIDE side)
		{
			this.side = side;
			isShowUnitList = false;
			showTroop = new List<bool>();
			troops = new List<TroopData>();
		}

		public void CleanAllTroop()
		{
			troops.Clear();
			showTroop.Clear();
		}

		public void CreateTroop()
		{
			TroopData troopData = new TroopData();
			for (int i = 0; i < 9; i++)
			{
				troopData.units.Add(new UnitData());
			}
			troops.Add(troopData);
			showTroop.Add(item: false);
		}

		public TroopData GetTroop(int troopIdx)
		{
			return troops[troopIdx];
		}

		public void RemoveTroop(int troopIdx)
		{
			troops.RemoveAt(troopIdx);
			showTroop.RemoveAt(troopIdx);
		}

		public void MoveUpTroop(int troopIdx)
		{
			if (troopIdx > 0)
			{
				TroopData value = troops[troopIdx - 1];
				troops[troopIdx - 1] = troops[troopIdx];
				troops[troopIdx] = value;
				bool value2 = showTroop[troopIdx - 1];
				showTroop[troopIdx - 1] = showTroop[troopIdx];
				showTroop[troopIdx] = value2;
			}
		}

		public void MoveDownTroop(int troopIdx)
		{
			if (troopIdx < troops.Count - 1)
			{
				TroopData value = troops[troopIdx + 1];
				troops[troopIdx + 1] = troops[troopIdx];
				troops[troopIdx] = value;
				bool value2 = showTroop[troopIdx + 1];
				showTroop[troopIdx + 1] = showTroop[troopIdx];
				showTroop[troopIdx] = value2;
			}
		}
	}

	[HideInInspector]
	public UserData attacker = new UserData(E_SIDE.ATTACKER);

	[HideInInspector]
	public UserData defender = new UserData(E_SIDE.DEFENDER);

	[HideInInspector]
	public bool isShowBattleItem;

	public UserData GetUser(E_SIDE side)
	{
		if (side == E_SIDE.ATTACKER)
		{
			return attacker;
		}
		return defender;
	}

	protected override void Init()
	{
		base.Init();
		BattleData battleData = BattleData.Create(EBattleType.Plunder);
		battleData.attacker = RoUser.Create();
		battleData.attacker.level = 1;
		battleData.attacker.nickname = "Attacker";
		for (int i = 0; i < attacker.troops.Count; i++)
		{
			RoTroop roTroop = RoTroop.Create(null);
			if (attacker.troops[i].commandId > 0)
			{
			}
			for (int j = 0; j < attacker.troops[i].units.Count; j++)
			{
				UnitData unitData = attacker.troops[i].units[j];
				if (unitData.unitId > 0)
				{
					roTroop.slots[j].unitId = unitData.unitId.ToString();
				}
				roTroop.slots[j].unitLevel = unitData.unitLevel;
				roTroop.slots[j].unitRank = unitData.unitRank;
				roTroop.slots[j].position = unitData.position;
			}
			battleData.attacker.battleTroopList.Add(roTroop);
		}
		battleData.defender = RoUser.Create();
		battleData.defender.level = 1;
		battleData.defender.nickname = "Attacker";
		for (int k = 0; k < defender.troops.Count; k++)
		{
			RoTroop roTroop2 = RoTroop.Create(null);
			if (defender.troops[k].commandId > 0)
			{
			}
			for (int l = 0; l < defender.troops[k].units.Count; l++)
			{
				UnitData unitData2 = defender.troops[k].units[l];
				if (unitData2.unitId > 0)
				{
					roTroop2.slots[l].unitId = unitData2.unitId.ToString();
				}
				roTroop2.slots[l].unitLevel = unitData2.unitLevel;
				roTroop2.slots[l].unitRank = unitData2.unitRank;
				roTroop2.slots[l].position = unitData2.position;
			}
			battleData.defender.battleTroopList.Add(roTroop2);
		}
		battleData.stageId = "1";
		battleData.worldId = "1";
		BattleData.Set(battleData);
	}
}
