public class UICooperateBattlePointGuildRankListItem : UIItemBase
{
	public UISprite bg;

	public new UILabel name;

	public UILabel server;

	public UISprite emblem;

	public UILabel level;

	public UILabel point;

	public UILabel rank;

	public UISprite mark;

	public UILabel stage;

	public void Set(Protocols.CooperateBattlePointGuildRankingInfo data)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		if (data.gIdx != localUser.guildInfo.idx)
		{
			bg.spriteName = "com_bg_popup_inside";
		}
		else
		{
			bg.spriteName = "login_bg_sever_select";
		}
		UISetter.SetLabel(name, data.name);
		UISetter.SetLabel(server, Localization.Format("19067", data.server));
		UISetter.SetSprite(emblem, "union_mark_" + $"{data.eblm:00}");
		UISetter.SetLabel(level, "Lv " + data.lv);
		UISetter.SetLabel(point, data.accDmg.ToString("N0"));
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
		UISetter.SetLabel(stage, Localization.Format("5090033", data.stage, data.step));
	}
}
