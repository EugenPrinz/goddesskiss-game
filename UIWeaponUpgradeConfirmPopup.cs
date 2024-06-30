public class UIWeaponUpgradeConfirmPopup : UISimplePopup
{
	public UISprite costIcon;

	public UILabel count;

	public void SetCost(string iconName, int count)
	{
		UISetter.SetSprite(costIcon, iconName);
		UISetter.SetLabel(this.count, count);
	}
}
