using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIInfinityBattleItem : UIItemBase
{
	public GameObject star;

	public UIClearConditionItem condition;

	public UILabel btnLabel;

	public UIDefaultListView rewardList;

	public GameObject completed;

	public GameObject rewardBtn;

	public void Set(string infinityIdx, EBattleClearCondition type, string battleClearValue, int missionIdx, int missionValue)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(star, missionValue >= 1);
		UISetter.SetActive(rewardBtn, missionValue < 2);
		UISetter.SetButtonEnable(rewardBtn, missionValue == 1);
		UISetter.SetActive(completed, missionValue == 2);
		UISetter.SetLabel(btnLabel, (missionValue != 1) ? Localization.Get("17078") : Localization.Get("1005"));
		List<RewardDataRow> list = regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == ERewardCategory.InfinityBattle && row.type == int.Parse(infinityIdx) && row.typeIndex == missionIdx);
		rewardList.InitRewardList(list);
		condition.Set(type, battleClearValue);
	}
}
