using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("1")]
	[HelpUrl("2")]
	public class SaveStep : StateAction
	{
		public FsmInt step;

		public override void OnEnter()
		{
			RemoteObjectManager.instance.RequestUpdateTutorialStep(step.Value);
			Finish();
		}
	}
}
