using Shared.Battle;

namespace Step
{
	public class UpdateBattle : AbstractStepUpdateAction
	{
		public IntData curTurn;

		public BoolData isBattleEnd;

		protected M04_Tutorial main;

		protected UIBattleMain ui;

		public override bool Enter()
		{
			main = (M04_Tutorial)UIManager.instance.battle.Main;
			if (main == null)
			{
				return false;
			}
			ui = main.ui;
			return base.Enter();
		}

		protected override void OnUpdate()
		{
			main.ui.SetRemainTime(main.GetRemainedTime());
			switch (main._state)
			{
			case M04_Tutorial.State.Unknown:
			case M04_Tutorial.State.Opening:
			case M04_Tutorial.State.CutIn:
			case M04_Tutorial.State.Result:
				return;
			}
			if (!main.DialogueEndCheck() || main._enteringTroopCount > 0)
			{
				return;
			}
			if (main.Simulator.isEnded)
			{
				isBattleEnd.value = true;
				_isFinish = true;
				return;
			}
			Input input = null;
			Input rhsInput = null;
			if (!main.Simulator.isReplayMode)
			{
				if (main.Simulator.option.waitingInputMode)
				{
					if (main._state == M04_Tutorial.State.InputWait)
					{
						main._TryPickTarget();
					}
				}
				else
				{
					main._TryPickTarget();
				}
				input = main._GetCorrectedInput();
				if (main.Simulator.option.waitingInputMode && main._state == M04_Tutorial.State.InputWait && input == null)
				{
					return;
				}
			}
			main._state = M04_Tutorial.State.Playing;
			float num = main._timeGameObject.TimedDeltaTime();
			main._timeStack += (int)(num * 1000f);
			bool flag = false;
			while (!flag && main._timeStack >= 66)
			{
				main._timeStack -= 66;
				if (main._unitRendererUpdater.delay > 0f)
				{
					main._unitRendererUpdater.delay -= 66f;
				}
				else if (!main._unitRendererUpdater.pause)
				{
					main.Simulator.Step(input, rhsInput);
					main._input = null;
					Frame frame = main.Simulator.frame;
					main.Simulator.AccessFrame(main._unitRendererCreator);
					main.Simulator.AccessFrame(main._skillIconUpdater);
					if ((main._unitRendererCreator.createdLhsTroopIndex >= 0 || main._unitRendererCreator.createdRhsTroopIndex >= 0) && main._TryPlayTroopEntryAnimation(main._unitRendererCreator))
					{
						flag = true;
					}
					main.Simulator.AccessFrame(main._projectileRendererCreator);
					main.Simulator.AccessFrame(main._unitRendererUpdater);
					if (main.Simulator.isEnded)
					{
						flag = true;
					}
					main._UpdateTurnUi(null);
					if (!main.Simulator.isReplayMode && frame.isWaitingLhsInput)
					{
						main._state = M04_Tutorial.State.InputWait;
						flag = true;
					}
					curTurn.value = frame.turn;
				}
			}
		}
	}
}
