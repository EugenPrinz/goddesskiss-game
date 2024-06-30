using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIDormitoryShopListItem : UIItemBase
	{
		public UIDormitoryItemData item;

		public UISprite costIcon;

		public UILabel cost;

		public UILabel buy;

		public UIButton buyButton;

		public GameObject decoView;

		public GameObject packageView;

		public GameObject remainItemView;

		public UITimer timer;

		[HideInInspector]
		public EDormitoryItemType type;

		[HideInInspector]
		public Protocols.Dormitory.ShopProductItemInfo data;

		[HideInInspector]
		public bool isLimited;

		public void Set(EDormitoryItemType type, Protocols.Dormitory.ShopProductItemInfo data)
		{
			this.type = type;
			this.data = data;
			InvalidData();
		}

		private void InvalidData()
		{
			Regulation regulation = RemoteObjectManager.instance.regulation;
			string key = $"{type}_{data.id}";
			DormitoryShopDataRow dormitoryShopDataRow = regulation.dormitoryShopDtbl[key];
			switch (dormitoryShopDataRow.type)
			{
			case EDormitoryItemType.Normal:
			case EDormitoryItemType.Advanced:
				item.Set(regulation.dormitoryDecorationDtbl[dormitoryShopDataRow.id]);
				break;
			case EDormitoryItemType.Wallpaper:
				item.Set(regulation.dormitoryWallPaperDtbl[dormitoryShopDataRow.id]);
				break;
			case EDormitoryItemType.Thema:
				item.Set(regulation.dormitoryThemeMap[dormitoryShopDataRow.id][0]);
				break;
			case EDormitoryItemType.CostumeBody:
				item.Set(regulation.dormitoryBodyCostumeDtbl[dormitoryShopDataRow.id]);
				break;
			}
			costIcon.spriteName = data.goodsDr.iconId;
			cost.text = data.cost.ToString();
			buyButton.isEnabled = RemoteObjectManager.instance.localUser.resourceList[data.goodsDr.serverFieldName] >= data.cost;
			decoView.SetActive(dormitoryShopDataRow.type == EDormitoryItemType.Normal || dormitoryShopDataRow.type == EDormitoryItemType.Advanced);
			packageView.SetActive(dormitoryShopDataRow.type == EDormitoryItemType.Thema);
			if (dormitoryShopDataRow.sellType == EDormitoryShopSellType.Limited)
			{
				TimeData timeData = new TimeData();
				timeData.SetByDuration(data.endRemain);
				timer.SetFinishString(Localization.Get("81060"));
				timer.RegisterOnFinished(delegate
				{
					buyButton.isEnabled = false;
				});
				UISetter.SetTimer(timer, timeData);
				remainItemView.SetActive(value: true);
			}
			else
			{
				remainItemView.SetActive(value: false);
			}
			buy.text = ((data.buyLimit != 0) ? string.Format("{0}({1}/{2})", Localization.Get("81041"), data.buyCount, data.buyLimit) : Localization.Get("81041"));
			isLimited = data.buyLimit > 0 && data.buyCount >= data.buyLimit;
		}
	}
}
