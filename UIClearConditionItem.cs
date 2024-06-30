using Shared.Regulation;

public class UIClearConditionItem : UIItemBase
{
	public UILabel label;

	public void Set(EBattleClearCondition condition, string value)
	{
		switch (condition)
		{
		case EBattleClearCondition.None:
			UISetter.SetLabel(label, Localization.Get("10000111"));
			break;
		case EBattleClearCondition.Survival:
			UISetter.SetLabel(label, string.Format(Localization.Get("10000104"), value));
			break;
		case EBattleClearCondition.LimitedTurn:
			UISetter.SetLabel(label, string.Format(Localization.Get("10000105"), value));
			break;
		case EBattleClearCondition.Include_Attacker:
			UISetter.SetLabel(label, string.Format(Localization.Get("10000106"), value));
			break;
		case EBattleClearCondition.Include_Defender:
			UISetter.SetLabel(label, string.Format(Localization.Get("10000107"), value));
			break;
		case EBattleClearCondition.Include_Supporter:
			UISetter.SetLabel(label, string.Format(Localization.Get("10000108"), value));
			break;
		case EBattleClearCondition.Include_Commander:
		{
			Regulation regulation = RemoteObjectManager.instance.regulation;
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[value];
			UISetter.SetLabel(label, string.Format(Localization.Get("10000109"), commanderDataRow.nickname));
			break;
		}
		case EBattleClearCondition.Include_Minimum_Count:
			UISetter.SetLabel(label, string.Format(Localization.Get("10000110"), value));
			break;
		case EBattleClearCondition.All_Survival:
			UISetter.SetLabel(label, Localization.Get("10000113"));
			break;
		}
	}
}
