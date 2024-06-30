namespace Step
{
	public class GetUnitObject : AbstractStepUpdateAction
	{
		public UnitData unitData;

		public GameObjectData ret;

		public override bool Enter()
		{
			if (ret == null || unitData.unitRenderer == null)
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
			ret.value = unitData.unitRenderer.gameObject;
		}
	}
}
