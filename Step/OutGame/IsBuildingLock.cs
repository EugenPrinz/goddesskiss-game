namespace Step.OutGame
{
	public class IsBuildingLock : AbstractStepCondition
	{
		public GameObjectData targetBuilding;

		public bool equals;

		protected UIBuilding building;

		public override bool Enter()
		{
			if (targetBuilding == null || targetBuilding.value == null)
			{
				return false;
			}
			building = targetBuilding.value.GetComponent<UIBuilding>();
			if (building == null)
			{
				return false;
			}
			return base.Enter();
		}

		public override bool Validate()
		{
			return building.Lock.activeSelf == equals;
		}
	}
}
