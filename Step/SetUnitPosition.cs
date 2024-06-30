using UnityEngine;

namespace Step
{
	public class SetUnitPosition : AbstractStepUpdateAction
	{
		public UnitData unitData;

		public GameObject postion;

		public override bool Enter()
		{
			if (unitData == null || unitData.unitRenderer == null)
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
			unitData.unitRenderer.transform.localPosition = postion.transform.localPosition;
		}
	}
}
