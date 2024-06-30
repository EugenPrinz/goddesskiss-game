public class UIChattingBlockUser : UIItemBase
{
	public UISprite thumbnail;

	public UILabel nickName;

	public UILabel server;

	private int _channel;

	private string _uno;

	public int channel => _channel;

	public string uno => _uno;

	public string userName => (!(nickName != null)) ? string.Empty : nickName.text;

	public void Set(Protocols.BlockUser user)
	{
		_channel = user.channel;
		_uno = user.uno;
		UISetter.SetLabel(nickName, user.nickName);
		if (!string.IsNullOrEmpty(user.thumbnail))
		{
			UISetter.SetSprite(thumbnail, RemoteObjectManager.instance.regulation.GetCostumeThumbnailName(int.Parse(user.thumbnail)));
		}
		UISetter.SetLabel(server, (_channel != 1) ? Localization.Get("7217") : Localization.Get("7216"));
	}
}
