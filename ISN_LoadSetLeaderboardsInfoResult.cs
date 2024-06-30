public class ISN_LoadSetLeaderboardsInfoResult : ISN_Result
{
	public GK_LeaderboardSet _LeaderBoardsSet;

	public GK_LeaderboardSet LeaderBoardsSet => _LeaderBoardsSet;

	public ISN_LoadSetLeaderboardsInfoResult(GK_LeaderboardSet lbset, bool IsResultSucceeded)
		: base(IsResultSucceeded)
	{
		_LeaderBoardsSet = lbset;
	}
}
