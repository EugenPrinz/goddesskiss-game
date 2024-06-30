public class ProbabilityListItem : UIItemBase
{
	public UILabel goodsName;

	public UILabel probability;

	public void Set(ERewardType type, float probability)
	{
		switch (type)
		{
		case ERewardType.Goods:
		case ERewardType.Box:
		case ERewardType.EventItem:
		case ERewardType.CollectionItem:
			UISetter.SetLabel(goodsName, Localization.Get("4316"));
			break;
		case ERewardType.Medal:
			UISetter.SetLabel(goodsName, Localization.Get("4020"));
			break;
		}
		if (type == ERewardType.Commander)
		{
			UISetter.SetLabel(goodsName, Localization.Get("5586"));
		}
		if (type == ERewardType.UnitMaterial || type == ERewardType.Item)
		{
			UISetter.SetLabel(goodsName, Localization.Get("4314"));
		}
		if (type == ERewardType.Favor)
		{
			UISetter.SetLabel(goodsName, Localization.Get("4315"));
		}
		if (type == ERewardType.Costume)
		{
			UISetter.SetLabel(goodsName, Localization.Get("20002"));
		}
		UISetter.SetLabel(this.probability, $"{probability}%");
	}
}
