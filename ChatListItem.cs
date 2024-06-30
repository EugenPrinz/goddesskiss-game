using System;
using UnityEngine;

public class ChatListItem : MonoBehaviour
{
	public GameObject content;

	public GameObject rootMsg;

	public UISprite icon;

	public UILabel nickname;

	public UILabel guildName;

	public UILabel level;

	public UILabel contents;

	public UILabel time;

	public UILabel server;

	public GameObject rootReplay;

	public RecordListItem recordItem;

	protected int _channel;

	protected int _world;

	protected string _uno;

	protected string _thumbnail;

	public int channel => _channel;

	public int world => _world;

	public string uno => _uno;

	public string thumbnail => _thumbnail;

	public string userName => (!(nickname != null)) ? string.Empty : nickname.text;

	public void SetActive(bool active)
	{
		UISetter.SetActive(content, active);
	}

	public void Set(Protocols.ChattingInfo.ChattingData data)
	{
		_channel = data.sendChannel;
		_uno = data.sendUno;
		_world = data.sendWorld;
		_thumbnail = data.thumbnail;
		Protocols.ChattingMsgData chatMsgData = data.chatMsgData;
		if (recordItem == null)
		{
			UISetter.SetActive(rootMsg, active: true);
			UISetter.SetActive(rootReplay, active: false);
			UISetter.SetLabel(nickname, data.nickname);
			UISetter.SetLabel(guildName, (!GameSetting.instance.guildName) ? string.Empty : (string.IsNullOrEmpty(data.guildName) ? Localization.Get("7180") : data.guildName));
			UISetter.SetLabel(level, Localization.Format("1021", data.level));
			UISetter.SetSprite(icon, RemoteObjectManager.instance.regulation.GetCostumeThumbnailName(int.Parse(data.thumbnail)));
			UISetter.SetLabel(contents, chatMsgData.data);
			UISetter.SetLabel(text: Utility.GetStringToDay(new DateTime((long)data.date)), label: time);
			UISetter.SetLabel(server, string.Format("{0}-{1}", (_channel != 1) ? "G" : "K", _world));
		}
		else if (chatMsgData.record != null)
		{
			UISetter.SetActive(rootMsg, active: false);
			UISetter.SetActive(rootReplay, active: true);
			recordItem.Set(chatMsgData.record);
		}
		else
		{
			UISetter.SetActive(rootMsg, active: true);
			UISetter.SetActive(rootReplay, active: false);
			UISetter.SetLabel(nickname, data.nickname);
			UISetter.SetLabel(guildName, (!GameSetting.instance.guildName) ? string.Empty : (string.IsNullOrEmpty(data.guildName) ? Localization.Get("7180") : data.guildName));
			UISetter.SetLabel(level, Localization.Format("1021", data.level));
			UISetter.SetSprite(icon, RemoteObjectManager.instance.regulation.GetCostumeThumbnailName(int.Parse(data.thumbnail)));
			UISetter.SetLabel(contents, chatMsgData.data);
			UISetter.SetLabel(text: Utility.GetStringToDay(new DateTime((long)data.date)), label: time);
			UISetter.SetLabel(server, string.Format("{0}-{1}", (_channel != 1) ? "G" : "K", _world));
		}
	}
}
