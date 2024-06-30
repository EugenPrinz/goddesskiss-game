using UnityEngine;

public class UIGuildSkillListItem : UIItemBase
{
	public new UILabel name;

	public UILabel description;

	public UILabel needLevel;

	public UILabel point;

	public UISprite icon;

	public UILabel level;

	public GameObject btnUpgrade;

	public GameObject buttons;

	public void Set(RoGuildSkill skill)
	{
		UISetter.SetLabel(description, Localization.Format(skill.description, skill.reg.value));
		UISetter.SetLabel(name, Localization.Get(skill.name));
		UISetter.SetLabel(level, "Lv" + skill.skillLevel);
		UISetter.SetLabel(needLevel, (!skill.isMaxLevel) ? string.Format(Localization.Get("110215"), skill.nextLevelReg.level) : string.Empty);
		UISetter.SetLabel(point, (!skill.isMaxLevel) ? skill.nextLevelReg.cost.ToString() : string.Empty);
		if (!skill.isMaxLevel && !skill.isEnoughUpgradePoint)
		{
			point.color = new Color(1f, 0f, 0f);
		}
		UISetter.SetActive(btnUpgrade, !skill.isMaxLevel);
		UISetter.SetSprite(icon, "union_skill_" + $"{skill.idx:00}");
		UISetter.SetActive(buttons, RemoteObjectManager.instance.localUser.guildInfo.memberGrade == 1);
		UISetter.SetGameObjectName(btnUpgrade, $"{_GetOriginalName(btnUpgrade)}-{skill.idx}");
	}

	private string _GetOriginalName(GameObject go)
	{
		if (go == null)
		{
			return string.Empty;
		}
		string text = go.name;
		if (!text.Contains("-"))
		{
			return text;
		}
		return text.Remove(text.IndexOf("-"));
	}
}
