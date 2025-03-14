using System;
using System.Collections.Generic;
using Shared.Regulation;

namespace Shared.Battle
{
	public class ClearMission
	{
		private Simulator _simulator;

		private List<ClearMissionItem> _missions = new List<ClearMissionItem>();

		private Action<ClearMissionItem>[] _updater;

		private int _clearCount;

		public int clearValue;

		public Action OnChangeClearState;

		public int clearCount
		{
			get
			{
				return _clearCount;
			}
			set
			{
				if (_clearCount != value)
				{
					_clearCount = value;
					if (OnChangeClearState != null)
					{
						OnChangeClearState();
					}
				}
			}
		}

		public int missionCount => _missions.Count;

		public void Init(Simulator simulator)
		{
			_updater = new Action<ClearMissionItem>[22]
			{
				None, Survival, LimitedTurn, Include_Attacker, Include_Defender, Include_Supporter, Include_Commander, Include_Minimum_Count, All_Survival, Less_Attacker,
				Less_Defender, Less_Supporter, Less_UseActiveSkill, Include_Grade_2, Include_Grade_3, Include_Grade_4, Include_Grade_5, Less_Grade_2, Less_Grade_3, Less_Grade_4,
				Less_Grade_5, ClearStage
			};
			_simulator = simulator;
			_clearCount = 0;
			_missions.Clear();
			Shared.Regulation.Regulation regulation = _simulator.regulation;
			if (_simulator.initState.battleType == EBattleType.Plunder)
			{
				WorldMapStageDataRow worldMapStageDataRow = regulation.worldMapStageDtbl[_simulator.initState.stageID];
				Add(new ClearMissionItem(EBattleClearCondition.ClearStage, 1));
				Add(new ClearMissionItem(EBattleClearCondition.LimitedTurn, 2, worldMapStageDataRow.turn1.ToString()));
				Add(new ClearMissionItem(EBattleClearCondition.LimitedTurn, 4, worldMapStageDataRow.turn2.ToString()));
			}
			else if (_simulator.initState.battleType == EBattleType.ScenarioBattle)
			{
				ScenarioBattleDataRow scenarioBattleDataRow = regulation.scenarioBattleDtbl[_simulator.initState.stageID];
				Add(new ClearMissionItem(EBattleClearCondition.ClearStage, 1));
				Add(new ClearMissionItem(EBattleClearCondition.LimitedTurn, 2, scenarioBattleDataRow.turn1.ToString()));
				Add(new ClearMissionItem(EBattleClearCondition.LimitedTurn, 4, scenarioBattleDataRow.turn2.ToString()));
			}
			else if (_simulator.initState.battleType == EBattleType.EventBattle)
			{
				EventBattleFieldDataRow eventBattleFieldDataRow = regulation.eventBattleFieldDtbl[simulator.initState.stageID];
				Add(new ClearMissionItem(EBattleClearCondition.ClearStage, 1));
				Add(new ClearMissionItem(eventBattleFieldDataRow.clearCondition1, 2, eventBattleFieldDataRow.clearCondition1_Value));
				Add(new ClearMissionItem(eventBattleFieldDataRow.clearCondition2, 4, eventBattleFieldDataRow.clearCondition2_Value));
			}
			else if (_simulator.initState.battleType == EBattleType.InfinityBattle)
			{
				InfinityFieldDataRow infinityFieldDataRow = regulation.infinityFieldDtbl[simulator.initState.stageID];
				Add(new ClearMissionItem(EBattleClearCondition.ClearStage, 1));
				Add(new ClearMissionItem(infinityFieldDataRow.clearMission01, 2, infinityFieldDataRow.clearMission01Count));
				Add(new ClearMissionItem(infinityFieldDataRow.clearMission02, 4, infinityFieldDataRow.clearMission02Count));
			}
			Update();
		}

		public void Add(ClearMissionItem item)
		{
			Update(item);
			_missions.Add(item);
		}

		public void Update(ClearMissionItem item)
		{
			if (!item.isFinish && _updater[(int)item.condition] != null)
			{
				_updater[(int)item.condition](item);
			}
		}

		public void MissionAllFail()
		{
			for (int i = 0; i < _missions.Count; i++)
			{
				_missions[i].isSuccess = false;
				_missions[i].isFinish = true;
			}
			clearCount = 0;
			clearValue = 0;
		}

		public void MissionAllSuccess()
		{
			int num = 0;
			for (int i = 0; i < _missions.Count; i++)
			{
				_missions[i].isSuccess = true;
				_missions[i].isFinish = true;
				num += _missions[i].id;
			}
			clearCount = _missions.Count;
			clearValue = num;
		}

		public void Update()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < _missions.Count; i++)
			{
				if (!_missions[i].isFinish)
				{
					Update(_missions[i]);
				}
				if (_missions[i].isSuccess)
				{
					num += _missions[i].id;
					num2++;
				}
			}
			clearCount = num2;
			clearValue = num;
		}

		private void None(ClearMissionItem item)
		{
			item.isSuccess = true;
			item.isFinish = true;
		}

		private void ClearStage(ClearMissionItem item)
		{
			if (_simulator.result != null)
			{
				item.isSuccess = _simulator.result.IsWin;
				item.isFinish = true;
			}
		}

		private void Survival(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			int num2 = _simulator.initState._lhsUnitCount - _simulator.frame._lhsDeadUnitCount;
			item.isSuccess = num2 >= num;
			if (!item.isSuccess)
			{
				item.isFinish = true;
			}
		}

		private void LimitedTurn(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			if (_simulator.initState.battleType == EBattleType.Plunder)
			{
				item.isSuccess = _simulator.frame._waveTurn < num;
				if (!item.isSuccess)
				{
					item.isFinish = true;
					return;
				}
			}
			item.isSuccess = _simulator.frame._waveTurn <= num;
			if (!item.isSuccess)
			{
				item.isFinish = true;
			}
		}

		private void Include_Attacker(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsAttackerCount >= num;
			item.isFinish = true;
		}

		private void Include_Defender(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsDefenderCount >= num;
			item.isFinish = true;
		}

		private void Include_Supporter(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsSupporterCount >= num;
			item.isFinish = true;
		}

		private void Include_Commander(ClearMissionItem item)
		{
			Shared.Regulation.Regulation regulation = _simulator.regulation;
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[item.conditionValue];
			item.isSuccess = false;
			IList<Troop> lhsTroops = _simulator.initState.lhsTroops;
			for (int i = 0; i < lhsTroops.Count; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					int lhsUnitIndex = _simulator.GetLhsUnitIndex(i, j);
					Unit unit = _simulator.frame.units[lhsUnitIndex];
					if (unit != null && unit._cdri >= 0)
					{
						CommanderDataRow commanderDataRow2 = regulation.commanderDtbl[unit._cdri];
						if (commanderDataRow2.resourceId == commanderDataRow.resourceId)
						{
							item.isSuccess = true;
							break;
						}
					}
				}
			}
			item.isFinish = true;
		}

		private void Include_Minimum_Count(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsUnitCount <= num;
			item.isFinish = true;
		}

		private void All_Survival(ClearMissionItem item)
		{
			item.isSuccess = _simulator.frame._lhsDeadUnitCount <= 0;
			if (!item.isSuccess)
			{
				item.isFinish = true;
			}
		}

		private void Less_Attacker(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsAttackerCount <= num;
			item.isFinish = true;
		}

		private void Less_Defender(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsDefenderCount <= num;
			item.isFinish = true;
		}

		private void Less_Supporter(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsSupporterCount <= num;
			item.isFinish = true;
		}

		private void Less_UseActiveSkill(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.frame._lhsActiveSkillUseCount <= num;
			if (!item.isSuccess)
			{
				item.isFinish = true;
			}
		}

		private void Include_Grade_2(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsInitGrade2Count >= num;
			item.isFinish = true;
		}

		private void Include_Grade_3(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsInitGrade3Count >= num;
			item.isFinish = true;
		}

		private void Include_Grade_4(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsInitGrade4Count >= num;
			item.isFinish = true;
		}

		private void Include_Grade_5(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsInitGrade5Count >= num;
			item.isFinish = true;
		}

		private void Less_Grade_2(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsInitGrade2Count > 0 && _simulator.initState._lhsInitGrade2Count <= num;
			item.isFinish = true;
		}

		private void Less_Grade_3(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsInitGrade3Count > 0 && _simulator.initState._lhsInitGrade3Count <= num;
			item.isFinish = true;
		}

		private void Less_Grade_4(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsInitGrade4Count > 0 && _simulator.initState._lhsInitGrade4Count <= num;
			item.isFinish = true;
		}

		private void Less_Grade_5(ClearMissionItem item)
		{
			int num = int.Parse(item.conditionValue);
			item.isSuccess = _simulator.initState._lhsInitGrade5Count > 0 && _simulator.initState._lhsInitGrade5Count <= num;
			item.isFinish = true;
		}
	}
}
