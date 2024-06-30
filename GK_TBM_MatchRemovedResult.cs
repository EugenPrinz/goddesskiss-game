public class GK_TBM_MatchRemovedResult : ISN_Result
{
	private string _MatchId;

	public string MatchId => _MatchId;

	public GK_TBM_MatchRemovedResult(string matchId)
		: base(IsResultSucceeded: true)
	{
		_MatchId = matchId;
	}

	public GK_TBM_MatchRemovedResult()
		: base(IsResultSucceeded: false)
	{
	}
}
