using System;

namespace ICode.Actions
{
	[Serializable]
	[Category(Category.StateMachine)]
	[Tooltip("Sets a node as active.")]
	public class SetNode : StateAction
	{
		[Tooltip("The name of the state to set.")]
		public FsmString nodeName;

		public override void OnEnter()
		{
			base.Root.Owner.SetNode(nodeName.Value);
			Finish();
		}
	}
}
