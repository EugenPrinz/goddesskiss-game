using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIDiamondShop : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UILabel vipLevel;

	public UIProgressBar vipExpProgress;

	public UILabel vipExpValue;

	public UILabel vipLevelUpCondition;

	public UIDefaultListView cashListView;

	[SerializeField]
	private UIDefaultListView cashListView_2;

	public GameObject CashAndGoldPanel;

	public UISprite block;

	private bool goVipInfo;

	private UIPurchasePopup PerchasePopup;

	[SerializeField]
	private List<UIFlipSwitch> ShopTabs;

	private int CurrTabIdx;

	public int initTabIdx = 1;

	[SerializeField]
	private UILabel warningLabel;

	private List<string> tabList_2 = new List<string>();

	private Dictionary<int, string> tabList = new Dictionary<int, string>();

	public void initData()
	{
		goVipInfo = false;
		SetUserData();
		base.regulation.cashShopDtbl.Sort(delegate(Protocols.CashShopData row, Protocols.CashShopData row1)
		{
			InAppProductDataRow inAppProduct = RemoteObjectManager.instance.regulation.GetInAppProduct(row.priceId);
			InAppProductDataRow inAppProduct2 = RemoteObjectManager.instance.regulation.GetInAppProduct(row1.priceId);
			return inAppProduct.sort.CompareTo(inAppProduct2.sort);
		});
		if (CurrTabIdx != 0)
		{
			ShopTabs[CurrTabIdx - 1].On();
		}
		SetTabInfo(CurrTabIdx != 2, CurrTabIdx);
	}

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
	}

	public void OnVipBtn()
	{
		goVipInfo = true;
		ClosePopup();
	}

	public void SetUserData()
	{
		int num = base.localUser.vipLevel;
		UISetter.SetLabel(vipLevel, num);
		if (base.regulation.vipExpDtbl.ContainsKey((num + 1).ToString()))
		{
			UISetter.SetActive(message, active: true);
			int exp = base.regulation.vipExpDtbl[num.ToString()].exp;
			UISetter.SetLabel(vipExpValue, $"{base.localUser.vipExp} / {exp}");
			UISetter.SetProgress(vipExpProgress, (float)base.localUser.vipExp / (float)exp);
			UISetter.SetLabel(message, Localization.Format("7080", exp - base.localUser.vipExp, num + 1));
		}
		else
		{
			UISetter.SetActive(message, active: false);
			UISetter.SetLabel(vipExpValue, base.localUser.vipExp);
			UISetter.SetProgress(vipExpProgress, 1f);
		}
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text.StartsWith("PID_"))
		{
			string pid = text.Replace("PID_", string.Empty);
			if (pid == "gk.package.monthly" || pid == "gk.package.monthly02" || pid == "gk.package.monthly03")
			{
				if (TimeSpan.FromSeconds(base.regulation.cashShopDtbl.Find((Protocols.CashShopData row) => row.priceId == pid).remainTime).Days >= 30)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7155"));
					return;
				}
				RemoteObjectManager.instance.RequestMakeOrderId(pid);
			}
			else
			{
				RemoteObjectManager.instance.RequestMakeOrderId(pid);
			}
		}
		else
		{
			if (text.StartsWith("RewardList-"))
			{
				string id = text.Substring(text.IndexOf("-") + 1);
				List<RewardDataRow> list = base.regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == ERewardCategory.CashShopPackage && row.type == int.Parse(id));
				if (list != null)
				{
					UIPopup.Create<UIRewardItemPreViewPopUp>("RewardItemPreViewPopUp").InitAndOpenItemList(list, Localization.Get("7182"));
				}
				return;
			}
			if (text.StartsWith("Tab-"))
			{
				string s = text.Substring(text.IndexOf("-") + 1);
				int num = int.Parse(s);
				SetTabInfo(num != 2, num);
				SoundManager.PlaySFX("BTN_Tap_001");
				return;
			}
		}
		base.OnClick(sender);
	}

	public void OnDisableButtonClick(GameObject sender)
	{
		string text = sender.name;
		string idx = text.Replace("PID_", string.Empty);
		InAppProductDataRow inAppProduct = RemoteObjectManager.instance.regulation.GetInAppProduct(idx);
		if (inAppProduct != null)
		{
			if (inAppProduct.availableLevel > base.localUser.level)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Format("10000210", inAppProduct.availableLevel));
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("10000207"));
			}
		}
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
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			AnimBG.Reset();
			AnimBlock.Reset();
			OpenPopup();
			_InitAndOpen(gold: false, cash: true, resource: false);
		}
	}

	public void InitAndOpenResourceShop()
	{
		_InitAndOpen(gold: false, cash: false, resource: true);
	}

	private void _InitAndOpen(bool gold, bool cash, bool resource)
	{
		if (gold == cash)
		{
			gold = true;
			cash = false;
		}
		CurrTabIdx = initTabIdx;
		initData();
		TabSetting();
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		initData();
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		if (!bBackKeyEnable)
		{
			bBackKeyEnable = true;
			HidePopup();
		}
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.4f);
		if (goVipInfo)
		{
			if (base.uiWorld != null)
			{
				base.uiWorld.mainCommand.OpenVipInfo(isShop: true);
			}
			else
			{
				UIPopup.Create<UIVipInfo>("vipInfo").Init(isShop: true);
			}
		}
		yield return new WaitForSeconds(0.4f);
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
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

	public override void Open()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Close()
	{
		HidePopup();
	}

	private void TabSetting()
	{
		for (int i = 0; i < base.regulation.cashShopDtbl.Count; i++)
		{
			InAppProductDataRow inAppProduct = RemoteObjectManager.instance.regulation.GetInAppProduct(base.regulation.cashShopDtbl[i].priceId);
			if (!tabList.ContainsKey(inAppProduct.isSell))
			{
				string value = string.Empty;
				switch (inAppProduct.isSell)
				{
				case 1:
					value = Localization.Get("1098");
					break;
				case 2:
					value = Localization.Get("1099");
					break;
				case 3:
					value = Localization.Get("1100");
					break;
				case 4:
					value = Localization.Get("1101");
					break;
				case 5:
					value = Localization.Get("1102");
					break;
				case 6:
					value = Localization.Get("1103");
					break;
				}
				tabList.Add(inAppProduct.isSell, value);
			}
		}
		for (int j = 0; j < ShopTabs.Count; j++)
		{
			UISetter.SetGameObjectName(ShopTabs[j].gameObject, "Tab-");
			UISetter.SetActive(ShopTabs[j], active: false);
		}
		List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>(tabList);
		list.Sort((KeyValuePair<int, string> first, KeyValuePair<int, string> next) => first.Key.CompareTo(next.Key));
		for (int k = 0; k < list.Count; k++)
		{
			UISetter.SetActive(ShopTabs[k], active: true);
			ShopTabs[k].GetComponent<DiamondShopTab>().SetTabName(list[k].Value);
			UISetter.SetGameObjectName(ShopTabs[k].gameObject, "Tab-" + (k + 1));
		}
	}

	private List<Protocols.CashShopData> SetCashShopList(int tabIdx)
	{
		List<Protocols.CashShopData> list = new List<Protocols.CashShopData>();
		for (int i = 0; i < base.regulation.cashShopDtbl.Count; i++)
		{
			InAppProductDataRow inAppProduct = RemoteObjectManager.instance.regulation.GetInAppProduct(base.regulation.cashShopDtbl[i].priceId);
			if (inAppProduct != null && inAppProduct.isSell == tabIdx)
			{
				list.Add(base.regulation.cashShopDtbl[i]);
			}
		}
		return list;
	}

	private void SetTabInfo(bool isActive, int tabIdx)
	{
		InAppProductDataRow inAppProductDataRow = RemoteObjectManager.instance.regulation.inAppProductDtbl.Find((InAppProductDataRow row) => row.isSell == tabIdx);
		if (inAppProductDataRow != null)
		{
			bool flag = inAppProductDataRow.uiType == 2;
			UISetter.SetActive(cashListView, !flag);
			UISetter.SetActive(cashListView_2, flag);
			if (flag)
			{
				cashListView_2.InitCashShop(SetCashShopList(tabIdx), "PID_");
				cashListView_2.ResetPosition();
			}
			else
			{
				cashListView.InitCashShop(SetCashShopList(tabIdx), "PID_");
				cashListView.ResetPosition();
			}
			switch (tabIdx)
			{
			case 1:
				UISetter.SetLabel(warningLabel, Localization.Get("1097"));
				break;
			case 2:
				UISetter.SetLabel(warningLabel, Localization.Get("1104"));
				break;
			case 3:
				UISetter.SetLabel(warningLabel, Localization.Get("1105"));
				break;
			case 4:
				UISetter.SetLabel(warningLabel, Localization.Get("1106"));
				break;
			case 5:
				UISetter.SetLabel(warningLabel, Localization.Get("1107"));
				break;
			case 6:
				UISetter.SetLabel(warningLabel, Localization.Get("1108"));
				break;
			}
		}
	}
}
