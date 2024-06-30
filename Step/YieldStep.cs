namespace Step
{
	public class YieldStep : AbstractStepAction
	{
		public override bool IsLock => true;

		public override bool IsEveryFrameUpdate => true;

		protected override void OnUpdate()
		{
			_isFinish = true;
		}
	}
}
