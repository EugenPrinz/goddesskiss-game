using Shared.Regulation;

public class UIConquestEnemyGuildInformation : UISimplePopup
{
	public UISprite emblem;

	public new UILabel name;

	public UILabel server;

	public UILabel level;

	public UILabel count;

	public void Init(Protocols.ConquestTroopInfo.Enemy eGuild)
	{
		GuildLevelInfoDataRow guildLevelInfoDataRow = RemoteObjectManager.instance.regulation.guildLevelInfoDtbl[eGuild.level.ToString()];
		UISetter.SetLabel(name, eGuild.name);
		UISetter.SetLabel(server, Localization.Format("19067", eGuild.world));
		UISetter.SetLabel(level, "Lv " + eGuild.level);
		UISetter.SetSpriteWithSnap(emblem, "union_mark_" + $"{eGuild.emblem:00}");
		UISetter.SetLabel(count, $"[FF6000FF]{eGuild.mcnt}[-][4E1F00FF]/{guildLevelInfoDataRow.maxcount}[-]");
	}
}
