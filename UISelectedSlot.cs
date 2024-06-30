using UnityEngine;

public class UISelectedSlot : MonoBehaviour
{
	[SerializeField]
	private UICommander commander;

	[SerializeField]
	private GameObject SelectCommanderSlot;

	[SerializeField]
	private UILabel EarnGold;

	[SerializeField]
	private UISelectDispatchPopup curPopup;

	[SerializeField]
	private UILabel EngageGold;

	public GameObject vipRoot;

	private RoCommander ro_commander;

	public void SetValidSlotRoot(RoCommander _commander)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		int num = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["VIPGRADE_DISPATCH_GOLD"].value);
		UISetter.SetActive(vipRoot, localUser.vipLevel >= num);
		if (_commander == null)
		{
			UISetter.SetActive(SelectCommanderSlot, active: false);
			UISetter.SetLabel(EarnGold, 0);
			UISetter.SetLabel(EngageGold, 0);
			ro_commander = null;
		}
		else
		{
			UISetter.SetActive(SelectCommanderSlot, active: true);
			commander.Set(_commander);
			ro_commander = _commander;
			int num2 = (int)(ro_commander.GetdispatchFloatGold * 10f);
			UISetter.SetLabel(EarnGold, ro_commander.GetdispatchGold);
			UISetter.SetLabel(EngageGold, num2.ToString("N0"));
		}
	}

	public void DispatchCommander()
	{
		if (ro_commander == null)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110077"));
			return;
		}
		RemoteObjectManager.instance.RequestDispatchCommander(int.Parse(ro_commander.id), curPopup.curSlotNum);
		curPopup.ClosePopup();
	}
}
