public class GK_TBM_EndTrunResult : ISN_Result
{
	private GK_TBM_Match _Match;

	public GK_TBM_Match Match => _Match;

	public GK_TBM_EndTrunResult(GK_TBM_Match match)
		: base(IsResultSucceeded: true)
	{
		_Match = match;
	}

	public GK_TBM_EndTrunResult(string errorData)
		: base(errorData)
	{
	}
}
