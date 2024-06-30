using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoomDecorator;
using Shared.Regulation;

public class Protocols
{
	[JsonObject(MemberSerialization.OptIn)]
	public class AuthLoginRequest
	{
		[JsonProperty("mIdx")]
		public int memberId { get; set; }

		[JsonProperty("tokn")]
		public string token { get; set; }

		[JsonProperty("wld")]
		public int world { get; set; }

		[JsonProperty("unm")]
		public string userName { get; set; }

		[JsonProperty("plfm")]
		public Platform platform { get; set; }

		[JsonProperty("devc")]
		public string deviceName { get; set; }

		[JsonProperty("dvid")]
		public string deviceId { get; set; }

		[JsonProperty("ptype")]
		public int patchType { get; set; }

		[JsonProperty("oscd")]
		public OSCode osCode { get; set; }

		[JsonProperty("osvr")]
		public string osVersion { get; set; }

		[JsonProperty("gmvr")]
		public string gameVersion { get; set; }

		[JsonProperty("apk")]
		public string apkFileName { get; set; }

		[JsonProperty("psId")]
		public string pushRegistrationId { get; set; }

		[JsonProperty("lang")]
		public string languageCode { get; set; }

		[JsonProperty("ctry")]
		public string countryCode { get; set; }

		[JsonProperty("gpid")]
		public string largoId { get; set; }

		[JsonProperty("ch")]
		public int channel { get; set; }
	}

	public enum OSCode
	{
		Unknown = 0,
		iOS = 10,
		Android = 20,
		Windows = 30,
		WebGL = 30
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class UserInformationResponse
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class Resource
		{
			[JsonProperty("unm")]
			private string __nickname { get; set; }

			public string nickname
			{
				get
				{
					if (__nickname == null)
					{
						return _localUser.nickname;
					}
					return __nickname;
				}
			}

			[JsonProperty("thmb")]
			private string __thumbnailId { get; set; }

			public string thumbnailId
			{
				get
				{
					if (__thumbnailId == null)
					{
						return _localUser.thumbnailId;
					}
					return __thumbnailId;
				}
			}

			[JsonProperty("exp")]
			private string __exp { get; set; }

			public int exp => ParseInt(__exp, _localUser.exp);

			[JsonProperty("lv")]
			private string __level { get; set; }

			public int level => ParseInt(__level, _localUser.level);

			[JsonProperty("vlv")]
			private string __vipLevel { get; set; }

			public int vipLevel => ParseInt(__vipLevel, _localUser.vipLevel);

			[JsonProperty("vexp")]
			private string __vipExp { get; set; }

			public int vipExp => ParseInt(__vipExp, _localUser.vipExp);

			[JsonProperty("gold")]
			private string __gold { get; set; }

			public int gold => ParseGold(__gold, _localUser.gold);

			[JsonProperty("cash")]
			private string __cash { get; set; }

			public int cash => ParseInt(__cash, _localUser.cash);

			[JsonProperty("honr")]
			private string __honor { get; set; }

			public int honor => ParseInt(__honor, _localUser.honor);

			[JsonProperty("sply")]
			private string __explorationTicket { get; set; }

			public int explorationTicket => ParseInt(__explorationTicket, _localUser.explorationTicket);

			[JsonProperty("swp")]
			private string __sweepTicket { get; set; }

			public int sweepTicket => ParseInt(__sweepTicket, _localUser.sweepTicket);

			[JsonProperty("keys")]
			private string __opener { get; set; }

			public int opener => ParseInt(__opener, _localUser.opener);

			[JsonProperty("chlg")]
			private string __challenge { get; set; }

			public int challenge => ParseInt(__challenge, _localUser.challenge);

			[JsonProperty("blcg")]
			private string __blackChallenge { get; set; }

			public int blackChallenge => ParseInt(__blackChallenge, _localUser.blackChallenge);

			[JsonProperty("opcn")]
			private string __opcon { get; set; }

			public int opcon => ParseInt(__opcon, _localUser.opcon);

			[JsonProperty("acon")]
			private string __challengeCoin { get; set; }

			public int challengeCoin => ParseInt(__challengeCoin, _localUser.challengeCoin);

			[JsonProperty("wbt")]
			private string __waveDuelTicket { get; set; }

			public int waveDuelTicket => ParseInt(__waveDuelTicket, _localUser.waveDuelTicket);

			[JsonProperty("wbc")]
			private string __waveDuelCoin { get; set; }

			public int waveDuelCoin => ParseInt(__waveDuelCoin, _localUser.waveDuelCoin);

			[JsonProperty("gcon")]
			private string __guildCoin { get; set; }

			public int guildCoin => ParseInt(__guildCoin, _localUser.guildCoin);

			[JsonProperty("rcon")]
			private string __raidCoin { get; set; }

			public int raidCoin => ParseInt(__raidCoin, _localUser.raidCoin);

			[JsonProperty("ncon")]
			private string __annCoin { get; set; }

			public int annCoin => ParseInt(__annCoin, _localUser.annCoin);

			[JsonProperty("bult")]
			private string __bullet { get; set; }

			public int bullet => ParseInt(__bullet, _localUser.bullet);

			[JsonProperty("chip")]
			private string __chip { get; set; }

			public int chip => ParseInt(__chip, _localUser.chip);

			[JsonProperty("abp")]
			private string __blueprintArmy { get; set; }

			public int blueprintArmy => ParseInt(__blueprintArmy, _localUser.blueprintArmy);

			[JsonProperty("nbp")]
			private string __blueprintNavy { get; set; }

			public int blueprintNavy => ParseInt(__blueprintNavy, _localUser.blueprintNavy);

			[JsonProperty("cmtr")]
			private string __commanderPromotionPoint { get; set; }

			public int commanderPromotionPoint => ParseInt(__commanderPromotionPoint, _localUser.commanderPromotionPoint);

			[JsonProperty("ebac")]
			private string __eventRaidTicket { get; set; }

			public int eventRaidTicket => ParseInt(__eventRaidTicket, _localUser.eventRaidTicket);

			[JsonProperty("oil")]
			private string __oil { get; set; }

			public int oil => ParseInt(__oil, _localUser.oil);

			[JsonProperty("wmat1")]
			private string __weaponMaterial1 { get; set; }

			public int weaponMaterial1 => ParseInt(__weaponMaterial1, _localUser.weaponMaterial1);

			[JsonProperty("wmat2")]
			private string __weaponMaterial2 { get; set; }

			public int weaponMaterial2 => ParseInt(__weaponMaterial2, _localUser.weaponMaterial2);

			[JsonProperty("wmat3")]
			private string __weaponMaterial3 { get; set; }

			public int weaponMaterial3 => ParseInt(__weaponMaterial3, _localUser.weaponMaterial3);

			[JsonProperty("wmat4")]
			private string __weaponMaterial4 { get; set; }

			public int weaponMaterial4 => ParseInt(__weaponMaterial4, _localUser.weaponMaterial4);

			[JsonProperty("wimt")]
			private string __weaponImmediateTicket { get; set; }

			public int weaponImmediateTicket => ParseInt(__weaponImmediateTicket, _localUser.weaponImmediateTicket);

			[JsonProperty("wmt")]
			private string __weaponMakeTicket { get; set; }

			public int weaponMakeTicket => ParseInt(__weaponMakeTicket, _localUser.weaponMakeTicket);

			[JsonProperty("ring")]
			private string __ring { get; set; }

			public int ring => ParseInt(__ring, _localUser.ring);

			[JsonProperty("cgft")]
			private string __commanderGift { get; set; }

			public int commanderGift => ParseInt(__commanderGift, _localUser.commanderGift);

			[JsonProperty("sbtk")]
			private string __worldDuelTicket { get; set; }

			public int worldDuelTicket => ParseInt(__worldDuelTicket, _localUser.worldDuelTicket);

			[JsonProperty("sbc1")]
			private string __worldDuelCoin { get; set; }

			public int worldDuelCoin => ParseInt(__worldDuelCoin, _localUser.worldDuelCoin);

			[JsonProperty("sbc2")]
			private string __worldDuelUpgradeCoin { get; set; }

			public int worldDuelUpgradeCoin => ParseInt(__worldDuelUpgradeCoin, _localUser.worldDuelUpgradeCoin);
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class BattleResult
		{
			[JsonObject(MemberSerialization.OptIn)]
			public class UserData
			{
				[JsonProperty("prnk")]
				public int prevRank { get; private set; }

				[JsonProperty("rank")]
				public int rank { get; private set; }

				[JsonProperty("prct")]
				public float rankPercent { get; private set; }

				[JsonProperty("score")]
				public int curScore { get; private set; }

				[JsonProperty("pscr")]
				public int prevScore { get; private set; }

				[JsonProperty("ascr")]
				public int getScore { get; private set; }

				[JsonProperty("wscr")]
				public int winScore { get; private set; }

				[JsonProperty("wst")]
				public int winCount { get; private set; }

				[JsonProperty("pwst")]
				public int prevWinCount { get; private set; }

				[JsonProperty("dpnt")]
				public int duelPoint { get; set; }
			}

			[JsonProperty("user")]
			public UserData user;

			[JsonProperty("save")]
			public bool save { get; set; }

			[JsonProperty("rsoc")]
			private Resource __resource { get; set; }

			public Resource resource => __resource;

			[JsonProperty("reward")]
			public List<RewardInfo.RewardData> rewardList { get; set; }

			[JsonProperty("comm")]
			public Dictionary<string, Commander> commanderData { get; set; }

			[JsonProperty("part")]
			public Dictionary<string, int> partData { get; set; }

			[JsonProperty("medl")]
			public Dictionary<string, int> medalData { get; set; }

			[JsonProperty("ersoc")]
			public Dictionary<string, int> eventResourceData { get; set; }

			[JsonProperty("item")]
			public Dictionary<string, int> itemData { get; set; }

			[JsonProperty("food")]
			public Dictionary<string, int> foodData { get; set; }

			[JsonProperty("favr")]
			public List<FavorUpData.CommanderFavor> commanderFavor { get; set; }

			[JsonProperty("vshp")]
			public int VipShopOpen { get; set; }

			[JsonProperty("vrtm")]
			public int VipShopResetTime { get; set; }

			[JsonProperty("guit")]
			public Dictionary<string, int> groupItemData { get; set; }

			[JsonProperty("tinfo")]
			public InfinityTowerData infinityData { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class BattleStatistics
		{
			[JsonProperty("egld")]
			public long totalGold { get; set; }

			[JsonProperty("pvew")]
			public int pveWinCount { get; set; }

			[JsonProperty("pvel")]
			public int pveLoseCount { get; set; }

			[JsonProperty("pvpw")]
			public int pvpWinCount { get; set; }

			[JsonProperty("pvpl")]
			public int pvpLoseCount { get; set; }

			[JsonProperty("acdc")]
			public int armyCommanderDestroyCount { get; set; }

			[JsonProperty("audc")]
			public int armyUnitDestroyCount { get; set; }

			[JsonProperty("ncdc")]
			public int navyCommanderDestroyCount { get; set; }

			[JsonProperty("nudc")]
			public int navyUnitDestroyCount { get; set; }

			[JsonProperty("pdgd")]
			public int totalPlunderGold { get; set; }

			[JsonProperty("wst")]
			public int winStreak { get; set; }

			[JsonProperty("wmst")]
			public int winMostStreak { get; set; }

			[JsonProperty("pwst")]
			public int preWinStreak { get; set; }

			[JsonProperty("atr")]
			public int arenaHighRank { get; set; }

			[JsonProperty("rtr")]
			public int raidHighRank { get; set; }

			[JsonProperty("rts")]
			public int raidHighScore { get; set; }

			[JsonProperty("ggc")]
			public int normalGachaCount { get; set; }

			[JsonProperty("cgc")]
			public int premiumGachaCount { get; set; }

			[JsonProperty("stcc")]
			public int stageClearCount { get; set; }

			[JsonProperty("swcc")]
			public int sweepClearCount { get; set; }

			[JsonProperty("edc")]
			public int unitDestroyCount { get; set; }

			[JsonProperty("pdc")]
			public int commanderDestroyCount { get; set; }

			[JsonProperty("vshp")]
			public int vipShop { get; set; }

			[JsonProperty("vrtm")]
			public int vipShopResetTime { get; set; }

			[JsonProperty("prdc")]
			public int predeckCount { get; set; }

			[JsonProperty("fpur")]
			public int firstPayment { get; set; }

			[JsonProperty("wmsc")]
			public int weaponMakeSlotCount { get; set; }

			[JsonProperty("wic")]
			public int weaponInventoryCount { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class Unit
		{
			[JsonProperty("uid")]
			public string id;

			[JsonProperty("lv")]
			public int level;

			[JsonProperty("hp")]
			public int Hp;

			[JsonProperty("sp")]
			public List<int> spList { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class Building
		{
			[JsonProperty("bid")]
			public int id { get; set; }

			[JsonProperty("stus")]
			public int state { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("remain")]
			public int remainTime { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class Commander
		{
			[JsonProperty("cid")]
			public string id { get; set; }

			[JsonProperty("lv")]
			private string __level { get; set; }

			public int level => ParseInt(__level, 0);

			[JsonProperty("grd")]
			private string __rank { get; set; }

			public int rank => ParseInt(__rank, 0);

			[JsonProperty("cls")]
			private string __cls { get; set; }

			public int cls => ParseInt(__cls, 0);

			[JsonProperty("exp")]
			private string __exp { get; set; }

			public int exp => ParseInt(__exp, 0);

			[JsonProperty("medl")]
			public int medl { get; set; }

			[JsonProperty("skv1")]
			private string __skv1 { get; set; }

			public int skv1 => ParseInt(__skv1, 0);

			[JsonProperty("skv2")]
			private string __skv2 { get; set; }

			public int skv2 => ParseInt(__skv2, 0);

			[JsonProperty("skv3")]
			private string __skv3 { get; set; }

			public int skv3 => ParseInt(__skv3, 0);

			[JsonProperty("skv4")]
			private string __skv4 { get; set; }

			public int skv4 => ParseInt(__skv4, 0);

			[JsonProperty("role")]
			public string role { get; set; }

			[JsonProperty("stus")]
			public string state { get; set; }

			[JsonProperty("cos")]
			public int currentCostume { get; set; }

			[JsonProperty("clst")]
			public List<int> haveCostume { get; set; }

			[JsonProperty("evt")]
			public Dictionary<int, int> eventCostume { get; set; }

			[JsonProperty("fs")]
			public int favorStep { get; set; }

			[JsonProperty("fp")]
			public int favorPoint { get; set; }

			[JsonProperty("rsf")]
			public int favorRewardStep { get; set; }

			[JsonProperty("favr")]
			public int favr { get; set; }

			[JsonProperty("fvrd")]
			public int fvrd { get; set; }

			[JsonProperty("eq")]
			public Dictionary<string, int> equipItemInfo { get; set; }

			[JsonProperty("mry")]
			public int marry { get; set; }

			[JsonProperty("tsdc")]
			public List<int> transcendence { get; set; }

			[JsonProperty("wp")]
			public Dictionary<string, WeaponData> equipWeaponInfo { get; set; }

			public bool isStateRankUp => state == "P";

			public bool isStateNormal => state == "N";
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class DailyCheckPoint
		{
			[JsonProperty("swbc")]
			public int sweepTicketBuyCount { get; set; }

			[JsonProperty("spbc")]
			public int explorationTicketBuyCount { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class PartData
		{
			[JsonProperty("pidx")]
			public string idx { get; set; }

			[JsonProperty("cnt")]
			public int count { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class UserGuild
		{
			[JsonObject(MemberSerialization.OptIn)]
			public class GuildSkill
			{
				[JsonProperty("gsid")]
				public int idx { get; set; }

				[JsonProperty("gslv")]
				public int level { get; set; }
			}

			[JsonProperty("gidx")]
			public int idx { get; set; }

			[JsonProperty("gnm")]
			public string name { get; set; }

			[JsonProperty("lev")]
			public int level { get; set; }

			[JsonProperty("pnt")]
			public int point { get; set; }

			[JsonProperty("apnt")]
			public int aPoint { get; set; }

			[JsonProperty("mstr")]
			public int memberGrade { get; set; }

			[JsonProperty("emb")]
			public int emblem { get; set; }

			[JsonProperty("gtyp")]
			public int guildType { get; set; }

			[JsonProperty("lvlm")]
			public int limitLevel { get; set; }

			[JsonProperty("ntc")]
			public string notice { get; set; }

			[JsonProperty("stat")]
			public int state { get; set; }

			[JsonProperty("ctime")]
			public double closeTime { get; set; }

			[JsonProperty("reg")]
			public double createTime { get; set; }

			[JsonProperty("mxCnt")]
			public int maxCount { get; set; }

			[JsonProperty("cnt")]
			public int count { get; set; }

			[JsonProperty("skill")]
			public List<GuildSkill> skillDada { get; set; }

			[JsonProperty("occupy")]
			public int occupy { get; set; }

			[JsonProperty("world")]
			public int world { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class VipRechargeData
		{
			[JsonProperty("vidx")]
			public int idx { get; set; }

			[JsonProperty("mid")]
			public int mid { get; set; }

			[JsonProperty("cnt")]
			public int count { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class TutorialData
		{
			[JsonProperty("step")]
			public int step { get; set; }

			[JsonProperty("skip")]
			public bool skip { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class PreDeck
		{
			[JsonProperty("dpid")]
			public int idx { get; set; }

			[JsonProperty("dpnm")]
			public string name { get; set; }

			[JsonProperty("deck")]
			public Dictionary<string, int> deckData { get; set; }
		}

		[JsonProperty("rsoc")]
		public Resource goodsInfo { get; set; }

		[JsonProperty("uifo")]
		public BattleStatistics battleStatisticsInfo { get; set; }

		[JsonProperty("comm")]
		private object __commanderInfo { get; set; }

		public Dictionary<string, Commander> commanderInfo
		{
			get
			{
				if (__commanderInfo == null)
				{
					return null;
				}
				JArray jArray = null;
				try
				{
					jArray = JArray.FromObject(__commanderInfo);
				}
				catch (Exception)
				{
				}
				if (jArray != null)
				{
					return null;
				}
				JObject jObject = JObject.FromObject(__commanderInfo);
				return jObject.ToObject<Dictionary<string, Commander>>();
			}
		}

		[JsonProperty("uno")]
		public string uno { get; set; }

		[JsonProperty("stage")]
		public int stage { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("gld")]
		public UserGuild guildInfo { get; set; }

		[JsonProperty("cc")]
		public Dictionary<string, List<int>> sweepClearData { get; set; }

		[JsonProperty("deck")]
		public List<PreDeck> preDeck { get; set; }

		[JsonProperty("nhcc")]
		public Dictionary<string, List<int>> donHaveCommCostumeData { get; set; }

		[JsonProperty("grp")]
		public List<int> completeRewardGroupIdx { get; set; }

		[JsonProperty("rstm")]
		public int resetRemain { get; set; }

		[JsonProperty("onoff")]
		public bool notification { get; set; }

		[JsonProperty("equip")]
		public Dictionary<string, Dictionary<int, EquipItemInfo>> equipItem { get; set; }

		[JsonProperty("guit")]
		public Dictionary<string, int> groupItemData { get; set; }

		[JsonProperty("weapon")]
		public Dictionary<string, WeaponData> weaponList { get; set; }
	}

	public enum UserInformationType
	{
		Resource,
		Upgrade,
		Unit,
		Building,
		Commander,
		Battle,
		DailyCheckPoint,
		Recharge,
		Tutorial,
		part,
		medal,
		item,
		DormitoryResource,
		DormitoryInfo,
		DormitoryInvenNormalDeco,
		DormitoryInvenAdvancedDeco,
		DormitoryInvenWallpaperDeco,
		DormitoryInvenCostumeBody,
		DormitoryInvenCostumeHead
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class BuildingLevelUpResponse
	{
		[JsonProperty("gold")]
		private string __gold { get; set; }

		public int gold => ParseGold(__gold, _localUser.gold);

		[JsonProperty("cash")]
		private string __cash { get; set; }

		public int cash => ParseInt(__cash, _localUser.cash);

		[JsonProperty("remain")]
		public int remainTime { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class BuildingInformationResponse
	{
		[JsonProperty("bid")]
		public EBuilding buildType { get; set; }

		[JsonProperty("stus")]
		public int stateCode { get; set; }

		[JsonProperty("lv")]
		public int level { get; set; }

		[JsonProperty("remain")]
		public int remainTime { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class SimpleCommanderInfo
	{
		[JsonProperty("cid")]
		public string id { get; set; }

		[JsonProperty("medl")]
		public int medal { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class RecruitCommanderResponse
	{
		[JsonProperty("gold")]
		public long gold { get; set; }

		[JsonProperty("honr")]
		public int honor { get; set; }

		[JsonProperty("medl")]
		public int medal { get; set; }

		[JsonProperty("comm")]
		public SimpleCommanderInfo commander { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class RecruitCommanderDelayResponse
	{
		[JsonProperty("wait")]
		public int wait { get; set; }

		[JsonProperty("gold")]
		private string __gold { get; set; }

		public int gold => ParseGold(__gold, _localUser.gold);

		[JsonProperty("cash")]
		private string __cash { get; set; }

		public int cash => ParseInt(__cash, _localUser.cash);

		[JsonProperty("comm")]
		public SimpleCommanderInfo commander { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CommanderRankUpResponse
	{
		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource rsoc;

		[JsonProperty("medl")]
		public Dictionary<string, int> medl;

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> comm { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class RecruitCommanderListResponse
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class Commander
		{
			[JsonProperty("cid")]
			public string id { get; set; }

			[JsonProperty("grd")]
			public string rank { get; set; }

			[JsonProperty("honr")]
			public int honor { get; set; }

			[JsonProperty("gold")]
			public int gold { get; set; }

			[JsonProperty("cash")]
			public int cash { get; set; }

			[JsonProperty("sell")]
			public bool recruited { get; set; }

			[JsonProperty("wait")]
			public int waitTime { get; set; }
		}

		[JsonProperty("list")]
		public List<Commander> list { get; set; }

		[JsonProperty("remain")]
		public int remainTime { get; set; }

		[JsonProperty("reload")]
		public string reload { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class RecruitCommanderListDictResponse
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class Commander
		{
			[JsonProperty("cid")]
			public string id { get; set; }

			[JsonProperty("grd")]
			public string rank { get; set; }

			[JsonProperty("honr")]
			public int honor { get; set; }

			[JsonProperty("gold")]
			public int gold { get; set; }

			[JsonProperty("cash")]
			public int cash { get; set; }

			[JsonProperty("sell")]
			public bool recruited { get; set; }

			[JsonProperty("wait")]
			public int waitTime { get; set; }

			[JsonProperty("dnm")]
			public int troopNickname { get; set; }
		}

		[JsonProperty("list")]
		public Dictionary<string, Commander> list { get; set; }

		[JsonProperty("remain")]
		public int remainTime { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class GetUnitResearchListResponse
	{
		[JsonProperty("idx")]
		public int id { get; set; }

		[JsonProperty("time")]
		public int remainTime { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class UnitLevelUpResponse
	{
		[JsonProperty("gold")]
		public long gold { get; set; }

		[JsonProperty("abp")]
		private string __blueprintArmy { get; set; }

		public int blueprintArmy
		{
			get
			{
				if (string.IsNullOrEmpty(__blueprintArmy))
				{
					return -1;
				}
				return int.Parse(__blueprintArmy);
			}
		}

		[JsonProperty("nbp")]
		private string __blueprintNavy { get; set; }

		public int blueprintNavy
		{
			get
			{
				if (string.IsNullOrEmpty(__blueprintNavy))
				{
					return -1;
				}
				return int.Parse(__blueprintNavy);
			}
		}
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class UnitUpgradeResponse
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class Resource
		{
			[JsonProperty("gold")]
			private string __gold { get; set; }

			public int gold => ParseGold(__gold, _localUser.gold);

			[JsonProperty("abp")]
			private string __blueprintArmy { get; set; }

			public int blueprintArmy => ParseInt(__blueprintArmy, _localUser.blueprintArmy);
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class Unit
		{
			[JsonProperty("uid")]
			public string id;

			[JsonProperty("lv")]
			public int level;

			[JsonProperty("sklv")]
			public int sklv;
		}

		[JsonProperty("ursc")]
		public List<UserInformationResponse.PartData> partData { get; set; }

		[JsonProperty("rsoc")]
		public Resource goodsInfo { get; set; }

		[JsonProperty("unit")]
		public Dictionary<string, Unit> unitInfo { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class GetTroopInformationResponse
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class Slot
		{
			[JsonProperty("uid")]
			public string unitId { get; set; }
		}

		[JsonProperty("cid")]
		public string commanderId { get; set; }

		[JsonProperty("dnm")]
		public string nickname { get; set; }

		[JsonProperty("deck")]
		public Dictionary<string, Slot> slots { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class WorldMapInformationResponse
	{
		[JsonProperty("mid")]
		public string stageId { get; set; }

		[JsonProperty("cnt")]
		public int clearCount { get; set; }

		[JsonProperty("star")]
		public int star { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class WorldMapEndProductionResponse
	{
		[JsonProperty("res")]
		public UserInformationResponse.Resource resource { get; private set; }

		[JsonProperty("reward")]
		public List<RewardInfo.RewardData> reward { get; set; }

		[JsonProperty("pldr")]
		public bool plundered { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ScrambleMapInformationResponse
	{
		[JsonProperty("stage")]
		public string stageId { get; set; }

		[JsonProperty("stus")]
		public int status { get; set; }

		[JsonProperty("rmhp")]
		private string __hp { get; set; }

		public int hp => ParseInt(__hp, 0);
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ScrambleMapHistory
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class userData
		{
			[JsonProperty("dnm")]
			public string troopName { get; private set; }

			[JsonProperty("pwr")]
			public int power { get; private set; }

			[JsonProperty("cid")]
			public string cid { get; private set; }

			[JsonProperty("lv")]
			public int level { get; private set; }

			[JsonProperty("grd")]
			public int grade { get; private set; }

			[JsonProperty("cls")]
			public int cls { get; private set; }

			[JsonProperty("cos")]
			public int costume { get; private set; }

			[JsonProperty("rsf")]
			public int favorRewardStep { get; private set; }

			[JsonProperty("mry")]
			public int marry { get; set; }

			[JsonProperty("tsdc")]
			public List<int> transcendence { get; set; }

			[JsonProperty("nm")]
			public string nickName { get; private set; }

			[JsonProperty("uLv")]
			public int userLevel { get; private set; }

			[JsonProperty("deck")]
			public object __troopSlotsSource { get; private set; }

			public Dictionary<int, Slot> troopSlots
			{
				get
				{
					if (__troopSlotsSource == null)
					{
						return null;
					}
					JArray jArray = null;
					try
					{
						jArray = JArray.FromObject(__troopSlotsSource);
					}
					catch (Exception)
					{
					}
					if (jArray != null)
					{
						return null;
					}
					JObject jObject = JObject.FromObject(__troopSlotsSource);
					return jObject.ToObject<Dictionary<int, Slot>>();
				}
			}
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class Slot
		{
			[JsonProperty("uid")]
			public string unitId { get; set; }

			[JsonProperty("hpPercent")]
			public int unitHp { get; set; }

			[JsonProperty("lev")]
			public int unitLevel { get; set; }
		}

		[JsonProperty("result")]
		public int result { get; set; }

		[JsonProperty("my")]
		public userData myHistory { get; set; }

		[JsonProperty("enmy")]
		public userData enemyHistory { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class DailyBonusCheckResponse
	{
		[JsonProperty("ver")]
		public string version { get; private set; }

		[JsonProperty("didx")]
		public int day { get; private set; }

		[JsonProperty("gidx")]
		public string goodsId { get; private set; }

		[JsonProperty("amnt")]
		public int goodsCount { get; private set; }

		[JsonProperty("sdt")]
		public string startTimeString { get; private set; }

		[JsonProperty("edt")]
		public string endTimeString { get; private set; }

		[JsonProperty("rcvd")]
		public int receiveState { get; private set; }

		public bool received => receiveState > 0;
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class GachaInformationResponse
	{
		[JsonProperty("gbIdx")]
		public string type { get; private set; }

		[JsonProperty("cnt")]
		public int freeOpenRemainCount { get; private set; }

		[JsonProperty("remain")]
		public int freeOpenRemainTime { get; private set; }

		[JsonProperty("acc")]
		public int pilotRate { get; private set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class GachaOpenBoxResponse
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class Reward
		{
			[JsonProperty("type")]
			public ERewardType type { get; private set; }

			[JsonProperty("idx")]
			public string id { get; private set; }

			[JsonProperty("cnt")]
			public int count { get; private set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class CommanderMedal
		{
			[JsonProperty("medl")]
			public int medal { get; private set; }
		}

		[JsonProperty("gbIdx")]
		public EGachaAnimationType type { get; private set; }

		[JsonProperty("gacha")]
		public List<Reward> rewardList { get; private set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource goodsResult { get; private set; }

		[JsonProperty("commMedl")]
		public Dictionary<string, CommanderMedal> commanderIdMedalDict { get; private set; }

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commanderIdDict { get; private set; }

		[JsonProperty("clst")]
		public List<Dictionary<string, RewardInfo.HaveCostumeInfo>> costumeData { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("info")]
		public GachaInformationResponse changedGachaInformation { get; private set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("equip")]
		public Dictionary<string, Dictionary<int, EquipItemInfo>> equipItem { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class GachaRatingDataTypeA
	{
		[JsonProperty("rating")]
		public float rating { get; private set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class GachaRatingDataTypeB
	{
		[JsonProperty("list")]
		public Dictionary<string, Dictionary<int, float>> list { get; private set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class SystemMessage
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class SystemCheck
		{
			[JsonObject(MemberSerialization.OptIn)]
			public class Message
			{
				[JsonProperty("ko")]
				public string ko { get; set; }

				[JsonProperty("cng")]
				public string cn { get; set; }

				[JsonProperty("cnb")]
				public string tw { get; set; }

				[JsonProperty("jp")]
				public string jp { get; set; }

				[JsonProperty("en")]
				public string en { get; set; }

				[JsonProperty("ru")]
				public string ru { get; set; }
			}

			[JsonProperty("fromTime")]
			public double fromTime { get; set; }

			[JsonProperty("toTime")]
			public double toTime { get; set; }

			[JsonProperty("msg")]
			public Message message { get; set; }

			[JsonProperty("now")]
			public double nowTime { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class NoticeList
		{
			[JsonProperty("realtime")]
			public NoticeData realtime { get; set; }

			[JsonProperty("chat")]
			public NoticeData chat { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class NoticeData
		{
			[JsonProperty("ctnt")]
			public string contents { get; set; }

			[JsonProperty("idx")]
			public int idx { get; set; }

			[JsonProperty("sdt")]
			public double startDate { get; set; }

			[JsonProperty("edt")]
			public double endDate { get; set; }
		}

		[JsonProperty("lvup")]
		public int level { get; set; }

		[JsonProperty("sess")]
		public string session { get; set; }

		[JsonProperty("cid")]
		public string commanderId { get; set; }

		[JsonProperty("dmid")]
		public int missionId { get; set; }

		[JsonProperty("fin")]
		public bool missionComplete { get; set; }

		[JsonProperty("gidx")]
		public string __gidx { get; set; }

		public int gidx => ParseInt(__gidx, (_localUser.guildInfo != null) ? _localUser.guildInfo.idx : 0);

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("systemCheck")]
		public SystemCheck systemCheck { get; set; }

		[JsonProperty("notice")]
		public NoticeList noticeList { get; set; }

		[JsonProperty("cnv")]
		public int carnival1 { get; set; }

		[JsonProperty("cnv2")]
		public int carnival2 { get; set; }

		[JsonProperty("cnv3")]
		public int carnival3 { get; set; }

		[JsonProperty("rstm")]
		public int resetRemain { get; set; }

		public int userLevel
		{
			get
			{
				if (!string.IsNullOrEmpty(session))
				{
					return level;
				}
				return -1;
			}
		}

		public int commanderLevel
		{
			get
			{
				if (!string.IsNullOrEmpty(commanderId))
				{
					return level;
				}
				return -1;
			}
		}

		public int getMissionId
		{
			get
			{
				if (missionComplete)
				{
					return missionId;
				}
				return -1;
			}
		}
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CashShopInfo
	{
		[JsonProperty("cstp")]
		public int cashType { get; set; }

		[JsonProperty("csam")]
		public int cashAmount { get; set; }

		[JsonProperty("gmcs")]
		public int gameCash { get; set; }

		[JsonProperty("csevt")]
		public int eventCashAmount { get; set; }

		[JsonProperty("gmcsevt")]
		public int eventGameCash { get; set; }

		[JsonProperty("evtId")]
		public int eventId { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class SituationExploreReward
	{
		[JsonProperty("sply")]
		public int exploreTicket { get; set; }

		[JsonProperty("gold")]
		private string __gold { get; set; }

		public int gold => ParseGold(__gold, _localUser.gold);

		[JsonProperty("cash")]
		private string __cash { get; set; }

		public int cash => ParseInt(__cash, _localUser.cash);

		[JsonProperty("reward")]
		private object __reward { get; set; }

		public Dictionary<string, int> reward
		{
			get
			{
				if (__reward == null)
				{
					return null;
				}
				JArray jArray = null;
				try
				{
					jArray = JArray.FromObject(__reward);
				}
				catch (Exception)
				{
				}
				if (jArray != null)
				{
					return null;
				}
				JObject jObject = JObject.FromObject(__reward);
				return jObject.ToObject<Dictionary<string, int>>();
			}
		}
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class PvPRankingList
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class RankData
		{
			[JsonProperty("rid")]
			public string replayId;

			[JsonProperty("uno")]
			public int id { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("name")]
			private string _name { get; set; }

			public string name => (!isNpc) ? _name : Localization.Get(_name);

			[JsonProperty("thmb")]
			public string thumb { get; set; }

			[JsonProperty("score")]
			public int score { get; set; }

			[JsonProperty("grd")]
			public int grade { get; set; }

			[JsonProperty("rank")]
			public int rank { get; set; }

			[JsonProperty("rgts")]
			public int time { get; set; }

			[JsonProperty("gnm", NullValueHandling = NullValueHandling.Ignore)]
			public string guildName { get; set; }

			[JsonProperty("gwld")]
			public int guildServer { get; set; }

			public bool isNpc => rank == 0 || id >= ConstValue.NpcStartUno;
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class RaidInfo
		{
			[JsonProperty("etm")]
			public int endTime { get; set; }
		}

		[JsonProperty("rank")]
		public List<RankData> rankList { get; set; }

		[JsonProperty("user")]
		public RankingUserData user { get; set; }

		[JsonProperty("boss")]
		public List<Dictionary<string, int>> bossData { get; set; }

		[JsonProperty("info")]
		public RaidInfo info { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class RankingUserData
	{
		[JsonProperty("rank")]
		public int ranking { get; set; }

		[JsonProperty("prct")]
		public float rankingRate { get; set; }

		[JsonProperty("score")]
		public int score { get; set; }

		[JsonProperty("nScore")]
		public int nextScore { get; set; }

		[JsonProperty("wst")]
		public int winningStreak { get; set; }

		[JsonProperty("lst")]
		public int losingStreak { get; set; }

		[JsonProperty("win")]
		public int winCnt { get; set; }

		[JsonProperty("lose")]
		public int loseCnt { get; set; }

		[JsonProperty("rcnt")]
		public int raidCnt { get; set; }

		[JsonProperty("nscr")]
		public int bestScore { get; set; }

		[JsonProperty("avrg")]
		public int averageScore { get; set; }

		[JsonProperty("ridx")]
		public int rewardId { get; set; }

		[JsonProperty("dpnt")]
		public int duelPoint { get; set; }

		[JsonProperty("didx")]
		public int rewardDuelPoint { get; set; }

		[JsonProperty("wrank")]
		public int winRank { get; set; }

		[JsonProperty("wridx")]
		public int winRankIdx { get; set; }

		[JsonProperty("drank")]
		public int raidRank { get; set; }

		[JsonProperty("dridx")]
		public int raidRewardPoint { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class PvPDuelList
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class PvPDuelData
		{
			[JsonProperty("uno")]
			public int uno { get; set; }

			[JsonProperty("idx")]
			public int idx { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("unm")]
			private string _name { get; set; }

			public string name => (!isNpc) ? _name : Localization.Get(_name);

			[JsonProperty("rank")]
			public int rank { get; set; }

			[JsonProperty("clr")]
			public string clear { get; set; }

			[JsonProperty("thmb")]
			public string thumbnail { get; set; }

			[JsonProperty("deck")]
			public List<PvPDuelDeck> deck { get; set; }

			[JsonProperty("decks")]
			public Dictionary<string, List<PvPDuelDeck>> decks { get; set; }

			[JsonProperty("gnm", NullValueHandling = NullValueHandling.Ignore)]
			public string guildName { get; set; }

			[JsonProperty("gld")]
			public List<GuildSkill> guildSkills { get; set; }

			[JsonProperty("grp")]
			public List<int> groupBuffs { get; set; }

			[JsonProperty("wld")]
			public int world { get; set; }

			[JsonProperty("buff")]
			public Dictionary<string, int> duelBuff { get; set; }

			[JsonProperty("bbf")]
			public List<int> activeBuff { get; set; }

			[JsonProperty("wrank")]
			public int winRank { get; set; }

			[JsonProperty("wridx")]
			public int winRankIdx { get; set; }

			[JsonProperty("score")]
			public int score { get; set; }

			[JsonProperty("win")]
			public int winCnt { get; set; }

			[JsonProperty("lose")]
			public int loseCnt { get; set; }

			[JsonIgnore]
			public bool isNpc => rank == 0 || uno >= ConstValue.NpcStartUno;
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class GuildSkill
		{
			[JsonProperty("gsid")]
			public int idx { get; set; }

			[JsonProperty("gslv")]
			public int level { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class PvPDuelDeck
		{
			[JsonProperty("pos")]
			public int position { get; set; }

			[JsonProperty("cid")]
			public int commanderId { get; set; }

			[JsonProperty("grd")]
			public int grade { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("cls")]
			public int cls { get; set; }

			[JsonProperty("cos")]
			public int costume { get; set; }

			[JsonProperty("rsf")]
			public int favorRewardStep { get; private set; }

			[JsonProperty("mry")]
			public int marry { get; set; }

			[JsonProperty("tsdc")]
			public List<int> transcendence { get; set; }

			[JsonProperty("skil1")]
			public int skill1 { get; set; }

			[JsonProperty("skil2")]
			public int skill2 { get; set; }

			[JsonProperty("skil3")]
			public int skill3 { get; set; }

			[JsonProperty("skil4")]
			public int skill4 { get; set; }

			[JsonProperty("equip")]
			public Dictionary<string, int> equipItem { get; set; }

			[JsonProperty("wp")]
			public Dictionary<string, WeaponData> weaponItem { get; set; }
		}

		[JsonProperty("rfrm")]
		public int remain { get; set; }

		[JsonProperty("itrm")]
		public int time { get; set; }

		[JsonProperty("oprm")]
		public int openRemain { get; set; }

		[JsonProperty("user")]
		public RankingUserData user { get; set; }

		[JsonProperty("list")]
		public Dictionary<int, PvPDuelData> duelList { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class RefreshPvPDuel
	{
		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource rsoc;

		[JsonProperty("list")]
		public Dictionary<int, PvPDuelList.PvPDuelData> duelList { get; set; }

		[JsonProperty("rfrm")]
		public int remain { get; set; }

		[JsonProperty("itrm")]
		public int time { get; set; }

		[JsonProperty("oprm")]
		public int openRemain { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class PvPDuelInformation
	{
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class MissionInfo
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class MissionData
		{
			[JsonProperty("dmid")]
			public int missionId { get; set; }

			[JsonProperty("dmpt")]
			public int point { get; set; }

			[JsonProperty("fin")]
			public int complete { get; set; }

			[JsonProperty("rcvd")]
			public int receive { get; set; }
		}

		[JsonProperty("dlms")]
		public List<MissionData> missionList { get; set; }

		[JsonProperty("dmg")]
		public int goal { get; set; }

		[JsonProperty("dmcc")]
		public int completeCount { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class AchievementInfo
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class AchievementData
		{
			[JsonProperty("acid")]
			public int achievementId { get; set; }

			[JsonProperty("asot")]
			public int sort { get; set; }

			[JsonProperty("apt")]
			public int point { get; set; }

			[JsonProperty("fin")]
			public int complete { get; set; }

			[JsonProperty("arcv")]
			public int receive { get; set; }
		}

		[JsonProperty("achv")]
		public List<AchievementData> AchievementList { get; set; }

		[JsonProperty("acg")]
		public int goal { get; set; }

		[JsonProperty("accc")]
		public int completeCount { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CompleteAchievementInfo
	{
		[JsonProperty("acid")]
		public int achievementId { get; set; }

		[JsonProperty("asot")]
		public int sort { get; set; }

		[JsonProperty("gts")]
		public int time { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class PowerPlantPlunderInfo
	{
		[JsonProperty("pRemain")]
		public int remain { get; set; }

		[JsonProperty("target")]
		public UserInformationResponse.Commander target { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ChattingMsgData
	{
		[JsonProperty("data")]
		public string data { get; set; }

		[JsonProperty("rply", NullValueHandling = NullValueHandling.Ignore)]
		public ChattingRecordInfo record { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, Formatting.None);
		}
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class SendChattingInfo
	{
		[JsonProperty("status")]
		public string status { get; set; }

		[JsonProperty("reward")]
		public List<RewardInfo.RewardData> rewardList { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commanderData { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("clst")]
		public List<Dictionary<string, RewardInfo.HaveCostumeInfo>> costumeData { get; set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("equip")]
		public Dictionary<string, Dictionary<int, EquipItemInfo>> equipItem { get; set; }

		[JsonProperty("guit")]
		public Dictionary<string, int> groupItemData { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ChattingRecordInfo
	{
		[JsonProperty("rid")]
		public string id { get; set; }

		[JsonProperty("unm")]
		public string userName { get; set; }

		[JsonProperty("enm")]
		public string enemyName { get; set; }

		[JsonProperty("type")]
		public ERePlayType rePlayType { get; set; }

		[JsonIgnore]
		public bool hasRecord { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ChattingInfo
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class ChattingData
		{
			[JsonIgnore]
			private ChattingMsgData _chatMsgData;

			[JsonProperty("chanel")]
			private string __channel { get; set; }

			public int channel => ParseInt(__channel, 0);

			[JsonProperty("uno")]
			private string __uno { get; set; }

			public int uno => ParseInt(__uno, 0);

			[JsonProperty("svr")]
			public int sendChannel { get; set; }

			[JsonProperty("swld")]
			public int sendWorld { get; set; }

			[JsonProperty("send")]
			public string sendUno { get; set; }

			[JsonProperty("snm")]
			public string nickname { get; set; }

			[JsonProperty("gnm", NullValueHandling = NullValueHandling.Ignore)]
			public string guildName { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("msg")]
			public string message { get; set; }

			[JsonProperty("date")]
			public double date { get; set; }

			[JsonProperty("thmb")]
			public string thumbnail { get; set; }

			public ChattingMsgData chatMsgData
			{
				get
				{
					if (_chatMsgData == null)
					{
						_chatMsgData = JsonConvert.DeserializeObject<ChattingMsgData>(message, Regulation.SerializerSettings);
						if (_chatMsgData.record != null)
						{
							_chatMsgData.record.hasRecord = true;
						}
					}
					return _chatMsgData;
				}
			}
		}

		[JsonProperty("whisper")]
		public List<ChattingData> whisperList { get; set; }

		[JsonProperty("channel")]
		public List<ChattingData> channelList { get; set; }

		[JsonProperty("guild")]
		public List<ChattingData> guildList { get; set; }

		[JsonProperty("time")]
		public int time { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class BlockUser
	{
		[JsonProperty("ch")]
		public int channel { get; set; }

		[JsonProperty("uno")]
		public string uno { get; set; }

		[JsonProperty("nick")]
		public string nickName { get; set; }

		[JsonProperty("thumb")]
		public string thumbnail { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class MailInfo
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class MailData
		{
			[JsonProperty("idx")]
			public int idx { get; set; }

			[JsonProperty("type")]
			public int type { get; set; }

			[JsonProperty("msg")]
			public string message { get; set; }

			[JsonProperty("rmtm")]
			public double remainTime { get; set; }

			[JsonProperty("reward")]
			public List<RewardInfo.RewardData> reward { get; set; }

			[JsonProperty("sts")]
			public string status { get; set; }

			[JsonProperty("recv")]
			private string __receive { get; set; }

			public int receive => ParseInt(__receive, 0);
		}

		[JsonProperty("list")]
		public List<MailData> mailList { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class RewardInfo
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class RewardData
		{
			[JsonProperty("rwdType")]
			public ERewardType rewardType { get; set; }

			[JsonProperty("rwdIdx")]
			public string rewardId { get; set; }

			[JsonProperty("cnt")]
			public int rewardCnt { get; set; }

			[JsonProperty("efct")]
			public int effect { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class CommanderMedal
		{
			[JsonProperty("medl")]
			public int medal { get; private set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class AchievementData
		{
			[JsonProperty("acid")]
			public int achievementId { get; set; }

			[JsonProperty("asot")]
			public int sort { get; set; }

			[JsonProperty("apt")]
			public int point { get; set; }

			[JsonProperty("fin")]
			public int complete { get; set; }

			[JsonProperty("arcv")]
			public int receive { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class HaveCostumeInfo
		{
			[JsonProperty("clst")]
			public List<int> haveCostume { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class ExplorationExp
		{
			[JsonProperty("idx")]
			public int idx { get; set; }

			[JsonProperty("exp")]
			public int exp { get; set; }
		}

		[JsonProperty("reward")]
		public List<RewardData> reward { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commander { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("rank")]
		public Dictionary<string, int> duelScoreData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("clst")]
		public List<Dictionary<string, HaveCostumeInfo>> costumeData { get; set; }

		[JsonProperty("equip")]
		public Dictionary<string, Dictionary<int, EquipItemInfo>> equipItem { get; set; }

		[JsonProperty("guit")]
		public Dictionary<string, int> groupItemData { get; set; }

		[JsonProperty("achv")]
		public AchievementData nextAchievement { get; set; }

		[JsonProperty("uinfo")]
		public UserInformationResponse.BattleStatistics userInfo { get; set; }

		[JsonProperty("achvs")]
		public List<AchievementData> nextAchievementList { get; set; }

		[JsonProperty("comp")]
		public List<int> receiveMissinIdx { get; set; }

		[JsonProperty("compa")]
		public Dictionary<string, int> receiveAchievementIdx { get; set; }

		[JsonProperty("drsoc")]
		public Dormitory.Resource dormitoryResource { get; set; }

		[JsonProperty("deco")]
		public Dictionary<string, int> dormitoryItemNormal { get; set; }

		[JsonProperty("sdeco")]
		public Dictionary<string, int> dormitoryItemAdvanced { get; set; }

		[JsonProperty("wall")]
		public Dictionary<string, int> dormitoryItemWallpaper { get; set; }

		[JsonProperty("bcos")]
		public Dictionary<string, int> dormitoryCostumeBody { get; set; }

		[JsonProperty("hcos")]
		public Dictionary<string, List<string>> dormitoryCostumeHead { get; set; }

		[JsonProperty("weapon")]
		public Dictionary<string, WeaponData> weaponList { get; set; }

		[JsonProperty("gts")]
		public int time { get; set; }

		[JsonProperty("prc")]
		public float inAppPrice { get; set; }

		[JsonProperty("exp")]
		public int exp { get; set; }

		[JsonProperty("exps")]
		public List<ExplorationExp> explorationExp { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class BankInfo
	{
		[JsonProperty("chip")]
		private string __chip { get; set; }

		public int chip => ParseInt(__chip, _localUser.chip);

		[JsonProperty("lev")]
		public int level { get; set; }

		[JsonProperty("luck")]
		public int luck { get; set; }

		[JsonProperty("rfcnt")]
		public int exchangeRateCnt { get; set; }

		[JsonProperty("remain")]
		public int remain { get; set; }

		[JsonProperty("cash")]
		private string __cash { get; set; }

		public int cash => ParseInt(__cash, _localUser.cash);
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ResourceRecharge
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class RechargeData
		{
			[JsonProperty("cnt")]
			public int cnt { get; set; }

			[JsonProperty("remain")]
			public int remain { get; set; }
		}

		[JsonProperty("bult")]
		public RechargeData bulletData;

		[JsonProperty("oil")]
		public RechargeData oilData;

		[JsonProperty("skil")]
		public RechargeData skillData;

		[JsonProperty("chip")]
		public RechargeData chip;

		[JsonProperty("wmat1")]
		public RechargeData weaponMaterialData1;

		[JsonProperty("wmat2")]
		public RechargeData weaponMaterialData2;

		[JsonProperty("wmat3")]
		public RechargeData weaponMaterialData3;

		[JsonProperty("wmat4")]
		public RechargeData weaponMaterialData4;

		[JsonProperty("gacha")]
		public Dictionary<string, GachaInformationResponse> gacha;

		[JsonProperty("world")]
		public int worldState;
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class RecordInfo
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class UserInfo
		{
			[JsonProperty("lnm")]
			public string lName;

			[JsonProperty("llv")]
			public int lLevel;

			[JsonProperty("rnm")]
			public string rName;

			[JsonProperty("rlv")]
			public string rLevel;
		}

		[JsonProperty("uno")]
		public int uno;

		[JsonProperty("rid")]
		public string id;

		[JsonProperty("replayData")]
		public object data;

		[JsonProperty("ws")]
		public int winState;

		[JsonProperty("smvr")]
		public int simulationVer;

		[JsonProperty("rlvr")]
		public double regulationVer;

		[JsonProperty("unm")]
		private string _userName;

		[JsonProperty("thmb")]
		public string thumbnail;

		[JsonProperty("lv")]
		public int level;

		[JsonProperty("rank")]
		public int rank;

		[JsonProperty("date")]
		public double date;

		[JsonProperty("vs")]
		public UserInfo userInfo;

		public string userName => (!isNpc) ? _userName : Localization.Get(_userName);

		[JsonProperty("gnm", NullValueHandling = NullValueHandling.Ignore)]
		public string guildName { get; set; }

		public bool isNpc => rank == 0 || uno >= ConstValue.NpcStartUno;
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class PlunderTimeMachine
	{
		[JsonProperty("reward")]
		public List<List<RewardInfo.RewardData>> reward { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("vshp")]
		public int VipShopOpen { get; set; }

		[JsonProperty("vrtm")]
		public int VipShopRemainTime { get; set; }

		[JsonProperty("guit")]
		public Dictionary<string, int> groupItemData { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class GuildMember
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class MemberData
		{
			[JsonProperty("uno")]
			public int uno { get; set; }

			[JsonProperty("unm")]
			public string name { get; set; }

			[JsonProperty("thumb")]
			public int thumnail { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("time")]
			public double lastTime { get; set; }

			[JsonProperty("mstr")]
			public int memberGrade { get; set; }

			[JsonProperty("dpnt")]
			public int todayPoint { get; set; }

			[JsonProperty("mpnt")]
			public int totalPoint { get; set; }

			[JsonProperty("pbpnt")]
			public int paymentBonusPoint { get; set; }

			[JsonProperty("world")]
			public int world { get; set; }
		}

		[JsonProperty("member")]
		public List<MemberData> memberData { get; set; }

		[JsonProperty("bb")]
		public int badge { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class GuildInfo
	{
		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("gld")]
		public UserInformationResponse.UserGuild guildInfo { get; set; }

		[JsonProperty("member")]
		public List<GuildMember.MemberData> memberData { get; set; }

		[JsonProperty("glist")]
		public List<RoGuild> guildList { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class SecretShop
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class ShopData
		{
			[JsonProperty("id")]
			public int id { get; private set; }

			[JsonProperty("sptp")]
			public ERewardType type { get; set; }

			[JsonProperty("spid")]
			public int idx { get; private set; }

			[JsonProperty("amnt")]
			public int count { get; private set; }

			[JsonProperty("ptyp")]
			public EPriceType costType { get; private set; }

			[JsonProperty("prc")]
			public int cost { get; private set; }

			[JsonProperty("sold")]
			public int sold { get; set; }

			[JsonProperty("rtime")]
			public int time { get; private set; }
		}

		[JsonProperty("shop")]
		public List<ShopData> shopList { get; private set; }

		[JsonProperty("rtime")]
		public int refreshTime { get; private set; }

		[JsonProperty("rcnt")]
		public int refreshCount { get; private set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; private set; }

		[JsonProperty("vrcnt")]
		public int reset { get; private set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ShopReward
	{
		[JsonProperty("shop")]
		public SecretShop.ShopData shop;

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource rsoc;

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commanderData { get; private set; }

		[JsonProperty("equip")]
		public Dictionary<string, Dictionary<int, EquipItemInfo>> equipItem { get; set; }

		[JsonProperty("guit")]
		public Dictionary<string, int> groupItemData { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ItemReward
	{
		[JsonProperty("reward")]
		public SecretShop.ShopData shop;

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource rsoc;

		[JsonProperty("ursc")]
		public List<UserInformationResponse.PartData> ursc;

		[JsonProperty("commMedl")]
		public Dictionary<string, SimpleCommanderInfo> commMedl;
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ScrambleStageInfo
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class UserInfo
		{
			[JsonProperty("unm")]
			public string name { get; private set; }

			[JsonProperty("lv")]
			public int level { get; private set; }

			[JsonProperty("thmb")]
			public int thumb { get; private set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class EnemyCommander
		{
			[JsonObject(MemberSerialization.OptIn)]
			public class Slot
			{
				[JsonProperty("uid")]
				public string unitId { get; set; }

				[JsonProperty("hp")]
				public int unitHp { get; set; }

				[JsonProperty("lv")]
				public int unitLevel { get; set; }
			}

			[JsonObject(MemberSerialization.OptIn)]
			public class GuildSkill
			{
				[JsonProperty("GS_Idx")]
				public string idx { get; set; }

				[JsonProperty("GS_Level")]
				public int level { get; set; }
			}

			[JsonProperty("uno")]
			public string uno { get; set; }

			[JsonProperty("cid")]
			public string id { get; set; }

			[JsonProperty("lv")]
			private string __level { get; set; }

			public int level => ParseInt(__level, 1);

			[JsonProperty("grd")]
			private string __rank { get; set; }

			public int rank => ParseInt(__rank, 1);

			[JsonProperty("cls")]
			private string __cls { get; set; }

			public int cls => ParseInt(__cls, 1);

			[JsonProperty("skv1")]
			private string __skv1 { get; set; }

			public int skv1 => ParseInt(__skv1, 1);

			[JsonProperty("skv2")]
			private string __skv2 { get; set; }

			public int skv2 => ParseInt(__skv2, 1);

			[JsonProperty("skv3")]
			private string __skv3 { get; set; }

			public int skv3 => ParseInt(__skv3, 1);

			[JsonProperty("skv4")]
			private string __skv4 { get; set; }

			public int skv4 => ParseInt(__skv4, 1);

			[JsonProperty("pwr")]
			public int power { get; set; }

			[JsonProperty("dnm")]
			public string troopNickname { get; set; }

			[JsonProperty("uLv")]
			public string userLevel { get; set; }

			[JsonProperty("nm")]
			public string nickname { get; set; }

			[JsonProperty("thumb")]
			public int thumbnail { get; set; }

			[JsonProperty("deck")]
			public object __troopSlotsSource { get; set; }

			[JsonProperty("gsk")]
			public List<GuildSkill> guildSkillList { get; set; }

			public Dictionary<int, Slot> troopSlots
			{
				get
				{
					if (__troopSlotsSource == null)
					{
						return null;
					}
					JArray jArray = null;
					try
					{
						jArray = JArray.FromObject(__troopSlotsSource);
					}
					catch (Exception)
					{
					}
					if (jArray != null)
					{
						return null;
					}
					JObject jObject = JObject.FromObject(__troopSlotsSource);
					return jObject.ToObject<Dictionary<int, Slot>>();
				}
			}
		}

		[JsonProperty("myDeck")]
		public Dictionary<string, UserInformationResponse.Unit> myDeck { get; set; }

		[JsonProperty("enemy")]
		public EnemyCommander enemy { get; set; }

		[JsonProperty("stageInfo")]
		public UserInfo user { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ScrambleRankingData
	{
		[JsonProperty("score")]
		public int score { get; set; }

		[JsonProperty("name")]
		public string name { get; set; }

		[JsonProperty("thmb")]
		public string thmb { get; set; }

		[JsonProperty("lv")]
		public int lv { get; set; }

		[JsonProperty("role")]
		public int role { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class FavorUpData
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class CommanderFavor
		{
		}

		[JsonProperty("todayFavr")]
		public int todayFavorCount { get; set; }

		[JsonProperty("favr")]
		public List<CommanderFavor> commanderFavor { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ServerData
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class ServerInfo
		{
			[JsonProperty("wld")]
			public int idx { get; set; }

			[JsonProperty("stus")]
			public int status { get; set; }

			[JsonProperty("thum")]
			public int thumnail { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("lcdt")]
			public double lastLoginTime { get; set; }
		}

		[JsonProperty("ws")]
		public List<ServerInfo> serverInfoList { get; set; }

		[JsonProperty("new")]
		public int newServer { get; set; }

		[JsonProperty("rsrv")]
		public int recommandServer { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class AlarmData
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class HoldData
		{
			[JsonProperty("cnt")]
			public int count { get; set; }

			[JsonProperty("time")]
			public int time { get; set; }
		}

		[JsonProperty("dcnt")]
		public int dcnt { get; set; }

		[JsonProperty("hold")]
		public HoldData hold { get; set; }

		[JsonProperty("cmdr")]
		public int cmdr { get; set; }

		[JsonProperty("shop")]
		public Dictionary<EShopType, int> shop { get; set; }

		[JsonProperty("expd")]
		public int expd { get; set; }

		[JsonProperty("mwdw")]
		public int mwdw { get; set; }

		[JsonProperty("srgs")]
		public int srgs { get; set; }

		[JsonProperty("srge")]
		public int srge { get; set; }

		[JsonProperty("ocps")]
		public int ocps { get; set; }

		[JsonProperty("ocpe")]
		public int ocpe { get; set; }

		[JsonProperty("raid")]
		public int raid { get; set; }

		[JsonProperty("arena")]
		public int arena { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class SellItemData
	{
		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("reward")]
		public List<RewardInfo.RewardData> rewardList { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("weapon")]
		public Dictionary<string, WeaponData> weaponData { get; set; }

		[JsonProperty("equip")]
		public Dictionary<string, Dictionary<int, EquipItemInfo>> equipItem { get; set; }

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commanderData { get; private set; }

		[JsonProperty("guit")]
		public Dictionary<string, int> groupItemData { get; set; }

		[JsonProperty("drsoc")]
		public Dormitory.Resource dormitoryResource { get; set; }

		[JsonProperty("deco")]
		public Dictionary<string, int> dormitoryItemNormal { get; set; }

		[JsonProperty("sdeco")]
		public Dictionary<string, int> dormitoryItemAdvanced { get; set; }

		[JsonProperty("wall")]
		public Dictionary<string, int> dormitoryItemWallpaper { get; set; }

		[JsonProperty("bcos")]
		public Dictionary<string, int> dormitoryCostumeBody { get; set; }

		[JsonProperty("hcos")]
		public Dictionary<string, List<string>> dormitoryCostumeHead { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CashShopData
	{
		[JsonProperty("ptyp")]
		public ECashRechargePriceType pType { get; private set; }

		[JsonProperty("prc")]
		public string price { get; private set; }

		[JsonProperty("prId")]
		public string priceId { get; private set; }

		[JsonProperty("evcs")]
		public int eventCash { get; private set; }

		[JsonProperty("fscs")]
		public int firstBuyCash { get; private set; }

		[JsonProperty("remain")]
		public double remainTime { get; private set; }

		[JsonProperty("cnt")]
		public int buyCount { get; private set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class NoticeData
	{
		[JsonProperty("idx")]
		public int idx { get; private set; }

		[JsonProperty("img")]
		public string img { get; private set; }

		[JsonProperty("ctnt")]
		public string notice { get; private set; }

		[JsonProperty("link")]
		public string link { get; private set; }

		[JsonProperty("sdt")]
		public double startDate { get; private set; }

		[JsonProperty("edt")]
		public double endDate { get; private set; }

		[JsonProperty("esdt")]
		public double eventStartDate { get; private set; }

		[JsonProperty("eedt")]
		public double eventEndDate { get; private set; }

		[JsonProperty("fixed")]
		public int notiFixed { get; private set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class WorldMapReward
	{
		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commanderData { get; private set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; private set; }
	}

	public class PreDeckInfoList
	{
		public List<UserInformationResponse.PreDeck> list { get; set; }
	}

	public class NavigatorInfo
	{
		public ENavigatorType type;

		public int stageIdx;

		public NavigatorInfo(ENavigatorType type, int position)
		{
			this.type = type;
			stageIdx = position;
		}
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class AnnihilationMapInfo
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class CommanderData
		{
			[JsonProperty("uid")]
			public string id { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("spd")]
			public int speed { get; set; }

			[JsonProperty("hp")]
			public int hp { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class AdvancePartyRewardInfo
		{
			[JsonProperty("reward")]
			public List<List<RewardInfo.RewardData>> rewardList { get; set; }

			[JsonProperty("rsoc")]
			public UserInformationResponse.Resource resource { get; set; }

			[JsonProperty("part")]
			public Dictionary<string, int> partData { get; set; }

			[JsonProperty("medl")]
			public Dictionary<string, int> medalData { get; set; }

			[JsonProperty("ersoc")]
			public Dictionary<string, int> eventResourceData { get; set; }

			[JsonProperty("item")]
			public Dictionary<string, int> itemData { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class StatusData
		{
			[JsonProperty("cid")]
			public string id { get; set; }

			[JsonProperty("dmghp")]
			private string __dmghp { get; set; }

			public int dmghp => ParseInt(__dmghp, 0);

			[JsonProperty("sp")]
			public int sp { get; set; }
		}

		[JsonProperty("mst")]
		public int stage { get; set; }

		[JsonProperty("adpt")]
		public int isPlayAdvanceParty { get; set; }

		[JsonProperty("lcid")]
		public List<string> dieCommanderList { get; set; }

		[JsonProperty("clear")]
		public int clear { get; set; }

		[JsonProperty("remain")]
		public int remainTime { get; set; }

		[JsonProperty("vcid")]
		public List<StatusData> commanderStatusList { get; set; }

		[JsonProperty("enemy")]
		public List<Dictionary<int, CommanderData>> enemyList { get; set; }

		[JsonProperty("rInfo")]
		private object __advancePartyReward { get; set; }

		public AdvancePartyRewardInfo advancePartyReward
		{
			get
			{
				if (__advancePartyReward == null)
				{
					return null;
				}
				JArray jArray = null;
				try
				{
					jArray = JArray.FromObject(__advancePartyReward);
				}
				catch (Exception)
				{
				}
				if (jArray != null)
				{
					return null;
				}
				JObject jObject = JObject.FromObject(__advancePartyReward);
				return jObject.ToObject<AdvancePartyRewardInfo>();
			}
		}

		[JsonProperty("mode")]
		public AnnihilationMode mode { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class BuyVipShop
	{
		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("uifo")]
		public UserInformationResponse.BattleStatistics userInfo { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class VipGacha
	{
		public class VipGachaInfo
		{
			[JsonProperty("rwdType")]
			public int rewardType { get; set; }

			[JsonProperty("rwdIdx")]
			public int rewardIdx { get; set; }

			[JsonProperty("rwdCnt")]
			public int rewardCount { get; set; }

			[JsonProperty("rwdRate")]
			public int rewardRate { get; set; }

			[JsonProperty("rwdPoint")]
			public float rewardPoint { get; set; }
		}

		public class VipGachaResult
		{
			[JsonProperty("type")]
			public int rewardType_result { get; set; }

			[JsonProperty("idx")]
			public int rewardIdx_result { get; set; }

			[JsonProperty("cnt")]
			public int rewardCount_result { get; set; }
		}

		[JsonProperty("list")]
		public Dictionary<string, VipGachaInfo> VipGachaInfoList { get; set; }

		[JsonProperty("cnt")]
		public int gachaCount { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commanderData { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("guit")]
		public Dictionary<string, int> groupItemData { get; set; }

		[JsonProperty("gacha")]
		public List<VipGachaResult> gacharesult { get; set; }

		[JsonProperty("rtm")]
		public int refreshTime { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class DiapatchCommanderInfo
	{
		[JsonProperty("cid")]
		public int cid { get; set; }

		[JsonProperty("rgtm")]
		public int runtime { get; set; }

		[JsonProperty("ecnt")]
		public int engageCnt { get; set; }

		[JsonProperty("egld")]
		public int getGold { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class RecallCommander
	{
		[JsonProperty("rgtm")]
		public int runtime;

		[JsonProperty("egld")]
		public int getGold_time;

		[JsonProperty("exgd")]
		public int getGold_engage;

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class GuildDispatchCommanderList
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class GuildDispatchCommanderInfo
		{
			[JsonProperty("tuno")]
			public int userIdx { get; set; }

			[JsonProperty("tlv")]
			public int userLevel { get; set; }

			[JsonProperty("tunm")]
			public string userName { get; set; }

			[JsonProperty("thmb")]
			public string userThumbnail { get; set; }

			[JsonProperty("tcid")]
			public int cid { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("grd")]
			public int grade { get; set; }

			[JsonProperty("cls")]
			public int cls { get; set; }

			[JsonProperty("skv1")]
			public int skillLv_1 { get; set; }

			[JsonProperty("skv2")]
			public int skillLv_2 { get; set; }

			[JsonProperty("skv3")]
			public int skillLv_3 { get; set; }

			[JsonProperty("skv4")]
			public int skillLv_4 { get; set; }

			[JsonProperty("cos")]
			public int costumeIdx { get; set; }

			[JsonProperty("rfs")]
			public int favorStep { get; set; }

			[JsonProperty("mry")]
			public int marry { get; set; }

			[JsonProperty("tsdc")]
			public List<int> transcendence { get; set; }

			[JsonProperty("empl")]
			public int possibleEngage { get; set; }

			[JsonProperty("exst")]
			public int existEngaged { get; set; }

			[JsonProperty("sp")]
			public int sp { get; set; }

			[JsonProperty("dmghp")]
			public int dmghp { get; set; }

			[JsonProperty("hp")]
			public int hp { get; set; }

			[JsonProperty("equip")]
			public Dictionary<string, int> equipItem { get; set; }

			[JsonProperty("wp")]
			public Dictionary<string, WeaponData> weaponItem { get; set; }
		}

		[JsonProperty("guild")]
		public List<GuildDispatchCommanderInfo> commanderList;

		[JsonProperty("npc")]
		public Dictionary<string, int> npcList;
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CarnivalList
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class CarnivaTime
		{
			public TimeData remainTimeData;

			[JsonProperty("rtm")]
			private string remain
			{
				set
				{
					double byDuration = 0.0;
					if (!string.IsNullOrEmpty(value))
					{
						byDuration = double.Parse(value);
					}
					if (remainTimeData == null)
					{
						remainTimeData = new TimeData();
					}
					remainTimeData.SetByDuration(byDuration);
				}
			}
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class ProcessData
		{
			public TimeData startTimeData;

			public TimeData endTimeData;

			public TimeData remainTimeData;

			[JsonProperty("cnt")]
			public int count { get; set; }

			[JsonProperty("comp")]
			public int complete { get; set; }

			[JsonProperty("recv")]
			public int receive { get; set; }

			[JsonProperty("able")]
			public int able { get; set; }

			[JsonProperty("lup")]
			public string lup { get; set; }

			[JsonProperty("nstm")]
			private string startTime
			{
				set
				{
					double byDuration = 0.0;
					if (!string.IsNullOrEmpty(value))
					{
						byDuration = double.Parse(value);
					}
					if (startTimeData == null)
					{
						startTimeData = new TimeData();
					}
					startTimeData.SetByDuration(byDuration);
				}
			}

			[JsonProperty("netm")]
			private string endTime
			{
				set
				{
					double byDuration = 0.0;
					if (!string.IsNullOrEmpty(value))
					{
						byDuration = double.Parse(value);
					}
					if (endTimeData == null)
					{
						endTimeData = new TimeData();
					}
					endTimeData.SetByDuration(byDuration);
				}
			}

			[JsonProperty("rtm")]
			private string remain
			{
				set
				{
					double byDuration = 0.0;
					if (!string.IsNullOrEmpty(value))
					{
						byDuration = double.Parse(value);
					}
					if (remainTimeData == null)
					{
						remainTimeData = new TimeData();
					}
					remainTimeData.SetByDuration(byDuration);
				}
			}
		}

		[JsonProperty("list")]
		public Dictionary<string, CarnivaTime> carnivalList { get; set; }

		[JsonProperty("ctnt")]
		public Dictionary<string, Dictionary<string, ProcessData>> carnivalProcessList { get; set; }

		[JsonProperty("reward")]
		public List<RewardInfo.RewardData> rewardList { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commanderData { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("clst")]
		public List<Dictionary<string, RewardInfo.HaveCostumeInfo>> costumeData { get; set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("equip")]
		public Dictionary<string, Dictionary<int, EquipItemInfo>> equipItemData { get; set; }

		[JsonProperty("guit")]
		public Dictionary<string, int> groupItemData { get; set; }

		[JsonProperty("ctm")]
		public int connectTime { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ChannelData
	{
		[JsonProperty("openDt")]
		public double openTime { get; set; }

		[JsonProperty("maxLv")]
		public int maxLevel { get; set; }

		[JsonProperty("maxSt")]
		public string maxStage { get; set; }

		[JsonProperty("plcnt")]
		public string commanderCount { get; set; }

		[JsonProperty("svcnt")]
		public string serverCount { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CommanderScenario
	{
		[JsonProperty("qrtr")]
		public List<string> complete { get; set; }

		[JsonProperty("rcvd")]
		public int receive { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CompleteScenario
	{
		[JsonProperty("reward")]
		public List<RewardInfo.RewardData> reward { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commander { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("rank")]
		public Dictionary<string, int> duelScoreData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("clst")]
		public List<Dictionary<string, RewardInfo.HaveCostumeInfo>> costumeData { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class RecieveScenarioReward
	{
		[JsonProperty("reward")]
		public List<RewardInfo.RewardData> reward { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commander { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("rank")]
		public Dictionary<string, int> duelScoreData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("clst")]
		public List<Dictionary<string, RewardInfo.HaveCostumeInfo>> costumeData { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class WaveBattleInfoList
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class WaveBattleInfo
		{
			[JsonProperty("wbid")]
			public int battleIdx { get; set; }

			[JsonProperty("otime")]
			public int openTime { get; set; }

			[JsonProperty("ctime")]
			public int closeTime { get; set; }

			[JsonProperty("clearCnt")]
			public int clearCount { get; set; }

			[JsonProperty("maxCnt")]
			public int maxCount { get; set; }
		}

		[JsonProperty("wavebattle")]
		public List<WaveBattleInfo> InfoList { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class GuildBoardData
	{
		[JsonProperty("idx")]
		public int idx { get; set; }

		[JsonProperty("msg")]
		public string msg { get; set; }

		[JsonProperty("regdt")]
		public double regdt { get; set; }

		[JsonProperty("uno")]
		public int uno { get; set; }

		[JsonProperty("thumb")]
		public string thumb { get; set; }

		[JsonProperty("unm")]
		public string unm { get; set; }

		[JsonProperty("dauth")]
		public int dauth { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ConquestInfo
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class PrevState
		{
			[JsonObject(MemberSerialization.OptIn)]
			public class Point
			{
				[JsonProperty("win")]
				public List<int> win { get; set; }

				[JsonProperty("lose")]
				public List<int> lose { get; set; }
			}

			[JsonProperty("isWin")]
			public int isWin { get; set; }

			[JsonProperty("exdt")]
			public int exdt { get; set; }

			[JsonProperty("point")]
			public Point pointData { get; set; }

			[JsonProperty("users")]
			public Dictionary<string, int> userList { get; set; }

			[JsonProperty("usrpnt")]
			public List<int> standbyList { get; set; }
		}

		[JsonProperty("step")]
		public EConquestState state { get; set; }

		[JsonProperty("remain")]
		public int remain { get; set; }

		[JsonProperty("signed")]
		public int sign { get; set; }

		[JsonProperty("join")]
		public int join { get; set; }

		[JsonProperty("side")]
		public string side { get; set; }

		[JsonProperty("prev")]
		public PrevState prev { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ConquestTroopInfo
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class Enemy
		{
			[JsonProperty("nm")]
			public string name { get; set; }

			[JsonProperty("world")]
			public int world { get; set; }

			[JsonProperty("emblem")]
			public int emblem { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("mcnt")]
			public int mcnt { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class Troop
		{
			[JsonProperty("point")]
			public int point { get; set; }

			[JsonProperty("status")]
			public string status { get; set; }

			[JsonProperty("remain")]
			public int remain { get; set; }

			[JsonProperty("mvtm")]
			public int mvtm { get; set; }

			[JsonProperty("path")]
			public List<int> path { get; set; }

			[JsonProperty("ucash")]
			public int ucash { get; set; }

			[JsonProperty("deck")]
			public Dictionary<string, string> deck { get; set; }

			public TimeData remainData { get; set; }
		}

		[JsonProperty("squard")]
		public Dictionary<int, Troop> squard { get; set; }

		[JsonProperty("slot")]
		public List<int> slot { get; set; }

		[JsonProperty("enemy")]
		public Enemy eGuild { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ConquestStageInfo
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class User
		{
			[JsonProperty("uno")]
			public string uno { get; set; }

			[JsonProperty("name")]
			public string name { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("thumb")]
			public string thumb { get; set; }

			[JsonProperty("auth")]
			public int auth { get; set; }

			[JsonProperty("standby")]
			public int standby { get; set; }

			[JsonProperty("move")]
			public int move { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class EnemyInfo
		{
			[JsonProperty("name")]
			public string name { get; set; }

			[JsonProperty("emblem")]
			public string emblem { get; set; }

			[JsonProperty("gidx")]
			public int gidx { get; set; }
		}

		[JsonProperty("einfo")]
		public EnemyInfo enemyInfo { get; set; }

		[JsonProperty("alie")]
		public List<User> alieList { get; set; }

		[JsonProperty("enemy")]
		public List<User> enemyList { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ConquestStageUser
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class Deck
		{
			[JsonProperty("cid")]
			public string cid { get; set; }

			[JsonProperty("lev")]
			public int level { get; set; }

			[JsonProperty("grade")]
			public int grade { get; set; }

			[JsonProperty("class")]
			public int cls { get; set; }

			[JsonProperty("cos")]
			public int costume { get; set; }

			[JsonProperty("pos")]
			public int position { get; set; }

			[JsonProperty("mry")]
			public int marry { get; set; }

			[JsonProperty("tsdc")]
			public List<int> transcendence { get; set; }
		}

		[JsonProperty("slot")]
		public int slot { get; set; }

		[JsonProperty("stat")]
		public string state { get; set; }

		[JsonProperty("deck")]
		public List<Deck> deck { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class GetRadarData
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class Radar
		{
			[JsonProperty("remain")]
			public int remain { get; set; }

			[JsonProperty("ovtm")]
			public int overTime { get; set; }

			[JsonProperty("stm")]
			public int startTime { get; set; }

			[JsonProperty("unm")]
			public string uName { get; set; }

			[JsonProperty("info")]
			public Dictionary<int, User> info { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class User
		{
			[JsonProperty("alie")]
			public Info alie { get; set; }

			[JsonProperty("enemy")]
			public Info enemy { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class Info
		{
			[JsonProperty("move")]
			public int move { get; set; }

			[JsonProperty("stand")]
			public int stand { get; set; }
		}

		[JsonProperty("Radar")]
		public Radar radar { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class GetConquestBattle
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class Entry
		{
			[JsonProperty("red")]
			public List<EntryInfo> red { get; set; }

			[JsonProperty("blue")]
			public List<EntryInfo> blue { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class EntryInfo
		{
			[JsonProperty("uno")]
			public string uno { get; set; }

			[JsonProperty("name")]
			public string name { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("thumb")]
			public string thumb { get; set; }

			[JsonProperty("deck")]
			public List<ConquestStageUser.Deck> deck { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class Battle
		{
			[JsonProperty("entry")]
			public BattleEntry entry { get; set; }

			[JsonProperty("result")]
			public Result result { get; set; }

			[JsonProperty("rid")]
			public string replayId { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class BattleEntry
		{
			[JsonProperty("red")]
			public Dictionary<string, BattleEntryInfo> red { get; set; }

			[JsonProperty("blue")]
			public Dictionary<string, BattleEntryInfo> blue { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class BattleEntryInfo
		{
			[JsonProperty("pos")]
			public int pos { get; set; }

			[JsonProperty("hp")]
			public int hp { get; set; }

			[JsonProperty("maxHp")]
			public int maxHp { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class Result
		{
			[JsonProperty("red")]
			public Dictionary<string, ResultInfo> red { get; set; }

			[JsonProperty("blue")]
			public Dictionary<string, ResultInfo> blue { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class ResultInfo
		{
			[JsonProperty("hp")]
			public int hp { get; set; }
		}

		[JsonProperty("entry")]
		public Entry entry { get; set; }

		[JsonProperty("battle")]
		public List<Battle> battle { get; set; }

		[JsonProperty("world")]
		public int enemyWorld { get; set; }

		[JsonProperty("egnm")]
		public string enemyName { get; set; }

		[JsonProperty("eside")]
		public string eSide { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class GuildRankingInfo
	{
		[JsonProperty("idx")]
		public int idx { get; set; }

		[JsonProperty("world")]
		public int world { get; set; }

		[JsonProperty("rnk")]
		public int rank { get; set; }

		[JsonProperty("gnm")]
		public string guildName { get; set; }

		[JsonProperty("lev")]
		public int level { get; set; }

		[JsonProperty("apt")]
		public int point { get; set; }

		[JsonProperty("scr")]
		public int score { get; set; }

		[JsonProperty("emb")]
		public int emblem { get; set; }

		[JsonProperty("cnt")]
		public int count { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class MoveConquestTroop
	{
		[JsonProperty("path")]
		public List<int> path { get; set; }

		[JsonProperty("distance")]
		public int distance { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource rsoc { get; set; }

		[JsonProperty("ucash")]
		public int ucash { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class EquipItemInfo
	{
		[JsonProperty("total")]
		public int totalCount { get; set; }

		[JsonProperty("avail")]
		public int availableCount { get; set; }

		[JsonProperty("list")]
		public List<int> equipCommanderList { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ExplorationData
	{
		[JsonProperty("idx")]
		public int idx { get; set; }

		private long _timeTick { get; set; }

		private double _remainTime { get; set; }

		[JsonProperty("rmtm")]
		public double remainTime
		{
			get
			{
				double num = _remainTime - elapsedTime;
				return (!(num >= 0.0)) ? 0.0 : num;
			}
			set
			{
				_timeTick = DateTime.Now.Ticks;
				_remainTime = value;
			}
		}

		private double elapsedTime => TimeSpan.FromTicks(DateTime.Now.Ticks - _timeTick).TotalSeconds;

		[JsonProperty("cid")]
		public List<string> cids { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ExplorationStartInfo
	{
		[JsonProperty("idx")]
		public int idx;

		[JsonProperty("cid")]
		public List<string> cids;
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CooperateInfo
	{
		[JsonProperty("stage")]
		public int stage { get; set; }

		[JsonProperty("step")]
		public int step { get; set; }

		[JsonProperty("remain")]
		public int remain { get; set; }

		[JsonProperty("dmg")]
		public ulong dmg { get; set; }

		[JsonProperty("ticket")]
		public int ticket { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CooperateReceiveInfo
	{
		[JsonProperty("stage")]
		public int stage { get; set; }

		[JsonProperty("step")]
		public int step { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CooperateBattleData
	{
		[JsonProperty("coop")]
		public CooperateInfo coop { get; set; }

		[JsonProperty("recv")]
		public CooperateReceiveInfo recv { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CooperateBattleRewardInfo : RewardInfo
	{
		[JsonProperty("coop")]
		public CooperateInfo coop { get; set; }

		[JsonProperty("recv")]
		public CooperateReceiveInfo recv { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CooperateBattlePointGuildRankingInfo
	{
		[JsonProperty("rank")]
		public int rank { get; set; }

		[JsonProperty("gIdx")]
		public int gIdx { get; set; }

		[JsonProperty("name")]
		public string name { get; set; }

		[JsonProperty("lv")]
		public int lv { get; set; }

		[JsonProperty("eblm")]
		public int eblm { get; set; }

		[JsonProperty("server")]
		public int server { get; set; }

		[JsonProperty("stage")]
		public int stage { get; set; }

		[JsonProperty("step")]
		public int step { get; set; }

		[JsonProperty("accDmg")]
		public ulong accDmg { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CooperateBattlePointUserRankingInfo
	{
		[JsonProperty("rank")]
		public int rank { get; set; }

		[JsonProperty("uno")]
		public string uno { get; set; }

		[JsonProperty("name")]
		public string name { get; set; }

		[JsonProperty("thumb")]
		public int thumb { get; set; }

		[JsonProperty("accDmg")]
		public ulong accDmg { get; set; }

		[JsonProperty("lv")]
		public int lv { get; set; }

		[JsonProperty("world")]
		public int world { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class RotationBanner
	{
		public class BannerList
		{
			[JsonProperty("imgUrl")]
			public string ImgUrl { get; set; }

			[JsonProperty("linkType")]
			public BannerListType linkType { get; set; }

			[JsonProperty("linkIdx")]
			public int linkIdx { get; set; }

			[JsonProperty("eidx")]
			public int eventIdx { get; set; }

			[JsonProperty("sdate")]
			public string startDate { get; set; }

			[JsonProperty("edate")]
			public string endDate { get; set; }
		}

		[JsonProperty("list")]
		public List<BannerList> bannerList;

		[JsonProperty("rt")]
		public int roataionTime { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class EventBattleInfo
	{
		[JsonProperty("eidx")]
		public string idx { get; set; }

		[JsonProperty("remain")]
		public double remain { get; set; }

		[JsonProperty("rcnt")]
		public int rewardCount { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class EventBattleData
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class EventData
		{
			[JsonProperty("efid")]
			public string efid { get; set; }

			[JsonProperty("esid")]
			public string esid { get; set; }

			[JsonProperty("remain")]
			public double remain { get; set; }

			[JsonProperty("type")]
			public int type { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class RaidData
		{
			[JsonProperty("remain")]
			public double remain { get; set; }
		}

		[JsonProperty("evt")]
		public EventData eventData { get; set; }

		[JsonProperty("boss")]
		public RaidData raidData { get; set; }

		[JsonProperty("bossCnt")]
		public int bossCnt { get; set; }

		[JsonProperty("rcnt")]
		public int rewardCnt { get; set; }

		[JsonProperty("rcntAll")]
		public int rewardCntAll { get; set; }

		[JsonProperty("map")]
		public Dictionary<int, int> clearList { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class EventBattleGachaInfo
	{
		[JsonProperty("season")]
		public int season { get; set; }

		[JsonProperty("reset")]
		public int reset { get; set; }

		[JsonProperty("info")]
		public Dictionary<int, int> info { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class EventBattleGachaOpen
	{
		[JsonProperty("reward")]
		public List<RewardInfo.RewardData> rewardList { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commanderData { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("clst")]
		public List<Dictionary<string, RewardInfo.HaveCostumeInfo>> costumeData { get; set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("equip")]
		public Dictionary<string, Dictionary<int, EquipItemInfo>> equipItem { get; set; }

		[JsonProperty("season")]
		public int season { get; set; }

		[JsonProperty("reset")]
		public int reset { get; set; }

		[JsonProperty("info")]
		public Dictionary<int, int> info { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class EventRaidData
	{
		[JsonProperty("mbid")]
		public string bossId { get; set; }

		[JsonProperty("eidx")]
		public string eIdx { get; set; }

		[JsonProperty("enmy")]
		public string enemy { get; set; }

		[JsonProperty("lv")]
		public int level { get; set; }

		[JsonProperty("hp")]
		public int hp { get; set; }

		[JsonProperty("dmg")]
		public int damage { get; set; }

		[JsonProperty("remain")]
		public int remain { get; set; }

		[JsonProperty("name")]
		public string userName { get; set; }

		[JsonProperty("attendCount")]
		public int attendCount { get; set; }

		[JsonProperty("isown")]
		public int isOwn { get; set; }

		[JsonProperty("isshare")]
		public int isShare { get; set; }

		[JsonProperty("isclr")]
		public int clear { get; set; }

		[JsonProperty("recvRwd")]
		public int receive { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class EventRaidRankingData
	{
		[JsonProperty("uname")]
		public string userName { get; set; }

		[JsonProperty("thumb")]
		public string userThumb { get; set; }

		[JsonProperty("level")]
		public string level { get; set; }

		[JsonProperty("accdmg")]
		public int damage { get; set; }

		[JsonProperty("authority")]
		public int authority { get; set; }

		[JsonProperty("isown")]
		public int isown { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class EventRaidList
	{
		[JsonProperty("rcnt")]
		public int rewardCount { get; set; }

		[JsonProperty("list")]
		public List<EventRaidData> bossList { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class EventRaidReward
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class EventRaidRewardData
		{
			[JsonProperty("attend")]
			public int attend { get; set; }

			[JsonProperty("own")]
			public int own { get; set; }

			[JsonProperty("mvp")]
			public int mvp { get; set; }

			[JsonProperty("mbids")]
			public List<string> mbids { get; set; }
		}

		[JsonProperty("reward")]
		public List<RewardInfo.RewardData> rewardList { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commanderData { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("clst")]
		public List<Dictionary<string, RewardInfo.HaveCostumeInfo>> costumeData { get; set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("equip")]
		public Dictionary<string, Dictionary<int, EquipItemInfo>> equipItem { get; set; }

		[JsonProperty("rwdCnt")]
		public EventRaidRewardData rewardCount { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class RankingReward
	{
		[JsonProperty("reward")]
		public List<RewardInfo.RewardData> rewardList { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commanderData { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("clst")]
		public List<Dictionary<string, RewardInfo.HaveCostumeInfo>> costumeData { get; set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("equip")]
		public Dictionary<string, Dictionary<int, EquipItemInfo>> equipItem { get; set; }

		[JsonProperty("rcvd")]
		public List<int> receiveIdx { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class WorldDuelInformation
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class UserData
		{
			[JsonProperty("wld")]
			public int world { get; set; }

			[JsonProperty("unm")]
			public string userName { get; set; }

			[JsonProperty("gld")]
			public string guildName { get; set; }

			[JsonProperty("gwld")]
			public int guildWorld { get; set; }

			[JsonProperty("score")]
			public int score { get; set; }

			[JsonProperty("win")]
			public int win { get; set; }

			[JsonProperty("lose")]
			public int lose { get; set; }

			[JsonProperty("thmb")]
			public string thmb { get; set; }
		}

		[JsonProperty("itrm")]
		public double resetTime { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("deck")]
		public Dictionary<string, string> deck { get; set; }

		[JsonProperty("buff")]
		public Dictionary<string, int> duelBuff { get; set; }

		[JsonProperty("bbf")]
		public List<int> activeBuff { get; set; }

		[JsonProperty("rank")]
		public RankingUserData user { get; set; }

		[JsonProperty("trank")]
		public List<PvPRankingList.RankData> rankingList { get; set; }

		[JsonProperty("open")]
		private string _open { get; set; }

		[JsonProperty("lsinfo")]
		public UserData bestRank { get; set; }

		[JsonProperty("retryinfo")]
		public PvPDuelList.PvPDuelData retryInfo { get; set; }

		public bool open
		{
			get
			{
				if (string.IsNullOrEmpty(_open))
				{
					return false;
				}
				return _open == "true" || _open == "True";
			}
		}
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class FirstPaymentRewardInfo
	{
		[JsonProperty("reward")]
		public List<RewardInfo.RewardData> rewardList { get; set; }

		[JsonProperty("rsoc")]
		public UserInformationResponse.Resource resource { get; set; }

		[JsonProperty("comm")]
		public Dictionary<string, UserInformationResponse.Commander> commanderData { get; set; }

		[JsonProperty("part")]
		public Dictionary<string, int> partData { get; set; }

		[JsonProperty("medl")]
		public Dictionary<string, int> medalData { get; set; }

		[JsonProperty("ersoc")]
		public Dictionary<string, int> eventResourceData { get; set; }

		[JsonProperty("item")]
		public Dictionary<string, int> itemData { get; set; }

		[JsonProperty("clst")]
		public List<Dictionary<string, RewardInfo.HaveCostumeInfo>> costumeData { get; set; }

		[JsonProperty("food")]
		public Dictionary<string, int> foodData { get; set; }

		[JsonProperty("equip")]
		public Dictionary<string, Dictionary<int, EquipItemInfo>> equipItemData { get; set; }

		[JsonProperty("guit")]
		public Dictionary<string, int> groupItemData { get; set; }

		[JsonProperty("uifo")]
		public UserInformationResponse.BattleStatistics userInfo { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class WeaponData
	{
		[JsonProperty("wid")]
		public string id { get; set; }

		[JsonProperty("wlv")]
		public int level { get; set; }

		[JsonProperty("cid")]
		public int cid { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class WeaponProgressSlotData
	{
		[JsonProperty("slot")]
		public int slot { get; set; }

		[JsonProperty("remain")]
		public int remain { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class InfinityTowerData
	{
		[JsonProperty("pifid")]
		public string curField { get; set; }

		[JsonProperty("field")]
		public Dictionary<string, Dictionary<int, int>> fieldData { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class InfinityTowerInformation
	{
		[JsonIgnore]
		public string retryStage;

		[JsonProperty("tinfo")]
		public InfinityTowerData infinityData { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class InfinityTowerReward : RewardInfo
	{
		[JsonProperty("tinfo")]
		public Dictionary<string, Dictionary<int, int>> fieldData { get; set; }
	}

	public class Dormitory
	{
		[JsonObject(MemberSerialization.OptIn)]
		public class Resource
		{
			[JsonProperty("drpt")]
			private string __dormitoryPoint { get; set; }

			public int dormitoryPoint => ParseInt(__dormitoryPoint, _localUser.dormitoryPoint);

			[JsonProperty("wood")]
			private string __wood { get; set; }

			public int wood => ParseInt(__wood, _localUser.wood);

			[JsonProperty("ston")]
			private string __ston { get; set; }

			public int ston => ParseInt(__ston, _localUser.ston);

			[JsonProperty("elec")]
			private string __elec { get; set; }

			public int elec => ParseInt(__elec, _localUser.elec);
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class InventoryData
		{
			[JsonProperty("deco")]
			public Dictionary<string, int> itemNormal { get; set; }

			[JsonProperty("sdeco")]
			public Dictionary<string, int> itemAdvanced { get; set; }

			[JsonProperty("wall")]
			public Dictionary<string, int> itemWallpaper { get; set; }

			[JsonProperty("bcos")]
			public Dictionary<string, int> costumeBody { get; set; }

			[JsonProperty("hcos")]
			public Dictionary<string, List<string>> costumeHead { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class Info : InventoryData
		{
			[JsonProperty("drsoc")]
			public Resource resource { get; set; }

			[JsonProperty("duifo")]
			public Dictionary<string, int> info { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class FloorCommanderInfo
		{
			[JsonProperty("cid")]
			public string id { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("grd")]
			public int grade { get; set; }

			[JsonProperty("cls")]
			public int cls { get; set; }

			[JsonProperty("cos")]
			public int costume { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class RoomInfo
		{
			private DormitoryUpgradeDataRow _upgradeInfo;

			[JsonProperty("fno")]
			public string fno { get; set; }

			[JsonProperty("fnm")]
			public string name { get; set; }

			[JsonProperty("fst")]
			public string state { get; set; }

			[JsonProperty("cids")]
			public List<string> commanders { get; set; }

			[JsonProperty("rtm")]
			public double remain { get; set; }

			[JsonProperty("fcom")]
			public List<FloorCommanderInfo> commanderInfos { get; set; }

			public DormitoryUpgradeDataRow upgradeInfo
			{
				get
				{
					if (_upgradeInfo != null)
					{
						return _upgradeInfo;
					}
					_upgradeInfo = RemoteObjectManager.instance.regulation.dormitoryUpgradeDtbl[fno];
					return _upgradeInfo;
				}
			}
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class FloorInfo
		{
			[JsonProperty("ptst")]
			public bool pointState { get; set; }

			[JsonProperty("fInfo")]
			public Dictionary<string, RoomInfo> floors { get; set; }

			[JsonIgnore]
			public bool isMasterUser { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class GetUserFloorInfoResponse : FloorInfo
		{
			[JsonProperty("tuno")]
			public string uno { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class ConstructFloorResponse
		{
			[JsonProperty("drsoc")]
			public Resource resource { get; set; }

			[JsonProperty("fInfo")]
			public Dictionary<string, RoomInfo> floors { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class FinishConstructFloorResponse
		{
			[JsonProperty("rsoc")]
			public UserInformationResponse.Resource resource { get; set; }

			[JsonProperty("fInfo")]
			public Dictionary<string, RoomInfo> floors { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class FloorDecoInfo
		{
			[JsonProperty("idx")]
			public string id { get; set; }

			[JsonProperty("px")]
			public int px { get; set; }

			[JsonProperty("py")]
			public int py { get; set; }

			[JsonProperty("rt")]
			public int rotation { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class FloorCharacterInfo
		{
			[JsonProperty("cid")]
			public string id { get; set; }

			[JsonProperty("fno")]
			public string fno { get; set; }

			[JsonProperty("bcos")]
			public string bodyId { get; set; }

			[JsonProperty("hcos")]
			public string headId { get; set; }

			[JsonProperty("rtm")]
			public double remain { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class FloorDetailInfo
		{
			[JsonProperty("fno")]
			public string fno { get; set; }

			[JsonProperty("fnm")]
			public string name { get; set; }

			[JsonProperty("fwp")]
			public string wallpaperId { get; set; }

			[JsonProperty("fdc")]
			public List<FloorDecoInfo> decos { get; set; }

			[JsonProperty("fcm")]
			public Dictionary<string, FloorCharacterInfo> characters { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class GetUserFloorDetailInfoResponse : FloorDetailInfo
		{
			[JsonProperty("tuno")]
			public string uno { get; set; }

			[JsonProperty("favor")]
			public bool favorState { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class ChangeDormitoryFloorNameResponse
		{
			[JsonProperty("rsoc")]
			public UserInformationResponse.Resource resource { get; set; }

			[JsonProperty("fnm")]
			public string name { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class ShopProductItemInfo
		{
			private GoodsDataRow _goodsDr;

			[JsonProperty("sidx")]
			public string id { get; set; }

			[JsonProperty("amnt")]
			public int amount { get; set; }

			[JsonProperty("sort")]
			public int sort { get; set; }

			[JsonProperty("gidx")]
			public string goodsId { get; set; }

			[JsonProperty("prc")]
			public int cost { get; set; }

			[JsonProperty("stm")]
			public double startRemain { get; set; }

			[JsonProperty("etm")]
			public double endRemain { get; set; }

			[JsonProperty("pcnt")]
			public int buyCount { get; set; }

			[JsonProperty("lcnt")]
			public int buyLimit { get; set; }

			public GoodsDataRow goodsDr
			{
				get
				{
					if (_goodsDr == null)
					{
						_goodsDr = RemoteObjectManager.instance.regulation.goodsDtbl[goodsId];
					}
					return _goodsDr;
				}
			}
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class ShopInfo
		{
			[JsonProperty("dshop")]
			public Dictionary<EDormitoryItemType, List<ShopProductItemInfo>> items;
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class BuyShopProductResponse : RewardInfo
		{
			[JsonProperty("dshop")]
			public Dictionary<EDormitoryItemType, ShopProductItemInfo> items { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class ChangeWallpaperResponse
		{
			[JsonProperty("fwp")]
			public string id { get; set; }

			[JsonProperty("wall")]
			public Dictionary<string, int> invenWallpaper { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class ArrangeDecorationResponse
		{
			[JsonProperty("deco")]
			public Dictionary<string, int> invenNormal { get; set; }

			[JsonProperty("sdeco")]
			public Dictionary<string, int> invenAdvanced { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class GetDormitoryCommanderInfoResponse
		{
			[JsonProperty("dcom")]
			public Dictionary<string, CommanderInfo> commanderData { get; set; }

			[JsonProperty("hcos")]
			public Dictionary<string, List<string>> headData { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class CommanderInfo
		{
			[JsonProperty("cid")]
			public string id { get; set; }

			[JsonProperty("fno")]
			public string fno { get; set; }

			[JsonProperty("rtm")]
			public double reamin { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class CommanderHeadData
		{
			[JsonProperty("hcos")]
			public string headId { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class CommanderBodyData
		{
			[JsonProperty("bcos")]
			public string bodyId { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class CommanderRaminData
		{
			[JsonProperty("rtm")]
			public double remain { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class ChangeCommanderHeadResponse
		{
			[JsonProperty("fcm")]
			public Dictionary<string, CommanderHeadData> headData { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class ChangeCommanderBodyResponse
		{
			[JsonProperty("bcos")]
			public Dictionary<string, int> invenBody { get; set; }

			[JsonProperty("fcm")]
			public Dictionary<string, CommanderBodyData> bodyData { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class GetPointResponse
		{
			[JsonProperty("reward")]
			public List<RewardInfo.RewardData> reward { get; set; }

			[JsonProperty("drsoc")]
			public Resource resource { get; set; }

			[JsonProperty("fcm")]
			public Dictionary<string, CommanderRaminData> reaminData { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class GetPointAllResponse : GetPointResponse
		{
			[JsonProperty("ptst")]
			public bool pointState { get; set; }
		}

		[JsonObject(MemberSerialization.OptIn)]
		public class SearchUserInfo
		{
			[JsonProperty("uno")]
			public string uno { get; set; }

			[JsonProperty("wld")]
			public int world { get; set; }

			[JsonProperty("unm")]
			public string name { get; set; }

			[JsonProperty("thumb")]
			public string thumbnail { get; set; }

			[JsonProperty("lv")]
			public int level { get; set; }

			[JsonProperty("time")]
			public double lastTime { get; set; }
		}
	}

	private static Dictionary<UserInformationType, string> _userInfoPair = new Dictionary<UserInformationType, string>
	{
		{
			UserInformationType.Resource,
			"rsoc"
		},
		{
			UserInformationType.Upgrade,
			"uifo"
		},
		{
			UserInformationType.Unit,
			"unit"
		},
		{
			UserInformationType.Building,
			"bld"
		},
		{
			UserInformationType.Commander,
			"comm"
		},
		{
			UserInformationType.DailyCheckPoint,
			"dcp"
		},
		{
			UserInformationType.Recharge,
			"rchg"
		},
		{
			UserInformationType.Tutorial,
			"ttrl"
		},
		{
			UserInformationType.part,
			"part"
		},
		{
			UserInformationType.DormitoryResource,
			"drsoc"
		},
		{
			UserInformationType.DormitoryInfo,
			"duifo"
		},
		{
			UserInformationType.DormitoryInvenNormalDeco,
			"deco"
		},
		{
			UserInformationType.DormitoryInvenAdvancedDeco,
			"sdeco"
		},
		{
			UserInformationType.DormitoryInvenWallpaperDeco,
			"wall"
		},
		{
			UserInformationType.DormitoryInvenCostumeBody,
			"bcos"
		},
		{
			UserInformationType.DormitoryInvenCostumeHead,
			"hcos"
		}
	};

	private static RoLocalUser _localUser => RemoteObjectManager.instance.localUser;

	public static List<string> GetUserInfoTargetList(params UserInformationType[] targetList)
	{
		List<string> list = new List<string>();
		if (targetList != null)
		{
			for (int i = 0; i < targetList.Length; i++)
			{
				list.Add(_userInfoPair[targetList[i]]);
			}
		}
		return list;
	}

	private static int ParseInt(string source, int defaultValue)
	{
		int result = 0;
		if (string.IsNullOrEmpty(source) || !int.TryParse(source, out result))
		{
			return defaultValue;
		}
		return result;
	}

	private static int ParseGold(string source, int defaultValue)
	{
		int result = defaultValue;
		if (string.IsNullOrEmpty(source))
		{
			return result;
		}
		if (!int.TryParse(source, out result))
		{
			return int.MaxValue;
		}
		return result;
	}
}
