using System;
using UnityEngine;

namespace ICode.Actions
{
	[Serializable]
	[Category(Category.Time)]
	[Tooltip("")]
	[HelpUrl("")]
	public class Wait : StateAction
	{
		[Tooltip("Time in seconds.")]
		public FsmFloat time;

		private float waitTime;

		public override void OnEnter()
		{
			waitTime = Time.time + time.Value;
		}

		public override void OnUpdate()
		{
			if (Time.time > waitTime)
			{
				Finish();
			}
		}
	}
}
