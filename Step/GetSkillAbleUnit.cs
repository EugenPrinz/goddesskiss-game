using Shared.Battle;

namespace Step
{
	public class GetSkillAbleUnit : AbstractStepUpdateAction
	{
		public UnitData ret;

		protected M04_Tutorial main;

		public override bool Enter()
		{
			main = (M04_Tutorial)UIManager.instance.battle.Main;
			if (ret == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			OnUpdate();
		}

		protected override void OnUpdate()
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
						ret.unitIdx = i;
						ret.unit = unit;
						ret.unitRenderer = main.UnitRenderers[i];
					}
				}
			}
		}
	}
}
