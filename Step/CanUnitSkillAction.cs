using Shared.Battle;

namespace Step
{
	public class CanUnitSkillAction : AbstractStepCondition
	{
		public UnitData unitData;

		protected M04_Tutorial main;

		protected Unit unit;

		public override bool Enter()
		{
			main = (M04_Tutorial)UIManager.instance.battle.Main;
			if (main == null)
			{
				return false;
			}
			if (unitData == null || unitData.unit == null)
			{
				return false;
			}
			unit = unitData.unit;
			return base.Enter();
		}

		public override bool Validate()
		{
			for (int i = 1; i < unit.skills.Count; i++)
			{
				if (unit.skills[i] != null && unit.skills[i].sp >= unit.skills[i].SkillDataRow.maxSp)
				{
					return true;
				}
			}
			return false;
		}
	}
}
