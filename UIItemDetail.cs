using Shared.Regulation;
using UnityEngine;

public class UIItemDetail : MonoBehaviour
{
	[SerializeField]
	private UILabel itemName;

	[SerializeField]
	private UILabel itemLevel;

	[SerializeField]
	private UILabel itemType;

	[SerializeField]
	private UILabel itemValue;

	[SerializeField]
	private GameObject Lock;

	[SerializeField]
	private UILabel setDetail;

	public void SetItemDetail(RoItem itemData, RoCommander commander)
	{
		if (itemData == null || commander == null)
		{
			SetLock(isLock: true);
			return;
		}
		SetLock(isLock: false);
		UISetter.SetLabel(itemName, Localization.Get(itemData.nameIdx));
		UISetter.SetLabel(itemLevel, string.Format(Localization.Get("1021"), itemData.level));
		UISetter.SetLabel(itemType, ItemTypeString(itemData.statType));
		UISetter.SetLabel(itemValue, "+" + itemData.statPoint);
	}

	public void SetLock(bool isLock)
	{
		UISetter.SetActive(Lock, isLock);
	}

	private string ItemTypeString(EItemStatType type)
	{
		string empty = string.Empty;
		return type switch
		{
			EItemStatType.ATK => empty = Localization.Get("1"), 
			EItemStatType.DEF => empty = Localization.Get("2"), 
			EItemStatType.ACCUR => empty = Localization.Get("5"), 
			EItemStatType.LUCK => empty = Localization.Get("3"), 
			_ => null, 
		};
	}

	public void SetEffect(RoCommander commander)
	{
		if (commander.completeSetItemEquip)
		{
			RoItem item = commander.FindEquipItem(1);
			EquipItemDataRow itemDb = RemoteObjectManager.instance.regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == item.id);
			ItemSetTypeString(itemDb);
		}
		else
		{
			UISetter.SetLabel(setDetail, Localization.Get("5030408"));
		}
	}

	private void ItemSetTypeString(EquipItemDataRow itemDb)
	{
		if (itemDb != null)
		{
			switch (itemDb.setItemType)
			{
			case EItemSetType.ATK:
				UISetter.SetLabel(setDetail, Localization.Get("1") + "+" + itemDb.statEffect);
				break;
			case EItemSetType.DEF:
				UISetter.SetLabel(setDetail, Localization.Get("2") + "+" + itemDb.statEffect);
				break;
			case EItemSetType.ACCUR:
				UISetter.SetLabel(setDetail, Localization.Get("5") + "+" + itemDb.statEffect);
				break;
			case EItemSetType.LUCK:
				UISetter.SetLabel(setDetail, Localization.Get("3") + "+" + itemDb.statEffect);
				break;
			case EItemSetType.CRITDMG:
				UISetter.SetLabel(setDetail, Localization.Get("8") + "+" + itemDb.statEffect + "%");
				break;
			case EItemSetType.CRITR:
				UISetter.SetLabel(setDetail, Localization.Get("6") + "+" + itemDb.statEffect + "%");
				break;
			}
		}
	}
}
