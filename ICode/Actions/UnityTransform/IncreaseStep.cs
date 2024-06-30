using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class IncreaseStep : StateAction
	{
		public FsmInt step;

		public FsmInt value;

		public override void OnEnter()
		{
			step.Value += value.Value;
			Finish();
		}
	}
}
