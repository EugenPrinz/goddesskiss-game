using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UILaboratory : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIDefaultListView targetListView;

	public UISpineAnimation spineAnimation;

	public List<UILabel> mainItemCount;

	public List<UILabel> commonItemCount;

	[SerializeField]
	private GameObject upgrade;

	[SerializeField]
	private GameObject decomposition;

	[SerializeField]
	private GameObject UpgradeBtnLock;

	[SerializeField]
	private GameObject DecompositionBtnLock;

	public UITexture goBG;

	[HideInInspector]
	public UIHaveItemListPopup haveItemListPopup;

	[SerializeField]
	private UILabel UpgradeNeedGoldValue;

	[SerializeField]
	private UILabel decompositionNeedGoldValue;

	[SerializeField]
	private List<UILabel> decompositionCommonItem;

	[SerializeField]
	private UILabel decompositionMainItemCount;

	[SerializeField]
	private UISprite decompositionMainItemSlot;

	[SerializeField]
	private UISprite decompositionMainItemIcon;

	private int needGold;

	[HideInInspector]
	public RoItem currSelectItem;

	[SerializeField]
	private GameObject RightNowBtn;

	[SerializeField]
	private UISprite decompositionItemImg;

	[SerializeField]
	private UISprite upgradeItemImg;

	[SerializeField]
	private UILabel decompositionItemLevel;

	[SerializeField]
	private UILabel upgradeItemLevel;

	[SerializeField]
	private UIFlipSwitch UpgradeTab;

	[SerializeField]
	private UIFlipSwitch DecompositionTab;

	[SerializeField]
	private GameObject emptySlot;

	public UIEquipPossibleCommanderListPopup possiblePopup;

	[SerializeField]
	private UILabel UpgradeSlotTxt;

	[SerializeField]
	private UILabel DecompositionSlotTxt;

	[SerializeField]
	private UILabel DecompositionCountLabel;

	[HideInInspector]
	public CurTabType laboratoryTabType;

	[HideInInspector]
	public int decompositionCount;

	public UILaboratoryEffect laboratoryEff;

	private UISecretShopPopup secretShop;

	public GameObject btnHeadQuarters;

	private UIWebviewPopup infoPopUp;

	private string infoUrl
	{
		get
		{
			string language = Localization.language;
			language = language.Substring(2, language.Length - 2).ToLower();
			return $"http://gkcdn.flerogames.com/guide/android/guide_ooparts.html#{language}";
		}
	}

	private new void Awake()
	{
		UISetter.SetSpine(spineAnimation, "n_005");
		Renderer[] componentsInChildren = laboratoryEff.GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.transform.localScale = new Vector3((float)Screen.width / ((float)Screen.height / 9f) / 16f, 1f, (float)Screen.width / ((float)Screen.height / 9f) / 16f);
		}
		base.Awake();
	}

	private void OnDestroy()
	{
		if (goBG != null)
		{
			goBG = null;
		}
		if (AnimBlock != null)
		{
			AnimBlock = null;
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		switch (sender.name)
		{
		case "Tab_upgrade":
			laboratoryTabType = CurTabType.Upgrade;
			currSelectItem = null;
			UISetter.SetActive(upgrade, active: true);
			UISetter.SetActive(decomposition, active: false);
			SetUpgradeItems();
			break;
		case "Tab_decomposition":
			laboratoryTabType = CurTabType.Decomposition;
			currSelectItem = null;
			UISetter.SetActive(upgrade, active: false);
			UISetter.SetActive(decomposition, active: true);
			SetDecompositionItems();
			break;
		case "Btn_upgrade":
			if (base.localUser.gold < needGold)
			{
				UISimplePopup.CreateBool(localization: true, "1029", "1030", null, "1001", "1000").onClick = delegate(GameObject go)
				{
					string text4 = go.name;
					if (text4 == "OK")
					{
						base.uiWorld.camp.GoNavigation("MetroBank");
					}
				};
			}
			else
			{
				int num = 0;
				num = ((currSelectItem.currEquipCommanderId != null) ? int.Parse(currSelectItem.currEquipCommanderId) : 0);
				base.network.RequestUpgradeItemEquipment(int.Parse(currSelectItem.id), num, currSelectItem.level);
			}
			break;
		case "Btn_Equip":
			if (currSelectItem != null)
			{
				if (possiblePopup == null)
				{
					return;
				}
				UISetter.SetActive(possiblePopup, active: true);
				possiblePopup.InitPossibleCommanderList(currSelectItem);
			}
			break;
		case "Btn_decomposition":
			if (base.localUser.gold < needGold)
			{
				UISimplePopup.CreateBool(localization: true, "1029", "1030", null, "1001", "1000").onClick = delegate(GameObject go)
				{
					string text3 = go.name;
					if (text3 == "OK")
					{
						base.uiWorld.camp.GoNavigation("MetroBank");
					}
				};
			}
			else
			{
				if (currSelectItem == null)
				{
					break;
				}
				UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), string.Format(Localization.Get("5030424"), string.Format("{0} {1}", Localization.Format("1021", currSelectItem.level), Localization.Get(currSelectItem.nameIdx))), null, Localization.Get("1001"), Localization.Get("1000")).onClick = delegate(GameObject go)
				{
					string text = go.name;
					if (text == "OK")
					{
						if (IsOverMaxCount())
						{
							UISimplePopup.CreateBool(localization: true, "1303", "5476", "5477", "1304", "1305").onClick = delegate(GameObject godisassemble)
							{
								string text2 = godisassemble.name;
								if (text2 == "OK")
								{
									base.network.RequestDecompositionItemEquipment(int.Parse(currSelectItem.id), currSelectItem.level, decompositionCount);
								}
							};
						}
						else
						{
							base.network.RequestDecompositionItemEquipment(int.Parse(currSelectItem.id), currSelectItem.level, decompositionCount);
						}
					}
				};
			}
			break;
		case "selectItemSlot":
			CreateHaveItemListPopup();
			break;
		case "Close":
			Close();
			break;
		case "InfoBtn":
			if (infoPopUp == null)
			{
				infoPopUp = UIPopup.Create<UIWebviewPopup>("UIHelpWebView");
				infoPopUp.Init(infoUrl);
			}
			break;
		case "ShopBtn":
			if (secretShop == null)
			{
				secretShop = UIPopup.Create<UISecretShopPopup>("SecretShopPopup");
				secretShop.Init(EShopType.WaveDuelShop);
			}
			break;
		case "Btn_HeadQuarter":
		{
			string curSelectCommanderId_forLaboratory = base.localUser.curSelectCommanderId_forLaboratory;
			Close();
			base.uiWorld.camp.GoNavigation("CommanderDetail", curSelectCommanderId_forLaboratory, "Oparts");
			break;
		}
		}
		base.OnClick(sender);
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		if (secretShop != null)
		{
			secretShop.OnRefresh();
		}
		SetUpgradeItems();
		SetDecompositionItems();
		UISetter.SetActive(RightNowBtn, currSelectItem != null && !currSelectItem.isEquip);
	}

	public void InitAndOpen()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			Open();
			OpenPopupShow();
			if (base.localUser.isGoLaboratory)
			{
				currSelectItem = base.localUser.curSelectItem_forLaboratory;
			}
			UISetter.SetActive(btnHeadQuarters, base.localUser.isGoLaboratory);
			UISetter.SetActive(RightNowBtn, currSelectItem != null && !currSelectItem.isEquip);
			UISetter.SetFlipSwitch(UpgradeTab, state: true);
			UISetter.SetFlipSwitch(DecompositionTab, state: false);
			UISetter.SetActive(upgrade, active: true);
			UISetter.SetActive(decomposition, active: false);
			laboratoryTabType = CurTabType.Upgrade;
			laboratoryEff.OffEffect();
			SetUpgradeItems();
			decompositionCount = 0;
		}
	}

	public void SetEffect(EItemStatType itemType)
	{
		laboratoryEff.SetEffect(laboratoryTabType, itemType);
	}

	public void OpenPopupShow()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Open()
	{
	}

	public override void Close()
	{
		bBackKeyEnable = true;
		base.localUser.isGoLaboratory = false;
		base.localUser.curSelectItem_forLaboratory = null;
		base.localUser.curSelectCommanderId_forLaboratory = string.Empty;
		laboratoryTabType = CurTabType.None;
		currSelectItem = null;
		if (infoPopUp != null)
		{
			infoPopUp.Close();
		}
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
		if (secretShop != null)
		{
			secretShop.Close();
		}
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
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
		AnimBG.Reset();
		AnimNpc.Reset();
		AnimTitle.Reset();
		AnimBlock.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public void AnimOpenFinish()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
	}

	protected override void OnEnablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: true);
	}

	protected override void OnDisablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: false);
	}

	public void SetUpgradeItems()
	{
		if (!upgrade.activeSelf)
		{
			return;
		}
		ResetCommonItemCount();
		int num = 0;
		int num2 = 0;
		if (currSelectItem == null)
		{
			UISetter.SetActive(upgradeItemImg, active: false);
			UISetter.SetActive(upgradeItemLevel, active: false);
			UISetter.SetLabel(UpgradeNeedGoldValue, 0);
			EquipItemUpgradeDataRow equipItemUpgradeDataRow = RemoteObjectManager.instance.regulation.FindUpgradeItemInfo(EItemStatType.ATK, 1);
			UISetter.SetLabel(commonItemCount[0], $"{base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial2).count}/{0}");
			UISetter.SetLabel(commonItemCount[1], $"{base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial3).count}/{0}");
			UISetter.SetLabel(commonItemCount[2], $"{base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial4).count}/{0}");
			UISetter.SetLabel(commonItemCount[3], $"{base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial5).count}/{0}");
			EquipItemUpgradeDataRow equipItemUpgradeDataRow2 = RemoteObjectManager.instance.regulation.FindUpgradeItemInfo(EItemStatType.ACCUR, 1);
			EquipItemUpgradeDataRow equipItemUpgradeDataRow3 = RemoteObjectManager.instance.regulation.FindUpgradeItemInfo(EItemStatType.DEF, 1);
			EquipItemUpgradeDataRow equipItemUpgradeDataRow4 = RemoteObjectManager.instance.regulation.FindUpgradeItemInfo(EItemStatType.LUCK, 1);
			UISetter.SetLabel(mainItemCount[0], $"{base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial1).count}/{0}");
			UISetter.SetLabel(mainItemCount[1], $"{base.localUser.FindPart(equipItemUpgradeDataRow3.upgradeMaterial1).count}/{0}");
			UISetter.SetLabel(mainItemCount[2], $"{base.localUser.FindPart(equipItemUpgradeDataRow2.upgradeMaterial1).count}/{0}");
			UISetter.SetLabel(mainItemCount[3], $"{base.localUser.FindPart(equipItemUpgradeDataRow4.upgradeMaterial1).count}/{0}");
			UISetter.SetActive(UpgradeBtnLock, active: true);
			UISetter.SetActive(UpgradeSlotTxt, active: true);
			UISetter.SetActive(RightNowBtn, active: false);
			laboratoryEff.OffEffect();
			return;
		}
		EquipItemUpgradeDataRow equipItemUpgradeDataRow5 = RemoteObjectManager.instance.regulation.FindUpgradeItemInfo(currSelectItem.upgradeType, currSelectItem.level);
		EquipItemDataRow equipItemDataRow = RemoteObjectManager.instance.regulation.equipItemDtbl.Find((EquipItemDataRow itemRow) => itemRow.key == currSelectItem.id);
		UISetter.SetActive(UpgradeSlotTxt, active: false);
		UISetter.SetActive(upgradeItemImg, active: true);
		UISetter.SetActive(upgradeItemLevel, active: true);
		UISetter.SetSprite(upgradeItemImg, equipItemDataRow.equipItemIcon);
		UISetter.SetLabel(upgradeItemLevel, string.Format(Localization.Get("1021"), currSelectItem.level));
		for (int i = 0; i < mainItemCount.Count; i++)
		{
			if (currSelectItem != null && currSelectItem.upgradeType == (EItemStatType)(i + 1))
			{
				RoPart roPart = base.localUser.FindPart(equipItemUpgradeDataRow5.upgradeMaterial1);
				num = equipItemUpgradeDataRow5.upgradeMaterial1Volume;
				num2 = roPart.count;
				needGold = equipItemUpgradeDataRow5.upgradeGoodsVolume;
				UISetter.SetLabel(commonItemCount[0], $"{base.localUser.FindPart(equipItemUpgradeDataRow5.upgradeMaterial2).count}/{equipItemUpgradeDataRow5.upgradeMaterial2Volume}");
				UISetter.SetLabel(commonItemCount[1], $"{base.localUser.FindPart(equipItemUpgradeDataRow5.upgradeMaterial3).count}/{equipItemUpgradeDataRow5.upgradeMaterial3Volume}");
				UISetter.SetLabel(commonItemCount[2], $"{base.localUser.FindPart(equipItemUpgradeDataRow5.upgradeMaterial4).count}/{equipItemUpgradeDataRow5.upgradeMaterial4Volume}");
				UISetter.SetLabel(commonItemCount[3], $"{base.localUser.FindPart(equipItemUpgradeDataRow5.upgradeMaterial5).count}/{equipItemUpgradeDataRow5.upgradeMaterial5Volume}");
				UISetter.SetLabel(UpgradeNeedGoldValue, needGold);
				UISetter.SetLabel(mainItemCount[i], $"{num2}/{num}");
				bool flag = equipItemUpgradeDataRow5.upgradeMaterial2Volume > base.localUser.FindPart(equipItemUpgradeDataRow5.upgradeMaterial2).count || equipItemUpgradeDataRow5.upgradeMaterial3Volume > base.localUser.FindPart(equipItemUpgradeDataRow5.upgradeMaterial3).count || equipItemUpgradeDataRow5.upgradeMaterial4Volume > base.localUser.FindPart(equipItemUpgradeDataRow5.upgradeMaterial4).count || equipItemUpgradeDataRow5.upgradeMaterial5Volume > base.localUser.FindPart(equipItemUpgradeDataRow5.upgradeMaterial5).count || num > num2 || currSelectItem.level >= int.Parse(base.regulation.defineDtbl["EQUIPITEM_LEVEL_LIMIT"].value);
				UISetter.SetActive(UpgradeBtnLock, flag);
				if (!flag)
				{
					laboratoryEff.SetEffect(laboratoryTabType, currSelectItem.statType);
				}
				else
				{
					laboratoryEff.OffEffect();
				}
				continue;
			}
			switch ((EItemStatType)i)
			{
			case EItemStatType.EQUIPED:
			{
				EquipItemUpgradeDataRow equipItemUpgradeDataRow9 = RemoteObjectManager.instance.regulation.FindUpgradeItemInfo(EItemStatType.ATK, 1);
				UISetter.SetLabel(mainItemCount[0], $"{base.localUser.FindPart(equipItemUpgradeDataRow9.upgradeMaterial1).count}/{0}");
				break;
			}
			case EItemStatType.ATK:
			{
				EquipItemUpgradeDataRow equipItemUpgradeDataRow8 = RemoteObjectManager.instance.regulation.FindUpgradeItemInfo(EItemStatType.DEF, 1);
				UISetter.SetLabel(mainItemCount[1], $"{base.localUser.FindPart(equipItemUpgradeDataRow8.upgradeMaterial1).count}/{0}");
				break;
			}
			case EItemStatType.DEF:
			{
				EquipItemUpgradeDataRow equipItemUpgradeDataRow7 = RemoteObjectManager.instance.regulation.FindUpgradeItemInfo(EItemStatType.ACCUR, 1);
				UISetter.SetLabel(mainItemCount[2], $"{base.localUser.FindPart(equipItemUpgradeDataRow7.upgradeMaterial1).count}/{0}");
				break;
			}
			case EItemStatType.ACCUR:
			{
				EquipItemUpgradeDataRow equipItemUpgradeDataRow6 = RemoteObjectManager.instance.regulation.FindUpgradeItemInfo(EItemStatType.LUCK, 1);
				UISetter.SetLabel(mainItemCount[3], $"{base.localUser.FindPart(equipItemUpgradeDataRow6.upgradeMaterial1).count}/{0}");
				break;
			}
			}
		}
	}

	private void ResetCommonItemCount()
	{
		for (int i = 0; i < commonItemCount.Count; i++)
		{
			UISetter.SetLabel(commonItemCount[0], $"{0}/{0}");
		}
	}

	public void SetDecompositionItems()
	{
		if (!decomposition.activeSelf)
		{
			return;
		}
		if (currSelectItem == null)
		{
			UISetter.SetActive(decompositionItemImg, active: false);
			UISetter.SetActive(decompositionItemLevel, active: false);
			UISetter.SetActive(emptySlot, active: true);
			UISetter.SetLabel(decompositionNeedGoldValue, 0);
			UISetter.SetActive(decompositionMainItemSlot, active: false);
			UISetter.SetActive(DecompositionCountLabel, active: false);
			UISetter.SetLabel(decompositionMainItemCount, 0);
			UISetter.SetLabel(decompositionCommonItem[0], 0);
			UISetter.SetLabel(decompositionCommonItem[1], 0);
			UISetter.SetLabel(decompositionCommonItem[2], 0);
			UISetter.SetLabel(decompositionCommonItem[3], 0);
			UISetter.SetActive(DecompositionBtnLock, active: true);
			UISetter.SetActive(DecompositionSlotTxt, active: true);
			laboratoryEff.OffEffect();
			return;
		}
		EquipItemDataRow equipItemDataRow = RemoteObjectManager.instance.regulation.equipItemDtbl.Find((EquipItemDataRow itemRow) => itemRow.key == currSelectItem.id);
		EquipItemDisassembleDataRow diassembleItem = RemoteObjectManager.instance.regulation.FindDisassembleItemInfo(currSelectItem.disassembleType, currSelectItem.level);
		needGold = diassembleItem.goodsIdxVolume * decompositionCount;
		string spriteName = string.Empty;
		string empty = string.Empty;
		UISetter.SetActive(DecompositionCountLabel, active: true);
		UISetter.SetLabel(DecompositionCountLabel, decompositionCount);
		UISetter.SetLabel(decompositionNeedGoldValue, needGold);
		UISetter.SetActive(DecompositionSlotTxt, active: false);
		UISetter.SetActive(DecompositionBtnLock, active: false);
		UISetter.SetActive(emptySlot, active: false);
		UISetter.SetActive(decompositionItemImg, active: true);
		UISetter.SetActive(decompositionItemLevel, active: true);
		UISetter.SetActive(decompositionMainItemSlot, active: true);
		UISetter.SetSprite(decompositionItemImg, equipItemDataRow.equipItemIcon);
		UISetter.SetLabel(decompositionItemLevel, string.Format(Localization.Get("1021"), currSelectItem.level));
		switch (diassembleItem.disassembleType)
		{
		case EItemStatType.ATK:
			spriteName = "allince_slot_red";
			break;
		case EItemStatType.DEF:
			spriteName = "allince_slot_green";
			break;
		case EItemStatType.ACCUR:
			spriteName = "allince_slot_yellow";
			break;
		case EItemStatType.LUCK:
			spriteName = "allince_slot_blue";
			break;
		}
		PartDataRow partDataRow = base.regulation.partDtbl.Find((PartDataRow partRow) => partRow.type == diassembleItem.disassembleMaterial1);
		UISetter.SetLabel(UpgradeNeedGoldValue, needGold);
		UISetter.SetSprite(decompositionMainItemSlot, spriteName);
		UISetter.SetSprite(decompositionMainItemIcon, partDataRow.serverFieldName);
		UISetter.SetLabel(decompositionMainItemCount, diassembleItem.return1 * decompositionCount);
		UISetter.SetLabel(decompositionCommonItem[0], diassembleItem.return2 * decompositionCount);
		UISetter.SetLabel(decompositionCommonItem[1], diassembleItem.return3 * decompositionCount);
		UISetter.SetLabel(decompositionCommonItem[2], diassembleItem.return4 * decompositionCount);
		UISetter.SetLabel(decompositionCommonItem[3], diassembleItem.return5 * decompositionCount);
		laboratoryEff.SetEffect(laboratoryTabType, currSelectItem.statType);
	}

	private void CreateHaveItemListPopup()
	{
		if (haveItemListPopup == null)
		{
			haveItemListPopup = UIPopup.Create<UIHaveItemListPopup>("HaveItemPopup");
			haveItemListPopup.SetHaveItemList(_equipOpen: false);
		}
		else
		{
			haveItemListPopup.SetHaveItemList(_equipOpen: false);
		}
	}

	private bool IsOverMaxCount()
	{
		if (currSelectItem != null)
		{
			EquipItemDisassembleDataRow equipItemDisassembleDataRow = RemoteObjectManager.instance.regulation.FindDisassembleItemInfo(currSelectItem.disassembleType, currSelectItem.level);
			if (equipItemDisassembleDataRow != null)
			{
				RoPart roPart = base.localUser.FindPart(equipItemDisassembleDataRow.disassembleMaterial1);
				if (roPart != null && !base.localUser.GetItemCheck(ERewardType.UnitMaterial, roPart.id, equipItemDisassembleDataRow.return1 * decompositionCount))
				{
					return true;
				}
				RoPart roPart2 = base.localUser.FindPart(equipItemDisassembleDataRow.disassembleMaterial2);
				if (roPart2 != null && !base.localUser.GetItemCheck(ERewardType.UnitMaterial, roPart2.id, equipItemDisassembleDataRow.return2 * decompositionCount))
				{
					return true;
				}
				RoPart roPart3 = base.localUser.FindPart(equipItemDisassembleDataRow.disassembleMaterial3);
				if (roPart3 != null && !base.localUser.GetItemCheck(ERewardType.UnitMaterial, roPart3.id, equipItemDisassembleDataRow.return3 * decompositionCount))
				{
					return true;
				}
				RoPart roPart4 = base.localUser.FindPart(equipItemDisassembleDataRow.disassembleMaterial4);
				if (roPart4 != null && !base.localUser.GetItemCheck(ERewardType.UnitMaterial, roPart4.id, equipItemDisassembleDataRow.return4 * decompositionCount))
				{
					return true;
				}
				RoPart roPart5 = base.localUser.FindPart(equipItemDisassembleDataRow.disassembleMaterial5);
				if (roPart5 != null && !base.localUser.GetItemCheck(ERewardType.UnitMaterial, roPart5.id, equipItemDisassembleDataRow.return5 * decompositionCount))
				{
					return true;
				}
			}
		}
		return false;
	}
}
