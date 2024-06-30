using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class GetTutorialStep : StateAction
	{
		[Shared]
		public FsmInt store;

		public bool everyFrame;

		public override void OnEnter()
		{
			if (RemoteObjectManager.instance.localUser == null)
			{
				Finish();
				return;
			}
			store.Value = RemoteObjectManager.instance.localUser.tutorialStep;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			store.Value = RemoteObjectManager.instance.localUser.tutorialStep;
		}
	}
}
