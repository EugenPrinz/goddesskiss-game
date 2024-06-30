using UnityEngine;

public class UIGuildUpgradePopup : UISimplePopup
{
	public UISprite icon;

	public UILabel level;

	public new UILabel name;

	public UILabel text1_1;

	public UILabel text1_2;

	public UILabel text1_3;

	public UILabel text2_1;

	public UILabel text2_2;

	public GameObject type1;

	public GameObject type2;

	private void Start()
	{
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
		SetAutoDestroy(autoDestory: true);
	}

	public void SetGuildUpgrade(string infoName, int infoLevel, string infoText1, string infoText2, string infoText3)
	{
		UISetter.SetActive(type1, active: true);
		UISetter.SetActive(type2, active: false);
		UISetter.SetSpriteWithSnap(icon, "union_mark_" + $"{base.localUser.guildInfo.emblem:00}");
		UISetter.SetLabel(name, infoName);
		UISetter.SetLabel(level, "Lv" + infoLevel);
		UISetter.SetLabel(text1_1, infoText1);
		UISetter.SetLabel(text1_2, infoText2);
		UISetter.SetLabel(text1_3, infoText3);
	}

	public void SetSkillUpgrade(int idx, string infoName, int infoLevel, string infoText1, string infoText2)
	{
		UISetter.SetActive(type1, active: false);
		UISetter.SetActive(type2, active: true);
		UISetter.SetSpriteWithSnap(icon, "union_skill_" + $"{idx:00}");
		UISetter.SetLabel(name, infoName);
		UISetter.SetLabel(level, "Lv" + infoLevel);
		UISetter.SetLabel(text2_1, infoText1);
		UISetter.SetLabel(text2_2, infoText2);
	}

	public override void OnClick(GameObject sender)
	{
		string text = base.gameObject.name;
		base.OnClick(sender);
	}
}
