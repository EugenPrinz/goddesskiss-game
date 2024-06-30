using UnityEngine;

namespace Step
{
	public class GetUnitAnchorObject : AbstractStepAction
	{
		public E_SIDE side;

		public int unitPosition;

		public GameObjectData ret;

		protected M04_Tutorial main;

		protected Transform unitAnchor;

		public override bool Enter()
		{
			main = (M04_Tutorial)UIManager.instance.battle.Main;
			if (ret == null)
			{
				return false;
			}
			if (side == E_SIDE.LEFT)
			{
				unitAnchor = main.lhsTroopAnchor.GetChild(unitPosition);
			}
			else
			{
				unitAnchor = main.rhsTroopAnchor.GetChild(unitPosition);
			}
			if (unitAnchor == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			ret.value = unitAnchor.gameObject;
		}
	}
}
