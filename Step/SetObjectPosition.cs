namespace Step
{
	public class SetObjectPosition : AbstractStepUpdateAction
	{
		public GameObjectData target;

		public Vector3Data vecterValue;

		public GameObjectData objValue;

		public bool isLocalPostion;

		public override bool Enter()
		{
			if (target == null || target.value == null)
			{
				return false;
			}
			if (vecterValue == null && objValue == null)
			{
				return false;
			}
			if (objValue != null && objValue.value == null)
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
			if (vecterValue != null)
			{
				if (isLocalPostion)
				{
					target.value.transform.localPosition = vecterValue.value;
				}
				else
				{
					target.value.transform.position = vecterValue.value;
				}
			}
			else if (objValue != null)
			{
				if (isLocalPostion)
				{
					target.value.transform.localPosition = objValue.value.transform.localPosition;
				}
				else
				{
					target.value.transform.position = objValue.value.transform.position;
				}
			}
		}
	}
}
