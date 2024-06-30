using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIScoreRewardItem : UIItemBase
{
	public UISprite bg;

	public UISprite scoreBg;

	public UILabel score;

	public GameObject getBtn;

	public GameObject completeRoot;

	public GameObject enableRoot;

	public GameObject disableRoot;

	public UIDefaultListView itemList;

	[HideInInspector]
	public ERewardState state;

	[HideInInspector]
	public List<RewardDataRow> rewardList;

	public void Set(int point, List<RewardDataRow> list)
	{
		rewardList = list;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetLabel(score, point);
		state = localUser.GetRewardState(point);
		UISetter.SetActive(completeRoot, state == ERewardState.Complete);
		UISetter.SetActive(getBtn, state != ERewardState.Complete);
		UISetter.SetActive(enableRoot, state == ERewardState.Receptible || state == ERewardState.NonReceptible);
		UISetter.SetActive(disableRoot, state == ERewardState.Nothing);
		list.Sort(compare);
		itemList.InitRewardList(list);
		switch (state)
		{
		case ERewardState.Nothing:
			UISetter.SetSprite(bg, "com_bg_popup_inside2");
			UISetter.SetSprite(scoreBg, "com_bt_circle_black");
			break;
		case ERewardState.Receptible:
			UISetter.SetSprite(bg, "com_bg_popup_inside");
			UISetter.SetSprite(scoreBg, "com_bt_circle_black");
			break;
		case ERewardState.NonReceptible:
			UISetter.SetSprite(bg, "com_bg_popup_inside");
			UISetter.SetSprite(scoreBg, "com_bt_circle_black");
			break;
		case ERewardState.Complete:
			UISetter.SetSprite(bg, "com_bg_popup_inside4");
			UISetter.SetSprite(scoreBg, "com_bt_circle_orange");
			break;
		}
	}

	public void SetRaidScore(int idx, List<RewardDataRow> list)
	{
		rewardList = list;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		RankingDataRow rankingDataRow = RemoteObjectManager.instance.regulation.rankingDtbl.Find((RankingDataRow row) => row.r_idx == idx);
		UISetter.SetLabel(score, Localization.Get(rankingDataRow.name));
		state = localUser.GetRaidRewardState(idx);
		UISetter.SetActive(completeRoot, state == ERewardState.Complete);
		UISetter.SetActive(getBtn, state != ERewardState.Complete);
		UISetter.SetActive(enableRoot, state == ERewardState.Receptible || state == ERewardState.NonReceptible);
		UISetter.SetActive(disableRoot, state == ERewardState.Nothing);
		list.Sort(compare);
		itemList.InitRewardList(list);
		switch (state)
		{
		case ERewardState.Nothing:
			UISetter.SetSprite(bg, "com_bg_popup_inside2");
			break;
		case ERewardState.Receptible:
			UISetter.SetSprite(bg, "com_bg_popup_inside");
			break;
		case ERewardState.NonReceptible:
			UISetter.SetSprite(bg, "com_bg_popup_inside");
			break;
		case ERewardState.Complete:
			UISetter.SetSprite(bg, "com_bg_popup_inside4");
			break;
		}
	}

	public int compare(RewardDataRow x, RewardDataRow y)
	{
		return x.typeIndex.ToString().CompareTo(y.typeIndex.ToString());
	}
}
