using UnityEngine;

namespace Step
{
	public class RemoveObject : AbstractStepAction
	{
		public GameObjectData objectData;

		public override bool Enter()
		{
			if (objectData == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			if (objectData.value != null)
			{
				Object.DestroyImmediate(objectData.value);
				objectData.value = null;
			}
		}
	}
}
