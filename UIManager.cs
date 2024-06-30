using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Cache;
using Shared.Battle;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public enum EState
	{
		World,
		Battle,
		Tutorial,
		Scenario,
		Editor
	}

	public delegate void LinkOpenVoidDelegate();

	public delegate void LinkOpenStringDelegate(string initData);

	private class LinkData
	{
		public LinkOpenVoidDelegate voidDelegate;

		public LinkOpenStringDelegate stringDelegate;

		public object target
		{
			get
			{
				if (voidDelegate != null)
				{
					return voidDelegate.Target;
				}
				if (stringDelegate != null)
				{
					return stringDelegate.Target;
				}
				return null;
			}
		}

		public static LinkData Create(LinkOpenVoidDelegate del)
		{
			LinkData linkData = new LinkData();
			linkData.voidDelegate = del;
			return linkData;
		}

		public static LinkData Create(LinkOpenStringDelegate del)
		{
			LinkData linkData = new LinkData();
			linkData.stringDelegate = del;
			return linkData;
		}

		public bool Open()
		{
			if (voidDelegate == null)
			{
				return false;
			}
			voidDelegate();
			return true;
		}

		public bool Open(string initData)
		{
			if (stringDelegate == null)
			{
				return false;
			}
			stringDelegate(initData);
			return true;
		}
	}

	public abstract class Group
	{
		public List<UIPanelBase> panelList { get; protected set; }

		public virtual bool isValid { get; protected set; }

		protected abstract void _Init();

		public void CloseAll()
		{
			panelList.ForEach(delegate(UIPanelBase panel)
			{
				UISetter.SetActive(panel, active: false);
			});
		}

		public IEnumerator InitCoroutine()
		{
			_Init();
			panelList = UIPanelBase.FindContainedPanelBase(this, includeParent: false);
			int openIdx = 0;
			int closeIdx = -4;
			while (openIdx < panelList.Count || closeIdx < panelList.Count)
			{
				if (openIdx >= 0 && openIdx < panelList.Count)
				{
					UISetter.SetActive(panelList[openIdx], active: true);
				}
				if (closeIdx >= 0 && closeIdx < panelList.Count)
				{
					UISetter.SetActive(panelList[closeIdx], active: false);
				}
				openIdx++;
				closeIdx++;
				yield return null;
			}
			isValid = true;
		}

		public virtual void Release()
		{
		}
	}

	[Serializable]
	public class World : Group
	{
		private string prefix = "Prefabs/UI/World/";

		private Transform parent = UIRoot.list[0].transform;

		public GameObject noticePopUp;

		public UICamp camp;

		public UIMainCommand mainCommand;

		private UIMetroBank _metroBank;

		public UIReward achievement;

		public UIReward mission;

		private UIWarHome _warHome;

		private UIReward _mail;

		private UIUserDetail _userDetail;

		public UIHeadQuarters _headQuarters;

		private UIShop _shop;

		private UIStorage _storage;

		private UIReadyBattle _readyBattle;

		private UICommanderList _commanderList;

		private UIRankingBattle _rankingBattle;

		private UIWorldMap _worldMap;

		public UIWorldMap worldMap;

		private UILevelUp _levelUp;

		private UIAnnihilationMap _annihilationMap;

		private UISeaRobberSweep _seaRobberSweep;

		private UISituation _situation;

		private UIDailyBonus _dailyBonus;

		public UIDailyBonus dailyBonus;

		private UIGacha _gacha;

		private UICommanderDetail _commnaderDetail;

		private UIUnitUpgradeComplete _unitUpgradeComplete;

		private UIRaid _raid;

		private UIGuild _guild;

		private UIPreDeckSetting _preDeckSetting;

		private ClassicRpgManager _dialogMrg;

		public ClassicRpgManager dialogMrg;

		private UISecretShop _secretShop;

		private UIVipShop _vipShop;

		private UIAlarm _alarm;

		private UIVipGacha _vipGacha;

		private UICarnival _carnival;

		private UIGroup _group;

		private UIConquestMap _conquestMap;

		private UICooperateBattle _cooperateBattle;

		private UIDateMode _dateMode;

		private UIWaveBattle _waveBattle;

		private UILaboratory _laboratory;

		private UIWeaponResearch _weaponResearch;

		private UIEventBattle _eventBattle;

		private UIInfinityBattle _infinityBattle;

		public bool onStart;

		public UIMetroBank metroBank => CreateUIMetroBank();

		public bool existMetroBank => _metroBank != null;

		public UIWarHome warHome => (!(_warHome == null)) ? _warHome : (_warHome = Utility.LoadAndInstantiateGameObject<UIWarHome>(prefix + "Mission_1", parent));

		public bool existWarHome => _warHome != null;

		public UIReward mail => CreateUIReward();

		public UIUserDetail userDetail => CreateUIUserDetail();

		public UIHeadQuarters headQuarters => CreateUIHeadQuarters();

		public bool existHeadQuarters => _headQuarters != null;

		public UIShop shop => CreateUIShop();

		public UIStorage storage => CreateUIStorage();

		public UIReadyBattle readyBattle => CreateUIReadyBattle();

		public bool existReadyBattle => _readyBattle != null;

		public UICommanderList commanderList => CreateUICommanderList();

		public bool existCommanderList => _commanderList != null;

		public UIRankingBattle rankingBattle => CreateUIRankingBattle();

		public bool existRankingBattle => _rankingBattle != null;

		public UILevelUp levelUp => CreateUILevelUp();

		public UIAnnihilationMap annihilationMap => CreateUIAnnihilationMap();

		public bool existAnnihilationMap => _annihilationMap != null;

		public UISeaRobberSweep seaRobberSweep => CreateUISeaRobberSweep();

		public UISituation situation => CreateUISituation();

		public bool existSituation => _situation != null;

		public UIGacha gacha => CreateUIGacha();

		public bool existGacha => _gacha != null;

		public UICommanderDetail commanderDetail => CreateUICommanderDetail();

		public bool existCommanderDetail => _commnaderDetail != null;

		public UIUnitUpgradeComplete unitUpgradeComplete => CreateUIUnitUpgradeComplete();

		public UIRaid raid => CreateUIRaid();

		public bool existRaid => _raid != null;

		public UIGuild guild => CreateUIGuild();

		public UIPreDeckSetting preDeckSetting => CreateUIPreDeckSetting();

		public bool existPreDeckSetting => _preDeckSetting != null;

		public UISecretShop secretShop => CreateUISecretShop();

		public bool existSecretShop => _secretShop != null;

		public UIVipShop vipShop => CreateUIVipShop();

		public bool existvipShop => _vipShop != null;

		public UIAlarm alarm => CreateUIAlarm();

		public UIVipGacha vipGacha => CreateUIVipGacha();

		public UICarnival carnival => CreateUICarnival();

		public bool existCarnival => _carnival != null;

		public UIGroup group => CreateUIGroup();

		public bool existGroup => _group != null;

		public UIConquestMap conquestMap => CreateUIConquestMap();

		public bool existConquestMap => _conquestMap != null;

		public UICooperateBattle cooperateBattle => CreateUICooperateBattle();

		public bool existCooperateBattle => _cooperateBattle != null;

		public UIDateMode dateMode => CreateUIDateMode();

		public bool existDateMode => _dateMode != null;

		public UIWaveBattle waveBattle => CreateUIWaveBattle();

		public bool existWaveBattle => _waveBattle != null;

		public UILaboratory laboratory => CreateUILaboratory();

		public bool existLaboratory => _laboratory != null;

		public UIWeaponResearch weaponResearch => CreateUIWeaponResearch();

		public bool existWeaponResearch => _weaponResearch != null;

		public UIEventBattle eventBattle => CreateUIEventBattle();

		public bool existEventBattle => _eventBattle != null;

		public UIInfinityBattle infinityBattle => CreateUIInfinityBattle();

		public bool existInfinityBattle => _infinityBattle != null;

		private UIMetroBank CreateUIMetroBank()
		{
			if (_metroBank == null)
			{
				_metroBank = Utility.LoadAndInstantiateGameObject<UIMetroBank>(prefix + "MetroBank", parent);
				base.panelList.Add(_metroBank);
			}
			return _metroBank;
		}

		private UIWarHome CreateUIWarHome()
		{
			if (_warHome == null)
			{
				_warHome = Utility.LoadAndInstantiateGameObject<UIWarHome>(prefix + "Mission_1", parent);
				base.panelList.Add(_warHome);
			}
			return _warHome;
		}

		private UIReward CreateUIReward()
		{
			if (_mail == null)
			{
				_mail = Utility.LoadAndInstantiateGameObject<UIReward>(prefix + "Mail", parent);
				base.panelList.Add(_mail);
			}
			return _mail;
		}

		private UIUserDetail CreateUIUserDetail()
		{
			if (_userDetail == null)
			{
				_userDetail = Utility.FindOrCreateGameObject<UIUserDetail>(prefix + "UserDetail", parent);
				base.panelList.Add(_userDetail);
			}
			return _userDetail;
		}

		private UIHeadQuarters CreateUIHeadQuarters()
		{
			if (_headQuarters == null)
			{
				_headQuarters = Utility.FindOrCreateGameObject<UIHeadQuarters>(prefix + "HeadQuarters", parent);
				base.panelList.Add(_headQuarters);
			}
			return _headQuarters;
		}

		private UIShop CreateUIShop()
		{
			if (_shop == null)
			{
				_shop = Utility.FindOrCreateGameObject<UIShop>(prefix + "Shop", parent);
				base.panelList.Add(_shop);
			}
			return _shop;
		}

		private UIStorage CreateUIStorage()
		{
			if (_storage == null)
			{
				_storage = Utility.FindOrCreateGameObject<UIStorage>(prefix + "Storage", parent);
				base.panelList.Add(_storage);
			}
			return _storage;
		}

		private UIReadyBattle CreateUIReadyBattle()
		{
			if (_readyBattle == null)
			{
				_readyBattle = Utility.FindOrCreateGameObject<UIReadyBattle>(prefix + "ReadyBattle", parent);
				base.panelList.Add(_readyBattle);
			}
			return _readyBattle;
		}

		private UICommanderList CreateUICommanderList()
		{
			if (_commanderList == null)
			{
				_commanderList = Utility.FindOrCreateGameObject<UICommanderList>(prefix + "CommanderList", parent);
				base.panelList.Add(_commanderList);
			}
			return _commanderList;
		}

		private UIRankingBattle CreateUIRankingBattle()
		{
			if (_rankingBattle == null)
			{
				_rankingBattle = Utility.FindOrCreateGameObject<UIRankingBattle>(prefix + "RankingBattle", parent);
				base.panelList.Add(_rankingBattle);
			}
			return _rankingBattle;
		}

		private UILevelUp CreateUILevelUp()
		{
			if (_levelUp == null)
			{
				_levelUp = Utility.FindOrCreateGameObject<UILevelUp>(prefix + "LevelUpPopUp", parent);
				base.panelList.Add(_levelUp);
			}
			return _levelUp;
		}

		private UIAnnihilationMap CreateUIAnnihilationMap()
		{
			if (_annihilationMap == null)
			{
				_annihilationMap = Utility.FindOrCreateGameObject<UIAnnihilationMap>(prefix + "AnnihilationMap", parent);
				base.panelList.Add(_annihilationMap);
			}
			return _annihilationMap;
		}

		private UISeaRobberSweep CreateUISeaRobberSweep()
		{
			if (_seaRobberSweep == null)
			{
				_seaRobberSweep = Utility.FindOrCreateGameObject<UISeaRobberSweep>(prefix + "SeaRobberSweep", parent);
				base.panelList.Add(_seaRobberSweep);
			}
			return _seaRobberSweep;
		}

		private UISituation CreateUISituation()
		{
			if (_situation == null)
			{
				_situation = Utility.FindOrCreateGameObject<UISituation>(prefix + "Situation", parent);
				base.panelList.Add(_situation);
			}
			return _situation;
		}

		private UIGacha CreateUIGacha()
		{
			if (_gacha == null)
			{
				_gacha = Utility.FindOrCreateGameObject<UIGacha>(prefix + "Gacha", parent);
				base.panelList.Add(_gacha);
			}
			return _gacha;
		}

		private UICommanderDetail CreateUICommanderDetail()
		{
			if (_commnaderDetail == null)
			{
				_commnaderDetail = Utility.FindOrCreateGameObject<UICommanderDetail>(prefix + "CommanderDetail", parent);
				base.panelList.Add(_commnaderDetail);
			}
			return _commnaderDetail;
		}

		private UIUnitUpgradeComplete CreateUIUnitUpgradeComplete()
		{
			if (_unitUpgradeComplete == null)
			{
				_unitUpgradeComplete = Utility.FindOrCreateGameObject<UIUnitUpgradeComplete>(prefix + "UIUnitUpgradeComplete", parent);
				base.panelList.Add(_unitUpgradeComplete);
			}
			return _unitUpgradeComplete;
		}

		private UIRaid CreateUIRaid()
		{
			if (_raid == null)
			{
				_raid = Utility.FindOrCreateGameObject<UIRaid>(prefix + "Raid", parent);
				base.panelList.Add(_raid);
			}
			return _raid;
		}

		private UIGuild CreateUIGuild()
		{
			if (_guild == null)
			{
				_guild = Utility.LoadAndInstantiateGameObject<UIGuild>(prefix + "Guild", parent);
				base.panelList.Add(_guild);
			}
			return _guild;
		}

		private UIPreDeckSetting CreateUIPreDeckSetting()
		{
			if (_preDeckSetting == null)
			{
				_preDeckSetting = Utility.FindOrCreateGameObject<UIPreDeckSetting>(prefix + "PreDeckSetting", parent);
				base.panelList.Add(_preDeckSetting);
			}
			return _preDeckSetting;
		}

		private UISecretShop CreateUISecretShop()
		{
			if (_secretShop == null)
			{
				_secretShop = Utility.LoadAndInstantiateGameObject<UISecretShop>(prefix + "SecretShop", parent);
				base.panelList.Add(_secretShop);
			}
			return _secretShop;
		}

		private UIVipShop CreateUIVipShop()
		{
			if (_vipShop == null)
			{
				_vipShop = Utility.LoadAndInstantiateGameObject<UIVipShop>(prefix + "VipShop", parent);
				base.panelList.Add(_vipShop);
			}
			return _vipShop;
		}

		private UIAlarm CreateUIAlarm()
		{
			if (_alarm == null)
			{
				_alarm = Utility.LoadAndInstantiateGameObject<UIAlarm>(prefix + "Alarm", parent);
				base.panelList.Add(_alarm);
			}
			return _alarm;
		}

		private UIVipGacha CreateUIVipGacha()
		{
			if (_vipGacha == null)
			{
				_vipGacha = Utility.LoadAndInstantiateGameObject<UIVipGacha>(prefix + "VipGacha", parent);
				base.panelList.Add(_vipGacha);
			}
			return _vipGacha;
		}

		private UICarnival CreateUICarnival()
		{
			if (_carnival == null)
			{
				_carnival = Utility.LoadAndInstantiateGameObject<UICarnival>(prefix + "Carnival", parent);
				base.panelList.Add(_carnival);
			}
			return _carnival;
		}

		private UIGroup CreateUIGroup()
		{
			if (_group == null)
			{
				_group = Utility.LoadAndInstantiateGameObject<UIGroup>(prefix + "Group", parent);
				base.panelList.Add(_group);
			}
			return _group;
		}

		private UIConquestMap CreateUIConquestMap()
		{
			if (_conquestMap == null)
			{
				_conquestMap = Utility.LoadAndInstantiateGameObject<UIConquestMap>(prefix + "ConquestMap", parent);
				base.panelList.Add(_conquestMap);
			}
			return _conquestMap;
		}

		private UICooperateBattle CreateUICooperateBattle()
		{
			if (_cooperateBattle == null)
			{
				_cooperateBattle = Utility.LoadAndInstantiateGameObject<UICooperateBattle>(prefix + "CooperateBattle", parent);
				base.panelList.Add(_cooperateBattle);
			}
			return _cooperateBattle;
		}

		private UIDateMode CreateUIDateMode()
		{
			if (_dateMode == null)
			{
				_dateMode = Utility.LoadAndInstantiateGameObject<UIDateMode>(prefix + "DateMode", parent);
				base.panelList.Add(_dateMode);
			}
			return _dateMode;
		}

		private UIWaveBattle CreateUIWaveBattle()
		{
			if (_waveBattle == null)
			{
				_waveBattle = Utility.FindOrCreateGameObject<UIWaveBattle>(prefix + "WaveBattle", parent);
				base.panelList.Add(_waveBattle);
			}
			return _waveBattle;
		}

		private UILaboratory CreateUILaboratory()
		{
			if (_laboratory == null)
			{
				_laboratory = Utility.FindOrCreateGameObject<UILaboratory>(prefix + "Laboratory", parent);
				base.panelList.Add(_laboratory);
			}
			return _laboratory;
		}

		private UIWeaponResearch CreateUIWeaponResearch()
		{
			if (_weaponResearch == null)
			{
				_weaponResearch = Utility.FindOrCreateGameObject<UIWeaponResearch>(prefix + "WeaponResearch", parent);
				base.panelList.Add(_weaponResearch);
			}
			return _weaponResearch;
		}

		private UIEventBattle CreateUIEventBattle()
		{
			if (_eventBattle == null)
			{
				_eventBattle = Utility.FindOrCreateGameObject<UIEventBattle>(prefix + "EventBattle", parent);
				base.panelList.Add(_eventBattle);
			}
			return _eventBattle;
		}

		private UIInfinityBattle CreateUIInfinityBattle()
		{
			if (_infinityBattle == null)
			{
				_infinityBattle = Utility.FindOrCreateGameObject<UIInfinityBattle>(prefix + "InfinityBattle", parent);
				base.panelList.Add(_infinityBattle);
			}
			return _infinityBattle;
		}

		protected override void _Init()
		{
			camp = Utility.LoadAndInstantiateGameObject<UICamp>(prefix + "Camp", parent);
			mainCommand = Utility.FindOrCreateGameObject<UIMainCommand>(prefix + "MainCommand", parent);
			worldMap = Utility.FindOrCreateGameObject<UIWorldMap>(prefix + "WorldMap", parent);
			dailyBonus = Utility.FindOrCreateGameObject<UIDailyBonus>(prefix + "DailyBonus", parent);
			dialogMrg = CacheManager.instance.UiCache.Create<ClassicRpgManager>("ClassicRpgManager", parent);
			dialogMrg.gameObject.SetActive(value: false);
		}
	}

	[Serializable]
	public class Battle : Group
	{
		protected AbstractBattle battleMain;

		protected UIBattleMain uiBattleMain;

		protected ClassicRpgManager dialogMgr;

		protected UIBattleResult battleResult;

		public AbstractBattle Main => battleMain;

		public UIBattleMain MainUI => uiBattleMain;

		public ClassicRpgManager DialogMrg => dialogMgr;

		public UIBattleResult BattleResult => battleResult;

		public Simulator Simulator => battleMain.Simulator;

		public AbstractBattle.ViewData ViewData => Main.viewData;

		public AbstractBattle.BattleTroopViewData LeftView => Main.viewData.leftView;

		public AbstractBattle.BattleTroopViewData RightView => Main.viewData.rightView;

		protected override void _Init()
		{
			Transform transform = UIRoot.list[0].transform;
			uiBattleMain = CacheManager.instance.UiCache.Create<UIBattleMain>("Battle", transform);
			uiBattleMain.name = "Battle";
			battleResult = CacheManager.instance.UiCache.Create<UIBattleResult>("BattleResultPopUp", transform);
			battleResult.name = "BattleResultPopUp";
			battleResult.gameObject.SetActive(value: false);
			dialogMgr = CacheManager.instance.UiCache.Create<ClassicRpgManager>("ClassicRpgManager", transform);
			dialogMgr.gameObject.SetActive(value: false);
			if (battleMain == null)
			{
				battleMain = UnityEngine.Object.FindObjectOfType<M03_Battle>();
			}
			if (battleMain == null)
			{
				battleMain = UnityEngine.Object.FindObjectOfType<M04_Tutorial>();
			}
			if (uiBattleMain == null)
			{
				uiBattleMain = battleMain.ui;
			}
			if (battleResult == null)
			{
				battleResult = UnityEngine.Object.FindObjectOfType<UIBattleResult>();
			}
			base.panelList = UIPanelBase.FindContainedPanelBase(this, includeParent: false);
		}
	}

	[Serializable]
	public class Scenario : Group
	{
		protected ClassicRpgManager dialogMgr;

		public ClassicRpgManager DialogMgr => dialogMgr;

		protected override void _Init()
		{
			Transform transform = UIRoot.list[0].transform;
			dialogMgr = CacheManager.instance.UiCache.Create<ClassicRpgManager>("ClassicRpgManager", transform);
			dialogMgr.gameObject.SetActive(value: false);
			base.panelList = UIPanelBase.FindContainedPanelBase(this, includeParent: false);
		}
	}

	private static UIManager _singleton;

	[HideInInspector]
	public EState state;

	private bool needRefreshUI;

	private SortedDictionary<string, LinkData> _linkDataDict = new SortedDictionary<string, LinkData>();

	public static bool hasInstance => _singleton != null;

	public static UIManager instance
	{
		get
		{
			if (_singleton == null)
			{
				_singleton = UnityEngine.Object.FindObjectOfType<UIManager>();
				if (_singleton == null)
				{
					GameObject gameObject = new GameObject("_UIManager");
					_singleton = gameObject.AddComponent<UIManager>();
					if (_singleton == null)
					{
						throw new NullReferenceException();
					}
				}
				_singleton._Init();
			}
			return _singleton;
		}
	}

	public World world { get; private set; }

	public Battle battle { get; private set; }

	public Scenario scenario { get; private set; }

	public ClassicRpgManager DialogMrg
	{
		get
		{
			switch (state)
			{
			case EState.World:
				if (world != null)
				{
					return world.dialogMrg;
				}
				break;
			case EState.Battle:
			case EState.Tutorial:
				if (battle != null)
				{
					return battle.DialogMrg;
				}
				break;
			case EState.Scenario:
				if (scenario != null)
				{
					return scenario.DialogMgr;
				}
				break;
			}
			return null;
		}
	}

	public Group current
	{
		get
		{
			switch (state)
			{
			case EState.World:
				return world;
			case EState.Battle:
			case EState.Tutorial:
				return battle;
			case EState.Scenario:
				return scenario;
			default:
				return null;
			}
		}
	}

	public void Release()
	{
		StopAllCoroutines();
		CacheItemPocket.Release();
		CacheManager.instance.CleanUp();
		if (current != null)
		{
			current.Release();
		}
	}

	private void _Init()
	{
		if (Application.loadedLevelName == Loading.WorldMap)
		{
			world = new World();
			_InitSingletonObject();
			state = EState.World;
		}
		else if (Application.loadedLevelName == Loading.Battle)
		{
			battle = new Battle();
			state = EState.Battle;
		}
		else if (Application.loadedLevelName == Loading.BattleTest)
		{
			battle = new Battle();
			state = EState.Battle;
		}
		else if (Application.loadedLevelName == Loading.Tutorial)
		{
			battle = new Battle();
			state = EState.Tutorial;
		}
		else if (Application.loadedLevelName == Loading.Scenario)
		{
			scenario = new Scenario();
			state = EState.Scenario;
		}
		else if (Application.loadedLevelName == Loading.EditProjectileEffect)
		{
			state = EState.Editor;
		}
	}

	private void _InitSingletonObject()
	{
	}

	private void Awake()
	{
		if (_singleton != null && _singleton != this)
		{
			UnityEngine.Object.DestroyImmediate(this);
			return;
		}
		_singleton = this;
		_Init();
	}

	private void OnDestroy()
	{
		if (_singleton == this)
		{
			_singleton = null;
		}
	}

	private void OnLevelWasLoaded(int level)
	{
	}

	private void LateUpdate()
	{
		if (needRefreshUI)
		{
			_RefreshOpenedUI();
			needRefreshUI = false;
		}
	}

	public void RefreshOpenedUI()
	{
		needRefreshUI = true;
	}

	private void _RefreshOpenedUI()
	{
		List<UIPanelBase> list = null;
		if (world != null)
		{
			list = world.panelList;
		}
		else if (battle != null)
		{
			list = battle.panelList;
		}
		list?.ForEach(delegate(UIPanelBase target)
		{
			if (target.isActive)
			{
				target.OnRefresh();
			}
		});
	}

	public bool OpenLink(string key)
	{
		string text = key;
		if (text.Contains("@"))
		{
			int num = text.IndexOf("@");
			string key2 = text.Substring(0, num);
			string initData = text.Substring(num + 1);
			return OpenLink(key2, initData);
		}
		if (text.StartsWith("Link-"))
		{
			text = text.Substring("Link-".Length);
		}
		if (!_linkDataDict.ContainsKey(text))
		{
			return false;
		}
		if (!_linkDataDict[text].Open())
		{
			return false;
		}
		return true;
	}

	public bool OpenLink(string key, string initData)
	{
		string text = key;
		if (text.StartsWith("Link-"))
		{
			text = text.Substring("Link-".Length);
		}
		if (!_linkDataDict.ContainsKey(text))
		{
			return false;
		}
		if (!_linkDataDict[text].Open(initData))
		{
			return false;
		}
		return true;
	}

	public void ClearLinkInfo()
	{
		_linkDataDict.Clear();
	}

	public void UnregisterLinkDelegate(object obj)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, LinkData> item in _linkDataDict)
		{
			if (item.Value.target == obj)
			{
				list.Add(item.Key);
			}
		}
		list.ForEach(delegate(string key)
		{
			_linkDataDict.Remove(key);
		});
	}

	public void RegisterLinkDelegate(object obj, string methodPrefix, string linkNamePrefix = "")
	{
		linkNamePrefix = ((!string.IsNullOrEmpty(linkNamePrefix)) ? (linkNamePrefix + ".") : string.Empty);
		Type type = obj.GetType();
		MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		string text = obj.ToString();
		foreach (MethodInfo methodInfo in methods)
		{
			string text2 = methodInfo.Name;
			if (!text2.StartsWith(methodPrefix) || methodInfo.ReturnType != typeof(void))
			{
				continue;
			}
			ParameterInfo[] parameters = methodInfo.GetParameters();
			LinkData linkData = null;
			if (parameters.Length <= 0)
			{
				LinkOpenVoidDelegate linkOpenVoidDelegate = null;
				try
				{
					linkOpenVoidDelegate = (LinkOpenVoidDelegate)Delegate.CreateDelegate(typeof(LinkOpenVoidDelegate), obj, text2);
					linkData = LinkData.Create(linkOpenVoidDelegate);
				}
				catch (Exception)
				{
					linkOpenVoidDelegate = null;
				}
			}
			else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
			{
				LinkOpenStringDelegate linkOpenStringDelegate = null;
				try
				{
					linkOpenStringDelegate = (LinkOpenStringDelegate)Delegate.CreateDelegate(typeof(LinkOpenStringDelegate), obj, text2);
					linkData = LinkData.Create(linkOpenStringDelegate);
				}
				catch (Exception)
				{
					linkOpenStringDelegate = null;
				}
			}
			if (linkData != null)
			{
				string key = linkNamePrefix + text2.Substring(methodPrefix.Length);
				if (!_linkDataDict.ContainsKey(key))
				{
					_linkDataDict.Add(key, linkData);
				}
			}
		}
	}

	private static Delegate _FindDelegate<T>(object instance, string methodName)
	{
		Type type = instance.GetType();
		MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		methodName = methodName.ToLower();
		for (int i = 0; i < methods.Length; i++)
		{
			string text = methods[i].Name;
			if (!(text.ToLower() != methodName))
			{
				return Delegate.CreateDelegate(typeof(T), instance, text);
			}
		}
		return null;
	}

	public static void RegisterOnClickDelegate(Group target, object instance, string receiverPrefix, UIPanelBase.OnClickDelegate defaultReceiver = null)
	{
		if (target == null || instance == null)
		{
			return;
		}
		List<UIPanelBase> panelList = target.panelList;
		panelList.ForEach(delegate(UIPanelBase pb)
		{
			string methodName = receiverPrefix + pb.gameObject.name.Replace("-", string.Empty);
			Delegate @delegate = _FindDelegate<UIPanelBase.OnClickDelegate>(instance, methodName);
			if ((object)@delegate != null)
			{
				pb.onClick = (UIPanelBase.OnClickDelegate)Delegate.Combine(pb.onClick, @delegate as UIPanelBase.OnClickDelegate);
			}
			else if (defaultReceiver != null)
			{
				pb.onClick = (UIPanelBase.OnClickDelegate)Delegate.Combine(pb.onClick, defaultReceiver);
			}
		});
	}

	public void SetPopupPositionY(GameObject obj)
	{
	}

	public void StartLocalCoroutine(IEnumerator routine)
	{
		StartCoroutine(routine);
	}

	public void StopLocalCoroution(IEnumerator routine)
	{
		StopCoroutine(routine);
	}

	public void EnableCameraTouchEvent(bool isEnable)
	{
		GameObject gameObject = GameObject.Find("Camera");
		if (!(gameObject.GetComponent<UICamera>() == null))
		{
			gameObject.GetComponent<UICamera>().useMouse = isEnable;
			gameObject.GetComponent<UICamera>().useTouch = isEnable;
		}
	}
}
