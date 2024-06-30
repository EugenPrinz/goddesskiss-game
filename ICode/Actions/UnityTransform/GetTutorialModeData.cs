using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class GetTutorialModeData : StateAction
	{
		[Shared]
		public FsmBool enable;

		[Shared]
		public FsmInt nextStep;

		public override void OnEnter()
		{
			if (RemoteObjectManager.instance.localUser == null)
			{
				Finish();
				return;
			}
			enable.Value = RemoteObjectManager.instance.localUser.tutorialData.enable;
			nextStep.Value = RemoteObjectManager.instance.localUser.tutorialData.nxtStep;
			Finish();
		}
	}
}
