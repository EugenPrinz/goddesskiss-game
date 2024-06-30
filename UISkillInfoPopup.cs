using Shared.Regulation;
using UnityEngine;

public class UISkillInfoPopup : UISimplePopup
{
	public UISprite icon;

	public UISprite attackRange;

	public UILabel description_1;

	public UILabel description_2;

	public UILabel description_3;

	private void Start()
	{
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
		SetAutoDestroy(autoDestory: true);
	}

	private void OnDestroy()
	{
		if (icon != null)
		{
			icon = null;
		}
	}

	public void SetInfo(RoCommander commander, string id)
	{
		SkillDataRow skillDataRow = base.regulation.skillDtbl[id];
		int num = 1;
		UISetter.SetSprite(attackRange, skillDataRow.rangeicon);
		UISetter.SetSprite(icon, skillDataRow.thumbnail);
		if (commander != null)
		{
			num = commander.GetSkillLevel(commander.GetSkillIndex(id));
			UISetter.SetLabel(description_1, Localization.Format(skillDataRow.skillDescription, (float)(skillDataRow.startBonus + skillDataRow.lvBonus * (num - 1)) / 100f));
		}
	}

	public void SetInfo(string id, int level = 1)
	{
		SkillDataRow skillDataRow = base.regulation.skillDtbl[id];
		UISetter.SetSprite(attackRange, skillDataRow.rangeicon);
		UISetter.SetSprite(icon, skillDataRow.thumbnail);
		UISetter.SetLabel(description_1, Localization.Format(skillDataRow.skillDescription, (float)(skillDataRow.startBonus + skillDataRow.lvBonus * (level - 1)) / 100f));
	}

	public override void OnClick(GameObject sender)
	{
		string text = base.gameObject.name;
		base.OnClick(sender);
	}
}
