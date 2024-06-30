namespace Step.OutGame
{
	public class OnClickCreateNickName : AbstractStepEvent
	{
		public UILabel inputNickName;

		public IntData nextStep;

		protected string requestNickName;

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

		protected override void OnUpdate()
		{
			if (RemoteObjectManager.instance.localUser.tutorialStep == nextStep.value)
			{
				RemoteObjectManager.instance.localUser.nickname = requestNickName;
				UIManager.instance.RefreshOpenedUI();
				_isFinish = true;
			}
		}

		public void OnClick()
		{
			if (_enable && !string.IsNullOrEmpty(inputNickName.text))
			{
				requestNickName = inputNickName.text;
				RemoteObjectManager.instance.RequestSetNickNameFromTutorial(inputNickName.text, nextStep.value);
			}
		}
	}
}
