public class GK_TBM_MatchQuitResult : ISN_Result
{
	private string _MatchId;

	public string MatchId => _MatchId;

	public GK_TBM_MatchQuitResult(string matchId)
		: base(IsResultSucceeded: true)
	{
		_MatchId = matchId;
	}

	public GK_TBM_MatchQuitResult()
		: base(IsResultSucceeded: false)
	{
	}
}
