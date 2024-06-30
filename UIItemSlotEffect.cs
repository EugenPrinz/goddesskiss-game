using System.Collections;
using UnityEngine;

public class UIItemSlotEffect : MonoBehaviour
{
	[SerializeField]
	private GameObject AtkEff;

	[SerializeField]
	private GameObject DefEff;

	[SerializeField]
	private GameObject AccEff;

	[SerializeField]
	private GameObject LuckEff;

	public IEnumerator OnEffect(EItemStatType itemType)
	{
		switch (itemType)
		{
		case EItemStatType.ATK:
			UISetter.SetActive(AtkEff, active: true);
			break;
		case EItemStatType.DEF:
			UISetter.SetActive(DefEff, active: true);
			break;
		case EItemStatType.ACCUR:
			UISetter.SetActive(AccEff, active: true);
			break;
		case EItemStatType.LUCK:
			UISetter.SetActive(LuckEff, active: true);
			break;
		}
		yield return new WaitForSeconds(1f);
		OffEffect();
	}

	public void OffEffect()
	{
		UISetter.SetActive(AtkEff, active: false);
		UISetter.SetActive(DefEff, active: false);
		UISetter.SetActive(AccEff, active: false);
		UISetter.SetActive(LuckEff, active: false);
	}
}
