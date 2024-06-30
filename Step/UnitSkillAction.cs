namespace Step
{
	public class UnitSkillAction : AbstractStepAction
	{
		public UnitData unitData;

		public IntData skillIdxData;

		public override bool Enter()
		{
			if (unitData == null || unitData.unit == null)
			{
				return false;
			}
			if (skillIdxData == null)
			{
				return false;
			}
			if (unitData.unit.skills[skillIdxData.value] == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			M04_Tutorial m04_Tutorial = (M04_Tutorial)UIManager.instance.battle.Main;
			m04_Tutorial.OnSelect(unitData.unitIdx, skillIdxData.value);
		}
	}
}
