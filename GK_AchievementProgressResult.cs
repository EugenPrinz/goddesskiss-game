public class GK_AchievementProgressResult : ISN_Result
{
	private GK_AchievementTemplate _tpl;

	public GK_AchievementTemplate info => _tpl;

	public GK_AchievementTemplate Achievement => _tpl;

	public GK_AchievementProgressResult(GK_AchievementTemplate tpl)
		: base(IsResultSucceeded: true)
	{
		_tpl = tpl;
	}
}
