using System.Collections;
using Shared.Regulation;
using UnityEngine;

public class UIDecompositionPopup : MonoBehaviour
{
	[SerializeField]
	private UILabel itemName;

	[SerializeField]
	private UILabel mainDesc;

	[SerializeField]
	private UILabel detailItemLv;

	[SerializeField]
	private UILabel itemCount;

	[SerializeField]
	private UISprite detailItemIcon;

	[SerializeField]
	private UILabel de_count;

	[SerializeField]
	private UILabel basicAbility;

	[SerializeField]
	private UILabel basicValue;

	[SerializeField]
	private UILabel setKind;

	[SerializeField]
	private UILabel setValue;

	[SerializeField]
	private UILabel inputLabel;

	[SerializeField]
	private UIInput input;

	[SerializeField]
	private GameObject decreaseBtn;

	[SerializeField]
	private GameObject addBtn;

	private bool isPress;

	private int selectedItemCount;

	private int decompositionCount = 1;

	private RoItem currSelectedItem;

	private const int maxDecompositionCount = 9999;

	public void SetDecompositionInfo(RoItem selectItem)
	{
		if (selectItem == null)
		{
			return;
		}
		currSelectedItem = selectItem;
		isPress = false;
		selectedItemCount = 0;
		decompositionCount = 1;
		if (selectItem.isEquip)
		{
			return;
		}
		EquipItemDataRow equipItemDataRow = RemoteObjectManager.instance.regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == selectItem.id);
		if (equipItemDataRow != null)
		{
			UISetter.SetLabel(itemCount, selectItem.itemCount);
			UISetter.SetLabel(mainDesc, Localization.Get(selectItem.nameIdx));
			UISetter.SetLabel(detailItemLv, string.Format(Localization.Get("1021"), selectItem.level));
			UISetter.SetSprite(detailItemIcon, equipItemDataRow.equipItemIcon);
			UISetter.SetLabel(basicAbility, ItemTypeString(selectItem.statType));
			UISetter.SetLabel(basicValue, "+" + selectItem.statPoint);
			UISetter.SetLabel(setKind, ItemSetTypeString(equipItemDataRow.setItemType));
			string text = equipItemDataRow.statEffect.ToString();
			if (equipItemDataRow.setItemType == EItemSetType.CRITDMG && equipItemDataRow.setItemType == EItemSetType.CRITR)
			{
				text += "%";
			}
			else
			{
				UISetter.SetLabel(setValue, "+" + text);
			}
			selectedItemCount = selectItem.itemCount;
			SetValue();
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

	public void OK()
	{
		if (UIManager.instance.world.existLaboratory)
		{
			UILaboratory laboratory = UIManager.instance.world.laboratory;
			if (laboratory.laboratoryTabType == CurTabType.Decomposition)
			{
				laboratory.currSelectItem = currSelectedItem;
				laboratory.decompositionCount = decompositionCount;
				laboratory.OnRefresh();
				StartCoroutine(laboratory.laboratoryEff.DecompositionSlotEffect(currSelectedItem.statType));
				UISetter.SetActive(this, active: false);
				if (laboratory.haveItemListPopup != null && laboratory.haveItemListPopup.isActive)
				{
					laboratory.haveItemListPopup.Close();
				}
			}
		}
		SoundManager.PlaySFX("BTN_Positive_001");
	}

	public void DecreaseItemStart()
	{
		StartCoroutine(ItemCalculation(-1));
	}

	public void DecreaseItemEnd()
	{
		isPress = false;
		if (ItemCheck(-1))
		{
			decompositionCount--;
			SetValue();
		}
	}

	public void AddItemStart()
	{
		StartCoroutine(ItemCalculation(1));
	}

	public void AddItemEnd()
	{
		isPress = false;
		if (ItemCheck(1))
		{
			decompositionCount++;
			SetValue();
		}
	}

	private IEnumerator ItemCalculation(int value)
	{
		float speed = 0.05f;
		isPress = true;
		yield return new WaitForSeconds(1f);
		while (ItemCheck(value) && isPress)
		{
			decompositionCount += value;
			SetValue();
			yield return new WaitForSeconds(speed);
		}
		yield return true;
	}

	private bool ItemCheck(int value)
	{
		if (value > 0)
		{
			if (decompositionCount < selectedItemCount && decompositionCount < 9999)
			{
				return true;
			}
		}
		else if (decompositionCount > 1)
		{
			return true;
		}
		return false;
	}

	public void ItemCountMax()
	{
		decompositionCount = selectedItemCount;
		if (decompositionCount > 9999)
		{
			decompositionCount = 9999;
		}
		SetValue();
	}

	public void SetInputValue()
	{
		if (string.IsNullOrEmpty(input.value) || int.Parse(input.value) == 0)
		{
			decompositionCount = 1;
		}
		else if (int.Parse(input.value) > selectedItemCount)
		{
			decompositionCount = selectedItemCount;
		}
		else
		{
			decompositionCount = int.Parse(input.value);
		}
		if (decompositionCount > 9999)
		{
			decompositionCount = 9999;
		}
		SetValue();
	}

	private void SetValue()
	{
		input.value = string.Empty;
		UISetter.SetLabel(inputLabel, decompositionCount);
		UISetter.SetButtonEnable(decreaseBtn, decompositionCount > 1);
		UISetter.SetButtonEnable(addBtn, decompositionCount < selectedItemCount && decompositionCount < 9999);
		UISetter.SetLabel(de_count, decompositionCount);
	}

	public void ClosePopup()
	{
		SoundManager.PlaySFX("BTN_Negative_001");
		UISetter.SetActive(this, active: false);
	}
}
