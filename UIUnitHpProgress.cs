public class UIUnitHpProgress : UIProgressBar
{
	private const float dangerValue = 0.3f;

	private const string baseBackgroundSpriteName = "com_gauge_box";

	private const string baseForegroundSpriteName = "com_gauge_nm";

	private const string dangerBackgroundSpriteName = "unit_gauge_bg_hp_danger";

	private const string dangerForegroundSpriteName = "unit_gauge_hp_danger";

	public override void ForceUpdate()
	{
		if (base.value <= 0.3f)
		{
			UISprite uISprite = base.backgroundWidget as UISprite;
			uISprite.spriteName = "unit_gauge_bg_hp_danger";
			uISprite = base.foregroundWidget as UISprite;
			uISprite.spriteName = "unit_gauge_hp_danger";
		}
		else
		{
			UISprite uISprite = base.backgroundWidget as UISprite;
			uISprite.spriteName = "com_gauge_box";
			uISprite = base.foregroundWidget as UISprite;
			uISprite.spriteName = "com_gauge_nm";
		}
		base.ForceUpdate();
	}
}
