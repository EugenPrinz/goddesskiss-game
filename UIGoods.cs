using System.Collections.Generic;
using System.Text;
using Shared.Regulation;
using UnityEngine;

public class UIGoods : UIItemBase
{
	public UIAtlas[] costumeAtlas;

	public UILabel day;

	public UILabel count;

	public UILabel nickname;

	public UISprite icon;

	public UISprite colorIcon;

	public UILabel gold;

	public UILabel cash;

	public UILabel description;

	public UILabel vip;

	public GameObject selectedRoot;

	public GameObject receivedRoot;

	public GameObject emptyRoot;

	public UISprite receivedBlock;

	public UISprite vipMark;

	public UILabel vipBonus;

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

	public GameObject goodsRoot;

	public GameObject commanderRoot;

	public UISprite dormitoryItemIcon;

	public GameObject weaponRoot;

	public UISprite weaponIcon;

	public GameObject weaponGrade;

	public UISprite weaponBg;

	public UILabel weaponUseCommander;

	public UILabel weaponLevel;

	public UILabel GetAmount;

	public UISprite costumeIcon;

	public GameObject ItemIconRoot;

	public UISprite ItemIcon;

	public GameObject effectRoot;

	public UILabel probability;

	private bool _selected;

	[HideInInspector]
	public ETooltipType rType;

	[HideInInspector]
	public string idx;

	public Animation openAnimation;

	public GameObject vipGacha_soldOut;

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(selectedRoot, selected);
		UISetter.SetColor(colorIcon, (!selected) ? Color.gray : Color.white);
		_selected = selected;
	}

	private void Start()
	{
	}

	public void Set(string key, int count)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		GoodsDataRow goodsDataRow = regulation.FindGoodsByServerFieldName(key);
		UISetter.SetActive(goodsIcon, active: true);
		UISetter.SetSprite(goodsIcon, goodsDataRow.iconId, snap: false);
		UISetter.SetLabel(nickname, Localization.Get(goodsDataRow.name));
		UISetter.SetLabel(this.count, count);
		rType = ETooltipType.Goods;
		idx = goodsDataRow.type;
	}

	public void SetGoodsId(string id, int count)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		GoodsDataRow goodsDataRow = regulation.FindGoodsServerFieldName(id);
		UISetter.SetActive(goodsIcon, active: true);
		UISetter.SetSprite(goodsIcon, goodsDataRow.iconId, snap: false);
		UISetter.SetLabel(nickname, Localization.Get(goodsDataRow.name));
		UISetter.SetLabel(this.count, count);
		rType = ETooltipType.Goods;
		idx = goodsDataRow.type;
	}

	public void SetReceive(bool received)
	{
		UISetter.SetActive(receivedRoot, received);
	}

	public void SetReceiveBlock(bool received)
	{
		UISetter.SetActive(receivedBlock, received);
	}

	public void Set(DailyBonusDataRow row)
	{
		UISetter.SetLabel(day, row.day);
		UISetter.SetLabel(count, "x" + row.goodsCount);
		Regulation regulation = RemoteObjectManager.instance.regulation;
		if (row.vipLevel > 0)
		{
			UISetter.SetActive(vipMark, active: true);
			UISetter.SetLabel(vipBonus, $"VIP {row.vipLevel} X{row.multiply}");
		}
		UISetter.SetActive(goodsIcon, row.rewardType == ERewardType.Goods);
		UISetter.SetActive(commanderIcon, row.rewardType == ERewardType.Commander);
		UISetter.SetActive(medalIcon, row.rewardType == ERewardType.Medal);
		UISetter.SetActive(partIcon, row.rewardType == ERewardType.UnitMaterial);
		UISetter.SetActive(ItemIconRoot, row.rewardType == ERewardType.Item);
		UISetter.SetActive(weaponRoot, row.rewardType == ERewardType.WeaponItem);
		idx = row.goodsId;
		if (row.rewardType == ERewardType.Goods)
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[row.goodsId];
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
			rType = ETooltipType.Goods;
		}
		else if (row.rewardType == ERewardType.Medal)
		{
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[row.goodsId];
			UISetter.SetSprite(medal, commanderDataRow.thumbnailId);
			rType = ETooltipType.Medal;
		}
		else if (row.rewardType == ERewardType.Commander)
		{
			CommanderDataRow commanderDataRow2 = regulation.commanderDtbl[row.goodsId];
			UISetter.SetSprite(commanderIcon, commanderDataRow2.thumbnailId);
			rType = ETooltipType.Commander;
		}
		else if (row.rewardType == ERewardType.UnitMaterial)
		{
			PartDataRow partDataRow = regulation.partDtbl[row.goodsId];
			UISetter.SetSprite(partBG, partDataRow.bgResource);
			UISetter.SetSprite(part, partDataRow.serverFieldName);
			UISetter.SetSprite(partMark, partDataRow.markResource);
			UISetter.SetSprite(partGrade, partDataRow.gradeResource);
			rType = ETooltipType.UnitMaterial;
		}
		else if (row.rewardType == ERewardType.Item)
		{
			EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow item) => item.key == row.goodsId);
			UISetter.SetSprite(ItemIcon, equipItemDataRow.equipItemIcon);
			rType = ETooltipType.Item;
		}
	}

	public void Set(GachaRewardDataRow row, List<int> countList, float probability = -1f)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(goodsIcon, row.rewardType == ERewardType.Goods);
		UISetter.SetActive(commanderIcon, row.rewardType == ERewardType.Commander);
		UISetter.SetActive(medalIcon, row.rewardType == ERewardType.Medal);
		UISetter.SetActive(partIcon, row.rewardType == ERewardType.UnitMaterial);
		UISetter.SetActive(costumeIcon, row.rewardType == ERewardType.Costume);
		UISetter.SetActive(ItemIconRoot, row.rewardType == ERewardType.Item);
		UISetter.SetActive(this.probability, probability > 0f);
		UISetter.SetLabel(this.probability, $"{probability}%");
		UISetter.SetActive(weaponRoot, row.rewardType == ERewardType.WeaponItem);
		if (row.rewardType == ERewardType.Goods && int.Parse(row.rewardId) == 4)
		{
			UISetter.SetLabel(count, $"{countList[0]}~{countList[countList.Count - 1]}");
		}
		else
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Length = 0;
			for (int i = 0; i < countList.Count; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append("/");
				}
				stringBuilder.Append(countList[i]);
			}
			UISetter.SetLabel(count, stringBuilder.ToString());
		}
		idx = row.rewardId;
		if (row.rewardType == ERewardType.Goods)
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[row.rewardId];
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
			UISetter.SetLabel(nickname, Localization.Get(goodsDataRow.name));
			rType = ETooltipType.Goods;
		}
		else if (row.rewardType == ERewardType.Commander)
		{
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[row.rewardId];
			UISetter.SetSprite(commanderIcon, commanderDataRow.thumbnailId);
			UISetter.SetLabel(count, string.Empty);
			UISetter.SetLabel(nickname, commanderDataRow.nickname);
			rType = ETooltipType.Commander;
		}
		else if (row.rewardType == ERewardType.Medal)
		{
			CommanderDataRow commanderDataRow2 = regulation.commanderDtbl[row.rewardId];
			UISetter.SetSprite(medal, commanderDataRow2.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow2.nickname);
			rType = ETooltipType.Medal;
		}
		else if (row.rewardType == ERewardType.UnitMaterial)
		{
			PartDataRow partDataRow = regulation.partDtbl[row.rewardId];
			UISetter.SetLabel(nickname, Localization.Get(partDataRow.name));
			UISetter.SetSprite(partBG, partDataRow.bgResource);
			UISetter.SetSprite(part, partDataRow.serverFieldName);
			UISetter.SetSprite(partMark, partDataRow.markResource);
			UISetter.SetSprite(partGrade, partDataRow.gradeResource);
			rType = ETooltipType.UnitMaterial;
		}
		else if (row.rewardType == ERewardType.Costume)
		{
			UISetter.SetLabel(count, string.Empty);
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.commanderCostumeDtbl[row.rewardId];
			CommanderDataRow commanderDataRow3 = regulation.commanderDtbl[commanderCostumeDataRow.cid.ToString()];
			costumeIcon.atlas = costumeAtlas[commanderCostumeDataRow.atlasNumber - 1];
			UISetter.SetSprite(costumeIcon, commanderDataRow3.resourceId + "_" + commanderCostumeDataRow.skinName);
			UISetter.SetLabel(nickname, Localization.Get(commanderCostumeDataRow.name));
			rType = ETooltipType.Costume;
		}
		else if (row.rewardType == ERewardType.Item)
		{
			EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow item) => item.key == row.rewardId);
			UISetter.SetSprite(ItemIcon, equipItemDataRow.equipItemIcon);
			UISetter.SetLabel(nickname, Localization.Get(equipItemDataRow.equipItemName));
			rType = ETooltipType.Item;
		}
	}

	public void Set(Protocols.RewardInfo.RewardData reward)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(goodsIcon, reward.rewardType == ERewardType.Goods || reward.rewardType == ERewardType.Favor || reward.rewardType == ERewardType.Box || (reward.rewardType >= ERewardType.GroupEff_1 && reward.rewardType <= ERewardType.GroupEff_8) || reward.rewardType == ERewardType.EventItem || reward.rewardType == ERewardType.CollectionItem || reward.rewardType == ERewardType.Dormitory_Goods);
		UISetter.SetActive(commanderIcon, reward.rewardType == ERewardType.Commander);
		UISetter.SetActive(medalIcon, reward.rewardType == ERewardType.Medal);
		UISetter.SetActive(partIcon, reward.rewardType == ERewardType.UnitMaterial);
		UISetter.SetActive(costumeIcon, reward.rewardType == ERewardType.Costume);
		UISetter.SetActive(weaponRoot, reward.rewardType == ERewardType.WeaponItem);
		UISetter.SetActive(count, reward.rewardType != ERewardType.Costume && reward.rewardType != ERewardType.Commander && reward.rewardType != ERewardType.WeaponItem);
		UISetter.SetLabel(count, reward.rewardCnt);
		UISetter.SetActive(ItemIconRoot, reward.rewardType == ERewardType.Item);
		UISetter.SetActive(effectRoot, reward.effect == 3);
		UISetter.SetActive(dormitoryItemIcon, reward.rewardType == ERewardType.Dormitory_NormalDeco || reward.rewardType == ERewardType.Dormitory_AdvancedDeco || reward.rewardType == ERewardType.Dormitory_Wallpaper || reward.rewardType == ERewardType.Dormitory_CostumeBody || reward.rewardType == ERewardType.Dormitory_CostumeHead);
		idx = reward.rewardId;
		if (reward.rewardType == ERewardType.Goods || reward.rewardType == ERewardType.Favor || reward.rewardType == ERewardType.Box || reward.rewardType == ERewardType.EventItem || reward.rewardType == ERewardType.CollectionItem || reward.rewardType == ERewardType.Dormitory_Goods)
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[reward.rewardId];
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
			rType = ETooltipType.Goods;
		}
		else if (reward.rewardType == ERewardType.Medal)
		{
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[reward.rewardId];
			UISetter.SetSprite(medal, commanderDataRow.thumbnailId);
			rType = ETooltipType.Medal;
		}
		else if (reward.rewardType == ERewardType.Commander)
		{
			CommanderDataRow commanderDataRow2 = regulation.commanderDtbl[reward.rewardId];
			UISetter.SetSprite(commanderIcon, commanderDataRow2.thumbnailId);
			rType = ETooltipType.Commander;
		}
		else if (reward.rewardType == ERewardType.UnitMaterial)
		{
			PartDataRow partDataRow = regulation.partDtbl[reward.rewardId];
			UISetter.SetSprite(partBG, partDataRow.bgResource);
			UISetter.SetSprite(part, partDataRow.serverFieldName);
			UISetter.SetSprite(partMark, partDataRow.markResource);
			UISetter.SetSprite(partGrade, partDataRow.gradeResource);
			rType = ETooltipType.UnitMaterial;
		}
		else if (reward.rewardType == ERewardType.Costume)
		{
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.commanderCostumeDtbl[reward.rewardId];
			CommanderDataRow commanderDataRow3 = regulation.commanderDtbl[commanderCostumeDataRow.cid.ToString()];
			costumeIcon.atlas = costumeAtlas[commanderCostumeDataRow.atlasNumber - 1];
			UISetter.SetSprite(costumeIcon, commanderDataRow3.resourceId + "_" + commanderCostumeDataRow.skinName);
			rType = ETooltipType.Costume;
		}
		else if (reward.rewardType == ERewardType.Item)
		{
			EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == reward.rewardId);
			if (equipItemDataRow != null)
			{
				UISetter.SetSprite(ItemIcon, equipItemDataRow.equipItemIcon);
				UISetter.SetLabel(count, reward.rewardCnt);
				rType = ETooltipType.Item;
			}
		}
		else if (reward.rewardType >= ERewardType.GroupEff_1 && reward.rewardType <= ERewardType.GroupEff_8)
		{
			string spriteName = string.Empty;
			if (int.Parse(reward.rewardId) <= 1000)
			{
				spriteName = "group_buff";
			}
			else if (int.Parse(reward.rewardId) > 1000)
			{
				spriteName = "group_buff_" + reward.rewardId;
			}
			UISetter.SetSprite(goodsIcon, spriteName);
			UISetter.SetActive(count, active: false);
			rType = ETooltipType.Undefined;
		}
		else if (reward.rewardType == ERewardType.WeaponItem)
		{
			WeaponDataRow weaponDataRow = regulation.weaponDtbl.Find((WeaponDataRow row) => row.idx == reward.rewardId);
			if (weaponDataRow != null)
			{
				UISetter.SetSprite(weaponIcon, weaponDataRow.weaponIcon);
				UISetter.SetRank(weaponGrade, weaponDataRow.weaponGrade);
				UISetter.SetSprite(weaponBg, $"weaponback_{weaponDataRow.weaponGrade}");
				UISetter.SetActive(count, active: false);
				rType = ETooltipType.WeaponItem;
			}
		}
		else if (reward.rewardType == ERewardType.Dormitory_NormalDeco || reward.rewardType == ERewardType.Dormitory_AdvancedDeco)
		{
			DormitoryDecorationDataRow dormitoryDecorationDataRow = regulation.dormitoryDecorationDtbl[reward.rewardId];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryTheme_" + dormitoryDecorationDataRow.atlasNumber, dormitoryDecorationDataRow.thumbnail);
			rType = ((reward.rewardType != ERewardType.Dormitory_NormalDeco) ? ETooltipType.Dormitory_AdvancedDeco : ETooltipType.Dormitory_NormalDeco);
		}
		else if (reward.rewardType == ERewardType.Dormitory_Wallpaper)
		{
			DormitoryWallpaperDataRow dormitoryWallpaperDataRow = regulation.dormitoryWallPaperDtbl[reward.rewardId];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryTheme_" + dormitoryWallpaperDataRow.atlasNumber, dormitoryWallpaperDataRow.thumbnail);
			rType = ETooltipType.Dormitory_Wallpaper;
		}
		else if (reward.rewardType == ERewardType.Dormitory_CostumeHead)
		{
			DormitoryHeadCostumeDataRow dormitoryHeadCostumeDataRow = regulation.dormitoryHeadCostumeDtbl[reward.rewardId];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryCostume_" + dormitoryHeadCostumeDataRow.atlasNumber, dormitoryHeadCostumeDataRow.thumbnail);
			rType = ETooltipType.Dormitory_CostumeHead;
		}
		else if (reward.rewardType == ERewardType.Dormitory_CostumeBody)
		{
			DormitoryBodyCostumeDataRow dormitoryBodyCostumeDataRow = regulation.dormitoryBodyCostumeDtbl[reward.rewardId];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryCostume_" + dormitoryBodyCostumeDataRow.atlasNumber, dormitoryBodyCostumeDataRow.thumbnail);
			rType = ETooltipType.Dormitory_CostumeBody;
		}
	}

	public void Set(RewardDataRow reward)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(goodsIcon, reward.rewardType == ERewardType.Goods || reward.rewardType == ERewardType.Favor || reward.rewardType == ERewardType.Box || (reward.rewardType >= ERewardType.GroupEff_1 && reward.rewardType <= ERewardType.GroupEff_8) || reward.rewardType == ERewardType.EventItem || reward.rewardType == ERewardType.Dormitory_Goods);
		UISetter.SetActive(commanderIcon, reward.rewardType == ERewardType.Commander);
		UISetter.SetActive(medalIcon, reward.rewardType == ERewardType.Medal);
		UISetter.SetActive(partIcon, reward.rewardType == ERewardType.UnitMaterial);
		UISetter.SetActive(costumeIcon, reward.rewardType == ERewardType.Costume);
		UISetter.SetActive(ItemIconRoot, reward.rewardType == ERewardType.Item);
		UISetter.SetActive(weaponRoot, reward.rewardType == ERewardType.WeaponItem);
		UISetter.SetActive(count, reward.rewardType != ERewardType.Costume && reward.rewardType != ERewardType.Commander && reward.rewardType != ERewardType.WeaponItem);
		UISetter.SetLabel(count, reward.maxCount);
		UISetter.SetLabel(vip, $"VIP {reward.typeIndex}");
		UISetter.SetActive(dormitoryItemIcon, reward.rewardType == ERewardType.Dormitory_NormalDeco || reward.rewardType == ERewardType.Dormitory_AdvancedDeco || reward.rewardType == ERewardType.Dormitory_Wallpaper || reward.rewardType == ERewardType.Dormitory_CostumeBody || reward.rewardType == ERewardType.Dormitory_CostumeHead);
		idx = reward.rewardIdx.ToString();
		if (reward.rewardType == ERewardType.Goods || reward.rewardType == ERewardType.Favor || reward.rewardType == ERewardType.CollectionItem || reward.rewardType == ERewardType.Box || reward.rewardType == ERewardType.EventItem || reward.rewardType == ERewardType.Dormitory_Goods)
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[reward.rewardIdx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(goodsDataRow.name));
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
			rType = ETooltipType.Goods;
		}
		else if (reward.rewardType == ERewardType.Medal)
		{
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(medal, commanderDataRow.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow.nickname);
			rType = ETooltipType.Medal;
		}
		else if (reward.rewardType == ERewardType.Commander)
		{
			CommanderDataRow commanderDataRow2 = regulation.commanderDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(commanderIcon, commanderDataRow2.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow2.nickname);
			rType = ETooltipType.Commander;
		}
		else if (reward.rewardType == ERewardType.UnitMaterial)
		{
			PartDataRow partDataRow = regulation.partDtbl[reward.rewardIdx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(partDataRow.name));
			UISetter.SetSprite(partBG, partDataRow.bgResource);
			UISetter.SetSprite(part, partDataRow.serverFieldName);
			UISetter.SetSprite(partMark, partDataRow.markResource);
			UISetter.SetSprite(partGrade, partDataRow.gradeResource);
			rType = ETooltipType.UnitMaterial;
		}
		else if (reward.rewardType == ERewardType.Costume)
		{
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.commanderCostumeDtbl[reward.rewardIdx.ToString()];
			CommanderDataRow commanderDataRow3 = regulation.commanderDtbl[commanderCostumeDataRow.cid.ToString()];
			costumeIcon.atlas = costumeAtlas[commanderCostumeDataRow.atlasNumber - 1];
			UISetter.SetSprite(costumeIcon, commanderDataRow3.resourceId + "_" + commanderCostumeDataRow.skinName);
			UISetter.SetLabel(nickname, Localization.Get(commanderCostumeDataRow.name));
			rType = ETooltipType.Costume;
		}
		else if (reward.rewardType == ERewardType.Item)
		{
			EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == reward.rewardIdx.ToString());
			if (equipItemDataRow != null)
			{
				UISetter.SetSprite(ItemIcon, equipItemDataRow.equipItemIcon);
				UISetter.SetLabel(nickname, Localization.Get(equipItemDataRow.equipItemName));
			}
			rType = ETooltipType.Item;
		}
		else if (reward.rewardType == ERewardType.WeaponItem)
		{
			WeaponDataRow weaponDataRow = regulation.weaponDtbl.Find((WeaponDataRow row) => row.idx == reward.rewardIdx.ToString());
			if (weaponDataRow != null)
			{
				UISetter.SetSprite(weaponIcon, weaponDataRow.weaponIcon);
				UISetter.SetRank(weaponGrade, weaponDataRow.weaponGrade);
				UISetter.SetSprite(weaponBg, $"weaponback_{weaponDataRow.weaponGrade}");
				UISetter.SetActive(count, active: false);
				rType = ETooltipType.WeaponItem;
			}
		}
		else if (reward.rewardType >= ERewardType.GroupEff_1 && reward.rewardType <= ERewardType.GroupEff_8)
		{
			string spriteName = string.Empty;
			if (reward.rewardIdx <= 1000)
			{
				spriteName = "group_buff";
			}
			else if (reward.rewardIdx > 1000)
			{
				spriteName = "group_buff_" + reward.rewardIdx;
			}
			UISetter.SetSprite(goodsIcon, spriteName);
			UISetter.SetActive(count, active: false);
			rType = ETooltipType.Undefined;
		}
		else if (reward.rewardType == ERewardType.Dormitory_NormalDeco || reward.rewardType == ERewardType.Dormitory_AdvancedDeco)
		{
			DormitoryDecorationDataRow dormitoryDecorationDataRow = regulation.dormitoryDecorationDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryTheme_" + dormitoryDecorationDataRow.atlasNumber, dormitoryDecorationDataRow.thumbnail);
			rType = ((reward.rewardType != ERewardType.Dormitory_NormalDeco) ? ETooltipType.Dormitory_AdvancedDeco : ETooltipType.Dormitory_NormalDeco);
		}
		else if (reward.rewardType == ERewardType.Dormitory_Wallpaper)
		{
			DormitoryWallpaperDataRow dormitoryWallpaperDataRow = regulation.dormitoryWallPaperDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryTheme_" + dormitoryWallpaperDataRow.atlasNumber, dormitoryWallpaperDataRow.thumbnail);
			rType = ETooltipType.Dormitory_Wallpaper;
		}
		else if (reward.rewardType == ERewardType.Dormitory_CostumeHead)
		{
			DormitoryHeadCostumeDataRow dormitoryHeadCostumeDataRow = regulation.dormitoryHeadCostumeDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryCostume_" + dormitoryHeadCostumeDataRow.atlasNumber, dormitoryHeadCostumeDataRow.thumbnail);
			rType = ETooltipType.Dormitory_CostumeHead;
		}
		else if (reward.rewardType == ERewardType.Dormitory_CostumeBody)
		{
			DormitoryBodyCostumeDataRow dormitoryBodyCostumeDataRow = regulation.dormitoryBodyCostumeDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryCostume_" + dormitoryBodyCostumeDataRow.atlasNumber, dormitoryBodyCostumeDataRow.thumbnail);
			rType = ETooltipType.Dormitory_CostumeBody;
		}
	}

	public void Set(RandomBoxRewardDataRow reward)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(goodsIcon, reward.rewardType == ERewardType.Goods || reward.rewardType == ERewardType.Favor || reward.rewardType == ERewardType.Box || (reward.rewardType >= ERewardType.GroupEff_1 && reward.rewardType <= ERewardType.GroupEff_8) || reward.rewardType == ERewardType.EventItem || reward.rewardType == ERewardType.Dormitory_Goods);
		UISetter.SetActive(commanderIcon, reward.rewardType == ERewardType.Commander);
		UISetter.SetActive(medalIcon, reward.rewardType == ERewardType.Medal);
		UISetter.SetActive(partIcon, reward.rewardType == ERewardType.UnitMaterial);
		UISetter.SetActive(costumeIcon, reward.rewardType == ERewardType.Costume);
		UISetter.SetActive(ItemIconRoot, reward.rewardType == ERewardType.Item);
		UISetter.SetActive(count, reward.rewardType != ERewardType.Costume && reward.rewardType != ERewardType.Commander);
		UISetter.SetLabel(count, reward.rewardAmountMax);
		UISetter.SetActive(dormitoryItemIcon, reward.rewardType == ERewardType.Dormitory_NormalDeco || reward.rewardType == ERewardType.Dormitory_AdvancedDeco || reward.rewardType == ERewardType.Dormitory_Wallpaper || reward.rewardType == ERewardType.Dormitory_CostumeBody || reward.rewardType == ERewardType.Dormitory_CostumeHead);
		idx = reward.rewardIdx.ToString();
		if (reward.rewardType == ERewardType.Goods || reward.rewardType == ERewardType.Favor || reward.rewardType == ERewardType.CollectionItem || reward.rewardType == ERewardType.Box || reward.rewardType == ERewardType.EventItem || reward.rewardType == ERewardType.Dormitory_Goods)
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[reward.rewardIdx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(goodsDataRow.name));
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
		}
		else if (reward.rewardType == ERewardType.Medal)
		{
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(medal, commanderDataRow.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow.nickname);
			rType = ETooltipType.Medal;
		}
		else if (reward.rewardType == ERewardType.Commander)
		{
			CommanderDataRow commanderDataRow2 = regulation.commanderDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(commanderIcon, commanderDataRow2.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow2.nickname);
		}
		else if (reward.rewardType == ERewardType.UnitMaterial)
		{
			PartDataRow partDataRow = regulation.partDtbl[reward.rewardIdx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(partDataRow.name));
			UISetter.SetSprite(partBG, partDataRow.bgResource);
			UISetter.SetSprite(part, partDataRow.serverFieldName);
			UISetter.SetSprite(partMark, partDataRow.markResource);
			UISetter.SetSprite(partGrade, partDataRow.gradeResource);
		}
		else if (reward.rewardType == ERewardType.Costume)
		{
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.commanderCostumeDtbl[reward.rewardIdx.ToString()];
			CommanderDataRow commanderDataRow3 = regulation.commanderDtbl[commanderCostumeDataRow.cid.ToString()];
			costumeIcon.atlas = costumeAtlas[commanderCostumeDataRow.atlasNumber - 1];
			UISetter.SetSprite(costumeIcon, commanderDataRow3.resourceId + "_" + commanderCostumeDataRow.skinName);
			UISetter.SetLabel(nickname, Localization.Get(commanderCostumeDataRow.name));
		}
		else if (reward.rewardType == ERewardType.Item)
		{
			EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == reward.rewardIdx.ToString());
			if (equipItemDataRow != null)
			{
				UISetter.SetSprite(ItemIcon, equipItemDataRow.equipItemIcon);
				UISetter.SetLabel(nickname, Localization.Get(equipItemDataRow.equipItemName));
			}
		}
		else if (reward.rewardType == ERewardType.Dormitory_NormalDeco || reward.rewardType == ERewardType.Dormitory_AdvancedDeco)
		{
			DormitoryDecorationDataRow dormitoryDecorationDataRow = regulation.dormitoryDecorationDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryTheme_" + dormitoryDecorationDataRow.atlasNumber, dormitoryDecorationDataRow.thumbnail);
			rType = ((reward.rewardType != ERewardType.Dormitory_NormalDeco) ? ETooltipType.Dormitory_AdvancedDeco : ETooltipType.Dormitory_NormalDeco);
		}
		else if (reward.rewardType == ERewardType.Dormitory_Wallpaper)
		{
			DormitoryWallpaperDataRow dormitoryWallpaperDataRow = regulation.dormitoryWallPaperDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryTheme_" + dormitoryWallpaperDataRow.atlasNumber, dormitoryWallpaperDataRow.thumbnail);
			rType = ETooltipType.Dormitory_Wallpaper;
		}
		else if (reward.rewardType == ERewardType.Dormitory_CostumeHead)
		{
			DormitoryHeadCostumeDataRow dormitoryHeadCostumeDataRow = regulation.dormitoryHeadCostumeDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryCostume_" + dormitoryHeadCostumeDataRow.atlasNumber, dormitoryHeadCostumeDataRow.thumbnail);
			rType = ETooltipType.Dormitory_CostumeHead;
		}
		else if (reward.rewardType == ERewardType.Dormitory_CostumeBody)
		{
			DormitoryBodyCostumeDataRow dormitoryBodyCostumeDataRow = regulation.dormitoryBodyCostumeDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryCostume_" + dormitoryBodyCostumeDataRow.atlasNumber, dormitoryBodyCostumeDataRow.thumbnail);
			rType = ETooltipType.Dormitory_CostumeBody;
		}
		rType = ETooltipType.Undefined;
	}

	public void Set(RewardDataRow reward, string vipLevel, string vipMultiply)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(goodsIcon, reward.rewardType == ERewardType.Goods || reward.rewardType == ERewardType.Favor || reward.rewardType == ERewardType.Box || (reward.rewardType >= ERewardType.GroupEff_1 && reward.rewardType <= ERewardType.GroupEff_8) || reward.rewardType == ERewardType.EventItem);
		UISetter.SetActive(commanderIcon, reward.rewardType == ERewardType.Commander);
		UISetter.SetActive(medalIcon, reward.rewardType == ERewardType.Medal);
		UISetter.SetActive(partIcon, reward.rewardType == ERewardType.UnitMaterial);
		UISetter.SetActive(costumeIcon, reward.rewardType == ERewardType.Costume);
		UISetter.SetActive(ItemIconRoot, reward.rewardType == ERewardType.Item);
		UISetter.SetActive(count, reward.rewardType != ERewardType.Costume && reward.rewardType != ERewardType.Commander);
		UISetter.SetLabel(count, reward.maxCount);
		UISetter.SetActive(vipMark, int.Parse(vipLevel) != 0);
		UISetter.SetLabel(vipBonus, $"VIP {vipLevel} X{vipMultiply}");
		idx = reward.rewardIdx.ToString();
		if (reward.rewardType == ERewardType.Goods || reward.rewardType == ERewardType.Favor || reward.rewardType == ERewardType.CollectionItem || reward.rewardType == ERewardType.Box || reward.rewardType == ERewardType.EventItem)
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[reward.rewardIdx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(goodsDataRow.name));
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
			rType = ETooltipType.Goods;
		}
		else if (reward.rewardType == ERewardType.Medal)
		{
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(medal, commanderDataRow.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow.nickname);
			rType = ETooltipType.Medal;
		}
		else if (reward.rewardType == ERewardType.Commander)
		{
			CommanderDataRow commanderDataRow2 = regulation.commanderDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(commanderIcon, commanderDataRow2.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow2.nickname);
			rType = ETooltipType.Commander;
		}
		else if (reward.rewardType == ERewardType.UnitMaterial)
		{
			PartDataRow partDataRow = regulation.partDtbl[reward.rewardIdx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(partDataRow.name));
			UISetter.SetSprite(partBG, partDataRow.bgResource);
			UISetter.SetSprite(part, partDataRow.serverFieldName);
			UISetter.SetSprite(partMark, partDataRow.markResource);
			UISetter.SetSprite(partGrade, partDataRow.gradeResource);
			rType = ETooltipType.UnitMaterial;
		}
		else if (reward.rewardType == ERewardType.Costume)
		{
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.commanderCostumeDtbl[reward.rewardIdx.ToString()];
			CommanderDataRow commanderDataRow3 = regulation.commanderDtbl[commanderCostumeDataRow.cid.ToString()];
			costumeIcon.atlas = costumeAtlas[commanderCostumeDataRow.atlasNumber - 1];
			UISetter.SetSprite(costumeIcon, commanderDataRow3.resourceId + "_" + commanderCostumeDataRow.skinName);
			UISetter.SetLabel(nickname, Localization.Get(commanderCostumeDataRow.name));
			rType = ETooltipType.Costume;
		}
		else if (reward.rewardType == ERewardType.Item)
		{
			EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == reward.rewardIdx.ToString());
			if (equipItemDataRow != null)
			{
				UISetter.SetSprite(ItemIcon, equipItemDataRow.equipItemIcon);
				UISetter.SetLabel(nickname, Localization.Get(equipItemDataRow.equipItemName));
			}
			rType = ETooltipType.Item;
		}
		else if (reward.rewardType >= ERewardType.GroupEff_1 && reward.rewardType <= ERewardType.GroupEff_8)
		{
			string spriteName = string.Empty;
			if (reward.rewardIdx <= 1000)
			{
				spriteName = "group_buff";
			}
			else if (reward.rewardIdx > 1000)
			{
				spriteName = "group_buff_" + reward.rewardIdx;
			}
			UISetter.SetSprite(goodsIcon, spriteName);
			UISetter.SetActive(count, active: false);
			rType = ETooltipType.Undefined;
		}
	}

	public void Set(ERewardType type, string id, int count)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISprite mb = goodsIcon;
		int active;
		switch (type)
		{
		default:
			active = ((type == ERewardType.Dormitory_Goods) ? 1 : 0);
			break;
		case ERewardType.Goods:
		case ERewardType.Box:
		case ERewardType.Favor:
		case ERewardType.GroupEff_1:
		case ERewardType.GroupEff_2:
		case ERewardType.GroupEff_3:
		case ERewardType.GroupEff_4:
		case ERewardType.GroupEff_5:
		case ERewardType.GroupEff_6:
		case ERewardType.GroupEff_7:
		case ERewardType.GroupEff_8:
			active = 1;
			break;
		}
		UISetter.SetActive(mb, (byte)active != 0);
		UISetter.SetActive(commanderIcon, type == ERewardType.Commander);
		UISetter.SetActive(medalIcon, type == ERewardType.Medal);
		UISetter.SetActive(partIcon, type == ERewardType.UnitMaterial);
		UISetter.SetActive(costumeIcon, type == ERewardType.Costume);
		UISetter.SetActive(ItemIconRoot, type == ERewardType.Item);
		UISetter.SetActive(weaponRoot, type == ERewardType.WeaponItem);
		UISetter.SetActive(this.count, type != ERewardType.Costume && type != ERewardType.Commander);
		UISetter.SetLabel(this.count, count);
		UISetter.SetActive(dormitoryItemIcon, type == ERewardType.Dormitory_NormalDeco || type == ERewardType.Dormitory_AdvancedDeco || type == ERewardType.Dormitory_Wallpaper || type == ERewardType.Dormitory_CostumeBody || type == ERewardType.Dormitory_CostumeHead);
		idx = id;
		switch (type)
		{
		case ERewardType.Goods:
		case ERewardType.Box:
		case ERewardType.Favor:
		case ERewardType.Dormitory_Goods:
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[id];
			UISetter.SetLabel(nickname, Localization.Get(goodsDataRow.name));
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
			rType = ETooltipType.Goods;
			return;
		}
		case ERewardType.Medal:
		{
			CommanderDataRow commanderDataRow3 = regulation.commanderDtbl[id];
			UISetter.SetSprite(medal, commanderDataRow3.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow3.nickname);
			rType = ETooltipType.Medal;
			return;
		}
		case ERewardType.Commander:
		{
			CommanderDataRow commanderDataRow2 = regulation.commanderDtbl[id];
			UISetter.SetSprite(commanderIcon, commanderDataRow2.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow2.nickname);
			rType = ETooltipType.Commander;
			return;
		}
		case ERewardType.UnitMaterial:
		{
			PartDataRow partDataRow = regulation.partDtbl[id];
			UISetter.SetLabel(nickname, Localization.Get(partDataRow.name));
			UISetter.SetSprite(partBG, partDataRow.bgResource);
			UISetter.SetSprite(part, partDataRow.serverFieldName);
			UISetter.SetSprite(partMark, partDataRow.markResource);
			UISetter.SetSprite(partGrade, partDataRow.gradeResource);
			rType = ETooltipType.UnitMaterial;
			return;
		}
		case ERewardType.Costume:
		{
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.commanderCostumeDtbl[id];
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[commanderCostumeDataRow.cid.ToString()];
			costumeIcon.atlas = costumeAtlas[commanderCostumeDataRow.atlasNumber - 1];
			UISetter.SetSprite(costumeIcon, commanderDataRow.resourceId + "_" + commanderCostumeDataRow.skinName);
			UISetter.SetLabel(nickname, Localization.Get(commanderCostumeDataRow.name));
			rType = ETooltipType.Costume;
			return;
		}
		case ERewardType.Item:
		{
			EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == id);
			if (equipItemDataRow != null)
			{
				UISetter.SetSprite(ItemIcon, equipItemDataRow.equipItemIcon);
				UISetter.SetLabel(nickname, Localization.Get(equipItemDataRow.equipItemName));
			}
			rType = ETooltipType.Item;
			return;
		}
		case ERewardType.GroupEff_1:
		case ERewardType.GroupEff_2:
		case ERewardType.GroupEff_3:
		case ERewardType.GroupEff_4:
		case ERewardType.GroupEff_5:
		case ERewardType.GroupEff_6:
		case ERewardType.GroupEff_7:
		case ERewardType.GroupEff_8:
		{
			string spriteName = string.Empty;
			int num = int.Parse(id);
			if (num <= 1000)
			{
				spriteName = "group_buff";
			}
			else if (num > 1000)
			{
				spriteName = "group_buff_" + num;
			}
			UISetter.SetSprite(goodsIcon, spriteName);
			UISetter.SetActive(this.count, active: false);
			rType = ETooltipType.Undefined;
			return;
		}
		}
		switch (type)
		{
		case ERewardType.Dormitory_NormalDeco:
		case ERewardType.Dormitory_AdvancedDeco:
		{
			DormitoryDecorationDataRow dormitoryDecorationDataRow = regulation.dormitoryDecorationDtbl[id];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryTheme_" + dormitoryDecorationDataRow.atlasNumber, dormitoryDecorationDataRow.thumbnail);
			rType = ((type != ERewardType.Dormitory_NormalDeco) ? ETooltipType.Dormitory_AdvancedDeco : ETooltipType.Dormitory_NormalDeco);
			break;
		}
		case ERewardType.Dormitory_Wallpaper:
		{
			DormitoryWallpaperDataRow dormitoryWallpaperDataRow = regulation.dormitoryWallPaperDtbl[id];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryTheme_" + dormitoryWallpaperDataRow.atlasNumber, dormitoryWallpaperDataRow.thumbnail);
			rType = ETooltipType.Dormitory_Wallpaper;
			break;
		}
		case ERewardType.Dormitory_CostumeHead:
		{
			DormitoryHeadCostumeDataRow dormitoryHeadCostumeDataRow = regulation.dormitoryHeadCostumeDtbl[id];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryCostume_" + dormitoryHeadCostumeDataRow.atlasNumber, dormitoryHeadCostumeDataRow.thumbnail);
			rType = ETooltipType.Dormitory_CostumeHead;
			break;
		}
		case ERewardType.Dormitory_CostumeBody:
		{
			DormitoryBodyCostumeDataRow dormitoryBodyCostumeDataRow = regulation.dormitoryBodyCostumeDtbl[id];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryCostume_" + dormitoryBodyCostumeDataRow.atlasNumber, dormitoryBodyCostumeDataRow.thumbnail);
			rType = ETooltipType.Dormitory_CostumeBody;
			break;
		}
		}
	}

	public void Set(GroupInfoDataRow reward)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(goodsIcon, reward.rewardType == ERewardType.Goods || reward.rewardType == ERewardType.Favor || reward.rewardType == ERewardType.CollectionItem || reward.rewardType == ERewardType.Box || (reward.rewardType >= ERewardType.GroupEff_1 && reward.rewardType <= ERewardType.GroupEff_8));
		UISetter.SetActive(commanderIcon, reward.rewardType == ERewardType.Commander);
		UISetter.SetActive(medalIcon, reward.rewardType == ERewardType.Medal);
		UISetter.SetActive(partIcon, reward.rewardType == ERewardType.UnitMaterial);
		UISetter.SetActive(costumeIcon, reward.rewardType == ERewardType.Costume);
		UISetter.SetActive(ItemIconRoot, reward.rewardType == ERewardType.Item);
		UISetter.SetActive(weaponRoot, reward.rewardType == ERewardType.WeaponItem);
		UISetter.SetActive(count, reward.rewardType != ERewardType.Costume && reward.rewardType != ERewardType.Commander && reward.rewardType != ERewardType.WeaponItem);
		UISetter.SetLabel(count, reward.minCount);
		UISetter.SetLabel(vip, $"VIP {reward.typeIndex}");
		idx = reward.rewardIdx.ToString();
		if (reward.rewardType == ERewardType.Goods || reward.rewardType == ERewardType.Favor || reward.rewardType == ERewardType.CollectionItem || reward.rewardType == ERewardType.Box || reward.rewardType == ERewardType.EventItem)
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[reward.rewardIdx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(goodsDataRow.name));
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
			rType = ETooltipType.Goods;
		}
		else if (reward.rewardType == ERewardType.Medal)
		{
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(medal, commanderDataRow.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow.nickname);
			rType = ETooltipType.Medal;
		}
		else if (reward.rewardType == ERewardType.Commander)
		{
			CommanderDataRow commanderDataRow2 = regulation.commanderDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(commanderIcon, commanderDataRow2.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow2.nickname);
			rType = ETooltipType.Commander;
		}
		else if (reward.rewardType == ERewardType.UnitMaterial)
		{
			PartDataRow partDataRow = regulation.partDtbl[reward.rewardIdx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(partDataRow.name));
			UISetter.SetSprite(partBG, partDataRow.bgResource);
			UISetter.SetSprite(part, partDataRow.serverFieldName);
			UISetter.SetSprite(partMark, partDataRow.markResource);
			UISetter.SetSprite(partGrade, partDataRow.gradeResource);
			rType = ETooltipType.UnitMaterial;
		}
		else if (reward.rewardType == ERewardType.Costume)
		{
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.commanderCostumeDtbl[reward.rewardIdx.ToString()];
			CommanderDataRow commanderDataRow3 = regulation.commanderDtbl[commanderCostumeDataRow.cid.ToString()];
			costumeIcon.atlas = costumeAtlas[commanderCostumeDataRow.atlasNumber - 1];
			UISetter.SetSprite(costumeIcon, commanderDataRow3.resourceId + "_" + commanderCostumeDataRow.skinName);
			UISetter.SetLabel(nickname, Localization.Get(commanderCostumeDataRow.name));
			rType = ETooltipType.Costume;
		}
		else if (reward.rewardType == ERewardType.Item)
		{
			EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == reward.rewardIdx.ToString());
			if (equipItemDataRow != null)
			{
				UISetter.SetSprite(ItemIcon, equipItemDataRow.equipItemIcon);
				UISetter.SetLabel(nickname, Localization.Get(equipItemDataRow.equipItemName));
			}
			rType = ETooltipType.Item;
		}
		else if (reward.rewardType >= ERewardType.GroupEff_1 && reward.rewardType <= ERewardType.GroupEff_8)
		{
			string spriteName = string.Empty;
			if (reward.rewardIdx <= 1000)
			{
				spriteName = "group_buff";
			}
			else if (reward.rewardIdx > 1000)
			{
				spriteName = "group_buff_" + reward.rewardIdx;
			}
			UISetter.SetSprite(goodsIcon, spriteName);
			UISetter.SetActive(count, active: false);
			rType = ETooltipType.Undefined;
		}
	}

	public void SetCarnivalItem(CurCarnivalItemInfo itemInfo)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetActive(goodsIcon, itemInfo.type == ERewardType.Goods || itemInfo.type == ERewardType.Favor || itemInfo.type == ERewardType.CollectionItem || itemInfo.type == ERewardType.EventItem);
		UISetter.SetActive(commanderIcon, itemInfo.type == ERewardType.Commander);
		UISetter.SetActive(medalIcon, itemInfo.type == ERewardType.Medal);
		UISetter.SetActive(partIcon, itemInfo.type == ERewardType.UnitMaterial);
		UISetter.SetActive(costumeIcon, itemInfo.type == ERewardType.Costume);
		UISetter.SetActive(weaponRoot, itemInfo.type == ERewardType.WeaponItem);
		UISetter.SetActive(count, itemInfo.type != ERewardType.Costume && itemInfo.type != ERewardType.Commander && itemInfo.type != ERewardType.WeaponItem);
		UISetter.SetActive(ItemIconRoot, itemInfo.type == ERewardType.Item);
		idx = itemInfo.idx;
		if (itemInfo.type == ERewardType.Goods || itemInfo.type == ERewardType.Favor || itemInfo.type == ERewardType.CollectionItem || itemInfo.type == ERewardType.EventItem)
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[idx];
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
			switch (goodsDataRow.gType)
			{
			case EGoods.ChargedGold:
			case EGoods.FreeGold:
				UISetter.SetLabel(count, $"{localUser.gold}/{itemInfo.needCount}");
				break;
			case EGoods.ChargedCash:
			case EGoods.FreeCash:
				UISetter.SetLabel(count, $"{localUser.cash}/{itemInfo.needCount}");
				break;
			default:
				UISetter.SetLabel(count, $"{localUser.resourceList[goodsDataRow.serverFieldName]}/{itemInfo.needCount}");
				break;
			}
			rType = ETooltipType.Goods;
		}
		else if (itemInfo.type == ERewardType.Medal)
		{
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[idx];
			UISetter.SetSprite(medal, commanderDataRow.thumbnailId);
			RoCommander roCommander = localUser.FindCommander(idx);
			if (roCommander != null)
			{
				UISetter.SetLabel(count, $"{roCommander.medal}/{itemInfo.needCount}");
			}
			rType = ETooltipType.Medal;
		}
		else if (itemInfo.type == ERewardType.UnitMaterial)
		{
			PartDataRow partDataRow = regulation.partDtbl[idx];
			UISetter.SetLabel(nickname, partDataRow.name);
			UISetter.SetSprite(partBG, partDataRow.bgResource);
			UISetter.SetSprite(part, partDataRow.serverFieldName);
			UISetter.SetSprite(partMark, partDataRow.markResource);
			UISetter.SetSprite(partGrade, partDataRow.gradeResource);
			RoPart roPart = localUser.FindPart(idx);
			UISetter.SetLabel(count, $"{roPart.count}/{itemInfo.needCount}");
			rType = ETooltipType.UnitMaterial;
		}
		else if (itemInfo.type == ERewardType.Item)
		{
			EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == idx);
			if (equipItemDataRow != null)
			{
				UISetter.SetSprite(ItemIcon, equipItemDataRow.equipItemIcon);
			}
			rType = ETooltipType.Item;
		}
	}

	public void SetCarvnivalItemNeedCount(CurCarnivalItemInfo itemInfo, int exchangeCount)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		switch (itemInfo.type)
		{
		case ERewardType.Goods:
		case ERewardType.Favor:
		case ERewardType.EventItem:
		{
			Regulation regulation = RemoteObjectManager.instance.regulation;
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[itemInfo.idx];
			UISetter.SetLabel(count, $"{localUser.resourceList[goodsDataRow.serverFieldName]}/{itemInfo.needCount * exchangeCount}");
			break;
		}
		case ERewardType.Medal:
		{
			RoCommander roCommander = localUser.FindCommander(idx);
			UISetter.SetLabel(count, $"{roCommander.medal}/{itemInfo.needCount * exchangeCount}");
			break;
		}
		case ERewardType.UnitMaterial:
		{
			RoPart roPart = localUser.FindPart(idx);
			UISetter.SetLabel(count, $"{roPart.count}/{itemInfo.needCount * exchangeCount}");
			break;
		}
		case ERewardType.Commander:
		case ERewardType.Box:
		case ERewardType.Costume:
		case ERewardType.Item:
			break;
		}
	}

	public void SetCarvnivalRewardCount(RewardDataRow reward, int exchangeCount)
	{
		UISetter.SetLabel(count, reward.maxCount * exchangeCount);
	}

	public void setMail(bool message)
	{
		UISetter.SetActive(goodsIcon, active: true);
		if (message)
		{
			UISetter.SetSprite(goodsIcon, "icon_letter");
		}
		else
		{
			UISetter.SetSprite(goodsIcon, "icon_presentbox");
		}
		UISetter.SetActive(commanderIcon, active: false);
		UISetter.SetActive(medalIcon, active: false);
		UISetter.SetActive(partIcon, active: false);
	}

	public void Set(CommanderTrainingTicketDataRow row, RoCommander commander = null)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		int type = (int)row.type;
		GoodsDataRow goodsDataRow = regulation.goodsDtbl[type.ToString()];
		DataTable<CommanderTrainingTicketDataRow> commanderTrainingTicketDtbl = regulation.commanderTrainingTicketDtbl;
		CommanderTrainingTicketDataRow commanderTrainingTicketDataRow = commanderTrainingTicketDtbl[row.type.ToString()];
		UISetter.SetSprite(icon, goodsDataRow.iconId);
		UISetter.SetLabel(count, localUser.resourceList[row.type.ToString()]);
		UISetter.SetLabel(nickname, "EXP+" + commanderTrainingTicketDataRow.exp);
		if (commander != null)
		{
			UISetter.SetActive(receivedBlock, localUser.resourceList[row.type.ToString()] <= 0);
		}
	}

	public void Set(ETooltipType type, string idx)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(goodsRoot, type == ETooltipType.Goods);
		UISetter.SetActive(commanderRoot, type == ETooltipType.Commander);
		UISetter.SetActive(partIcon, type == ETooltipType.UnitMaterial);
		UISetter.SetActive(medalIcon, type == ETooltipType.Medal);
		UISetter.SetActive(costumeIcon, type == ETooltipType.Costume);
		UISetter.SetActive(ItemIconRoot, type == ETooltipType.Item);
		UISetter.SetActive(weaponRoot, type == ETooltipType.WeaponItem);
		UISetter.SetActive(dormitoryItemIcon, type == ETooltipType.Dormitory_NormalDeco || type == ETooltipType.Dormitory_AdvancedDeco || type == ETooltipType.Dormitory_Wallpaper || type == ETooltipType.Dormitory_CostumeBody || type == ETooltipType.Dormitory_CostumeHead);
		string key = string.Empty;
		switch (type)
		{
		case ETooltipType.Goods:
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[idx.ToString()];
			key = goodsDataRow.description;
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
			UISetter.SetLabel(nickname, Localization.Get(goodsDataRow.name));
			break;
		}
		case ETooltipType.Commander:
		{
			CommanderDataRow commanderDataRow4 = regulation.commanderDtbl[idx.ToString()];
			key = commanderDataRow4.explanation;
			UISetter.SetSprite(commanderIcon, commanderDataRow4.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow4.nickname);
			break;
		}
		case ETooltipType.Medal:
		{
			CommanderDataRow commanderDataRow3 = regulation.commanderDtbl[idx.ToString()];
			UISetter.SetSprite(medal, commanderDataRow3.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow3.nickname);
			key = commanderDataRow3.medalExplanation;
			break;
		}
		case ETooltipType.UnitMaterial:
		{
			PartDataRow partDataRow = regulation.partDtbl[idx.ToString()];
			key = partDataRow.description;
			UISetter.SetLabel(nickname, Localization.Get(partDataRow.name));
			UISetter.SetSprite(partBG, partDataRow.bgResource);
			UISetter.SetSprite(part, partDataRow.serverFieldName);
			UISetter.SetSprite(partMark, partDataRow.markResource);
			UISetter.SetSprite(partGrade, partDataRow.gradeResource);
			break;
		}
		case ETooltipType.Costume:
		{
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.commanderCostumeDtbl[idx.ToString()];
			CommanderDataRow commanderDataRow2 = regulation.commanderDtbl[commanderCostumeDataRow.cid.ToString()];
			key = commanderCostumeDataRow.Desc;
			costumeIcon.atlas = costumeAtlas[commanderCostumeDataRow.atlasNumber - 1];
			UISetter.SetSprite(costumeIcon, commanderDataRow2.resourceId + "_" + commanderCostumeDataRow.skinName);
			UISetter.SetLabel(nickname, Localization.Get(commanderCostumeDataRow.name));
			break;
		}
		case ETooltipType.Item:
		{
			EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == idx);
			if (equipItemDataRow != null)
			{
				UISetter.SetSprite(ItemIcon, equipItemDataRow.equipItemIcon);
				UISetter.SetLabel(nickname, Localization.Get(equipItemDataRow.equipItemName));
				key = equipItemDataRow.equipItemSubText;
			}
			break;
		}
		case ETooltipType.WeaponItem:
		{
			WeaponDataRow weaponDataRow = regulation.weaponDtbl.Find((WeaponDataRow row) => row.idx == idx);
			if (weaponDataRow != null)
			{
				RoWeapon roWeapon = RemoteObjectManager.instance.localUser.FindWeapon(weaponDataRow.idx);
				UISetter.SetActive(weaponUseCommander, roWeapon != null && roWeapon.currEquipCommanderId > 0);
				int num = 0;
				if (roWeapon != null && roWeapon.currEquipCommanderId > 0)
				{
					CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[roWeapon.currEquipCommanderId.ToString()];
					UISetter.SetLabel(weaponUseCommander, Localization.Format("70024", Localization.Get(commanderDataRow.nickname)));
				}
				UISetter.SetActive(weaponLevel, num != 0);
				UISetter.SetLabel(weaponLevel, $"+{num}");
				UISetter.SetSprite(weaponIcon, weaponDataRow.weaponIcon);
				UISetter.SetRank(weaponGrade, weaponDataRow.weaponGrade);
				UISetter.SetLabel(nickname, Localization.Get(weaponDataRow.weaponName));
				UISetter.SetSprite(weaponBg, $"weaponback_{weaponDataRow.weaponGrade}");
				key = weaponDataRow.weaponDescription;
				weaponGrade.GetComponent<UIGrid>().Reposition();
				weaponGrade.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
		case ETooltipType.Dormitory_NormalDeco:
		case ETooltipType.Dormitory_AdvancedDeco:
		{
			DormitoryDecorationDataRow dormitoryDecorationDataRow = regulation.dormitoryDecorationDtbl[idx];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryTheme_" + dormitoryDecorationDataRow.atlasNumber, dormitoryDecorationDataRow.thumbnail);
			UISetter.SetLabel(nickname, Localization.Get(dormitoryDecorationDataRow.name));
			key = dormitoryDecorationDataRow.description;
			break;
		}
		case ETooltipType.Dormitory_Wallpaper:
		{
			DormitoryWallpaperDataRow dormitoryWallpaperDataRow = regulation.dormitoryWallPaperDtbl[idx];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryTheme_" + dormitoryWallpaperDataRow.atlasNumber, dormitoryWallpaperDataRow.thumbnail);
			UISetter.SetLabel(nickname, Localization.Get(dormitoryWallpaperDataRow.name));
			key = dormitoryWallpaperDataRow.description;
			break;
		}
		case ETooltipType.Dormitory_CostumeHead:
		{
			DormitoryHeadCostumeDataRow dormitoryHeadCostumeDataRow = regulation.dormitoryHeadCostumeDtbl[idx];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryCostume_" + dormitoryHeadCostumeDataRow.atlasNumber, dormitoryHeadCostumeDataRow.thumbnail);
			UISetter.SetLabel(nickname, Localization.Get(dormitoryHeadCostumeDataRow.name));
			key = dormitoryHeadCostumeDataRow.description;
			break;
		}
		case ETooltipType.Dormitory_CostumeBody:
		{
			DormitoryBodyCostumeDataRow dormitoryBodyCostumeDataRow = regulation.dormitoryBodyCostumeDtbl[idx];
			UISetter.SetSprite(dormitoryItemIcon, "DormitoryCostume_" + dormitoryBodyCostumeDataRow.atlasNumber, dormitoryBodyCostumeDataRow.thumbnail);
			UISetter.SetLabel(nickname, Localization.Get(dormitoryBodyCostumeDataRow.name));
			key = dormitoryBodyCostumeDataRow.description;
			break;
		}
		}
		UISetter.SetLabel(description, Localization.Get(key));
	}

	public void SetRandomCarnivalReward(RewardDataRow reward)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(goodsIcon, reward.rewardType == ERewardType.Goods || reward.rewardType == ERewardType.Favor || reward.rewardType == ERewardType.Box || (reward.rewardType >= ERewardType.GroupEff_1 && reward.rewardType <= ERewardType.GroupEff_8) || reward.rewardType == ERewardType.EventItem);
		UISetter.SetActive(commanderIcon, reward.rewardType == ERewardType.Commander);
		UISetter.SetActive(medalIcon, reward.rewardType == ERewardType.Medal);
		UISetter.SetActive(partIcon, reward.rewardType == ERewardType.UnitMaterial);
		UISetter.SetActive(costumeIcon, reward.rewardType == ERewardType.Costume);
		UISetter.SetActive(ItemIconRoot, reward.rewardType == ERewardType.Item);
		UISetter.SetActive(count, reward.rewardType != ERewardType.Costume && reward.rewardType != ERewardType.Commander);
		UISetter.SetLabel(count, $"{reward.minCount}~{reward.maxCount}");
		idx = reward.rewardIdx.ToString();
		if (reward.rewardType == ERewardType.Goods || reward.rewardType == ERewardType.Favor || reward.rewardType == ERewardType.CollectionItem || reward.rewardType == ERewardType.Box || reward.rewardType == ERewardType.EventItem)
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[reward.rewardIdx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(goodsDataRow.name));
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
			rType = ETooltipType.Goods;
		}
		else if (reward.rewardType == ERewardType.Medal)
		{
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(medal, commanderDataRow.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow.nickname);
			rType = ETooltipType.Medal;
		}
		else if (reward.rewardType == ERewardType.Commander)
		{
			CommanderDataRow commanderDataRow2 = regulation.commanderDtbl[reward.rewardIdx.ToString()];
			UISetter.SetSprite(commanderIcon, commanderDataRow2.thumbnailId);
			UISetter.SetLabel(nickname, commanderDataRow2.nickname);
			rType = ETooltipType.Commander;
		}
		else if (reward.rewardType == ERewardType.UnitMaterial)
		{
			PartDataRow partDataRow = regulation.partDtbl[reward.rewardIdx.ToString()];
			UISetter.SetLabel(nickname, Localization.Get(partDataRow.name));
			UISetter.SetSprite(partBG, partDataRow.bgResource);
			UISetter.SetSprite(part, partDataRow.serverFieldName);
			UISetter.SetSprite(partMark, partDataRow.markResource);
			UISetter.SetSprite(partGrade, partDataRow.gradeResource);
			rType = ETooltipType.UnitMaterial;
		}
		else if (reward.rewardType == ERewardType.Costume)
		{
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.commanderCostumeDtbl[reward.rewardIdx.ToString()];
			CommanderDataRow commanderDataRow3 = regulation.commanderDtbl[commanderCostumeDataRow.cid.ToString()];
			costumeIcon.atlas = costumeAtlas[commanderCostumeDataRow.atlasNumber - 1];
			UISetter.SetSprite(costumeIcon, commanderDataRow3.resourceId + "_" + commanderCostumeDataRow.skinName);
			UISetter.SetLabel(nickname, Localization.Get(commanderCostumeDataRow.name));
			rType = ETooltipType.Costume;
		}
		else if (reward.rewardType == ERewardType.Item)
		{
			EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == reward.rewardIdx.ToString());
			if (equipItemDataRow != null)
			{
				UISetter.SetSprite(ItemIcon, equipItemDataRow.equipItemIcon);
				UISetter.SetLabel(nickname, Localization.Get(equipItemDataRow.equipItemName));
			}
			rType = ETooltipType.Item;
		}
		else if (reward.rewardType >= ERewardType.GroupEff_1 && reward.rewardType <= ERewardType.GroupEff_8)
		{
			string spriteName = string.Empty;
			if (reward.rewardIdx <= 1000)
			{
				spriteName = "group_buff";
			}
			else if (reward.rewardIdx > 1000)
			{
				spriteName = "group_buff_" + reward.rewardIdx;
			}
			UISetter.SetSprite(goodsIcon, spriteName);
			UISetter.SetActive(count, active: false);
			rType = ETooltipType.Undefined;
		}
	}

	public virtual void OnClick()
	{
		if (rType != 0 && !string.IsNullOrEmpty(idx))
		{
			UITooltip.Show(rType, idx);
		}
	}

	public void CloseTooltip()
	{
		UITooltip.Hide();
	}

	public void Set(Protocols.VipGacha.VipGachaInfo gachaInfo)
	{
		if (gachaInfo == null)
		{
			return;
		}
		if (gachaInfo.rewardRate > 0)
		{
			UISetter.SetActive(vipGacha_soldOut, active: false);
		}
		else
		{
			UISetter.SetActive(vipGacha_soldOut, active: true);
		}
		UISetter.SetLabel(GetAmount, "x" + gachaInfo.rewardCount);
		Regulation regulation = RemoteObjectManager.instance.regulation;
		idx = gachaInfo.rewardIdx.ToString();
		UISetter.SetActive(goodsIcon, gachaInfo.rewardType == 1);
		UISetter.SetActive(commanderIcon, gachaInfo.rewardType == 3);
		UISetter.SetActive(medalIcon, gachaInfo.rewardType == 2);
		UISetter.SetActive(partIcon, gachaInfo.rewardType == 5);
		UISetter.SetActive(ItemIconRoot, gachaInfo.rewardType == 8);
		UISetter.SetLabel(probability, $"{gachaInfo.rewardPoint}%");
		switch ((ERewardType)gachaInfo.rewardType)
		{
		case ERewardType.Commander:
		{
			CommanderDataRow commanderDataRow = regulation.commanderDtbl[idx];
			UISetter.SetSprite(commanderIcon, commanderDataRow.thumbnailId);
			rType = ETooltipType.Commander;
			break;
		}
		case ERewardType.Medal:
		{
			CommanderDataRow commanderDataRow2 = regulation.commanderDtbl[idx];
			UISetter.SetSprite(medal, commanderDataRow2.thumbnailId);
			rType = ETooltipType.Medal;
			break;
		}
		case ERewardType.Goods:
		{
			UISetter.SetActive(goodsIcon, active: true);
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[idx];
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
			rType = ETooltipType.Goods;
			break;
		}
		case ERewardType.Item:
		{
			EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == idx);
			if (equipItemDataRow != null)
			{
				UISetter.SetSprite(ItemIcon, equipItemDataRow.equipItemIcon);
			}
			rType = ETooltipType.Item;
			break;
		}
		case ERewardType.Box:
		case ERewardType.UnitMaterial:
		case ERewardType.Favor:
		case ERewardType.Costume:
			break;
		}
	}
}
