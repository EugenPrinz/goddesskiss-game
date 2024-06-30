using Shared.Regulation;

public class UIGuildRankingListItem : UIItemBase
{
	public new UILabel name;

	public UILabel server;

	public UILabel memberCount;

	public UISprite emblem;

	public UILabel level;

	public UILabel point;

	public UILabel score;

	public UISprite bg;

	public UISprite mark;

	public UILabel rank;

	public void Set(Protocols.GuildRankingInfo data)
	{
		GuildLevelInfoDataRow guildLevelInfoDataRow = RemoteObjectManager.instance.regulation.guildLevelInfoDtbl[data.level.ToString()];
		UISetter.SetLabel(name, data.guildName);
		UISetter.SetLabel(server, Localization.Format("19067", data.world));
		UISetter.SetLabel(memberCount, $"{data.count} / {guildLevelInfoDataRow.maxcount}");
		UISetter.SetLabel(level, "Lv " + data.level);
		UISetter.SetLabel(point, data.point.ToString("N0"));
		UISetter.SetLabel(score, Localization.Format("17070", data.score));
		UISetter.SetSprite(emblem, "union_mark_" + $"{data.emblem:00}");
		UISetter.SetLabel(rank, data.rank);
		if (data.rank < 4)
		{
			UISetter.SetActive(mark, active: true);
			UISetter.SetActive(rank, active: false);
			UISetter.SetSprite(mark, "pvp_ranking_" + data.rank);
		}
		else
		{
			UISetter.SetActive(rank, active: true);
			UISetter.SetActive(mark, active: false);
		}
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		if (data.idx != localUser.guildInfo.idx)
		{
			bg.spriteName = "com_bg_popup_inside";
		}
		else
		{
			bg.spriteName = "login_bg_sever_select";
		}
	}
}
