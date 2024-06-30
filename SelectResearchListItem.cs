using Shared.Regulation;
using UnityEngine;

public class SelectResearchListItem : UIItemBase
{
	public EBuilding type;

	public GameObject Lock;

	public UILabel locklevel;

	private int userLevel => RemoteObjectManager.instance.localUser.level;

	private Regulation regulation => RemoteObjectManager.instance.regulation;

	private void Start()
	{
		BuildingLevelDataRow buildingLevelDataRow = regulation.buildingLevelDtbl.Find((BuildingLevelDataRow row) => row.type == type);
		if (buildingLevelDataRow == null)
		{
			UISetter.SetActive(Lock, active: true);
			UISetter.SetLabel(locklevel, Localization.Format("5040004", 999));
			return;
		}
		int num = buildingLevelDataRow.userLevel;
		if (userLevel < num)
		{
			UISetter.SetActive(Lock, active: true);
			UISetter.SetLabel(locklevel, Localization.Format("5040004", num));
		}
		else
		{
			UISetter.SetActive(Lock, active: false);
		}
	}
}
