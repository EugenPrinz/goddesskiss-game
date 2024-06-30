using Shared.Battle;

namespace Step
{
	public class GetActivedSkillIdx : AbstractStepUpdateAction
	{
		public UnitData unitData;

		public IntData ret;

		protected M04_Tutorial main;

		public override bool Enter()
		{
			main = (M04_Tutorial)UIManager.instance.battle.Main;
			if (unitData == null || ret == null)
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
			Unit unit = unitData.unit;
			for (int i = 1; i < unit.skills.Count; i++)
			{
				if (unit.skills[i] != null && unit.skills[i].sp >= unit.skills[i].SkillDataRow.maxSp)
				{
					ret.value = i;
					break;
				}
			}
		}
	}
}
