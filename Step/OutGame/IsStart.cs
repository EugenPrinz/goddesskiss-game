namespace Step.OutGame
{
	public class IsStart : AbstractStepCondition
	{
		public override bool Validate()
		{
			return UIManager.instance.world.onStart;
		}
	}
}
