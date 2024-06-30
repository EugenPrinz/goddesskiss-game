using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Facebook.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoomDecorator;
using RoomDecorator.Data;
using RoomDecorator.Event;
using RoomDecorator.UI;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;
using Util;

public class RemoteObjectManager : MonoBehaviour
{
	private class NetworkTestData
	{
		public string methodName;

		public object[] paramList;

		public static NetworkTestData Create(string name, params object[] paramList)
		{
			NetworkTestData networkTestData = new NetworkTestData();
			networkTestData.methodName = name;
			networkTestData.paramList = paramList;
			return networkTestData;
		}
	}

	private static bool appQuit;

	private static RemoteObjectManager _instance;

	private DateTime _baseTime = new DateTime(1970, 1, 1, 0, 0, 0);

	private Dictionary<string, bool> _testProtocolDict = new Dictionary<string, bool>();

	private string RegistrationId = string.Empty;

	public const string ServerUrl = "http://gk.flerogames.com/checkData.php";

	public const string ServerChattingUrl = "http://gkchat.flerogames.com/talk/server.php";

	public const string MarketUrl = "https://play.google.com/store/apps/details?id=com.flerogames.GK";

	private string _GameServerUrl = "http://gk.flerogames.com/checkData.php";

	private string _ChattingServerUrl = "http://gkchat.flerogames.com/talk/server.php";

	private string tempNickName = string.Empty;

	private readonly 

	public static RemoteObjectManager instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			appQuit = false;
			RemoteObjectManager[] array = UnityEngine.Object.FindObjectsOfType<RemoteObjectManager>();
			RemoteObjectManager[] array2 = array;
			foreach (RemoteObjectManager remoteObjectManager in array2)
			{
				UnityEngine.Object.DestroyImmediate(remoteObjectManager.gameObject);
			}
			GameObject gameObject = new GameObject();
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			if (Application.isPlaying)
			{
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			_instance = gameObject.AddComponent<RemoteObjectManager>();
			JsonRpcClient.instance.RegisterClass(_instance.GetType());
			return _instance;
		}
	}

	public double loginTime { get; private set; }

	public bool bChangeFullMember { get; set; }

	public bool bLogin { get; set; }

	public Regulation regulation { get; set; }

	public RoLocalUser localUser { get; private set; }

	public List<RoUser> duelRankingList { get; private set; }

	public List<RoUser> raidRankingList { get; private set; }

	public bool waitingScenarioComplete { get; set; }

	public static RoStatistics statistics
	{
		get
		{
			if (instance == null || instance.localUser == null)
			{
				return null;
			}
			return instance.localUser.statistics;
		}
	}

	public static string KEY => "OZdg";

	public string lastJsonRpcRequestError { get; private set; }

	public string GameServerUrl
	{
		get
		{
			return _GameServerUrl;
		}
		set
		{
			_GameServerUrl = value;
		}
	}

	public string ChattingServerUrl
	{
		get
		{
			return _ChattingServerUrl;
		}
		set
		{
			_ChattingServerUrl = value;
		}
	}

	private void OnApplicationQuit()
	{
		if (!appQuit)
		{
			appQuit = true;
			RequestLogout();
		}
	}

	public double GetCurrentTime()
	{
		return (DateTime.UtcNow - _baseTime).TotalSeconds;
	}

	public TimeSpan GetTime()
	{
		return DateTime.UtcNow - _baseTime;
	}

	private void Start()
	{
	}

	private void _TestUnit()
	{
		regulation.unitDtbl.ForEach(delegate(UnitDataRow unit)
		{
			if (!unit.isCommander)
			{
				localUser.unitList.Add(RoUnit.Create(unit.key, 1, 1, 1, 0, "0", 0, 0, new List<int>()));
			}
		});
	}

	private void _TestRewardList()
	{
		localUser.missionList.AddRange(JsonConvert.DeserializeObject<JsonTable<RoMission>>(Regulation.RegulationFile["missionTable"], new JsonConverter[1]
		{
			new JsonTable<RoMission>.Converter()
		}));
		localUser.achievementList.AddRange(JsonConvert.DeserializeObject<JsonTable<RoMission>>(Regulation.RegulationFile["achievementTable"], new JsonConverter[1]
		{
			new JsonTable<RoMission>.Converter()
		}));
		int num = 0;
		List<RoReward> rewardList = localUser.GetRewardList(EReward.Mail);
		for (int i = 0; i < rewardList.Count; i++)
		{
			if (rewardList[i].received)
			{
				num++;
			}
		}
		localUser.newMailCount = num;
	}

	private void _TestCommanderRecruitList()
	{
		localUser.recruit.Clear();
		List<string> list = CreateRandomCommanderIdList(8);
		list.ForEach(delegate(string commanderId)
		{
			localUser.recruit.entryList.Add(RoRecruit.Entry.Create(commanderId, 1, 1, 1, 0, 0, 0, new List<int>()));
		});
		localUser.recruit.refreshTime.SetByDuration(RoRecruit.RefreshInterval);
	}

	public List<string> CreateRandomCommanderIdList(int count, bool isNpc = false, EBranch branch = EBranch.Undefined)
	{
		List<string> commanderIdList = regulation.GetCommanderIdList(branch, isNpc);
		if (commanderIdList.Count <= count)
		{
			return commanderIdList;
		}
		List<string> list = new List<string>();
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < count; i++)
		{
			int num = UnityEngine.Random.Range(0, commanderIdList.Count);
			if (hashSet.Contains(num))
			{
				i--;
				continue;
			}
			hashSet.Add(num);
			string item = commanderIdList[num];
			list.Add(item);
		}
		return list;
	}

	private void _TestTroop()
	{
	}

	public void DuelRankingList()
	{
		duelRankingList = new List<RoUser>();
		RequestPvPDuelList();
	}

	public void WaveDuelRankingList()
	{
		duelRankingList = new List<RoUser>();
		RequestPvPWaveDuelList();
	}

	public void RaidRankingList()
	{
		raidRankingList = new List<RoUser>();
		RequestGetRaidRankList();
	}

	public void _TestDuelTarget()
	{
	}

	public void _TestWorldMap()
	{
		localUser.worldMapList.Clear();
		localUser.worldMapList = RoWorldMap.CreateAll();
	}

	private void _MakeTestDict()
	{
		_testProtocolDict.Clear();
		Type type = GetType();
		MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		Type typeFromHandle = typeof(JsonRpcClient.RequestAttribute);
		foreach (MethodInfo methodInfo in methods)
		{
			object[] customAttributes = methodInfo.GetCustomAttributes(typeFromHandle, inherit: false);
			if (customAttributes.Length != 0)
			{
				JsonRpcClient.RequestAttribute requestAttribute = (JsonRpcClient.RequestAttribute)customAttributes[0];
				if (requestAttribute.url == "http://gk.flerogames.com/checkData.php")
				{
					_testProtocolDict.Add(methodInfo.Name, value: false);
				}
			}
		}
	}

	private void _CheckReceiveTestData(string key)
	{
		if (_testProtocolDict.ContainsKey(key))
		{
			_testProtocolDict[key] = true;
		}
	}

	private int _ConvertStringToInt(string id)
	{
		int result = 0;
		if (!int.TryParse(id, out result))
		{
			result = -1;
		}
		return result;
	}

	public void RequestLogin()
	{
		if (localUser == null)
		{
			localUser = RoLocalUser.CreateLocalUser();
		}
		RegistrationId = SA_Singleton<GoogleCloudMessageService>.instance.registrationId;
		string text = SystemInfo.operatingSystem;
		int num = text.IndexOf("/");
		if (num > 0)
		{
			text = text.Substring(0, num);
		}
		else if (text.Length > 40)
		{
			text = text.Substring(0, 40);
		}
		JsonRpcClient.instance.SendRequest(this, "Login", JObject.FromObject(new Protocols.AuthLoginRequest
		{
			memberId = localUser.mIdx,
			token = localUser.tokn,
			world = localUser.world,
			platform = localUser.platform,
			deviceName = SystemInfo.deviceModel,
			deviceId = SystemInfo.deviceUniqueIdentifier,
			patchType = 1,
			osCode = Protocols.OSCode.Android,
			osVersion = text,
			gameVersion = Application.version,
			apkFileName = "notyet",
			pushRegistrationId = RegistrationId,
			languageCode = GetLanguageCode(),
			countryCode = localUser.localeCountryCode,
			largoId = GetUUID(),
			channel = localUser.channel
		}));
	}

	public string GetLanguageCode()
	{
		return Localization.language switch
		{
			"S_Kr" => "ko", 
			"S_En" => "en", 
			"S_Jp" => "jp", 
			"S_Beon" => "tw", 
			"S_Gan" => "zh", 
			"S_Deu" => "de", 
			"S_Esp" => "es", 
			"S_Idn" => "in", 
			"S_Tha" => "th", 
			"S_Fr" => "fr", 
			"S_Rus" => "ru", 
			_ => "en", 
		};
	}

	public string GetUUID()
	{
		string empty = string.Empty;
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getContentResolver", new object[0]);
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.provider.Settings$Secure");
		string text = androidJavaClass2.CallStatic<string>("getString", new object[2] { androidJavaObject, "android_id" });
		AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("android.os.Build");
		string static2 = androidJavaObject2.GetStatic<string>("FINGERPRINT");
		empty = text + "_" + static2;
		return Md5Sum(empty);
	}

	public string Md5Sum(string strToEncrypt)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(strToEncrypt);
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text += Convert.ToString(array[i], 16).PadLeft(2, '0');
		}
		return text.PadLeft(32, '0');
	}

	public string getMD5Hash(string strToEncrypt)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(strToEncrypt);
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text += Convert.ToString(array[i], 16).PadLeft(2, '0');
		}
		return text.PadLeft(32, '0');
	}

	public void ScheduleLocalPush(ELocalPushType type, int time)
	{
		switch (type)
		{
		case ELocalPushType.LeaveVipShop:
			if (time <= 1800)
			{
				return;
			}
			time -= 1800;
			break;
		case ELocalPushType.BulletFullCharge:
		{
			UserLevelDataRow userLevelDataRow = instance.regulation.GetUserLevelDataRow(localUser.level);
			GoodsDataRow goodsDataRow = instance.regulation.goodsDtbl[5.ToString()];
			int num = (userLevelDataRow.maxBullet - localUser.bullet - 1) * goodsDataRow.rechargeTime;
			if (num < 0)
			{
				num = 0;
			}
			time += num;
			break;
		}
		}
		if (PlayerPrefs.HasKey(type.ToString()))
		{
			CancelLocalPush(type);
		}
		if (time > 0 && DateTime.Now.AddSeconds(time).Hour < 21 && DateTime.Now.AddSeconds(time).Hour >= 8 && (type != 0 || bool.Parse(PlayerPrefs.GetString("Setting-PushBullet", bool.FalseString))) && (type != ELocalPushType.SkillPointFullCharge || bool.Parse(PlayerPrefs.GetString("Setting-PushSkillPoint", bool.FalseString))) && (type != ELocalPushType.PremiumGachaFree || bool.Parse(PlayerPrefs.GetString("Setting-PushPremium", bool.FalseString))) && (type != ELocalPushType.LeaveVipShop || bool.Parse(PlayerPrefs.GetString("Setting-PushVipShop", bool.FalseString))))
		{
			string message = Localization.Get(((int)(type + 7198)).ToString());
			PlayerPrefs.SetInt(type.ToString(), SA_Singleton<AndroidNotificationManager>.Instance.ScheduleLocalNotification(Localization.Get("7197"), message, time));
		}
	}

	public void CancelLocalPush(ELocalPushType type)
	{
		if (PlayerPrefs.HasKey(type.ToString()))
		{
			SA_Singleton<AndroidNotificationManager>.Instance.CancelLocalNotification(PlayerPrefs.GetInt(type.ToString()));
			PlayerPrefs.DeleteKey(type.ToString());
		}
	}

	private void OnNotificationScheduleResult(ISN_Result res)
	{
		IOSNotificationController.OnNotificationScheduleResult -= OnNotificationScheduleResult;
		if (!res.IsSucceeded)
		{
		}
	}

	public void RequestSignUp(string uid, string pwd)
	{
		if (localUser == null)
		{
			localUser = RoLocalUser.CreateLocalUser();
		}
		JsonRpcClient.instance.SendRequest(this, "SignUp", uid, pwd, 1, localUser.channel);
	}

	public void RequestLogout()
	{
		JsonRpcClient.instance.SendRequest(this, "Logout");
	}

	public void RequestGuestSignUp()
	{
		if (localUser == null)
		{
			localUser = RoLocalUser.CreateLocalUser();
		}
		JsonRpcClient.instance.SendRequest(this, "GuestSignUp", 0, localUser.channel);
	}

	public void RequestSignIn(string uid, string pwd)
	{
		if (localUser == null)
		{
			localUser = RoLocalUser.CreateLocalUser();
		}
		JsonRpcClient.instance.SendRequest(this, "SignIn", uid, pwd, 1, localUser.channel);
	}

	public void RequestGuestSignIn(string uid)
	{
		if (localUser == null)
		{
			localUser = RoLocalUser.CreateLocalUser();
		}
		JsonRpcClient.instance.SendRequest(this, "GuestSignIn", uid, 0, localUser.channel);
	}

	public void RequestFBSignIn(string token)
	{
		if (localUser == null)
		{
			localUser = RoLocalUser.CreateLocalUser();
		}
		JsonRpcClient.instance.SendRequest(this, "FBSignIn", token, 2, localUser.channel);
	}

	public void RequestGoogleSignIn(string token)
	{
		if (localUser == null)
		{
			localUser = RoLocalUser.CreateLocalUser();
		}
		JsonRpcClient.instance.SendRequest(this, "GoogleSignIn", token, 3, localUser.channel);
	}

	public void RequestChangeMembership(string uid, string pwd, string puid)
	{
		JsonRpcClient.instance.SendRequest(this, "ChangeMembership", uid, pwd, 1, puid, localUser.channel);
	}

	public void RequestChangeMembershipOpenPlatform(string tokn, Platform plfm, string puid)
	{
		JsonRpcClient.instance.SendRequest(this, "ChangeMembershipOpenPlatform", tokn, plfm, puid, localUser.channel);
	}

	public void RequestServerStatus()
	{
		JsonRpcClient.instance.SendRequest(this, "ServerStatus", localUser.mIdx.ToString(), localUser.tokn, localUser.channel);
	}

	public void RequestUserTerm()
	{
		JsonRpcClient.instance.SendRequest(this, "UserTerm", localUser.channel);
	}

	private void _TestLOGIN()
	{
		string text = SystemInfo.operatingSystem;
		int num = text.IndexOf("/");
		if (num > 0)
		{
			text = text.Substring(0, num);
		}
		else if (text.Length > 40)
		{
			text = text.Substring(0, 40);
		}
		JsonRpcClient.instance.SendRequest(this, "Login", JObject.FromObject(new Protocols.AuthLoginRequest
		{
			world = 1,
			platform = Platform.Dbros,
			deviceName = SystemInfo.deviceModel,
			deviceId = SystemInfo.deviceUniqueIdentifier,
			patchType = 1,
			osCode = Protocols.OSCode.Android,
			osVersion = text,
			gameVersion = Application.version,
			apkFileName = "notyet",
			pushRegistrationId = "PushPush"
		}));
	}

	public void LoginForLocalTest()
	{
		loginTime = 1000.0;
		if (regulation == null)
		{
			regulation = Regulation.FromLocalResources();
		}
		if (localUser == null)
		{
			localUser = RoLocalUser.CreateLocalUser("User");
		}
		else
		{
			localUser.nickname = "TEST";
		}
		localUser.InitData();
		localUser.AddExp(9999);
		localUser.SetDummyDuelInfo();
		_TestTroop();
		_TestCommanderRecruitList();
		_TestRewardList();
		_TestWorldMap();
		Localization.language = GameSetting.instance.language;
	}

	public void RequestChangeNickname(string nickname)
	{
		JsonRpcClient.instance.SendRequest(this, "ChangeNickname", nickname);
	}

	public void RequestRecruitCommanderList()
	{
		JsonRpcClient.instance.SendRequest(this, "GetRecruitCommanderList", null);
	}

	public void RequestRecruitCommanderDelay(string commanderId)
	{
		int num = localUser.recruit.FindIndex(commanderId);
		int num2 = _ConvertStringToInt(commanderId);
		JsonRpcClient.instance.SendRequest(this, "RecruitCommanderDelay", num2, num);
	}

	public void RequestCommanderDelayCancle(string commanderId)
	{
		int num = _ConvertStringToInt(commanderId);
		JsonRpcClient.instance.SendRequest(this, "CommanderDelayCancle", num);
	}

	public void RequestRecruitCommanderRefresh(bool _isCash)
	{
		JsonRpcClient.instance.SendRequest(this, "RecruitCommanderRefresh", null);
	}

	public void RequestRecruitCommander(string commanderId)
	{
		int num = _ConvertStringToInt(commanderId);
		JsonRpcClient.instance.SendRequest(this, "RecruitCommander", num);
	}

	public void RequestCommanderLevelUp(string commanderId, int cnt, string ctt)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "CommanderLevelUp", _ConvertStringToInt(commanderId), cnt, ctt);
		if (localUser.tutorialData.enable)
		{
			JsonRpcClient.instance.EnqueueRequest(this, "UpdateTutorialStep", localUser.tutorialData.nxtStep);
		}
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestCommanderRankUp(string commanderId)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "CommanderRankUp", _ConvertStringToInt(commanderId));
		if (localUser.tutorialData.enable)
		{
			JsonRpcClient.instance.EnqueueRequest(this, "UpdateTutorialStep", localUser.tutorialData.nxtStep);
		}
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestCommanderRankUpImmediate(string commanderId)
	{
		JsonRpcClient.instance.SendRequest(this, "CommanderRankUpImmediate", _ConvertStringToInt(commanderId));
	}

	public void RequestCommanderSkillLevelUp(string commanderId, int skillIdx, int cnt)
	{
		JsonRpcClient.instance.SendRequest(this, "CommanderSkillLevelUp", _ConvertStringToInt(commanderId), skillIdx, cnt);
	}

	public void RequestCommanderClassUp(string commanderId)
	{
		JsonRpcClient.instance.SendRequest(this, "CommanderClassUp", _ConvertStringToInt(commanderId));
	}

	public void RequestTutorialCommanderClassUp(string commanderId)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "CommanderClassUp", _ConvertStringToInt(commanderId));
		JsonRpcClient.instance.EnqueueRequest(this, "UpdateTutorialStep", localUser.tutorialData.nxtStep);
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestUnitLevelUp(string id)
	{
		JsonRpcClient.instance.SendRequest(this, "UnitUpgrade", _ConvertStringToInt(id));
	}

	public void RequestUnitLevelUpImmediate(string id)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "UnitLevelUpImmediate", _ConvertStringToInt(id));
		JsonRpcClient.instance.EnqueueRequest(this, "GetUserInformation", Protocols.GetUserInfoTargetList(Protocols.UserInformationType.Unit));
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestGetPartOfUserInfomation(Protocols.UserInformationType infoType)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GetUserInformation", Protocols.GetUserInfoTargetList(Protocols.UserInformationType.Unit));
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestAllTroopInformation()
	{
		List<RoTroop> troopList = localUser.troopList;
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestChangeTroopNickname(string commanderId, string nickname)
	{
		JsonRpcClient.instance.SendRequest(this, "ChangeTroopNickname", _ConvertStringToInt(commanderId), nickname);
	}

	public void RequestChangeTroopFormation(RoTroop troop, EBattleType type = EBattleType.Undefined)
	{
	}

	public void RequestAnnihilationChangeTroopFormation(RoTroop troop, EBattleType type = EBattleType.Undefined)
	{
	}

	public void RequestBuyItem(string itemCode)
	{
		UIManager.instance.RefreshOpenedUI();
	}

	public void RequestWorldMapInformation(string worldMapId)
	{
		JsonRpcClient.instance.SendRequest(this, "WorldMapInformation", _ConvertStringToInt(worldMapId));
	}

	public void RequestWorldMapReward(string worldMapId)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "WorldMapReward", _ConvertStringToInt(worldMapId));
		if (localUser.tutorialData.enable)
		{
			JsonRpcClient.instance.EnqueueRequest(this, "UpdateTutorialStep", localUser.tutorialData.nxtStep);
		}
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestStartAnnihilation(BattleData battleData)
	{
	}

	public void RequestPowerPlantBattleEnd(string stageId)
	{
		JsonRpcClient.instance.SendRequest(this, "PowerPlantBattleEnd", _ConvertStringToInt(stageId));
	}

	public void RequestPlunder(BattleData battleData, int kind = -1)
	{
		if (battleData == null)
		{
			return;
		}
		BattleData.Get();
		BattleData.Set(battleData);
		RoTroop attackerTroop = battleData.attackerTroop;
		string key = null;
		bool flag = false;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		int num = 0;
		for (int i = 0; i < attackerTroop.slots.Length; i++)
		{
			RoTroop.Slot slot = attackerTroop.slots[i];
			if (!slot.IsValid())
			{
				continue;
			}
			if (slot.charType == ECharacterType.Mercenary || slot.charType == ECharacterType.SuperMercenary)
			{
				dictionary2.Add(slot.userIdx.ToString(), slot.commanderId);
				key = (slot.position + 1).ToString();
				flag = true;
			}
			if (slot.charType == ECharacterType.NPCMercenary || slot.charType == ECharacterType.SuperNPCMercenary)
			{
				NPCMercenaryDataRow nPCMercenaryDataRow = regulation.npcMercenaryDtbl.Find((NPCMercenaryDataRow row) => row.unitId == slot.unitId);
				if (nPCMercenaryDataRow != null)
				{
					num = int.Parse(nPCMercenaryDataRow.id);
				}
				key = (slot.position + 1).ToString();
				flag = true;
			}
			dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
		}
		JsonRpcClient.instance.EnqueueRequest(this, "WorldMapStageStart", (int)battleData.type, JObject.FromObject(dictionary), JObject.FromObject(dictionary2), kind, _ConvertStringToInt(battleData.stageId), num);
		JsonRpcClient.instance.EnqueueRequest(this, "BulletCharge");
		JsonRpcClient.instance.SendAllQueuedRequests();
		if (flag)
		{
			dictionary.Remove(key);
		}
		PlayerPrefs.SetString("StageDeck", JObject.FromObject(dictionary).ToString());
		localUser.lastPlayStage = int.Parse(battleData.stageId);
	}

	public void RequestDuel(BattleData battleData)
	{
		if (battleData != null)
		{
			BattleData.Get();
			BattleData.Set(battleData);
			Loading.Load(Loading.Battle);
		}
	}

	public void RequestRaid(BattleData battleData, int kind = -1)
	{
		if (battleData == null)
		{
			return;
		}
		BattleData.Get();
		BattleData.Set(battleData);
		string key = null;
		bool flag = false;
		int raidId = battleData.raidData.raidId;
		RoTroop attackerTroop = battleData.attackerTroop;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		int num = 0;
		for (int i = 0; i < attackerTroop.slots.Length; i++)
		{
			RoTroop.Slot slot = attackerTroop.slots[i];
			if (!slot.IsValid())
			{
				continue;
			}
			if (slot.charType == ECharacterType.Mercenary || slot.charType == ECharacterType.SuperMercenary)
			{
				dictionary2.Add(slot.userIdx.ToString(), slot.commanderId);
				key = (slot.position + 1).ToString();
				flag = true;
			}
			if (slot.charType == ECharacterType.NPCMercenary || slot.charType == ECharacterType.SuperNPCMercenary)
			{
				NPCMercenaryDataRow nPCMercenaryDataRow = regulation.npcMercenaryDtbl.Find((NPCMercenaryDataRow row) => row.unitId == slot.unitId);
				if (nPCMercenaryDataRow != null)
				{
					num = int.Parse(nPCMercenaryDataRow.id);
				}
				key = (slot.position + 1).ToString();
				flag = true;
			}
			dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
		}
		JsonRpcClient.instance.SendRequest(this, "RaidStart", (int)battleData.type, JObject.FromObject(dictionary), JObject.FromObject(dictionary2), kind, raidId, num);
		if (flag)
		{
			dictionary.Remove(key);
		}
		PlayerPrefs.SetString("RaidDeck", JObject.FromObject(dictionary).ToString());
	}

	public void RequestGetRaidRankList()
	{
		JsonRpcClient.instance.SendRequest(this, "GetRaidRankList");
	}

	public void RequestGetRaidInfo()
	{
		JsonRpcClient.instance.SendRequest(this, "GetRaidInfo");
	}

	public void RequestBuildingLevelUp(EBuilding type)
	{
		JsonRpcClient.instance.SendRequest(this, "BuildingLevelUp", (int)type);
	}

	public void RequestBuildingLevelUpImmediate(EBuilding type)
	{
		JsonRpcClient.instance.SendRequest(this, "BuildingLevelUpImmediate", (int)type);
	}

	public void RequestBuildingUpgradeCompleteIn(EBuilding type)
	{
		JsonRpcClient.instance.SendRequest(this, "BuildingUpgradeCompleteIn", (int)type);
	}

	public void RequestBuildingUpgradeCompleteOut(EBuilding type)
	{
		JsonRpcClient.instance.SendRequest(this, "BuildingUpgradeCompleteOut", (int)type);
	}

	public void RequestBuildingInformation(EBuilding type)
	{
		JsonRpcClient.instance.SendRequest(this, "BuildingInformation", (int)type);
	}

	public void RequestSituationInformation()
	{
		JsonRpcClient.instance.SendRequest(this, "SituationInformation");
	}

	public void RequestSituationSweepStart(BattleData battleData)
	{
		if (battleData == null)
		{
			return;
		}
		BattleData.Get();
		BattleData.Set(battleData);
		RoTroop attackerTroop = battleData.attackerTroop;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < attackerTroop.slots.Length; i++)
		{
			RoTroop.Slot slot = attackerTroop.slots[i];
			if (slot.IsValid() && slot.charType != ECharacterType.Helper)
			{
				dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
			}
		}
		JsonRpcClient.instance.SendRequest(this, "SituationSweepStart", (int)battleData.type, battleData.sweepType, battleData.sweepLevel, JObject.FromObject(dictionary));
		PlayerPrefs.SetString($"SweepDeck_{battleData.sweepType}_{battleData.sweepLevel}", JObject.FromObject(dictionary).ToString());
	}

	public void RequestInfinityBattleStart(BattleData battleData)
	{
		if (battleData == null)
		{
			return;
		}
		BattleData.Get();
		BattleData.Set(battleData);
		RoTroop attackerTroop = battleData.attackerTroop;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < attackerTroop.slots.Length; i++)
		{
			RoTroop.Slot slot = attackerTroop.slots[i];
			if (slot.IsValid())
			{
				dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
			}
		}
		JsonRpcClient.instance.EnqueueRequest(this, "InfinityBattleStart", (int)battleData.type, int.Parse(battleData.stageId), JObject.FromObject(dictionary));
		JsonRpcClient.instance.EnqueueRequest(this, "SaveInfinityBattleDeck", JObject.FromObject(dictionary));
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestSituationSweepResult(bool win, EWeekType type, string lv, string cid)
	{
		if (win)
		{
			JsonRpcClient.instance.SendRequest(this, "TEST_SituationStageBattleWin", (int)type, _ConvertStringToInt(lv), _ConvertStringToInt(cid));
		}
		else
		{
			JsonRpcClient.instance.SendRequest(this, "TEST_SituationStageBattleLose", (int)type, _ConvertStringToInt(lv), _ConvertStringToInt(cid));
		}
	}

	public void RequestDailyBonusCheck()
	{
		if (localUser.dailyBonus.needCheck)
		{
			JsonRpcClient.instance.SendRequest(this, "DailyBonusCheck", null);
		}
	}

	public void RequestDailyBonusReceive()
	{
		JsonRpcClient.instance.SendRequest(this, "DailyBonusReceive", null);
	}

	public void RequestBuyGoods(EGoods goods)
	{
		JsonRpcClient.instance.SendRequest(this, "BuyGoods", (int)goods);
	}

	public void RequestPvPWaveDuelRankingList()
	{
		JsonRpcClient.instance.SendRequest(this, "PvPWaveDuelRankingList");
	}

	public void RequestPvPWaveDuelList()
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GetWaveDuelDefenderInfo");
		JsonRpcClient.instance.EnqueueRequest(this, "PvPWaveDuelList");
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestRefreshPvPWaveDuelList()
	{
		JsonRpcClient.instance.SendRequest(this, "RefreshPvPWaveDuelList");
	}

	public void RequestWaveDuelDefenderSetting(List<RoTroop> troops)
	{
		Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
		for (int i = 0; i < troops.Count; i++)
		{
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			for (int j = 0; j < troops[i].slots.Length; j++)
			{
				RoTroop.Slot slot = troops[i].slots[j];
				if (slot.IsValid())
				{
					dictionary2.Add((slot.position + 1).ToString(), slot.commanderId);
				}
			}
			dictionary.Add((i + 1).ToString(), dictionary2);
		}
		JsonRpcClient.instance.SendRequest(this, "WaveDuelDefenderSetting", JObject.FromObject(dictionary));
	}

	public void RequestPvPStartWaveDuel(BattleData battleData)
	{
		BattleData.Get();
		BattleData.Set(battleData);
		List<RoTroop> battleTroopList = battleData.attacker.battleTroopList;
		Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
		for (int i = 0; i < battleTroopList.Count; i++)
		{
			RoTroop roTroop = battleTroopList[i];
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			for (int j = 0; j < roTroop.slots.Length; j++)
			{
				RoTroop.Slot slot = roTroop.slots[j];
				if (slot.IsValid())
				{
					dictionary2.Add((slot.position + 1).ToString(), slot.commanderId);
				}
			}
			dictionary.Add((i + 1).ToString(), dictionary2);
		}
		battleData.record = Simulator.Simulation(instance.regulation, battleData, GameSetting.instance.effect);
		battleData.isWin = battleData.record.result.IsWin;
		JToken jToken = (JToken)battleData.record;
		JToken jToken2 = (JToken)battleData.record.result;
		JsonRpcClient.instance.SendRequest(this, "PvPStartWaveDuel", (int)battleData.type, _ConvertStringToInt(battleData.defender.id), battleData.record.result.checksum, _ConvertStringToArray(jToken.ToString(Formatting.None)), _ConvertStringToArray(jToken2.ToString(Formatting.None)));
		PlayerPrefs.SetString("WaveDuelDeck", JObject.FromObject(dictionary).ToString());
	}

	public void RequestPvPRankingList()
	{
		JsonRpcClient.instance.SendRequest(this, "PvPRankingList");
	}

	public void RequestPvPDuelList()
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GetDefenderInfo");
		JsonRpcClient.instance.EnqueueRequest(this, "PvPDuelList");
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestRefreshPvPDuelList()
	{
		JsonRpcClient.instance.SendRequest(this, "RefreshPvPDuelList");
	}

	public void RequestPvPDuelInfo(string index)
	{
		JsonRpcClient.instance.SendRequest(this, "PvPDuelInfo", _ConvertStringToInt(index));
	}

	public void RequestPvPStartDuel(BattleData battleData)
	{
		BattleData.Get();
		BattleData.Set(battleData);
		RoTroop attackerTroop = battleData.attackerTroop;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < attackerTroop.slots.Length; i++)
		{
			RoTroop.Slot slot = attackerTroop.slots[i];
			if (slot.IsValid())
			{
				dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
			}
		}
		battleData.record = Simulator.Simulation(instance.regulation, battleData, GameSetting.instance.effect);
		battleData.isWin = battleData.record.result.IsWin;
		JToken jToken = (JToken)battleData.record;
		JToken jToken2 = (JToken)battleData.record.result;
		JsonRpcClient.instance.SendRequest(this, "PvPStartDuel", (int)battleData.type, JObject.FromObject(dictionary), _ConvertStringToInt(battleData.defender.id), battleData.record.result.checksum, _ConvertStringToArray(jToken.ToString(Formatting.None)), _ConvertStringToArray(jToken2.ToString(Formatting.None)));
		PlayerPrefs.SetString("DuelDeck", JObject.FromObject(dictionary).ToString());
	}

	public void RequestPvPStartWorldDuel(BattleData battleData)
	{
		BattleData.Get();
		BattleData.Set(battleData);
		RoTroop attackerTroop = battleData.attackerTroop;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < attackerTroop.slots.Length; i++)
		{
			RoTroop.Slot slot = attackerTroop.slots[i];
			if (slot.IsValid())
			{
				dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
			}
		}
		battleData.record = Simulator.Simulation(instance.regulation, battleData, GameSetting.instance.effect);
		battleData.isWin = battleData.record.result.IsWin;
		JToken jToken = (JToken)battleData.record;
		JToken jToken2 = (JToken)battleData.record.result;
		JsonRpcClient.instance.SendRequest(this, "PvPStartWorldDuel", (int)battleData.type, battleData.worldDuelReMatch ? 1 : 0, JObject.FromObject(dictionary), _ConvertStringToInt(battleData.defender.id), battleData.record.result.checksum, _ConvertStringToArray(jToken.ToString(Formatting.None)), _ConvertStringToArray(jToken2.ToString(Formatting.None)));
		PlayerPrefs.SetString("WorldDuelDeck", JObject.FromObject(dictionary).ToString());
	}

	public void RequestPvPResult(bool win)
	{
		int num = (win ? 1 : 0);
		JsonRpcClient.instance.SendRequest(this, "TEST_DuelWin", num);
	}

	public void RequestGachaInformation()
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GachaInformation", null);
		foreach (KeyValuePair<string, RoGacha> item in localUser.gacha)
		{
			if (item.Value.gachaRatingTypeA.Count == 0 && item.Value.gachaRatingTypeB.Count == 0)
			{
				JsonRpcClient.instance.EnqueueRequest(this, "GachaRatingInformationType");
				break;
			}
		}
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestGachaOpenBox(string type, int count)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GachaOpenBox", int.Parse(type), count);
		if (localUser.tutorialData.enable)
		{
			JsonRpcClient.instance.EnqueueRequest(this, "UpdateTutorialStep", localUser.tutorialData.nxtStep);
		}
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestUpdateTroopRole(string cid, string role)
	{
		JsonRpcClient.instance.SendRequest(this, "UpdateTroopRole", _ConvertStringToInt(cid), role);
	}

	public void RequestDefenderSetting(RoTroop troop)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < troop.slots.Length; i++)
		{
			RoTroop.Slot slot = troop.slots[i];
			if (slot.IsValid())
			{
				dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
			}
		}
		JsonRpcClient.instance.SendRequest(this, "DefenderSetting", JObject.FromObject(dictionary));
	}

	public void RequestGetDefenderInfo()
	{
		JsonRpcClient.instance.SendRequest(this, "GetDefenderInfo");
	}

	public void RequestCouponList()
	{
		JsonRpcClient.instance.SendRequest(this, "GetCouponList");
	}

	public void RequestInputCoupon(string coupon)
	{
		JsonRpcClient.instance.SendRequest(this, "InputCoupon", coupon);
	}

	public void RequestBadWordList(List<string> lang)
	{
		JsonRpcClient.instance.SendRequest(this, "GetBadWordList", lang);
	}

	public void RequestWhisperChatting(int from, string fromnm, int to, string tonm, string msg)
	{
	}

	public void RequestChChatting(int channel, int send, string snm, string msg, int ucash)
	{
	}

	public void RequestGuildChatting(int guild, int send, string snm, string msg, int ucash)
	{
	}

	public void RequestCheckChattingMsg()
	{
	}

	public void RequestSendwaitChannelMsg()
	{
	}

	public void RequestSendwaitGuildMsg()
	{
	}

	public void RequestBlockUsers()
	{
		JsonRpcClient.instance.SendRequest(this, "GetChatIgnoreList");
	}

	public void RequestAddBlockUser(int channel, string uno, string nick, string thumb)
	{
		JsonRpcClient.instance.SendRequest(this, "AddChatIgnore", channel, uno, nick, thumb);
	}

	public void RequestRemoveBlockUser(int channel, string uno)
	{
		JsonRpcClient.instance.SendRequest(this, "DelChatIgnore", channel, uno);
	}

	public void RequestMission()
	{
		List<string> list = new List<string>();
		list.Add("dlms");
		list.Add("achv");
		list.Add("uifo");
		JsonRpcClient.instance.EnqueueRequest(this, "Mission", list);
		JsonRpcClient.instance.EnqueueRequest(this, "CompleteAchievement");
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestCompleteMissionGoal(int type, int idx)
	{
		JsonRpcClient.instance.SendRequest(this, "CompleteMissionGoal", type, idx);
	}

	public void RequestVipBuyCount(EVipRechargeType renewType)
	{
		if (renewType != 0)
		{
			localUser.resetTimeData = null;
		}
		JsonRpcClient.instance.EnqueueRequest(this, "GetVipBuyCount", Protocols.GetUserInfoTargetList(Protocols.UserInformationType.Recharge), (int)renewType);
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestMissionReward(string idx)
	{
		JsonRpcClient.instance.SendRequest(this, "MissionReward", _ConvertStringToInt(idx));
	}

	public void RequestAllMissionReward()
	{
		JsonRpcClient.instance.SendRequest(this, "AllMissionReward");
	}

	public void RequestAchievementReward(string idx, int sort)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "AchievementReward", _ConvertStringToInt(idx), sort);
		if (localUser.tutorialData.enable)
		{
			JsonRpcClient.instance.EnqueueRequest(this, "UpdateTutorialStep", localUser.tutorialData.nxtStep);
		}
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestAllAchievementReward()
	{
		JsonRpcClient.instance.SendRequest(this, "AllAchievementReward");
	}

	public void RequestCompleteAchievement()
	{
		JsonRpcClient.instance.SendRequest(this, "CompleteAchievement");
	}

	public void RequestAchievement()
	{
		List<string> list = new List<string>();
		list.Add("achv");
		JsonRpcClient.instance.SendRequest(this, "Achievement", list);
	}

	public void RequestMailList()
	{
		JsonRpcClient.instance.SendRequest(this, "GetMailList");
	}

	public void RequestGetReward(string idx, int type)
	{
		JsonRpcClient.instance.SendRequest(this, "GetReward", _ConvertStringToInt(idx), type);
	}

	public void RequestReadMail(string idx)
	{
		JsonRpcClient.instance.SendRequest(this, "ReadMail", _ConvertStringToInt(idx));
	}

	public void RequestGetRewardAll()
	{
		JsonRpcClient.instance.SendRequest(this, "GetRewardAll");
	}

	public void RequestBankInfo()
	{
		JsonRpcClient.instance.SendRequest(this, "BankInfo");
	}

	public void RequestBankRoulletStart(int cnt, int vcnt)
	{
		JsonRpcClient.instance.SendRequest(this, "BankRoulletStart", 601, cnt, vcnt + 1);
	}

	public void RequestGetBankReward()
	{
		JsonRpcClient.instance.SendRequest(this, "GetBankReward");
	}

	public void RequestShopBuyGold(int type)
	{
		JsonRpcClient.instance.SendRequest(this, "ShopBuyGold", type);
	}

	public void RequestPing()
	{
		JsonRpcClient.instance.SendRequest(this, "Ping");
	}

	public void RequestGetRegion()
	{
		JsonRpcClient.instance.SendFirstRequest(this, "GetRegion");
	}

	public IEnumerator RequestDBVersionCheck()
	{
		if (localUser == null)
		{
			localUser = RoLocalUser.CreateLocalUser();
		}
		JsonRpcClient.instance.SendRequest(this, "DBVersionCheck", localUser.channel);
		yield break;
	}

	private JObject _ConvertStringToObject(string str)
	{
		if (str == null)
		{
			return null;
		}
		return JsonConvert.DeserializeObject<JObject>(str);
	}

	private JArray _ConvertStringToArray(string str)
	{
		if (str == null)
		{
			return null;
		}
		return JsonConvert.DeserializeObject<JArray>(str);
	}

	public void RequestBattleOut(EBattleType type, string checkSum, string info, string result)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "BattleOut", (int)type, checkSum, _ConvertStringToArray(info), _ConvertStringToArray(result));
		if (GameSetting.instance.repeatBattle)
		{
			if (type == EBattleType.Guerrilla)
			{
				JsonRpcClient.instance.EnqueueRequest(this, "SituationInformation");
			}
			JsonRpcClient.instance.EnqueueRequest(this, "BulletCharge");
		}
		else if (type == EBattleType.Plunder)
		{
			JsonRpcClient.instance.EnqueueRequest(this, "BulletCharge");
		}
		if (localUser.tutorialData.enable)
		{
			if (localUser.tutorialData.nxtStep == 6)
			{
				AdjustManager.Instance.SimpleEvent("7dq1w9");
			}
			JsonRpcClient.instance.EnqueueRequest(this, "UpdateTutorialStep", localUser.tutorialData.nxtStep);
		}
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestEventBattleOut(EBattleType type, string checkSum, string info, string result, int idx, int level = -1)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "BattleOut", (int)type, checkSum, _ConvertStringToArray(info), _ConvertStringToArray(result));
		if (GameSetting.instance.repeatBattle)
		{
			JsonRpcClient.instance.EnqueueRequest(this, "BulletCharge");
		}
		if (localUser.tutorialData.enable)
		{
			JsonRpcClient.instance.EnqueueRequest(this, "UpdateTutorialStep", localUser.tutorialData.nxtStep);
		}
		JsonRpcClient.instance.EnqueueRequest(this, "GetEventBattleData", idx, level);
		EventBattleDataRow eventBattleDataRow = regulation.eventBattleDtbl[idx.ToString()];
		if (eventBattleDataRow.gachaOneTimeAmount != 0 && int.Parse(eventBattleDataRow.eventPointIdx) > 0)
		{
			JsonRpcClient.instance.EnqueueRequest(this, "GetEventBattleGachaInfo", idx);
		}
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestBulletCharge()
	{
		JsonRpcClient.instance.SendRequest(this, "BulletCharge");
	}

	public void RequestResourceRecharge(int vidx, int mid, int vcnt)
	{
		JsonRpcClient.instance.SendRequest(this, "ResourceRecharge", vidx, mid, vcnt + 1);
	}

	public void RequestGetRecordList(ERePlayType type, ERePlaySubType subType)
	{
		JsonRpcClient.instance.SendRequest(this, "GetReplayList", type, subType, 1000000);
	}

	public void RequestGetRecordInfo(string id, ERePlayType type)
	{
		JsonRpcClient.instance.SendRequest(this, "GetReplayInfo", id, type);
	}

	public void RequestGetRecordList(ERePlayType type)
	{
		JsonRpcClient.instance.SendRequest(this, "GetRecordList", (int)type, 1000000);
	}

	public void RequestGetTutorialStep()
	{
		JsonRpcClient.instance.SendRequest(this, "GetTutorialStep", Protocols.GetUserInfoTargetList(Protocols.UserInformationType.Tutorial));
	}

	public void RequestLoginTutorialSkip()
	{
		JsonRpcClient.instance.SendRequest(this, "LoginTutorialSkip", 1);
	}

	public void RequestUpdateTutorialStep(int step)
	{
		JsonRpcClient.instance.SendRequest(this, "UpdateTutorialStep", step);
	}

	public void RequestSetNickNameFromTutorial(string nickName, int step)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "SetNickNameFromTutorial", nickName, step);
		if (localUser.isTutorialSkip)
		{
			JsonRpcClient.instance.EnqueueRequest(this, "BulletCharge");
		}
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestReceivePvPReward()
	{
		JsonRpcClient.instance.SendRequest(this, "ReceivePvPReward");
	}

	public void RequestReceiveDuelPointReward(string didx)
	{
		JsonRpcClient.instance.SendRequest(this, "ReceiveDuelPointReward", _ConvertStringToInt(didx));
	}

	public void RequestReceiveRaidReward()
	{
		JsonRpcClient.instance.SendRequest(this, "ReceiveRaidReward");
	}

	public void RequestUseTimeMachine(string mid, int cnt, int cash)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "UseTimeMachine", _ConvertStringToInt(mid), cnt, cash);
		JsonRpcClient.instance.EnqueueRequest(this, "BulletCharge");
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestUseTimeMachineSweep(int type, int lv, int cnt)
	{
		JsonRpcClient.instance.SendRequest(this, "UseTimeMachineSweep", type, lv, cnt);
	}

	public void RequestCheckBadge()
	{
		JsonRpcClient.instance.EnqueueRequest(this, "CheckBadge");
		JsonRpcClient.instance.EnqueueRequest(this, "GetWebEvent", localUser.channel);
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestChangeUserThumbnail(string idx)
	{
		JsonRpcClient.instance.SendRequest(this, "ChangeUserThumbnail", _ConvertStringToInt(idx));
	}

	public void RequestGetSecretShopList(EShopType type)
	{
		JsonRpcClient.instance.SendRequest(this, "GetSecretShopList", (int)type);
	}

	public void RequestRefreshSecretShopList(EShopType type)
	{
		JsonRpcClient.instance.SendRequest(this, "RefreshSecretShopList", (int)type);
	}

	public void RequestBuySecretShopItem(EShopType type, int id)
	{
		JsonRpcClient.instance.SendRequest(this, "BuySecretShopItem", (int)type, id);
	}

	public void RequestGetCashShopList()
	{
		JsonRpcClient.instance.SendRequest(this, "GetCashShopList");
	}

	public void RequestMakeOrderId(string pid)
	{
		localUser.lastBuyPid = pid;
		JsonRpcClient.instance.SendRequest(this, "MakeOrderId", pid);
	}

	public void RequestCheckPayment(string packageName, string productId, double purchaseTime, int purchaseState, string developerPayload, string purchaseToken, string orderId)
	{
		JsonRpcClient.instance.SendRequest(this, "CheckPayment", packageName, productId, purchaseTime, purchaseState, developerPayload, purchaseToken, orderId);
	}

	public void RequestCheckPaymentIOS(string productId, string receipt)
	{
		JsonRpcClient.instance.SendRequest(this, "CheckPaymentIOS", productId, receipt, localUser.lastIOSPayload);
	}

	public void RequestCheckPaymentOneStore(string txid, string signdata)
	{
		JsonRpcClient.instance.SendRequest(this, "CheckPaymentOneStore", localUser.lastBuyPid, localUser.lastIOSPayload, txid, signdata);
	}

	public void RequestCheckPaymentAmazon(string productId, string userId, string receipt)
	{
		JsonRpcClient.instance.SendRequest(this, "CheckPaymentAmazon", productId, userId, receipt, localUser.lastIOSPayload);
	}

	public void RequestCheckAlarm()
	{
	}

	public void RequestGuildList()
	{
		JsonRpcClient.instance.SendRequest(this, "GuildList");
	}

	public void RequestCreateGuild(string name, int type, int limitLevel, int emblem)
	{
		JsonRpcClient.instance.SendRequest(this, "CreateGuild", name, type, limitLevel, emblem);
	}

	public void RequestSearchGuild(string gnm)
	{
		JsonRpcClient.instance.SendRequest(this, "SearchGuild", gnm);
	}

	public void RequestFreeJoinGuild(int idx)
	{
		JsonRpcClient.instance.SendRequest(this, "FreeJoinGuild", idx);
	}

	public void RequestApplyGuildJoin(int idx)
	{
		JsonRpcClient.instance.SendRequest(this, "ApplyGuildJoin", idx);
	}

	public void RequestCancelGuildJoin(int idx)
	{
		JsonRpcClient.instance.SendRequest(this, "CancelGuildJoin", idx);
	}

	public void RequestGuildMemberList()
	{
		JsonRpcClient.instance.SendRequest(this, "GuildMemberList");
	}

	public void RequestGuildInfoAndMember()
	{
		List<string> list = new List<string>();
		list.Add("gld");
		JsonRpcClient.instance.EnqueueRequest(this, "GuildInfo", list);
		JsonRpcClient.instance.EnqueueRequest(this, "GuildMemberList");
		JsonRpcClient.instance.EnqueueRequest(this, "GetConquestInfo");
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestUpdateGuildInfo(int act, string val)
	{
		JsonRpcClient.instance.SendRequest(this, "UpdateGuildInfo", act, val);
	}

	public void RequestManageGuildJoinMember()
	{
		JsonRpcClient.instance.SendRequest(this, "ManageGuildJoinMember");
	}

	public void RequestApproveGuildJoin(int uno)
	{
		JsonRpcClient.instance.SendRequest(this, "ApproveGuildJoin", uno);
	}

	public void RequestRefuseGuildJoin(int uno)
	{
		JsonRpcClient.instance.SendRequest(this, "RefuseGuildJoin", uno);
	}

	public void RequestLeaveGuild()
	{
		JsonRpcClient.instance.SendRequest(this, "LeaveGuild");
	}

	public void RequestGuildCloseDown()
	{
		JsonRpcClient.instance.SendRequest(this, "GuildCloseDown");
	}

	public void RequestDelegatingGuild(int uno)
	{
		JsonRpcClient.instance.SendRequest(this, "DelegatingGuild", uno);
	}

	public void RequestDeportGuildMember(int uno)
	{
		JsonRpcClient.instance.SendRequest(this, "DeportGuildMember", uno);
	}

	public void RequestAppointSubMaster(int uno)
	{
		JsonRpcClient.instance.SendRequest(this, "AppointSubMaster", uno);
	}

	public void RequestFireSubMaster(int uno)
	{
		JsonRpcClient.instance.SendRequest(this, "FireSubMaster", uno);
	}

	public void RequestUpgradeGuildSkill(int gsid)
	{
		JsonRpcClient.instance.SendRequest(this, "UpgradeGuildSkill", gsid);
	}

	public void RequestUpgradeGuildLevel()
	{
		JsonRpcClient.instance.SendRequest(this, "UpgradeGuildLevel");
	}

	public void RequestGetGuildBoard(int page)
	{
		JsonRpcClient.instance.SendRequest(this, "GetGuildBoard", page);
	}

	public void RequestGuildBoardWrite(string msg, int page)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GuildBoardWrite", msg);
		JsonRpcClient.instance.EnqueueRequest(this, "GetGuildBoard", page);
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestGuildBoardDelete(int idx, int page)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GuildBoardDelete", idx);
		JsonRpcClient.instance.EnqueueRequest(this, "GetGuildBoard", page);
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestSetConquestTroop(BattleData battleData)
	{
		if (battleData == null)
		{
			return;
		}
		BattleData.Get();
		BattleData.Set(battleData);
		RoTroop attackerTroop = battleData.attackerTroop;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < attackerTroop.slots.Length; i++)
		{
			RoTroop.Slot slot = attackerTroop.slots[i];
			if (slot.IsValid())
			{
				dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
			}
		}
		JsonRpcClient.instance.SendRequest(this, "SetConquestTroop", battleData.conquestDeckId, JObject.FromObject(dictionary));
	}

	public void RequestConquestJoin()
	{
		JsonRpcClient.instance.SendRequest(this, "ConquestJoin");
	}

	public void RequestGetConquestInfo()
	{
		JsonRpcClient.instance.SendRequest(this, "GetConquestInfo");
	}

	public void RequestGetConquestReplay(string replayId)
	{
		JsonRpcClient.instance.SendRequest(this, "GetConquestReplay", replayId);
	}

	public void RequestGetConquestTroop()
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GetConquestTroop");
		JsonRpcClient.instance.EnqueueRequest(this, "GetConquestNotice", 1);
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestSetConquestMoveTroop(int dest, int slot, int ucash)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "SetConquestMoveTroop", dest, slot, ucash);
		JsonRpcClient.instance.EnqueueRequest(this, "GetConquestStageInfo", dest);
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestGetConquestMovePath(int dest, int slot)
	{
		JsonRpcClient.instance.SendRequest(this, "GetConquestMovePath", dest, slot);
	}

	public void RequestDeleteConquestTroop(int slot)
	{
		JsonRpcClient.instance.SendRequest(this, "DeleteConquestTroop", slot);
	}

	public void RequestBuyConquestTroopSlot(int slot)
	{
		JsonRpcClient.instance.SendRequest(this, "BuyConquestTroopSlot", slot);
	}

	public void RequestGetConquestStageInfo(int point)
	{
		JsonRpcClient.instance.SendRequest(this, "GetConquestStageInfo", point);
	}

	public void RequestGetConquestStageUserInfo(int uno, int point)
	{
		JsonRpcClient.instance.SendRequest(this, "GetConquestStageUserInfo", uno, point);
	}

	public void RequestGetConquestCurrentStateInfo()
	{
		JsonRpcClient.instance.SendRequest(this, "GetConquestCurrentStateInfo");
	}

	public void RequestGetConquestRadar()
	{
		JsonRpcClient.instance.SendRequest(this, "GetConquestRadar");
	}

	public void RequestGetConquestBattle(int point, int skip = 0)
	{
		JsonRpcClient.instance.SendRequest(this, "GetConquestBattle", point, skip);
	}

	public void RequestGetGuildRanking(int type)
	{
		JsonRpcClient.instance.SendRequest(this, "GetGuildRanking", type);
	}

	public void RequestGetConquestNotice()
	{
		JsonRpcClient.instance.SendRequest(this, "GetConquestNotice", 0);
	}

	public void RequestSetConquestNotice(string notice)
	{
		JsonRpcClient.instance.SendRequest(this, "SetConquestNotice", notice);
	}

	public void RequestStartConquestRadar()
	{
		JsonRpcClient.instance.SendRequest(this, "StartConquestRadar");
	}

	public void RequestAnnihilationMapInformation()
	{
		JsonRpcClient.instance.SendRequest(this, "AnnihilationMapInformation");
	}

	public void RequestAnnihilationEnemyInformation(string id)
	{
		JsonRpcClient.instance.SendRequest(this, "AnnihilationEnemyInformation", _ConvertStringToInt(id));
	}

	public void RequestGetAnnihilationHistory(string id)
	{
		JsonRpcClient.instance.SendRequest(this, "GetAnnihilationHistory", _ConvertStringToInt(id));
	}

	public void RequestGetAnnihilationRanking()
	{
		JsonRpcClient.instance.SendRequest(this, "GetAnnihilationRanking");
	}

	public void RequestGiftFood(string cid, string cgid, int amnt)
	{
		JsonRpcClient.instance.SendRequest(this, "GiftFood", int.Parse(cid), int.Parse(cgid), amnt);
	}

	public void RequestGetFavorReward(string cid, int step)
	{
		JsonRpcClient.instance.SendRequest(this, "GetFavorReward", int.Parse(cid), step);
	}

	public void RequestSellItem(EStorageType type, string itemId, int count)
	{
		JsonRpcClient.instance.SendRequest(this, "SellItem", (int)type, _ConvertStringToInt(itemId), count);
	}

	public void RequestOpenItem(EStorageType type, string itemId, int count, int rType = 0, int rIdx = 0)
	{
		JsonRpcClient.instance.SendRequest(this, "OpenItem", (int)type, _ConvertStringToInt(itemId), count, rType, rIdx);
	}

	public void RequestPreDeckSetting(List<Protocols.UserInformationResponse.PreDeck> deckList)
	{
		JsonRpcClient.instance.SendRequest(this, "PreDeckSetting", JObject.FromObject(new Protocols.PreDeckInfoList
		{
			list = deckList
		}));
	}

	public void RequestExchangeMedal(int count, int cid)
	{
		JsonRpcClient.instance.SendRequest(this, "ExchangeMedal", count, cid);
	}

	public void RequestBuyCommanderCostume(int cid, int cos)
	{
		JsonRpcClient.instance.SendRequest(this, "BuyCommanderCostume", cid, cos);
	}

	public void RequestChangeCommanderCostume(int cid, int cos)
	{
		JsonRpcClient.instance.SendRequest(this, "ChangeCommanderCostume", cid, cos);
	}

	public void RequestChangeLanguage()
	{
		JsonRpcClient.instance.SendRequest(this, "ChangeLanguage", GetLanguageCode());
	}

	public void RequestGetNotice()
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GetEventNotice");
		JsonRpcClient.instance.EnqueueRequest(this, "GetCommonNotice");
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public IEnumerator GameVersionInfo()
	{
		int num = 20;
		if (localUser == null)
		{
			localUser = RoLocalUser.CreateLocalUser();
		}
		localUser.channel = PlayerPrefs.GetInt("Channel", 1);
		JsonRpcClient.instance.SendFirstRequest(this, "GameVersionInfo", Application.version, num, localUser.channel);
		yield break;
	}

	public void RequestGetAnnihilationMapInfo(int goReady = 0)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GuildDispatchCommanderList", 2);
		JsonRpcClient.instance.EnqueueRequest(this, "GetAnnihilationMapInfo", goReady);
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestDispatchAdvancedParty(RoTroop troop)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < troop.slots.Length; i++)
		{
			RoTroop.Slot slot = troop.slots[i];
			if (slot.IsValid())
			{
				list.Add(slot.commanderId);
			}
		}
		JsonRpcClient.instance.SendRequest(this, "DispatchAdvancedParty", list);
	}

	public void RequestAnnihilationStageStart(BattleData battleData, int kind = -1)
	{
		if (battleData == null)
		{
			return;
		}
		string text = null;
		bool flag = false;
		BattleData.Get();
		BattleData.Set(battleData);
		RoTroop attackerTroop = battleData.attackerTroop;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Dictionary<string, string> o = new Dictionary<string, string>();
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		for (int i = 0; i < attackerTroop.slots.Length; i++)
		{
			RoTroop.Slot slot = attackerTroop.slots[i];
			if (slot.IsValid())
			{
				dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
			}
		}
		JsonRpcClient.instance.SendRequest(this, "AnnihilationStageStart", (int)battleData.type, JObject.FromObject(dictionary), JObject.FromObject(o), kind, _ConvertStringToInt(battleData.stageId));
		PlayerPrefs.SetString("AnnihilationDeck", JObject.FromObject(dictionary).ToString());
	}

	public void RequestResetAnnihilationStage(AnnihilationMode mode)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "ResetAnnihilationStage", mode);
		JsonRpcClient.instance.EnqueueRequest(this, "GuildDispatchCommanderList", 2);
		JsonRpcClient.instance.EnqueueRequest(this, "GetAnnihilationMapInfo", 0);
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void ReqeustRenewUserGameData()
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GetUserInformation", Protocols.GetUserInfoTargetList(default(Protocols.UserInformationType)));
		JsonRpcClient.instance.EnqueueRequest(this, "GetVipBuyCount", Protocols.GetUserInfoTargetList(Protocols.UserInformationType.Recharge), 0);
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestGetUserInformation(Protocols.UserInformationType type)
	{
		JsonRpcClient.instance.SendRequest(this, "GetUserInformation", Protocols.GetUserInfoTargetList(type));
	}

	public void RequestBuyVipShop()
	{
		JsonRpcClient.instance.SendRequest(this, "GetBuyVipShop");
	}

	public void RequestVipGachaInfo()
	{
		JsonRpcClient.instance.SendRequest(this, "GetVipGachaInfo");
	}

	public void RequestBuyVipGacha()
	{
		JsonRpcClient.instance.SendRequest(this, "BuyVipGacah");
	}

	public void RequestGetDispatchCommanderList()
	{
		JsonRpcClient.instance.SendRequest(this, "GetDispatchCommanderList");
	}

	public void RequestDispatchCommander(int cid, int slotIdx)
	{
		JsonRpcClient.instance.SendRequest(this, "DispatchCommander", cid, slotIdx);
	}

	public void RequestRecallCommander(int slotIdx)
	{
		JsonRpcClient.instance.SendRequest(this, "RecallDispatch", slotIdx);
	}

	public void RequestGetCarnivalList(ECarnivalCategory type, int eidx = 0)
	{
		JsonRpcClient.instance.SendRequest(this, "GetCarnivalList", eidx, (int)type);
	}

	public void RequestCarnivalComplete(int cidx, string idx, int eidx = 0, int cnt = 0)
	{
		JsonRpcClient.instance.SendRequest(this, "CarnivalComplete", cidx, _ConvertStringToInt(idx), eidx, cnt);
	}

	public void RequestCarnivalBuyPackage(int cidx, string idx)
	{
		JsonRpcClient.instance.SendRequest(this, "CarnivalBuyPackage", cidx, _ConvertStringToInt(idx));
	}

	public void RequestCarnivalSelectItem(int ctid, int cidx, int ridx)
	{
		JsonRpcClient.instance.SendRequest(this, "CarnivalSelectItem", ctid, cidx, ridx);
	}

	public void RequestGuildDispatchCommanderList(int type)
	{
		JsonRpcClient.instance.SendRequest(this, "GuildDispatchCommanderList", type);
	}

	public void RequestGetChangeDeviceCode()
	{
		JsonRpcClient.instance.SendRequest(this, "GetChangeDeviceCode");
	}

	public void RequestCheckChangeDeviceCode(string code)
	{
		JsonRpcClient.instance.SendRequest(this, "CheckChangeDeviceCode", code, localUser.channel);
	}

	public void RequestCheckPlatformExist(Platform plfm, string tokn)
	{
		if (plfm == Platform.FaceBook || plfm == Platform.Google)
		{
			localUser.openPlatformToken = tokn;
			JsonRpcClient.instance.SendRequest(this, "CheckOpenPlatformExist", plfm, tokn, localUser.channel);
		}
	}

	public void RequestChangeDevice(Platform plfm)
	{
		if (localUser == null)
		{
			localUser = RoLocalUser.CreateLocalUser();
		}
		Protocols.OSCode oSCode = Protocols.OSCode.Android;
		RegistrationId = SA_Singleton<GoogleCloudMessageService>.instance.registrationId;
		string text = SystemInfo.operatingSystem;
		int num = text.IndexOf("/");
		if (num > 0)
		{
			text = text.Substring(0, num);
		}
		else if (text.Length > 40)
		{
			text = text.Substring(0, 40);
		}
		JsonRpcClient.instance.SendRequest(this, "ChangeDevice", localUser.channel, localUser.changeDeviceCode, localUser.openPlatformToken, plfm, SystemInfo.deviceModel, SystemInfo.deviceUniqueIdentifier, 1, oSCode, text, Application.version, "notyet", GetLanguageCode(), GetUUID());
	}

	public void RequestSetPushOnOff(bool onoff)
	{
		JsonRpcClient.instance.SendRequest(this, "SetPushOnOff", onoff ? 1 : 0);
	}

	public void RequestStartDateMode()
	{
		JsonRpcClient.instance.SendRequest(this, "StartDateMode");
	}

	public void RequestDateModeGetGift()
	{
		JsonRpcClient.instance.SendRequest(this, "DateModeGetGift");
	}

	public void RequestChangeDeviceDbros(Platform plfm, string uid, string pwd)
	{
		if (localUser == null)
		{
			localUser = RoLocalUser.CreateLocalUser();
		}
		Protocols.OSCode oSCode = Protocols.OSCode.Android;
		RegistrationId = SA_Singleton<GoogleCloudMessageService>.instance.registrationId;
		string text = SystemInfo.operatingSystem;
		int num = text.IndexOf("/");
		if (num > 0)
		{
			text = text.Substring(0, num);
		}
		else if (text.Length > 40)
		{
			text = text.Substring(0, 40);
		}
		JsonRpcClient.instance.SendRequest(this, "ChangeDeviceDbros", localUser.channel, localUser.changeDeviceCode, uid, pwd, plfm, SystemInfo.deviceModel, SystemInfo.deviceUniqueIdentifier, 1, oSCode, text, Application.version, "notyet", GetLanguageCode(), GetUUID());
	}

	public void RequestGetCommanderScenario()
	{
		JsonRpcClient.instance.SendRequest(this, "GetCommanderScenario");
	}

	public void RequestRecieveCommanderScenarioReward(int cid, int sid)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "RecieveCommanderScenarioReward", cid, sid);
		JsonRpcClient.instance.EnqueueRequest(this, "GetCommanderScenario");
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestCompleteCommanderScenario(int cid, int sid, int sqid)
	{
		waitingScenarioComplete = true;
		JsonRpcClient.instance.EnqueueRequest(this, "CompleteCommanderScenario", cid, sid, sqid);
		JsonRpcClient.instance.EnqueueRequest(this, "GetCommanderScenario");
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestScenarioBattle(BattleData _battleData)
	{
		BattleData.Set(_battleData);
		Loading.Load(Loading.Battle);
	}

	public void RequestWaveBattleList()
	{
		JsonRpcClient.instance.SendRequest(this, "WaveBattleList");
	}

	public void RequestWaveBattleStart(BattleData battleData, int kind = -1)
	{
		if (battleData == null)
		{
			return;
		}
		BattleData.Get();
		BattleData.Set(battleData);
		RoTroop attackerTroop = battleData.attackerTroop;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
		int num = 0;
		for (int i = 0; i < attackerTroop.slots.Length; i++)
		{
			RoTroop.Slot slot = attackerTroop.slots[i];
			if (!slot.IsValid())
			{
				continue;
			}
			if (slot.charType == ECharacterType.Mercenary || slot.charType == ECharacterType.SuperMercenary)
			{
				dictionary2.Add(slot.userIdx.ToString(), slot.commanderId);
			}
			if (slot.charType == ECharacterType.Helper)
			{
				continue;
			}
			dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
			if (slot.charType == ECharacterType.NPCMercenary || slot.charType == ECharacterType.SuperNPCMercenary)
			{
				NPCMercenaryDataRow nPCMercenaryDataRow = regulation.npcMercenaryDtbl.Find((NPCMercenaryDataRow row) => row.unitId == slot.unitId);
				if (nPCMercenaryDataRow != null)
				{
					num = int.Parse(nPCMercenaryDataRow.id);
				}
			}
		}
		JsonRpcClient.instance.SendRequest(this, "WaveBattleStart", (int)battleData.type, JObject.FromObject(dictionary), JObject.FromObject(dictionary2), kind, _ConvertStringToInt(battleData.stageId), num);
	}

	public void RequestGetGroupReward(int gidx)
	{
		JsonRpcClient.instance.SendRequest(this, "GetGroupReward", gidx);
	}

	public void RequestSetItemEquipment(int eidx, int cid, int elv)
	{
		JsonRpcClient.instance.SendRequest(this, "SetItemEquipment", eidx, cid, elv);
	}

	public void RequestReleaseItemEquipment(int eidx, int cid)
	{
		JsonRpcClient.instance.SendRequest(this, "ReleaseItemEquipment", eidx, cid);
	}

	public void RequestUpgradeItemEquipment(int eidx, int cid, int elv)
	{
		JsonRpcClient.instance.SendRequest(this, "UpgradeItemEquipment", eidx, cid, elv);
	}

	public void RequestDecompositionItemEquipment(int eidx, int elv, int amnt)
	{
		JsonRpcClient.instance.SendRequest(this, "DecompositionItemEquipment", eidx, elv, amnt);
	}

	public void RequestChangeItemEquipment(int releaseIdx, int releaseCid, int equipIdx, int equipCid, int elv)
	{
		JsonRpcClient.instance.SendRequest(this, "ReleaseItemEquipment", releaseIdx, releaseCid);
		JsonRpcClient.instance.SendRequest(this, "SetItemEquipment", equipIdx, equipCid, elv);
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestGetDispatchCommanderListFromLogin()
	{
		JsonRpcClient.instance.SendRequest(this, "GetDispatchCommanderListFromLogin");
	}

	public void RequestExplorationList()
	{
		JsonRpcClient.instance.SendRequest(this, "GetExplorationList");
	}

	public void RequestExplorationStart(int idx, List<string> cids)
	{
		JsonRpcClient.instance.SendRequest(this, "ExplorationStart", idx, cids);
	}

	public void RequestExplorationStartAll(string jsonData)
	{
		JsonRpcClient.instance.SendRequest(this, "ExplorationStartAll", _ConvertStringToArray(jsonData));
	}

	public void RequestExplorationCancel(int idx)
	{
		JsonRpcClient.instance.SendRequest(this, "ExplorationCancel", idx);
	}

	public void RequestExplorationComplete(int idx)
	{
		JsonRpcClient.instance.SendRequest(this, "ExplorationComplete", idx);
	}

	public void RequestExplorationCompleteAll(List<int> idxs)
	{
		JsonRpcClient.instance.SendRequest(this, "ExplorationCompleteAll", idxs);
	}

	public void RequestCooperateBattleInfo()
	{
		JsonRpcClient.instance.SendRequest(this, "CooperateBattleInfo");
	}

	public void RequestCooperateBattlePointGuildRank()
	{
		JsonRpcClient.instance.SendRequest(this, "CooperateBattlePointGuildRank");
	}

	public void RequestCooperateBattlePointRank(int stage)
	{
		JsonRpcClient.instance.SendRequest(this, "CooperateBattlePointRank", stage);
	}

	public void RequestCooperateBattleComplete(int stage, int step)
	{
		JsonRpcClient.instance.SendRequest(this, "CooperateBattleComplete", stage, step);
	}

	public void RequestCooperateBattleReceiveAllReward(int stage)
	{
		JsonRpcClient.instance.SendRequest(this, "CooperateBattleComplete", stage, 0);
	}

	public void RequestCooperateBattleStart(BattleData battleData)
	{
		BattleData.Get();
		BattleData.Set(battleData);
		RoTroop attackerTroop = battleData.attackerTroop;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < attackerTroop.slots.Length; i++)
		{
			RoTroop.Slot slot = attackerTroop.slots[i];
			if (slot.IsValid())
			{
				dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
			}
		}
		JsonRpcClient.instance.SendRequest(this, "CooperateBattleStart", (int)battleData.type, JObject.FromObject(dictionary), battleData.stageLevel, regulation.cooperateBattleDtbl[battleData.stageId].step);
		PlayerPrefs.SetString("CooperateBattleDeck", JObject.FromObject(dictionary).ToString());
	}

	public void RequestStartWebEvent()
	{
		JsonRpcClient.instance.SendRequest(this, "StartWebEvent", localUser.channel);
	}

	public void RequestGetRotationBannerInfo()
	{
		JsonRpcClient.instance.SendRequest(this, "GetRotationBannerInfo");
	}

	public void RequestGetEventBattleList()
	{
		JsonRpcClient.instance.SendRequest(this, "GetEventBattleList");
	}

	public void RequestGetEventBattleData(int idx, int level = -1)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GetEventBattleData", idx, level);
		EventBattleDataRow eventBattleDataRow = regulation.eventBattleDtbl[idx.ToString()];
		if (eventBattleDataRow.gachaOneTimeAmount != 0 && int.Parse(eventBattleDataRow.eventPointIdx) > 0)
		{
			JsonRpcClient.instance.EnqueueRequest(this, "GetEventBattleGachaInfo", idx);
		}
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestEventRaidSummon(int idx)
	{
		JsonRpcClient.instance.SendRequest(this, "EventRaidSummon", idx);
	}

	public void RequestEventRaidList(int idx)
	{
		JsonRpcClient.instance.SendRequest(this, "EventRaidList", idx);
	}

	public void RequestEventRaidShare(int bid)
	{
		JsonRpcClient.instance.SendRequest(this, "EventRaidShare", bid);
	}

	public void RequestEventRaidData(int bid)
	{
		JsonRpcClient.instance.SendRequest(this, "EventRaidData", bid);
	}

	public void RequestEventRaidRankingData(int bid)
	{
		JsonRpcClient.instance.SendRequest(this, "EventRaidRankingData", bid);
	}

	public void RequestGetEventRaidReward(int bid)
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GetEventRaidReward", bid);
		JsonRpcClient.instance.EnqueueRequest(this, "EventRaidList", 3);
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestGetMarried(int cid)
	{
		JsonRpcClient.instance.SendRequest(this, "GetMarried", cid);
	}

	public void RequestGetPlugEventInfo()
	{
		JsonRpcClient.instance.SendRequest(this, "GetPlugEventInfo");
	}

	public void RequestGetPostEventReward(int menuId)
	{
		JsonRpcClient.instance.SendRequest(this, "GetPostEventReward", menuId);
	}

	public void RequestGetCommentEventReward(int articleId)
	{
		JsonRpcClient.instance.SendRequest(this, "GetCommentEventReward", articleId);
	}

	public void RequestEventBattleGachaOpen(int eventIdx, int count)
	{
		JsonRpcClient.instance.SendRequest(this, "EventBattleGachaOpen", eventIdx, count);
	}

	public void RequestEventBattleGachaReset(int eventId)
	{
		JsonRpcClient.instance.SendRequest(this, "EventBattleGachaReset", eventId);
	}

	public void RequestEventBattle(BattleData battleData, int kind = -1)
	{
		if (battleData == null)
		{
			return;
		}
		BattleData.Get();
		BattleData.Set(battleData);
		RoTroop attackerTroop = battleData.attackerTroop;
		string key = null;
		bool flag = false;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		int num = 0;
		for (int i = 0; i < attackerTroop.slots.Length; i++)
		{
			RoTroop.Slot slot = attackerTroop.slots[i];
			if (!slot.IsValid() || slot.charType == ECharacterType.Helper)
			{
				continue;
			}
			if (slot.charType == ECharacterType.Mercenary || slot.charType == ECharacterType.SuperMercenary)
			{
				dictionary2.Add(slot.userIdx.ToString(), slot.commanderId);
				key = (slot.position + 1).ToString();
				flag = true;
			}
			dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
			if (slot.charType == ECharacterType.NPCMercenary || slot.charType == ECharacterType.SuperNPCMercenary)
			{
				NPCMercenaryDataRow nPCMercenaryDataRow = regulation.npcMercenaryDtbl.Find((NPCMercenaryDataRow row) => row.unitId == slot.unitId);
				if (nPCMercenaryDataRow != null)
				{
					num = int.Parse(nPCMercenaryDataRow.id);
				}
				key = (slot.position + 1).ToString();
				flag = true;
			}
		}
		JsonRpcClient.instance.SendRequest(this, "EventBattleStart", (int)battleData.type, JObject.FromObject(dictionary), JObject.FromObject(dictionary2), kind, battleData.eventId, battleData.eventLevel, num);
		if (flag)
		{
			dictionary.Remove(key);
		}
		int num2 = Utility.GetEventBattleDeckIndex(battleData.eventId);
		string empty = string.Empty;
		if (num2 == -1)
		{
			for (int j = 0; j < ConstValue.eventBattleSaveDeckCount; j++)
			{
				empty = $"EventBattleDeck_{j}";
				if (!PlayerPrefs.HasKey(empty))
				{
					num2 = j;
					break;
				}
			}
		}
		if (num2 == -1)
		{
			Utility.ClearEventBattleDeck();
			num2 = 0;
		}
		empty = $"EventBattleDeck_{num2.ToString()}";
		PlayerPrefs.SetString(empty, $"{battleData.eventId}_{JObject.FromObject(dictionary).ToString()}");
	}

	public void RequestEventRaidBattleStart(BattleData battleData)
	{
		BattleData.Get();
		BattleData.Set(battleData);
		RoTroop attackerTroop = battleData.attackerTroop;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < attackerTroop.slots.Length; i++)
		{
			RoTroop.Slot slot = attackerTroop.slots[i];
			if (slot.IsValid())
			{
				dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
			}
		}
		JsonRpcClient.instance.SendRequest(this, "EventRaidBattleStart", (int)battleData.type, JObject.FromObject(dictionary), int.Parse(battleData.eventRaidBossId));
		PlayerPrefs.SetString("EventRaidBattleDeck", JObject.FromObject(dictionary).ToString());
	}

	public void RequestOnCamp()
	{
		JsonRpcClient.instance.EnqueueRequest(this, "BulletCharge");
		JsonRpcClient.instance.EnqueueRequest(this, "CheckBadge");
		JsonRpcClient.instance.EnqueueRequest(this, "GetWebEvent", localUser.channel);
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestLoginResult()
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GetVipBuyCount", Protocols.GetUserInfoTargetList(Protocols.UserInformationType.Recharge), 0);
		JsonRpcClient.instance.EnqueueRequest(this, "GetEventNotice");
		JsonRpcClient.instance.EnqueueRequest(this, "GetCommonNotice");
		JsonRpcClient.instance.EnqueueRequest(this, "GetChatIgnoreList");
		JsonRpcClient.instance.EnqueueRequest(this, "GetExplorationList");
		JsonRpcClient.instance.EnqueueRequest(this, "GetDispatchCommanderListFromLogin");
		JsonRpcClient.instance.EnqueueRequest(this, "GetCouponList");
		JsonRpcClient.instance.EnqueueRequest(this, "GetTutorialStep", Protocols.GetUserInfoTargetList(Protocols.UserInformationType.Tutorial));
		JsonRpcClient.instance.EnqueueRequest(this, "GetCommanderScenario");
		JsonRpcClient.instance.EnqueueRequest(this, "GetEventRemaingTime");
		JsonRpcClient.instance.EnqueueRequest(this, "GetDormitoryInfo", Protocols.GetUserInfoTargetList(Protocols.UserInformationType.DormitoryResource, Protocols.UserInformationType.DormitoryInfo, Protocols.UserInformationType.DormitoryInvenNormalDeco, Protocols.UserInformationType.DormitoryInvenAdvancedDeco, Protocols.UserInformationType.DormitoryInvenWallpaperDeco, Protocols.UserInformationType.DormitoryInvenCostumeHead, Protocols.UserInformationType.DormitoryInvenCostumeBody));
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestGetRankingReward(int type, int ridx)
	{
		JsonRpcClient.instance.SendRequest(this, "GetRankingReward", type, ridx);
	}

	public void RequestGetFirstPaymentReward()
	{
		JsonRpcClient.instance.SendRequest(this, "GetFirstPaymentReward");
	}

	public void RequestBuyPredeckSlot()
	{
		JsonRpcClient.instance.SendRequest(this, "BuyPredeckSlot");
	}

	public void RequestWorldDuelInformation()
	{
		JsonRpcClient.instance.SendRequest(this, "WorldDuelInformation");
	}

	public void RequestWorldDuelDefenderSetting(RoTroop troop)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < troop.slots.Length; i++)
		{
			RoTroop.Slot slot = troop.slots[i];
			if (slot.IsValid())
			{
				dictionary.Add((slot.position + 1).ToString(), slot.commanderId);
			}
		}
		JsonRpcClient.instance.SendRequest(this, "WorldDuelDefenderSetting", JObject.FromObject(dictionary));
	}

	public void RequesWorldDuelBuffSetting(string type)
	{
		JsonRpcClient.instance.SendRequest(this, "WorldDuelBuffSetting", type);
	}

	public void RequestWorldDuelBuffUpgrade(string type)
	{
		JsonRpcClient.instance.SendRequest(this, "WorldDuelBuffUpgrade", type);
	}

	public void RequestWorldDuelEnemyInfo()
	{
		JsonRpcClient.instance.SendRequest(this, "WorldDuelEnemyInfo");
	}

	public void RequestTranscendenceSkillUp(int cid, int slot)
	{
		JsonRpcClient.instance.SendRequest(this, "TranscendenceSkillUp", cid, slot);
	}

	public void RequestGetWeaponProgressList()
	{
		JsonRpcClient.instance.EnqueueRequest(this, "GetWeaponProgressList");
		JsonRpcClient.instance.EnqueueRequest(this, "BulletCharge");
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestWeaponProgressSlotOpen()
	{
		JsonRpcClient.instance.EnqueueRequest(this, "WeaponProgressSlotOpen");
		JsonRpcClient.instance.EnqueueRequest(this, "GetWeaponProgressList");
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestStartWeaponProgress(int slot, int mat1, int mat2, int mat3, int mat4)
	{
		PlayerPrefs.SetString("WeaponMaterial", $"{mat1},{mat2},{mat3},{mat4}");
		JsonRpcClient.instance.SendRequest(this, "StartWeaponProgress", slot, mat1, mat2, mat3, mat4);
	}

	public void RequestWeaponProgressFinish(int slot)
	{
		JsonRpcClient.instance.SendRequest(this, "WeaponProgressFinish", slot);
	}

	public void RequestWeaponProgressUseImmediateTicket(int slot)
	{
		JsonRpcClient.instance.SendRequest(this, "WeaponProgressUseImmediateTicket", slot);
	}

	public void RequestWeaponProgressBuyImmediateTicket(int slot)
	{
		JsonRpcClient.instance.SendRequest(this, "WeaponProgressBuyImmediateTicket", slot);
	}

	public void RequestDecompositionWeapon(List<int> list)
	{
		JsonRpcClient.instance.SendRequest(this, "DecompositionWeapon", list);
	}

	public void RequestUpgradeWeaponInventory()
	{
		JsonRpcClient.instance.SendRequest(this, "UpgradeWeaponInventory");
	}

	public void RequestEquipWeapon(int cid, int wno)
	{
		JsonRpcClient.instance.SendRequest(this, "EquipWeapon", cid, wno);
	}

	public void RequestReleaseWeapon(int cid, int wno)
	{
		JsonRpcClient.instance.SendRequest(this, "ReleaseWeapon", cid, wno);
	}

	public void RequestUpgradeWeapon(int wno)
	{
		JsonRpcClient.instance.SendRequest(this, "UpgradeWeapon", wno);
	}

	public void RequestTradeWeaponUpgradeTicket(int idx, int count)
	{
		JsonRpcClient.instance.SendRequest(this, "TradeWeaponUpgradeTicket", idx, count);
	}

	public void RequestComposeWeaponBox(int idx, int count)
	{
		JsonRpcClient.instance.SendRequest(this, "ComposeWeaponBox", idx, count);
	}

	public void RequestGetWeaponProgressHistory(int type)
	{
		JsonRpcClient.instance.SendRequest(this, "GetWeaponProgressHistory", type);
	}

	public void RequestInfinityBattleInformation(int fieldId, string retryStage = "")
	{
		JsonRpcClient.instance.EnqueueRequest(this, "InfinityBattleInformation", fieldId, retryStage);
		JsonRpcClient.instance.EnqueueRequest(this, "GetInfinityBattleDeck");
		JsonRpcClient.instance.SendAllQueuedRequests();
	}

	public void RequestStartInfinityBattleScenario(int fieldId)
	{
		JsonRpcClient.instance.SendRequest(this, "StartInfinityBattleScenario", fieldId);
	}

	public void RequestInfinityBattleGetReward(int fieldId, int missionIdx)
	{
		JsonRpcClient.instance.SendRequest(this, "InfinityBattleGetReward", fieldId, missionIdx);
	}

	public void RequestGetDormitoryFloorInfo()
	{
		JsonRpcClient.instance.SendRequest(this, "GetDormitoryFloorInfo");
	}

	public void RequestGetDormitoryUserFloorInfo(string uno)
	{
		JsonRpcClient.instance.SendRequest(this, "GetDormitoryUserFloorInfo", uno);
	}

	public void RequestConstructDormitoryFloor(string fno)
	{
		JsonRpcClient.instance.SendRequest(this, "ConstructDormitoryFloor", fno);
	}

	public void RequestFinishConstructDormitoryFloor(string fno, int imm)
	{
		JsonRpcClient.instance.SendRequest(this, "FinishConstructDormitoryFloor", fno, imm);
	}

	public void RequestGetDormitoryFloorDetailInfo(string fno)
	{
		JsonRpcClient.instance.SendRequest(this, "GetDormitoryFloorDetailInfo", fno);
	}

	public void RequestGetDormitoryUserFloorDetailInfo(string uno, string fno)
	{
		JsonRpcClient.instance.SendRequest(this, "GetDormitoryUserFloorDetailInfo", uno, fno);
	}

	public void RequestChangeDormitoryFloorName(string fno, string floorName)
	{
		JsonRpcClient.instance.SendRequest(this, "ChangeDormitoryFloorName", fno, floorName);
	}

	public void RequestGetDormitoryShopProductList()
	{
		JsonRpcClient.instance.SendRequest(this, "GetDormitoryShopProductList");
	}

	public void RequestGetDormitoryPoint(string cid)
	{
		JsonRpcClient.instance.SendRequest(this, "GetDormitoryPoint", cid);
	}

	public void RequestGetDormitoryPointAll(string fno)
	{
		JsonRpcClient.instance.SendRequest(this, "GetDormitoryPointAll", fno);
	}

	public void RequestBuyDormitoryShopProduct(EDormitoryItemType type, string id)
	{
		JsonRpcClient.instance.SendRequest(this, "BuyDormitoryShopProduct", type, id);
	}

	public void RequestSellDormitoryItem(EStorageType type, string id, int amount)
	{
		JsonRpcClient.instance.SendRequest(this, "SellDormitoryItem", type, id, amount);
	}

	public void RequestGetRecommendUser()
	{
		JsonRpcClient.instance.SendRequest(this, "GetRecommendUser");
	}

	public void RequestSearchDormitoryUser(string nickName)
	{
		JsonRpcClient.instance.SendRequest(this, "SearchDormitoryUser", nickName);
	}

	public void RequestGetDormitoryGuildUser()
	{
		JsonRpcClient.instance.SendRequest(this, "GetDormitoryGuildUser");
	}

	public void RequestGetDormitoryFavorUser()
	{
		JsonRpcClient.instance.SendRequest(this, "GetDormitoryFavorUser");
	}

	public void RequestAddDormitoryFavorUser(string tuno)
	{
		JsonRpcClient.instance.SendRequest(this, "AddDormitoryFavorUser", tuno);
	}

	public void RequestRemoveDormitoryFavorUser(string tuno)
	{
		JsonRpcClient.instance.SendRequest(this, "RemoveDormitoryFavorUser", tuno);
	}

	public void RequestGetDormitoryCommanderInfo()
	{
		JsonRpcClient.instance.SendRequest(this, "GetDormitoryCommanderInfo");
	}

	public void RequestArrangeDormitoryCommander(string fno, string cid)
	{
		JsonRpcClient.instance.SendRequest(this, "ArrangeDormitoryCommander", fno, cid);
	}

	public void RequestRemoveDormitoryCommander(string cid)
	{
		JsonRpcClient.instance.SendRequest(this, "RemoveDormitoryCommander", cid);
	}

	public void RequestChangeDormitoryCommanderBody(string cid, string idx)
	{
		JsonRpcClient.instance.SendRequest(this, "ChangeDormitoryCommanderBody", cid, idx);
	}

	public void RequestChangeDormitoryCommanderHead(string cid, string idx)
	{
		JsonRpcClient.instance.SendRequest(this, "ChangeDormitoryCommanderHead", cid, idx);
	}

	public void RequestBuyDormitoryHeadCostume(string idx)
	{
		JsonRpcClient.instance.SendRequest(this, "BuyDormitoryHeadCostume", idx);
	}

	public void RequestChangeDormitoryWallpaper(string fno, string idx)
	{
		JsonRpcClient.instance.SendRequest(this, "ChangeDormitoryWallpaper", fno, idx);
	}

	public void RequestArrangeDormitoryDecoration(string fno, string idx, int px, int py, int rt)
	{
		JsonRpcClient.instance.SendRequest(this, "ArrangeDormitoryDecoration", fno, idx, px, py, rt);
	}

	public void RequestEditDormitoryDecoration(string fno, int px, int py, int epx, int epy, int ert)
	{
		JsonRpcClient.instance.SendRequest(this, "EditDormitoryDecoration", fno, px, py, epx, epy, ert);
	}

	public void RequestRemoveDormitoryDecoration(string fno, int px, int py)
	{
		JsonRpcClient.instance.SendRequest(this, "RemoveDormitoryDecoration", fno, px, py);
	}

	private IEnumerator _TestNetworkMethod()
	{
		List<NetworkTestData> testList = new List<NetworkTestData>
		{
			NetworkTestData.Create("Login", JObject.FromObject(new Protocols.AuthLoginRequest
			{
				world = 1,
				userName = "Tester",
				platform = Platform.Dbros,
				deviceName = SystemInfo.deviceModel,
				deviceId = SystemInfo.deviceUniqueIdentifier,
				patchType = 1,
				osCode = Protocols.OSCode.Windows,
				osVersion = SystemInfo.operatingSystem,
				gameVersion = "0.1",
				apkFileName = "notyet",
				pushRegistrationId = "PushPush"
			})),
			NetworkTestData.Create("GetUserInformation", Protocols.GetUserInfoTargetList(Protocols.UserInformationType.Resource, Protocols.UserInformationType.Building, Protocols.UserInformationType.Upgrade, Protocols.UserInformationType.Unit, Protocols.UserInformationType.Commander, Protocols.UserInformationType.DailyCheckPoint, Protocols.UserInformationType.part))
		};
		_MakeTestDict();
		yield return null;
		for (int idx = 0; idx < testList.Count; idx++)
		{
			NetworkTestData td = testList[idx];
			JsonRpcClient.instance.SendRequest(this, td.methodName, td.paramList);
			while (!_testProtocolDict[td.methodName])
			{
				yield return null;
			}
		}
	}

	private IEnumerator OnJsonRpcRequestError(JsonRpcClient.Request request, string result, int code)
	{
		if (!Application.isPlaying)
		{
			yield break;
		}
		UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("19013"), string.Empty, Localization.Get("19014") + "\n(ErrorCode:" + code + ")", Localization.Get("5133"));
		if (uISimplePopup != null)
		{
			uISimplePopup.onClose = delegate
			{
				Application.Quit();
			};
		}
	}

	private IEnumerator OnSystemMessage(JsonRpcClient.Request request, Protocols.SystemMessage result)
	{
		if (!string.IsNullOrEmpty(result.session))
		{
			JsonRpcClient.session = result.session;
		}
		if (result.userLevel > 0)
		{
			localUser.beforeLevel = localUser.level;
			localUser.level = result.userLevel;
			if ((localUser.level == 3 || localUser.level == 5 || localUser.level == 10 || localUser.level == 15 || localUser.level == 20 || localUser.level == 30 || localUser.level == 30 || localUser.level == 40 || localUser.level == 50 || localUser.level == 60 || localUser.level == 70 || localUser.level == 71) && FB.IsInitialized)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["fb_content_id"] = "AchievedLevel";
				FB.LogAppEvent("fb_mobile_level_achieved", localUser.level, dictionary);
			}
			if (localUser.level == 3)
			{
				AdjustManager.Instance.SimpleEvent("e1hpty");
			}
			else if (localUser.level == 5)
			{
				AdjustManager.Instance.SimpleEvent("rcdmbv");
			}
			else if (localUser.level == 10)
			{
				AdjustManager.Instance.SimpleEvent("wywjrw");
			}
			else if (localUser.level == 15)
			{
				AdjustManager.Instance.SimpleEvent("4zozc7");
			}
			if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
			{
				if (localUser.level >= 6)
				{
					SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQAA");
				}
				if (localUser.level >= 12)
				{
					SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQAQ");
				}
				if (localUser.level >= 24)
				{
					SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQAg");
				}
				if (localUser.level >= 36)
				{
					SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQAw");
				}
				if (localUser.level >= 50)
				{
					SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQBA");
				}
			}
		}
		if (result.commanderLevel > 0)
		{
			RoCommander roCommander = localUser.FindCommander(result.commanderId);
			roCommander.level = result.commanderLevel;
		}
		if (result.getMissionId > 0)
		{
			RoMission roMission = localUser.FindMission(result.getMissionId.ToString());
			roMission.combleted = true;
			roMission.conditionCount = roMission.count;
		}
		if (result.gidx == 0)
		{
			if (localUser.guildInfo != null)
			{
				localUser.guildInfo = null;
				if (JsonRpcClient.instance.systemRequestName == "GuildMemberList")
				{
					instance.RequestGuildList();
				}
				for (int i = 0; i < localUser.commanderList.Count; i++)
				{
					RoCommander roCommander2 = localUser.commanderList[i];
					if (roCommander2.scramble || roCommander2.occupation)
					{
						roCommander2.role = "N";
					}
				}
				localUser.ResetDispatchPossible();
			}
		}
		else if (localUser.guildInfo == null)
		{
			localUser.guildInfo = new Protocols.UserInformationResponse.UserGuild();
		}
		if (result.systemCheck != null)
		{
			_ = string.Empty;
			UISimplePopup uISimplePopup = UISimplePopup.CreateOK(message: PlayerPrefs.GetString("Language") switch
			{
				"S_Kr" => result.systemCheck.message.ko, 
				"S_En" => result.systemCheck.message.en, 
				"S_Jp" => result.systemCheck.message.jp, 
				"S_Beon" => result.systemCheck.message.tw, 
				"S_Gan" => result.systemCheck.message.cn, 
				"S_Rus" => result.systemCheck.message.ru, 
				_ => result.systemCheck.message.en, 
			}, localization: false, title: Localization.Get("7142"), subMessage: null, button: Localization.Get("5133"));
			if (uISimplePopup != null)
			{
				uISimplePopup.onClose = delegate
				{
					Application.Quit();
				};
			}
			yield break;
		}
		if (result.noticeList != null)
		{
			if (result.noticeList.realtime != null)
			{
				localUser.AddNotice(result.noticeList.realtime.idx, result.noticeList.realtime.contents);
			}
			if (result.noticeList.chat == null)
			{
			}
		}
		localUser.RefreshGoodsFromNetwork(result.resource);
		if (result.carnival1 > 0)
		{
			localUser.badgeCarnivalComplete[1] = true;
		}
		if (result.carnival2 > 0)
		{
			localUser.badgeCarnivalComplete[2] = true;
		}
		if (result.carnival3 > 0)
		{
			localUser.badgeCarnivalComplete[3] = true;
		}
		if (result.resetRemain != 0)
		{
			if (localUser.resetTimeData == null)
			{
				localUser.resetTimeData = TimeData.Create();
			}
			localUser.resetTimeData.SetByDuration(result.resetRemain);
			ReqeustRenewUserGameData();
		}
		if (UIManager.instance.world == null || !UIManager.instance.world.existMetroBank || !UIManager.instance.world.metroBank.isActive)
		{
			UIManager.instance.RefreshOpenedUI();
		}
	}

	private JObject _ConvertRequestToObject(JsonRpcClient.Request request)
	{
		if (request == null)
		{
			return null;
		}
		return JsonConvert.DeserializeObject<JObject>(request.jsonString);
	}

	private string _FindRequestProperty(JsonRpcClient.Request request, string propName)
	{
		JObject jObject = _ConvertRequestToObject(request);
		string result = null;
		try
		{
			result = jObject["params"][propName].ToString();
		}
		catch (Exception)
		{
		}
		return result;
	}

	private string _FindRequestProperty(JsonRpcClient.Request request, string propName, string propName1)
	{
		JObject jObject = _ConvertRequestToObject(request);
		string result = null;
		try
		{
			result = jObject["params"][propName][propName1].ToString();
		}
		catch (Exception)
		{
		}
		return result;
	}

	public static string Base64Decode(string data)
	{
		try
		{
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			Decoder decoder = uTF8Encoding.GetDecoder();
			byte[] array = Convert.FromBase64String(data);
			int charCount = decoder.GetCharCount(array, 0, array.Length);
			char[] array2 = new char[charCount];
			decoder.GetChars(array, 0, array.Length, array2, 0);
			return new string(array2);
		}
		catch (Exception)
		{
			return data;
		}
	}

	private void OnLoginSuccess()
	{
		if (PlayerPrefs.GetInt("ProloguePlay") == 0)
		{
			PlayerPrefs.SetInt("ProloguePlay", 1);
			if (instance.localUser.isTutorialSkip)
			{
				Loading.Load(Loading.WorldMap);
			}
			else
			{
				Loading.Load(Loading.Prologue);
			}
		}
		else
		{
			Loading.Load(Loading.WorldMap);
		}
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1201", false, true)]
	public void Login(JObject send)
	{
	}

	private IEnumerator LoginResult(JsonRpcClient.Request request, string result, string sess, Protocols.UserInformationResponse info)
	{
		JsonRpcClient.session = sess;
		loginTime = (DateTime.Now - new DateTime(0L)).TotalSeconds;
		localUser.InitData();
		localUser.FromNetwork(info);
		localUser.uno = info.uno;
		localUser.lastClearStage = info.stage;
		localUser.SetSweepClearState(info.sweepClearData);
		localUser.SetDonHaveCommCostume(info.donHaveCommCostumeData);
		localUser.SetGroupCompleteData(info.completeRewardGroupIdx);
		localUser.tempBullet = localUser.bullet;
		if (localUser.statistics.vipShop == 1 && localUser.statistics.vipShopResetTime <= 0)
		{
			localUser.statistics.isBuyVipShop = true;
		}
		else
		{
			localUser.statistics.isBuyVipShop = false;
		}
		localUser.resetTimeData = TimeData.Create();
		int resetRemain = info.resetRemain;
		localUser.resetTimeData.SetByDuration(resetRemain);
		GameSetting.instance.Notification = info.notification;
		_TestRewardList();
		Localization.language = GameSetting.instance.language;
		_CheckReceiveTestData("Login");
		string @string = PlayerPrefs.GetString("Uno");
		if (!string.IsNullOrEmpty(@string) && localUser.uno != @string)
		{
			PlayerPrefs.DeleteKey("StageDeck");
			PlayerPrefs.DeleteKey("DuelDeck");
			PlayerPrefs.DeleteKey("RaidDeck");
			PlayerPrefs.DeleteKey("AnnihilationDeck");
			PlayerPrefs.DeleteKey("Carnival");
			PlayerPrefs.DeleteKey("MercenaryDeck");
			PlayerPrefs.DeleteKey("WaveDuelDeck");
			PlayerPrefs.DeleteKey("CooperateBattleDeck");
			PlayerPrefs.DeleteKey("bgm_Volume");
			PlayerPrefs.DeleteKey("voice_Volume");
			PlayerPrefs.DeleteKey("WorldDuelDeck");
			PlayerPrefs.DeleteKey("WeaponMaterial");
			localUser.ResetCommanderCostume();
			localUser.ResetSweepDeck();
		}
		PlayerPrefs.SetString("Uno", localUser.uno);
		localUser.SetCommanderTypeAndIdx();
		RequestLoginResult();
		yield break;
	}

	private IEnumerator LoginError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 10015:
			UISimplePopup.CreateOK(localization: true, "19074", "19075", null, "1001");
			break;
		case 19999:
			UISimplePopup.CreateOK(localization: true, "1310", "19510", null, "1001");
			break;
		default:
			UISimplePopup.CreateDebugOK(code.ToString(), null, "Confirm");
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1213", true, true)]
	public void SignUp(string uid, string pwd, int plfm, int ch)
	{
	}

	private IEnumerator SignUpResult(JsonRpcClient.Request request, string result, string uid)
	{
		_CheckReceiveTestData("SignUp");
		string text = _FindRequestProperty(request, "pwd");
		PlayerPrefs.SetString("MemberID", uid);
		PlayerPrefs.SetString("MemberPW", text);
		instance.RequestSignIn(uid, text);
		yield break;
	}

	private IEnumerator SignUpError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 10014:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19033"));
			break;
		case 20014:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7054"));
			break;
		default:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), "Error code:" + code);
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1213", true, true)]
	public void GuestSignUp(int plfm, int ch)
	{
	}

	private IEnumerator GuestSignUpResult(JsonRpcClient.Request request, string result, string uid)
	{
		_CheckReceiveTestData("GuestSignUp");
		PlayerPrefs.SetString("GuestID", uid);
		instance.RequestGuestSignIn(uid);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1215", true, true)]
	public void SignIn(string uid, string pwd, int plfm, int ch)
	{
	}

	private IEnumerator SignInResult(JsonRpcClient.Request request, string result, int mIdx, string tokn, int srv)
	{
		_CheckReceiveTestData("SignIn");
		PlayerPrefs.SetString("MemberID", _FindRequestProperty(request, "uid"));
		PlayerPrefs.SetString("MemberPW", _FindRequestProperty(request, "pwd"));
		PlayerPrefs.SetInt("MemberPlatform", int.Parse(_FindRequestProperty(request, "plfm")));
		localUser.id = _FindRequestProperty(request, "uid");
		localUser.mIdx = mIdx;
		localUser.tokn = tokn;
		localUser.world = srv;
		localUser.platform = (Platform)int.Parse(_FindRequestProperty(request, "plfm"));
		AdjustManager.Instance.SimpleEvent("tqt4j6");
		LocalStorage.SaveLoginData(_FindRequestProperty(request, "uid"), _FindRequestProperty(request, "pwd"), int.Parse(_FindRequestProperty(request, "plfm")));
		instance.bLogin = true;
		yield break;
	}

	private IEnumerator SignInError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 10016:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19058"));
			break;
		case 10018:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19059"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1215", true, true)]
	public void GuestSignIn(string uid, int plfm, int ch)
	{
	}

	private IEnumerator GuestSignInResult(JsonRpcClient.Request request, string result, int mIdx, string tokn, int srv)
	{
		_CheckReceiveTestData("GuestSignIn");
		PlayerPrefs.SetString("MemberID", _FindRequestProperty(request, "uid"));
		PlayerPrefs.SetString("MemberPW", null);
		PlayerPrefs.SetInt("MemberPlatform", int.Parse(_FindRequestProperty(request, "plfm")));
		localUser.id = _FindRequestProperty(request, "uid");
		localUser.mIdx = mIdx;
		localUser.tokn = tokn;
		localUser.world = srv;
		localUser.platform = (Platform)int.Parse(_FindRequestProperty(request, "plfm"));
		AdjustManager.Instance.SimpleEvent("tqt4j6");
		LocalStorage.SaveLoginData(_FindRequestProperty(request, "uid"), null, int.Parse(_FindRequestProperty(request, "plfm")));
		instance.bLogin = true;
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1215", true, true)]
	public void FBSignIn(string tokn, int plfm, int ch)
	{
	}

	private IEnumerator FBSignInResult(JsonRpcClient.Request request, string result, int mIdx, string tokn, int srv)
	{
		_CheckReceiveTestData("SignIn");
		PlayerPrefs.SetString("MemberID", localUser.platformUserInfo);
		PlayerPrefs.SetString("MemberPW", null);
		PlayerPrefs.SetInt("MemberPlatform", int.Parse(_FindRequestProperty(request, "plfm")));
		localUser.mIdx = mIdx;
		localUser.tokn = tokn;
		localUser.world = srv;
		localUser.platform = (Platform)int.Parse(_FindRequestProperty(request, "plfm"));
		AdjustManager.Instance.SimpleEvent("tqt4j6");
		LocalStorage.SaveLoginData(localUser.platformUserInfo, null, int.Parse(_FindRequestProperty(request, "plfm")));
		instance.bLogin = true;
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1215", true, true)]
	public void GoogleSignIn(string tokn, int plfm, int ch)
	{
	}

	private IEnumerator GoogleSignInResult(JsonRpcClient.Request request, string result, int mIdx, string tokn, int srv)
	{
		_CheckReceiveTestData("SignIn");
		PlayerPrefs.SetString("MemberID", localUser.platformUserInfo);
		PlayerPrefs.SetString("MemberPW", null);
		PlayerPrefs.SetInt("MemberPlatform", int.Parse(_FindRequestProperty(request, "plfm")));
		localUser.mIdx = mIdx;
		localUser.tokn = tokn;
		localUser.world = srv;
		localUser.platform = (Platform)int.Parse(_FindRequestProperty(request, "plfm"));
		AdjustManager.Instance.SimpleEvent("tqt4j6");
		LocalStorage.SaveLoginData(localUser.platformUserInfo, null, int.Parse(_FindRequestProperty(request, "plfm")));
		instance.bLogin = true;
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1214", true, true)]
	public void ChangeMembership(string uid, string pwd, int plfm, string puid, int ch)
	{
	}

	private IEnumerator ChangeMembershipResult(JsonRpcClient.Request request, string result)
	{
		_CheckReceiveTestData("ChangeMembership");
		string text = _FindRequestProperty(request, "uid");
		string text2 = _FindRequestProperty(request, "pwd");
		PlayerPrefs.SetString("MemberID", text);
		PlayerPrefs.SetString("MemberPW", text2);
		LocalStorage.RemoveLoginData(PlayerPrefs.GetString("GuestID"));
		PlayerPrefs.SetString("GuestID", null);
		instance.RequestSignIn(text, text2);
		yield break;
	}

	private IEnumerator ChangeMembershipError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 10014:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19033"));
			break;
		case 20014:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7054"));
			break;
		default:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), "Error code:" + code);
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1214", true, true)]
	public void ChangeMembershipOpenPlatform(string tokn, Platform plfm, string puid, int ch)
	{
	}

	private IEnumerator ChangeMembershipOpenPlatformResult(JsonRpcClient.Request request, string result)
	{
		string token = _FindRequestProperty(request, "tokn");
		Platform platform = (Platform)int.Parse(_FindRequestProperty(request, "plfm"));
		PlayerPrefs.SetString("MemberID", localUser.platformUserInfo);
		PlayerPrefs.SetString("MemberPW", null);
		PlayerPrefs.SetInt("MemberPlatform", (int)platform);
		LocalStorage.RemoveLoginData(PlayerPrefs.GetString("GuestID"));
		PlayerPrefs.SetString("GuestID", null);
		LocalStorage.SaveLoginData(localUser.platformUserInfo, null, (int)platform);
		switch (platform)
		{
		case Platform.FaceBook:
			RequestFBSignIn(token);
			break;
		case Platform.Google:
			RequestGoogleSignIn(token);
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1116", true, true)]
	public void ServerStatus(string mIdx, string tokn, int ch)
	{
	}

	private IEnumerator ServerStatusResult(JsonRpcClient.Request request, Protocols.ServerData result)
	{
		_CheckReceiveTestData("ServerStatus");
		M01_Title m01_Title = UnityEngine.Object.FindObjectOfType(typeof(M01_Title)) as M01_Title;
		m01_Title.SetServerStatus(result);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8104", true, true)]
	public void UserTerm(int ch)
	{
	}

	private IEnumerator UserTermResult(JsonRpcClient.Request request, string result, string member, string wemade)
	{
		_CheckReceiveTestData("UserTerm");
		M01_Title m01_Title = UnityEngine.Object.FindObjectOfType(typeof(M01_Title)) as M01_Title;
		m01_Title.obj_Agreement.SetActive(value: true);
		m01_Title.obj_Login.SetActive(value: false);
		m01_Title.obj_Agreement.GetComponent<UIAgreement>().SetText(wemade, member);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1103", true, true)]
	public void GetUserInformation(List<string> type)
	{
	}

	private IEnumerator GetUserInformationResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		_CheckReceiveTestData("GetUserInformation");
		localUser.FromNetwork(result);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1103", true, true)]
	public void GetVipBuyCount(List<string> type, int renewType)
	{
	}

	private IEnumerator GetVipBuyCountResult(JsonRpcClient.Request request, string result, List<Protocols.UserInformationResponse.VipRechargeData> rchg)
	{
		if (rchg.Count == 0)
		{
			localUser.resourceRechargeList.Clear();
			localUser.stageRechargeList.Clear();
		}
		for (int i = 0; i < rchg.Count; i++)
		{
			VipRechargeDataRow vipRechargeDataRow = regulation.vipRechargeDtbl[rchg[i].idx.ToString()];
			if (vipRechargeDataRow.type != 2)
			{
				string key = rchg[i].idx.ToString();
				localUser.resourceRechargeList[key] = rchg[i].count;
			}
			else
			{
				string key2 = rchg[i].mid.ToString();
				localUser.stageRechargeList[key2] = rchg[i].count;
			}
		}
		int num = int.Parse(_FindRequestProperty(request, "renewType"));
		if (num != 0)
		{
			UIManager.instance.world.mainCommand.OpenVipRechargePopUp((EVipRechargeType)num);
		}
		_CheckReceiveTestData("GetUserInformation");
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1202", true, true)]
	public void Logout()
	{
	}

	private IEnumerator LogoutResult(JsonRpcClient.Request request, string result)
	{
		_CheckReceiveTestData("Logout");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1408", true, true)]
	public void ChangeNickname(string unm)
	{
	}

	private IEnumerator ChangeNicknameResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		string nickname = _FindRequestProperty(request, "unm");
		localUser.nickname = nickname;
		localUser.RefreshGoodsFromNetwork(result.goodsInfo);
		_CheckReceiveTestData("ChangeNicknameResult");
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator ChangeNicknameError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 20014:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7054"));
			break;
		case 20005:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7145"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4214", true, true)]
	public void CommanderLevelUp(int cid, int cnt, string ctt)
	{
	}

	private IEnumerator CommanderLevelUpResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		if (result.commanderInfo != null)
		{
			foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> item in result.commanderInfo)
			{
				_ = item.Value;
				RoCommander roCommander = localUser.FindCommander(item.Value.id);
				roCommander.aExp = item.Value.exp;
			}
		}
		localUser.RefreshItemFromNetwork(result.itemData);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator CommanderLevelUpError(JsonRpcClient.Request request, string result, int code, string message)
	{
		switch (code)
		{
		case 20001:
			UISimplePopup.CreateOK(localization: true, "10044", "10045", null, "1001");
			break;
		case 20003:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("8021"));
			break;
		case 30003:
		{
			if (request == null)
			{
			}
			string value = _FindRequestProperty(request, "cid");
			if (string.IsNullOrEmpty(value))
			{
			}
			break;
		}
		default:
			UISimplePopup.CreateDebugOK(code.ToString(), message, "확인");
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4218", true, true)]
	public void CommanderRankUp(int cid)
	{
	}

	private IEnumerator CommanderRankUpResult(JsonRpcClient.Request request, Protocols.CommanderRankUpResponse result)
	{
		CommanderCompleteType type = CommanderCompleteType.Undefined;
		localUser.gold = result.rsoc.gold;
		if (result.comm != null)
		{
			SoundManager.PlaySFX("SE_Promotion_001");
			foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> keyval in result.comm)
			{
				UIManager.instance.EnableCameraTouchEvent(isEnable: false);
				Protocols.UserInformationResponse.Commander data = keyval.Value;
				RoCommander roCommander = localUser.FindCommander(keyval.Value.id);
				roCommander.rank = keyval.Value.rank;
				type = ((roCommander.state != ECommanderState.Nomal) ? CommanderCompleteType.Recruit : CommanderCompleteType.Promotion);
				roCommander.state = ECommanderState.Nomal;
				roCommander.aMedal = result.medl[keyval.Value.id];
				if (type == CommanderCompleteType.Promotion)
				{
					UICommanderDetail commanderDetail = UIManager.instance.world.commanderDetail;
					UISetter.SetActive(commanderDetail.rankUpAnimation.gameObject, active: true);
					commanderDetail.rankUpAnimation.Play("CommanderRanklUp_Start");
					while (commanderDetail.rankUpAnimation.IsPlaying("CommanderRanklUp_Start"))
					{
						yield return null;
					}
					UISetter.SetActive(commanderDetail.rankUpAnimation.gameObject, active: false);
				}
				UICommanderComplete complete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				complete.Init(type, roCommander.id);
				if (keyval.Value.haveCostume != null && keyval.Value.haveCostume.Count > 0)
				{
					roCommander.haveCostumeList = keyval.Value.haveCostume;
				}
				if (roCommander.transcendence == null || roCommander.transcendence.Count == 0)
				{
					roCommander.transcendence = new List<int> { 0, 0, 0, 0 };
				}
			}
			if (type == CommanderCompleteType.Promotion)
			{
				if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED && localUser.commanderList.FindAll((RoCommander row) => (int)row.rank == 5).Count >= 5)
				{
					SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQEA");
				}
				if (GameCenterManager.IsPlayerAuthenticated && localUser.commanderList.FindAll((RoCommander row) => (int)row.rank == 5).Count >= 5)
				{
					GameCenterManager.SubmitAchievement(100f, "CgkIj7Xpxu4GEAIQEA");
				}
			}
		}
		UIManager.instance.RefreshOpenedUI();
	}

	private IEnumerator CommanderRankUpError(JsonRpcClient.Request request, string result, int code, string message)
	{
		switch (code)
		{
		case 20001:
			UISimplePopup.CreateOK(localization: true, "5838", "5086", null, "1001");
			break;
		case 30003:
		{
			if (request == null)
			{
			}
			string text = _FindRequestProperty(request, "cid");
			if (!string.IsNullOrEmpty(text))
			{
				RequestCommanderRankUpImmediate(text);
			}
			break;
		}
		default:
			UISimplePopup.CreateDebugOK(code.ToString(), message, Localization.Get("5838"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4220", true, true)]
	public void CommanderRankUpImmediate(int cid)
	{
	}

	private IEnumerator CommanderRankUpImmediateResult(JsonRpcClient.Request request, string result, int cash, int medl, Protocols.SimpleCommanderInfo comm)
	{
		string text = _FindRequestProperty(request, "cid");
		if (!string.IsNullOrEmpty(text))
		{
			RoCommander roCommander = localUser.FindCommander(text);
			roCommander.rank = (int)roCommander.rank + 1;
			roCommander.rankUpTime.SetInvalidValue();
		}
		localUser.cash = cash;
		localUser.medal = medl;
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator CommanderRankUpImmediateError(JsonRpcClient.Request request, string result, int code, string message)
	{
		UISimplePopup.CreateDebugOK(code.ToString(), message, "확인");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4124", true, true)]
	public void CommanderSkillLevelUp(int cid, int sidx, int cnt)
	{
	}

	private IEnumerator CommanderSkillLevelUpResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		string text = _FindRequestProperty(request, "cid");
		string id = _FindRequestProperty(request, "sidx");
		int cnt = int.Parse(_FindRequestProperty(request, "cnt"));
		if (!string.IsNullOrEmpty(text))
		{
			RoCommander roCommander = localUser.FindCommander(text);
			roCommander.SkillLevelUp(_ConvertStringToInt(id), cnt);
		}
		localUser.RefreshGoodsFromNetwork(result.goodsInfo);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4304", true, true)]
	public void CommanderClassUp(int cid)
	{
	}

	private IEnumerator CommanderClassUpResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		if (result.goodsInfo != null)
		{
			localUser.RefreshGoodsFromNetwork(result.goodsInfo);
		}
		if (result.partData != null)
		{
			localUser.RefreshPartFromNetwork(result.partData);
		}
		if (result.commanderInfo != null)
		{
			SoundManager.PlaySFX("SE_Upgrade_001");
			foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> item in result.commanderInfo)
			{
				Protocols.UserInformationResponse.Commander value = item.Value;
				RoCommander roCommander = localUser.FindCommander(item.Value.id);
				roCommander.cls = item.Value.cls;
				UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				uICommanderComplete.Init(CommanderCompleteType.ClassUp, roCommander.id);
				RoUnit roUnit = localUser.FindUnit(roCommander.unitId);
				roUnit.cls = value.cls;
				roUnit.level = roCommander.level;
			}
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "5110", true, true)]
	public void GetRecruitCommanderList()
	{
	}

	private IEnumerator GetRecruitCommanderListResult(JsonRpcClient.Request request, Protocols.RecruitCommanderListResponse result)
	{
		_RefreshRecruitList(result);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "5211", true, true)]
	public void RecruitCommander(int cid)
	{
	}

	private IEnumerator RecruitCommanderResult(JsonRpcClient.Request request, Protocols.RecruitCommanderResponse result)
	{
		string text = _FindRequestProperty(request, "cid");
		if (!string.IsNullOrEmpty(text))
		{
			RoRecruit.Entry entry = localUser.recruit.Find(text);
			if (entry != null)
			{
				entry.recruited = true;
				RoCommander roCommander = localUser.FindCommander(text);
				CommanderCompleteType type = ((roCommander.state != ECommanderState.Nomal) ? CommanderCompleteType.Recruit : CommanderCompleteType.Transmission);
				roCommander.state = ECommanderState.Nomal;
				roCommander.aMedal = result.commander.medal;
				UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				uICommanderComplete.Init(type, roCommander.id);
				localUser.gold = (int)Math.Min(result.gold, 2147483647L);
				localUser.honor = result.honor;
				UIManager.instance.RefreshOpenedUI();
			}
		}
		yield break;
	}

	private IEnumerator RecruitCommanderError(JsonRpcClient.Request request, string result, int code, string message)
	{
		switch (code)
		{
		case 20001:
			UISimplePopup.CreateDebugOK("자원부족", result, "확인");
			break;
		}
		yield break;
	}

	private void _RefreshRecruitList(Protocols.RecruitCommanderListResponse data)
	{
		RoRecruit recruit = localUser.recruit;
		recruit.refreshTime.SetByDuration(data.remainTime);
		recruit.entryList.Clear();
		List<RoRecruit.Entry> destList = recruit.entryList;
		data.list.ForEach(delegate(Protocols.RecruitCommanderListResponse.Commander commander)
		{
			RoRecruit.Entry entry = RoRecruit.Entry.Create(commander.id, 1, 1, 1, 0, 0, 0, new List<int>());
			entry.recruited = commander.recruited;
			entry.exist = localUser.FindCommander(commander.id) != null;
			entry.gold = commander.gold;
			entry.honor = commander.honor;
			entry.delayTime.SetByDuration(commander.waitTime);
			destList.Add(entry);
		});
	}

	private void _RefreshRecruitList(Protocols.RecruitCommanderListDictResponse data)
	{
		RoRecruit recruit = localUser.recruit;
		recruit.refreshTime.SetByDuration(data.remainTime);
		recruit.entryList.Clear();
		List<RoRecruit.Entry> entryList = recruit.entryList;
		foreach (Protocols.RecruitCommanderListDictResponse.Commander value in data.list.Values)
		{
			RoRecruit.Entry entry = RoRecruit.Entry.Create(value.id, 1, 1, 1, 0, 0, 0, new List<int>());
			entry.recruited = value.recruited;
			entry.exist = localUser.FindCommander(value.id) != null;
			entry.gold = value.gold;
			entry.honor = value.honor;
			entry.delayTime.SetByDuration(value.waitTime);
			entryList.Add(entry);
		}
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "5215", true, true)]
	public void RecruitCommanderDelay(int cid, int pos)
	{
	}

	private IEnumerator RecruitCommanderDelayResult(JsonRpcClient.Request request, Protocols.RecruitCommanderDelayResponse result)
	{
		string text = _FindRequestProperty(request, "cid");
		if (!string.IsNullOrEmpty(text))
		{
			RoRecruit.Entry entry = localUser.recruit.Find(text);
			entry.delayTime.SetByDuration(result.wait);
			localUser.gold = result.gold;
			localUser.cash = result.cash;
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "5217", true, true)]
	public void CommanderDelayCancle(int cid)
	{
	}

	private IEnumerator CommanderDelayCancleResult(JsonRpcClient.Request request, string result)
	{
		if (!string.Equals(result, "false"))
		{
			string text = _FindRequestProperty(request, "cid");
			if (!string.IsNullOrEmpty(text))
			{
				RoRecruit.Entry entry = localUser.recruit.Find(text);
				entry.delayTime.SetInvalidValue();
				UIManager.instance.RefreshOpenedUI();
			}
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4301", true, true)]
	public void UnitLevelUp(int idx)
	{
	}

	private IEnumerator UnitLevelUpResult(JsonRpcClient.Request request, Protocols.UnitLevelUpResponse result)
	{
		localUser.gold = (int)Math.Min(result.gold, 2147483647L);
		if (result.blueprintArmy >= 0)
		{
			localUser.blueprintArmy = result.blueprintArmy;
		}
		if (result.blueprintNavy >= 0)
		{
			localUser.blueprintNavy = result.blueprintNavy;
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator UnitLevelUpError(JsonRpcClient.Request request, string result, int code, string message)
	{
		UISimplePopup.CreateDebugOK(code.ToString(), message, "확인");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4302", true, true)]
	public void UnitLevelUpImmediate(int idx)
	{
	}

	private IEnumerator UnitLevelUpImmediateResult(JsonRpcClient.Request request, string result, int cash)
	{
		string text = _FindRequestProperty(request, "idx");
		if (!string.IsNullOrEmpty(text))
		{
			RoUnit roUnit = localUser.FindUnit(text);
			roUnit.trainingTime.SetInvalidValue();
			localUser.cash = cash;
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator UnitLevelUpImmediateError(JsonRpcClient.Request request, string result, int code, string message)
	{
		UISimplePopup.CreateDebugOK(code.ToString(), message, "확인");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4303", true, true)]
	public void GetUnitResearchList()
	{
	}

	private IEnumerator GetUnitResearchListResult(JsonRpcClient.Request request, List<Protocols.GetUnitResearchListResponse> result)
	{
		result.ForEach(delegate(Protocols.GetUnitResearchListResponse data)
		{
			RoUnit roUnit = localUser.FindUnit(data.id.ToString());
			roUnit.trainingTime.SetByDuration(data.remainTime);
		});
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator GetUnitResearchListError(JsonRpcClient.Request request, string result, int code, string message)
	{
		UISimplePopup.CreateDebugOK(code.ToString(), message, "확인");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4304", true, true)]
	public void UnitUpgrade(int idx)
	{
	}

	private IEnumerator UnitUpgradeResult(JsonRpcClient.Request request, Protocols.UnitUpgradeResponse result)
	{
		localUser.gold = result.goodsInfo.gold;
		localUser.blueprintArmy = result.goodsInfo.blueprintArmy;
		foreach (Protocols.UserInformationResponse.PartData partDatum in result.partData)
		{
			localUser.SetUserPart(partDatum.idx, partDatum.count);
		}
		foreach (KeyValuePair<string, Protocols.UnitUpgradeResponse.Unit> item in result.unitInfo)
		{
			Protocols.UnitUpgradeResponse.Unit value = item.Value;
			RoUnit roUnit = localUser.FindUnit(item.Key);
			roUnit.level = value.level;
			UIManager.instance.world.unitUpgradeComplete.Init(roUnit);
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator UnitUpgradeError(JsonRpcClient.Request request, string result, int code, string message)
	{
		UISimplePopup.CreateDebugOK(code.ToString(), message, "확인");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "5119", true, true)]
	public void GetTroopInformation(int cid)
	{
	}

	private IEnumerator GetTroopInformationResult(JsonRpcClient.Request request, Protocols.UserInformationResponse.Commander result)
	{
		localUser.AddOrRefreshCommanderFromNetwork(result);
		_CheckReceiveTestData("GetTroopInfo");
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "5218", true, true)]
	public void ChangeTroopNickname(int cid, string name)
	{
	}

	private IEnumerator ChangeTroopNicknameResult(JsonRpcClient.Request request, string result)
	{
		string commanderId = _FindRequestProperty(request, "cid");
		string nickname = _FindRequestProperty(request, "name");
		RoTroop roTroop = localUser.FindTroopByCommanderId(commanderId);
		roTroop.nickname = nickname;
		_CheckReceiveTestData("TroopChangeNickname");
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator ChangeTroopNicknameError(JsonRpcClient.Request request, string result, int code, string message)
	{
		if (code == 90001)
		{
		}
		UISimplePopup.CreateOK(localization: false, "ChangeTroopNicknameError", code.ToString(), message, "확인");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2201", true, true)]
	public void WorldMapInformation(int world)
	{
	}

	private IEnumerator WorldMapInformationResult(JsonRpcClient.Request request, string result, List<Protocols.WorldMapInformationResponse> stage, int rwd)
	{
		string id = _FindRequestProperty(request, "world");
		RoWorldMap worldMap = localUser.FindWorldMap(id);
		stage.ForEach(delegate(Protocols.WorldMapInformationResponse data)
		{
			RoWorldMap.Stage stage2 = worldMap.FindStage(data.stageId);
			stage2.clear = true;
			stage2.star = data.star;
			stage2.clearCount = data.clearCount;
		});
		worldMap.rwd = ((rwd != 0) ? true : false);
		worldMap.RefreshByClearCount();
		_CheckReceiveTestData("WorldMapInformation");
		UIWorldMap worldMap2 = UIManager.instance.world.worldMap;
		worldMap2._Set(worldMap);
		if (!worldMap2.isActive)
		{
			worldMap2.Open();
		}
		worldMap2.SetBGM();
		worldMap2.StartStageNavigation();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2209", true, true)]
	public void WorldMapReward(int world)
	{
	}

	private IEnumerator WorldMapRewardResult(JsonRpcClient.Request request, Protocols.WorldMapReward result)
	{
		if (result.commanderData != null)
		{
			foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> commanderDatum in result.commanderData)
			{
				RoCommander roCommander = localUser.FindCommander(commanderDatum.Value.id);
				CommanderCompleteType type = ((roCommander.state == ECommanderState.Nomal) ? CommanderCompleteType.Transmission : CommanderCompleteType.WorldMapReward);
				roCommander.state = ECommanderState.Nomal;
				UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				uICommanderComplete.Init(type, roCommander.id);
				if (commanderDatum.Value.haveCostume != null && commanderDatum.Value.haveCostume.Count > 0)
				{
					roCommander.haveCostumeList = commanderDatum.Value.haveCostume;
				}
			}
			UIManager.instance.world.worldMap.currentWorldMap.rwd = true;
		}
		if (result.medalData != null)
		{
			foreach (KeyValuePair<string, int> medalDatum in result.medalData)
			{
				RoCommander roCommander2 = localUser.FindCommander(medalDatum.Key);
				CommanderCompleteType type2 = ((roCommander2.state == ECommanderState.Nomal) ? CommanderCompleteType.Transmission : CommanderCompleteType.WorldMapReward);
				roCommander2.state = ECommanderState.Nomal;
				roCommander2.aMedal = medalDatum.Value;
				UICommanderComplete uICommanderComplete2 = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				uICommanderComplete2.Init(type2, roCommander2.id);
			}
			UIManager.instance.world.worldMap.currentWorldMap.rwd = true;
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3231", true, true)]
	public void WorldMapStageStart(int type, JObject deck, JObject gdp, int ucash, int mid, int np)
	{
	}

	private IEnumerator WorldMapStageStartResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.Resource rsoc, List<Protocols.RewardInfo.RewardData> reward)
	{
		localUser.useBullet = true;
		localUser.RefreshGoodsFromNetwork(rsoc);
		BattleData battleData = BattleData.Get();
		battleData.rewardItems = reward;
		BattleData.Set(battleData);
		_CheckReceiveTestData("TEST_WorldMapStageBattleStart");
		Loading.Load(Loading.Battle);
		yield break;
	}

	private IEnumerator WorldMapStageStartError(JsonRpcClient.Request request, string result, int code)
	{
		if (UIManager.instance.battle != null)
		{
			if (code == 21006 || code == 21007)
			{
				UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, string.Format(Localization.Get("10000009"), Localization.Get("4006")), Localization.Get("1001"));
			}
		}
		else
		{
			StartCoroutine(OnJsonRpcRequestError(request, result, code));
		}
		yield break;
	}

	private IEnumerator TEST_WorldMapStageBattleStartError(JsonRpcClient.Request request, string result, int code, string message)
	{
		switch (code)
		{
		}
		UISimplePopup.CreateOK(localization: false, "WorldMapStageBattleStart", code.ToString(), message, "확인");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6112", true, true)]
	public void DailyBonusCheck()
	{
	}

	private IEnumerator DailyBonusCheckResult(JsonRpcClient.Request request, Protocols.DailyBonusCheckResponse result)
	{
		localUser.RefreshDailyBonusFromNetwork(result);
		_CheckReceiveTestData("DailyBonusCheck");
		UIDailyBonus dailyBonus = UIManager.instance.world.dailyBonus;
		if (!result.received)
		{
			dailyBonus.InitAndOpenDailyBonus();
		}
		else
		{
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6213", true, true)]
	public void DailyBonusReceive()
	{
	}

	private IEnumerator DailyBonusReceiveResult(JsonRpcClient.Request request, Protocols.RewardInfo reward)
	{
		if (reward.commander != null)
		{
			foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> item in reward.commander)
			{
				Protocols.UserInformationResponse.Commander value = item.Value;
				RoCommander roCommander = localUser.FindCommander(value.id);
				CommanderCompleteType type = ((roCommander.state != ECommanderState.Nomal) ? CommanderCompleteType.Recruit : CommanderCompleteType.Transmission);
				UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				uICommanderComplete.Init(type, value.id);
			}
		}
		else
		{
			UIPopup.Create<UIGetItem>("GetItem").Set(reward.reward, string.Empty);
			SoundManager.PlaySFX("SE_ItemGet_001");
		}
		localUser.RefreshRewardFromNetwork(reward);
		localUser.dailyBonus.isReceived = true;
		UIManager.instance.RefreshOpenedUI();
		_CheckReceiveTestData("DailyBonusReceive");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6311", true, true)]
	public void GachaInformation()
	{
	}

	private IEnumerator GachaInformationResult(JsonRpcClient.Request request, Dictionary<string, Protocols.GachaInformationResponse> result)
	{
		_CheckReceiveTestData("GachaInformation");
		foreach (Protocols.GachaInformationResponse value in result.Values)
		{
			localUser.RefreshGachaFromNetwork(value);
		}
		if (!UIManager.instance.world.existGacha || !UIManager.instance.world.gacha.isActive)
		{
			UIManager.instance.world.gacha.InitAndOpenGacha();
		}
		else
		{
			UIManager.instance.world.gacha.RefreshGacha();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6312", true, true)]
	public void GachaOpenBox(int gbIdx, int cnt)
	{
	}

	private IEnumerator GachaOpenBoxResult(JsonRpcClient.Request request, Protocols.GachaOpenBoxResponse result)
	{
		_CheckReceiveTestData("GachaOpenBox");
		localUser.RefreshGachaFromNetwork(result.changedGachaInformation);
		localUser.RefreshGoodsFromNetwork(result.goodsResult);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.RefreshCostumeFromNetwork(result.costumeData);
		localUser.RefreshUserEquipItemFromNetwork(result.equipItem);
		localUser.RefreshItemFromNetwork(result.foodData);
		string gachaId = _FindRequestProperty(request, "gbIdx");
		if (UIManager.instance.world.gacha.isActive)
		{
			List<UIGacha.BoxData> list = new List<UIGacha.BoxData>();
			CommanderCompleteType getType = CommanderCompleteType.Undefined;
			result.rewardList.ForEach(delegate(Protocols.GachaOpenBoxResponse.Reward data)
			{
				if (data != null)
				{
					EGachaAnimationType gachaType = EGachaAnimationType.Normal;
					GachaRewardDataRow gachaRewardDataRow = regulation.gachaRewardDtbl.Find((GachaRewardDataRow row) => row.gachaType == gachaId && row.rewardType == data.type && row.rewardId == data.id);
					int num = 0;
					int getCommanderMedal = 0;
					bool isNew = false;
					if (data.type == ERewardType.Medal || data.type == ERewardType.Commander)
					{
						RoCommander roCommander2 = localUser.FindCommander(data.id);
						getType = ((roCommander2.state != ECommanderState.Nomal) ? CommanderCompleteType.Recruit : CommanderCompleteType.Transmission);
						getCommanderMedal = data.count;
						if (data.type == ERewardType.Commander)
						{
							roCommander2.state = ECommanderState.Nomal;
						}
					}
					if (gachaRewardDataRow != null)
					{
						if (gachaRewardDataRow.effectType == 1)
						{
							gachaType = EGachaAnimationType.RainBow;
						}
						else if (gachaRewardDataRow.effectType == 2)
						{
							gachaType = EGachaAnimationType.Premium;
						}
					}
					else if (data.type == ERewardType.Commander)
					{
						gachaType = EGachaAnimationType.RainBow;
					}
					list.Add(new UIGacha.BoxData
					{
						gachaType = gachaType,
						rewardType = data.type,
						rewardId = data.id,
						rewardCount = data.count,
						getType = getType,
						getCommanderMedal = getCommanderMedal,
						isNew = isNew
					});
				}
			});
			if (result.commanderIdDict != null)
			{
				foreach (Protocols.UserInformationResponse.Commander value in result.commanderIdDict.Values)
				{
					RoCommander roCommander = localUser.FindCommander(value.id);
					if (value.haveCostume != null && value.haveCostume.Count > 0)
					{
						roCommander.haveCostumeList = value.haveCostume;
					}
				}
			}
			UIManager.instance.world.gacha.OpenBox(list);
		}
		if (result.changedGachaInformation != null && result.changedGachaInformation.type == "2" && result.changedGachaInformation.freeOpenRemainTime > 0)
		{
			ScheduleLocalPush(ELocalPushType.PremiumGachaFree, result.changedGachaInformation.freeOpenRemainTime);
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6315", true, true)]
	public void GachaRatingInformationType()
	{
	}

	private IEnumerator GachaRatingInformationTypeResult(JsonRpcClient.Request request, object result)
	{
		if (result == null)
		{
			yield break;
		}
		Dictionary<string, object> dictionary = _ConvertJObject<Dictionary<string, object>>(result);
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			string key = item.Key;
			object value = item.Value;
			Dictionary<ERewardType, Protocols.GachaRatingDataTypeA> dictionary2 = _ConvertJObject<Dictionary<ERewardType, Protocols.GachaRatingDataTypeA>>(value);
			Dictionary<ERewardType, Protocols.GachaRatingDataTypeB> dictionary3 = _ConvertJObject<Dictionary<ERewardType, Protocols.GachaRatingDataTypeB>>(value);
			if (dictionary2 != null)
			{
				localUser.RefreshGachaProbabilityTypeAFromNetwork(key, dictionary2);
			}
			if (dictionary3 != null)
			{
				localUser.RefreshGachaProbabilityTypeBFromNetwork(key, dictionary3);
			}
		}
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6315", true, true)]
	public void GachaRatingInformationTypeB()
	{
	}

	private IEnumerator GachaRatingInformationTypeBResult(JsonRpcClient.Request request, Dictionary<string, Dictionary<ERewardType, Protocols.GachaRatingDataTypeB>> result)
	{
		if (result != null && result.Count != 0)
		{
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8117", true, true)]
	public void GetCouponList()
	{
	}

	private IEnumerator GetCouponListResult(JsonRpcClient.Request request, string result, List<string> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			localUser.coupons.Add(list[i].ToLower());
		}
		yield return null;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8116", true, true)]
	public void InputCoupon(string num)
	{
	}

	private IEnumerator InputCouponResult(JsonRpcClient.Request request, Protocols.SendChattingInfo result)
	{
		string text = string.Empty;
		if (result.rewardList != null)
		{
			UIPopup.Create<UIGetItem>("GetItem").Set(result.rewardList, string.Empty);
			SoundManager.PlaySFX("SE_ItemGet_001");
			for (int i = 0; i < result.rewardList.Count; i++)
			{
				Protocols.RewardInfo.RewardData rewardData = result.rewardList[i];
				if (rewardData.rewardType == ERewardType.Commander)
				{
					text = rewardData.rewardId;
				}
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			RoCommander roCommander = localUser.FindCommander(text);
			if (roCommander != null)
			{
				UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				if (uICommanderComplete != null)
				{
					if (roCommander.state != ECommanderState.Nomal)
					{
						uICommanderComplete.Init(CommanderCompleteType.Recruit, roCommander.id);
					}
					else
					{
						uICommanderComplete.Init(CommanderCompleteType.Transmission, roCommander.id);
					}
				}
				if (result.commanderData != null)
				{
					foreach (Protocols.UserInformationResponse.Commander value in result.commanderData.Values)
					{
						if (value.haveCostume != null && value.haveCostume.Count > 0)
						{
							roCommander.haveCostumeList = value.haveCostume;
						}
					}
				}
			}
		}
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshItemFromNetwork(result.eventResourceData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.AddCommanderFromNetwork(result.commanderData);
		localUser.RefreshCostumeFromNetwork(result.costumeData);
		localUser.RefreshItemFromNetwork(result.foodData);
		localUser.RefreshUserEquipItemFromNetwork(result.equipItem);
		localUser.RefreshItemFromNetwork(result.groupItemData);
		yield break;
	}

	private IEnumerator InputCouponError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 99004:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7054"));
			break;
		case 52003:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7903"));
			break;
		case 52005:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7904"));
			break;
		case 52007:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7902"));
			break;
		case 52008:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7901"));
			break;
		case 52010:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7905"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8118", true, true)]
	public void GetBadWordList(List<string> lang)
	{
	}

	private IEnumerator GetBadWordListResult(JsonRpcClient.Request request, string result, Dictionary<string, List<string>> word)
	{
		localUser.badWords = word;
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6101", true, true)]
	public void GetMailList()
	{
	}

	private IEnumerator GetMailListResult(JsonRpcClient.Request request, Protocols.MailInfo result)
	{
		int num = 0;
		for (int num2 = localUser.rewardList.Count - 1; num2 >= 0; num2--)
		{
			if (localUser.rewardList[num2].type == EReward.Mail)
			{
				localUser.rewardList.Remove(localUser.rewardList[num2]);
				localUser.newMailCount--;
			}
		}
		if (result != null)
		{
			foreach (Protocols.MailInfo.MailData mail in result.mailList)
			{
				RoReward roReward = new RoReward();
				roReward.type = EReward.Mail;
				roReward.id = mail.idx.ToString();
				roReward.subType = mail.type;
				if (mail.type == 2)
				{
					roReward.title = Localization.Get("17079");
					if (!string.IsNullOrEmpty(mail.message))
					{
						string[] array = mail.message.Split(',');
						if (array.Length == 2)
						{
							RankingDataRow rankingDataRow = instance.regulation.rankingDtbl[array[1].ToString()];
							roReward.description = Localization.Format("17080", array[0], Localization.Get(rankingDataRow.name));
						}
					}
				}
				else if (mail.type == 3)
				{
					roReward.title = Localization.Get("18913");
					if (!string.IsNullOrEmpty(mail.message))
					{
						string[] array2 = mail.message.Split(',');
						if (array2.Length == 2)
						{
							RankingDataRow rankingDataRow2 = instance.regulation.rankingDtbl[array2[1].ToString()];
							roReward.description = Localization.Format("18914", array2[0], Localization.Get(rankingDataRow2.name));
						}
					}
				}
				else if (mail.type == 4)
				{
					roReward.title = Localization.Get("4818");
					if (!string.IsNullOrEmpty(mail.message))
					{
						string[] array3 = mail.message.Split(',');
						if (array3.Length == 2)
						{
							roReward.description = Localization.Format("4813", array3[0]);
						}
					}
				}
				else if (mail.type == 5)
				{
					if (!string.IsNullOrEmpty(mail.message))
					{
						string[] array4 = mail.message.Split(',');
						if (array4[0] == "1")
						{
							roReward.title = Localization.Get("110285");
							roReward.description = Localization.Get("110361");
						}
						else if (array4[0] == "2")
						{
							roReward.title = Localization.Get("110287");
							roReward.description = Localization.Get("110363");
						}
						else if (array4[0] == "3")
						{
							roReward.title = Localization.Format("110288", array4[1]);
							roReward.description = Localization.Get("110364");
						}
						else if (array4[0] == "4")
						{
							roReward.title = Localization.Get("110286");
							roReward.description = Localization.Get("110362");
						}
					}
				}
				else if (mail.type == 6)
				{
					roReward.title = Localization.Get("5050015");
					if (!string.IsNullOrEmpty(mail.message))
					{
						string[] array5 = mail.message.Split(',');
						if (array5.Length == 2)
						{
							RankingDataRow rankingDataRow3 = instance.regulation.rankingDtbl[array5[1].ToString()];
							roReward.description = Localization.Format("5050016", array5[0], Localization.Get(rankingDataRow3.name));
						}
					}
				}
				else if (mail.type == 7)
				{
					roReward.title = Localization.Get("70067");
					if (!string.IsNullOrEmpty(mail.message))
					{
						string[] array6 = mail.message.Split(',');
						if (array6.Length == 2)
						{
							roReward.description = Localization.Format("70068", array6[1]);
						}
					}
				}
				else if (mail.type == 8)
				{
					if (!string.IsNullOrEmpty(mail.message))
					{
						string[] array7 = mail.message.Split(',');
						if (array7.Length == 2)
						{
							roReward.title = Localization.Format("21007", array7[1]);
							roReward.description = Localization.Format("21008", array7[1]);
						}
					}
				}
				else if (mail.type == 9)
				{
					roReward.title = Localization.Get("400021");
					if (!string.IsNullOrEmpty(mail.message))
					{
						string[] array8 = mail.message.Split(',');
						if (array8.Length == 2)
						{
							RankingDataRow rankingDataRow4 = instance.regulation.rankingDtbl[array8[1].ToString()];
							roReward.description = Localization.Format("400022", array8[0], Localization.Get(rankingDataRow4.name));
						}
					}
				}
				else if (mail.type == 10)
				{
					roReward.title = Localization.Get("400023");
					if (!string.IsNullOrEmpty(mail.message))
					{
						string[] array9 = mail.message.Split(',');
						if (array9.Length == 2)
						{
							RankingDataRow rankingDataRow5 = instance.regulation.rankingDtbl[array9[1].ToString()];
							roReward.description = Localization.Format("400024", array9[0], Localization.Get(rankingDataRow5.name));
						}
					}
				}
				else
				{
					roReward.description = mail.message;
				}
				roReward.completeTime = mail.remainTime;
				if (string.IsNullOrEmpty(mail.status))
				{
					roReward.received = false;
				}
				else
				{
					roReward.received = (mail.status.Equals("R") ? true : false);
				}
				if (mail.reward != null)
				{
					roReward.rewardItem = mail.reward;
				}
				localUser.rewardList.Add(roReward);
				localUser.newMailCount++;
				if (!roReward.received && roReward.IsCompleted())
				{
					num++;
				}
			}
		}
		localUser.badgeNewMailCount = num;
		UIManager.instance.world.mail.Set(EReward.Mail);
		UIManager.instance.world.mail.OpenPopup();
		UIManager.instance.RefreshOpenedUI();
		_CheckReceiveTestData("GetMailListResult");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6102", true, true)]
	public void GetReward(int idx, int type)
	{
	}

	private IEnumerator GetRewardResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		string id = _FindRequestProperty(request, "idx");
		RoReward item = localUser.FindReward(id);
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		localUser.rewardList.Remove(item);
		localUser.newMailCount--;
		localUser.badgeNewMailCount--;
		localUser.RefreshRewardFromNetwork(result);
		UIManager.instance.world.mail.Set(EReward.Mail);
		UIManager.instance.RefreshOpenedUI();
		_CheckReceiveTestData("GetRewardResult");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6103", true, true)]
	public void ReadMail(int idx)
	{
	}

	private IEnumerator ReadMailResult(JsonRpcClient.Request request, string result)
	{
		localUser.badgeNewMailCount--;
		string id = _FindRequestProperty(request, "idx");
		RoReward roReward = localUser.FindReward(id);
		roReward.received = true;
		UIManager.instance.RefreshOpenedUI();
		_CheckReceiveTestData("ReadMailResult");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6104", true, true)]
	public void GetRewardAll()
	{
	}

	private IEnumerator GetRewardAllResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		if (result.reward.Count < 1)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7066"));
			yield break;
		}
		List<RoReward> list = new List<RoReward>();
		for (int num = localUser.rewardList.Count - 1; num >= 0; num--)
		{
			if (localUser.rewardList[num].type == EReward.Mail && (!string.IsNullOrEmpty(localUser.rewardList[num].rewardId) || localUser.rewardList[num].rewardItem != null))
			{
				RoReward item = localUser.rewardList[num];
				list.Add(item);
				localUser.rewardList.Remove(item);
				localUser.newMailCount--;
				localUser.badgeNewMailCount--;
			}
		}
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		localUser.RefreshRewardFromNetwork(result);
		UIManager.instance.world.mail.Set(EReward.Mail);
		UIManager.instance.RefreshOpenedUI();
		_CheckReceiveTestData("GetRewardResult");
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6105", true, true)]
	public void GetRankingReward(int type, int ridx)
	{
	}

	private IEnumerator GetRankingRewardResult(JsonRpcClient.Request request, Protocols.RankingReward result)
	{
		string s = _FindRequestProperty(request, "type");
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.RefreshCostumeFromNetwork(result.costumeData);
		localUser.RefreshUserEquipItemFromNetwork(result.equipItem);
		localUser.RefreshItemFromNetwork(result.foodData);
		UIPopup.Create<UIGetItem>("GetItem").Set(result.rewardList, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		if (int.Parse(s) == 4)
		{
			localUser.raidRewardPoint = result.receiveIdx[result.receiveIdx.Count - 1];
		}
		else if (int.Parse(s) == 5)
		{
			localUser.winRankIdx = result.receiveIdx[0];
		}
		else if (int.Parse(s) == 8)
		{
			localUser.worldWinRankIdx = result.receiveIdx[0];
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6108", true, true)]
	public void GetFirstPaymentReward()
	{
	}

	private IEnumerator GetFirstPaymentRewardResult(JsonRpcClient.Request request, Protocols.FirstPaymentRewardInfo result)
	{
		if (result.commanderData != null)
		{
			string[] array = new string[result.commanderData.Count];
			result.commanderData.Keys.CopyTo(array, 0);
			RoCommander roCommander = localUser.FindCommander(array[0]);
			if (roCommander != null)
			{
				UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				if (uICommanderComplete != null)
				{
					if (roCommander.state != ECommanderState.Nomal)
					{
						uICommanderComplete.Init(CommanderCompleteType.Recruit, roCommander.id);
					}
					else
					{
						uICommanderComplete.Init(CommanderCompleteType.Transmission, roCommander.id);
					}
				}
			}
		}
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.AddCommanderFromNetwork(result.commanderData);
		localUser.RefreshCostumeFromNetwork(result.costumeData);
		localUser.RefreshItemFromNetwork(result.eventResourceData);
		localUser.RefreshUserEquipItemFromNetwork(result.equipItemData);
		localUser.RefreshItemFromNetwork(result.foodData);
		localUser.RefreshItemFromNetwork(result.groupItemData);
		localUser.statistics.firstPayment = result.userInfo.firstPayment;
		UIPopup.Create<UIGetItem>("GetItem").Set(result.rewardList, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7201", true, true)]
	public void RequestPayment()
	{
	}

	private IEnumerator RequestPaymentResult(JsonRpcClient.Request request, Dictionary<int, Protocols.CashShopInfo> result)
	{
		_CheckReceiveTestData("RequestPayment");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3202", true, true)]
	public void SituationInformation()
	{
	}

	private IEnumerator SituationInformationResult(JsonRpcClient.Request request, string result, int did, int rtm)
	{
		if (UIManager.instance.world != null)
		{
			if (!UIManager.instance.world.existSituation || !UIManager.instance.world.situation.isActive)
			{
				UIManager.instance.world.situation.InitAndOpenSituation();
			}
			UIManager.instance.world.situation.SetSweepEnable(did, rtm);
		}
		if (UIManager.instance.battle != null)
		{
			UIBattleResult battleResult = UIManager.instance.battle.BattleResult;
			battleResult.Open();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3231", true, true)]
	public void SituationSweepStart(int type, int stype, int lv, JObject deck)
	{
	}

	private IEnumerator SituationSweepStartResult(JsonRpcClient.Request request, string result, List<Protocols.RewardInfo.RewardData> reward)
	{
		BattleData battleData = BattleData.Get();
		battleData.rewardItems = reward;
		BattleData.Set(battleData);
		Loading.Load(Loading.Battle);
		yield break;
	}

	private IEnumerator SituationSweepStartError(JsonRpcClient.Request request, string result, int code)
	{
		if (code != 11011)
		{
			yield break;
		}
		if (UIManager.instance.world != null)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7044"));
		}
		if (UIManager.instance.battle == null || !GameSetting.instance.repeatBattle)
		{
			yield break;
		}
		GameSetting.instance.repeatBattle = false;
		UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, Localization.Get("18045"), Localization.Get("1001"));
		uISimplePopup.onClose = delegate
		{
			BattleData battleData = BattleData.Get();
			BattleData.Set(battleData);
			if (battleData != null)
			{
				battleData.move = EBattleResultMove.Situation;
			}
			Loading.Load(Loading.WorldMap);
		};
	}

	private T _ConvertJObject<T>(object obj)
	{
		JArray jArray = null;
		try
		{
			jArray = JArray.FromObject(obj);
		}
		catch (Exception)
		{
		}
		if (jArray != null)
		{
			return default(T);
		}
		JObject jObject = JObject.FromObject(obj);
		return jObject.ToObject<T>();
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3123", true, true)]
	public void PvPRankingList()
	{
	}

	private IEnumerator PvPRankingListResult(JsonRpcClient.Request request, object result)
	{
		duelRankingList.Clear();
		if (result != null)
		{
		}
		Protocols.PvPRankingList pvPRankingList = _ConvertJObject<Protocols.PvPRankingList>(result);
		if (pvPRankingList != null)
		{
			for (int i = 0; i < pvPRankingList.rankList.Count; i++)
			{
				RoUser item = RoUser.CreateRankListUser(EBattleType.Duel, pvPRankingList.rankList[i]);
				duelRankingList.Add(item);
			}
			UIPopup.Create<RankingList>("RankingList").Set(EBattleType.Duel, instance.duelRankingList);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3124", true, true)]
	public void RefreshPvPDuelList()
	{
	}

	private IEnumerator RefreshPvPDuelListResult(JsonRpcClient.Request request, Protocols.RefreshPvPDuel result)
	{
		localUser.duelTargetList.Clear();
		localUser.RefreshGoodsFromNetwork(result.rsoc);
		localUser.duelTargetRefreshTime.SetByDuration(result.remain);
		localUser.currentSeasonDuelTime.SetByDuration(result.time);
		if (result.duelList != null)
		{
			for (int i = 1; i <= result.duelList.Count; i++)
			{
				localUser.duelTargetList.Add(result.duelList[i].idx, RoUser.CreateDuelListUser(EBattleType.Duel, result.duelList[i]));
			}
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3125", true, true)]
	public void PvPDuelList()
	{
	}

	private IEnumerator PvPDuelListResult(JsonRpcClient.Request request, Protocols.PvPDuelList result)
	{
		localUser.duelTargetList.Clear();
		localUser.duelTargetRefreshTime.SetByDuration(result.remain);
		localUser.currentSeasonDuelTime.SetByDuration(result.time);
		if (result.duelList != null)
		{
			for (int i = 1; i <= result.duelList.Count; i++)
			{
				localUser.duelTargetList.Add(result.duelList[i].idx, RoUser.CreateDuelListUser(EBattleType.Duel, result.duelList[i]));
			}
		}
		if (result.user != null)
		{
			localUser.duelScore = result.user.score;
			localUser.duelNextScore = result.user.nextScore;
			localUser.duelGradeIdx = result.user.rewardId;
			localUser.duelWinningStreak = Mathf.Max(result.user.winningStreak - 1, 0);
			localUser.duelLosingStreak = Mathf.Max(result.user.losingStreak - 1, 0);
			localUser.duelRanking = result.user.ranking;
			localUser.duelRankingRate = result.user.rankingRate;
			localUser.duelWinCount = result.user.winCnt;
			localUser.duelLoseCount = result.user.loseCnt;
			localUser.duelPoint = result.user.duelPoint;
			localUser.rewardDuelPoint = result.user.rewardDuelPoint;
			localUser.winRank = result.user.winRank;
			localUser.winRankIdx = result.user.winRankIdx;
		}
		UIManager.instance.world.rankingBattle.InitAndOpen(EBattleType.Duel);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3221", true, true)]
	public void PvPDuelInfo(int idx)
	{
	}

	private IEnumerator PvPDuelInfoResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.Commander target)
	{
		string s = _FindRequestProperty(request, "idx");
		RoUser roUser = localUser.duelTargetList[int.Parse(s)];
		if (target != null)
		{
			roUser.UpdateUserTroop(target);
		}
		BattleData battleData = UIManager.instance.world.rankingBattle.battleData;
		battleData.defender = roUser;
		UIManager.instance.world.readyBattle.InitAndOpenReadyBattle(battleData);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3231", true, true)]
	public void PvPStartDuel(int type, JObject deck, int idx, string checkSum, JArray info, JArray result)
	{
	}

	private IEnumerator PvPStartDuelResult(JsonRpcClient.Request request, object result)
	{
		BattleData battleData = BattleData.Get();
		BattleData.Set(battleData);
		if (battleData == null || (battleData.type != EBattleType.GuildScramble && result == null))
		{
			yield break;
		}
		if (battleData.type != EBattleType.GuildScramble)
		{
			Protocols.UserInformationResponse.BattleResult battleResult = _ConvertJObject<Protocols.UserInformationResponse.BattleResult>(result);
			if (battleData.type == EBattleType.Duel)
			{
				battleData.dualResult = battleResult;
				localUser.duelPoint = battleResult.user.duelPoint;
				if (battleData.record.result.IsWin)
				{
					if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
					{
						statistics.pvpWinCount++;
						if (statistics.pvpWinCount >= 10)
						{
							SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQCg");
						}
						if (statistics.pvpWinCount >= 100)
						{
							SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQCw");
						}
						if (statistics.pvpWinCount >= 300)
						{
							SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQDA");
						}
						if (statistics.pvpWinCount >= 500)
						{
							SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQDQ");
						}
						if (statistics.pvpWinCount >= 1000)
						{
							SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQDg");
						}
					}
					if (GameCenterManager.IsPlayerAuthenticated)
					{
						statistics.pvpWinCount++;
						if (statistics.pvpWinCount == 10)
						{
							GameCenterManager.SubmitAchievement(100f, "CgkIj7Xpxu4GEAIQCg");
						}
						else if (statistics.pvpWinCount == 100)
						{
							GameCenterManager.SubmitAchievement(100f, "CgkIj7Xpxu4GEAIQCw");
						}
						else if (statistics.pvpWinCount == 300)
						{
							GameCenterManager.SubmitAchievement(100f, "CgkIj7Xpxu4GEAIQDA");
						}
						else if (statistics.pvpWinCount == 500)
						{
							GameCenterManager.SubmitAchievement(100f, "CgkIj7Xpxu4GEAIQDQ");
						}
						else if (statistics.pvpWinCount == 1000)
						{
							GameCenterManager.SubmitAchievement(100f, "CgkIj7Xpxu4GEAIQDg");
						}
					}
				}
				localUser.RefreshGoodsFromNetwork(battleResult.resource);
				localUser.RefreshPartFromNetwork(battleResult.partData);
				localUser.RefreshMedalFromNetwork(battleResult.medalData);
				localUser.RefreshFavorFromNetwork(battleResult.commanderFavor);
				localUser.RefreshItemFromNetwork(battleResult.eventResourceData);
				localUser.RefreshItemFromNetwork(battleResult.itemData);
			}
		}
		Loading.Load(Loading.Battle);
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3231", true, true)]
	public void PvPStartWorldDuel(int type, int retry, JObject deck, int idx, string checkSum, JArray info, JArray result)
	{
	}

	private IEnumerator PvPStartWorldDuelResult(JsonRpcClient.Request request, object result)
	{
		BattleData _battleData = BattleData.Get();
		BattleData.Set(_battleData);
		Protocols.UserInformationResponse.BattleResult battleResult2 = null;
		if (_battleData != null && result != null)
		{
			if (_battleData.type == EBattleType.WorldDuel)
			{
				battleResult2 = (_battleData.dualResult = _ConvertJObject<Protocols.UserInformationResponse.BattleResult>(result));
				localUser.RefreshGoodsFromNetwork(battleResult2.resource);
				localUser.RefreshPartFromNetwork(battleResult2.partData);
				localUser.RefreshMedalFromNetwork(battleResult2.medalData);
				localUser.RefreshFavorFromNetwork(battleResult2.commanderFavor);
				localUser.RefreshItemFromNetwork(battleResult2.eventResourceData);
				localUser.RefreshItemFromNetwork(battleResult2.itemData);
			}
			UIWorldDuelInfoPopup popup = UIPopup.Create<UIWorldDuelInfoPopup>("WorldDuelInfoPopup");
			if (!_battleData.worldDuelReMatch)
			{
				popup.Init(localUser, localUser.worldDuelTarget);
			}
			else
			{
				popup.Init(localUser, localUser.worldDuelReMatchTarget);
			}
			popup.StartGameObjectDestroy();
			while (!popup.battleEnable)
			{
				yield return null;
			}
			Loading.Load(Loading.Battle);
		}
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "5219", true, true)]
	public void UpdateTroopRole(int cid, string role)
	{
	}

	private IEnumerator UpdateTroopRoleResult(JsonRpcClient.Request request, bool result)
	{
		if (!result)
		{
			yield break;
		}
		string cid = _FindRequestProperty(request, "cid");
		string role = _FindRequestProperty(request, "role");
		localUser.commanderList.ForEach(delegate(RoCommander commander)
		{
			if (string.Equals(commander.role, role) || string.Equals(commander.id, cid))
			{
				commander.role = ((!commander.id.Equals(cid)) ? "N" : role);
			}
		});
		UIManager.instance.RefreshOpenedUI();
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3228", true, true)]
	public void DefenderSetting(JObject deck)
	{
	}

	private IEnumerator DefenderSettingResult(JsonRpcClient.Request request, string result)
	{
		if (result.Equals("True"))
		{
			Dictionary<string, string> source = JsonConvert.DeserializeObject<Dictionary<string, string>>(_FindRequestProperty(request, "deck"));
			localUser.RefreshDefenderTroop(source);
			UIManager.instance.world.readyBattle.CloseAnimation();
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3229", true, true)]
	public void GetDefenderInfo()
	{
	}

	private IEnumerator GetDefenderInfoResult(JsonRpcClient.Request request, string result, Dictionary<string, string> deck)
	{
		localUser.RefreshDefenderTroop(deck);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3403", true, true)]
	public void PvPWaveDuelRankingList()
	{
	}

	private IEnumerator PvPWaveDuelRankingListResult(JsonRpcClient.Request request, object result)
	{
		duelRankingList.Clear();
		if (result != null)
		{
		}
		Protocols.PvPRankingList pvPRankingList = _ConvertJObject<Protocols.PvPRankingList>(result);
		if (pvPRankingList != null)
		{
			for (int i = 0; i < pvPRankingList.rankList.Count; i++)
			{
				RoUser item = RoUser.CreateRankListUser(EBattleType.WaveDuel, pvPRankingList.rankList[i]);
				duelRankingList.Add(item);
			}
			UIPopup.Create<RankingList>("RankingList").Set(EBattleType.WaveDuel, instance.duelRankingList);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3402", true, true)]
	public void RefreshPvPWaveDuelList()
	{
	}

	private IEnumerator RefreshPvPWaveDuelListResult(JsonRpcClient.Request request, Protocols.RefreshPvPDuel result)
	{
		localUser.duelTargetList.Clear();
		localUser.RefreshGoodsFromNetwork(result.rsoc);
		localUser.duelTargetRefreshTime.SetByDuration(result.remain);
		localUser.currentSeasonDuelTime.SetByDuration(result.time);
		localUser.currentSeasonOpenRemainDuelTime.SetByDuration(result.openRemain);
		if (result.duelList != null)
		{
			for (int i = 1; i <= result.duelList.Count; i++)
			{
				localUser.duelTargetList.Add(result.duelList[i].idx, RoUser.CreateWaveDuelListUser(result.duelList[i]));
			}
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3401", true, true)]
	public void PvPWaveDuelList()
	{
	}

	private IEnumerator PvPWaveDuelListResult(JsonRpcClient.Request request, Protocols.PvPDuelList result)
	{
		localUser.duelTargetList.Clear();
		localUser.duelTargetRefreshTime.SetByDuration(result.remain);
		localUser.currentSeasonDuelTime.SetByDuration(result.time);
		localUser.currentSeasonOpenRemainDuelTime.SetByDuration(result.openRemain);
		if (result.duelList != null)
		{
			for (int i = 1; i <= result.duelList.Count; i++)
			{
				localUser.duelTargetList.Add(result.duelList[i].idx, RoUser.CreateWaveDuelListUser(result.duelList[i]));
			}
		}
		if (result.user != null)
		{
			localUser.duelScore = result.user.score;
			localUser.duelNextScore = result.user.nextScore;
			localUser.duelGradeIdx = result.user.rewardId;
			localUser.duelWinningStreak = Mathf.Max(result.user.winningStreak - 1, 0);
			localUser.duelRanking = result.user.ranking;
			localUser.duelRankingRate = result.user.rankingRate;
			localUser.duelWinCount = result.user.winCnt;
			localUser.duelLoseCount = result.user.loseCnt;
			localUser.duelPoint = result.user.duelPoint;
			localUser.rewardDuelPoint = result.user.rewardDuelPoint;
		}
		UIManager.instance.world.rankingBattle.InitAndOpen(EBattleType.WaveDuel);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3405", true, true)]
	public void WaveDuelDefenderSetting(JObject decks)
	{
	}

	private IEnumerator WaveDuelDefenderSettingResult(JsonRpcClient.Request request, string result)
	{
		if (!result.Equals("True"))
		{
			yield break;
		}
		Dictionary<string, Dictionary<string, string>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(_FindRequestProperty(request, "decks"));
		List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
		for (int i = 1; i <= ConstValue.waveDuelTroopCount; i++)
		{
			string key = i.ToString();
			if (dictionary.ContainsKey(key))
			{
				list.Add(dictionary[key]);
			}
			else
			{
				list.Add(new Dictionary<string, string>());
			}
		}
		localUser.RefreshDefenderTroop(list);
		UIManager.instance.world.readyBattle.CloseAnimation();
		UIManager.instance.RefreshOpenedUI();
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3404", true, true)]
	public void GetWaveDuelDefenderInfo()
	{
	}

	private IEnumerator GetWaveDuelDefenderInfoResult(JsonRpcClient.Request request, string result, Dictionary<string, Dictionary<string, string>> decks)
	{
		List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
		for (int i = 1; i <= ConstValue.waveDuelTroopCount; i++)
		{
			string key = i.ToString();
			if (decks.ContainsKey(key))
			{
				list.Add(decks[key]);
			}
			else
			{
				list.Add(new Dictionary<string, string>());
			}
		}
		localUser.RefreshDefenderTroop(list);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3231", true, true)]
	public void PvPStartWaveDuel(int type, int idx, string checkSum, JArray info, JArray result)
	{
	}

	private IEnumerator PvPStartWaveDuelResult(JsonRpcClient.Request request, object result)
	{
		BattleData battleData = BattleData.Get();
		BattleData.Set(battleData);
		if (battleData == null || (battleData.type != EBattleType.GuildScramble && result == null))
		{
			yield break;
		}
		if (battleData.type != EBattleType.GuildScramble)
		{
			Protocols.UserInformationResponse.BattleResult battleResult = _ConvertJObject<Protocols.UserInformationResponse.BattleResult>(result);
			if (battleData.type == EBattleType.WaveDuel)
			{
				battleData.dualResult = battleResult;
				localUser.RefreshGoodsFromNetwork(battleResult.resource);
				localUser.RefreshPartFromNetwork(battleResult.partData);
				localUser.RefreshMedalFromNetwork(battleResult.medalData);
				localUser.RefreshFavorFromNetwork(battleResult.commanderFavor);
				localUser.RefreshItemFromNetwork(battleResult.eventResourceData);
				localUser.RefreshItemFromNetwork(battleResult.itemData);
			}
		}
		Loading.Load(Loading.Battle);
	}

	private IEnumerator PvPStartWaveDuelError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 70009:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, Localization.Get("7044"), Localization.Get("1001"));
			break;
		case 41050:
		{
			UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Get("5040013"), null, "1001");
			uISimplePopup.onClose = delegate
			{
				RequestRefreshPvPWaveDuelList();
				UIManager.instance.world.readyBattle.CloseAnimation();
			};
			break;
		}
		default:
			StartCoroutine(OnJsonRpcRequestError(request, result, code));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1103", true, true)]
	public void Mission(List<string> type)
	{
	}

	private IEnumerator MissionResult(JsonRpcClient.Request request, object result)
	{
		_CheckReceiveTestData("MissionList");
		for (int i = 0; i < localUser.missionList.Count; i++)
		{
			if (localUser.missionList[i].level <= localUser.level)
			{
				localUser.missionList[i].bListShow = true;
			}
			if (localUser.missionList[i].VipCount > localUser.vipLevel)
			{
				localUser.missionList[i].bListShow = false;
			}
			localUser.missionList[i].received = false;
			localUser.missionList[i].combleted = false;
			localUser.missionList[i].conditionCount = 0;
		}
		if (result != null)
		{
			Protocols.MissionInfo missionInfo = _ConvertJObject<Protocols.MissionInfo>(result);
			if (missionInfo != null)
			{
				localUser.missionCompleteCount = missionInfo.completeCount;
				localUser.missionGoal = missionInfo.goal;
				localUser.badgeMissionCount = 0;
				for (int j = 0; j < missionInfo.missionList.Count; j++)
				{
					RoMission roMission = localUser.FindMission(missionInfo.missionList[j].missionId.ToString());
					if (roMission != null)
					{
						roMission.received = missionInfo.missionList[j].receive == 1;
						roMission.combleted = missionInfo.missionList[j].complete == 1;
						roMission.conditionCount = missionInfo.missionList[j].point;
						if (roMission.combleted && !roMission.received)
						{
							localUser.badgeMissionCount++;
						}
					}
				}
			}
			Protocols.AchievementInfo achievementInfo = _ConvertJObject<Protocols.AchievementInfo>(result);
			if (achievementInfo != null)
			{
				localUser.achievementCompleteCount = achievementInfo.completeCount;
				localUser.achievementGoal = achievementInfo.goal;
				localUser.badgeAchievementCount = 0;
				for (int k = 0; k < achievementInfo.AchievementList.Count; k++)
				{
					RoMission roMission2 = localUser.FindAchievement(achievementInfo.AchievementList[k].achievementId.ToString(), achievementInfo.AchievementList[k].sort);
					if (roMission2 != null)
					{
						roMission2.received = achievementInfo.AchievementList[k].receive == 1;
						roMission2.combleted = achievementInfo.AchievementList[k].complete == 1;
						roMission2.conditionCount = achievementInfo.AchievementList[k].point;
						if (roMission2.combleted && !roMission2.received)
						{
							localUser.badgeAchievementCount++;
						}
						roMission2.bListShow = true;
					}
				}
			}
			Protocols.UserInformationResponse userInformationResponse = _ConvertJObject<Protocols.UserInformationResponse>(result);
			if (userInformationResponse != null)
			{
				localUser.FromNetwork(userInformationResponse);
			}
			UIManager.instance.RefreshOpenedUI();
		}
		UIManager.instance.world.warHome.Set(EReward.DailyMission);
		UIManager.instance.world.warHome.Open();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6233", true, true)]
	public void MissionReward(int dmid)
	{
	}

	private IEnumerator MissionRewardResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		string id = _FindRequestProperty(request, "dmid");
		RoMission roMission = localUser.FindMission(id);
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		SoundManager.PlaySFX("SE_DailyMission_001");
		localUser.badgeMissionCount--;
		localUser.RefreshRewardFromNetwork(result);
		UIManager.instance.RefreshOpenedUI();
		roMission.received = true;
		localUser.missionCompleteCount++;
		UIManager.instance.world.warHome.Set(EReward.DailyMission);
		_CheckReceiveTestData("MissionRewardResult");
		yield break;
	}

	private IEnumerator MissionRewardError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 13001)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7044"));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6236", true, true)]
	public void AllMissionReward()
	{
	}

	private IEnumerator AllMissionRewardResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		SoundManager.PlaySFX("SE_DailyMission_001");
		localUser.RefreshRewardFromNetwork(result);
		UIManager.instance.RefreshOpenedUI();
		if (result.receiveMissinIdx != null)
		{
			foreach (int item in result.receiveMissinIdx)
			{
				RoMission roMission = localUser.FindMission(item.ToString());
				if (roMission != null)
				{
					roMission.received = true;
				}
				localUser.badgeMissionCount--;
				localUser.missionCompleteCount++;
			}
		}
		UIManager.instance.world.warHome.Set(EReward.DailyMission);
		yield break;
	}

	private IEnumerator AllMissionRewardError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 13009)
		{
			UIManager.instance.world.warHome.Close();
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7045"));
		}
		else
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), "Error Code:" + code);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6231", true, true)]
	public void AchievementReward(int acid, int asot)
	{
	}

	private IEnumerator AchievementRewardResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		string id = _FindRequestProperty(request, "acid");
		int sort = int.Parse(_FindRequestProperty(request, "asot"));
		RoMission roMission = localUser.FindAchievement(id, sort);
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		SoundManager.PlaySFX("SE_DailyMission_001");
		localUser.badgeAchievementCount--;
		localUser.RefreshRewardFromNetwork(result);
		roMission.received = true;
		roMission.bListShow = false;
		roMission.completeTime = result.time;
		if (result.nextAchievement != null)
		{
			RoMission roMission2 = localUser.FindAchievement(result.nextAchievement.achievementId.ToString(), result.nextAchievement.sort);
			if (roMission2 != null)
			{
				roMission2.bListShow = true;
				roMission2.received = result.nextAchievement.receive == 1;
				roMission2.combleted = result.nextAchievement.complete == 1;
				roMission2.conditionCount = result.nextAchievement.point;
				if (roMission2.combleted && !roMission2.received)
				{
					localUser.badgeAchievementCount++;
				}
			}
		}
		localUser.achievementCompleteCount++;
		UIManager.instance.world.warHome.Set(EReward.Achievement);
		UIManager.instance.RefreshOpenedUI();
		_CheckReceiveTestData("AchievementRewardResult");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6235", true, true)]
	public void AllAchievementReward()
	{
	}

	private IEnumerator AllAchievementRewardResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		SoundManager.PlaySFX("SE_DailyMission_001");
		localUser.RefreshRewardFromNetwork(result);
		foreach (KeyValuePair<string, int> item in result.receiveAchievementIdx)
		{
			RoMission roMission = localUser.FindAchievement(item.Key, item.Value);
			localUser.badgeAchievementCount--;
			roMission.received = true;
			roMission.bListShow = false;
			roMission.completeTime = result.time;
			localUser.achievementCompleteCount++;
		}
		if (localUser.badgeAchievementCount < 0)
		{
			localUser.badgeAchievementCount = 0;
		}
		if (result.nextAchievementList != null)
		{
			foreach (Protocols.RewardInfo.AchievementData nextAchievement in result.nextAchievementList)
			{
				RoMission roMission2 = localUser.FindAchievement(nextAchievement.achievementId.ToString(), nextAchievement.sort);
				if (roMission2 != null)
				{
					roMission2.bListShow = true;
					roMission2.received = nextAchievement.receive == 1;
					roMission2.combleted = nextAchievement.complete == 1;
					roMission2.conditionCount = nextAchievement.point;
					if (roMission2.combleted && !roMission2.received)
					{
						localUser.badgeAchievementCount++;
					}
				}
			}
		}
		UIManager.instance.world.warHome.Set(EReward.Achievement);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator AllAchievementRewardError(JsonRpcClient.Request request, string result, int code)
	{
		NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), "Error Code:" + code);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6132", true, true)]
	public void CompleteAchievement()
	{
	}

	private IEnumerator CompleteAchievementResult(JsonRpcClient.Request request, Protocols.CompleteAchievementInfo[] result)
	{
		for (int i = 0; i < result.Length; i++)
		{
			RoMission roMission = localUser.FindAchievement(result[i].achievementId.ToString(), result[i].sort);
			if (roMission != null)
			{
				roMission.received = true;
				roMission.completeTime = result[i].time;
			}
		}
		_CheckReceiveTestData("CompleteAchievementResult");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6234", true, true)]
	public void CompleteMissionGoal(int type, int idx)
	{
	}

	private IEnumerator CompleteMissionGoalResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		string text = _FindRequestProperty(request, "type");
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		SoundManager.PlaySFX("SE_DailyMissionReward_001");
		localUser.RefreshRewardFromNetwork(result);
		UIManager.instance.RefreshOpenedUI();
		if (text == "1")
		{
			localUser.missionGoal++;
			UIManager.instance.world.warHome.SetInfo(EReward.DailyMission);
		}
		else if (text == "2")
		{
			localUser.achievementGoal++;
			UIManager.instance.world.warHome.SetInfo(EReward.Achievement);
		}
		_CheckReceiveTestData("CompleteGoalResult");
		yield break;
	}

	[JsonRpcClient.Request("http://gkchat.flerogames.com/talk/server.php", "sendMsg", true, true)]
	public void SendWhisperMsgChatting(int from, string fromnm, int to, string tonm, string msg)
	{
	}

	private IEnumerator SendWhisperMsgChattingResult(JsonRpcClient.Request request, string result)
	{
		localUser.lastChatTimeTick = DateTime.Now.Ticks;
		yield break;
	}

	[JsonRpcClient.Request("http://gkchat.flerogames.com/talk/server.php", "sendMsg", true, true)]
	public void SendChMsgChatting(int channel, int send, string snm, string msg, int ucash, int thmb, int lv)
	{
	}

	private IEnumerator SendChMsgChattingResult(JsonRpcClient.Request request, Protocols.SendChattingInfo result)
	{
		localUser.lastChatTimeTick = DateTime.Now.Ticks;
		string text = string.Empty;
		if (result.rewardList != null)
		{
			UIPopup.Create<UIGetItem>("GetItem").Set(result.rewardList, string.Empty);
			SoundManager.PlaySFX("SE_ItemGet_001");
			for (int i = 0; i < result.rewardList.Count; i++)
			{
				Protocols.RewardInfo.RewardData rewardData = result.rewardList[i];
				if (rewardData.rewardType == ERewardType.Commander)
				{
					text = rewardData.rewardId;
				}
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			RoCommander roCommander = localUser.FindCommander(text);
			if (roCommander != null)
			{
				UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				if (uICommanderComplete != null)
				{
					if (roCommander.state != ECommanderState.Nomal)
					{
						uICommanderComplete.Init(CommanderCompleteType.Recruit, roCommander.id);
					}
					else
					{
						uICommanderComplete.Init(CommanderCompleteType.Transmission, roCommander.id);
					}
				}
			}
		}
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshItemFromNetwork(result.eventResourceData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.AddCommanderFromNetwork(result.commanderData);
		localUser.RefreshCostumeFromNetwork(result.costumeData);
		localUser.RefreshItemFromNetwork(result.foodData);
		localUser.RefreshItemFromNetwork(result.groupItemData);
		RequestSendwaitChannelMsg();
		UIManager.instance.world.mainCommand.chat.TimeOutCount = 10;
		yield break;
	}

	private IEnumerator SendChMsgChattingError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 99004:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7054"));
			break;
		case 52003:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7903"));
			break;
		case 52005:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7904"));
			break;
		case 52007:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7902"));
			break;
		case 52008:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7901"));
			break;
		case 52010:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7905"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gkchat.flerogames.com/talk/server.php", "sendMsg", true, true)]
	public void SendGuildMsgChatting(int guild, int send, string snm, string msg, int ucash, int thmb, int lv)
	{
	}

	private IEnumerator SendGuildMsgChattingResult(JsonRpcClient.Request request, Protocols.SendChattingInfo result)
	{
		localUser.lastChatTimeTick = DateTime.Now.Ticks;
		string text = string.Empty;
		if (result.rewardList != null)
		{
			UIPopup.Create<UIGetItem>("GetItem").Set(result.rewardList, string.Empty);
			SoundManager.PlaySFX("SE_ItemGet_001");
			for (int i = 0; i < result.rewardList.Count; i++)
			{
				Protocols.RewardInfo.RewardData rewardData = result.rewardList[i];
				if (rewardData.rewardType == ERewardType.Commander)
				{
					text = rewardData.rewardId;
				}
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			RoCommander roCommander = localUser.FindCommander(text);
			if (roCommander != null)
			{
				UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				if (uICommanderComplete != null)
				{
					if (roCommander.state != ECommanderState.Nomal)
					{
						uICommanderComplete.Init(CommanderCompleteType.Recruit, roCommander.id);
					}
					else
					{
						uICommanderComplete.Init(CommanderCompleteType.Transmission, roCommander.id);
					}
				}
			}
		}
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshItemFromNetwork(result.eventResourceData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.AddCommanderFromNetwork(result.commanderData);
		localUser.RefreshCostumeFromNetwork(result.costumeData);
		localUser.RefreshItemFromNetwork(result.foodData);
		localUser.RefreshItemFromNetwork(result.groupItemData);
		RequestSendwaitGuildMsg();
		UIManager.instance.world.mainCommand.chat.TimeOutCount = 10;
		yield break;
	}

	private IEnumerator SendGuildMsgChattingError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 99004:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7054"));
			break;
		case 52003:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7903"));
			break;
		case 52005:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7904"));
			break;
		case 52007:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7902"));
			break;
		case 52008:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7901"));
			break;
		case 52010:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7905"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gkchat.flerogames.com/talk/server.php", "wait", true, true)]
	public void SendWaitChatMsg(int date, int guild)
	{
	}

	private IEnumerator SendWaitChatMsgResult(JsonRpcClient.Request request, Protocols.ChattingInfo result)
	{
		if (result.whisperList != null)
		{
			for (int i = 0; i < result.whisperList.Count; i++)
			{
				localUser.chattingWhisperList.Add(result.whisperList[i]);
			}
		}
		if (result.guildList != null)
		{
			for (int j = 0; j < result.guildList.Count; j++)
			{
				localUser.chattingGuildList.Add(result.guildList[j]);
			}
		}
		if (result.channelList != null)
		{
			for (int k = 0; k < result.channelList.Count; k++)
			{
				if (result.channelList[k].chatMsgData.record != null)
				{
					Protocols.ChattingInfo.ChattingData chattingData = new Protocols.ChattingInfo.ChattingData();
					chattingData.message = result.channelList[k].message;
					localUser.chattingchannelList.Add(chattingData);
					result.channelList[k].chatMsgData.data = Localization.Get("6143");
					result.channelList[k].chatMsgData.record = null;
					localUser.chattingchannelList.Add(result.channelList[k]);
				}
				else
				{
					localUser.chattingchannelList.Add(result.channelList[k]);
				}
			}
		}
		localUser.chattingDate = result.time;
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gkchat.flerogames.com/talk/server.php", "waitChannel", true, true)]
	public void SendwaitChannelMsg()
	{
	}

	private IEnumerator SendwaitChannelMsgResult(JsonRpcClient.Request request, object result)
	{
		if (result == null)
		{
			yield break;
		}
		Protocols.ChattingInfo chattingInfo = _ConvertJObject<Protocols.ChattingInfo>(result);
		if (chattingInfo.channelList != null)
		{
			for (int i = 0; i < chattingInfo.channelList.Count; i++)
			{
				if (chattingInfo.channelList[i].chatMsgData.record != null)
				{
					Protocols.ChattingInfo.ChattingData chattingData = new Protocols.ChattingInfo.ChattingData();
					chattingData.message = chattingInfo.channelList[i].message;
					localUser.chattingchannelList.Add(chattingData);
					chattingInfo.channelList[i].chatMsgData.data = Localization.Get("6143");
					chattingInfo.channelList[i].chatMsgData.record = null;
					localUser.chattingchannelList.Add(chattingInfo.channelList[i]);
				}
				else
				{
					localUser.chattingchannelList.Add(chattingInfo.channelList[i]);
				}
			}
		}
		while (localUser.chattingchannelList.Count > ConstValue.chatLimitCount)
		{
			if (localUser.chattingchannelList[0].chatMsgData.record != null)
			{
				localUser.chattingchannelList.RemoveAt(0);
				if (localUser.chattingchannelList.Count <= 0)
				{
					break;
				}
			}
			localUser.chattingchannelList.RemoveAt(0);
		}
		UIManager.instance.RefreshOpenedUI();
	}

	[JsonRpcClient.Request("http://gkchat.flerogames.com/talk/server.php", "waitGuild", true, true)]
	public void SendwaitGuildMsg()
	{
	}

	private IEnumerator SendwaitGuildMsgResult(JsonRpcClient.Request request, object result)
	{
		if (result == null)
		{
			yield break;
		}
		Protocols.ChattingInfo chattingInfo = _ConvertJObject<Protocols.ChattingInfo>(result);
		if (chattingInfo.guildList != null)
		{
			for (int i = 0; i < chattingInfo.guildList.Count; i++)
			{
				localUser.chattingGuildList.Add(chattingInfo.guildList[i]);
			}
		}
		UIManager.instance.RefreshOpenedUI();
	}

	[JsonRpcClient.Request("http://gkchat.flerogames.com/talk/server.php", "checkMsg", true, true)]
	public void CheckChattingMsg()
	{
	}

	private IEnumerator CheckChattingMsgResult(JsonRpcClient.Request request, string result, int guild, int channel)
	{
		UIManager.instance.world.mainCommand.chat.guildBadgeState = guild == 1;
		UIManager.instance.world.mainCommand.chat.channelBadgeState = channel == 1;
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1300", true, true)]
	public void GetChatIgnoreList()
	{
	}

	private IEnumerator GetChatIgnoreListResult(JsonRpcClient.Request request, List<Protocols.BlockUser> result)
	{
		localUser.blockUsers.Clear();
		for (int i = 0; i < result.Count; i++)
		{
			localUser.blockUsers.Add($"{result[i].channel}_{result[i].uno}", result[i]);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1301", true, true)]
	public void AddChatIgnore(int ch, string uno, string nick, string thumb)
	{
	}

	private IEnumerator AddChatIgnoreResult(JsonRpcClient.Request request, Protocols.BlockUser result)
	{
		localUser.blockUsers.Add($"{result.channel}_{result.uno}", result);
		NetworkAnimation.Instance.CreateFloatingText(string.Format(Localization.Get("7205"), result.nickName));
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1302", true, true)]
	public void DelChatIgnore(int ch, string uno)
	{
	}

	private IEnumerator DelChatIgnoreResult(JsonRpcClient.Request request, bool result)
	{
		if (result)
		{
			int num = int.Parse(_FindRequestProperty(request, "ch"));
			string arg = _FindRequestProperty(request, "uno");
			string key = $"{num}_{arg}";
			if (localUser.blockUsers.ContainsKey(key))
			{
				NetworkAnimation.Instance.CreateFloatingText(string.Format(Localization.Get("7211"), localUser.blockUsers[key].nickName));
				localUser.blockUsers.Remove(key);
				UIManager.instance.RefreshOpenedUI();
			}
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "5302", true, true)]
	public void BankInfo()
	{
	}

	private IEnumerator BankInfoResult(JsonRpcClient.Request request, Protocols.BankInfo result)
	{
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1503", true, true)]
	public void BankRoulletStart(int vidx, int cnt, int vcnt)
	{
	}

	private IEnumerator BankRoulletStartResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.Resource rsoc, int cnt, List<int> luck)
	{
		localUser.RefreshGoodsFromNetwork(rsoc);
		string key = 601.ToString();
		localUser.resourceRechargeList[key] = cnt;
		UIManager.instance.world.metroBank.RoulletPlay(luck);
		yield break;
	}

	private IEnumerator BankRoulletStartError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 53010)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7054"));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "5303", true, true)]
	public void GetBankReward()
	{
	}

	private IEnumerator GetBankRewardResult(JsonRpcClient.Request request, string result, long gold)
	{
		localUser.gold = (int)Math.Min(gold, 2147483647L);
		UIManager.instance.world.metroBank.InitUI();
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7203", true, true)]
	public void ShopBuyGold(int type)
	{
	}

	private IEnumerator ShopBuyGoldResult(JsonRpcClient.Request request, Protocols.UserInformationResponse.Resource result)
	{
		localUser.gold = result.gold;
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3113", true, true)]
	public void GetRaidRankList()
	{
	}

	private IEnumerator GetRaidRankListResult(JsonRpcClient.Request request, object result)
	{
		raidRankingList.Clear();
		if (result != null)
		{
			Protocols.PvPRankingList data = _ConvertJObject<Protocols.PvPRankingList>(result);
			for (int idx = 0; idx < data.rankList.Count; idx++)
			{
				raidRankingList.Add(RoUser.CreateRaidRankListUser(data.rankList[idx]));
				yield return null;
			}
			localUser.raidScore = data.user.score;
			localUser.raidRankingRate = data.user.rankingRate;
			localUser.raidGradeIdx = data.user.rewardId;
			localUser.raidRanking = data.user.ranking;
			localUser.raidCount = data.user.raidCnt;
			localUser.raidBestScore = data.user.bestScore;
			localUser.raidAverageScore = data.user.averageScore;
			localUser.raidRank = data.user.raidRank;
			localUser.raidRewardPoint = data.user.raidRewardPoint;
			if (!UIManager.instance.world.existRaid || !UIManager.instance.world.raid.isActive)
			{
				UIManager.instance.world.raid.InitAndOpen();
			}
			UIManager.instance.world.raid.SetRaidId(data.bossData, data.info.endTime);
			UIManager.instance.world.raid.Init();
		}
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3114", true, true)]
	public void GetRaidInfo()
	{
	}

	private IEnumerator GetRaidInfoResult(JsonRpcClient.Request request, string result)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3231", true, true)]
	public void RaidStart(int type, JObject deck, JObject gdp, int ucash, int bid, int np)
	{
	}

	private IEnumerator RaidStartResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.Resource rsoc)
	{
		localUser.RefreshGoodsFromNetwork(rsoc);
		Loading.Load(Loading.Battle);
		yield break;
	}

	private IEnumerator RaidStartError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 70009)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7044"));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1000", true, true)]
	public void GetRegion()
	{
	}

	private IEnumerator GetRegionResult(JsonRpcClient.Request request, Dictionary<string, Protocols.ChannelData> result)
	{
		M00_Init m00_Init = UnityEngine.Object.FindObjectOfType(typeof(M00_Init)) as M00_Init;
		if (m00_Init != null)
		{
			m00_Init.SelectLangeAndVersinonCheck(result);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8101", true, true)]
	public void DBVersionCheck(int ch)
	{
	}

	private IEnumerator DBVersionCheckResult(JsonRpcClient.Request request, string result, double ver)
	{
		localUser.serverDBVersion = ver;
		regulation.SetVersion(ver);
		M00_Init m00_Init = UnityEngine.Object.FindObjectOfType(typeof(M00_Init)) as M00_Init;
		if (m00_Init != null)
		{
			m00_Init.OnPatchStart();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3232", true, true)]
	public void BattleOut(int type, string checkSum, JArray info, JArray result)
	{
	}

	private IEnumerator BattleOutResult(JsonRpcClient.Request request, object result)
	{
		BattleData battleData = BattleData.Get();
		BattleData.Set(battleData);
		Protocols.UserInformationResponse.BattleResult battleResult = null;
		UIManager.instance.battle.Main.SendMessage("OnBattleOutResult", SendMessageOptions.DontRequireReceiver);
		if (battleData == null || (battleData.type != EBattleType.GuildScramble && result == null))
		{
			yield break;
		}
		if (battleData.type != EBattleType.GuildScramble)
		{
			battleResult = _ConvertJObject<Protocols.UserInformationResponse.BattleResult>(result);
			if (!localUser.statistics.isBuyVipShop && battleResult.VipShopOpen == 1)
			{
				localUser.statistics.vipShop = battleResult.VipShopOpen;
				localUser.statistics.vipShopResetTime_Data.SetByDuration(battleResult.VipShopResetTime);
				localUser.statistics.vipShopisFloating = true;
				ScheduleLocalPush(ELocalPushType.LeaveVipShop, battleResult.VipShopResetTime);
			}
		}
		bool isWin = battleData.isWin;
		UIBattleMain mainUI = UIManager.instance.battle.MainUI;
		UIBattleResult battleResult2 = UIManager.instance.battle.BattleResult;
		if (battleData.type == EBattleType.Plunder)
		{
			if (isWin)
			{
				WorldMapStageDataRow worldMapStageDataRow = regulation.worldMapStageDtbl[battleData.stageId];
				battleResult2.plunderResult.SetBattleTime((float)UIManager.instance.battle.Simulator.frame.time / 1000f);
				battleResult2.plunderResult.SetGetExp(worldMapStageDataRow.bullet);
				battleResult2.plunderResult.SetRewardDataAndOpen(battleResult.rewardList);
				if (localUser.lastClearStage < int.Parse(battleData.stageId))
				{
					localUser.lastClearStage = int.Parse(battleData.stageId);
				}
				if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
				{
					if (int.Parse(battleData.stageId) >= 23)
					{
						SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQBQ");
					}
					if (int.Parse(battleData.stageId) >= 43)
					{
						SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQBg");
					}
					if (int.Parse(battleData.stageId) >= 63)
					{
						SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQBw");
					}
					if (int.Parse(battleData.stageId) >= 83)
					{
						SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQCA");
					}
					if (int.Parse(battleData.stageId) >= 103)
					{
						SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQCQ");
					}
				}
				if (GameCenterManager.IsPlayerAuthenticated)
				{
					if (int.Parse(battleData.stageId) == 23)
					{
						GameCenterManager.SubmitAchievement(100f, "CgkIj7Xpxu4GEAIQBQ");
					}
					else if (int.Parse(battleData.stageId) == 43)
					{
						GameCenterManager.SubmitAchievement(100f, "CgkIj7Xpxu4GEAIQBg");
					}
					else if (int.Parse(battleData.stageId) == 63)
					{
						GameCenterManager.SubmitAchievement(100f, "CgkIj7Xpxu4GEAIQBw");
					}
					else if (int.Parse(battleData.stageId) == 83)
					{
						GameCenterManager.SubmitAchievement(100f, "CgkIj7Xpxu4GEAIQCA");
					}
					else if (int.Parse(battleData.stageId) == 103)
					{
						GameCenterManager.SubmitAchievement(100f, "CgkIj7Xpxu4GEAIQCQ");
					}
				}
			}
			if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
			{
				statistics.unitDestroyCount += battleData.unitKillCount;
				if (statistics.unitDestroyCount >= 200)
				{
					SA_Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIj7Xpxu4GEAIQDw");
				}
			}
			if (GameCenterManager.IsPlayerAuthenticated)
			{
				statistics.unitDestroyCount += battleData.unitKillCount;
				if (statistics.unitDestroyCount >= 200)
				{
					GameCenterManager.SubmitAchievement(100f, "CgkIj7Xpxu4GEAIQDw");
				}
			}
			UISetter.SetActive(battleResult2.plunderResult.btnWorldMap, active: true);
			UISetter.SetActive(battleResult2.plunderResult.btnCamp, active: true);
			UISetter.SetActive(battleResult2.plunderResult.btnSituation, active: false);
			UISetter.SetActive(battleResult2.plunderResult.btnAnnihilation, active: false);
			UISetter.SetActive(battleResult2.plunderResult.btnNextAnnihilation, active: false);
			UISetter.SetActive(battleResult2.plunderResult.btnWaveBattle, active: false);
			battleResult2.Set(battleData);
			if (battleData.isWin)
			{
				battleResult2.plunderResult.SetUpdataCommanderData(battleResult.commanderData);
			}
			localUser.useBullet = true;
			UISetter.SetActive(mainUI.main, active: false);
		}
		else if (battleData.type == EBattleType.Raid)
		{
			battleResult2.raidResult.SetBattleTime((float)UIManager.instance.battle.Simulator.frame.time / 1000f);
			battleResult2.raidResult.SetRank(battleResult.user.rank);
			battleResult2.raidResult.SetRankPersent(battleResult.user.rankPercent);
			battleResult2.raidResult.SetBaseScore(battleResult.user.prevScore);
			battleResult2.raidResult.SetScore(battleResult.user.prevScore, battleResult.user.curScore);
			battleResult2.Set(battleData);
			UISetter.SetActive(mainUI.main, active: false);
		}
		else if (battleData.type == EBattleType.Guerrilla || battleData.type == EBattleType.SeaRobber)
		{
			if (isWin)
			{
				battleResult2.plunderResult.SetBattleTime((float)UIManager.instance.battle.Simulator.frame.time / 1000f);
				battleResult2.plunderResult.SetRewardDataAndOpen(battleResult.rewardList);
				localUser.AddSweepClearState(battleData.sweepType, battleData.sweepLevel);
			}
			battleResult2.Set(battleData);
			UISetter.SetActive(mainUI.main, active: false);
		}
		else if (battleData.type == EBattleType.Annihilation)
		{
			if (isWin)
			{
				battleResult2.plunderResult.SetBattleTime((float)UIManager.instance.battle.Simulator.frame.time / 1000f);
				if (battleResult.resource != null)
				{
					battleResult2.plunderResult.SetGetExp(0);
				}
				if (battleResult.rewardList != null)
				{
					battleResult2.plunderResult.SetRewardDataAndOpen(battleResult.rewardList);
				}
			}
			int num = localUser.lastClearAnnihilationStage + 1;
			if (localUser.lastClearAnnihilationStage > 100 && localUser.lastClearAnnihilationStage < 200)
			{
				num -= 100;
			}
			else if (localUser.lastClearAnnihilationStage > 200)
			{
				num -= 200;
			}
			UISetter.SetActive(battleResult2.plunderResult.btnNextAnnihilation, isWin && num <= 21);
			battleResult2.Set(battleData);
			UISetter.SetActive(mainUI.main, active: false);
		}
		else if (battleData.type == EBattleType.WaveBattle)
		{
			battleResult2.plunderResult.SetRewardDataAndOpen(battleResult.rewardList);
			battleResult2.Set(battleData);
			UISetter.SetActive(mainUI.main, active: false);
		}
		else if (battleData.type == EBattleType.CooperateBattle)
		{
			battleResult2.Set(battleData);
			UISetter.SetActive(mainUI.main, active: false);
		}
		else if (battleData.type == EBattleType.EventBattle)
		{
			if (isWin)
			{
				battleResult2.plunderResult.SetBattleTime((float)UIManager.instance.battle.Simulator.frame.time / 1000f);
				battleResult2.plunderResult.SetGetExp(0);
				battleResult2.plunderResult.SetRewardDataAndOpen(battleResult.rewardList);
			}
			battleResult2.Set(battleData);
			if (battleData.isWin)
			{
				battleResult2.plunderResult.SetUpdataCommanderData(battleResult.commanderData);
			}
			UISetter.SetActive(mainUI.main, active: false);
		}
		else if (battleData.type == EBattleType.EventRaid)
		{
			battleResult2.eventRaidResult.SetRewardData(battleResult.rewardList);
			battleResult2.Set(battleData);
			UISetter.SetActive(mainUI.main, active: false);
		}
		else if (battleData.type == EBattleType.InfinityBattle)
		{
			battleResult2.Set(battleData);
			battleResult2.infinityBattleResult.data = battleResult.infinityData;
			UISetter.SetActive(mainUI.main, active: false);
		}
		if (!GameSetting.instance.repeatBattle)
		{
			battleResult2.Open();
		}
		if (battleResult != null)
		{
			localUser.RefreshGoodsFromNetwork(battleResult.resource);
			localUser.RefreshPartFromNetwork(battleResult.partData);
			localUser.RefreshMedalFromNetwork(battleResult.medalData);
			localUser.RefreshFavorFromNetwork(battleResult.commanderFavor);
			localUser.RefreshItemFromNetwork(battleResult.eventResourceData);
			localUser.RefreshItemFromNetwork(battleResult.itemData);
			localUser.RefreshItemFromNetwork(battleResult.foodData);
			localUser.RefreshItemFromNetwork(battleResult.groupItemData);
		}
		UIManager.instance.RefreshOpenedUI();
	}

	private IEnumerator BattleOutError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 70009)
		{
			UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, Localization.Get("7044"), Localization.Get("1004"));
			if (uISimplePopup != null)
			{
				uISimplePopup.onClose = delegate
				{
					BattleData battleData = BattleData.Get();
					battleData.move = EBattleResultMove.WorldMap;
					Loading.Load(Loading.WorldMap);
				};
			}
			yield break;
		}
		UISimplePopup uISimplePopup2 = UISimplePopup.CreateOK(localization: false, Localization.Get("19013"), string.Empty, Localization.Get("19014") + "\n(ErrorCode:" + code + ")", Localization.Get("5133"));
		if (uISimplePopup2 != null)
		{
			uISimplePopup2.onClose = delegate
			{
				Application.Quit();
			};
		}
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1217", true, true)]
	public void BulletCharge()
	{
	}

	private IEnumerator BulletChargeResult(JsonRpcClient.Request request, Protocols.ResourceRecharge result)
	{
		localUser.bullet = result.bulletData.cnt;
		localUser.bulletRemain = result.bulletData.remain;
		localUser.oil = result.oilData.cnt;
		localUser.oilRemain = result.oilData.remain;
		localUser.weaponMaterial1 = result.weaponMaterialData1.cnt;
		localUser.weaponMaterialRemainTime1.SetByDuration(result.weaponMaterialData1.remain);
		localUser.weaponMaterial2 = result.weaponMaterialData2.cnt;
		localUser.weaponMaterialRemainTime2.SetByDuration(result.weaponMaterialData2.remain);
		localUser.weaponMaterial3 = result.weaponMaterialData3.cnt;
		localUser.weaponMaterialRemainTime3.SetByDuration(result.weaponMaterialData3.remain);
		localUser.weaponMaterial4 = result.weaponMaterialData4.cnt;
		localUser.weaponMaterialRemainTime4.SetByDuration(result.weaponMaterialData4.remain);
		localUser.badgeWorldMap = result.worldState != -1;
		foreach (Protocols.GachaInformationResponse value in result.gacha.Values)
		{
			localUser.RefreshGachaFromNetwork(value);
		}
		if (localUser.useBullet)
		{
			ScheduleLocalPush(ELocalPushType.BulletFullCharge, localUser.bulletRemain);
			localUser.useBullet = false;
		}
		if (UIManager.instance.world != null)
		{
			UIManager.instance.world.mainCommand.BulletControl();
			UIManager.instance.world.mainCommand.OilControl();
			if (UIManager.instance.world.existWeaponResearch && UIManager.instance.world.weaponResearch.isActive)
			{
				UIManager.instance.world.weaponResearch.inProgress.WeaponMaterialControl();
			}
			UIManager.instance.RefreshOpenedUI();
		}
		if (UIManager.instance.battle != null)
		{
			UIBattleResult battleResult = UIManager.instance.battle.BattleResult;
			battleResult.Open();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1503", true, true)]
	public void ResourceRecharge(int vidx, int mid, int vcnt)
	{
	}

	private IEnumerator ResourceRechargeResult(JsonRpcClient.Request request, object result, object rsoc)
	{
		string text = _FindRequestProperty(request, "vidx");
		int num = 0;
		VipRechargeDataRow vipRechargeDataRow = regulation.vipRechargeDtbl[text];
		if (vipRechargeDataRow.type == 1 || vipRechargeDataRow.type == 4)
		{
			if (localUser.resourceRechargeList.ContainsKey(text))
			{
				num = localUser.resourceRechargeList[text];
			}
			else
			{
				localUser.resourceRechargeList.Add(text, num);
			}
			num++;
			localUser.resourceRechargeList[text] = num;
			Protocols.UserInformationResponse userInformationResponse = _ConvertJObject<Protocols.UserInformationResponse>(result);
			localUser.RefreshGoodsFromNetwork(userInformationResponse.goodsInfo);
			if (text == 109.ToString())
			{
				localUser.changeSkillPoint = true;
				RequestBulletCharge();
			}
			else if (text == 401.ToString())
			{
				_ = UIManager.instance.battle.MainUI;
				UIBattleResult battleResult = UIManager.instance.battle.BattleResult;
				battleResult.duelResult.ResetLoseScore();
			}
		}
		else if (vipRechargeDataRow.type == 2)
		{
			string text2 = _FindRequestProperty(request, "mid");
			if (localUser.stageRechargeList.ContainsKey(text2))
			{
				num = localUser.stageRechargeList[text2];
			}
			else
			{
				localUser.stageRechargeList.Add(text2, num);
			}
			num++;
			localUser.stageRechargeList[text2] = num;
			RoWorldMap.Stage stage = localUser.FindWorldMapStage(text2);
			stage.clearCount = 0;
			Protocols.UserInformationResponse.Resource source = _ConvertJObject<Protocols.UserInformationResponse.Resource>(rsoc);
			localUser.RefreshGoodsFromNetwork(source);
		}
		else if (vipRechargeDataRow.type == 3)
		{
			if (localUser.resourceRechargeList.ContainsKey(text))
			{
				num = localUser.resourceRechargeList[text];
			}
			else
			{
				localUser.resourceRechargeList.Add(text, num);
			}
			num++;
			localUser.resourceRechargeList[text] = num;
			Protocols.RecruitCommanderListResponse recruitCommanderListResponse = _ConvertJObject<Protocols.RecruitCommanderListResponse>(result);
			_RefreshRecruitList(recruitCommanderListResponse);
			localUser.RefreshGoodsFromNetwork(recruitCommanderListResponse.resource);
		}
		if (UIManager.instance.world != null && UIManager.instance.world.existCarnival && UIManager.instance.world.carnival.isActive)
		{
			RequestGetCarnivalList(UIManager.instance.world.carnival.categoryType);
		}
		else if ((vipRechargeDataRow.type != 1 && vipRechargeDataRow.type != 4) || !(text == 109.ToString()))
		{
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator ResourceRechargeError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 53010)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7054"));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3133", true, true)]
	public void GetReplayList(ERePlayType type, ERePlaySubType stype, int ver)
	{
	}

	private IEnumerator GetReplayListResult(JsonRpcClient.Request request, List<Protocols.RecordInfo> result)
	{
		if (result == null)
		{
			yield break;
		}
		_ = UIManager.instance.world;
		ERePlayType eRePlayType = (ERePlayType)_ConvertStringToInt(_FindRequestProperty(request, "type"));
		if (eRePlayType == ERePlayType.Challenge || eRePlayType == ERePlayType.WaveDuel)
		{
			ERePlaySubType eRePlaySubType = (ERePlaySubType)_ConvertStringToInt(_FindRequestProperty(request, "stype"));
			if (eRePlaySubType == ERePlaySubType.Attack)
			{
				localUser.atkRecordList = result;
			}
			else
			{
				localUser.defRecordList = result;
			}
		}
		UIManager.instance.RefreshOpenedUI();
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3134", true, true)]
	public void GetReplayInfo(string rid, ERePlayType type)
	{
	}

	private IEnumerator GetReplayInfoResult(JsonRpcClient.Request request, Protocols.RecordInfo result)
	{
		if (result == null)
		{
			yield break;
		}
		if (result.data == null)
		{
			if (localUser.playingChatRecord != null)
			{
				localUser.playingChatRecord.hasRecord = false;
				localUser.playingChatRecord = null;
			}
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19079"));
			yield break;
		}
		localUser.playingChatRecord = null;
		_ = Regulation.SerializerSettings;
		Record record = (Record)JsonConvert.DeserializeObject<JToken>(result.data.ToString());
		BattleData battleData = BattleData.Get();
		battleData.isReplayMode = true;
		battleData.record = record;
		battleData.attacker.nickname = record.initState.dualData._playerName;
		battleData.attacker.level = record.initState.dualData._playerLevel;
		battleData.attacker.guildName = record.initState.dualData._playerGuildName;
		battleData.defender.nickname = record.initState.dualData._enemyName;
		battleData.defender.level = record.initState.dualData._enemyLevel;
		battleData.defender.duelRanking = record.initState.dualData._enemyRank;
		battleData.defender.guildName = record.initState.dualData._enemyGuildName;
		battleData.defender.uno = record.initState.dualData._enemyUno;
		BattleData.Set(battleData);
		Loading.Load(Loading.Battle);
	}

	private IEnumerator GetReplayInfoError(JsonRpcClient.Request request, string result, int code)
	{
		if (localUser.playingChatRecord != null)
		{
			localUser.playingChatRecord.hasRecord = false;
			localUser.playingChatRecord = null;
		}
		NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19079"));
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3133", true, true)]
	public void GetRecordList(int type, int ver)
	{
	}

	private IEnumerator GetRecordListResult(JsonRpcClient.Request request, List<Protocols.RecordInfo> result)
	{
		if (result != null)
		{
			_ = UIManager.instance.world;
			int num = _ConvertStringToInt(_FindRequestProperty(request, "type"));
			if (num == 6 || num == 17)
			{
				localUser.atkRecordList = result;
			}
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1103", true, true)]
	public void GetTutorialStep(List<string> type)
	{
	}

	private IEnumerator GetTutorialStepResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.TutorialData ttrl)
	{
		localUser.tutorialStep = ttrl.step;
		localUser.isTutorialSkip = ttrl.skip;
		if (!localUser.isTutorialSkip && localUser.tutorialStep <= 1)
		{
			bool isTutorialSkip = false;
			UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: true, "19800", "19801", null, "1304", "1305");
			uISimplePopup.onClick = delegate(GameObject sender)
			{
				string text = sender.name;
				if (text == "OK")
				{
					isTutorialSkip = true;
				}
			};
			uISimplePopup.onClose = delegate
			{
				if (isTutorialSkip)
				{
					AdjustManager.Instance.SimpleEvent("rkj4ku");
					RequestLoginTutorialSkip();
				}
				else
				{
					AdjustManager.Instance.SimpleEvent("biks65");
					OnLoginSuccess();
				}
			};
		}
		else
		{
			OnLoginSuccess();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1219", true, true)]
	public void LoginTutorialSkip(int skip)
	{
	}

	private IEnumerator LoginTutorialSkipResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.TutorialData ttrl)
	{
		localUser.isTutorialSkip = ttrl.skip;
		OnLoginSuccess();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1218", true, true)]
	public void UpdateTutorialStep(int step)
	{
	}

	private IEnumerator UpdateTutorialStepResult(JsonRpcClient.Request request, string result, int step)
	{
		localUser.tutorialStep = step;
		if (step == 12 && FB.IsInitialized)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["fb_content_id"] = "CompletedTutorial";
			string logEvent = "fb_mobile_tutorial_completion";
			Dictionary<string, object> parameters = dictionary;
			FB.LogAppEvent(logEvent, null, parameters);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1408", true, true)]
	public void SetNickNameFromTutorial(string unm, int step)
	{
	}

	private IEnumerator SetNickNameFromTutorialResult(JsonRpcClient.Request request, string result, int step)
	{
		AdjustManager.Instance.SimpleEvent("50lc50");
		if (FB.IsInitialized)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["fb_content_id"] = "CompletedRegistration";
			string logEvent = "fb_mobile_complete_registration";
			Dictionary<string, object> parameters = dictionary;
			FB.LogAppEvent(logEvent, null, parameters);
		}
		if (localUser.isTutorialSkip)
		{
			Protocols.UserInformationResponse userInformationResponse = JsonConvert.DeserializeObject<Protocols.UserInformationResponse>(result);
			if (userInformationResponse != null)
			{
				localUser.ResetCommanderMedals();
				localUser.FromNetwork(userInformationResponse);
				localUser.lastClearStage = userInformationResponse.stage;
			}
		}
		localUser.tutorialStep = step;
		yield break;
	}

	private IEnumerator SetNickNameFromTutorialError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 20014:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7054"));
			break;
		case 20005:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7145"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3227", true, true)]
	public void ReceivePvPReward()
	{
	}

	private IEnumerator ReceivePvPRewardResult(JsonRpcClient.Request request, string result)
	{
		UIPopup.Create<ReceiveDuelRewardPopup>("ReceiveDuelRewardPopup").Set(PvPRewardType.PvP);
		localUser.badgeChallenge = false;
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3230", true, true)]
	public void ReceiveDuelPointReward(int didx)
	{
	}

	private IEnumerator ReceiveDuelPointRewardResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		localUser.rewardDuelPoint = result.duelScoreData["didx"];
		localUser.badgeChallenge = localUser.duelPoint >= localUser.rewardDuelPoint && localUser.rewardDuelPoint != 0;
		localUser.RefreshRewardFromNetwork(result);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3215", true, true)]
	public void ReceiveRaidReward()
	{
	}

	private IEnumerator ReceiveRaidRewardResult(JsonRpcClient.Request request, string result)
	{
		UIPopup.Create<ReceiveDuelRewardPopup>("ReceiveDuelRewardPopup").Set(PvPRewardType.Raid);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2208", true, true)]
	public void UseTimeMachine(int mid, int cnt, int cash)
	{
	}

	private IEnumerator UseTimeMachineResult(JsonRpcClient.Request request, Protocols.PlunderTimeMachine result)
	{
		string stageId = _FindRequestProperty(request, "mid");
		string s = _FindRequestProperty(request, "cnt");
		localUser.FindWorldMapStage(stageId).clearCount += int.Parse(s);
		localUser.useBullet = true;
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshItemFromNetwork(result.eventResourceData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.RefreshItemFromNetwork(result.groupItemData);
		if (!localUser.statistics.isBuyVipShop && result.VipShopOpen == 1)
		{
			localUser.statistics.vipShopResetTime_Data.SetByDuration(result.VipShopRemainTime);
			localUser.statistics.vipShop = result.VipShopOpen;
			localUser.statistics.vipShopisFloating = true;
			ScheduleLocalPush(ELocalPushType.LeaveVipShop, result.VipShopRemainTime);
		}
		UIPopup.Create<UITimeMachinePopup>("TimeMachinePopup").Init(result.reward, ETimeMachineType.Stage);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3201", true, true)]
	public void UseTimeMachineSweep(int stype, int lv, int cnt)
	{
	}

	private IEnumerator UseTimeMachineSweepResult(JsonRpcClient.Request request, Protocols.PlunderTimeMachine result)
	{
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshItemFromNetwork(result.eventResourceData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.RefreshItemFromNetwork(result.foodData);
		localUser.RefreshItemFromNetwork(result.groupItemData);
		if (!localUser.statistics.isBuyVipShop && result.VipShopOpen == 1)
		{
			localUser.statistics.vipShopResetTime_Data.SetByDuration(result.VipShopRemainTime);
			localUser.statistics.vipShop = result.VipShopOpen;
			localUser.statistics.vipShopisFloating = true;
			ScheduleLocalPush(ELocalPushType.LeaveVipShop, result.VipShopRemainTime);
		}
		UIManager.instance.RefreshOpenedUI();
		UIPopup.Create<UITimeMachinePopup>("TimeMachinePopup").Init(result.reward, ETimeMachineType.Sweep);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1504", true, true)]
	public void CheckBadge()
	{
	}

	private IEnumerator CheckBadgeResult(JsonRpcClient.Request request, string result, int arena, int dlms, int achv, int rwd, Dictionary<string, int> shop, List<string> cnvl, int ccnv, List<string> cnvl2, int ccnv2, List<string> cnvl3, int ccnv3, int wb, int gb, int grp, int ercnt, int iftw)
	{
		localUser.badgeChallenge = arena > 0;
		localUser.badgeMissionCount = dlms;
		localUser.badgeAchievementCount = achv;
		localUser.badgeNewMailCount = rwd;
		localUser.badgeRaidShop = shop.ContainsKey("raid") && shop["raid"] > 0;
		localUser.badgeChallengeShop = shop.ContainsKey("arena") && shop["arena"] > 0;
		localUser.badgeWaveDuelShop = shop.ContainsKey("arena3") && shop["arena3"] > 0;
		localUser.badgeCarnivalTabList[1] = cnvl;
		localUser.badgeCarnivalComplete[1] = ((ccnv != 0) ? true : false);
		localUser.badgeCarnivalTabList[2] = cnvl2;
		localUser.badgeCarnivalComplete[2] = ((ccnv2 != 0) ? true : false);
		localUser.badgeCarnivalTabList[3] = cnvl3;
		localUser.badgeCarnivalComplete[3] = ((ccnv3 != 0) ? true : false);
		localUser.badgeWaveBattle = ((wb != 0) ? true : false);
		localUser.badgeGuild = ((gb != 0) ? true : false);
		localUser.badgeGroupCount = grp;
		localUser.badgeEventRaidReward = ercnt > 0;
		localUser.badgeInfinityBattleReward = iftw > 0;
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1502", true, true)]
	public void ChangeUserThumbnail(int idx)
	{
	}

	private IEnumerator ChangeUserThumbnailResult(JsonRpcClient.Request request, string result)
	{
		if (result.Equals("True") || result.Equals("true"))
		{
			string id = _FindRequestProperty(request, "idx");
			localUser.thumbnailId = localUser.FindCommander(id).currentCostume.ToString();
			UIManager.instance.world.mainCommand.spineTest.Set();
			UIManager.instance.world.mainCommand.spineTest.Show();
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1505", true, true)]
	public void CheckAlarm()
	{
	}

	private IEnumerator CheckAlarmResult(JsonRpcClient.Request request, Protocols.AlarmData result)
	{
		localUser.alarmData = result;
		UIManager.instance.world.alarm.InitAndOpenAlarm();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8200", true, true)]
	public void GetSecretShopList(int styp)
	{
	}

	private IEnumerator GetSecretShopListResult(JsonRpcClient.Request request, Protocols.SecretShop result)
	{
		if (result != null && result.shopList.Count != 0)
		{
			string text = _FindRequestProperty(request, "styp");
			if (text == 3.ToString())
			{
				localUser.badgeRaidShop = false;
			}
			else if (text == 2.ToString())
			{
				localUser.badgeChallengeShop = false;
			}
			else if (text == 7.ToString())
			{
				localUser.badgeWaveDuelShop = false;
			}
			localUser.shopList = result.shopList;
			localUser.shopRefreshTime.SetByDuration(result.refreshTime);
			localUser.shopRefreshCount = result.refreshCount;
			localUser.shopRefreshFree = result.reset == 0;
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8202", true, true)]
	public void RefreshSecretShopList(int styp)
	{
	}

	private IEnumerator RefreshSecretShopListResult(JsonRpcClient.Request request, Protocols.SecretShop result)
	{
		if (result != null && result.shopList.Count != 0)
		{
			localUser.shopList = result.shopList;
			localUser.RefreshGoodsFromNetwork(result.resource);
			localUser.shopRefreshCount = result.refreshCount;
			localUser.shopRefreshFree = result.reset == 0;
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7100", true, true)]
	public void GetCashShopList()
	{
	}

	private IEnumerator GetCashShopListResult(JsonRpcClient.Request request, List<Protocols.CashShopData> result)
	{
		if (result != null)
		{
			if (result.Count != 0)
			{
				regulation.cashShopDtbl = result;
			}
			if (UIManager.instance.world != null)
			{
				UIManager.instance.world.mainCommand.OpenAndInitDiamondShop();
			}
			Message.Send("Shop.Open.CashShop");
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7202", true, true)]
	public void MakeOrderId(string productId)
	{
	}

	private IEnumerator MakeOrderIdResult(JsonRpcClient.Request request, string result, string payload)
	{
		localUser.lastIOSPayload = payload;
		GameBillingManager.purchase(localUser.lastBuyPid, payload);
		yield break;
	}

	private IEnumerator MakeOrderIdError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 10129:
			NetworkAnimation.Instance.CreateFloatingText("Error Code:" + code);
			break;
		case 10128:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("10000207"));
			break;
		default:
			NetworkAnimation.Instance.CreateFloatingText("Error Code:" + code);
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7201", true, true)]
	public void CheckPayment(string packageName, string productId, double purchaseTime, int purchaseState, string developerPayload, string purchaseToken, string orderId)
	{
	}

	private IEnumerator CheckPaymentResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		StartCoroutine(CheckPaymentTotalResult(request, result));
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7201", true, true)]
	public void CheckPaymentIOS(string productId, string receipt, string developerPayload)
	{
	}

	private IEnumerator CheckPaymentIOSResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		StartCoroutine(CheckPaymentTotalResult(request, result));
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7201", true, true)]
	public void CheckPaymentOneStore(string productId, string developerPayload, string txid, string signdata)
	{
	}

	private IEnumerator CheckPaymentOneStoreResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		StartCoroutine(CheckPaymentTotalResult(request, result));
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7201", true, true)]
	public void CheckPaymentAmazon(string productId, string userId, string receipt, string developerPayload)
	{
	}

	private IEnumerator CheckPaymentAmazonResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		StartCoroutine(CheckPaymentTotalResult(request, result));
		yield break;
	}

	private IEnumerator CheckPaymentTotalResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		UIManager.World world = UIManager.instance.world;
		bool flag = localUser.vipLevel != result.resource.vipLevel;
		float inAppPrice = result.inAppPrice;
		if (result.reward != null && result.reward.Count > 0)
		{
			UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
			SoundManager.PlaySFX("SE_ItemGet_001");
		}
		if (result.userInfo != null)
		{
			localUser.statistics.firstPayment = result.userInfo.firstPayment;
		}
		localUser.RefreshRewardFromNetwork(result);
		UIManager.instance.RefreshOpenedUI();
		string text = _FindRequestProperty(request, "productId");
		if (FB.IsInitialized)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["fb_content_id"] = "AddedPaymentInfo";
			dictionary["fb_payment_info_available"] = text;
			string logEvent = "fb_mobile_add_payment_info";
			Dictionary<string, object> parameters = dictionary;
			FB.LogAppEvent(logEvent, null, parameters);
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2["mygame_packagename"] = text;
			FB.LogPurchase(inAppPrice, "USD", dictionary2);
		}
		switch (text)
		{
		case "gk.dia.100":
			AdjustManager.Instance.RevenueEvent("lt7ya0", inAppPrice);
			break;
		case "gk.dia.400":
			AdjustManager.Instance.RevenueEvent("bvtogn", inAppPrice);
			break;
		case "gk.dia.1200":
			AdjustManager.Instance.RevenueEvent("pkqgvx", inAppPrice);
			break;
		case "gk.dia.2000":
			AdjustManager.Instance.RevenueEvent("ebptpz", inAppPrice);
			break;
		case "gk.dia.4000":
			AdjustManager.Instance.RevenueEvent("sa7ufx", inAppPrice);
			break;
		case "gk.package.monthly":
			AdjustManager.Instance.RevenueEvent("sdk3vu", inAppPrice);
			break;
		case "gk.package.monthly03":
			AdjustManager.Instance.RevenueEvent("p2prt3", inAppPrice);
			break;
		case "gk.package.starter.01":
			AdjustManager.Instance.RevenueEvent("8ycf4m", inAppPrice);
			break;
		case "gk.package.starter.02":
			AdjustManager.Instance.RevenueEvent("w2ifnn", inAppPrice);
			break;
		case "gk.package.starter.03":
			AdjustManager.Instance.RevenueEvent("urqrls", inAppPrice);
			break;
		case "gk.package.starter.04":
			AdjustManager.Instance.RevenueEvent("2x7ggq", inAppPrice);
			break;
		case "gk.package.starter.05":
			AdjustManager.Instance.RevenueEvent("apyz3u", inAppPrice);
			break;
		case "gk.package.starter.06":
			AdjustManager.Instance.RevenueEvent("34rggg", inAppPrice);
			break;
		case "gk.package.starter.07":
			AdjustManager.Instance.RevenueEvent("kvwr3j", inAppPrice);
			break;
		case "gk.package.starter.08":
			AdjustManager.Instance.RevenueEvent("cyres1", inAppPrice);
			break;
		case "gk.package.gold.01":
			AdjustManager.Instance.RevenueEvent("hguwxm", inAppPrice);
			break;
		case "gk.package.interlevel":
			AdjustManager.Instance.RevenueEvent("veflju", inAppPrice);
			break;
		case "gk.package.ticket.01":
			AdjustManager.Instance.RevenueEvent("x74eow", inAppPrice);
			break;
		case "gk.package.gift.01":
			AdjustManager.Instance.RevenueEvent("vsrt1x", inAppPrice);
			break;
		case "gk.package.ticket.02":
			AdjustManager.Instance.RevenueEvent("s1hrug", inAppPrice);
			break;
		case "gk.package.gift.02":
			AdjustManager.Instance.RevenueEvent("y08121", inAppPrice);
			break;
		case "gk.package.pilot01":
			AdjustManager.Instance.RevenueEvent("nk6cdb", inAppPrice);
			break;
		case "gk.package.pilot02":
			AdjustManager.Instance.RevenueEvent("l7s441", inAppPrice);
			break;
		case "gk.package.pilot03":
			AdjustManager.Instance.RevenueEvent("kegoyx", inAppPrice);
			break;
		case "gk.package.monthly.gold":
			AdjustManager.Instance.RevenueEvent("deeg8e", inAppPrice);
			break;
		case "gk.package.gift.premium":
			AdjustManager.Instance.RevenueEvent("kojdub", inAppPrice);
			break;
		case "gk.package.growth.gold":
			AdjustManager.Instance.RevenueEvent("wcxlp0", inAppPrice);
			break;
		case "gk.package.enhance.red":
			AdjustManager.Instance.RevenueEvent("wd28jj", inAppPrice);
			break;
		case "gk.package.wring.basic":
			AdjustManager.Instance.RevenueEvent("ryttek", inAppPrice);
			break;
		case "gk.package.wring.adv":
			AdjustManager.Instance.RevenueEvent("xhk2w7", inAppPrice);
			break;
		case "gk.package.baldr01":
			AdjustManager.Instance.RevenueEvent("k4sx0m", inAppPrice);
			break;
		case "gk.package.baldr02":
			AdjustManager.Instance.RevenueEvent("kclizw", inAppPrice);
			break;
		case "gk.package.baldr03":
			AdjustManager.Instance.RevenueEvent("mmrhb7", inAppPrice);
			break;
		case "gk.package.growth.commander":
			AdjustManager.Instance.RevenueEvent("hljqgz", inAppPrice);
			break;
		case "gk.package.clear.stage":
			AdjustManager.Instance.RevenueEvent("uzng79", inAppPrice);
			break;
		case "gk.package.starter.booster":
			AdjustManager.Instance.RevenueEvent("yfhiut", inAppPrice);
			break;
		case "gk.package.new.pilot01":
			AdjustManager.Instance.RevenueEvent("xowv93", inAppPrice);
			break;
		case "gk.package.new.pilot02":
			AdjustManager.Instance.RevenueEvent("30ghla", inAppPrice);
			break;
		case "gk.package.special.pilot":
			AdjustManager.Instance.RevenueEvent("vdw3ym", inAppPrice);
			break;
		case "gk.package.special.limit01":
			AdjustManager.Instance.RevenueEvent("woaev4", inAppPrice);
			break;
		case "gk.package.special.limit02":
			AdjustManager.Instance.RevenueEvent("vcffuz", inAppPrice);
			break;
		case "gk.package.pilot04":
			AdjustManager.Instance.RevenueEvent("bqmi4j", inAppPrice);
			break;
		case "gk.package.pilot05":
			AdjustManager.Instance.RevenueEvent("9895vi", inAppPrice);
			break;
		case "gk.package.pilot06":
			AdjustManager.Instance.RevenueEvent("v4sy21", inAppPrice);
			break;
		case "gk.package.pilot07":
			AdjustManager.Instance.RevenueEvent("vadwa7", inAppPrice);
			break;
		case "gk.package.pilot08":
			AdjustManager.Instance.RevenueEvent("ys1o2h", inAppPrice);
			break;
		case "gk.package.dia.200.discount":
			AdjustManager.Instance.RevenueEvent("jc6h46", inAppPrice);
			break;
		case "gk.package.dia.400.discount":
			AdjustManager.Instance.RevenueEvent("2b671h", inAppPrice);
			break;
		case "gk.package.dia.1200.discount":
			AdjustManager.Instance.RevenueEvent("2g3c8q", inAppPrice);
			break;
		case "gk.package.dia.2000.discount":
			AdjustManager.Instance.RevenueEvent("kk0hhd", inAppPrice);
			break;
		case "gk.package.dia.4000.discount":
			AdjustManager.Instance.RevenueEvent("m452dx", inAppPrice);
			break;
		case "gk.package.lvlboost.01":
			AdjustManager.Instance.RevenueEvent("7r5n59", inAppPrice);
			break;
		case "gk.package.lvlboost.02":
			AdjustManager.Instance.RevenueEvent("jxyi67", inAppPrice);
			break;
		case "gk.package.lvlboost.03":
			AdjustManager.Instance.RevenueEvent("jsqrng", inAppPrice);
			break;
		case "gk.package.lvlboost.04":
			AdjustManager.Instance.RevenueEvent("6cabyw", inAppPrice);
			break;
		case "gk.package.lvlboost.05":
			AdjustManager.Instance.RevenueEvent("8dqwce", inAppPrice);
			break;
		case "gk.package.dormitory01":
			AdjustManager.Instance.RevenueEvent("qvslr2", inAppPrice);
			break;
		case "gk.package.monthly.dorm":
			AdjustManager.Instance.RevenueEvent("fa99ax", inAppPrice);
			break;
		case "gk.package.ltd.bonuspack":
			AdjustManager.Instance.RevenueEvent("lkpu8i", inAppPrice);
			break;
		case "gk.package.crystal.a":
			AdjustManager.Instance.RevenueEvent("on5rkm", inAppPrice);
			break;
		case "gk.package.crystal.b":
			AdjustManager.Instance.RevenueEvent("146e6o", inAppPrice);
			break;
		case "gk.package.crystal.c":
			AdjustManager.Instance.RevenueEvent("2qiudc", inAppPrice);
			break;
		case "gk.package.crystal.d":
			AdjustManager.Instance.RevenueEvent("g6uyzo", inAppPrice);
			break;
		case "gk.package.crystal.e":
			AdjustManager.Instance.RevenueEvent("vyefkn", inAppPrice);
			break;
		case "gk.package.crystal.f":
			AdjustManager.Instance.RevenueEvent("l0aamv", inAppPrice);
			break;
		case "gk.package.crystal.g":
			AdjustManager.Instance.RevenueEvent("pftp6l", inAppPrice);
			break;
		case "gk.package.crystal.h":
			AdjustManager.Instance.RevenueEvent("b8dl5n", inAppPrice);
			break;
		case "gk.package.crystal.i":
			AdjustManager.Instance.RevenueEvent("dsgwkf", inAppPrice);
			break;
		case "gk.package.dia.8000":
			AdjustManager.Instance.RevenueEvent("5jhck2", inAppPrice);
			break;
		case "gk.package.monthly.gold02":
			AdjustManager.Instance.RevenueEvent("f6mbwp", inAppPrice);
			break;
		case "gk.package.monthly.affection":
			AdjustManager.Instance.RevenueEvent("f0jzwl", inAppPrice);
			break;
		case "gk.package.monthly.premium.affection":
			AdjustManager.Instance.RevenueEvent("76kz46", inAppPrice);
			break;
		case "gk.package.5500r1":
			AdjustManager.Instance.RevenueEvent("4f3jtz", inAppPrice);
			break;
		case "gk.package.5500r2":
			AdjustManager.Instance.RevenueEvent("pazdpf", inAppPrice);
			break;
		case "gk.package.11000r1":
			AdjustManager.Instance.RevenueEvent("o0wt43", inAppPrice);
			break;
		case "gk.package.11000r2":
			AdjustManager.Instance.RevenueEvent("pzs0fq", inAppPrice);
			break;
		case "gk.package.11000r3":
			AdjustManager.Instance.RevenueEvent("slj6as", inAppPrice);
			break;
		case "gk.package.11000r4":
			AdjustManager.Instance.RevenueEvent("bor0w5", inAppPrice);
			break;
		case "gk.package.11000r5":
			AdjustManager.Instance.RevenueEvent("74fg8t", inAppPrice);
			break;
		case "gk.package.33000r1":
			AdjustManager.Instance.RevenueEvent("1b77tz", inAppPrice);
			break;
		case "gk.package.33000r2":
			AdjustManager.Instance.RevenueEvent("gnc4sj", inAppPrice);
			break;
		case "gk.package.33000r3":
			AdjustManager.Instance.RevenueEvent("6h3dp9", inAppPrice);
			break;
		case "gk.package.33000r4":
			AdjustManager.Instance.RevenueEvent("pws2lz", inAppPrice);
			break;
		case "gk.package.33000r5":
			AdjustManager.Instance.RevenueEvent("7c4u97", inAppPrice);
			break;
		case "gk.package.55000r1":
			AdjustManager.Instance.RevenueEvent("872ua8", inAppPrice);
			break;
		case "gk.package.55000r2":
			AdjustManager.Instance.RevenueEvent("oljq03", inAppPrice);
			break;
		case "gk.package.55000r3":
			AdjustManager.Instance.RevenueEvent("d89w5x", inAppPrice);
			break;
		case "gk.package.55000r4":
			AdjustManager.Instance.RevenueEvent("h0ulqs", inAppPrice);
			break;
		case "gk.package.55000r5":
			AdjustManager.Instance.RevenueEvent("wf4r62", inAppPrice);
			break;
		case "gk.package.99000r1":
			AdjustManager.Instance.RevenueEvent("4gsym7", inAppPrice);
			break;
		case "gk.package.99000r2":
			AdjustManager.Instance.RevenueEvent("2j9n1y", inAppPrice);
			break;
		case "gk.package.99000r3":
			AdjustManager.Instance.RevenueEvent("mssu51", inAppPrice);
			break;
		case "gk.package.99000r4":
			AdjustManager.Instance.RevenueEvent("g9yqmb", inAppPrice);
			break;
		case "gk.package.99000r5":
			AdjustManager.Instance.RevenueEvent("ecxk18", inAppPrice);
			break;
		case "gk.package.110000r1":
			AdjustManager.Instance.RevenueEvent("ew82jt", inAppPrice);
			break;
		case "gk.package.110000r2":
			AdjustManager.Instance.RevenueEvent("24t4d7", inAppPrice);
			break;
		case "gk.package.110000r3":
			AdjustManager.Instance.RevenueEvent("v1mmll", inAppPrice);
			break;
		case "gk.package.110000r4":
			AdjustManager.Instance.RevenueEvent("n82enz", inAppPrice);
			break;
		case "gk.package.110000r5":
			AdjustManager.Instance.RevenueEvent("y641b9", inAppPrice);
			break;
		}
		if (world != null)
		{
			RoBuilding roBuilding = localUser.FindBuilding(EBuilding.VipShop);
			if (roBuilding != null && localUser.vipLevel == roBuilding.reg.vipLevel && !roBuilding.GetUIBuilding().gameObject.activeSelf)
			{
				NetworkAnimation.Instance.CreateFloatingText_OnlyUIToast(Localization.Get("22006"), roBuilding.reg.resourceId);
			}
			RoBuilding roBuilding2 = localUser.FindBuilding(EBuilding.VipGacha);
			if (roBuilding2 != null && localUser.vipLevel == roBuilding2.reg.vipLevel && !roBuilding2.GetUIBuilding().gameObject.activeSelf)
			{
				string spriteName = "Loot_carrier";
				NetworkAnimation.Instance.CreateFloatingText_OnlyUIToast(Localization.Get("7167"), spriteName);
			}
		}
		if (world != null && world.existCarnival && world.carnival.isActive)
		{
			RequestGetCarnivalList(UIManager.instance.world.carnival.categoryType);
		}
		else
		{
			UIManager.instance.RefreshOpenedUI();
		}
		int num = int.Parse(regulation.defineDtbl["VIPGRADE_GACHA_DELAY_FREE"].value);
		int num2 = int.Parse(regulation.defineDtbl["VIPGRADE_GACHA_FREE_PREMIUM"].value);
		if (world != null && world.existGacha && world.gacha.isActive && (localUser.vipLevel >= num || localUser.vipLevel >= num2) && flag)
		{
			RequestGachaInformation();
		}
		int num3 = int.Parse(regulation.defineDtbl["VIPGRADE_BATTLESHOP_REFRESH"].value);
		if (world != null && world.existSecretShop && world.secretShop.isActive && localUser.vipLevel >= num3 && flag)
		{
			UIManager.instance.RefreshOpenedUI();
		}
		if (world != null && world.existWaveBattle && world.waveBattle.isActive && flag)
		{
			RequestWaveBattleList();
		}
		int num4 = int.Parse(regulation.defineDtbl["DAILYMISSION_MIN_VIP"].value);
		if (world != null && world.existWarHome && world.warHome.isActive && localUser.vipLevel >= num4 && flag)
		{
			world.warHome.Close();
		}
		Message.Send("Update.Goods");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8201", true, true)]
	public void BuySecretShopItem(int styp, int id)
	{
	}

	private IEnumerator BuySecretShopItemResult(JsonRpcClient.Request request, Protocols.ShopReward result)
	{
		if (result == null)
		{
			yield break;
		}
		List<Protocols.RewardInfo.RewardData> list = new List<Protocols.RewardInfo.RewardData>();
		for (int i = 0; i < localUser.shopList.Count; i++)
		{
			Protocols.SecretShop.ShopData shopData = localUser.shopList[i];
			if (shopData.id == result.shop.id)
			{
				shopData.sold = result.shop.sold;
			}
		}
		Protocols.RewardInfo.RewardData rewardData = new Protocols.RewardInfo.RewardData();
		rewardData.rewardType = result.shop.type;
		rewardData.rewardId = result.shop.idx.ToString();
		rewardData.rewardCnt = result.shop.count;
		list.Add(rewardData);
		UIPopup.Create<UIGetItem>("GetItem").Set(list, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		localUser.RefreshItemFromNetwork(result.eventResourceData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshGoodsFromNetwork(result.rsoc);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.AddCommanderFromNetwork(result.commanderData);
		localUser.RefreshItemFromNetwork(result.foodData);
		localUser.RefreshUserEquipItemFromNetwork(result.equipItem);
		localUser.RefreshItemFromNetwork(result.groupItemData);
		UIManager.instance.RefreshOpenedUI();
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7301", true, true)]
	public void AnnihilationMapInformation()
	{
	}

	private IEnumerator AnnihilationMapInformationResult(JsonRpcClient.Request request, List<Protocols.ScrambleMapInformationResponse> result)
	{
		if (result != null)
		{
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7306", true, true)]
	public void AnnihilationEnemyInformation(int stage)
	{
	}

	private IEnumerator AnnihilationEnemyInformationResult(JsonRpcClient.Request request, Protocols.ScrambleStageInfo result)
	{
		if (result != null)
		{
			string stageId = _FindRequestProperty(request, "stage");
			RoTroop roTroop = localUser.FindScrambleTroop();
			roTroop.UpdateScrambleTroop(result.myDeck);
			RoUser defender = RoUser.CreateDummyUser(result.enemy.id, result.enemy.nickname, result.enemy.id);
			RoUser attacker = instance.localUser.CreateForBattle(roTroop.id);
			BattleData battleData = BattleData.Create(EBattleType.GuildScramble);
			battleData.defender = defender;
			battleData.attacker = attacker;
			battleData.stageId = stageId;
			UIManager.instance.world.readyBattle.InitAndOpenReadyBattle(battleData);
			if (result.user != null)
			{
				UIManager.instance.world.readyBattle.duel.SetProgressingBattle(result.user);
			}
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3231", true, true)]
	public void StartAnnihilation(int type, int cid, int stage)
	{
	}

	private IEnumerator StartAnnihilationResult(JsonRpcClient.Request request, string result)
	{
		if (result != null)
		{
			Loading.Load(Loading.Battle);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7211", true, true)]
	public void GuildList()
	{
	}

	private IEnumerator GuildListResult(JsonRpcClient.Request request, Protocols.GuildInfo result)
	{
		if (result.guildList != null)
		{
			UIManager.instance.world.guild.InitAndOpenGuildList(result.guildList);
		}
		else if (result.memberData != null && result.guildInfo != null)
		{
			localUser.RefreshGuildFromNetwork(result.guildInfo);
			UIManager.instance.world.guild.InitAndOpenGuildInfo(result.memberData);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7212", true, true)]
	public void CreateGuild(string gnm, int gtyp, int lvlm, int emb)
	{
		tempNickName = localUser.nickname;
	}

	private IEnumerator CreateGuildResult(JsonRpcClient.Request request, Protocols.GuildInfo result)
	{
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshGuildFromNetwork(result.guildInfo);
		if (result.memberData == null)
		{
			localUser.nickname = tempNickName;
			List<Protocols.GuildMember.MemberData> list = new List<Protocols.GuildMember.MemberData>();
			Protocols.GuildMember.MemberData memberData = new Protocols.GuildMember.MemberData();
			memberData.uno = int.Parse(localUser.uno);
			memberData.thumnail = int.Parse(localUser.thumbnailId);
			memberData.name = localUser.nickname;
			memberData.level = localUser.level;
			memberData.lastTime = 10.0;
			memberData.memberGrade = 1;
			memberData.world = localUser.world;
			list.Add(memberData);
			UIManager.instance.world.guild.InitGuildMemberList(list);
		}
		else
		{
			UIManager.instance.world.guild.InitGuildMemberList(result.memberData);
		}
		for (int i = 0; i < localUser.guildSkillList.Count; i++)
		{
			localUser.guildSkillList[i].skillLevel = 0;
		}
		NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("112011"));
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator CreateGuildError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71005:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110021"));
			break;
		case 71009:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110022"));
			break;
		case 71303:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110219"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7218", true, true)]
	public void SearchGuild(string gnm)
	{
	}

	private IEnumerator SearchGuildResult(JsonRpcClient.Request request, RoGuild result)
	{
		if (!string.IsNullOrEmpty(result.gnm))
		{
			List<RoGuild> list = new List<RoGuild>();
			list.Add(result);
			UIManager.instance.world.guild.InitGuildList(list);
			UIManager.instance.world.guild.guildListView.scrollView.SetDragAmount(0f, 0f, updateScrollbars: false);
		}
		else
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110216"));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7253", true, true)]
	public void FreeJoinGuild(int gidx)
	{
	}

	private IEnumerator FreeJoinGuildResult(JsonRpcClient.Request request, Protocols.GuildInfo result)
	{
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshGuildFromNetwork(result.guildInfo);
		UIManager.instance.world.guild.InitGuildMemberList(result.memberData);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator FreeJoinGuildError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71301:
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110303"));
			int gidx = int.Parse(_FindRequestProperty(request, "gidx"));
			UIManager.instance.world.guild.RomoveGuildList(gidx);
			break;
		}
		case 71302:
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110303"));
			int idx = int.Parse(_FindRequestProperty(request, "gidx"));
			UIManager.instance.world.guild.ChangeGuildItemType(idx, 2);
			break;
		}
		case 71303:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110219"));
			break;
		case 71110:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110265"));
			break;
		case 71111:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110266"));
			break;
		case 71112:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110306"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7251", true, true)]
	public void ApplyGuildJoin(int gidx)
	{
	}

	private IEnumerator ApplyGuildJoinResult(JsonRpcClient.Request request, string result)
	{
		int idx = int.Parse(_FindRequestProperty(request, "gidx"));
		UIManager.instance.world.guild.ChangeGuildItemState(idx, "req");
		yield break;
	}

	private IEnumerator ApplyGuildJoinError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71301:
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110303"));
			int gidx = int.Parse(_FindRequestProperty(request, "gidx"));
			UIManager.instance.world.guild.RomoveGuildList(gidx);
			break;
		}
		case 71302:
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110303"));
			int idx = int.Parse(_FindRequestProperty(request, "gidx"));
			UIManager.instance.world.guild.ChangeGuildItemType(idx, 1);
			break;
		}
		case 71303:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110219"));
			break;
		case 71110:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110265"));
			break;
		case 71111:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110266"));
			break;
		case 71112:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110306"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7252", true, true)]
	public void CancelGuildJoin(int gidx)
	{
	}

	private IEnumerator CancelGuildJoinResult(JsonRpcClient.Request request, bool result)
	{
		if (!result)
		{
			UIManager.instance.world.guild.Close();
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110222"));
		}
		else
		{
			int idx = int.Parse(_FindRequestProperty(request, "gidx"));
			UIManager.instance.world.guild.ChangeGuildItemState(idx, "list");
		}
		yield break;
	}

	private IEnumerator CancelGuildJoinError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 71304)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110222"));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7219", true, true)]
	public void GuildMemberList()
	{
	}

	private IEnumerator GuildMemberListResult(JsonRpcClient.Request request, Protocols.GuildMember result)
	{
		if (localUser.IsExistGuild() && result != null)
		{
			UIManager.instance.world.guild.InitAndOpenGuildInfo(result.memberData);
			UISetter.SetActive(UIManager.instance.world.guild.guildBoardBadge, result.badge == 1);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1103", true, true)]
	public void GuildInfo(List<string> type)
	{
	}

	private IEnumerator GuildInfoResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		if (result == null || !localUser.IsExistGuild())
		{
			RequestGuildList();
		}
		else
		{
			localUser.FromNetwork(result);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7217", true, true)]
	public void LeaveGuild()
	{
	}

	private IEnumerator LeaveGuildResult(JsonRpcClient.Request request, string result)
	{
		localUser.guildInfo = null;
		localUser.ResetDispatchPossible();
		for (int i = 0; i < localUser.commanderList.Count; i++)
		{
			RoCommander roCommander = localUser.commanderList[i];
			if (roCommander.scramble || roCommander.occupation)
			{
				roCommander.role = "N";
			}
		}
		UIGuildManagePopup uIGuildManagePopup = UnityEngine.Object.FindObjectOfType(typeof(UIGuildManagePopup)) as UIGuildManagePopup;
		if (uIGuildManagePopup != null)
		{
			uIGuildManagePopup.Close();
		}
		UIManager.instance.world.guild.Close();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7213", true, true)]
	public void UpdateGuildInfo(int act, string val)
	{
	}

	private IEnumerator UpdateGuildInfoResult(JsonRpcClient.Request request, Protocols.GuildInfo result)
	{
		localUser.RefreshGoodsFromNetwork(result.resource);
		UIGuildManagePopup uIGuildManagePopup = UnityEngine.Object.FindObjectOfType(typeof(UIGuildManagePopup)) as UIGuildManagePopup;
		switch (int.Parse(_FindRequestProperty(request, "act")))
		{
		case 0:
			localUser.guildInfo.name = result.guildInfo.name;
			if (uIGuildManagePopup != null)
			{
				uIGuildManagePopup.SetChangeName(localUser.guildInfo.name);
			}
			break;
		case 1:
			localUser.guildInfo.emblem = result.guildInfo.emblem;
			if (uIGuildManagePopup != null)
			{
				uIGuildManagePopup.SetChangeEmblem(localUser.guildInfo.emblem);
			}
			break;
		case 2:
			localUser.guildInfo.limitLevel = result.guildInfo.limitLevel;
			if (uIGuildManagePopup != null)
			{
				uIGuildManagePopup.SetChangeMinLevel(localUser.guildInfo.limitLevel);
			}
			break;
		case 3:
			localUser.guildInfo.guildType = result.guildInfo.guildType;
			if (uIGuildManagePopup != null)
			{
				uIGuildManagePopup.SetChangeType(localUser.guildInfo.guildType);
			}
			break;
		case 4:
			localUser.guildInfo.notice = result.guildInfo.notice;
			break;
		}
		if (uIGuildManagePopup != null)
		{
			uIGuildManagePopup.SetGuildInof();
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator UpdateGuildInfoError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71005:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110021"));
			break;
		case 71009:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110022"));
			break;
		case 71007:
		case 71018:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110303"));
			UIManager.instance.world.guild.Close();
			break;
		default:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), "Error Code:" + code);
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7254", true, true)]
	public void ManageGuildJoinMember()
	{
	}

	private IEnumerator ManageGuildJoinMemberResult(JsonRpcClient.Request request, List<Protocols.GuildMember.MemberData> result)
	{
		UIPopup.Create<UIGuildMemberJoinPopUp>("GuildMemberJoinPopUp").InitAndOpenMemberList(result);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7255", true, true)]
	public void ApproveGuildJoin(int uno)
	{
	}

	private IEnumerator ApproveGuildJoinResult(JsonRpcClient.Request request, string result)
	{
		int uno = int.Parse(_FindRequestProperty(request, "uno"));
		UIGuildMemberJoinPopUp uIGuildMemberJoinPopUp = UnityEngine.Object.FindObjectOfType(typeof(UIGuildMemberJoinPopUp)) as UIGuildMemberJoinPopUp;
		if (uIGuildMemberJoinPopUp != null)
		{
			uIGuildMemberJoinPopUp.AddGildMember(uno);
		}
		yield break;
	}

	private IEnumerator ApproveGuildJoinError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71305:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110221"));
			break;
		case 71306:
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110307"));
			int uno = int.Parse(_FindRequestProperty(request, "uno"));
			UIGuildMemberJoinPopUp uIGuildMemberJoinPopUp2 = UnityEngine.Object.FindObjectOfType(typeof(UIGuildMemberJoinPopUp)) as UIGuildMemberJoinPopUp;
			if (uIGuildMemberJoinPopUp2 != null)
			{
				uIGuildMemberJoinPopUp2.RemoveJoinMember(uno);
			}
			break;
		}
		case 71007:
		case 71018:
		case 71107:
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110303"));
			UIGuildMemberJoinPopUp uIGuildMemberJoinPopUp = UnityEngine.Object.FindObjectOfType(typeof(UIGuildMemberJoinPopUp)) as UIGuildMemberJoinPopUp;
			if (uIGuildMemberJoinPopUp != null)
			{
				uIGuildMemberJoinPopUp.Close();
			}
			UIManager.instance.world.guild.Close();
			break;
		}
		default:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), "Error Code:" + code);
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7256", true, true)]
	public void RefuseGuildJoin(int uno)
	{
	}

	private IEnumerator RefuseGuildJoinResult(JsonRpcClient.Request request, string result)
	{
		int uno = int.Parse(_FindRequestProperty(request, "uno"));
		UIGuildMemberJoinPopUp uIGuildMemberJoinPopUp = UnityEngine.Object.FindObjectOfType(typeof(UIGuildMemberJoinPopUp)) as UIGuildMemberJoinPopUp;
		if (uIGuildMemberJoinPopUp != null)
		{
			uIGuildMemberJoinPopUp.RemoveJoinMember(uno);
		}
		yield break;
	}

	private IEnumerator RefuseGuildJoinError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71305:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110221"));
			break;
		case 71306:
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110307"));
			int uno = int.Parse(_FindRequestProperty(request, "uno"));
			UIGuildMemberJoinPopUp uIGuildMemberJoinPopUp2 = UnityEngine.Object.FindObjectOfType(typeof(UIGuildMemberJoinPopUp)) as UIGuildMemberJoinPopUp;
			if (uIGuildMemberJoinPopUp2 != null)
			{
				uIGuildMemberJoinPopUp2.RemoveJoinMember(uno);
			}
			break;
		}
		case 71007:
		case 71018:
		case 71107:
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110303"));
			UIGuildMemberJoinPopUp uIGuildMemberJoinPopUp = UnityEngine.Object.FindObjectOfType(typeof(UIGuildMemberJoinPopUp)) as UIGuildMemberJoinPopUp;
			if (uIGuildMemberJoinPopUp != null)
			{
				uIGuildMemberJoinPopUp.Close();
			}
			UIManager.instance.world.guild.Close();
			break;
		}
		default:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), "Error Code:" + code);
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7214", true, true)]
	public void GuildCloseDown()
	{
	}

	private IEnumerator GuildCloseDownResult(JsonRpcClient.Request request, string result, double ctime)
	{
		localUser.guildInfo.closeTime = ctime;
		localUser.guildInfo.state = 1;
		localUser.guildInfo.memberGrade = 0;
		UIGuildManagePopup uIGuildManagePopup = UnityEngine.Object.FindObjectOfType(typeof(UIGuildManagePopup)) as UIGuildManagePopup;
		if (uIGuildManagePopup != null)
		{
			uIGuildManagePopup.Close();
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7215", true, true)]
	public void DelegatingGuild(int tuno)
	{
	}

	private IEnumerator DelegatingGuildResult(JsonRpcClient.Request request, string result)
	{
		int uno = int.Parse(_FindRequestProperty(request, "tuno"));
		localUser.guildInfo.memberGrade = 0;
		UIManager.instance.world.guild.DelegationGildMaster(uno);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator DelegatingGuildError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 71001)
		{
			int uno = int.Parse(_FindRequestProperty(request, "tuno"));
			UIManager.instance.world.guild.RemoveMemberList(uno);
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110228"));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7216", true, true)]
	public void DeportGuildMember(int tuno)
	{
	}

	private IEnumerator DeportGuildMemberResult(JsonRpcClient.Request request, string result)
	{
		int uno = int.Parse(_FindRequestProperty(request, "tuno"));
		UIManager.instance.world.guild.RemoveMemberList(uno);
		yield break;
	}

	private IEnumerator DeportGuildMemberError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
		{
			int uno = int.Parse(_FindRequestProperty(request, "tuno"));
			UIManager.instance.world.guild.RemoveMemberList(uno);
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110228"));
			break;
		}
		case 71307:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110118"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7257", true, true)]
	public void AppointSubMaster(int tuno)
	{
	}

	private IEnumerator AppointSubMasterResult(JsonRpcClient.Request request, string result)
	{
		int uno = int.Parse(_FindRequestProperty(request, "tuno"));
		UIManager.instance.world.guild.AppointSubMaster(bAppoint: true, uno);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator AppointSubMasterError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
		{
			int uno = int.Parse(_FindRequestProperty(request, "tuno"));
			UIManager.instance.world.guild.RemoveMemberList(uno);
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110228"));
			break;
		}
		case 71019:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Format("110114", regulation.defineDtbl["GUILD_MAX_AIDE"].value));
			break;
		default:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), "Error Code:" + code);
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7258", true, true)]
	public void FireSubMaster(int tuno)
	{
	}

	private IEnumerator FireSubMasterResult(JsonRpcClient.Request request, string result)
	{
		int uno = int.Parse(_FindRequestProperty(request, "tuno"));
		UIManager.instance.world.guild.AppointSubMaster(bAppoint: false, uno);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator FireSubMasterError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 71001)
		{
			int uno = int.Parse(_FindRequestProperty(request, "tuno"));
			UIManager.instance.world.guild.RemoveMemberList(uno);
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110228"));
		}
		else
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), "Error Code:" + code);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7220", true, true)]
	public void UpgradeGuildSkill(int gsid)
	{
	}

	private IEnumerator UpgradeGuildSkillResult(JsonRpcClient.Request request, Protocols.GuildInfo result)
	{
		localUser.guildInfo.point = result.guildInfo.point;
		foreach (Protocols.UserInformationResponse.UserGuild.GuildSkill list in result.guildInfo.skillDada)
		{
			localUser.guildSkillList.Find((RoGuildSkill skill) => skill.idx == list.idx).skillLevel = list.level;
		}
		UIGuildManagePopup uIGuildManagePopup = UnityEngine.Object.FindObjectOfType(typeof(UIGuildManagePopup)) as UIGuildManagePopup;
		if (uIGuildManagePopup != null)
		{
			uIGuildManagePopup.OnRefresh();
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator UpgradeGuildSkillError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 71014)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110259"));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7222", true, true)]
	public void UpgradeGuildLevel()
	{
	}

	private IEnumerator UpgradeGuildLevelResult(JsonRpcClient.Request request, Protocols.GuildInfo result)
	{
		localUser.guildInfo.point = result.guildInfo.point;
		localUser.guildInfo.level = result.guildInfo.level;
		localUser.guildInfo.maxCount = result.guildInfo.maxCount;
		UIGuildManagePopup uIGuildManagePopup = UnityEngine.Object.FindObjectOfType(typeof(UIGuildManagePopup)) as UIGuildManagePopup;
		if (uIGuildManagePopup != null)
		{
			uIGuildManagePopup.OnRefresh();
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7450", true, true)]
	public void GetGuildBoard(int page)
	{
	}

	private IEnumerator GetGuildBoardResult(JsonRpcClient.Request request, string result, int tPage, int page, List<Protocols.GuildBoardData> list)
	{
		UIManager.instance.world.guild.OpenGuildBoard(page, tPage, list);
		yield break;
	}

	private IEnumerator GetGuildBoardError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 71001)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			UIManager.instance.world.guild.Close();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7451", true, true)]
	public void GuildBoardWrite(string msg)
	{
	}

	private IEnumerator GuildBoardWriteResult(JsonRpcClient.Request request, string result)
	{
		yield break;
	}

	private IEnumerator GuildBoardWriteError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71131:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7054"));
			break;
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			UIManager.instance.world.guild.Close();
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7452", true, true)]
	public void GuildBoardDelete(int idx)
	{
	}

	private IEnumerator GuildBoardDeleteResult(JsonRpcClient.Request request, string result)
	{
		yield break;
	}

	private IEnumerator GuildBoardDeleteError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 71001 || code == 71007 || code == 71018)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			UIManager.instance.world.guild.Close();
		}
		else
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), "Error Code:" + code);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7501", true, true)]
	public void ConquestJoin()
	{
	}

	private IEnumerator ConquestJoinResult(JsonRpcClient.Request request, string result)
	{
		NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110310"));
		UIManager.instance.world.guild.conquestInfo.sign = 1;
		UIManager.instance.world.guild.SetConquestStateLabel();
		yield break;
	}

	private IEnumerator ConquestJoinError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			break;
		case 71501:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71502:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71007:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			break;
		case 71511:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			break;
		}
		UIManager.instance.world.guild.Close();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7502", true, true)]
	public void GetConquestInfo()
	{
	}

	private IEnumerator GetConquestInfoResult(JsonRpcClient.Request request, Protocols.ConquestInfo result)
	{
		if (localUser.IsExistGuild() && result != null)
		{
			UIManager.instance.world.guild.SetConquestState(result);
		}
		yield break;
	}

	private IEnumerator GetConquestInfoError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 71501)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110365"));
			UIManager.instance.world.guild.SetConquestError();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7503", true, true)]
	public void SetConquestTroop(int slot, JObject deck)
	{
	}

	private IEnumerator SetConquestTroopResult(JsonRpcClient.Request request, string result)
	{
		if (!result.Equals("True"))
		{
			yield break;
		}
		Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(_FindRequestProperty(request, "deck"));
		string s = _FindRequestProperty(request, "slot");
		for (int i = 0; i < localUser.commanderList.Count; i++)
		{
			RoCommander roCommander = localUser.commanderList[i];
			if (roCommander.conquestDeckId == int.Parse(s))
			{
				roCommander.conquestDeckId = 0;
			}
		}
		foreach (KeyValuePair<string, string> item in dictionary)
		{
			RoCommander roCommander2 = localUser.FindCommander(item.Value);
			roCommander2.conquestDeckId = int.Parse(s);
		}
		localUser.RefreshConquestTroop(int.Parse(s), dictionary);
		NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110323"));
		UIManager.instance.world.readyBattle.CloseAnimation();
		UIManager.instance.RefreshOpenedUI();
	}

	private IEnumerator SetConquestTroopError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			UIManager.instance.world.guild.Close();
			break;
		case 71501:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71502:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71507:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71503:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			break;
		case 71504:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			break;
		}
		UIManager.instance.world.guild.Close();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7505", true, true)]
	public void DeleteConquestTroop(int slot)
	{
	}

	private IEnumerator DeleteConquestTroopResult(JsonRpcClient.Request request, string result)
	{
		if (!result.Equals("True"))
		{
			yield break;
		}
		int num = int.Parse(_FindRequestProperty(request, "slot"));
		for (int i = 0; i < localUser.commanderList.Count; i++)
		{
			RoCommander roCommander = localUser.commanderList[i];
			if (roCommander.conquestDeckId == num)
			{
				roCommander.conquestDeckId = 0;
			}
		}
		localUser.conquestDeck[num] = null;
		UIManager.instance.RefreshOpenedUI();
	}

	private IEnumerator DeleteConquestTroopError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			break;
		case 71501:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71502:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71507:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71503:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			break;
		}
		UIManager.instance.world.guild.Close();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7504", true, true)]
	public void GetConquestTroop()
	{
	}

	private IEnumerator GetConquestTroopResult(JsonRpcClient.Request request, Protocols.ConquestTroopInfo result)
	{
		if (result != null && UIManager.instance.world.guild.isActive)
		{
			localUser.ResetConquestSlot();
			UIManager.instance.world.conquestMap.InitAndOpenConquestMap();
			UIManager.instance.world.conquestMap._Set(result);
		}
		yield break;
	}

	private IEnumerator GetConquestTroopError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			break;
		case 71501:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71502:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71507:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		}
		UIManager.instance.world.guild.Close();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7506", true, true)]
	public void BuyConquestTroopSlot(int slot)
	{
	}

	private IEnumerator BuyConquestTroopSlotResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.Resource rsoc)
	{
		if (result != null)
		{
			string s = _FindRequestProperty(request, "slot");
			localUser.RefreshGoodsFromNetwork(rsoc);
			localUser.conquestDeckSlotState.Add(int.Parse(s));
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator BuyConquestTroopSlotError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			break;
		case 71501:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71502:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71507:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71505:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			break;
		case 20002:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			break;
		}
		UIManager.instance.world.guild.Close();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7510", true, true)]
	public void GetConquestStageInfo(int point)
	{
	}

	private IEnumerator GetConquestStageInfoResult(JsonRpcClient.Request request, Protocols.ConquestStageInfo result)
	{
		if (result != null)
		{
			UIConquestMap conquestMap = UIManager.instance.world.conquestMap;
			if (conquestMap.isActive)
			{
				conquestMap.CreateStageInfoPopup(result);
			}
		}
		yield break;
	}

	private IEnumerator GetConquestStageInfoError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			break;
		case 71501:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71502:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		}
		UIManager.instance.world.guild.Close();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7507", true, true)]
	public void GetConquestMovePath(int dest, int slot)
	{
	}

	private IEnumerator GetConquestMovePathResult(JsonRpcClient.Request request, string result, List<int> path, int distance)
	{
		if (!string.IsNullOrEmpty(result) && UIManager.instance.world.conquestMap.isActive && UIManager.instance.world.conquestMap.stagePopup != null)
		{
			string s = _FindRequestProperty(request, "dest");
			string s2 = _FindRequestProperty(request, "slot");
			UIManager.instance.world.conquestMap.stagePopup.OnOpenMoveInfoPopup(int.Parse(s), int.Parse(s2), distance);
		}
		yield break;
	}

	private IEnumerator GetConquestMovePathError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			break;
		case 71501:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71502:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71507:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71506:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			break;
		case 71508:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			break;
		}
		UIManager.instance.world.guild.Close();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7508", true, true)]
	public void SetConquestMoveTroop(int dest, int slot, int ucash)
	{
	}

	private IEnumerator SetConquestMoveTroopResult(JsonRpcClient.Request request, Protocols.MoveConquestTroop result)
	{
		if (result != null)
		{
			string s = _FindRequestProperty(request, "slot");
			Protocols.ConquestTroopInfo.Troop troop = localUser.conquestDeck[int.Parse(s)];
			troop.point = result.path[result.path.Count - 1];
			troop.path = result.path;
			troop.status = "M";
			troop.remain = result.distance;
			troop.ucash = result.ucash;
			troop.mvtm = result.distance;
			if (troop.remainData == null)
			{
				troop.remainData = new TimeData();
			}
			troop.remainData.SetByDuration(troop.remain);
			if (result.rsoc != null)
			{
				localUser.RefreshGoodsFromNetwork(result.rsoc);
			}
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator SetConquestMoveTroopError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			break;
		case 71501:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71502:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71507:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71506:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			break;
		case 71508:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			break;
		case 20002:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			ReqeustRenewUserGameData();
			break;
		}
		UIManager.instance.world.guild.Close();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7509", true, true)]
	public void GetConquestCurrentStateInfo()
	{
	}

	private IEnumerator GetConquestCurrentStateInfoResult(JsonRpcClient.Request request, List<Protocols.ConquestStageInfo.User> result)
	{
		if (UIManager.instance.world.conquestMap.isActive)
		{
			UIManager.instance.world.conquestMap.CreateConquestCurrentPopup(result);
		}
		yield break;
	}

	private IEnumerator GetConquestCurrentStateInfoError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			break;
		case 71501:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71502:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		}
		UIManager.instance.world.guild.Close();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7511", true, true)]
	public void StartConquestRadar()
	{
	}

	private IEnumerator StartConquestRadarResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.Resource rsoc, Protocols.GetRadarData.Radar radar)
	{
		if (!string.IsNullOrEmpty(result))
		{
			localUser.RefreshGoodsFromNetwork(rsoc);
			UIConquestMap conquestMap = UIManager.instance.world.conquestMap;
			if (conquestMap.isActive)
			{
				radar.overTime = 1;
				conquestMap.StartRadar(radar);
			}
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator StartConquestRadarError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			UIManager.instance.world.guild.Close();
			break;
		case 71501:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			UIManager.instance.world.guild.Close();
			break;
		case 71502:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			UIManager.instance.world.guild.Close();
			break;
		case 71509:
		{
			UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			if (uISimplePopup != null)
			{
				uISimplePopup.onClose = delegate
				{
					RequestGetConquestRadar();
				};
			}
			break;
		}
		case 20002:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			ReqeustRenewUserGameData();
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7512", true, true)]
	public void GetConquestRadar()
	{
	}

	private IEnumerator GetConquestRadarResult(JsonRpcClient.Request request, Protocols.GetRadarData result)
	{
		if (result != null)
		{
			UIConquestMap conquestMap = UIManager.instance.world.conquestMap;
			if (conquestMap.isActive)
			{
				conquestMap.SetRadar();
				conquestMap.StartRadar(result.radar);
			}
		}
		yield break;
	}

	private IEnumerator GetConquestRadarError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			break;
		case 71501:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71502:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		}
		UIManager.instance.world.guild.Close();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7513", true, true)]
	public void GetConquestStageUserInfo(int tuno, int point)
	{
	}

	private IEnumerator GetConquestStageUserInfoResult(JsonRpcClient.Request request, List<Protocols.ConquestStageUser> result)
	{
		if (result.Count > 0 && UIManager.instance.world.conquestMap.isActive && UIManager.instance.world.conquestMap.stagePopup != null)
		{
			UIManager.instance.world.conquestMap.stagePopup.CreateAlieUserDeckPopup(result);
		}
		yield break;
	}

	private IEnumerator GetConquestStageUserInfoError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			break;
		case 71501:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71502:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		}
		UIManager.instance.world.guild.Close();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7514", true, true)]
	public void GetConquestBattle(int point, int skip)
	{
	}

	private IEnumerator GetConquestBattleResult(JsonRpcClient.Request request, Protocols.GetConquestBattle result)
	{
		if (result != null && UIManager.instance.world.guild.historyPopup != null)
		{
			int skip = int.Parse(_FindRequestProperty(request, "skip"));
			UIManager.instance.world.guild.historyPopup.CreateBattleResultPopup(result, skip);
		}
		yield break;
	}

	private IEnumerator GetConquestBattleError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			break;
		case 71501:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		case 71502:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			break;
		}
		UIManager.instance.world.guild.Close();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7515", true, true)]
	public void GetGuildRanking(int type)
	{
	}

	private IEnumerator GetGuildRankingResult(JsonRpcClient.Request request, List<Protocols.GuildRankingInfo> result)
	{
		int type = int.Parse(_FindRequestProperty(request, "type"));
		if (UIManager.instance.world.guild.isActive)
		{
			UIManager.instance.world.guild.CreateConquestHistoryPopup(type, result);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7516", true, true)]
	public void GetConquestNotice(int check)
	{
	}

	private IEnumerator GetConquestNoticeResult(JsonRpcClient.Request request, string result, string notice)
	{
		int num = int.Parse(_FindRequestProperty(request, "check"));
		if (num != 1 || !string.IsNullOrEmpty(notice))
		{
			if (localUser.guildInfo.memberGrade != 0)
			{
				UIInputConquestNotice uIInputConquestNotice = UIPopup.Create<UIInputConquestNotice>("InputConquestNotice");
				uIInputConquestNotice.SetDefault(notice);
			}
			else
			{
				UIConquestNotice uIConquestNotice = UIPopup.Create<UIConquestNotice>("ConquestNotice");
				uIConquestNotice.Init(notice);
			}
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7517", true, true)]
	public void SetConquestNotice(string notice)
	{
	}

	private IEnumerator SetConquestNoticeResult(JsonRpcClient.Request request, string result, string notice)
	{
		NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110377"));
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7518", true, true)]
	public void GetConquestReplay(string rid)
	{
	}

	private IEnumerator GetConquestReplayResult(JsonRpcClient.Request request, object result)
	{
		if (result == null)
		{
			if (localUser.playingChatRecord != null)
			{
				localUser.playingChatRecord.hasRecord = false;
				localUser.playingChatRecord = null;
			}
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19079"));
			yield break;
		}
		localUser.playingChatRecord = null;
		_ = Regulation.SerializerSettings;
		Record record = (Record)JsonConvert.DeserializeObject<JToken>(result.ToString());
		BattleData battleData = BattleData.Create(EBattleType.Conquest);
		battleData.attacker = localUser.CreateForBattle(new List<RoTroop> { null });
		battleData.defender = RoUser.Create();
		battleData.isReplayMode = true;
		battleData.record = record;
		battleData.move = EBattleResultMove.Conquest;
		battleData.attacker.nickname = record.initState.dualData._playerName;
		battleData.attacker.level = record.initState.dualData._playerLevel;
		battleData.attacker.guildName = record.initState.dualData._playerGuildName;
		battleData.defender.nickname = record.initState.dualData._enemyName;
		battleData.defender.level = record.initState.dualData._enemyLevel;
		battleData.defender.duelRanking = record.initState.dualData._enemyRank;
		battleData.defender.guildName = record.initState.dualData._enemyGuildName;
		battleData.defender.uno = record.initState.dualData._enemyUno;
		BattleData.Set(battleData);
		Loading.Load(Loading.Battle);
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4307", true, true)]
	public void GiftFood(int cid, int cgid, int amnt)
	{
	}

	private IEnumerator GiftFoodResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		if (result.commanderInfo != null)
		{
			foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> item in result.commanderInfo)
			{
				_ = item.Value;
				RoCommander roCommander = localUser.FindCommander(item.Value.id);
				roCommander.favorStep = item.Value.favorStep;
				roCommander.favorPoint = item.Value.favorPoint;
			}
		}
		localUser.RefreshGoodsFromNetwork(result.goodsInfo);
		localUser.RefreshItemFromNetwork(result.foodData);
		UIManager.instance.RefreshOpenedUI();
		localUser.sendingGift = false;
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4308", true, true)]
	public void GetFavorReward(int cid, int step)
	{
	}

	private IEnumerator GetFavorRewardResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		RoCommander roCommander = localUser.FindCommander(_FindRequestProperty(request, "cid"));
		roCommander.favorRewardStep = int.Parse(_FindRequestProperty(request, "step"));
		if (result.commander == null)
		{
			UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
			SoundManager.PlaySFX("SE_ItemGet_001");
		}
		localUser.RefreshRewardFromNetwork(result);
		UIManager.instance.RefreshOpenedUI();
		if (regulation.FindCostumeData(int.Parse(localUser.thumbnailId)).cid == int.Parse(_FindRequestProperty(request, "cid")))
		{
			UIManager.instance.world.mainCommand.spineTest.SetInteraction(roCommander);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7401", true, true)]
	public void SellItem(int ityp, int tidx, int amnt)
	{
	}

	private IEnumerator SellItemResult(JsonRpcClient.Request request, Protocols.SellItemData result)
	{
		_ = instance.regulation;
		int.Parse(_FindRequestProperty(request, "ityp"));
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshItemFromNetwork(result.foodData);
		localUser.RefreshItemFromNetwork(result.eventResourceData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		NetworkAnimation.Instance.CreateFloatingText(Localization.Get("1315"));
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7402", true, true)]
	public void OpenItem(int ityp, int tidx, int amnt, int rtyp, int ridx)
	{
	}

	private IEnumerator OpenItemResult(JsonRpcClient.Request request, Protocols.SellItemData result)
	{
		_ = instance.regulation;
		int.Parse(_FindRequestProperty(request, "ityp"));
		if (result.rewardList != null)
		{
			UIPopup.Create<UIGetItem>("GetItem").Set(result.rewardList, string.Empty);
			SoundManager.PlaySFX("SE_ItemGet_001");
		}
		ERewardType eRewardType = (ERewardType)int.Parse(_FindRequestProperty(request, "rtyp"));
		string id = _FindRequestProperty(request, "ridx");
		if (eRewardType == ERewardType.Commander)
		{
			RoCommander roCommander = localUser.FindCommander(id);
			if (roCommander != null)
			{
				UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				if (uICommanderComplete != null)
				{
					if (roCommander.state != ECommanderState.Nomal)
					{
						uICommanderComplete.Init(CommanderCompleteType.Recruit, roCommander.id);
					}
					else
					{
						uICommanderComplete.Init(CommanderCompleteType.Transmission, roCommander.id);
					}
				}
			}
			if (result.commanderData != null)
			{
				foreach (Protocols.UserInformationResponse.Commander value in result.commanderData.Values)
				{
					RoCommander roCommander2 = localUser.FindCommander(value.id);
					if (value.haveCostume != null && value.haveCostume.Count > 0)
					{
						roCommander2.haveCostumeList = value.haveCostume;
					}
				}
			}
		}
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshItemFromNetwork(result.foodData);
		localUser.RefreshItemFromNetwork(result.eventResourceData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.AddCommanderFromNetwork(result.commanderData);
		localUser.RefreshUserEquipItemFromNetwork(result.equipItem);
		localUser.RefreshItemFromNetwork(result.groupItemData);
		localUser.RefreshWeaponFromNetwork(result.weaponData);
		localUser.RefreshGoodsFromNetwork(result.dormitoryResource);
		localUser.RefreshDormitoryItemNormalFromNetwork(result.dormitoryItemNormal);
		localUser.RefreshDormitoryItemAdvancedFromNetwork(result.dormitoryItemAdvanced);
		localUser.RefreshDormitoryItemWallpaperFromNetwork(result.dormitoryItemWallpaper);
		localUser.RefreshDormitoryCostumeBodyFromNetwork(result.dormitoryCostumeBody);
		localUser.RefreshDormitoryCostumeHeadFromNetwork(result.dormitoryCostumeHead);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "5220", false, true)]
	public void PreDeckSetting(JObject send)
	{
	}

	private IEnumerator PreDeckSettingResult(JsonRpcClient.Request request, string result)
	{
		List<Protocols.UserInformationResponse.PreDeck> list = JsonConvert.DeserializeObject<List<Protocols.UserInformationResponse.PreDeck>>(_FindRequestProperty(request, "list"));
		foreach (Protocols.UserInformationResponse.PreDeck deck in list)
		{
			Protocols.UserInformationResponse.PreDeck preDeck = localUser.preDeckList.Find((Protocols.UserInformationResponse.PreDeck row) => row.idx == deck.idx);
			if (preDeck != null)
			{
				preDeck.name = deck.name;
				preDeck.deckData = deck.deckData;
			}
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "5221", true, true)]
	public void ExchangeMedal(int amnt, int cid)
	{
	}

	private IEnumerator ExchangeMedalResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		localUser.FromNetwork(result);
		UIManager.instance.RefreshOpenedUI();
		UITranscendencePopup uITranscendencePopup = UnityEngine.Object.FindObjectOfType(typeof(UITranscendencePopup)) as UITranscendencePopup;
		if (uITranscendencePopup != null)
		{
			uITranscendencePopup.Set();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4306", true, true)]
	public void BuyCommanderCostume(int cid, int cos)
	{
	}

	private IEnumerator BuyCommanderCostumeResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		string text = _FindRequestProperty(request, "cid");
		int num = int.Parse(_FindRequestProperty(request, "cos"));
		RoCommander roCommander = localUser.FindCommander(text);
		if (!roCommander.haveCostumeList.Contains(num))
		{
			roCommander.haveCostumeList.Add(num);
		}
		if (roCommander.state == ECommanderState.Undefined)
		{
			localUser.AddDonHaveCommCostume(text, num);
		}
		localUser.FromNetwork(result);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4305", true, true)]
	public void ChangeCommanderCostume(int cid, int cos)
	{
	}

	private IEnumerator ChangeCommanderCostumeResult(JsonRpcClient.Request request, string result)
	{
		string s = _FindRequestProperty(request, "cid");
		string thumbnailId = _FindRequestProperty(request, "cos");
		if (regulation.FindCostumeData(int.Parse(localUser.thumbnailId)).cid == int.Parse(s))
		{
			localUser.thumbnailId = thumbnailId;
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1229", true, true)]
	public void ChangeLanguage(string lang)
	{
	}

	private IEnumerator ChangeLanguageResult(JsonRpcClient.Request request, string result)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8110", true, true)]
	public void GetCommonNotice()
	{
	}

	private IEnumerator GetCommonNoticeResult(JsonRpcClient.Request request, List<Protocols.NoticeData> result)
	{
		if (result != null)
		{
			for (int i = 0; i < result.Count; i++)
			{
				localUser.eventNoticeList.Add(result[i]);
			}
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8111", true, true)]
	public void GetEventNotice()
	{
	}

	private IEnumerator GetEventNoticeResult(JsonRpcClient.Request request, List<Protocols.NoticeData> result)
	{
		if (result != null)
		{
			for (int i = 0; i < result.Count; i++)
			{
				localUser.eventNoticeList.Add(result[i]);
			}
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8114", true, true)]
	public void GetShutDownNotice()
	{
	}

	private IEnumerator GetShutDownNoticeResult(JsonRpcClient.Request request, List<Protocols.NoticeData> result)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8115", true, true)]
	public void GameVersionInfo(string ver, int oscd, int ch)
	{
	}

	private IEnumerator GameVersionInfoResult(JsonRpcClient.Request request, string ver, int stat, string cdn, string game, string chat, double policy, Dictionary<string, double> word, int fc, int gglogin)
	{
		localUser.gameVersionState = stat;
		PatchManager.Instance.SERVER_URL = Base64Decode(cdn);
		GameServerUrl = Base64Decode(game);
		ChattingServerUrl = Base64Decode(chat);
		if (policy > 0.0 && policy > double.Parse(PlayerPrefs.GetString("UserTermVersion", "0")))
		{
			localUser.userTermVersion = policy;
			localUser.bShowUserTerm = true;
		}
		localUser.badWordVersions = word;
		localUser.bDownLoadFileCheck = Convert.ToBoolean(fc);
		localUser.bEnableGoogleAccount = Convert.ToBoolean(gglogin);
		StartCoroutine(RequestDBVersionCheck());
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3135", true, true)]
	public void GetAnnihilationMapInfo(int goReady)
	{
	}

	private IEnumerator GetAnnihilationMapInfoResult(JsonRpcClient.Request request, Protocols.AnnihilationMapInfo result)
	{
		string s = _FindRequestProperty(request, "goReady");
		localUser.lastClearAnnihilationStage = result.stage;
		localUser.CommanderStatusReset();
		if (result.dieCommanderList != null)
		{
			for (int i = 0; i < result.dieCommanderList.Count; i++)
			{
				localUser.FindCommander(result.dieCommanderList[i])?.Die();
			}
		}
		if (result.commanderStatusList != null)
		{
			for (int j = 0; j < result.commanderStatusList.Count; j++)
			{
				Protocols.AnnihilationMapInfo.StatusData statusData = result.commanderStatusList[j];
				RoCommander roCommander = localUser.FindCommander(statusData.id);
				if (roCommander != null && !roCommander.isDie)
				{
					roCommander.sp = statusData.sp;
					roCommander.dmgHp = statusData.dmghp;
				}
			}
		}
		UIAnnihilationMap annihilationMap = UIManager.instance.world.annihilationMap;
		annihilationMap.isPlay = result.dieCommanderList.Count > 0;
		annihilationMap.isPlayAdvanceParty = ((result.isPlayAdvanceParty != 0) ? true : false);
		annihilationMap.SetEnemy(result.enemyList);
		annihilationMap.SetTime(result.remainTime, result.clear);
		annihilationMap.SetMode(result.mode);
		annihilationMap.InitAndOpenAnnihilationMap(int.Parse(s) == 1);
		yield break;
	}

	private IEnumerator GetAnnihilationMapInfoError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 70107)
		{
			UIPopup.Create<UISelectAnnihilationModePopup>("ModeSelectPopup").InitAndOpen();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3236", true, true)]
	public void DispatchAdvancedParty(List<string> cids)
	{
	}

	private IEnumerator DispatchAdvancedPartyResult(JsonRpcClient.Request request, Protocols.AnnihilationMapInfo result)
	{
		localUser.lastClearAnnihilationStage = result.stage;
		List<string> list = JsonConvert.DeserializeObject<List<string>>(_FindRequestProperty(request, "cids"));
		AnnihilateBattleDataRow annihilateBattleDataRow = regulation.annihilateBattleDtbl[(localUser.lastClearAnnihilationStage - 1).ToString()];
		for (int i = 0; i < list.Count; i++)
		{
			RoCommander roCommander = localUser.FindCommander(list[i]);
			if (roCommander != null)
			{
				roCommander.Die();
				roCommander.SetSp(annihilateBattleDataRow.sp);
			}
		}
		if (result.commanderStatusList != null)
		{
			for (int j = 0; j < result.commanderStatusList.Count; j++)
			{
				Protocols.AnnihilationMapInfo.StatusData statusData = result.commanderStatusList[j];
				RoCommander roCommander2 = localUser.FindCommander(statusData.id);
				if (roCommander2 != null)
				{
					roCommander2.sp = statusData.sp;
					roCommander2.dmgHp = statusData.dmghp;
				}
			}
		}
		if (result.advancePartyReward != null)
		{
			localUser.RefreshGoodsFromNetwork(result.advancePartyReward.resource);
			localUser.RefreshItemFromNetwork(result.advancePartyReward.eventResourceData);
			localUser.RefreshItemFromNetwork(result.advancePartyReward.itemData);
			localUser.RefreshMedalFromNetwork(result.advancePartyReward.medalData);
			localUser.RefreshPartFromNetwork(result.advancePartyReward.partData);
			UIManager.instance.RefreshOpenedUI();
			UIPopup.Create<UITimeMachinePopup>("TimeMachinePopup").Init(result.advancePartyReward.rewardList, ETimeMachineType.AdvanceParty);
		}
		UIAnnihilationMap annihilationMap = UIManager.instance.world.annihilationMap;
		annihilationMap.isPlay = true;
		annihilationMap.isPlayAdvanceParty = ((result.isPlayAdvanceParty != 0) ? true : false);
		annihilationMap.SetEnemy(result.enemyList);
		annihilationMap.ButtonControll();
		annihilationMap.StageOpenAnimation();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3231", true, true)]
	public void AnnihilationStageStart(int type, JObject deck, JObject gdp, int ucash, int mst)
	{
	}

	private IEnumerator AnnihilationStageStartResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		if (result != null)
		{
			localUser.RefreshGoodsFromNetwork(result.goodsInfo);
		}
		Loading.Load(Loading.Battle);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3237", true, true)]
	public void ResetAnnihilationStage(AnnihilationMode mode)
	{
	}

	private IEnumerator ResetAnnihilationStageResult(JsonRpcClient.Request request, Protocols.AnnihilationMapInfo result)
	{
		localUser.ResetAdvancePossible();
		UIAnnihilationMap annihilationMap = UIManager.instance.world.annihilationMap;
		annihilationMap.isPlay = false;
		annihilationMap.isPlayAdvanceParty = false;
		annihilationMap.isReset = true;
		if (PlayerPrefs.HasKey("MercenaryDeck"))
		{
			PlayerPrefs.DeleteKey("MercenaryDeck");
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8203", true, true)]
	public void GetBuyVipShop()
	{
	}

	private IEnumerator GetBuyVipShopResult(JsonRpcClient.Request request, Protocols.BuyVipShop result)
	{
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.FromNetwork(result.userInfo);
		if (result.userInfo.vipShop == 1 && result.userInfo.vipShopResetTime == 0)
		{
			localUser.statistics.isBuyVipShop = true;
		}
		CancelLocalPush(ELocalPushType.LeaveVipShop);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6314", true, true)]
	public void GetVipGachaInfo()
	{
	}

	private IEnumerator GetVipGachaInfoResult(JsonRpcClient.Request request, Protocols.VipGacha result)
	{
		localUser.gachaInfoList.Clear();
		foreach (KeyValuePair<string, Protocols.VipGacha.VipGachaInfo> vipGachaInfo in result.VipGachaInfoList)
		{
			localUser.gachaInfoList.Add(vipGachaInfo.Value);
		}
		localUser.vipGachaCount = result.gachaCount;
		localUser.vipGachaRefreshTime.SetByDuration(result.refreshTime);
		UIVipGachaContents vipGachaContents = UIManager.instance.world.vipGacha.vipGachaContents;
		if (vipGachaContents != null)
		{
			vipGachaContents.Init(localUser.gachaInfoList);
			vipGachaContents.RegisterEndPopup();
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6313", true, true)]
	public void BuyVipGacah()
	{
	}

	private IEnumerator BuyVipGacahResult(JsonRpcClient.Request request, Protocols.VipGacha result)
	{
		List<Protocols.VipGacha.VipGachaResult> gacharesult = result.gacharesult;
		Protocols.RewardInfo.RewardData rewardData = new Protocols.RewardInfo.RewardData();
		List<Protocols.RewardInfo.RewardData> list = new List<Protocols.RewardInfo.RewardData>();
		ERewardType eRewardType = ERewardType.Undefined;
		int num = -1;
		for (int i = 0; i < localUser.gachaInfoList.Count; i++)
		{
			if (localUser.gachaInfoList[i].rewardType == gacharesult[0].rewardType_result && localUser.gachaInfoList[i].rewardIdx == gacharesult[0].rewardIdx_result)
			{
				num = localUser.gachaInfoList[i].rewardIdx;
				localUser.gachaInfoList[i].rewardRate--;
				eRewardType = (ERewardType)gacharesult[0].rewardType_result;
				rewardData.rewardType = (ERewardType)gacharesult[0].rewardType_result;
				rewardData.rewardCnt = gacharesult[0].rewardCount_result;
				rewardData.rewardId = gacharesult[0].rewardIdx_result.ToString();
				list.Add(rewardData);
			}
		}
		if (eRewardType == ERewardType.Commander)
		{
			RoCommander roCommander = instance.localUser.FindCommander(num.ToString());
			if (roCommander != null)
			{
				UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				if (uICommanderComplete != null)
				{
					if (roCommander.state != ECommanderState.Nomal)
					{
						uICommanderComplete.Init(CommanderCompleteType.Recruit, roCommander.id);
					}
					else
					{
						uICommanderComplete.Init(CommanderCompleteType.Transmission, roCommander.id);
					}
				}
				UIVipGachaContents vipGachaContents = UIManager.instance.world.vipGacha.vipGachaContents;
				if (vipGachaContents != null)
				{
					vipGachaContents.UnRegisterEndPopup();
				}
				instance.RequestVipGachaInfo();
			}
		}
		else if (list != null)
		{
			UIPopup.Create<UIGetItem>("GetItem").Set(list, string.Empty);
			SoundManager.PlaySFX("SE_ItemGet_001");
		}
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.AddCommanderFromNetwork(result.commanderData);
		localUser.RefreshItemFromNetwork(result.groupItemData);
		localUser.vipGachaCount = result.gachaCount;
		localUser.gachaInfoList.Clear();
		foreach (KeyValuePair<string, Protocols.VipGacha.VipGachaInfo> vipGachaInfo in result.VipGachaInfoList)
		{
			localUser.gachaInfoList.Add(vipGachaInfo.Value);
		}
		localUser.vipGachaCount = result.gachaCount;
		UIVipGachaContents vipGachaContents2 = UIManager.instance.world.vipGacha.vipGachaContents;
		if (vipGachaContents2 != null)
		{
			UIManager.instance.world.vipGacha.vipGachaContents.Init(localUser.gachaInfoList);
		}
		if (result.commanderData != null)
		{
			foreach (Protocols.UserInformationResponse.Commander value in result.commanderData.Values)
			{
				RoCommander roCommander2 = localUser.FindCommander(value.id);
				if (value.haveCostume != null && value.haveCostume.Count > 0)
				{
					roCommander2.haveCostumeList = value.haveCostume;
				}
			}
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7171", true, true)]
	public void DispatchCommander(int cid, int slot)
	{
	}

	private IEnumerator DispatchCommanderResult(JsonRpcClient.Request request, Dictionary<string, Protocols.DiapatchCommanderInfo> result)
	{
		RoLocalUser.SlotDispatchInfo slotDispatchInfo = new RoLocalUser.SlotDispatchInfo();
		foreach (KeyValuePair<string, Protocols.DiapatchCommanderInfo> item in result)
		{
			slotDispatchInfo.SlotNum = item.Key;
			slotDispatchInfo.dispatchCommanderInfo = item.Value;
			if (!localUser.slotDispatchInfo.Contains(slotDispatchInfo))
			{
				localUser.slotDispatchInfo.Add(slotDispatchInfo);
			}
		}
		if (UIManager.instance.world.guild.dispatch != null)
		{
			UIManager.instance.world.guild.dispatch.SetDispatchList();
		}
		yield break;
	}

	private IEnumerator DispatchCommanderError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 71001)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			UIManager.instance.world.guild.CloseDispatchPopup();
			UIManager.instance.world.guild.Close();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7172", true, true)]
	public void GetDispatchCommanderList()
	{
	}

	private IEnumerator GetDispatchCommanderListResult(JsonRpcClient.Request request, Dictionary<string, Protocols.DiapatchCommanderInfo> result)
	{
		if (result == null)
		{
			yield break;
		}
		localUser.slotDispatchInfo.Clear();
		foreach (KeyValuePair<string, Protocols.DiapatchCommanderInfo> item in result)
		{
			RoLocalUser.SlotDispatchInfo slotDispatchInfo = new RoLocalUser.SlotDispatchInfo();
			slotDispatchInfo.SlotNum = item.Key;
			slotDispatchInfo.dispatchCommanderInfo = item.Value;
			localUser.slotDispatchInfo.Add(slotDispatchInfo);
		}
		if (UIManager.instance.world.guild.dispatch != null)
		{
			UIManager.instance.world.guild.dispatch.SetDispatchList();
		}
	}

	private IEnumerator GetDispatchCommanderListError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 71001)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			UIManager.instance.world.guild.CloseDispatchPopup();
			UIManager.instance.world.guild.Close();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7173", true, true)]
	public void RecallDispatch(int slot)
	{
	}

	private IEnumerator RecallDispatchResult(JsonRpcClient.Request request, Protocols.RecallCommander result)
	{
		if (result != null)
		{
			localUser.RefreshGoodsFromNetwork(result.resource);
			localUser.ResetDispatchPossible();
			DispatchRecallResultPopup dispatchRecallResultPopup = UIPopup.Create<DispatchRecallResultPopup>("resultPopup");
			if (UIManager.instance.world.guild.dispatch != null)
			{
				dispatchRecallResultPopup.SetPopup(result.runtime, result.getGold_time, result.getGold_engage);
			}
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator RecallDispatchError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71203:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110010"));
			break;
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			UIManager.instance.world.guild.CloseDispatchPopup();
			UIManager.instance.world.guild.Close();
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6241", true, true)]
	public void GetCarnivalList(int eidx, int cctype)
	{
	}

	private IEnumerator GetCarnivalListResult(JsonRpcClient.Request request, Protocols.CarnivalList result)
	{
		if (result == null)
		{
			yield break;
		}
		localUser.connectTime = result.connectTime;
		int eidx = int.Parse(_FindRequestProperty(request, "eidx"));
		foreach (KeyValuePair<string, Dictionary<string, Protocols.CarnivalList.ProcessData>> carnivalProcess in result.carnivalProcessList)
		{
			foreach (KeyValuePair<string, Protocols.CarnivalList.ProcessData> item in carnivalProcess.Value)
			{
				switch (regulation.FindCarnivalType(carnivalProcess.Key))
				{
				case ECarnivalType.NewUserExchangeEvent_Reward:
				case ECarnivalType.NewUserExchangeEvent_Mission:
				case ECarnivalType.ExchangeEvent_Reward:
				case ECarnivalType.ExchangeEvent_Mission:
				case ECarnivalType.EventBattle_Exchange:
					if (item.Value.receive == 1)
					{
						item.Value.complete = 1;
					}
					else
					{
						item.Value.complete = (localUser.IsCompleteExchangeCarnival(item.Key) ? 1 : 0);
					}
					break;
				}
			}
		}
		localUser.carnivalList = result;
		if (result.carnivalList.Count == 0)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6037"));
		}
		else if (!UIManager.instance.world.existCarnival || !UIManager.instance.world.carnival.isActive)
		{
			UIManager.instance.world.carnival.Init(eidx);
		}
		else
		{
			UIManager.instance.RefreshOpenedUI();
		}
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6242", true, true)]
	public void CarnivalComplete(int ctid, int cidx, int eidx, int cnt)
	{
	}

	private IEnumerator CarnivalCompleteResult(JsonRpcClient.Request request, Protocols.CarnivalList result)
	{
		if (result == null)
		{
			yield break;
		}
		string key = _FindRequestProperty(request, "ctid");
		ECarnivalCategory categoryType = regulation.carnivalTypeDtbl[key].categoryType;
		if (result.rewardList != null)
		{
			UIPopup.Create<UIGetItem>("GetItem").Set(result.rewardList, string.Empty);
			SoundManager.PlaySFX("SE_ItemGet_001");
		}
		if (result.commanderData != null)
		{
			string[] array = new string[result.commanderData.Count];
			result.commanderData.Keys.CopyTo(array, 0);
			RoCommander roCommander = localUser.FindCommander(array[0]);
			if (roCommander != null)
			{
				UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				if (uICommanderComplete != null)
				{
					if (roCommander.state != ECommanderState.Nomal)
					{
						uICommanderComplete.Init(CommanderCompleteType.Recruit, roCommander.id);
					}
					else
					{
						uICommanderComplete.Init(CommanderCompleteType.Transmission, roCommander.id);
					}
				}
			}
		}
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshItemFromNetwork(result.eventResourceData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.AddCommanderFromNetwork(result.commanderData);
		localUser.RefreshCostumeFromNetwork(result.costumeData);
		localUser.RefreshItemFromNetwork(result.foodData);
		localUser.RefreshUserEquipItemFromNetwork(result.equipItemData);
		localUser.RefreshCarnivalFromNetwork(result.carnivalProcessList);
		localUser.badgeCarnivalComplete[(int)categoryType] = localUser.badgeCarnival(categoryType);
		localUser.RefreshItemFromNetwork(result.groupItemData);
		UIManager.instance.RefreshOpenedUI();
	}

	private IEnumerator CarnivalCompleteError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 20001)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5065"));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6243", true, true)]
	public void CarnivalBuyPackage(int ctid, int cidx)
	{
	}

	private IEnumerator CarnivalBuyPackageResult(JsonRpcClient.Request request, Protocols.CarnivalList result)
	{
		if (result == null)
		{
			yield break;
		}
		string key = _FindRequestProperty(request, "ctid");
		ECarnivalCategory categoryType = regulation.carnivalTypeDtbl[key].categoryType;
		if (result.rewardList != null)
		{
			UIPopup.Create<UIGetItem>("GetItem").Set(result.rewardList, string.Empty);
			SoundManager.PlaySFX("SE_ItemGet_001");
		}
		if (result.commanderData != null)
		{
			string[] array = new string[result.commanderData.Count];
			result.commanderData.Keys.CopyTo(array, 0);
			RoCommander roCommander = localUser.FindCommander(array[0]);
			if (roCommander != null)
			{
				UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				if (uICommanderComplete != null)
				{
					if (roCommander.state != ECommanderState.Nomal)
					{
						uICommanderComplete.Init(CommanderCompleteType.Recruit, roCommander.id);
					}
					else
					{
						uICommanderComplete.Init(CommanderCompleteType.Transmission, roCommander.id);
					}
				}
			}
		}
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshItemFromNetwork(result.eventResourceData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.AddCommanderFromNetwork(result.commanderData);
		localUser.RefreshCarnivalFromNetwork(result.carnivalProcessList);
		localUser.RefreshCostumeFromNetwork(result.costumeData);
		localUser.RefreshItemFromNetwork(result.foodData);
		localUser.RefreshUserEquipItemFromNetwork(result.equipItemData);
		localUser.RefreshItemFromNetwork(result.groupItemData);
		localUser.badgeCarnivalComplete[(int)categoryType] = localUser.badgeCarnival(categoryType);
		UIManager.instance.RefreshOpenedUI();
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6244", true, true)]
	public void CarnivalSelectItem(int ctid, int cidx, int ridx)
	{
	}

	private IEnumerator CarnivalSelectItemResult(JsonRpcClient.Request request, string result, Dictionary<string, Dictionary<string, Protocols.CarnivalList.ProcessData>> ctnt)
	{
		if (result != null)
		{
			localUser.RefreshCarnivalFromNetwork(ctnt);
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator CarnivalSelectItemError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 20001)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5065"));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7174", true, true)]
	public void GuildDispatchCommanderList(int type)
	{
	}

	private IEnumerator GuildDispatchCommanderListResult(JsonRpcClient.Request request, Protocols.GuildDispatchCommanderList result)
	{
		localUser.SetMercenaryList(result);
		string s = _FindRequestProperty(request, "type");
		int num = int.Parse(s);
		if (num == 1 || num == 5 || num == 10 || num == 15)
		{
			UIManager.instance.world.commanderList.SetEngageCommanderList();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1230", true, true)]
	public void GetChangeDeviceCode()
	{
	}

	private IEnumerator GetChangeDeviceCodeResult(JsonRpcClient.Request request, string result)
	{
		UISimplePopup.CreateOK("ChangeDevicePopup").Set(localization: true, "19517", result, string.Empty, "1001", string.Empty, string.Empty);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1231", true, true)]
	public void CheckChangeDeviceCode(string dac, int ch)
	{
	}

	private IEnumerator CheckChangeDeviceCodeResult(JsonRpcClient.Request request, string result, Platform plfm)
	{
		localUser.changeDeviceCode = _FindRequestProperty(request, "dac");
		Protocols.OSCode osType = Protocols.OSCode.Android;
		UIPopup.Create<UISelectPlatformPopup>("SelectPlatformPopup").InitAndOpen(osType, plfm);
		yield break;
	}

	private IEnumerator CheckChangeDeviceCodeError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 10024)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("19525"));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1232", true, true)]
	public void CheckOpenPlatformExist(Platform plfm, string tokn, int ch)
	{
	}

	private IEnumerator CheckOpenPlatformExistResult(JsonRpcClient.Request request, bool result)
	{
		Platform plfm = (Platform)int.Parse(_FindRequestProperty(request, "plfm"));
		if (result)
		{
			UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: true, "19527", (plfm != Platform.FaceBook) ? "19529" : "19528", null, "1001", "1000");
			uISimplePopup.onClick = delegate(GameObject popupSender)
			{
				string text = popupSender.name;
				if (text == "OK")
				{
					if (localUser.loginType == 1)
					{
						RequestChangeDevice(plfm);
					}
					else
					{
						RequestChangeMembershipOpenPlatform(localUser.openPlatformToken, plfm, PlayerPrefs.GetString("GuestID"));
					}
				}
			};
		}
		else if (localUser.loginType == 1)
		{
			RequestChangeDevice(plfm);
		}
		else
		{
			RequestChangeMembershipOpenPlatform(localUser.openPlatformToken, plfm, PlayerPrefs.GetString("GuestID"));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1233", true, true)]
	public void ChangeDevice(int ch, string dac, string tokn, Platform plfm, string devc, string dvid, int ptype, Protocols.OSCode oscd, string osvr, string gmvr, string apk, string lang, string gpid)
	{
	}

	private IEnumerator ChangeDeviceResult(JsonRpcClient.Request request, bool result, int mIdx, string tokn)
	{
		Platform platform = (Platform)int.Parse(_FindRequestProperty(request, "plfm"));
		PlayerPrefs.SetString("MemberID", localUser.platformUserInfo);
		PlayerPrefs.SetString("MemberPW", null);
		PlayerPrefs.SetInt("MemberPlatform", (int)platform);
		localUser.mIdx = mIdx;
		localUser.tokn = tokn;
		localUser.platform = platform;
		LocalStorage.SaveLoginData(localUser.platformUserInfo, null, (int)platform);
		instance.bLogin = true;
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4401", true, true)]
	public void StartDateMode()
	{
	}

	private IEnumerator StartDateModeResult(JsonRpcClient.Request request, Protocols.ResourceRecharge result)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4402", true, true)]
	public void DateModeGetGift()
	{
	}

	private IEnumerator DateModeGetGiftResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		if (result.reward != null && result.reward.Count > 0)
		{
			UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
			SoundManager.PlaySFX("SE_ItemGet_001");
			localUser.RefreshRewardFromNetwork(result);
			UIManager.instance.RefreshOpenedUI();
		}
		else
		{
			UISimplePopup.CreateOK(localization: false, Localization.Get("300005"), Localization.Get("300006"), string.Empty, Localization.Get("1001"));
		}
		if (result.commander != null)
		{
			foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> item in result.commander)
			{
				_ = item.Value;
				RoCommander roCommander = localUser.FindCommander(item.Value.id);
				roCommander.favorStep = item.Value.favorStep;
				roCommander.favorPoint = item.Value.favorPoint;
			}
		}
		UIManager.instance.world.dateMode.GetDateGift(result.time);
		yield break;
	}

	private IEnumerator DateModeGetGiftError(JsonRpcClient.Request request, string result, int code)
	{
		NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), "Error code:" + code);
		UIManager.instance.world.dateMode.CloseGiftPopup();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1506", true, true)]
	public void SetPushOnOff(int onoff)
	{
	}

	private IEnumerator SetPushOnOffResult(JsonRpcClient.Request request, bool result)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1233", true, true)]
	public void ChangeDeviceDbros(int ch, string dac, string uid, string pwd, Platform plfm, string devc, string dvid, int ptype, Protocols.OSCode oscd, string osvr, string gmvr, string apk, string lang, string gpid)
	{
	}

	private IEnumerator ChangeDeviceDbrosResult(JsonRpcClient.Request request, bool result, int mIdx, string tokn)
	{
		string text = _FindRequestProperty(request, "uid");
		string text2 = _FindRequestProperty(request, "pwd");
		PlayerPrefs.SetString("MemberID", text);
		PlayerPrefs.SetString("MemberPW", text2);
		instance.RequestSignIn(text, text2);
		yield break;
	}

	private IEnumerator ChangeDeviceDbrosError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 10014:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("19033"));
			break;
		case 20014:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7054"));
			break;
		default:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), "Error code:" + code);
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4309", true, true)]
	public void GetCommanderScenario()
	{
	}

	private IEnumerator GetCommanderScenarioResult(JsonRpcClient.Request request, Dictionary<string, Dictionary<string, Protocols.CommanderScenario>> result)
	{
		localUser.sn_resultDictionary = result;
		if (UIManager.instance.world != null && UIManager.instance.world.existCommanderDetail && UIManager.instance.world.commanderDetail.isActive)
		{
			UIManager.instance.world.commanderDetail.InitScenarioList();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4310", true, true)]
	public void CompleteCommanderScenario(int cid, int sid, int sqid)
	{
	}

	private IEnumerator CompleteCommanderScenarioResult(JsonRpcClient.Request request, Protocols.CompleteScenario result)
	{
		if (result != null)
		{
			ScenarioResultPopup scenarioResultPopup = UIPopup.Create<ScenarioResultPopup>("ScenarioResultPopup");
			if (scenarioResultPopup != null)
			{
				scenarioResultPopup.Init(result.reward);
			}
			scenarioResultPopup.onClose = delegate
			{
				waitingScenarioComplete = false;
			};
			localUser.RefreshGoodsFromNetwork(result.resource);
			localUser.RefreshPartFromNetwork(result.partData);
			localUser.RefreshItemFromNetwork(result.eventResourceData);
			localUser.RefreshItemFromNetwork(result.itemData);
			localUser.RefreshMedalFromNetwork(result.medalData);
			localUser.AddCommanderFromNetwork(result.commander);
			localUser.RefreshCostumeFromNetwork(result.costumeData);
			localUser.RefreshItemFromNetwork(result.foodData);
			UIManager.instance.RefreshOpenedUI();
		}
		else
		{
			waitingScenarioComplete = false;
		}
		yield break;
	}

	private IEnumerator CompleteCommanderScenarioError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 30111)
		{
			ScenarioResultPopup scenarioResultPopup = UIPopup.Create<ScenarioResultPopup>("ScenarioResultPopup");
			if (scenarioResultPopup != null)
			{
				scenarioResultPopup.Init(null, isAgainClear: true);
			}
			scenarioResultPopup.onClose = delegate
			{
				waitingScenarioComplete = false;
			};
		}
		else
		{
			waitingScenarioComplete = false;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4311", true, true)]
	public void RecieveCommanderScenarioReward(int cid, int sid)
	{
	}

	private IEnumerator RecieveCommanderScenarioRewardResult(JsonRpcClient.Request request, Protocols.RecieveScenarioReward result)
	{
		if (result != null)
		{
			ScenarioResultPopup scenarioResultPopup = UIPopup.Create<ScenarioResultPopup>("ScenarioResultPopup");
			if (scenarioResultPopup != null)
			{
				scenarioResultPopup.Init(result.reward);
			}
			localUser.RefreshGoodsFromNetwork(result.resource);
			localUser.RefreshPartFromNetwork(result.partData);
			localUser.RefreshItemFromNetwork(result.eventResourceData);
			localUser.RefreshItemFromNetwork(result.itemData);
			localUser.RefreshMedalFromNetwork(result.medalData);
			localUser.AddCommanderFromNetwork(result.commander);
			localUser.RefreshCostumeFromNetwork(result.costumeData);
			localUser.RefreshItemFromNetwork(result.foodData);
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3301", true, true)]
	public void WaveBattleList()
	{
	}

	private IEnumerator WaveBattleListResult(JsonRpcClient.Request request, Protocols.WaveBattleInfoList result)
	{
		if (result != null && result.InfoList != null)
		{
			if (!UIManager.instance.world.existWaveBattle || !UIManager.instance.world.waveBattle.isActive)
			{
				UIManager.instance.world.waveBattle.InitAndOpen();
			}
			UIManager.instance.world.waveBattle.SetWaveData(result.InfoList);
		}
		yield return null;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3231", true, true)]
	public void WaveBattleStart(int type, JObject deck, JObject gdp, int ucash, int idx, int np)
	{
	}

	private IEnumerator WaveBattleStartResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		if (result != null)
		{
			localUser.RefreshGoodsFromNetwork(result.goodsInfo);
		}
		Loading.Load(Loading.Battle);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3510", true, true)]
	public void GetGroupReward(int giIdx)
	{
	}

	private IEnumerator GetGroupRewardResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		string text = _FindRequestProperty(request, "giIdx");
		localUser.AddGroupCompleteData(int.Parse(text));
		if (result.commander != null)
		{
			foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> item in result.commander)
			{
				Protocols.UserInformationResponse.Commander value = item.Value;
				RoCommander roCommander = localUser.FindCommander(value.id);
				CommanderCompleteType type = ((roCommander.state != ECommanderState.Nomal) ? CommanderCompleteType.Recruit : CommanderCompleteType.Transmission);
				UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
				uICommanderComplete.Init(type, value.id);
			}
		}
		else
		{
			string strMessage = string.Empty;
			GroupInfoDataRow groupInfoDataRow = instance.regulation.FindGroupInfoWhereGroupIdx(text);
			if (groupInfoDataRow != null)
			{
				strMessage = groupInfoDataRow.groupComment;
			}
			UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, strMessage);
			SoundManager.PlaySFX("SE_ItemGet_001");
		}
		if (localUser.badgeGroupCount > 0)
		{
			localUser.badgeGroupCount--;
		}
		localUser.RefreshRewardFromNetwork(result);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator GetGroupRewardError(JsonRpcClient.Request request, string result, int code)
	{
		NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), "Error code:" + code);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7421", true, true)]
	public void SetItemEquipment(int eidx, int cid, int elv)
	{
	}

	private IEnumerator SetItemEquipmentResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		if (result.equipItem == null || result.commanderInfo == null)
		{
			yield break;
		}
		RoCommander roCommander = null;
		foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> item in result.commanderInfo)
		{
			string key = item.Key;
			roCommander = localUser.FindCommander(key);
			if (roCommander == null)
			{
				continue;
			}
			foreach (KeyValuePair<string, int> item2 in item.Value.equipItemInfo)
			{
				RoItem roItem = localUser.EquipedList_FindItem(item2.Key, key, item2.Value);
				if (roItem == null)
				{
					localUser.EquipedList_AddItem(RoItem.Create(item2.Key, item2.Value, 1, key));
					RoItem roItem2 = localUser.EquipedList_FindItem(item2.Key, key, item2.Value);
					roCommander.SetEquipItem(roItem2.pointType, roItem2);
				}
				else
				{
					roCommander.SetEquipItem(roItem.pointType, roItem);
				}
			}
		}
		int num = 0;
		RoItem currSelectItem = null;
		_ = string.Empty;
		foreach (KeyValuePair<string, Dictionary<int, Protocols.EquipItemInfo>> item3 in result.equipItem)
		{
			string key2 = item3.Key;
			int num2 = 0;
			foreach (KeyValuePair<int, Protocols.EquipItemInfo> item4 in item3.Value)
			{
				int key3 = item4.Key;
				if ((result.equipItem.Count == 2 && num == 0) || (item3.Value.Count == 2 && num2 == 0))
				{
					RoItem roItem3 = localUser.EquipedList_FindItem(key2, roCommander.id, key3);
					if (roItem3 != null)
					{
						localUser.EquipedeList_RemoveItem(roItem3);
					}
				}
				currSelectItem = localUser.EquipedList_FindItem(key2, roCommander.id, key3);
				localUser.SetEquipPossibleItemCount(key2, key3, item4.Value.availableCount);
				num2++;
			}
			num++;
		}
		if (UIManager.instance.world.existCommanderDetail && UIManager.instance.world.commanderDetail.isActive)
		{
			UICommanderDetail commanderDetail = UIManager.instance.world.commanderDetail;
			commanderDetail.OnRefresh();
			commanderDetail.haveItemListPopup.RefreshList();
		}
		if (UIManager.instance.world.existLaboratory && UIManager.instance.world.laboratory.isActive)
		{
			UIManager.instance.world.laboratory.currSelectItem = currSelectItem;
			UIManager.instance.world.laboratory.OnRefresh();
		}
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7422", true, true)]
	public void ReleaseItemEquipment(int eidx, int cid)
	{
	}

	private IEnumerator ReleaseItemEquipmentResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		if (result.equipItem == null || result.commanderInfo == null)
		{
			yield break;
		}
		string releaseItemIdx = string.Empty;
		int num = 0;
		foreach (KeyValuePair<string, Dictionary<int, Protocols.EquipItemInfo>> item in result.equipItem)
		{
			releaseItemIdx = item.Key;
			foreach (KeyValuePair<int, Protocols.EquipItemInfo> item2 in item.Value)
			{
				num = item2.Key;
				localUser.SetEquipPossibleItemCount(releaseItemIdx, num, item2.Value.availableCount);
			}
		}
		foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> item3 in result.commanderInfo)
		{
			string key = item3.Key;
			RoCommander roCommander = localUser.FindCommander(key);
			if (roCommander != null)
			{
				EquipItemDataRow equipItemDataRow = regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == releaseItemIdx);
				RoItem roItem = localUser.EquipedList_FindItem(releaseItemIdx, key, num);
				if (roItem != null)
				{
					localUser.EquipedeList_RemoveItem(roItem);
					roCommander.ClearEquipItem(equipItemDataRow.pointType, roItem);
				}
			}
		}
		if (UIManager.instance.world.existCommanderDetail && UIManager.instance.world.commanderDetail.isActive)
		{
			UICommanderDetail commanderDetail = UIManager.instance.world.commanderDetail;
			commanderDetail.OnRefresh();
			commanderDetail.haveItemListPopup.RefreshList();
		}
		if (UIManager.instance.world.existLaboratory && UIManager.instance.world.laboratory.isActive)
		{
			UIManager.instance.world.laboratory.haveItemListPopup.RefreshList();
		}
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7423", true, true)]
	public void UpgradeItemEquipment(int eidx, int cid, int elv)
	{
	}

	private IEnumerator UpgradeItemEquipmentResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshGoodsFromNetwork(result.goodsInfo);
		RoItem roItem = null;
		string id = string.Empty;
		int lv = 0;
		foreach (KeyValuePair<string, Dictionary<int, Protocols.EquipItemInfo>> item in result.equipItem)
		{
			id = item.Key;
			foreach (KeyValuePair<int, Protocols.EquipItemInfo> item2 in item.Value)
			{
				lv = item2.Key;
				localUser.SetEquipPossibleItemCount(id, lv, item2.Value.availableCount);
			}
		}
		if (result.commanderInfo != null)
		{
			foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> item3 in result.commanderInfo)
			{
				if (item3.Value.equipItemInfo == null)
				{
					continue;
				}
				foreach (KeyValuePair<string, int> item4 in item3.Value.equipItemInfo)
				{
					localUser.EquipedList_upgradeItem(item4.Key, item4.Value, item3.Key);
					roItem = localUser.EquipedList_FindItem(item4.Key, item3.Key, item4.Value);
					RoCommander roCommander = localUser.FindCommander(item3.Key);
					if (roCommander != null && roItem != null)
					{
						roCommander.SetEquipItem(roItem.pointType, roItem);
					}
				}
			}
		}
		if (roItem == null)
		{
			roItem = localUser.EquipPossibleList_FindItem(id, lv);
		}
		if (UIManager.instance.world.existLaboratory && UIManager.instance.world.laboratory.isActive)
		{
			UIManager.instance.world.laboratory.currSelectItem = roItem;
			UIManager.instance.world.laboratory.OnRefresh();
			SoundManager.PlaySFX("SE_Upgrade_001");
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7424", true, true)]
	public void DecompositionItemEquipment(int eidx, int elv, int amnt)
	{
	}

	private IEnumerator DecompositionItemEquipmentResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result, List<Protocols.RewardInfo.RewardData> reward)
	{
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshGoodsFromNetwork(result.goodsInfo);
		_ = string.Empty;
		foreach (KeyValuePair<string, Dictionary<int, Protocols.EquipItemInfo>> item in result.equipItem)
		{
			string key = item.Key;
			foreach (KeyValuePair<int, Protocols.EquipItemInfo> item2 in item.Value)
			{
				int key2 = item2.Key;
				localUser.SetEquipPossibleItemCount(key, key2, item2.Value.availableCount);
			}
		}
		UIPopup.Create<UIGetItem>("GetItem").Set(reward, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		if (UIManager.instance.world.existLaboratory && UIManager.instance.world.laboratory.isActive)
		{
			UIManager.instance.world.laboratory.currSelectItem = null;
			UIManager.instance.world.laboratory.OnRefresh();
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3501", true, true)]
	public void GetExplorationList()
	{
	}

	private IEnumerator GetExplorationListResult(JsonRpcClient.Request request, List<Protocols.ExplorationData> result)
	{
		for (int i = 0; i < result.Count; i++)
		{
			string worldMap = regulation.explorationDtbl[result[i].idx.ToString()].worldMap;
			localUser.explorationDtbl[worldMap].Set(result[i]);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3502", true, true)]
	public void ExplorationStart(int idx, List<string> cid)
	{
	}

	private IEnumerator ExplorationStartResult(JsonRpcClient.Request request, bool result)
	{
		if (result)
		{
			string key = _FindRequestProperty(request, "idx");
			ExplorationDataRow explorationDataRow = regulation.explorationDtbl[key];
			string worldMap = explorationDataRow.worldMap;
			Protocols.ExplorationData explorationData = new Protocols.ExplorationData();
			explorationData.idx = explorationDataRow.idx;
			explorationData.remainTime = explorationDataRow.searchTime * 3600;
			explorationData.cids = JsonConvert.DeserializeObject<List<string>>(_FindRequestProperty(request, "cid"));
			localUser.explorationDtbl[worldMap].Set(explorationData);
			UIExplorationPopup.UIRefresh(worldMap);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3550", true, true)]
	public void ExplorationStartAll(JArray search)
	{
	}

	private IEnumerator ExplorationStartAllResult(JsonRpcClient.Request request, bool result)
	{
		if (!result)
		{
			yield break;
		}
		string value = _FindRequestProperty(request, "search");
		List<Protocols.ExplorationStartInfo> list = JsonConvert.DeserializeObject<List<Protocols.ExplorationStartInfo>>(value);
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				ExplorationDataRow explorationDataRow = regulation.explorationDtbl[list[i].idx.ToString()];
				string worldMap = explorationDataRow.worldMap;
				Protocols.ExplorationData explorationData = new Protocols.ExplorationData();
				explorationData.idx = explorationDataRow.idx;
				explorationData.remainTime = explorationDataRow.searchTime * 3600;
				explorationData.cids = list[i].cids;
				localUser.explorationDtbl[worldMap].Set(explorationData);
				UIExplorationPopup.UIRefresh(worldMap);
			}
		}
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3503", true, true)]
	public void ExplorationCancel(int idx)
	{
	}

	private IEnumerator ExplorationCancelResult(JsonRpcClient.Request request, bool result)
	{
		if (result)
		{
			string key = _FindRequestProperty(request, "idx");
			ExplorationDataRow explorationDataRow = regulation.explorationDtbl[key];
			string worldMap = explorationDataRow.worldMap;
			localUser.explorationDtbl[worldMap].Set(null);
			UIExplorationPopup.UIRefresh(worldMap);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3504", true, true)]
	public void ExplorationComplete(int idx)
	{
	}

	private IEnumerator ExplorationCompleteResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		string key = _FindRequestProperty(request, "idx");
		ExplorationDataRow explorationDataRow = regulation.explorationDtbl[key];
		string worldMap = explorationDataRow.worldMap;
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		localUser.RefreshRewardFromNetwork(result);
		int num = regulation.commanderLevelDtbl[(localUser.level + 1).ToString()].aexp - 1;
		RoExploration roExploration = localUser.explorationDtbl[worldMap];
		for (int i = 0; i < roExploration.commanders.Count; i++)
		{
			int num2 = (int)roExploration.commanders[i].aExp + result.exp;
			if (num2 > num)
			{
				num2 = num;
			}
			roExploration.commanders[i].aExp = num2;
		}
		roExploration.Set(null);
		UIExplorationPopup.UIRefresh(worldMap);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3551", true, true)]
	public void ExplorationCompleteAll(List<int> idxs)
	{
	}

	private IEnumerator ExplorationCompleteAllResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		List<int> idxs = JsonConvert.DeserializeObject<List<int>>(_FindRequestProperty(request, "idxs"));
		if (idxs != null)
		{
			for (int i = 0; i < idxs.Count; i++)
			{
				ExplorationDataRow explorationDataRow = regulation.explorationDtbl[idxs[i].ToString()];
				string worldMap = explorationDataRow.worldMap;
				int num = 0;
				if (result.explorationExp != null)
				{
					int num2 = result.explorationExp.FindIndex((Protocols.RewardInfo.ExplorationExp x) => x.idx == idxs[i]);
					if (num2 >= 0)
					{
						num = result.explorationExp[num2].exp;
					}
				}
				int num3 = regulation.commanderLevelDtbl[(localUser.level + 1).ToString()].aexp - 1;
				RoExploration roExploration = localUser.explorationDtbl[worldMap];
				for (int j = 0; j < roExploration.commanders.Count; j++)
				{
					int num4 = (int)roExploration.commanders[j].aExp + num;
					if (num4 > num3)
					{
						num4 = num3;
					}
					roExploration.commanders[j].aExp = num4;
				}
				roExploration.Set(null);
				UIExplorationPopup.UIRefresh(worldMap);
			}
		}
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		localUser.RefreshRewardFromNetwork(result);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7172", true, true)]
	public void GetDispatchCommanderListFromLogin()
	{
	}

	private IEnumerator GetDispatchCommanderListFromLoginResult(JsonRpcClient.Request request, Dictionary<string, Protocols.DiapatchCommanderInfo> result)
	{
		if (result == null)
		{
			yield break;
		}
		foreach (KeyValuePair<string, Protocols.DiapatchCommanderInfo> item in result)
		{
			RoCommander roCommander = localUser.FindCommander(item.Value.cid.ToString());
			if (roCommander != null)
			{
				roCommander.isDispatch = true;
			}
		}
	}

	private IEnumerator GetDispatchCommanderListFromLoginError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7601", true, true)]
	public void CooperateBattleInfo()
	{
	}

	private IEnumerator CooperateBattleInfoResult(JsonRpcClient.Request request, Protocols.CooperateBattleData result)
	{
		int coopStage = localUser.coopStage;
		if (!UIManager.instance.world.existCooperateBattle)
		{
			UIManager.instance.world.cooperateBattle.InitAndOpen(result);
		}
		else if (!UIManager.instance.world.cooperateBattle.isActive)
		{
			UIManager.instance.world.cooperateBattle.InitAndOpen(result);
		}
		else
		{
			UIManager.instance.world.cooperateBattle.Set(result);
		}
		if (coopStage > 0 && localUser.coopStage > coopStage)
		{
			UIManager.instance.world.cooperateBattle.LevelUp();
		}
		yield break;
	}

	private IEnumerator CooperateBattleInfoError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			if (UIManager.instance.world.guild.isActive)
			{
				UIManager.instance.world.guild.Close();
			}
			break;
		case 71605:
		{
			int num = int.Parse(regulation.defineDtbl["COOPERATE_BATTLE_OPEN_GUILD_LEVEL"].value);
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("110089", num));
			break;
		}
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7603", true, true)]
	public void CooperateBattlePointGuildRank()
	{
	}

	private IEnumerator CooperateBattlePointGuildRankResult(JsonRpcClient.Request request, List<Protocols.CooperateBattlePointGuildRankingInfo> result)
	{
		if (UIManager.instance.world.cooperateBattle.isActive)
		{
			UIManager.instance.world.cooperateBattle.SetRankingData(result);
		}
		yield break;
	}

	private IEnumerator CooperateBattlePointGuildRankError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			if (UIManager.instance.world.guild.isActive)
			{
				UIManager.instance.world.guild.Close();
			}
			break;
		case 71603:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5090032"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7604", true, true)]
	public void CooperateBattlePointRank(int step)
	{
	}

	private IEnumerator CooperateBattlePointRankResult(JsonRpcClient.Request request, List<Protocols.CooperateBattlePointUserRankingInfo> result)
	{
		if (UIManager.instance.world.cooperateBattle.isActive)
		{
			int stage = int.Parse(_FindRequestProperty(request, "step"));
			UIManager.instance.world.cooperateBattle.SetRankingData(stage, result);
		}
		yield break;
	}

	private IEnumerator CooperateBattlePointRankError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			if (UIManager.instance.world.guild.isActive)
			{
				UIManager.instance.world.guild.Close();
			}
			break;
		case 71603:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5090032"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "7602", true, true)]
	public void CooperateBattleComplete(int stage, int step)
	{
	}

	private IEnumerator CooperateBattleCompleteResult(JsonRpcClient.Request request, Protocols.CooperateBattleRewardInfo result)
	{
		UIGetItem uIGetItem = UIPopup.Create<UIGetItem>("GetItem");
		uIGetItem.Set(result.reward, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		localUser.RefreshRewardFromNetwork(result);
		UIManager.instance.RefreshOpenedUI();
		UICooperateBattle cooperateBattle = UIManager.instance.world.cooperateBattle;
		if (!cooperateBattle.isActive)
		{
			yield break;
		}
		Protocols.CooperateBattleData cooperateBattleData = new Protocols.CooperateBattleData();
		cooperateBattleData.coop = result.coop;
		cooperateBattleData.recv = result.recv;
		int prevStage = localUser.coopStage;
		cooperateBattle.Set(cooperateBattleData);
		uIGetItem.onClose = delegate
		{
			if (UIManager.instance.world.cooperateBattle.isActive && prevStage > 0 && localUser.coopStage > prevStage)
			{
				UIManager.instance.world.cooperateBattle.LevelUp();
			}
		};
	}

	private IEnumerator CooperateBattleCompleteError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			if (UIManager.instance.world.guild.isActive)
			{
				UIManager.instance.world.guild.Close();
			}
			break;
		case 71603:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5090029"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3231", true, true)]
	public void CooperateBattleStart(int type, JObject deck, int stage, int step)
	{
	}

	private IEnumerator CooperateBattleStartResult(JsonRpcClient.Request request, bool result)
	{
		Loading.Load(Loading.Battle);
		yield break;
	}

	private IEnumerator CooperateBattleStartError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 71001:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110303"));
			if (UIManager.instance.world.guild.isActive)
			{
				UIManager.instance.world.guild.Close();
			}
			break;
		case 71604:
		{
			UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("5090012"), Localization.Get("5090013"), null, "1001");
			uISimplePopup.onClose = delegate
			{
				RequestCooperateBattleInfo();
				if (UIManager.instance.world.readyBattle.isActive)
				{
					UIManager.instance.world.readyBattle.CloseAnimation();
				}
			};
			break;
		}
		case 71603:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5090010"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8119", true, true)]
	public void GetWebEvent(int ch)
	{
	}

	private IEnumerator GetWebEventResult(JsonRpcClient.Request request, string result, List<string> wev)
	{
		if (!localUser.badgeWebEvent)
		{
			if (localUser.webEventUrls == null || localUser.webEventUrls.Count != wev.Count)
			{
				localUser.badgeWebEvent = true;
			}
			else
			{
				for (int i = 0; i < localUser.webEventUrls.Count; i++)
				{
					if (localUser.webEventUrls[i] != wev[i])
					{
						localUser.badgeWebEvent = true;
						break;
					}
				}
			}
		}
		localUser.webEventUrls = wev;
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8119", true, true)]
	public void StartWebEvent(int ch)
	{
	}

	private IEnumerator StartWebEventResult(JsonRpcClient.Request request, string result, List<string> wev)
	{
		localUser.webEventUrls = wev;
		localUser.badgeWebEvent = false;
		if (localUser.webEventUrls.Count > 0)
		{
			UIPopup.Create<UIWebviewPopup>("UIWebView").Init(Base64Decode(localUser.webEventUrls[0]));
		}
		else
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("1999"));
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "6401", true, true)]
	public void GetRotationBannerInfo()
	{
	}

	private IEnumerator GetRotationBannerInfoResult(JsonRpcClient.Request request, Protocols.RotationBanner result)
	{
		if (result == null)
		{
			if (UIManager.instance.world.mainCommand != null)
			{
				UISetter.SetActive(UIManager.instance.world.mainCommand.banner.gameObject, active: false);
			}
		}
		else if (result.bannerList != null && result.bannerList.Count > 0)
		{
			UIMainCommand mainCommand = UIManager.instance.world.mainCommand;
			if (mainCommand != null)
			{
				UISetter.SetActive(mainCommand.banner, active: true);
				mainCommand.banner.InitUrlList(result);
			}
		}
		else
		{
			UISetter.SetActive(UIManager.instance.world.mainCommand.banner.gameObject, active: false);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2301", true, true)]
	public void GetEventBattleList()
	{
	}

	private IEnumerator GetEventBattleListResult(JsonRpcClient.Request request, List<Protocols.EventBattleInfo> result)
	{
		if (result.Count > 0)
		{
			UIEventBattleListPopup uIEventBattleListPopup = UIPopup.Create<UIEventBattleListPopup>("EventBattleListPopup");
			uIEventBattleListPopup.Init(result);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2302", true, true)]
	public void GetEventBattleData(int eidx, int level)
	{
	}

	private IEnumerator GetEventBattleDataResult(JsonRpcClient.Request request, Protocols.EventBattleData result)
	{
		int eventId = int.Parse(_FindRequestProperty(request, "eidx"));
		int num = int.Parse(_FindRequestProperty(request, "level"));
		if (result == null)
		{
			yield break;
		}
		if (UIManager.instance.world != null)
		{
			UIEventBattle eventBattle;
			if (!UIManager.instance.world.existEventBattle)
			{
				eventBattle = UIManager.instance.world.eventBattle;
				eventBattle.Init();
			}
			else
			{
				eventBattle = UIManager.instance.world.eventBattle;
				if (!UIManager.instance.world.eventBattle.isActive)
				{
					eventBattle.Init();
				}
			}
			eventBattle.SetEventBattle(eventId, result);
			if (num > 0)
			{
				eventBattle.StartEventReadyBattle(num);
			}
			else if (num == 0)
			{
				eventBattle.CreateEventRaidListPopup(2);
			}
		}
		else
		{
			localUser.lastShowEventScenarioPlayTurn = int.Parse(result.eventData.esid);
		}
		UIManager.instance.RefreshOpenedUI();
	}

	private IEnumerator GetEventBattleDataError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 70201:
		case 70210:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6600"));
			break;
		case 70204:
		case 70205:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2303", true, true)]
	public void EventRaidSummon(int eidx)
	{
	}

	private IEnumerator EventRaidSummonResult(JsonRpcClient.Request request, string result, Dictionary<string, int> ersoc, Protocols.EventBattleData.RaidData bInfo, int bossCnt)
	{
		int.Parse(_FindRequestProperty(request, "eidx"));
		if (result != null)
		{
			localUser.RefreshItemFromNetwork(ersoc);
			UIEventBattle eventBattle = UIManager.instance.world.eventBattle;
			eventBattle.StartWarningEffect();
			eventBattle.SetRaidData(bInfo, bossCnt);
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator EventRaidSummonError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 70201:
		case 70210:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6600"));
			if (UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
			{
				UIManager.instance.world.eventBattle.ClosePopUp();
			}
			break;
		case 70204:
		case 70205:
		{
			UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, Localization.Format("110367", code), Localization.Get("1001"));
			uISimplePopup.onClose = delegate
			{
				if (UIManager.instance.world.readyBattle.isActive && UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
				{
					UIManager.instance.world.eventBattle.ClosePopUp();
				}
			};
			break;
		}
		case 70206:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6603"));
			if (UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
			{
				UIManager.instance.world.eventBattle.ClosePopUp();
			}
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2304", true, true)]
	public void EventRaidShare(int mbid)
	{
	}

	private IEnumerator EventRaidShareResult(JsonRpcClient.Request request, string result)
	{
		string bid = _FindRequestProperty(request, "mbid");
		if (result == "true" || result == "True")
		{
			UIEventBattle eventBattle = UIManager.instance.world.eventBattle;
			eventBattle.EventRaidShared(bid);
			UISimplePopup.CreateOK(localization: true, "1303", "6609", null, "5775");
		}
		yield break;
	}

	private IEnumerator EventRaidShareError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 70201:
		case 70210:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6600"));
			if (UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
			{
				UIManager.instance.world.eventBattle.ClosePopUp();
			}
			break;
		case 70204:
		case 70205:
		{
			UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, Localization.Format("110367", code), Localization.Get("1001"));
			uISimplePopup.onClose = delegate
			{
				if (UIManager.instance.world.readyBattle.isActive && UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
				{
					UIManager.instance.world.eventBattle.ClosePopUp();
				}
			};
			break;
		}
		case 70202:
		case 70203:
		case 70207:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6606"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2305", true, true)]
	public void EventRaidList(int type)
	{
	}

	private IEnumerator EventRaidListResult(JsonRpcClient.Request request, Protocols.EventRaidList result)
	{
		int type = int.Parse(_FindRequestProperty(request, "type"));
		UIEventBattle eventBattle = UIManager.instance.world.eventBattle;
		eventBattle.CreateEventRaidPopup(type, result.rewardCount, result.bossList);
		yield break;
	}

	private IEnumerator EventRaidListError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 70201:
		case 70210:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6600"));
			if (UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
			{
				UIManager.instance.world.eventBattle.ClosePopUp();
			}
			break;
		case 70204:
		case 70205:
		{
			UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), string.Empty, Localization.Get("1001"));
			uISimplePopup.onClose = delegate
			{
				if (UIManager.instance.world.readyBattle.isActive && UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
				{
					UIManager.instance.world.eventBattle.ClosePopUp();
				}
			};
			break;
		}
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2306", true, true)]
	public void EventRaidData(int mbid)
	{
	}

	private IEnumerator EventRaidDataResult(JsonRpcClient.Request request, string result, Protocols.EventRaidData bInfo)
	{
		string bid = _FindRequestProperty(request, "mbid");
		UIEventBattle eventBattle = UIManager.instance.world.eventBattle;
		eventBattle.UpdateEventRaid(bid, bInfo);
		eventBattle.StartRaidReadyBattle(bid);
		yield break;
	}

	private IEnumerator EventRaidDataError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 70201:
		case 70210:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6600"));
			if (UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
			{
				UIManager.instance.world.eventBattle.CloseEventRaid();
			}
			break;
		case 70204:
		case 70205:
		{
			UISimplePopup uISimplePopup2 = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), string.Empty, Localization.Get("1001"));
			uISimplePopup2.onClose = delegate
			{
				if (UIManager.instance.world.readyBattle.isActive && UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
				{
					UIManager.instance.world.eventBattle.ClosePopUp();
				}
			};
			break;
		}
		case 70202:
		case 70203:
		case 70207:
		{
			UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, Localization.Get("6606"), Localization.Get("1001"));
			uISimplePopup.onClose = delegate
			{
				if (UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
				{
					UIManager.instance.world.eventBattle.RefreshEventRaid();
				}
			};
			break;
		}
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2307", true, true)]
	public void EventRaidRankingData(int mbid)
	{
	}

	private IEnumerator EventRaidRankingDataResult(JsonRpcClient.Request request, List<Protocols.EventRaidRankingData> result)
	{
		_FindRequestProperty(request, "mbid");
		UIEventBattle eventBattle = UIManager.instance.world.eventBattle;
		eventBattle.CreateRaidRankingPopup(result);
		yield break;
	}

	private IEnumerator EventRaidRankingError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 70201:
		case 70210:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6600"));
			if (UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
			{
				UIManager.instance.world.eventBattle.CloseEventRaid();
			}
			break;
		case 70204:
		case 70205:
		{
			UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), string.Empty, Localization.Get("1001"));
			uISimplePopup.onClose = delegate
			{
				if (UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
				{
					UIManager.instance.world.eventBattle.ClosePopUp();
				}
			};
			break;
		}
		case 70202:
		case 70203:
		case 70207:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6606"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2308", true, true)]
	public void GetEventRaidReward(int mbid)
	{
	}

	private IEnumerator GetEventRaidRewardResult(JsonRpcClient.Request request, Protocols.EventRaidReward result)
	{
		int num = int.Parse(_FindRequestProperty(request, "mbid"));
		UIPopup.Create<UIGetItem>("GetItem").Set(result.rewardList, result.rewardCount);
		SoundManager.PlaySFX("SE_ItemGet_001");
		UIEventBattle eventBattle = UIManager.instance.world.eventBattle;
		if (num > 0)
		{
			eventBattle.EventRaidRewardReceive(result.rewardCount.mbids);
		}
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshItemFromNetwork(result.foodData);
		localUser.RefreshItemFromNetwork(result.eventResourceData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator GetEventRaidRewardError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 70201:
		case 70210:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6600"));
			if (UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
			{
				UIManager.instance.world.eventBattle.CloseEventRaid();
			}
			break;
		case 70204:
		case 70205:
		case 70208:
		case 70209:
		{
			UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), string.Empty, Localization.Get("1001"));
			uISimplePopup.onClose = delegate
			{
				if (UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive)
				{
					UIManager.instance.world.eventBattle.ClosePopUp();
				}
			};
			break;
		}
		case 70211:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6610"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4312", true, true)]
	public void GetMarried(int cid)
	{
	}

	private IEnumerator GetMarriedResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		string text = _FindRequestProperty(request, "cid");
		if (result.commanderInfo != null)
		{
			foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> item in result.commanderInfo)
			{
				_ = item.Value;
				RoCommander roCommander = localUser.FindCommander(item.Value.id);
				roCommander.marry = item.Value.marry;
			}
		}
		localUser.RefreshGoodsFromNetwork(result.goodsInfo);
		UIManager.instance.RefreshOpenedUI();
		CommanderScenarioDataRow commanderScenarioDataRow = regulation.FindCommanderScenario(text, 0);
		if (commanderScenarioDataRow != null)
		{
			localUser.currScenario.scenarioId = commanderScenarioDataRow.csid;
			localUser.currScenario.commanderId = int.Parse(text);
			Loading.Load(Loading.Scenario);
		}
		yield break;
	}

	private IEnumerator GetMarriedError(JsonRpcClient.Request request, string result, int code)
	{
		NetworkAnimation.Instance.CreateFloatingText("Error code:" + code);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8300", true, true)]
	public void GetPlugEventInfo()
	{
	}

	private IEnumerator GetPlugEventInfoResult(JsonRpcClient.Request request, string result, List<int> pst, List<int> cmt)
	{
		localUser.articleEvent = pst;
		localUser.commentEvent = cmt;
		yield break;
	}

	private IEnumerator GetPlugEventInfoError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8301", true, true)]
	public void GetPostEventReward(int bId)
	{
	}

	private IEnumerator GetPostEventRewardResult(JsonRpcClient.Request request, string result)
	{
		yield break;
	}

	private IEnumerator GetPostEventRewardError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8302", true, true)]
	public void GetCommentEventReward(int pId)
	{
	}

	private IEnumerator GetCommentEventRewardResult(JsonRpcClient.Request request, string result)
	{
		yield break;
	}

	private IEnumerator GetCommentEventRewardError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2310", true, true)]
	public void GetEventBattleGachaInfo(int eidx)
	{
	}

	private IEnumerator GetEventBattleGachaInfoResult(JsonRpcClient.Request request, Protocols.EventBattleGachaInfo result)
	{
		int.Parse(_FindRequestProperty(request, "eidx"));
		if (result != null && UIManager.instance.world != null)
		{
			UIManager.instance.world.eventBattle.SetEventGachaData(result);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2311", true, true)]
	public void EventBattleGachaOpen(int eidx, int cnt)
	{
	}

	private IEnumerator EventBattleGachaOpenResult(JsonRpcClient.Request request, Protocols.EventBattleGachaOpen result)
	{
		localUser.RefreshGoodsFromNetwork(result.resource);
		localUser.RefreshPartFromNetwork(result.partData);
		localUser.RefreshItemFromNetwork(result.foodData);
		localUser.RefreshItemFromNetwork(result.eventResourceData);
		localUser.RefreshItemFromNetwork(result.itemData);
		localUser.RefreshMedalFromNetwork(result.medalData);
		localUser.AddCommanderFromNetwork(result.commanderData);
		localUser.RefreshUserEquipItemFromNetwork(result.equipItem);
		Protocols.EventBattleGachaInfo infoData = new Protocols.EventBattleGachaInfo
		{
			reset = result.reset,
			season = result.season,
			info = result.info
		};
		yield return UIManager.instance.world.eventBattle.OpenGacha(infoData);
		UIPopup.Create<UIGetItem>("GetItem").Set(result.rewardList, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		UIManager.instance.RefreshOpenedUI();
	}

	private IEnumerator EventBattleGachaOpenError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 70210)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("6600"));
		}
		else
		{
			StartCoroutine(OnJsonRpcRequestError(request, result, code));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "2312", true, true)]
	public void EventBattleGachaReset(int eidx)
	{
	}

	private IEnumerator EventBattleGachaResetResult(JsonRpcClient.Request request, Protocols.EventBattleGachaInfo result)
	{
		int.Parse(_FindRequestProperty(request, "eidx"));
		if (result != null && UIManager.instance.world != null)
		{
			UIManager.instance.world.eventBattle.SetEventGachaData(result);
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("11000004"));
		}
		yield break;
	}

	private IEnumerator EventBattleGachaResetError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 70210)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("6600"));
		}
		else
		{
			StartCoroutine(OnJsonRpcRequestError(request, result, code));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3231", true, true)]
	public void EventBattleStart(int type, JObject deck, JObject gdp, int ucash, int eidx, int efid, int np)
	{
	}

	private IEnumerator EventBattleStartResult(JsonRpcClient.Request request, string result, List<Protocols.RewardInfo.RewardData> reward)
	{
		BattleData battleData = BattleData.Get();
		battleData.rewardItems = reward;
		BattleData.Set(battleData);
		Loading.Load(Loading.Battle);
		yield break;
	}

	private IEnumerator EventBattleStartError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 70201:
			if (UIManager.instance.world != null)
			{
				UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, Localization.Get("6601"), Localization.Get("1001"));
				uISimplePopup.onClose = delegate
				{
					if (UIManager.instance.world.readyBattle.isActive)
					{
						UIManager.instance.world.readyBattle.CloseAnimation();
					}
				};
			}
			if (UIManager.instance.battle != null && GameSetting.instance.repeatBattle)
			{
				GameSetting.instance.repeatBattle = false;
				UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, Localization.Get("10000010"), Localization.Get("1001"));
			}
			break;
		case 20002:
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Format("110367", code), null, "1001");
			RequestGetUserInformation(Protocols.UserInformationType.Resource);
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3231", true, true)]
	public void EventRaidBattleStart(int type, JObject deck, int mbid)
	{
	}

	private IEnumerator EventRaidBattleStartResult(JsonRpcClient.Request request, string result)
	{
		Loading.Load(Loading.Battle);
		yield break;
	}

	private IEnumerator EventRaidBattleStartError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 70201:
		{
			UISimplePopup uISimplePopup2 = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, Localization.Get("6601"), Localization.Get("1001"));
			uISimplePopup2.onClose = delegate
			{
				if (UIManager.instance.world.readyBattle.isActive)
				{
					UIManager.instance.world.readyBattle.CloseAnimation();
				}
			};
			break;
		}
		case 70202:
		case 70203:
		{
			UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, Localization.Get("6606"), Localization.Get("1001"));
			uISimplePopup.onClose = delegate
			{
				if (UIManager.instance.world.readyBattle.isActive)
				{
					UIManager.instance.world.readyBattle.CloseAnimation();
					UIManager.instance.world.eventBattle.RefreshEventRaid();
				}
			};
			break;
		}
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8120", true, true)]
	public void GetEventRemaingTime()
	{
	}

	private IEnumerator GetEventRemaingTimeResult(JsonRpcClient.Request request, string result, Dictionary<string, int> buff)
	{
		if (result != null && buff.Count > 0)
		{
			localUser.eventRemaingTime.Clear();
			List<string> list = new List<string>(buff.Keys);
			for (int i = 0; i < list.Count; i++)
			{
				string key = list[i];
				int num = buff[key];
				TimeData timeData = TimeData.Create();
				timeData.SetByDuration(num);
				localUser.eventRemaingTime.Add(key, timeData);
			}
		}
		yield break;
	}

	private IEnumerator GetEventRemaingTimeError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "5222", true, true)]
	public void BuyPredeckSlot()
	{
	}

	private IEnumerator BuyPredeckSlotResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		if (result != null)
		{
			localUser.RefreshGoodsFromNetwork(result.goodsInfo);
			localUser.statistics.predeckCount = result.battleStatisticsInfo.predeckCount;
			localUser.RefreshPreDeckFromNetwork(result.preDeck);
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator BuyPredeckSlotError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3601", true, true)]
	public void WorldDuelInformation()
	{
	}

	private IEnumerator WorldDuelInformationResult(JsonRpcClient.Request request, Protocols.WorldDuelInformation result)
	{
		if (result != null)
		{
			localUser.currentSeasonDuelTime.SetByDuration(result.resetTime);
			localUser.RefreshDefenderTroop(result.deck);
			localUser.RefreshGoodsFromNetwork(result.resource);
			localUser.worldDuelBattleEnable = result.open;
			localUser.worldDuelBuff = result.duelBuff;
			localUser.RefreshWorldDuelActiveBuffFromNetwork(result.activeBuff);
			if (duelRankingList == null)
			{
				duelRankingList = new List<RoUser>();
			}
			duelRankingList.Clear();
			for (int i = 0; i < result.rankingList.Count; i++)
			{
				RoUser item = RoUser.CreateRankListUser(EBattleType.WorldDuel, result.rankingList[i]);
				duelRankingList.Add(item);
			}
			if (result.bestRank.world != 0)
			{
				localUser.worldDuelBestRank = RoUser.CreateRankListUser(result.bestRank);
			}
			else
			{
				localUser.worldDuelBestRank = null;
			}
			if (result.retryInfo.uno != 0)
			{
				localUser.worldDuelReMatchTarget = RoUser.CreateDuelListUser(EBattleType.WorldDuel, result.retryInfo);
			}
			else
			{
				localUser.worldDuelReMatchTarget = null;
			}
			localUser.worldDuelRanking = result.user.ranking;
			localUser.worldDuelScore = result.user.score;
			localUser.worldWinRank = result.user.winRank;
			localUser.worldWinRankIdx = result.user.winRankIdx;
			localUser.worldWinCount = result.user.winCnt;
			localUser.worldLoseCount = result.user.loseCnt;
			localUser.duelGradeIdx = localUser.GetWorldDuelRankGrade();
			UIManager.instance.world.rankingBattle.InitAndOpen(EBattleType.WorldDuel);
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator WorldDuelInformationError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3603", true, true)]
	public void WorldDuelDefenderSetting(JObject deck)
	{
	}

	private IEnumerator WorldDuelDefenderSettingResult(JsonRpcClient.Request request, string result)
	{
		if (!string.IsNullOrEmpty(result))
		{
			Dictionary<string, string> source = JsonConvert.DeserializeObject<Dictionary<string, string>>(_FindRequestProperty(request, "deck"));
			localUser.RefreshDefenderTroop(source);
			UIManager.instance.world.readyBattle.CloseAnimation();
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator WorldDuelDefenderSettingError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3610", true, true)]
	public void WorldDuelBuffSetting(string bbf)
	{
	}

	private IEnumerator WorldDuelBuffSettingResult(JsonRpcClient.Request request, string result)
	{
		if (result == "true" || result == "True")
		{
			string text = _FindRequestProperty(request, "bbf");
			string value = text.Substring(0, 3);
			string value2 = text.Substring(3);
			EWorldDuelBuff key = (EWorldDuelBuff)Enum.Parse(typeof(EWorldDuelBuff), value);
			EWorldDuelBuffEffect value3 = (EWorldDuelBuffEffect)Enum.Parse(typeof(EWorldDuelBuffEffect), value2);
			localUser.activeBuff[key] = value3;
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator WorldDuelBuffSettingError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3605", true, true)]
	public void WorldDuelBuffUpgrade(string type)
	{
	}

	private IEnumerator WorldDuelBuffUpgradeResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.Resource rsoc, Dictionary<string, int> buff)
	{
		if (!string.IsNullOrEmpty(result))
		{
			localUser.RefreshGoodsFromNetwork(rsoc);
			localUser.RefreshWorldDuelBuffFromNetwork(buff);
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator WorldDuelBuffUpgradeError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3606", true, true)]
	public void WorldDuelEnemyInfo()
	{
	}

	private IEnumerator WorldDuelEnemyInfoResult(JsonRpcClient.Request request, Protocols.PvPDuelList.PvPDuelData result)
	{
		if (result != null)
		{
			localUser.worldDuelTarget = RoUser.CreateDuelListUser(EBattleType.WorldDuel, result);
			UIManager.instance.world.rankingBattle.OpenWorldDuelReadyBattle();
		}
		yield break;
	}

	private IEnumerator WorldDuelEnemyInfoError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "4313", true, true)]
	public void TranscendenceSkillUp(int cid, int slot)
	{
	}

	private IEnumerator TranscendenceSkillUpResult(JsonRpcClient.Request request, Protocols.UserInformationResponse result)
	{
		localUser.FromNetwork(result);
		UIManager.instance.RefreshOpenedUI();
		yield return null;
		int slot = int.Parse(_FindRequestProperty(request, "slot"));
		UITranscendencePopup obj = UnityEngine.Object.FindObjectOfType(typeof(UITranscendencePopup)) as UITranscendencePopup;
		if (obj != null)
		{
			obj.Set(slot);
		}
	}

	private IEnumerator TranscendenceSkillUpError(JsonRpcClient.Request request, string result, int code)
	{
		NetworkAnimation.Instance.CreateFloatingText("Error code:" + code);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8501", true, true)]
	public void GetWeaponProgressList()
	{
	}

	private IEnumerator GetWeaponProgressListResult(JsonRpcClient.Request request, List<Protocols.WeaponProgressSlotData> result)
	{
		if (!UIManager.instance.world.existWeaponResearch || !UIManager.instance.world.weaponResearch.isActive)
		{
			UIManager.instance.world.weaponResearch.InitAndOpenWeaponResearch();
		}
		UIManager.instance.world.weaponResearch.SelectTabContents();
		UIManager.instance.world.weaponResearch.SetWeaponProgressData(result);
		yield break;
	}

	private IEnumerator GetWeaponProgressListError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8500", true, true)]
	public void StartWeaponProgress(int slot, int wmat1, int wmat2, int wmat3, int wmat4)
	{
	}

	private IEnumerator StartWeaponProgressResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.Resource rsoc, int slot, int remain)
	{
		UIManager.instance.world.weaponResearch.inProgress.StartProgress(slot, remain);
		localUser.RefreshGoodsFromNetwork(rsoc);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator StartWeaponProgressError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8502", true, true)]
	public void WeaponProgressSlotOpen()
	{
	}

	private IEnumerator WeaponProgressSlotOpenResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.BattleStatistics uifo, Protocols.UserInformationResponse.Resource rsoc)
	{
		localUser.statistics.weaponMakeSlotCount = uifo.weaponMakeSlotCount;
		localUser.RefreshGoodsFromNetwork(rsoc);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator WeaponProgressSlotOpenError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8503", true, true)]
	public void WeaponProgressFinish(int slot)
	{
	}

	private IEnumerator WeaponProgressFinishResult(JsonRpcClient.Request request, string result, Dictionary<string, Protocols.WeaponData> weapon, List<Protocols.RewardInfo.RewardData> reward)
	{
		int slot = int.Parse(_FindRequestProperty(request, "slot"));
		localUser.RefreshWeaponFromNetwork(weapon);
		UIPopup.Create<UIGetItem>("GetItem").Set(reward, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		UIManager.instance.world.weaponResearch.inProgress.ResetProgress(slot);
		yield break;
	}

	private IEnumerator WeaponProgressFinishError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8504", true, true)]
	public void WeaponProgressBuyImmediateTicket(int slot)
	{
	}

	private IEnumerator WeaponProgressBuyImmediateTicketResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.Resource rsoc, Dictionary<string, Protocols.WeaponData> weapon, List<Protocols.RewardInfo.RewardData> reward)
	{
		int slot = int.Parse(_FindRequestProperty(request, "slot"));
		localUser.RefreshWeaponFromNetwork(weapon);
		localUser.RefreshGoodsFromNetwork(rsoc);
		UIPopup.Create<UIGetItem>("GetItem").Set(reward, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		UIManager.instance.world.weaponResearch.inProgress.ResetProgress(slot);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator WeaponProgressBuyImmediateTicketError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8505", true, true)]
	public void WeaponProgressUseImmediateTicket(int slot)
	{
	}

	private IEnumerator WeaponProgressUseImmediateTicketResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.Resource rsoc, Dictionary<string, Protocols.WeaponData> weapon, List<Protocols.RewardInfo.RewardData> reward)
	{
		int slot = int.Parse(_FindRequestProperty(request, "slot"));
		localUser.RefreshWeaponFromNetwork(weapon);
		localUser.RefreshGoodsFromNetwork(rsoc);
		UIPopup.Create<UIGetItem>("GetItem").Set(reward, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		UIManager.instance.world.weaponResearch.inProgress.ResetProgress(slot);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator WeaponProgressUseImmediateTicketError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8506", true, true)]
	public void EquipWeapon(int cid, int wno)
	{
	}

	private IEnumerator EquipWeaponResult(JsonRpcClient.Request request, string result, Dictionary<string, Protocols.WeaponData> weapon)
	{
		foreach (KeyValuePair<string, Protocols.WeaponData> item in weapon)
		{
			RoWeapon roWeapon = localUser.FindWeapon(item.Key);
			RoCommander roCommander;
			if (roWeapon.currEquipCommanderId != 0)
			{
				roCommander = localUser.FindCommander(roWeapon.currEquipCommanderId.ToString());
				roCommander.RemoveWeaponItem(roWeapon.data.slotType);
			}
			roCommander = localUser.FindCommander(item.Value.cid.ToString());
			if (roCommander != null)
			{
				roCommander.EquipWeaponItem(roWeapon);
				if (roCommander.EnableWeaponSet())
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70093"));
				}
			}
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator EquipWeaponError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8507", true, true)]
	public void ReleaseWeapon(int cid, int wno)
	{
	}

	private IEnumerator ReleaseWeaponResult(JsonRpcClient.Request request, string result, Dictionary<string, Protocols.WeaponData> weapon)
	{
		foreach (KeyValuePair<string, Protocols.WeaponData> item in weapon)
		{
			RoWeapon roWeapon = localUser.FindWeapon(item.Key);
			RoCommander roCommander = localUser.FindCommander(roWeapon.currEquipCommanderId.ToString());
			bool flag = roCommander.EnableWeaponSet();
			roCommander.RemoveWeaponItem(roWeapon.data.slotType);
			if (flag && !roCommander.EnableWeaponSet())
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70094"));
			}
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator ReleaseWeaponError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8509", true, true)]
	public void DecompositionWeapon(List<int> wnos)
	{
	}

	private IEnumerator DecompositionWeaponResult(JsonRpcClient.Request request, string result, Dictionary<string, int> part, List<Protocols.RewardInfo.RewardData> reward, List<string> wnos)
	{
		localUser.RefreshPartFromNetwork(part);
		localUser.RemoveWeapon(wnos);
		UIManager.instance.world.weaponResearch.weaponList.ResetDecompositionList();
		UIPopup.Create<UIGetItem>("GetItem").Set(reward, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator DecompositionWeaponError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8508", true, true)]
	public void UpgradeWeapon(int wno)
	{
	}

	private IEnumerator UpgradeWeaponResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.Resource rsoc, Dictionary<string, int> part, Dictionary<string, Protocols.WeaponData> weapon)
	{
		localUser.RefreshGoodsFromNetwork(rsoc);
		localUser.RefreshPartFromNetwork(part);
		localUser.RefreshWeaponFromNetwork(weapon);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator UpgradeWeaponError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8513", true, true)]
	public void UpgradeWeaponInventory()
	{
	}

	private IEnumerator UpgradeWeaponInventoryResult(JsonRpcClient.Request request, string result, Protocols.UserInformationResponse.BattleStatistics uifo, Protocols.UserInformationResponse.Resource rsoc)
	{
		localUser.statistics.weaponInventoryCount = uifo.weaponInventoryCount;
		localUser.RefreshGoodsFromNetwork(rsoc);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator UpgradeWeaponInventoryError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8512", true, true)]
	public void ComposeWeaponBox(int gidx, int amnt)
	{
	}

	private IEnumerator ComposeWeaponBoxResult(JsonRpcClient.Request request, string result, Dictionary<string, int> item, List<Protocols.RewardInfo.RewardData> reward)
	{
		localUser.RefreshItemFromNetwork(item);
		UIPopup.Create<UIGetItem>("GetItem").Set(reward, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator ComposeWeaponBoxError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8514", true, true)]
	public void TradeWeaponUpgradeTicket(int tidx, int amnt)
	{
	}

	private IEnumerator TradeWeaponUpgradeTicketResult(JsonRpcClient.Request request, string result, Dictionary<string, int> part, List<Protocols.RewardInfo.RewardData> reward)
	{
		localUser.RefreshPartFromNetwork(part);
		UIPopup.Create<UIGetItem>("GetItem").Set(reward, string.Empty);
		SoundManager.PlaySFX("SE_ItemGet_001");
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator TradeWeaponUpgradeTicketError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8520", true, true)]
	public void GetWeaponProgressHistory(int type)
	{
	}

	private IEnumerator GetWeaponProgressHistoryResult(JsonRpcClient.Request request, string result, List<List<string>> recp)
	{
		int key = int.Parse(_FindRequestProperty(request, "type"));
		if (!localUser.weaponHistory.ContainsKey(key))
		{
			localUser.weaponHistory.Add(key, recp);
		}
		if (UIManager.instance.world.weaponResearch.inProgress.historyPopup == null)
		{
			UIManager.instance.world.weaponResearch.inProgress.historyPopup = UIPopup.Create<UIWeaponProgressHistoryPopup>("WeaponProgressHistoryPopup");
		}
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	private IEnumerator GetWeaponProgressHistoryError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8700", true, true)]
	public void InfinityBattleInformation(int ifid, string retryStage)
	{
	}

	private IEnumerator InfinityBattleInformationResult(JsonRpcClient.Request request, Protocols.InfinityTowerInformation result)
	{
		if (localUser.infinityStageList == null)
		{
			localUser.infinityStageList = new Dictionary<string, Dictionary<int, int>>();
		}
		localUser.infinityStageList.Clear();
		int num = regulation.infinityFieldDtbl.length - 1;
		while (0 <= num)
		{
			InfinityFieldDataRow infinityFieldDataRow = regulation.infinityFieldDtbl[num];
			Dictionary<int, int> value = new Dictionary<int, int>();
			if (result.infinityData.fieldData.ContainsKey(infinityFieldDataRow.infinityFieldIdx))
			{
				value = result.infinityData.fieldData[infinityFieldDataRow.infinityFieldIdx];
			}
			localUser.infinityStageList.Add(infinityFieldDataRow.infinityFieldIdx, value);
			num--;
		}
		result.retryStage = _FindRequestProperty(request, "retryStage");
		UIManager.instance.world.infinityBattle.Init();
		UIManager.instance.world.infinityBattle.SetData(result);
		yield break;
	}

	private IEnumerator InfinityBattleInformationError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8701", true, true)]
	public void GetInfinityBattleDeck()
	{
	}

	private IEnumerator GetInfinityBattleDeckResult(JsonRpcClient.Request request, string result, JObject deck)
	{
		if (deck != null)
		{
			PlayerPrefs.SetString("InfinityBattleDeck", JObject.FromObject(deck).ToString());
		}
		yield break;
	}

	private IEnumerator GetInfinityBattleDeckError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8702", true, true)]
	public void SaveInfinityBattleDeck(JObject deck)
	{
	}

	private IEnumerator SaveInfinityBattleDeckResult(JsonRpcClient.Request request, string result)
	{
		string value = _FindRequestProperty(request, "deck");
		PlayerPrefs.SetString("InfinityBattleDeck", value);
		yield break;
	}

	private IEnumerator SaveInfinityBattleDeckError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8703", true, true)]
	public void StartInfinityBattleScenario(int ifid)
	{
	}

	private IEnumerator StartInfinityBattleScenarioResult(JsonRpcClient.Request request, string result)
	{
		if (!string.IsNullOrEmpty(result))
		{
			string key = _FindRequestProperty(request, "ifid");
			InfinityFieldDataRow infinityFieldDataRow = regulation.infinityFieldDtbl[key];
			localUser.currScenario.scenarioId = infinityFieldDataRow.scenarioIdx;
			localUser.currScenario.commanderId = 0;
			Loading.Load(Loading.Scenario);
		}
		yield break;
	}

	private IEnumerator StartInfinityBattleScenarioError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8704", true, true)]
	public void InfinityBattleGetReward(int ifid, int msid)
	{
	}

	private IEnumerator InfinityBattleGetRewardResult(JsonRpcClient.Request request, Protocols.InfinityTowerReward result)
	{
		if (result != null)
		{
			UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
			SoundManager.PlaySFX("SE_ItemGet_001");
			localUser.RefreshRewardFromNetwork(result);
			UIManager.instance.world.infinityBattle.UpdateInfinityBattleData(string.Empty, result.fieldData);
			UIManager.instance.RefreshOpenedUI();
		}
		yield break;
	}

	private IEnumerator InfinityBattleGetRewardError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "3231", true, true)]
	public void InfinityBattleStart(int type, int ifid, JObject deck)
	{
	}

	private IEnumerator InfinityBattleStartResult(JsonRpcClient.Request request, string result)
	{
		BattleData battleData = BattleData.Get();
		BattleData.Set(battleData);
		Loading.Load(Loading.Battle);
		yield break;
	}

	private IEnumerator InfinityBattleStartError(JsonRpcClient.Request request, string result, int code)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8600", true, true)]
	public void GetDormitoryInfo(List<string> type)
	{
	}

	private IEnumerator GetDormitoryInfoResult(JsonRpcClient.Request request, Protocols.Dormitory.Info result)
	{
		localUser.dormitory.Set(result);
		localUser.RefreshGoodsFromNetwork(result.resource);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8601", true, true)]
	public void GetDormitoryFloorInfo()
	{
	}

	private IEnumerator GetDormitoryFloorInfoResult(JsonRpcClient.Request request, Protocols.Dormitory.FloorInfo result)
	{
		result.isMasterUser = true;
		UIPopup.Create<UIRoomList>("RoomList").Set(result);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8670", true, true)]
	public void GetDormitoryUserFloorInfo(string tuno)
	{
	}

	private IEnumerator GetDormitoryUserFloorInfoResult(JsonRpcClient.Request request, Protocols.Dormitory.GetUserFloorInfoResponse result)
	{
		result.isMasterUser = false;
		UIPopup.Create<UIRoomList>("RoomList").Set(result);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8602", true, true)]
	public void ConstructDormitoryFloor(string fno)
	{
	}

	private IEnumerator ConstructDormitoryFloorResult(JsonRpcClient.Request request, Protocols.Dormitory.ConstructFloorResponse result)
	{
		localUser.RefreshGoodsFromNetwork(result.resource);
		Message.Send("Update.Goods");
		Message.Send("Room.Update.Build", result);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8603", true, true)]
	public void FinishConstructDormitoryFloor(string fno, int imm)
	{
	}

	private IEnumerator FinishConstructDormitoryFloorResult(JsonRpcClient.Request request, Protocols.Dormitory.FinishConstructFloorResponse result)
	{
		localUser.RefreshGoodsFromNetwork(result.resource);
		Message.Send("Update.Goods");
		Message.Send("Room.Update.Build", result);
		UIManager.instance.RefreshOpenedUI();
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8620", true, true)]
	public void GetDormitoryFloorDetailInfo(string fno)
	{
	}

	private IEnumerator GetDormitoryFloorDetailInfoResult(JsonRpcClient.Request request, Protocols.Dormitory.FloorDetailInfo result)
	{
		DormitoryInitData.Instance.Set(result);
		Loading.Load(Loading.Dormitory);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8671", true, true)]
	public void GetDormitoryUserFloorDetailInfo(string tuno, string fno)
	{
	}

	private IEnumerator GetDormitoryUserFloorDetailInfoResult(JsonRpcClient.Request request, Protocols.Dormitory.GetUserFloorDetailInfoResponse result)
	{
		DormitoryInitData.Instance.Set(result);
		Loading.Load(Loading.Dormitory);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8604", true, true)]
	public void ChangeDormitoryFloorName(string fno, string fnm)
	{
	}

	private IEnumerator ChangeDormitoryFloorNameResult(JsonRpcClient.Request request, Protocols.Dormitory.ChangeDormitoryFloorNameResponse result)
	{
		SingletonMonoBehaviour<DormitoryData>.Instance.room.name = result.name;
		localUser.RefreshGoodsFromNetwork(result.resource);
		Message.Send("Room.Update.Name");
		Message.Send("Update.Goods");
		yield break;
	}

	private IEnumerator ChangeDormitoryFloorNameResultError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 85102:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7054"));
			break;
		case 85101:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81015"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8610", true, true)]
	public void GetDormitoryShopProductList()
	{
	}

	private IEnumerator GetDormitoryShopProductListResult(JsonRpcClient.Request request, Protocols.Dormitory.ShopInfo result)
	{
		UIPopup.Create<UIDormitoryShop>("DormitoryShop").Set(result);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8611", true, true)]
	public void BuyDormitoryShopProduct(EDormitoryItemType styp, string sidx)
	{
	}

	private IEnumerator BuyDormitoryShopProductResult(JsonRpcClient.Request request, Protocols.Dormitory.BuyShopProductResponse result)
	{
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		localUser.RefreshRewardFromNetwork(result);
		Message.Send("Update.Goods");
		Message.Send("Inven.Update");
		Message.Send("Shop.Update", result);
		yield break;
	}

	private IEnumerator BuyDormitoryShopProductError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 85120:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81044"));
			break;
		case 85121:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81061"));
			break;
		case 85123:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81042"));
			break;
		case 85124:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81043"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8612", true, true)]
	public void SellDormitoryItem(EStorageType ityp, string tidx, int amnt)
	{
	}

	private IEnumerator SellDormitoryItemResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		localUser.RefreshRewardFromNetwork(result);
		Message.Send("Update.Goods");
		Message.Send("Inven.Update");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8621", true, true)]
	public void ChangeDormitoryWallpaper(string fno, string idx)
	{
	}

	private IEnumerator ChangeDormitoryWallpaperResult(JsonRpcClient.Request request, Protocols.Dormitory.ChangeWallpaperResponse result)
	{
		SingletonMonoBehaviour<DormitoryData>.Instance.room.wallpaper = result.id;
		SingletonMonoBehaviour<DormitoryData>.Instance.inventory.UpdateData(EDormitoryItemType.Wallpaper, result.invenWallpaper);
		Message.Send("Inven.Update");
		Message.Send("Room.Update.WallPaper");
		yield break;
	}

	private IEnumerator ChangeDormitoryWallpaperError(JsonRpcClient.Request request, string result, int code)
	{
		if (code != 85111)
		{
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8630", true, true)]
	public void ArrangeDormitoryDecoration(string fno, string idx, int px, int py, int rt)
	{
	}

	private IEnumerator ArrangeDormitoryDecorationResult(JsonRpcClient.Request request, Protocols.Dormitory.ArrangeDecorationResponse result)
	{
		SingletonMonoBehaviour<DormitoryData>.Instance.dormitory.UpdateInvenData(EDormitoryItemType.Normal, result.invenNormal);
		SingletonMonoBehaviour<DormitoryData>.Instance.dormitory.UpdateInvenData(EDormitoryItemType.Advanced, result.invenAdvanced);
		Message.Send<string>("Inven.Update");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8631", true, true)]
	public void EditDormitoryDecoration(string fno, int px, int py, int epx, int epy, int ert)
	{
	}

	private IEnumerator EditDormitoryDecorationResult(JsonRpcClient.Request request, string result)
	{
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8632", true, true)]
	public void RemoveDormitoryDecoration(string fno, int px, int py)
	{
	}

	private IEnumerator RemoveDormitoryDecorationResult(JsonRpcClient.Request request, Protocols.Dormitory.ArrangeDecorationResponse result)
	{
		SingletonMonoBehaviour<DormitoryData>.Instance.dormitory.UpdateInvenData(EDormitoryItemType.Normal, result.invenNormal);
		SingletonMonoBehaviour<DormitoryData>.Instance.dormitory.UpdateInvenData(EDormitoryItemType.Advanced, result.invenAdvanced);
		Message.Send<string>("Inven.Update");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8640", true, true)]
	public void GetDormitoryCommanderInfo()
	{
	}

	private IEnumerator GetDormitoryCommanderInfoResult(JsonRpcClient.Request request, Protocols.Dormitory.GetDormitoryCommanderInfoResponse result)
	{
		localUser.dormitory.Set(result.commanderData);
		if (result.headData != null)
		{
			localUser.dormitory.UpdateHeadData(result.headData);
		}
		Message.Send("Chr.Get");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8641", true, true)]
	public void ArrangeDormitoryCommander(string fno, string cid)
	{
	}

	private IEnumerator ArrangeDormitoryCommanderResult(JsonRpcClient.Request request, string result, Dictionary<string, Protocols.Dormitory.CommanderInfo> dcom, Dictionary<string, Protocols.Dormitory.FloorCharacterInfo> fcm)
	{
		Dictionary<string, Protocols.Dormitory.CommanderInfo>.Enumerator enumerator = dcom.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SingletonMonoBehaviour<DormitoryData>.Instance.characters[enumerator.Current.Key].fno = enumerator.Current.Value.fno;
			Message.Send("Chr.Update.Floor", enumerator.Current.Key);
		}
		Dictionary<string, Protocols.Dormitory.FloorCharacterInfo>.Enumerator enumerator2 = fcm.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			RoCharacter roCharacter = SingletonMonoBehaviour<DormitoryData>.Instance.characters[enumerator.Current.Key];
			roCharacter.Set(enumerator2.Current.Value);
			SingletonMonoBehaviour<DormitoryData>.Instance.room.AddCharacter(roCharacter);
			Message.Send("Room.Add.Character", roCharacter);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8642", true, true)]
	public void RemoveDormitoryCommander(string cid)
	{
	}

	private IEnumerator RemoveDormitoryCommanderResult(JsonRpcClient.Request request, string result, Dictionary<string, Protocols.Dormitory.CommanderInfo> dcom)
	{
		Dictionary<string, Protocols.Dormitory.CommanderInfo>.Enumerator enumerator = dcom.GetEnumerator();
		while (enumerator.MoveNext())
		{
			RoCharacter roCharacter = SingletonMonoBehaviour<DormitoryData>.Instance.characters[enumerator.Current.Key];
			roCharacter.fno = enumerator.Current.Value.fno;
			Message.Send("Chr.Update.Floor", enumerator.Current.Key);
			if (SingletonMonoBehaviour<DormitoryData>.Instance.room.ContainsCharacter(roCharacter.id))
			{
				SingletonMonoBehaviour<DormitoryData>.Instance.room.RemoveCharacter(roCharacter.id);
				Message.Send("Room.Remove.Character", roCharacter.id);
			}
		}
		yield break;
	}

	private IEnumerator RemoveDormitoryCommanderError(JsonRpcClient.Request request, string result, int code)
	{
		if (code == 85144)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81031"));
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8614", true, true)]
	public void BuyDormitoryHeadCostume(string idx)
	{
	}

	private IEnumerator BuyDormitoryHeadCostumeResult(JsonRpcClient.Request request, Protocols.RewardInfo result)
	{
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		localUser.RefreshRewardFromNetwork(result);
		Message.Send("Update.Goods");
		Message.Send("Inven.Update");
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8643", true, true)]
	public void ChangeDormitoryCommanderHead(string cid, string idx)
	{
	}

	private IEnumerator ChangeDormitoryCommanderHeadResult(JsonRpcClient.Request request, Protocols.Dormitory.ChangeCommanderHeadResponse result)
	{
		Dictionary<string, Protocols.Dormitory.CommanderHeadData>.Enumerator enumerator = result.headData.GetEnumerator();
		while (enumerator.MoveNext())
		{
			string key = enumerator.Current.Key;
			SingletonMonoBehaviour<DormitoryData>.Instance.characters[key].head.id = enumerator.Current.Value.headId;
			Message.Send("Chr.Update.Costume", key);
		}
		yield break;
	}

	private IEnumerator ChangeDormitoryCommanderError(JsonRpcClient.Request request, string result, int code)
	{
		if (code != 85150)
		{
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8644", true, true)]
	public void ChangeDormitoryCommanderBody(string cid, string idx)
	{
	}

	private IEnumerator ChangeDormitoryCommanderBodyResult(JsonRpcClient.Request request, Protocols.Dormitory.ChangeCommanderBodyResponse result)
	{
		SingletonMonoBehaviour<DormitoryData>.Instance.dormitory.UpdateInvenData(EDormitoryItemType.CostumeBody, result.invenBody);
		Dictionary<string, Protocols.Dormitory.CommanderBodyData>.Enumerator enumerator = result.bodyData.GetEnumerator();
		while (enumerator.MoveNext())
		{
			string key = enumerator.Current.Key;
			SingletonMonoBehaviour<DormitoryData>.Instance.characters[key].body.id = enumerator.Current.Value.bodyId;
			Message.Send("Chr.Update.Costume", key);
		}
		yield break;
	}

	private IEnumerator ChangeDormitoryCommanderBodyError(JsonRpcClient.Request request, string result, int code)
	{
		if (code != 85153)
		{
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8650", true, true)]
	public void GetDormitoryPoint(string cid)
	{
	}

	private IEnumerator GetDormitoryPointResult(JsonRpcClient.Request request, Protocols.Dormitory.GetPointResponse result)
	{
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		localUser.RefreshGoodsFromNetwork(result.resource);
		Message.Send("Update.Goods");
		Dictionary<string, Protocols.Dormitory.CommanderRaminData>.Enumerator enumerator = result.reaminData.GetEnumerator();
		while (enumerator.MoveNext())
		{
			string key = enumerator.Current.Key;
			SingletonMonoBehaviour<DormitoryData>.Instance.characters[key].remain.SetByDuration(enumerator.Current.Value.remain);
			Message.Send("Chr.Update.RewardRemain", key);
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8651", true, true)]
	public void GetDormitoryPointAll(string fno)
	{
	}

	private IEnumerator GetDormitoryPointAllResult(JsonRpcClient.Request request, Protocols.Dormitory.GetPointAllResponse result)
	{
		UIPopup.Create<UIGetItem>("GetItem").Set(result.reward, string.Empty);
		localUser.RefreshGoodsFromNetwork(result.resource);
		Message.Send("Update.Goods");
		Message.Send("Room.Update.PointState", result.pointState);
		if (result.reaminData != null)
		{
			Dictionary<string, Protocols.Dormitory.CommanderRaminData>.Enumerator enumerator = result.reaminData.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string key = enumerator.Current.Key;
				SingletonMonoBehaviour<DormitoryData>.Instance.characters[key].remain.SetByDuration(enumerator.Current.Value.remain);
				Message.Send("Chr.Update.RewardRemain", key);
			}
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8665", true, true)]
	public void GetRecommendUser()
	{
	}

	private IEnumerator GetRecommendUserResult(JsonRpcClient.Request request, string result, List<Protocols.Dormitory.SearchUserInfo> slist)
	{
		MessageEvent.Search.Data data = new MessageEvent.Search.Data();
		data.type = EVisitType.Search;
		data.users = slist;
		Message.Send("Search.Get.Users", data);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8660", true, true)]
	public void SearchDormitoryUser(string nick)
	{
	}

	private IEnumerator SearchDormitoryUserResult(JsonRpcClient.Request request, string result, List<Protocols.Dormitory.SearchUserInfo> slist)
	{
		MessageEvent.Search.Data data = new MessageEvent.Search.Data();
		data.type = EVisitType.Search;
		data.users = slist;
		if (slist.Count == 0)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81062"));
		}
		Message.Send("Search.Get.Users", data);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8661", true, true)]
	public void AddDormitoryFavorUser(string tuno)
	{
	}

	private IEnumerator AddDormitoryFavorUserResult(JsonRpcClient.Request request, string result)
	{
		string text = _FindRequestProperty(request, "tuno");
		if (SingletonMonoBehaviour<DormitoryData>.Instance.user.uno == text)
		{
			SingletonMonoBehaviour<DormitoryData>.Instance.favorState = true;
			Message.Send("User.Update.FavorState");
		}
		Message.Send("Favor.Add", text);
		yield break;
	}

	private IEnumerator AddDormitoryFavorUserResultError(JsonRpcClient.Request request, string result, int code)
	{
		switch (code)
		{
		case 85171:
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81065"));
			break;
		}
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8662", true, true)]
	public void RemoveDormitoryFavorUser(string tuno)
	{
	}

	private IEnumerator RemoveDormitoryFavorUserResult(JsonRpcClient.Request request, string result)
	{
		string text = _FindRequestProperty(request, "tuno");
		if (SingletonMonoBehaviour<DormitoryData>.Instance.user.uno == text)
		{
			SingletonMonoBehaviour<DormitoryData>.Instance.favorState = false;
			Message.Send("User.Update.FavorState");
		}
		Message.Send("Favor.Remove", text);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8664", true, true)]
	public void GetDormitoryGuildUser()
	{
	}

	private IEnumerator GetDormitoryGuildUserResult(JsonRpcClient.Request request, string result, List<Protocols.Dormitory.SearchUserInfo> glist)
	{
		MessageEvent.Search.Data data = new MessageEvent.Search.Data();
		data.type = EVisitType.Guild;
		data.users = glist;
		Message.Send("Search.Get.Users", data);
		yield break;
	}

	[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "8663", true, true)]
	public void GetDormitoryFavorUser()
	{
	}

	private IEnumerator GetDormitoryFavorUserResult(JsonRpcClient.Request request, string result, List<Protocols.Dormitory.SearchUserInfo> ulist)
	{
		MessageEvent.Search.Data data = new MessageEvent.Search.Data();
		data.type = EVisitType.Favorites;
		data.users = ulist;
		Message.Send("Search.Get.Users", data);
		yield break;
	}
}
