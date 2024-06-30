namespace Step
{
	public class SetPanelDepth : AbstractStepAction
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
			UIPanel component = target.value.GetComponent<UIPanel>();
			if (component == null)
			{
				return false;
			}
			component.depth = depth.value;
			return base.Enter();
		}
	}
}
