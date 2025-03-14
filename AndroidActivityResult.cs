using System;

public class AndroidActivityResult
{
	protected AdroidActivityResultCodes _code;

	protected int _requestId;

	public AdroidActivityResultCodes code => _code;

	public int requestId => _requestId;

	public bool IsSucceeded
	{
		get
		{
			if (code == AdroidActivityResultCodes.RESULT_OK)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsFailed => !IsSucceeded;

	public AndroidActivityResult(string rId, string codeString)
	{
		_requestId = Convert.ToInt32(rId);
		_code = (AdroidActivityResultCodes)Convert.ToInt32(codeString);
	}
}
