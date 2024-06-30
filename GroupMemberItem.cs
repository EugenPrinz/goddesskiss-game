using Shared.Regulation;
using UnityEngine;

public class GroupMemberItem : UIGoods
{
	public GameObject costumeRoot;

	public UISprite cardBack;

	public UISprite cardOutline_1;

	public UISprite cardOutline_2;

	public UISprite cardOutline_3;

	public UISprite cardOutline_4;

	public UISprite cardCornerUp_1;

	public UISprite cardCornerUp_2;

	public UISprite cardCornerDown_1;

	public UISprite cardCornerDown_2;

	public UISprite cardOutlineTop_1;

	public UISprite cardOutlineTop_2;

	public UISprite cardOutlineBottom_1;

	public UISprite cardOutlineBottom_2;

	public UISprite getBlock;

	public UISprite gradeBlock;

	private int grade;

	public override void OnClick()
	{
		base.OnClick();
	}

	public void NotGetBlock()
	{
		if (rType == ETooltipType.Commander)
		{
			SetNavigationOpen(EStorageType.Medal, idx);
		}
		else if (rType == ETooltipType.Costume)
		{
			SetNavigationOpen(EStorageType.Costume, idx);
		}
		else if (rType == ETooltipType.Goods)
		{
			SetNavigationOpen(EStorageType.CollectionItem, idx);
		}
	}

	public void NotGradeBlock()
	{
		if (rType == ETooltipType.Commander)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("10084", Localization.Get((8900 + grade).ToString())));
		}
	}

	public void Set(GroupMemberDataRow itemInfo)
	{
		base.gameObject.transform.localScale = new Vector2(0.78f, 0.78f);
		Regulation regulation = RemoteObjectManager.instance.regulation;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetActive(commanderRoot, itemInfo.memberType == 1);
		UISetter.SetActive(costumeRoot, itemInfo.memberType == 0);
		UISetter.SetActive(goodsIcon, itemInfo.memberType == 2);
		UISetter.SetActive(count, itemInfo.memberType == 2);
		idx = itemInfo.memberIdx;
		if (itemInfo.memberType == 1)
		{
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[idx];
			UISetter.SetSprite(commanderIcon, commanderDataRow.thumbnailId);
			SetCardFrame(GetClassGroup(itemInfo.grade), GetSubClass(itemInfo.grade));
			RoCommander roCommander = localUser.commanderList.Find((RoCommander row) => row.id == idx);
			UISetter.SetActive(getBlock, roCommander.state != ECommanderState.Nomal);
			UISetter.SetActive(gradeBlock, roCommander.state == ECommanderState.Nomal && (int)roCommander.cls < itemInfo.grade);
			grade = itemInfo.grade;
			rType = ETooltipType.Commander;
		}
		else if (itemInfo.memberType == 0)
		{
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.commanderCostumeDtbl[idx];
			CommanderDataRow commanderData = regulation.commanderDtbl[commanderCostumeDataRow.cid.ToString()];
			costumeIcon.atlas = costumeAtlas[commanderCostumeDataRow.atlasNumber - 1];
			UISetter.SetSprite(costumeIcon, commanderData.resourceId + "_" + commanderCostumeDataRow.skinName);
			RoCommander roCommander2 = localUser.commanderList.Find((RoCommander row) => row.id == commanderData.id);
			if (roCommander2.state == ECommanderState.Nomal)
			{
				UISetter.SetActive(getBlock, !roCommander2.haveCostumeList.Contains(int.Parse(idx)));
			}
			else
			{
				UISetter.SetActive(getBlock, !localUser.isGetDonHaveCommCostume(commanderData.id, int.Parse(idx)));
			}
			UISetter.SetActive(gradeBlock, active: false);
			rType = ETooltipType.Costume;
		}
		else if (itemInfo.memberType == 2)
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[idx];
			UISetter.SetLabel(count, itemInfo.grade);
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
			int num = localUser.resourceList[goodsDataRow.serverFieldName];
			UISetter.SetActive(getBlock, itemInfo.grade > num);
			UISetter.SetActive(gradeBlock, active: false);
			rType = ETooltipType.Goods;
		}
	}

	private void SetNavigationOpen(EStorageType storageType, string id)
	{
		ItemExchangeDataRow itemExchangeDataRow = RemoteObjectManager.instance.regulation.itemExchangeDtbl.Find((ItemExchangeDataRow row) => row.type == storageType && row.typeidx == id);
		if (itemExchangeDataRow != null)
		{
			UINavigationPopUp uINavigationPopUp = UIPopup.Create<UINavigationPopUp>("NavigationPopUp");
			uINavigationPopUp.Init(storageType, id, itemExchangeDataRow);
			if (storageType == EStorageType.Part)
			{
				uINavigationPopUp.title.text = Localization.Get("8023");
			}
			else if (storageType == EStorageType.Medal)
			{
				uINavigationPopUp.title.text = Localization.Get("5608");
			}
			else if (storageType == EStorageType.Food)
			{
				uINavigationPopUp.title.text = Localization.Get("8023");
			}
			else if (storageType == EStorageType.Costume || storageType == EStorageType.CollectionItem)
			{
				uINavigationPopUp.title.text = Localization.Get("5608");
			}
		}
	}

	private void SetCardFrame(int classGroup, int subClass)
	{
		UISetter.SetSprite(cardOutline_1, "ig-character-bg-line-0" + classGroup);
		UISetter.SetActive(cardOutlineTop_1, subClass == 5);
		UISetter.SetActive(cardOutlineTop_2, subClass == 5);
		UISetter.SetActive(cardOutlineBottom_1, subClass == 5);
		UISetter.SetActive(cardOutlineBottom_2, subClass == 5);
		if (subClass == 5)
		{
			UISetter.SetSprite(cardCornerUp_1, "ig-character-frame" + (classGroup - 1) + "-4-up");
			UISetter.SetSprite(cardCornerUp_2, "ig-character-frame" + (classGroup - 1) + "-4-up");
			UISetter.SetSprite(cardCornerDown_1, "ig-character-frame" + (classGroup - 1) + "-4-down");
			UISetter.SetSprite(cardCornerDown_2, "ig-character-frame" + (classGroup - 1) + "-4-down");
			UISetter.SetSprite(cardOutlineTop_1, "ig-character-frame" + (classGroup - 1) + "-5-up");
			UISetter.SetSprite(cardOutlineTop_2, "ig-character-frame" + (classGroup - 1) + "-5-up");
			return;
		}
		UISetter.SetSprite(cardCornerUp_1, "ig-character-frame" + (classGroup - 1) + "-" + (subClass - 1) + "-up");
		UISetter.SetSprite(cardCornerUp_2, "ig-character-frame" + (classGroup - 1) + "-" + (subClass - 1) + "-up");
		UISetter.SetSprite(cardCornerDown_1, "ig-character-frame" + (classGroup - 1) + "-" + (subClass - 1) + "-down");
		UISetter.SetSprite(cardCornerDown_2, "ig-character-frame" + (classGroup - 1) + "-" + (subClass - 1) + "-down");
	}

	private static int GetClassGroup(int tier)
	{
		int num = 1;
		switch (tier)
		{
		case 1:
			return 1;
		case 2:
		case 3:
		case 4:
			return 2;
		default:
			if (tier >= 5 && tier <= 8)
			{
				return 3;
			}
			if (tier >= 9 && tier <= 13)
			{
				return 4;
			}
			if (tier >= 14 && tier <= 18)
			{
				return 5;
			}
			return 6;
		}
	}

	private static int GetSubClass(int tier)
	{
		int num = 1;
		switch (tier)
		{
		case 1:
			return tier;
		case 2:
		case 3:
		case 4:
			return tier - 1;
		default:
			if (tier >= 5 && tier <= 8)
			{
				return tier - 4;
			}
			if (tier >= 9 && tier <= 13)
			{
				return tier - 8;
			}
			if (tier >= 14 && tier <= 18)
			{
				return tier - 13;
			}
			return tier - 18;
		}
	}
}
