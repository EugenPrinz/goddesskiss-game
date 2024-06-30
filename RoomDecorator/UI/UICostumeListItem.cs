using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UICostumeListItem : UIItemBase
	{
		public GameObject select;

		public UILabel itemName;

		public UISprite image;

		public UILabel count;

		public GameObject costView;

		public UISprite costIcon;

		public UILabel costValue;

		private int _amount;

		public void Set(DormitoryHeadCostumeDataRow data, int amount)
		{
			_amount = amount;
			count.gameObject.SetActive(value: false);
			itemName.text = Localization.Get(data.name);
			image.SetAtlasImage("DormitoryCostume_" + data.atlasNumber, data.thumbnail);
			if (amount <= 0)
			{
				costView.SetActive(value: true);
				costIcon.spriteName = data.goodsDr.icon;
				costValue.text = data.price.ToString();
			}
			else
			{
				costView.SetActive(value: false);
			}
		}

		public void Set(DormitoryBodyCostumeDataRow data, int amount)
		{
			_amount = amount;
			costView.SetActive(value: false);
			count.gameObject.SetActive(value: true);
			itemName.text = Localization.Get(data.name);
			image.SetAtlasImage("DormitoryCostume_" + data.atlasNumber, data.thumbnail);
			count.text = amount.ToString();
		}

		public override void SetSelection(bool selected)
		{
			UISetter.SetActive(select, selected);
			UISetter.SetLabel(count, (!selected) ? _amount : (_amount - 1));
		}
	}
}
