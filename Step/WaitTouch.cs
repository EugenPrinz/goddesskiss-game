using UnityEngine;

namespace Step
{
	public class WaitTouch : AbstractStepAction
	{
		public override bool IsLock => true;

		public override bool IsEveryFrameUpdate => true;

		protected override void OnUpdate()
		{
			if (Input.GetMouseButtonDown(0))
			{
				_isFinish = true;
			}
		}
	}
}
