using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIRankingRewardItem : UIItemBase
{
	public UISprite icon;

	public UILabel title;

	public UIDefaultListView rewardListView;

	public GameObject receiveBtn;

	public GameObject completeRoot;

	public UILabel gradeNum;

	public UILabel ranking;

	[HideInInspector]
	public ERewardState state;

	[HideInInspector]
	public List<RewardDataRow> rewardList;

	public void Set(RankingDataRow data, int idx)
	{
		UISetter.SetSprite(icon, data.icon);
		UISetter.SetLabel(title, Localization.Get(data.name));
		UISetter.SetActive(receiveBtn, active: false);
		UISetter.SetActive(completeRoot, active: false);
		UISetter.SetLabel(ranking, Localization.Format("18010", idx + 1));
		UISetter.SetActive(gradeNum, data.type == ERankingContentsType.Challenge || data.type == ERankingContentsType.WorldDuel);
		if (data.type == ERankingContentsType.Challenge || data.type == ERankingContentsType.WorldDuel)
		{
			SetGradeLabel(data);
		}
		rewardList = RemoteObjectManager.instance.regulation.FindDuelRankingRewardList((int)data.type, data.r_idx);
		rewardListView.InitRewardList(rewardList);
	}

	public void SetReward(RankingDataRow data, ERankingContentsType type)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetSprite(icon, data.icon);
		UISetter.SetLabel(title, Localization.Get(data.name));
		if (type == ERankingContentsType.ChallengeGrade)
		{
			state = localUser.GetDuelRewardState(data.r_idx);
		}
		else
		{
			state = localUser.GetWorldDuelRewardState(data.r_idx, type);
		}
		UISetter.SetActive(receiveBtn, state == ERewardState.Receptible || state == ERewardState.NonReceptible);
		UISetter.SetActive(completeRoot, state == ERewardState.Complete);
		SetGradeLabel(data);
		rewardList = RemoteObjectManager.instance.regulation.FindDuelRankingRewardList((int)type, data.r_idx);
		rewardListView.InitRewardList(rewardList);
	}

	private void SetGradeLabel(RankingDataRow data)
	{
		List<RankingDataRow> list = RemoteObjectManager.instance.regulation.rankingDtbl.FindAll((RankingDataRow row) => row.type == data.type && row.icon == data.icon);
		UISetter.SetActive(gradeNum, list.Count > 1);
		int num = list.IndexOf(data);
		UISetter.SetLabel(gradeNum, num + 1);
	}
}
