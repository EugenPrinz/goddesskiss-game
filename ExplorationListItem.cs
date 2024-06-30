using Shared.Regulation;
using UnityEngine;

public class ExplorationListItem : MonoBehaviour
{
	public UILabel mapName;

	public UISprite bg;

	public Color openBgColor;

	public Color closeBgColor;

	public GameObject readyView;

	public UILabel condition;

	public GameObject exploringView;

	public UILabel explorationLabel;

	public UITimer remainTimer;

	public UIDefaultListView commanderListView;

	public GameObject completeView;

	public UILabel completeLabel;

	public GameObject closeView;

	public UISprite ememyCommander;

	private RoExploration _exploration;

	protected EExplorationState state => _exploration.state;

	protected RoLocalUser localUser => RemoteObjectManager.instance.localUser;

	public void Set(RoExploration exploration)
	{
		_exploration = exploration;
		if (_exploration.isOpen)
		{
			int num = 8900 + _exploration.Dr.minClass;
			UISetter.SetLabel(condition, string.Format(Localization.Get("5080001"), Localization.Get(num.ToString())));
			UISetter.SetActive(closeView, active: false);
		}
		else
		{
			UISetter.SetLabel(condition, string.Format(Localization.Get("5080017"), Localization.Get(_exploration.mapName)));
			CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[_exploration.worldMap.dataRow.c_idx];
			UISetter.SetSprite(ememyCommander, commanderDataRow.worldMapRewardId);
			UISetter.SetActive(closeView, active: true);
		}
		UISetter.SetLabel(explorationLabel, string.Format("{0} {1}", Localization.Get(_exploration.mapName), Localization.Get("5080003")));
		UISetter.SetLabel(completeLabel, string.Format("{0} {1}", Localization.Get(_exploration.mapName), Localization.Get("5080004")));
		UISetter.SetLabel(mapName, Localization.Get(_exploration.mapName));
		UISetter.SetSprite(bg, _exploration.worldMap.dataRow.listImg);
		OnRefresh();
	}

	public void OnRefresh()
	{
		switch (state)
		{
		case EExplorationState.Idle:
			UISetter.SetActive(readyView, active: true);
			UISetter.SetActive(exploringView, active: false);
			UISetter.SetActive(completeView, active: false);
			UISetter.SetColor(bg, (!_exploration.isOpen) ? closeBgColor : openBgColor);
			remainTimer.Stop();
			if (commanderListView != null)
			{
				commanderListView.ResizeItemList(0);
			}
			break;
		case EExplorationState.Exploring:
		{
			UISetter.SetActive(readyView, active: false);
			UISetter.SetActive(exploringView, active: true);
			UISetter.SetActive(completeView, active: false);
			if (commanderListView != null)
			{
				commanderListView.ResizeItemList(5);
				for (int i = 0; i < commanderListView.itemList.Count; i++)
				{
					UICommander uICommander = commanderListView.itemList[i] as UICommander;
					if (!(uICommander == null))
					{
						if (i < _exploration.commanders.Count)
						{
							uICommander.Set(_exploration.commanders[i]);
							UISetter.SetActive(uICommander.validSlotRoot, active: true);
						}
						else
						{
							UISetter.SetActive(uICommander.validSlotRoot, active: false);
						}
						uICommander.SetSelection(selected: false);
					}
				}
			}
			TimeData timeData = TimeData.Create();
			timeData.SetByDuration(_exploration.data.remainTime);
			UISetter.SetTimer(remainTimer, timeData, "1048");
			remainTimer.RegisterOnFinished(delegate
			{
				OnRefresh();
			});
			break;
		}
		case EExplorationState.Complete:
			UISetter.SetActive(readyView, active: false);
			UISetter.SetActive(exploringView, active: false);
			UISetter.SetActive(completeView, active: true);
			break;
		}
	}

	public void Complete()
	{
		RemoteObjectManager.instance.RequestExplorationComplete(_exploration.Dr.idx);
	}
}
