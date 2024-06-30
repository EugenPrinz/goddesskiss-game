using System.Collections;
using System.Collections.Generic;
using RoomDecorator.Data;
using RoomDecorator.Event;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIInventory : MonoBehaviour
	{
		public GEAnimNGUI animBG;

		public UIFlipSwitch tabNormal;

		public UIFlipSwitch tabAdvanced;

		public UIFlipSwitch tabWallpaper;

		public UILabel itemLimit;

		public UILabel itemCount;

		public Color itemCountColor;

		public Color itemCountOverColor;

		public UIListViewBase itemListView;

		public UIInventorySelectObject selectObject;

		public UISellItem sellItem;

		private bool _isOpen;

		private bool _isClose = true;

		private EDormitoryItemType _curTab;

		private RoDormitory.Config _config;

		private RoDormitory.InventoryData _data;

		private void Awake()
		{
			_config = RemoteObjectManager.instance.localUser.dormitory.config;
			_data = RemoteObjectManager.instance.localUser.dormitory.invenData;
			_curTab = EDormitoryItemType.Normal;
			itemListView.itemIdPrefix = "Item-";
		}

		private void Start()
		{
			animBG.Reset();
			InvalidTab();
		}

		private void OnEnable()
		{
			Message.AddListener("Inven.Update", InvalidInventory);
		}

		private void OnDisable()
		{
			Message.RemoveListener("Inven.Update", InvalidInventory);
		}

		public void Open()
		{
			if (!_isOpen)
			{
				_isOpen = true;
				_isClose = false;
				SingletonMonoBehaviour<GridManager>.Instance.UndoSelectedFurniture();
				base.gameObject.SetActive(value: true);
				itemListView.ResetPosition();
				InvalidInventory();
				animBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
				SoundManager.PlaySFX("SE_MenuOpen_001");
			}
		}

		private void Close()
		{
			if (!_isClose)
			{
				_isClose = true;
				animBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
				StartCoroutine("WaitClose");
				SoundManager.PlaySFX("SE_MenuClose_001");
			}
		}

		private IEnumerator WaitClose()
		{
			yield return new WaitForSeconds(0.4f);
			base.gameObject.SetActive(value: false);
			_isOpen = false;
		}

		private void ChangeTab(EDormitoryItemType type)
		{
			if (_curTab != type)
			{
				itemListView.ResetPosition();
				_curTab = type;
				InvalidTab();
				InvalidInventory();
				SoundManager.PlaySFX("BTN_Tap_001");
			}
		}

		private void InvalidTab()
		{
			tabNormal.Set((_curTab == EDormitoryItemType.Normal) ? SwitchStatus.ON : SwitchStatus.OFF);
			tabAdvanced.Set((_curTab == EDormitoryItemType.Advanced) ? SwitchStatus.ON : SwitchStatus.OFF);
			tabWallpaper.Set((_curTab == EDormitoryItemType.Wallpaper) ? SwitchStatus.ON : SwitchStatus.OFF);
		}

		private void InvalidInventory()
		{
			int num = _data.itemNormal.Count + _data.itemAdvanced.Count + _data.itemWallpaper.Count;
			itemLimit.text = _config.inventoryLimit.ToString();
			itemCount.text = num.ToString();
			itemCount.color = ((num <= _config.inventoryLimit) ? itemCountColor : itemCountOverColor);
			Dictionary<string, RoDormitory.InvenSlot> dictionary = null;
			switch (_curTab)
			{
			case EDormitoryItemType.Normal:
				dictionary = _data.itemNormal;
				break;
			case EDormitoryItemType.Advanced:
				dictionary = _data.itemAdvanced;
				break;
			case EDormitoryItemType.Wallpaper:
				dictionary = _data.itemWallpaper;
				break;
			}
			if (dictionary != null)
			{
				itemListView.ResizeItemList(dictionary.Count);
				int num2 = 0;
				Dictionary<string, RoDormitory.InvenSlot>.Enumerator enumerator = dictionary.GetEnumerator();
				while (enumerator.MoveNext())
				{
					UIInventoryListItem uIInventoryListItem = itemListView.itemList[num2] as UIInventoryListItem;
					uIInventoryListItem.gameObject.name = itemListView.itemIdPrefix + enumerator.Current.Key;
					uIInventoryListItem.Set(enumerator.Current.Value);
					num2++;
				}
			}
		}

		public void OnClick(GameObject sender)
		{
			if (_isClose)
			{
				return;
			}
			string text = sender.name;
			switch (text)
			{
			case "Close":
				Close();
				return;
			case "NormalTab":
				ChangeTab(EDormitoryItemType.Normal);
				return;
			case "AdvancedTab":
				ChangeTab(EDormitoryItemType.Advanced);
				return;
			case "WallpaperTab":
				ChangeTab(EDormitoryItemType.Wallpaper);
				return;
			}
			if (!text.StartsWith(itemListView.itemIdPrefix))
			{
				return;
			}
			UIInventoryListItem component = sender.GetComponent<UIInventoryListItem>();
			RoDormitory.InvenSlot slot = component.data;
			string text2 = text.Substring(text.IndexOf("-") + 1);
			selectObject.Set(component.data.item);
			selectObject.Open();
			selectObject.onClick = delegate(GameObject obj)
			{
				switch (obj.name)
				{
				case "Place":
					Try_Place(slot);
					break;
				case "Sell":
					Try_Sell(slot);
					break;
				}
			};
		}

		public void Try_Place(RoDormitory.InvenSlot data)
		{
			switch (data.item.type)
			{
			case EDormitoryItemType.Normal:
			case EDormitoryItemType.Advanced:
				SingletonMonoBehaviour<GridManager>.Instance.CreateFurniture(data.item);
				Close();
				break;
			case EDormitoryItemType.Wallpaper:
				if (!(SingletonMonoBehaviour<DormitoryData>.Instance.room.wallpaper == data.item.id))
				{
					RemoteObjectManager.instance.RequestChangeDormitoryWallpaper(SingletonMonoBehaviour<DormitoryData>.Instance.room.fno, data.item.id);
				}
				break;
			}
		}

		public void Try_Sell(RoDormitory.InvenSlot data)
		{
			sellItem.Set(data);
			sellItem.Open();
		}
	}
}
