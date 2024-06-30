using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIDormitoryPackageListItem : UIItemBase
	{
		public UIDormitoryItemData item;

		[HideInInspector]
		public ETooltipType tooltipType;

		[HideInInspector]
		public string itemId;

		public void Set(DormitoryThemeDataRow data)
		{
			Regulation regulation = SingletonMonoBehaviour<DormitoryData>.Instance.regulation;
			itemId = data.itemId;
			switch (data.type)
			{
			case EDormitoryItemType.Normal:
			case EDormitoryItemType.Advanced:
				item.Set(regulation.dormitoryDecorationDtbl[data.itemId]);
				tooltipType = ((data.type != EDormitoryItemType.Normal) ? ETooltipType.Dormitory_AdvancedDeco : ETooltipType.Dormitory_NormalDeco);
				break;
			case EDormitoryItemType.Wallpaper:
				item.Set(regulation.dormitoryWallPaperDtbl[data.itemId]);
				tooltipType = ETooltipType.Dormitory_Wallpaper;
				break;
			case EDormitoryItemType.CostumeBody:
				item.Set(regulation.dormitoryBodyCostumeDtbl[data.itemId]);
				tooltipType = ETooltipType.Dormitory_CostumeBody;
				break;
			}
			item.SetAmount(data.amount);
		}

		public virtual void OnClick()
		{
			if (tooltipType != 0 && !string.IsNullOrEmpty(itemId))
			{
				UITooltip.Show(tooltipType, itemId);
			}
		}
	}
}
