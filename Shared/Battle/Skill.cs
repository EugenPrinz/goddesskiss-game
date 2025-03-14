using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Regulation;

namespace Shared.Battle
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Skill
	{
		public const int FirePointCount = 3;

		public const int FireSubCount = 2;

		[JsonProperty]
		internal int _dri;

		[JsonProperty]
		internal int _level;

		[JsonProperty]
		internal bool _bCutInSkill;

		[JsonProperty]
		internal int _unitMotionDri;

		[JsonProperty]
		internal int _returnMotionDri;

		[JsonProperty]
		internal List<FirePoint> _firePoints;

		[JsonProperty]
		internal List<FirePoint> _fireSubPoints;

		[JsonIgnore]
		internal List<WeaponEffect> _weaponEffects;

		[JsonIgnore]
		internal SkillDataRow _skillDataRow;

		[JsonIgnore]
		internal SkillLevelFormal _skillLevelFormal;

		[JsonIgnore]
		internal int _fireActionDri;

		[JsonIgnore]
		internal FireActionDataRow _fireActionDr;

		[JsonIgnore]
		internal bool _fireRender;

		[JsonIgnore]
		internal bool _hitRender;

		[JsonIgnore]
		internal string _skillName;

		[JsonProperty]
		internal int _targetIndex;

		[JsonProperty]
		internal int _remainedMotionTime;

		[JsonProperty]
		internal int _remainedReturnMotionTime;

		[JsonProperty]
		internal int _initMotionTime;

		[JsonProperty]
		internal int _sp;

		[JsonProperty]
		internal int _curSp;

		[JsonIgnore]
		internal int _perCurSp;

		[JsonProperty]
		internal int _attackDamage;

		[JsonProperty]
		internal int _accuracy;

		[JsonProperty]
		internal int _criticalChance;

		[JsonProperty]
		internal int _criticalDamage;

		[JsonProperty]
		internal int _bloodsucking;

		[JsonIgnore]
		internal bool _activeState;

		public int dri => _dri;

		public int level => _level;

		public int unitMotionDri => _unitMotionDri;

		public int returnMotionDri => _returnMotionDri;

		public IList<FirePoint> firePoints => _firePoints.AsReadOnly();

		public SkillDataRow SkillDataRow => _skillDataRow;

		public SkillLevelFormal SkillLevelFormal => _skillLevelFormal;

		public bool IsCutInSkill => _bCutInSkill;

		public int fireActionDri => _fireActionDri;

		public FireActionDataRow FireActionDr => _fireActionDr;

		public int targetIndex => _targetIndex;

		public int sp => _sp;

		public int curSp => _curSp;

		public int perCurSp => _perCurSp;

		public int attackDamage => _attackDamage;

		public int accuracy => _accuracy;

		public int criticalChance => _criticalChance;

		public int criticalDamage => _criticalDamage;

		public int bloodsucking => _bloodsucking;

		public int remainedMotionTime => _remainedMotionTime;

		public int remainedReturnMotionTime => _remainedReturnMotionTime;

		public bool activeState => _activeState;

		public bool isActiveSkill => _skillDataRow.spCostOnTurn > 0;

		public virtual float Amount => (float)_curSp / (float)_skillDataRow.maxSp;

		public int openGrade => _skillDataRow.openGrade;

		public bool isIgnoreDeathType => _skillDataRow.remainedHealthRate == 0;

		public bool isMeleeDeathSkillType
		{
			get
			{
				if (!isIgnoreDeathType)
				{
					return false;
				}
				if (returnMotionDri < 0)
				{
					return false;
				}
				return true;
			}
		}

		public int occurrenceProbability => _skillDataRow.occurrenceProbability;

		public bool CanUse
		{
			get
			{
				if (_sp >= _skillDataRow.maxSp)
				{
					return _curSp == _sp;
				}
				return false;
			}
		}

		public bool HasSkillEvent
		{
			get
			{
				if (_skillDataRow.spCostOnBeHit > 0)
				{
					return true;
				}
				if (_skillDataRow.spCostOnCombo > 0)
				{
					return true;
				}
				if (_skillDataRow.spCostOnEnter > 0)
				{
					return true;
				}
				if (_skillDataRow.remainedHealthRate >= 0)
				{
					return true;
				}
				if (_skillDataRow.occurrenceProbability > 0)
				{
					return true;
				}
				return false;
			}
		}

		public bool HasEventCounter => _skillDataRow.spCostOnBeHit > 0;

		public bool HasEventCombo => _skillDataRow.spCostOnCombo > 0;

		public bool HasEventEnter => _skillDataRow.spCostOnEnter > 0;

		public bool HasEventHealthRate => _skillDataRow.remainedHealthRate >= 0;

		public bool HasEventOccurrenceProbability => _skillDataRow.occurrenceProbability > 0;

		private Skill()
		{
		}

		public bool isDamageSkill(int subIdx = 0)
		{
			if (_skillDataRow.fireTypes[subIdx] == ESkillType.Buff || _skillDataRow.fireTypes[subIdx] == ESkillType.DeBuff || _skillDataRow.fireTypes[subIdx] == ESkillType.Heal)
			{
				return false;
			}
			if (subIdx == 0 && _skillDataRow.healing > 0)
			{
				return false;
			}
			return true;
		}

		internal static Skill _Create(Shared.Regulation.Regulation rg, string drk)
		{
			int num = rg.skillDtbl.FindIndex(drk);
			if (num < 0)
			{
				throw new ArgumentException("drk doesn't exist. : " + drk);
			}
			SkillDataRow skillDataRow = rg.skillDtbl[num];
			skillDataRow.InitializeData();
			Skill skill = new Skill();
			skill._skillLevelFormal = SkillLevelFormal._Create(rg, drk);
			skill._skillDataRow = skillDataRow;
			skill._dri = num;
			skill._level = 1;
			skill._unitMotionDri = rg.unitMotionDtbl.FindIndex(skillDataRow.unitMotionDrk);
			if (skill._unitMotionDri < 0)
			{
				throw new ArgumentException("unitMotionDrk doesn't exist. : " + skillDataRow.unitMotionDrk);
			}
			skill._returnMotionDri = -1;
			if (skillDataRow.hasReturnMotion)
			{
				skill._returnMotionDri = rg.unitMotionDtbl.FindIndex(skillDataRow.returnMotionDrk);
				if (skill._returnMotionDri < 0)
				{
					throw new ArgumentException("returnMotionDrk doesn't exist. : " + skillDataRow.returnMotionDrk);
				}
			}
			skill._sp = 0;
			skill._curSp = skill._sp;
			float num2 = (float)skill.SkillDataRow.maxSp / 1000f * 66f;
			skill._perCurSp = (int)num2;
			skill._bCutInSkill = false;
			skill._fireActionDri = -1;
			if (!string.IsNullOrEmpty(skillDataRow.cutInEffectId) && skillDataRow.cutInEffectId.CompareTo("-") != 0)
			{
				skill._bCutInSkill = true;
				skill._fireActionDri = rg.fireActionDtbl.FindIndex(skillDataRow.cutInEffectId);
				if (skill._fireActionDri >= 0)
				{
					skill._fireActionDr = rg.fireActionDtbl[skill._fireActionDri];
					skill._bCutInSkill = true;
				}
			}
			skill._activeState = skill.CanUse;
			skill._attackDamage = 0;
			skill._accuracy = 0;
			skill._criticalChance = 0;
			skill._criticalDamage = 0;
			skill._bloodsucking = 0;
			skill._remainedMotionTime = 0;
			skill._remainedReturnMotionTime = 0;
			skill._firePoints = new List<FirePoint>();
			for (int i = 0; i < 3; i++)
			{
				string text = ((i < skillDataRow._projectileDrks.Count) ? skillDataRow._projectileDrks[i] : null);
				if (string.IsNullOrEmpty(text) || text == "0")
				{
					skill._firePoints.Add(null);
					continue;
				}
				FirePoint item = FirePoint._Create(rg, skillDataRow, i, 0);
				skill._firePoints.Add(item);
			}
			skill._targetIndex = -1;
			skill._fireRender = true;
			skill._hitRender = true;
			skill._fireSubPoints = new List<FirePoint>();
			for (int j = 0; j < 2; j++)
			{
				string text2 = skillDataRow.fireSubProjectileDrks[j];
				if (!string.IsNullOrEmpty(text2) && !(text2 == "0"))
				{
					FirePoint item2 = FirePoint._Create(rg, skillDataRow, j, j + 1);
					skill._fireSubPoints.Add(item2);
				}
			}
			skill._weaponEffects = new List<WeaponEffect>();
			if (!string.IsNullOrEmpty(skillDataRow.skillName) && skillDataRow.skillName != "-")
			{
				skill._skillName = Localization.Get(skillDataRow.skillName);
			}
			return skill;
		}

		internal static Skill _Copy(Skill src)
		{
			Skill skill = new Skill();
			skill._dri = src._dri;
			skill._level = src._level;
			skill._bCutInSkill = src._bCutInSkill;
			skill._unitMotionDri = src._unitMotionDri;
			skill._sp = src._sp;
			skill._curSp = src._curSp;
			skill._attackDamage = src._attackDamage;
			skill._accuracy = src._accuracy;
			skill._criticalChance = src._criticalChance;
			skill._criticalDamage = src._criticalDamage;
			skill._bloodsucking = src._bloodsucking;
			skill._remainedMotionTime = src._remainedMotionTime;
			skill._remainedReturnMotionTime = src._remainedReturnMotionTime;
			skill._firePoints = new List<FirePoint>();
			foreach (FirePoint firePoint in src._firePoints)
			{
				skill._firePoints.Add(FirePoint._Copy(firePoint));
			}
			skill._targetIndex = src._targetIndex;
			skill._fireRender = src._fireRender;
			skill._hitRender = src._hitRender;
			skill._skillName = src._skillName;
			skill._fireSubPoints = new List<FirePoint>();
			for (int i = 0; i < src._fireSubPoints.Count; i++)
			{
				skill._fireSubPoints.Add(FirePoint._Copy(src._fireSubPoints[i]));
			}
			skill._weaponEffects = new List<WeaponEffect>();
			for (int j = 0; j < src._weaponEffects.Count; j++)
			{
				skill._weaponEffects.Add(WeaponEffect._Copy(src._weaponEffects[j]));
			}
			return skill;
		}

		public static bool IsTargetJobType(EJob job, int val)
		{
			if (val == 0)
			{
				return true;
			}
			switch (job)
			{
			case EJob.Attack:
			case EJob.Attack_x:
				return (val & 1) > 0;
			case EJob.Defense:
			case EJob.Defense_x:
				return (val & 2) > 0;
			case EJob.Support:
			case EJob.Support_x:
				return (val & 4) > 0;
			default:
				return false;
			}
		}
	}
}
