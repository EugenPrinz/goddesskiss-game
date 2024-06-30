using UnityEngine;

public class RoulletListItem : UIItemBase
{
	public UISprite icon;

	public UISprite multipleSprite;

	public UILabel costLabel;

	public Animation itemAnimation;

	private const string RoulletIconName = "metro_gold_";

	private const string RoulletSpriteName = "metro_x";

	public void Set(int multiple)
	{
		UISetter.SetSprite(icon, string.Format("metro_gold_{0}", multiple.ToString("00")));
		UISetter.SetSprite(multipleSprite, $"metro_x{multiple}", snap: true);
	}

	public void Set(int multiple, int cost)
	{
		Set(multiple);
		UISetter.SetLabel(costLabel, cost * multiple);
	}
}
