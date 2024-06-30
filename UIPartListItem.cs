public class UIPartListItem : UIItemBase
{
	public UILabel lbName;

	public UILabel count;

	public UISprite iconBG;

	public UISprite icon;

	public UISprite iconMark;

	public UISprite iconOutline;

	private RoPart tempPart;

	public void Set(RoPart part)
	{
		UISetter.SetLabel(lbName, Localization.Get(part.nameIdx));
		UISetter.SetLabel(count, part.count);
		UISetter.SetSprite(iconBG, part.GetPartData().bgResource);
		UISetter.SetSprite(icon, part.GetPartData().serverFieldName);
		UISetter.SetSprite(iconMark, part.GetPartData().markResource);
		UISetter.SetSprite(iconOutline, part.GetPartData().gradeResource);
		tempPart = part;
	}

	private void OnPress(bool show)
	{
		if (show)
		{
			if (lbName != null || tempPart != null)
			{
				UITooltip.Show(lbName.text, Localization.Get(tempPart.descriptionIdx));
			}
		}
		else
		{
			UITooltip.Hide();
		}
	}

	private void OnDrag()
	{
		UITooltip.Hide();
	}
}
