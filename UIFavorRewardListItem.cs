using Shared.Regulation;
using UnityEngine;

public class UIFavorRewardListItem : UIItemBase
{
	public new UILabel name;

	public UILabel description;

	public GameObject objFinish;

	public GameObject objComplete;

	public GameObject objLock;

	public GameObject rewardMedal;

	public UISprite thumnail;

	public GameObject rewardItem;

	public UISprite icon;

	public void Set(RewardDataRow rewardData)
	{
		RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommander(rewardData.type.ToString());
		FavorStepDataRow favorStepDataRow = RemoteObjectManager.instance.regulation.FindFavorStepData(rewardData.typeIndex);
		UISetter.SetLabel(name, Localization.Format("10072", rewardData.typeIndex));
		UISetter.SetActive(rewardMedal, rewardData.rewardType == ERewardType.Medal);
		UISetter.SetActive(rewardItem, rewardData.rewardType == ERewardType.Goods);
		if (rewardData.rewardType == ERewardType.Goods)
		{
			GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.FindGoodsServerFieldName(rewardData.rewardIdx.ToString());
			UISetter.SetLabel(description, Localization.Format("10073", Localization.Get(goodsDataRow.name), rewardData.minCount));
			UISetter.SetSprite(icon, goodsDataRow.iconId);
		}
		else if (rewardData.rewardType == ERewardType.Medal)
		{
			UISetter.SetLabel(description, $"개인훈장 {rewardData.minCount}개 지급");
			UISetter.SetSprite(thumnail, "c_" + $"{rewardData.rewardIdx:000}" + "_1");
		}
		UISetter.SetActive(objComplete, (int)roCommander.FavorStep >= rewardData.typeIndex);
		UISetter.SetActive(objLock, (int)roCommander.FavorStep + 1 == rewardData.typeIndex);
		UISetter.SetActive(objFinish, active: false);
		UISetter.SetActive(objComplete, active: false);
		UISetter.SetActive(objLock, active: true);
		if ((int)roCommander.FavorStep + 1 >= rewardData.typeIndex)
		{
			UISetter.SetActive(objLock, active: false);
		}
		if ((int)roCommander.FavorStep >= rewardData.typeIndex)
		{
			UISetter.SetActive(objComplete, active: true);
		}
		else if ((int)roCommander.FavorCount >= favorStepDataRow.favor)
		{
			UISetter.SetActive(objFinish, active: true);
		}
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
