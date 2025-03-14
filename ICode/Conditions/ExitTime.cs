using System;
using UnityEngine;

namespace ICode.Conditions
{
	[Serializable]
	[Category(Category.Time)]
	[Tooltip("Delay a transition.")]
	public class ExitTime : Condition
	{
		[Tooltip("Time in seconds.")]
		public FsmFloat time;

		[Tooltip("Remember the time, switching state will not reset it.")]
		public FsmBool remember;

		private float exitTime;

		public override void OnEnter()
		{
			base.OnEnter();
			if (!remember.Value || !(exitTime > Time.time))
			{
				exitTime = Time.time + time.Value;
			}
		}

		public override bool Validate()
		{
			return Time.time > exitTime;
		}
	}
}
