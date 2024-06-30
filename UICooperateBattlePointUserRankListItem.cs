using UnityEngine;

public class UICooperateBattlePointUserRankListItem : UIItemBase
{
	public UISprite icon;

	public UISprite bg;

	public new UILabel name;

	public UILabel level;

	public UILabel point;

	public UILabel rank;

	public UISprite mark;

	public UILabel world;

	public void Set(Protocols.CooperateBattlePointUserRankingInfo data)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetSprite(icon, RemoteObjectManager.instance.regulation.GetCostumeThumbnailName(data.thumb));
		UISetter.SetColor(bg, (!(data.uno == localUser.uno)) ? Color.white : new Color(84f / 85f, 77f / 85f, 38f / 85f));
		UISetter.SetLabel(name, data.name);
		UISetter.SetLabel(level, "Lv " + data.lv);
		UISetter.SetLabel(point, data.accDmg.ToString("N0"));
		UISetter.SetLabel(world, Localization.Format("19067", data.world));
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
	}
}
