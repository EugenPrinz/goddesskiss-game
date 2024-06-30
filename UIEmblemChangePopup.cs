using UnityEngine;

public class UIEmblemChangePopup : UISimplePopup
{
	public UISprite guildEmblem;

	public UISprite changeEmblem;

	private void Start()
	{
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
		SetAutoDestroy(autoDestory: true);
	}

	public void SetEmblem(int changeEmblemIdx)
	{
		UISetter.SetSpriteWithSnap(guildEmblem, "union_mark_" + $"{base.localUser.guildInfo.emblem:00}");
		UISetter.SetSpriteWithSnap(changeEmblem, "union_mark_" + $"{changeEmblemIdx:00}");
	}

	public override void OnClick(GameObject sender)
	{
		string text = base.gameObject.name;
		base.OnClick(sender);
	}
}
