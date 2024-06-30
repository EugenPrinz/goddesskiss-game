public class GK_TBM_MatchInitResult : ISN_Result
{
	private GK_TBM_Match _Match;

	public GK_TBM_Match Match => _Match;

	public GK_TBM_MatchInitResult(GK_TBM_Match match)
		: base(IsResultSucceeded: true)
	{
		_Match = match;
	}

	public GK_TBM_MatchInitResult(string errorData)
		: base(errorData)
	{
	}
}
