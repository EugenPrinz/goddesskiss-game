public class GK_UserInfoLoadResult : ISN_Result
{
	private string _playerId;

	private GK_Player _tpl;

	public string playerId => _playerId;

	public GK_Player playerTemplate => _tpl;

	public GK_UserInfoLoadResult(string id)
		: base(IsResultSucceeded: false)
	{
		_playerId = id;
	}

	public GK_UserInfoLoadResult(GK_Player tpl)
		: base(IsResultSucceeded: true)
	{
		_tpl = tpl;
	}
}
