public class ISN_Result
{
	protected ISN_Error _Error;

	protected bool _IsSucceeded = true;

	public bool IsSucceeded => _IsSucceeded;

	public bool IsFailed => !_IsSucceeded;

	public ISN_Error Error => _Error;

	public ISN_Result(bool IsResultSucceeded)
	{
		_IsSucceeded = IsResultSucceeded;
	}

	public ISN_Result(ISN_Error e)
	{
		SetError(e);
	}

	public ISN_Result(string errorData)
	{
		ISN_Error error = new ISN_Error(errorData);
		SetError(error);
	}

	public void SetError(ISN_Error e)
	{
		_Error = e;
		_IsSucceeded = false;
	}
}
