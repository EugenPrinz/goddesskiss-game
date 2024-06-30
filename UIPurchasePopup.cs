using UnityEngine;

public class UIPurchasePopup : UISimplePopup
{
	public UISprite ConsumeIcon;

	public UISprite PurchaseIcon;

	public UILabel PurchaseLabel;

	public GameObject CashPanel;

	private PurChaseType type;

	private int index;

	private int cost;

	public void initData(PurChaseType _type, string idx)
	{
		type = _type;
		index = int.Parse(idx);
		cost = 0;
		string text = string.Empty;
		if (_type == PurChaseType.CASH)
		{
			text = "Popup.Purchase.Title.Cash";
			UISetter.SetSprite(ConsumeIcon, "store_won");
			UISetter.SetSprite(PurchaseIcon, "main_icon_cash");
			UISetter.SetLabel(PurchaseLabel, Localization.Format("Popup.Purchase.Description.Cash", 2000, 200));
			UISetter.SetActive(CashPanel, active: true);
		}
		Set(localization: true, text, null, null, "Common.Buy", "Common.Cancel", null);
	}

	public override void OnClick(GameObject sender)
	{
		if (type == PurChaseType.GOLD)
		{
			if (cost <= base.localUser.cash)
			{
				base.network.RequestShopBuyGold(index);
			}
			else
			{
				UISimplePopup.CreateOK(localization: true, "1031", "1032", null, "1001");
			}
		}
		base.OnClick(sender);
	}
}
