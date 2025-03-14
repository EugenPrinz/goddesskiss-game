public class GK_LeaderboardResult : ISN_Result
{
	private GK_Leaderboard _Leaderboard;

	public GK_Leaderboard Leaderboard => _Leaderboard;

	public GK_LeaderboardResult(GK_Leaderboard leaderboard)
		: base(IsResultSucceeded: true)
	{
		Setinfo(leaderboard);
	}

	public GK_LeaderboardResult(GK_Leaderboard leaderboard, string errorData)
		: base(errorData)
	{
		Setinfo(leaderboard);
	}

	private void Setinfo(GK_Leaderboard leaderboard)
	{
		_Leaderboard = leaderboard;
	}
}
