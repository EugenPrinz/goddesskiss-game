namespace Step
{
	public class GetTurn : AbstractStepUpdateAction
	{
		public IntData ret;

		protected M04_Tutorial main;

		public override bool Enter()
		{
			main = (M04_Tutorial)UIManager.instance.battle.Main;
			if (main == null)
			{
				return false;
			}
			if (ret == null)
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
			ret.value = main.Simulator.frame.turn;
		}
	}
}
