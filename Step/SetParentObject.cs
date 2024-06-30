namespace Step
{
	public class SetParentObject : AbstractStepAction
	{
		public GameObjectData data;

		public GameObjectData parent;

		public override bool Enter()
		{
			if (data == null || data.value == null || parent == null || parent.value == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			data.value.transform.parent = parent.value.transform;
		}
	}
}
