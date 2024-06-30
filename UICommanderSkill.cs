using Shared.Regulation;
using UnityEngine;

public class UICommanderSkill : UISkill
{
	public UILabel level;

	public UILabel cost;

	public GameObject upgradeBtn;

	public GameObject upgradeRoot;

	public GameObject openRoot;

	public GameObject addSkillRoot;

	public UILabel openGrade;

	public GameObject upgradeEffect;

	public void SetSkill(SkillDataRow skillData, int skillLevel, int skillCost, int grade, bool isOpen, int idx, bool isUpgradeLevel = true)
	{
		Set(skillData);
		UISetter.SetLabel(level, skillLevel);
		UISetter.SetLabel(cost, skillCost);
		UISetter.SetGameObjectName(upgradeBtn, $"{_GetOriginalName(upgradeBtn)}-{skillData.key}");
		UISetter.SetLabel(openGrade, Localization.Format("10039", Localization.Get((8900 + grade).ToString())));
		UISetter.SetActive(upgradeRoot, isOpen && idx < 4);
		UISetter.SetActive(openRoot, !isOpen && idx < 4);
		UISetter.SetActive(addSkillRoot, idx == 4);
		UISetter.SetButtonGray(upgradeBtn, isUpgradeLevel);
	}

	private void OnDisable()
	{
		UISetter.SetActive(upgradeEffect, active: false);
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

	public void OnUpgradeClicked()
	{
		UISetter.SetActive(upgradeEffect, active: false);
		UISetter.SetActive(upgradeEffect, active: true);
	}
}
