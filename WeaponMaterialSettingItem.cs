using System.Collections.Generic;
using Shared.Regulation;

public class WeaponMaterialSettingItem : UIItemBase
{
	public UISprite icon;

	public new UILabel name;

	public List<UILabel> countList;

	public void Set(GoodsDataRow row)
	{
		UISetter.SetSprite(icon, row.iconId);
		UISetter.SetLabel(name, Localization.Get(row.name));
	}

	public void SetMaterial(int count)
	{
		int num = count / 100;
		int num2 = count % 100 / 10;
		int num3 = count % 100 % 10;
		UISetter.SetLabel(countList[0], num);
		UISetter.SetLabel(countList[1], num2);
		UISetter.SetLabel(countList[2], num3);
	}
}
