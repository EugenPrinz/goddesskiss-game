using Shared.Regulation;
using UnityEngine;

public class CarnivalTabListItem : UIItemBase
{
	public UILabel name1;

	public UILabel name2;

	public UIFlipSwitch flip;

	public GameObject star;

	private string id;

	private RoLocalUser localUser;

	public override void SetSelection(bool selected)
	{
		base.SetSelection(selected);
		if (selected)
		{
			localUser.SetCarnivalIdx(id);
			Set(id);
		}
		flip.Set(selected ? SwitchStatus.ON : SwitchStatus.OFF);
	}

	public void Set(string _id)
	{
		id = _id;
		localUser = RemoteObjectManager.instance.localUser;
		CarnivalTypeDataRow carnivalTypeDataRow = RemoteObjectManager.instance.regulation.carnivalTypeDtbl[id];
		UISetter.SetLabel(name1, Localization.Get(carnivalTypeDataRow.name));
		UISetter.SetLabel(name2, Localization.Get(carnivalTypeDataRow.name));
		UISetter.SetActive(star, localUser.isNewCarnival(id) || localUser.isExistCarnivalComplete(id));
	}
}
