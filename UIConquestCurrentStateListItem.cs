using UnityEngine;

public class UIConquestCurrentStateListItem : UIConquestStageUserListItem
{
	public GameObject resultBtn;

	public void Set(Protocols.ConquestStageInfo.User user)
	{
		Set(user, isAlie: true);
	}

	public override void Set(Protocols.GuildMember.MemberData user, int count)
	{
		base.Set(user, count);
	}

	public override void Set(Protocols.GuildMember.MemberData user, Protocols.ConquestStageInfo.User data)
	{
		base.Set(user, data);
	}
}
