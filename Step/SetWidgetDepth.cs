namespace Step
{
	public class SetWidgetDepth : AbstractStepAction
	{
		public GameObjectData target;

		public IntData depth;

		public override bool Enter()
		{
			if (target == null || target.value == null)
			{
				return false;
			}
			if (depth == null)
			{
				return false;
			}
			UIWidget component = target.value.GetComponent<UIWidget>();
			if (component == null)
			{
				return false;
			}
			component.depth = depth.value;
			return base.Enter();
		}
	}
}
