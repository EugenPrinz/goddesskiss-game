using System;

namespace ICode.Conditions
{
	[Serializable]
	[Category(Category.StateMachine)]
	[Tooltip("Called when a state is finished, usefull for Sequence states.")]
	public class IsFinished : Condition
	{
		private bool isTrigger;

		public override void OnEnter()
		{
			base.Root.Owner.onReceiveEvent += OnTrigger;
		}

		public override void OnExit()
		{
			base.Root.Owner.onReceiveEvent -= OnTrigger;
			isTrigger = false;
		}

		private void OnTrigger(string eventName, object parameter)
		{
			if (eventName.Equals("FINISHED"))
			{
				isTrigger = true;
			}
		}

		public override bool Validate()
		{
			if (isTrigger)
			{
				isTrigger = false;
				return true;
			}
			return isTrigger;
		}
	}
}
