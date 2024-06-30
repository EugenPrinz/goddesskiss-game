using UnityEngine;

namespace Step
{
	public class GetChildObjectFromIndex : AbstractStepAction
	{
		public GameObjectData target;

		public int index;

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
			if (index >= target.value.transform.childCount)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			Transform child = target.value.transform.GetChild(index);
			ret.value = child.gameObject;
		}
	}
}
