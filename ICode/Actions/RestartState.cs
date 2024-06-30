using System;

namespace ICode.Actions
{
	[Serializable]
	[Category(Category.StateMachine)]
	[Tooltip("Restarts the active state.")]
	public class RestartState : StateAction
	{
		public override void OnEnter()
		{
			if (base.Root.Owner.ActiveNode is State)
			{
				(base.Root.Owner.ActiveNode as State).Restart();
			}
			Finish();
		}
	}
}
