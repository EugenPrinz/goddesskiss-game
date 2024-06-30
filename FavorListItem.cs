using Shared.Regulation;
using UnityEngine;

public class FavorListItem : UIItemBase
{
	public UILabel profileType;

	public UILabel profileContent;

	public UILabel rewardStat;

	public UILabel rewardCount;

	public UISprite rewardIcon;

	public UISprite rewardCommanderIcon;

	public UILabel blockLimit;

	public GameObject goods;

	public GameObject stat;

	public GameObject block;

	public GameObject open;

	public BoxCollider collider;

	public bool openState => open.activeSelf;

	public void Set(FavorDataRow row, string cid, int favorStep, int favorRewardStep)
	{
		if (collider != null)
		{
		}
		UISetter.SetLabel(profileType, Localization.Get((44000 + row.step - 1).ToString()));
		UISetter.SetLabel(profileContent, Localization.Get(row.profile));
		UISetter.SetLabel(blockLimit, Localization.Format("20027", row.step));
		UISetter.SetActive(block, favorRewardStep < row.step);
		UISetter.SetActive(open, favorRewardStep < row.step && favorStep >= row.step);
		UISetter.SetActive(goods, row.statType == StatType.NONE);
		UISetter.SetActive(stat, row.statType != StatType.NONE);
		if (row.statType != 0)
		{
			UISetter.SetActive(rewardCommanderIcon, active: false);
			UISetter.SetLabel(rewardStat, GetStatType(row.statType));
			UISetter.SetLabel(rewardCount, (row.statType != StatType.CRITDMG && row.statType != StatType.CRITR) ? (" + " + row.stat) : (" + " + Localization.Format("5781", row.stat)));
			return;
		}
		int num = 0;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		RewardDataRow rewardItem = regulation.rewardDtbl.Find((RewardDataRow data) => data.category == ERewardCategory.FavorComplete && data.type == int.Parse(cid) && data.typeIndex == row.step);
		if (rewardItem == null)
		{
			return;
		}
		UISetter.SetActive(rewardCommanderIcon, rewardItem.rewardType == ERewardType.Medal);
		if (rewardItem.rewardType == ERewardType.Medal)
		{
			UISetter.SetSprite(rewardCommanderIcon, $"{regulation.commanderDtbl.Find((CommanderDataRow list) => list.id == rewardItem.rewardIdx.ToString()).resourceId}_1");
			UISetter.SetSpriteWithSnap(rewardIcon, "icon_c_line_2", pixelPerfect: false);
			num = rewardItem.minCount;
		}
		if (rewardItem.rewardType == ERewardType.Costume)
		{
			UISetter.SetSpriteWithSnap(rewardIcon, "like-costume", pixelPerfect: false);
			num = rewardItem.minCount;
		}
		else if (rewardItem.rewardIdx == 1000)
		{
			UISetter.SetSpriteWithSnap(rewardIcon, "Goods-exp", pixelPerfect: false);
			num = rewardItem.minCount;
		}
		else
		{
			if (regulation.goodsDtbl.ContainsKey(rewardItem.rewardIdx.ToString()))
			{
				UISetter.SetSpriteWithSnap(rewardIcon, regulation.goodsDtbl[rewardItem.rewardIdx.ToString()].iconId, pixelPerfect: false);
			}
			num = rewardItem.minCount;
		}
		UISetter.SetLabel(rewardCount, " x " + num);
	}

	private string GetStatType(StatType type)
	{
		string result = string.Empty;
		switch (type)
		{
		case StatType.ATK:
			result = Localization.Get("1");
			break;
		case StatType.DEF:
			result = Localization.Get("2");
			break;
		case StatType.HP:
			result = Localization.Get("4");
			break;
		case StatType.ACCUR:
			result = Localization.Get("5");
			break;
		case StatType.LUCK:
			result = Localization.Get("3");
			break;
		case StatType.CRITR:
			result = Localization.Get("6");
			break;
		case StatType.CRITDMG:
			result = Localization.Get("8");
			break;
		case StatType.MOB:
			result = Localization.Get("7");
			break;
		}
		return result;
	}
}
