using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class EnableTutorialMode : StateAction
	{
		public FsmBool enable;

		public FsmInt nextStep;

		public override void OnEnter()
		{
			if (RemoteObjectManager.instance.localUser == null)
			{
				Finish();
				return;
			}
			RemoteObjectManager.instance.localUser.tutorialData.enable = enable.Value;
			RemoteObjectManager.instance.localUser.tutorialData.nxtStep = nextStep.Value;
			Finish();
		}
	}
}
