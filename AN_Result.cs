public class AN_Result
{
	protected bool _IsSucceeded = true;

	public bool IsSucceeded => _IsSucceeded;

	public bool IsFailed => !_IsSucceeded;

	public AN_Result(bool IsResultSucceeded)
	{
		_IsSucceeded = IsResultSucceeded;
	}
}
