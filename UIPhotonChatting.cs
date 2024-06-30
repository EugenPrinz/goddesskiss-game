using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Crosstales.BWF;
using Crosstales.BWF.Model;
using ExitGames.Client.Photon.Chat;
using Newtonsoft.Json;
using Shared.Regulation;
using UnityEngine;

public class UIPhotonChatting : MonoBehaviour
{
	public UIFlipSwitch[] channelTabs;

	public UIScrollWrap scrollWrap;

	public UIInput input;

	public UIPopupList serverListPopup;

	public GameObject messageView;

	public GameObject blockUserView;

	public UIDefaultListView blockUserListView;

	public UILabel blockUserCount;

	public const int blockUserChannelIdx = 3;

	public int messageLimit = 40;

	public int messageMaxPerPass = 10;

	public int wrapOffset;

	public const int inputDelayTm = 0;

	public const int channelNameLimit = 5;

	protected int channelIdx;

	protected float updateDelayTm = 0.5f;

	private GameObject infoPopUp;

	private GameObject blockPopup;

	private bool _bStart;

	protected RoLocalUser localUser => RemoteObjectManager.instance.localUser;

	protected RemoteObjectManager network => RemoteObjectManager.instance;

	private void Awake()
	{
		scrollWrap.onInitializeItem = OnInitializeItem;
	}

	private void Start()
	{
		serverListPopup.Clear();
		serverListPopup.items.Add(Localization.Get("7215"));
		serverListPopup.items.Add(Localization.Get("7217"));
		serverListPopup.items.Add(Localization.Get("7216"));
		if (PhotonChatting.instance.chatServerIdx > serverListPopup.items.Count)
		{
			PhotonChatting.instance.chatServerIdx = 0;
		}
		serverListPopup.value = serverListPopup.items[PhotonChatting.instance.chatServerIdx];
		EventDelegate.Add(serverListPopup.onChange, delegate
		{
			int num = serverListPopup.items.FindIndex((string x) => x == UIPopupList.current.value);
			if (num >= 0)
			{
				PhotonChatting.instance.chatServerIdx = num;
			}
		});
		_bStart = true;
	}

	private void OnEnable()
	{
		if (_bStart)
		{
			int num = serverListPopup.items.FindIndex((string x) => x == serverListPopup.value);
			if (num >= 0)
			{
				serverListPopup.Clear();
				serverListPopup.items.Add(Localization.Get("7215"));
				serverListPopup.items.Add(Localization.Get("7217"));
				serverListPopup.items.Add(Localization.Get("7216"));
				serverListPopup.value = serverListPopup.items[num];
			}
		}
		StartCoroutine("InitChannel");
	}

	private IEnumerator InitChannel()
	{
		UISetter.SetActive(messageView, active: true);
		UISetter.SetActive(blockUserView, active: false);
		yield return null;
		yield return null;
		SetChannelIndex(channelIdx);
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	public void Refresh()
	{
		if (blockUserView.activeSelf)
		{
			UISetter.SetLabel(blockUserCount, string.Format(Localization.Get("7207"), localUser.blockUsers.Count, RemoteObjectManager.instance.regulation.defineDtbl["CHAT_BLOCK_USER_MAX_COUNT"].value));
			blockUserListView.InitChattingBlockUserList(localUser.blockUsers.Values.ToList());
			blockUserListView.scrollView.InvalidateBounds();
			blockUserListView.scrollView.RestrictWithinBounds(instant: false, horizontal: false, vertical: true);
		}
	}

	protected bool CheckInputTime()
	{
		if (PhotonChatting.instance.inputTmTick != 0)
		{
			long value = DateTime.Now.Ticks - PhotonChatting.instance.inputTmTick;
			int num = (int)TimeSpan.FromTicks(value).TotalSeconds;
			if (num < 0)
			{
				num = 0;
			}
			int num2 = -num;
			if (num2 > 0)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7055", num2));
				return false;
			}
		}
		return true;
	}

	public bool IsCoupon(string msg)
	{
		if (localUser.coupons.Count > 0)
		{
			string text = string.Empty;
			for (int i = 0; i < localUser.coupons.Count; i++)
			{
				if (i > 0)
				{
					text += "|";
				}
				text += $"\\b{localUser.coupons[i]}\\b";
			}
			string text2 = msg.ToLower();
			Match match = Regex.Match(text2, text);
			if (match.Success && match.Value.Length == text2.Length)
			{
				return true;
			}
		}
		if (msg.Length == 16)
		{
			string text3 = msg.ToLower();
			if (text3.StartsWith("cpn"))
			{
				return true;
			}
		}
		return false;
	}

	private void _UpdateMessage()
	{
		ChatChannel channel = PhotonChatting.instance.GetChannel(channelIdx);
		if (channel == null || channel.MessageCount <= 0)
		{
			return;
		}
		int num = 0;
		for (num = 0; num < channel.MessageCount && num < messageMaxPerPass; num++)
		{
			try
			{
				Protocols.ChattingInfo.ChattingData chattingData = JsonConvert.DeserializeObject<Protocols.ChattingInfo.ChattingData>((string)channel.Messages[num]);
				if (localUser.blockUsers.ContainsKey($"{chattingData.sendChannel}_{chattingData.sendUno}"))
				{
					continue;
				}
				chattingData.date = channel.MessageTimes[num];
				if (chattingData.chatMsgData.record != null)
				{
					if (GameSetting.instance.chatReplayBattle && chattingData.sendChannel == localUser.channel)
					{
						chattingData.chatMsgData.data = Localization.Get("6143");
						chattingData.chatMsgData.record = null;
						localUser.chatMessages[channelIdx].Add(chattingData);
						Protocols.ChattingInfo.ChattingData chattingData2 = new Protocols.ChattingInfo.ChattingData();
						chattingData2.message = chattingData.message;
						localUser.chatMessages[channelIdx].Add(chattingData2);
					}
				}
				else
				{
					localUser.chatMessages[channelIdx].Add(chattingData);
				}
			}
			catch (Exception)
			{
			}
		}
		channel.RemoveMessages(num);
		scrollWrap.Refresh();
		int count = localUser.chatMessages[channelIdx].Count;
		if (count <= messageLimit)
		{
			return;
		}
		int num2 = count - messageLimit;
		if (num2 > 0)
		{
			if (num2 > count)
			{
				num2 = count;
			}
			localUser.chatMessages[channelIdx].RemoveRange(0, num2);
		}
	}

	private IEnumerator UpdateMessage()
	{
		scrollWrap.ResetPosition();
		PhotonChatting.instance.SetChannelMessageLeave(channelIdx, messageLimit);
		while (true)
		{
			_UpdateMessage();
			yield return new WaitForSeconds(updateDelayTm);
		}
	}

	public void CleanMessage()
	{
		localUser.chatMessages[channelIdx].Clear();
		scrollWrap.ResetPosition();
	}

	public void SetChannelIndex(int index)
	{
		channelIdx = index;
		if (channelIdx == 2 && !localUser.IsExistGuild())
		{
			channelIdx = 0;
		}
		if (channelIdx == 2)
		{
			string channelName = $"#Guild-{localUser.channel}-{localUser.world}-{localUser.guildInfo.idx}";
			if (!PhotonChatting.instance.IsConnectedChannel(channelName))
			{
				PhotonChatting.instance.ChannelSwitching(channelIdx, channelName);
				CleanMessage();
			}
		}
		for (int i = 0; i < channelTabs.Length; i++)
		{
			UISetter.SetFlipSwitch(channelTabs[i], channelIdx == i);
		}
		if (channelIdx == 3)
		{
			ShowBlockUsers();
			return;
		}
		input.defaultText = string.Format("{0}(Ch. {1})", Localization.Get("7038"), (channelIdx == 2) ? localUser.guildName : PhotonChatting.instance.GetChannelName(channelIdx));
		StopCoroutine("UpdateMessage");
		StartCoroutine("UpdateMessage");
	}

	public void OnInitializeItem(GameObject go, int wrapIndex, int realIndex)
	{
		ChatListItem component = go.GetComponent<ChatListItem>();
		List<Protocols.ChattingInfo.ChattingData> list = localUser.chatMessages[channelIdx];
		int num = realIndex + wrapOffset;
		if (num < 0 || num >= list.Count)
		{
			component.SetActive(active: false);
			return;
		}
		component.Set(list[list.Count - 1 - num]);
		component.SetActive(active: true);
	}

	public bool CanChangeChannel()
	{
		if (channelIdx == 2)
		{
			return false;
		}
		return true;
	}

	public void SendMessage()
	{
		if (!CheckInputTime())
		{
			return;
		}
		string text = Regex.Replace(input.value, "(?<!\r)\n", " ");
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (text[0].Equals('/'))
		{
			if (CanChangeChannel())
			{
				string[] array = text.Split(' ');
				if (array[0].ToLower().Equals("/ch") && array.Length == 2)
				{
					string text2 = array[1];
					if (!PhotonChatting.instance.IsConnectedChannel(text2))
					{
						if (text2.Length > 5)
						{
							NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7112"));
							return;
						}
						Match match = Regex.Match(text2, "[^a-zA-Z0-9]");
						if (match.Success)
						{
							NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7112"));
							return;
						}
						PhotonChatting.instance.ChannelSwitching(channelIdx, text2);
						NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7075", PhotonChatting.instance.GetChannelName(channelIdx)));
						input.defaultText = string.Format("{0}(Ch. {1})", Localization.Get("7038"), PhotonChatting.instance.GetChannelName(channelIdx));
						CleanMessage();
					}
					else
					{
						NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7098"));
					}
				}
			}
		}
		else
		{
			int remainSendTime = PhotonChatting.instance.GetRemainSendTime();
			if (remainSendTime > 0)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7055", remainSendTime));
				return;
			}
			if (IsCoupon(text))
			{
				network.RequestInputCoupon(text);
			}
			else
			{
				List<string> list = new List<string>();
				list.Add(text);
				text = BWFManager.ReplaceAll(text, ManagerMask.BadWord);
				if (!string.IsNullOrEmpty(text))
				{
					Formatting formatting = Formatting.None;
					JsonSerializerSettings serializerSettings = Regulation.SerializerSettings;
					Protocols.ChattingMsgData chattingMsgData = new Protocols.ChattingMsgData();
					chattingMsgData.data = text;
					Protocols.ChattingInfo.ChattingData chattingData = new Protocols.ChattingInfo.ChattingData();
					chattingData.sendChannel = localUser.channel;
					chattingData.sendWorld = localUser.world;
					chattingData.sendUno = localUser.uno;
					chattingData.nickname = localUser.nickname;
					chattingData.guildName = localUser.guildName;
					chattingData.level = localUser.level;
					chattingData.thumbnail = localUser.thumbnailId;
					CommanderCostumeDataRow costume = RemoteObjectManager.instance.regulation.FindCostumeData(int.Parse(localUser.thumbnailId));
					if (costume != null)
					{
						RoCommander roCommander = localUser.FindCommander(costume.cid.ToString());
						if (roCommander != null && roCommander.isBasicCostume)
						{
							CommanderCostumeDataRow commanderCostumeDataRow = RemoteObjectManager.instance.regulation.commanderCostumeDtbl.Find((CommanderCostumeDataRow row) => row.cid == costume.cid && row.skinName == roCommander.currentViewCostume);
							if (commanderCostumeDataRow != null)
							{
								chattingData.thumbnail = commanderCostumeDataRow.ctid.ToString();
							}
						}
					}
					chattingData.date = RemoteObjectManager.instance.GetCurrentTime();
					chattingData.message = chattingMsgData.ToString();
					text = JsonConvert.SerializeObject(chattingData, formatting, serializerSettings);
					PhotonChatting.instance.SendChatMessage(channelIdx, text);
				}
			}
		}
		input.value = string.Empty;
	}

	public void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "BtnSend")
		{
			SoundManager.PlaySFX("BTN_Send_001");
			SendMessage();
		}
		else if (text == "BtnInfo" && infoPopUp == null)
		{
			UISimplePopup uISimplePopup = UISimplePopup.CreateOK("InformationChatPopup");
			uISimplePopup.Set(localization: true, "7123", "7125", string.Empty, "1001", string.Empty, string.Empty);
			infoPopUp = uISimplePopup.gameObject;
		}
	}

	private void ShowBlockUsers()
	{
		if (!blockUserView.activeSelf)
		{
			StopCoroutine("UpdateMessage");
			UISetter.SetActive(messageView, active: false);
			UISetter.SetActive(blockUserView, active: true);
			UISetter.SetLabel(blockUserCount, string.Format(Localization.Get("7207"), localUser.blockUsers.Count, RemoteObjectManager.instance.regulation.defineDtbl["CHAT_BLOCK_USER_MAX_COUNT"].value));
			blockUserListView.ResetPosition();
			blockUserListView.InitChattingBlockUserList(localUser.blockUsers.Values.ToList());
		}
	}

	public void OnTab(GameObject sender)
	{
		int num = 0;
		string text = sender.name;
		if (text == "BlockUsers")
		{
			num = 3;
		}
		else
		{
			UISetter.SetActive(messageView, active: true);
			UISetter.SetActive(blockUserView, active: false);
			num = int.Parse(sender.name);
			if (num == 2 && !localUser.IsExistGuild())
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Format("1327"));
				num = ((channelIdx != 2) ? channelIdx : 0);
			}
		}
		SetChannelIndex(num);
	}

	public void AddBlockUser(ChatListItem item)
	{
		int channel = item.channel;
		string uno = item.uno;
		string userName = item.userName;
		string thumbnail = item.thumbnail;
		if (string.IsNullOrEmpty(uno) || blockPopup != null || (localUser.channel == channel && localUser.uno == uno) || localUser.blockUsers.ContainsKey($"{channel}_{uno}"))
		{
			return;
		}
		int num = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["CHAT_BLOCK_USER_MAX_COUNT"].value);
		if (localUser.blockUsers.Count >= num)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7206"));
			return;
		}
		UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: false, Localization.Get("7203"), string.Format(Localization.Get("7204"), userName, num), null, Localization.Get("1001"), Localization.Get("1000"));
		uISimplePopup.onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				RemoteObjectManager.instance.RequestAddBlockUser(channel, uno, userName, thumbnail);
			}
		};
		blockPopup = uISimplePopup.gameObject;
	}

	public void RemoveBlockUser(UIChattingBlockUser item)
	{
		int channel = item.channel;
		string uno = item.uno;
		if (string.IsNullOrEmpty(uno) || blockPopup != null || !localUser.blockUsers.ContainsKey($"{channel}_{uno}"))
		{
			return;
		}
		UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: false, Localization.Get("7209"), string.Format(Localization.Get("7210"), item.userName), null, Localization.Get("1001"), Localization.Get("1000"));
		uISimplePopup.onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				RemoteObjectManager.instance.RequestRemoveBlockUser(channel, uno);
			}
		};
		blockPopup = uISimplePopup.gameObject;
	}
}
