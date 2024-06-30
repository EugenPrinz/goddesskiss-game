using System;
using System.Collections.Generic;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

public class UIBattleUnit : MonoBehaviour
{
	[Serializable]
	public struct UnitInfoView
	{
		public GameObject root;

		public UILabel unitLevel;

		public UILabel unitTurn;

		public UISprite unitLevelStatusIcon;

		public UISprite skillIcon;

		public UISprite skillProgress;

		public GameObject skillOnActive;
	}

	[Serializable]
	public class UnitTypeView
	{
		public GameObject root;

		public UISprite jobType;

		public UISprite unitClassGroup;

		public bool enable
		{
			get
			{
				return root != null && root.activeSelf;
			}
			set
			{
				UISetter.SetActive(root, value);
			}
		}

		public void Set(Unit unit)
		{
			if (enable)
			{
				Regulation regulation = RemoteObjectManager.instance.regulation;
				UnitDataRow unitDataRow = regulation.unitDtbl[unit.dri];
				UISetter.SetSprite(jobType, "com_icon_" + unitDataRow.job.ToString().ToLower());
				UISetter.SetSprite(unitClassGroup, string.Format(UICommander.thumbnailGroupBackgroundPrefix + "{0}", UIUnit.GetClassGroup(unit.cls)));
			}
		}
	}

	[Serializable]
	public class BuffStatusView
	{
		public GameObject root;

		public GameObjectPool pool;

		public Transform content;

		public UIGrid grid;

		protected Dictionary<int, GameObject> _items;

		public void Init()
		{
			if (_items == null)
			{
				_items = new Dictionary<int, GameObject>();
			}
		}

		public void Add(Status status)
		{
			if (_items == null)
			{
				Init();
			}
			if (!_items.ContainsKey(status.Dri) && !string.IsNullOrEmpty(status.DataRow.thumbnail) && !(status.DataRow.thumbnail == "-"))
			{
				GameObject gameObject = pool.Create(content);
				if (gameObject != null)
				{
					UISprite componentInChildren = gameObject.GetComponentInChildren<UISprite>();
					UISetter.SetSprite(componentInChildren, status.DataRow.thumbnail);
					_items.Add(status.Dri, gameObject);
				}
				grid.Reposition();
			}
		}

		public void Remove(Status status)
		{
			if (_items.ContainsKey(status.Dri))
			{
				GameObject obj = _items[status.Dri];
				pool.Release(obj);
				_items.Remove(status.Dri);
				grid.Reposition();
			}
		}

		public void Clean()
		{
			while (content.childCount > 0)
			{
				Transform child = content.transform.GetChild(0);
				pool.Release(child.gameObject);
			}
			if (_items != null)
			{
				_items.Clear();
			}
		}
	}

	public delegate void SelectSkillDelegate(int unitIdx, int skillIdx);

	public GameObject root;

	public GameObject statusRoot;

	public bool backgroundProgressUpdate;

	public UIProgressBar foregroundHpProgress;

	public UIProgressBar backgroundHpProgress;

	public UIProgressBar foregroundSkillProgress;

	public UIProgressBar backgroundSkillProgress;

	public float speed = 10f;

	public UnitInfoView infoView;

	public UnitTypeView unitTypeView;

	public BuffStatusView buffView;

	protected int unitIdx;

	protected int activeSkillIdx;

	public SelectSkillDelegate _SkillClick;

	[HideInInspector]
	public UICommanderTag commanderTag;

	private bool firstSetHp = true;

	private float _maxHp = 100f;

	private float _backgroundHp;

	private float _foregroundHp;

	private float _hpAnimationDir = -1f;

	private bool firstSetSkill = true;

	private float _maxSkill = 10000f;

	private float _backgroundSkill;

	private float _foregroundSkill;

	private float _skillAnimationDir = -1f;

	protected Unit unit;

	protected Skill activeSkill;

	public void OnClick()
	{
		if ((bool)infoView.skillOnActive && unit.side == EBattleSide.Left && activeSkillIdx > 0)
		{
			_SkillClick(unitIdx, activeSkillIdx);
		}
	}

	private void Awake()
	{
		buffView.Init();
	}

	public void AddStatus(Status status)
	{
		buffView.Add(status);
	}

	public void RemoveStatus(Status status)
	{
		buffView.Remove(status);
	}

	public void CleanUp()
	{
		buffView.Clean();
	}

	public void SetCommanderTag(UICommanderTag commanderTag)
	{
		this.commanderTag = commanderTag;
	}

	public void SetCommander(string commanderId, int grade)
	{
		commanderTag.Set(commanderId, grade);
	}

	public void SetCommanderId(string commanderId)
	{
		if (!(commanderTag == null))
		{
			SetCommanderTagEnable(!string.IsNullOrEmpty(commanderId));
			commanderTag.Set(commanderId, UnityEngine.Random.Range(0, 6));
		}
	}

	public void SetCommanderTagEnable(bool enable)
	{
		UISetter.SetActive(commanderTag, enable);
	}

	public void SetTurn(int turn)
	{
		if (turn < 0)
		{
			UISetter.SetLabel(infoView.unitTurn, "WAIT");
		}
		else if (turn == 0)
		{
			UISetter.SetLabel(infoView.unitTurn, "[bb5533]NOW[-]");
		}
		else
		{
			UISetter.SetLabel(infoView.unitTurn, turn);
		}
	}

	public void OnActivedSkill(bool enable)
	{
		UISetter.SetActive(infoView.skillIcon, enable);
		UISetter.SetActive(infoView.skillOnActive, enable);
	}

	public void SetHp(float max, float curr)
	{
		UISetter.SetProgress(foregroundHpProgress, curr / max);
		if (backgroundProgressUpdate)
		{
			UISetter.SetProgress(backgroundHpProgress, curr / max);
		}
		_maxHp = max;
		_backgroundHp = curr;
		_foregroundHp = curr;
		_hpAnimationDir = 0f;
	}

	public void SetAnimateHp(float max, float curr)
	{
		if (firstSetHp)
		{
			SetHp(max, curr);
			firstSetHp = false;
			return;
		}
		if (_maxHp != max)
		{
			_foregroundHp = curr;
			_hpAnimationDir = 0f;
			UISetter.SetProgress(foregroundHpProgress, curr / max);
		}
		if (curr > _foregroundHp)
		{
			_hpAnimationDir = 1f;
			_backgroundHp = curr;
			if (backgroundProgressUpdate)
			{
				UISetter.SetProgress(backgroundHpProgress, curr / max);
			}
		}
		else if (curr < _foregroundHp)
		{
			_hpAnimationDir = -1f;
			_foregroundHp = curr;
			UISetter.SetProgress(foregroundHpProgress, curr / max);
			if (commanderTag != null && commanderTag.isActiveAndEnabled)
			{
				commanderTag.SetState("Behit");
			}
		}
		_maxHp = max;
	}

	public void UpdateSkillAmount()
	{
		if (!(infoView.skillProgress == null) && activeSkill != null)
		{
			infoView.skillProgress.fillAmount = activeSkill.Amount;
		}
	}

	public void SetUnit(Unit unit, int unitIdx)
	{
		this.unit = unit;
		this.unitIdx = unitIdx;
		firstSetHp = true;
		UISetter.SetActive(root, active: true);
		UISetter.SetActive(statusRoot, active: true);
		unitTypeView.enable = true;
		unitTypeView.Set(unit);
		if (unit.side != 0)
		{
			UISetter.SetActive(infoView.root, active: false);
			UISetter.SetActive(backgroundSkillProgress, active: false);
			UISetter.SetActive(foregroundSkillProgress, active: false);
			return;
		}
		UISetter.SetActive(infoView.root, active: false);
		UISetter.SetActive(backgroundSkillProgress, active: false);
		UISetter.SetActive(foregroundSkillProgress, active: false);
		UISetter.SetLabel(infoView.unitLevel, unit.level);
		if (unit.level < 21)
		{
			UISetter.SetSprite(infoView.unitLevelStatusIcon, "battle_rating_01");
		}
		else if (unit.level < 41)
		{
			UISetter.SetSprite(infoView.unitLevelStatusIcon, "battle_rating_02");
		}
		else if (unit.level < 61)
		{
			UISetter.SetSprite(infoView.unitLevelStatusIcon, "battle_rating_03");
		}
		else if (unit.level < 81)
		{
			UISetter.SetSprite(infoView.unitLevelStatusIcon, "battle_rating_04");
		}
		else
		{
			UISetter.SetSprite(infoView.unitLevelStatusIcon, "battle_rating_05");
		}
		UISetter.SetActive(infoView.skillIcon, active: false);
		UISetter.SetActive(infoView.skillOnActive, active: false);
		UISetter.SetActive(infoView.skillProgress, active: false);
		for (int i = 1; i < unit.skills.Count; i++)
		{
			if (unit.skills[i] == null || !unit.skills[i].isActiveSkill || unit._cls < unit.skills[i].SkillDataRow.openGrade)
			{
				continue;
			}
			activeSkill = unit.skills[i];
			activeSkillIdx = i;
			if (infoView.skillIcon != null)
			{
				infoView.skillIcon.spriteName = activeSkill.SkillDataRow.thumbnail;
				infoView.skillIcon.color = new Color(1f, 1f, 1f);
			}
			if (infoView.skillProgress != null)
			{
				infoView.skillProgress.spriteName = activeSkill.SkillDataRow.thumbnail;
				infoView.skillProgress.fillAmount = activeSkill.Amount;
				if (infoView.skillIcon != null)
				{
					infoView.skillIcon.color = new Color(0.35f, 0.35f, 0.35f);
				}
			}
			if (unit.skills[i].CanUse)
			{
				UISetter.SetActive(infoView.skillIcon, active: true);
				UISetter.SetActive(infoView.skillOnActive, active: true);
				UISetter.SetActive(infoView.skillProgress, active: true);
			}
			break;
		}
	}

	public void SetSkill(float max, float curr)
	{
		UISetter.SetProgress(foregroundSkillProgress, curr / max);
		if (backgroundProgressUpdate)
		{
			UISetter.SetProgress(backgroundSkillProgress, curr / max);
		}
		_maxSkill = max;
		_backgroundSkill = curr;
		_foregroundSkill = curr;
		_skillAnimationDir = 0f;
	}

	public void SetAnimateSkill(float max, float curr)
	{
		if (firstSetSkill)
		{
			SetSkill(max, curr);
			firstSetSkill = false;
			return;
		}
		if (curr > _foregroundSkill)
		{
			_skillAnimationDir = 1f;
			if (backgroundProgressUpdate)
			{
				UISetter.SetProgress(backgroundSkillProgress, curr / max);
			}
			_backgroundSkill = curr;
		}
		else
		{
			_skillAnimationDir = -1f;
			UISetter.SetProgress(foregroundSkillProgress, curr / max);
			_foregroundSkill = curr;
		}
		_maxSkill = max;
	}

	public void Dead()
	{
		UISetter.SetActive(infoView.root, active: false);
		UISetter.SetActive(statusRoot, active: false);
		unitTypeView.enable = false;
		if (commanderTag != null && commanderTag.isActiveAndEnabled)
		{
			commanderTag.SetState("Dead");
		}
	}

	public void Revival()
	{
		UISetter.SetActive(statusRoot, active: true);
		if (commanderTag != null && commanderTag.isActiveAndEnabled)
		{
			commanderTag.SetState("Idle");
		}
	}

	private void OnEnable()
	{
		buffView.grid.Reposition();
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		float num = deltaTime * _hpAnimationDir * (speed * 0.001f * _maxHp);
		if (_hpAnimationDir < 0f)
		{
			_backgroundHp += num;
			if (_backgroundHp < _foregroundHp)
			{
				_hpAnimationDir = 0f;
				_backgroundHp = _foregroundHp;
			}
			if (backgroundProgressUpdate)
			{
				UISetter.SetProgress(backgroundHpProgress, _backgroundHp / _maxHp);
			}
		}
		else if (_hpAnimationDir > 0f)
		{
			_foregroundHp += num;
			if (_backgroundHp < _foregroundHp)
			{
				_hpAnimationDir = 0f;
				_foregroundHp = _backgroundHp;
			}
			UISetter.SetProgress(foregroundHpProgress, _foregroundHp / _maxHp);
		}
		num = deltaTime * _skillAnimationDir * (speed * 0.001f * _maxSkill);
		if (_skillAnimationDir < 0f)
		{
			_backgroundSkill += num;
			if (_backgroundSkill < _foregroundSkill)
			{
				_skillAnimationDir = 0f;
				_backgroundSkill = _foregroundSkill;
			}
			if (backgroundProgressUpdate)
			{
				UISetter.SetProgress(backgroundSkillProgress, _backgroundSkill / _maxSkill);
			}
		}
		else if (_skillAnimationDir > 0f)
		{
			_foregroundSkill += num;
			if (_backgroundSkill < _foregroundSkill)
			{
				_skillAnimationDir = 0f;
				_foregroundSkill = _backgroundSkill;
			}
			UISetter.SetProgress(foregroundSkillProgress, _foregroundSkill / _maxSkill);
		}
	}
}
