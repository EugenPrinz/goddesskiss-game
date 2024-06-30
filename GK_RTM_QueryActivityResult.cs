public class GK_RTM_QueryActivityResult : ISN_Result
{
	private int _Activity;

	public int Activity => _Activity;

	public GK_RTM_QueryActivityResult(int activity)
		: base(IsResultSucceeded: true)
	{
		_Activity = activity;
	}

	public GK_RTM_QueryActivityResult(string errorData)
		: base(errorData)
	{
	}
}
