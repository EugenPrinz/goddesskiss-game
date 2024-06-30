using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class ExitStep : StateAction
	{
		public FsmGameObject tutorial;

		public FsmGameObject step;

		protected ICodeBehaviour _tutorial;

		protected ICodeBehaviour _step;

		public override void OnEnter()
		{
			_tutorial = tutorial.Value.GetComponent<ICodeBehaviour>();
			_step = step.Value.GetComponent<ICodeBehaviour>();
			_tutorial.EnableStateMachine();
			_tutorial.SendEvent("FinishStep", null);
			_step.DisableStateMachine(pause: true);
			Finish();
		}
	}
}
