using UnityEngine;

public class MercenaryChecker : MonoBehaviour
{
	private UICommanderSelectItem currSelectItem;

	public void SelectMercenary(UICommanderSelectItem _selectItem)
	{
		UICommanderList commanderList = UIManager.instance.world.commanderList;
		RoCommander commander = _selectItem.GetCommander();
		if (currSelectItem != null && currSelectItem == _selectItem)
		{
			commanderList.RemoveMercenary(currSelectItem.GetCommander().id);
			currSelectItem.CheckRoot(isActive: false);
			currSelectItem = null;
		}
		else
		{
			if (!commanderList.IsPossibleEngage(commander.id))
			{
				return;
			}
			if (commanderList.IsAlreadyEngageMercenary())
			{
				commanderList.RemoveMercenary(currSelectItem.GetCommander().id);
			}
			commanderList.AddMercenary(commander);
			if (currSelectItem != _selectItem)
			{
				if (currSelectItem != null)
				{
					currSelectItem.CheckRoot(isActive: false);
				}
				currSelectItem = _selectItem;
				currSelectItem.CheckRoot(isActive: true);
			}
		}
	}
}
