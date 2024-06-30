using System;
using System.Collections.Generic;
using Shared.Regulation;

public class RoExploration
{
	public int mapIdx;

	public int idx;

	private EExplorationState _state;

	public Protocols.ExplorationData data;

	public RoWorldMap worldMap;

	public List<RoCommander> commanders;

	public List<ExplorationDataRow> types;

	public Action OnComplete;

	public string mapID => Dr.worldMap;

	public ExplorationDataRow Dr => types[idx];

	public EExplorationState state
	{
		get
		{
			if (_state == EExplorationState.Exploring && data.remainTime == 0.0)
			{
				_state = EExplorationState.Complete;
			}
			return _state;
		}
		set
		{
			if (_state != value)
			{
				_state = value;
				if (_state == EExplorationState.Complete && OnComplete != null)
				{
					OnComplete();
				}
			}
		}
	}

	public bool isOpen
	{
		get
		{
			int num = int.Parse(mapID);
			int num2 = (RemoteObjectManager.instance.localUser.lastClearStage - ConstValue.tutorialMaximumStage) / 20;
			return num2 >= num;
		}
	}

	public string mapName => worldMap.name;

	public RoExploration()
	{
		idx = 0;
		_state = EExplorationState.Idle;
		commanders = new List<RoCommander>();
		types = new List<ExplorationDataRow>();
	}

	public void Set(Protocols.ExplorationData info)
	{
		if (data != null)
		{
			for (int i = 0; i < commanders.Count; i++)
			{
				commanders[i].isExploration = false;
			}
		}
		data = info;
		if (data != null)
		{
			commanders = new List<RoCommander>();
			if (data.remainTime > 0.0)
			{
				_state = EExplorationState.Exploring;
			}
			else
			{
				_state = EExplorationState.Complete;
			}
			for (int j = 0; j < data.cids.Count; j++)
			{
				RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommander(data.cids[j]);
				if (roCommander != null)
				{
					roCommander.isExploration = true;
					commanders.Add(roCommander);
				}
			}
			idx = types.FindIndex((ExplorationDataRow x) => x.idx == info.idx);
		}
		else
		{
			_state = EExplorationState.Idle;
			for (int k = 0; k < commanders.Count; k++)
			{
				commanders[k].isExploration = false;
			}
		}
	}

	public void RemoveDispatchCommanders()
	{
		if (_state != 0)
		{
			return;
		}
		List<RoCommander> list = new List<RoCommander>();
		for (int i = 0; i < commanders.Count; i++)
		{
			if (!commanders[i].isDispatch)
			{
				list.Add(commanders[i]);
			}
		}
		commanders = list;
	}

	public void RemoveCommander(RoCommander target)
	{
		if (state == EExplorationState.Idle)
		{
			int num = commanders.FindIndex((RoCommander commander) => commander.id == target.id);
			if (num >= 0)
			{
				commanders.RemoveAt(num);
			}
		}
	}

	public void RemoveCommanders(List<RoCommander> target)
	{
		if (target != null && state == EExplorationState.Idle)
		{
			for (int i = 0; i < target.Count; i++)
			{
				RemoveCommander(target[i]);
			}
		}
	}
}
