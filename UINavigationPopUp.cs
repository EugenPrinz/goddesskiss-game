using System.Collections;
using Shared.Regulation;
using UnityEngine;

public class UINavigationPopUp : UIPopup
{
	public UIDefaultListView naviListView;

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public GameObject Medal;

	public GameObject Part;

	public GameObject Goods;

	public GameObject Costume;

	public UILabel itemName;

	public UILabel itemCount;

	public UISprite medalIcon;

	public UISprite partIcon;

	public UISprite partBG;

	public UISprite goodsIcon;

	public UISprite costumeIcon;

	public UIAtlas[] costumeAtlas;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
	}

	public void Init(EStorageType type, string idx, ItemExchangeDataRow item)
	{
		UISetter.SetActive(Medal, type == EStorageType.Medal);
		UISetter.SetActive(Part, type == EStorageType.Part);
		UISetter.SetActive(Goods, type == EStorageType.Food || type == EStorageType.Goods || type == EStorageType.CollectionItem);
		UISetter.SetActive(Costume, type == EStorageType.Costume);
		switch (type)
		{
		case EStorageType.Part:
		{
			PartDataRow partDataRow = base.regulation.FindPartData(idx);
			RoPart roPart = base.localUser.FindPart(idx);
			UISetter.SetLabel(itemName, Localization.Get(partDataRow.name));
			UISetter.SetSprite(partBG, partDataRow.bgResource);
			UISetter.SetSprite(partIcon, partDataRow.serverFieldName);
			UISetter.SetLabel(itemCount, string.Format("{0} : {1} / {2}", Localization.Get("5609"), roPart.count, partDataRow.max));
			break;
		}
		case EStorageType.Medal:
		{
			CommanderDataRow commander = base.regulation.GetCommander(idx);
			RoCommander roCommander = base.localUser.FindCommander(idx);
			CommanderRankDataRow commanderRankDataRow = base.regulation.FindCommanderRankData((int)roCommander.rank + 1);
			UISetter.SetSprite(medalIcon, commander.thumbnailId);
			UISetter.SetLabel(itemName, commander.nickname);
			UISetter.SetLabel(itemCount, string.Format("{0} : {1} / {2}", Localization.Get("5609"), roCommander.medal, (commanderRankDataRow == null) ? Localization.Get("1309") : roCommander.maxMedal.ToString()));
			break;
		}
		case EStorageType.Goods:
		case EStorageType.Food:
		case EStorageType.CollectionItem:
		{
			GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.FindGoodsServerFieldName(idx);
			UISetter.SetSprite(goodsIcon, goodsDataRow.iconId);
			UISetter.SetLabel(itemName, Localization.Get(goodsDataRow.name));
			UISetter.SetLabel(itemCount, string.Format("{0} : {1} / {2}", Localization.Get("5609"), base.localUser.resourceList[goodsDataRow.serverFieldName], goodsDataRow.max));
			break;
		}
		case EStorageType.Costume:
		{
			UISetter.SetLabel(itemCount, string.Empty);
			CommanderCostumeDataRow commanderCostumeDataRow = base.regulation.commanderCostumeDtbl[idx];
			CommanderDataRow commanderDataRow = base.regulation.commanderDtbl[commanderCostumeDataRow.cid.ToString()];
			costumeIcon.atlas = costumeAtlas[commanderCostumeDataRow.atlasNumber - 1];
			UISetter.SetSprite(costumeIcon, commanderDataRow.resourceId + "_" + commanderCostumeDataRow.skinName);
			UISetter.SetLabel(itemName, Localization.Get(commanderCostumeDataRow.name));
			break;
		}
		}
		naviListView.Init(item.navigatorInfo(), "Navi_");
	}

	public void GetCommanderStage(string id)
	{
		UISetter.SetActive(base.uiWorld.mainCommand.sideMenu, active: false);
		RoWorldMap roWorldMap = base.localUser.FindWorldMapByStage(id);
		base.uiWorld.worldMap.InitAndOpenWorldMap(roWorldMap.id, id);
	}

	public void GoWorldMap(string mapId)
	{
		UISetter.SetActive(base.uiWorld.mainCommand.sideMenu, active: false);
		base.uiWorld.worldMap.InitAndOpenWorldMap(mapId);
	}

	public void GoBuilding(ENavigatorType dest)
	{
		UITranscendencePopup uITranscendencePopup = Object.FindObjectOfType(typeof(UITranscendencePopup)) as UITranscendencePopup;
		if (uITranscendencePopup != null)
		{
			uITranscendencePopup.Close();
		}
		if (base.uiWorld.existCommanderDetail && base.uiWorld.commanderDetail.gameObject.activeSelf)
		{
			base.uiWorld.commanderDetail.ClosePopup();
		}
		if (base.uiWorld.existHeadQuarters && base.uiWorld.headQuarters.gameObject.activeSelf)
		{
			base.uiWorld.headQuarters.ClosePopUp();
		}
		if (base.uiWorld.existGroup && base.uiWorld.group.gameObject.activeSelf)
		{
			base.uiWorld.group.ClosePopup();
		}
		switch (dest)
		{
		case ENavigatorType.Challenge:
			base.uiWorld.camp.GoNavigation("Duel");
			break;
		case ENavigatorType.Raid:
			base.uiWorld.camp.GoNavigation("Raid");
			break;
		case ENavigatorType.GuildShop:
			base.uiWorld.camp.GoBlackMarket(EShopType.GuildShop);
			break;
		case ENavigatorType.WarHome:
			base.uiWorld.camp.GoNavigation("WarMemorial");
			break;
		case ENavigatorType.Gacha:
			base.uiWorld.camp.GoNavigation("Gacha");
			break;
		case ENavigatorType.Annihilation:
			base.uiWorld.camp.GoNavigation("Loot");
			break;
		case ENavigatorType.DailyBattle:
			base.uiWorld.camp.GoNavigation("Situation");
			break;
		case ENavigatorType.Shop:
			base.uiWorld.camp.GoNavigation("BlackMarket");
			break;
		case ENavigatorType.LastStageMap:
			base.uiWorld.camp.GoNavigation("LastStage");
			break;
		}
		if (dest == ENavigatorType.WaveDuel)
		{
			base.uiWorld.camp.GoNavigation("WaveDuel");
		}
		if (dest == ENavigatorType.HeadQuarter)
		{
			base.uiWorld.camp.GoNavigation("HeadQuarter");
		}
		if (dest == ENavigatorType.Carnival)
		{
			base.uiWorld.camp.GoNavigation("Carnival");
		}
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "Close")
		{
			SoundManager.PlaySFX("BTN_Negative_001");
			ClosePopup();
		}
		else
		{
			if (!naviListView.Contains(text))
			{
				return;
			}
			SoundManager.PlaySFX("BTN_Norma_001");
			string pureId = naviListView.GetPureId(text);
			if (pureId.StartsWith(ENavigatorType.Stage.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5767"));
					return;
				}
				string id = pureId.Substring((ENavigatorType.Stage.ToString() + "_").Length);
				GetCommanderStage(id);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.Challenge.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5897"));
					return;
				}
				GoBuilding(ENavigatorType.Challenge);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.Raid.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5897"));
					return;
				}
				GoBuilding(ENavigatorType.Raid);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.GuildShop.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5897"));
					return;
				}
				GoBuilding(ENavigatorType.GuildShop);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.WarHome.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5897"));
					return;
				}
				GoBuilding(ENavigatorType.WarHome);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.Gacha.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5897"));
					return;
				}
				GoBuilding(ENavigatorType.Gacha);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.Event.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5897"));
					return;
				}
				GoBuilding(ENavigatorType.Event);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.DailyBattle.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5897"));
					return;
				}
				GoBuilding(ENavigatorType.DailyBattle);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.Shop.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5897"));
					return;
				}
				GoBuilding(ENavigatorType.Shop);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.Annihilation.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5897"));
					return;
				}
				GoBuilding(ENavigatorType.Annihilation);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.StageGroup.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1345"));
					return;
				}
				string mapId = pureId.Substring((ENavigatorType.StageGroup.ToString() + "_").Length);
				GoWorldMap(mapId);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.LastStageMap.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("1345"));
					return;
				}
				GoBuilding(ENavigatorType.LastStageMap);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.WaveDuel.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5897"));
					return;
				}
				GoBuilding(ENavigatorType.WaveDuel);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.HeadQuarter.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5897"));
					return;
				}
				GoBuilding(ENavigatorType.HeadQuarter);
				base.OnClick(sender);
			}
			else if (pureId.StartsWith(ENavigatorType.Carnival.ToString() + "_"))
			{
				if (sender.transform.Find("MoveBtn").GetComponent<UIButton>().isGray)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5897"));
					return;
				}
				GoBuilding(ENavigatorType.Carnival);
				base.OnClick(sender);
			}
		}
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
