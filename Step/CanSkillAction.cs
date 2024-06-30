using Shared.Battle;

namespace Step
{
	public class CanSkillAction : AbstractStepCondition
	{
		public UnitData retUnitData;

		public IntData retSkillIdxData;

		protected M04_Tutorial main;

		public override bool Enter()
		{
			main = (M04_Tutorial)UIManager.instance.battle.Main;
			if (main == null)
			{
				return false;
			}
			if (retUnitData == null || retSkillIdxData == null)
			{
				return false;
			}
			return base.Enter();
		}

		public override bool Validate()
		{
			int lhsTroopStartIndex = main.Simulator.GetLhsTroopStartIndex(0);
			for (int i = lhsTroopStartIndex; i < 9; i++)
			{
				Unit unit = main.Simulator.frame.units[i];
				if (unit == null)
				{
					continue;
				}
				for (int j = 1; j < unit.skills.Count; j++)
				{
					if (unit.skills[j] != null && unit.skills[j].sp >= unit.skills[j].SkillDataRow.maxSp)
					{
						retUnitData.unitIdx = i;
						retUnitData.unit = unit;
						retUnitData.unitRenderer = main.UnitRenderers[i];
						retSkillIdxData.value = j;
						return true;
					}
				}
			}
			return false;
		}
	}
}
