using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class GroupListItem : UIItemBase
{
	public UILabel title;

	public UILabel description;

	public UIDefaultListView memberListView;

	public UIGoods reward;

	public UISprite bg;

	public UILabel completeLabel;

	public GameObject rewardBtn;

	public GameObject disableBtn;

	public GameObject completeBtn;

	public void Set(GroupInfoDataRow row)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetLabel(title, Localization.Get(row.groupName));
		UISetter.SetLabel(description, Localization.Get(row.groupComment));
		UISetter.SetActive(description, !row.groupComment.Equals("0"));
		List<GroupMemberDataRow> list = RemoteObjectManager.instance.regulation.FindGroupMemberList(row.groupIdx);
		memberListView.InitGroupMember(list, "Member-");
		UISetter.SetGameObjectName(rewardBtn.gameObject, "EnableBtn-" + row.groupIdx);
		UISetter.SetActive(rewardBtn, active: false);
		UISetter.SetActive(disableBtn, active: false);
		UISetter.SetActive(completeBtn, active: false);
		reward.Set(row);
		if (localUser.completeRewardGroupList.Contains(int.Parse(row.groupIdx)))
		{
			UISetter.SetActive(completeBtn, active: true);
			if (row.rewardType >= ERewardType.GroupEff_1 && row.rewardType <= ERewardType.GroupEff_8)
			{
				UISetter.SetLabel(completeLabel, Localization.Get("1307"));
			}
			else
			{
				UISetter.SetLabel(completeLabel, Localization.Get("1009"));
			}
			bg.color = new Color(81f / 85f, 37f / 51f, 0.6509804f);
		}
		else if (localUser.isGetRewardGroup(row.groupIdx))
		{
			UISetter.SetActive(rewardBtn, active: true);
			bg.color = new Color(0.9843137f, 77f / 85f, 0.44313726f);
		}
		else
		{
			UISetter.SetActive(disableBtn, active: true);
			bg.color = new Color(1f, 1f, 1f);
		}
	}
}
