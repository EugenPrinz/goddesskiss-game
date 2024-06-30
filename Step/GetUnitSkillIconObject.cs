using UnityEngine;

namespace Step
{
	public class GetUnitSkillIconObject : AbstractStepAction
	{
		public UnitData unitData;

		public IntData skillIdxData;

		public GameObjectData ret;

		protected GameObject icon;

		public override bool Enter()
		{
			if (unitData == null || skillIdxData == null || ret == null)
			{
				return false;
			}
			return false;
		}

		protected override void OnEnter()
		{
			OnUpdate();
		}

		protected override void OnUpdate()
		{
			ret.value = icon;
		}
	}
}
