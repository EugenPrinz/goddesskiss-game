using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIWeaponUpgradePopup : UIPopup
{
	[Serializable]
	public class UIWeaponTradeTicket : UIInnerPartBase
	{
		public UISprite baseIcon;

		public UISprite icon;

		public UILabel baseCount;

		public UILabel count;

		public UILabel inputLabel;

		public UIInput input;

		public GameObject decreaseBtn;

		public GameObject addBtn;

		private int ticketMax;

		private int ticketMin;

		private int ticketCnt;

		private bool isPress;

		private string ticketIdx;

		private UIWeaponUpgradePopup parentPanel;

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			parentPanel = parent.GetComponent<UIWeaponUpgradePopup>();
		}

		public void Set(string basePartId, string partId)
		{
			ticketIdx = partId;
			RoPart roPart = base.localUser.FindPart(basePartId);
			PartDataRow partDataRow = base.regulation.partDtbl[partId];
			ticketMin = 1;
			ticketMax = roPart.count;
			ticketCnt = ticketMin;
			UISetter.SetSprite(baseIcon, roPart.GetPartData().serverFieldName);
			UISetter.SetSprite(icon, partDataRow.serverFieldName);
			UISetter.SetLabel(baseCount, roPart.count);
			UISetter.SetLabel(count, ticketCnt);
			SetValue();
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			base.OnClick(sender, parent);
			string name = sender.name;
			if (name == "TradeOkBtn")
			{
				base.network.RequestTradeWeaponUpgradeTicket(int.Parse(ticketIdx), ticketCnt);
			}
			UISetter.SetActive(root, active: false);
		}

		private void SetValue()
		{
			input.value = string.Empty;
			UISetter.SetLabel(inputLabel, ticketCnt);
			UISetter.SetLabel(baseCount, ticketMax - ticketCnt);
			UISetter.SetLabel(count, ticketCnt);
			UISetter.SetButtonEnable(decreaseBtn, ticketCnt > ticketMin);
			UISetter.SetButtonEnable(addBtn, ticketCnt < ticketMax);
		}

		public void DecreaseItemStart()
		{
			if (!isPress)
			{
				parentPanel.StartCoroutine(ItemCalculation(-1));
			}
		}

		public void DecreaseItemEnd()
		{
			if (isPress)
			{
				isPress = false;
				if (ItemCheck(-1))
				{
					ticketCnt--;
					SetValue();
				}
			}
		}

		public void AddItemStart()
		{
			if (!isPress)
			{
				parentPanel.StartCoroutine(ItemCalculation(1));
			}
		}

		public void AddItemEnd()
		{
			if (isPress)
			{
				isPress = false;
				if (ItemCheck(1))
				{
					ticketCnt++;
					SetValue();
				}
			}
		}

		public void ItemCountMax()
		{
			ticketCnt = ticketMax;
			SetValue();
		}

		private bool ItemCheck(int value)
		{
			if (value > 0)
			{
				if (ticketCnt < ticketMax)
				{
					return true;
				}
			}
			else if (ticketCnt > 1)
			{
				return true;
			}
			return false;
		}

		private IEnumerator ItemCalculation(int value)
		{
			float speed = 0.05f;
			isPress = true;
			yield return new WaitForSeconds(1f);
			while (ItemCheck(value) && isPress)
			{
				ticketCnt += value;
				SetValue();
				yield return new WaitForSeconds(speed);
			}
			yield return true;
		}

		public void SetInputValue()
		{
			if (string.IsNullOrEmpty(input.value) || int.Parse(input.value) < ticketMin)
			{
				ticketCnt = ticketMin;
			}
			else if (int.Parse(input.value) > ticketMax)
			{
				ticketCnt = ticketMax;
			}
			else
			{
				ticketCnt = int.Parse(input.value);
			}
			SetValue();
		}

		public void SetLimitLength(int limit)
		{
			input.characterLimit = limit;
		}
	}

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public UICommanderWeaponItem weaponItem;

	public UICommanderWeaponItem upgradeWeaponItem;

	public GameObject upgradeMaxLabel;

	public GameObject upgradeMaxBtnRoot;

	public GameObject upgradeBtnRoot;

	public List<UISprite> ticketIcon;

	public List<UILabel> ticketCount;

	public UISprite costIcon;

	public UILabel costTicket;

	public UILabel costGold;

	public UIWeaponTradeTicket tradeTicket;

	private UIWeaponUpgradeConfirmPopup upgradeConfirmPopup;

	private string weaponIdx = string.Empty;

	private int slotType;

	private int level;

	public readonly string[] upgradeTicket = new string[6] { "60001", "60002", "60003", "60004", "60005", "60006" };

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		OpenPopup();
		SendOnInitToInnerParts();
	}

	public void Set(string weaponIdx)
	{
		RoWeapon roWeapon = base.localUser.FindWeapon(weaponIdx);
		this.weaponIdx = weaponIdx;
		slotType = roWeapon.data.slotType;
		level = roWeapon.level;
		bool flag = isUpgradeMax();
		UISetter.SetActive(upgradeMaxBtnRoot, flag);
		UISetter.SetActive(upgradeMaxLabel, flag);
		UISetter.SetActive(upgradeBtnRoot, !flag);
		if (!flag)
		{
			RoWeapon weapon = RoWeapon.Create(string.Empty, roWeapon.data.idx, roWeapon.level + 1);
			upgradeWeaponItem.Set(weapon);
		}
		else
		{
			upgradeWeaponItem.Set(roWeapon);
			upgradeWeaponItem.WeaponEmpty(empty: true);
		}
		weaponItem.Set(roWeapon);
		SetTicketLabel();
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		Set(weaponIdx);
	}

	private void SetTicketLabel()
	{
		for (int i = 0; i < upgradeTicket.Length; i++)
		{
			PartDataRow partDataRow = base.regulation.partDtbl[upgradeTicket[i]];
			RoPart roPart = base.localUser.FindPart(upgradeTicket[i]);
			UISetter.SetSprite(ticketIcon[i], partDataRow.serverFieldName);
			UISetter.SetLabel(ticketCount[i], (roPart != null) ? roPart.count.ToString() : "0");
		}
		string key = $"{slotType}-{level + 1}";
		if (base.regulation.weaponUpgradeDtbl.ContainsKey(key))
		{
			WeaponUpgradeDataRow weaponUpgradeDataRow = base.regulation.weaponUpgradeDtbl[key];
			PartDataRow partDataRow2 = base.regulation.partDtbl[weaponUpgradeDataRow.pIdx];
			UISetter.SetSprite(costIcon, partDataRow2.serverFieldName);
			UISetter.SetLabel(costTicket, weaponUpgradeDataRow.value);
			UISetter.SetLabel(costGold, weaponUpgradeDataRow.gold);
		}
		else
		{
			key = $"{slotType}-{level}";
			WeaponUpgradeDataRow weaponUpgradeDataRow2 = base.regulation.weaponUpgradeDtbl[key];
			PartDataRow partDataRow3 = base.regulation.partDtbl[weaponUpgradeDataRow2.pIdx];
			UISetter.SetSprite(costIcon, partDataRow3.serverFieldName);
			UISetter.SetLabel(costTicket, 0);
			UISetter.SetLabel(costGold, 0);
		}
	}

	public override void OnClick(GameObject sender)
	{
		switch (sender.name)
		{
		case "Close":
			ClosePopup();
			break;
		case "UpgradeBtn":
			if (ResourceCheck())
			{
				CreateUpgradeConfirmPopup();
			}
			break;
		case "TradeTicket":
			OpenTradeTicket();
			break;
		default:
			SendOnClickToInnerParts(sender);
			break;
		}
	}

	private bool ResourceCheck()
	{
		string key = $"{slotType}-{level + 1}";
		int num = 0;
		WeaponUpgradeDataRow weaponUpgradeDataRow = base.regulation.weaponUpgradeDtbl[key];
		PartDataRow partDataRow = base.regulation.partDtbl[weaponUpgradeDataRow.pIdx];
		RoPart roPart = base.localUser.FindPart(weaponUpgradeDataRow.pIdx);
		if (roPart != null)
		{
			num = roPart.count;
		}
		if (num < weaponUpgradeDataRow.value)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70937"));
			return false;
		}
		if (base.localUser.gold < weaponUpgradeDataRow.gold)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70937"));
			return false;
		}
		return true;
	}

	private void CreateUpgradeConfirmPopup()
	{
		if (upgradeConfirmPopup != null)
		{
			return;
		}
		string key = $"{slotType}-{level + 1}";
		WeaponUpgradeDataRow weaponUpgradeDataRow = base.regulation.weaponUpgradeDtbl[key];
		PartDataRow partDataRow = base.regulation.partDtbl[weaponUpgradeDataRow.pIdx];
		upgradeConfirmPopup = UIPopup.Create<UIWeaponUpgradeConfirmPopup>("WeaponUpgradeConfirmPopup");
		upgradeConfirmPopup.Set(localization: true, "1303", "70019", string.Empty, "1304", "1305", string.Empty);
		upgradeConfirmPopup.SetCost(partDataRow.serverFieldName, weaponUpgradeDataRow.value);
		upgradeConfirmPopup.onClick = delegate(GameObject sender)
		{
			if (sender.name == "OK")
			{
				base.network.RequestUpgradeWeapon(int.Parse(weaponIdx));
			}
		};
	}

	private void OpenTradeTicket()
	{
		RoPart roPart = base.localUser.FindPart(upgradeTicket[0]);
		if (roPart.count == 0)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70938"));
			return;
		}
		UISetter.SetActive(tradeTicket.root, active: true);
		WeaponUpgradeDataRow weaponUpgradeDataRow = base.regulation.weaponUpgradeDtbl.Find((WeaponUpgradeDataRow row) => row.slotType == slotType);
		tradeTicket.Set(upgradeTicket[0], weaponUpgradeDataRow.pIdx);
	}

	private bool isUpgradeMax()
	{
		string key = $"{slotType}-{level + 1}";
		if (!base.regulation.weaponUpgradeDtbl.ContainsKey(key))
		{
			return true;
		}
		return false;
	}

	public void SetInputValue()
	{
		tradeTicket.SetInputValue();
	}

	public void DecreaseItemStart()
	{
		tradeTicket.DecreaseItemStart();
	}

	public void DecreaseItemEnd()
	{
		tradeTicket.DecreaseItemEnd();
	}

	public void AddItemStart()
	{
		tradeTicket.AddItemStart();
	}

	public void AddItemEnd()
	{
		tradeTicket.AddItemEnd();
	}

	public void ItemCountMax()
	{
		tradeTicket.ItemCountMax();
	}

	public void OpenPopup()
	{
		base.Open();
		AnimBG.Reset();
		AnimBlock.Reset();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
