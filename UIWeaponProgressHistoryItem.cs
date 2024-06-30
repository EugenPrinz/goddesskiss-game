using System.Collections.Generic;
using Shared.Regulation;

public class UIWeaponProgressHistoryItem : UIItemBase
{
	public UILabel lbName;

	public UILabel level;

	public UISprite userIcon;

	public UISprite weaponIcon;

	public UISprite weaponBg;

	public UILabel meterialCount1;

	public UILabel meterialCount2;

	public UILabel meterialCount3;

	public UILabel meterialCount4;

	public UIGrid grade;

	public void Set(List<string> list)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetLabel(lbName, list[0]);
		UISetter.SetLabel(level, Localization.Format("1021", list[1]));
		UISetter.SetSprite(userIcon, regulation.GetCostumeThumbnailName(int.Parse(list[2])));
		WeaponDataRow weaponDataRow = regulation.weaponDtbl[list[3]];
		UISetter.SetSprite(weaponIcon, weaponDataRow.weaponIcon);
		UISetter.SetLabel(meterialCount1, list[4]);
		UISetter.SetLabel(meterialCount2, list[5]);
		UISetter.SetLabel(meterialCount3, list[6]);
		UISetter.SetLabel(meterialCount4, list[7]);
		UISetter.SetRank(grade, weaponDataRow.weaponGrade);
		UISetter.SetSprite(weaponBg, $"weaponback_{weaponDataRow.weaponGrade}");
	}
}
