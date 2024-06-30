using System;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Transports;
using UnityEngine;

public class SocketChatting : MonoBehaviour
{
	private static SocketChatting _instance;

	public int MessageLimit = 500;

	private SocketManager Manager;

	public const int MaximumChannelCount = 4;

	public const int guildChannelIdx = 2;

	private string[] _localChannels;

	private Dictionary<string, SocketChatChannel> _serverChannels;

	private int sendLimit = 3;

	private int sendResetDelayTm = 5;

	private int sendCount;

	private long sendResetTmTick;

	[HideInInspector]
	public long inputTmTick;

	private int _chatServerIdx;

	public static SocketChatting instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<SocketChatting>();
			}
			return _instance;
		}
	}

	protected RemoteObjectManager network => RemoteObjectManager.instance;

	private RoLocalUser localUser => RemoteObjectManager.instance.localUser;

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
				if (Manager != null)
				{
					((IManager)Manager).TryToReconnect();
				}
			}
		}
	}

	public string serverNamespace
	{
		get
		{
			int num = 0;
			return $"{2}_{chatServerIdx}_{num}";
		}
	}

	public static void Relase()
	{
		if (!(_instance == null))
		{
			UnityEngine.Object.DestroyImmediate(_instance.gameObject);
			_instance = null;
		}
	}

	private void Awake()
	{
		Application.runInBackground = true;
		if (_instance != null && _instance != this)
		{
			UnityEngine.Object.DestroyImmediate(this);
			return;
		}
		_instance = this;
		_chatServerIdx = PlayerPrefs.GetInt("ChatServer", 0);
		_localChannels = new string[4];
		_serverChannels = new Dictionary<string, SocketChatChannel>();
		sendLimit = int.Parse(network.regulation.defineDtbl["GUILD_CHAT_COUNT"].value);
		sendResetDelayTm = int.Parse(network.regulation.defineDtbl["GUILD_CHAT_DELAY"].value);
		SocketOptions socketOptions = new SocketOptions();
		socketOptions.AutoConnect = false;
		socketOptions.ConnectWith = TransportTypes.WebSocket;
		Manager = new SocketManager(new Uri(network.ChattingServerUrl), socketOptions);
		Manager.GetSocket("/chat").On(SocketIOEventTypes.Error, OnSocketError);
		Manager.GetSocket("/chat").On(SocketIOEventTypes.Connect, OnConnect);
		Manager.GetSocket("/chat").On("broadcast", OnBroadcast);
		Manager.GetSocket("/chat").On("err", OnError);
		Manager.GetSocket("/chat").On("forceDisconnect", OnForceDisconnect);
		Manager.Open();
		Manager.GetSocket("/chat").On(SocketIOEventTypes.Disconnect, OnDisconnect);
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		if (!PlayerPrefs.HasKey("Channel0"))
		{
			PlayerPrefs.SetString("Channel0", "1");
		}
		if (!PlayerPrefs.HasKey("Channel1"))
		{
			PlayerPrefs.SetString("Channel1", UnityEngine.Random.Range(2, 1000).ToString());
		}
		_localChannels = new string[3]
		{
			PlayerPrefs.GetString("Channel0", "1"),
			PlayerPrefs.GetString("Channel1", UnityEngine.Random.Range(2, 1000).ToString()),
			(!localUser.IsExistGuild()) ? "#Guild" : $"#Guild-{localUser.channel}-{localUser.world}-{localUser.guildInfo.idx}"
		};
	}

	private void OnDestroy()
	{
		if (Manager != null)
		{
			Manager.Close();
		}
	}

	private void OnConnect(Socket socket, Packet packet, params object[] args)
	{
		_localChannels = new string[3]
		{
			PlayerPrefs.GetString("Channel0", "1"),
			PlayerPrefs.GetString("Channel1", UnityEngine.Random.Range(2, 1000).ToString()),
			(!localUser.IsExistGuild()) ? "#Guild" : $"#Guild-{localUser.channel}-{localUser.world}-{localUser.guildInfo.idx}"
		};
		List<string> list = new List<string>();
		for (int i = 0; i < _localChannels.Length; i++)
		{
			list.Add(GetServerChannelName(_localChannels[i]));
		}
		_serverChannels.Clear();
		Manager.GetSocket("/chat").Emit("login", localUser.uno);
		Manager.GetSocket("/chat").Emit("join", OnJoin, list);
	}

	private void OnDisconnect(Socket socket, Packet packet, params object[] args)
	{
		_serverChannels.Clear();
	}

	private void OnJoin(Socket socket, Packet packet, params object[] args)
	{
		for (int i = 0; i < args.Length; i++)
		{
			if (!(args[i] is List<object>))
			{
				continue;
			}
			List<object> list = (List<object>)args[i];
			for (int j = 0; j < list.Count; j++)
			{
				string key = list[j].ToString();
				if (!_serverChannels.ContainsKey(key))
				{
					SocketChatChannel socketChatChannel = new SocketChatChannel(key);
					socketChatChannel.MessageLimit = MessageLimit;
					_serverChannels.Add(key, socketChatChannel);
				}
			}
		}
	}

	private void OnLeave(Socket socket, Packet packet, params object[] args)
	{
		Dictionary<string, SocketChatChannel> dictionary = new Dictionary<string, SocketChatChannel>();
		Dictionary<string, SocketChatChannel>.Enumerator enumerator = _serverChannels.GetEnumerator();
		while (enumerator.MoveNext())
		{
			dictionary.Add(enumerator.Current.Key, enumerator.Current.Value);
		}
		for (int i = 0; i < args.Length; i++)
		{
			if (!(args[i] is List<object>))
			{
				continue;
			}
			List<object> list = (List<object>)args[i];
			for (int j = 0; j < list.Count; j++)
			{
				string key = list[j].ToString();
				if (dictionary.ContainsKey(key))
				{
					dictionary.Remove(key);
				}
			}
		}
		enumerator = dictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			_serverChannels.Remove(enumerator.Current.Key);
		}
	}

	private void OnBroadcast(Socket socket, Packet packet, params object[] args)
	{
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i] is Dictionary<string, object>)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)args[i];
				string key = (string)dictionary["channel"];
				string message = (string)dictionary["msg"];
				if (_serverChannels.TryGetValue(key, out var value))
				{
					value.Add(message);
				}
			}
		}
	}

	private void OnError(Socket socket, Packet packet, params object[] args)
	{
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i] is Dictionary<string, object>)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)args[i];
			}
		}
	}

	private void OnForceDisconnect(Socket socket, Packet packet, params object[] args)
	{
		UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, Localization.Get("70150"), Localization.Get("1001"));
		if (uISimplePopup != null)
		{
			uISimplePopup.onClose = delegate
			{
				Application.Quit();
			};
		}
	}

	private void OnSocketError(Socket socket, Packet packet, params object[] args)
	{
	}

	public void SendAllChannelMessage(string msg)
	{
		if (!string.IsNullOrEmpty(msg))
		{
			sendCount++;
			inputTmTick = DateTime.Now.Ticks;
			Dictionary<string, SocketChatChannel>.Enumerator enumerator = _serverChannels.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Manager.GetSocket("/chat").Emit("message", enumerator.Current.Key, msg);
			}
		}
	}

	public void SendChatMessage(int index, string msg)
	{
		if (index < 4)
		{
			SendChatMessage(GetServerChannelName(_localChannels[index]), msg);
		}
	}

	public void SendChatMessage(string serverChannelName, string msg)
	{
		if (!string.IsNullOrEmpty(serverChannelName) && !string.IsNullOrEmpty(msg) && _serverChannels.ContainsKey(serverChannelName))
		{
			sendCount++;
			inputTmTick = DateTime.Now.Ticks;
			Manager.GetSocket("/chat").Emit("message", serverChannelName, msg);
		}
	}

	public void ChannelSwitching(int index, string nextChannelName)
	{
		if (index < 4 && ChannelSwitching(GetServerChannelName(index), GetServerChannelName(nextChannelName)))
		{
			_localChannels[index] = nextChannelName;
			PlayerPrefs.SetString($"Channel{index}", nextChannelName);
		}
	}

	private bool ChannelSwitching(string prevServerChannelName, string nextServerChannelName)
	{
		if (string.IsNullOrEmpty(nextServerChannelName))
		{
			return false;
		}
		if (prevServerChannelName == nextServerChannelName)
		{
			return false;
		}
		Leave(prevServerChannelName);
		Join(nextServerChannelName);
		return true;
	}

	public void Join(string serverChannelName)
	{
		if (!_serverChannels.ContainsKey(serverChannelName))
		{
			Manager.GetSocket("/chat").Emit("join", OnJoin, serverChannelName);
		}
	}

	public void Leave(string serverChannelName)
	{
		if (_serverChannels.ContainsKey(serverChannelName))
		{
			Manager.GetSocket("/chat").Emit("leave", OnLeave, serverChannelName);
		}
	}

	public bool IsConnectedLocalChannel(string localChannelName)
	{
		for (int i = 0; i < _localChannels.Length; i++)
		{
			if (localChannelName == _localChannels[i])
			{
				return true;
			}
		}
		return false;
	}

	public string GetLocalChannelName(int channelIdx)
	{
		return _localChannels[channelIdx];
	}

	private string GetServerChannelName(int channelIdx)
	{
		return GetServerChannelName(GetLocalChannelName(channelIdx));
	}

	private string GetServerChannelName(string localChannelName)
	{
		if (string.IsNullOrEmpty(localChannelName))
		{
			return string.Empty;
		}
		return $"{serverNamespace}_{localChannelName}";
	}

	public SocketChatChannel GetChannel(int channelIdx)
	{
		string serverChannelName = GetServerChannelName(channelIdx);
		if (string.IsNullOrEmpty(serverChannelName))
		{
			return null;
		}
		return GetChannel(serverChannelName);
	}

	public SocketChatChannel GetChannel(string serverChannelName)
	{
		if (!_serverChannels.TryGetValue(serverChannelName, out var value))
		{
			return null;
		}
		return value;
	}

	public void SetChannelMessageLeave(int channelIdx, int count)
	{
		GetChannel(channelIdx)?.LeaveMessage(count);
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
}
