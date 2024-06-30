public class UIConquestNotice : UISimplePopup
{
	public UILabel label;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
	}

	public void Init(string notice)
	{
		UISetter.SetLabel(label, (!string.IsNullOrEmpty(notice)) ? notice : Localization.Get("110375"));
	}
}
