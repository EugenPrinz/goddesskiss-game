using Shared.Regulation;
using UnityEngine;

public class UIStorageListItem : UIItemBase
{
	public UILabel count;

	public new UILabel name;

	public UILabel description;

	public GameObject costRoot;

	public UISprite costIcon;

	public UILabel cost;

	public GameObject selectedRoot;

	public UISprite goodsIcon;

	public UISprite commanderIcon;

	public GameObject partIcon;

	public UISprite part;

	public UISprite partBG;

	public UISprite partGrade;

	public UISprite partMark;

	public GameObject medalIcon;

	public UISprite medal;

	public UISprite medalMark;

	public GameObject sellBtn;

	public GameObject openBtn;

	public GameObject selectBtn;

	private bool _selected;

	[SerializeField]
	private GameObject equipItemDesc;

	[SerializeField]
	private UILabel statBaseValue;

	[SerializeField]
	private UILabel setEffectDesc;

	[SerializeField]
	private UILabel setEffecValue;

	[SerializeField]
	private GameObject ItemIconObj;

	[SerializeField]
	private UISprite itemIcon;

	[SerializeField]
	private UILabel itemCount;

	[SerializeField]
	private UILabel subDesc_1;

	[SerializeField]
	private UILabel subDesc_2;

	[SerializeField]
	private UILabel subDesc_3;

	[SerializeField]
	private UILabel subDescValue_2;

	[SerializeField]
	private UILabel subDescValue_3;

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(selectedRoot, selected);
		_selected = selected;
	}

	public void Set(string key, int count, EStorageType type, int level = 0)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(goodsIcon, type == EStorageType.Goods || type == EStorageType.Food || type == EStorageType.Box);
		UISetter.SetActive(medalIcon, type == EStorageType.Medal);
		UISetter.SetActive(partIcon, type == EStorageType.Part);
		UISetter.SetActive(ItemIconObj, type == EStorageType.Item);
		UISetter.SetActive(costRoot, active: true);
		UISetter.SetActive(sellBtn, active: true);
		UISetter.SetActive(openBtn, active: false);
		UISetter.SetActive(selectBtn, active: false);
		ItemExchangeDataRow itemExchangeDataRow = null;
		GoodsDataRow goodsDataRow = null;
		switch (type)
		{
		case EStorageType.Medal:
		{
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[key];
			itemExchangeDataRow = regulation.FindExchangeItemData(type, key);
			UISetter.SetSprite(medal, commanderDataRow.thumbnailId, snap: false);
			UISetter.SetLabel(name, Localization.Get(commanderDataRow.nickname));
			UISetter.SetActive(description, active: true);
			UISetter.SetActive(equipItemDesc, active: false);
			UISetter.SetLabel(description, Localization.Get(commanderDataRow.medalExplanation));
			UISetter.SetLabel(this.count, count);
			break;
		}
		case EStorageType.Goods:
		case EStorageType.Box:
		case EStorageType.Food:
		{
			GoodsDataRow goodsData = regulation.FindGoodsByServerFieldName(key);
			itemExchangeDataRow = regulation.FindExchangeItemData((!goodsData.isBox) ? type : EStorageType.Box, goodsData.GetKey());
			UISetter.SetSprite(goodsIcon, goodsData.iconId, snap: false);
			UISetter.SetLabel(name, Localization.Get(goodsData.name));
			UISetter.SetActive(description, active: true);
			UISetter.SetActive(equipItemDesc, active: false);
			UISetter.SetLabel(description, Localization.Get(goodsData.description));
			UISetter.SetActive(costRoot, !goodsData.isBox);
			UISetter.SetActive(sellBtn, !goodsData.isBox);
			if (goodsData.isBox)
			{
				RandomBoxRewardDataRow randomBoxRewardDataRow = regulation.randomBoxRewardDtbl.Find((RandomBoxRewardDataRow box) => box.idx == goodsData.type);
				UISetter.SetActive(openBtn, randomBoxRewardDataRow.giveType != 2);
				UISetter.SetActive(selectBtn, randomBoxRewardDataRow.giveType == 2);
			}
			UISetter.SetLabel(this.count, count);
			break;
		}
		case EStorageType.Item:
		{
			UISetter.SetActive(description, active: false);
			UISetter.SetActive(equipItemDesc, active: true);
			UISetter.SetActive(costRoot, active: false);
			UISetter.SetActive(sellBtn, active: false);
			UISetter.SetActive(openBtn, active: false);
			UISetter.SetActive(selectBtn, active: false);
			RoItem item = RemoteObjectManager.instance.localUser.EquipPossibleList_FindItem(key, level);
			if (item == null)
			{
				return;
			}
			EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == item.id);
			if (equipItemDataRow == null)
			{
				return;
			}
			UISetter.SetLabel(name, Localization.Get(item.nameIdx));
			UISetter.SetLabel(subDesc_1, Localization.Get("5030421"));
			UISetter.SetLabel(subDesc_2, ItemTypeString(item.statType));
			UISetter.SetLabel(subDescValue_2, item.statPoint.ToString());
			UISetter.SetLabel(subDesc_3, ItemSetTypeString(item.setType));
			if (item.setType == EItemSetType.CRITDMG && item.setType == EItemSetType.CRITR)
			{
				UISetter.SetLabel(subDescValue_3, "+" + equipItemDataRow.statEffect + "%");
			}
			else
			{
				UISetter.SetLabel(subDescValue_3, "+" + equipItemDataRow.statEffect);
			}
			UISetter.SetSprite(itemIcon, equipItemDataRow.equipItemIcon);
			UISetter.SetLabel(this.count, string.Format(Localization.Get("1021"), level));
			UISetter.SetLabel(name, Localization.Get(equipItemDataRow.equipItemName));
			itemExchangeDataRow = null;
			break;
		}
		}
		if (itemExchangeDataRow != null && int.Parse(itemExchangeDataRow.pricetypeidx) != 0)
		{
			GoodsDataRow goodsDataRow2 = regulation.goodsDtbl[itemExchangeDataRow.pricetypeidx];
			UISetter.SetSprite(costIcon, goodsDataRow2.iconId);
			UISetter.SetLabel(cost, itemExchangeDataRow.price);
		}
	}

	public void Set(RoPart partData)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(description, active: true);
		UISetter.SetActive(ItemIconObj, active: false);
		UISetter.SetActive(goodsIcon, active: false);
		UISetter.SetActive(commanderIcon, active: false);
		UISetter.SetActive(medalIcon, active: false);
		UISetter.SetActive(partIcon, active: true);
		UISetter.SetActive(costRoot, active: true);
		UISetter.SetActive(sellBtn, active: true);
		UISetter.SetActive(openBtn, active: false);
		UISetter.SetActive(selectBtn, active: false);
		UISetter.SetActive(equipItemDesc, active: false);
		PartDataRow partDataRow = regulation.partDtbl[partData.id];
		ItemExchangeDataRow itemExchangeDataRow = regulation.FindExchangeItemData(EStorageType.Part, partData.id);
		UISetter.SetLabel(name, Localization.Get(partDataRow.name));
		UISetter.SetLabel(count, partData.count);
		UISetter.SetLabel(description, Localization.Get(partDataRow.description));
		UISetter.SetLabel(cost, itemExchangeDataRow.price);
		UISetter.SetSprite(partBG, partDataRow.bgResource);
		UISetter.SetSprite(part, partDataRow.serverFieldName);
		UISetter.SetSprite(partMark, partDataRow.markResource);
		UISetter.SetSprite(partGrade, partDataRow.gradeResource);
	}

	public void Set(RoItem itemData)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(description, active: false);
		UISetter.SetActive(ItemIconObj, active: true);
		UISetter.SetActive(goodsIcon, active: false);
		UISetter.SetActive(commanderIcon, active: false);
		UISetter.SetActive(medalIcon, active: false);
		UISetter.SetActive(partIcon, active: false);
		UISetter.SetActive(equipItemDesc, active: true);
		UISetter.SetActive(costRoot, active: false);
		UISetter.SetActive(sellBtn, active: false);
		UISetter.SetActive(openBtn, active: false);
		UISetter.SetActive(selectBtn, active: false);
		EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == itemData.id);
		EquipItemDisassembleDataRow equipItemDisassembleDataRow = regulation.FindDisassembleItemInfo(itemData.disassembleType, itemData.level);
		UISetter.SetLabel(count, string.Format(Localization.Get("1021"), itemData.level));
		UISetter.SetSprite(itemIcon, equipItemDataRow.equipItemIcon);
		UISetter.SetLabel(itemCount, itemData.itemCount);
	}

	public void SetItemDetailDesc(RoItem item)
	{
		if (item == null)
		{
			return;
		}
		EquipItemDataRow equipItemDataRow = RemoteObjectManager.instance.regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == item.id);
		if (equipItemDataRow != null)
		{
			UISetter.SetLabel(name, Localization.Get(item.nameIdx));
			UISetter.SetLabel(subDesc_1, Localization.Get("5030421"));
			UISetter.SetLabel(subDesc_2, ItemTypeString(item.statType));
			UISetter.SetLabel(subDescValue_2, item.statPoint.ToString());
			UISetter.SetLabel(subDesc_3, ItemSetTypeString(item.setType));
			if (item.setType == EItemSetType.CRITDMG && item.setType == EItemSetType.CRITR)
			{
				UISetter.SetLabel(subDescValue_3, "+" + equipItemDataRow.statEffect + "%");
			}
			else
			{
				UISetter.SetLabel(subDescValue_3, "+" + equipItemDataRow.statEffect);
			}
		}
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

	private string ItemSetTypeString(EItemSetType setType)
	{
		string empty = string.Empty;
		return setType switch
		{
			EItemSetType.ATK => empty = Localization.Get("1"), 
			EItemSetType.DEF => empty = Localization.Get("2"), 
			EItemSetType.ACCUR => empty = Localization.Get("5"), 
			EItemSetType.LUCK => empty = Localization.Get("3"), 
			EItemSetType.CRITDMG => empty = Localization.Get("8"), 
			EItemSetType.CRITR => empty = Localization.Get("6"), 
			_ => null, 
		};
	}
}
