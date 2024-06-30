using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Chat;
using Newtonsoft.Json;
using Shared.Regulation;
using UnityEngine;

public class PhotonChatting : MonoBehaviour, IChatClientListener
{
	public const int MaximumChannelCount = 3;

	public const int guildChannelIdx = 2;

	private static PhotonChatting _instance;

	public int HistoryLengthToFetch;

	public int MessageLimit = 300;

	protected ChatClient chatClient;

	protected string[] indexChannel;

	protected Dictionary<string, int> channelRef;

	protected int sendLimit = 3;

	protected int sendResetDelayTm = 5;

	protected int sendCount;

	protected long sendResetTmTick;

	[HideInInspector]
	public long inputTmTick;

	private int _chatServerIdx;

	public static PhotonChatting instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<PhotonChatting>();
			}
			return _instance;
		}
	}

	protected RemoteObjectManager network => RemoteObjectManager.instance;

	public bool CanChat
	{
		get
		{
			if (chatClient == null)
			{
				return false;
			}
			return chatClient.CanChat;
		}
	}

	public int chatServerIdx
	{
		get
		{
			return _chatServerIdx;
		}
		set
		{
			if (_chatServerIdx != value)
			{
				PlayerPrefs.SetInt("ChatServer", 0);
				_chatServerIdx = value;
				if (chatClient != null)
				{
					chatClient.Disconnect();
				}
			}
		}
	}

	private RoLocalUser localUser => RemoteObjectManager.instance.localUser;

	protected string userName => RemoteObjectManager.instance.localUser.nickname;

	public bool IsConnectedChannel(string channelName)
	{
		for (int i = 0; i < indexChannel.Length; i++)
		{
			if (channelName == indexChannel[i])
			{
				return true;
			}
		}
		return false;
	}

	public string GetChannelName(int channelIdx)
	{
		return indexChannel[channelIdx];
	}

	public ChatChannel GetChannel(int channelIdx)
	{
		string channelName = GetChannelName(channelIdx);
		if (string.IsNullOrEmpty(channelName))
		{
			return null;
		}
		if (!chatClient.TryGetChannel(channelName, out var channel))
		{
			return null;
		}
		return channel;
	}

	public ChatChannel GetChannel(string channelName)
	{
		if (!chatClient.TryGetChannel(channelName, out var channel))
		{
			return null;
		}
		return channel;
	}

	public void SetChannelMessageLeave(int channelIdx, int count)
	{
		GetChannel(channelIdx)?.LeaveMessage(count);
	}

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			UnityEngine.Object.DestroyImmediate(this);
			return;
		}
		_instance = this;
		_chatServerIdx = PlayerPrefs.GetInt("ChatServer", 0);
		chatClient = new ChatClient(this);
		chatClient.MessageLimit = MessageLimit;
		indexChannel = new string[3];
		channelRef = new Dictionary<string, int>();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		Application.runInBackground = true;
		sendLimit = int.Parse(network.regulation.defineDtbl["GUILD_CHAT_COUNT"].value);
		sendResetDelayTm = int.Parse(network.regulation.defineDtbl["GUILD_CHAT_DELAY"].value);
	}

	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(userName))
		{
			string appVersion = _chatServerIdx.ToString();
			chatClient.Connect(ChatSettings.Instance.AppId, appVersion, new AuthenticationValues(userName));
		}
	}

	private void OnDisable()
	{
		if (chatClient != null)
		{
			chatClient.Disconnect();
		}
	}

	private void OnApplicationQuit()
	{
		if (chatClient != null)
		{
			chatClient.Disconnect();
		}
	}

	private void Update()
	{
		if (chatClient == null || Application.internetReachability == NetworkReachability.NotReachable)
		{
			return;
		}
		if (chatClient.State == ChatState.Uninitialized || chatClient.State == ChatState.Disconnected)
		{
			if (!string.IsNullOrEmpty(userName))
			{
				string appVersion = _chatServerIdx.ToString();
				chatClient.Connect(ChatSettings.Instance.AppId, appVersion, new AuthenticationValues(userName));
			}
		}
		else
		{
			chatClient.Service();
		}
	}

	public void SendAllChannelMessage(string msg)
	{
		sendCount++;
		inputTmTick = DateTime.Now.Ticks;
		Dictionary<string, ChatChannel>.Enumerator enumerator = chatClient.PublicChannels.GetEnumerator();
		while (enumerator.MoveNext())
		{
			chatClient.PublishMessage(enumerator.Current.Key, msg);
		}
	}

	public void SendChatMessage(int channelIdx, string msg)
	{
		sendCount++;
		inputTmTick = DateTime.Now.Ticks;
		if (string.IsNullOrEmpty(msg))
		{
			return;
		}
		string text = indexChannel[channelIdx];
		if (!string.IsNullOrEmpty(text))
		{
			ChatChannel channel = null;
			if (chatClient.TryGetChannel(text, out channel))
			{
				chatClient.PublishMessage(text, msg);
			}
		}
	}

	public void ChannelSwitching(int index, string channelName)
	{
		if (string.IsNullOrEmpty(channelName) || index >= 3)
		{
			return;
		}
		string text = indexChannel[index];
		if (!string.IsNullOrEmpty(text))
		{
			channelRef[text]--;
			if (channelRef[text] <= 0)
			{
				channelRef.Remove(text);
				chatClient.Unsubscribe(new string[1] { text });
			}
		}
		chatClient.Subscribe(new string[1] { channelName });
		indexChannel[index] = channelName;
		if (channelRef.ContainsKey(channelName))
		{
			channelRef[channelName]++;
		}
		else
		{
			channelRef.Add(channelName, 1);
		}
		PlayerPrefs.SetString($"Channel{index}", channelName);
	}

	public void OnConnected()
	{
		indexChannel = new string[3]
		{
			PlayerPrefs.GetString("Channel0", "1"),
			PlayerPrefs.GetString("Channel1", UnityEngine.Random.Range(2, 1000).ToString()),
			(!localUser.IsExistGuild()) ? "#Guild" : $"#Guild-{localUser.channel}-{localUser.world}-{localUser.guildInfo.idx}"
		};
		channelRef = new Dictionary<string, int>();
		for (int i = 0; i < indexChannel.Length; i++)
		{
			if (channelRef.ContainsKey(indexChannel[i]))
			{
				channelRef[indexChannel[i]]++;
			}
			else
			{
				channelRef.Add(indexChannel[i], 1);
			}
		}
		chatClient.Subscribe(indexChannel, HistoryLengthToFetch);
		chatClient.SetOnlineStatus(2);
	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
		for (int i = 0; i < results.Length; i++)
		{
			if (results[i])
			{
			}
		}
	}

	public void OnDisconnected()
	{
	}

	public void DebugReturn(DebugLevel level, string message)
	{
		switch (level)
		{
		case DebugLevel.ERROR:
			UnityEngine.Debug.LogError(message);
			break;
		case DebugLevel.WARNING:
			UnityEngine.Debug.LogWarning(message);
			break;
		default:
			UnityEngine.Debug.Log(message);
			break;
		}
	}

	public void OnChatStateChange(ChatState state)
	{
	}

	public void OnUnsubscribed(string[] channels)
	{
	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{
	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
	}

	public int GetRemainSendTime()
	{
		long value = DateTime.Now.Ticks - sendResetTmTick;
		int num = (int)TimeSpan.FromTicks(value).TotalSeconds;
		if (num < 0)
		{
			num = sendResetDelayTm;
		}
		int num2 = sendResetDelayTm - num;
		if (num2 <= 0)
		{
			sendResetTmTick = DateTime.Now.Ticks;
			sendCount = 0;
		}
		if (sendCount >= sendLimit)
		{
			return num2;
		}
		return 0;
	}

	[ContextMenu("TestMessage")]
	public void SendTestMessage()
	{
		string data = "테스트입니다";
		Formatting formatting = Formatting.None;
		JsonSerializerSettings serializerSettings = Regulation.SerializerSettings;
		Protocols.ChattingMsgData chattingMsgData = new Protocols.ChattingMsgData();
		chattingMsgData.data = data;
		data = JsonConvert.SerializeObject(chattingMsgData, formatting, serializerSettings);
		Protocols.ChattingInfo.ChattingData chattingData = new Protocols.ChattingInfo.ChattingData();
		chattingData.nickname = localUser.nickname;
		chattingData.level = localUser.level;
		chattingData.thumbnail = localUser.thumbnailId;
		chattingData.date = RemoteObjectManager.instance.GetCurrentTime();
		chattingData.message = data;
		data = JsonConvert.SerializeObject(chattingData, formatting, serializerSettings);
		SendChatMessage(0, data);
	}

	private IEnumerator TestMessage(string chName, string msg)
	{
		for (int i = 0; i < 2000; i++)
		{
			if (!chatClient.TryGetChannel(chName, out var _))
			{
				break;
			}
			chatClient.PublishMessage(chName, msg);
			yield return new WaitForSeconds(0.3f);
		}
	}

	private void AddMessageToChannel(string chName, string msg)
	{
		ChatChannel channel = null;
		if (chatClient.TryGetChannel(chName, out channel))
		{
			Formatting formatting = Formatting.None;
			JsonSerializerSettings serializerSettings = Regulation.SerializerSettings;
			Protocols.ChattingMsgData chattingMsgData = new Protocols.ChattingMsgData();
			chattingMsgData.data = msg;
			msg = JsonConvert.SerializeObject(chattingMsgData, formatting, serializerSettings);
			Protocols.ChattingInfo.ChattingData chattingData = new Protocols.ChattingInfo.ChattingData();
			chattingData.nickname = localUser.nickname;
			chattingData.level = localUser.level;
			chattingData.thumbnail = localUser.thumbnailId;
			chattingData.date = RemoteObjectManager.instance.GetCurrentTime();
			chattingData.message = msg;
			msg = JsonConvert.SerializeObject(chattingData, formatting, serializerSettings);
			channel.Add("Bot", msg);
		}
	}
}
