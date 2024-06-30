using RoomDecorator.Data;
using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIInventoryListItem : UIItemBase
	{
		public UISprite image;

		public UILabel tileSize;

		public UILabel itemCount;

		[HideInInspector]
		public RoDormitory.InvenSlot data;

		public void Set(RoDormitory.InvenSlot slot)
		{
			data = slot;
			itemCount.text = data.amount.ToString();
			itemCount.color = ((data.amount <= SingletonMonoBehaviour<DormitoryData>.Instance.config.itemAmountLimit) ? Color.white : Color.red);
			switch (data.item.type)
			{
			case EDormitoryItemType.Normal:
			case EDormitoryItemType.Advanced:
			{
				DormitoryDecorationDataRow dormitoryDecorationDataRow = (DormitoryDecorationDataRow)data.item.data;
				image.SetAtlasImage("DormitoryTheme_" + dormitoryDecorationDataRow.atlasNumber, dormitoryDecorationDataRow.thumbnail);
				tileSize.gameObject.SetActive(value: true);
				tileSize.text = $"{dormitoryDecorationDataRow.xSize}x{dormitoryDecorationDataRow.ySize}";
				break;
			}
			case EDormitoryItemType.Wallpaper:
			{
				DormitoryWallpaperDataRow dormitoryWallpaperDataRow = (DormitoryWallpaperDataRow)data.item.data;
				image.SetAtlasImage("DormitoryTheme_" + dormitoryWallpaperDataRow.atlasNumber, dormitoryWallpaperDataRow.thumbnail);
				tileSize.gameObject.SetActive(value: false);
				break;
			}
			}
		}
	}
}
