using Shared.Regulation;
using UnityEngine;

public class UIEventBattleGachaItem : UIItemBase
{
	public UIGoods goods;

	public UILabel count;

	public GameObject pickup;

	public GameObject empty;

	public void Set(EventBattleGachaRewardDataRow row, int remainCount)
	{
		goods.Set(row.rewardType, row.rewardIdx, row.rewardAmount);
		UISetter.SetLabel(count, $"{remainCount}/{row.rewardCount}");
		UISetter.SetActive(empty, remainCount <= 0);
		UISetter.SetActive(pickup, row.mainReward == 1);
	}
}
