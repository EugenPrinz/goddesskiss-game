using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class StartStep : StateAction
	{
		public FsmGameObject stepBehaviour;

		public override void OnEnter()
		{
			ICodeBehaviour component = stepBehaviour.Value.GetComponent<ICodeBehaviour>();
			component.EnableStateMachine();
			component.SendEvent("Start", null);
			Finish();
		}
	}
}
