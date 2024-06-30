using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIHaveItemListPopup : UIPopup
{
	[SerializeField]
	private UIDefaultListView haveItemListView;

	[SerializeField]
	private UIHaveListItem haveItem;

	public static readonly string ItemIdPrefix = "HaveItem-";

	[SerializeField]
	private UILabel mainDesc;

	[SerializeField]
	private UILabel subDesc_1;

	[SerializeField]
	private UILabel subDesc_2;

	[SerializeField]
	private UILabel subDesc_3;

	[SerializeField]
	private UILabel subDesc_value1;

	[SerializeField]
	private UILabel subDesc_value2;

	[SerializeField]
	private UILabel subDesc_value3;

	[SerializeField]
	private UISprite detailItemIcon;

	[SerializeField]
	private UILabel detailItemLv;

	[SerializeField]
	private UILabel equipOrselect_txt;

	[SerializeField]
	private UILabel clearOrchange_txt;

	[SerializeField]
	private GameObject up_arrow;

	[SerializeField]
	private GameObject btn_Laboratory;

	private string sub_txt_1 = string.Empty;

	private string sub_txt_2 = string.Empty;

	private string sub_txt_3 = string.Empty;

	private bool isCommanderDetailOpen;

	private int currSelectPointType;

	[HideInInspector]
	public RoCommander currCommander;

	[SerializeField]
	private UIFlipSwitch AllTab;

	[SerializeField]
	private UIFlipSwitch Type_1;

	[SerializeField]
	private UIFlipSwitch Type_2;

	[SerializeField]
	private UIFlipSwitch Type_3;

	[SerializeField]
	private UIFlipSwitch Type_4;

	[SerializeField]
	private GameObject btn_selectOrEquip;

	[SerializeField]
	private GameObject btn_clearOrChange;

	[SerializeField]
	private GameObject btn_lock;

	[SerializeField]
	private GameObject btn_upgrade;

	private EItemStatType curSelectTabType;

	private List<RoItem> possibleItemList = new List<RoItem>();

	private List<RoItem> equipedItemList = new List<RoItem>();

	private List<RoItem> curItemList;

	private RoItem selectItem;

	[SerializeField]
	private UIDecompositionPopup decompositionPopup;

	public void SetHaveItemList(bool _equipOpen, int slotPos = 0)
	{
		SetRecyclable(recyclable: false);
		selectItem = null;
		isCommanderDetailOpen = _equipOpen;
		currSelectPointType = slotPos;
		possibleItemList = base.localUser.GetEquipPossibleItemList();
		equipedItemList = base.localUser.GetEquipedItemList();
		UISetter.SetFlipSwitch(AllTab, state: true);
		UISetter.SetFlipSwitch(Type_1, state: false);
		UISetter.SetFlipSwitch(Type_2, state: false);
		UISetter.SetFlipSwitch(Type_3, state: false);
		UISetter.SetFlipSwitch(Type_4, state: false);
		SortItemList(EItemStatType.EQUIPED);
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		switch (text)
		{
		case "Close":
			Close();
			break;
		case "All_tab":
			selectItem = null;
			curSelectTabType = EItemStatType.EQUIPED;
			SortItemList(EItemStatType.EQUIPED);
			haveItemListView.ResetPosition();
			break;
		case "Item_1":
			selectItem = null;
			curSelectTabType = EItemStatType.ATK;
			SortItemList(EItemStatType.ATK);
			haveItemListView.ResetPosition();
			break;
		case "Item_2":
			selectItem = null;
			curSelectTabType = EItemStatType.DEF;
			SortItemList(EItemStatType.DEF);
			haveItemListView.ResetPosition();
			break;
		case "Item_3":
			selectItem = null;
			curSelectTabType = EItemStatType.ACCUR;
			SortItemList(EItemStatType.ACCUR);
			haveItemListView.ResetPosition();
			break;
		case "Item_4":
			selectItem = null;
			curSelectTabType = EItemStatType.LUCK;
			SortItemList(EItemStatType.LUCK);
			haveItemListView.ResetPosition();
			break;
		case "Btn_Equip":
			if (currSelectPointType != 0 && currSelectPointType != selectItem.pointType)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5030423"));
				return;
			}
			base.network.RequestSetItemEquipment(int.Parse(selectItem.id), int.Parse(currCommander.id), selectItem.level);
			break;
		case "Btn_Select":
			if (base.uiWorld.existLaboratory && base.uiWorld.laboratory.isActive)
			{
				if (base.uiWorld.laboratory.laboratoryTabType == CurTabType.Decomposition)
				{
					SoundManager.PlaySFX("BTN_Norma_001");
					UISetter.SetActive(decompositionPopup, active: true);
					decompositionPopup.SetDecompositionInfo(selectItem);
				}
				else
				{
					SoundManager.PlaySFX("SE_5011_Passive1PreAttack_001");
					base.uiWorld.laboratory.currSelectItem = selectItem;
					base.uiWorld.laboratory.OnRefresh();
					Close();
				}
			}
			break;
		case "Btn_Clear":
			base.network.RequestReleaseItemEquipment(int.Parse(selectItem.id), int.Parse(selectItem.currEquipCommanderId));
			break;
		case "Btn_Change":
			if (isCommanderDetailOpen)
			{
				if (currSelectPointType != 0 && currSelectPointType != selectItem.pointType)
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5030423"));
					return;
				}
				base.network.RequestSetItemEquipment(int.Parse(selectItem.id), int.Parse(currCommander.id), selectItem.level);
			}
			else if (base.uiWorld.existLaboratory && base.uiWorld.laboratory.isActive)
			{
				if (base.uiWorld.laboratory.laboratoryTabType == CurTabType.Decomposition)
				{
					SoundManager.PlaySFX("BTN_Norma_001");
					UISetter.SetActive(decompositionPopup, active: true);
					decompositionPopup.SetDecompositionInfo(selectItem);
				}
				else
				{
					SoundManager.PlaySFX("SE_5011_Passive1PreAttack_001");
					base.uiWorld.laboratory.currSelectItem = selectItem;
					base.uiWorld.laboratory.OnRefresh();
					Close();
				}
			}
			break;
		case "Btn_Upgrade":
		case "Btn_Laboratory":
			base.localUser.isGoLaboratory = true;
			base.localUser.curSelectItem_forLaboratory = selectItem;
			base.localUser.curSelectCommanderId_forLaboratory = ((currCommander == null) ? string.Empty : currCommander.id);
			Close();
			if (base.uiWorld.existCommanderDetail && base.uiWorld.commanderDetail.isActive)
			{
				base.uiWorld.commanderDetail.ClosePopup();
			}
			base.uiWorld.headQuarters.ClosePopUp();
			base.uiWorld.camp.GoNavigation("Laboratory");
			break;
		}
		if (haveItemListView.Contains(text))
		{
			haveItemListView.SetSelection(text, selected: true);
			string[] array = text.Split('_');
			int num = int.Parse(array[0]);
			int index = int.Parse(array[1]);
			selectItem = curItemList[index];
			SetItemDetail();
			SoundManager.PlaySFX("BTN_Norma_001");
		}
		base.OnClick(sender);
	}

	public void RefreshList()
	{
		selectItem = null;
		possibleItemList = base.localUser.GetEquipPossibleItemList();
		equipedItemList = base.localUser.GetEquipedItemList();
		SortItemList(curSelectTabType);
	}

	private void SortItemList(EItemStatType sortType)
	{
		List<RoItem> list = new List<RoItem>();
		if (sortType != 0)
		{
			for (int i = 0; i < possibleItemList.Count; i++)
			{
				if (possibleItemList[i].statType != sortType)
				{
					continue;
				}
				if (isCommanderDetailOpen)
				{
					if (possibleItemList[i].pointType == currSelectPointType)
					{
						list.Add(possibleItemList[i]);
					}
				}
				else
				{
					list.Add(possibleItemList[i]);
				}
			}
		}
		else if (isCommanderDetailOpen)
		{
			for (int j = 0; j < equipedItemList.Count; j++)
			{
				if (equipedItemList[j].pointType == currSelectPointType)
				{
					list.Add(equipedItemList[j]);
				}
			}
		}
		else
		{
			list = equipedItemList;
		}
		list.Sort(delegate(RoItem sort_1, RoItem sort_2)
		{
			if (sort_1.level > sort_2.level)
			{
				return -1;
			}
			return (sort_1.level != sort_2.level) ? 1 : 0;
		});
		if (list.Count > 0)
		{
			curItemList = list;
			UISetter.SetActive(haveItemListView, active: true);
			haveItemListView.InitHaveItemList(list);
			haveItemListView.SetSelection(list[0].id + "_" + 0, selected: true);
			haveItem.Set(list[0]);
			selectItem = list[0];
		}
		else
		{
			UISetter.SetActive(haveItemListView, active: false);
		}
		SetItemDetail();
	}

	public void SetItemDetail()
	{
		if (selectItem == null)
		{
			UISetter.SetLabel(detailItemLv, string.Empty);
			UISetter.SetSprite(detailItemIcon, string.Empty);
			UISetter.SetLabel(mainDesc, string.Empty);
			UISetter.SetLabel(subDesc_1, string.Empty);
			UISetter.SetLabel(subDesc_value1, string.Empty);
			UISetter.SetLabel(subDesc_2, string.Empty);
			UISetter.SetLabel(subDesc_value2, string.Empty);
			UISetter.SetActive(up_arrow, active: false);
			UISetter.SetActive(btn_selectOrEquip, active: false);
			UISetter.SetActive(btn_clearOrChange, active: false);
			UISetter.SetActive(btn_upgrade, active: false);
			UISetter.SetLabel(subDesc_3, string.Empty);
			UISetter.SetLabel(subDesc_value3, string.Empty);
			UISetter.SetActive(btn_Laboratory, isCommanderDetailOpen);
			UISetter.SetActive(btn_lock, active: false);
			return;
		}
		EquipItemDataRow equipItemDataRow = base.regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == selectItem.id);
		if (equipItemDataRow == null)
		{
			return;
		}
		if (selectItem.isEquip && isCommanderDetailOpen)
		{
			RoCommander roCommander = base.localUser.FindCommander(selectItem.currEquipCommanderId);
			if (roCommander != null)
			{
				UISetter.SetLabel(mainDesc, roCommander.nickname);
			}
		}
		else
		{
			UISetter.SetLabel(mainDesc, Localization.Get(selectItem.nameIdx));
		}
		UISetter.SetLabel(detailItemLv, selectItem.level);
		UISetter.SetSprite(detailItemIcon, equipItemDataRow.equipItemIcon);
		SetItemSetTypeString(equipItemDataRow, selectItem);
		if (currCommander != null)
		{
			RoItem roItem = currCommander.FindEquipItem(selectItem.pointType);
			if (roItem != null)
			{
				if (roItem.statType == selectItem.statType)
				{
					if (roItem.statPoint > selectItem.statPoint)
					{
						UISetter.SetLabel(subDesc_1, ItemTypeString(roItem.statType));
						UISetter.SetLabel(subDesc_value1, "-" + (roItem.statPoint - selectItem.statPoint));
					}
					else if (roItem.statPoint == selectItem.statPoint)
					{
						UISetter.SetLabel(subDesc_1, Localization.Get("5030410"));
						UISetter.SetLabel(subDesc_value1, 0);
					}
					else if (roItem.statPoint < selectItem.statPoint)
					{
						UISetter.SetLabel(subDesc_1, ItemTypeString(roItem.statType));
						UISetter.SetLabel(subDesc_value1, "+" + (selectItem.statPoint - roItem.statPoint));
					}
					UISetter.SetLabel(subDesc_2, Localization.Get("5030410"));
					UISetter.SetLabel(subDesc_value2, 0);
				}
				else
				{
					UISetter.SetLabel(subDesc_1, ItemTypeString(roItem.statType));
					UISetter.SetLabel(subDesc_value1, "-" + roItem.statPoint);
					UISetter.SetLabel(subDesc_2, ItemTypeString(selectItem.statType));
					UISetter.SetLabel(subDesc_value2, "+" + selectItem.statPoint);
				}
			}
			else if (selectItem.isEquip)
			{
				UISetter.SetLabel(subDesc_1, ItemTypeString(selectItem.statType));
				UISetter.SetLabel(subDesc_value1, "+" + selectItem.statPoint);
				UISetter.SetLabel(subDesc_2, Localization.Get("5030410"));
				UISetter.SetLabel(subDesc_value2, 0);
			}
			else
			{
				UISetter.SetLabel(subDesc_1, Localization.Get("5030421"));
				UISetter.SetLabel(subDesc_value1, string.Empty);
				UISetter.SetLabel(subDesc_2, ItemTypeString(selectItem.statType));
				UISetter.SetLabel(subDesc_value2, "+" + selectItem.statPoint);
			}
		}
		else
		{
			RoCommander roCommander2 = base.localUser.FindCommander(selectItem.currEquipCommanderId);
			if (roCommander2 != null)
			{
				UISetter.SetLabel(subDesc_1, roCommander2.nickname);
				UISetter.SetLabel(subDesc_value1, string.Empty);
			}
			else
			{
				UISetter.SetLabel(subDesc_1, Localization.Get("5030421"));
				UISetter.SetLabel(subDesc_value1, string.Empty);
			}
			UISetter.SetLabel(subDesc_2, ItemTypeString(selectItem.statType));
			UISetter.SetLabel(subDesc_value2, "+" + selectItem.statPoint);
		}
		UISetter.SetActive(up_arrow, active: false);
		EquipItemUpgradeDataRow equipItemUpgradeDataRow = base.regulation.equipItemUpgradeDtbl.Find((EquipItemUpgradeDataRow row) => row.upgradeType == selectItem.upgradeType && row.level == selectItem.level);
		if (equipItemUpgradeDataRow != null)
		{
			RoPart roPart = base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial1);
			if (roPart != null && roPart.count > equipItemUpgradeDataRow.upgradeMaterial1Volume)
			{
				RoPart roPart2 = base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial2);
				if (roPart2 != null && roPart2.count > equipItemUpgradeDataRow.upgradeMaterial2Volume)
				{
					RoPart roPart3 = base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial3);
					if (roPart3 != null && roPart3.count > equipItemUpgradeDataRow.upgradeMaterial3Volume)
					{
						RoPart roPart4 = base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial4);
						if (roPart4 != null && roPart4.count > equipItemUpgradeDataRow.upgradeMaterial4Volume)
						{
							RoPart roPart5 = base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial5);
							if (roPart5 != null && roPart5.count > equipItemUpgradeDataRow.upgradeMaterial5Volume)
							{
								UISetter.SetActive(up_arrow, active: true);
							}
						}
					}
				}
			}
		}
		setButton();
	}

	private void setButton()
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		UISetter.SetActive(btn_lock, active: false);
		if (isCommanderDetailOpen)
		{
			if (currCommander == null)
			{
				return;
			}
			if (selectItem.isEquip)
			{
				UISetter.SetActive(btn_selectOrEquip, active: false);
				UISetter.SetActive(btn_clearOrChange, active: true);
				UISetter.SetGameObjectName(btn_clearOrChange, "Btn_Clear");
				UISetter.SetLabel(clearOrchange_txt, Localization.Get("5030412"));
			}
			else
			{
				if (currSelectPointType != selectItem.pointType)
				{
					UISetter.SetActive(btn_lock, active: true);
				}
				else
				{
					RoItem roItem = currCommander.FindEquipItem(currSelectPointType);
					if (roItem != null && roItem.id == selectItem.id && roItem.level == selectItem.level)
					{
						UISetter.SetActive(btn_lock, active: true);
					}
				}
				if (currCommander.FindEquipItem(selectItem.pointType) != null)
				{
					UISetter.SetActive(btn_selectOrEquip, active: false);
					UISetter.SetActive(btn_clearOrChange, active: true);
					UISetter.SetGameObjectName(btn_clearOrChange, "Btn_Change");
					UISetter.SetLabel(clearOrchange_txt, Localization.Get("5030418"));
				}
				else
				{
					UISetter.SetActive(btn_selectOrEquip, active: true);
					UISetter.SetActive(btn_clearOrChange, active: false);
					UISetter.SetGameObjectName(btn_selectOrEquip, "Btn_Equip");
					UISetter.SetLabel(equipOrselect_txt, Localization.Get("5030411"));
				}
			}
		}
		else if (base.uiWorld.existLaboratory)
		{
			UILaboratory laboratory = base.uiWorld.laboratory;
			RoItem currSelectItem = laboratory.currSelectItem;
			if (currSelectItem == null)
			{
				UISetter.SetActive(btn_selectOrEquip, active: true);
				UISetter.SetActive(btn_clearOrChange, active: false);
				UISetter.SetGameObjectName(btn_selectOrEquip, "Btn_Select");
				UISetter.SetLabel(equipOrselect_txt, Localization.Get("5030419"));
			}
			else
			{
				UISetter.SetActive(btn_selectOrEquip, active: false);
				UISetter.SetActive(btn_clearOrChange, active: true);
				UISetter.SetGameObjectName(btn_clearOrChange, "Btn_Change");
				UISetter.SetLabel(clearOrchange_txt, Localization.Get("5030418"));
			}
			if (laboratory.laboratoryTabType == CurTabType.Decomposition && selectItem.isEquip)
			{
				UISetter.SetActive(btn_selectOrEquip, active: false);
				UISetter.SetActive(btn_clearOrChange, active: true);
				UISetter.SetGameObjectName(btn_clearOrChange, "Btn_Clear");
				UISetter.SetLabel(clearOrchange_txt, Localization.Get("5030412"));
			}
		}
		UISetter.SetActive(btn_Laboratory, isCommanderDetailOpen);
		UISetter.SetActive(btn_upgrade, isCommanderDetailOpen);
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

	private void SetItemSetTypeString(EquipItemDataRow currselectItemDb, RoItem currSelectItem)
	{
		if (currselectItemDb == null)
		{
			return;
		}
		if (currCommander != null)
		{
			if (currCommander.completeSetItemEquip)
			{
				foreach (KeyValuePair<int, RoItem> pair in currCommander.GetEquipItemList())
				{
					if (pair.Value.pointType == currSelectItem.pointType)
					{
						if (pair.Value.setType != currSelectItem.setType)
						{
							EquipItemDataRow equipItemDataRow = base.regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == pair.Value.id);
							if (equipItemDataRow != null)
							{
								UISetter.SetLabel(subDesc_3, ItemSetTypeString(equipItemDataRow.setItemType));
								string text = equipItemDataRow.statEffect.ToString();
								if (equipItemDataRow.setItemType == EItemSetType.CRITDMG && equipItemDataRow.setItemType == EItemSetType.CRITR)
								{
									text += "%";
								}
								UISetter.SetLabel(subDesc_value3, "-" + text);
							}
						}
						else
						{
							string text2 = currselectItemDb.statEffect.ToString();
							if (currselectItemDb.setItemType == EItemSetType.CRITDMG && currselectItemDb.setItemType == EItemSetType.CRITR)
							{
								text2 += "%";
							}
							UISetter.SetLabel(subDesc_3, ItemSetTypeString(currselectItemDb.setItemType));
							UISetter.SetLabel(subDesc_value3, "+" + text2);
						}
					}
				}
				return;
			}
			UISetter.SetLabel(subDesc_3, ItemSetTypeString(currselectItemDb.setItemType));
			string text3 = currselectItemDb.statEffect.ToString();
			if (currselectItemDb.setItemType == EItemSetType.CRITDMG && currselectItemDb.setItemType == EItemSetType.CRITR)
			{
				text3 += "%";
			}
			if (currCommander.checkCompleteSetItem(currselectItemDb.setItemType))
			{
				UISetter.SetLabel(subDesc_value3, "+" + text3);
			}
			else
			{
				UISetter.SetLabel(subDesc_value3, 0);
			}
		}
		else
		{
			UISetter.SetLabel(subDesc_3, ItemSetTypeString(currselectItemDb.setItemType));
			string text4 = currselectItemDb.statEffect.ToString();
			if (currselectItemDb.setItemType == EItemSetType.CRITDMG && currselectItemDb.setItemType == EItemSetType.CRITR)
			{
				text4 += "%";
			}
			else
			{
				UISetter.SetLabel(subDesc_value3, "+" + text4);
			}
		}
	}

	private string ItemSetTypeString(EItemSetType setType)
	{
		string arg = Localization.Get("5030407");
		return setType switch
		{
			EItemSetType.ATK => arg = string.Format("{0} {1}", arg, Localization.Get("1")), 
			EItemSetType.DEF => arg = string.Format("{0} {1}", arg, Localization.Get("2")), 
			EItemSetType.ACCUR => arg = string.Format("{0} {1}", arg, Localization.Get("5")), 
			EItemSetType.LUCK => arg = string.Format("{0} {1}", arg, Localization.Get("3")), 
			EItemSetType.CRITDMG => arg = string.Format("{0} {1}", arg, Localization.Get("8")), 
			EItemSetType.CRITR => arg = string.Format("{0} {1}", arg, Localization.Get("6")), 
			_ => null, 
		};
	}
}
