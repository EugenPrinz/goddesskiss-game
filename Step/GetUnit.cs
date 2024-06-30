namespace Step
{
	public class GetUnit : AbstractStepAction
	{
		public E_SIDE side;

		public int unitPosition;

		public UnitData ret;

		protected override void OnEnter()
		{
			M04_Tutorial m04_Tutorial = (M04_Tutorial)UIManager.instance.battle.Main;
			int num = -1;
			num = ((side != 0) ? m04_Tutorial.Simulator.GetRhsUnitIndex(0, unitPosition) : m04_Tutorial.Simulator.GetLhsUnitIndex(0, unitPosition));
			ret.position = unitPosition;
			ret.unit = m04_Tutorial.Simulator.frame.units[num];
			ret.unitIdx = num;
			ret.unitRenderer = m04_Tutorial.UnitRenderers[num];
		}
	}
}
