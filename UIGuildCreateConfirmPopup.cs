using UnityEngine;

public class UIGuildCreateConfirmPopup : UISimplePopup
{
	public new UILabel name;

	public UISprite emblem;

	public UILabel level;

	public UILabel type;

	public UILabel price;

	private void Start()
	{
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
		SetAutoDestroy(autoDestory: true);
	}

	public override void OnClick(GameObject sender)
	{
		string text = base.gameObject.name;
		if (text == "UIGuildCreateConfirmPopup")
		{
		}
		base.OnClick(sender);
	}

	public void SetInfo(string guildName, int guildEmblem, int guilLevel, int guildType, int guilPrice)
	{
		UISetter.SetLabel(name, guildName);
		UISetter.SetSpriteWithSnap(emblem, "union_mark_" + $"{guildEmblem:00}");
		UISetter.SetLabel(level, "Lv " + guilLevel);
		UISetter.SetLabel(type, (guildType != 1) ? Localization.Get("110018") : Localization.Get("110017"));
	}
}
