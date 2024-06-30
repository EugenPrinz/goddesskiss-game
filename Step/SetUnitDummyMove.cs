namespace Step
{
	public class SetUnitDummyMove : AbstractStepAction
	{
		public UnitData unitData;

		protected M04_Tutorial main;

		public override bool Enter()
		{
			if (unitData == null || unitData.unitRenderer == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			unitData.unitRenderer.BeginDummyMove();
		}
	}
}
