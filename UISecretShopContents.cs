using System;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UISecretShopContents : UIPanelBase
{
	private enum moveType
	{
		Previous,
		Next
	}

	[Serializable]
	public class UIBuyContents : UIInnerPartBase
	{
		public UIShopListItem item;

		public UILabel itemCount;

		public GameObject itemCountRoot;

		private UIPanelBase parentPanel;

		private bool isPress;

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			parentPanel = parent;
			UISetter.SetActive(root, active: false);
		}

		public void Set(Protocols.SecretShop.ShopData data)
		{
			UISetter.SetActive(root, active: true);
			item.Set(data);
			SetLabel(data.type, data.idx.ToString());
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			UISecretShopContents uISecretShopContents = parent as UISecretShopContents;
			string name = sender.name;
			if (name.StartsWith("gold_"))
			{
				int num = int.Parse(name.Substring("gold_".Length));
				Protocols.SecretShop.ShopData data = uISecretShopContents.currentList[num - 1];
				if (!base.localUser.BuyItemCheck(data))
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7065"));
					return;
				}
				if (uISecretShopContents.isBuyEnable(num))
				{
					SoundManager.PlaySFX("BTN_Buy_001");
					RemoteObjectManager.instance.RequestBuySecretShopItem(uISecretShopContents.shopType, num);
				}
				else
				{
					uISecretShopContents.NotEnough(MultiplePopUpType.NOTENOUGH_GOLD);
				}
			}
			else if (name.StartsWith("cash_"))
			{
				int num2 = int.Parse(name.Substring("cash_".Length));
				Protocols.SecretShop.ShopData data2 = uISecretShopContents.currentList[num2 - 1];
				if (!base.localUser.BuyItemCheck(data2))
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7065"));
					return;
				}
				if (uISecretShopContents.shopType == EShopType.VipShop)
				{
					if (base.localUser.shopRefreshTime.GetRemain() < 0.0)
					{
						UISimplePopup.CreateOK(localization: true, "22002", "22003", null, "1001").onClick = delegate(GameObject btn)
						{
							string name2 = btn.name;
							if (name2 == "OK")
							{
								if (UIManager.instance.world.existvipShop)
								{
									if (UIManager.instance.world.vipShop.isActive)
									{
										UIManager.instance.world.vipShop.Close();
									}
									else
									{
										UIManager.instance.world.secretShop.Close();
									}
								}
								else
								{
									UIManager.instance.world.secretShop.Close();
								}
							}
						};
					}
					else if (uISecretShopContents.isBuyEnable(num2))
					{
						SoundManager.PlaySFX("BTN_Buy_001");
						RemoteObjectManager.instance.RequestBuySecretShopItem(uISecretShopContents.shopType, num2);
					}
					else
					{
						uISecretShopContents.NotEnough(MultiplePopUpType.NOTENOUGH_CASH);
					}
				}
				else if (uISecretShopContents.isBuyEnable(num2))
				{
					SoundManager.PlaySFX("BTN_Buy_001");
					RemoteObjectManager.instance.RequestBuySecretShopItem(uISecretShopContents.shopType, num2);
				}
				else
				{
					uISecretShopContents.NotEnough(MultiplePopUpType.NOTENOUGH_CASH);
				}
			}
			else if (name.StartsWith("challengeCoin_"))
			{
				int num3 = int.Parse(name.Substring("challengeCoin_".Length));
				Protocols.SecretShop.ShopData data3 = uISecretShopContents.currentList[num3 - 1];
				if (!base.localUser.BuyItemCheck(data3))
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7065"));
					return;
				}
				if (uISecretShopContents.isBuyEnable(num3))
				{
					SoundManager.PlaySFX("BTN_Buy_001");
					RemoteObjectManager.instance.RequestBuySecretShopItem(uISecretShopContents.shopType, num3);
				}
				else
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("1994"), Localization.Get("4302")));
				}
			}
			else if (name.StartsWith("waveDuelCoin_"))
			{
				int num4 = int.Parse(name.Substring("waveDuelCoin_".Length));
				Protocols.SecretShop.ShopData data4 = uISecretShopContents.currentList[num4 - 1];
				if (!base.localUser.BuyItemCheck(data4))
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7065"));
					return;
				}
				if (uISecretShopContents.isBuyEnable(num4))
				{
					SoundManager.PlaySFX("BTN_Buy_001");
					RemoteObjectManager.instance.RequestBuySecretShopItem(uISecretShopContents.shopType, num4);
				}
				else
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("1994"), Localization.Get("5050002")));
				}
			}
			else if (name.StartsWith("worldDuelCoin_"))
			{
				int num5 = int.Parse(name.Substring("worldDuelCoin_".Length));
				Protocols.SecretShop.ShopData data5 = uISecretShopContents.currentList[num5 - 1];
				if (!base.localUser.BuyItemCheck(data5))
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7065"));
					return;
				}
				if (uISecretShopContents.isBuyEnable(num5))
				{
					SoundManager.PlaySFX("BTN_Buy_001");
					RemoteObjectManager.instance.RequestBuySecretShopItem(uISecretShopContents.shopType, num5);
				}
				else
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("1994"), Localization.Get("21013")));
				}
			}
			else if (name.StartsWith("raidCoin_"))
			{
				int num6 = int.Parse(name.Substring("raidCoin_".Length));
				Protocols.SecretShop.ShopData data6 = uISecretShopContents.currentList[num6 - 1];
				if (!base.localUser.BuyItemCheck(data6))
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7065"));
					return;
				}
				if (uISecretShopContents.isBuyEnable(num6))
				{
					SoundManager.PlaySFX("BTN_Buy_001");
					RemoteObjectManager.instance.RequestBuySecretShopItem(uISecretShopContents.shopType, num6);
				}
				else
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("1994"), Localization.Get("4303")));
				}
			}
			else if (name.StartsWith("guildCoin_"))
			{
				int num7 = int.Parse(name.Substring("guildCoin_".Length));
				Protocols.SecretShop.ShopData data7 = uISecretShopContents.currentList[num7 - 1];
				if (!base.localUser.BuyItemCheck(data7))
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7065"));
					return;
				}
				if (uISecretShopContents.isBuyEnable(num7))
				{
					SoundManager.PlaySFX("BTN_Buy_001");
					RemoteObjectManager.instance.RequestBuySecretShopItem(uISecretShopContents.shopType, num7);
				}
				else
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("1994"), Localization.Get("4304")));
				}
			}
			else if (name.StartsWith("annihilationCoin_"))
			{
				int num8 = int.Parse(name.Substring("annihilationCoin_".Length));
				Protocols.SecretShop.ShopData data8 = uISecretShopContents.currentList[num8 - 1];
				if (!base.localUser.BuyItemCheck(data8))
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7065"));
					return;
				}
				if (uISecretShopContents.isBuyEnable(num8))
				{
					SoundManager.PlaySFX("BTN_Buy_001");
					RemoteObjectManager.instance.RequestBuySecretShopItem(uISecretShopContents.shopType, num8);
				}
				else
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("1994"), Localization.Get("4333")));
				}
			}
			UISetter.SetActive(root, active: false);
		}

		private void SetLabel(ERewardType type, string idx)
		{
			RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
			Regulation regulation = RemoteObjectManager.instance.regulation;
			switch (type)
			{
			case ERewardType.Goods:
			{
				GoodsDataRow goodsDataRow = regulation.goodsDtbl[idx];
				UISetter.SetLabel(itemCount, $"{roLocalUser.resourceList[goodsDataRow.serverFieldName]} / {goodsDataRow.max}");
				break;
			}
			case ERewardType.UnitMaterial:
			{
				PartDataRow partDataRow = regulation.FindPartData(idx);
				RoPart roPart = roLocalUser.FindPart(idx);
				UISetter.SetLabel(itemCount, $"{roPart.count} / {partDataRow.max}");
				break;
			}
			case ERewardType.Medal:
			case ERewardType.Commander:
			{
				RoCommander roCommander = roLocalUser.FindCommander(idx);
				UISetter.SetLabel(itemCount, roCommander.medal);
				break;
			}
			}
		}
	}

	[SerializeField]
	private UILabel NextRefreshTime_txt;

	public UILabel title;

	public UIDefaultListView itemListView;

	public UILabel refreshTime;

	public UITimer timer;

	public UILabel refreshCount;

	public UISprite refreshIcon;

	public GameObject resourceRoot;

	public UILabel resourceCount;

	public UISprite resourceIcon;

	public GameObject arrowRoot;

	public UIBuyContents buyContents;

	public GameObject refreshFree;

	public GameObject refreshPay;

	private List<Protocols.SecretShop.ShopData> currentList;

	[HideInInspector]
	public EShopType shopType;

	private bool popup;

	private const EShopType minShopType = EShopType.BasicShop;

	private const EShopType maxShopType = EShopType.WorldDuelShop;

	[SerializeField]
	private GameObject RefreshBtn;

	[SerializeField]
	private GameObject VipShopTimeObj;

	[SerializeField]
	private UITimer VipShopTimer;

	private TimeData shopTime;

	public void Init(List<Protocols.SecretShop.ShopData> _list, EShopType _type)
	{
		UISetter.SetActive(refreshTime.gameObject, active: false);
		shopTime = TimeData.Create();
		currentList = _list;
		itemListView.Init(_list, "Item_");
		UISetter.SetActive(title, active: true);
		Regulation regulation = RemoteObjectManager.instance.regulation;
		RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
		string text = string.Empty;
		int hour = DateTime.Now.Hour;
		int index = 0;
		ShopDataRow shopDataRow = regulation.shopDtbl.Find((ShopDataRow row) => row.type == shopType);
		for (int i = 0; i < shopDataRow.openTime.Count; i++)
		{
			if (shopDataRow.openTime[i].Contains(":"))
			{
				int num = int.Parse(shopDataRow.openTime[i].Substring(0, shopDataRow.openTime[i].IndexOf(":")));
				if (hour < num)
				{
					index = i;
					break;
				}
			}
		}
		UISetter.SetActive(NextRefreshTime_txt.gameObject, active: true);
		UISetter.SetActive(VipShopTimeObj, active: false);
		UISetter.SetActive(resourceRoot, shopType != EShopType.BasicShop);
		UISetter.SetLabel(refreshTime, shopDataRow.openTime[index]);
		UISetter.SetLabel(NextRefreshTime_txt, Localization.Get("1991"));
		UISetter.SetActive(timer, active: true);
		UISetter.SetTimer(timer, roLocalUser.shopRefreshTime);
		UISetter.SetActive(RefreshBtn, active: true);
		int num2 = 0;
		GoodsDataRow goodsDataRow = null;
		RoBuilding roBuilding = null;
		switch (_type)
		{
		case EShopType.BasicShop:
			text = Localization.Get(roLocalUser.FindBuilding(EBuilding.BlackMarket).reg.locNameKey);
			break;
		case EShopType.ChallengeShop:
			goodsDataRow = regulation.goodsDtbl[13.ToString()];
			text = Localization.Get(roLocalUser.FindBuilding(EBuilding.Challenge).reg.locNameKey);
			UISetter.SetSprite(resourceIcon, "Goods-acon");
			UISetter.SetLabel(resourceCount, $"{roLocalUser.challengeCoin} / {goodsDataRow.max}");
			break;
		case EShopType.WaveDuelShop:
			goodsDataRow = regulation.goodsDtbl[58.ToString()];
			text = Localization.Get("5040008");
			UISetter.SetSprite(resourceIcon, "Goods-wbc");
			UISetter.SetLabel(resourceCount, $"{roLocalUser.waveDuelCoin} / {goodsDataRow.max}");
			break;
		case EShopType.GuildShop:
			goodsDataRow = regulation.goodsDtbl[15.ToString()];
			text = Localization.Get(roLocalUser.FindBuilding(EBuilding.Guild).reg.locNameKey);
			UISetter.SetSprite(resourceIcon, "Goods-gcon");
			UISetter.SetLabel(resourceCount, $"{roLocalUser.guildCoin} / {goodsDataRow.max}");
			break;
		case EShopType.RaidShop:
			goodsDataRow = regulation.goodsDtbl[14.ToString()];
			text = Localization.Get(roLocalUser.FindBuilding(EBuilding.Raid).reg.locNameKey);
			UISetter.SetSprite(resourceIcon, "Goods-rcon");
			UISetter.SetLabel(resourceCount, $"{roLocalUser.raidCoin} / {goodsDataRow.max}");
			break;
		case EShopType.VipShop:
			roBuilding = roLocalUser.FindBuilding(EBuilding.VipShop);
			text = Localization.Get(roBuilding.reg.locNameKey);
			if (!roLocalUser.statistics.isBuyVipShop)
			{
				UISetter.SetTimer(VipShopTimer, roLocalUser.shopRefreshTime, "22001");
			}
			else
			{
				UISetter.SetLabel(refreshTime, shopDataRow.openTime[index]);
			}
			SetGameObjInVipShop(roLocalUser.statistics.isBuyVipShop);
			UISetter.SetActive(resourceRoot, active: false);
			break;
		case EShopType.AnnihilationShop:
			goodsDataRow = regulation.goodsDtbl[20.ToString()];
			text = Localization.Get(roLocalUser.FindBuilding(EBuilding.Loot).reg.locNameKey);
			UISetter.SetSprite(resourceIcon, "Goods-ncon");
			UISetter.SetLabel(resourceCount, $"{roLocalUser.annCoin} / {goodsDataRow.max}");
			break;
		case EShopType.WorldDuelShop:
			goodsDataRow = regulation.goodsDtbl[8002.ToString()];
			text = Localization.Get(roLocalUser.FindBuilding(EBuilding.WorldChallenge).reg.locNameKey);
			UISetter.SetSprite(resourceIcon, "ps-server-silver");
			UISetter.SetLabel(resourceCount, $"{roLocalUser.worldDuelCoin} / {goodsDataRow.max}");
			break;
		}
		if (_type == EShopType.BasicShop || _type == EShopType.VipShop)
		{
			UISetter.SetLabel(title, Localization.Get(text));
		}
		else
		{
			UISetter.SetLabel(title, Localization.Format("1990", text));
		}
		GoodsDataRow goodsDataRow2 = regulation.goodsDtbl[shopDataRow.g_idx];
		int num3 = int.Parse(regulation.defineDtbl["VIPGRADE_BATTLESHOP_REFRESH"].value);
		int num4 = (int)((float)shopDataRow.startRechargePrice * Mathf.Pow(float.Parse(shopDataRow.priceAddPercent) / 100f, Mathf.Floor((float)roLocalUser.shopRefreshCount / (float)shopDataRow.numberMeasure)));
		UISetter.SetSprite(refreshIcon, goodsDataRow2.iconId);
		UISetter.SetLabel(refreshCount, num4);
		UISetter.SetActive(refreshFree, roLocalUser.shopRefreshFree && roLocalUser.vipLevel >= num3 && shopType != EShopType.BasicShop && shopType != EShopType.VipShop);
		UISetter.SetActive(refreshPay, !refreshFree.activeSelf);
		if (shopType == EShopType.VipShop && !roLocalUser.statistics.isBuyVipShop)
		{
			VipShopTimer.SetFinishString(Localization.Get("22002"));
		}
		else
		{
			timer.RegisterOnFinished(delegate
			{
				RemoteObjectManager.instance.RequestGetSecretShopList(shopType);
			});
		}
		UISetter.SetActive(arrowRoot, ArrowEnable());
		SendOnInitToInnerParts();
	}

	public override void OnRefresh()
	{
		Init(RemoteObjectManager.instance.localUser.shopList, shopType);
	}

	public void SetType(EShopType _type, bool _popup)
	{
		popup = _popup;
		shopType = _type;
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text.StartsWith("Item_"))
		{
			int index = int.Parse(text.Substring("Item_".Length));
			buyContents.Set(currentList[index]);
			return;
		}
		switch (text)
		{
		case "PreviousShop":
			SoundManager.PlaySFX("BTN_NextShop_001");
			GetSecretShopList(moveType.Previous);
			return;
		case "NextShop":
			SoundManager.PlaySFX("BTN_NextShop_001");
			GetSecretShopList(moveType.Next);
			return;
		case "RefreshBtn":
			RefreshShop();
			return;
		case "BuyCancleBtn":
			SoundManager.PlaySFX("BTN_Negative_001");
			break;
		}
		SendOnClickToInnerParts(sender);
	}

	private void GetSecretShopList(moveType mType)
	{
		if (mType == moveType.Previous)
		{
			if (shopType == EShopType.BasicShop)
			{
				shopType = EShopType.WorldDuelShop;
			}
			else
			{
				int num = (int)shopType;
				num--;
				shopType = (EShopType)num;
			}
		}
		else if (shopType == EShopType.WorldDuelShop)
		{
			shopType = EShopType.BasicShop;
		}
		else
		{
			int num2 = (int)shopType;
			num2++;
			shopType = (EShopType)num2;
		}
		if (!BuildingEnable(shopType) || shopType == EShopType.GuildShop)
		{
			GetSecretShopList(mType);
			return;
		}
		itemListView.scrollView.ResetPosition();
		RemoteObjectManager.instance.RequestGetSecretShopList(shopType);
	}

	private bool BuildingEnable(EShopType _type)
	{
		bool flag = false;
		RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
		RoBuilding roBuilding = null;
		switch (_type)
		{
		case EShopType.BasicShop:
			roBuilding = roLocalUser.FindBuilding(EBuilding.BlackMarket);
			break;
		case EShopType.ChallengeShop:
			roBuilding = roLocalUser.FindBuilding(EBuilding.Challenge);
			break;
		case EShopType.GuildShop:
			roBuilding = roLocalUser.FindBuilding(EBuilding.Guild);
			break;
		case EShopType.RaidShop:
			roBuilding = roLocalUser.FindBuilding(EBuilding.Raid);
			break;
		case EShopType.AnnihilationShop:
			roBuilding = roLocalUser.FindBuilding(EBuilding.Loot);
			break;
		case EShopType.WorldDuelShop:
			roBuilding = roLocalUser.FindBuilding(EBuilding.WorldChallenge);
			break;
		case EShopType.VipShop:
			return roLocalUser.statistics.vipShop == 1;
		case EShopType.WaveDuelShop:
			if (roLocalUser.level < int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["ARENA_3WAVE_OPEN_LEVEL"].value))
			{
				return false;
			}
			roBuilding = roLocalUser.FindBuilding(EBuilding.Challenge);
			break;
		}
		return roLocalUser.level >= roBuilding.firstLevelReg.userLevel;
	}

	private bool ArrowEnable()
	{
		if (popup)
		{
			return false;
		}
		RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
		RoBuilding roBuilding = roLocalUser.FindBuilding(EBuilding.Challenge);
		if (roLocalUser.level >= roBuilding.firstLevelReg.userLevel)
		{
			return true;
		}
		return false;
	}

	public bool isBuyEnable(int id)
	{
		RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
		for (int i = 0; i < currentList.Count; i++)
		{
			Protocols.SecretShop.ShopData shopData = currentList[i];
			if (shopData.id != id)
			{
				continue;
			}
			if (shopData.costType == EPriceType.Gold)
			{
				if (roLocalUser.gold < shopData.cost)
				{
					return false;
				}
			}
			else if (shopData.costType == EPriceType.Cash)
			{
				if (roLocalUser.cash < shopData.cost)
				{
					return false;
				}
			}
			else if (shopData.costType == EPriceType.ChallengeCoin)
			{
				if (roLocalUser.challengeCoin < shopData.cost)
				{
					return false;
				}
			}
			else if (shopData.costType == EPriceType.WaveDuelCoin)
			{
				if (roLocalUser.waveDuelCoin < shopData.cost)
				{
					return false;
				}
			}
			else if (shopData.costType == EPriceType.RaidCoin)
			{
				if (roLocalUser.raidCoin < shopData.cost)
				{
					return false;
				}
			}
			else if (shopData.costType == EPriceType.GuildCoin)
			{
				if (roLocalUser.guildCoin < shopData.cost)
				{
					return false;
				}
			}
			else if (shopData.costType == EPriceType.AnnihilationCoin)
			{
				if (roLocalUser.annCoin < shopData.cost)
				{
					return false;
				}
			}
			else if (shopData.costType == EPriceType.WorldDuelCoin && roLocalUser.worldDuelCoin < shopData.cost)
			{
				return false;
			}
		}
		return true;
	}

	private void RefreshShop()
	{
		RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
		int num = int.Parse(refreshCount.text);
		int num2 = 0;
		if (shopType == EShopType.BasicShop || shopType == EShopType.VipShop)
		{
			num2 = roLocalUser.cash;
		}
		else if (shopType == EShopType.ChallengeShop)
		{
			num2 = roLocalUser.challengeCoin;
		}
		else if (shopType == EShopType.GuildShop)
		{
			num2 = roLocalUser.guildCoin;
		}
		else if (shopType == EShopType.RaidShop)
		{
			num2 = roLocalUser.raidCoin;
		}
		else if (shopType == EShopType.AnnihilationShop)
		{
			num2 = roLocalUser.annCoin;
		}
		else if (shopType == EShopType.WaveDuelShop)
		{
			num2 = roLocalUser.waveDuelCoin;
		}
		else if (shopType == EShopType.WorldDuelShop)
		{
			num2 = roLocalUser.worldDuelCoin;
		}
		if (num > num2 && !refreshFree.activeSelf)
		{
			if (shopType == EShopType.BasicShop || shopType == EShopType.VipShop)
			{
				NotEnough(MultiplePopUpType.NOTENOUGH_CASH);
			}
			else if (shopType == EShopType.ChallengeShop)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("1994"), Localization.Get("4302")));
			}
			else if (shopType == EShopType.GuildShop)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("1994"), Localization.Get("4304")));
			}
			else if (shopType == EShopType.RaidShop)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("1994"), Localization.Get("4303")));
			}
			else if (shopType == EShopType.AnnihilationShop)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("1994"), Localization.Get("4333")));
			}
			else if (shopType == EShopType.WaveDuelShop)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("1994"), Localization.Get("5050002")));
			}
			else if (shopType == EShopType.WorldDuelShop)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("1994"), Localization.Get("21013")));
			}
		}
		else
		{
			SoundManager.PlaySFX("BTN_Refresh_001");
			itemListView.scrollView.ResetPosition();
			RemoteObjectManager.instance.RequestRefreshSecretShopList(shopType);
		}
	}

	public void NotEnough(MultiplePopUpType type)
	{
		switch (type)
		{
		case MultiplePopUpType.NOTENOUGH_CASH:
			UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject sender)
			{
				string text = sender.name;
				if (text == "OK")
				{
					UIManager.instance.world.mainCommand.OpenDiamonShop();
				}
			};
			break;
		case MultiplePopUpType.NOTENOUGH_GOLD:
			UISimplePopup.CreateBool(localization: true, "1029", "1030", null, "1001", "1000").onClick = delegate(GameObject sender)
			{
				string text2 = sender.name;
				if (text2 == "OK")
				{
					UIManager.instance.world.camp.GoNavigation("MetroBank");
				}
			};
			break;
		}
	}

	public void CloseBuyContents()
	{
		UISetter.SetActive(buyContents, active: false);
	}

	private void VipShopEndPopup()
	{
		UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: true, "22002", "22003", null, "1001");
	}

	private void SetGameObjInVipShop(bool isOn)
	{
		UISetter.SetActive(RefreshBtn, isOn);
		UISetter.SetActive(NextRefreshTime_txt.gameObject, isOn);
		UISetter.SetActive(VipShopTimeObj, !isOn);
		UISetter.SetActive(timer, isOn);
		if (!isOn)
		{
			UISetter.SetTimer(VipShopTimer, base.localUser.shopRefreshTime, "22001");
			return;
		}
		UISetter.SetLabel(NextRefreshTime_txt, Localization.Get("1991"));
		UISetter.SetTimer(timer, base.localUser.shopRefreshTime);
	}
}
