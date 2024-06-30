using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Shared.Regulation;

public class RoExplorationTable : IEnumerable<RoExploration>, IEnumerable
{
	private List<RoExploration> _dataRows;

	private Dictionary<string, int> _indexMap;

	public bool hasCompleteState
	{
		get
		{
			for (int i = 0; i < length; i++)
			{
				if (this[i].state == EExplorationState.Complete)
				{
					return true;
				}
			}
			return false;
		}
	}

	public RoExploration this[int index] => _dataRows[index];

	public RoExploration this[string key] => _dataRows[FindIndex(key)];

	public int length => _dataRows.Count;

	public void Init()
	{
		_dataRows = new List<RoExploration>();
		_indexMap = new Dictionary<string, int>();
		Regulation regulation = RemoteObjectManager.instance.regulation;
		for (int i = 0; i < regulation.explorationDtbl.length; i++)
		{
			ExplorationDataRow explorationDataRow = regulation.explorationDtbl[i];
			if (!_indexMap.ContainsKey(explorationDataRow.worldMap))
			{
				RoExploration roExploration = new RoExploration();
				roExploration.mapIdx = _dataRows.Count;
				roExploration.types.Add(explorationDataRow);
				int index = int.Parse(roExploration.mapID);
				roExploration.worldMap = RemoteObjectManager.instance.localUser.worldMapList[index];
				_indexMap.Add(explorationDataRow.worldMap, _dataRows.Count);
				_dataRows.Add(roExploration);
			}
			else
			{
				int index2 = _indexMap[explorationDataRow.worldMap];
				_dataRows[index2].types.Add(explorationDataRow);
			}
		}
	}

	public IEnumerator<RoExploration> GetEnumerator()
	{
		return _dataRows.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool ContainsKey(string key)
	{
		return FindIndex(key) >= 0;
	}

	public bool ContainsKey(int key)
	{
		return FindIndex(key.ToString()) >= 0;
	}

	public int FindIndex(string key)
	{
		if (!_indexMap.TryGetValue(key, out var value))
		{
			return -1;
		}
		return value;
	}

	public bool IsValidIndex(int idx)
	{
		return idx >= 0 && idx < _dataRows.Count;
	}

	public JArray ToJsonArray()
	{
		return JArray.FromObject(_dataRows);
	}

	public void ForEach(Action<RoExploration> action)
	{
		_dataRows.ForEach(action);
	}

	public RoExploration Find(Predicate<RoExploration> match)
	{
		return _dataRows.Find(match);
	}

	public List<RoExploration> FindAll(Predicate<RoExploration> match)
	{
		return _dataRows.FindAll(match);
	}

	public void RemoveDispatchCommanders()
	{
		for (int i = 0; i < _dataRows.Count; i++)
		{
			_dataRows[i].RemoveDispatchCommanders();
		}
	}
}
