using Shared.Regulation;

public class UIWeaponDecompositionPopup : UISimplePopup
{
	public UICommanderWeaponItem weaponItem;

	public UISprite ticketIcon;

	public void Set(RoWeapon weapon)
	{
		weaponItem.Set(weapon);
		PartDataRow partDataRow = base.regulation.partDtbl[weapon.data.decompositionReward];
		UISetter.SetSprite(ticketIcon, partDataRow.serverFieldName);
	}
}
