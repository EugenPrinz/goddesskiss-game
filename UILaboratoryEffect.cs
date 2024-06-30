using System.Collections;
using UnityEngine;

public class UILaboratoryEffect : MonoBehaviour
{
	[SerializeField]
	private GameObject UP_AtkEff;

	[SerializeField]
	private GameObject UP_DefEff;

	[SerializeField]
	private GameObject UP_AccEff;

	[SerializeField]
	private GameObject UP_LuckEff;

	[SerializeField]
	private GameObject DE_AtkEff;

	[SerializeField]
	private GameObject DE_DefEff;

	[SerializeField]
	private GameObject DE_AccEFf;

	[SerializeField]
	private GameObject DE_LuckEff;

	[SerializeField]
	private GameObject DE_AtkEffSlot;

	[SerializeField]
	private GameObject DE_DefEffSlot;

	[SerializeField]
	private GameObject DE_AccEffSlot;

	[SerializeField]
	private GameObject DE_LuckEFfSlot;

	public void OffEffect()
	{
		UISetter.SetActive(UP_AtkEff, active: false);
		UISetter.SetActive(UP_DefEff, active: false);
		UISetter.SetActive(UP_AccEff, active: false);
		UISetter.SetActive(UP_LuckEff, active: false);
		UISetter.SetActive(DE_AtkEff, active: false);
		UISetter.SetActive(DE_DefEff, active: false);
		UISetter.SetActive(DE_AccEFf, active: false);
		UISetter.SetActive(DE_LuckEff, active: false);
		UISetter.SetActive(DE_AtkEffSlot, active: false);
		UISetter.SetActive(DE_DefEffSlot, active: false);
		UISetter.SetActive(DE_AccEffSlot, active: false);
		UISetter.SetActive(DE_LuckEFfSlot, active: false);
	}

	public void SetEffect(CurTabType tabType, EItemStatType itemType)
	{
		switch (tabType)
		{
		case CurTabType.Upgrade:
			UpgradeEffect(itemType);
			break;
		case CurTabType.Decomposition:
			DecompositionEffect(itemType);
			break;
		}
	}

	private void UpgradeEffect(EItemStatType itemType)
	{
		UISetter.SetActive(UP_AtkEff, active: false);
		UISetter.SetActive(UP_DefEff, active: false);
		UISetter.SetActive(UP_AccEff, active: false);
		UISetter.SetActive(UP_LuckEff, active: false);
		switch (itemType)
		{
		case EItemStatType.ATK:
			UISetter.SetActive(UP_AtkEff, active: true);
			break;
		case EItemStatType.DEF:
			UISetter.SetActive(UP_DefEff, active: true);
			break;
		case EItemStatType.ACCUR:
			UISetter.SetActive(UP_AccEff, active: true);
			break;
		case EItemStatType.LUCK:
			UISetter.SetActive(UP_LuckEff, active: true);
			break;
		}
	}

	private void DecompositionEffect(EItemStatType itemType)
	{
		UISetter.SetActive(DE_AtkEff, active: false);
		UISetter.SetActive(DE_DefEff, active: false);
		UISetter.SetActive(DE_AccEFf, active: false);
		UISetter.SetActive(DE_LuckEff, active: false);
		switch (itemType)
		{
		case EItemStatType.ATK:
			UISetter.SetActive(DE_AtkEff, active: true);
			break;
		case EItemStatType.DEF:
			UISetter.SetActive(DE_DefEff, active: true);
			break;
		case EItemStatType.ACCUR:
			UISetter.SetActive(DE_AccEFf, active: true);
			break;
		case EItemStatType.LUCK:
			UISetter.SetActive(DE_LuckEff, active: true);
			break;
		}
	}

	public IEnumerator DecompositionSlotEffect(EItemStatType itemType)
	{
		switch (itemType)
		{
		case EItemStatType.ATK:
			UISetter.SetActive(DE_AtkEffSlot, active: true);
			break;
		case EItemStatType.DEF:
			UISetter.SetActive(DE_DefEffSlot, active: true);
			break;
		case EItemStatType.ACCUR:
			UISetter.SetActive(DE_AccEffSlot, active: true);
			break;
		case EItemStatType.LUCK:
			UISetter.SetActive(DE_LuckEFfSlot, active: true);
			break;
		}
		yield return new WaitForSeconds(1.2f);
		UISetter.SetActive(DE_AtkEffSlot, active: false);
		UISetter.SetActive(DE_DefEffSlot, active: false);
		UISetter.SetActive(DE_AccEffSlot, active: false);
		UISetter.SetActive(DE_LuckEFfSlot, active: false);
	}
}
