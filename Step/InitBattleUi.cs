using System.Collections;

namespace Step
{
	public class InitBattleUi : AbstractStepAction
	{
		protected M04_Tutorial main;

		public override bool IsLock => true;

		public override bool IsEveryFrameUpdate => true;

		protected override void OnEnter()
		{
			StartCoroutine("Init");
		}

		private IEnumerator Init()
		{
			main = (M04_Tutorial)UIManager.instance.battle.Main;
			main.BattleData = BattleData.Get();
			if (main.BattleData != null)
			{
				yield return StartCoroutine(main.InitUI(main.BattleData));
				_isFinish = true;
			}
		}
	}
}
