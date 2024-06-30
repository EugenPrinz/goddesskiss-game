using Shared.Regulation;

public class UICommanderInformation : UIPopup
{
	public UIStatus nowLevelStatus;

	public UIStatus nextLevelStatus;

	public UIStatus nextGradeStatus;

	public UILabel userMedal;

	public UILabel userTicket;

	public UILabel rankupGold;

	public UILabel rankupMedal;

	public UILabel trainingGold;

	public UILabel trainingTicket;

	private RoCommander commander;

	public void initData(RoCommander _commander)
	{
		commander = _commander;
		SetData();
		Open();
	}

	private void SetUserData()
	{
		UISetter.SetLabel(userMedal, base.localUser.medal);
		UISetter.SetLabel(userTicket, base.localUser.resourceList[ETrainingTicketType.ctt1.ToString()]);
	}

	public void OnTrainingBtnClicked()
	{
	}

	public void OnPromotionBtnClicked()
	{
		RemoteObjectManager.instance.RequestCommanderRankUp(commander.id);
	}

	public void SetData()
	{
		nowLevelStatus.Set(commander);
		nextLevelStatus.Set(commander.CreateNextLevel());
		nextGradeStatus.Set(commander.CreateNextRank());
		CommanderTrainingTicketDataRow commanderTrainingTicketDataRow = base.regulation.FindCommanderTrainingTicketData(ETrainingTicketType.ctt1);
		CommanderRankDataRow commanderRankDataRow = base.regulation.FindCommanderRankData(commander.level);
		UISetter.SetLabel(rankupGold, commanderRankDataRow.gold);
		UISetter.SetLabel(rankupMedal, commanderRankDataRow.medal);
		UISetter.SetLabel(trainingGold, commanderTrainingTicketDataRow.gold);
		SetUserData();
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		SetData();
	}
}
