using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Cache;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared;
using Shared.Battle;
using Shared.Regulation;
using Step;
using UnityEngine;

public class M04_Tutorial : AbstractBattle
{
	public enum State
	{
		Unknown,
		Opening,
		Playing,
		InputWait,
		CutIn,
		Result
	}

	private enum EDialogueState
	{
		None,
		Play,
		Stoping
	}

	public enum E_CUTIN_STATE
	{
		IDLE,
		PLAYING
	}

	public class _UnitRendererCreator : FrameAccessor
	{
		public delegate void OnClickDelegate(int unitIndex, int skillIndex);

		public UnitRenderer[] renderers;

		public Transform lhsTroopAnchor;

		public Transform rhsTroopAnchor;

		public UIBattleMain ui;

		public int createdLhsTroopIndex;

		public int createdRhsTroopIndex;

		public Vector3 createUnitPosition;

		public UnitCache unitRenderCache;

		public CacheWithPool etcEffectCache;

		public M04_Tutorial battle;

		public OnClickDelegate onClick;

		public static _UnitRendererCreator Create(M04_Tutorial battle)
		{
			_UnitRendererCreator unitRendererCreator = new _UnitRendererCreator();
			unitRendererCreator.battle = battle;
			unitRendererCreator.renderers = battle._unitRenderers;
			unitRendererCreator.lhsTroopAnchor = battle.lhsTroopAnchor;
			unitRendererCreator.rhsTroopAnchor = battle.rhsTroopAnchor;
			unitRendererCreator.ui = battle.ui;
			unitRendererCreator.createdLhsTroopIndex = -1;
			unitRendererCreator.createdRhsTroopIndex = -1;
			unitRendererCreator.unitRenderCache = CacheManager.instance.UnitCache;
			unitRendererCreator.etcEffectCache = CacheManager.instance.EtcEffectCache;
			unitRendererCreator.createUnitPosition = new Vector3(0f, 0f, 9999f);
			return unitRendererCreator;
		}

		public override bool OnFrameAccessStart()
		{
			createdLhsTroopIndex = -1;
			createdRhsTroopIndex = -1;
			if (base.simulator.isLhsAnnihilated)
			{
				Manager<UIBattleUnitController>.GetInstance().Clean();
			}
			return true;
		}

		public override bool OnUnitAccessStart()
		{
			int num = base.unitIndex;
			if (!base.frame.IsUnitInBattle(num))
			{
				return false;
			}
			if (battle.lastUserUnit == null && base.unit.side == EBattleSide.Left)
			{
				battle.lastUserUnit = base.unit;
			}
			UnitRenderer unitRenderer = renderers[num];
			if (unitRenderer != null)
			{
				return false;
			}
			if (base.unit.isDead)
			{
				return false;
			}
			int lhsTroopIndex = base.simulator.GetLhsTroopIndex(num);
			int rhsTroopIndex = base.simulator.GetRhsTroopIndex(num);
			Transform transform = null;
			SplitScreenDrawSide splitScreenDrawSide = SplitScreenDrawSide.Unknown;
			if (lhsTroopIndex >= 0)
			{
				createdLhsTroopIndex = lhsTroopIndex;
				transform = lhsTroopAnchor;
				splitScreenDrawSide = SplitScreenDrawSide.Left;
			}
			if (rhsTroopIndex >= 0)
			{
				createdRhsTroopIndex = rhsTroopIndex;
				transform = rhsTroopAnchor;
				splitScreenDrawSide = SplitScreenDrawSide.Right;
			}
			int index = num % 9;
			transform = transform.GetChild(index);
			unitRenderer = unitRenderCache.Create(base.unitDr.prefabId, createUnitPosition, Quaternion.identity, transform);
			if (unitRenderer == null)
			{
				return false;
			}
			unitRenderer.Init();
			renderers[num] = unitRenderer;
			unitRenderer.gameObject.name = "Unit-" + num;
			unitRenderer.SetUnit(base.unit);
			unitRenderer.ui = battle.ui.uiBattleUnitPanel.CreateUnitUI();
			if (unitRenderer.ui != null)
			{
				unitRenderer.ui.transform.position = SplitScreenManager.instance.ConvertPosCutToUI(splitScreenDrawSide, unitRenderer.transform.position);
				if (rhsTroopIndex >= 0 && base.simulator.initState.battleType == EBattleType.Raid)
				{
					unitRenderer.ui.gameObject.SetActive(value: false);
				}
				unitRenderer.ui.SetUnit(base.unit, base.unitIndex);
				unitRenderer.ui._SkillClick = delegate(int unitIdx, int skillIdx)
				{
					if (onClick != null)
					{
						onClick(unitIdx, skillIdx);
					}
				};
				unitRenderer.ui.gameObject.name = unitRenderer.gameObject.name;
			}
			if (splitScreenDrawSide == SplitScreenDrawSide.Left)
			{
				unitRenderer.uiCommander = Manager<UIBattleUnitController>.GetInstance().Create(base.unit);
			}
			unitRenderer.drawSide = splitScreenDrawSide;
			if (unitRenderer.selectedMark == null)
			{
				GameObject gameObject = CacheManager.instance.UiCache.Create("SelectMark", transform);
				gameObject.transform.parent = unitRenderer.transform;
				gameObject.transform.localPosition = Vector3.zero;
				BattleUnitSelectMark component = gameObject.GetComponent<BattleUnitSelectMark>();
				unitRenderer.selectedMark = component;
			}
			unitRenderer.selectedMark.SetTurnUnit(active: false);
			unitRenderer.selectedMark.SetAttackTargetCadidate(active: false);
			unitRenderer.selectedMark.SetAttackTarget(active: false);
			return false;
		}

		private UIBattleUnit _CreateUnitUi(GameObject parent, GameObject prefab)
		{
			GameObject gameObject = NGUITools.AddChild(parent, prefab);
			gameObject.SetActive(value: true);
			return gameObject.GetComponent<UIBattleUnit>();
		}
	}

	public class _ProjectileRendererCreator : FrameAccessor
	{
		public UnitRenderer[] unitRenderers;

		public Shared.Battle.Random random;

		protected M04_Tutorial battle;

		protected CacheWithPool rendererCache;

		protected bool bAlive;

		protected EActionEffWithFireType actionEffWithFireType;

		public static _ProjectileRendererCreator Create(M04_Tutorial battle)
		{
			_ProjectileRendererCreator projectileRendererCreator = new _ProjectileRendererCreator();
			projectileRendererCreator.battle = battle;
			projectileRendererCreator.rendererCache = CacheManager.instance.ControllerCache;
			projectileRendererCreator.unitRenderers = battle._unitRenderers;
			return projectileRendererCreator;
		}

		public override bool OnFrameAccessStart()
		{
			random = new Shared.Battle.Random(base.frame.randomSeed);
			return true;
		}

		public override bool OnSkillAccessStart()
		{
			actionEffWithFireType = EActionEffWithFireType.None;
			if (GameSetting.instance.effect && base.skill.IsCutInSkill && base.skill.fireActionDri >= 0)
			{
				actionEffWithFireType = base.skillDr.actionEffWithFireType;
			}
			return true;
		}

		public override void OnProjectileAccessStart()
		{
			bAlive = false;
			if (!base.frame.IsUnitInBattle(base.unitIndex))
			{
				return;
			}
			bAlive = true;
			if (base.projectile.elapsedTime != 0)
			{
				return;
			}
			bool shouldRenderSplash = base.projectileDr.shouldRenderSplash;
			if (base.projectile.isSplash && !shouldRenderSplash)
			{
				return;
			}
			UnitRenderer unitRenderer = unitRenderers[base.unitIndex];
			int fireEventIndex = base.projectile.fireEventIndex;
			FireEvent fireEvent = base.unitMotionDr.fireEvents[fireEventIndex];
			string firePointBonePath = fireEvent.firePointBonePath;
			Transform bone = unitRenderer.GetBone(firePointBonePath);
			UnitRenderer unitRenderer2 = unitRenderers[base.projectile.targetIndex];
			if (unitRenderer2 == null)
			{
				return;
			}
			Vector3 position = unitRenderer2.transform.position;
			Vector3 zero = Vector3.zero;
			if (base.isMissedProjectile)
			{
				float num = 1.5f + 0.5f * (0.01f * (float)random.Next(0, 100));
				float f = (float)random.Next(0, 360) * ((float)Math.PI / 180f);
				zero.x = num * Mathf.Cos(f);
				zero.y = num * Mathf.Sin(f);
			}
			zero.y = 0f;
			position += zero;
			Unit unit = base.frame.units[unitRenderer2.unitIdx];
			if (base.unit.side == EBattleSide.Right && unit.side == EBattleSide.Left)
			{
				battle.ui.SetUICommander(unit);
			}
			ProjectileController projectileController = rendererCache.Create<ProjectileController>("ProjectileController");
			if (!(projectileController != null))
			{
				return;
			}
			if (actionEffWithFireType == EActionEffWithFireType.None && !base.projectile.isSplash && !string.IsNullOrEmpty(base.skillDr.fireSound) && base.skillDr.fireSound != "0")
			{
				SoundManager.PlaySFX(base.skillDr.fireSound);
			}
			if (base.simulator.regulation.projectileMotionPhaseDtbl.length != battle._projectileCache.elements.Count)
			{
				projectileController.name = base.projectile.id.ToString();
				ProjectileMotionPhase projectileMotionPhase = projectileController.Create(base.projectile.fireKey, bone.position);
				if (projectileMotionPhase != null)
				{
					projectileMotionPhase.Set(unitRenderer, unitRenderer);
					projectileMotionPhase.SetEventDealy(base.projectileHitDelayTime);
				}
				ProjectileMotionPhase projectileMotionPhase2 = projectileController.Create(base.projectile.hitKey, position);
				if (projectileMotionPhase2 != null)
				{
					projectileMotionPhase2.Set(unitRenderer, unitRenderer2);
				}
			}
			else
			{
				projectileController.name = base.projectile.id.ToString();
				ProjectileMotionPhase projectileMotionPhase3 = projectileController.Create(base.projectile.id / 100000, bone.position);
				if (projectileMotionPhase3 != null)
				{
					projectileMotionPhase3.Set(unitRenderer, unitRenderer);
					projectileMotionPhase3.SetEventDealy(base.projectileHitDelayTime);
				}
				ProjectileMotionPhase projectileMotionPhase4 = projectileController.Create(base.projectile.id % 100000, position);
				if (projectileMotionPhase4 != null)
				{
					projectileMotionPhase4.Set(unitRenderer, unitRenderer2);
				}
			}
		}

		public override void OnProjectileAccessEnd()
		{
			if (!bAlive)
			{
				return;
			}
			if (!base.projectile.isSplash)
			{
				bool flag = true;
				if (base.skill.FireActionDr != null && base.simulator.CanEnableFireAction(base.unit) && actionEffWithFireType == EActionEffWithFireType.FireAndHit)
				{
					flag = false;
				}
				if (flag && Simulator.HasTimeEvent(base.projectileHitTime - base.skillDr.hitSoundDelay, base.projectile.elapsedTime) && !string.IsNullOrEmpty(base.skillDr.hitSound) && base.skillDr.hitSound != "0")
				{
					SoundManager.PlaySFX(base.skillDr.hitSound);
				}
			}
			if (!Simulator.HasTimeEvent(base.projectileHitTime, base.projectile.elapsedTime))
			{
				return;
			}
			bool shouldRenderSplash = base.projectileDr.shouldRenderSplash;
			if (!base.projectile.isSplash)
			{
				if (base.isMissedProjectile)
				{
					if (!string.IsNullOrEmpty(base.skillDr.beMissSound) && base.skillDr.beMissSound != "0")
					{
						SoundManager.PlaySFX(base.skillDr.beMissSound);
					}
				}
				else if (!string.IsNullOrEmpty(base.skillDr.beHitSound) && base.skillDr.beHitSound != "0")
				{
					SoundManager.PlaySFX(base.skillDr.beHitSound);
				}
			}
			if (base.projectile._beHitId < 0)
			{
				return;
			}
			UnitRenderer unitRenderer = unitRenderers[base.projectile.targetIndex];
			if (!(unitRenderer != null))
			{
				return;
			}
			ProjectileController projectileController = rendererCache.Create<ProjectileController>("ProjectileController");
			if (!(projectileController != null))
			{
				return;
			}
			UnitRenderer actor = unitRenderers[base.unitIndex];
			projectileController.name = base.projectile.id.ToString();
			if (base.simulator.regulation.projectileMotionPhaseDtbl.length != battle._projectileCache.elements.Count)
			{
				ProjectileMotionPhase projectileMotionPhase = projectileController.Create(base.projectile.beHitKey, unitRenderers[base.projectile.targetIndex].transform.position);
				if (projectileMotionPhase != null)
				{
					projectileMotionPhase.Set(actor, unitRenderer);
				}
			}
			else
			{
				ProjectileMotionPhase projectileMotionPhase2 = projectileController.Create(base.projectile._beHitId, unitRenderers[base.projectile.targetIndex].transform.position);
				if (projectileMotionPhase2 != null)
				{
					projectileMotionPhase2.Set(actor, unitRenderer);
				}
			}
		}
	}

	public class _UnitRendererUpdater : FrameAccessor
	{
		public UIBattleMain ui;

		public SplitScreenManager splitScreenManager;

		public CutInEffect cutInEffect;

		public UnitRenderer[] renderers;

		public bool pause;

		protected M04_Tutorial battle;

		public float delay;

		public static _UnitRendererUpdater Create(M04_Tutorial battle)
		{
			_UnitRendererUpdater unitRendererUpdater = new _UnitRendererUpdater();
			unitRendererUpdater.ui = battle.ui;
			unitRendererUpdater.splitScreenManager = battle._splitScreenManager;
			unitRendererUpdater.renderers = battle._unitRenderers;
			unitRendererUpdater.battle = battle;
			return unitRendererUpdater;
		}

		public override bool OnFrameAccessStart()
		{
			pause = false;
			return true;
		}

		public override bool OnUnitAccessStart()
		{
			int num = base.unitIndex;
			SplitScreenManager splitScreenManager = this.splitScreenManager;
			UnitRenderer unitRenderer = renderers[num];
			if (unitRenderer == null)
			{
				return false;
			}
			if (base.unit.takenRevival)
			{
				unitRenderer.Revival("idle");
			}
			if (base.unit.isDead)
			{
				if (base.unit._onDead && !unitRenderer.IsDead)
				{
					battle.ui.UICommanderState(base.unit, "Dead");
					unitRenderer.Dead("destroy");
					if (base.unit.dropGold > 0)
					{
						UIDropItem dropItem2 = CacheManager.instance.UiCache.Create<UIDropItem>("UIDropItem", UIRoot.list[0].transform);
						if (dropItem2 != null)
						{
							dropItem2.SetType(UIDropItem.EType.Gold);
							dropItem2.value = base.unit.dropGold;
							dropItem2.target = UIManager.instance.battle.MainUI.main.uiGetGoldView.root.transform;
							dropItem2.transform.position = splitScreenManager.ConvertPosCutToUI(SplitScreenDrawSide.Right, unitRenderer.transform.position);
							dropItem2._End = delegate
							{
								ui.TakenGold(dropItem2.value);
							};
						}
					}
					if (base.unit.dropItemCnt > 0)
					{
						for (int i = 0; i < base.unit.dropItemCnt; i++)
						{
							UIDropItem dropItem = CacheManager.instance.UiCache.Create<UIDropItem>("UIDropItem", UIRoot.list[0].transform);
							if (dropItem != null)
							{
								dropItem.SetType(UIDropItem.EType.Item);
								dropItem.value = 1;
								dropItem.target = UIManager.instance.battle.MainUI.main.uiGetBoxView.root.transform;
								dropItem.transform.position = splitScreenManager.ConvertPosCutToUI(SplitScreenDrawSide.Right, unitRenderer.transform.position);
								dropItem._End = delegate
								{
									ui.TakenItem(dropItem.value);
								};
							}
						}
					}
					unitRenderer.CleanStatus();
					if (unitRenderer.uiCommander != null)
					{
						unitRenderer.uiCommander.UpdateUI();
					}
				}
			}
			else
			{
				if (base.frame.turnUnitIndex == base.unitIndex && base.simulator.initState.battleType == EBattleType.Raid && base.frame.IsRhsUnitInBattle(base.unitIndex))
				{
					bool flag = false;
					if (true && base.frame.onTurn)
					{
						string key = $"{base.simulator.initState.raidData._raidId}_{base.unit._turn}";
						if (base.simulator.regulation.raidDtbl.ContainsKey(key))
						{
							RaidDataRow raidDataRow = base.simulator.regulation.raidDtbl[key];
							if (raidDataRow.effectName != "-")
							{
								CacheManager.instance.EtcEffectCache.Create(raidDataRow.effectName, unitRenderer.transform.position, Quaternion.identity);
							}
						}
					}
				}
				Dictionary<int, Status>.Enumerator statusItr = base.unit.StatusItr;
				while (statusItr.MoveNext())
				{
					Status value = statusItr.Current.Value;
					if (value.ElapsedTimeTick == 0)
					{
						unitRenderer.AddStatus(value);
					}
					if (!value.IsAlive)
					{
						unitRenderer.RemoveStatus(value);
					}
				}
				if (unitRenderer.ui != null)
				{
					unitRenderer.ui.UpdateSkillAmount();
					unitRenderer.ui.SetAnimateHp(base.unit.maxHealth, base.unit.health);
				}
				if (unitRenderer.uiCommander != null)
				{
					unitRenderer.uiCommander.UpdateUI();
				}
				Vector3 position = unitRenderer.transform.position;
				position += UnityEngine.Random.insideUnitSphere * 0.5f;
				position = splitScreenManager.ConvertPosCutToUI(unitRenderer.drawSide, position);
				int num2 = base.unit.health;
				if (base.unit.takenDamage > 0)
				{
					num2 = (int)Math.Max(num2 - base.unit.takenDamage, 0L);
					if (!base.unit.isPlayingAction)
					{
						unitRenderer.PlayAnimation("behit");
					}
					battle.ui.UICommanderState(base.unit, "Behit");
					float num3 = (float)num2 / (float)base.unit.maxHealth;
					unitRenderer.SetInjury(num3 <= 0.3f);
					ui.damageBubble.Add(position, base.unit.takenDamage, Color.white);
					if (base.unit.side == EBattleSide.Left)
					{
						battle.lastUserUnit = base.unit;
						ui.UICommanderShake(base.unit);
					}
				}
				if (base.unit.takenCriticalDamage > 0)
				{
					num2 = (int)Math.Max(num2 - base.unit.takenCriticalDamage, 0L);
					if (!base.unit.isPlayingAction)
					{
						unitRenderer.PlayAnimation("behit");
					}
					battle.ui.UICommanderState(base.unit, "Behit");
					float num4 = (float)num2 / (float)base.unit.maxHealth;
					unitRenderer.SetInjury(num4 <= 0.3f);
					ui.criticalDamageBubble.Add(position, base.unit.takenCriticalDamage, Color.red);
					if (ui.criticalBubble != null)
					{
						ui.criticalBubble.Add(position);
					}
					if (base.unit.side == EBattleSide.Left)
					{
						battle.lastUserUnit = base.unit;
						ui.UICommanderShake(base.unit);
						UIManager.instance.battle.LeftView.cameraShake.Begin();
					}
					else
					{
						if (ui.slideCritical != null)
						{
							ui.slideCritical.Add(ui.slideCritical.transform.position);
						}
						UIManager.instance.battle.RightView.cameraShake.Begin();
					}
				}
				if (base.unit.takenHealing > 0)
				{
					num2 = (int)Math.Min(num2 + base.unit.takenHealing, base.unit.maxHealth);
					float num5 = (float)num2 / (float)base.unit.maxHealth;
					unitRenderer.SetInjury(num5 <= 0.3f);
					ui.damageBubble.Add(position, base.unit.takenHealing, Color.green);
				}
				if (base.unit._takenCriticalHealing > 0)
				{
					num2 = (int)Math.Min(num2 + base.unit._takenCriticalHealing, base.unit.maxHealth);
					float num6 = (float)num2 / (float)base.unit.maxHealth;
					unitRenderer.SetInjury(num6 <= 0.3f);
					ui.criticalDamageBubble.Add(position, base.unit._takenCriticalHealing, Color.green);
				}
				if (base.unit.avoidanceCount > 0)
				{
					if (!base.unit.isPlayingAction)
					{
						unitRenderer.PlayAnimation("avoid");
					}
					ui.missBubble.Add(position);
				}
				if (base.simulator.option.enableFatalCut && ui.fatalCut.CanFatalCut(base.unit))
				{
					ui.fatalCut.Show(base.unit);
				}
			}
			return true;
		}

		public override bool OnSkillAccessStart()
		{
			UnitRenderer renderer = renderers[base.unitIndex];
			if (renderer == null)
			{
				return false;
			}
			if (renderer.ui != null && base.skill.isActiveSkill && base.unit._cls >= base.skill.SkillDataRow.openGrade)
			{
				float max = base.skillDr.maxSp;
				float curr = base.skill.sp;
				renderer.ui.SetAnimateSkill(max, curr);
				renderer.ui.OnActivedSkill(base.skill.CanUse);
			}
			int num = base.unitMotionDr.playTime;
			FireEvent fireEvent = base.unitMotionDr.fireEvents[0];
			bool flag = false;
			bool hasReturnMotion = base.skill.returnMotionDri >= 0;
			if (base.skill.IsCutInSkill && !base.unit.isEnableEventSkill && fireEvent != null)
			{
				num -= fireEvent.time;
				num += 66;
				flag = true;
			}
			if (base.skill.remainedMotionTime > 0 && !base.skill.IsCutInSkill && !renderer.IsDead)
			{
				int elapsedTime = base.unitMotionDr.playTime - base.skill.remainedMotionTime;
				if (Simulator.HasTimeEvent(base.skillDr.actionSoundDelay, elapsedTime) && !string.IsNullOrEmpty(base.skillDr.actionSound) && base.skillDr.actionSound != "0")
				{
					SoundManager.PlaySFX(base.skillDr.actionSound);
				}
			}
			bool flag2 = false;
			int num2 = base.skill.remainedMotionTime + 66;
			for (int i = 0; i < base.unitMotionDr.atkSwitchEvents.Count; i++)
			{
				flag2 = true;
				AttackSwitchEvent attackSwitchEvent = base.unitMotionDr.atkSwitchEvents[i];
				if (attackSwitchEvent == null)
				{
					break;
				}
				int num3 = base.unitMotionDr.playTime - attackSwitchEvent.time;
				if (num3 >= num2 || num3 < base.skill.remainedMotionTime)
				{
					continue;
				}
				switch (attackSwitchEvent.eType)
				{
				case AttackSwitchEvent.E_SWITCH_TYPE.TARGET:
				{
					Transform transform = renderer.transform.Find("root_node/_TP");
					if (transform != null)
					{
						renderer.transform.localRotation = new Quaternion(0f, 180f, 0f, 0f);
						renderer.transform.position = renderer.transform.position + (renderers[base.skill._targetIndex].transform.position - transform.position);
					}
					break;
				}
				case AttackSwitchEvent.E_SWITCH_TYPE.LOCAL:
					renderer.transform.localPosition = new Vector3(0f, 0f, 0f);
					renderer.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
					break;
				}
			}
			if (num != base.skill.remainedMotionTime)
			{
				if (hasReturnMotion && base.skill.remainedReturnMotionTime > 0)
				{
					UnitMotionDataRow unitMotionDataRow = base.simulator.regulation.unitMotionDtbl[base.skill.returnMotionDri];
					int playTime = unitMotionDataRow.playTime;
					if (base.skill.remainedReturnMotionTime == playTime)
					{
						string returnMotionDrk = base.skillDr.returnMotionDrk;
						returnMotionDrk = returnMotionDrk.Substring(returnMotionDrk.LastIndexOf("/") + 1);
						renderer.StartCoroutine(_PlayUnitMotion(renderer, returnMotionDrk, 0f));
					}
				}
				return false;
			}
			if (base.unit.side == EBattleSide.Left)
			{
				battle.lastUserUnit = base.unit;
				if (base.unit._cdri >= 0)
				{
					CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[base.unit._cdri];
					if (commanderDataRow != null)
					{
						if (base.skill.isActiveSkill)
						{
							SoundManager.PlayVoiceEvent(commanderDataRow, ECommanderVoiceEventType.ActiveSkill, looping: false, 0f, float.MaxValue, 1f);
						}
						else if (base.skillIndex == 2)
						{
							SoundManager.PlayVoiceEvent(commanderDataRow, ECommanderVoiceEventType.Passive1, looping: false, 0f, float.MaxValue, 1f);
						}
						else if (base.skillIndex == 3)
						{
							SoundManager.PlayVoiceEvent(commanderDataRow, ECommanderVoiceEventType.Passive2, looping: false, 0f, float.MaxValue, 1f);
						}
					}
				}
			}
			if (base.unit.isEnableEventSkill)
			{
				Vector3 position = renderer.transform.position;
				position += UnityEngine.Random.insideUnitSphere * 0.5f;
				position = splitScreenManager.ConvertPosCutToUI(renderer.drawSide, position);
				switch (base.unit.eventSkillType)
				{
				case EventSkillType.OnBeHit:
					ui.counterBubble.Add(position);
					if (base.unit.side == EBattleSide.Left && ui.slideCounter != null)
					{
						ui.slideCounter.Add(ui.slideCounter.transform.position);
					}
					break;
				case EventSkillType.OnCombo:
					ui.comboBubble.Add(position);
					if (base.unit.side == EBattleSide.Left && ui.slideCombo != null)
					{
						ui.slideCombo.Add(ui.slideCombo.transform.position);
					}
					break;
				}
			}
			if (flag)
			{
				FireEffect fireEffect = CacheManager.instance.FireEffectCache.Create<FireEffect>(base.skill._skillDataRow.cutInEffectId);
				if (fireEffect != null)
				{
					pause = true;
					fireEffect.canInterfereSkill = base.simulator.option.canInterfereSkill;
					fireEffect.actionEffSound = base.skillDr.actionEffSound;
					fireEffect.actionEffWithFireType = base.skillDr.actionEffWithFireType;
					fireEffect.actionSound = base.skillDr.actionSound;
					fireEffect.actionSoundDelay = base.skillDr.actionSoundDelay;
					fireEffect.fireEvent = base.unitMotionDr.fireEvents[0];
					fireEffect.Set(renderer, renderers[base.skill._targetIndex], base.simulator.CanEnableFireAction(base.unit));
					string motionName = base.skillDr.unitMotionDrk;
					motionName = motionName.Substring(motionName.LastIndexOf("/") + 1);
					fireEffect.OnFire = delegate
					{
						pause = false;
						if (!renderer.IsDead)
						{
							renderer.StartCoroutine(_PlayUnitMotion(renderer, motionName, 0f, hasReturnMotion));
						}
					};
				}
				else
				{
					if (base.skillIndex > 0)
					{
						E_TARGET_SIDE side = E_TARGET_SIDE.LEFT;
						if (base.frame.IsRhsUnitInBattle(base.unitIndex))
						{
							side = E_TARGET_SIDE.RIGHT;
						}
						CutInDataSet cutInDataSet = Manager<CutInController>.GetInstance().Create(base.unit, base.skill, side, renderer, renderers[base.skill._targetIndex], fireEvent);
						cutInDataSet.commanderDri = base.unit._cdri;
					}
					if (!renderer.IsDead)
					{
						if (!string.IsNullOrEmpty(base.skillDr.actionSound) && base.skillDr.actionSound != "0")
						{
							SoundManager.PlaySFX(base.skillDr.actionSound);
						}
						string unitMotionDrk = base.skillDr.unitMotionDrk;
						unitMotionDrk = unitMotionDrk.Substring(unitMotionDrk.LastIndexOf("/") + 1);
						renderer.StartCoroutine(_PlayUnitMotion(renderer, unitMotionDrk, 0f, hasReturnMotion));
					}
				}
			}
			else if (!renderer.IsDead)
			{
				string unitMotionDrk2 = base.skillDr.unitMotionDrk;
				unitMotionDrk2 = unitMotionDrk2.Substring(unitMotionDrk2.LastIndexOf("/") + 1);
				renderer.StartCoroutine(_PlayUnitMotion(renderer, unitMotionDrk2, 0f, hasReturnMotion));
			}
			return false;
		}

		private IEnumerator _PlayUnitMotion(UnitRenderer renderer, string motionName, float delay, bool hasReturnMotion = false)
		{
			if (delay > 0f)
			{
				yield return new WaitForSeconds(delay);
			}
			renderer.PlayAnimation(motionName, hasReturnMotion);
		}
	}

	private class _MainUiUpdater : FrameAccessor
	{
	}

	public class _SkillIconUpdater : FrameAccessor
	{
		public UIBattleMain ui;

		public UnitRenderer[] _unitRenderers;

		public M04_Tutorial battle;

		public static _SkillIconUpdater Create(M04_Tutorial battle)
		{
			_SkillIconUpdater skillIconUpdater = new _SkillIconUpdater();
			skillIconUpdater.ui = battle.ui;
			skillIconUpdater.battle = battle;
			return skillIconUpdater;
		}

		public override bool OnFrameAccessStart()
		{
			if (!base.frame.CanUseSkill(base.simulator.option))
			{
				return false;
			}
			battle.waitingInput = false;
			return true;
		}

		public override bool OnUnitAccessStart()
		{
			if (battle.waitingInput)
			{
				return false;
			}
			if (!base.frame.IsUnitInBattle(base.unitIndex))
			{
				return false;
			}
			if (base.unit.side != 0)
			{
				return false;
			}
			if (base.unit.isDead)
			{
				return false;
			}
			if (!base.unit.hasActiveSkill)
			{
				return false;
			}
			if (!base.simulator.CanSkillAction(base.unit, base.unit._activeSkillIdx))
			{
				return false;
			}
			int num = base.frame.FindSkillTarget(base.simulator.regulation, base.unit._unitIdx, base.unit._activeSkillIdx);
			if (num < 0)
			{
				return false;
			}
			UIBattleUnitControllerItem uIBattleUnitControllerItem = Manager<UIBattleUnitController>.GetInstance().FindItem(base.unitIndex);
			if (uIBattleUnitControllerItem != null)
			{
				battle.waitingInput = true;
				battle.clickIcon.transform.position = uIBattleUnitControllerItem.transform.position;
				battle.clickIcon.gameObject.SetActive(value: true);
				battle.clickIcon.Set(base.unitIndex, base.skillIndex);
				battle.Pause();
			}
			return false;
		}
	}

	public class _TurnUiUpdater : FrameAccessor
	{
		public M04_Tutorial battle;

		public UIBattleMain ui;

		public UnitRenderer[] renderers;

		public Shared.Battle.Input input;

		public static _TurnUiUpdater Create(M04_Tutorial battle)
		{
			_TurnUiUpdater turnUiUpdater = new _TurnUiUpdater();
			turnUiUpdater.ui = battle.ui;
			turnUiUpdater.battle = battle;
			return turnUiUpdater;
		}

		public override bool OnFrameAccessStart()
		{
			if (base.frame.rhsOnWave)
			{
				ui.SetRhsCurrWave(base.frame._rhsWave);
				battle.StartOpening(base.frame._rhsWave);
			}
			if (base.frame.onWaveTurn)
			{
				ui.SetUITurn(base.frame._waveTurn);
				if (base.simulator.initState.battleType != EBattleType.Raid)
				{
					ui.SetClearRank(3);
				}
			}
			if (base.frame.onTurn)
			{
				for (int i = 0; i < base.frame.units.Count; i++)
				{
					UnitRenderer unitRenderer = renderers[i];
					if (unitRenderer == null)
					{
						continue;
					}
					if (i == base.frame.turnUnitIndex)
					{
						Unit unit = base.frame.units[i];
						if (!unit.isStatusStun && !unit.isDead)
						{
							if (unitRenderer.ui != null)
							{
								unitRenderer.ui.SetTurn(0);
							}
							unitRenderer.SetTurnUnit(isTurnUnit: true);
							if (unit.side == EBattleSide.Left)
							{
								ui.SetUICommander(unit);
							}
						}
					}
					else
					{
						unitRenderer.SetTurnUnit(isTurnUnit: false);
					}
					unitRenderer.SetAttackTargetCandidate(isTarget: false);
					unitRenderer.SetAttackTarget(isTarget: false);
				}
			}
			if (!base.frame.isWaitingInput)
			{
				return true;
			}
			return false;
		}

		public override bool OnSkillAccessStart()
		{
			if (base.unitIndex != base.frame.turnUnitIndex)
			{
				return false;
			}
			if (base.skill.remainedMotionTime == 0)
			{
				return false;
			}
			UnitRenderer unitRenderer = renderers[base.skill.targetIndex];
			if (unitRenderer != null)
			{
				unitRenderer.SetAttackTarget(isTarget: true);
			}
			return true;
		}

		public override void OnProjectileAccessStart()
		{
			UnitRenderer unitRenderer = renderers[base.projectile.targetIndex];
			if (unitRenderer != null)
			{
				unitRenderer.SetAttackTarget(isTarget: true);
			}
		}
	}

	public class _InputCorrector : FrameAccessor
	{
		public Shared.Battle.Input input;

		public bool isAuto;

		public static _InputCorrector Create(M04_Tutorial battle)
		{
			return new _InputCorrector();
		}

		public override bool OnFrameAccessStart()
		{
			if (!base.frame.CanUseSkill(base.simulator.option))
			{
				input = null;
				return false;
			}
			if (input == null)
			{
				if (isAuto)
				{
					if (base.simulator.option.immediatelyUseActiveSkill)
					{
						for (int i = 0; i < 9; i++)
						{
							Unit unit = base.frame.units[i + base.frame.lhsTroopStartIndex];
							if (unit != null && !unit.isDead && unit.hasActiveSkill && base.simulator.CanSkillAction(unit, unit._activeSkillIdx))
							{
								int num = base.frame.FindSkillTarget(base.simulator.regulation, unit._unitIdx, unit._activeSkillIdx);
								if (num >= 0)
								{
									input = new Shared.Battle.Input(unit._unitIdx, unit._activeSkillIdx, -1);
									break;
								}
							}
						}
					}
					else if (base.frame.isWaitingLhsInput)
					{
						Unit unit = base.frame.units[base.frame.turnUnitIndex];
						if (unit != null && unit.hasActiveSkill && base.simulator.CanSkillAction(unit, unit._activeSkillIdx))
						{
							int num2 = base.frame.FindSkillTarget(base.simulator.regulation, unit._unitIdx, unit._activeSkillIdx);
							if (num2 >= 0)
							{
								input = new Shared.Battle.Input(unit._unitIdx, unit._activeSkillIdx, -1);
							}
						}
					}
				}
			}
			else
			{
				Unit unit = base.frame.units[input.unitIndex];
				if (unit != null && base.simulator.CanUnitControl(unit) && unit.hasActiveSkill && base.simulator.CanSkillAction(unit, unit._activeSkillIdx))
				{
					int num3 = base.frame.FindSkillTarget(base.simulator.regulation, unit._unitIdx, unit._activeSkillIdx);
					if (num3 >= 0)
					{
						return false;
					}
				}
				input = null;
			}
			return false;
		}
	}

	public bool waitingInput;

	public UISkill clickIcon;

	public E_CUTIN_STATE eCutInState;

	public State _state;

	public StepActor tutorialActor;

	public _UnitRendererCreator _unitRendererCreator;

	public _ProjectileRendererCreator _projectileRendererCreator;

	public _UnitRendererUpdater _unitRendererUpdater;

	public _SkillIconUpdater _skillIconUpdater;

	public _TurnUiUpdater _turnUiUpdater;

	public _InputCorrector _inputCorrector;

	private EDialogueState _dialogueState;

	[HideInInspector]
	public Unit lastUserUnit;

	public UIAtlas commanderAtlas;

	public UIAtlas commanderAtlas_2;

	public UIAtlas battleCommanderUnitAtlas;

	public UIAtlas iconAtlas;

	private bool _isInit;

	public static string KEY => "hH7=";

	public override Simulator Simulator
	{
		get
		{
			return _simulator;
		}
		set
		{
			_simulator = value;
		}
	}

	public override BattleData BattleData
	{
		get
		{
			return _battleData;
		}
		set
		{
			_battleData = value;
		}
	}

	public override UnitRenderer[] UnitRenderers
	{
		get
		{
			return _unitRenderers;
		}
		set
		{
			_unitRenderers = value;
		}
	}

	public IEnumerator InitUI(BattleData bd)
	{
		ui = UIManager.instance.battle.MainUI;
		if (bd != null)
		{
			ui.Set(bd);
		}
		else
		{
			ui.Set(RoUser.CreateDummyUser("AA", "BABO", UnityEngine.Random.Range(1, 20).ToString()), RoUser.CreateDummyUser("BB", "BOBA", UnityEngine.Random.Range(1, 20).ToString()));
		}
		ui.Open();
		yield return null;
	}

	private void Start()
	{
		AdjustManager.Instance.SimpleEvent("k0ekr6");
		StartCoroutine(BattleStart());
	}

	private void OnDestroy()
	{
		AdjustManager.Instance.SimpleEvent("r5kicj");
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			Manager<UIOptionController>.GetInstance().Show();
		}
	}

	private IEnumerator BattleStart()
	{
		GameObject loading = GameObject.Find("Loading");
		if (loading != null)
		{
			loading.transform.parent = GameObject.Find("UI Root").transform;
			loading.transform.localPosition = Vector3.zero;
			yield return null;
		}
		UIFade.Out(0f);
		yield return null;
		_HideTroopAnchor(base.lhsTroopAnchor);
		_HideTroopAnchor(base.rhsTroopAnchor);
		if (!AssetBundleManager.HasAssetBundle("CommanderAtlas.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("CommanderAtlas.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("CommanderAtlas_2.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("CommanderAtlas_2.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("BattleCommanderUnit.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("BattleCommanderUnit.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("Icon.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("Icon.assetbundle"));
		}
		if (AssetBundleManager.HasAssetBundle("CommanderAtlas.assetbundle"))
		{
			commanderAtlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("CommanderAtlas.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			commanderAtlas.spriteMaterial = Resources.Load<Material>("Atlas/CommanderAtlas");
		}
		if (AssetBundleManager.HasAssetBundle("CommanderAtlas_2.assetbundle"))
		{
			commanderAtlas_2.replacement = AssetBundleManager.GetObjectFromAssetBundle("CommanderAtlas_2.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			commanderAtlas_2.spriteMaterial = Resources.Load<Material>("Atlas/CommanderAtlas_2");
		}
		if (AssetBundleManager.HasAssetBundle("BattleCommanderUnit.assetbundle"))
		{
			battleCommanderUnitAtlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("BattleCommanderUnit.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			battleCommanderUnitAtlas.spriteMaterial = Resources.Load<Material>("Atlas/BattleCommanderUnit");
		}
		if (AssetBundleManager.HasAssetBundle("Icon.assetbundle"))
		{
			iconAtlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("Icon.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			iconAtlas.spriteMaterial = Resources.Load<Material>("Atlas/Icon");
		}
		CacheManager.instance.SoundPocketCache.Create("Pocket_BGM_Stage_001");
		if (RemoteObjectManager.instance.regulation.fireActionDtbl == null)
		{
			string json = Utility.ReadTextAsset("Regulation/FireActionDataTable");
			RemoteObjectManager.instance.regulation.SetFromLocalResources("fireAction", json);
		}
		_isAuto = false;
		_isPause = false;
		_timeScale = defaultTimeScale;
		Time.timeScale = defaultTimeScale;
		_priorityTargetIndex = -1;
		_state = State.Unknown;
		_input = null;
		_timeStack = 0;
		_timeGameObject = GetComponent<TimedGameObject>();
		_splitScreenManager = UnityEngine.Object.FindObjectOfType<SplitScreenManager>();
		_controllerCache = CacheManager.instance.ControllerCache;
		_projectileCache = CacheManager.instance.ProjectileCache;
		_unitCache = CacheManager.instance.UnitCache;
		_terrainCache = CacheManager.instance.TerrainCache;
		_statusEffectCache = CacheManager.instance.StatusEffectCache;
		_fireEffectCache = CacheManager.instance.FireEffectCache;
		_etcEffectCache = CacheManager.instance.EtcEffectCache;
		_cutInEffectCache = CacheManager.instance.CutInEffectCache;
		yield return StartCoroutine(UIManager.instance.battle.InitCoroutine());
		if (CacheItemPocket.instance != null)
		{
			UnityEngine.Object.Destroy(CacheItemPocket.instance.gameObject);
		}
		Manager<UIBattleUnitController>.GetInstance()._Click = OnSelect;
		Manager<UIOptionController>.GetInstance().InitUI();
		Manager<UIOptionController>.GetInstance().opt_sound = GameSetting.instance.se;
		Manager<UIOptionController>.GetInstance().opt_skill = true;
		Manager<UIOptionController>.GetInstance().opt_bgm = GameSetting.instance.bgm;
		Manager<UIOptionController>.GetInstance().onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			M04_Tutorial m04_Tutorial = this;
			if (text != null && text == "btn_end")
			{
				UISimplePopup popup = UISimplePopup.CreateBool(localization: false, "전투종료", "월드맵으로 이동하시면 현재 전투에서 패배하게 됩니다.", "정말 이동하시겠습니까?", "예", "아니오");
				popup.onClick = delegate(GameObject obj)
				{
					if (obj.name == "OK")
					{
						popup.ClosePopup();
						popup.Close();
						if (m04_Tutorial._battleData != null)
						{
							m04_Tutorial._battleData.move = EBattleResultMove.MyTown;
						}
						Loading.Load(Loading.WorldMap);
					}
				};
			}
		};
		Manager<UIOptionController>.GetInstance().onShow = delegate
		{
			Time.timeScale = 0f;
		};
		Manager<UIOptionController>.GetInstance().onHide = delegate
		{
			Time.timeScale = defaultTimeScale;
		};
		clickIcon._Click = delegate(int unitIdx, int skillIdx)
		{
			Resume();
			clickIcon.gameObject.SetActive(value: false);
			OnSelect(unitIdx);
			waitingInput = false;
			AdjustManager.Instance.SimpleEvent("ko3zzh");
		};
		if (loading != null)
		{
			loading.GetComponent<UILoading>().Out();
		}
		tutorialActor.gameObject.SetActive(value: true);
		tutorialActor.OnFinish += delegate
		{
			Time.timeScale = 1f;
		};
		_isInit = true;
	}

	protected void ShowdialogueEvent(EBattleDialogueEventType eventType)
	{
		string text = "Chapter";
		int num = 0;
		int num2 = 0;
		string text2 = string.Empty;
		if (_battleData != null)
		{
			num = int.Parse(_battleData.worldId);
			num2 = int.Parse(_battleData.stageId);
			num2 = num2 - (num - 1) * 20 - ConstValue.tutorialMaximumStage;
			text2 = $"{text}-{num:00}-{num2:00}-{(int)eventType:00}";
			if (eventType == EBattleDialogueEventType.WaveEvent)
			{
				int num3 = _simulator.frame._rhsWave - 1;
				if (num3 < 0)
				{
					num3 = 0;
				}
				text2 = $"{text2}-{num3:00}";
			}
		}
		if (!string.IsNullOrEmpty(text2) && ClassicRpgManager.HasDialogue(text2))
		{
			_dialogueState = EDialogueState.Play;
			UISetter.SetActive(UIManager.instance.battle.DialogMrg, active: true);
			UIManager.instance.battle.DialogMrg.InitWorldMapStart(text2);
		}
	}

	public bool DialogueEndCheck()
	{
		switch (_dialogueState)
		{
		case EDialogueState.Play:
			if (!UIManager.instance.battle.DialogMrg.gameObject.activeSelf)
			{
				_dialogueState = EDialogueState.Stoping;
				StartCoroutine("DialogueStop");
			}
			return false;
		case EDialogueState.Stoping:
			return false;
		default:
			return true;
		}
	}

	private IEnumerator DialogueStop()
	{
		yield return new WaitForSeconds(1f);
		_dialogueState = EDialogueState.None;
	}

	public float GetRemainedTime()
	{
		Frame frame = _simulator.frame;
		int timeLimit = _simulator.record.option.timeLimit;
		return (float)(timeLimit - frame.time) / 1000f;
	}

	private string _VerifyBattle(Record record)
	{
		Formatting formatting = Formatting.Indented;
		JsonSerializerSettings serializerSettings = Regulation.SerializerSettings;
		string value = JsonConvert.SerializeObject(record, formatting, serializerSettings);
		Record record2 = JsonConvert.DeserializeObject<Record>(value, serializerSettings);
		Simulator simulator = Simulator.Create(_simulator.regulation, record2);
		while (!simulator.isEnded)
		{
			simulator.Step(null, null);
		}
		return simulator.record.result.checksum;
	}

	private string _VerifyBattleWithProcess(Record record)
	{
		Formatting formatting = Formatting.None;
		JsonSerializerSettings serializerSettings = Regulation.SerializerSettings;
		Process process = new Process();
		process.StartInfo.FileName = Application.dataPath + "/../btsm.exe";
		string text = "./Regulation-1000000.json";
		string text2 = JsonConvert.SerializeObject(record, formatting, serializerSettings);
		process.StartInfo.Arguments = "\"" + text + "\" \"" + text2 + " \"";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.Start();
		StreamReader standardOutput = process.StandardOutput;
		process.WaitForExit();
		string value = standardOutput.ReadToEnd();
		List<Result> list = JsonConvert.DeserializeObject<List<Result>>(value, serializerSettings);
		return list[0].checksum;
	}

	private string _VerifyBattleWithServer(Record record)
	{
		Formatting formatting = Formatting.Indented;
		JsonSerializerSettings serializerSettings = Regulation.SerializerSettings;
		string url = "http://52.0.216.50/battle-simulator/index.php";
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("version", "1000000");
		wWWForm.AddField("record", JsonConvert.SerializeObject(record, formatting, serializerSettings));
		using (WWW wWW = new WWW(url, wWWForm))
		{
			while (!wWW.isDone)
			{
			}
			if (string.IsNullOrEmpty(wWW.error))
			{
				JArray jArray = JArray.Parse(wWW.text);
				List<Result> list = jArray[1].ToObject<List<Result>>(JsonSerializer.Create(serializerSettings));
				return list[0].checksum;
			}
		}
		return string.Empty;
	}

	public void _UpdateTurnUi(Shared.Battle.Input input)
	{
		if (_simulator.option.playMode != Option.PlayMode.RealTime)
		{
			_turnUiUpdater.renderers = _unitRenderers;
			_turnUiUpdater.input = input;
			_simulator.AccessFrame(_turnUiUpdater);
		}
	}

	private void _HideTroopAnchor(Transform troopAnchor)
	{
		for (int i = 0; i < troopAnchor.childCount; i++)
		{
			Transform child = troopAnchor.GetChild(i);
			Renderer[] componentsInChildren = child.GetComponentsInChildren<Renderer>(includeInactive: true);
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = false;
			}
		}
	}

	public IEnumerator _InitGameBattleItem()
	{
		if (_simulator.battleItemDrks != null && _simulator.battleItemDrks.Count > 0)
		{
			int lhsTroopStartIndex = _simulator.frame.lhsTroopStartIndex;
			if (lhsTroopStartIndex >= 0)
			{
				for (int i = 0; i < 9; i++)
				{
					if (_unitRenderers[i + lhsTroopStartIndex] != null)
					{
						Vector3 position = _unitRenderers[i + lhsTroopStartIndex].transform.position;
						_etcEffectCache.Create("StartBuffEffect", position, Quaternion.identity);
					}
				}
			}
		}
		yield return new WaitForSeconds(0.7f);
	}

	public IEnumerator _PlayCommanderBoardAnimation()
	{
		yield return new WaitForSeconds(0.2f);
	}

	private IEnumerator _EnableCommanderBoard()
	{
		yield return new WaitForSeconds(2f);
		ui.UICommanderEnable(EBattleSide.Left);
	}

	public void StartOpening(int curWave)
	{
		if (curWave <= 1)
		{
			UISetter.SetSprite(viewData.openingIcon, "battlestart");
			UISetter.SetActive(viewData.openingIcon, active: true);
			UISetter.SetActive(viewData.waveIcon, active: false);
		}
		else
		{
			int num = curWave;
			UISetter.SetActive(viewData.openingIcon, active: false);
			UISetter.SetActive(viewData.waveIcon, active: true);
			UISetter.SetLabel(viewData.waveValue, num.ToString());
		}
		StartCoroutine("_PlayOpeningAnimation");
	}

	public IEnumerator _PlayOpeningAnimation()
	{
		if (sceneAnimator != null)
		{
			_state = State.Opening;
			sceneAnimator.enabled = true;
			sceneAnimator.Rebind();
			sceneAnimator.Play("Opening");
			while (!sceneAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wait"))
			{
				yield return null;
			}
			sceneAnimator.enabled = false;
		}
		_state = State.Playing;
		ui.CloseOpeninig();
	}

	private IEnumerator StartDialogueEvent()
	{
		ShowdialogueEvent(EBattleDialogueEventType.StartEvent);
		while (_dialogueState == EDialogueState.Play)
		{
			DialogueEndCheck();
			yield return null;
		}
	}

	public IEnumerator _ScrollSpeedTo(float from, float to, float duration)
	{
		yield return null;
		float elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			elapsedTime += _timeGameObject.TimedDeltaTime();
			float speed = Mathf.Lerp(from, to, elapsedTime / duration);
			_SetTerrainScrollSpeed(speed);
			yield return null;
		}
	}

	public IEnumerator _PlayResultAnimation(Result result)
	{
		_state = State.Result;
		yield return new WaitForSeconds(2f);
		Time.timeScale = 1f;
		bool isWin = _simulator.result.winSide < 0;
		UIBattleResult uiResult = UIManager.instance.battle.BattleResult;
		uiResult.SetMainCommanderUnit(lastUserUnit);
		if (_battleData != null)
		{
			if (_battleData.isReplayMode)
			{
				_battleData.move = EBattleResultMove.Challenge;
				Loading.Load(Loading.WorldMap);
				yield break;
			}
			_battleData.isWin = isWin;
			Formatting formatting = Formatting.Indented;
			JsonSerializerSettings serializerSettings = Regulation.SerializerSettings;
			string info = JsonConvert.SerializeObject(_simulator.record, formatting, serializerSettings);
			string result2 = JsonConvert.SerializeObject(result, formatting, serializerSettings);
			RemoteObjectManager.instance.RequestBattleOut(_battleData.type, _simulator.result.checksum, info, result2);
			BattleData.Set(_battleData);
			uiResult.onClick = _OnClickBattleResult;
		}
		else
		{
			uiResult.Set(_simulator, _simulator.regulation);
			uiResult.Open();
		}
	}

	private void _OnClickBattleResult(GameObject sender)
	{
		Time.timeScale = 1f;
		UIFade.Out(0f);
		switch (sender.name)
		{
		case "WorldMap":
			_battleData.move = EBattleResultMove.WorldMap;
			break;
		case "MyTown":
			_battleData.move = EBattleResultMove.MyTown;
			break;
		case "Challenge":
			_battleData.move = EBattleResultMove.Challenge;
			break;
		case "Academy":
			_battleData.move = EBattleResultMove.Academy;
			break;
		case "UnitResearch":
			_battleData.move = EBattleResultMove.UnitResearch;
			break;
		case "Raid":
			_battleData.move = EBattleResultMove.Raid;
			break;
		case "NextStage":
			_battleData.move = EBattleResultMove.NextStage;
			break;
		case "ReStart":
			_battleData.move = EBattleResultMove.ReStart;
			break;
		case "ScrambleMap":
			_battleData.move = EBattleResultMove.Guild;
			break;
		}
		Loading.Load(Loading.WorldMap);
	}

	public bool _TryPickTarget()
	{
		if (!UnityEngine.Input.GetMouseButtonDown(0))
		{
			return false;
		}
		SplitScreenManager splitScreenManager = _splitScreenManager;
		SplitScreenManager.PickingResult pickingResult = splitScreenManager.PickObject(UnityEngine.Input.mousePosition);
		if (pickingResult == null)
		{
			return false;
		}
		string text = pickingResult.target.name;
		if (!text.StartsWith("Unit-"))
		{
			return false;
		}
		int num = int.Parse(text.Substring("Unit-".Length));
		if (pickingResult.drawSide == SplitScreenDrawSide.Left && CanUserInput())
		{
			Unit unit = _simulator.frame.units[num];
			_input = new Shared.Battle.Input(num, unit._activeSkillIdx, -1);
		}
		return true;
	}

	public Shared.Battle.Input _GetCorrectedInput()
	{
		_inputCorrector.input = ((_input != null) ? Shared.Battle.Input.Copy(_input) : null);
		_inputCorrector.isAuto = _isAuto;
		_simulator.AccessFrame(_inputCorrector);
		return _inputCorrector.input;
	}

	public bool _TryPlayTroopEntryAnimation(_UnitRendererCreator urc)
	{
		bool result = false;
		if (urc.createdLhsTroopIndex >= 0)
		{
			StartCoroutine(_PlayTroopEntryAnimation(base.lhsTroopAnchor, 10f, 2f, 2f));
			result = true;
		}
		if (urc.createdRhsTroopIndex >= 0)
		{
			StartCoroutine(_PlayTroopEntryAnimation(base.rhsTroopAnchor, 10f, 2f, 2f));
			result = true;
		}
		return result;
	}

	public override void PlayUnitEntry(UnitRenderer unitRenderer, float delay = 2f, float playTime = 2f)
	{
		if (!(unitRenderer == null))
		{
			StartCoroutine(_PlayUnitEntryAnimation(unitRenderer, delay, playTime));
		}
	}

	protected IEnumerator _PlayUnitEntryAnimation(UnitRenderer unitRenderer, float delay, float playTime)
	{
		AnimationCurve curve = troopEntryAnimationCurve;
		float elapsedTime = 0f;
		bool isEnded = false;
		float entryDistance = 0f - unitRenderer.transform.localPosition.z;
		while (!isEnded)
		{
			elapsedTime += _timeGameObject.TimedDeltaTime();
			if (elapsedTime > playTime)
			{
				isEnded = true;
			}
			if (isEnded)
			{
				elapsedTime = playTime;
			}
			if (unitRenderer.IsDead)
			{
				break;
			}
			float z2 = 0f - entryDistance;
			float t = elapsedTime / playTime;
			z2 += entryDistance * curve.Evaluate(t);
			Vector3 localPosition = unitRenderer.transform.localPosition;
			localPosition.z = z2;
			unitRenderer.transform.localPosition = localPosition;
			if (isEnded)
			{
				unitRenderer.BeginDummyMove();
			}
			yield return null;
		}
	}

	public IEnumerator _PlayTroopEntryAnimation(Transform anchor, float entryDistance, float delay, float playTime)
	{
		AnimationCurve curve = troopEntryAnimationCurve;
		float elapsedTime = 0f;
		bool isEnded = false;
		_enteringTroopCount++;
		while (!isEnded)
		{
			elapsedTime += _timeGameObject.TimedDeltaTime();
			if (elapsedTime > playTime)
			{
				isEnded = true;
			}
			if (isEnded)
			{
				elapsedTime = playTime;
			}
			float z2 = 0f - entryDistance;
			float t = elapsedTime / playTime;
			z2 += entryDistance * curve.Evaluate(t);
			for (int i = 0; i < anchor.childCount; i++)
			{
				Transform child = anchor.GetChild(i);
				if (child.childCount == 0)
				{
					continue;
				}
				int index = child.childCount - 1;
				UnitRenderer component = child.GetChild(index).GetComponent<UnitRenderer>();
				if (!component.IsDead)
				{
					Vector3 localPosition = component.transform.localPosition;
					localPosition.z = z2;
					component.transform.localPosition = localPosition;
					if (isEnded)
					{
						component.BeginDummyMove();
					}
				}
			}
			yield return null;
		}
		_enteringTroopCount--;
	}

	public void _SetLeftTerrainTheme(string theme)
	{
		TerrainScroller terrainScroller = UIManager.instance.battle.LeftView.terrainScroller;
		string text = $"{theme}_{1}";
		CacheElement element = CacheManager.instance.TerrainCache.GetElement(text);
		if (element == null)
		{
			text = theme;
		}
		terrainScroller.theme = text;
	}

	public void _SetRightTerrainTheme(string theme)
	{
		TerrainScroller terrainScroller = UIManager.instance.battle.RightView.terrainScroller;
		string text = $"{theme}_{2}";
		CacheElement element = CacheManager.instance.TerrainCache.GetElement(text);
		if (element == null)
		{
			text = theme;
		}
		terrainScroller.theme = text;
	}

	public void _SetTerrainScrollSpeed(float speed)
	{
		TerrainScroller terrainScroller = UIManager.instance.battle.LeftView.terrainScroller;
		terrainScroller.speed = speed;
		TerrainScroller terrainScroller2 = UIManager.instance.battle.RightView.terrainScroller;
		terrainScroller2.speed = speed;
	}

	public Simulator _CreateSimulator(BattleData bd, Regulation rg)
	{
		EBattleType eBattleType = EBattleType.Undefined;
		RaidData raidData = null;
		string empty = string.Empty;
		List<string> list = null;
		List<Troop> list2 = null;
		List<Troop> list3 = null;
		if (bd != null && bd.isReplayMode)
		{
			return Simulator.Create(rg, bd.record);
		}
		if (bd != null)
		{
			eBattleType = bd.type;
			if (eBattleType == EBattleType.Raid)
			{
				raidData = bd.raidData.ToBattleRaidData();
			}
			empty = bd.stageId;
			list2 = new List<Troop>();
			for (int i = 0; i < bd.attacker.battleTroopList.Count; i++)
			{
				list2.Add(bd.attacker.battleTroopList[i].ToBattleTroop());
			}
			list3 = new List<Troop>();
			for (int j = 0; j < bd.defender.battleTroopList.Count; j++)
			{
				list3.Add(bd.defender.battleTroopList[j].ToBattleTroop());
			}
			if (bd.rewardItems != null)
			{
				bd.rewardItems.RemoveAll((Protocols.RewardInfo.RewardData row) => row.rewardType == ERewardType.Favor);
				Protocols.RewardInfo.RewardData rewardData = bd.rewardItems.Find((Protocols.RewardInfo.RewardData row) => row.rewardType == ERewardType.Goods && row.rewardId == "4");
				if (rewardData != null)
				{
					bd.rewardItems.Remove(rewardData);
				}
				if (bd.rewardItems.Count > 0)
				{
					int count = bd.rewardItems.Count;
					int count2 = list3.Count;
					int num = 0;
					for (int k = 1; k <= count2; k++)
					{
						num += k;
					}
					int num2 = 0;
					for (int l = 0; l < list3.Count; l++)
					{
						Troop troop = list3[l];
						int num3 = 0;
						if (l == list3.Count - 1)
						{
							num3 = count - num2;
						}
						else
						{
							float num4 = (float)(l + 1) / (float)num;
							num3 = (int)((float)count * num4);
						}
						if (num3 > 0)
						{
							num2 += num3;
							List<Troop.Slot> list4 = troop._slots.FindAll((Troop.Slot x) => x != null && !string.IsNullOrEmpty(x.id));
							for (int m = 0; m < num3; m++)
							{
								int index = UnityEngine.Random.Range(0, list4.Count);
								list4[index].dropItemCnt++;
							}
						}
					}
				}
			}
			list = new List<string>();
		}
		else
		{
			empty = "1";
			string[] unitNames = new string[1] { "5017" };
			List<string> unitList = new List<string>();
			RemoteObjectManager.instance.regulation.GetUnitList(EBranch.Army, includeHidden: true).ForEach(delegate(UnitDataRow row)
			{
				unitList.Add(row.key);
			});
			list2 = _CreateTestTroops("Army", new string[1] { "5002" }, unitNames, 1, 1, 1, 1);
			list3 = _CreateTestTroops("Army", new string[1] { "30001" }, unitNames, 1, 1, 1, 1);
			int num5 = 5;
			int count3 = list3.Count;
			int num6 = 0;
			for (int n = 1; n <= count3; n++)
			{
				num6 += n;
			}
			int num7 = 0;
			for (int num8 = 0; num8 < list3.Count; num8++)
			{
				Troop troop2 = list3[num8];
				int num9 = 0;
				if (num8 == list3.Count - 1)
				{
					num9 = num5 - num7;
				}
				else
				{
					float num10 = (float)(num8 + 1) / (float)num6;
					num9 = (int)((float)num5 * num10);
				}
				if (num9 <= 0)
				{
					continue;
				}
				num7 += num9;
				List<Troop.Slot> list5 = troop2._slots.FindAll((Troop.Slot x) => x != null && !string.IsNullOrEmpty(x.id));
				if (list5.Count > 0)
				{
					for (int num11 = 0; num11 < num9; num11++)
					{
						int index2 = UnityEngine.Random.Range(0, list5.Count);
						list5[index2].dropItemCnt++;
					}
				}
			}
			list = new List<string>();
			list.Add("1");
			list.Add("2");
			list.Add("3");
			list.Add("4");
			eBattleType = EBattleType.Plunder;
		}
		int randomSeed = UnityEngine.Random.Range(0, 30000);
		InitState initState = InitState.Create(eBattleType, list2, list3, list, randomSeed);
		initState._stageId = empty;
		if (eBattleType == EBattleType.Raid)
		{
			initState._raidData = raidData;
		}
		return Simulator.Create(rg, initState);
	}

	protected void Resume()
	{
		TimeControllerManager.instance.GameMain.ResumeGroupTime();
	}

	protected void Pause()
	{
		TimeControllerManager.instance.GameMain.PauseGroupTime();
	}

	public void _OnClickMain(GameObject sender)
	{
		string text = sender.name;
		Frame frame = _simulator.frame;
		float num = -1f;
		Shared.Battle.Input input = null;
		switch (text)
		{
		case "Speed-x1":
			num = defaultTimeScale * 2f;
			break;
		case "Speed-x2":
			num = defaultTimeScale;
			break;
		case "Auto":
			_isAuto = true;
			break;
		case "Manual":
			_isAuto = false;
			break;
		case "Pause":
			if (!UIManager.instance.battle.DialogMrg.isActiveAndEnabled)
			{
				if (Manager<UIOptionController>.GetInstance().isShow)
				{
					Manager<UIOptionController>.GetInstance().Hide();
				}
				else if (!_simulator.isGiveUp && (_simulator.initState.battleType != EBattleType.Duel || isReplayMode))
				{
					Manager<UIOptionController>.GetInstance().Show();
				}
			}
			break;
		}
		if (!_isPause && num > 0f)
		{
			_timeScale = num;
			Time.timeScale = _timeScale;
			ui.SelectSpeed(text);
		}
		ui.SelectAutoBattle(_isAuto);
		if (input != null)
		{
			_input = input;
		}
		if (!_isAuto && _state == State.InputWait && !_simulator.frame.isWaitingInput)
		{
			_state = State.Playing;
		}
	}

	public void OnSelect(int unitIndex)
	{
		if (CanUserInput())
		{
			Unit unit = _simulator.frame.units[unitIndex];
			_input = new Shared.Battle.Input(unitIndex, unit._activeSkillIdx, -1);
		}
	}

	public void OnSelect(int unitIndex, int skillIndex)
	{
		if (CanUserInput())
		{
			_input = new Shared.Battle.Input(unitIndex, skillIndex, -1);
		}
	}

	protected bool CanUserInput()
	{
		if (_simulator.isReplayMode)
		{
			return false;
		}
		if (_simulator.isEnded)
		{
			return false;
		}
		return true;
	}

	private List<Troop> _CreateTestTroops(string branch, string[] commanderNames, string[] unitNames, int minTroopCount, int maxTroopCount, int minSlotCount, int maxSlotCount)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		int num = UnityEngine.Random.Range(minTroopCount, maxTroopCount + 1);
		int num2 = UnityEngine.Random.Range(minSlotCount, maxSlotCount + 1);
		List<Troop> list = new List<Troop>();
		for (int i = 0; i < num; i++)
		{
			Troop troop = new Troop();
			troop._slots = new List<Troop.Slot>();
			for (int j = 0; j < 9; j++)
			{
				Troop.Slot slot = new Troop.Slot();
				slot.skills = new List<Troop.Slot.Skill>();
				if (j < num2)
				{
					int num3 = UnityEngine.Random.Range(0, unitNames.Length);
					slot.id = unitNames[num3];
					slot.health = int.MaxValue;
					slot.level = UnityEngine.Random.Range(1, 5);
					slot.rank = 1;
					slot.cid = UnityEngine.Random.Range(1, 5).ToString();
					slot.cls = UnityEngine.Random.Range(1, 5);
					UnitDataRow unitDataRow = regulation.unitDtbl[slot.id];
					IList<string> skillDrks = unitDataRow.skillDrks;
					for (int k = 1; k < skillDrks.Count; k++)
					{
						string text = skillDrks[k];
						if (!string.IsNullOrEmpty(text) && text != "0")
						{
							Troop.Slot.Skill skill = new Troop.Slot.Skill();
							skill.id = text;
							skill.lv = UnityEngine.Random.Range(1, 5);
							skill.sp = 100;
							slot.skills.Add(skill);
						}
					}
				}
				troop._slots.Add(slot);
			}
			list.Add(troop);
		}
		return list;
	}
}
