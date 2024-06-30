using UnityEngine;

public class UIGuildListItem : UIItemBase
{
	public new UILabel name;

	public UILabel world;

	public UILabel memberCount;

	public UILabel notice;

	public UISprite emblem;

	public UILabel level;

	public UILabel point;

	public UISprite bg;

	public GameObject joinButton;

	public GameObject applyButton;

	public GameObject cancelButton;

	public void Set(RoGuild guild)
	{
		UISetter.SetLabel(name, guild.gnm);
		UISetter.SetLabel(world, Localization.Format("19067", guild.world));
		UISetter.SetLabel(memberCount, $"{guild.cnt} / {guild.mxCnt}");
		UISetter.SetLabel(notice, guild.ntc);
		UISetter.SetLabel(level, "Lv " + guild.lev);
		UISetter.SetLabel(point, guild.apnt);
		UISetter.SetActive(joinButton, guild.list != "req" && guild.gtyp == 1);
		UISetter.SetActive(applyButton, guild.list != "req" && guild.gtyp == 2);
		UISetter.SetActive(cancelButton, guild.list == "req");
		if (guild.list == "req")
		{
			bg.color = new Color(0.98f, 0.9f, 0.44f);
		}
		else
		{
			bg.color = Color.white;
		}
		UISetter.SetSpriteWithSnap(emblem, "union_mark_" + $"{guild.emb:00}");
		UISetter.SetGameObjectName(joinButton, $"{_GetOriginalName(joinButton)}-{guild.gidx}");
		UISetter.SetGameObjectName(applyButton, $"{_GetOriginalName(applyButton)}-{guild.gidx}");
		UISetter.SetGameObjectName(cancelButton, $"{_GetOriginalName(cancelButton)}-{guild.gidx}");
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
