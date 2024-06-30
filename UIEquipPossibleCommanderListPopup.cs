using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIEquipPossibleCommanderListPopup : MonoBehaviour
{
	[SerializeField]
	private UIDefaultListView possibleCommanderListView;

	[SerializeField]
	private UISprite itemIcon;

	[SerializeField]
	private UILabel itemLv;

	[SerializeField]
	private UILabel commanderName;

	[SerializeField]
	private UILabel subDesc_1;

	[SerializeField]
	private UILabel subDesc_value1;

	[SerializeField]
	private UILabel subDesc_2;

	[SerializeField]
	private UILabel subDesc_value2;

	[SerializeField]
	private UILabel setDesc;

	[SerializeField]
	private UILabel setValue;

	[SerializeField]
	private GameObject EquipBtn;

	private string idPrefix = "commander_";

	private RoItem currSelectItem;

	private RoCommander currSelectCommander;

	private RoLocalUser localUser;

	private Regulation regulation;

	private RemoteObjectManager network;

	public void InitPossibleCommanderList(RoItem item)
	{
		if (item != null)
		{
			network = RemoteObjectManager.instance;
			if (network != null)
			{
				localUser = network.localUser;
				regulation = network.regulation;
			}
			SetPossibleCommanderList(item);
		}
	}

	public void SetPossibleCommanderList(RoItem item)
	{
		currSelectItem = item;
		List<RoCommander> list = SortCommanderList();
		if (list == null)
		{
			UISetter.SetActive(itemIcon, active: false);
			UISetter.SetLabel(itemLv, string.Empty);
			UISetter.SetActive(possibleCommanderListView, active: false);
			UISetter.SetLabel(commanderName, string.Empty);
			UISetter.SetLabel(subDesc_1, string.Empty);
			UISetter.SetLabel(subDesc_value1, string.Empty);
			UISetter.SetLabel(subDesc_2, string.Empty);
			UISetter.SetLabel(subDesc_value2, string.Empty);
			UISetter.SetLabel(setDesc, string.Empty);
			UISetter.SetLabel(setValue, string.Empty);
			UISetter.SetActive(EquipBtn, active: false);
		}
		else
		{
			UISetter.SetActive(EquipBtn, active: true);
			UISetter.SetActive(possibleCommanderListView, active: true);
			possibleCommanderListView.Init(list, idPrefix);
			possibleCommanderListView.SetSelection(list[0].id, selected: true);
			SetDetailIcon(item);
			currSelectCommander = list[0];
			SetItemDetail();
		}
	}

	public void OnClick(GameObject sender)
	{
		string text = sender.name;
		switch (text)
		{
		case "Btn_equip":
			network.RequestSetItemEquipment(int.Parse(currSelectItem.id), int.Parse(currSelectCommander.id), currSelectItem.level);
			SoundManager.PlaySFX("BTN_Norma_001");
			Close();
			break;
		case "Btn_change":
		{
			RoItem roItem = currSelectCommander.FindEquipItem(currSelectItem.pointType);
			if (roItem != null)
			{
				network.RequestChangeItemEquipment(int.Parse(roItem.id), int.Parse(roItem.currEquipCommanderId), int.Parse(currSelectItem.id), int.Parse(currSelectCommander.id), currSelectItem.level);
			}
			SoundManager.PlaySFX("BTN_Norma_001");
			break;
		}
		case "Close":
			Close();
			SoundManager.PlaySFX("SE_MenuClose_001");
			break;
		}
		if (text.StartsWith(idPrefix))
		{
			string id = text.Substring(idPrefix.Length);
			possibleCommanderListView.SetSelection(id, selected: true);
			currSelectCommander = localUser.FindCommander(id);
			if (currSelectCommander != null)
			{
				SetItemDetail();
			}
			SoundManager.PlaySFX("BTN_Norma_001");
		}
	}

	public void Close()
	{
		UISetter.SetActive(this, active: false);
	}

	private void SetDetailIcon(RoItem item)
	{
		EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == item.id);
		if (equipItemDataRow != null)
		{
			UISetter.SetActive(itemIcon, active: true);
			UISetter.SetSprite(itemIcon, equipItemDataRow.equipItemIcon);
		}
		UISetter.SetLabel(itemLv, string.Format(Localization.Get("1021"), item.level));
	}

	public void SetItemDetail()
	{
		EquipItemDataRow itemSetTypeString = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == currSelectItem.id);
		UISetter.SetLabel(commanderName, currSelectCommander.nickname);
		SetItemSetTypeString(itemSetTypeString);
		if (currSelectCommander == null)
		{
			return;
		}
		RoItem roItem = currSelectCommander.FindEquipItem(currSelectItem.pointType);
		if (roItem != null)
		{
			if (roItem.statType == currSelectItem.statType)
			{
				if (roItem.statPoint > currSelectItem.statPoint)
				{
					UISetter.SetLabel(subDesc_1, ItemTypeString(roItem.statType));
					UISetter.SetLabel(subDesc_value1, "+" + (roItem.statPoint - currSelectItem.statPoint));
				}
				else if (roItem.statPoint == currSelectItem.statPoint)
				{
					UISetter.SetLabel(subDesc_1, Localization.Get("5030410"));
					UISetter.SetLabel(subDesc_value1, 0);
				}
				else if (roItem.statPoint < currSelectItem.statPoint)
				{
					UISetter.SetLabel(subDesc_1, ItemTypeString(roItem.statType));
					UISetter.SetLabel(subDesc_value1, "-" + (currSelectItem.statPoint - roItem.statPoint));
				}
				UISetter.SetLabel(subDesc_2, Localization.Get("5030410"));
				UISetter.SetLabel(subDesc_value2, 0);
			}
			else
			{
				UISetter.SetLabel(subDesc_1, ItemTypeString(roItem.statType));
				UISetter.SetLabel(subDesc_value1, "-" + roItem.statPoint);
				UISetter.SetLabel(subDesc_2, ItemTypeString(currSelectItem.statType));
				UISetter.SetLabel(subDesc_value2, "+" + currSelectItem.statPoint);
			}
		}
		else
		{
			UISetter.SetLabel(subDesc_1, ItemTypeString(currSelectItem.statType));
			UISetter.SetLabel(subDesc_value1, "+" + currSelectItem.statPoint);
			UISetter.SetLabel(subDesc_2, Localization.Get("5030410"));
			UISetter.SetLabel(subDesc_value2, 0);
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

	private void SetItemSetTypeString(EquipItemDataRow currselectItemDb)
	{
		if (currselectItemDb == null || currSelectCommander == null)
		{
			return;
		}
		if (currSelectCommander.completeSetItemEquip)
		{
			foreach (KeyValuePair<int, RoItem> pair in currSelectCommander.GetEquipItemList())
			{
				if (pair.Value.pointType == currSelectItem.pointType)
				{
					if (pair.Value.setType != currSelectItem.setType)
					{
						EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == pair.Value.id);
						UISetter.SetLabel(setDesc, string.Format("{0} {1}", Localization.Get("5030407"), ItemSetTypeString(pair.Value.setType)));
						if (equipItemDataRow != null)
						{
							UISetter.SetLabel(setDesc, string.Format("{0} {1}", Localization.Get("5030407"), ItemSetTypeString(pair.Value.setType)));
							string text = equipItemDataRow.statEffect.ToString();
							if (equipItemDataRow.setItemType == EItemSetType.CRITDMG && equipItemDataRow.setItemType == EItemSetType.CRITR)
							{
								text += "%";
							}
							UISetter.SetLabel(setValue, "-" + text);
						}
					}
					else
					{
						string text2 = currselectItemDb.statEffect.ToString();
						if (currselectItemDb.setItemType == EItemSetType.CRITDMG && currselectItemDb.setItemType == EItemSetType.CRITR)
						{
							text2 += "%";
						}
						UISetter.SetLabel(setDesc, string.Format("{0} {1}", Localization.Get("5030407"), ItemSetTypeString(pair.Value.setType)));
						UISetter.SetLabel(setValue, "+" + text2);
					}
				}
			}
			return;
		}
		UISetter.SetLabel(setDesc, string.Format("{0} {1}", Localization.Get("5030407"), ItemSetTypeString(currselectItemDb.setItemType)));
		string text3 = currselectItemDb.statEffect.ToString();
		if (currselectItemDb.setItemType == EItemSetType.CRITDMG && currselectItemDb.setItemType == EItemSetType.CRITR)
		{
			text3 += "%";
		}
		if (currSelectCommander.checkCompleteSetItem(currselectItemDb.setItemType))
		{
			UISetter.SetLabel(setValue, "+" + text3);
		}
		else
		{
			UISetter.SetLabel(setValue, 0);
		}
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

	private List<RoCommander> SortCommanderList()
	{
		List<RoCommander> commanderList = localUser.GetCommanderList(EJob.All, have: true, recruit: true);
		if (commanderList == null)
		{
			return null;
		}
		List<RoCommander> list = new List<RoCommander>();
		for (int i = 0; i < commanderList.Count; i++)
		{
			if (OpenSlot(currSelectItem.pointType, commanderList[i].cls) && commanderList[i].FindEquipItem(currSelectItem.pointType) == null)
			{
				list.Add(commanderList[i]);
			}
		}
		if (list.Count > 0)
		{
			return list;
		}
		return null;
	}

	private bool OpenSlot(int pointType, int cls)
	{
		switch (pointType)
		{
		case 1:
			if (int.Parse(regulation.defineDtbl["EQUIPITEM_1SLOT_OPEN_CLASS_LIMIT"].value) <= cls)
			{
				return true;
			}
			return false;
		case 2:
			if (int.Parse(regulation.defineDtbl["EQUIPITEM_2SLOT_OPEN_CLASS_LIMIT"].value) <= cls)
			{
				return true;
			}
			return false;
		case 3:
			if (int.Parse(regulation.defineDtbl["EQUIPITEM_3SLOT_OPEN_CLASS_LIMIT"].value) <= cls)
			{
				return true;
			}
			return false;
		case 4:
			if (int.Parse(regulation.defineDtbl["EQUIPITEM_4SLOT_OPEN_CLASS_LIMIT"].value) <= cls)
			{
				return true;
			}
			return false;
		default:
			return false;
		}
	}
}
