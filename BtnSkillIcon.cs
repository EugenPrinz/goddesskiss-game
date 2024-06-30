using Shared.Battle;
using UnityEngine;

public class BtnSkillIcon : MonoBehaviour
{
	protected bool bEnable;

	protected int skillIdx;

	protected UISkillView skillView;

	protected Skill targetSkill;

	public UISprite imgSkillIcon;

	public UISprite imgSkillProgress;

	public UISprite imgOnActiveIcon;

	public Color onActiveColor;

	public Color enableColor;

	public Color disableColor;

	public virtual string StrSkillIconName => string.Empty;

	public virtual float Amount => (float)targetSkill.sp / (float)targetSkill.SkillDataRow.maxSp;

	public void Initilize(int skillIdx, UISkillView skillView)
	{
		this.skillIdx = skillIdx;
		this.skillView = skillView;
	}

	public void Set(Skill skill)
	{
		targetSkill = skill;
		UpdateUiStatus();
	}

	public void SetEnable(bool bEnable)
	{
		this.bEnable = bEnable;
		imgOnActiveIcon.gameObject.SetActive(bEnable);
	}

	protected void UpdateUiStatus()
	{
		imgSkillProgress.fillAmount = Amount;
		SetEnable(bEnable: false);
	}

	private void OnClick()
	{
		if (CanTouch())
		{
			SetEnable(bEnable: true);
			skillView.OnSelectSkill(skillIdx);
		}
	}

	private bool CanTouch()
	{
		if (Amount < 1f)
		{
			return false;
		}
		return skillView.CanTouch();
	}
}
