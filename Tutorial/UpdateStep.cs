using Step;

namespace Tutorial
{
	public class UpdateStep : AbstractStepAction
	{
		public IntData nextStep;

		public override bool IsLock => true;

		public override bool IsEveryFrameUpdate => true;

		public override bool Enter()
		{
			if (nextStep == null)
			{
				return false;
			}
			if (nextStep.value <= RemoteObjectManager.instance.localUser.tutorialStep)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			RemoteObjectManager.instance.RequestUpdateTutorialStep(nextStep.value);
		}

		protected override void OnUpdate()
		{
			if (RemoteObjectManager.instance.localUser.tutorialStep == nextStep.value)
			{
				_isFinish = true;
			}
		}
	}
}
