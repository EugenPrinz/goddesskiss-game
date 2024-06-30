using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class BookListItem : UICommander
{
	public UILabel count;

	public UIProgressBar progressBar;

	public GameObject LockBg;

	private CommanderDataRow data;

	private RoCommander commander;

	public void Set(CommanderDataRow _data)
	{
		data = _data;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		if (IsGet())
		{
			UISetter.SetActive(LockBg, active: false);
		}
		else
		{
			UISetter.SetActive(LockBg, active: true);
		}
		int num = 0;
		num = ((commander.state == ECommanderState.Nomal) ? ((int)commander.rank) : 0);
		CommanderRankDataRow commanderRankDataRow = regulation.commanderRankDtbl[(num + 1).ToString()];
		UISetter.SetLabel(count, $"{commander.medal} / {commanderRankDataRow.medal}");
		progressBar.value = (float)commander.medal / (float)commanderRankDataRow.medal;
		Set(commander);
	}

	public bool IsGet()
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		RoCommander roCommander = localUser.FindCommander(data.id);
		if (roCommander == null)
		{
			commander = RoCommander.Create(data.id, 1, 1, 1, 0, 0, 0, new List<int>());
			return false;
		}
		if (roCommander.state == ECommanderState.Getting)
		{
			commander = roCommander;
			return false;
		}
		if (roCommander.state == ECommanderState.Nomal)
		{
			commander = roCommander;
			return true;
		}
		return false;
	}

	public void OnClick()
	{
		if (IsGet())
		{
			UIManager.instance.world.commanderDetail.InitAndOpenCard();
			UIManager.instance.world.commanderDetail.Set(commander.id, CommanderDetailType.Training);
			UIManager.instance.world.commanderDetail.OpenPopupShow();
		}
	}
}
