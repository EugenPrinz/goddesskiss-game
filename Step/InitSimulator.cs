using System.Collections;
using Shared.Regulation;
using UnityEngine;

namespace Step
{
	public class InitSimulator : AbstractStepAction
	{
		protected M04_Tutorial main;

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
			Regulation regulation = RemoteObjectManager.instance.regulation;
			main.Simulator = main._CreateSimulator(main.BattleData, regulation);
			main.Simulator.record._option.enableEffect = GameSetting.instance.effect;
			main.UnitRenderers = new UnitRenderer[main.Simulator.unitCount];
			main._unitRendererCreator = M04_Tutorial._UnitRendererCreator.Create(main);
			main._unitRendererCreator.onClick = main.OnSelect;
			main._projectileRendererCreator = M04_Tutorial._ProjectileRendererCreator.Create(main);
			main._unitRendererUpdater = M04_Tutorial._UnitRendererUpdater.Create(main);
			main._skillIconUpdater = M04_Tutorial._SkillIconUpdater.Create(main);
			main._skillIconUpdater._unitRenderers = main.UnitRenderers;
			main._turnUiUpdater = M04_Tutorial._TurnUiUpdater.Create(main);
			main._turnUiUpdater.renderers = main.UnitRenderers;
			main._turnUiUpdater.input = null;
			main._inputCorrector = M04_Tutorial._InputCorrector.Create(main);
			main._inputCorrector.input = null;
			main._inputCorrector.isAuto = false;
			Manager<UIBattleUnitController>.GetInstance().Clean();
			main.Simulator.AccessFrame(main._unitRendererCreator);
			main.Simulator.AccessFrame(main._unitRendererUpdater);
			string theme = "land_needleleaf";
			string theme2 = "land_needleleaf";
			if (main.BattleData != null)
			{
				Regulation regulation2 = RemoteObjectManager.instance.regulation;
				switch (main.BattleData.type)
				{
				case EBattleType.Plunder:
				{
					WorldMapStageDataRow worldMapStageDataRow = regulation2.worldMapStageDtbl[main.BattleData.stageId];
					theme = worldMapStageDataRow.battlemap;
					theme2 = worldMapStageDataRow.enemymap;
					break;
				}
				case EBattleType.GuildScramble:
				{
					GuildStruggleDataRow guildStruggleDataRow = regulation2.guildStruggleDtbl[main.BattleData.stageId];
					theme = guildStruggleDataRow.battlemap;
					theme2 = guildStruggleDataRow.enemymap;
					break;
				}
				case EBattleType.Guerrilla:
				{
					SweepDataRow sweepDataRow = regulation2.sweepDtbl[$"{main.BattleData.sweepType}_{main.BattleData.sweepLevel}"];
					theme = sweepDataRow.battlemap;
					theme2 = sweepDataRow.enemymap;
					break;
				}
				case EBattleType.Raid:
				{
					RaidChallengeDataRow raidChallengeDataRow = regulation2.raidChallengeDtbl[main.BattleData.raidData.raidId.ToString()];
					theme = raidChallengeDataRow.battlemap;
					theme2 = raidChallengeDataRow.enemymap;
					break;
				}
				}
				main._SetLeftTerrainTheme(theme);
				main._SetRightTerrainTheme(theme2);
			}
			else
			{
				main._SetLeftTerrainTheme(theme);
				main._SetRightTerrainTheme(theme2);
			}
			main._SetTerrainScrollSpeed(ConstValue.battleTerrainScrollSpeed);
			main.ui.onClick = main._OnClickMain;
			main.ui.SetRemainTime(main.GetRemainedTime());
			main.ui.SetAutoEnable(main.Simulator.ableAuto);
			main.ui.main.uiWaveView.SetEnable(enable: false);
			main.ui.SetPauseEnable(enable: false);
			Time.timeScale = main.defaultTimeScale;
			main.ui.UICommanderEnable(EBattleSide.Right);
			main.ui.SetRhsInitWave(main.Simulator.rhsTroops.Count);
			main.ui.InitTurn(5, 10);
			main.ui.main.uiSpeedView.SetEnable(enable: false);
			StartCoroutine("Init");
		}

		private IEnumerator Init()
		{
			yield return null;
			yield return null;
			yield return null;
			yield return null;
			UIFade.In(0.8f);
			float playTime = 5f;
			float speed = ConstValue.battleTerrainScrollSpeed;
			main.StartCoroutine(main._ScrollSpeedTo(0.25f * speed, speed, playTime));
			main.ui.UICommanderEnable(EBattleSide.Right);
			while (main._enteringTroopCount > 0)
			{
				yield return null;
			}
			yield return StartCoroutine(main._PlayCommanderBoardAnimation());
			yield return StartCoroutine(main._InitGameBattleItem());
			main.ui.StartBattle();
			main._state = M04_Tutorial.State.Playing;
			_isFinish = true;
			yield return null;
		}
	}
}
