using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Regulation;
using UnityEngine;

public class RecordListItem : UIUser
{
	public UIUser user;

	public UISprite winState;

	public UILabel passTime;

	public UILabel vsTag;

	public GameObject replayBtn;

	public GameObject waveDuelRoot;

	public UILabel waveDuelNickname;

	public UILabel waveDuelGuildName;

	public List<GameObject> waveDuelDisableTargets;

	private Protocols.RecordInfo record;

	private Protocols.ChattingRecordInfo chatRecord;

	private const string winSpriteName = "pvp_img_win";

	private const string loseSpriteName = "pvp_img_lose";

	private const string replayItemPrefix = "RePlay-";

	private ERePlayType rePlayType = ERePlayType.Challenge;

	public void Set(ERePlayType type, Protocols.RecordInfo _record)
	{
		record = _record;
		chatRecord = null;
		rePlayType = type;
		UISetter.SetSprite(winState, (record.winState != -1) ? "pvp_img_lose" : "pvp_img_win");
		TimeSpan timeSpan = new TimeSpan(new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(record.date).Ticks);
		TimeSpan timeSpan2 = new TimeSpan(DateTime.UtcNow.Ticks);
		TimeSpan timeSpan3 = timeSpan2 - timeSpan;
		if (timeSpan3.Days > 0)
		{
			UISetter.SetLabel(passTime, string.Format(Localization.Get("1020"), timeSpan3.Days));
		}
		else if (timeSpan3.Hours > 0)
		{
			UISetter.SetLabel(passTime, string.Format(Localization.Get("1019"), timeSpan3.Hours));
		}
		else
		{
			UISetter.SetLabel(passTime, string.Format(Localization.Get("17035"), timeSpan3.Minutes));
		}
		user.Set(record);
	}

	public void Set(Protocols.ChattingRecordInfo data)
	{
		UISetter.SetLabel(vsTag, $"[ {data.userName} vs {data.enemyName} ]");
		UISetter.SetGameObjectName(replayBtn, "RePlay-" + data.id);
		record = null;
		chatRecord = data;
		rePlayType = chatRecord.rePlayType;
	}

	public void Set(EBattleType type, RoUser user)
	{
		Set(user, type);
		if (type == EBattleType.WaveDuel)
		{
			UISetter.SetActive(duelRoot, active: true);
			UISetter.SetActive(waveDuelRoot, active: true);
			UISetter.SetLabel(waveDuelNickname, user.nickname);
			UISetter.SetLabel(waveDuelGuildName, (!GameSetting.instance.guildName) ? string.Empty : (string.IsNullOrEmpty(user.guildName) ? Localization.Get("7180") : user.guildName));
			for (int i = 0; i < waveDuelDisableTargets.Count; i++)
			{
				UISetter.SetActive(waveDuelDisableTargets[i], active: false);
			}
			rePlayType = ERePlayType.WaveDuel;
		}
		else
		{
			UISetter.SetActive(waveDuelRoot, active: false);
			rePlayType = ERePlayType.Challenge;
		}
		if (!string.IsNullOrEmpty(user.replayId))
		{
			UISetter.SetGameObjectName(replayBtn, "RePlay-" + user.replayId);
		}
		else
		{
			UISetter.SetActive(replayBtn, active: false);
		}
		record = null;
		chatRecord = null;
	}

	public void PlayRecord()
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		if (chatRecord != null)
		{
			if (!chatRecord.hasRecord)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19079"));
				return;
			}
			localUser.playingChatRecord = chatRecord;
		}
		BattleData battleData = BattleData.Create(EBattleType.Undefined);
		battleData.attacker = localUser.CreateForBattle(new List<RoTroop> { null });
		battleData.defender = RoUser.Create();
		if (record != null)
		{
			battleData.defender.nickname = record.userName;
			battleData.defender.level = record.level;
			battleData.defender.duelRanking = record.rank;
			battleData.defender.guildName = record.guildName;
		}
		if (chatRecord != null)
		{
			battleData.move = EBattleResultMove.MyTown;
		}
		BattleData.Set(battleData);
		string pureId = Utility.GetPureId(replayBtn.name, "RePlay-");
		RemoteObjectManager.instance.RequestGetRecordInfo(pureId, rePlayType);
	}

	protected bool CheckShareDealyTime()
	{
		if (SocketChatting.instance.inputTmTick != 0)
		{
			long value = DateTime.Now.Ticks - SocketChatting.instance.inputTmTick;
			int num = (int)TimeSpan.FromTicks(value).TotalSeconds;
			int num2 = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["BATTLE_SHARE_DELAY"].value);
			int num3 = num2 - num;
			if (num3 > 0)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7055", num3));
				return false;
			}
		}
		return true;
	}

	public void SharePlayRecord()
	{
		if (CheckShareDealyTime())
		{
			int remainSendTime = SocketChatting.instance.GetRemainSendTime();
			if (remainSendTime > 0)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7055", remainSendTime));
				return;
			}
			Protocols.ChattingRecordInfo chattingRecordInfo = new Protocols.ChattingRecordInfo();
			chattingRecordInfo.id = record.id;
			chattingRecordInfo.userName = record.userInfo.lName;
			chattingRecordInfo.enemyName = record.userInfo.rName;
			chattingRecordInfo.rePlayType = rePlayType;
			Protocols.ChattingMsgData chattingMsgData = new Protocols.ChattingMsgData();
			chattingMsgData.record = chattingRecordInfo;
			Formatting formatting = Formatting.None;
			JsonSerializerSettings serializerSettings = Regulation.SerializerSettings;
			Protocols.ChattingInfo.ChattingData chattingData = new Protocols.ChattingInfo.ChattingData();
			chattingData.sendChannel = RemoteObjectManager.instance.localUser.channel;
			chattingData.sendWorld = RemoteObjectManager.instance.localUser.world;
			chattingData.sendUno = RemoteObjectManager.instance.localUser.uno;
			chattingData.nickname = RemoteObjectManager.instance.localUser.nickname;
			chattingData.guildName = RemoteObjectManager.instance.localUser.guildName;
			chattingData.level = RemoteObjectManager.instance.localUser.level;
			chattingData.thumbnail = RemoteObjectManager.instance.localUser.thumbnailId;
			chattingData.date = RemoteObjectManager.instance.GetCurrentTime();
			chattingData.message = chattingMsgData.ToString();
			string msg = JsonConvert.SerializeObject(chattingData, formatting, serializerSettings);
			SocketChatting.instance.SendAllChannelMessage(msg);
		}
	}
}
