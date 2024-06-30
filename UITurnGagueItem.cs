public class UITurnGagueItem : UIItemBase
{
	public enum EType
	{
		Default,
		Raid
	}

	public UILabel label;

	public UISprite icon;

	protected EType type;

	protected int index;
}
