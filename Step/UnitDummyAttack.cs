using System.Collections;
using System.Collections.Generic;
using Cache;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

namespace Step
{
	public class UnitDummyAttack : AbstractStepAction
	{
		public class DummyProjectile
		{
			public string projectileKey;

			public int firePhaseId;

			public int hitPhaseId;

			public int beHitId;

			public bool isMiss;

			public bool isCri;

			public int hitTime;
		}

		public E_SIDE side;

		public int unitPosition;

		public int targetPosition;

		public int skillIdx;

		public string projectileKey;

		[Range(0f, 100f)]
		public float missRate;

		[Range(0f, 100f)]
		public float criRate;

		protected M04_Tutorial main;

		protected Regulation reg;

		protected UnitRenderer unitRenderer;

		protected UnitRenderer targetUnitRenderer;

		protected Skill skill;

		protected Unit unit;

		protected UnitDataRow unitDr;

		protected SkillDataRow skillDr;

		protected UnitMotionDataRow unitMotionDr;

		public override bool IsEveryFrameUpdate => true;

		protected override void OnEnter()
		{
			main = (M04_Tutorial)UIManager.instance.battle.Main;
			reg = RemoteObjectManager.instance.regulation;
			int num = -1;
			int num2 = -1;
			if (side == E_SIDE.LEFT)
			{
				num = main.Simulator.GetLhsUnitIndex(0, unitPosition);
				num2 = main.Simulator.GetRhsUnitIndex(0, targetPosition);
			}
			else
			{
				num = main.Simulator.GetRhsUnitIndex(0, unitPosition);
				num2 = main.Simulator.GetLhsUnitIndex(0, targetPosition);
			}
			if (num >= 0 && num2 >= 0)
			{
				Unit unit = main.Simulator.frame.units[num];
				if (unit != null && skillIdx < unit.skills.Count && unit.skills[skillIdx] != null)
				{
					unitRenderer = main.UnitRenderers[num];
					targetUnitRenderer = main.UnitRenderers[num2];
					skill = unit.skills[skillIdx];
					StartCoroutine(_Fire(unitRenderer, targetUnitRenderer, skill));
				}
			}
		}

		private IEnumerator _Fire(UnitRenderer attacker, UnitRenderer target, Skill skilData)
		{
			string animationName2 = skilData.SkillDataRow.unitMotionDrk;
			animationName2 = animationName2.Substring(animationName2.LastIndexOf("/") + 1);
			attacker.PlayAnimation(animationName2);
			UnitMotionDataRow motionDr = reg.unitMotionDtbl[skilData.SkillDataRow.unitMotionDrk];
			IList<FireEvent> fireEvents = motionDr.fireEvents;
			for (int i = 0; i < fireEvents.Count; i++)
			{
				FireEvent fireEvent = fireEvents[i];
				if (fireEvent == null)
				{
					continue;
				}
				FirePoint firePoint = skilData.firePoints[fireEvent.firePointTypeIndex];
				if (firePoint != null)
				{
					DummyProjectile dummyProjectile = _CreateProjectile(firePoint);
					if (dummyProjectile != null)
					{
						StartCoroutine(_LaunchProjectile(fireEvent.time, attacker, fireEvent.firePointBonePath, target.transform.position, target, dummyProjectile));
					}
				}
			}
			yield return null;
		}

		private DummyProjectile _CreateProjectile(FirePoint firePoint)
		{
			DummyProjectile dummyProjectile = new DummyProjectile();
			DataTable<ProjectileMotionPhaseDataRow> projectileMotionPhaseDtbl = reg.projectileMotionPhaseDtbl;
			ProjectileDataRow projectileDataRow = reg.projectileDtbl[firePoint.projectileDri];
			string text = projectileKey;
			string key = text + "/FirePhase";
			int num = projectileMotionPhaseDtbl.FindIndex(key);
			if (num == -1)
			{
				return null;
			}
			ProjectileMotionPhaseDataRow projectileMotionPhaseDataRow = projectileMotionPhaseDtbl[num];
			num += UnityEngine.Random.Range(0, projectileMotionPhaseDataRow.patternCount) + 1;
			projectileMotionPhaseDataRow = projectileMotionPhaseDtbl[num];
			dummyProjectile.projectileKey = text;
			dummyProjectile.firePhaseId = num;
			bool flag = UnityEngine.Random.value < missRate * 0.01f;
			string key2 = text + ((!flag) ? "/HitPhase" : "/MissPhase");
			int num2 = projectileMotionPhaseDtbl.FindIndex(key2);
			if (num2 == -1)
			{
				return null;
			}
			ProjectileMotionPhaseDataRow projectileMotionPhaseDataRow2 = projectileMotionPhaseDtbl[num2];
			num2 += UnityEngine.Random.Range(0, projectileMotionPhaseDataRow2.patternCount) + 1;
			projectileMotionPhaseDataRow2 = projectileMotionPhaseDtbl[num2];
			dummyProjectile.hitPhaseId = num2;
			dummyProjectile.hitTime = projectileMotionPhaseDataRow.duration + projectileMotionPhaseDataRow2.eventTime;
			if (!flag)
			{
				string key3 = text + "/BeHitPhase";
				int num3 = projectileMotionPhaseDtbl.FindIndex(key3);
				if (num3 >= 0)
				{
					ProjectileMotionPhaseDataRow projectileMotionPhaseDataRow3 = projectileMotionPhaseDtbl[num3];
					num3 += UnityEngine.Random.Range(0, projectileMotionPhaseDataRow3.patternCount) + 1;
				}
				dummyProjectile.beHitId = num3;
				bool isCri = UnityEngine.Random.value < criRate * 0.01f;
				dummyProjectile.isCri = isCri;
			}
			dummyProjectile.isMiss = flag;
			return dummyProjectile;
		}

		private IEnumerator _LaunchProjectile(int delayMS, UnitRenderer attacker, string launchBonePath, Vector3 hitPos, UnitRenderer beHitUnit, DummyProjectile projectile)
		{
			FireEffect fireEffect = CacheManager.instance.FireEffectCache.Create<FireEffect>(skill._skillDataRow.key);
			if (fireEffect != null)
			{
			}
			yield return new WaitForSeconds((float)delayMS * 0.001f);
			Vector3 firePosition = attacker.GetBone(launchBonePath).position;
			ProjectileController projectileRenderer = CacheManager.instance.ControllerCache.Create<ProjectileController>("ProjectileController");
			if (!(projectileRenderer != null))
			{
				yield break;
			}
			projectileRenderer.Create(projectile.firePhaseId, firePosition);
			projectileRenderer.Create(projectile.hitPhaseId, hitPos);
			yield return new WaitForSeconds((float)projectile.hitTime * 0.001f);
			if (!projectile.isMiss)
			{
				beHitUnit.PlayAnimation("behit");
			}
			else
			{
				beHitUnit.PlayAnimation("avoid");
			}
			if (projectile.beHitId > 0)
			{
				ProjectileController projectileController = CacheManager.instance.ControllerCache.Create<ProjectileController>("ProjectileController");
				if (projectileController != null)
				{
					projectileController.name = "1";
					projectileController.Create(projectile.beHitId, hitPos);
				}
			}
		}
	}
}
