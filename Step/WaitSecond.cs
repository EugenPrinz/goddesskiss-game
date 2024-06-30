using System.Collections;
using UnityEngine;

namespace Step
{
	public class WaitSecond : AbstractStepAction
	{
		public float waitSecond;

		public override bool IsLock => true;

		public override bool IsEveryFrameUpdate => true;

		protected override void OnEnter()
		{
			if (waitSecond <= 0f)
			{
				_isFinish = true;
			}
			else
			{
				StartCoroutine("Waiting");
			}
		}

		private IEnumerator Waiting()
		{
			yield return new WaitForSeconds(waitSecond);
			_isFinish = true;
		}
	}
}
