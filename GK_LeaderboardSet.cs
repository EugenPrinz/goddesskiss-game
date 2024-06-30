using System;
using System.Collections.Generic;

public class GK_LeaderboardSet
{
	public string Title;

	public string Identifier;

	public string GroupIdentifier;

	public List<GK_LeaderBoardInfo> _BoardsInfo = new List<GK_LeaderBoardInfo>();

	public List<GK_LeaderBoardInfo> BoardsInfo => _BoardsInfo;

	public event Action<ISN_LoadSetLeaderboardsInfoResult> OnLoaderboardsInfoLoaded = delegate
	{
	};

	public void LoadLeaderBoardsInfo()
	{
		GameCenterManager.LoadLeaderboardsForSet(Identifier);
	}

	public void AddBoardInfo(GK_LeaderBoardInfo info)
	{
		_BoardsInfo.Add(info);
	}

	public void SendFailLoadEvent()
	{
		ISN_LoadSetLeaderboardsInfoResult obj = new ISN_LoadSetLeaderboardsInfoResult(this, IsResultSucceeded: false);
		this.OnLoaderboardsInfoLoaded(obj);
	}

	public void SendSuccessLoadEvent()
	{
		ISN_LoadSetLeaderboardsInfoResult obj = new ISN_LoadSetLeaderboardsInfoResult(this, IsResultSucceeded: true);
		this.OnLoaderboardsInfoLoaded(obj);
	}
}
