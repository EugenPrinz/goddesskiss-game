using UnityEngine;

public class UIEventRaidJoinListItem : UIItemBase
{
	public UISprite userThumb;

	public UISprite bg;

	public UILabel userLevel;

	public UILabel userName;

	public UILabel damage;

	public GameObject authRoot;

	public UILabel authLabel;

	public void Set(Protocols.EventRaidRankingData data)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetSprite(userThumb, RemoteObjectManager.instance.regulation.GetCostumeThumbnailName(int.Parse(data.userThumb)));
		UISetter.SetSprite(bg, (data.isown != 1) ? "com_bg_popup_inside" : "login_bg_sever_select");
		UISetter.SetLabel(userName, data.userName);
		UISetter.SetLabel(damage, data.damage.ToString("N0"));
		UISetter.SetLabel(userLevel, $"Lv {data.level}");
		UISetter.SetActive(authRoot, data.authority != 0);
		if (data.authority != 0)
		{
			UISetter.SetLabel(authLabel, (data.authority != 2) ? Localization.Get("110112") : Localization.Get("112009"));
		}
	}
}
