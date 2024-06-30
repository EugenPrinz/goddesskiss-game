using System;
using System.Collections.Generic;

namespace Step
{
	public class InitBattleDatas : AbstractStepAction
	{
		[Serializable]
		public class UnitData
		{
			public string commanderId;

			public int unitId;

			public int unitLevel;

			public int unitRank;

			public int unitCls;

			public int unitCostume;

			public int favorRewardStep;

			public int marry;

			public List<int> transcendence;

			public int position;

			public UnitData()
			{
				commanderId = string.Empty;
				unitId = 0;
				unitLevel = 1;
				unitRank = 1;
				unitCls = 1;
				unitCostume = 0;
				favorRewardStep = 0;
				marry = 0;
				transcendence = new List<int>();
				position = -1;
			}
		}

		[Serializable]
		public class TroopData
		{
			public const int RowCount = 3;

			public const int ColCount = 3;

			public const int SlotCount = 9;

			public List<UnitData> units = new List<UnitData>();
		}

		[Serializable]
		public class UserData
		{
			public string nickNameKey;

			public int level;

			public List<TroopData> troops;

			public int TroopCount => troops.Count;

			public UserData()
			{
				troops = new List<TroopData>();
			}

			public void CleanAllTroop()
			{
				troops.Clear();
			}

			public void CreateTroop()
			{
				TroopData troopData = new TroopData();
				for (int i = 0; i < 9; i++)
				{
					troopData.units.Add(new UnitData());
				}
				troops.Add(troopData);
			}

			public TroopData GetTroop(int troopIdx)
			{
				return troops[troopIdx];
			}

			public void RemoveTroop(int troopIdx)
			{
				troops.RemoveAt(troopIdx);
			}
		}

		public UserData attacker;

		public UserData defender;

		protected override void OnEnter()
		{
			BattleData battleData = BattleData.Create(EBattleType.Plunder);
			battleData.attacker = RoUser.Create();
			battleData.attacker.level = 1;
			battleData.attacker.nickname = Localization.Get(attacker.nickNameKey);
			for (int i = 0; i < attacker.troops.Count; i++)
			{
				RoTroop roTroop = RoTroop.Create(null);
				for (int j = 0; j < attacker.troops[i].units.Count; j++)
				{
					UnitData unitData = attacker.troops[i].units[j];
					if (unitData.unitId > 0)
					{
						roTroop.slots[j].unitId = unitData.unitId.ToString();
					}
					roTroop.slots[j].commanderId = unitData.commanderId;
					roTroop.slots[j].unitLevel = unitData.unitLevel;
					roTroop.slots[j].unitRank = unitData.unitRank;
					roTroop.slots[j].unitCls = unitData.unitCls;
					roTroop.slots[j].unitCostume = unitData.unitCostume;
					roTroop.slots[j].favorRewardStep = unitData.favorRewardStep;
					roTroop.slots[j].marry = unitData.marry;
					roTroop.slots[j].transcendence = unitData.transcendence;
					roTroop.slots[j].position = unitData.position;
				}
				battleData.attacker.battleTroopList.Add(roTroop);
			}
			battleData.defender = RoUser.Create();
			battleData.defender.level = 1;
			battleData.defender.nickname = Localization.Get(defender.nickNameKey);
			for (int k = 0; k < defender.troops.Count; k++)
			{
				RoTroop roTroop2 = RoTroop.Create(null);
				for (int l = 0; l < defender.troops[k].units.Count; l++)
				{
					UnitData unitData2 = defender.troops[k].units[l];
					if (unitData2.unitId > 0)
					{
						roTroop2.slots[l].unitId = unitData2.unitId.ToString();
					}
					roTroop2.slots[l].commanderId = unitData2.commanderId;
					roTroop2.slots[l].unitLevel = unitData2.unitLevel;
					roTroop2.slots[l].unitRank = unitData2.unitRank;
					roTroop2.slots[l].unitCls = unitData2.unitCls;
					roTroop2.slots[l].unitCostume = unitData2.unitCostume;
					roTroop2.slots[l].favorRewardStep = unitData2.favorRewardStep;
					roTroop2.slots[l].marry = unitData2.marry;
					roTroop2.slots[l].transcendence = unitData2.transcendence;
					roTroop2.slots[l].position = unitData2.position;
				}
				battleData.defender.battleTroopList.Add(roTroop2);
			}
			battleData.stageId = "1";
			BattleData.Set(battleData);
		}
	}
}
