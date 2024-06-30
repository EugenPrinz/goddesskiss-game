using System;

namespace ICode.Actions.Variable
{
	[Serializable]
	[Category(Category.Variable)]
	[Tooltip("Sets the int value of a variable.")]
	public class SetInt : StateAction
	{
		[Shared]
		[Tooltip("The variable to use.")]
		public FsmInt variable;

		[Tooltip("The value to set.")]
		public FsmInt value;

		[Tooltip("Execute the action every frame.")]
		public bool everyFrame;

		public override void OnEnter()
		{
			variable.Value = value;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			variable.Value = value;
		}
	}
}
