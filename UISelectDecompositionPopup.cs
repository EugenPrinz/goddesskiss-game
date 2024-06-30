using System.Collections.Generic;

public class UISelectDecompositionPopup : UISimplePopup
{
	public List<UILabel> weaponCountList;

	public List<UILabel> ticketCountList;

	public void Set(List<RoWeapon> weaponList)
	{
		int j;
		for (j = 0; j < weaponCountList.Count; j++)
		{
			List<RoWeapon> list = weaponList.FindAll((RoWeapon row) => row.data.weaponGrade == j + 1);
			UISetter.SetLabel(weaponCountList[j], Localization.Format("70074", list.Count));
		}
		for (int i = 0; i < ticketCountList.Count; i++)
		{
			int count = 0;
			List<RoWeapon> list2 = weaponList.FindAll((RoWeapon row) => row.data.slotType == i + 1);
			list2.ForEach(delegate(RoWeapon row)
			{
				count += row.data.value;
			});
			UISetter.SetLabel(ticketCountList[i], count);
		}
	}
}
