using System;

namespace ICode.Actions
{
	[Serializable]
	public class StateAction : ExecutableNode
	{
		private bool isFinished;

		public bool IsFinished => isFinished;

		public void Finish()
		{
			isFinished = true;
		}

		public void Reset()
		{
			isFinished = false;
			base.IsEntered = false;
		}

		public virtual void OnUpdate()
		{
		}
	}
}
