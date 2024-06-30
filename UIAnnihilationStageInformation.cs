using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIAnnihilationStageInformation : UIPopup
{
	public UILabel goldLabel;

	public UIGrid normalGrid;

	public UIGrid vipGrid;

	public void Init(int stage)
	{
		SetAutoDestroy(autoDestory: true);
		RewardDataRow goldData = null;
		List<RewardDataRow> normalRewardList = new List<RewardDataRow>();
		List<RewardDataRow> vipRewardList = new List<RewardDataRow>();
		UserLevelDataRow userLevelDataRow = base.regulation.GetUserLevelDataRow(base.localUser.level);
		base.regulation.rewardDtbl.ForEach(delegate(RewardDataRow row)
		{
			if (row.category == ERewardCategory.Annihilation && row.type == stage)
			{
				if (row.typeIndex == 100)
				{
					goldData = row;
				}
				else if (row.typeIndex == 0)
				{
					normalRewardList.Add(row);
				}
				else
				{
					vipRewardList.Add(row);
				}
			}
		});
		if (vipRewardList.Count > 1)
		{
			vipRewardList.Sort((RewardDataRow row, RewardDataRow row1) => row.typeIndex.CompareTo(row1.typeIndex));
		}
		Transform transform = normalGrid.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			UIGoods component = transform.GetChild(i).gameObject.GetComponent<UIGoods>();
			UISetter.SetActive(component.emptyRoot, i >= normalRewardList.Count);
			if (i < normalRewardList.Count)
			{
				RewardDataRow reward = normalRewardList[i];
				component.Set(reward);
			}
		}
		Transform transform2 = vipGrid.transform;
		for (int j = 0; j < transform2.childCount; j++)
		{
			UIGoods component2 = transform2.GetChild(j).gameObject.GetComponent<UIGoods>();
			UISetter.SetActive(component2.vip, j < vipRewardList.Count);
			UISetter.SetActive(component2.emptyRoot, j >= vipRewardList.Count);
			if (j < vipRewardList.Count)
			{
				RewardDataRow reward2 = vipRewardList[j];
				component2.Set(reward2);
			}
		}
		if (goldData != null)
		{
			UISetter.SetLabel(goldLabel, string.Format("{0}~{1}", ((int)((float)goldData.minCount * userLevelDataRow.goldIncrease / 100f)).ToString("N0"), ((int)((float)goldData.maxCount * userLevelDataRow.goldIncrease / 100f)).ToString("N0")));
		}
	}
}
