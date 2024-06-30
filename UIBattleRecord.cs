using System.Collections.Generic;
using Shared.Regulation;

public class UIBattleRecord : UIPanelBase
{
	public UILabel accrueGold;

	public UILabel nomalBox;

	public UILabel PremiumBox;

	public UILabel haveCommander;

	public UILabel mostPower;

	public UILabel totalPower;

	public UILabel arenaCount;

	public UILabel arenaGrade;

	public UILabel raidPoint;

	public UILabel raidGrade;

	public UILabel stageClear;

	public UILabel defeadUnit;

	private RoLocalUser _currUser;

	public void Set(RoLocalUser user)
	{
		_currUser = user;
		RoStatistics statistics = user.statistics;
		UISetter.SetLabel(accrueGold, $"{statistics.totalGold:N0}");
		UISetter.SetLabel(nomalBox, Localization.Format("14902", statistics.normalGachaCount));
		UISetter.SetLabel(PremiumBox, Localization.Format("14903", statistics.premiumGachaCount));
		UISetter.SetLabel(haveCommander, user.GetCommanderCount() + " / " + user.commanderList.Count);
		int num = 0;
		int num2 = 0;
		List<RoCommander> commanderList = user.GetCommanderList(EBranch.Army, have: true);
		commanderList.Sort((RoCommander row, RoCommander row1) => row1.currLevelUnitReg.GetTotalPower().CompareTo(row.currLevelUnitReg.GetTotalPower()));
		for (int i = 0; i < commanderList.Count; i++)
		{
			if (i < 5)
			{
				num2 += commanderList[i].currLevelUnitReg.GetTotalPower();
			}
			num += commanderList[i].currLevelUnitReg.GetTotalPower();
		}
		UISetter.SetLabel(mostPower, Localization.Get("14906") + $"{num2:N0}");
		UISetter.SetLabel(totalPower, Localization.Get("1257") + ":" + $"{num:N0}");
		float num3 = statistics.pvpWinCount + statistics.pvpLoseCount;
		float num4 = (float)statistics.pvpWinCount / num3 * 100f;
		if (num3 == 0f)
		{
			num4 = 0f;
		}
		UISetter.SetLabel(arenaCount, Localization.Format("14907", num3, $"{num4:N1}"));
		if (statistics.arenaHighRank > 0)
		{
			RankingDataRow rankingDataRow = RemoteObjectManager.instance.regulation.rankingDtbl[statistics.arenaHighRank.ToString()];
			UISetter.SetLabel(arenaGrade, Localization.Get("14908") + ":" + Localization.Get(rankingDataRow.name));
		}
		else
		{
			UISetter.SetLabel(arenaGrade, Localization.Get("14908") + ":" + Localization.Get("14912"));
		}
		UISetter.SetLabel(raidPoint, Localization.Get("5465") + ":" + $"{statistics.raidHighScore:N0}");
		if (statistics.raidHighRank > 0)
		{
			RankingDataRow rankingDataRow2 = RemoteObjectManager.instance.regulation.rankingDtbl[statistics.raidHighRank.ToString()];
			UISetter.SetLabel(raidGrade, Localization.Get("14908") + ":" + Localization.Get(rankingDataRow2.name));
		}
		else
		{
			UISetter.SetLabel(raidGrade, Localization.Get("14908") + ":" + Localization.Get("14912"));
		}
		UISetter.SetLabel(stageClear, Localization.Format("14910", statistics.stageClearCount + statistics.sweepClearCount));
		UISetter.SetLabel(defeadUnit, Localization.Format("14911", statistics.unitDestroyCount));
	}
}
