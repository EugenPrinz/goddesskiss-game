using System.Collections.Generic;

public class GK_FetchResult : ISN_Result
{
	private List<GK_SavedGame> _SavedGames = new List<GK_SavedGame>();

	public List<GK_SavedGame> SavedGames => _SavedGames;

	public GK_FetchResult(List<GK_SavedGame> saves)
		: base(IsResultSucceeded: true)
	{
		_SavedGames = saves;
	}

	public GK_FetchResult(string errorData)
		: base(errorData)
	{
	}
}
