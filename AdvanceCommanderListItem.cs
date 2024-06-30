using UnityEngine;

public class AdvanceCommanderListItem : UICommander
{
	public GameObject vaildRoot;

	public GameObject dieRoot;

	public UILabel dieLabel;

	public GameObject checkRoot;

	private RoCommander currCommander;

	[SerializeField]
	private UISelectedSlot selectCommander;

	[SerializeField]
	private GameObject SelectCommanderSlot;

	public void SetItem(RoCommander commander)
	{
		commander?.DeleteCurrLevelUnitReg();
		UISetter.SetGameObjectName(vaildRoot, $"AddCommander-{commander.id}");
		UISetter.SetGameObjectName(checkRoot, $"RemoveCommander-{commander.id}");
		UISetter.SetGameObjectName(dieRoot, $"DieCommander-{commander.id}");
		UISetter.SetActive(checkRoot, commander.isAdvance);
		UISetter.SetActive(dieRoot, commander.isDie);
		Set(commander);
		currCommander = commander;
	}

	public void DispatchSetItem(RoCommander commander)
	{
		UISetter.SetGameObjectName(vaildRoot, $"AddCommander-{commander.id}");
		UISetter.SetGameObjectName(checkRoot, $"RemoveCommander-{commander.id}");
		UISetter.SetActive(dieRoot, commander.isDispatch || commander.isExploration);
		if (commander.isExploration || commander.isDispatch)
		{
			UISetter.SetActive(dieRoot, active: true);
			UISetter.SetLabel(dieLabel, (!commander.isExploration) ? Localization.Get("110075") : Localization.Get("5080003"));
		}
		else
		{
			UISetter.SetActive(dieRoot, active: false);
		}
		Set(commander);
		currCommander = commander;
	}

	public override void SetSelection(bool selected)
	{
		if (!(selectCommander == null))
		{
			UISetter.SetActive(checkRoot, selected);
			if (selected)
			{
				selectCommander.SetValidSlotRoot(currCommander);
			}
			else
			{
				selectCommander.SetValidSlotRoot(null);
			}
		}
	}
}
