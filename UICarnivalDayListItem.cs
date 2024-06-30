using UnityEngine;

public class UICarnivalDayListItem : UIItemBase
{
	public UILabel open;

	public UILabel close;

	public GameObject badge;

	public UIFlipSwitch flip;

	public void Set(string day, bool badge)
	{
		UISetter.SetLabel(open, Localization.Format("1056", day));
		UISetter.SetLabel(close, Localization.Format("1056", day));
		UISetter.SetActive(this.badge, badge);
	}

	public override void SetSelection(bool selected)
	{
		base.SetSelection(selected);
		flip.Set(selected ? SwitchStatus.ON : SwitchStatus.OFF);
	}
}
