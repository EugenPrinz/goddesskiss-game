namespace Step
{
	public abstract class AbstractStepUpdateAction : AbstractStep
	{
		public bool isPriority;

		public bool isLock = true;

		public bool everyUpdateFrame = true;

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

		public override bool IsEveryFrameUpdate
		{
			get
			{
				return everyUpdateFrame;
			}
			set
			{
				everyUpdateFrame = value;
			}
		}
	}
}
