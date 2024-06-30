using System.Collections.Generic;
using Shared.Regulation;

public class CooperateBattleRewardListItem : UIItemBase
{
	public int step;

	public UILabel title;

	public UILabel rewardVal;

	private void Start()
	{
		Set(RemoteObjectManager.instance.regulation.cooperateBattleStepDtbl[step][0]);
	}

	public void Set(CooperateBattleDataRow data)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetLabel(title, Localization.Get(data.name));
		List<RewardDataRow> list = regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == ERewardCategory.CoopereateBattle && row.type == step);
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			num += list[i].minCount;
		}
		UISetter.SetLabel(rewardVal, num);
	}
}
