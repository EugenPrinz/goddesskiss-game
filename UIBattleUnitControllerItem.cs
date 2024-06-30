using System.Collections;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

public class UIBattleUnitControllerItem : MonoBehaviour
{
	public delegate void SelectDelegate(int unitIdx);

	public UIBattleUnitStatus unitStatus;

	public GameObject onActivedSkill;

	public GameObject onLockSkill;

	public GameObject onSkillReservation;

	public GameObject onDamage;

	public SelectDelegate _Click;

	protected Unit _unit;

	protected Skill _skill;

	protected bool _isTurn;

	protected float _skillAmount;

	protected string commanderResId;

	private float _hpAnimationDir = -1f;

	private float _speed = 10f;

	public int unitIdx => _unit._unitIdx;

	public bool canInput
	{
		get
		{
			if (onLockSkill.activeSelf)
			{
				return false;
			}
			return onActivedSkill.activeSelf;
		}
	}

	public bool skillReservation
	{
		get
		{
			return onSkillReservation.activeSelf;
		}
		set
		{
			onSkillReservation.SetActive(value);
		}
	}

	public void OnClick()
	{
		if (canInput && _Click != null)
		{
			_Click(_unit._unitIdx);
		}
	}

	protected void Idle()
	{
		StopAllCoroutines();
		UISetter.SetActive(onDamage, active: false);
	}

	protected void Dead()
	{
		StopAllCoroutines();
		UISetter.SetActive(onDamage, active: true);
	}

	private IEnumerator PlayBeHit()
	{
		UISetter.SetActive(onDamage, active: true);
		yield return new WaitForSeconds(0.4f);
		UISetter.SetActive(onDamage, active: false);
	}

	public void Set(Unit unit)
	{
		unitStatus.transform.localPosition = Vector3.zero;
		_unit = unit;
		if (unit._cdri >= 0)
		{
			CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[unit._cdri];
			commanderResId = commanderDataRow.resourceId;
		}
		unitStatus.Set(unit);
		unitStatus.statusView.SetEnable(enable: true);
		if (!unit.hasActiveSkill)
		{
			_skill = null;
			_skillAmount = 0f;
			UISetter.SetActive(unitStatus.statusView.spProgress.gameObject, active: false);
		}
		else
		{
			_skill = _unit.skills[_unit._activeSkillIdx];
			_skillAmount = _skill.Amount;
			UISetter.SetProgress(unitStatus.statusView.spProgress, _skillAmount);
			UISetter.SetActive(unitStatus.statusView.spProgress.gameObject, active: true);
		}
		Idle();
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (_unit == null)
		{
			return;
		}
		if (_unit.isDead)
		{
			if (_isTurn)
			{
				_isTurn = false;
				LoseTurnAnimation();
			}
			_skillAmount = 0f;
			UISetter.SetProgress(unitStatus.statusView.spProgress, 0f);
			UISetter.SetProgress(unitStatus.statusView.hpProgress, 0f);
			UISetter.SetActive(onActivedSkill, active: false);
			UISetter.SetActive(onLockSkill, active: false);
			UISetter.SetActive(onSkillReservation, active: false);
			Dead();
			return;
		}
		if (_unit.takenDamage > 0 || _unit.takenCriticalDamage > 0)
		{
			StopAllCoroutines();
			StartCoroutine(PlayBeHit());
		}
		UpdateHpAmount();
		bool active = false;
		if (_unit.isStatusStun || _unit.isStatusSilence)
		{
			active = true;
		}
		else if (_unit.isStatusAggro && (_skill.SkillDataRow.targetType == ESkillTargetType.Own || _skill.SkillDataRow.targetType == ESkillTargetType.Friend))
		{
			active = true;
		}
		UISetter.SetActive(onLockSkill, active);
		if (_skill != null)
		{
			_skillAmount = _skill.Amount;
		}
		if (!_isTurn)
		{
			if (_unit.isTurn)
			{
				_isTurn = true;
				GetTurnAnimation();
			}
		}
		else if (!_unit.isTurn)
		{
			_isTurn = false;
			LoseTurnAnimation();
		}
	}

	private void Update()
	{
		if (_unit != null && !_unit.isDead)
		{
			UpdateSkillAmount();
		}
	}

	protected void GetTurnAnimation()
	{
		iTween.Stop(base.gameObject);
		iTween.MoveTo(base.gameObject, iTween.Hash("y", 30, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeInBack, "oncompletetarget", base.gameObject));
	}

	protected void LoseTurnAnimation()
	{
		iTween.Stop(base.gameObject);
		iTween.MoveTo(base.gameObject, iTween.Hash("y", 0, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeOutBack, "oncompletetarget", base.gameObject));
	}

	protected void UpdateSkillAmount()
	{
		if (_skill != null)
		{
			UpdateProgress(unitStatus.statusView.spProgress, _skillAmount);
			UISetter.SetActive(onActivedSkill, unitStatus.statusView.spProgress.value >= 1f);
		}
	}

	protected void UpdateHpAmount()
	{
		UpdateProgress(unitStatus.statusView.hpProgress, (float)_unit.health / (float)_unit.maxHealth);
	}

	protected void UpdateProgress(UIProgressBar progress, float value)
	{
		if (progress.value == value)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		float num = 0f;
		float num2 = 1f;
		num = ((!(value < progress.value)) ? 1f : (-1f));
		float num3 = deltaTime * num * num2;
		progress.value += num3;
		if (num < 0f)
		{
			if (value > progress.value)
			{
				progress.value = value;
			}
		}
		else if (value < progress.value)
		{
			progress.value = value;
		}
	}
}
