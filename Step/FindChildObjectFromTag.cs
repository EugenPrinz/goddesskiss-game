using UnityEngine;

namespace Step
{
	public class FindChildObjectFromTag : AbstractStepAction
	{
		public GameObjectData target;

		public new string tag;

		public GameObjectData ret;

		public override bool Enter()
		{
			if (target == null || target.value == null)
			{
				return false;
			}
			if (ret == null)
			{
				return false;
			}
			Transform transform = target.value.transform.Find(tag);
			if (transform == null)
			{
				return false;
			}
			ret.value = transform.gameObject;
			return base.Enter();
		}
	}
}
