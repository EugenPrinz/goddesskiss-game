using UnityEngine;

public class PvPListItem : UIItemBase
{
	public GameObject Result;

	public UIUser uiUser;

	public GameObject battleBtn;

	public void Set(EBattleType battleType, RoUser pvpUser)
	{
		bool flag = int.Parse(pvpUser.uno) >= ConstValue.NpcStartUno || pvpUser.duelRanking == 0;
		UISetter.SetActive(Result, pvpUser.pvpResult);
		if (!flag)
		{
			uiUser.Set(pvpUser);
		}
		else
		{
			uiUser.SetNpc(pvpUser);
		}
		int num = 0;
		for (int i = 0; i < pvpUser.battleTroopList.Count; i++)
		{
			num += pvpUser.battleTroopList[i].GetTotalPower(battleType);
		}
		UISetter.SetLabel(uiUser.power, Localization.Format("18004", num));
	}

	public void Set(RoUser pvpUser)
	{
		bool flag = int.Parse(pvpUser.uno) >= ConstValue.NpcStartUno || pvpUser.duelRanking == 0;
		UISetter.SetActive(Result, pvpUser.pvpResult);
		if (!flag)
		{
			uiUser.Set(pvpUser);
		}
		else
		{
			uiUser.SetNpc(pvpUser);
		}
		UISetter.SetLabel(uiUser.power, Localization.Format("18004", pvpUser.mainTroop.GetTotalPower(EBattleType.Duel)));
	}
}
