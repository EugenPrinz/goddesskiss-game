using System.Collections;
using RoomDecorator.Data;
using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UISellItem : UIPopup
	{
		public GEAnimNGUI animBG;

		public GEAnimNGUI animBlock;

		public UILabel itemName;

		public UISprite itemIcon;

		public UILabel itemTileSize;

		public UISprite rewardIcon;

		public UILabel rewardAmount;

		public UIInput input;

		private bool _isOpen;

		private bool _isClose = true;

		private string _id;

		private EStorageType _storageType;

		private int _itemAmount;

		private int _sellAmount;

		private int _rewardAmount;

		private bool _isPress;

		public override void Open()
		{
			if (!_isOpen)
			{
				_isOpen = true;
				_isClose = false;
				base.Open();
				animBG.Reset();
				animBlock.Reset();
				animBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
				animBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
			}
		}

		public override void Close()
		{
			if (!_isClose)
			{
				_isClose = true;
				animBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
				animBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
				StartCoroutine("WaitClose");
			}
		}

		private IEnumerator WaitClose()
		{
			yield return new WaitForSeconds(0.4f);
			_isOpen = false;
			base.Close();
		}

		public void Set(RoDormitory.InvenSlot data)
		{
			_id = data.item.id;
			_itemAmount = data.amount;
			_sellAmount = ((_itemAmount > 0) ? 1 : 0);
			switch (data.item.type)
			{
			case EDormitoryItemType.Normal:
			case EDormitoryItemType.Advanced:
				Set((DormitoryDecorationDataRow)data.item.data);
				break;
			case EDormitoryItemType.Wallpaper:
				Set((DormitoryWallpaperDataRow)data.item.data);
				break;
			case EDormitoryItemType.CostumeBody:
				Set((DormitoryBodyCostumeDataRow)data.item.data);
				break;
			default:
				_storageType = EStorageType.Undefined;
				break;
			}
			InvalidInput();
		}

		private void Set(DormitoryDecorationDataRow data)
		{
			_storageType = EStorageType.DormitoryFurniture;
			itemName.text = Localization.Get(data.name);
			itemIcon.SetAtlasImage("DormitoryTheme_" + data.atlasNumber, data.thumbnail);
			itemTileSize.gameObject.SetActive(value: true);
			itemTileSize.text = $"{data.xSize}x{data.ySize}";
			Set(data.itemExchangeDr);
		}

		private void Set(DormitoryWallpaperDataRow data)
		{
			_storageType = EStorageType.DormitoryWallpaper;
			itemName.text = Localization.Get(data.name);
			itemIcon.SetAtlasImage("DormitoryTheme_" + data.atlasNumber, data.thumbnail);
			itemTileSize.gameObject.SetActive(value: false);
			Set(data.itemExchangeDr);
		}

		private void Set(DormitoryBodyCostumeDataRow data)
		{
			_storageType = EStorageType.DormitoryCostume;
			itemName.text = Localization.Get(data.name);
			itemIcon.SetAtlasImage("DormitoryCostume_" + data.atlasNumber, data.thumbnail);
			itemTileSize.gameObject.SetActive(value: false);
			Set(data.itemExchangeDr);
		}

		private void Set(ItemExchangeDataRow data)
		{
			GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.goodsDtbl[data.pricetypeidx];
			rewardIcon.spriteName = goodsDataRow.iconId;
			_rewardAmount = data.price;
		}

		private void InvalidInput()
		{
			if (_sellAmount > _itemAmount)
			{
				_sellAmount = _itemAmount;
			}
			if (_sellAmount < 0)
			{
				_sellAmount = 0;
			}
			input.value = _sellAmount.ToString();
			rewardAmount.text = (_sellAmount * _rewardAmount).ToString();
		}

		public void OnInputSummit()
		{
			_sellAmount = int.Parse(input.value);
			InvalidInput();
		}

		public void OnPress(GameObject sender)
		{
			if (_isPress)
			{
				return;
			}
			_isPress = true;
			StopAllCoroutines();
			switch (sender.name)
			{
			case "AddcreaseBtn":
				_sellAmount++;
				InvalidInput();
				if (_sellAmount < _itemAmount)
				{
					StartCoroutine(ItemCalculation(1));
				}
				break;
			case "DecreaseBtn":
				_sellAmount--;
				InvalidInput();
				if (_sellAmount > 0)
				{
					StartCoroutine(ItemCalculation(-1));
				}
				break;
			}
		}

		public void OnRelease(GameObject sender)
		{
			_isPress = false;
		}

		private IEnumerator ItemCalculation(int value)
		{
			float speed = 0.05f;
			yield return new WaitForSeconds(1f);
			while (_isPress)
			{
				_sellAmount += value;
				InvalidInput();
				if (_sellAmount == 0 || _sellAmount >= _itemAmount)
				{
					break;
				}
				yield return new WaitForSeconds(speed);
			}
			yield return null;
		}

		public new void OnClick(GameObject sender)
		{
			if (_isClose)
			{
				return;
			}
			base.OnClick(sender);
			switch (sender.name)
			{
			case "Max":
				_sellAmount = _itemAmount;
				InvalidInput();
				break;
			case "Sell":
				if (_sellAmount > 0)
				{
					RemoteObjectManager.instance.RequestSellDormitoryItem(_storageType, _id, _sellAmount);
				}
				Close();
				break;
			case "Cancel":
				Close();
				break;
			}
		}
	}
}
