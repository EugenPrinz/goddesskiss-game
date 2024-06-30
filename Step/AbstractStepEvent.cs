namespace Step
{
	public abstract class AbstractStepEvent : AbstractStep
	{
		public bool isPriority;

		public override bool IsPriority
		{
			get
			{
				return isPriority;
			}
			set
			{
				isPriority = value;
			}
		}

		public override bool IsLock => true;

		public override bool IsEveryFrameUpdate => true;
	}
}
