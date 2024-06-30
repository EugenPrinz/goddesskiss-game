using Step;

namespace TutorialStep
{
	public class GetStep : AbstractStepUpdateAction
	{
		public IntData ret;

		public override bool Enter()
		{
			if (ret == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			OnUpdate();
		}

		protected override void OnUpdate()
		{
			ret.value = RemoteObjectManager.instance.localUser.tutorialStep;
		}
	}
}
