using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using RoomDecorator;
using RoomDecorator.Data;
using Shared;
using Shared.Regulation;
using UnityEngine;

[JsonObject]
public class RoLocalUser : RoUser
{
	public class SlotDispatchInfo
	{
		public string SlotNum;

		public Protocols.DiapatchCommanderInfo dispatchCommanderInfo;
	}

	public class MercynaryUserList
	{
		public int userUno;

		public int userLevel;

		public string npcId;

		public bool isNpc;

		public bool isEngagePossible;

		public List<Protocols.GuildDispatchCommanderList.GuildDispatchCommanderInfo> commanderList = new List<Protocols.GuildDispatchCommanderList.GuildDispatchCommanderInfo>();
	}

	public struct ScenarioBattleInfo
	{
		public string _battleIdx;

		public int _uType;

		public string _unitId;

		public int _unitLevel;

		public int _unitPosition;

		public int _unitGrade;

		public int _unitClass;

		public List<int> _skillLevel;
	}

	public struct ScenarioCompleteInfo
	{
		public bool isComplete;

		public List<string> completeQuarterIdx;
	}

	private int _armyPlatoonNumber;

	private int _navyPlatoonNumber;

	public long requestDefenceRecordTmTick;

	public int chattingDate;

	public long lastChatTimeTick;

	public Protocols.ChattingRecordInfo playingChatRecord;

	public List<string> webEventUrls;

	public bool badgeWebEvent;

	public RoDormitory dormitory;

	public Dictionary<string, List<int>> sweepClearList;

	public Dictionary<string, List<int>> donHaveCommCostumeList;

	public List<int> articleEvent;

	public List<int> commentEvent;

	public List<Protocols.SecretShop.ShopData> shopList;

	public TimeData shopRefreshTime;

	public TimeData weaponMaterialRemainTime1 = new TimeData();

	public TimeData weaponMaterialRemainTime2 = new TimeData();

	public TimeData weaponMaterialRemainTime3 = new TimeData();

	public TimeData weaponMaterialRemainTime4 = new TimeData();

	public int shopRefreshCount;

	public bool shopRefreshFree;

	public int bulletRemain;

	public int oilRemain;

	public int chipRemain;

	public int gachaRemain;

	public string lastBuyPid;

	public string lastIOSPayload;

	public bool worldDuelBattleEnable;

	public RoUser worldDuelBestRank;

	public RoUser worldDuelTarget;

	public RoUser worldDuelReMatchTarget;

	public Protocols.AlarmData alarmData;

	public Protocols.UserInformationResponse.UserGuild guildInfo;

	public Dictionary<int, List<List<string>>> weaponHistory;

	public Dictionary<string, Dictionary<int, int>> infinityStageList;

	public Dictionary<int, Protocols.ConquestTroopInfo.Troop> conquestDeck;

	public List<int> conquestDeckSlotState;

	public List<Protocols.UserInformationResponse.PreDeck> preDeckList;

	public List<RoTroop> defenderTroops;

	private UserLevelDataRow maxLevelData;

	public List<Protocols.VipGacha.VipGachaInfo> gachaInfoList = new List<Protocols.VipGacha.VipGachaInfo>();

	public int vipGachaCount;

	public TimeData vipGachaRefreshTime = new TimeData();

	public List<SlotDispatchInfo> slotDispatchInfo = new List<SlotDispatchInfo>();

	public Protocols.CarnivalList carnivalList;

	public int connectTime;

	public TimeData resetTimeData;

	public bool isFirstLogin;

	public Dictionary<string, TimeData> eventRemaingTime;

	private List<Protocols.GuildDispatchCommanderList.GuildDispatchCommanderInfo> AllCommanderList = new List<Protocols.GuildDispatchCommanderList.GuildDispatchCommanderInfo>();

	public List<RoCommander> EngageCommander = new List<RoCommander>();

	public AfootScenario currScenario = default(AfootScenario);

	public List<MercynaryUserList> allMercynaryList = new List<MercynaryUserList>();

	public Dictionary<string, Dictionary<string, Protocols.CommanderScenario>> sn_resultDictionary;

	public List<BannerInfo> OriginBannerList = new List<BannerInfo>();

	public static string KEY => "6:k}";

	public int exp { get; set; }

	public string mainTroopId { get; set; }

	public float expProgress
	{
		get
		{
			DataTable<UserLevelDataRow> userLevelDtbl = RemoteObjectManager.instance.regulation.userLevelDtbl;
			return 1f;
		}
	}

	public double battleResultCheckTime { get; private set; }

	public List<RoBattleResult> battleResultList { get; private set; }

	public List<RoTroop> troopList { get; private set; }

	public List<RoUnit> unitList { get; private set; }

	public List<RoCommander> commanderList { get; private set; }

	public List<RoReward> rewardList { get; set; }

	public List<Protocols.MailInfo.MailData> mailList { get; set; }

	public List<RoPart> partList { get; set; }

	public Dictionary<string, int> resourceList { get; set; }

	public List<RoMission> missionList { get; set; }

	public List<RoMission> achievementList { get; set; }

	public RoRecruit recruit { get; set; }

	public Dictionary<int, RoUser> duelTargetList { get; set; }

	public int duelTargetIdx { get; set; }

	public List<RoWorldMap> worldMapList { get; set; }

	public TimeData duelTargetRefreshTime { get; set; }

	public List<Protocols.RecordInfo> atkRecordList { get; set; }

	public List<Protocols.RecordInfo> defRecordList { get; set; }

	public bool isUpdateAtkRecord { get; set; }

	public Dictionary<EReward, bool> eventPefectRewardReceiveDict { get; set; }

	public Dictionary<string, RoOccupationInfo> occupationInfoDict { get; private set; }

	public RoStatistics statistics { get; set; }

	public List<string> myAreaIds { get; private set; }

	public List<string> lostAreaIds { get; private set; }

	public Dictionary<EBuilding, RoBuilding> buildingDict { get; private set; }

	public TimeData currentSeasonDuelTime { get; set; }

	public TimeData currentSeasonOpenRemainDuelTime { get; set; }

	public TimeData currentCooperateRemainTime { get; set; }

	public int coopStage { get; set; }

	public int coopStep { get; set; }

	public List<Protocols.ChattingInfo.ChattingData> chattingWhisperList { get; private set; }

	public List<Protocols.ChattingInfo.ChattingData> chattingGuildList { get; private set; }

	public List<Protocols.ChattingInfo.ChattingData> chattingchannelList { get; private set; }

	public List<string> coupons { get; set; }

	public List<Protocols.ChattingInfo.ChattingData>[] chatMessages { get; private set; }

	public Dictionary<string, Protocols.BlockUser> blockUsers { get; private set; }

	public RoExplorationTable explorationDtbl { get; private set; }

	public int newMailCount { get; set; }

	public int gold
	{
		get
		{
			return resourceList["gold"];
		}
		set
		{
			resourceList["gold"] = value;
		}
	}

	public int cash
	{
		get
		{
			return resourceList["cash"];
		}
		set
		{
			resourceList["cash"] = value;
		}
	}

	public int challengeCoin
	{
		get
		{
			return resourceList["acon"];
		}
		set
		{
			resourceList["acon"] = value;
		}
	}

	public int raidCoin
	{
		get
		{
			return resourceList["rcon"];
		}
		set
		{
			resourceList["rcon"] = value;
		}
	}

	public int guildCoin
	{
		get
		{
			return resourceList["gcon"];
		}
		set
		{
			resourceList["gcon"] = value;
		}
	}

	public int annCoin
	{
		get
		{
			return resourceList["ncon"];
		}
		set
		{
			resourceList["ncon"] = value;
		}
	}

	public int vipLevel { get; set; }

	public int vipExp { get; set; }

	public int timeMachine
	{
		get
		{
			return resourceList["tmm"];
		}
		set
		{
			resourceList["tmm"] = value;
		}
	}

	public int honor { get; set; }

	public int medal
	{
		get
		{
			return resourceList["medl"];
		}
		set
		{
			resourceList["medl"] = value;
		}
	}

	public int medal2
	{
		get
		{
			return resourceList["medl2"];
		}
		set
		{
			resourceList["medl2"] = value;
		}
	}

	public int medal3
	{
		get
		{
			return resourceList["medl3"];
		}
		set
		{
			resourceList["medl3"] = value;
		}
	}

	public int medal4
	{
		get
		{
			return resourceList["medl4"];
		}
		set
		{
			resourceList["medl4"] = value;
		}
	}

	public int medal5
	{
		get
		{
			return resourceList["medl5"];
		}
		set
		{
			resourceList["medl5"] = value;
		}
	}

	public int munitions { get; set; }

	public int chip { get; set; }

	public int blueprintArmy { get; set; }

	public int blueprintNavy { get; set; }

	public int explorationTicket { get; set; }

	public int sweepTicket { get; set; }

	public int opener
	{
		get
		{
			return resourceList["keys"];
		}
		set
		{
			resourceList["keys"] = value;
		}
	}

	public int eventRaidTicket
	{
		get
		{
			return resourceList["ebac"];
		}
		set
		{
			resourceList["ebac"] = value;
		}
	}

	public int opcon { get; set; }

	public int challenge
	{
		get
		{
			return resourceList["chlg"];
		}
		set
		{
			resourceList["chlg"] = value;
		}
	}

	public int waveDuelTicket
	{
		get
		{
			return resourceList["wbt"];
		}
		set
		{
			resourceList["wbt"] = value;
		}
	}

	public int waveDuelCoin
	{
		get
		{
			return resourceList["wbc"];
		}
		set
		{
			resourceList["wbc"] = value;
		}
	}

	public int worldDuelTicket
	{
		get
		{
			return resourceList["sbtk"];
		}
		set
		{
			resourceList["sbtk"] = value;
		}
	}

	public int worldDuelCoin
	{
		get
		{
			return resourceList["sbc1"];
		}
		set
		{
			resourceList["sbc1"] = value;
		}
	}

	public int worldDuelUpgradeCoin
	{
		get
		{
			return resourceList["sbc2"];
		}
		set
		{
			resourceList["sbc2"] = value;
		}
	}

	public int cooperateBattleTicket { get; set; }

	public int bullet
	{
		get
		{
			return resourceList["bult"];
		}
		set
		{
			resourceList["bult"] = value;
		}
	}

	public int oil
	{
		get
		{
			return resourceList["oil"];
		}
		set
		{
			resourceList["oil"] = value;
		}
	}

	public int ring
	{
		get
		{
			return resourceList["ring"];
		}
		set
		{
			resourceList["ring"] = value;
		}
	}

	public int blackChallenge { get; set; }

	public int commanderPromotionPoint { get; set; }

	public int commanderTrainingTicket1
	{
		get
		{
			return resourceList["ctt1"];
		}
		set
		{
			resourceList["ctt1"] = value;
		}
	}

	public int commanderTrainingTicket2
	{
		get
		{
			return resourceList["ctt2"];
		}
		set
		{
			resourceList["ctt2"] = value;
		}
	}

	public int commanderTrainingTicket3
	{
		get
		{
			return resourceList["ctt3"];
		}
		set
		{
			resourceList["ctt3"] = value;
		}
	}

	public int commanderTrainingTicket4
	{
		get
		{
			return resourceList["ctt4"];
		}
		set
		{
			resourceList["ctt4"] = value;
		}
	}

	public int weaponMaterial1
	{
		get
		{
			return resourceList["wmat1"];
		}
		set
		{
			resourceList["wmat1"] = value;
		}
	}

	public int weaponMaterial2
	{
		get
		{
			return resourceList["wmat2"];
		}
		set
		{
			resourceList["wmat2"] = value;
		}
	}

	public int weaponMaterial3
	{
		get
		{
			return resourceList["wmat3"];
		}
		set
		{
			resourceList["wmat3"] = value;
		}
	}

	public int weaponMaterial4
	{
		get
		{
			return resourceList["wmat4"];
		}
		set
		{
			resourceList["wmat4"] = value;
		}
	}

	public int weaponImmediateTicket
	{
		get
		{
			return resourceList["wimt"];
		}
		set
		{
			resourceList["wimt"] = value;
		}
	}

	public int weaponMakeTicket
	{
		get
		{
			return resourceList["wmt"];
		}
		set
		{
			resourceList["wmt"] = value;
		}
	}

	public int commanderGift { get; set; }

	public int dormitoryPoint
	{
		get
		{
			return resourceList["drpt"];
		}
		set
		{
			resourceList["drpt"] = value;
		}
	}

	public int wood
	{
		get
		{
			return resourceList["wood"];
		}
		set
		{
			resourceList["wood"] = value;
		}
	}

	public int ston
	{
		get
		{
			return resourceList["ston"];
		}
		set
		{
			resourceList["ston"] = value;
		}
	}

	public int elec
	{
		get
		{
			return resourceList["elec"];
		}
		set
		{
			resourceList["elec"] = value;
		}
	}

	public int achievementGoal { get; set; }

	public int missionGoal { get; set; }

	public int achievementCompleteCount { get; set; }

	public int missionCompleteCount { get; set; }

	public double serverDBVersion { get; set; }

	public int gameVersionState { get; set; }

	public bool bDownLoadFileCheck { get; set; }

	public bool bEnableGoogleAccount { get; set; }

	public Dictionary<string, double> badWordVersions { get; set; }

	public Dictionary<string, List<string>> badWords { get; set; }

	public double userTermVersion { get; set; }

	public bool bShowUserTerm { get; set; }

	public int badgeNewMailCount { get; set; }

	public int badgeMissionCount { get; set; }

	public int badgeAchievementCount { get; set; }

	public bool badgeChat { get; set; }

	public bool badgeChallenge { get; set; }

	public bool badgeWorldMap { get; set; }

	public bool badgeRaidShop { get; set; }

	public bool badgeChallengeShop { get; set; }

	public bool badgeWaveDuelShop { get; set; }

	public int badgeGroupCount { get; set; }

	public bool badgeEventRaidReward { get; set; }

	public bool badgeInfinityBattleReward { get; set; }

	public Dictionary<int, bool> badgeCarnivalComplete { get; set; }

	public Dictionary<int, List<string>> badgeCarnivalTabList { get; set; }

	public bool badgeWaveBattle { get; set; }

	public bool badgeGuild { get; set; }

	public RoDailyBonus dailyBonus { get; set; }

	public Dictionary<string, RoGacha> gacha { get; set; }

	public Dictionary<string, int> stageRechargeList { get; private set; }

	public Dictionary<string, int> resourceRechargeList { get; private set; }

	public List<string> assetbundleSpineKey { get; private set; }

	public Dictionary<int, string> noticeList { get; private set; }

	public List<Protocols.NoticeData> eventNoticeList { get; set; }

	public bool useBullet { get; set; }

	public int tempBullet { get; set; }

	public bool changeSkillPoint { get; set; }

	public bool sendingGift { get; set; }

	public TutorialData tutorialData { get; set; }

	public int tutorialStep
	{
		get
		{
			return tutorialData.curStep;
		}
		set
		{
			tutorialData.curStep = value;
		}
	}

	private bool _isEnableTutorial { get; set; }

	public bool isEnableTutorial
	{
		get
		{
			if (tutorialStep < ConstValue.tutorialEndStep)
			{
				return true;
			}
			return _isEnableTutorial;
		}
		set
		{
			_isEnableTutorial = value;
		}
	}

	public bool isTutorialSkip
	{
		get
		{
			return tutorialData.skip;
		}
		set
		{
			tutorialData.skip = value;
		}
	}

	public new string guildName
	{
		get
		{
			if (!IsExistGuild())
			{
				return string.Empty;
			}
			return guildInfo.name;
		}
	}

	public string _conquestTeam { get; set; }

	public EConquestTeam conquestTeam
	{
		get
		{
			if (string.IsNullOrEmpty(_conquestTeam))
			{
				return EConquestTeam.None;
			}
			if (_conquestTeam == "R")
			{
				return EConquestTeam.Red;
			}
			if (_conquestTeam == "B")
			{
				return EConquestTeam.Blue;
			}
			return EConquestTeam.None;
		}
	}

	private List<RoItem> EquipPossibleItemList { get; set; }

	private List<RoItem> EquipedItemList { get; set; }

	public bool isGoLaboratory { get; set; }

	public RoItem curSelectItem_forLaboratory { get; set; }

	public string curSelectCommanderId_forLaboratory { get; set; }

	public int lastShowEventScenarioPlayTurn { get; set; }

	private List<RoWeapon> weaponList { get; set; }

	public List<string> newWeaponList { get; set; }

	public bool isMaxLevel => exp >= maxLevelData.uExp;

	public int GenerateNextPlatoonNumber(EBranch branch)
	{
		return branch switch
		{
			EBranch.Army => ++_armyPlatoonNumber, 
			EBranch.Navy => ++_navyPlatoonNumber, 
			_ => 1, 
		};
	}

	public int PeekNextPlatoonNumber(EBranch branch)
	{
		return branch switch
		{
			EBranch.Army => _armyPlatoonNumber + 1, 
			EBranch.Navy => _navyPlatoonNumber + 1, 
			_ => 1, 
		};
	}

	public void ChatBotMessage(int channelIdx, string msg)
	{
		Formatting formatting = Formatting.None;
		JsonSerializerSettings serializerSettings = Regulation.SerializerSettings;
		Protocols.ChattingMsgData chattingMsgData = new Protocols.ChattingMsgData();
		chattingMsgData.data = msg;
		msg = JsonConvert.SerializeObject(chattingMsgData, formatting, serializerSettings);
		Protocols.ChattingInfo.ChattingData chattingData = new Protocols.ChattingInfo.ChattingData();
		chattingData.nickname = base.nickname;
		chattingData.level = base.level;
		chattingData.thumbnail = base.thumbnailId;
		chattingData.date = RemoteObjectManager.instance.GetCurrentTime();
		chattingData.message = msg;
		chatMessages[channelIdx].Add(chattingData);
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
	}

	public static RoLocalUser CreateLocalUser(string id = null)
	{
		RoLocalUser roLocalUser = new RoLocalUser();
		roLocalUser.id = id;
		roLocalUser.InitEmpty();
		roLocalUser.duelTargetRefreshTime = TimeData.Create();
		roLocalUser.currentSeasonDuelTime = TimeData.Create();
		roLocalUser.currentSeasonOpenRemainDuelTime = TimeData.Create();
		roLocalUser.shopRefreshTime = TimeData.Create();
		roLocalUser.currentCooperateRemainTime = TimeData.Create();
		return roLocalUser;
	}

	public RoUser CreateForBattle(string troopId)
	{
		return CreateForBattle(new List<string> { troopId });
	}

	public RoUser CreateForBattle(List<string> readyTroopIdList)
	{
		RoUser roUser = RoUser.Create(base.id);
		roUser.nickname = base.nickname;
		roUser.guildName = guildName;
		roUser.level = base.level;
		roUser.activeBuff = base.activeBuff;
		roUser.worldDuelBuff = base.worldDuelBuff;
		roUser.battleTroopList = troopList.FindAll((RoTroop troop) => readyTroopIdList.Contains(troop.id));
		return roUser;
	}

	public RoUser CreateForBattle(List<RoTroop> troopList)
	{
		RoUser roUser = RoUser.Create(base.id);
		roUser.nickname = base.nickname;
		roUser.guildName = guildName;
		roUser.level = base.level;
		roUser.world = base.world;
		roUser.battleTroopList = troopList;
		roUser.guildSkillList = base.guildSkillList;
		roUser.activeBuff = base.activeBuff;
		roUser.worldDuelBuff = base.worldDuelBuff;
		roUser.completeRewardGroupList = completeRewardGroupList;
		return roUser;
	}

	public override void InitEmpty()
	{
		if (battleResultList == null)
		{
			battleResultList = new List<RoBattleResult>();
		}
		if (troopList == null)
		{
			troopList = new List<RoTroop>();
		}
		if (occupationInfoDict == null)
		{
			occupationInfoDict = new Dictionary<string, RoOccupationInfo>();
		}
		if (myAreaIds == null)
		{
			myAreaIds = new List<string>();
		}
		if (lostAreaIds == null)
		{
			lostAreaIds = new List<string>();
		}
		if (buildingDict == null)
		{
			buildingDict = new Dictionary<EBuilding, RoBuilding>();
		}
		if (rewardList == null)
		{
			rewardList = new List<RoReward>();
			newMailCount = 0;
		}
		if (partList == null)
		{
			partList = new List<RoPart>();
		}
		if (missionList == null)
		{
			missionList = new List<RoMission>();
		}
		if (achievementList == null)
		{
			achievementList = new List<RoMission>();
		}
		if (base.guildSkillList == null)
		{
			base.guildSkillList = new List<RoGuildSkill>();
		}
		if (eventPefectRewardReceiveDict == null)
		{
			eventPefectRewardReceiveDict = new Dictionary<EReward, bool>();
			eventPefectRewardReceiveDict.Add(EReward.DailyMission, value: false);
			eventPefectRewardReceiveDict.Add(EReward.WeeklyMission, value: false);
			eventPefectRewardReceiveDict.Add(EReward.MonthlyMission, value: false);
		}
		if (statistics == null)
		{
			statistics = RoStatistics.Create();
		}
		if (recruit == null)
		{
			recruit = RoRecruit.Create();
		}
		if (unitList == null)
		{
			unitList = new List<RoUnit>();
		}
		if (commanderList == null)
		{
			commanderList = new List<RoCommander>();
		}
		if (duelTargetList == null)
		{
			duelTargetList = new Dictionary<int, RoUser>();
		}
		if (atkRecordList == null)
		{
			atkRecordList = new List<Protocols.RecordInfo>();
			isUpdateAtkRecord = true;
		}
		if (defRecordList == null)
		{
			defRecordList = new List<Protocols.RecordInfo>();
		}
		if (worldMapList == null)
		{
			worldMapList = new List<RoWorldMap>();
		}
		if (dailyBonus == null)
		{
			dailyBonus = RoDailyBonus.Create();
		}
		if (gacha == null)
		{
			gacha = new Dictionary<string, RoGacha>();
		}
		if (chattingWhisperList == null)
		{
			chattingWhisperList = new List<Protocols.ChattingInfo.ChattingData>();
		}
		if (chattingchannelList == null)
		{
			chattingchannelList = new List<Protocols.ChattingInfo.ChattingData>();
		}
		if (chattingGuildList == null)
		{
			chattingGuildList = new List<Protocols.ChattingInfo.ChattingData>();
		}
		if (coupons == null)
		{
			coupons = new List<string>();
		}
		if (chatMessages == null)
		{
			chatMessages = new List<Protocols.ChattingInfo.ChattingData>[4];
			for (int i = 0; i < chatMessages.Length; i++)
			{
				chatMessages[i] = new List<Protocols.ChattingInfo.ChattingData>();
			}
		}
		if (blockUsers == null)
		{
			blockUsers = new Dictionary<string, Protocols.BlockUser>();
		}
		if (mailList == null)
		{
			mailList = new List<Protocols.MailInfo.MailData>();
		}
		if (stageRechargeList == null)
		{
			stageRechargeList = new Dictionary<string, int>();
		}
		if (resourceRechargeList == null)
		{
			resourceRechargeList = new Dictionary<string, int>();
		}
		if (resourceList == null)
		{
			resourceList = new Dictionary<string, int>();
		}
		if (shopList == null)
		{
			shopList = new List<Protocols.SecretShop.ShopData>();
		}
		if (defenderTroops == null)
		{
			defenderTroops = new List<RoTroop>();
			defenderTroops.Add(RoTroop.Create(base.id));
		}
		if (sweepClearList == null)
		{
			sweepClearList = new Dictionary<string, List<int>>();
		}
		if (donHaveCommCostumeList == null)
		{
			donHaveCommCostumeList = new Dictionary<string, List<int>>();
		}
		if (completeRewardGroupList == null)
		{
			completeRewardGroupList = new List<int>();
		}
		if (preDeckList == null)
		{
			preDeckList = new List<Protocols.UserInformationResponse.PreDeck>();
		}
		if (assetbundleSpineKey == null)
		{
			assetbundleSpineKey = new List<string>();
		}
		if (tutorialData == null)
		{
			tutorialData = new TutorialData();
		}
		if (noticeList == null)
		{
			noticeList = new Dictionary<int, string>();
		}
		if (eventNoticeList == null)
		{
			eventNoticeList = new List<Protocols.NoticeData>();
		}
		if (badgeCarnivalTabList == null)
		{
			badgeCarnivalTabList = new Dictionary<int, List<string>>();
			badgeCarnivalComplete = new Dictionary<int, bool>();
			for (int j = 0; j < 4; j++)
			{
				badgeCarnivalTabList.Add(j, new List<string>());
				badgeCarnivalComplete.Add(j, value: false);
			}
		}
		if (EquipPossibleItemList == null)
		{
			EquipPossibleItemList = new List<RoItem>();
		}
		if (EquipedItemList == null)
		{
			EquipedItemList = new List<RoItem>();
		}
		if (explorationDtbl == null)
		{
			explorationDtbl = new RoExplorationTable();
		}
		if (eventRemaingTime == null)
		{
			eventRemaingTime = new Dictionary<string, TimeData>();
		}
		if (base.activeBuff == null)
		{
			base.activeBuff = new Dictionary<EWorldDuelBuff, EWorldDuelBuffEffect>();
			base.activeBuff.Add(EWorldDuelBuff.att, EWorldDuelBuffEffect.b);
			base.activeBuff.Add(EWorldDuelBuff.def, EWorldDuelBuffEffect.b);
			base.activeBuff.Add(EWorldDuelBuff.sup, EWorldDuelBuffEffect.b);
		}
		if (weaponList == null)
		{
			weaponList = new List<RoWeapon>();
		}
		if (newWeaponList == null)
		{
			newWeaponList = new List<string>();
		}
		if (weaponHistory == null)
		{
			weaponHistory = new Dictionary<int, List<List<string>>>();
		}
		if (dormitory == null)
		{
			dormitory = new RoDormitory();
		}
		base.lastPlayStage = -1;
		chattingDate = 0;
		shopRefreshCount = 0;
		base.lastConquestReplayPoint = 0;
		isFirstLogin = false;
		coopStage = 0;
		coopStep = 0;
	}

	public void InitData()
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		regulation.unitDtbl.ForEach(delegate(UnitDataRow unit)
		{
			unitList.Add(RoUnit.Create(unit.key, 1, 1, 1, 0, "0", 0, 0, new List<int>()));
		});
		regulation.partDtbl.ForEach(delegate(PartDataRow part)
		{
			partList.Add(RoPart.Create(part.type, 0));
		});
		regulation.guildSkillDtbl.ForEach(delegate(GuildSkillDataRow guild)
		{
			if (guild.skilllevel == 0)
			{
				base.guildSkillList.Add(RoGuildSkill.Create(guild.idx));
			}
		});
		regulation.sweepDtbl.ForEach(delegate(SweepDataRow sweep)
		{
			if (!sweepClearList.ContainsKey(sweep.type.ToString()))
			{
				sweepClearList.Add(sweep.type.ToString(), new List<int>());
			}
		});
		if (conquestDeck == null)
		{
			int num = int.Parse(regulation.defineDtbl["GUILD_OCCUPY_TEAM"].value) + int.Parse(regulation.defineDtbl["GUILD_OCCUPY_PREMIUM_TEAM"].value);
			conquestDeck = new Dictionary<int, Protocols.ConquestTroopInfo.Troop>();
			for (int i = 0; i < num; i++)
			{
				conquestDeck.Add(i + 1, null);
			}
		}
		regulation.SetCommander();
		SetResourceList();
		buildingDict = _CreateBuildingDict();
		worldMapList.Clear();
		worldMapList = RoWorldMap.CreateAll();
		explorationDtbl.Init();
		dormitory.Init();
		int length = RemoteObjectManager.instance.regulation.userLevelDtbl.length;
		maxLevelData = RemoteObjectManager.instance.regulation.userLevelDtbl[length - 1];
	}

	private Dictionary<EBuilding, RoBuilding> _CreateBuildingDict()
	{
		Dictionary<EBuilding, RoBuilding> dictionary = new Dictionary<EBuilding, RoBuilding>();
		string[] names = Enum.GetNames(typeof(EBuilding));
		Type typeFromHandle = typeof(EBuilding);
		for (int i = 0; i < names.Length; i++)
		{
			string text = names[i];
			EBuilding eBuilding = (EBuilding)Enum.Parse(typeFromHandle, names[i]);
			dictionary.Add(eBuilding, RoBuilding.Create(eBuilding));
		}
		return dictionary;
	}

	public void AddExp(int addExp)
	{
		exp += addExp;
		DataTable<UserLevelDataRow> userLevelDtbl = RemoteObjectManager.instance.regulation.userLevelDtbl;
		for (int i = base.level; i < userLevelDtbl.length; i++)
		{
			UserLevelDataRow userLevelDataRow = userLevelDtbl[i];
			if (userLevelDataRow.exp > exp)
			{
				break;
			}
			base.level = userLevelDataRow.level;
		}
	}

	private void SetResourceList()
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		regulation.goodsDtbl.ForEach(delegate(GoodsDataRow item)
		{
			if (!resourceList.ContainsKey(item.serverFieldName))
			{
				resourceList.Add(item.serverFieldName, 0);
			}
		});
	}

	public Dictionary<string, int> GetUserResourceList(EStorageType strorageType)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (KeyValuePair<string, int> resource in resourceList)
		{
			GoodsDataRow row = RemoteObjectManager.instance.regulation.FindGoodsByServerFieldName(resource.Key);
			switch (strorageType)
			{
			case EStorageType.Goods:
			{
				ItemExchangeDataRow itemExchangeDataRow2 = RemoteObjectManager.instance.regulation.itemExchangeDtbl.Find((ItemExchangeDataRow item) => item.typeidx == row.type && (item.type == EStorageType.Goods || item.type == EStorageType.Box));
				if (itemExchangeDataRow2 != null && (itemExchangeDataRow2.type == EStorageType.Goods || itemExchangeDataRow2.type == EStorageType.Box) && row.storage && resource.Value > 0)
				{
					dictionary.Add(resource.Key, resource.Value);
				}
				break;
			}
			case EStorageType.Food:
			{
				ItemExchangeDataRow itemExchangeDataRow = RemoteObjectManager.instance.regulation.itemExchangeDtbl.Find((ItemExchangeDataRow item) => item.typeidx == row.type && item.type == EStorageType.Food);
				if (itemExchangeDataRow != null && itemExchangeDataRow.type == EStorageType.Food && row.storage && resource.Value > 0)
				{
					dictionary.Add(resource.Key, resource.Value);
				}
				break;
			}
			}
		}
		return dictionary;
	}

	public void SetSweepClearState(Dictionary<string, List<int>> list)
	{
		foreach (KeyValuePair<string, List<int>> item in list)
		{
			for (int i = 0; i < item.Value.Count; i++)
			{
				if (sweepClearList.ContainsKey(item.Key))
				{
					sweepClearList[item.Key].Add(item.Value[i]);
				}
			}
		}
	}

	public bool GetSweepClearState(int type, int level)
	{
		if (!sweepClearList.ContainsKey(type.ToString()))
		{
			return false;
		}
		List<int> list = sweepClearList[type.ToString()];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == level)
			{
				return true;
			}
		}
		return false;
	}

	public void ResetSweepDeck()
	{
		int num = 6;
		List<SweepDataRow> list = RemoteObjectManager.instance.regulation.sweepDtbl.FindAll((SweepDataRow row) => row.type == 1);
		if (list != null && list.Count > 0)
		{
			num = list[list.Count - 1].level;
		}
		for (int i = 1; i < 4; i++)
		{
			for (int j = 1; j < num + 1; j++)
			{
				string key = $"SweepDeck_{i}_{j}";
				PlayerPrefs.DeleteKey(key);
			}
		}
	}

	public void AddSweepClearState(int type, int level)
	{
		for (int i = 0; i < sweepClearList[type.ToString()].Count; i++)
		{
			if (sweepClearList[type.ToString()][i] == level)
			{
				return;
			}
		}
		sweepClearList[type.ToString()].Add(level);
	}

	public void AddGroupCompleteData(int grid)
	{
		if (completeRewardGroupList == null || completeRewardGroupList.Contains(grid))
		{
			return;
		}
		completeRewardGroupList.Add(grid);
		Regulation regulation = RemoteObjectManager.instance.regulation;
		completeRewardGroupList.Sort((int a, int b) => regulation.groupInfoDtbl.Find((GroupInfoDataRow data) => int.Parse(data.groupIdx) == b).typeIndex.CompareTo(regulation.groupInfoDtbl.Find((GroupInfoDataRow data) => int.Parse(data.groupIdx) == a).typeIndex));
	}

	public int RewardGroupCountInTap(string tapIdx)
	{
		int num = 0;
		List<GroupInfoDataRow> list = RemoteObjectManager.instance.regulation.groupInfoDtbl.FindAll((GroupInfoDataRow row) => row.tabidx == tapIdx);
		foreach (GroupInfoDataRow item in list)
		{
			if (!completeRewardGroupList.Contains(int.Parse(item.groupIdx)) && isGetRewardGroup(item.groupIdx))
			{
				num++;
			}
		}
		return num;
	}

	public bool isGetRewardGroup(string grid)
	{
		List<GroupMemberDataRow> list = RemoteObjectManager.instance.regulation.FindGroupMemberList(grid);
		foreach (GroupMemberDataRow data in list)
		{
			if (data.memberType == 1)
			{
				RoCommander roCommander = commanderList.Find((RoCommander row) => row.id == data.memberIdx);
				if (roCommander != null && (roCommander.state != ECommanderState.Nomal || (int)roCommander.cls < data.grade))
				{
					return false;
				}
			}
			else if (data.memberType == 0)
			{
				CommanderCostumeDataRow costumeData = RemoteObjectManager.instance.regulation.commanderCostumeDtbl[data.memberIdx];
				RoCommander roCommander2 = commanderList.Find((RoCommander row) => row.id == costumeData.cid.ToString());
				if (roCommander2.state == ECommanderState.Nomal)
				{
					if (!roCommander2.haveCostumeList.Contains(int.Parse(data.memberIdx)))
					{
						return false;
					}
				}
				else if (!isGetDonHaveCommCostume(roCommander2.id, int.Parse(data.memberIdx)))
				{
					return false;
				}
			}
			else if (data.memberType == 2)
			{
				GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.goodsDtbl[data.memberIdx];
				int num = resourceList[goodsDataRow.serverFieldName];
				if (data.grade > num)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void SetDonHaveCommCostume(Dictionary<string, List<int>> list)
	{
		if (list != null)
		{
			donHaveCommCostumeList = list;
		}
	}

	public List<int> GetDonHaveCommCostumeList(string gid)
	{
		if (!donHaveCommCostumeList.ContainsKey(gid))
		{
			return null;
		}
		return donHaveCommCostumeList[gid];
	}

	public bool isGetDonHaveCommCostume(string gid, int ctid)
	{
		if (!donHaveCommCostumeList.ContainsKey(gid))
		{
			return false;
		}
		List<int> list = donHaveCommCostumeList[gid];
		return list.Contains(ctid);
	}

	public void AddDonHaveCommCostume(string cid, int ctid)
	{
		if (donHaveCommCostumeList.ContainsKey(cid))
		{
			if (!donHaveCommCostumeList[cid].Contains(ctid))
			{
				donHaveCommCostumeList[cid].Add(ctid);
			}
		}
		else
		{
			donHaveCommCostumeList.Add(cid, new List<int> { ctid });
		}
	}

	public RoTroop FindTroop(string id)
	{
		return troopList.Find((RoTroop troop) => troop.id == id);
	}

	public RoTroop FindTroopByCommanderId(string commanderId)
	{
		return null;
	}

	public RoTroop FindDefenderTroop()
	{
		List<RoCommander> defenderCommanderList = GetDefenderCommanderList();
		if (defenderCommanderList.Count < 1)
		{
			return null;
		}
		RoTroop roTroop = RoTroop.Create(base.id);
		for (int i = 0; i < defenderCommanderList.Count; i++)
		{
			RoCommander roCommander = defenderCommanderList[i];
			RoTroop.Slot nextEmptySlot = roTroop.GetNextEmptySlot();
			nextEmptySlot.unitId = roCommander.unitId;
			nextEmptySlot.unitLevel = roCommander.level;
		}
		return roTroop;
	}

	public RoTroop FindAttackerTroop()
	{
		return null;
	}

	public RoTroop FindScrambleTroop()
	{
		return null;
	}

	public RoTroop FindOccupationTroop()
	{
		return null;
	}

	public RoUser FindDuelUser(string id)
	{
		return duelTargetList[int.Parse(id)];
	}

	public RoWorldMap FindWorldMap(string id)
	{
		return worldMapList.Find((RoWorldMap worldMap) => worldMap.id == id);
	}

	public RoWorldMap FindWorldMapByOffset(string id, int offset)
	{
		DataTable<WorldMapDataRow> worldMapDtbl = RemoteObjectManager.instance.regulation.worldMapDtbl;
		if (!worldMapDtbl.ContainsKey(id))
		{
			return null;
		}
		int num = worldMapDtbl.FindIndex(id);
		if (worldMapDtbl.IsValidIndex(num + offset))
		{
			return FindWorldMap(worldMapDtbl[num + offset].id);
		}
		return null;
	}

	public RoWorldMap FindWorldMapByStage(string stageId)
	{
		DataTable<WorldMapStageDataRow> worldMapStageDtbl = RemoteObjectManager.instance.regulation.worldMapStageDtbl;
		if (!worldMapStageDtbl.ContainsKey(stageId))
		{
			return null;
		}
		return FindWorldMap(worldMapStageDtbl[stageId].worldMapId);
	}

	public RoWorldMap.Stage FindWorldMapStage(string stageId)
	{
		return FindWorldMapByStage(stageId)?.FindStage(stageId);
	}

	public RoWorldMap FindLastOpenedWorldMap()
	{
		RoWorldMap roWorldMap = FindWorldMapByStage((base.lastClearStage + 1).ToString());
		if (roWorldMap == null || roWorldMap.dataRow.unlockUserLevel > base.level)
		{
			roWorldMap = FindWorldMapByStage(base.lastClearStage.ToString());
		}
		return roWorldMap;
	}

	public bool canMoveWorldMap(string id)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		int num = regulation.worldMapDtbl.FindIndex(id);
		if (!regulation.worldMapDtbl.IsValidIndex(num))
		{
			return false;
		}
		RoWorldMap roWorldMap = FindLastOpenedWorldMap();
		if (int.Parse(roWorldMap.id) < int.Parse(id))
		{
			return false;
		}
		if (regulation.worldMapDtbl[num].unlockUserLevel > base.level)
		{
			return false;
		}
		return true;
	}

	public List<RoTroop> GetUserTroopList(EBranch branch)
	{
		return new List<RoTroop>();
	}

	public List<RoCommander> GetCommanderList(EJob job)
	{
		List<RoCommander> list = new List<RoCommander>();
		commanderList.ForEach(delegate(RoCommander commander)
		{
			if ((commander.branch == EBranch.Army || commander.branch == EBranch.Undefined) && (commander.job == job || job == EJob.All) && commander.state == ECommanderState.Nomal)
			{
				list.Add(commander);
			}
		});
		return list;
	}

	public int GetCommanderCount()
	{
		int cnt = 0;
		commanderList.ForEach(delegate(RoCommander commander)
		{
			if ((commander.branch == EBranch.Army || commander.branch == EBranch.Undefined) && commander.state == ECommanderState.Nomal)
			{
				cnt++;
			}
		});
		return cnt;
	}

	public int GetAnnihilationCommanderCount()
	{
		int cnt = 0;
		commanderList.ForEach(delegate(RoCommander commander)
		{
			if ((commander.branch == EBranch.Army || commander.branch == EBranch.Undefined) && commander.state == ECommanderState.Nomal && commander.isAdvancePossible && !commander.isDie)
			{
				cnt++;
			}
		});
		return cnt;
	}

	public int GetGuerrillaCommanderCount(EJob job)
	{
		int cnt = 0;
		commanderList.ForEach(delegate(RoCommander commander)
		{
			if ((commander.branch == EBranch.Army || commander.branch == EBranch.Undefined) && commander.state == ECommanderState.Nomal && commander.job == job)
			{
				cnt++;
			}
		});
		return cnt;
	}

	public int GetConquestCommanderCount(int conquestDeckId)
	{
		int cnt = 0;
		commanderList.ForEach(delegate(RoCommander commander)
		{
			if ((commander.branch == EBranch.Army || commander.branch == EBranch.Undefined) && commander.state == ECommanderState.Nomal && commander.conquestDeckId == 0)
			{
				cnt++;
			}
		});
		return cnt;
	}

	public List<RoCommander> GetCommanderList(EBranch branch, bool have)
	{
		List<RoCommander> list = new List<RoCommander>();
		commanderList.ForEach(delegate(RoCommander commander)
		{
			if (branch == commander.branch || branch == EBranch.Undefined)
			{
				if (have)
				{
					if (commander.state == ECommanderState.Nomal)
					{
						list.Add(commander);
					}
				}
				else if (commander.state != ECommanderState.Nomal)
				{
					list.Add(commander);
				}
			}
		});
		return list;
	}

	public List<RoCommander> GetCommanderList(EJob job, bool have, bool recruit = false)
	{
		List<RoCommander> list = new List<RoCommander>();
		commanderList.ForEach(delegate(RoCommander commander)
		{
			bool flag = false;
			if (job == EJob.All)
			{
				flag = true;
			}
			if (job == commander.job || job == EJob.Undefined)
			{
				flag = true;
			}
			if (flag)
			{
				if (have)
				{
					if (commander.state == ECommanderState.Nomal)
					{
						list.Add(commander);
					}
					else if (recruit && commander.medal >= commander.maxMedal)
					{
						list.Add(commander);
					}
				}
				else if (commander.state != ECommanderState.Nomal)
				{
					if (recruit)
					{
						if (commander.medal < commander.maxMedal)
						{
							list.Add(commander);
						}
					}
					else
					{
						list.Add(commander);
					}
				}
			}
		});
		return list;
	}

	public List<RoCommander> GetDefenderCommanderList()
	{
		List<RoCommander> list = new List<RoCommander>();
		commanderList.ForEach(delegate(RoCommander commander)
		{
			if (commander.state == ECommanderState.Nomal && commander.defender)
			{
				list.Add(commander);
			}
		});
		return list;
	}

	public RoBuilding FindBuilding(EBuilding type)
	{
		if (!buildingDict.ContainsKey(type))
		{
			return null;
		}
		return buildingDict[type];
	}

	public RoBuilding FindBuilding(string typeId)
	{
		EBuilding type = (EBuilding)Enum.Parse(typeof(EBuilding), typeId);
		return FindBuilding(type);
	}

	public RoCommander FindCommander(string id)
	{
		return commanderList.Find((RoCommander commander) => commander.id == id);
	}

	public void CommanderStatusReset()
	{
		commanderList.ForEach(delegate(RoCommander commander)
		{
			commander.StatusReset();
		});
	}

	public RoCommander FindCommanderResourceId(string resourceId)
	{
		return commanderList.Find((RoCommander commander) => commander.resourceId == resourceId);
	}

	public void ResetCommanderCostume()
	{
		if (commanderList == null)
		{
			return;
		}
		for (int i = 0; i < commanderList.Count; i++)
		{
			if (PlayerPrefs.HasKey(commanderList[i].id))
			{
				PlayerPrefs.DeleteKey(commanderList[i].id);
			}
		}
	}

	public void ResetCommanderMedals()
	{
		if (commanderList != null)
		{
			for (int i = 0; i < commanderList.Count; i++)
			{
				commanderList[i].aMedal = 0;
			}
		}
	}

	public Dictionary<string, int> FindSrorageCommanderMedalList()
	{
		Dictionary<string, int> list = new Dictionary<string, int>();
		Regulation regulation = RemoteObjectManager.instance.regulation;
		regulation.itemExchangeDtbl.ForEach(delegate(ItemExchangeDataRow item)
		{
			if (item.type == EStorageType.Medal)
			{
				RoCommander roCommander = FindCommander(item.typeidx);
				if (roCommander != null && roCommander.medal > 0)
				{
					list.Add(roCommander.id, roCommander.medal);
				}
			}
		});
		return list;
	}

	public List<RoPart> FindSroragePartList()
	{
		List<RoPart> list = new List<RoPart>();
		Regulation regulation = RemoteObjectManager.instance.regulation;
		regulation.itemExchangeDtbl.ForEach(delegate(ItemExchangeDataRow item)
		{
			if (item.type == EStorageType.Part)
			{
				RoPart roPart = FindPart(item.typeidx);
				if (roPart != null && roPart.count > 0)
				{
					list.Add(roPart);
				}
			}
		});
		return list;
	}

	public RoCommander FindCommanderByUnitId(string unitId)
	{
		return commanderList.Find((RoCommander commander) => commander.unitId == unitId);
	}

	public List<RoCommander> GetAdvancePossibleCommanderList(EJob job)
	{
		List<RoCommander> list = new List<RoCommander>();
		commanderList.ForEach(delegate(RoCommander commander)
		{
			if ((commander.branch == EBranch.Army || commander.branch == EBranch.Undefined) && (commander.job == job || job == EJob.All) && commander.state == ECommanderState.Nomal && commander.isAdvancePossible)
			{
				list.Add(commander);
			}
		});
		return list;
	}

	public int GetAdvancePossibleCommanderCount()
	{
		int count = 0;
		commanderList.ForEach(delegate(RoCommander commander)
		{
			if ((commander.branch == EBranch.Army || commander.branch == EBranch.Undefined) && commander.state == ECommanderState.Nomal && commander.isAdvancePossible)
			{
				count++;
			}
		});
		return count;
	}

	public void ResetAdvancePossible()
	{
		commanderList.ForEach(delegate(RoCommander commander)
		{
			commander.isAdvance = false;
		});
	}

	public void ResetDispatchPossible()
	{
		commanderList.ForEach(delegate(RoCommander commander)
		{
			commander.isDispatch = false;
		});
	}

	public void SetNewCommander(CommanderDataRow row)
	{
		RoCommander roCommander = RoCommander.Create(row.id, 1, row.grade, 1, 0, 0, 0, new List<int>());
		roCommander.SetBaseCostume();
		commanderList.Add(roCommander);
	}

	public void SetCommanderTypeAndIdx()
	{
		for (int i = 0; i < commanderList.Count; i++)
		{
			commanderList[i].charType = ECharacterType.Commander;
			commanderList[i].userIdx = int.Parse(base.uno);
		}
	}

	public RoUnit FindUnit(string id)
	{
		return unitList.Find((RoUnit unit) => unit.id == id);
	}

	public List<RoUnit> FindUnitList(EBranch branch)
	{
		return unitList.FindAll((RoUnit unit) => branch == EBranch.Undefined || unit.unitReg.branch == branch);
	}

	public List<RoUnit> FindUnlockedUnit(EBranch branch)
	{
		return unitList.FindAll((RoUnit unit) => (branch == EBranch.Undefined || unit.unitReg.branch == branch) && unit.unlocked);
	}

	public List<RoUnit> FindUnlockedUnit(EBranch branch, EUnitType type)
	{
		return unitList.FindAll((RoUnit unit) => (branch == EBranch.Undefined || unit.unitReg.branch == branch) && unit.unitReg.type == type && unit.unlocked);
	}

	public RoTroop ReplaceTroop(RoTroop troop)
	{
		for (int i = 0; i < troopList.Count; i++)
		{
			if (troopList[i].id == troop.id)
			{
				troopList[i] = troop;
				return troop;
			}
		}
		return null;
	}

	public RoPart FindPart(string id)
	{
		return partList.Find((RoPart row) => row.id == id);
	}

	public void SetUserPart(string id, int count)
	{
		RoPart roPart = partList.Find((RoPart row) => row.id == id);
		if (roPart != null)
		{
			roPart.count = count;
		}
	}

	public List<RoPart> GetUserPartList()
	{
		return partList.FindAll((RoPart row) => row.count > 0);
	}

	public RoMission FindMission(string id)
	{
		return missionList.Find((RoMission row) => row.idx == id);
	}

	public RoMission FindAchievement(string id, int sort)
	{
		return achievementList.Find((RoMission row) => row.idx == id && row.sort == sort);
	}

	public RoReward FindReward(string id)
	{
		for (int i = 0; i < rewardList.Count; i++)
		{
			if (rewardList[i].id == id)
			{
				return rewardList[i];
			}
		}
		return null;
	}

	public List<RoReward> GetRewardList(EReward type)
	{
		List<RoReward> list = new List<RoReward>();
		rewardList.ForEach(delegate(RoReward evt)
		{
			if (type == evt.type)
			{
				list.Add(evt);
			}
		});
		return list;
	}

	public bool ConfirmReward(RoReward evt)
	{
		if (evt.received)
		{
			return false;
		}
		bool result = false;
		evt.received = true;
		if (evt.type == EReward.Mail)
		{
			if (!string.IsNullOrEmpty(evt.rewardId))
			{
				result = true;
				ApplyRewardValue(evt);
			}
			newMailCount--;
		}
		else if (!string.IsNullOrEmpty(evt.rewardId))
		{
			rewardList.Add(evt.CreateMail());
			newMailCount++;
		}
		if (newMailCount < 0)
		{
		}
		return result;
	}

	public int ReceiveMailCount()
	{
		int num = 0;
		for (int num2 = rewardList.Count - 1; num2 >= 0; num2--)
		{
			if (rewardList[num2].type == EReward.Mail && !string.IsNullOrEmpty(rewardList[num2].rewardId))
			{
				num++;
			}
		}
		return num;
	}

	public TimeData FindRecruitDelayTime(string commanderId)
	{
		for (int i = 0; i < recruit.entryList.Count; i++)
		{
			if (string.Equals(commanderId, recruit.entryList[i].commander.id))
			{
				return recruit.entryList[i].delayTime;
			}
		}
		return null;
	}

	public RoRecruit.Entry FindRecruitEntry(string commanderId)
	{
		for (int i = 0; i < recruit.entryList.Count; i++)
		{
			if (string.Equals(commanderId, recruit.entryList[i].commander.id))
			{
				return recruit.entryList[i];
			}
		}
		return null;
	}

	public void ApplyRewardValue(RoReward evt)
	{
		Type type = GetType();
		PropertyInfo property = type.GetProperty(evt.rewardId);
		Type propertyType = property.PropertyType;
		if (propertyType == typeof(int))
		{
			int num = (int)property.GetValue(this, null);
			property.SetValue(this, num + evt.rewardCount, null);
		}
		else if (propertyType == typeof(long))
		{
			long num2 = (long)property.GetValue(this, null);
			property.SetValue(this, num2 + evt.rewardCount, null);
		}
		else if (propertyType == typeof(float))
		{
			float num3 = (float)property.GetValue(this, null);
			property.SetValue(this, num3 + (float)evt.rewardCount, null);
		}
		else if (propertyType == typeof(double))
		{
			double num4 = (double)property.GetValue(this, null);
			property.SetValue(this, num4 + (double)evt.rewardCount, null);
		}
	}

	public void RefreshDailyBonusFromNetwork(Protocols.DailyBonusCheckResponse recvData)
	{
		dailyBonus.isReceived = recvData.received;
		dailyBonus.version = recvData.version;
		dailyBonus.today = recvData.day;
		dailyBonus.timeLimitString = recvData.endTimeString;
	}

	public void FromNetwork(Protocols.UserInformationResponse recvData)
	{
		if (recvData == null)
		{
			return;
		}
		if (recvData.goodsInfo != null)
		{
			RefreshGoodsFromNetwork(recvData.goodsInfo);
		}
		if (recvData.battleStatisticsInfo != null)
		{
			Utility.CopyToSameNameProperties(recvData.battleStatisticsInfo, statistics);
		}
		RefreshWeaponFromNetworkLogin(recvData.weaponList);
		if (recvData.commanderInfo != null)
		{
			foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> item in recvData.commanderInfo)
			{
				Protocols.UserInformationResponse.Commander value = item.Value;
				AddOrRefreshCommanderFromNetwork(item.Value);
			}
		}
		RefreshMedalFromNetwork(recvData.medalData);
		RefreshItemFromNetwork(recvData.eventResourceData);
		RefreshItemFromNetwork(recvData.itemData);
		RefreshItemFromNetwork(recvData.foodData);
		RefreshPartFromNetwork(recvData.partData);
		RefreshPreDeckFromNetwork(recvData.preDeck);
		RefreshUserEquipItemFromNetwork(recvData.equipItem);
		RefreshGuildFromNetwork(recvData.guildInfo);
		RefreshItemFromNetwork(recvData.groupItemData);
	}

	public void FromNetwork(Protocols.UserInformationResponse.BattleStatistics data)
	{
		if (data != null)
		{
			Utility.CopyToSameNameProperties(data, statistics);
		}
	}

	public void RefreshRewardFromNetwork(Protocols.RewardInfo reward)
	{
		if (reward.resource != null)
		{
			RefreshGoodsFromNetwork(reward.resource);
		}
		if (reward.commander != null)
		{
			foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> item in reward.commander)
			{
				Protocols.UserInformationResponse.Commander value = item.Value;
				RoCommander roCommander = FindCommander(value.id);
				roCommander.state = ECommanderState.Nomal;
				if (item.Value.haveCostume != null && item.Value.haveCostume.Count > 0)
				{
					roCommander.haveCostumeList = item.Value.haveCostume;
				}
			}
		}
		RefreshMedalFromNetwork(reward.medalData);
		RefreshItemFromNetwork(reward.eventResourceData);
		RefreshItemFromNetwork(reward.itemData);
		RefreshItemFromNetwork(reward.foodData);
		RefreshPartFromNetwork(reward.partData);
		RefreshCostumeFromNetwork(reward.costumeData);
		RefreshItemFromNetwork(reward.groupItemData);
		RefreshUserEquipItemFromNetwork(reward.equipItem);
		RefreshWeaponFromNetwork(reward.weaponList);
		RefreshGoodsFromNetwork(reward.dormitoryResource);
		RefreshDormitoryItemNormalFromNetwork(reward.dormitoryItemNormal);
		RefreshDormitoryItemAdvancedFromNetwork(reward.dormitoryItemAdvanced);
		RefreshDormitoryItemWallpaperFromNetwork(reward.dormitoryItemWallpaper);
		RefreshDormitoryCostumeBodyFromNetwork(reward.dormitoryCostumeBody);
		RefreshDormitoryCostumeHeadFromNetwork(reward.dormitoryCostumeHead);
	}

	public void RefreshPartsFromNetwork(List<Protocols.UserInformationResponse.PartData> parts)
	{
		if (parts == null)
		{
			return;
		}
		foreach (Protocols.UserInformationResponse.PartData part in parts)
		{
			SetUserPart(part.idx, part.count);
		}
	}

	public void RefreshPreDeckFromNetwork(List<Protocols.UserInformationResponse.PreDeck> decks)
	{
		if (decks == null)
		{
			return;
		}
		foreach (Protocols.UserInformationResponse.PreDeck deck in decks)
		{
			preDeckList.Add(deck);
		}
	}

	public void RefreshFavorFromNetwork(List<Protocols.FavorUpData.CommanderFavor> commanderList)
	{
		if (commanderList == null)
		{
			return;
		}
		foreach (Protocols.FavorUpData.CommanderFavor commander in commanderList)
		{
		}
	}

	public void RefreshGuildFromNetwork(Protocols.UserInformationResponse.UserGuild guild)
	{
		if (guild == null)
		{
			return;
		}
		guildInfo = guild;
		if (guild.skillDada == null)
		{
			return;
		}
		foreach (Protocols.UserInformationResponse.UserGuild.GuildSkill list in guild.skillDada)
		{
			base.guildSkillList.Find((RoGuildSkill row) => row.idx == list.idx).skillLevel = list.level;
		}
	}

	public void RefreshGoodsFromNetwork(Protocols.UserInformationResponse.Resource source)
	{
		Utility.CopyToSameNameProperties(source, this);
		if (UIManager.instance.world != null)
		{
			UIManager.instance.world.mainCommand.MaxBulletCheck();
		}
	}

	public void RefreshGoodsFromNetwork(Protocols.Dormitory.Resource source)
	{
		if (source != null)
		{
			Utility.CopyToSameNameProperties(source, this);
		}
	}

	public void RefreshDormitoryItemNormalFromNetwork(Dictionary<string, int> source)
	{
		if (source != null)
		{
			dormitory.invenData.UpdateData(EDormitoryItemType.Normal, source);
		}
	}

	public void RefreshDormitoryItemAdvancedFromNetwork(Dictionary<string, int> source)
	{
		if (source != null)
		{
			dormitory.invenData.UpdateData(EDormitoryItemType.Advanced, source);
		}
	}

	public void RefreshDormitoryItemWallpaperFromNetwork(Dictionary<string, int> source)
	{
		if (source != null)
		{
			dormitory.invenData.UpdateData(EDormitoryItemType.Wallpaper, source);
		}
	}

	public void RefreshDormitoryCostumeBodyFromNetwork(Dictionary<string, int> source)
	{
		if (source != null)
		{
			dormitory.invenData.UpdateData(EDormitoryItemType.CostumeBody, source);
		}
	}

	public void RefreshDormitoryCostumeHeadFromNetwork(Dictionary<string, List<string>> source)
	{
		if (source != null)
		{
			dormitory.invenData.UpdateHead(source);
		}
	}

	public void RefreshGachaFromNetwork(Protocols.GachaInformationResponse source)
	{
		if (source != null)
		{
			RoGacha roGacha = null;
			roGacha = ((!gacha.ContainsKey(source.type)) ? RoGacha.Create() : gacha[source.type]);
			roGacha.type = source.type;
			roGacha.remainCount = source.freeOpenRemainCount;
			roGacha.freeOpenTime.SetByDuration(source.freeOpenRemainTime);
			roGacha.pilotRate = source.pilotRate;
			if (!gacha.ContainsKey(source.type))
			{
				gacha.Add(source.type, roGacha);
			}
		}
	}

	public void RefreshGachaProbabilityTypeAFromNetwork(string type, Dictionary<ERewardType, Protocols.GachaRatingDataTypeA> source)
	{
		if (source != null)
		{
			RoGacha roGacha = null;
			roGacha = ((!gacha.ContainsKey(type)) ? RoGacha.Create() : gacha[type]);
			roGacha.gachaRatingTypeA = source;
		}
	}

	public void RefreshGachaProbabilityTypeBFromNetwork(string type, Dictionary<ERewardType, Protocols.GachaRatingDataTypeB> source)
	{
		if (source != null)
		{
			RoGacha roGacha = null;
			roGacha = ((!gacha.ContainsKey(type)) ? RoGacha.Create() : gacha[type]);
			roGacha.gachaRatingTypeB = source;
		}
	}

	public void RefreshMedalFromNetwork(Dictionary<string, int> source)
	{
		if (source == null)
		{
			return;
		}
		foreach (KeyValuePair<string, int> item in source)
		{
			RoCommander roCommander = FindCommander(item.Key);
			if (roCommander == null)
			{
			}
			roCommander.aMedal = item.Value;
		}
	}

	public void RefreshWeaponFromNetworkLogin(Dictionary<string, Protocols.WeaponData> source)
	{
		if (source == null)
		{
			return;
		}
		foreach (KeyValuePair<string, Protocols.WeaponData> item in source)
		{
			RoWeapon roWeapon = FindWeapon(item.Key);
			if (roWeapon == null)
			{
				roWeapon = RoWeapon.Create(item.Key, item.Value.id, item.Value.level, item.Value.cid);
				weaponList.Add(roWeapon);
				continue;
			}
			if (string.IsNullOrEmpty(item.Value.id))
			{
				roWeapon.wIdx = item.Value.id;
			}
			if (item.Value.level != 0)
			{
				roWeapon.level = item.Value.level;
			}
			if (string.IsNullOrEmpty(item.Value.cid.ToString()))
			{
				roWeapon.currEquipCommanderId = item.Value.cid;
				RoCommander roCommander = FindCommander(item.Value.cid.ToString());
				roCommander.EquipWeaponItem(roWeapon);
			}
		}
	}

	public void RefreshWeaponFromNetwork(Dictionary<string, Protocols.WeaponData> source)
	{
		if (source == null)
		{
			return;
		}
		foreach (KeyValuePair<string, Protocols.WeaponData> item in source)
		{
			RoWeapon roWeapon = FindWeapon(item.Key);
			if (roWeapon == null)
			{
				roWeapon = RoWeapon.Create(item.Key, item.Value.id, item.Value.level, item.Value.cid);
				weaponList.Add(roWeapon);
				if (!newWeaponList.Contains(roWeapon.idx))
				{
					newWeaponList.Add(roWeapon.idx);
				}
				continue;
			}
			if (string.IsNullOrEmpty(item.Value.id))
			{
				roWeapon.wIdx = item.Value.id;
			}
			if (item.Value.level != 0)
			{
				roWeapon.level = item.Value.level;
			}
			if (string.IsNullOrEmpty(item.Value.cid.ToString()))
			{
				roWeapon.currEquipCommanderId = item.Value.cid;
				RoCommander roCommander = FindCommander(item.Value.cid.ToString());
				roCommander.EquipWeaponItem(roWeapon);
			}
		}
	}

	public void RefreshItemFromNetwork(Dictionary<string, int> source)
	{
		if (source == null)
		{
			return;
		}
		Regulation regulation = RemoteObjectManager.instance.regulation;
		foreach (KeyValuePair<string, int> item in source)
		{
			GoodsDataRow goodsDataRow = regulation.FindGoodsServerFieldName(int.Parse(item.Key));
			resourceList[goodsDataRow.serverFieldName] = item.Value;
		}
	}

	public void RefreshPartFromNetwork(Dictionary<string, int> source)
	{
		if (source == null)
		{
			return;
		}
		foreach (KeyValuePair<string, int> item in source)
		{
			SetUserPart(item.Key, item.Value);
		}
	}

	public void RefreshCostumeFromNetwork(List<Dictionary<string, Protocols.RewardInfo.HaveCostumeInfo>> source)
	{
		if (source == null)
		{
			return;
		}
		for (int i = 0; i < source.Count; i++)
		{
			foreach (KeyValuePair<string, Protocols.RewardInfo.HaveCostumeInfo> item in source[i])
			{
				string key = item.Key;
				RoCommander roCommander = FindCommander(key);
				for (int j = 0; j < item.Value.haveCostume.Count; j++)
				{
					int num = item.Value.haveCostume[j];
					if (!roCommander.haveCostumeList.Contains(num))
					{
						roCommander.haveCostumeList.Add(num);
						if (roCommander.state == ECommanderState.Undefined)
						{
							AddDonHaveCommCostume(key, num);
						}
					}
				}
			}
		}
	}

	public void RefreshUserEquipItemFromNetwork(Dictionary<string, Dictionary<int, Protocols.EquipItemInfo>> itemList)
	{
		if (itemList == null)
		{
			return;
		}
		string itemIdx = string.Empty;
		int itemLevel = 0;
		string empty = string.Empty;
		foreach (KeyValuePair<string, Dictionary<int, Protocols.EquipItemInfo>> item in itemList)
		{
			itemIdx = item.Key;
			foreach (KeyValuePair<int, Protocols.EquipItemInfo> item2 in item.Value)
			{
				itemLevel = item2.Key;
				if (item2.Value.equipCommanderList != null)
				{
					for (int i = 0; i < item2.Value.equipCommanderList.Count; i++)
					{
						string commanderId = item2.Value.equipCommanderList[i].ToString();
						RoItem roItem = EquipedList_FindItem(itemIdx, commanderId, item2.Key);
						if (roItem == null)
						{
							EquipedItemList.Add(RoItem.Create(itemIdx, itemLevel, 1, commanderId));
						}
						RoItem roItem2 = EquipedList_FindItem(itemIdx, commanderId, item2.Key);
						RoCommander roCommander = FindCommander(commanderId);
						if (roItem2 != null)
						{
							roCommander?.SetEquipItem(roItem2.pointType, roItem2);
						}
					}
				}
				RoItem roItem3 = EquipPossibleItemList.Find((RoItem row) => row.id == itemIdx && row.level == itemLevel);
				if (roItem3 == null && item2.Value.availableCount > 0)
				{
					EquipPossibleItemList.Add(RoItem.Create(itemIdx, itemLevel, item2.Value.availableCount, null));
				}
				else
				{
					SetEquipPossibleItemCount(itemIdx, itemLevel, item2.Value.availableCount);
				}
			}
		}
	}

	public void RefreshDefenderTroop(List<Dictionary<string, string>> source)
	{
		defenderTroops = new List<RoTroop>();
		Regulation regulation = RemoteObjectManager.instance.regulation;
		if (source == null || source.Count == 0)
		{
			RoTroop roTroop = RoTroop.Create(base.id);
			roTroop.ResetSlots();
			defenderTroops.Add(roTroop);
			return;
		}
		for (int i = 0; i < source.Count; i++)
		{
			RoTroop roTroop2 = RoTroop.Create(base.id);
			roTroop2.ResetSlots();
			defenderTroops.Add(roTroop2);
			Dictionary<string, string> dictionary = source[i];
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				RoTroop.Slot nextEmptySlot = roTroop2.GetNextEmptySlot();
				RoCommander roCommander = FindCommander(item.Value);
				nextEmptySlot.unitId = roCommander.unitId;
				nextEmptySlot.position = int.Parse(item.Key) - 1;
				nextEmptySlot.commanderId = roCommander.id;
				nextEmptySlot.unitRank = roCommander.rank;
				nextEmptySlot.unitLevel = roCommander.level;
				nextEmptySlot.unitCls = roCommander.cls;
				nextEmptySlot.unitCostume = roCommander.currentCostume;
				nextEmptySlot.favorRewardStep = roCommander.favorRewardStep;
				nextEmptySlot.marry = roCommander.marry;
				nextEmptySlot.transcendence = roCommander.transcendence;
				nextEmptySlot.equipItem = roCommander.GetEquipItemList();
				nextEmptySlot.charType = roCommander.charType;
				nextEmptySlot.weaponItem = roCommander.WeaponItem;
				for (int j = 0; j < roCommander.unitReg.skillDrks.Count; j++)
				{
					Troop.Slot.Skill skill = new Troop.Slot.Skill();
					SkillDataRow skillDataRow = regulation.skillDtbl[roCommander.unitReg.skillDrks[j]];
					skill.id = roCommander.unitReg.skillDrks[j];
					skill.lv = roCommander.skillList[j];
					nextEmptySlot.skills.Add(skill);
				}
			}
		}
	}

	public void RefreshDefenderTroop(Dictionary<string, string> source)
	{
		defenderTroops = new List<RoTroop>();
		Regulation regulation = RemoteObjectManager.instance.regulation;
		if (source == null || source.Count == 0)
		{
			RoTroop roTroop = RoTroop.Create(base.id);
			roTroop.ResetSlots();
			defenderTroops.Add(roTroop);
			return;
		}
		RoTroop roTroop2 = RoTroop.Create(base.id);
		roTroop2.ResetSlots();
		defenderTroops.Add(roTroop2);
		foreach (KeyValuePair<string, string> item in source)
		{
			RoTroop.Slot nextEmptySlot = roTroop2.GetNextEmptySlot();
			RoCommander roCommander = FindCommander(item.Value);
			nextEmptySlot.unitId = roCommander.unitId;
			nextEmptySlot.position = int.Parse(item.Key) - 1;
			nextEmptySlot.commanderId = roCommander.id;
			nextEmptySlot.unitRank = roCommander.rank;
			nextEmptySlot.unitLevel = roCommander.level;
			nextEmptySlot.unitCls = roCommander.cls;
			nextEmptySlot.unitCostume = roCommander.currentCostume;
			nextEmptySlot.favorRewardStep = roCommander.favorRewardStep;
			nextEmptySlot.marry = roCommander.marry;
			nextEmptySlot.transcendence = roCommander.transcendence;
			nextEmptySlot.equipItem = roCommander.GetEquipItemList();
			nextEmptySlot.charType = roCommander.charType;
			nextEmptySlot.weaponItem = roCommander.WeaponItem;
			for (int i = 0; i < roCommander.unitReg.skillDrks.Count; i++)
			{
				Troop.Slot.Skill skill = new Troop.Slot.Skill();
				SkillDataRow skillDataRow = regulation.skillDtbl[roCommander.unitReg.skillDrks[i]];
				skill.id = roCommander.unitReg.skillDrks[i];
				skill.lv = roCommander.skillList[i];
				nextEmptySlot.skills.Add(skill);
			}
		}
	}

	public void RefreshConquestTroop(int slot, Dictionary<string, string> source)
	{
		if (source == null || source.Count == 0)
		{
			return;
		}
		if (conquestDeck[slot] == null)
		{
			Protocols.ConquestTroopInfo.Troop troop = new Protocols.ConquestTroopInfo.Troop();
			troop.point = 0;
			troop.remain = 0;
			troop.status = "S";
			troop.deck = new Dictionary<string, string>();
			troop.path = new List<int>();
			conquestDeck[slot] = troop;
		}
		conquestDeck[slot].deck.Clear();
		foreach (KeyValuePair<string, string> item in source)
		{
			conquestDeck[slot].deck.Add(item.Key, item.Value);
		}
	}

	public void AddOrRefreshCommanderFromNetwork(Protocols.UserInformationResponse.Commander source)
	{
		RoCommander roCommander = FindCommander(source.id);
		if (roCommander == null)
		{
			return;
		}
		if (source.level != 0)
		{
			roCommander.level = source.level;
		}
		if (source.rank != 0)
		{
			roCommander.rank = source.rank;
		}
		if (source.cls != 0)
		{
			roCommander.cls = source.cls;
		}
		roCommander.role = source.role;
		roCommander.state = ((source.state == "N") ? ECommanderState.Nomal : ECommanderState.Getting);
		if (source.skv1 != 0)
		{
			roCommander.skillList[0] = source.skv1;
		}
		if (source.skv2 != 0)
		{
			roCommander.skillList[1] = source.skv2;
		}
		if (source.skv3 != 0)
		{
			roCommander.skillList[2] = source.skv3;
		}
		if (source.skv4 != 0)
		{
			roCommander.skillList[3] = source.skv4;
		}
		if (source.exp != 0)
		{
			roCommander.aExp = source.exp;
		}
		roCommander.currentCostume = source.currentCostume;
		roCommander.haveCostumeList = source.haveCostume;
		roCommander.eventCostume = source.eventCostume;
		roCommander.favorStep = source.favorStep;
		roCommander.favorPoint = source.favorPoint;
		roCommander.favorRewardStep = source.favorRewardStep;
		roCommander.marry = source.marry;
		roCommander.transcendence = source.transcendence;
		roCommander.FavorCount = source.favr;
		roCommander.FavorStep = source.fvrd;
		roCommander.marry = source.marry;
		if (source.equipWeaponInfo.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<string, Protocols.WeaponData> item in source.equipWeaponInfo)
		{
			RoWeapon weapon = FindWeapon(item.Key);
			roCommander.EquipWeaponItem(weapon);
		}
	}

	public void AddCommanderFromNetwork(Dictionary<string, Protocols.UserInformationResponse.Commander> list)
	{
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (Protocols.UserInformationResponse.Commander value in list.Values)
		{
			RoCommander roCommander = FindCommander(value.id);
			if (roCommander != null)
			{
				roCommander.state = ECommanderState.Nomal;
			}
		}
	}

	public void RefreshCarnivalFromNetwork(Dictionary<string, Dictionary<string, Protocols.CarnivalList.ProcessData>> list)
	{
		foreach (KeyValuePair<string, Dictionary<string, Protocols.CarnivalList.ProcessData>> item in list)
		{
			if (carnivalList.carnivalProcessList.ContainsKey(item.Key))
			{
				foreach (KeyValuePair<string, Protocols.CarnivalList.ProcessData> item2 in list[item.Key])
				{
					if (carnivalList.carnivalProcessList[item.Key].ContainsKey(item2.Key))
					{
						carnivalList.carnivalProcessList[item.Key][item2.Key].count = list[item.Key][item2.Key].count;
						carnivalList.carnivalProcessList[item.Key][item2.Key].receive = list[item.Key][item2.Key].receive;
						carnivalList.carnivalProcessList[item.Key][item2.Key].able = list[item.Key][item2.Key].able;
						switch (RemoteObjectManager.instance.regulation.FindCarnivalType(item.Key))
						{
						case ECarnivalType.NewUserExchangeEvent_Reward:
						case ECarnivalType.NewUserExchangeEvent_Mission:
						case ECarnivalType.ExchangeEvent_Reward:
						case ECarnivalType.ExchangeEvent_Mission:
						case ECarnivalType.EventBattle_Exchange:
							if (list[item.Key][item2.Key].receive == 1)
							{
								carnivalList.carnivalProcessList[item.Key][item2.Key].complete = 1;
							}
							else
							{
								carnivalList.carnivalProcessList[item.Key][item2.Key].complete = (IsCompleteExchangeCarnival(item2.Key) ? 1 : 0);
							}
							break;
						default:
							carnivalList.carnivalProcessList[item.Key][item2.Key].complete = list[item.Key][item2.Key].complete;
							break;
						case ECarnivalType.SelectReward:
						case ECarnivalType.BulletReward:
							carnivalList.carnivalProcessList[item.Key][item2.Key] = list[item.Key][item2.Key];
							break;
						}
					}
					else
					{
						carnivalList.carnivalProcessList[item.Key].Add(item2.Key, item2.Value);
					}
				}
			}
			else
			{
				carnivalList.carnivalProcessList.Add(item.Key, item.Value);
			}
		}
	}

	public void RefreshWorldDuelActiveBuffFromNetwork(List<int> source)
	{
		if (source != null && source.Count != 0)
		{
			for (int i = 0; i < source.Count; i++)
			{
				EWorldDuelBuff key = (EWorldDuelBuff)(i + 1);
				EWorldDuelBuffEffect eWorldDuelBuffEffect = EWorldDuelBuffEffect.b;
				eWorldDuelBuffEffect = ((source[i] == 0) ? EWorldDuelBuffEffect.b : EWorldDuelBuffEffect.d);
				base.activeBuff[key] = eWorldDuelBuffEffect;
			}
		}
	}

	public void RefreshWorldDuelBuffFromNetwork(Dictionary<string, int> source)
	{
		if (source != null && source.Count != 0)
		{
			string[] array = new string[source.Count];
			source.Keys.CopyTo(array, 0);
			foreach (string key in array)
			{
				base.worldDuelBuff[key] = source[key];
			}
		}
	}

	public void BulletCharge()
	{
		UserLevelDataRow userLevelDataRow = RemoteObjectManager.instance.regulation.GetUserLevelDataRow(base.level);
		GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.goodsDtbl[5.ToString()];
		bullet++;
		if (userLevelDataRow.maxBullet <= bullet)
		{
			bulletRemain = 0;
		}
		else
		{
			bulletRemain = goodsDataRow.rechargeTime;
		}
		tempBullet = bullet;
		UIManager.instance.world.mainCommand.BulletControl();
	}

	public void OilCharge()
	{
		GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.goodsDtbl[24.ToString()];
		oil++;
		if (goodsDataRow.rechargeMax <= oil)
		{
			oilRemain = 0;
		}
		else
		{
			oilRemain = goodsDataRow.rechargeTime;
		}
		UIManager.instance.world.mainCommand.OilControl();
	}

	public void WeaponMaterialCharge1()
	{
		GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.goodsDtbl[3001.ToString()];
		weaponMaterial1++;
		if (goodsDataRow.rechargeMax <= weaponMaterial1)
		{
			weaponMaterialRemainTime1.SetByDuration(0.0);
		}
		else
		{
			weaponMaterialRemainTime1.SetByDuration(goodsDataRow.rechargeTime);
		}
		UIManager.instance.world.weaponResearch.inProgress.WeaponMaterialControl1();
	}

	public void WeaponMaterialCharge2()
	{
		GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.goodsDtbl[3002.ToString()];
		weaponMaterial2++;
		if (goodsDataRow.rechargeMax <= weaponMaterial2)
		{
			weaponMaterialRemainTime2.SetByDuration(0.0);
		}
		else
		{
			weaponMaterialRemainTime2.SetByDuration(goodsDataRow.rechargeTime);
		}
		UIManager.instance.world.weaponResearch.inProgress.WeaponMaterialControl2();
	}

	public void WeaponMaterialCharge3()
	{
		GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.goodsDtbl[3003.ToString()];
		weaponMaterial3++;
		if (goodsDataRow.rechargeMax <= weaponMaterial3)
		{
			weaponMaterialRemainTime3.SetByDuration(0.0);
		}
		else
		{
			weaponMaterialRemainTime3.SetByDuration(goodsDataRow.rechargeTime);
		}
		UIManager.instance.world.weaponResearch.inProgress.WeaponMaterialControl3();
	}

	public void WeaponMaterialCharge4()
	{
		GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.goodsDtbl[3004.ToString()];
		weaponMaterial4++;
		if (goodsDataRow.rechargeMax <= weaponMaterial4)
		{
			weaponMaterialRemainTime4.SetByDuration(0.0);
		}
		else
		{
			weaponMaterialRemainTime4.SetByDuration(goodsDataRow.rechargeTime);
		}
		UIManager.instance.world.weaponResearch.inProgress.WeaponMaterialControl4();
	}

	public bool IsBadgeAcademy()
	{
		for (int i = 0; i < recruit.entryList.Count; i++)
		{
			if (recruit.entryList[i].honor <= honor && recruit.entryList[i].gold <= gold)
			{
				return true;
			}
		}
		for (int j = 0; j < commanderList.Count; j++)
		{
			int num = 0;
			if (commanderList[j].state == ECommanderState.Nomal)
			{
				num = commanderList[j].rank;
			}
			CommanderRankDataRow commanderRankDataRow = RemoteObjectManager.instance.regulation.FindCommanderRankData(num + 1);
			int num2 = commanderList[j].medal + medal;
			if (commanderRankDataRow != null && commanderRankDataRow.medal <= num2 && commanderRankDataRow.gold <= gold)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsBadgeHeadQuarter()
	{
		bool result = false;
		for (int i = 0; i < commanderList.Count; i++)
		{
			CommanderRankDataRow commanderRankDataRow = RemoteObjectManager.instance.regulation.FindCommanderRankData((int)commanderList[i].rank + 1);
			if (commanderRankDataRow != null && commanderList[i].medal >= commanderList[i].maxMedal)
			{
				result = true;
			}
		}
		if (PossibleClassUpUnitCount(isGoldCheck: false) > 0)
		{
			result = true;
		}
		return result;
	}

	public bool IsExistGuild()
	{
		return guildInfo != null;
	}

	public int PossibleClassUpUnitCount(bool isGoldCheck = true)
	{
		int num = 0;
		List<RoCommander> list = GetCommanderList(EJob.All, have: true);
		for (int i = 0; i < list.Count; i++)
		{
			RoCommander roCommander = list[i];
			if (roCommander != null && roCommander.IsClassUp(isGoldCheck))
			{
				num++;
			}
		}
		return num;
	}

	public bool UserLevelUpCheck()
	{
		if (base.beforeLevel > 0 && base.level != base.beforeLevel)
		{
			if (UIManager.instance.world != null)
			{
				UIManager.instance.world.levelUp.InitData(base.beforeLevel, base.level);
			}
			return true;
		}
		return false;
	}

	public ERewardState GetRewardState(int point)
	{
		if (rewardDuelPoint == 0)
		{
			return ERewardState.Complete;
		}
		if (duelPoint < point)
		{
			return ERewardState.Nothing;
		}
		if (rewardDuelPoint == point)
		{
			return ERewardState.Receptible;
		}
		if (rewardDuelPoint < point)
		{
			return ERewardState.NonReceptible;
		}
		if (rewardDuelPoint > point || rewardDuelPoint == 0)
		{
			return ERewardState.Complete;
		}
		return ERewardState.Nothing;
	}

	public ERewardState GetRaidRewardState(int rIdx)
	{
		ERewardState result = ERewardState.Nothing;
		List<RankingDataRow> list = RemoteObjectManager.instance.regulation.rankingDtbl.FindAll((RankingDataRow row) => row.type == ERankingContentsType.RaidScore);
		if (list.Count == 0)
		{
			return ERewardState.Nothing;
		}
		int index = 0;
		if (raidRewardPoint != 0)
		{
			index = list.FindIndex((RankingDataRow x) => x.r_idx == raidRewardPoint) + 1;
		}
		if (raidRank >= rIdx)
		{
			if (raidRewardPoint < rIdx)
			{
				RankingDataRow rankingDataRow = list[index];
				result = ((rIdx != rankingDataRow.r_idx) ? ERewardState.NonReceptible : ERewardState.Receptible);
			}
			else
			{
				result = ERewardState.Complete;
			}
		}
		return result;
	}

	public ERewardState GetDuelRewardState(int rIdx)
	{
		ERewardState result = ERewardState.Nothing;
		List<RankingDataRow> list = RemoteObjectManager.instance.regulation.rankingDtbl.FindAll((RankingDataRow row) => row.type == ERankingContentsType.Challenge);
		if (list.Count == 0)
		{
			return ERewardState.Nothing;
		}
		int index = list.Count - 1;
		if (winRankIdx != 0)
		{
			index = list.FindIndex((RankingDataRow x) => x.r_idx == winRankIdx) - 1;
		}
		if (winRank != 0 && winRank <= rIdx)
		{
			if (winRankIdx == 0 || winRankIdx > rIdx)
			{
				RankingDataRow rankingDataRow = list[index];
				result = ((rIdx != rankingDataRow.r_idx) ? ERewardState.NonReceptible : ERewardState.Receptible);
			}
			else
			{
				result = ERewardState.Complete;
			}
		}
		return result;
	}

	public ERewardState GetWorldDuelRewardState(int rIdx, ERankingContentsType type)
	{
		ERewardState result = ERewardState.Nothing;
		List<RankingDataRow> list = RemoteObjectManager.instance.regulation.rankingDtbl.FindAll((RankingDataRow row) => row.type == type);
		if (list.Count == 0)
		{
			return ERewardState.Nothing;
		}
		int index = list.Count - 1;
		if (worldWinRankIdx != 0)
		{
			index = list.FindIndex((RankingDataRow x) => x.r_idx == worldWinRankIdx) - 1;
		}
		if (worldWinRank != 0 && worldWinRank <= rIdx)
		{
			if (worldWinRankIdx == 0 || worldWinRankIdx > rIdx)
			{
				RankingDataRow rankingDataRow = list[index];
				result = ((rIdx != rankingDataRow.r_idx) ? ERewardState.NonReceptible : ERewardState.Receptible);
			}
			else
			{
				result = ERewardState.Complete;
			}
		}
		return result;
	}

	public bool GetItemCheckList(List<RewardDataRow> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			RewardDataRow rewardDataRow = list[i];
			if (!GetItemCheck(rewardDataRow.rewardType, rewardDataRow.rewardIdx.ToString(), rewardDataRow.maxCount))
			{
				return false;
			}
		}
		return true;
	}

	public bool BuyItemCheck(Protocols.SecretShop.ShopData data)
	{
		return GetItemCheck(data.type, data.idx.ToString(), data.count);
	}

	public bool GetMailItemCheckList(List<RoReward> rewardList)
	{
		for (int i = 0; i < rewardList.Count; i++)
		{
			RoReward roReward = rewardList[i];
			if (roReward.rewardItem == null)
			{
				continue;
			}
			for (int j = 0; j < roReward.rewardItem.Count; j++)
			{
				Protocols.RewardInfo.RewardData rewardData = roReward.rewardItem[j];
				if (!GetItemCheck(rewardData.rewardType, rewardData.rewardId, rewardData.rewardCnt))
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool GetMailItemCheck(RoReward reward)
	{
		for (int i = 0; i < reward.rewardItem.Count; i++)
		{
			Protocols.RewardInfo.RewardData rewardData = reward.rewardItem[i];
			if (!GetItemCheck(rewardData.rewardType, rewardData.rewardId, rewardData.rewardCnt))
			{
				return false;
			}
		}
		return true;
	}

	public bool GetItemMaxCheck(List<RoReward> rewardList, EGoods _goodsType)
	{
		List<RoReward> list = new List<RoReward>();
		int num = 0;
		for (int i = 0; i < rewardList.Count; i++)
		{
			for (int j = 0; j < rewardList[i].rewardItem.Count; j++)
			{
				GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.FindGoodsServerFieldName(rewardList[i].rewardItem[j].rewardId);
				if (goodsDataRow != null)
				{
					string type = goodsDataRow.type;
					int num2 = (int)_goodsType;
					if (type == num2.ToString())
					{
						num += rewardList[i].rewardItem[j].rewardCnt;
					}
				}
			}
		}
		num += challenge;
		int rechargeMax = RemoteObjectManager.instance.regulation.goodsDtbl.Find(delegate(GoodsDataRow data)
		{
			string type2 = data.type;
			int num3 = (int)_goodsType;
			return type2 == num3.ToString();
		}).rechargeMax;
		if (num > rechargeMax)
		{
			return false;
		}
		return true;
	}

	public bool GetItemMaxCheck(RoReward reward, EGoods _goodsType)
	{
		List<Protocols.RewardInfo.RewardData> list = new List<Protocols.RewardInfo.RewardData>();
		int num = 0;
		for (int i = 0; i < reward.rewardItem.Count; i++)
		{
			GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.FindGoodsServerFieldName(reward.rewardItem[i].rewardId);
			if (goodsDataRow != null)
			{
				string type = goodsDataRow.type;
				int num2 = (int)_goodsType;
				if (type == num2.ToString())
				{
					num += reward.rewardItem[i].rewardCnt;
				}
			}
		}
		num += challenge;
		int rechargeMax = RemoteObjectManager.instance.regulation.goodsDtbl.Find(delegate(GoodsDataRow data)
		{
			string type2 = data.type;
			int num3 = (int)_goodsType;
			return type2 == num3.ToString();
		}).rechargeMax;
		if (num > rechargeMax)
		{
			return false;
		}
		return true;
	}

	public bool GetMissionItemCheck(RoMission misson)
	{
		RewardDataRow rewardDataRow = RemoteObjectManager.instance.regulation.rewardDtbl.Find((RewardDataRow item) => item.category == (ERewardCategory)((misson.sort <= 0) ? 4 : 3) && item.type == int.Parse(misson.idx) && item.typeIndex == misson.sort);
		if (!GetItemCheck(rewardDataRow.rewardType, rewardDataRow.rewardIdx.ToString(), rewardDataRow.minCount))
		{
			return false;
		}
		return true;
	}

	public bool GetItemCheck(ERewardType type, string id, int count)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		switch (type)
		{
		case ERewardType.Goods:
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[id];
			if (goodsDataRow.max > 0)
			{
				int num = resourceList[goodsDataRow.serverFieldName];
				if (goodsDataRow.max < num + count)
				{
					return false;
				}
			}
			break;
		}
		case ERewardType.UnitMaterial:
		{
			PartDataRow partDataRow = regulation.partDtbl[id];
			if (partDataRow.max > 0)
			{
				int count2 = FindPart(id).count;
				if (partDataRow.max < count2 + count)
				{
					return false;
				}
			}
			break;
		}
		}
		return true;
	}

	public void AddNotice(int idx, string notice)
	{
		if (!noticeList.ContainsKey(idx))
		{
			noticeList.Add(idx, notice);
		}
		else
		{
			noticeList[idx] = notice;
		}
		if (UIManager.instance.world != null)
		{
			UIManager.instance.world.mainCommand.callMessage.SetNotice();
		}
	}

	public List<Protocols.NoticeData> FindNoticeList(ENoticeType type)
	{
		return eventNoticeList.FindAll((Protocols.NoticeData data) => (type == ENoticeType.WebView) ? (data.img == null) : (data.img != null));
	}

	public bool badgeCarnival(ECarnivalCategory category)
	{
		bool flag = false;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		if (carnivalList != null && carnivalList.carnivalList.Count > 0)
		{
			string[] array = new string[carnivalList.carnivalList.Count];
			carnivalList.carnivalList.Keys.CopyTo(array, 0);
			if (!regulation.carnivalTypeDtbl.ContainsKey(array[0]))
			{
				NetworkAnimation.Instance.CreateFloatingText("Carnival not exist key : " + array[0]);
				return false;
			}
			CarnivalTypeDataRow carnivalTypeDataRow = regulation.carnivalTypeDtbl[array[0]];
			if (category == carnivalTypeDataRow.categoryType)
			{
				foreach (string key in carnivalList.carnivalList.Keys)
				{
					if (isNewCarnival(key))
					{
						return true;
					}
				}
				foreach (string key2 in carnivalList.carnivalList.Keys)
				{
					if (!carnivalList.carnivalProcessList.ContainsKey(key2))
					{
						continue;
					}
					Dictionary<string, Protocols.CarnivalList.ProcessData> dictionary = carnivalList.carnivalProcessList[key2];
					foreach (KeyValuePair<string, Protocols.CarnivalList.ProcessData> item in dictionary)
					{
						if (item.Value.complete == 1 && item.Value.receive == 0)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		return isNewCarnivalCheck(category);
	}

	private bool isNewCarnivalCheck(ECarnivalCategory category)
	{
		for (int i = 0; i < badgeCarnivalTabList[(int)category].Count; i++)
		{
			if (isNewCarnival(badgeCarnivalTabList[(int)category][i]))
			{
				return true;
			}
		}
		return false;
	}

	public bool isNewCarnival(string idx)
	{
		bool result = true;
		string @string = PlayerPrefs.GetString("Carnival");
		string[] array = @string.Split('|');
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == idx)
			{
				result = false;
			}
		}
		return result;
	}

	public bool isExistCarnivalComplete(string idx)
	{
		bool result = false;
		if (carnivalList.carnivalProcessList.ContainsKey(idx))
		{
			foreach (KeyValuePair<string, Protocols.CarnivalList.ProcessData> item in carnivalList.carnivalProcessList[idx])
			{
				CarnivalTypeDataRow carnivalTypeDataRow = RemoteObjectManager.instance.regulation.carnivalTypeDtbl[idx];
				if (carnivalTypeDataRow.Type == ECarnivalType.SelectReward)
				{
					if ((item.Value.count != 0 && item.Value.startTimeData.GetRemain() <= 0.0) || (item.Value.count == 0 && item.Value.startTimeData.GetRemain() <= 0.0))
					{
						result = true;
					}
				}
				else if (item.Value.receive == 0 && item.Value.complete == 1)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public void SetCarnivalIdx(string idx)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		string @string = PlayerPrefs.GetString("Carnival");
		if (!ContainsCarnivalIdx(idx))
		{
			@string = $"{@string}|{idx}";
			PlayerPrefs.SetString("Carnival", @string);
		}
		CarnivalTypeDataRow carnivalTypeDataRow = regulation.carnivalTypeDtbl[idx];
		badgeCarnivalComplete[(int)carnivalTypeDataRow.categoryType] = badgeCarnival(carnivalTypeDataRow.categoryType);
		UIManager.instance.world.mainCommand.BadgeControl();
	}

	public string[] GetCarnivalIdxList()
	{
		string @string = PlayerPrefs.GetString("Carnival");
		return @string.Split('|');
	}

	public bool ContainsCarnivalIdx(string idx)
	{
		bool result = false;
		string[] carnivalIdxList = GetCarnivalIdxList();
		for (int i = 0; i < carnivalIdxList.Length; i++)
		{
			if (!string.IsNullOrEmpty(carnivalIdxList[i]) && carnivalIdxList[i] == idx)
			{
				result = true;
			}
		}
		return result;
	}

	public void SetMercenaryList(Protocols.GuildDispatchCommanderList _mercenaryList)
	{
		allMercynaryList.Clear();
		Dictionary<string, int> npcList = _mercenaryList.npcList;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		foreach (KeyValuePair<string, int> item in npcList)
		{
			MercynaryUserList mercynaryUserList = new MercynaryUserList();
			mercynaryUserList.isNpc = true;
			mercynaryUserList.npcId = item.Key;
			mercynaryUserList.isEngagePossible = ((item.Value != 0) ? true : false);
			allMercynaryList.Add(mercynaryUserList);
		}
		List<Protocols.GuildDispatchCommanderList.GuildDispatchCommanderInfo> commanderList = _mercenaryList.commanderList;
		if (commanderList != null)
		{
			for (int i = 0; i < commanderList.Count; i++)
			{
				MercynaryUserList mercynaryUserList2 = allMercynaryList.Find((MercynaryUserList row) => commanderList[i].userIdx == row.userUno);
				if (mercynaryUserList2 == null)
				{
					MercynaryUserList mercynaryUserList3 = new MercynaryUserList();
					mercynaryUserList3.isNpc = false;
					mercynaryUserList3.isEngagePossible = ((commanderList[i].possibleEngage != 0) ? true : false);
					mercynaryUserList3.userUno = commanderList[i].userIdx;
					mercynaryUserList3.userLevel = commanderList[i].userLevel;
					mercynaryUserList3.commanderList.Add(commanderList[i]);
					allMercynaryList.Add(mercynaryUserList3);
				}
				else
				{
					mercynaryUserList2.commanderList.Add(commanderList[i]);
				}
			}
		}
		SortingMercenaryList();
		SetMercenarcyCommander();
	}

	public void SortingMercenaryList()
	{
		List<MercynaryUserList> list = allMercynaryList.FindAll((MercynaryUserList row) => row.isEngagePossible);
		List<MercynaryUserList> list2 = allMercynaryList.FindAll((MercynaryUserList row) => !row.isEngagePossible);
		list.Sort(delegate(MercynaryUserList sort_1, MercynaryUserList sort_2)
		{
			if (sort_1.isNpc)
			{
				return -1;
			}
			if (!sort_2.isNpc)
			{
				if (sort_1.userLevel > sort_2.userLevel)
				{
					return -1;
				}
				if (sort_1.userLevel < sort_2.userLevel)
				{
					return 1;
				}
				if (sort_1.userUno > sort_2.userUno)
				{
					return -1;
				}
				return 1;
			}
			return 0;
		});
		list2.Sort(delegate(MercynaryUserList sort_1, MercynaryUserList sort_2)
		{
			if (sort_1.isNpc)
			{
				return -1;
			}
			if (!sort_2.isNpc)
			{
				if (sort_1.userLevel > sort_2.userLevel)
				{
					return -1;
				}
				if (sort_1.userLevel < sort_2.userLevel)
				{
					return 1;
				}
				if (sort_1.userUno > sort_2.userUno)
				{
					return -1;
				}
				return 1;
			}
			return 0;
		});
		allMercynaryList.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			allMercynaryList.Add(list[i]);
		}
		for (int j = 0; j < list2.Count; j++)
		{
			allMercynaryList.Add(list2[j]);
		}
	}

	private void SetMercenarcyCommander()
	{
		if (allMercynaryList == null)
		{
			return;
		}
		EngageCommander.Clear();
		for (int i = 0; i < allMercynaryList.Count; i++)
		{
			if (allMercynaryList[i].isNpc)
			{
				NPCMercenaryDataRow nPCMercenaryDataRow = RemoteObjectManager.instance.regulation.FindNpcMercenary(allMercynaryList[i].npcId);
				if (nPCMercenaryDataRow != null)
				{
					CommanderDataRow commanderByUnitId = RemoteObjectManager.instance.regulation.GetCommanderByUnitId(nPCMercenaryDataRow.unitId);
					if (commanderByUnitId != null)
					{
						RoCommander roCommander = new RoCommander();
						roCommander.id = commanderByUnitId.id;
						roCommander.cls = nPCMercenaryDataRow.unitClass;
						roCommander.rank = nPCMercenaryDataRow.unitGrade;
						roCommander.level = nPCMercenaryDataRow.unitLevel;
						roCommander.charType = ECharacterType.NPCMercenary;
						List<int> list = new List<int>();
						list.Add(nPCMercenaryDataRow.skillLevel[0]);
						list.Add(nPCMercenaryDataRow.skillLevel[1]);
						list.Add(nPCMercenaryDataRow.skillLevel[2]);
						list.Add(nPCMercenaryDataRow.skillLevel[3]);
						roCommander.skillList = list;
						EngageCommander.Add(roCommander);
					}
				}
				continue;
			}
			for (int j = 0; j < allMercynaryList[i].commanderList.Count; j++)
			{
				RoCommander roCommander = new RoCommander();
				roCommander.id = allMercynaryList[i].commanderList[j].cid.ToString();
				roCommander.cls = allMercynaryList[i].commanderList[j].cls;
				roCommander.rank = allMercynaryList[i].commanderList[j].grade;
				roCommander.level = allMercynaryList[i].commanderList[j].level;
				roCommander.currentCostume = allMercynaryList[i].commanderList[j].costumeIdx;
				roCommander.userName = allMercynaryList[i].commanderList[j].userName;
				roCommander.userIdx = allMercynaryList[i].commanderList[j].userIdx;
				roCommander.charType = ECharacterType.Mercenary;
				roCommander.favorRewardStep = allMercynaryList[i].commanderList[j].favorStep;
				roCommander.marry = allMercynaryList[i].commanderList[j].marry;
				roCommander.transcendence = allMercynaryList[i].commanderList[j].transcendence;
				List<int> list2 = new List<int>();
				list2.Add(allMercynaryList[i].commanderList[j].skillLv_1);
				list2.Add(allMercynaryList[i].commanderList[j].skillLv_2);
				list2.Add(allMercynaryList[i].commanderList[j].skillLv_3);
				list2.Add(allMercynaryList[i].commanderList[j].skillLv_4);
				roCommander.skillList = list2;
				foreach (KeyValuePair<string, int> pair in allMercynaryList[i].commanderList[j].equipItem)
				{
					EquipItemDataRow equipItemDataRow = RemoteObjectManager.instance.regulation.equipItemDtbl.Find((EquipItemDataRow item) => item.key == pair.Key);
					roCommander.SetEquipItem(equipItemDataRow.pointType, RoItem.Create(pair.Key, pair.Value, 1, roCommander.id));
				}
				foreach (KeyValuePair<string, Protocols.WeaponData> item in allMercynaryList[i].commanderList[j].weaponItem)
				{
					WeaponDataRow weaponDataRow = RemoteObjectManager.instance.regulation.weaponDtbl[item.Value.id];
					roCommander.EquipWeaponItem(RoWeapon.Create("0", weaponDataRow.idx, item.Value.level));
				}
				EngageCommander.Add(roCommander);
			}
		}
	}

	public RoCommander FindMercenaryCommander(string commanderId, int userIdx, ECharacterType charType)
	{
		if (EngageCommander == null || EngageCommander.Count == 0)
		{
			return null;
		}
		for (int i = 0; i < EngageCommander.Count; i++)
		{
			if (charType == ECharacterType.NPCMercenary)
			{
				if (EngageCommander[i].id == commanderId)
				{
					return EngageCommander[i];
				}
			}
			else if (EngageCommander[i].id == commanderId && EngageCommander[i].userIdx == userIdx)
			{
				return EngageCommander[i];
			}
		}
		return null;
	}

	public ScenarioCompleteInfo GetScenarioCompleteInfo(string cid, string csid)
	{
		ScenarioCompleteInfo result = default(ScenarioCompleteInfo);
		result.completeQuarterIdx = new List<string>();
		Dictionary<string, Protocols.CommanderScenario> dictionary = new Dictionary<string, Protocols.CommanderScenario>();
		if (sn_resultDictionary != null && sn_resultDictionary.Count > 0 && sn_resultDictionary.ContainsKey(cid))
		{
			dictionary = sn_resultDictionary[cid];
		}
		if (dictionary.Count > 0 && dictionary.ContainsKey(csid))
		{
			int count = dictionary[csid].complete.Count;
			for (int i = 0; i < count; i++)
			{
				result.completeQuarterIdx.Add(dictionary[csid].complete[i]);
			}
			if (dictionary[csid].receive == 1)
			{
				result.isComplete = true;
			}
			else
			{
				result.isComplete = false;
			}
		}
		return result;
	}

	public bool IsCompleteExchangeCarnival(string carnivalIdx)
	{
		bool flag = false;
		bool flag2 = false;
		List<CurCarnivalItemInfo> list = new List<CurCarnivalItemInfo>();
		List<CarnivalDataRow> list2 = RemoteObjectManager.instance.regulation.carnivalDtbl.FindAll((CarnivalDataRow item) => item.idx == carnivalIdx);
		if (list2 != null)
		{
			for (int i = 0; i < list2.Count; i++)
			{
				CurCarnivalItemInfo item2 = default(CurCarnivalItemInfo);
				item2.idx = list2[i].commodityIdx;
				item2.type = list2[i].commodityType;
				item2.needCount = list2[i].commodityCount;
				list.Add(item2);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				switch (list[j].type)
				{
				case ERewardType.Goods:
				case ERewardType.Favor:
				case ERewardType.EventItem:
				{
					GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.goodsDtbl[list[j].idx];
					if (resourceList[goodsDataRow.serverFieldName] >= list[j].needCount)
					{
						if (j == 0)
						{
							flag = true;
						}
						else
						{
							flag2 = true;
						}
					}
					break;
				}
				case ERewardType.Medal:
				{
					RoCommander roCommander = FindCommander(list[j].idx);
					if (roCommander.medal >= list[j].needCount)
					{
						if (j == 0)
						{
							flag = true;
						}
						else
						{
							flag2 = true;
						}
					}
					break;
				}
				case ERewardType.UnitMaterial:
				{
					RoPart roPart = FindPart(list[j].idx);
					if (roPart.count >= list[j].needCount)
					{
						if (j == 0)
						{
							flag = true;
						}
						else
						{
							flag2 = true;
						}
					}
					break;
				}
				}
			}
		}
		if (list.Count == 1)
		{
			if (flag)
			{
				return true;
			}
		}
		else if (flag && flag2)
		{
			return true;
		}
		return false;
	}

	public void ResetConquestSlot()
	{
		int[] array = new int[conquestDeck.Count];
		conquestDeck.Keys.CopyTo(array, 0);
		for (int i = 0; i < array.Length; i++)
		{
			conquestDeck[array[i]] = null;
		}
		for (int j = 0; j < commanderList.Count; j++)
		{
			RoCommander roCommander = commanderList[j];
			roCommander.conquestDeckId = 0;
		}
	}

	public List<RoItem> GetEquipPossibleItemList()
	{
		return EquipPossibleItemList;
	}

	public void EquipPossibleList_RemoveItem(RoItem item)
	{
		EquipPossibleItemList.Remove(item);
	}

	public void EquipPossibleList_AddItem(RoItem item)
	{
		EquipPossibleItemList.Add(item);
	}

	public RoItem EquipPossibleList_FindItem(string id, int lv)
	{
		return EquipPossibleItemList.Find((RoItem row) => row.id == id && row.level == lv);
	}

	public void SetEquipPossibleItemCount(string id, int lv, int count)
	{
		if (count == 0)
		{
			RoItem roItem = EquipPossibleList_FindItem(id, lv);
			if (roItem != null)
			{
				EquipPossibleList_RemoveItem(roItem);
			}
			return;
		}
		RoItem roItem2 = EquipPossibleItemList.Find((RoItem row) => row.id == id && row.level == lv);
		if (roItem2 == null)
		{
			EquipPossibleList_AddItem(RoItem.Create(id, lv, count, null));
		}
		else
		{
			roItem2.itemCount = count;
		}
	}

	public List<RoItem> GetEquipedItemList()
	{
		return EquipedItemList;
	}

	public void EquipedeList_RemoveItem(RoItem item)
	{
		EquipedItemList.Remove(item);
	}

	public void EquipedList_AddItem(RoItem item)
	{
		EquipedItemList.Add(item);
	}

	public RoItem EquipedList_FindItem(string id, string commanderId, int level)
	{
		return EquipedItemList.Find((RoItem row) => row.id == id && row.currEquipCommanderId == commanderId && row.level == level);
	}

	public void EquipedList_upgradeItem(string id, int curLv, string commanderId)
	{
		EquipedList_FindItem(id, commanderId, curLv - 1)?.SetItemLevel(id, curLv);
	}

	public List<RoItem> FindEquipedItemList(string id)
	{
		return EquipedItemList.FindAll((RoItem row) => row.id == id);
	}

	public int GetAllItemCount(string idx)
	{
		int num = 0;
		if (EquipedItemList != null)
		{
			for (int i = 0; i < EquipedItemList.Count; i++)
			{
				if (EquipedItemList[i].id == idx)
				{
					num += EquipedItemList[i].itemCount;
				}
			}
		}
		if (EquipPossibleItemList != null)
		{
			for (int j = 0; j < EquipPossibleItemList.Count; j++)
			{
				if (EquipPossibleItemList[j].id == idx)
				{
					num += EquipPossibleItemList[j].itemCount;
				}
			}
		}
		return num;
	}

	public bool CanOpenFreeGachaBox()
	{
		bool result = false;
		foreach (RoGacha value in gacha.Values)
		{
			if (value.canOpenFreeBox)
			{
				result = true;
			}
		}
		return result;
	}

	public List<RoWeapon> GetWeaponList(int slotType, int privateType)
	{
		return weaponList.FindAll((RoWeapon row) => row.data.slotType == slotType && row.data.privateWeapon == privateType);
	}

	public List<RoWeapon> GetWeaponList(int slotType, int privateType, string unitId)
	{
		return weaponList.FindAll((RoWeapon row) => (privateType == 0) ? (row.data.slotType == slotType && row.data.privateWeapon == privateType) : (row.data.slotType == slotType && row.data.privateWeapon == privateType && row.data.unitIdx == unitId));
	}

	public List<RoWeapon> GetWeaponList(int slotType)
	{
		return weaponList.FindAll((RoWeapon row) => slotType == 0 || row.data.slotType == slotType);
	}

	public RoWeapon FindWeapon(string id)
	{
		return weaponList.Find((RoWeapon row) => row.idx == id);
	}

	public bool GetWeaponPossible(int count = 1)
	{
		return weaponList.Count + count <= statistics.weaponInventoryCount;
	}

	public int GetWeaponCount()
	{
		return weaponList.Count;
	}

	public void RemoveWeapon(List<string> idList)
	{
		for (int i = 0; i < idList.Count; i++)
		{
			string text = idList[i];
			RoWeapon item = FindWeapon(text);
			weaponList.Remove(item);
		}
	}

	public bool InfinityEnableReward(string field)
	{
		if (!infinityStageList.ContainsKey(field))
		{
			return true;
		}
		Dictionary<int, int> dictionary = infinityStageList[field];
		if (dictionary == null || dictionary.Count < 3)
		{
			return true;
		}
		foreach (KeyValuePair<int, int> item in dictionary)
		{
			if (item.Value == 0)
			{
				return true;
			}
		}
		return false;
	}
}
