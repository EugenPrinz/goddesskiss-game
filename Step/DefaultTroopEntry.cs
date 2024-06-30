using System.Collections;

namespace Step
{
	public class DefaultTroopEntry : AbstractStepAction
	{
		protected M04_Tutorial main;

		protected bool lhsEntry;

		protected bool rhsEntry;

		protected float playTime = 5f;

		public override bool IsLock => true;

		public override bool IsEveryFrameUpdate => true;

		public override bool Enter()
		{
			main = (M04_Tutorial)UIManager.instance.battle.Main;
			if (main == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			lhsEntry = false;
			rhsEntry = false;
			StartCoroutine("StartLhsEntry");
			StartCoroutine("StartRhsEntry");
		}

		protected override void OnUpdate()
		{
			if (lhsEntry && rhsEntry)
			{
				_isFinish = true;
			}
		}

		private IEnumerator StartLhsEntry()
		{
			yield return StartCoroutine(main._PlayTroopEntryAnimation(main.lhsTroopAnchor, 10f, 0f, playTime));
			lhsEntry = true;
		}

		private IEnumerator StartRhsEntry()
		{
			yield return StartCoroutine(main._PlayTroopEntryAnimation(main.rhsTroopAnchor, 10f, 0f, playTime));
			rhsEntry = true;
		}
	}
}
