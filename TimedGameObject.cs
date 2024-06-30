using UnityEngine;

public class TimedGameObject : SWP_TimedGameObject
{
	public ETimeGroupType groupType = ETimeGroupType.GameMainGroup;

	protected float localTimeSpeed = 100f;

	protected bool init;

	private void OnEnable()
	{
		if (!init || (UIManager.instance.state != UIManager.EState.Editor && UIManager.instance.battle == null))
		{
			return;
		}
		if (TimeGroupController == null)
		{
			if (TimeControllerManager.instance == null)
			{
				return;
			}
			TimeGroupController = TimeControllerManager.instance.GetController(groupType);
			if (TimeGroupController == null)
			{
				Object.DestroyImmediate(base.gameObject);
				return;
			}
		}
		((TimeGroupController)TimeGroupController).AddTimeObject(this);
		SetSpeed(TimeGroupController.ControllerSpeedPercent);
	}

	private void OnDisable()
	{
		if (UIManager.hasInstance && (UIManager.instance.state == UIManager.EState.Editor || UIManager.instance.battle != null) && TimeGroupController != null && !(TimeControllerManager.instance == null))
		{
			((TimeGroupController)TimeGroupController).RemoveTimeObject(this);
		}
	}

	private void Start()
	{
		ControllerGroupID = (int)groupType;
		if (UIManager.instance.state != UIManager.EState.Editor && UIManager.instance.battle == null)
		{
			return;
		}
		if (TimeGroupController == null)
		{
			if (TimeControllerManager.instance == null)
			{
				return;
			}
			TimeGroupController = TimeControllerManager.instance.GetController(groupType);
			if (TimeGroupController == null)
			{
				Object.DestroyImmediate(base.gameObject);
				return;
			}
		}
		((TimeGroupController)TimeGroupController).AddTimeObject(this);
		localTimeSpeed = 100f;
		SetSpeed(TimeGroupController.ControllerSpeedPercent);
		init = true;
	}

	protected override void SetSpeed(float _fNewSpeed)
	{
		fCurrentSpeedPercent = Mathf.Clamp(_fNewSpeed * (localTimeSpeed / 100f), 0f, 1000f);
		fCurrentSpeedZeroToOne = ((fCurrentSpeedPercent != 0f) ? (fCurrentSpeedPercent / 100f) : 0f);
		if (SearchObjects)
		{
			SetSpeedAssigned(_fNewSpeed, fCurrentSpeedPercent, fCurrentSpeedZeroToOne);
		}
		else
		{
			SetSpeedLooping(_fNewSpeed, fCurrentSpeedPercent, fCurrentSpeedZeroToOne);
		}
		if (fCurrentSpeedPercent != 0f)
		{
			fPreviousSpeedPercentage = fCurrentSpeedPercent;
		}
	}

	public void SetTimeSpeed(float _fNewSpeed, bool update = true)
	{
		if (localTimeSpeed == _fNewSpeed)
		{
			return;
		}
		localTimeSpeed = _fNewSpeed;
		if (update)
		{
			float speed = 0f;
			switch (groupType)
			{
			case ETimeGroupType.GameMainGroup:
				speed = TimeControllerManager.instance.GameMain.ControllerSpeedPercent;
				break;
			case ETimeGroupType.BattleGroup:
				speed = TimeControllerManager.instance.Battle.ControllerSpeedPercent;
				break;
			case ETimeGroupType.CutInGroup:
				speed = TimeControllerManager.instance.CutIn.ControllerSpeedPercent;
				break;
			case ETimeGroupType.SkillActorGroup:
				speed = TimeControllerManager.instance.SkillActor.ControllerSpeedPercent;
				break;
			case ETimeGroupType.ProjectileGroup:
				speed = TimeControllerManager.instance.Projectile.ControllerSpeedPercent;
				break;
			case ETimeGroupType.EtcGroup:
				speed = TimeControllerManager.instance.ETC.ControllerSpeedPercent;
				break;
			}
			SetSpeed(speed);
		}
	}

	public void ChangeTimeGroup(ETimeGroupType groupType, bool update = true)
	{
		if (this.groupType == groupType)
		{
			return;
		}
		this.groupType = groupType;
		ControllerGroupID = (int)groupType;
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		((TimeGroupController)TimeGroupController).RemoveTimeObject(this);
		TimeGroupController = TimeControllerManager.instance.GetController(groupType);
		if (TimeGroupController == null)
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		((TimeGroupController)TimeGroupController).AddTimeObject(this);
		if (update)
		{
			SetSpeed(TimeGroupController.ControllerSpeedPercent);
		}
	}
}
