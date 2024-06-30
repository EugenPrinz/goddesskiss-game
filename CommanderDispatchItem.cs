using UnityEngine;

public class CommanderDispatchItem : UITroop
{
	private RoTroop troop;

	private EBattleType type;

	public GameObject emptyRoot;

	public GameObject dispatchRoot;

	public GameObject vipRoot;

	[SerializeField]
	private UILabel earnGold;

	[SerializeField]
	private UILabel time;

	[SerializeField]
	private GameObject Open_obj;

	[SerializeField]
	private GameObject Close_obj;

	[SerializeField]
	private UICommander commander;

	public void SetItem(RoTroop _troop, EBattleType _type)
	{
		type = _type;
		UISetter.SetActive(emptyRoot, _troop == null);
		UISetter.SetActive(dispatchRoot, _troop != null);
		if (_troop != null)
		{
			troop = _troop;
			Set(troop);
		}
	}

	public void SetItem(Protocols.DiapatchCommanderInfo dispatchCommanderInfo)
	{
		if (dispatchCommanderInfo != null)
		{
			RoLocalUser localUser = RemoteObjectManager.instance.localUser;
			int num = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["VIPGRADE_DISPATCH_GOLD"].value);
			UISetter.SetActive(Close_obj, active: false);
			UISetter.SetActive(Open_obj, active: true);
			UISetter.SetActive(vipRoot, localUser.vipLevel >= num);
			RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommander(dispatchCommanderInfo.cid.ToString());
			commander.SetCommander_OnlyDispatch(roCommander);
			roCommander.isDispatch = true;
			earnGold.text = dispatchCommanderInfo.getGold.ToString();
			UISetter.SetLabel(time, Utility.GetTimeString(dispatchCommanderInfo.runtime));
		}
	}

	public void CloseSlot()
	{
		UISetter.SetActive(Close_obj, active: true);
		UISetter.SetActive(Open_obj, active: false);
	}

	public void OnChangeTroopClicked()
	{
	}

	public void OnEditTroopClicked()
	{
	}

	public void OnClearClicked()
	{
		RemoteObjectManager.instance.RequestUpdateTroopRole(troop.commanderId, "N");
	}

	public void ClickAddCommander(GameObject sender)
	{
		if (UIManager.instance.world.guild.dispatch.selectPopup != null && UIManager.instance.world.guild.dispatch.selectPopup.isActive)
		{
			return;
		}
		int slotNum = 0;
		UISelectDispatchPopup selectDispatchPopup = UIManager.instance.world.guild.dispatch.GetSelectDispatchPopup();
		if (selectDispatchPopup != null)
		{
			switch (sender.name)
			{
			case "Troop_1":
				slotNum = 1;
				break;
			case "Troop_2":
				slotNum = 2;
				break;
			}
			selectDispatchPopup.InitListView(slotNum);
		}
	}

	public void ClickReturnCommander(GameObject sender)
	{
		int slotIdx = 0;
		switch (sender.name)
		{
		case "Troop_1":
			slotIdx = 1;
			break;
		case "Troop_2":
			slotIdx = 2;
			break;
		}
		RemoteObjectManager.instance.RequestRecallCommander(slotIdx);
	}
}
