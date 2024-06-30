using System;
using Shared.Regulation;

namespace RoomDecorator.UI
{
	[Serializable]
	public class UIDormitoryItemData
	{
		public UILabel itemName;

		public UISprite thumbnail;

		public UILabel itemTileSize;

		public UILabel itemAmount;

		public void Set(DormitoryThemeDataRow data)
		{
			UISetter.SetLabel(itemName, Localization.Get(data.name));
			if (thumbnail != null)
			{
				thumbnail.SetAtlasImage("DormitoryTheme_" + data.atlasNumber, data.thumbnail);
			}
		}

		public void Set(DormitoryDecorationDataRow data)
		{
			UISetter.SetLabel(itemName, Localization.Get(data.name));
			if (thumbnail != null)
			{
				thumbnail.SetAtlasImage("DormitoryTheme_" + data.atlasNumber, data.thumbnail);
			}
			UISetter.SetActive(itemTileSize, active: true);
			UISetter.SetLabel(itemTileSize, $"{data.xSize}x{data.ySize}");
		}

		public void Set(DormitoryWallpaperDataRow data)
		{
			UISetter.SetLabel(itemName, Localization.Get(data.name));
			if (thumbnail != null)
			{
				thumbnail.SetAtlasImage("DormitoryTheme_" + data.atlasNumber, data.thumbnail);
			}
			UISetter.SetActive(itemTileSize, active: false);
		}

		public void Set(DormitoryBodyCostumeDataRow data)
		{
			UISetter.SetLabel(itemName, Localization.Get(data.name));
			if (thumbnail != null)
			{
				thumbnail.SetAtlasImage("DormitoryCostume_" + data.atlasNumber, data.thumbnail);
			}
			UISetter.SetActive(itemTileSize, active: false);
		}

		public void SetAmount(int amount)
		{
			UISetter.SetLabel(itemAmount, amount);
		}
	}
}
