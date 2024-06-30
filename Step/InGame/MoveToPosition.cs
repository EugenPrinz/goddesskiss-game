using UnityEngine;

namespace Step.InGame
{
	public class MoveToPosition : AbstractStepUpdateAction
	{
		public Vector3Data postionData;

		public GameObject target;

		public override bool Enter()
		{
			if (postionData == null || target == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			OnUpdate();
		}

		protected override void OnUpdate()
		{
			target.transform.position = postionData.value;
		}
	}
}
