using System.Collections;
using System.Collections.Generic;
using RoomDecorator.Event;
using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIDormitoryShop : UIPopup
	{
		public GEAnimNGUI animBG;

		public GEAnimNGUI animBlock;

		public UIFlipSwitch tabNormal;

		public UIFlipSwitch tabAdvanced;

		public UIFlipSwitch tabWallpaper;

		public UIFlipSwitch tabThema;

		public UIFlipSwitch tabCostume;

		public UIListViewBase itemListView;

		public GameObject empty;

		public UIDormitoryPackageInfo packageInfo;

		private EDormitoryItemType _curTab;

		private Protocols.Dormitory.ShopInfo _data;

		protected override void Awake()
		{
			_curTab = EDormitoryItemType.Normal;
			base.Awake();
			SetRecyclable(recyclable: false);
		}

		private void Start()
		{
			Open();
		}

		protected override void OnEnable()
		{
			Message.AddListener<Protocols.Dormitory.BuyShopProductResponse>("Shop.Update", BuyShopProduct);
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			Message.RemoveListener<Protocols.Dormitory.BuyShopProductResponse>("Shop.Update", BuyShopProduct);
			base.OnDisable();
		}

		public override void Open()
		{
			base.Open();
			animBG.Reset();
			animBlock.Reset();
			animBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
			animBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
		}

		public override void Close()
		{
			animBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
			animBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
			StartCoroutine("WaitClose");
		}

		private IEnumerator WaitClose()
		{
			yield return new WaitForSeconds(0.4f);
			base.Close();
		}

		public void Set(Protocols.Dormitory.ShopInfo data)
		{
			_data = data;
			itemListView.ResetPosition();
			if (data.items != null)
			{
				Dictionary<EDormitoryItemType, List<Protocols.Dormitory.ShopProductItemInfo>>.Enumerator enumerator = data.items.GetEnumerator();
				while (enumerator.MoveNext())
				{
					_data.items[enumerator.Current.Key].Sort((Protocols.Dormitory.ShopProductItemInfo arg1, Protocols.Dormitory.ShopProductItemInfo arg2) => arg1.sort.CompareTo(arg2.sort));
				}
			}
			InvalidTab();
			InvalidItemData();
		}

		public void BuyShopProduct(Protocols.Dormitory.BuyShopProductResponse data)
		{
			if (_data == null)
			{
				return;
			}
			if (data.items != null)
			{
				Dictionary<EDormitoryItemType, Protocols.Dormitory.ShopProductItemInfo>.Enumerator itr = data.items.GetEnumerator();
				while (itr.MoveNext())
				{
					Protocols.Dormitory.ShopProductItemInfo shopProductItemInfo = _data.items[itr.Current.Key].Find((Protocols.Dormitory.ShopProductItemInfo x) => x.id == itr.Current.Value.id);
					if (shopProductItemInfo != null)
					{
						shopProductItemInfo.buyCount = itr.Current.Value.buyCount;
					}
				}
			}
			InvalidItemData();
		}

		private void ChangeTab(EDormitoryItemType type)
		{
			if (_curTab != type)
			{
				_curTab = type;
				itemListView.ResetPosition();
				InvalidTab();
				InvalidItemData();
				SoundManager.PlaySFX("BTN_Tap_001");
			}
		}

		private void InvalidTab()
		{
			tabNormal.Set((_curTab == EDormitoryItemType.Normal) ? SwitchStatus.ON : SwitchStatus.OFF);
			tabAdvanced.Set((_curTab == EDormitoryItemType.Advanced) ? SwitchStatus.ON : SwitchStatus.OFF);
			tabWallpaper.Set((_curTab == EDormitoryItemType.Wallpaper) ? SwitchStatus.ON : SwitchStatus.OFF);
			tabThema.Set((_curTab == EDormitoryItemType.Thema) ? SwitchStatus.ON : SwitchStatus.OFF);
			tabCostume.Set((_curTab == EDormitoryItemType.CostumeBody) ? SwitchStatus.ON : SwitchStatus.OFF);
		}

		private void InvalidItemData()
		{
			if (_data != null)
			{
				List<Protocols.Dormitory.ShopProductItemInfo> list = _data.items[_curTab];
				itemListView.ResizeItemList(list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					UIDormitoryShopListItem uIDormitoryShopListItem = itemListView.itemList[i] as UIDormitoryShopListItem;
					uIDormitoryShopListItem.gameObject.name = itemListView.itemIdPrefix + list[i].id;
					uIDormitoryShopListItem.Set(_curTab, list[i]);
				}
				empty.SetActive(list.Count == 0);
			}
		}

		public void OnMessage(GameObject arg0, GameObject arg1)
		{
			string text = arg0.name;
			string id = ((!(arg1 == null)) ? arg1.name : string.Empty);
			switch (text)
			{
			case "Buy":
			{
				UIDormitoryShopListItem component2 = arg1.GetComponent<UIDormitoryShopListItem>();
				if (base.localUser.resourceList[component2.data.goodsDr.serverFieldName] < component2.data.cost)
				{
					if (component2.data.goodsDr.serverFieldName == "cash")
					{
						UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject obj)
						{
							if (obj.name == "OK")
							{
								RemoteObjectManager.instance.RequestGetCashShopList();
							}
						};
					}
					else
					{
						NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81065"));
					}
					return;
				}
				if (component2.isLimited)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81061"));
					return;
				}
				UIBuyItem uIBuyItem = UIPopup.Create<UIBuyItem>("BuyItem");
				uIBuyItem.Set(Localization.Get("1303"), Localization.Format("70019", component2.item.itemName.text), component2.costIcon.spriteName, component2.data.cost.ToString(), Localization.Get("1304"), Localization.Get("1305"));
				uIBuyItem.onClick = delegate(GameObject obj)
				{
					if (obj.name == "Ok")
					{
						base.network.RequestBuyDormitoryShopProduct(_curTab, id);
					}
				};
				break;
			}
			case "ItemInfo":
			{
				UIDormitoryShopListItem component = arg1.GetComponent<UIDormitoryShopListItem>();
				string key = $"{component.type}_{component.data.id}";
				DormitoryShopDataRow dormitoryShopDataRow = base.regulation.dormitoryShopDtbl[key];
				if (dormitoryShopDataRow.type == EDormitoryItemType.Thema)
				{
					packageInfo.Set(base.regulation.dormitoryThemeMap[dormitoryShopDataRow.id]);
					packageInfo.Open();
					break;
				}
				ETooltipType eTooltipType = ETooltipType.Undefined;
				switch (dormitoryShopDataRow.type)
				{
				case EDormitoryItemType.Normal:
					eTooltipType = ETooltipType.Dormitory_NormalDeco;
					break;
				case EDormitoryItemType.Advanced:
					eTooltipType = ETooltipType.Dormitory_AdvancedDeco;
					break;
				case EDormitoryItemType.Wallpaper:
					eTooltipType = ETooltipType.Dormitory_Wallpaper;
					break;
				case EDormitoryItemType.CostumeBody:
					eTooltipType = ETooltipType.Dormitory_CostumeBody;
					break;
				}
				if (eTooltipType != 0)
				{
					UITooltip.Show(eTooltipType, dormitoryShopDataRow.id);
				}
				break;
			}
			}
			base.OnClick(arg0);
		}

		public override void OnClick(GameObject sender)
		{
			switch (sender.name)
			{
			case "NormalTab":
				ChangeTab(EDormitoryItemType.Normal);
				break;
			case "AdvancedTab":
				ChangeTab(EDormitoryItemType.Advanced);
				break;
			case "WallpaperTab":
				ChangeTab(EDormitoryItemType.Wallpaper);
				break;
			case "ThemaTab":
				ChangeTab(EDormitoryItemType.Thema);
				break;
			case "CostumeTab":
				ChangeTab(EDormitoryItemType.CostumeBody);
				break;
			case "Close":
				Close();
				break;
			}
			base.OnClick(sender);
		}
	}
}
