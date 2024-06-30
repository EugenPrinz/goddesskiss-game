using Shared.Regulation;

public class UIBattleItem : UIItemBase
{
	public UISprite bg;

	public UISprite icon;

	public void Set(GuildSkillDataRow dr)
	{
		UISetter.SetSprite(icon, "union_skill_" + $"{dr.idx:00}");
	}
}
