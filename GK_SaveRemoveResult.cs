public class GK_SaveRemoveResult : ISN_Result
{
	private string _SaveName = string.Empty;

	public string SaveName => _SaveName;

	public GK_SaveRemoveResult(string name)
		: base(IsResultSucceeded: true)
	{
		_SaveName = name;
	}

	public GK_SaveRemoveResult(string name, string errorData)
		: base(errorData)
	{
		_SaveName = name;
	}
}
