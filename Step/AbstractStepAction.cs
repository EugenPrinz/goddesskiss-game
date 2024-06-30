namespace Step
{
	public abstract class AbstractStepAction : AbstractStep
	{
		public override bool IsLock
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public override bool IsEveryFrameUpdate => false;
	}
}
