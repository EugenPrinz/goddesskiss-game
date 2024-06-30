using Shared.Regulation;
using UnityEngine;

public class GroupTapListItem : UIItemBase
{
	public UILabel name1;

	public UILabel name2;

	public UIFlipSwitch flip;

	public GameObject star;

	private string id;

	public override void SetSelection(bool selected)
	{
		base.SetSelection(selected);
		if (selected)
		{
		}
		flip.Set(selected ? SwitchStatus.ON : SwitchStatus.OFF);
	}

	public void Set(string _id)
	{
		id = _id;
		GroupInfoDataRow groupInfoDataRow = RemoteObjectManager.instance.regulation.FindGroupInfo(id);
		UISetter.SetLabel(name1, Localization.Get(groupInfoDataRow.tabname));
		UISetter.SetLabel(name2, Localization.Get(groupInfoDataRow.tabname));
		int num = RemoteObjectManager.instance.localUser.RewardGroupCountInTap(id);
		UISetter.SetActive(star, num > 0);
		RemoteObjectManager.instance.localUser.badgeGroupCount += num;
	}
}
