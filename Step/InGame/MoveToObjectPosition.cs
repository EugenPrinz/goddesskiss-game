using UnityEngine;

namespace Step.InGame
{
	public class MoveToObjectPosition : AbstractStepUpdateAction
	{
		public GameObjectData objData;

		public GameObject target;

		public override bool Enter()
		{
			if (objData == null || target == null)
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
			target.transform.position = objData.value.transform.position;
		}
	}
}
