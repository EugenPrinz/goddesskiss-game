using System.Collections;
using Shared.Regulation;
using UnityEngine;

public class UIGachaBox : UIItemBase
{
	public UIAtlas[] costumeAtlas;

	public GameObject backlight;

	public UISprite goodsIcon;

	public GameObject materialIcon;

	public UISprite materialBg;

	public UISprite material;

	public GameObject medalIcon;

	public UISprite medal;

	public UISprite medalMark;

	public UISprite background;

	public UISprite costumeIcon;

	public UILabel goodsCount;

	public UISprite commanderIcon;

	public GameObject goodsRoot;

	public GameObject ItemRoot;

	public UISprite itemIcon;

	public Animation openAnimation;

	public string moveAnimationName;

	public BoxCollider collider;

	public GameObject cardEffect;

	private ETooltipType rType;

	private string idx;

	private CommanderCompleteType getType;

	private bool isTouch;

	public void Set(EGachaAnimationType gachaType, GoodsDataRow goods, int count)
	{
		ResetCard();
		SetBackgroundCard(gachaType);
		UISetter.SetActive(costumeIcon, active: false);
		UISetter.SetActive(commanderIcon, active: false);
		UISetter.SetActive(goodsIcon, active: true);
		UISetter.SetActive(materialIcon, active: false);
		UISetter.SetActive(medalIcon, active: false);
		UISetter.SetActive(ItemRoot, active: false);
		UISetter.SetSprite(goodsIcon, goods.iconId);
		UISetter.SetLabel(goodsCount, "x" + count);
		idx = goods.type;
		rType = ETooltipType.Goods;
	}

	public void Set(EGachaAnimationType gachaType, PartDataRow part, int count)
	{
		ResetCard();
		SetBackgroundCard(gachaType);
		UISetter.SetActive(costumeIcon, active: false);
		UISetter.SetActive(commanderIcon, active: false);
		UISetter.SetActive(goodsIcon, active: false);
		UISetter.SetActive(materialIcon, active: true);
		UISetter.SetActive(medalIcon, active: false);
		UISetter.SetSprite(materialBg, part.bgResource);
		UISetter.SetSprite(material, part.serverFieldName);
		UISetter.SetActive(ItemRoot, active: false);
		UISetter.SetLabel(goodsCount, "x" + count);
		idx = part.type;
		rType = ETooltipType.UnitMaterial;
	}

	public void Set(EGachaAnimationType gachaType, CommanderDataRow commander)
	{
		ResetCard();
		SetBackgroundCard(gachaType);
		UISetter.SetActive(costumeIcon, active: false);
		UISetter.SetActive(commanderIcon, active: true);
		UISetter.SetActive(medalIcon, active: false);
		UISetter.SetActive(goodsIcon, active: false);
		UISetter.SetActive(materialIcon, active: false);
		UISetter.SetSprite(commanderIcon, commander.thumbnailId);
		UISetter.SetLabel(goodsCount, string.Empty);
		UISetter.SetActive(ItemRoot, active: false);
		idx = commander.id;
		rType = ETooltipType.Commander;
	}

	public void Set(EGachaAnimationType gachaType, CommanderCostumeDataRow costumeData)
	{
		ResetCard();
		SetBackgroundCard(gachaType);
		CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[costumeData.cid.ToString()];
		UISetter.SetActive(costumeIcon, active: true);
		UISetter.SetActive(commanderIcon, active: false);
		UISetter.SetActive(goodsIcon, active: false);
		UISetter.SetActive(medalIcon, active: false);
		UISetter.SetActive(materialIcon, active: false);
		UISetter.SetActive(ItemRoot, active: false);
		costumeIcon.atlas = costumeAtlas[costumeData.atlasNumber - 1];
		UISetter.SetSprite(costumeIcon, commanderDataRow.resourceId + "_" + costumeData.skinName);
		UISetter.SetLabel(goodsCount, string.Empty);
		idx = costumeData.ctid.ToString();
		rType = ETooltipType.Costume;
	}

	public void Set(UIGacha.BoxData data)
	{
		ResetCard();
		SetBackgroundCard(data.gachaType);
		UISetter.SetActive(costumeIcon, active: false);
		UISetter.SetActive(goodsIcon, active: false);
		UISetter.SetActive(materialIcon, active: false);
		UISetter.SetActive(ItemRoot, active: false);
		UISetter.SetActive(commanderIcon, data.rewardType == ERewardType.Commander);
		UISetter.SetActive(medalIcon, data.rewardType == ERewardType.Medal);
		UISetter.SetActive(goodsCount, data.getCommanderMedal > 0);
		UISetter.SetSprite(medal, data.commanderData.thumbnailId);
		UISetter.SetSprite(commanderIcon, data.commanderData.thumbnailId);
		UISetter.SetLabel(goodsCount, "x" + data.getCommanderMedal);
		idx = data.commanderData.id;
		rType = ((data.rewardType != ERewardType.Commander) ? ETooltipType.Medal : ETooltipType.Commander);
	}

	public void Set(EGachaAnimationType gachaType, EquipItemDataRow item, int count)
	{
		ResetCard();
		SetBackgroundCard(gachaType);
		UISetter.SetActive(costumeIcon, active: false);
		UISetter.SetActive(commanderIcon, active: false);
		UISetter.SetActive(goodsIcon, active: false);
		UISetter.SetActive(materialIcon, active: false);
		UISetter.SetActive(medalIcon, active: false);
		UISetter.SetLabel(goodsCount, "x" + count);
		UISetter.SetActive(ItemRoot, active: true);
		UISetter.SetSprite(itemIcon, item.equipItemIcon);
		idx = item.key;
		rType = ETooltipType.Item;
	}

	private void SetBackgroundCard(EGachaAnimationType gachaType)
	{
		switch (gachaType)
		{
		case EGachaAnimationType.Normal:
			UISetter.SetSprite(background, "gacha-bronzecard");
			break;
		case EGachaAnimationType.Premium:
			UISetter.SetSprite(background, "gacha-silvercard");
			break;
		default:
			UISetter.SetSprite(background, "gacha-goldcard");
			break;
		}
	}

	public IEnumerator GetCommander()
	{
		if (rType == ETooltipType.Commander)
		{
			UISetter.SetActive(cardEffect, active: true);
			yield return new WaitForSeconds(0.2f);
			UICommanderComplete uiComplete = UIManager.instance.world.gacha.CreateCommanderComplete(getType, idx);
			while (uiComplete != null)
			{
				yield return null;
			}
		}
		UISetter.SetActive(background, active: false);
		UISetter.SetActive(backlight, active: true);
	}

	public void BoxOpen()
	{
		if (background.isActiveAndEnabled)
		{
			StartCoroutine(GetCommander());
		}
	}

	public void GetType(CommanderCompleteType type)
	{
		getType = type;
	}

	public void OnClick()
	{
		if (rType != 0 && !string.IsNullOrEmpty(idx))
		{
			UITooltip.Show(rType, idx);
		}
	}

	private void ResetCard()
	{
		isTouch = false;
		UISetter.SetActive(background, active: true);
		UISetter.SetAlpha(background, 1f);
		UISetter.SetActive(backlight, active: false);
		UISetter.SetCollider(collider, active: false);
		UISetter.SetActive(cardEffect, active: false);
		background.transform.localPosition = new Vector3(0f, 2f, 0f);
	}

	public void CloseTooltip()
	{
		UITooltip.Hide();
	}

	public void OnTouchStart()
	{
		if (rType == ETooltipType.Commander)
		{
			UISetter.SetActive(cardEffect, active: true);
		}
	}

	public void OnTouchEnd()
	{
		StartCoroutine(GetCommander());
	}
}
