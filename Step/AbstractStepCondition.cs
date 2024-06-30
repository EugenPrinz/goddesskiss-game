namespace Step
{
	public abstract class AbstractStepCondition : AbstractStep
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

		public override bool IsEveryFrameUpdate => true;

		public override bool Enter()
		{
			_enable = true;
			if (Validate())
			{
				_isFinish = true;
			}
			OnEnter();
			return true;
		}

		protected override void OnUpdate()
		{
			if (Validate())
			{
				_isFinish = true;
			}
		}

		public virtual bool Validate()
		{
			return false;
		}
	}
}
