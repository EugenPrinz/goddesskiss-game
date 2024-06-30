using Shared.Regulation;
using UnityEngine;

public class UIHaveListItem : UIItemBase
{
	[SerializeField]
	private UILabel itemLv;

	[SerializeField]
	private UISprite itemIcon;

	[SerializeField]
	private GameObject equipCheck;

	[SerializeField]
	private GameObject selectedRoot;

	[SerializeField]
	private UILabel Count;

	private bool _selected;

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(selectedRoot, selected);
		_selected = selected;
	}

	public void Set(RoItem item)
	{
		if (item != null)
		{
			EquipItemDataRow equipItemDataRow = RemoteObjectManager.instance.regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == item.id);
			if (equipItemDataRow != null)
			{
				UISetter.SetLabel(itemLv, string.Format(Localization.Get("1021"), item.level));
				UISetter.SetSprite(itemIcon, equipItemDataRow.equipItemIcon);
				UISetter.SetActive(equipCheck, item.isEquip);
				UISetter.SetLabel(Count, item.itemCount);
			}
		}
	}
}
