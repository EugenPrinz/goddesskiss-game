public class ScrambleHistoryItem : UIItemBase
{
	public UIUser uiAttacker;

	public UIUser uiDefender;

	public UISprite attackerResult;

	public UISprite defenderrResult;

	private const string WinSpriteName = "alliance_win";

	private const string LoseSpriteName = "alliance_lose";

	public void Set(Protocols.ScrambleMapHistory history)
	{
	}
}
