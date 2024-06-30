namespace Step
{
	public class GetUiUnitPosition : AbstractStepUpdateAction
	{
		public UnitData unitData;

		public Vector3Data ret;

		public override bool Enter()
		{
			if (unitData == null || ret == null)
			{
				return false;
			}
			if (unitData.unitRenderer == null)
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
			ret.value = unitData.unitRenderer._ssm.ConvertPosCutToUI(unitData.unitRenderer.drawSide, unitData.unitRenderer.transform.position);
		}
	}
}
