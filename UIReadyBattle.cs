using System;
using System.Collections;
using System.Collections.Generic;
using DialoguerCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

public class UIReadyBattle : UIPopup
{
	[Serializable]
	public class Duel : UIInnerPartBase
	{
		public UIUser rightUser;

		public UICommander rightCommander;

		public GameObject UseResource;

		public GameObject NotUseResource;

		public GameObject attackBtn;

		public UISprite ticket;

		public UIUser user;

		public GameObject progressing;

		public void Set()
		{
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			UISetter.SetButtonGray(attackBtn, !readyBattle.IsEmptyTroop());
			UISetter.SetActive(progressing, active: false);
			UISetter.SetActive(UseResource, readyBattle.battleData.type == EBattleType.Duel || readyBattle.battleData.type == EBattleType.WaveDuel || readyBattle.battleData.type == EBattleType.WorldDuel);
			string spriteName = "Goods-chlg";
			if (readyBattle.battleData.type == EBattleType.WaveDuel)
			{
				spriteName = "Goods-wbt";
			}
			else if (readyBattle.battleData.type == EBattleType.Duel)
			{
				spriteName = "Goods-chlg";
			}
			else if (readyBattle.battleData.type == EBattleType.WorldDuel)
			{
				spriteName = "Goods-sd";
			}
			UISetter.SetSprite(ticket, spriteName);
			UISetter.SetActive(NotUseResource, readyBattle.battleData.type == EBattleType.Annihilation || readyBattle.battleData.type == EBattleType.InfinityBattle);
		}

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			UIReadyBattle uIReadyBattle = parent as UIReadyBattle;
			if (uIReadyBattle.battleData.type == EBattleType.Duel || uIReadyBattle.battleData.type == EBattleType.WaveDuel || uIReadyBattle.battleData.type == EBattleType.Annihilation || uIReadyBattle.battleData.type == EBattleType.WorldDuel || uIReadyBattle.battleData.type == EBattleType.InfinityBattle)
			{
				uIReadyBattle.StartCoroutine(uIReadyBattle.SelectDuelTroopDelay());
				Set();
			}
		}

		public override void OnRefresh()
		{
			base.OnRefresh();
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			if (readyBattle.battleData.type == EBattleType.Duel || readyBattle.battleData.type == EBattleType.WaveDuel || readyBattle.battleData.type == EBattleType.Annihilation || readyBattle.battleData.type == EBattleType.WorldDuel || readyBattle.battleData.type == EBattleType.InfinityBattle)
			{
				Set();
			}
		}

		public void SetProgressingBattle(Protocols.ScrambleStageInfo.UserInfo userData)
		{
			UISetter.SetButtonEnable(attackBtn, enable: false);
			UISetter.SetActive(progressing, active: true);
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			UIReadyBattle uIReadyBattle = parent as UIReadyBattle;
			string name = sender.name;
			if (!(name == "Attack"))
			{
				return;
			}
			if (attackBtn.GetComponent<UIButton>().isGray)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				return;
			}
			base.localUser.isUpdateAtkRecord = true;
			if (uIReadyBattle.battleData.type == EBattleType.Duel)
			{
				uIReadyBattle.StartDuel();
			}
			else if (uIReadyBattle.battleData.type == EBattleType.Annihilation)
			{
				uIReadyBattle.StartAnnihilation();
			}
			else if (uIReadyBattle.battleData.type == EBattleType.WaveDuel)
			{
				if (base.localUser.currentSeasonDuelTime.GetRemain() <= 0.0)
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5050013"));
				}
				else
				{
					uIReadyBattle.StartWaveDuel();
				}
			}
			else if (uIReadyBattle.battleData.type == EBattleType.WorldDuel)
			{
				if (base.localUser.currentSeasonDuelTime.GetRemain() <= 0.0)
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5050013"));
				}
				else
				{
					uIReadyBattle.StartWorldDuel();
				}
			}
			else if (uIReadyBattle.battleData.type == EBattleType.InfinityBattle)
			{
				uIReadyBattle.CheckInfinityBattle();
			}
			else
			{
				uIReadyBattle.StartSweep();
			}
		}
	}

	[Serializable]
	public class Plunder : UIInnerPartBase
	{
		public UITroop stageInfo;

		public UILabel skipBattleCost;

		public UIDefaultListView commanderListView;

		public GameObject RemainCountRoot;

		public GameObject readyRoot;

		public GameObject startRoot;

		public GameObject readyPlunderRoot;

		public GameObject readySweepRoot;

		public GameObject AttackBtn;

		public GameObject timeMachineRoot;

		public UILabel timeMachine;

		public UILabel title;

		public UILabel description;

		public UISprite icon1;

		public UISprite icon2;

		public UILabel bulletTopCnt;

		public UILabel bulletStartCnt;

		public GameObject jobRoot;

		public UISprite jobIcon;

		public UILabel jobLabel;

		public UILabel banLabel;

		public UILabel clearCnt;

		public GameObject rechargeBtn;

		public GameObject skipBtn;

		public GameObject skipsBtn;

		public UILabel skipCnt;

		public UILabel skipsCnt;

		public GameObject starGrid;

		public GameObject starGridRoot;

		public UICommander bossCommander;

		public UIDefaultListView clearConditionList;

		public UIFlipSwitch flipRepeat;

		public void Set()
		{
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			UISetter.SetLabel(skipBattleCost, 100);
			BattleData battleData = readyBattle.battleData;
			UISetter.SetButtonGray(AttackBtn, !readyBattle._curLhsTroop.IsEmpty());
			UISetter.SetActive(readyPlunderRoot, battleData.type == EBattleType.Plunder);
			UISetter.SetActive(readySweepRoot, battleData.type == EBattleType.Guerrilla);
			List<EnemyCommanderDataRow> list = new List<EnemyCommanderDataRow>();
			UISetter.SetActive(starGridRoot, battleData.type == EBattleType.Plunder || battleData.type == EBattleType.EventBattle);
			UISetter.SetActive(clearConditionList, battleData.type == EBattleType.EventBattle);
			UISetter.SetActive(timeMachineRoot, battleData.type == EBattleType.Guerrilla || battleData.type == EBattleType.Plunder);
			UISetter.SetActive(flipRepeat, battleData.type == EBattleType.EventBattle);
			description.height = ((battleData.type != EBattleType.EventBattle) ? 150 : 75);
			if (battleData.type == EBattleType.Plunder)
			{
				RoWorldMap.Stage stage = base.localUser.FindWorldMapStage(battleData.stageId);
				WorldMapStageDataRow worldMapStageDataRow = base.regulation.worldMapStageDtbl[battleData.stageId];
				UISetter.SetActive(jobRoot, active: false);
				stageInfo.Set(battleData.defender.mainTroop);
				list = base.regulation.enemyCommanderDtbl.FindAll((EnemyCommanderDataRow enemy) => enemy.id == stage.data.enemyId);
				bossCommander.Set(list[list.Count - 1].commanderId);
				UISetter.SetActive(rechargeBtn, stage.clearCount == stage.typeData.battleCount);
				UISetter.SetActive(timeMachineRoot, active: true);
				UISetter.SetLabel(title, Localization.Get(worldMapStageDataRow.title));
				UISetter.SetLabel(description, Localization.Get(worldMapStageDataRow.description));
				UISetter.SetLabel(clearCnt, $"{stage.typeData.battleCount - stage.clearCount}/{stage.typeData.battleCount}");
				UISetter.SetLabel(bulletTopCnt, $"x{worldMapStageDataRow.bullet}");
				UISetter.SetLabel(bulletStartCnt, worldMapStageDataRow.bullet);
				UISetter.SetSprite(icon1, "Goods-bult");
				UISetter.SetSprite(icon2, "Goods-bult");
				UISetter.SetRank(starGrid, stage.star);
				int num = Mathf.Min(stage.clearEnableCount, int.Parse(base.regulation.defineDtbl["BATTLE_CLEAR_MAX_COUNT"].value));
				if (stage.data.bullet * num > base.localUser.bullet)
				{
					num = base.localUser.bullet / stage.data.bullet;
				}
				bool canBattle = stage.canBattle;
				UISetter.SetActive(RemainCountRoot, worldMapStageDataRow.type != EStageTypeIdRange.GuardPost && stage.typeData.battleCount <= int.Parse(base.regulation.defineDtbl["BATTLE_CLEAR_MAX_COUNT"].value));
				UISetter.SetButtonGray(skipBtn, canBattle && int.Parse(battleData.stageId) <= base.localUser.lastClearStage);
				UISetter.SetButtonGray(skipsBtn, canBattle && int.Parse(battleData.stageId) <= base.localUser.lastClearStage);
				UISetter.SetLabel(skipCnt, Localization.Format("1251", 1));
				UISetter.SetLabel(skipsCnt, Localization.Format("1251", Mathf.Max(num, 1)));
				UISetter.SetGameObjectName(skipsBtn, $"SkipsBtn-{num}");
			}
			else if (battleData.type == EBattleType.SeaRobber || battleData.type == EBattleType.Guerrilla)
			{
				SweepDataRow sweepData = base.regulation.FindSweepRow(readyBattle.battleData.sweepType, readyBattle.battleData.sweepLevel);
				list = base.regulation.enemyCommanderDtbl.FindAll((EnemyCommanderDataRow enemy) => enemy.id == sweepData.uid);
				bossCommander.Set(list[list.Count - 1].commanderId);
				UISetter.SetLabel(title, Localization.Get(sweepData.name));
				UISetter.SetActive(RemainCountRoot, active: false);
				UISetter.SetLabel(description, Localization.Get(sweepData.description));
				UISetter.SetLabel(bulletTopCnt, $"x{1}");
				UISetter.SetLabel(bulletStartCnt, 1);
				UISetter.SetSprite(icon1, "Goods-swp");
				UISetter.SetSprite(icon2, "Goods-swp");
				int num2 = int.Parse(base.regulation.defineDtbl["WARROOM_CLEAR_MAX_COUNT"].value);
				if (num2 > base.localUser.sweepTicket)
				{
					num2 = base.localUser.sweepTicket;
				}
				bool sweepClearState = base.localUser.GetSweepClearState(readyBattle.battleData.sweepType, readyBattle.battleData.sweepLevel);
				UISetter.SetActive(timeMachineRoot, active: true);
				UISetter.SetButtonGray(skipBtn, sweepClearState && base.localUser.sweepTicket > 0);
				UISetter.SetButtonGray(skipsBtn, sweepClearState && base.localUser.sweepTicket > 0);
				UISetter.SetLabel(skipCnt, Localization.Format("1251", 1));
				UISetter.SetLabel(skipsCnt, Localization.Format("1251", (base.localUser.sweepTicket <= num2) ? Mathf.Max(base.localUser.sweepTicket, 1) : num2));
				UISetter.SetGameObjectName(skipsBtn, $"SkipsBtn-{num2}");
				UISetter.SetActive(jobRoot, sweepData.type != 0);
				UISetter.SetSprite(jobIcon, "com_icon_" + ((EJob)sweepData.type).ToString().ToLower());
				if (sweepData.type == 1)
				{
					UISetter.SetLabel(jobLabel, Localization.Get("1323"));
				}
				else if (sweepData.type == 2)
				{
					UISetter.SetLabel(jobLabel, Localization.Get("1324"));
				}
				else if (sweepData.type == 3)
				{
					UISetter.SetLabel(jobLabel, Localization.Get("1325"));
				}
			}
			else
			{
				if (battleData.type != EBattleType.EventBattle)
				{
					return;
				}
				EventBattleFieldDataRow row = base.regulation.FindEventBattle(battleData.eventId, battleData.eventLevel);
				stageInfo.Set(battleData.defender.mainTroop);
				list = base.regulation.enemyCommanderDtbl.FindAll((EnemyCommanderDataRow enemy) => enemy.id == row.enemy);
				bossCommander.Set(list[list.Count - 1].commanderId);
				UISetter.SetActive(rechargeBtn, active: false);
				UISetter.SetLabel(title, Localization.Get(row.name));
				UISetter.SetLabel(description, Localization.Get(row.explanation));
				UISetter.SetLabel(bulletTopCnt, $"x{1}");
				UISetter.SetLabel(bulletStartCnt, 1);
				UISetter.SetSprite(icon1, "Goods-eaac");
				UISetter.SetSprite(icon2, "Goods-eaac");
				UISetter.SetRank(starGrid, battleData.eventClear);
				UISetter.SetActive(RemainCountRoot, active: false);
				UISetter.SetActive(jobRoot, row.job != EJob.Undefined);
				UISetter.SetLabel(banLabel, (row.job != EJob.Attack && row.job != EJob.Defense && row.job != EJob.Support) ? Localization.Get("10000012") : Localization.Get("18905"));
				if (row.job != 0)
				{
					UISetter.SetSprite(jobIcon, "com_icon_" + row.job.ToString().ToLower());
					if (row.job == EJob.Attack)
					{
						UISetter.SetLabel(jobLabel, Localization.Get("1323"));
					}
					else if (row.job == EJob.Defense)
					{
						UISetter.SetLabel(jobLabel, Localization.Get("1324"));
					}
					else if (row.job == EJob.Support)
					{
						UISetter.SetLabel(jobLabel, Localization.Get("1325"));
					}
					else if (row.job == EJob.Attack_x)
					{
						UISetter.SetLabel(jobLabel, Localization.Get("1323"));
					}
					else if (row.job == EJob.Defense_x)
					{
						UISetter.SetLabel(jobLabel, Localization.Get("1324"));
					}
					else if (row.job == EJob.Support_x)
					{
						UISetter.SetLabel(jobLabel, Localization.Get("1325"));
					}
				}
				flipRepeat.Lock(battleData.eventClear <= 0);
				UISetter.SetFlipSwitch(flipRepeat, GameSetting.instance.repeatBattle);
				clearConditionList.InitClearCondition(row);
			}
		}

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			UIReadyBattle uIReadyBattle = parent as UIReadyBattle;
			if (uIReadyBattle.battleData.type == EBattleType.Plunder || uIReadyBattle.battleData.type == EBattleType.SeaRobber || uIReadyBattle.battleData.type == EBattleType.Guerrilla || uIReadyBattle.battleData.type == EBattleType.EventBattle)
			{
				UISetter.SetActive(readyRoot, active: true);
				UISetter.SetActive(startRoot, active: false);
				uIReadyBattle.SetStageRewardInformation();
				Set();
			}
		}

		public override void OnRefresh()
		{
			base.OnRefresh();
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			if (readyBattle.battleData.type == EBattleType.Plunder || readyBattle.battleData.type == EBattleType.SeaRobber || readyBattle.battleData.type == EBattleType.Guerrilla || readyBattle.battleData.type == EBattleType.EventBattle)
			{
				Set();
			}
		}

		public void Ready()
		{
			UIReadyBattle uIReadyBattle = base.parentPanelBase as UIReadyBattle;
			if (uIReadyBattle.battleData.type == EBattleType.Plunder)
			{
				RoWorldMap.Stage stage = base.localUser.FindWorldMapStage(uIReadyBattle.battleData.stageId);
				WorldMapStageDataRow worldMapStageDataRow = base.regulation.worldMapStageDtbl[uIReadyBattle.battleData.stageId];
				if (!stage.canBattle)
				{
					EVipRechargeType type = ((stage.typeData.battleCount != 0) ? EVipRechargeType.StageType2 : EVipRechargeType.StageType1);
					base.uiWorld.mainCommand.OpenVipRechargePopUp(type);
					return;
				}
			}
			else if (uIReadyBattle.battleData.type == EBattleType.Guerrilla)
			{
				if (base.localUser.sweepTicket < 1)
				{
					UISimplePopup.CreateOK(localization: true, "5635", "12014", null, "1001");
					return;
				}
			}
			else if (uIReadyBattle.battleData.type != EBattleType.EventBattle)
			{
			}
			uIReadyBattle.SelectPlunderTroop();
		}

		public void Attack()
		{
			UIReadyBattle uIReadyBattle = base.parentPanelBase as UIReadyBattle;
			if (AttackBtn.GetComponent<UIButton>().isGray)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
			}
			else if (uIReadyBattle.battleData.type == EBattleType.Plunder)
			{
				if (uIReadyBattle.CheckEngagge())
				{
					uIReadyBattle.SetBattle_withMercenary();
				}
				else
				{
					uIReadyBattle.StartStageBattle();
				}
			}
			else if (uIReadyBattle.battleData.type == EBattleType.EventBattle)
			{
				if (uIReadyBattle.CheckEngagge())
				{
					uIReadyBattle.SetBattle_withMercenary();
				}
				else
				{
					uIReadyBattle.StartEventBattle();
				}
			}
			else
			{
				uIReadyBattle.StartSweep();
			}
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			UIReadyBattle uIReadyBattle = parent as UIReadyBattle;
			base.OnClick(sender, parent);
			string name = sender.name;
			switch (name)
			{
			case "Ready":
				Ready();
				return;
			case "Attack":
				Attack();
				return;
			case "SkipBtn":
				if (skipBtn.GetComponent<UIButton>().isGray)
				{
					if (uIReadyBattle.battleData.type == EBattleType.Plunder)
					{
						RoWorldMap.Stage stage = base.localUser.FindWorldMapStage(uIReadyBattle.battleData.stageId);
						WorldMapStageDataRow worldMapStageDataRow = base.regulation.worldMapStageDtbl[uIReadyBattle.battleData.stageId];
						if (!stage.canBattle)
						{
							EVipRechargeType type = ((stage.typeData.battleCount != 3) ? EVipRechargeType.StageType2 : EVipRechargeType.StageType1);
							base.uiWorld.mainCommand.OpenVipRechargePopUp(type);
						}
						else
						{
							NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("6138"));
						}
					}
					else if (base.localUser.GetSweepClearState(uIReadyBattle.battleData.sweepType, uIReadyBattle.battleData.sweepLevel))
					{
						UISimplePopup.CreateOK(localization: true, "5635", "12014", null, "1001");
					}
					else
					{
						NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("6138"));
					}
				}
				else
				{
					SoundManager.PlaySFX("BTN_Sweep_001");
					if (uIReadyBattle.battleData.type == EBattleType.Plunder)
					{
						UseTimeMachine(skips: false);
					}
					else
					{
						UseTimeMachineSweep(skips: false);
					}
				}
				return;
			}
			if (name.StartsWith("SkipsBtn"))
			{
				if (skipsBtn.GetComponent<UIButton>().isGray)
				{
					if (uIReadyBattle.battleData.type == EBattleType.Plunder)
					{
						RoWorldMap.Stage stage2 = base.localUser.FindWorldMapStage(uIReadyBattle.battleData.stageId);
						WorldMapStageDataRow worldMapStageDataRow2 = base.regulation.worldMapStageDtbl[uIReadyBattle.battleData.stageId];
						if (!stage2.canBattle)
						{
							EVipRechargeType type2 = ((stage2.typeData.battleCount != 3) ? EVipRechargeType.StageType2 : EVipRechargeType.StageType1);
							base.uiWorld.mainCommand.OpenVipRechargePopUp(type2);
						}
						else
						{
							NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("6138"));
						}
					}
					else if (base.localUser.GetSweepClearState(uIReadyBattle.battleData.sweepType, uIReadyBattle.battleData.sweepLevel))
					{
						UISimplePopup.CreateOK(localization: true, "5635", "12014", null, "1001");
					}
					else
					{
						NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("6138"));
					}
				}
				else
				{
					SoundManager.PlaySFX("BTN_Sweep_001");
					if (uIReadyBattle.battleData.type == EBattleType.Plunder)
					{
						UseTimeMachine(skips: true);
					}
					else
					{
						UseTimeMachineSweep(skips: true);
					}
				}
				return;
			}
			switch (name)
			{
			case "RechargeBtn":
			{
				RoWorldMap.Stage stage3 = base.localUser.FindWorldMapStage(uIReadyBattle.battleData.stageId);
				WorldMapStageDataRow worldMapStageDataRow3 = base.regulation.worldMapStageDtbl[uIReadyBattle.battleData.stageId];
				EVipRechargeType type3 = ((stage3.typeData.battleCount != 3) ? EVipRechargeType.StageType2 : EVipRechargeType.StageType1);
				base.uiWorld.mainCommand.OpenVipRechargePopUp(type3);
				break;
			}
			case "RepeatMode":
				if (!GameSetting.instance.repeatBattle)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("10000002"), (uIReadyBattle.battleData.type != EBattleType.Plunder) ? Localization.Get("4008") : Localization.Get("4006")));
					GameSetting.instance.repeatBattle = true;
					UISetter.SetFlipSwitch(flipRepeat, GameSetting.instance.repeatBattle);
				}
				else
				{
					GameSetting.instance.repeatBattle = false;
					UISetter.SetFlipSwitch(flipRepeat, GameSetting.instance.repeatBattle);
				}
				break;
			case "RepeatModeLock":
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("10000007"));
				break;
			}
		}

		private void UseTimeMachine(bool skips)
		{
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			if (skips)
			{
				int num = int.Parse(skipsBtn.name.Substring(skipsBtn.name.IndexOf("-") + 1));
				if (num == 0)
				{
					base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Bullet);
				}
				else
				{
					base.network.RequestUseTimeMachine(readyBattle.battleData.stageId, num, 0);
				}
			}
			else
			{
				RoWorldMap.Stage stage = base.localUser.FindWorldMapStage(readyBattle.battleData.stageId);
				if (base.localUser.bullet < stage.data.bullet)
				{
					base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Bullet);
				}
				else
				{
					base.network.RequestUseTimeMachine(readyBattle.battleData.stageId, 1, 0);
				}
			}
		}

		private void UseTimeMachineSweep(bool skips)
		{
			int num = int.Parse(base.regulation.defineDtbl["WARROOM_CLEAR_MAX_COUNT"].value);
			int cnt = 1;
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			if (skips)
			{
				cnt = ((base.localUser.sweepTicket <= num) ? base.localUser.sweepTicket : num);
			}
			base.network.RequestUseTimeMachineSweep(readyBattle.battleData.sweepType, readyBattle.battleData.sweepLevel, cnt);
		}
	}

	[Serializable]
	public class Raid : UIInnerPartBase
	{
		public GameObject readyRoot;

		public GameObject startRoot;

		public GameObject raidRoot;

		public GameObject eventRaidRoot;

		public GameObject useRoot;

		public GameObject AttackBtn;

		public UISprite useIcon;

		public UILabel useCount;

		public UIGrid useGrid;

		public List<UISprite> raidPositionList;

		public UIUnit raidUnit;

		public UISprite bossThumb;

		public UIProgressBar hpProgress;

		public UILabel hpLabel;

		public UILabel maxHpLabel;

		public void Set()
		{
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			UISetter.SetButtonGray(AttackBtn, !readyBattle._curLhsTroop.IsEmpty());
		}

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			UIReadyBattle uIReadyBattle = parent as UIReadyBattle;
			if (uIReadyBattle.battleData.type != EBattleType.Raid && uIReadyBattle.battleData.type != EBattleType.EventRaid)
			{
				return;
			}
			ResetRaidPosition();
			uIReadyBattle.StartCoroutine(uIReadyBattle.SelectRaidTroopDelay());
			UISetter.SetActive(raidRoot, uIReadyBattle.battleData.type == EBattleType.Raid);
			UISetter.SetActive(eventRaidRoot, uIReadyBattle.battleData.type == EBattleType.EventRaid);
			if (uIReadyBattle.battleData.type == EBattleType.Raid)
			{
				raidUnit.Set(uIReadyBattle.battleData.raidData.commanderId);
				RaidChallengeDataRow raidChallengeDataRow = RemoteObjectManager.instance.regulation.raidChallengeDtbl[uIReadyBattle.battleData.raidData.raidId.ToString()];
				UISetter.SetActive(raidPositionList[raidChallengeDataRow.commanderPos], active: true);
				UISetter.SetSprite(raidPositionList[raidChallengeDataRow.commanderPos], "raid-bg-boss-fm02");
				for (int i = 0; i < raidChallengeDataRow.parts.Count; i++)
				{
					if (!string.IsNullOrEmpty(raidChallengeDataRow.parts[i]) && raidChallengeDataRow.parts[i] != "0")
					{
						UISetter.SetActive(raidPositionList[raidChallengeDataRow.partsPosition[i]], active: true);
						UISetter.SetSprite(raidPositionList[raidChallengeDataRow.partsPosition[i]], "raid-bg-boss-fm03");
					}
				}
				string id = 10.ToString();
				GoodsDataRow goodsDataRow = base.regulation.FindGoodsServerFieldName(id);
				UISetter.SetActive(useRoot, active: true);
				useGrid.repositionNow = true;
				UISetter.SetSprite(useIcon, goodsDataRow.iconId);
				UISetter.SetLabel(useCount, 1);
			}
			else if (uIReadyBattle.battleData.type == EBattleType.EventRaid)
			{
				RoTroop.Slot slot = uIReadyBattle.battleData.defenderTroop.slots[0];
				RoUnit roUnit = RoUnit.Create(slot.unitId, slot.unitLevel, slot.unitRank, slot.unitCls, slot.unitCostume, slot.commanderId, slot.favorRewardStep, slot.marry, slot.transcendence, EBattleType.EventRaid);
				UISetter.SetSprite(bossThumb, $"{roUnit.currLevelReg.resourceName}_Front");
				UISetter.SetLabel(hpLabel, slot.health.ToString("N0"));
				UISetter.SetLabel(maxHpLabel, string.Format("/{0}", roUnit.currLevelReg.originMaxHealth.ToString("N0")));
				UISetter.SetProgress(hpProgress, (float)slot.health / (float)roUnit.currLevelReg.originMaxHealth);
				UISetter.SetActive(raidPositionList[slot.position], active: true);
				UISetter.SetSprite(raidPositionList[slot.position], "raid-bg-boss-fm02");
				string id2 = 6.ToString();
				GoodsDataRow goodsDataRow2 = base.regulation.FindGoodsServerFieldName(id2);
				useGrid.repositionNow = true;
				UISetter.SetSprite(useIcon, goodsDataRow2.iconId);
				UISetter.SetLabel(useCount, 1);
			}
			Set();
		}

		private void ResetRaidPosition()
		{
			for (int i = 0; i < raidPositionList.Count; i++)
			{
				UISetter.SetActive(raidPositionList[i], active: false);
			}
		}

		public override void OnRefresh()
		{
			base.OnRefresh();
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			if (readyBattle.battleData.type == EBattleType.Raid || readyBattle.battleData.type == EBattleType.EventRaid)
			{
				Set();
			}
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			UIReadyBattle uIReadyBattle = parent as UIReadyBattle;
			string name = sender.name;
			if (name == "Attack")
			{
				if (AttackBtn.GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else if (uIReadyBattle.battleData.type == EBattleType.Raid)
				{
					if (uIReadyBattle.CheckEngagge())
					{
						uIReadyBattle.SetBattle_withMercenary();
					}
					else
					{
						uIReadyBattle.StartRaid();
					}
				}
				else if (uIReadyBattle.battleData.type == EBattleType.EventRaid)
				{
					uIReadyBattle.StartEventRaid();
				}
			}
			else if (!(name == "Ready"))
			{
			}
		}
	}

	[Serializable]
	public class Defender : UIInnerPartBase
	{
		public GameObject saveDeckBtn;

		public void Set()
		{
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			UISetter.SetButtonGray(saveDeckBtn, !readyBattle.IsEmptyTroop());
		}

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			UIReadyBattle uIReadyBattle = parent as UIReadyBattle;
			if (uIReadyBattle.battleData.type == EBattleType.Raid)
			{
				Set();
			}
		}

		public override void OnRefresh()
		{
			base.OnRefresh();
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			if (readyBattle.battleData.type == EBattleType.Defender || readyBattle.battleData.type == EBattleType.WaveDuelDefender || readyBattle.battleData.type == EBattleType.WorldDuelDefender || readyBattle.battleData.type == EBattleType.Conquest)
			{
				Set();
			}
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			UIReadyBattle uIReadyBattle = parent as UIReadyBattle;
			string name = sender.name;
			if (name == "Setting")
			{
				if (uIReadyBattle.IsEmptyTroop())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else if (uIReadyBattle.battleData.type == EBattleType.Defender || uIReadyBattle.battleData.type == EBattleType.WaveDuelDefender || uIReadyBattle.battleData.type == EBattleType.WorldDuelDefender)
				{
					uIReadyBattle.SaveDefenderDeck();
				}
				else if (uIReadyBattle.battleData.type == EBattleType.Conquest)
				{
					uIReadyBattle.SaveConquestDeck();
				}
			}
		}
	}

	[Serializable]
	public class WaveBattle : UIInnerPartBase
	{
		public UITroop stageInfo;

		public UICommander bossCommander;

		public GameObject readyRoot;

		public GameObject startRoot;

		public GameObject AttackBtn;

		public UILabel title;

		public UILabel description;

		public UILabel bulletTopCnt;

		private WaveBattleDataRow row;

		public void Set()
		{
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			BattleData battleData = readyBattle.battleData;
			UISetter.SetButtonGray(AttackBtn, !readyBattle._curLhsTroop.IsEmpty());
			row = base.regulation.FindWaveBattleData(int.Parse(battleData.stageId));
			bossCommander.Set("20101");
			UISetter.SetLabel(title, Localization.Get(row.name));
			UISetter.SetLabel(description, Localization.Get(row.explanation));
			UISetter.SetLabel(bulletTopCnt, $"x{1}");
		}

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			UIReadyBattle uIReadyBattle = parent as UIReadyBattle;
			if (uIReadyBattle.battleData.type == EBattleType.WaveBattle)
			{
				UISetter.SetActive(readyRoot, active: true);
				UISetter.SetActive(startRoot, active: false);
				uIReadyBattle.SetStageRewardInformation();
				Set();
			}
		}

		public override void OnRefresh()
		{
			base.OnRefresh();
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			if (readyBattle.battleData.type == EBattleType.WaveBattle)
			{
				Set();
			}
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			UIReadyBattle uIReadyBattle = parent as UIReadyBattle;
			base.OnClick(sender, parent);
			string name = sender.name;
			if (name == "Ready")
			{
				uIReadyBattle.SelectWaveBattleTroop();
			}
			else
			{
				if (!(name == "Attack"))
				{
					return;
				}
				if (AttackBtn.GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else if (uIReadyBattle.battleData.type == EBattleType.WaveBattle)
				{
					if (uIReadyBattle.CheckEngagge())
					{
						uIReadyBattle.SetBattle_withMercenary();
					}
					else
					{
						uIReadyBattle.StartWaveBattle();
					}
				}
			}
		}
	}

	[Serializable]
	public class CooperateBattle : UIInnerPartBase
	{
		public GameObject attackBtn;

		public GameObject bossView;

		public UISprite bossUnit;

		public List<UISprite> bossPositionList;

		private CooperateBattleDataRow _coopDr;

		public void Set()
		{
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			UISetter.SetButtonGray(attackBtn, !readyBattle._curLhsTroop.IsEmpty());
			BattleData battleData = readyBattle.battleData;
			_coopDr = base.regulation.cooperateBattleDtbl[battleData.stageId];
			if (_coopDr.enemyType == ECooperateBattleEnemyType.Boss)
			{
				UISetter.SetActive(readyBattle.forBattleRoot, active: false);
				UISetter.SetActive(bossView, active: true);
				UnitDataRow unitDataRow = base.regulation.unitDtbl[_coopDr.enemy];
				UISetter.SetSprite(bossUnit, $"{unitDataRow.resourceName}_Front");
				ResetRaidPosition();
				UISetter.SetActive(bossPositionList[_coopDr.enemyrPos - 1], active: true);
				UISetter.SetSprite(bossPositionList[_coopDr.enemyrPos - 1], "raid-bg-boss-fm02");
				for (int i = 0; i < _coopDr.parts.Count; i++)
				{
					if (!string.IsNullOrEmpty(_coopDr.parts[i]) && _coopDr.parts[i] != "0")
					{
						UISetter.SetActive(bossPositionList[_coopDr.partsPos[i] - 1], active: true);
						UISetter.SetSprite(bossPositionList[_coopDr.partsPos[i] - 1], "raid-bg-boss-fm03");
					}
				}
			}
			else
			{
				UISetter.SetActive(readyBattle.forBattleRoot, active: true);
				UISetter.SetActive(bossView, active: false);
			}
		}

		private void ResetRaidPosition()
		{
			for (int i = 0; i < bossPositionList.Count; i++)
			{
				UISetter.SetActive(bossPositionList[i], active: false);
			}
		}

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			UIReadyBattle uIReadyBattle = parent as UIReadyBattle;
			if (uIReadyBattle.battleData.type == EBattleType.CooperateBattle)
			{
				uIReadyBattle.StartCoroutine(uIReadyBattle.SelectCooperateBattleTroopDelay());
				Set();
			}
		}

		public override void OnRefresh()
		{
			UIReadyBattle readyBattle = base.uiWorld.readyBattle;
			if (readyBattle.battleData.type == EBattleType.CooperateBattle)
			{
				Set();
			}
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			UIReadyBattle uIReadyBattle = parent as UIReadyBattle;
			string name = sender.name;
			if (!(name == "Attack"))
			{
				return;
			}
			if (attackBtn.GetComponent<UIButton>().isGray)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
			}
			else if (uIReadyBattle.battleData.type == EBattleType.CooperateBattle)
			{
				if (base.localUser.currentCooperateRemainTime.GetRemain() <= 0.0)
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5090010"));
				}
				else
				{
					uIReadyBattle.StartCooperateBattle();
				}
			}
		}
	}

	public UIUser leftUser;

	public UITroop left;

	public UIDefaultListView rewardListView;

	public UITroop right;

	public UIDefaultListView skillListView;

	public UILabel skillDescription;

	public UILabel skillName;

	public UILabel unitName;

	public GameObject addSkillRoot;

	public UILabel myPower;

	public UILabel mySpeed;

	public UILabel enemyPower;

	public UILabel enemySpeed;

	public UILabel wave;

	public GameObject previousTroop;

	public GameObject nextTroop;

	public GameObject forBattleRoot;

	public GameObject leftInformation;

	public GameObject leftUnitInformation;

	public GameObject forBattleRightInfoRoot;

	public GameObject easyRoot;

	public GameObject normalRoot;

	public GameObject hardRoot;

	public GameObject attackArrow;

	public GameObject ForWaveBattleRighRoot;

	public UISprite speedArrow;

	public UISprite commanderSelectBg;

	public Duel duel;

	public Plunder plunder;

	public Raid raid;

	public Defender defender;

	public WaveBattle waveBattle;

	public CooperateBattle cooperateBattle;

	public GameObject EditTroopBtn;

	public UIScrollView skillDescriptionScroll;

	[HideInInspector]
	public string listItemPrefix = "Troop-";

	public List<UIPositionPlate> positionList;

	public List<UIPositionPlate> enemyPositionList;

	private int _selectedPositionId;

	private string _selectedCommanderId;

	private int troopPosition;

	private string selectSkill;

	private List<Protocols.GuildDispatchCommanderList> AllCommanderList = new List<Protocols.GuildDispatchCommanderList>();

	private Dictionary<int, RoCommander> helperCommander = new Dictionary<int, RoCommander>();

	public UILabel lhsWave;

	public GameObject lhsMovePrevTroop;

	public GameObject lhsMoveNextTroop;

	public GameObject disableMarker;

	private int _curlhsTroopIndex;

	[HideInInspector]
	public List<RoTroop> lhsTroops;

	public UIEngageInfoPopup engageInfo;

	public List<UICommanderSelectItem> commanderList;

	public UISprite dragUnit;

	private int dragStartPosition = -1;

	public BattleData battleData { get; private set; }

	public RoTroop selectedTroop => _curLhsTroop;

	private RoTroop _curLhsTroop => lhsTroops[_curlhsTroopIndex];

	protected override void Awake()
	{
		base.Awake();
		lhsTroops = new List<RoTroop>();
	}

	public void LhsMovePrevTroop()
	{
		if (_curlhsTroopIndex > 0)
		{
			_curlhsTroopIndex--;
			LhsNextTroop();
		}
	}

	public void LhsMoveNextTroop()
	{
		if (_curlhsTroopIndex < lhsTroops.Count - 1)
		{
			_curlhsTroopIndex++;
			LhsNextTroop();
		}
	}

	private void OnRelease()
	{
		if (battleData.type != EBattleType.Raid && battleData.type != EBattleType.EventRaid)
		{
			for (int i = 0; i < right.positionPlateList.Count; i++)
			{
				right.positionPlateList[i].ResetIncompatible();
				right.positionPlateList[i].IsActiveHpRoot(battleData.type == EBattleType.Annihilation);
			}
		}
	}

	private void OnResetPlate()
	{
		for (int i = 0; i < positionList.Count; i++)
		{
			positionList[i].Reset();
			positionList[i].IsActiveHpRoot(battleData.type == EBattleType.Annihilation);
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close")
		{
			CloseAnimation();
			return;
		}
		if (text == "ReadyTroop")
		{
			UIPopup.Create<UISelectDeckPopup>("SelectDeckPopup").Init(battleData);
			return;
		}
		if (skillListView.Contains(text))
		{
			string pureId = skillListView.GetPureId(text);
			if (!string.Equals(pureId, "0"))
			{
				selectSkill = pureId;
				skillListView.SetSelection(pureId, selected: true);
				SetSkillInfo();
				SoundManager.PlaySFX("BTN_Norma_001");
			}
			return;
		}
		if (text.StartsWith("Plate-"))
		{
			int num = int.Parse(text.Substring(text.IndexOf("-") + 1));
			RoTroop.Slot slotByPosition = _curLhsTroop.GetSlotByPosition(num);
			if (slotByPosition != null && _selectedPositionId != num)
			{
				_selectedPositionId = num;
				InitSkillData();
				SetSkillInfo();
			}
			SoundManager.PlaySFX("BTN_Norma_001");
			return;
		}
		if (text.StartsWith("CommanderSelect-"))
		{
			string id = text.Substring(text.IndexOf("-") + 1);
			RoTroop.Slot slotByCommanderId = _curLhsTroop.GetSlotByCommanderId(id);
			if (slotByCommanderId != null && _selectedPositionId != slotByCommanderId.position)
			{
				_selectedPositionId = slotByCommanderId.position;
				InitSkillData();
				SetSkillInfo();
			}
			return;
		}
		switch (text)
		{
		case "PreviousTroop":
			troopPosition--;
			NextTroop();
			break;
		case "NextTroop":
			troopPosition++;
			NextTroop();
			break;
		case "LhsPrevTroop":
			LhsMovePrevTroop();
			break;
		case "LhsNextTroop":
			LhsMoveNextTroop();
			break;
		case "UseConquest":
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110322"));
			break;
		case "CommanderSelect":
			OpenCommanderList();
			break;
		default:
			SendOnClickToInnerParts(sender);
			break;
		}
	}

	private void _Set(BattleData _battleData)
	{
		if (_battleData != null)
		{
			battleData = _battleData;
			RoUser roUser = battleData.defender;
			if (roUser != null && roUser.battleTroopList.Count >= 1)
			{
			}
		}
		UISetter.SetUser(leftUser, base.localUser);
	}

	private void _SelectTroop()
	{
		left.Set(_curLhsTroop);
	}

	private void NextTroop()
	{
		if (battleData.type != EBattleType.Raid && battleData.type != EBattleType.WaveBattle && battleData.type != EBattleType.EventRaid)
		{
			UISetter.SetLabel(wave, Localization.Format("5628", troopPosition + 1));
			ResetDragObject();
			ResetTargetPlate();
			UpdateArrow();
			OnRefresh();
		}
	}

	private void LhsNextTroop()
	{
		if (battleData.type == EBattleType.Raid || battleData.type == EBattleType.WaveBattle || battleData.type == EBattleType.EventRaid)
		{
			return;
		}
		UISetter.SetLabel(lhsWave, Localization.Format("5628", _curlhsTroopIndex + 1));
		if (_curLhsTroop.IsEmpty())
		{
			OnRelease();
			ResetTargetPlate();
			ResetSkillData();
		}
		else
		{
			_selectedPositionId = _curLhsTroop.slots[0].position;
			if (_selectedPositionId <= 0)
			{
				_selectedPositionId = _curLhsTroop.GetNotEmptySlotPosition();
			}
			if (_selectedPositionId >= 0)
			{
				InitSkillData();
			}
		}
		ResetDragObject();
		LhsUpdateArrow();
		OnRefresh();
	}

	private void LhsUpdateArrow()
	{
		UISetter.SetActive(lhsMovePrevTroop, _curlhsTroopIndex > 0);
		UISetter.SetActive(lhsMoveNextTroop, _curlhsTroopIndex < lhsTroops.Count - 1);
		left.Set(lhsTroops[_curlhsTroopIndex]);
	}

	public bool IsEmptyTroop()
	{
		bool flag = false;
		bool result = true;
		for (int i = 0; i < lhsTroops.Count; i++)
		{
			bool flag2 = lhsTroops[i].IsEmpty();
			if (i == 0 && flag2)
			{
				result = true;
				break;
			}
			if (flag && !flag2)
			{
				result = true;
				break;
			}
			result = false;
			flag = flag2;
		}
		return result;
	}

	private void UpdateArrow()
	{
		UISetter.SetActive(previousTroop, troopPosition > 0);
		UISetter.SetActive(nextTroop, troopPosition < battleData.defender.battleTroopList.Count - 1);
		if (battleData.type == EBattleType.WaveDuel)
		{
			if (troopPosition < ConstValue.waveDuelTroopCount - 1)
			{
				UISetter.SetActive(disableMarker, active: false);
				right.Set(battleData.defender.battleTroopList[troopPosition]);
			}
			else
			{
				UISetter.SetActive(disableMarker, active: true);
				right.Set(RoTroop.Create(base.localUser.id));
			}
		}
		else if (battleData.type == EBattleType.WorldDuel && !battleData.worldDuelReMatch)
		{
			UISetter.SetActive(disableMarker, active: true);
			right.Set(RoTroop.Create(base.localUser.id));
		}
		else
		{
			right.Set(battleData.defender.battleTroopList[troopPosition], battleData.type);
		}
	}

	public void InitAndOpenReadyBattle(BattleData _battleData)
	{
		GameSetting.instance.repeatBattle = false;
		_curlhsTroopIndex = 0;
		lhsTroops.Clear();
		lhsTroops.Add(RoTroop.Create(base.localUser.id));
		if (_battleData.type == EBattleType.WaveDuel || _battleData.type == EBattleType.WaveDuelDefender)
		{
			for (int i = lhsTroops.Count; i < ConstValue.waveDuelTroopCount; i++)
			{
				lhsTroops.Add(RoTroop.Create(base.localUser.id));
			}
		}
		else if (_battleData.type == EBattleType.EventBattle)
		{
			EventBattleFieldDataRow eventBattleFieldDataRow = base.regulation.FindEventBattle(_battleData.eventId, _battleData.eventLevel);
			for (int j = lhsTroops.Count; j < eventBattleFieldDataRow.weWave; j++)
			{
				lhsTroops.Add(RoTroop.Create(base.localUser.id));
			}
		}
		if (lhsTroops.Count > 1)
		{
			UISetter.SetActive(lhsMovePrevTroop, _curlhsTroopIndex > 0);
			UISetter.SetActive(lhsMoveNextTroop, _curlhsTroopIndex < lhsTroops.Count - 1);
			if (lhsWave != null)
			{
				UISetter.SetActive(lhsWave.gameObject, active: true);
				UISetter.SetLabel(lhsWave, Localization.Format("5628", _curlhsTroopIndex + 1));
			}
		}
		else
		{
			UISetter.SetActive(lhsMoveNextTroop, active: false);
			UISetter.SetActive(lhsMovePrevTroop, active: false);
			if (lhsWave != null)
			{
				UISetter.SetActive(lhsWave.gameObject, active: false);
			}
		}
		if (_battleData != null && _battleData.type != 0)
		{
			UISetter.SetActive(engageInfo.gameObject, active: false);
			InitValue();
			_Set(_battleData);
			InitTroopPosition();
			OnResetPlate();
			if (battleData.type == EBattleType.Defender || battleData.type == EBattleType.WaveDuelDefender || battleData.type == EBattleType.WorldDuelDefender)
			{
				SelectDefenderTroop();
			}
			else if (battleData.type == EBattleType.WaveBattle || battleData.type == EBattleType.Guerrilla || battleData.type == EBattleType.EventBattle)
			{
				SetHelperDeck();
			}
			else if (battleData.type == EBattleType.Conquest)
			{
				SelectConquestTroop();
			}
			EBattleType type = battleData.type;
			if (type == EBattleType.Plunder || type == EBattleType.Raid || type == EBattleType.WaveBattle || type == EBattleType.EventBattle)
			{
				base.localUser.EngageCommander.Clear();
			}
			UISetter.SetActive(forBattleRoot, battleData.type != EBattleType.Raid && battleData.type != EBattleType.WaveBattle && battleData.type != EBattleType.EventRaid);
			UISetter.SetActive(forBattleRightInfoRoot, (battleData.type != EBattleType.Defender && battleData.type != EBattleType.WaveDuelDefender && battleData.type != EBattleType.WorldDuelDefender && battleData.type != EBattleType.WorldDuel && battleData.type != EBattleType.Conquest) || (battleData.type == EBattleType.WorldDuel && battleData.worldDuelReMatch));
			UISetter.SetActive(duel, battleData.type == EBattleType.Duel || battleData.type == EBattleType.WaveDuel || battleData.type == EBattleType.WorldDuel || battleData.type == EBattleType.Annihilation || battleData.type == EBattleType.InfinityBattle);
			UISetter.SetActive(plunder, battleData.type == EBattleType.Plunder || battleData.type == EBattleType.SeaRobber || battleData.type == EBattleType.Guerrilla || battleData.type == EBattleType.EventBattle);
			UISetter.SetActive(defender, battleData.type == EBattleType.Defender || battleData.type == EBattleType.WaveDuelDefender || battleData.type == EBattleType.WorldDuelDefender || battleData.type == EBattleType.Conquest);
			UISetter.SetActive(raid, battleData.type == EBattleType.Raid || battleData.type == EBattleType.EventRaid);
			UISetter.SetActive(raid.readyRoot, active: false);
			UISetter.SetActive(raid.startRoot, active: false);
			UISetter.SetActive(leftInformation, battleData.type != EBattleType.Defender && battleData.type != EBattleType.Conquest && battleData.type != EBattleType.Duel && battleData.type != EBattleType.WorldDuel && battleData.type != EBattleType.WaveDuelDefender && battleData.type != EBattleType.WorldDuelDefender && battleData.type != EBattleType.WaveDuel && battleData.type != EBattleType.Annihilation && battleData.type != EBattleType.InfinityBattle && battleData.type != EBattleType.Raid && battleData.type != EBattleType.EventRaid && battleData.type != EBattleType.CooperateBattle);
			UISetter.SetActive(leftUnitInformation, active: true);
			UISetter.SetActive(plunder.readyRoot, active: false);
			UISetter.SetActive(plunder.startRoot, active: false);
			UISetter.SetActive(waveBattle, battleData.type == EBattleType.WaveBattle);
			UISetter.SetActive(waveBattle.readyRoot, active: false);
			UISetter.SetActive(waveBattle.startRoot, active: false);
			UISetter.SetActive(attackArrow, battleData.type != EBattleType.WaveBattle);
			UISetter.SetActive(ForWaveBattleRighRoot, battleData.type == EBattleType.WaveBattle);
			UISetter.SetActive(EditTroopBtn, battleData.type != EBattleType.Annihilation && battleData.type != EBattleType.Guerrilla && battleData.type != EBattleType.Conquest && battleData.type != EBattleType.EventBattle);
			UISetter.SetActive(cooperateBattle, battleData.type == EBattleType.CooperateBattle);
			UISetter.SetActive(disableMarker, active: false);
			UISetter.SetLabel(title, Localization.Get("1252"));
			commanderSelectBg.height = ((battleData.type != EBattleType.Annihilation) ? 193 : 217);
			Open();
			SendOnInitToInnerParts();
			NextTroop();
			SetCommanderSlot();
		}
	}

	private void SetHelperDeck()
	{
		List<EnemyUnitDataRow> list = new List<EnemyUnitDataRow>();
		helperCommander.Clear();
		if (battleData.type == EBattleType.Guerrilla)
		{
			SweepDataRow sweepData = base.regulation.FindSweepRow(battleData.sweepType, battleData.sweepLevel);
			EnemyCommanderDataRow helper = base.regulation.enemyCommanderDtbl.Find((EnemyCommanderDataRow row) => row.id == sweepData.helper);
			list = base.regulation.enemyUnitDtbl.FindAll((EnemyUnitDataRow row) => row.id == helper.id);
		}
		else if (battleData.type == EBattleType.WaveBattle)
		{
			WaveBattleDataRow wave_db = base.regulation.FindWaveBattleData(int.Parse(battleData.stageId));
			list = base.regulation.enemyUnitDtbl.FindAll((EnemyUnitDataRow row) => row.id == wave_db.helper.ToString());
		}
		else if (battleData.type == EBattleType.EventBattle)
		{
			EventBattleFieldDataRow eventBattle = base.regulation.eventBattleFieldDtbl[$"{battleData.eventId}_{battleData.eventLevel}"];
			if (eventBattle.helper != 0)
			{
				list = base.regulation.enemyUnitDtbl.FindAll((EnemyUnitDataRow row) => row.id == eventBattle.helper.ToString());
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			RoTroop.Slot nextEmptySlot = _curLhsTroop.GetNextEmptySlot();
			EnemyUnitDataRow enemyUnitDataRow = list[i];
			CommanderDataRow commanderDataRow = base.regulation.GetCommanderByUnitId(enemyUnitDataRow.unitId);
			RoCommander roCommander = RoCommander.Create(commanderDataRow.id, enemyUnitDataRow.unitLevel, enemyUnitDataRow.unitGrade, enemyUnitDataRow.unitClass, 0, 0, 0, new List<int>());
			if (commanderDataRow == null)
			{
				commanderDataRow = base.regulation.commanderDtbl["1"];
			}
			nextEmptySlot.unitId = enemyUnitDataRow.unitId;
			nextEmptySlot.commanderId = commanderDataRow.id;
			nextEmptySlot.unitLevel = enemyUnitDataRow.unitLevel;
			nextEmptySlot.health = roCommander.currLevelUnitReg.maxHealth;
			nextEmptySlot.unitCls = enemyUnitDataRow.unitClass;
			nextEmptySlot.unitRank = enemyUnitDataRow.unitGrade;
			nextEmptySlot.charType = ECharacterType.Helper;
			nextEmptySlot.position = enemyUnitDataRow.unitPosition - 1;
			nextEmptySlot.scale = (float)enemyUnitDataRow.unitScale / 100f;
			UnitDataRow unitDataRow = base.regulation.unitDtbl[enemyUnitDataRow.unitId];
			for (int j = 0; j < unitDataRow.skillDrks.Count; j++)
			{
				if (!(unitDataRow.skillDrks[j] == "0"))
				{
					Troop.Slot.Skill skill = new Troop.Slot.Skill();
					SkillDataRow skillDataRow = base.regulation.skillDtbl[unitDataRow.skillDrks[j]];
					skill.id = unitDataRow.skillDrks[j];
					skill.lv = enemyUnitDataRow.skillLevel[j];
					nextEmptySlot.skills.Add(skill);
					roCommander.skillList[j] = enemyUnitDataRow.skillLevel[j];
				}
			}
			helperCommander.Add(nextEmptySlot.position, roCommander);
		}
	}

	private void WaveBattle_SetHelperDeck()
	{
		WaveBattleDataRow wave_db = base.regulation.FindWaveBattleData(int.Parse(battleData.stageId));
		helperCommander.Clear();
		List<EnemyUnitDataRow> list = base.regulation.enemyUnitDtbl.FindAll((EnemyUnitDataRow row) => row.id == wave_db.helper.ToString());
		for (int i = 0; i < list.Count; i++)
		{
			RoTroop.Slot nextEmptySlot = _curLhsTroop.GetNextEmptySlot();
			EnemyUnitDataRow enemyUnitDataRow = list[i];
			CommanderDataRow commanderDataRow = base.regulation.GetCommanderByUnitId(enemyUnitDataRow.unitId);
			if (commanderDataRow == null)
			{
				commanderDataRow = base.regulation.commanderDtbl["1"];
			}
			nextEmptySlot.unitId = enemyUnitDataRow.unitId;
			nextEmptySlot.commanderId = commanderDataRow.id;
			nextEmptySlot.unitLevel = enemyUnitDataRow.unitLevel;
			nextEmptySlot.health = base.regulation.unitDtbl[nextEmptySlot.unitId].maxHealth;
			nextEmptySlot.unitCls = enemyUnitDataRow.unitClass;
			nextEmptySlot.unitRank = enemyUnitDataRow.unitGrade;
			nextEmptySlot.charType = ECharacterType.Helper;
			nextEmptySlot.position = enemyUnitDataRow.unitPosition - 1;
			nextEmptySlot.scale = (float)enemyUnitDataRow.unitScale / 100f;
			RoCommander roCommander = RoCommander.Create(commanderDataRow.id, enemyUnitDataRow.unitLevel, enemyUnitDataRow.unitGrade, enemyUnitDataRow.unitClass, 0, 0, 0, new List<int>());
			UnitDataRow unitDataRow = base.regulation.unitDtbl[enemyUnitDataRow.unitId];
			for (int j = 0; j < unitDataRow.skillDrks.Count; j++)
			{
				if (!(unitDataRow.skillDrks[j] == "0"))
				{
					Troop.Slot.Skill skill = new Troop.Slot.Skill();
					SkillDataRow skillDataRow = base.regulation.skillDtbl[unitDataRow.skillDrks[j]];
					skill.id = unitDataRow.skillDrks[j];
					skill.lv = enemyUnitDataRow.skillLevel[j];
					nextEmptySlot.skills.Add(skill);
					roCommander.skillList[j] = enemyUnitDataRow.skillLevel[j];
				}
			}
			helperCommander.Add(nextEmptySlot.position, roCommander);
		}
	}

	private void SetConquestDeck()
	{
		int conquestDeckId = battleData.conquestDeckId;
		if (base.localUser.conquestDeck[conquestDeckId] == null)
		{
			return;
		}
		foreach (KeyValuePair<string, string> item in base.localUser.conquestDeck[conquestDeckId].deck)
		{
			RoCommander roCommander = base.localUser.FindCommander(item.Value);
			RoTroop.Slot nextEmptySlot = _curLhsTroop.GetNextEmptySlot();
			nextEmptySlot.unitId = roCommander.unitId;
			nextEmptySlot.unitLevel = roCommander.level;
			nextEmptySlot.exp = roCommander.aExp;
			nextEmptySlot.health = roCommander.hp;
			nextEmptySlot.commanderId = roCommander.id;
			nextEmptySlot.unitCls = roCommander.cls;
			nextEmptySlot.unitCostume = roCommander.currentCostume;
			nextEmptySlot.favorRewardStep = roCommander.favorRewardStep;
			nextEmptySlot.marry = roCommander.marry;
			nextEmptySlot.transcendence = roCommander.transcendence;
			nextEmptySlot.unitRank = roCommander.rank;
			nextEmptySlot.position = int.Parse(item.Key) - 1;
			nextEmptySlot.charType = roCommander.charType;
			nextEmptySlot.userIdx = roCommander.userIdx;
			nextEmptySlot.equipItem = roCommander.GetEquipItemList();
			nextEmptySlot.weaponItem = roCommander.WeaponItem;
			for (int i = 0; i < roCommander.unitReg.skillDrks.Count; i++)
			{
				Troop.Slot.Skill skill = new Troop.Slot.Skill();
				SkillDataRow skillDataRow = base.regulation.skillDtbl[roCommander.unitReg.skillDrks[i]];
				if (skillDataRow.isActiveSkillType)
				{
					skill.sp = roCommander.sp;
				}
				skill.id = roCommander.unitReg.skillDrks[i];
				skill.lv = roCommander.skillList[i];
				nextEmptySlot.skills.Add(skill);
			}
		}
	}

	private void LoadStageDeck()
	{
		string key = string.Empty;
		if (battleData.type == EBattleType.Guerrilla)
		{
			key = $"SweepDeck_{battleData.sweepType}_{battleData.sweepLevel}";
		}
		else if (battleData.type == EBattleType.EventBattle)
		{
			int eventBattleDeckIndex = Utility.GetEventBattleDeckIndex(battleData.eventId);
			if (eventBattleDeckIndex != -1)
			{
				key = $"EventBattleDeck_{eventBattleDeckIndex}";
			}
		}
		else if (battleData.type == EBattleType.Plunder)
		{
			key = "StageDeck";
		}
		else if (battleData.type == EBattleType.Duel)
		{
			key = "DuelDeck";
		}
		else if (battleData.type == EBattleType.Raid)
		{
			key = "RaidDeck";
		}
		else if (battleData.type == EBattleType.Annihilation)
		{
			key = "AnnihilationDeck";
		}
		else if (battleData.type == EBattleType.WaveDuel)
		{
			key = "WaveDuelDeck";
		}
		else if (battleData.type == EBattleType.CooperateBattle)
		{
			key = "CooperateBattleDeck";
		}
		else if (battleData.type == EBattleType.EventRaid)
		{
			key = "EventRaidBattleDeck";
		}
		else if (battleData.type == EBattleType.WorldDuel)
		{
			key = "WorldDuelDeck";
		}
		else if (battleData.type == EBattleType.InfinityBattle)
		{
			key = "InfinityBattleDeck";
		}
		string text = PlayerPrefs.GetString(key);
		if (!string.IsNullOrEmpty(text))
		{
			Dictionary<string, Dictionary<string, string>> dictionary;
			try
			{
				if (battleData.type == EBattleType.EventBattle)
				{
					text = text.Split('_')[1];
				}
				dictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(text);
			}
			catch (Exception)
			{
				dictionary = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> value = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
				dictionary.Add("1", value);
			}
			Dictionary<string, Dictionary<string, string>>.Enumerator enumerator = dictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int num = int.Parse(enumerator.Current.Key) - 1;
				if (num >= lhsTroops.Count)
				{
					break;
				}
				Dictionary<string, string> value2 = enumerator.Current.Value;
				if ((battleData.type == EBattleType.Guerrilla || battleData.type == EBattleType.EventBattle) && lhsTroops[num].NotEmptySlotCount + value2.Count > 5)
				{
					deleteSaveDeck(key);
					return;
				}
				foreach (KeyValuePair<string, string> item in value2)
				{
					RoCommander roCommander = base.localUser.FindCommander(item.Value);
					if (roCommander.state != ECommanderState.Nomal)
					{
						deleteSaveDeck(key);
						return;
					}
					if (battleData.type == EBattleType.Annihilation)
					{
						if (roCommander.isDie)
						{
							continue;
						}
					}
					else if (battleData.type == EBattleType.EventBattle)
					{
						EventBattleFieldDataRow eventBattleFieldDataRow = base.regulation.FindEventBattle(battleData.eventId, battleData.eventLevel);
						if (eventBattleFieldDataRow.job != 0 && eventBattleFieldDataRow.job != EJob.All)
						{
							if (eventBattleFieldDataRow.job <= EJob.Support)
							{
								if (roCommander.job != eventBattleFieldDataRow.job)
								{
									continue;
								}
							}
							else
							{
								EJob eJob = EJob.Undefined;
								if (eventBattleFieldDataRow.job == EJob.Attack_x)
								{
									eJob = EJob.Attack;
								}
								else if (eventBattleFieldDataRow.job == EJob.Defense_x)
								{
									eJob = EJob.Defense;
								}
								else if (eventBattleFieldDataRow.job == EJob.Support_x)
								{
									eJob = EJob.Support;
								}
								if (roCommander.job == eJob)
								{
									continue;
								}
							}
						}
					}
					if (lhsTroops[num].GetSlotByPosition(int.Parse(item.Key) - 1) != null)
					{
						deleteSaveDeck(key);
						return;
					}
					RoTroop.Slot nextEmptySlot = lhsTroops[num].GetNextEmptySlot();
					nextEmptySlot.unitId = roCommander.unitId;
					nextEmptySlot.unitLevel = roCommander.level;
					nextEmptySlot.exp = roCommander.aExp;
					nextEmptySlot.health = roCommander.hp;
					nextEmptySlot.commanderId = roCommander.id;
					nextEmptySlot.unitCls = roCommander.cls;
					nextEmptySlot.unitCostume = roCommander.currentCostume;
					nextEmptySlot.favorRewardStep = roCommander.favorRewardStep;
					nextEmptySlot.marry = roCommander.marry;
					nextEmptySlot.transcendence = roCommander.transcendence;
					nextEmptySlot.unitRank = roCommander.rank;
					nextEmptySlot.position = int.Parse(item.Key) - 1;
					nextEmptySlot.charType = roCommander.charType;
					nextEmptySlot.userIdx = roCommander.userIdx;
					nextEmptySlot.equipItem = roCommander.GetEquipItemList();
					nextEmptySlot.weaponItem = roCommander.WeaponItem;
					for (int i = 0; i < roCommander.unitReg.skillDrks.Count; i++)
					{
						Troop.Slot.Skill skill = new Troop.Slot.Skill();
						SkillDataRow skillDataRow = base.regulation.skillDtbl[roCommander.unitReg.skillDrks[i]];
						if (skillDataRow.isActiveSkillType)
						{
							skill.sp = roCommander.sp;
						}
						skill.id = roCommander.unitReg.skillDrks[i];
						skill.lv = roCommander.skillList[i];
						nextEmptySlot.skills.Add(skill);
					}
				}
			}
		}
		if (!_curLhsTroop.IsEmpty())
		{
			_selectedPositionId = _curLhsTroop.slots[0].position;
			InitSkillData();
			OnRefresh();
		}
		else
		{
			ResetSkillData();
			OnRelease();
		}
	}

	private void LoadMercenaryDeck(string key)
	{
		string @string = PlayerPrefs.GetString(key);
		if (string.IsNullOrEmpty(@string))
		{
			return;
		}
		Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(@string);
		if (base.localUser.EngageCommander == null || base.localUser.EngageCommander.Count == 0)
		{
			return;
		}
		RoCommander roCommander = null;
		for (int i = 0; i < base.localUser.EngageCommander.Count; i++)
		{
			if (base.localUser.EngageCommander[i].existEngaged == 1)
			{
				roCommander = base.localUser.EngageCommander[i];
			}
		}
		if (roCommander == null)
		{
			return;
		}
		int num = 0;
		foreach (KeyValuePair<string, string> item in dictionary)
		{
			if (roCommander.mercenaryHp <= 0 && (int)roCommander.dmgHp > 0)
			{
				deleteSaveDeck(key);
				break;
			}
			if (battleData.type == EBattleType.Annihilation && roCommander.isDie)
			{
				continue;
			}
			RoTroop.Slot slot = _curLhsTroop.slots[num];
			slot.unitId = roCommander.unitId;
			slot.unitLevel = roCommander.level;
			slot.exp = roCommander.aExp;
			slot.health = roCommander.hp;
			slot.commanderId = roCommander.id;
			slot.unitCls = roCommander.cls;
			slot.unitCostume = roCommander.currentCostume;
			slot.unitRank = roCommander.rank;
			slot.position = int.Parse(item.Key) - 1;
			slot.userIdx = roCommander.userIdx;
			slot.charType = ECharacterType.Mercenary;
			slot.existEngage = roCommander.existEngaged;
			for (int j = 0; j < roCommander.unitReg.skillDrks.Count; j++)
			{
				Troop.Slot.Skill skill = new Troop.Slot.Skill();
				SkillDataRow skillDataRow = base.regulation.skillDtbl[roCommander.unitReg.skillDrks[j]];
				if (skillDataRow.isActiveSkillType)
				{
					skill.sp = roCommander.sp;
				}
				skill.id = roCommander.unitReg.skillDrks[j];
				skill.lv = roCommander.skillList[j];
				slot.skills.Add(skill);
			}
			slot.equipItem = roCommander.GetEquipItemList();
			slot.weaponItem = roCommander.WeaponItem;
			num++;
		}
	}

	public void SetCurrentTroop(RoTroop troop)
	{
		SetCommanderSlot();
	}

	private void SetCommanderSlot()
	{
		for (int i = 0; i < commanderList.Count; i++)
		{
			UICommanderSelectItem uICommanderSelectItem = commanderList[i];
			RoTroop.Slot slot = _curLhsTroop.slots[i];
			if (slot.charType == ECharacterType.Mercenary || slot.charType == ECharacterType.NPCMercenary)
			{
				RoCommander commander = base.localUser.FindMercenaryCommander(slot.commanderId, slot.userIdx, slot.charType);
				uICommanderSelectItem.Set(commander, _curLhsTroop, battleData, slot.charType, i);
			}
			else if (slot.charType == ECharacterType.Helper)
			{
				RoCommander commander2 = helperCommander[slot.position];
				uICommanderSelectItem.Set(commander2, _curLhsTroop, battleData, slot.charType);
			}
			else
			{
				RoCommander commander3 = base.localUser.FindCommander(slot.commanderId);
				uICommanderSelectItem.Set(commander3, _curLhsTroop, battleData, slot.charType);
			}
		}
		if (_curLhsTroop.IsEmpty())
		{
			ResetSkillData();
			return;
		}
		RoTroop.Slot slotByPosition = _curLhsTroop.GetSlotByPosition(_selectedPositionId);
		if (_selectedPositionId == -1 || slotByPosition == null || slotByPosition.commanderId != _selectedCommanderId)
		{
			_selectedPositionId = _curLhsTroop.GetNotEmptySlotPosition();
			InitSkillData();
		}
	}

	public void SelectUnit()
	{
	}

	public bool CanStageDeck(Dictionary<string, int> preDeck)
	{
		for (int i = 0; i < lhsTroops.Count; i++)
		{
			if (i == _curlhsTroopIndex)
			{
				continue;
			}
			Dictionary<string, int>.Enumerator enumerator = preDeck.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (lhsTroops[i].GetSlotByCommanderId(enumerator.Current.Value.ToString()) != null)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void SetStageDeck(Dictionary<string, int> preDeck)
	{
		_selectedPositionId = -1;
		_selectedCommanderId = string.Empty;
		RoTroop.Slot[] slots = _curLhsTroop.slots;
		foreach (RoTroop.Slot slot in slots)
		{
			slot.ResetSlot();
		}
		int num = 0;
		if (preDeck.Count > 0)
		{
			foreach (KeyValuePair<string, int> item in preDeck)
			{
				RoCommander roCommander = base.localUser.FindCommander(item.Value.ToString());
				if (battleData.type == EBattleType.Annihilation && (!roCommander.isAdvancePossible || roCommander.isDie))
				{
					continue;
				}
				RoTroop.Slot slot2 = _curLhsTroop.slots[num];
				slot2.unitId = roCommander.unitId;
				slot2.unitLevel = roCommander.level;
				slot2.exp = roCommander.aExp;
				slot2.health = roCommander.hp;
				slot2.commanderId = roCommander.id;
				slot2.unitCls = roCommander.cls;
				slot2.unitCostume = roCommander.currentCostume;
				slot2.favorRewardStep = roCommander.favorRewardStep;
				slot2.marry = roCommander.marry;
				slot2.transcendence = roCommander.transcendence;
				slot2.unitRank = roCommander.rank;
				slot2.position = int.Parse(item.Key) - 1;
				slot2.charType = roCommander.charType;
				slot2.userIdx = roCommander.userIdx;
				slot2.equipItem = roCommander.GetEquipItemList();
				slot2.weaponItem = roCommander.WeaponItem;
				for (int j = 0; j < roCommander.unitReg.skillDrks.Count; j++)
				{
					Troop.Slot.Skill skill = new Troop.Slot.Skill();
					SkillDataRow skillDataRow = base.regulation.skillDtbl[roCommander.unitReg.skillDrks[j]];
					if (skillDataRow.isActiveSkillType)
					{
						skill.sp = roCommander.sp;
					}
					skill.id = roCommander.unitReg.skillDrks[j];
					skill.lv = roCommander.skillList[j];
					slot2.skills.Add(skill);
				}
				num++;
			}
		}
		if (!_curLhsTroop.IsEmpty())
		{
			_selectedPositionId = _curLhsTroop.slots[0].position;
			InitSkillData();
		}
		else
		{
			ResetSkillData();
			OnRelease();
		}
		OnRefresh();
	}

	private void deleteSaveDeck(string key)
	{
		PlayerPrefs.DeleteKey(key);
		for (int i = 0; i < lhsTroops.Count; i++)
		{
			lhsTroops[i].ResetSlots();
		}
		ResetSkillData();
		OnRelease();
		if (battleData.type == EBattleType.Guerrilla || battleData.type == EBattleType.EventBattle || battleData.type == EBattleType.WaveBattle)
		{
			SetHelperDeck();
		}
	}

	private bool FullUnitCheck()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < lhsTroops.Count; i++)
		{
			RoTroop roTroop = lhsTroops[i];
			for (int j = 0; j < roTroop.slots.Length; j++)
			{
				RoTroop.Slot slot = roTroop.slots[j];
				if (slot.IsValidId())
				{
					num++;
				}
				if (slot.IsValidId() && slot.charType != ECharacterType.Helper)
				{
					num2++;
				}
			}
		}
		if (battleData.type == EBattleType.Annihilation)
		{
			return num == 5 * lhsTroops.Count || num == base.localUser.GetAnnihilationCommanderCount();
		}
		if (battleData.type == EBattleType.Conquest)
		{
			return num == 5 * lhsTroops.Count;
		}
		if (battleData.type == EBattleType.Guerrilla)
		{
			SweepDataRow sweepDataRow = base.regulation.FindSweepRow(battleData.sweepType, battleData.sweepLevel);
			return num == 5 * lhsTroops.Count || num2 == base.localUser.GetGuerrillaCommanderCount((EJob)sweepDataRow.type);
		}
		return num == 5 * lhsTroops.Count || num == base.localUser.GetCommanderCount();
	}

	private void InitValue()
	{
		_selectedPositionId = -1;
		_selectedCommanderId = string.Empty;
		_curlhsTroopIndex = 0;
	}

	private void InitTroopPosition()
	{
		troopPosition = GetInitTroopPosition();
	}

	private int GetInitTroopPosition()
	{
		if (battleData.type == EBattleType.Annihilation)
		{
			int num = 0;
			AnnihilateBattleDataRow annihilateBattleDataRow = RemoteObjectManager.instance.regulation.annihilateBattleDtbl.Find((AnnihilateBattleDataRow row) => row.idx == battleData.stageId);
			if (annihilateBattleDataRow != null)
			{
				for (int i = 0; i < battleData.defender.battleTroopList.Count; i++)
				{
					int num2 = 0;
					for (int j = 0; j < battleData.defender.battleTroopList[i].slots.Length; j++)
					{
						if (string.IsNullOrEmpty(battleData.defender.battleTroopList[i].slots[j].unitId))
						{
							num2++;
						}
					}
					if (num2 == battleData.defender.battleTroopList[i].slots.Length)
					{
						num++;
					}
				}
				if (num != 0 && num < annihilateBattleDataRow.battleWave)
				{
					return num;
				}
				return 0;
			}
			return 0;
		}
		return 0;
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		_Set(battleData);
		SendOnRefreshToInnerParts();
		if (_curLhsTroop != null)
		{
			SetCommanderSlot();
			_SelectTroop();
			SetInfoLabel();
			SetSkillInfo();
		}
	}

	private void SetInfoLabel()
	{
		int totalPower = _curLhsTroop.GetTotalPower();
		int totalSpeed = _curLhsTroop.GetTotalSpeed();
		int num = totalSpeed;
		if (_curlhsTroopIndex != 0)
		{
			num = lhsTroops[0].GetTotalSpeed();
		}
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		if (battleData.type != EBattleType.WaveBattle || (battleData.type == EBattleType.WorldDuel && battleData.worldDuelReMatch))
		{
			if (battleData.type != EBattleType.Plunder && battleData.type != EBattleType.Guerrilla)
			{
				num2 = ((battleData.type != EBattleType.CooperateBattle) ? battleData.defender.battleTroopList[troopPosition].GetTotalPower(battleData.type) : base.regulation.cooperateBattleDtbl[battleData.stageId].power);
			}
			else
			{
				string eid = battleData.defender.id.Substring(battleData.defender.id.IndexOf("-") + 1);
				EnemyCommanderDataRow enemyCommanderDataRow = base.regulation.enemyCommanderDtbl.Find((EnemyCommanderDataRow row) => row.id == eid && row.wave == troopPosition + 1);
				num2 = enemyCommanderDataRow.power;
			}
			num3 = battleData.defender.battleTroopList[troopPosition].GetTotalSpeed(battleData.type);
			num4 = num3;
			if (troopPosition != 0)
			{
				num4 = battleData.defender.battleTroopList[0].GetTotalSpeed(battleData.type);
			}
		}
		UISetter.SetLabel(enemyPower, num2);
		UISetter.SetLabel(enemySpeed, num3);
		UISetter.SetLabel(myPower, totalPower);
		UISetter.SetLabel(mySpeed, totalSpeed);
		DefineDataRow defineDataRow = base.regulation.defineDtbl["ENEMY_DIFFICULTY"];
		int num5 = Mathf.Abs(totalPower - num2);
		int num6 = 0;
		num6 = ((totalPower == 0 || totalPower < num5) ? 100 : ((num5 != 0) ? (100 / (totalPower / num5)) : 0));
		int num7 = int.Parse(defineDataRow.value);
		UISetter.SetActive(easyRoot, num7 < num6 && totalPower - num2 > 0);
		UISetter.SetActive(hardRoot, num7 < num6 && totalPower - num2 < 0);
		UISetter.SetActive(normalRoot, !easyRoot.activeSelf && !hardRoot.activeSelf);
		UISetter.SetSprite(speedArrow, (num < num4) ? "world_direction_01" : "world_direction_02");
	}

	private void SetSkillInfo()
	{
		if (_selectedPositionId == -1)
		{
			return;
		}
		SkillDataRow skillDataRow = base.regulation.skillDtbl[selectSkill];
		RoTroop.Slot slotByPosition = _curLhsTroop.GetSlotByPosition(_selectedPositionId);
		RoCommander roCommander = new RoCommander();
		roCommander = ((slotByPosition.charType == ECharacterType.Mercenary || slotByPosition.charType == ECharacterType.NPCMercenary) ? base.localUser.FindMercenaryCommander(slotByPosition.commanderId, slotByPosition.userIdx, slotByPosition.charType) : ((slotByPosition.charType != ECharacterType.Helper) ? base.localUser.FindCommander(slotByPosition.commanderId) : helperCommander[slotByPosition.position]));
		int skillIndex = roCommander.GetSkillIndex(selectSkill);
		int skillLevel = roCommander.GetSkillLevel(skillIndex);
		UISetter.SetLabel(skillName, $"Lv{skillLevel} {Localization.Get(skillDataRow.skillName)}");
		UISetter.SetLabel(skillDescription, Localization.Format(skillDataRow.skillDescription, (float)(skillDataRow.startBonus + skillDataRow.lvBonus * (skillLevel - 1)) / 100f));
		ResetTargetPlate();
		if (skillDataRow.targetType == ESkillTargetType.Own || skillDataRow.targetType == ESkillTargetType.Friend)
		{
			if (skillDataRow.targetType == ESkillTargetType.Friend)
			{
				SetFreindTargetPlate(state: true);
			}
			else
			{
				SetFreindTargetPlate(state: false);
			}
		}
		else
		{
			SetFreindTargetPlate(state: false);
			if (battleData.type == EBattleType.WaveDuel)
			{
				if (troopPosition < ConstValue.waveDuelTroopCount - 1)
				{
					SetEnemyTargetPlate(skillDataRow, _selectedPositionId, skillDataRow._projectileDrks);
				}
			}
			else
			{
				SetEnemyTargetPlate(skillDataRow, _selectedPositionId, skillDataRow._projectileDrks);
			}
		}
		skillDescriptionScroll.ResetPosition();
	}

	public void InitSkillData()
	{
		OnRelease();
		List<SkillDataRow> list = new List<SkillDataRow>();
		RoTroop.Slot slotByPosition = _curLhsTroop.GetSlotByPosition(_selectedPositionId);
		RoCommander roCommander = new RoCommander();
		roCommander = ((slotByPosition.charType == ECharacterType.Mercenary || slotByPosition.charType == ECharacterType.NPCMercenary) ? base.localUser.FindMercenaryCommander(slotByPosition.commanderId, slotByPosition.userIdx, slotByPosition.charType) : ((slotByPosition.charType != ECharacterType.Helper) ? base.localUser.FindCommander(slotByPosition.commanderId) : helperCommander[slotByPosition.position]));
		_selectedCommanderId = roCommander.id;
		RoUnit roUnit = RoUnit.Create(roCommander.unitId, roCommander.level, roCommander.rank, roCommander.cls, roCommander.currentCostume, roCommander.id, roCommander.favorRewardStep, roCommander.marry, roCommander.transcendence);
		UISetter.SetLabel(unitName, Localization.Format("1021", string.Concat(roUnit.level, "  ", Localization.Get(roUnit.unitReg.nameKey))));
		List<string> skillIdList = roCommander.GetSkillIdList();
		for (int i = 0; i < 4; i++)
		{
			SkillDataRow skillDataRow = null;
			string text = skillIdList[i];
			if (!string.Equals(text, "0"))
			{
				skillDataRow = base.regulation.skillDtbl[text];
				list.Add(skillDataRow);
			}
		}
		skillListView.InitSkillList(list, "Skill-");
		skillListView.SetSelection(skillIdList[0], selected: true);
		selectSkill = skillIdList[0];
		UISetter.SetActive(addSkillRoot, skillIdList.Count > 4);
		if (skillIdList.Count > 4)
		{
			UISetter.SetGameObjectName(addSkillRoot, $"Skill-{skillIdList[4]}");
		}
	}

	public void ResetSkillData()
	{
		UISetter.SetLabel(skillName, string.Empty);
		UISetter.SetLabel(skillDescription, string.Empty);
		UISetter.SetLabel(unitName, string.Empty);
		List<SkillDataRow> list = new List<SkillDataRow>();
		skillListView.InitSkillList(list, "Skill-");
		UISetter.SetActive(addSkillRoot, active: false);
		_selectedPositionId = -1;
		_selectedCommanderId = string.Empty;
		PlateSelect(_selectedPositionId);
	}

	public void SelectPlunderTroop()
	{
		_SelectTroop();
		UISetter.SetActive(forBattleRoot, active: true);
		UISetter.SetActive(leftInformation, active: false);
		UISetter.SetActive(leftUnitInformation, active: true);
		UISetter.SetActive(plunder.readyRoot, active: false);
		UISetter.SetActive(plunder.startRoot, active: true);
		UISetter.SetActive(raid.readyRoot, active: false);
		UISetter.SetActive(raid.startRoot, active: false);
		LoadStageDeck();
		SetInfoLabel();
	}

	public void SelectDuelTroop()
	{
		_SelectTroop();
		UISetter.SetActive(leftInformation, active: false);
		UISetter.SetActive(leftUnitInformation, active: true);
		UISetter.SetActive(plunder.readyRoot, active: false);
		UISetter.SetActive(plunder.startRoot, active: false);
		UISetter.SetActive(raid.readyRoot, active: false);
		UISetter.SetActive(raid.startRoot, active: false);
		LoadStageDeck();
		SetInfoLabel();
	}

	public void SelectWaveBattleTroop()
	{
		_SelectTroop();
		UISetter.SetActive(forBattleRoot, active: false);
		UISetter.SetActive(leftInformation, active: false);
		UISetter.SetActive(leftUnitInformation, active: true);
		UISetter.SetActive(plunder.readyRoot, active: false);
		UISetter.SetActive(plunder.startRoot, active: true);
		UISetter.SetActive(waveBattle.readyRoot, active: false);
		UISetter.SetActive(waveBattle.startRoot, active: false);
		UISetter.SetActive(attackArrow, active: false);
		LoadStageDeck();
		SetInfoLabel();
	}

	public IEnumerator SelectDuelTroopDelay()
	{
		yield return null;
		SelectDuelTroop();
	}

	public void SelectDefenderTroop()
	{
		lhsTroops = new List<RoTroop>(base.localUser.defenderTroops.Count);
		for (int i = 0; i < base.localUser.defenderTroops.Count; i++)
		{
			lhsTroops.Add(base.localUser.defenderTroops[i].Clone());
		}
		if (!_curLhsTroop.IsEmpty())
		{
			_selectedPositionId = _curLhsTroop.slots[0].position;
			InitSkillData();
		}
		else
		{
			OnRelease();
			ResetSkillData();
			ResetTargetPlate();
		}
		OnRefresh();
	}

	public void SelectConquestTroop()
	{
		SetConquestDeck();
		if (!_curLhsTroop.IsEmpty())
		{
			_selectedPositionId = _curLhsTroop.slots[0].position;
			InitSkillData();
		}
		else
		{
			OnRelease();
			ResetSkillData();
		}
		OnRefresh();
	}

	public void SelectRaidTroop()
	{
		_SelectTroop();
		UISetter.SetActive(leftInformation, active: false);
		UISetter.SetActive(leftUnitInformation, active: true);
		UISetter.SetActive(plunder.readyRoot, active: false);
		UISetter.SetActive(plunder.startRoot, active: false);
		UISetter.SetActive(raid.readyRoot, active: false);
		UISetter.SetActive(raid.startRoot, active: true);
		LoadStageDeck();
		SetInfoLabel();
	}

	public IEnumerator SelectRaidTroopDelay()
	{
		yield return null;
		SelectRaidTroop();
	}

	public void SelectCooperateBattleTroop()
	{
		_SelectTroop();
		UISetter.SetActive(leftInformation, active: false);
		UISetter.SetActive(leftUnitInformation, active: true);
		UISetter.SetActive(plunder.readyRoot, active: false);
		UISetter.SetActive(plunder.startRoot, active: false);
		UISetter.SetActive(raid.readyRoot, active: false);
		UISetter.SetActive(raid.startRoot, active: false);
		LoadStageDeck();
		SetInfoLabel();
	}

	public IEnumerator SelectCooperateBattleTroopDelay()
	{
		yield return null;
		SelectCooperateBattleTroop();
	}

	private void SetUnitIncompatible(RoTroop.Slot slot)
	{
		UnitDataRow unitDataRow = base.regulation.unitDtbl[slot.unitId];
		for (int i = 0; i < enemyPositionList.Count; i++)
		{
			RoTroop.Slot slotByPosition = battleData.defender.battleTroopList[troopPosition].GetSlotByPosition(i);
			if (slotByPosition != null)
			{
				UnitDataRow unitDataRow2 = base.regulation.unitDtbl[slotByPosition.unitId];
				if (unitDataRow.typeUpper == unitDataRow2.type)
				{
					enemyPositionList[i].SetIncompatible(IncompatibleType.Up);
				}
				else if (unitDataRow.typeDown == unitDataRow2.type)
				{
					enemyPositionList[i].SetIncompatible(IncompatibleType.Down);
				}
				else
				{
					enemyPositionList[i].SetIncompatible(IncompatibleType.Equal);
				}
			}
		}
	}

	private void SetStageRewardInformation()
	{
		if (!string.IsNullOrEmpty(battleData.stageId) || battleData.sweepLevel != 0)
		{
			List<RewardDataRow> list = null;
			if (battleData.type == EBattleType.Plunder)
			{
				list = base.regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == ERewardCategory.World && row.type == int.Parse(battleData.stageId));
			}
			else if (battleData.type == EBattleType.SeaRobber || battleData.type == EBattleType.Guerrilla)
			{
				list = base.regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == ERewardCategory.Situation && row.type == battleData.sweepType && row.typeIndex == battleData.sweepLevel);
			}
			else
			{
				if (battleData.type == EBattleType.Annihilation)
				{
					return;
				}
				if (battleData.type == EBattleType.WaveBattle)
				{
					list = base.regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == ERewardCategory.WaveBattle && row.type == int.Parse(battleData.stageId));
					Dictionary<int, RewardDataRow> dictionary = new Dictionary<int, RewardDataRow>();
					List<RewardDataRow> list2 = new List<RewardDataRow>();
					for (int i = 0; i < list.Count; i++)
					{
						if (!dictionary.ContainsKey(list[i].rewardIdx))
						{
							dictionary.Add(list[i].rewardIdx, list[i]);
							list2.Add(list[i]);
						}
					}
					rewardListView.InitRewardList(list2);
					return;
				}
			}
			rewardListView.InitRewardList(list);
		}
		else if (battleData.type == EBattleType.EventBattle)
		{
			List<RewardDataRow> list3 = base.regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == ERewardCategory.EventBattle && row.type == battleData.eventId && row.typeIndex == battleData.eventLevel);
			rewardListView.InitRewardList(list3);
		}
	}

	private void PlateSelect(int position)
	{
		CommanderSelect(position);
		for (int i = 0; i < positionList.Count; i++)
		{
			positionList[i].SelectPlateState((i == position) ? true : false);
		}
	}

	private void CommanderSelect(int position)
	{
		RoTroop.Slot slotByPosition = _curLhsTroop.GetSlotByPosition(position);
		int num = -1;
		if (slotByPosition != null)
		{
			num = slotByPosition.slotNum - 1;
		}
		for (int i = 0; i < commanderList.Count; i++)
		{
			commanderList[i].SetSelection((i == num) ? true : false);
		}
	}

	private void EnemyUnitPlateSetting(List<RoTroop> troopList)
	{
	}

	private void OnDragStart(GameObject go)
	{
		if (dragUnit == null)
		{
			return;
		}
		string text = go.name;
		if (text.StartsWith("Plate-"))
		{
			int num = int.Parse(text.Substring(text.IndexOf("-") + 1));
			UIPositionPlate uIPositionPlate = positionList[num];
			if (_curLhsTroop.GetSlotByPosition(num) == null || _curLhsTroop.GetSlotByPosition(num).charType == ECharacterType.Helper)
			{
				dragStartPosition = -1;
				return;
			}
			uIPositionPlate.CopyThumbnail(dragUnit);
			UISetter.SetActive(dragUnit, active: true);
			uIPositionPlate.SetThumbnailAlpha(0.5f);
			dragStartPosition = num;
		}
	}

	private void OnDragEnd(GameObject go)
	{
		UISetter.SetActive(dragUnit, active: false);
		dragStartPosition = -1;
	}

	private void OnDrag(GameObject go, Vector2 delta)
	{
		if (dragStartPosition >= 0)
		{
			dragUnit.transform.position = UICamera.lastWorldPosition;
		}
	}

	private void OnDragOver(GameObject go, GameObject obj)
	{
		if (dragStartPosition < 0)
		{
			return;
		}
		string text = go.name;
		positionList[dragStartPosition].SetThumbnailAlpha(1f);
		if (!text.StartsWith("Plate-"))
		{
			return;
		}
		int num = int.Parse(text.Substring(text.IndexOf("-") + 1));
		RoTroop.Slot slotByPosition = _curLhsTroop.GetSlotByPosition(dragStartPosition);
		RoTroop.Slot slot = _curLhsTroop.GetSlotByPosition(num);
		if (slot != null && slot.charType == ECharacterType.Helper)
		{
			slot = slotByPosition;
		}
		if (slotByPosition != null && slot != null)
		{
			if (slotByPosition.position == _selectedPositionId)
			{
				_selectedPositionId = slot.position;
			}
			else if (slot.position == _selectedPositionId)
			{
				_selectedPositionId = slotByPosition.position;
			}
			int position = slotByPosition.position;
			slotByPosition.position = slot.position;
			slot.position = position;
		}
		else if (slot == null)
		{
			if (slotByPosition.position == _selectedPositionId)
			{
				_selectedPositionId = num;
			}
			slotByPosition.position = num;
		}
		SoundManager.PlaySFX("BTN_Formation_001");
		OnRefresh();
	}

	private void CommanderListReset()
	{
	}

	private void OpenCommanderList()
	{
		base.uiWorld.commanderList.Init(lhsTroops, _curlhsTroopIndex, battleData);
	}

	private bool ExistEnemy(int position)
	{
		RoTroop roTroop = battleData.defender.battleTroopList[troopPosition];
		if (roTroop.GetSlotByPosition(position) != null)
		{
			return true;
		}
		return false;
	}

	private int GetSkillTargetPosition(SkillDataRow row, int position)
	{
		int[,] array = null;
		switch (position)
		{
		case 0:
		case 3:
		case 6:
			array = row.targetPattern switch
			{
				ESkillTargetPattern.Normal => Frame._PriorityTable036, 
				ESkillTargetPattern.sequence => Frame._PriorityFrontTable036, 
				ESkillTargetPattern.Front => Frame._PriorityFrontTable036, 
				ESkillTargetPattern.Back => Frame._PriorityBackTable036, 
				_ => Frame._PriorityTable036, 
			};
			break;
		case 1:
		case 4:
		case 7:
			array = row.targetPattern switch
			{
				ESkillTargetPattern.Normal => Frame._PriorityTable147, 
				ESkillTargetPattern.sequence => Frame._PriorityFrontTable036, 
				ESkillTargetPattern.Front => Frame._PriorityFrontTable147, 
				ESkillTargetPattern.Back => Frame._PriorityBackTable147, 
				_ => Frame._PriorityTable147, 
			};
			break;
		case 2:
		case 5:
		case 8:
			array = row.targetPattern switch
			{
				ESkillTargetPattern.Normal => Frame._PriorityTable258, 
				ESkillTargetPattern.sequence => Frame._PriorityFrontTable036, 
				ESkillTargetPattern.Front => Frame._PriorityFrontTable258, 
				ESkillTargetPattern.Back => Frame._PriorityBackTable258, 
				_ => Frame._PriorityTable258, 
			};
			break;
		}
		for (int i = 0; i < array.GetLength(0); i++)
		{
			for (int j = 0; j < array.GetLength(1); j++)
			{
				if (ExistEnemy(array[i, j]))
				{
					return array[i, j];
				}
			}
		}
		return -1;
	}

	private List<int> GetSkillTargetPositionList(int target, string pattern)
	{
		List<int> list = new List<int>();
		if (target == -1)
		{
			return list;
		}
		int num = target % 3;
		int num2 = target / 3;
		int num3 = 2 - num2;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				string text = pattern.Substring((num3 + i) * 5, 5);
				int num4 = 2 - num;
				string text2 = text.Substring(num4 + j, 1);
				if (text2.Equals("P"))
				{
					list.Add(1);
				}
				else
				{
					list.Add(int.Parse(text2.ToString()));
				}
			}
		}
		return list;
	}

	private void SetFreindTargetPlate(bool state)
	{
		if (state)
		{
			for (int i = 0; i < positionList.Count; i++)
			{
				positionList[i].IsTargetPlate(state);
				UISetter.SetActive(positionList[i].selectedRoot, i == _selectedPositionId);
			}
		}
		else
		{
			PlateSelect(_selectedPositionId);
		}
	}

	private void SetEnemyTargetPlate(SkillDataRow row, int position, List<string> patternList)
	{
		if (battleData.type == EBattleType.Raid || battleData.type == EBattleType.WaveBattle || battleData.type == EBattleType.Conquest || battleData.type == EBattleType.EventRaid)
		{
			return;
		}
		ResetTargetPlate();
		for (int i = 0; i < patternList.Count; i++)
		{
			if (base.regulation.projectileDtbl.ContainsKey(patternList[i]))
			{
				ProjectileDataRow projectileDataRow = base.regulation.projectileDtbl[patternList[i]];
				int skillTargetPosition = GetSkillTargetPosition(row, position);
				List<int> skillTargetPositionList = GetSkillTargetPositionList(skillTargetPosition, projectileDataRow.splashPattern);
				if (skillTargetPositionList.Count == 0)
				{
					break;
				}
				for (int j = 0; j < enemyPositionList.Count; j++)
				{
					enemyPositionList[j].IsEnemyTargetPlate(skillTargetPositionList[j] == 1);
				}
			}
		}
	}

	private void ResetTargetPlate()
	{
		if (battleData.type != EBattleType.Raid && battleData.type != EBattleType.WaveBattle && battleData.type != EBattleType.Conquest && battleData.type != EBattleType.EventRaid)
		{
			for (int i = 0; i < enemyPositionList.Count; i++)
			{
				enemyPositionList[i].IsEnemyTargetPlate(state: false);
			}
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		UICamera.onDragStart = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onDragStart, new UICamera.VoidDelegate(OnDragStart));
		UICamera.onDragEnd = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onDragEnd, new UICamera.VoidDelegate(OnDragEnd));
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Remove(UICamera.onDrag, new UICamera.VectorDelegate(OnDrag));
		UICamera.onDragOut = (UICamera.ObjectDelegate)Delegate.Remove(UICamera.onDragOut, new UICamera.ObjectDelegate(OnDragOver));
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		UICamera.onDragStart = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onDragStart, new UICamera.VoidDelegate(OnDragStart));
		UICamera.onDragEnd = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onDragEnd, new UICamera.VoidDelegate(OnDragEnd));
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Combine(UICamera.onDrag, new UICamera.VectorDelegate(OnDrag));
		UICamera.onDragOut = (UICamera.ObjectDelegate)Delegate.Combine(UICamera.onDragOut, new UICamera.ObjectDelegate(OnDragOver));
	}

	public void ResetDragObject()
	{
		UISetter.SetActive(dragUnit, active: false);
		if (dragStartPosition >= 0)
		{
			positionList[dragStartPosition].SetThumbnailAlpha(1f);
			dragStartPosition = -1;
		}
	}

	public void StartWaveDuel()
	{
		if (base.localUser.waveDuelTicket < 1)
		{
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.waveDuelTicket);
		}
		if (FullUnitCheck())
		{
			battleData.attacker = base.localUser.CreateForBattle(lhsTroops);
			base.network.RequestPvPStartWaveDuel(battleData);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "5040011", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (IsEmptyTroop())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else
				{
					battleData.attacker = base.localUser.CreateForBattle(lhsTroops);
					base.network.RequestPvPStartWaveDuel(battleData);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public void StartDuel()
	{
		if (base.localUser.challenge < 1)
		{
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Challenge);
			return;
		}
		if (FullUnitCheck())
		{
			battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
			base.network.RequestPvPStartDuel(battleData);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "1331", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (_curLhsTroop.IsEmpty())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else
				{
					battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
					base.network.RequestPvPStartDuel(battleData);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public void StartWorldDuel()
	{
		if (base.localUser.worldDuelTicket < 1)
		{
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.WorldDuelTicket);
			return;
		}
		if (FullUnitCheck())
		{
			battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
			base.network.RequestPvPStartWorldDuel(battleData);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "1331", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (_curLhsTroop.IsEmpty())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else
				{
					battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
					base.network.RequestPvPStartWorldDuel(battleData);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public void StartSweep()
	{
		if (FullUnitCheck())
		{
			battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
			base.network.RequestSituationSweepStart(battleData);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "1331", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (_curLhsTroop.IsEmpty())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else
				{
					battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
					base.network.RequestSituationSweepStart(battleData);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public void StartRaid()
	{
		if (base.localUser.opener < 1)
		{
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Key);
			return;
		}
		if (FullUnitCheck())
		{
			battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
			base.network.RequestRaid(battleData);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "1331", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (_curLhsTroop.IsEmpty())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else
				{
					battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
					base.network.RequestRaid(battleData);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public void StartEventRaid()
	{
		if (base.localUser.eventRaidTicket < 1)
		{
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.EventRaidTicket);
			return;
		}
		if (FullUnitCheck())
		{
			battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
			base.network.RequestEventRaidBattleStart(battleData);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "1331", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (_curLhsTroop.IsEmpty())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else
				{
					battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
					base.network.RequestEventRaidBattleStart(battleData);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public void StartStageBattle()
	{
		RoWorldMap.Stage stage = base.localUser.FindWorldMapStage(battleData.stageId);
		WorldMapStageDataRow worldMapStageDataRow = base.regulation.worldMapStageDtbl[battleData.stageId];
		if (base.localUser.bullet < worldMapStageDataRow.bullet)
		{
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Bullet);
			return;
		}
		if (FullUnitCheck())
		{
			battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
			base.network.RequestPlunder(battleData);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "1331", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (_curLhsTroop.IsEmpty())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else
				{
					battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
					base.network.RequestPlunder(battleData);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public void StartEventBattle()
	{
		if (base.localUser.oil < 1)
		{
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Oil);
			return;
		}
		if (FullUnitCheck())
		{
			if (IsShowEventBattleScenario())
			{
				StartCoroutine(startEventBattleScenario());
				return;
			}
			battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
			base.network.RequestEventBattle(battleData);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "1331", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (_curLhsTroop.IsEmpty())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else if (IsShowEventBattleScenario())
				{
					StartCoroutine(startEventBattleScenario());
				}
				else
				{
					battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
					base.network.RequestEventBattle(battleData);
				}
			}
		};
	}

	public void SetBattle_withMercenary()
	{
		if (GameSetting.instance.repeatBattle)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1382"));
			return;
		}
		switch (battleData.type)
		{
		case EBattleType.Plunder:
		{
			RoWorldMap.Stage stage = base.localUser.FindWorldMapStage(battleData.stageId);
			WorldMapStageDataRow worldMapStageDataRow = base.regulation.worldMapStageDtbl[battleData.stageId];
			if (base.localUser.bullet < worldMapStageDataRow.bullet)
			{
				base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Bullet);
				return;
			}
			break;
		}
		case EBattleType.Raid:
			if (base.localUser.opener < 1)
			{
				base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Key);
				return;
			}
			break;
		case EBattleType.EventBattle:
			if (base.localUser.oil < 1)
			{
				base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Oil);
				return;
			}
			break;
		}
		if (FullUnitCheck())
		{
			engageInfo.SetEngagedPopup();
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "1331", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (_curLhsTroop.IsEmpty())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else if (CheckEngagge())
				{
					engageInfo.SetEngagedPopup();
				}
				else
				{
					StartBattle_withMercenary(-1);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public void StartBattle_withMercenary(int kind)
	{
		battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
		switch (battleData.type)
		{
		case EBattleType.Raid:
			base.network.RequestRaid(battleData, kind);
			break;
		case EBattleType.Plunder:
			base.network.RequestPlunder(battleData, kind);
			break;
		case EBattleType.WaveBattle:
			base.network.RequestWaveBattleStart(battleData, kind);
			break;
		case EBattleType.EventBattle:
			base.network.RequestEventBattle(battleData, kind);
			break;
		}
	}

	public void StartAnnihilation()
	{
		if (FullUnitCheck())
		{
			battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
			base.network.RequestAnnihilationStageStart(battleData);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "1331", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (_curLhsTroop.IsEmpty())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else
				{
					battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
					base.network.RequestAnnihilationStageStart(battleData);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public void StartInfinityBattle()
	{
		if (FullUnitCheck())
		{
			battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
			base.network.RequestInfinityBattleStart(battleData);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "1331", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (_curLhsTroop.IsEmpty())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else
				{
					battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
					base.network.RequestInfinityBattleStart(battleData);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public void StartWaveBattle()
	{
		if (FullUnitCheck())
		{
			battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
			base.network.RequestWaveBattleStart(battleData);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "1331", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (_curLhsTroop.IsEmpty())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else
				{
					battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
					base.network.RequestWaveBattleStart(battleData);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public void StartCooperateBattle()
	{
		if (FullUnitCheck())
		{
			battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
			base.network.RequestCooperateBattleStart(battleData);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "1331", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (_curLhsTroop.IsEmpty())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else
				{
					battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
					base.network.RequestCooperateBattleStart(battleData);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public bool CheckEngagge()
	{
		for (int i = 0; i < _curLhsTroop.slots.Length; i++)
		{
			if ((_curLhsTroop.slots[i].charType == ECharacterType.Mercenary || _curLhsTroop.slots[i].charType == ECharacterType.NPCMercenary) && _curLhsTroop.slots[i].existEngage == 0)
			{
				return true;
			}
		}
		return false;
	}

	public void CheckInfinityBattle()
	{
		if (base.localUser.InfinityEnableReward(battleData.stageId))
		{
			StartInfinityBattle();
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "3400004", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				StartInfinityBattle();
			}
		};
	}

	public void SaveDefenderDeck()
	{
		if (FullUnitCheck())
		{
			if (battleData.type == EBattleType.WaveDuelDefender)
			{
				base.network.RequestWaveDuelDefenderSetting(lhsTroops);
			}
			else if (battleData.type == EBattleType.Defender)
			{
				base.network.RequestDefenderSetting(_curLhsTroop);
			}
			else if (battleData.type == EBattleType.WorldDuelDefender)
			{
				base.network.RequestWorldDuelDefenderSetting(_curLhsTroop);
			}
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", (battleData.type != EBattleType.WaveDuelDefender) ? "1331" : "5040011", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (IsEmptyTroop())
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1332"));
				}
				else if (battleData.type == EBattleType.WaveDuelDefender)
				{
					base.network.RequestWaveDuelDefenderSetting(lhsTroops);
				}
				else if (battleData.type == EBattleType.Defender)
				{
					base.network.RequestDefenderSetting(_curLhsTroop);
				}
				else if (battleData.type == EBattleType.WorldDuelDefender)
				{
					base.network.RequestWorldDuelDefenderSetting(_curLhsTroop);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public void SaveConquestDeck()
	{
		if (FullUnitCheck())
		{
			battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
			base.network.RequestSetConquestTroop(battleData);
		}
		else
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("18916"));
		}
	}

	private bool IsShowEventBattleScenario()
	{
		EventBattleScenarioDataRow eventBattleScenarioDataRow = base.regulation.eventBattleScenarioDtbl.Find((EventBattleScenarioDataRow row) => row.eventIdx == battleData.eventId.ToString() && row.eventType == battleData.eventLevel.ToString() && row.timing == EventScenarioTimingType.AfterBattleBtn);
		if (eventBattleScenarioDataRow == null)
		{
			return false;
		}
		int playTurn = eventBattleScenarioDataRow.playTurn;
		int lastShowEventScenarioPlayTurn = base.localUser.lastShowEventScenarioPlayTurn;
		if (playTurn > lastShowEventScenarioPlayTurn)
		{
			return true;
		}
		return false;
	}

	private IEnumerator startEventBattleScenario()
	{
		EventBattleScenarioDataRow scenarioData = base.regulation.eventBattleScenarioDtbl.Find((EventBattleScenarioDataRow row) => row.eventIdx == battleData.eventId.ToString() && row.timing == EventScenarioTimingType.AfterBattleBtn && row.eventType == battleData.eventLevel.ToString());
		if (scenarioData != null)
		{
			ClassicRpgManager rpgManager = UIManager.instance.world.dialogMrg;
			if (rpgManager != null)
			{
				rpgManager.StartEventScenario();
				rpgManager.InitScenarioDialogue(scenarioData.scenarioIdx, DialogueType.Event);
			}
			while (rpgManager.isPlayDialogue)
			{
				yield return null;
			}
		}
		battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { _curLhsTroop });
		base.network.RequestEventBattle(battleData);
	}

	public void OpenAnimation()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", 0, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeOutBack));
	}

	public void CloseAnimation()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", -1000, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeInBack, "oncomplete", "Close", "oncompletetarget", base.gameObject));
	}

	private void SaveDeck()
	{
		if (battleData.type == EBattleType.WaveDuel)
		{
			List<RoTroop> battleTroopList = battleData.attacker.battleTroopList;
			Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
			for (int i = 0; i < battleTroopList.Count; i++)
			{
				RoTroop roTroop = battleTroopList[i];
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				for (int j = 0; j < roTroop.slots.Length; j++)
				{
					RoTroop.Slot slot = roTroop.slots[j];
					if (slot.IsValid())
					{
						dictionary2.Add((slot.position + 1).ToString(), slot.commanderId);
					}
				}
				dictionary.Add((i + 1).ToString(), dictionary2);
			}
			PlayerPrefs.SetString("WaveDuelDeck", JObject.FromObject(dictionary).ToString());
			return;
		}
		Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
		Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
		RoTroop attackerTroop = battleData.attackerTroop;
		for (int k = 0; k < attackerTroop.slots.Length; k++)
		{
			RoTroop.Slot slot2 = attackerTroop.slots[k];
			if (slot2.IsValid() && slot2.charType != ECharacterType.Helper)
			{
				if (slot2.charType == ECharacterType.Mercenary || slot2.charType == ECharacterType.SuperMercenary)
				{
					dictionary3.Add((slot2.position + 1).ToString(), slot2.commanderId);
				}
				else
				{
					dictionary4.Add((slot2.position + 1).ToString(), slot2.commanderId);
				}
			}
		}
		switch (battleData.type)
		{
		case EBattleType.Plunder:
			PlayerPrefs.SetString("StageDeck", JObject.FromObject(dictionary4).ToString());
			break;
		case EBattleType.Raid:
			PlayerPrefs.SetString("RaidDeck", JObject.FromObject(dictionary4).ToString());
			break;
		case EBattleType.Annihilation:
			PlayerPrefs.SetString("MercenaryDeck", JObject.FromObject(dictionary3).ToString());
			PlayerPrefs.SetString("AnnihilationDeck", JObject.FromObject(dictionary4).ToString());
			break;
		case EBattleType.CooperateBattle:
			PlayerPrefs.SetString("CooperateBattleDeck", JObject.FromObject(dictionary4).ToString());
			break;
		case EBattleType.Duel:
			PlayerPrefs.SetString("DuelDeck", JObject.FromObject(dictionary4).ToString());
			break;
		case EBattleType.Guerrilla:
			PlayerPrefs.SetString($"SweepDeck_{battleData.sweepType}_{battleData.sweepLevel}", JObject.FromObject(dictionary4).ToString());
			break;
		}
	}

	public void CloseStartItem()
	{
	}

	public override void Open()
	{
		base.Open();
		OpenAnimation();
	}

	public override void Close()
	{
		base.Close();
	}
}
