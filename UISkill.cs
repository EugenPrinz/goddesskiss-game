using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

public class UISkill : UIItemBase
{
	public delegate void SelectDelegate(int unitIdx, int skillIdx);

	public UISprite bg;

	public UISprite icon;

	public UISprite iconProgress;

	public GameObject usableMarkRoot;

	public new UILabel name;

	public SelectDelegate _Click;

	protected int unitIdx;

	protected int skillIdx;

	protected Skill skill;

	protected SkillDataRow skillData;

	private bool _selected;

	public GameObject selectedRoot;

	public void Set(int unitIdx, int skillIdx, Skill skill, SkillDataRow skillData)
	{
		this.unitIdx = unitIdx;
		this.skillIdx = skillIdx;
		this.skill = skill;
		this.skillData = skillData;
		if (!string.IsNullOrEmpty(this.skillData.thumbnail))
		{
			icon.spriteName = this.skillData.thumbnail;
			iconProgress.spriteName = this.skillData.thumbnail;
		}
		UpdateStatus();
	}

	public void Set(int unitIdx, int skillIdx)
	{
		this.unitIdx = unitIdx;
		this.skillIdx = skillIdx;
	}

	public void UpdateStatus()
	{
		float val = (float)skill.sp / (float)skillData.maxSp;
		UISetter.SetProgress(iconProgress, val);
	}

	private void OnClick()
	{
		if (_Click != null)
		{
			_Click(unitIdx, skillIdx);
		}
	}

	public void Set(SkillDataRow skillData)
	{
		if (skillData == null)
		{
			UISetter.SetActive(selectedRoot, active: false);
			UISetter.SetActive(icon, active: false);
			return;
		}
		UISetter.SetActive(selectedRoot, active: true);
		UISetter.SetActive(icon, active: true);
		UISetter.SetSprite(icon, skillData.thumbnail);
		UISetter.SetLabel(name, Localization.Get(skillData.skillName));
	}

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(selectedRoot, selected);
		_selected = selected;
	}

	private void OnDestroy()
	{
		if (icon != null)
		{
			icon = null;
		}
	}
}
