using UnityEngine;

public class UIMultiplePopup : UISimplePopup
{
	public GameObject btn1;

	public GameObject btn2;

	public GameObject btn3;

	public UILabel btnLabel1;

	public UILabel btnLabel2;

	public UILabel btnLabel3;

	public UILabel btnPaymentLabel;

	public UISprite costSprite;

	public UILabel costLabel;

	public GameObject other;

	public GameObject payment;

	public UIGrid btnGrid;

	private const int minCellWidth = 200;

	private const int maxCellWidth = 290;

	private int btnCount;

	private int cost;

	public MultiplePopUpType type;

	public void initData(MultiplePopUpType _type, int _cost = 0)
	{
		type = _type;
		cost = _cost;
		btnCount = 0;
		switch (_type)
		{
		case MultiplePopUpType.NOTENOUGH_GOLD:
			SetData(2, "1029", "1030", "Popup.Common.NotEnoughGold.Shop.SubMessage", "Close", null, "GoMetroBank", "Common.Confirm", null, "1004", _otherState: true, _paymentState: false);
			break;
		case MultiplePopUpType.NOTENOUGH_CASH:
			SetData(1, "1031", "1032", "Popup.Common.NotEnoughCash.Shop.SubMessage", "Close", null, "GoCashShop", "Common.Confirm", null, "Building.UnitResearch.Popup.NotEnough.Shop", _otherState: true, _paymentState: false);
			break;
		case MultiplePopUpType.RECRUIT_COMMANDER:
			SetData(2, "Popup.Academy.Commander.Complete.Title", "Popup.Academy.Commander.Complete.Message", "Popup.Academy.Commander.Complete.SubMessage", "Close", null, "CommanderRecruit", "Common.Confirm", null, "Building.UnitResearch.Popup.NotEnough.Shop", _otherState: false, _paymentState: true);
			break;
		case MultiplePopUpType.RECRUIT_COMMANDER_DELAY:
			SetData(2, "Popup.Academy.Commander.Delay.Title", "Popup.Academy.Commander.Delay.Message", "Popup.Academy.Commander.Delay.SubMessage", "Close", null, "CommanderDelay", "Common.Cancel", null, "Popup.Academy.Commander.Delay.Setting", _otherState: false, _paymentState: true, EPaymentType.Cash);
			break;
		case MultiplePopUpType.RECRUIT_COMMANDER_DELAY_RENEWAL:
			SetData(3, "Popup.Academy.Commander.Delay.Renewal.Title", "Popup.Academy.Commander.Delay.Renewal.Message", "Popup.Academy.Commander.Delay.Renewal.SubMessage", "Close", "CommanderDelayCancle", "CommanderDelayRenewal", "Common.Cancel", "Popup.Academy.Commander.Delay.Renewal.Cancle", "Popup.Academy.Commander.Delay.Renewal.Ok", _otherState: false, _paymentState: true);
			break;
		case MultiplePopUpType.RECRUIT_COMMANDER_REFRESH:
			SetData(2, "Popup.Academy.Commander.Refresh.Title", "Popup.Academy.Commander.Refresh.Message", "Popup.Academy.Commander.Refresh.SubMessage", "Close", null, "CommanderRefresh", "Common.Cancel", null, "Popup.Academy.Commander.Refresh", _otherState: false, _paymentState: true);
			break;
		case MultiplePopUpType.UPGRADE_BUILDING_INSTANTLY:
			SetData(2, "5984", "5985", "5986", "Close", null, "UpgradeBuilding", "1000", null, "5647", _otherState: false, _paymentState: true, EPaymentType.Cash);
			break;
		}
	}

	public void SetData(int _btnCount, string _title, string _message, string _subMessage, string _btnName1, string _btnName2, string _btnName3, string _btnLabel1, string _btnLabel2, string _btnLabel3, bool _otherState, bool _paymentState, EPaymentType _type = EPaymentType.Undefined, int _cost = 0)
	{
		btnCount = _btnCount;
		UISetter.SetLabel(title, Localization.Get(_title));
		UISetter.SetLabel(message, Localization.Get(_message));
		if (subMessage != null)
		{
			if (_subMessage == null)
			{
				UISetter.SetActive(subMessage, active: false);
			}
			else
			{
				UISetter.SetActive(subMessage, active: true);
				UISetter.SetLabel(subMessage, Localization.Get(_subMessage));
			}
		}
		if (_btnName1 != null)
		{
			btn1.name = _btnName1;
			UISetter.SetLabel(btnLabel1, Localization.Get(_btnLabel1));
		}
		if (_btnName2 != null)
		{
			btn2.name = _btnName2;
			UISetter.SetLabel(btnLabel2, Localization.Get(_btnLabel2));
		}
		if (_btnName3 != null)
		{
			btn3.name = _btnName3;
			if (_otherState)
			{
				UISetter.SetLabel(btnLabel3, Localization.Get(_btnLabel3));
			}
			else
			{
				switch (_type)
				{
				case EPaymentType.Cash:
					UISetter.SetSprite(costSprite, "Goods-cash");
					break;
				case EPaymentType.Ring:
					UISetter.SetSprite(costSprite, "Goods-ring");
					break;
				default:
					UISetter.SetSprite(costSprite, "Goods-gold");
					break;
				}
				if (_cost == 0)
				{
					_cost = cost;
				}
				UISetter.SetLabel(btnPaymentLabel, Localization.Get(_btnLabel3));
				UISetter.SetLabel(costLabel, _cost.ToString("N0"));
			}
		}
		UISetter.SetActive(other, _otherState);
		UISetter.SetActive(payment, _paymentState);
		SetButtonControl();
	}

	private void SetButtonControl()
	{
		if (btnCount < 3)
		{
			UISetter.SetActive(btn2, active: false);
			btnGrid.cellWidth = 200f;
			btnGrid.Reposition();
		}
		else
		{
			UISetter.SetActive(btn2, active: true);
			btnGrid.cellWidth = 290f;
			btnGrid.Reposition();
		}
	}

	public override void OnClick(GameObject sender)
	{
		switch (sender.name)
		{
		case "GoMetroBank":
			ClosePopup();
			base.network.RequestBankInfo();
			break;
		case "UpgradeBuilding":
		{
			EBuilding selectedBuilding = base.uiWorld.camp.selectedBuilding;
			base.network.RequestBuildingLevelUpImmediate(selectedBuilding);
			break;
		}
		}
		base.OnClick(sender);
	}
}
