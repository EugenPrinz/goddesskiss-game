using Shared.Regulation;
using UnityEngine;

public class PreDeckListItem : UITroop
{
	public GameObject lockRoot;

	public UILabel cost;

	public GameObject selectRoot;

	public void Set(RoTroop troop, int index)
	{
		Set(troop);
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		int num = int.Parse(regulation.defineDtbl["BASE_DECK_COUNT"].value);
		int num2 = int.Parse(regulation.defineDtbl["DECK_PLUS_CASH"].value);
		int num3 = int.Parse(regulation.defineDtbl["DECK_PLUS_CASH_VALUE"].value);
		int predeckCount = localUser.statistics.predeckCount;
		UISetter.SetActive(lockRoot, index + 1 > predeckCount);
		UISetter.SetLabel(cost, (num2 + num3 * (predeckCount - num)).ToString("N0"));
	}

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(selectRoot, selected);
	}
}
