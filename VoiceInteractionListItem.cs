public class VoiceInteractionListItem : UIItemBase
{
	public new UILabel name;

	public UISprite block;

	public UILabel favor;

	public UISprite favorIcon;

	public UISprite marryIcon;

	public void Set(string voiceName, bool overFavor, int favorStep, bool marryMark)
	{
		UISetter.SetLabel(name, voiceName);
		UISetter.SetActive(block, overFavor);
		UISetter.SetLabel(favor, favorStep);
		UISetter.SetActive(favor, !marryMark);
		UISetter.SetActive(favorIcon, !marryMark);
		UISetter.SetActive(marryIcon, marryMark);
	}
}
