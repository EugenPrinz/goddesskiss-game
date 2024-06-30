public class GK_SaveResult : ISN_Result
{
	private GK_SavedGame _SavedGame;

	public GK_SavedGame SavedGame => _SavedGame;

	public GK_SaveResult(GK_SavedGame save)
		: base(IsResultSucceeded: true)
	{
		_SavedGame = save;
	}

	public GK_SaveResult(string errorData)
		: base(errorData)
	{
	}
}
