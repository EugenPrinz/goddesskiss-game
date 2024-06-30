using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

public class UIChatting : UIPopup
{
	[CompilerGenerated]
	private sealed class _003CInitOpenAndGuildData_003Ec__AnonStorey1
	{
		internal List<Protocols.ChattingInfo.ChattingData> chattingList;
	}

	[CompilerGenerated]
	private sealed class _003CInitOpenAndAllData_003Ec__AnonStorey2
	{
		internal List<Protocols.ChattingInfo.ChattingData> chattingList;
	}

	[CompilerGenerated]
	private sealed class _003CInitOpenAndWhisperData_003Ec__AnonStorey3
	{
		internal List<Protocols.ChattingInfo.ChattingData> chattingList;
	}

	public GameObject contents;

	public GameObject block;

	public UIPhotonChatting photonChatting;

	public UISocketChatting socketChatting;

	public UIDraggablePanel2 allChatListView;

	public UIDraggablePanel2 guildChatListView;

	public UIDraggablePanel2 whisperChatListView;

	public GameObject nickNameListRoot;

	public UIDefaultListView nickNameListView;

	public Dictionary<int, NickNameType> nickNameList;

	public UISprite guildIcon;

	public UILabel guildName;

	public UILabel guildNotice;

	public UILabel guildLevel;

	public UIFlipSwitch switchAll;

	public UIFlipSwitch switchGuild;

	public UIFlipSwitch switchWhisper;

	public UIFlipSwitch switchChat;

	public UIInput allInput;

	public UIInput guildInput;

	public UIInput whisperInput;

	public UIInput whisperNickNameInput;

	public UILabel whisperNickNameLabel;

	public GameObject send;

	public GameObject recharge;

	public GameObject Lock;

	public GameObject guildRoot;

	public GameObject allRoot;

	public GameObject whisperRoot;

	public UILabel speak;

	public int TimeOutCount;

	private bool isSend;

	private int sendDelay;

	private int selectNickName;

	private int openLevel;

	private bool allChatRefresh;

	private bool guildChatRefresh;

	private bool isAnimation;

	public GameObject channelBadge;

	public GameObject guildBadge;

	public bool channelBadgeState;

	public bool guildBadgeState;

	private void Start()
	{
		isAnimation = false;
		allChatRefresh = false;
		guildBadgeState = false;
		channelBadgeState = false;
		selectNickName = -1;
		sendDelay = 10;
		isSend = true;
		switchChat.Off();
		nickNameList = new Dictionary<int, NickNameType>();
		UISetter.SetFlipSwitch(switchAll, state: true);
		UISetter.SetFlipSwitch(switchGuild, state: false);
		UISetter.SetFlipSwitch(switchWhisper, state: false);
		UISetter.SetActive(allRoot, active: true);
		UISetter.SetActive(guildRoot, active: false);
		UISetter.SetActive(whisperRoot, active: false);
		UISetter.SetActive(block, active: false);
		UISetter.SetActive(nickNameListRoot, active: false);
		ChattingLockCheck();
		UISetter.SetActive(contents, active: false);
	}

	public void InitOpenAndGuildData()
	{
		_003CInitOpenAndGuildData_003Ec__AnonStorey1 _003CInitOpenAndGuildData_003Ec__AnonStorey = new _003CInitOpenAndGuildData_003Ec__AnonStorey1();
	}

	public void InitOpenAndAllData()
	{
		_003CInitOpenAndAllData_003Ec__AnonStorey2 _003CInitOpenAndAllData_003Ec__AnonStorey = new _003CInitOpenAndAllData_003Ec__AnonStorey2();
	}

	public void InitOpenAndWhisperData()
	{
		_003CInitOpenAndWhisperData_003Ec__AnonStorey3 _003CInitOpenAndWhisperData_003Ec__AnonStorey = new _003CInitOpenAndWhisperData_003Ec__AnonStorey3();
	}

	public void OnInput()
	{
		SoundManager.PlaySFX("BTN_Send_001");
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "ChatBtn")
		{
			if (!isAnimation)
			{
				OnChat(switchChat.GetState() == SwitchStatus.OFF);
			}
			return;
		}
		if (text == "SendBtn")
		{
			SoundManager.PlaySFX("BTN_Send_001");
			SendMessage();
			return;
		}
		if (nickNameListView.Contains(text))
		{
			selectNickName = int.Parse(nickNameListView.GetPureId(text));
			string nickname = nickNameList.Values.ElementAt(selectNickName).nickname;
			UISetter.SetLabel(whisperNickNameLabel, nickname);
			UISetter.SetActive(nickNameListRoot, active: false);
			return;
		}
		switch (text)
		{
		case "NickNameInputBG":
			if (nickNameList.Count > 0)
			{
				UISetter.SetActive(nickNameListRoot, !nickNameListRoot.activeSelf);
			}
			break;
		case "Lock":
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Format("16049", openLevel));
			break;
		case "GuildLock":
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("1327"));
			break;
		}
	}

	public void SendMessage()
	{
		if (switchAll.GetState() == SwitchStatus.ON)
		{
			if (allInput.value.Length != 0)
			{
				if (!isSend)
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7055", sendDelay));
					return;
				}
				string data = Regex.Replace(allInput.value, "(?<!\r)\n", " ");
				Protocols.ChattingMsgData chattingMsgData = new Protocols.ChattingMsgData();
				chattingMsgData.data = data;
				base.network.RequestChChatting(1, 1, base.localUser.nickname, chattingMsgData.ToString(), 1);
				allInput.value = string.Empty;
				StartCoroutine(SendDelay());
			}
		}
		else if (switchGuild.GetState() == SwitchStatus.ON)
		{
			if (guildInput.value.Length != 0)
			{
				if (!isSend)
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7055", sendDelay));
					return;
				}
				string data2 = Regex.Replace(guildInput.value, "(?<!\r)\n", " ");
				Protocols.ChattingMsgData chattingMsgData2 = new Protocols.ChattingMsgData();
				chattingMsgData2.data = data2;
				base.network.RequestGuildChatting(base.localUser.guildInfo.idx, 1, base.localUser.nickname, chattingMsgData2.ToString(), 0);
				guildInput.value = string.Empty;
				StartCoroutine(SendDelay());
			}
		}
		else if (switchWhisper.GetState() == SwitchStatus.ON)
		{
			if (!isSend)
			{
				UIManager.instance.world.mainCommand.CreateFloatingText(new Vector3(-0.541f, 0.226f, 0f), $"다음 메세지 전송까지 {sendDelay}초 남았습니다.");
			}
			else if (selectNickName != -1)
			{
				int uno = nickNameList.Values.ElementAt(selectNickName).uno;
				Protocols.ChattingMsgData chattingMsgData3 = new Protocols.ChattingMsgData();
				chattingMsgData3.data = whisperInput.value;
				base.network.RequestWhisperChatting(int.Parse(base.localUser.uno), string.Empty, uno, string.Empty, chattingMsgData3.ToString());
				guildInput.value = string.Empty;
				StartCoroutine(SendDelay());
			}
			else
			{
				UISimplePopup.CreateOK(localization: true, "1310", "유저를 선택해주세요.", null, "1001");
			}
		}
	}

	private IEnumerator SendDelay()
	{
		for (isSend = false; sendDelay > 0; sendDelay--)
		{
			yield return new WaitForSeconds(1f);
		}
		isSend = true;
		sendDelay = 10;
		yield return true;
	}

	public void OnTab(GameObject sender)
	{
		switch (sender.name)
		{
		case "All":
			InitOpenAndAllData();
			break;
		case "Guild":
			InitOpenAndGuildData();
			break;
		case "Whisper":
			InitOpenAndWhisperData();
			break;
		}
		UpdateBadge();
	}

	private void _SetPage(bool allState, bool guildState, bool whisperState)
	{
		int num = 0;
		num += (allState ? 1 : 0);
		num += (guildState ? 1 : 0);
		num += (whisperState ? 1 : 0);
		if (num > 1 || num == 0)
		{
			allState = true;
			guildState = false;
			whisperState = false;
		}
		UISetter.SetActive(allRoot, allState);
		UISetter.SetActive(guildRoot, guildState);
		UISetter.SetActive(whisperRoot, whisperState);
		UISetter.SetFlipSwitch(switchAll, allState);
		UISetter.SetFlipSwitch(switchGuild, guildState);
		UISetter.SetFlipSwitch(switchWhisper, whisperState);
	}

	private void UpdateBadge()
	{
		UISetter.SetActive(channelBadge, channelBadgeState);
		UISetter.SetActive(guildBadge, guildBadgeState);
		base.uiWorld.mainCommand.BadgeControl();
	}

	public void OnChat(bool state)
	{
		if (state)
		{
			SoundManager.PlaySFX("SE_MenuOpen_001");
			UISetter.SetActive(contents, active: true);
			UISetter.SetActive(block, active: true);
			StartTimer();
			InitOpenAndAllData();
			UpdateBadge();
			OpenAnimation();
		}
		else if (switchChat.GetState() == SwitchStatus.ON)
		{
			SoundManager.PlaySFX("SE_MenuClose_001");
			UISetter.SetActive(block, active: false);
			StopTimer();
			CloseAnimation();
		}
		UISetter.SetFlipSwitch(switchChat, state);
	}

	public void CloseChat()
	{
		OnChat(state: false);
	}

	public override void OnRefresh()
	{
		ChattingLockCheck();
		if (contents.activeSelf)
		{
			if (photonChatting != null)
			{
				photonChatting.Refresh();
			}
			if (socketChatting != null)
			{
				socketChatting.Refresh();
			}
			if (switchAll.GetState() == SwitchStatus.ON)
			{
				UISetter.SetActive(send, active: true);
				UISetter.SetActive(recharge, active: false);
				InitOpenAndAllData();
			}
			else if (switchGuild.GetState() == SwitchStatus.ON)
			{
				InitOpenAndGuildData();
			}
		}
	}

	private void ChattingLockCheck()
	{
		UISetter.SetActive(Lock, active: false);
		openLevel = 1;
	}

	public void AddNickName(int uno, string nickName)
	{
		int key = -1;
		foreach (KeyValuePair<int, NickNameType> nickName2 in nickNameList)
		{
			if (nickName2.Value.uno == uno)
			{
				key = nickName2.Key;
			}
		}
		if (nickNameList.ContainsKey(key))
		{
			nickNameList.Remove(key);
		}
		NickNameType value = default(NickNameType);
		value.uno = uno;
		value.nickname = nickName;
		nickNameList.Add(nickNameList.Count, value);
	}

	private void StartTimer()
	{
	}

	private void CountDown()
	{
		TimeOutCount--;
		if (TimeOutCount < 1)
		{
			CancelInvoke();
			StartTimer();
		}
	}

	private void StopTimer()
	{
		CancelInvoke();
	}

	private void OpenAnimation()
	{
		iTween.MoveTo(root, iTween.Hash("x", -1278, "islocal", true, "time", 0.2, "delay", 0, "easeType", iTween.EaseType.linear, "oncomplete", "OpenAnimaionEnd", "oncompletetarget", base.gameObject));
	}

	private void CloseAnimation()
	{
		isAnimation = true;
		iTween.MoveTo(root, iTween.Hash("x", -358, "islocal", true, "time", 0.2, "delay", 0, "easeType", iTween.EaseType.linear, "oncomplete", "CloseAnimaionEnd", "oncompletetarget", base.gameObject));
	}

	public void OpenAnimaionEnd()
	{
		isAnimation = false;
	}

	public void CloseAnimaionEnd()
	{
		isAnimation = false;
		UISetter.SetActive(contents, active: false);
	}
}
