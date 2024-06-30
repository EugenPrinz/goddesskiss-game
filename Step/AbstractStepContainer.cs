namespace Step
{
	public abstract class AbstractStepContainer : AbstractStep
	{
		public bool isPriority;

		public bool isLock = true;

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

		public override bool IsLock
		{
			get
			{
				return isLock;
			}
			set
			{
				isLock = value;
			}
		}

		public override bool IsEveryFrameUpdate => true;

		public virtual void Load()
		{
		}
	}
}
