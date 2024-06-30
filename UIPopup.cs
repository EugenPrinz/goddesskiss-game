using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopup : UIPanelBase
{
	public delegate void OnCloseDelegate();

	public static UIPopup current = null;

	private static int _popupCount = 0;

	public static List<UIPopup> openedPopups = new List<UIPopup>();

	public UILabel title;

	public UILabel message;

	protected bool _isSelected;

	protected bool _autoDestory = true;

	private bool _recyclable;

	protected bool _isClosed;

	protected int _waitRoutineCount;

	public OnCloseDelegate onClose;

	protected bool recyclable { get; set; }

	public static void InitUIPopup()
	{
		openedPopups.Clear();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		_popupCount++;
		_isClosed = false;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		_popupCount--;
	}

	protected override void Awake()
	{
		base.Awake();
		if (GetType() != typeof(UIPopup))
		{
			_autoDestory = false;
			_recyclable = true;
		}
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		switch (text)
		{
		case "Cancel":
			SoundManager.PlaySFX("BTN_Negative_001");
			break;
		case "Close":
			SoundManager.PlaySFX("SE_MenuClose_001");
			break;
		case "OK":
		case "Complete":
			SoundManager.PlaySFX("BTN_Positive_001");
			break;
		}
		if (base.name == "HeadQuarters")
		{
			if (text.StartsWith("Commander"))
			{
				SoundManager.PlaySFX("BTN_Norma_001");
			}
			else
			{
				switch (text)
				{
				case "AllTab":
				case "AttackersTab":
				case "DefendersTab":
				case "SupportersTab":
					SoundManager.PlaySFX("BTN_Tap_001");
					break;
				case "PreDeck":
					SoundManager.PlaySFX("BTN_Positive_001");
					break;
				}
			}
		}
		else if (base.name == "CommanderDetail")
		{
			switch (text)
			{
			case "CommanderBtn":
			case "UnitBtn":
			case "CostumeBtn":
			case "FavorBtn":
			case "ScenarioBtn":
			case "VoiceBtn":
				SoundManager.PlaySFX("BTN_Tap_001");
				break;
			default:
				if (!text.StartsWith("Ticket"))
				{
					if (text == "ClassUpBtn" || text.StartsWith("SkillLevelUpBtn"))
					{
						SoundManager.PlaySFX("BTN_Norma_001");
					}
					else if (text.StartsWith("Costume_") || text == "Toggle_item" || text == "Toggle_unit")
					{
						SoundManager.PlaySFX("BTN_Norma_001");
					}
					break;
				}
				goto case "GetMedalBtn";
			case "GetMedalBtn":
			case "ExchangeBtn":
			case "RankUpBtn":
				SoundManager.PlaySFX("BTN_Norma_001");
				break;
			}
		}
		else if (base.name == "ReadyBattle")
		{
			switch (text)
			{
			case "Attack":
				SoundManager.PlaySFX("BTN_Battle_001");
				break;
			case "Ready":
				SoundManager.PlaySFX("BTN_Positive_001");
				break;
			case "Setting":
				SoundManager.PlaySFX("BTN_Positive_001");
				break;
			case "AllTab":
			case "AttackTab":
			case "DefenseTab":
			case "SupportTab":
			case "MercenaryTab":
			case "NextTroop":
			case "PreviousTroop":
				SoundManager.PlaySFX("BTN_Tap_001");
				break;
			case "CommanderSelect":
			case "EditTroop":
				SoundManager.PlaySFX("BTN_Positive_001");
				break;
			}
		}
		else if (base.name == "Mission_1")
		{
			switch (text)
			{
			case "Mission":
			case "Achievement":
			case "CompleteAchievement":
			case "BattleRecord":
			case "BtnGoalReceive":
				SoundManager.PlaySFX("BTN_Tap_001");
				break;
			}
		}
		else if (base.name == "RankingBattle")
		{
			switch (text)
			{
			case "RankingBtn":
			case "RecordBtn":
			case "RewardInfoBtn":
			case "ScoreRewardPopUp":
				SoundManager.PlaySFX("BTN_Positive_001");
				break;
			case "RefreshDuelTarget":
				SoundManager.PlaySFX("BTN_Refresh_001");
				break;
			default:
				if (text.StartsWith("DuelTargetCommander"))
				{
					SoundManager.PlaySFX("BTN_Positive_001");
				}
				else if (text == "CommanderEditBtn")
				{
					SoundManager.PlaySFX("BTN_Positive_001");
				}
				break;
			}
		}
		else if (base.name == "Storage")
		{
			switch (text)
			{
			case "Part":
			case "Medal":
			case "Goods":
			case "Food":
			case "Items":
				SoundManager.PlaySFX("BTN_Tap_001");
				break;
			default:
				if (text.StartsWith("PartItem") || text.StartsWith("MedalItem") || text.StartsWith("GoodsItem") || text.StartsWith("InvenItem"))
				{
					SoundManager.PlaySFX("BTN_Norma_001");
					break;
				}
				switch (text)
				{
				case "OpenSellContents":
					SoundManager.PlaySFX("BTN_Norma_001");
					break;
				case "SellCancleBtn":
					SoundManager.PlaySFX("BTN_Negative_001");
					break;
				case "SellOkBtn":
					SoundManager.PlaySFX("BTN_Positive_001");
					break;
				}
				break;
			}
		}
		else if (base.name == "Gacha")
		{
			if (text.StartsWith("Normal") || text.StartsWith("Premium"))
			{
				SoundManager.PlaySFX("BTN_Positive_001");
			}
			switch (text)
			{
			case "Close-BoxOpenSingle":
			case "Close-BoxOpenMultiple":
				SoundManager.PlaySFX("BTN_Norma_001");
				break;
			case "OpenNormalRewardList":
			case "OpenPremiumRewardList":
				SoundManager.PlaySFX("BTN_Norma_001");
				break;
			case "Close-PremiumGoodsList":
			case "Close-NormalGoodsList":
				SoundManager.PlaySFX("BTN_Positive_001");
				break;
			}
		}
		else if (base.name == "Raid")
		{
			if (text == "RankingBtn" || text == "RewardInfoBtn")
			{
				SoundManager.PlaySFX("BTN_Positive_001");
			}
			else if (text.StartsWith("Skill"))
			{
				SoundManager.PlaySFX("BTN_Norma_001");
			}
			else if (text == "StartBattle")
			{
				SoundManager.PlaySFX("BTN_Positive_001");
			}
			else if (text == "SecretShopPopup")
			{
				SoundManager.PlaySFX("BTN_Positive_001");
			}
		}
		else if (base.name == "Situation")
		{
			if (text.StartsWith("BtnWeek"))
			{
				SoundManager.PlaySFX("BTN_Positive_001");
			}
		}
		else if (base.name == "WaveBattle")
		{
			SoundManager.PlaySFX("BTN_Positive_001");
		}
		else if (base.name == "Group")
		{
			if (text.StartsWith("Tab-"))
			{
				SoundManager.PlaySFX("BTN_Tap_001");
			}
		}
		else if (base.name == "UserDetail")
		{
			switch (text)
			{
			case "Profile":
			case "Setting":
			case "Info":
			case "Notice":
			case "Lang-S_Beon":
			case "Lang-S_En":
				SoundManager.PlaySFX("BTN_Tap_001");
				break;
			}
		}
		else if (text == "Btn_Upgrade" || text == "UpgradeBtn" || text.StartsWith("Unit-") || text.StartsWith("Troop-") || text == "AutoOrganization" || text == "UnitInBtn")
		{
			SoundManager.PlaySFX("BTN_Positive_001");
		}
		else if (text == "Receive" || text == "Play" || text == "Retry")
		{
			SoundManager.PlaySFX("BTN_Positive_001");
		}
		else if (text == "Delay" || text == "RefreshBtn" || text == "RecruitTab" || text == "ArmyTab" || text == "NavyTab")
		{
			SoundManager.PlaySFX("BTN_Positive_001");
		}
		else
		{
			if (!(text == "UserDetail"))
			{
				switch (text)
				{
				case "Achievement":
				case "Mail":
				case "Mission":
				case "CallBtn":
				case "Link-CashShop":
				case "Link-ChallengeShop":
				case "Link-GoldShop":
				case "BulletRecharge":
				case "KeyRecharge":
				case "ChallengeRecharge":
				case "ToCamp":
				case "ToWorldMap":
				case "UserDetail":
				case "Profile":
				case "Setting":
				case "Notice":
				case "Info":
					break;
				default:
					goto IL_1096;
				}
			}
			SoundManager.PlaySFX("BTN_Positive_001");
		}
		goto IL_13e0;
		IL_1096:
		if (base.name == "Laboratory")
		{
			switch (text)
			{
			case "Tab_upgrade":
			case "Tab_decomposition":
				SoundManager.PlaySFX("BTN_Tap_001");
				break;
			case "selectItemSlot":
			case "Btn_Equip":
				SoundManager.PlaySFX("BTN_Positive_001");
				break;
			}
		}
		else if (base.name == "HaveItemPopup")
		{
			switch (text)
			{
			case "All_tab":
			case "Item_1":
			case "Item_2":
			case "Item_3":
			case "Item_4":
				SoundManager.PlaySFX("BTN_Tap_001");
				break;
			case "Btn_Equip":
			case "Btn_Select":
			case "Btn_Clear":
			case "Btn_Change":
				SoundManager.PlaySFX("BTN_Norma_001");
				break;
			case "Btn_Laboratory":
				SoundManager.PlaySFX("BTN_Positive_001");
				break;
			}
		}
		else
		{
			switch (text)
			{
			case "AllTab":
			case "AttackTab":
			case "DefenseTab":
			case "SupportTab":
			case "MercenaryTab":
			case "PrevMove":
			case "NextMove":
				SoundManager.PlaySFX("BTN_Tap_001");
				break;
			}
			if (text.StartsWith("AddButton-") || text.StartsWith("RemoveButton-"))
			{
				SoundManager.PlaySFX("BTN_Norma_001");
			}
		}
		goto IL_13e0;
		IL_13e0:
		if (onClick != null)
		{
			current = this;
			base.OnClick(sender);
			current = null;
			if (_autoDestory && _waitRoutineCount <= 0)
			{
				Close();
			}
		}
		else if (_autoDestory && _waitRoutineCount <= 0)
		{
			Close();
		}
	}

	public Coroutine WaitResult()
	{
		return StartCoroutine(_WaitResult());
	}

	private IEnumerator _WaitResult()
	{
		if (!_recyclable)
		{
			_waitRoutineCount++;
			while (!_isSelected)
			{
				yield return null;
			}
			_waitRoutineCount--;
			if (_autoDestory && _waitRoutineCount <= 0)
			{
				Close();
			}
		}
		else
		{
			while (base.gameObject.activeSelf)
			{
				yield return null;
			}
		}
	}

	public virtual void Close()
	{
		if (_isClosed)
		{
			return;
		}
		_isClosed = true;
		if (!(this is UIMainCommand))
		{
			SetPopupState(state: false);
			if (!openedPopups.Remove(this))
			{
			}
			if (openedPopups.Count > 0 && UIManager.instance.world != null)
			{
				openedPopups[0].SendMessage("SetPopupState", true, SendMessageOptions.DontRequireReceiver);
			}
		}
		StopAllCoroutines();
		base.gameObject.SetActive(value: false);
		if (onClose != null)
		{
			onClose();
		}
		if (!_recyclable)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public virtual void Open()
	{
		if (root != null)
		{
			root.SetActive(value: true);
		}
		if (!(this is UIMainCommand))
		{
			if (openedPopups.Count > 0 && UIManager.instance.world != null)
			{
				openedPopups[0].SendMessage("SetPopupState", false, SendMessageOptions.DontRequireReceiver);
			}
			openedPopups.Insert(0, this);
			SetPopupState(state: true);
		}
		switch (base.gameObject.name)
		{
		case "LevelUpPopUp":
			SoundManager.PlaySFX("SE_LevelUp_001");
			break;
		case "DailyBonus":
		case "SupplyBaseEditFormation":
		case "InputUserString":
		case "PlatoonRemoveAllUnit":
		case "ResourcePurchasePopup":
		case "BuildingUpgrade":
			SoundManager.PlaySFX("EFM_OpenPopup_001");
			break;
		case "WarMemorial":
		case "Gacha":
		case "Academy":
		case "SupplyBaseSelectTroop":
		case "HeadQuarters":
		case "MetroBank":
		case "Situation":
		case "UnitResearch":
		case "BlackMarket":
		case "Raid":
		case "Guild":
		case "RankingBattle":
		case "Mission_1":
		case "WaveBattle":
			SoundManager.PlaySFX("SE_MenuOpen_001");
			break;
		default:
			SoundManager.PlaySFX("SE_MenuOpen_001");
			break;
		case "Camp":
		case "WorldMap":
		case "MainCommand":
		case "GetItem":
		case "Battle":
			break;
		}
		MoveToFront();
	}

	public void SetAutoDestroy(bool autoDestory)
	{
		_autoDestory = autoDestory;
		_recyclable = _recyclable && !_autoDestory;
	}

	public void SetRecyclable(bool recyclable)
	{
		_recyclable = recyclable;
	}

	protected static UIPopup _Create(string prefabName)
	{
		UIRoot uIRoot = UIRoot.list[0];
		if (uIRoot == null)
		{
			return null;
		}
		GameObject gameObject = Utility.LoadAndInstantiateGameObject("Prefabs/UI/Popup/" + prefabName, uIRoot.transform);
		if (gameObject == null)
		{
			return null;
		}
		UIPopup component = gameObject.GetComponent<UIPopup>();
		if (component == null)
		{
			return null;
		}
		component._isSelected = false;
		component._waitRoutineCount = 0;
		component.gameObject.SetActive(value: true);
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
		return component;
	}

	public static T Create<T>(string prefabName) where T : MonoBehaviour
	{
		UIPopup uIPopup = _Create(prefabName);
		if (uIPopup == null)
		{
			return (T)null;
		}
		return uIPopup.GetComponent<T>();
	}

	public static UIPopup Create(string prefabName)
	{
		return Create<UIPopup>(prefabName);
	}

	public void DefaultOpenAnimation()
	{
		if (root != null)
		{
			root.transform.localPosition = Vector3.up * -1000f;
		}
		iTween.MoveTo(base.gameObject, iTween.Hash("y", 0, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeOutBack));
	}

	public void DefaultCloseAnimation()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", -1000, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeInBack, "oncomplete", "Close", "oncompletetarget", base.gameObject));
	}
}
