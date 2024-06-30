using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Cache;
using DialoguerCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

public class M03_Battle : AbstractBattle
{
	private enum State
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

	private class _UnitRendererCreator : FrameAccessor
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

		public M03_Battle battle;

		public OnClickDelegate onClick;

		public static _UnitRendererCreator Create(M03_Battle battle)
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
				if (base.frame._lhsTroopStartIndex < 0)
				{
					ui.SetLhsAllAnnihilated();
				}
			}
			if (base.simulator.isRhsAnnihilated && base.frame._rhsTroopStartIndex >= base.frame.units.Count)
			{
				ui.SetRhsAllAnnihilated();
			}
			return true;
		}

		public override bool OnUnitAccessStart()
		{
			_CreateUnit(base.unitIndex);
			return false;
		}

		private bool _CreateUnit(int index)
		{
			Unit unit = base.frame.units[index];
			if (!base.frame.IsUnitInBattle(index))
			{
				return false;
			}
			if (battle.lastUserUnit == null && unit.side == EBattleSide.Left)
			{
				battle.lastUserUnit = base.unit;
			}
			UnitRenderer unitRenderer = renderers[index];
			if (unitRenderer != null)
			{
				return false;
			}
			if (unit.isDead && !unit.takenRevival)
			{
				return false;
			}
			int lhsTroopIndex = base.simulator.GetLhsTroopIndex(index);
			int rhsTroopIndex = base.simulator.GetRhsTroopIndex(index);
			Transform transform = null;
			Transform transform2 = null;
			SplitScreenDrawSide splitScreenDrawSide = SplitScreenDrawSide.Unknown;
			if (lhsTroopIndex >= 0)
			{
				createdLhsTroopIndex = lhsTroopIndex;
				transform2 = lhsTroopAnchor;
				splitScreenDrawSide = SplitScreenDrawSide.Left;
			}
			if (rhsTroopIndex >= 0)
			{
				createdRhsTroopIndex = rhsTroopIndex;
				transform2 = rhsTroopAnchor;
				splitScreenDrawSide = SplitScreenDrawSide.Right;
			}
			if (unit._charType == ECharacterType.Raid)
			{
				transform = transform2.GetChild(4);
			}
			else if (unit._charType == ECharacterType.RaidPart)
			{
				int num = base.simulator.GetRhsTroopStartIndex(rhsTroopIndex) + unit._mainIdx;
				if (renderers[num] == null)
				{
					_CreateUnit(num);
				}
				transform = renderers[num].GetMountPosition("_PT" + unit._partIdx);
			}
			else
			{
				int index2 = index % 9;
				transform = transform2.GetChild(index2);
			}
			UnitDataRow unitDataRow = base.simulator.regulation.unitDtbl[unit.dri];
			unitRenderer = unitRenderCache.Create(unitDataRow.prefabId, createUnitPosition, Quaternion.identity, transform);
			if (unitRenderer == null)
			{
				return false;
			}
			unitRenderer.SetModelType(base.unitDr.modelType);
			if (unit._charType == ECharacterType.RaidPart)
			{
				unitRenderer.transform.localPosition = Vector3.zero;
				unitRenderer.transform.localScale = Vector3.one;
				unitRenderer.transform.localRotation = Quaternion.identity;
			}
			unitRenderer.scale = unit._scale;
			unitRenderer.Init();
			renderers[index] = unitRenderer;
			unitRenderer.gameObject.name = "Unit-" + index;
			unitRenderer.SetUnit(unit);
			unitRenderer.ui = battle.ui.uiBattleUnitPanel.CreateUnitUI();
			if (unitRenderer.ui != null)
			{
				unitRenderer.ui.transform.position = SplitScreenManager.instance.ConvertPosCutToUI(splitScreenDrawSide, unitRenderer.uiTransform.position);
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
				GameObject gameObject = CacheManager.instance.UiCache.Create("SelectMark", transform2);
				gameObject.transform.parent = unitRenderer.transform;
				gameObject.transform.localPosition = Vector3.zero;
				BattleUnitSelectMark component = gameObject.GetComponent<BattleUnitSelectMark>();
				unitRenderer.selectedMark = component;
			}
			unitRenderer.selectedMark.SetTurnUnit(active: false);
			unitRenderer.selectedMark.SetAttackTargetCadidate(active: false);
			unitRenderer.selectedMark.SetAttackTarget(active: false);
			return true;
		}

		private UIBattleUnit _CreateUnitUi(GameObject parent, GameObject prefab)
		{
			GameObject gameObject = NGUITools.AddChild(parent, prefab);
			gameObject.SetActive(value: true);
			return gameObject.GetComponent<UIBattleUnit>();
		}
	}

	private class _ProjectileRendererCreator : FrameAccessor
	{
		public UnitRenderer[] unitRenderers;

		public Shared.Battle.Random random;

		protected M03_Battle battle;

		protected CacheWithPool rendererCache;

		protected bool bAlive;

		protected EActionEffWithFireType actionEffWithFireType;

		protected bool playFireSound;

		protected bool playHitSound;

		protected bool playBeHitSound;

		protected bool playBeMissSound;

		public static _ProjectileRendererCreator Create(M03_Battle battle)
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

		public override bool OnFirePointAccessStart()
		{
			playFireSound = false;
			playHitSound = false;
			playBeHitSound = false;
			playBeMissSound = false;
			return base.OnFirePointAccessStart();
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
			Vector3 position = unitRenderer2.mainTransform.position;
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
			if (actionEffWithFireType == EActionEffWithFireType.None && !base.projectile.isSplash && !string.IsNullOrEmpty(base.skillDr.fireSound) && base.skillDr.fireSound != "0" && !playFireSound)
			{
				SoundManager.PlaySFX(base.skillDr.fireSound);
				playFireSound = true;
			}
			if (base.simulator.regulation.projectileMotionPhaseDtbl.length != battle._projectileCache.elements.Count)
			{
				projectileController.name = base.projectile.id.ToString();
				ProjectileMotionPhase projectileMotionPhase = projectileController.Create(base.projectile.fireKey, bone.position, base.skill._fireRender);
				if (projectileMotionPhase != null)
				{
					projectileMotionPhase.Set(unitRenderer, unitRenderer);
					projectileMotionPhase.SetEventDealy(base.projectileHitDelayTime);
				}
				ProjectileMotionPhase projectileMotionPhase2 = projectileController.Create(base.projectile.hitKey, position, base.skill._hitRender);
				if (projectileMotionPhase2 != null)
				{
					projectileMotionPhase2.Set(unitRenderer, unitRenderer2);
				}
			}
			else
			{
				projectileController.name = base.projectile.id.ToString();
				ProjectileMotionPhase projectileMotionPhase3 = projectileController.Create(base.projectile.id / 100000, bone.position, base.skill._fireRender);
				if (projectileMotionPhase3 != null)
				{
					projectileMotionPhase3.Set(unitRenderer, unitRenderer);
					projectileMotionPhase3.SetEventDealy(base.projectileHitDelayTime);
				}
				ProjectileMotionPhase projectileMotionPhase4 = projectileController.Create(base.projectile.id % 100000, position, base.skill._hitRender);
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
				if (flag && Simulator.HasTimeEvent(base.projectileHitTime - base.skillDr.hitSoundDelay, base.projectile.elapsedTime) && !string.IsNullOrEmpty(base.skillDr.hitSound) && base.skillDr.hitSound != "0" && !playHitSound && (base.skillDr.hitSoundType == EHitSoundType.None || (base.skillDr.hitSoundType == EHitSoundType.First && base.projectile.fireEventIndex == 0)))
				{
					SoundManager.PlaySFX(base.skillDr.hitSound);
					playHitSound = true;
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
					if (!string.IsNullOrEmpty(base.skillDr.beMissSound) && base.skillDr.beMissSound != "0" && !playBeMissSound)
					{
						SoundManager.PlaySFX(base.skillDr.beMissSound);
						playBeMissSound = true;
					}
				}
				else if (!string.IsNullOrEmpty(base.skillDr.beHitSound) && base.skillDr.beHitSound != "0" && !playBeHitSound)
				{
					SoundManager.PlaySFX(base.skillDr.beHitSound);
					playBeHitSound = true;
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

	private class _UnitRendererUpdater : FrameAccessor
	{
		public UIBattleMain ui;

		public SplitScreenManager splitScreenManager;

		public CutInEffect cutInEffect;

		public UnitRenderer[] renderers;

		public bool pause;

		protected M03_Battle battle;

		public float delay;

		public static _UnitRendererUpdater Create(M03_Battle battle)
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
					if (base.unit._charType == ECharacterType.RaidPart)
					{
						unitRenderer.Dead("destroy");
						int rhsTroopIndex = base.simulator.GetRhsTroopIndex(num);
						int num2 = base.simulator.GetRhsTroopStartIndex(rhsTroopIndex) + base.unit._mainIdx;
						UnitRenderer unitRenderer2 = renderers[num2];
						if (unitRenderer2 != null && !unitRenderer2.unit.isPlayingAction)
						{
							unitRenderer2.PlayAnimation("destroy_" + base.unit._partIdx);
						}
					}
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
					if (unitRenderer.ui != null)
					{
						unitRenderer.ui.UpdateSkillAmount();
						unitRenderer.ui.SetAnimateHp(base.unit.maxHealth, base.unit.health);
					}
					if (unitRenderer.uiCommander != null)
					{
						if (unitRenderer.uiCommander.skillReservation)
						{
							battle.DeleteInputUnit(base.unitIndex);
						}
						unitRenderer.uiCommander.UpdateUI();
					}
				}
				if (base.frame.onWaveTurn && base.simulator.initState.battleType == EBattleType.Raid && (base.unit._charType == ECharacterType.Raid || base.unit._charType == ECharacterType.RaidPart))
				{
					string key = $"{base.simulator.initState.raidData.raidId}_{base.frame._waveTurn}_{base.unit._unitIdx % 9}";
					if (base.simulator.regulation.raidDtbl.ContainsKey(key))
					{
						RaidDataRow raidDataRow = base.simulator.regulation.raidDtbl[key];
						if (raidDataRow.effectName != "-")
						{
							if (unitRenderer.phaseEffect != null)
							{
								CacheManager.instance.EtcEffectCache.Create(raidDataRow.effectName, Vector3.zero, Quaternion.identity, unitRenderer.phaseEffect.transform);
							}
							else
							{
								CacheManager.instance.EtcEffectCache.Create(raidDataRow.effectName, unitRenderer.transform.position, Quaternion.identity);
							}
						}
					}
				}
			}
			else
			{
				if (base.frame.onTurn && base.simulator.initState.battleType == EBattleType.Raid && (base.unit._charType == ECharacterType.Raid || base.unit._charType == ECharacterType.RaidPart) && base.frame.turnUnitIndex == base.unitIndex)
				{
					string key2 = $"{base.simulator.initState.raidData._raidId}_{base.frame._waveTurn}_{base.unit._unitIdx % 9}";
					if (base.simulator.regulation.raidDtbl.ContainsKey(key2))
					{
						RaidDataRow raidDataRow2 = base.simulator.regulation.raidDtbl[key2];
						if (raidDataRow2.effectName != "-")
						{
							if (unitRenderer.phaseEffect != null)
							{
								CacheManager.instance.EtcEffectCache.Create(raidDataRow2.effectName, Vector3.zero, Quaternion.identity, unitRenderer.phaseEffect.transform);
							}
							else
							{
								CacheManager.instance.EtcEffectCache.Create(raidDataRow2.effectName, unitRenderer.transform.position, Quaternion.identity);
							}
						}
					}
				}
				Dictionary<int, Status>.Enumerator statusItr = base.unit.StatusItr;
				while (statusItr.MoveNext())
				{
					Status value = statusItr.Current.Value;
					if (value.IsAlive)
					{
						if (value.ElapsedTimeTick == 0)
						{
							unitRenderer.AddStatus(value);
						}
					}
					else
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
					if (unitRenderer.uiCommander.skillReservation && !unitRenderer.uiCommander.canInput)
					{
						battle.DeleteInputUnit(base.unitIndex);
					}
				}
				Vector3 position = unitRenderer.uiTransform.position;
				position += UnityEngine.Random.insideUnitSphere * 0.5f;
				position = splitScreenManager.ConvertPosCutToUI(unitRenderer.drawSide, position);
				int num3 = base.unit.health;
				if (base.unit.takenDamage > 0)
				{
					base.unit._readyFatal = true;
					num3 = (int)Math.Max(num3 - base.unit.takenDamage, 0L);
					if (!base.unit.isPlayingAction)
					{
						unitRenderer.PlayAnimation("behit");
					}
					battle.ui.UICommanderState(base.unit, "Behit");
					int num4 = (int)((long)num3 * 100L / base.unit.maxHealth);
					unitRenderer.SetInjury(num4 <= 30);
					ui.damageBubble.Add(position, base.unit.takenDamage, Color.white);
					if (base.unit.side == EBattleSide.Left)
					{
						battle.lastUserUnit = base.unit;
						ui.UICommanderShake(base.unit);
						UIManager.instance.battle.LeftView.cameraShake.shakeTime = 0.05f;
						UIManager.instance.battle.LeftView.cameraShake.maxShakeRange = 0.2f;
						UIManager.instance.battle.LeftView.cameraShake.Begin();
					}
					else
					{
						UIManager.instance.battle.RightView.cameraShake.shakeTime = 0.05f;
						UIManager.instance.battle.RightView.cameraShake.maxShakeRange = 0.2f;
						UIManager.instance.battle.RightView.cameraShake.Begin();
					}
				}
				if (base.unit.takenCriticalDamage > 0)
				{
					base.unit._readyFatal = true;
					num3 = (int)Math.Max(num3 - base.unit.takenCriticalDamage, 0L);
					if (!base.unit.isPlayingAction)
					{
						unitRenderer.PlayAnimation("behit");
					}
					battle.ui.UICommanderState(base.unit, "Behit");
					int num5 = (int)((long)num3 * 100L / base.unit.maxHealth);
					unitRenderer.SetInjury(num5 <= 30);
					ui.criticalDamageBubble.Add(position, base.unit.takenCriticalDamage, Color.red);
					if (ui.criticalBubble != null)
					{
						ui.criticalBubble.Add(position);
					}
					if (base.unit.side == EBattleSide.Left)
					{
						battle.lastUserUnit = base.unit;
						ui.UICommanderShake(base.unit);
						UIManager.instance.battle.LeftView.cameraShake.shakeTime = 0.15f;
						UIManager.instance.battle.LeftView.cameraShake.maxShakeRange = 0.5f;
						UIManager.instance.battle.LeftView.cameraShake.Begin();
					}
					else
					{
						if (ui.slideCritical != null)
						{
							ui.slideCritical.Add(ui.slideCritical.transform.position);
						}
						UIManager.instance.battle.RightView.cameraShake.shakeTime = 0.15f;
						UIManager.instance.battle.RightView.cameraShake.maxShakeRange = 0.5f;
						UIManager.instance.battle.RightView.cameraShake.Begin();
					}
				}
				if (base.unit.uiTakenDamage > 0)
				{
					base.unit._readyFatal = true;
					num3 = (int)Math.Max(num3 - base.unit.uiTakenDamage, 0L);
					int num6 = (int)((long)num3 * 100L / base.unit.maxHealth);
					unitRenderer.SetInjury(num6 <= 30);
					ui.damageBubble.Add(position, base.unit.uiTakenDamage, Color.white);
				}
				if (base.unit.takenHealing > 0)
				{
					num3 = (int)Math.Min(num3 + base.unit.takenHealing, base.unit.maxHealth);
					int num7 = (int)((long)num3 * 100L / base.unit.maxHealth);
					unitRenderer.SetInjury(num7 <= 30);
					ui.damageBubble.Add(position, base.unit.takenHealing, Color.green);
				}
				if (base.unit._takenCriticalHealing > 0)
				{
					num3 = (int)Math.Min(num3 + base.unit._takenCriticalHealing, base.unit.maxHealth);
					int num8 = (int)((long)num3 * 100L / base.unit.maxHealth);
					unitRenderer.SetInjury(num8 <= 30);
					ui.criticalDamageBubble.Add(position, base.unit._takenCriticalHealing, Color.green);
				}
				if (base.unit.uiTakenHealing > 0)
				{
					num3 = (int)Math.Min(num3 + base.unit.uiTakenHealing, base.unit.maxHealth);
					int num9 = (int)((long)num3 * 100L / base.unit.maxHealth);
					unitRenderer.SetInjury(num9 <= 30);
					ui.damageBubble.Add(position, base.unit.uiTakenHealing, Color.green);
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
			if (base.skill.IsCutInSkill && fireEvent != null)
			{
				FireActionDataRow.TimeSet timeSet = base.skill.FireActionDr.GetTimeSet(base.simulator.CanEnableFireAction(base.unit));
				if (timeSet.timeSleepDuringFire)
				{
					num -= fireEvent.time;
				}
				num += 66;
				flag = true;
			}
			if (base.skill.remainedMotionTime > 0 && !base.skill.IsCutInSkill && (!renderer.IsDead || base.skill.isIgnoreDeathType))
			{
				int elapsedTime = base.unitMotionDr.playTime - base.skill.remainedMotionTime;
				if (Simulator.HasTimeEvent(base.skillDr.actionSoundDelay, elapsedTime) && !string.IsNullOrEmpty(base.skillDr.actionSound) && base.skillDr.actionSound != "0")
				{
					SoundManager.PlaySFX(base.skillDr.actionSound);
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
			if (base.unit.side == EBattleSide.Left)
			{
				CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[base.unit._cdri];
				if (base.skillIndex > 0)
				{
					if (base.skill.isActiveSkill)
					{
						if (!base.simulator.option.enableEffect)
						{
							RoCommander roCommander = null;
							if (base.unit._charType != ECharacterType.Mercenary && base.unit._charType != ECharacterType.SuperMercenary && base.unit._charType != ECharacterType.NPCMercenary && base.unit._charType != ECharacterType.SuperNPCMercenary)
							{
								roCommander = RemoteObjectManager.instance.localUser.FindCommander(base.unit.cid);
							}
							if (roCommander != null && roCommander.isBasicCostume)
							{
								ui.slideSkill.Add(1, commanderDataRow.resourceId + "_" + roCommander.currentViewCostume, base.unit.skills[base.skillIndex]._skillName);
							}
							else
							{
								ui.slideSkill.Add(1, commanderDataRow.resourceId + "_" + RemoteObjectManager.instance.regulation.GetCostumeName(base.unit.ctid), base.unit.skills[base.skillIndex]._skillName);
							}
						}
						SoundManager.PlayVoiceEvent(commanderDataRow, ECommanderVoiceEventType.ActiveSkill, looping: false, 0f, float.MaxValue, 1f);
					}
					else
					{
						RoCommander roCommander2 = null;
						if (base.unit._charType != ECharacterType.Mercenary && base.unit._charType != ECharacterType.SuperMercenary && base.unit._charType != ECharacterType.NPCMercenary && base.unit._charType != ECharacterType.SuperNPCMercenary)
						{
							roCommander2 = RemoteObjectManager.instance.localUser.FindCommander(base.unit.cid);
						}
						if (roCommander2 != null && roCommander2.isBasicCostume)
						{
							ui.slideSkill.Add(0, commanderDataRow.resourceId + "_" + roCommander2.currentViewCostume, base.unit.skills[base.skillIndex]._skillName);
						}
						else
						{
							ui.slideSkill.Add(0, commanderDataRow.resourceId + "_" + RemoteObjectManager.instance.regulation.GetCostumeName(base.unit.ctid), base.unit.skills[base.skillIndex]._skillName);
						}
						if (base.skillIndex == 2)
						{
							SoundManager.PlayVoiceEvent(commanderDataRow, ECommanderVoiceEventType.Passive1, looping: false, 0f, float.MaxValue, 1f);
						}
						else if (base.skillIndex == 3)
						{
							SoundManager.PlayVoiceEvent(commanderDataRow, ECommanderVoiceEventType.Passive2, looping: false, 0f, float.MaxValue, 1f);
						}
					}
				}
				else
				{
					SoundManager.PlayVoiceEvent(commanderDataRow, ECommanderVoiceEventType.Attack, looping: false, 0f, float.MaxValue, 1f);
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
					base.skill._fireRender = ((!fireEffect.enableEffect) ? fireEffect.off.option.fireRender : fireEffect.on.option.fireRender);
					base.skill._hitRender = ((!fireEffect.enableEffect) ? fireEffect.off.option.hitRender : fireEffect.on.option.hitRender);
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
			Unit target = renderer.unit;
			if (target._charType == ECharacterType.RaidPart)
			{
				int rhsTroopIndex = base.simulator.GetRhsTroopIndex(base.unit._unitIdx);
				int num = base.simulator.GetRhsTroopStartIndex(rhsTroopIndex) + target._mainIdx;
				UnitRenderer unitRenderer = renderers[num];
				if (unitRenderer != null && !unitRenderer.unit.isPlayingAction)
				{
					unitRenderer.PlayAnimation(motionName);
				}
			}
			renderer.PlayAnimation(motionName, hasReturnMotion);
		}
	}

	private class _MainUiUpdater : FrameAccessor
	{
	}

	private class _SkillIconUpdater : FrameAccessor
	{
		public UIBattleMain ui;

		public UnitRenderer[] _unitRenderers;

		public M03_Battle battle;

		public static _SkillIconUpdater Create(M03_Battle battle)
		{
			_SkillIconUpdater skillIconUpdater = new _SkillIconUpdater();
			skillIconUpdater.ui = battle.ui;
			skillIconUpdater.battle = battle;
			return skillIconUpdater;
		}

		public override bool OnFrameAccessStart()
		{
			return false;
		}

		public override bool OnUnitAccessStart()
		{
			if (!base.frame.IsUnitInBattle(base.unitIndex))
			{
				return false;
			}
			return false;
		}

		public override bool OnSkillAccessStart()
		{
			return false;
		}
	}

	private class _TurnUiUpdater : FrameAccessor
	{
		public M03_Battle battle;

		public UIBattleMain ui;

		public UnitRenderer[] renderers;

		public Shared.Battle.Input input;

		public static _TurnUiUpdater Create(M03_Battle battle)
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
				if (base.simulator.rhsTroops[base.frame._rhsWave - 1]._activeSlotCount > 0)
				{
					ui.SetRhsCurrWave(base.frame._rhsWave);
					battle.StartOpening(base.frame._rhsWave);
				}
				battle._isNewWave = true;
			}
			if (base.frame.lhsOnWave)
			{
				ui.SetLhsCurrWave(base.frame._lhsWave);
			}
			if (base.frame.onWaveTurn)
			{
				ui.SetUITurn(base.frame._waveTurn);
			}
			if (base.simulator.initState.battleType != EBattleType.Raid)
			{
				ui.SetClearRank(base.simulator.mission.clearCount);
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

	private class _InputCorrector : FrameAccessor
	{
		public Shared.Battle.Input input;

		public bool isAuto;

		public M03_Battle battle;

		public static _InputCorrector Create(M03_Battle battle)
		{
			_InputCorrector inputCorrector = new _InputCorrector();
			inputCorrector.battle = battle;
			return inputCorrector;
		}

		public override bool OnFrameAccessStart()
		{
			if (!base.frame.CanUseSkill(base.simulator.option))
			{
				return false;
			}
			if (input == null)
			{
				if (battle.HasInputUnit())
				{
					input = battle.GetUnitInput();
					if (input != null)
					{
						return false;
					}
				}
				if (isAuto)
				{
					if (base.simulator.option.immediatelyUseActiveSkill)
					{
						for (int i = 0; i < Manager<UIBattleUnitController>.GetInstance()._items.Count; i++)
						{
							UIBattleUnitControllerItem uIBattleUnitControllerItem = Manager<UIBattleUnitController>.GetInstance()._items[i];
							if (uIBattleUnitControllerItem.canInput)
							{
								Unit unit = base.frame.units[uIBattleUnitControllerItem.unitIdx];
								if (unit != null && !unit.isDead && base.simulator.CanUnitInput(unit))
								{
									input = new Shared.Battle.Input(unit._unitIdx, unit._activeSkillIdx, -1);
									break;
								}
							}
						}
					}
					else if (base.frame.isWaitingLhsInput)
					{
						UnitRenderer unitRenderer = battle.UnitRenderers[base.frame.turnUnitIndex];
						if (unitRenderer != null && unitRenderer.uiCommander != null && unitRenderer.uiCommander.canInput)
						{
							Unit unit = base.frame.units[base.frame.turnUnitIndex];
							if (unit != null && base.simulator.CanUnitInput(unit))
							{
								input = new Shared.Battle.Input(unit._unitIdx, unit._activeSkillIdx, -1);
							}
						}
					}
				}
			}
			else if (!base.simulator.CanUnitInput(input.unitIndex))
			{
				input = null;
			}
			return false;
		}
	}

	public UIAtlas commanderAtlas;

	public UIAtlas commanderAtlas_2;

	public UIAtlas skillIconAtals;

	public UIAtlas unitAtlas;

	public UIAtlas unitAtlas_2;

	public UIAtlas battleCommanderUnitAtlas;

	public UIAtlas iconAtlas;

	public E_CUTIN_STATE eCutInState;

	private State _state;

	private _UnitRendererCreator _unitRendererCreator;

	private _ProjectileRendererCreator _projectileRendererCreator;

	private _UnitRendererUpdater _unitRendererUpdater;

	private _SkillIconUpdater _skillIconUpdater;

	private _TurnUiUpdater _turnUiUpdater;

	private _InputCorrector _inputCorrector;

	private EDialogueState _dialogueState;

	protected bool _isNewWave = true;

	[HideInInspector]
	public Unit lastUserUnit;

	protected bool _isEnd;

	protected int maximumInputUnit = 3;

	protected List<int> inputUnit = new List<int>();

	private int nextenemyId;

	protected bool netRequesting;

	private EventBattleScenarioDataRow eventScenarioData;

	private bool _isInit;

	public static string KEY => "-F=S";

	public override Simulator Simulator => _simulator;

	public override BattleData BattleData => _battleData;

	public RoLocalUser localUser => RemoteObjectManager.instance.localUser;

	public Shared.Battle.Input GetUnitInput()
	{
		for (int i = 0; i < inputUnit.Count; i++)
		{
			Unit unit = _simulator.frame.units[inputUnit[i]];
			if (_simulator.CanUnitInput(inputUnit[i]))
			{
				return new Shared.Battle.Input(inputUnit[i], unit._activeSkillIdx, -1);
			}
			if (unit.isUsingSkill)
			{
				return null;
			}
		}
		return null;
	}

	public void AddInputUnit(int unitIndex)
	{
		if (!(_unitRenderers[unitIndex].uiCommander != null) || _unitRenderers[unitIndex].uiCommander.onLockSkill.activeSelf)
		{
			return;
		}
		if (inputUnit.Count >= maximumInputUnit)
		{
			if (_unitRenderers[inputUnit[0]].uiCommander != null)
			{
				_unitRenderers[inputUnit[0]].uiCommander.skillReservation = false;
			}
			inputUnit.RemoveAt(0);
		}
		_unitRenderers[unitIndex].uiCommander.skillReservation = true;
		inputUnit.Add(unitIndex);
	}

	public void DeleteInputUnit(int unitIndex)
	{
		if (_unitRenderers[unitIndex].uiCommander != null)
		{
			_unitRenderers[unitIndex].uiCommander.skillReservation = false;
		}
		inputUnit.Remove(unitIndex);
	}

	public void SetInputUnit(int unitIndex)
	{
		if (HasInputUnit(unitIndex))
		{
			DeleteInputUnit(unitIndex);
		}
		else
		{
			AddInputUnit(unitIndex);
		}
	}

	public bool HasInputUnit()
	{
		return inputUnit.Count > 0;
	}

	public bool HasInputUnit(int unitIndex)
	{
		return inputUnit.FindIndex((int x) => x == unitIndex) >= 0;
	}

	private IEnumerator InitUI(BattleData bd)
	{
		ui = UIManager.instance.battle.MainUI;
		if (bd != null)
		{
			ui.Set(bd);
		}
		else
		{
			EBattleType battleType = EBattleType.Plunder;
			ui.Set(battleType);
			ui.Set(RoUser.CreateDummyUser("AA", "-", UnityEngine.Random.Range(1, 2).ToString()), RoUser.CreateDummyUser("BB", "-", UnityEngine.Random.Range(1, 2).ToString()));
		}
		ui.Open();
		yield return null;
	}

	private void Start()
	{
		StartCoroutine(BattleStart());
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus && _simulator != null && !_simulator.isGiveUp)
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
		if (!AssetBundleManager.HasAssetBundle("SkillIcon.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("SkillIcon.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("Unit.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("Unit.assetbundle"));
		}
		if (!AssetBundleManager.HasAssetBundle("Unit_2.assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle("Unit_2.assetbundle"));
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
		if (AssetBundleManager.HasAssetBundle("SkillIcon.assetbundle"))
		{
			skillIconAtals.replacement = AssetBundleManager.GetObjectFromAssetBundle("SkillIcon.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			skillIconAtals.spriteMaterial = Resources.Load<Material>("Atlas/SkillIcon");
		}
		if (AssetBundleManager.HasAssetBundle("Unit.assetbundle"))
		{
			unitAtlas.replacement = AssetBundleManager.GetObjectFromAssetBundle("Unit.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			unitAtlas.spriteMaterial = Resources.Load<Material>("Atlas/Unit");
		}
		if (AssetBundleManager.HasAssetBundle("Unit_2.assetbundle"))
		{
			unitAtlas_2.replacement = AssetBundleManager.GetObjectFromAssetBundle("Unit_2.assetbundle").GetComponent<UIAtlas>();
		}
		else
		{
			unitAtlas_2.spriteMaterial = Resources.Load<Material>("Atlas/Unit_2");
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
		_isAuto = false;
		_isPause = false;
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
		_battleData = BattleData.Get();
		if (_battleData != null && _battleData.isReplayMode)
		{
			isReplayMode = true;
			_battleData.type = (EBattleType)_battleData.record.initState._battleType;
		}
		_simulator = _CreateSimulator(_battleData, RemoteObjectManager.instance.regulation);
		if (!_simulator.isReplayMode)
		{
			_simulator.record._option.enableEffect = GameSetting.instance.effect;
		}
		yield return StartCoroutine(InitUI(_battleData));
		_unitRenderers = new UnitRenderer[_simulator.unitCount];
		_unitRendererCreator = _UnitRendererCreator.Create(this);
		_unitRendererCreator.onClick = OnSelect;
		_projectileRendererCreator = _ProjectileRendererCreator.Create(this);
		_unitRendererUpdater = _UnitRendererUpdater.Create(this);
		_skillIconUpdater = _SkillIconUpdater.Create(this);
		_skillIconUpdater._unitRenderers = _unitRenderers;
		_turnUiUpdater = _TurnUiUpdater.Create(this);
		_turnUiUpdater.renderers = _unitRenderers;
		_turnUiUpdater.input = null;
		_inputCorrector = _InputCorrector.Create(this);
		_inputCorrector.input = null;
		_inputCorrector.isAuto = _isAuto;
		_simulator.AccessFrame(_unitRendererCreator);
		_simulator.AccessFrame(_unitRendererUpdater);
		string leftMap = "land_needleleaf";
		string rightMap = "land_needleleaf";
		Regulation reg = RemoteObjectManager.instance.regulation;
		InitState initState = _simulator.initState;
		bool ableAutoMode = false;
		if (_battleData != null)
		{
		}
		if (initState != null)
		{
			switch (initState.battleType)
			{
			case EBattleType.Plunder:
			{
				RoWorldMap.Stage stage = localUser.FindWorldMapStage(initState.stageID);
				leftMap = stage.data.battlemap;
				rightMap = stage.data.enemymap;
				break;
			}
			case EBattleType.Annihilation:
			{
				AnnihilateBattleDataRow annihilateBattleDataRow = reg.annihilateBattleDtbl[initState.stageID];
				leftMap = annihilateBattleDataRow.battleMap;
				rightMap = annihilateBattleDataRow.enemyMap;
				break;
			}
			case EBattleType.GuildScramble:
			{
				GuildStruggleDataRow guildStruggleDataRow = reg.guildStruggleDtbl[initState.stageID];
				leftMap = guildStruggleDataRow.battlemap;
				rightMap = guildStruggleDataRow.enemymap;
				break;
			}
			case EBattleType.Guerrilla:
			{
				SweepDataRow sweepDataRow = reg.sweepDtbl[initState.stageID];
				leftMap = sweepDataRow.battlemap;
				rightMap = sweepDataRow.enemymap;
				break;
			}
			case EBattleType.Raid:
			{
				RaidChallengeDataRow raidChallengeDataRow = reg.raidChallengeDtbl[initState.raidData.raidId.ToString()];
				leftMap = raidChallengeDataRow.battlemap;
				rightMap = raidChallengeDataRow.enemymap;
				ableAutoMode = true;
				break;
			}
			case EBattleType.Duel:
			case EBattleType.WaveDuel:
			case EBattleType.Conquest:
			case EBattleType.WorldDuel:
				leftMap = "land_challenge";
				rightMap = "land_challenge";
				ableAutoMode = true;
				break;
			case EBattleType.WaveBattle:
			{
				WaveBattleDataRow waveBattleDataRow = reg.FindWaveBattleData(1);
				leftMap = waveBattleDataRow.battlemap;
				rightMap = waveBattleDataRow.enemymap;
				ableAutoMode = true;
				break;
			}
			case EBattleType.CooperateBattle:
			{
				CooperateBattleDataRow cooperateBattleDataRow = reg.cooperateBattleDtbl[initState.stageID];
				leftMap = cooperateBattleDataRow.battleMap;
				rightMap = cooperateBattleDataRow.enemyMap;
				break;
			}
			case EBattleType.EventBattle:
			{
				EventBattleFieldDataRow eventBattleFieldDataRow = reg.eventBattleFieldDtbl[initState.stageID];
				leftMap = eventBattleFieldDataRow.battleMap;
				rightMap = eventBattleFieldDataRow.enemyMap;
				ableAutoMode = true;
				break;
			}
			case EBattleType.EventRaid:
			{
				EventRaidDataRow eventRaidDataRow = reg.eventRaidDtbl[initState.stageID];
				leftMap = eventRaidDataRow.battleMap;
				rightMap = eventRaidDataRow.enemyMap;
				ableAutoMode = true;
				break;
			}
			case EBattleType.InfinityBattle:
			{
				InfinityFieldDataRow infinityFieldDataRow = reg.infinityFieldDtbl[initState.stageID];
				leftMap = infinityFieldDataRow.battleMap;
				rightMap = infinityFieldDataRow.enemyMap;
				ableAutoMode = true;
				break;
			}
			}
			_SetLeftTerrainTheme(leftMap);
			_SetRightTerrainTheme(rightMap);
		}
		_SetTerrainScrollSpeed(ConstValue.battleTerrainScrollSpeed);
		_timeScale = defaultTimeScale;
		if (GameSetting.instance.speedUp)
		{
			_timeScale = 2f;
			ui.SelectSpeed("Speed-x1");
		}
		Time.timeScale = _timeScale;
		SoundManager.SetPitchSFX(_timeScale);
		if (isReplayMode)
		{
			ableAutoMode = false;
			ui.main.uiBtnResultView.SetEnable(enable: false);
			UISetter.SetActive(ui.dummySkip, active: false);
		}
		else
		{
			if (localUser.tutorialData.enable)
			{
				ui.SetPauseEnable(enable: false);
			}
			UISetter.SetActive(ui.dummySkip, active: false);
		}
		if (ableAutoMode)
		{
			if (_simulator.initState.battleType == EBattleType.Duel || _simulator.initState.battleType == EBattleType.WaveDuel || _simulator.initState.battleType == EBattleType.WorldDuel || _simulator.initState.battleType == EBattleType.Conquest)
			{
				_isAuto = true;
			}
			else
			{
				if (GameSetting.instance.repeatBattle)
				{
					GameSetting.instance.autoSkill = true;
				}
				_isAuto = GameSetting.instance.autoSkill;
			}
			ui.SetAutoEnable(enable: true);
		}
		else
		{
			ui.SetAutoEnable(enable: false);
		}
		ui.RepositionGrids();
		ui.SelectAutoBattle(_isAuto);
		ui.onClick = _OnClickMain;
		ui.SetRemainTime(GetRemainedTime());
		Manager<UIBattleUnitController>.GetInstance()._Click = OnSelect;
		Manager<UIOptionController>.GetInstance().InitUI();
		Manager<UIOptionController>.GetInstance().opt_sound = GameSetting.instance.se;
		Manager<UIOptionController>.GetInstance().opt_skill = true;
		Manager<UIOptionController>.GetInstance().opt_bgm = GameSetting.instance.bgm;
		Manager<UIOptionController>.GetInstance().onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text != null && text == "btn_end")
			{
				if (isReplayMode)
				{
					if (_battleData != null)
					{
						if (_battleData.move == EBattleResultMove.Undefined)
						{
							switch (_battleData.type)
							{
							case EBattleType.Duel:
								_battleData.move = EBattleResultMove.Challenge;
								break;
							case EBattleType.WaveDuel:
								_battleData.move = EBattleResultMove.WaveDuel;
								break;
							case EBattleType.WorldDuel:
								_battleData.move = EBattleResultMove.WorldDuel;
								break;
							default:
								_battleData.move = EBattleResultMove.MyTown;
								break;
							}
						}
						BattleData.Set(_battleData);
					}
					if (Time.timeScale != 1f)
					{
						Time.timeScale = 1f;
						SoundManager.SetPitchSFX(1f);
					}
					Loading.Load(Loading.WorldMap);
				}
				else
				{
					M03_Battle m03_Battle = this;
					string title = Localization.Get("1335");
					string message = string.Empty;
					string subMessage = string.Empty;
					string empty = string.Empty;
					if (_simulator.initState.battleType == EBattleType.Plunder)
					{
						message = Localization.Get("1336");
						subMessage = Localization.Get("1337");
					}
					else if (_simulator.initState.battleType == EBattleType.Raid)
					{
						message = Localization.Get("18909");
					}
					else if (_simulator.initState.battleType == EBattleType.Guerrilla)
					{
						message = string.Format(Localization.Get("1355"), Localization.Get("1904"));
						subMessage = Localization.Get("1337");
					}
					else if (_simulator.initState.battleType == EBattleType.Duel || _simulator.initState.battleType == EBattleType.WaveDuel || _simulator.initState.battleType == EBattleType.WorldDuel)
					{
						message = string.Format(Localization.Get("1355"), Localization.Get("1908"));
					}
					else if (_simulator.initState.battleType == EBattleType.Annihilation)
					{
						message = Localization.Get("80010");
					}
					else if (_simulator.initState.battleType == EBattleType.WaveBattle)
					{
						message = Localization.Get("4810");
					}
					else if (_simulator.initState.battleType == EBattleType.CooperateBattle)
					{
						message = Localization.Get("80010");
					}
					else if (_simulator.initState.battleType == EBattleType.EventBattle)
					{
						message = Localization.Get("80010");
					}
					else if (_simulator.initState.battleType == EBattleType.EventRaid)
					{
						message = Localization.Get("6612");
					}
					else if (_simulator.initState.battleType == EBattleType.InfinityBattle)
					{
						message = Localization.Get("10000126");
					}
					UISimplePopup popup = UISimplePopup.CreateBool(localization: false, title, message, subMessage, Localization.Get("1304"), Localization.Get("1305"));
					popup.onClick = delegate(GameObject obj)
					{
						if (obj.name == "OK")
						{
							GameSetting.instance.repeatBattle = false;
							popup.ClosePopup();
							popup.Close();
							if (m03_Battle._simulator.initState.battleType == EBattleType.Raid || m03_Battle._simulator.initState.battleType == EBattleType.WaveBattle)
							{
								m03_Battle._simulator.isGiveUp = true;
								Manager<UIOptionController>.GetInstance().Hide();
							}
							else
							{
								if (m03_Battle._battleData != null)
								{
									m03_Battle._battleData.move = EBattleResultMove.MyTown;
									if (m03_Battle._simulator.initState.battleType == EBattleType.Plunder)
									{
										m03_Battle._battleData.move = EBattleResultMove.WorldMap;
									}
									else if (m03_Battle._simulator.initState.battleType == EBattleType.Raid)
									{
										m03_Battle._battleData.move = EBattleResultMove.Raid;
									}
									else if (m03_Battle._simulator.initState.battleType == EBattleType.Guerrilla)
									{
										m03_Battle._battleData.move = EBattleResultMove.Situation;
									}
									else if (m03_Battle._simulator.initState.battleType == EBattleType.Duel)
									{
										m03_Battle._battleData.move = EBattleResultMove.Challenge;
									}
									else if (m03_Battle._simulator.initState.battleType == EBattleType.WaveDuel)
									{
										m03_Battle._battleData.move = EBattleResultMove.WaveDuel;
									}
									else if (m03_Battle._simulator.initState.battleType == EBattleType.WorldDuel)
									{
										m03_Battle._battleData.move = EBattleResultMove.WorldDuel;
									}
									else if (m03_Battle._simulator.initState.battleType == EBattleType.Annihilation)
									{
										m03_Battle._battleData.move = EBattleResultMove.Annihilation;
									}
									else if (m03_Battle._simulator.initState.battleType == EBattleType.WaveBattle)
									{
										m03_Battle._battleData.move = EBattleResultMove.WaveBattle;
									}
									else if (m03_Battle._simulator.initState.battleType == EBattleType.Conquest)
									{
										m03_Battle._battleData.move = EBattleResultMove.Conquest;
									}
									else if (m03_Battle._simulator.initState.battleType == EBattleType.CooperateBattle)
									{
										m03_Battle._battleData.move = EBattleResultMove.CooperateBattle;
									}
									else if (m03_Battle._simulator.initState.battleType == EBattleType.EventBattle)
									{
										m03_Battle._battleData.move = EBattleResultMove.EventBattle;
									}
									else if (m03_Battle._simulator.initState.battleType == EBattleType.EventRaid)
									{
										m03_Battle._battleData.move = EBattleResultMove.EventRaid;
									}
									BattleData.Set(m03_Battle._battleData);
								}
								if (Time.timeScale != 1f)
								{
									Time.timeScale = 1f;
									SoundManager.SetPitchSFX(1f);
								}
								Loading.Load(Loading.WorldMap);
							}
						}
					};
				}
			}
		};
		Manager<UIOptionController>.GetInstance().onShow = delegate
		{
			Pause();
		};
		Manager<UIOptionController>.GetInstance().onHide = delegate
		{
			Resume();
		};
		ui.SetLhsInitWave(Simulator.ActiveTroopCount(_simulator.lhsTroops));
		ui.SetRhsInitWave((_simulator.initState.battleType == EBattleType.Annihilation) ? _simulator.rhsTroops.Count : Simulator.ActiveTroopCount(_simulator.rhsTroops));
		ui.SetLhsGuildSkills(_simulator.guildSkillState);
		if (_simulator.initState.battleType == EBattleType.Duel || _simulator.initState.battleType == EBattleType.WaveDuel || _simulator.initState.battleType == EBattleType.WorldDuel || _simulator.initState.battleType == EBattleType.Conquest)
		{
			ui.SetRhsGuildSkills(_simulator.initState.dualData._enemyGuildSkills);
		}
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		UIFade.In(1.3f);
		if (loading != null)
		{
			loading.GetComponent<UILoading>().Out();
		}
		float playTime = 5f;
		float speed = ConstValue.battleTerrainScrollSpeed;
		StartCoroutine(_PlayTroopEntryAnimation(base.lhsTroopAnchor, 10f, 0f, playTime));
		StartCoroutine(_PlayTroopEntryAnimation(base.rhsTroopAnchor, 10f, 0f, playTime));
		StartCoroutine(_ScrollSpeedTo(0.25f * speed, speed, playTime));
		ui.UICommanderEnable(EBattleSide.Right);
		yield return new WaitForSeconds(1f);
		if (_battleData != null && !_battleData.IsCleared)
		{
			if (_battleData.type == EBattleType.Plunder || _battleData.type == EBattleType.EventBattle)
			{
				EBattleDialogueEventType type = ((_battleData.type == EBattleType.Plunder) ? EBattleDialogueEventType.StartEvent : EBattleDialogueEventType.EventBattleScenario);
				yield return StartCoroutine(StartDialogueEvent(type));
				yield return new WaitForSeconds(0.8f);
			}
		}
		else
		{
			yield return new WaitForSeconds(1.2f);
		}
		StartOpening(1);
		while (_enteringTroopCount > 0)
		{
			yield return null;
		}
		yield return StartCoroutine(_PlayCommanderBoardAnimation());
		yield return StartCoroutine(_InitGameBattleItem());
		_isInit = true;
		ui.StartBattle();
	}

	protected void ShowdialogueEvent(EBattleDialogueEventType eventType)
	{
		string text = "Chapter";
		int num = 0;
		int num2 = 0;
		string text2 = string.Empty;
		if (_battleData != null)
		{
			if (eventType != EBattleDialogueEventType.EventBattleScenario)
			{
				num = int.Parse(_battleData.worldId);
				num2 = int.Parse(_battleData.stageId);
				if (num > 0)
				{
					num2 = num2 - (num - 1) * 20 - ConstValue.tutorialMaximumStage;
				}
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
			else if (IsShowFirstEventScenario())
			{
				text2 = eventScenarioData.scenarioIdx;
			}
		}
		if (string.IsNullOrEmpty(text2))
		{
			return;
		}
		if (eventType != EBattleDialogueEventType.EventBattleScenario)
		{
			if (DialoguerDataManager.HasDialogue(DialogueType.Origin, text2))
			{
				_dialogueState = EDialogueState.Play;
				UISetter.SetActive(UIManager.instance.battle.DialogMrg, active: true);
				UIManager.instance.battle.DialogMrg.InitWorldMapStart(text2);
			}
		}
		else if (DialoguerDataManager.HasDialogue(DialogueType.Event, text2))
		{
			_dialogueState = EDialogueState.Play;
			UISetter.SetActive(UIManager.instance.battle.DialogMrg, active: true);
			UIManager.instance.battle.DialogMrg.StartEventScenario();
			UIManager.instance.battle.DialogMrg.InitScenarioDialogue(text2, DialogueType.Event);
		}
	}

	private bool IsShowFirstEventScenario()
	{
		EventScenarioTimingType timingType = EventScenarioTimingType.BeforeBattle;
		if (_simulator.result != null && _simulator.result.IsWin && _battleData != null && !_battleData.isWin)
		{
			timingType = EventScenarioTimingType.AfterBattle;
		}
		else if (_battleData != null && _battleData.isWin)
		{
			timingType = EventScenarioTimingType.AfterBattleResult;
		}
		EventBattleScenarioDataRow eventBattleScenarioDataRow = RemoteObjectManager.instance.regulation.eventBattleScenarioDtbl.Find((EventBattleScenarioDataRow row) => row.eventIdx == _battleData.eventId.ToString() && row.eventType == _battleData.eventLevel.ToString() && row.timing == timingType);
		if (eventBattleScenarioDataRow == null)
		{
			return false;
		}
		eventScenarioData = eventBattleScenarioDataRow;
		int playTurn = eventBattleScenarioDataRow.playTurn;
		int lastShowEventScenarioPlayTurn = localUser.lastShowEventScenarioPlayTurn;
		if (playTurn > lastShowEventScenarioPlayTurn)
		{
			return true;
		}
		return false;
	}

	protected bool DialogueEndCheck()
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

	protected IEnumerator PlayTurnOut()
	{
		UISetter.SetSprite(viewData.openingIcon, "ig-turn-over");
		UISetter.SetActive(viewData.openingIcon, active: true);
		UISetter.SetActive(viewData.waveIcon, active: false);
		yield return StartCoroutine("_PlayOpeningAnimation");
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

	private IEnumerator CleanMemory()
	{
		yield return null;
		AssetBundleManager.DeleteAssetBundleAll();
	}

	private void Update()
	{
		if (!_isInit || _isEnd || netRequesting)
		{
			return;
		}
		ui.SetRemainTime(GetRemainedTime());
		switch (_state)
		{
		case State.Unknown:
		case State.Opening:
		case State.CutIn:
		case State.Result:
			return;
		}
		if (!DialogueEndCheck() || _enteringTroopCount > 0)
		{
			return;
		}
		if (_isNewWave && _battleData != null && !_battleData.IsCleared && _battleData.type == EBattleType.Plunder)
		{
			ShowdialogueEvent(EBattleDialogueEventType.WaveEvent);
			_isNewWave = false;
			if (_dialogueState == EDialogueState.Play)
			{
				return;
			}
		}
		if (!_isEnd && (_simulator.isEnded || (_simulator.isReplayMode && _simulator.result != null)))
		{
			if (_battleData != null && _battleData.type == EBattleType.WaveBattle)
			{
				if (_simulator.result.winSide == -1)
				{
					if (_simulator.result.IsWin)
					{
						InitState initState = InitState.Copy(_simulator.initState);
						initState._randomSeed = UnityEngine.Random.Range(0, 30000);
						initState.waveBattleData._wave = _simulator.frame._rhsWave + 1;
						initState.waveBattleData._waveTurn = _simulator.frame._waveTurn;
						initState.waveBattleData._lhsTurnLine = _simulator.frame._lhsTimeLine;
						initState.waveBattleData._lhsWaitingInput = false;
						initState.waveBattleData._lhsTurnUnitIndex = -1;
						if (_simulator.frame._isWaitingInput)
						{
							Unit unit = _simulator.frame.units[_simulator.frame.turnUnitIndex];
							if (unit.side == EBattleSide.Left)
							{
								initState.waveBattleData._lhsWaitingInput = true;
								initState.waveBattleData._lhsTurnUnitIndex = _simulator.frame.turnUnitIndex;
							}
						}
						for (int i = 0; i < initState.lhsTroops.Count; i++)
						{
							Troop troop = initState._lhsTroops[i];
							int lhsTroopStartIndex = _simulator.GetLhsTroopStartIndex(i);
							for (int j = 0; j < 9; j++)
							{
								Troop.Slot slot = troop._slots[j];
								Unit unit2 = _simulator.frame.units[lhsTroopStartIndex + j];
								if (unit2 == null)
								{
									continue;
								}
								UnitDataRow unitDr = RemoteObjectManager.instance.regulation.unitDtbl[unit2.dri];
								slot.health = unit2.health;
								for (int x = 0; x < 4; x++)
								{
									Skill skill = unit2.skills[x];
									if (skill != null)
									{
										int num = slot.skills.FindIndex((Troop.Slot.Skill t) => t.id == unitDr.skillDrks[x]);
										if (num >= 0)
										{
											Troop.Slot.Skill skill2 = slot.skills[num];
											skill2.sp = skill.sp;
										}
									}
								}
							}
						}
						WaveBattleDataRow waveBattleDataRow = RemoteObjectManager.instance.regulation.FindWaveBattleData(int.Parse(_battleData.stageId));
						List<EnemyUnitDataRow> data = RemoteObjectManager.instance.regulation.FindNextWaveBattleEnemy(waveBattleDataRow.enemy.ToString(), initState.waveBattleData._wave);
						initState._rhsTroops = RoTroop.CreateWaveBattleTroop(data);
						Simulator simulator = _simulator;
						UnitRenderer[] unitRenderers = _unitRenderers;
						netRequesting = true;
						JToken jToken = (JToken)_simulator.record;
						JToken jToken2 = (JToken)_simulator.result;
						string text = jToken.ToString(Formatting.None);
						if (_battleData.type == EBattleType.EventBattle)
						{
							RemoteObjectManager.instance.RequestEventBattleOut(_battleData.type, _simulator.result.checksum, jToken.ToString(Formatting.None), jToken2.ToString(Formatting.None), _battleData.eventId, _battleData.eventLevel);
						}
						else
						{
							RemoteObjectManager.instance.RequestBattleOut(_battleData.type, _simulator.result.checksum, jToken.ToString(Formatting.None), jToken2.ToString(Formatting.None));
						}
						_simulator = Simulator.Create(RemoteObjectManager.instance.regulation, initState);
						_simulator.record._option.enableEffect = simulator.option.enableEffect;
						_unitRenderers = new UnitRenderer[_simulator.unitCount];
						for (int k = 0; k < _simulator.lhsTroops.Count; k++)
						{
							int lhsTroopStartIndex2 = _simulator.GetLhsTroopStartIndex(k);
							for (int l = 0; l < 9; l++)
							{
								_unitRenderers[lhsTroopStartIndex2 + l] = unitRenderers[lhsTroopStartIndex2 + l];
								Unit value = simulator.frame.units[lhsTroopStartIndex2 + l];
								_simulator.frame._units[lhsTroopStartIndex2 + l] = value;
							}
						}
						_unitRendererCreator = _UnitRendererCreator.Create(this);
						_unitRendererCreator.onClick = OnSelect;
						_projectileRendererCreator = _ProjectileRendererCreator.Create(this);
						_unitRendererUpdater = _UnitRendererUpdater.Create(this);
						_skillIconUpdater = _SkillIconUpdater.Create(this);
						_skillIconUpdater._unitRenderers = _unitRenderers;
						_turnUiUpdater = _TurnUiUpdater.Create(this);
						_turnUiUpdater.renderers = _unitRenderers;
						_turnUiUpdater.input = null;
						_inputCorrector = _InputCorrector.Create(this);
						_inputCorrector.input = null;
						_inputCorrector.isAuto = _isAuto;
						_simulator.AccessFrame(_unitRendererCreator);
						_simulator.AccessFrame(_unitRendererUpdater);
						float num2 = 5f;
						float battleTerrainScrollSpeed = ConstValue.battleTerrainScrollSpeed;
						StartCoroutine(_PlayTroopEntryAnimation(base.rhsTroopAnchor, 10f, 0f, num2));
						StartCoroutine(_ScrollSpeedTo(0.25f * battleTerrainScrollSpeed, battleTerrainScrollSpeed, num2));
						ui.SetRhsCurrWave(_simulator.frame._rhsWave);
						StartOpening(_simulator.frame._rhsWave);
						_isNewWave = true;
						AssetBundleManager.DeleteAssetBundleAllWithoutBgm();
						Resources.UnloadUnusedAssets();
						GC.Collect();
						GC.WaitForPendingFinalizers();
						return;
					}
				}
				else
				{
					StartCoroutine(_PlayResultAnimation(_simulator.result));
				}
			}
			_isEnd = true;
			if ((_state == State.Playing || _state == State.InputWait) && _battleData != null)
			{
				EBattleType type = _battleData.type;
				if (type == EBattleType.ScenarioBattle)
				{
					StartCoroutine(_PlayScenarioBattleResultAnimation(_simulator.result));
				}
				else
				{
					StartCoroutine(_PlayResultAnimation(_simulator.result));
				}
			}
			return;
		}
		Shared.Battle.Input input = null;
		Shared.Battle.Input rhsInput = null;
		if (!_simulator.isReplayMode)
		{
			if (_simulator.option.waitingInputMode)
			{
				if (_state == State.InputWait)
				{
					_TryPickTarget();
				}
			}
			else
			{
				_TryPickTarget();
			}
			input = _GetCorrectedInput();
			if (_simulator.option.waitingInputMode && _state == State.InputWait && input == null)
			{
				return;
			}
		}
		_state = State.Playing;
		float num3 = _timeGameObject.TimedDeltaTime();
		_timeStack += (int)(num3 * 1000f);
		bool flag = false;
		while (!flag && _timeStack >= 66)
		{
			_timeStack -= 66;
			if (_unitRendererUpdater.delay > 0f)
			{
				_unitRendererUpdater.delay -= 66f;
				continue;
			}
			if (_unitRendererUpdater.pause)
			{
				continue;
			}
			_simulator.Step(input, rhsInput);
			if (input != null && input.result && HasInputUnit(input.unitIndex))
			{
				DeleteInputUnit(input.unitIndex);
			}
			_input = null;
			Frame frame = _simulator.frame;
			_simulator.AccessFrame(_unitRendererCreator);
			_simulator.AccessFrame(_skillIconUpdater);
			if (_simulator.initState.battleType == EBattleType.Raid || _simulator.initState.battleType == EBattleType.CooperateBattle || _simulator.initState.battleType == EBattleType.EventRaid)
			{
				ui.SetScore(_simulator.frame.totalAttackDamage);
			}
			if (_unitRendererCreator.createdLhsTroopIndex >= 0 || _unitRendererCreator.createdRhsTroopIndex >= 0)
			{
				flag = true;
				if (_simulator.initState.battleType == EBattleType.WaveDuel)
				{
					AssetBundleManager.DeleteAssetBundleAllWithoutBgm();
					Resources.UnloadUnusedAssets();
					GC.Collect();
					GC.WaitForPendingFinalizers();
				}
				StartCoroutine(_TryPlayTroopEntryAnimation(_unitRendererCreator));
			}
			_simulator.AccessFrame(_projectileRendererCreator);
			_simulator.AccessFrame(_unitRendererUpdater);
			_UpdateTurnUi(null);
			if (!_simulator.isReplayMode && frame.isWaitingLhsInput)
			{
				_state = State.InputWait;
				flag = true;
			}
		}
	}

	private IEnumerator _PlayCutInEffect()
	{
		eCutInState = E_CUTIN_STATE.PLAYING;
		yield return null;
		SplitScreenManager ssm = _splitScreenManager;
		CutInEffect cie = _unitRendererUpdater.cutInEffect;
		GameObject black = cie.transform.Find("Black").gameObject;
		string parentPath = "SceneRoot/UI Root/Battle/CutInEffect/" + cie.side;
		cie.transform.parent = GameObject.Find(parentPath).transform;
		cie.transform.localScale = Vector3.one;
		cie.unitRenderer.SetRenderQueueForCutIn();
		Vector3 prevUnitScale = cie.unitRenderer.transform.localScale;
		if (cie.isDefault)
		{
			cie.unitRenderer.transform.localScale *= 1.3f;
		}
		GameObject unitPanel = GameObject.Find("SceneRoot/UI Root/Battle/UnitPanel");
		unitPanel.SetActive(value: false);
		Transform cieCamera = cie.transform.Find("Camera");
		Transform srcCamera = ssm.left.camera.transform;
		if (cie.side == CutInEffect.Side.Right)
		{
			srcCamera = ssm.right.camera.transform;
		}
		int TargetSlot = 7;
		Vector3 cameraOffSet = Vector3.zero;
		int slotIdx = cie.unitRenderer.unitIdx % 9;
		Transform cieSplitLine = cie.transform.Find("SplitLine");
		Transform srcSplitLine = GameObject.Find("SceneRoot/Main Camera/SplitLine").transform;
		while (!cie.isEnded)
		{
			srcCamera.localPosition = cieCamera.localPosition + cameraOffSet;
			srcCamera.localEulerAngles = cieCamera.localEulerAngles;
			srcSplitLine.localPosition = cieSplitLine.localPosition;
			srcSplitLine.localEulerAngles = cieSplitLine.localEulerAngles;
			ssm.left.black.SetActive(black.activeSelf);
			ssm.right.black.SetActive(black.activeSelf);
			yield return null;
		}
		yield return null;
		srcCamera.localPosition = Vector3.zero;
		srcCamera.localEulerAngles = Vector3.zero;
		srcSplitLine.localPosition = Vector3.zero;
		srcSplitLine.localEulerAngles = Vector3.zero;
		unitPanel.SetActive(value: true);
		ssm.left.black.SetActive(value: false);
		ssm.right.black.SetActive(value: false);
		cie.unitRenderer.transform.localScale = prevUnitScale;
		cie.unitRenderer.ResetRenderQueue();
		cie.transform.parent = null;
		UnityEngine.Object.Destroy(cie.gameObject);
		_unitRendererUpdater.cutInEffect = null;
		eCutInState = E_CUTIN_STATE.IDLE;
	}

	private void _UpdateTurnUi(Shared.Battle.Input input)
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

	private IEnumerator _InitGameBattleItem()
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

	private IEnumerator _PlayCommanderBoardAnimation()
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

	private IEnumerator _PlayOpeningAnimation()
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
			ui.SetUnitPanelEnable(enable: true);
		}
		_state = State.Playing;
		ui.CloseOpeninig();
	}

	private IEnumerator StartDialogueEvent(EBattleDialogueEventType type)
	{
		ShowdialogueEvent(type);
		while (_dialogueState == EDialogueState.Play)
		{
			DialogueEndCheck();
			yield return null;
		}
	}

	private IEnumerator _ScrollSpeedTo(float from, float to, float duration)
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

	private IEnumerator _PlayResultAnimation(Result result)
	{
		_state = State.Result;
		if (!result.IsWin && _simulator.isTurnOut)
		{
			yield return new WaitForSeconds(0.5f);
			yield return PlayTurnOut();
			yield return new WaitForSeconds(1.5f);
		}
		else
		{
			yield return new WaitForSeconds(2.7f);
		}
		if (result.IsWin && _battleData != null && !_battleData.IsCleared && (_battleData.type == EBattleType.Plunder || _battleData.type == EBattleType.EventBattle))
		{
			EBattleDialogueEventType type = ((_battleData.type != EBattleType.Plunder) ? EBattleDialogueEventType.EventBattleScenario : EBattleDialogueEventType.BattleWinEvent);
			ShowdialogueEvent(type);
			while (!DialogueEndCheck())
			{
				yield return null;
			}
		}
		UIFade.Out(0.5f);
		yield return new WaitForSeconds(0.5f);
		Time.timeScale = 1f;
		SoundManager.SetPitchSFX(1f);
		bool isWin = result.winSide < 0;
		if (_simulator.initState.battleType == EBattleType.Annihilation && _simulator.isTurnOut)
		{
			isWin = true;
		}
		UIBattleResult uiResult = UIManager.instance.battle.BattleResult;
		uiResult.SetBattleResult(result);
		uiResult.SetMainCommanderUnit(lastUserUnit);
		uiResult.lhsTroopIdx = Simulator.GetLhsTroopIndex(_simulator.frame._lhsTroopStartIndex);
		if (uiResult.lhsTroopIdx < 0)
		{
			uiResult.lhsTroopIdx = Simulator.ActiveTroopCount(_simulator.lhsTroops) - 1;
		}
		if (_battleData != null)
		{
			if (_simulator.isReplayMode)
			{
				if (_battleData.type == EBattleType.Duel)
				{
					if (isReplayMode)
					{
						if (_battleData.move == EBattleResultMove.Undefined)
						{
							_battleData.move = EBattleResultMove.Challenge;
						}
						BattleData.Set(_battleData);
						Loading.Load(Loading.WorldMap);
					}
					else if (_battleData.dualResult != null)
					{
						uiResult.duelResult.SetBattleTime((float)UIManager.instance.battle.Simulator.frame.time / 1000f);
						uiResult.duelResult.SetRank(_battleData.dualResult.user.rank);
						uiResult.duelResult.SetRankPersent(_battleData.dualResult.user.rankPercent);
						uiResult.duelResult.SetBaseScore(_battleData.dualResult.user.prevScore);
						uiResult.duelResult.SetGetScore(_battleData.dualResult.user.getScore);
						uiResult.duelResult.SetCurScore(_battleData.dualResult.user.curScore);
						uiResult.duelResult.SetWinScore(_battleData.dualResult.user.winScore);
						if (isWin)
						{
							uiResult.duelResult.SetWinSocreName(_battleData.dualResult.user.winCount);
						}
						else
						{
							uiResult.duelResult.SetLoseStreakName(_battleData.dualResult.user.winCount);
						}
						_battleData.isWin = isWin;
						_battleData.clearRank = result.clearRank;
						_battleData.dualResult = null;
						BattleData.Set(_battleData);
						uiResult.onClick = _OnClickBattleResult;
						uiResult.Set(_battleData);
						uiResult.Open();
					}
				}
				else if (_battleData.type == EBattleType.WaveDuel)
				{
					if (isReplayMode)
					{
						if (_battleData.move == EBattleResultMove.Undefined)
						{
							_battleData.move = EBattleResultMove.WaveDuel;
						}
						BattleData.Set(_battleData);
						Loading.Load(Loading.WorldMap);
					}
					else if (_battleData.dualResult != null)
					{
						uiResult.waveDuelResult.SetRank(_battleData.dualResult.user.prevRank, _battleData.dualResult.user.rank);
						_battleData.isWin = isWin;
						_battleData.clearRank = result.clearRank;
						_battleData.dualResult = null;
						BattleData.Set(_battleData);
						uiResult.onClick = _OnClickBattleResult;
						uiResult.Set(_battleData);
						uiResult.Open();
					}
				}
				else if (_battleData.type == EBattleType.WorldDuel)
				{
					if (isReplayMode)
					{
						if (_battleData.move == EBattleResultMove.Undefined)
						{
							_battleData.move = EBattleResultMove.WorldDuel;
						}
						BattleData.Set(_battleData);
						Loading.Load(Loading.WorldMap);
					}
					else if (_battleData.dualResult != null)
					{
						uiResult.worldDuelResult.SetBaseScore(_battleData.dualResult.user.prevScore);
						uiResult.worldDuelResult.SetGetScore(_battleData.dualResult.user.getScore);
						uiResult.worldDuelResult.SetCurScore(_battleData.dualResult.user.curScore);
						_battleData.isWin = isWin;
						_battleData.clearRank = result.clearRank;
						_battleData.dualResult = null;
						BattleData.Set(_battleData);
						uiResult.onClick = _OnClickBattleResult;
						uiResult.Set(_battleData);
						uiResult.Open();
					}
				}
				else
				{
					if (_battleData.move == EBattleResultMove.Undefined)
					{
						_battleData.move = EBattleResultMove.MyTown;
					}
					BattleData.Set(_battleData);
					Loading.Load(Loading.WorldMap);
				}
				yield break;
			}
			if (_battleData.type == EBattleType.CooperateBattle)
			{
				uiResult.coopBattleResult.SetDamage(result.totalAttackDamage);
			}
			else if (_battleData.type == EBattleType.EventRaid)
			{
				long num = _simulator.initState.rhsTroopsMaxHealth - _simulator.initState.rhsTroopsHealth;
				if (num < 0)
				{
					num = 0L;
				}
				long totalDamage = num + result.totalAttackDamage;
				uiResult.eventRaidResult.SetBaseDamage(num);
				uiResult.eventRaidResult.SetCurDamage(result.totalAttackDamage);
				uiResult.eventRaidResult.SetTotalDamage(totalDamage);
			}
			if (_battleData.type == EBattleType.WaveBattle)
			{
				_battleData.WaveCount = _simulator.record.initState.waveBattleData._wave;
			}
			_battleData.unitKillCount = result.armyDestoryCnt;
			_battleData.isWin = isWin;
			_battleData.clearRank = result.clearRank;
			BattleData.Set(_battleData);
			uiResult.onClick = _OnClickBattleResult;
			JToken jToken = (JToken)_simulator.record;
			JToken jToken2 = (JToken)result;
			RemoteObjectManager.instance.RequestBattleOut(_battleData.type, result.checksum, jToken.ToString(Formatting.None), jToken2.ToString(Formatting.None));
		}
		else
		{
			uiResult.Set(_simulator, _simulator.regulation);
			uiResult.Open();
		}
	}

	private IEnumerator _PlayScenarioBattleResultAnimation(Result result)
	{
		_state = State.Result;
		if (!_simulator.result.IsWin && _simulator.isTurnOut)
		{
			yield return new WaitForSeconds(0.5f);
			yield return PlayTurnOut();
			yield return new WaitForSeconds(1.5f);
		}
		else
		{
			yield return new WaitForSeconds(2.7f);
		}
		if (_simulator.result.IsWin && _battleData != null && !_battleData.IsCleared && _battleData.type == EBattleType.Plunder)
		{
			ShowdialogueEvent(EBattleDialogueEventType.BattleWinEvent);
			while (!DialogueEndCheck())
			{
				yield return null;
			}
		}
		UIFade.Out(0.5f);
		yield return new WaitForSeconds(0.5f);
		Time.timeScale = 1f;
		SoundManager.SetPitchSFX(1f);
		bool isWin = _simulator.result.winSide < 0;
		if (_battleData != null)
		{
			_battleData.unitKillCount = result.armyDestoryCnt;
			_battleData.isWin = isWin;
			_battleData.clearRank = result.clearRank;
			BattleData.Set(_battleData);
			_ = (JToken)_simulator.record;
			_ = (JToken)result;
			RemoteObjectManager.instance.RequestCompleteCommanderScenario(localUser.currScenario.commanderId, localUser.currScenario.scenarioId, localUser.currScenario.scenarioQuarterId);
		}
	}

	public void OnBattleOutResult()
	{
		netRequesting = false;
	}

	private void _OnClickBattleResult(GameObject sender)
	{
		SoundManager.PlaySFX("BTN_Norma_001");
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
		case "WaveDuel":
			_battleData.move = EBattleResultMove.WaveDuel;
			break;
		case "WorldDuel":
			_battleData.move = EBattleResultMove.WorldDuel;
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
		case "Situation":
			_battleData.move = EBattleResultMove.Situation;
			break;
		case "Annihilation":
			_battleData.move = EBattleResultMove.Annihilation;
			break;
		case "NextAnnihilation":
			_battleData.move = EBattleResultMove.NextAnnihilation;
			break;
		case "WaveBattle":
			_battleData.move = EBattleResultMove.WaveBattle;
			break;
		case "CooperateBattle":
			_battleData.move = EBattleResultMove.CooperateBattle;
			break;
		case "EventBattle":
			_battleData.move = EBattleResultMove.EventBattle;
			break;
		case "EventBattleRetry":
			_battleData.move = EBattleResultMove.EventBattleRetry;
			break;
		case "EventRaid":
			_battleData.move = EBattleResultMove.EventRaid;
			break;
		case "EventRaidRetry":
			_battleData.move = EBattleResultMove.EventRaidRetry;
			break;
		case "RetryInfinityBattle":
			_battleData.move = EBattleResultMove.InfinityBattleRetry;
			break;
		case "InfinityBattle":
			_battleData.move = EBattleResultMove.InfinityBattle;
			break;
		}
		Loading.Load(Loading.WorldMap);
	}

	private bool _TryPickTarget()
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

	private Shared.Battle.Input _GetCorrectedInput()
	{
		_inputCorrector.input = ((_input != null) ? Shared.Battle.Input.Copy(_input) : null);
		_inputCorrector.isAuto = _isAuto;
		_simulator.AccessFrame(_inputCorrector);
		return _inputCorrector.input;
	}

	private IEnumerator _TryPlayTroopEntryAnimation(_UnitRendererCreator urc)
	{
		if (urc.createdLhsTroopIndex >= 0)
		{
			StartCoroutine(_PlayTroopEntryAnimation(base.lhsTroopAnchor, 10f, 2f, 2f));
			_state = State.Opening;
			while (_enteringTroopCount > 0)
			{
				yield return null;
			}
			_state = State.Playing;
			ui.CloseOpeninig();
			ui.StartBattle();
		}
		if (urc.createdRhsTroopIndex >= 0)
		{
			StartCoroutine(_PlayTroopEntryAnimation(base.rhsTroopAnchor, 10f, 2f, 2f));
			ui.UICommanderEnable(EBattleSide.Right);
		}
		yield return null;
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

	private IEnumerator _PlayTroopEntryAnimation(Transform anchor, float entryDistance, float delay, float playTime)
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

	private void _SetLeftTerrainTheme(string theme)
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

	private void _SetRightTerrainTheme(string theme)
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

	private void _SetTerrainScrollSpeed(float speed)
	{
		TerrainScroller terrainScroller = UIManager.instance.battle.LeftView.terrainScroller;
		terrainScroller.speed = speed;
		TerrainScroller terrainScroller2 = UIManager.instance.battle.RightView.terrainScroller;
		terrainScroller2.speed = speed;
	}

	private Simulator _CreateSimulator(BattleData bd, Regulation rg)
	{
		EBattleType eBattleType = EBattleType.Undefined;
		RaidData raidData = null;
		string empty = string.Empty;
		List<string> list = null;
		List<Troop> list2 = null;
		List<Troop> list3 = null;
		List<GuildSkillState> list4 = null;
		List<GuildSkillState> list5 = null;
		List<string> list6 = null;
		List<string> list7 = null;
		if (bd != null && (bd.isReplayMode || (bd.type == EBattleType.WaveDuel && bd.dualResult != null) || (bd.type == EBattleType.Duel && bd.dualResult != null) || (bd.type == EBattleType.WorldDuel && bd.dualResult != null)))
		{
			return Simulator.Create(rg, bd.record);
		}
		if (bd != null)
		{
			eBattleType = bd.type;
			empty = bd.stageId;
			switch (eBattleType)
			{
			case EBattleType.Raid:
				raidData = bd.raidData.ToBattleRaidData();
				break;
			case EBattleType.Guerrilla:
				empty = $"{bd.sweepType}_{bd.sweepLevel}";
				break;
			case EBattleType.EventBattle:
				empty = $"{bd.eventId}_{bd.eventLevel}";
				break;
			case EBattleType.EventRaid:
				empty = $"{bd.eventId}_{bd.eventRaidIdx}";
				break;
			case EBattleType.Duel:
			case EBattleType.WaveDuel:
			case EBattleType.Conquest:
			case EBattleType.WorldDuel:
				if (bd.defender.guildSkillList != null)
				{
					list5 = new List<GuildSkillState>();
					for (int i = 0; i < bd.defender.guildSkillList.Count; i++)
					{
						if (bd.defender.guildSkillList[i].skillLevel > 0)
						{
							list5.Add(bd.defender.guildSkillList[i].ToBattleGuildSkillData());
						}
					}
				}
				if (bd.defender.completeRewardGroupList != null)
				{
					list7 = new List<string>();
					for (int j = 0; j < bd.defender.completeRewardGroupList.Count; j++)
					{
						list7.Add(bd.defender.completeRewardGroupList[j].ToString());
					}
				}
				break;
			}
			list2 = new List<Troop>();
			for (int k = 0; k < bd.attacker.battleTroopList.Count; k++)
			{
				list2.Add(bd.attacker.battleTroopList[k].ToBattleTroop());
			}
			list3 = new List<Troop>();
			switch (eBattleType)
			{
			case EBattleType.Raid:
			{
				RaidChallengeDataRow raidChallengeDataRow = rg.raidChallengeDtbl[raidData.raidId.ToString()];
				CommanderDataRow commanderDataRow = rg.commanderDtbl[raidChallengeDataRow.commanderId];
				UnitDataRow unitDataRow = rg.unitDtbl[commanderDataRow.unitId];
				Troop troop = new Troop();
				troop._slots = new List<Troop.Slot>();
				troop._slots.AddRange(new Troop.Slot[9]);
				Troop.Slot slot = new Troop.Slot();
				slot.charType = 5;
				slot.id = unitDataRow.key;
				slot.health = unitDataRow.maxHealth;
				troop._slots[raidChallengeDataRow.commanderPos] = slot;
				for (int m = 0; m < raidChallengeDataRow.parts.Count; m++)
				{
					if (!string.IsNullOrEmpty(raidChallengeDataRow.parts[m]) && !(raidChallengeDataRow.parts[m] == "0") && troop._slots[raidChallengeDataRow.partsPosition[m]] == null)
					{
						unitDataRow = rg.unitDtbl[raidChallengeDataRow.parts[m]];
						slot = new Troop.Slot();
						slot.charType = 6;
						slot.id = unitDataRow.key;
						slot.health = unitDataRow.maxHealth;
						slot.partIdx = m;
						slot.mainIdx = raidChallengeDataRow.commanderPos;
						troop._slots[raidChallengeDataRow.partsPosition[m]] = slot;
					}
				}
				list3.Add(troop);
				break;
			}
			case EBattleType.WaveBattle:
			{
				WaveBattleDataRow waveBattleDataRow = rg.FindWaveBattleData(int.Parse(bd.stageId));
				List<EnemyUnitDataRow> data = rg.FindNextWaveBattleEnemy(waveBattleDataRow.enemy.ToString(), 1);
				list3 = RoTroop.CreateWaveBattleTroop(data);
				break;
			}
			case EBattleType.CooperateBattle:
			{
				CooperateBattleDataRow cooperateBattleDataRow = rg.cooperateBattleDtbl[empty];
				UnitDataRow unitDataRow2 = rg.unitDtbl[cooperateBattleDataRow.enemy];
				Troop troop2 = new Troop();
				troop2._slots = new List<Troop.Slot>();
				troop2._slots.AddRange(new Troop.Slot[9]);
				Troop.Slot slot2 = new Troop.Slot();
				slot2.charType = 5;
				slot2.id = unitDataRow2.key;
				slot2.level = cooperateBattleDataRow.enemyLevel;
				slot2.cls = cooperateBattleDataRow.enemyClass;
				slot2.health = unitDataRow2.maxHealth;
				troop2._slots[cooperateBattleDataRow.enemyrPos - 1] = slot2;
				if (cooperateBattleDataRow.enemyType == ECooperateBattleEnemyType.Boss)
				{
					for (int n = 0; n < cooperateBattleDataRow.parts.Count; n++)
					{
						if (!string.IsNullOrEmpty(cooperateBattleDataRow.parts[n]) && !(cooperateBattleDataRow.parts[n] == "0") && troop2._slots[cooperateBattleDataRow.partsPos[n] - 1] == null)
						{
							unitDataRow2 = rg.unitDtbl[cooperateBattleDataRow.parts[n]];
							slot2 = new Troop.Slot();
							slot2.charType = 6;
							slot2.id = unitDataRow2.key;
							slot2.level = cooperateBattleDataRow.enemyLevel;
							slot2.cls = cooperateBattleDataRow.enemyClass;
							slot2.health = unitDataRow2.maxHealth;
							slot2.partIdx = n;
							slot2.mainIdx = cooperateBattleDataRow.enemyrPos - 1;
							troop2._slots[cooperateBattleDataRow.partsPos[n] - 1] = slot2;
						}
					}
				}
				list3.Add(troop2);
				break;
			}
			default:
			{
				for (int l = 0; l < bd.defender.battleTroopList.Count; l++)
				{
					list3.Add(bd.defender.battleTroopList[l].ToBattleTroop());
				}
				break;
			}
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
					for (int num2 = 1; num2 <= count2; num2++)
					{
						num += num2;
					}
					int num3 = 0;
					for (int num4 = 0; num4 < list3.Count; num4++)
					{
						Troop troop3 = list3[num4];
						int num5 = 0;
						if (num4 == list3.Count - 1)
						{
							num5 = count - num3;
						}
						else
						{
							int num6 = (num4 + 1) * 100 / num;
							num5 = count * num6 / 100;
						}
						if (num5 > 0)
						{
							num3 += num5;
							List<Troop.Slot> list8 = troop3._slots.FindAll((Troop.Slot x) => x != null && !string.IsNullOrEmpty(x.id));
							for (int num7 = 0; num7 < num5; num7++)
							{
								int index = UnityEngine.Random.Range(0, list8.Count);
								list8[index].dropItemCnt++;
							}
						}
					}
				}
			}
			list = new List<string>();
			RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
			if (roLocalUser.IsExistGuild() && roLocalUser.guildInfo.idx != 0 && roLocalUser.guildSkillList != null)
			{
				list4 = new List<GuildSkillState>();
				for (int num8 = 0; num8 < roLocalUser.guildSkillList.Count; num8++)
				{
					GuildSkillState guildSkillState = roLocalUser.guildSkillList[num8].ToBattleGuildSkillData();
					if (guildSkillState != null && guildSkillState._skillLevel > 0)
					{
						list4.Add(guildSkillState);
					}
				}
			}
			if (roLocalUser.completeRewardGroupList != null)
			{
				list6 = new List<string>();
				for (int num9 = 0; num9 < roLocalUser.completeRewardGroupList.Count; num9++)
				{
					list6.Add(roLocalUser.completeRewardGroupList[num9].ToString());
				}
			}
		}
		else
		{
			empty = "1";
			string[] array = new string[1] { "5017" };
			List<string> unitList = new List<string>();
			RemoteObjectManager.instance.regulation.GetUnitList(EBranch.Army, includeHidden: true).ForEach(delegate(UnitDataRow row)
			{
				unitList.Add(row.key);
			});
			array = unitList.ToArray();
			list2 = _CreateTestTroops("Army", new string[1] { "5002" }, array, 1, 1, 5, 5);
			list3 = _CreateTestTroops("Army", new string[1] { "30001" }, array, 1, 1, 5, 5);
			int num10 = 5;
			int count3 = list3.Count;
			int num11 = 0;
			for (int num12 = 1; num12 <= count3; num12++)
			{
				num11 += num12;
			}
			int num13 = 0;
			for (int num14 = 0; num14 < list3.Count; num14++)
			{
				Troop troop4 = list3[num14];
				int num15 = 0;
				if (num14 == list3.Count - 1)
				{
					num15 = num10 - num13;
				}
				else
				{
					int num16 = (num14 + 1) * 100 / num11;
					num15 = num10 * num16 / 100;
				}
				if (num15 <= 0)
				{
					continue;
				}
				num13 += num15;
				List<Troop.Slot> list9 = troop4._slots.FindAll((Troop.Slot x) => x != null && !string.IsNullOrEmpty(x.id));
				if (list9.Count > 0)
				{
					for (int num17 = 0; num17 < num15; num17++)
					{
						int index2 = UnityEngine.Random.Range(0, list9.Count);
						list9[index2].dropItemCnt++;
					}
				}
			}
			list = new List<string>();
			list.Add("1");
			list.Add("2");
			list.Add("3");
			list.Add("4");
		}
		int randomSeed = UnityEngine.Random.Range(0, 30000);
		InitState initState = InitState.Create(eBattleType, list2, list3, list, randomSeed);
		initState._stageId = empty;
		initState._guildSkills = list4;
		initState._groupBuffs = list6;
		switch (eBattleType)
		{
		case EBattleType.Raid:
			initState._raidData = raidData;
			break;
		case EBattleType.Duel:
		case EBattleType.WaveDuel:
		case EBattleType.Conquest:
		case EBattleType.WorldDuel:
			initState._dualData = new DualData();
			initState._dualData._playerName = bd.attacker.nickname;
			initState._dualData._playerLevel = bd.attacker.level;
			initState._dualData._playerGuildName = bd.attacker.guildName;
			if (bd.defender.duelRanking != 0)
			{
				initState._dualData._enemyName = bd.defender.nickname;
			}
			else
			{
				initState._dualData._enemyName = Localization.Get(bd.defender.nickname);
			}
			initState._dualData._enemyGuildName = bd.defender.guildName;
			initState._dualData._enemyLevel = bd.defender.level;
			initState._dualData._enemyRank = bd.defender.duelRanking;
			initState._dualData._enemyGuildSkills = list5;
			initState._dualData._enemyUno = bd.defender.uno;
			initState._dualData._enemyGroupBuffs = list7;
			if (eBattleType == EBattleType.WorldDuel)
			{
				initState._dualData._worldDuelData = new WorldDuelData();
				initState._dualData._worldDuelData._playerWorld = bd.attacker.world;
				initState._dualData._worldDuelData._playerBuffs = bd.attacker.GetBuffIdxList();
				initState._dualData._worldDuelData._enemyWorld = bd.defender.world;
				initState._dualData._worldDuelData._enemyBuffs = bd.defender.GetBuffIdxList();
			}
			break;
		case EBattleType.WaveBattle:
			initState._waveBattleData = WaveBattleData.Create(BattleData.stageId, 1, 0);
			break;
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

	private void _OnClickMain(GameObject sender)
	{
		SoundManager.PlaySFX("BTN_Norma_001");
		string text = sender.name;
		Frame frame = _simulator.frame;
		float num = -1f;
		Shared.Battle.Input input = null;
		switch (text)
		{
		case "Speed-x1":
			num = 2f;
			GameSetting.instance.speedUp = true;
			break;
		case "Speed-x2":
			num = defaultTimeScale;
			GameSetting.instance.speedUp = false;
			break;
		case "Auto":
			_isAuto = true;
			GameSetting.instance.autoSkill = _isAuto;
			break;
		case "Manual":
			_isAuto = false;
			GameSetting.instance.autoSkill = _isAuto;
			break;
		case "AutoLock":
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("17082"));
			break;
		case "Pause":
			if (Manager<UIOptionController>.GetInstance().isShow)
			{
				Manager<UIOptionController>.GetInstance().Hide();
			}
			else if (!_simulator.isGiveUp && ((_simulator.initState.battleType != EBattleType.Duel && _simulator.initState.battleType != EBattleType.WaveDuel && _simulator.initState.battleType != EBattleType.WorldDuel) || isReplayMode))
			{
				Manager<UIOptionController>.GetInstance().Show();
			}
			break;
		case "Skip":
		{
			if (_state == State.Result)
			{
				break;
			}
			int rhsTroopStartIndex = _simulator.GetRhsTroopStartIndex(_simulator.rhsTroops.Count - 1);
			_simulator.frame._rhsTroopStartIndex = rhsTroopStartIndex;
			for (int i = 0; i < frame.units.Count; i++)
			{
				Unit unit = frame.units[i];
				if (unit != null && unit.side == EBattleSide.Right)
				{
					unit._health = 0;
					unit._isDead = true;
				}
			}
			_simulator.mission.MissionAllSuccess();
			_state = State.Playing;
			_dialogueState = EDialogueState.None;
			_enteringTroopCount = 0;
			_isInit = true;
			break;
		}
		case "Result":
			if (_state != State.Result && _battleData != null && _battleData.record != null)
			{
				StartCoroutine(_PlayResultAnimation(_battleData.record.result));
			}
			break;
		case "Repeat":
			if (GameSetting.instance.repeatBattle)
			{
				UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), Localization.Get("10000005"), string.Empty, Localization.Get("10000006"), Localization.Get("1000"));
				uISimplePopup.onClick = delegate(GameObject obj)
				{
					if (obj.name == "OK")
					{
						GameSetting.instance.repeatBattle = false;
						UISetter.SetFlipSwitch(ui.flipRepeat, GameSetting.instance.repeatBattle);
					}
				};
			}
			else
			{
				GameSetting.instance.repeatBattle = true;
				UISetter.SetFlipSwitch(ui.flipRepeat, GameSetting.instance.repeatBattle);
			}
			break;
		}
		if (!_isPause && num > 0f)
		{
			SoundManager.SetPitchSFX(num);
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

	protected void OnSelect(int unitIndex)
	{
		if (CanUserInput())
		{
			SetInputUnit(unitIndex);
		}
	}

	private void OnSelect(int unitIndex, int skillIndex)
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

	private void OnDestroy()
	{
	}
}
