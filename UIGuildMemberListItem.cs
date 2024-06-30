using Shared.Regulation;
using UnityEngine;

public class UIGuildMemberListItem : UIItemBase
{
	public new UILabel name;

	public UILabel time;

	public UISprite icon;

	public UILabel level;

	public GameObject master;

	public GameObject subMaster;

	public UISprite myBG;

	public GameObject btnReject;

	public GameObject btnApprove;

	public UILabel totalPoint;

	public UILabel todayPoint;

	public UILabel world;

	public UILabel pbPoint;

	public void Set(Protocols.GuildMember.MemberData member)
	{
		UISetter.SetLabel(name, member.name);
		UISetter.SetLabel(time, Utility.GetTimeString(member.lastTime));
		UISetter.SetLabel(level, "Lv" + member.level);
		UISetter.SetLabel(world, Localization.Format("19067", member.world));
		UISetter.SetLabel(pbPoint, Localization.Get("21010") + member.paymentBonusPoint);
		UISetter.SetActive(master, member.memberGrade == 1);
		UISetter.SetActive(subMaster, member.memberGrade == 2);
		UISetter.SetActive(myBG, member.uno.ToString() == RemoteObjectManager.instance.localUser.uno);
		Regulation regulation = RemoteObjectManager.instance.regulation;
		if (member.uno.ToString() == RemoteObjectManager.instance.localUser.uno)
		{
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.FindCostumeData(member.thumnail);
			if (commanderCostumeDataRow != null)
			{
				RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommander(commanderCostumeDataRow.cid.ToString());
				if (roCommander != null && roCommander.isBasicCostume)
				{
					UISetter.SetSpriteWithSnap(icon, roCommander.resourceId + "_" + roCommander.currentViewCostume, pixelPerfect: false);
				}
				else
				{
					UISetter.SetSpriteWithSnap(icon, regulation.GetCostumeThumbnailName(member.thumnail), pixelPerfect: false);
				}
			}
		}
		else
		{
			UISetter.SetSpriteWithSnap(icon, regulation.GetCostumeThumbnailName(member.thumnail), pixelPerfect: false);
		}
		UISetter.SetLabel(totalPoint, member.totalPoint);
		UISetter.SetLabel(todayPoint, string.Format("{0}/{1}", member.todayPoint, regulation.defineDtbl["GUILD_POINT_DAILY_MAX"].value));
		UISetter.SetGameObjectName(btnReject, $"{_GetOriginalName(btnReject)}-{member.uno}");
		UISetter.SetGameObjectName(btnApprove, $"{_GetOriginalName(btnApprove)}-{member.uno}");
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
