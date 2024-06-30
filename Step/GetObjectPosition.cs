namespace Step
{
	public class GetObjectPosition : AbstractStepUpdateAction
	{
		public GameObjectData target;

		public Vector3Data ret;

		public bool isLocalPostion;

		public override bool Enter()
		{
			if (target == null || target.value == null)
			{
				return false;
			}
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
			if (isLocalPostion)
			{
				ret.value = target.value.transform.localPosition;
			}
			else
			{
				ret.value = target.value.transform.position;
			}
		}
	}
}
