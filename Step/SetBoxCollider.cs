using UnityEngine;

namespace Step
{
	public class SetBoxCollider : AbstractStepAction
	{
		public GameObjectData target;

		public Vector3 center;

		public Vector3 size;

		public override bool Enter()
		{
			if (target == null || target.value == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			BoxCollider boxCollider = target.value.GetComponent<BoxCollider>();
			if (boxCollider == null)
			{
				boxCollider = target.value.AddComponent<BoxCollider>();
			}
			boxCollider.center = center;
			boxCollider.size = size;
		}
	}
}
