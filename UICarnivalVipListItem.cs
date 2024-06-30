using Shared.Regulation;

public class UICarnivalVipListItem : UIItemBase
{
	public UILabel name1;

	public UILabel name2;

	public UIFlipSwitch flip;

	public void Set(CarnivalDataRow row)
	{
		int vipLevel = RemoteObjectManager.instance.localUser.vipLevel;
		UISetter.SetLabel(name1, Localization.Format("7082", row.userVip));
		UISetter.SetLabel(name2, Localization.Format("7082", row.userVip));
		UISetter.SetActive(name1, vipLevel >= row.userVip);
		UISetter.SetActive(name2, vipLevel < row.userVip);
	}

	public override void SetSelection(bool selected)
	{
		base.SetSelection(selected);
		flip.Set(selected ? SwitchStatus.ON : SwitchStatus.OFF);
	}
}
