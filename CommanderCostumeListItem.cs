using Shared.Regulation;
using UnityEngine;

public class CommanderCostumeListItem : UIItemBase
{
	public UIAtlas[] atlas;

	public UISprite thumbnail;

	public UILabel skinName;

	public UISprite select;

	public UISprite costIcon;

	public UILabel cost;

	public UILabel description;

	public GameObject newTag;

	public GameObject hotTag;

	public GameObject saleTag;

	public GameObject costInfo;

	public UILabel statType1;

	public UILabel statType2;

	public UILabel statType3;

	public UILabel stat1;

	public UILabel stat2;

	public UILabel stat3;

	public UIGrid buttonGrid;

	public GameObject buyButton;

	private const string GoldCostTypeName = "Goods-gold";

	private const string CashCostTypeName = "Goods-cash";

	public void Set(CommanderCostumeDataRow row, bool isHave, bool isNew = false, bool isHot = false, bool isSale = false)
	{
		CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[row.cid.ToString()];
		thumbnail.atlas = atlas[row.atlasNumber - 1];
		UISetter.SetSprite(thumbnail, commanderDataRow.resourceId + "_" + row.skinName, snap: true);
		UISetter.SetLabel(skinName, Localization.Get(row.name));
		if (row.sell == 2)
		{
			UISetter.SetSprite(costIcon, "main_icon_lock", snap: true);
			UISetter.SetLabel(cost, Localization.Get("20033"));
			if (cost != null)
			{
				cost.color = Color.white;
			}
		}
		else
		{
			if (row.gid == EPriceType.Cash)
			{
				UISetter.SetSprite(costIcon, "Goods-cash");
			}
			else if (row.gid == EPriceType.Gold)
			{
				UISetter.SetSprite(costIcon, "Goods-gold");
			}
			UISetter.SetLabel(cost, row.sellPrice);
			if (cost != null)
			{
				cost.color = Color.yellow;
			}
			if (costIcon != null)
			{
				costIcon.height = 50;
				costIcon.width = 50;
			}
		}
		UISetter.SetActive(costInfo, !isHave);
		UISetter.SetActive(statType1, row.statType1 != StatType.NONE);
		UISetter.SetActive(statType2, row.statType2 != StatType.NONE);
		UISetter.SetActive(statType3, row.statType3 != StatType.NONE);
		if (row.statType1 != 0)
		{
			UISetter.SetLabel(statType1, GetStatType(row.statType1));
			UISetter.SetLabel(stat1, (row.statType1 != StatType.CRITDMG && row.statType1 != StatType.CRITR) ? row.stat1.ToString() : Localization.Format("5781", row.stat1));
		}
		if (row.statType2 != 0)
		{
			UISetter.SetLabel(statType2, GetStatType(row.statType2));
			UISetter.SetLabel(stat2, (row.statType2 != StatType.CRITDMG && row.statType2 != StatType.CRITR) ? row.stat2.ToString() : Localization.Format("5781", row.stat2));
		}
		if (row.statType3 != 0)
		{
			UISetter.SetLabel(statType3, GetStatType(row.statType3));
			UISetter.SetLabel(stat3, (row.statType3 != StatType.CRITDMG && row.statType3 != StatType.CRITR) ? row.stat3.ToString() : Localization.Format("5781", row.stat3));
		}
		UISetter.SetActive(newTag, !isHave);
		UISetter.SetActive(hotTag, !isHave);
		UISetter.SetActive(saleTag, !isHave);
		if (!isHave)
		{
			UISetter.SetActive(newTag, isNew);
			UISetter.SetActive(hotTag, isHot);
			UISetter.SetActive(saleTag, isSale);
		}
	}

	public void SetBuy(CommanderCostumeDataRow row)
	{
		CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[row.cid.ToString()];
		thumbnail.atlas = atlas[row.atlasNumber - 1];
		UISetter.SetSprite(thumbnail, commanderDataRow.resourceId + "_" + row.skinName);
		UISetter.SetLabel(skinName, Localization.Get(row.name));
		UISetter.SetLabel(description, Localization.Get(row.Desc));
		if (row.sell == 2)
		{
			UISetter.SetActive(costIcon, active: false);
			UISetter.SetActive(buyButton, active: false);
			UISetter.SetLabel(cost, Localization.Get("20033"));
		}
		else
		{
			UISetter.SetLabel(cost, row.sellPrice);
			UISetter.SetActive(costIcon, active: true);
			UISetter.SetActive(buyButton, active: true);
			if (row.gid == EPriceType.Cash)
			{
				UISetter.SetSprite(costIcon, "Goods-cash");
			}
			else if (row.gid == EPriceType.Gold)
			{
				UISetter.SetSprite(costIcon, "Goods-gold");
			}
		}
		buttonGrid.Reposition();
	}

	private string GetStatType(StatType type)
	{
		string result = string.Empty;
		switch (type)
		{
		case StatType.ATK:
			result = Localization.Get("1");
			break;
		case StatType.DEF:
			result = Localization.Get("2");
			break;
		case StatType.HP:
			result = Localization.Get("4");
			break;
		case StatType.ACCUR:
			result = Localization.Get("5");
			break;
		case StatType.LUCK:
			result = Localization.Get("3");
			break;
		case StatType.CRITR:
			result = Localization.Get("6");
			break;
		case StatType.CRITDMG:
			result = Localization.Get("8");
			break;
		case StatType.MOB:
			result = Localization.Get("7");
			break;
		}
		return result;
	}

	public override void SetSelection(bool selected)
	{
		base.SetSelection(selected);
		UISetter.SetActive(select, selected);
		if (selected)
		{
			skinName.color = Color.yellow;
		}
		else
		{
			skinName.color = Color.white;
		}
	}
}
