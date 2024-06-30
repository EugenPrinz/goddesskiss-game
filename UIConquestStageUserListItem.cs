using UnityEngine;

public class UIConquestStageUserListItem : UIItemBase
{
	public UISprite icon;

	public UISprite bg;

	public new UILabel name;

	public UILabel level;

	public UILabel enemyLabel;

	public UILabel alieLabel;

	public GameObject authRoot;

	public UILabel authLabel;

	public GameObject block;

	public void Set(Protocols.ConquestStageInfo.User user, bool isAlie)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetSprite(icon, RemoteObjectManager.instance.regulation.GetCostumeThumbnailName(int.Parse(user.thumb)));
		UISetter.SetColor(bg, (!(user.uno == localUser.uno)) ? Color.white : new Color(84f / 85f, 77f / 85f, 38f / 85f));
		UISetter.SetLabel(name, user.name);
		UISetter.SetLabel(level, user.level);
		UISetter.SetActive(alieLabel, isAlie);
		UISetter.SetActive(enemyLabel, !isAlie);
		UISetter.SetActive(authRoot, user.auth != 0);
		if (user.auth != 0)
		{
			UISetter.SetLabel(authLabel, (user.auth != 2) ? Localization.Get("110112") : Localization.Get("112009"));
		}
		if (isAlie)
		{
			UISetter.SetLabel(alieLabel, Localization.Format("110327", user.standby, user.move));
		}
		else
		{
			UISetter.SetLabel(enemyLabel, Localization.Format("110328", user.standby));
		}
	}

	public virtual void Set(Protocols.GuildMember.MemberData user, int count)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetSprite(icon, RemoteObjectManager.instance.regulation.GetCostumeThumbnailName(user.thumnail));
		UISetter.SetColor(bg, (user.uno != int.Parse(localUser.uno)) ? Color.white : new Color(84f / 85f, 77f / 85f, 38f / 85f));
		UISetter.SetLabel(name, user.name);
		UISetter.SetLabel(level, user.level);
		UISetter.SetActive(authRoot, user.memberGrade != 0);
		if (user.memberGrade != 0)
		{
			UISetter.SetLabel(authLabel, (user.memberGrade != 1) ? Localization.Get("110112") : Localization.Get("112009"));
		}
		UISetter.SetActive(block, count == 0);
		UISetter.SetLabel(alieLabel, (count != 0) ? Localization.Format("110338", count) : Localization.Get("110067"));
	}

	public virtual void Set(Protocols.GuildMember.MemberData user, Protocols.ConquestStageInfo.User data)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetSprite(icon, RemoteObjectManager.instance.regulation.GetCostumeThumbnailName(user.thumnail));
		UISetter.SetColor(bg, (user.uno != int.Parse(localUser.uno)) ? Color.white : new Color(84f / 85f, 77f / 85f, 38f / 85f));
		UISetter.SetLabel(name, user.name);
		UISetter.SetLabel(level, user.level);
		UISetter.SetActive(authRoot, user.memberGrade != 0);
		if (user.memberGrade != 0)
		{
			UISetter.SetLabel(authLabel, (user.memberGrade != 1) ? Localization.Get("110112") : Localization.Get("112009"));
		}
		if (data != null)
		{
			UISetter.SetLabel(alieLabel, Localization.Format("110327", data.standby, data.move));
		}
		else
		{
			UISetter.SetLabel(alieLabel, Localization.Format("110327", 0, 0));
		}
	}
}
