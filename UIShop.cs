using UnityEngine;

public class UIShop : UIPopup
{
	public UILabel vipLevel;

	public UIProgressBar vipExpProgress;

	public UILabel vipExpValue;

	public UILabel vipLevelUpCondition;

	public UILabel eventContent;

	public UILabel flatSumContent;

	public UILabel userGold;

	public UILabel userCash;

	public UIDefaultListView goldListView;

	public UIDefaultListView cashListView;

	public UIFlipSwitch switchGold;

	public UIFlipSwitch switchCash;

	public UIFlipSwitch switchResource;

	public GameObject CashAndGoldPanel;

	public GameObject ResourcePanel;

	public UISprite block;

	private bool goVipInfo;

	public void initData()
	{
		goVipInfo = false;
	}

	private void Start()
	{
		UISetter.SetLabel(flatSumContent, Localization.Format("Shop.FlatSum", 30, 120, 3600, 200));
	}

	private void SetUserData()
	{
		UISetter.SetLabel(userGold, base.localUser.gold.ToString());
		UISetter.SetLabel(userCash, base.localUser.cash.ToString());
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "Close")
		{
			CloseAnimation();
		}
		else if (text.StartsWith("Cash-"))
		{
			UIPurchasePopup uIPurchasePopup = UIPopup.Create<UIPurchasePopup>("PurchasePopup");
			uIPurchasePopup.initData(PurChaseType.CASH, cashListView.GetPureId(text));
		}
		else if (text.StartsWith("Gold-"))
		{
			UIPurchasePopup uIPurchasePopup2 = UIPopup.Create<UIPurchasePopup>("PurchasePopup");
			uIPurchasePopup2.initData(PurChaseType.GOLD, goldListView.GetPureId(text));
		}
		else if (text == "VipBtn")
		{
			goVipInfo = true;
			CloseAnimation();
		}
		base.OnClick(sender);
	}

	public void Set()
	{
		_InitAndOpen(gold: true, cash: false, resource: false);
	}

	public void InitAndOpenGoldShop()
	{
		_InitAndOpen(gold: true, cash: false, resource: false);
	}

	public void InitAndOpenCashShop()
	{
		_InitAndOpen(gold: false, cash: true, resource: false);
	}

	public void InitAndOpenResourceShop()
	{
		_InitAndOpen(gold: false, cash: false, resource: true);
	}

	public void OnTab(GameObject sender)
	{
		switch (sender.name)
		{
		case "Gold":
			ShopControl(gold: true, cash: false, resource: false);
			break;
		case "Cash":
			ShopControl(gold: false, cash: true, resource: false);
			break;
		case "Resource":
			ShopControl(gold: false, cash: false, resource: true);
			break;
		}
	}

	public void ShopControl(bool gold, bool cash, bool resource)
	{
		SetUserData();
		if (resource)
		{
			UISetter.SetActive(CashAndGoldPanel, active: false);
			UISetter.SetActive(ResourcePanel, active: true);
		}
		else
		{
			if (gold)
			{
				UISetter.SetLabel(eventContent, Localization.Format("Shop.Event.Gold", 1000));
			}
			else
			{
				UISetter.SetLabel(eventContent, Localization.Get("Shop.Event.Cash"));
			}
			UISetter.SetActive(CashAndGoldPanel, active: true);
			UISetter.SetActive(ResourcePanel, active: false);
		}
		goldListView.scrollView.ResetPosition();
		cashListView.scrollView.ResetPosition();
		UISetter.SetActive(goldListView, gold);
		UISetter.SetActive(cashListView, cash);
	}

	private void _InitAndOpen(bool gold, bool cash, bool resource)
	{
		if (gold == cash)
		{
			gold = true;
			cash = false;
		}
		initData();
		Open();
		UISetter.SetFlipSwitch(switchGold, gold);
		UISetter.SetFlipSwitch(switchCash, cash);
		UISetter.SetFlipSwitch(switchResource, resource);
		ShopControl(gold, cash, resource);
		OpenAnimation();
	}

	private void OpenAnimation()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", 0, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeOutBack));
	}

	private void CloseAnimation()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", -1000, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeInBack, "oncomplete", "End", "oncompletetarget", base.gameObject));
	}

	public new void Open()
	{
		UISetter.SetActive(root, active: true);
	}

	public void End()
	{
		UISetter.SetActive(root, active: false);
	}
}
