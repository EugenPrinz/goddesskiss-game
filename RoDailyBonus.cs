using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Regulation;

[JsonObject(MemberSerialization.OptIn)]
public class RoDailyBonus
{
	private string _version { get; set; }

	[JsonProperty]
	public string version
	{
		get
		{
			return _version;
		}
		set
		{
			_version = value;
			_totalDayCount = -1;
		}
	}

	[JsonProperty]
	public int today { get; set; }

	public DailyBonusDataRow todayData => RemoteObjectManager.instance.regulation.dailyBonusDtbl.Find((DailyBonusDataRow row) => row.version == version && today == row.day);

	[JsonProperty]
	public string timeLimitString { get; set; }

	public DateTime timeLimit => Utility.ConvertToDateTime(timeLimitString);

	public DateTime lastReceiveCheckTime { get; set; }

	public bool needCheck
	{
		get
		{
			if (!_isReceived)
			{
				return true;
			}
			if (lastReceiveCheckTime.Day != DateTime.Now.Day)
			{
				return true;
			}
			return false;
		}
	}

	private bool _isReceived { get; set; }

	[JsonProperty]
	public bool isReceived
	{
		get
		{
			return _isReceived;
		}
		set
		{
			_isReceived = value;
			lastReceiveCheckTime = DateTime.Now;
		}
	}

	public int lastCompleteDay
	{
		get
		{
			if (isReceived)
			{
				return today;
			}
			return today - 1;
		}
	}

	private int _totalDayCount { get; set; }

	public int totalDayCount
	{
		get
		{
			if (_totalDayCount >= 0)
			{
				return _totalDayCount;
			}
			return GetCurrentVersionDataList()?.Count ?? 0;
		}
	}

	public List<DailyBonusDataRow> GetCurrentVersionDataList()
	{
		return RemoteObjectManager.instance.regulation.dailyBonusDtbl.FindAll((DailyBonusDataRow row) => row.version == version);
	}

	public static RoDailyBonus Create()
	{
		RoDailyBonus roDailyBonus = new RoDailyBonus();
		roDailyBonus.isReceived = false;
		return roDailyBonus;
	}
}
