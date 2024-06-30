using Shared.Regulation;
using UnityEngine;

public class UICashShopListItem : UIItemBase
{
	public UILabel title;

	public UILabel cost;

	public UILabel cost2;

	public UILabel description;

	public UILabel description2;

	public UILabel rechargeCash;

	public UISprite costIcon;

	public UISprite costIcon2;

	public UISprite icon;

	public UISprite detailView;

	public UILabel buyCount;

	public UIButton enableBuyButton;

	public UIButton disableBuyButton;

	public UILabel disableBuyLabel;

	public GameObject selectSprite;

	public GameObject btnRoot;

	public GameObject eventRoot;

	public GameObject firstEventRoot;

	public GameObject disableRoot;

	public GameObject buyCountRoot;

	public void Set(Protocols.CashShopData row)
	{
		InAppProductDataRow inAppProduct = RemoteObjectManager.instance.regulation.GetInAppProduct(row.priceId);
		UISetter.SetActive(eventRoot, row.eventCash > 0);
		UISetter.SetActive(firstEventRoot, inAppProduct.type != ECashRechargeType.Package && row.firstBuyCash > 0);
		UISetter.SetActive(rechargeCash, inAppProduct.type != ECashRechargeType.Package);
		UISetter.SetActive(detailView, inAppProduct.type == ECashRechargeType.Package);
		if (inAppProduct.type == ECashRechargeType.Month)
		{
			UISetter.SetLabel(description, Localization.Get(inAppProduct.stringidx));
			if (row.remainTime > 0.0)
			{
				UISetter.SetLabel(description2, Localization.Format("1048", Utility.GetTimeString(row.remainTime)));
			}
			else
			{
				UISetter.SetLabel(description2, Localization.Get(inAppProduct.explanation));
			}
		}
		else if (row.eventCash > 0)
		{
			UISetter.SetLabel(description, Localization.Get(inAppProduct.stringidxEvent));
			UISetter.SetLabel(description2, Localization.Format(inAppProduct.explanationEvent, inAppProduct.cash, row.eventCash));
		}
		else if (row.firstBuyCash > 0)
		{
			UISetter.SetLabel(description, Localization.Get(inAppProduct.stringidx));
			UISetter.SetLabel(description2, Localization.Format(inAppProduct.explanationFirst, inAppProduct.cash));
		}
		else
		{
			UISetter.SetLabel(description, Localization.Get(inAppProduct.stringidx));
			UISetter.SetLabel(description2, Localization.Format(inAppProduct.explanation, inAppProduct.cash));
		}
		UISetter.SetLabel(cost, row.price);
		UISetter.SetLabel(cost2, row.price);
		UISetter.SetSprite(costIcon, (row.pType != ECashRechargePriceType.Won) ? "store_dollar" : "store_won", snap: true);
		UISetter.SetSprite(costIcon2, (row.pType != ECashRechargePriceType.Won) ? "store_dollar_gray" : "store_won_gray", snap: true);
		UISetter.SetLabel(rechargeCash, (inAppProduct.type != ECashRechargeType.Month) ? (inAppProduct.cash + row.eventCash + row.firstBuyCash).ToString() : Localization.Format("5768", "30"));
		UISetter.SetSprite(icon, inAppProduct.icon, snap: true);
		if (inAppProduct.availableLevel > RemoteObjectManager.instance.localUser.level)
		{
			UISetter.SetActive(enableBuyButton, active: false);
			UISetter.SetActive(disableBuyButton, active: true);
			UISetter.SetLabel(disableBuyLabel, Localization.Format("10000209", inAppProduct.availableLevel));
		}
		else
		{
			UISetter.SetActive(enableBuyButton, inAppProduct.count == 0 || row.buyCount < inAppProduct.count);
			UISetter.SetActive(disableBuyButton, inAppProduct.count > 0 && row.buyCount >= inAppProduct.count);
			UISetter.SetLabel(disableBuyLabel, Localization.Get("10000208"));
		}
		UISetter.SetActive(buyCountRoot, inAppProduct.count > 0);
		UISetter.SetLabel(buyCount, row.buyCount + " / " + inAppProduct.count);
		UISetter.SetGameObjectName(detailView.gameObject, $"RewardList-{inAppProduct.iapidx}");
	}
}
