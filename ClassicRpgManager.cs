using System.Collections;
using System.Collections.Generic;
using Cache;
using DialoguerCore;
using Shared.Regulation;
using Spine.Unity;
using UnityEngine;

public class ClassicRpgManager : MonoBehaviour
{
	private bool _autoDestory;

	private string _windowTargetText = string.Empty;

	private string _windowCurrentText = string.Empty;

	private string _nameText = string.Empty;

	private bool _isBranchedText;

	private string[] _branchedTextChoices;

	private int _currentChoice;

	private string _theme;

	private float _windowTweenValue;

	private bool _windowReady;

	private float _nameTweenValue;

	private bool _usingPositionRect;

	private int _textFrames = int.MaxValue;

	private bool _dialogue;

	private bool _showDialogueBox;

	public GameObject btnSkip;

	public GameObject btnScenarioSkip;

	public GameObject btnAuto;

	public GameObject btnLog;

	public UILabel lbLeftName;

	public UILabel lbRightName;

	public UILabel lbCenterName;

	public UILabel lbLeftWndCurText;

	public UILabel lbRightWndCurText;

	public UILabel lbCenterWndCurText;

	public UILabel lbIntroWndCurText;

	public UISpineAnimation rootSpineData;

	public GameObject goLeftSpineData;

	public GameObject goRightSpineData;

	public GameObject goCenter;

	public GameObject goIntro;

	public bool isTutorial;

	private string strEnableImage;

	private Dictionary<string, SkeletonAnimation> dicSpineData = new Dictionary<string, SkeletonAnimation>();

	[SerializeField]
	private UITexture BG;

	private string bgPrefix = "Texture/UI/";

	[SerializeField]
	private GameObject bubbleSprite;

	[SerializeField]
	private GameObject SpriteAni;

	[SerializeField]
	private GameObject ScenarioTitle;

	[SerializeField]
	private UILabel ScenarioTitle_txt;

	[SerializeField]
	private UILabel ScenarioTitle_Order;

	[SerializeField]
	private BoxCollider BG_coll;

	[SerializeField]
	private UISprite sparkBG;

	private UIDialogueBoxes box;

	private bool isScenario;

	private const float MAXTIME = 2f;

	private float runtime;

	private bool isScenarioStart;

	private string scenarioID = string.Empty;

	private const int OriginFontSize = 28;

	private string spineEmotion;

	[SerializeField]
	private UIShake uiShake;

	private bool isChangefontSize;

	private int fontSize;

	[HideInInspector]
	public List<ScenarioLogInfo> logInfoList = new List<ScenarioLogInfo>();

	[SerializeField]
	private UIScenarioLog scenarioLog;

	private string EffPath = "Prefabs/Test/";

	private List<EffectInfo> effInfoList = new List<EffectInfo>();

	[SerializeField]
	private GameObject EffectParent;

	private string currBG = string.Empty;

	private string prevBG = string.Empty;

	[SerializeField]
	private GameObject BtnWin;

	[SerializeField]
	private GameObject BtnTextFast;

	private const int DefaultTextFrame = 1;

	private const int FastTextFrame = 5;

	private int textVelocity;

	[SerializeField]
	private GameObject SettingWindow;

	[SerializeField]
	private GameObject btnSetting;

	[SerializeField]
	private UIGameSetting setting;

	[SerializeField]
	private GameObject SpeedOn;

	[SerializeField]
	private GameObject SpeedOff;

	[SerializeField]
	private GameObject AutoOn;

	[SerializeField]
	private GameObject AutoOff;

	[SerializeField]
	private GameObject WindowOn;

	[SerializeField]
	private GameObject WindowOff;

	private bool isAuto;

	private bool isSpeedUp;

	[SerializeField]
	private UIScrollBar scrollBar;

	private string currSpeaker = string.Empty;

	private float shakeTime;

	private float shakeMin;

	private float shakeMax;

	private string voiceClip = string.Empty;

	private AudioSource currVoice;

	private AudioSource currEffSound;

	private bool isGoingProgressing;

	private bool isWindowHide;

	private string preLanguage = string.Empty;

	private string scenarioTextLocalKey = string.Empty;

	private string spineScale = string.Empty;

	private bool isSpark;

	private bool isContainUserNickname;

	private string curSpine = string.Empty;

	private bool isEvent;

	private bool isInfinity;

	[HideInInspector]
	public bool isPlayDialogue;

	[SerializeField]
	private UILabel SkipLabel;

	[SerializeField]
	private GameObject WeddingEvent;

	private bool isWeddingEvent;

	[SerializeField]
	private GameObject WeddingFlower;

	private void OnDisable()
	{
	}

	private void Awake()
	{
		Dialoguer.Initialize();
		addDialoguerEvents();
	}

	public void StartScenario()
	{
		Dialoguer.Initialize(DialogueType.Scenario);
		addDialoguerEvents();
		UIInteraction.playVoice = false;
		SoundManager.StopMusic();
	}

	public void StartEventScenario()
	{
		Dialoguer.Initialize(DialogueType.Event);
		addDialoguerEvents();
		UIInteraction.playVoice = false;
		SoundManager.StopMusic();
	}

	public void StartInfinityScenario()
	{
		Dialoguer.Initialize(DialogueType.Infinity);
		addDialoguerEvents();
		UIInteraction.playVoice = false;
		SoundManager.StopMusic();
	}

	private void OnDestroy()
	{
		Dialoguer.events.ClearAll();
		foreach (string key in dicSpineData.Keys)
		{
			SkeletonAnimation skeletonAnimation = dicSpineData[key];
			if (skeletonAnimation != null)
			{
				Object.DestroyImmediate(skeletonAnimation.gameObject);
				skeletonAnimation = null;
			}
		}
		dicSpineData.Clear();
	}

	public static bool HasDialogue(string name, DialogueType type = DialogueType.Origin)
	{
		if (!Dialoguer.ready)
		{
			Dialoguer.Initialize(type);
		}
		return DialoguerDataManager.HasDialogue(name);
	}

	public void InitWorldMapStart(string dialogeName, bool isTutorial = false)
	{
		foreach (string key in dicSpineData.Keys)
		{
			SkeletonAnimation skeletonAnimation = dicSpineData[key];
			if (skeletonAnimation != null)
			{
				Object.DestroyImmediate(skeletonAnimation.gameObject);
				skeletonAnimation = null;
			}
		}
		dicSpineData.Clear();
		this.isTutorial = isTutorial;
		isScenario = false;
		_dialogue = true;
		_windowReady = true;
		_showDialogueBox = true;
		_windowTargetText = string.Empty;
		_windowCurrentText = string.Empty;
		_nameText = string.Empty;
		lbLeftWndCurText.text = string.Empty;
		lbLeftName.text = string.Empty;
		lbRightWndCurText.text = string.Empty;
		lbRightName.text = string.Empty;
		lbCenterName.text = string.Empty;
		lbCenterWndCurText.text = string.Empty;
		lbIntroWndCurText.text = string.Empty;
		Font font = Resources.Load("Font/NotoSansCJKkr-Bold") as Font;
		if (font != null)
		{
			lbLeftWndCurText.trueTypeFont = font;
			lbLeftName.trueTypeFont = font;
			lbRightWndCurText.trueTypeFont = font;
			lbRightName.trueTypeFont = font;
			lbCenterWndCurText.trueTypeFont = font;
			lbCenterName.trueTypeFont = font;
		}
		UISetter.SetActive(btnAuto, active: false);
		UISetter.SetActive(btnLog, active: false);
		UISetter.SetActive(btnSetting, active: false);
		UISetter.SetActive(SpeedOn, active: false);
		UISetter.SetActive(SpeedOff, active: false);
		UISetter.SetActive(AutoOn, active: false);
		UISetter.SetActive(AutoOff, active: false);
		UISetter.SetActive(WindowOn, active: false);
		UISetter.SetActive(WindowOff, active: false);
		UISetter.SetActive(btnScenarioSkip, active: false);
		Dialoguer.StartDialogue(dialogeName);
		base.gameObject.SetActive(_showDialogueBox);
	}

	public void InitScenarioDialogue(string dialogeName, DialogueType type)
	{
		foreach (string key in dicSpineData.Keys)
		{
			SkeletonAnimation skeletonAnimation = dicSpineData[key];
			if (skeletonAnimation != null)
			{
				Object.DestroyImmediate(skeletonAnimation.gameObject);
				skeletonAnimation = null;
			}
		}
		dicSpineData.Clear();
		isTutorial = false;
		switch (type)
		{
		case DialogueType.Scenario:
			isScenario = true;
			isEvent = false;
			isInfinity = false;
			UISetter.SetLabel(SkipLabel, Localization.Get("1051"));
			break;
		case DialogueType.Event:
			isScenario = false;
			isEvent = true;
			isInfinity = false;
			UISetter.SetLabel(SkipLabel, Localization.Get("1011"));
			break;
		case DialogueType.Infinity:
			isScenario = false;
			isEvent = false;
			isInfinity = true;
			break;
		}
		_dialogue = true;
		_windowReady = true;
		_showDialogueBox = true;
		_windowTargetText = string.Empty;
		_windowCurrentText = string.Empty;
		_nameText = string.Empty;
		lbLeftWndCurText.text = string.Empty;
		lbLeftName.text = string.Empty;
		lbRightWndCurText.text = string.Empty;
		lbRightName.text = string.Empty;
		lbCenterName.text = string.Empty;
		lbCenterWndCurText.text = string.Empty;
		lbCenterWndCurText.text = string.Empty;
		lbIntroWndCurText.text = string.Empty;
		currBG = string.Empty;
		prevBG = string.Empty;
		textVelocity = 1;
		voiceClip = string.Empty;
		Font font = Resources.Load("Font/T3NOWGE") as Font;
		if (font != null)
		{
			lbLeftWndCurText.trueTypeFont = font;
			lbLeftName.trueTypeFont = font;
			lbRightWndCurText.trueTypeFont = font;
			lbRightName.trueTypeFont = font;
			lbCenterWndCurText.trueTypeFont = font;
			lbCenterName.trueTypeFont = font;
		}
		BG_coll.enabled = true;
		scenarioID = dialogeName;
		UISetter.SetActive(btnSkip, active: false);
		base.gameObject.SetActive(_showDialogueBox);
		logInfoList.Clear();
		effInfoList.Clear();
		isAuto = false;
		isSpeedUp = false;
		currSpeaker = string.Empty;
		isGoingProgressing = false;
		isWindowHide = false;
		scenarioTextLocalKey = string.Empty;
		isSpark = false;
		isContainUserNickname = false;
		curSpine = string.Empty;
		InitButton();
		Dialoguer.StartDialogue(dialogeName);
	}

	public void InitStart(int n)
	{
		foreach (string key in dicSpineData.Keys)
		{
			SkeletonAnimation skeletonAnimation = dicSpineData[key];
			if (skeletonAnimation != null)
			{
				Object.DestroyImmediate(skeletonAnimation.gameObject);
				skeletonAnimation = null;
			}
		}
		dicSpineData.Clear();
		_dialogue = true;
		_windowReady = true;
		_showDialogueBox = true;
		_windowTargetText = string.Empty;
		_windowCurrentText = string.Empty;
		_nameText = string.Empty;
		lbLeftWndCurText.text = string.Empty;
		lbLeftName.text = string.Empty;
		lbRightWndCurText.text = string.Empty;
		lbRightName.text = string.Empty;
		lbCenterName.text = string.Empty;
		lbCenterWndCurText.text = string.Empty;
		lbIntroWndCurText.text = string.Empty;
		Font font = Resources.Load("Font/NotoSansCJKkr-Bold") as Font;
		if (font != null)
		{
			lbLeftWndCurText.trueTypeFont = font;
			lbLeftName.trueTypeFont = font;
			lbRightWndCurText.trueTypeFont = font;
			lbRightName.trueTypeFont = font;
			lbCenterWndCurText.trueTypeFont = font;
			lbCenterName.trueTypeFont = font;
		}
		Dialoguer.StartDialogue(n);
		UISetter.SetActive(btnAuto, active: false);
		UISetter.SetActive(btnLog, active: false);
		UISetter.SetActive(btnSetting, active: false);
		UISetter.SetActive(SpeedOn, active: false);
		UISetter.SetActive(SpeedOff, active: false);
		UISetter.SetActive(AutoOn, active: false);
		UISetter.SetActive(AutoOff, active: false);
		UISetter.SetActive(WindowOn, active: false);
		UISetter.SetActive(WindowOff, active: false);
		UISetter.SetActive(btnScenarioSkip, active: false);
		base.gameObject.SetActive(_showDialogueBox);
	}

	public void OnSkipClick()
	{
		if (box != null)
		{
			Object.Destroy(box.gameObject);
		}
		uiShake.StopShakeScenarioSceen();
		DestroyEffect();
		Dialoguer.EndDialogue();
	}

	public void addDialoguerEvents()
	{
		Dialoguer.events.onStarted += onDialogueStartedHandler;
		Dialoguer.events.onEnded += onDialogueEndedHandler;
		Dialoguer.events.onInstantlyEnded += onDialogueInstantlyEndedHandler;
		Dialoguer.events.onTextPhase += onDialogueTextPhaseHandler;
		Dialoguer.events.onWindowClose += onDialogueWindowCloseHandler;
		Dialoguer.events.onMessageEvent += onDialoguerMessageEvent;
	}

	private void onDialogueStartedHandler()
	{
		_dialogue = true;
		isPlayDialogue = true;
	}

	private void onDialogueEndedHandler()
	{
		if (isScenario || isEvent || isInfinity)
		{
			if (UIManager.instance.world != null)
			{
				CacheManager.instance.SoundPocketCache.Create("Pocket_BGM_World");
			}
			UIInteraction.playVoice = true;
			uiShake.StopShakeScenarioSceen();
			DestroyEffect();
			if (scenarioLog.root.activeSelf)
			{
				scenarioLog.CloseLog();
			}
			isPlayDialogue = false;
			UISetter.SetTexture(BG, null);
			if (currVoice != null && currVoice.isPlaying)
			{
				currVoice.Stop();
				currVoice = null;
				voiceClip = null;
			}
		}
		_dialogue = false;
		_showDialogueBox = false;
		if (base.gameObject != null)
		{
			base.gameObject.SetActive(_showDialogueBox);
			resetWindowSize();
		}
	}

	private void onDialogueInstantlyEndedHandler()
	{
		_dialogue = false;
		_showDialogueBox = false;
		if (base.gameObject != null)
		{
			base.gameObject.SetActive(_showDialogueBox);
			resetWindowSize();
		}
	}

	private void onDialogueTextPhaseHandler(DialoguerTextData data)
	{
		if (!string.IsNullOrEmpty(data.createObjectPath) || !string.IsNullOrEmpty(data.EffSoundClip))
		{
			_dialogue = false;
			StartCoroutine(InitEffect(data));
			return;
		}
		if (currVoice != null && currVoice.isPlaying)
		{
			currVoice.Stop();
			currVoice = null;
			voiceClip = null;
		}
		currEffSound = null;
		_dialogue = true;
		ResetFontSize();
		if (isChangefontSize)
		{
			SetFontSize(fontSize);
			isChangefontSize = false;
		}
		for (int i = 0; i < effInfoList.Count; i++)
		{
			SetEffect(effInfoList[i]);
		}
		spineScale = string.Empty;
		_usingPositionRect = data.usingPositionRect;
		_windowCurrentText = string.Empty;
		_windowTargetText = data.text;
		scenarioTextLocalKey = data.text;
		if (!string.IsNullOrEmpty(data.audio))
		{
			CacheManager.instance.SoundCache.PlayBGM(Loading.WorldMap, data.audio);
		}
		BG_coll.enabled = false;
		if (isScenario || isEvent || isInfinity)
		{
			if (!string.IsNullOrEmpty(data.theme))
			{
				isScenarioStart = true;
			}
			else
			{
				isScenarioStart = false;
			}
			SetActiveGameObject_connectionScenario(isScenarioStart);
		}
		isContainUserNickname = false;
		if (_windowTargetText != string.Empty)
		{
			if (!string.IsNullOrEmpty(data.strArgumnet) && data.strArgumnet == "nick")
			{
				string arg = string.Empty;
				if (RemoteObjectManager.instance.localUser != null)
				{
					arg = RemoteObjectManager.instance.localUser.nickname;
				}
				if (isTutorial)
				{
					_windowTargetText = string.Format(Localization.GetTutorial(data.text), arg);
				}
				else if (isScenario)
				{
					_windowTargetText = string.Format(Localization.GetScenario(data.text), arg);
				}
				else if (isEvent)
				{
					_windowTargetText = string.Format(Localization.GetEvent(data.text), arg);
				}
				else if (isInfinity)
				{
					_windowTargetText = string.Format(Localization.GetInfinity(data.text), arg);
				}
				else
				{
					_windowTargetText = string.Format(Localization.GetTalk(data.text), arg);
				}
				isContainUserNickname = true;
			}
			else if (isTutorial)
			{
				_windowTargetText = Localization.GetTutorial(data.text);
			}
			else if (isScenario)
			{
				_windowTargetText = string.Format(Localization.GetScenario(data.text));
			}
			else if (isEvent)
			{
				_windowTargetText = string.Format(Localization.GetEvent(data.text));
			}
			else if (isInfinity)
			{
				_windowTargetText = string.Format(Localization.GetInfinity(data.text));
			}
			else
			{
				_windowTargetText = Localization.GetTalk(data.text);
			}
		}
		string text = data.strEmotion;
		if (string.IsNullOrEmpty(text))
		{
			text = "a_01_idle1";
		}
		string empty = string.Empty;
		strEnableImage = data.metadata;
		Vector3 startPos = new Vector3(data.startPosX, data.startPosY, 0f);
		Vector3 endPos = new Vector3(data.endPosX, data.endPosY, 0f);
		float num = 1f;
		float num2 = 0f;
		string option_p = string.Empty;
		if (data.None_p)
		{
			option_p = "none_p";
		}
		else if (data.Once_p)
		{
			option_p = "once_p";
		}
		else if (data.PingPong_p)
		{
			option_p = "ping_p";
		}
		num = ((data.duration_p != 0f) ? data.duration_p : num);
		num2 = data.startDelay_p;
		Vector3 vector = new Vector3(data.startScaleX, data.startScaleY, 1f);
		Vector3 vector2 = new Vector3(data.endScaleX, data.endScaleY, 1f);
		string empty2 = string.Empty;
		if (data.None_s)
		{
			empty2 = "none_s";
		}
		else if (data.Once_s)
		{
			empty2 = "once_s";
		}
		else if (data.PingPong_s)
		{
			empty2 = "ping_s";
		}
		float num3 = 1f;
		float num4 = 0f;
		num3 = ((data.duration_s != 0f) ? data.duration_s : num3);
		num4 = data.startDelay_s;
		Vector3 startRot = new Vector3(data.startRotationX, data.startRotationY, data.startRotationZ);
		Vector3 endRot = new Vector3(data.endRotationX, data.endRotationY, data.endRotationZ);
		float num5 = 1f;
		float num6 = 0f;
		num5 = ((data.duration_r != 0f) ? data.duration_r : num5);
		num6 = data.startDelay_r;
		string option_r = string.Empty;
		if (data.None_r)
		{
			option_r = "none_r";
		}
		else if (data.Once_r)
		{
			option_r = "once_r";
		}
		else if (data.PingPong_r)
		{
			option_r = "ping_r";
		}
		string[] array = data.portrait.Split('/');
		if (array != null)
		{
			empty = array[0];
			if (array.Length == 2)
			{
				spineScale = array[1];
			}
		}
		else
		{
			empty = "c_001";
		}
		currSpeaker = data.name;
		if (strEnableImage.Equals("Left"))
		{
			if (data.name == "1005000")
			{
				_nameText = RemoteObjectManager.instance.localUser.nickname;
			}
			else
			{
				_nameText = Localization.Get(data.name);
			}
			UISetter.SetLabel(lbLeftName, _nameText);
			goCenter.SetActive(value: false);
			goIntro.SetActive(value: false);
			goLeftSpineData.SetActive(value: true);
			goRightSpineData.SetActive(value: false);
		}
		else if (strEnableImage.Equals("Right"))
		{
			if (data.name == "1005000")
			{
				_nameText = RemoteObjectManager.instance.localUser.nickname;
			}
			else
			{
				_nameText = Localization.Get(data.name);
			}
			UISetter.SetLabel(lbRightName, _nameText);
			goCenter.SetActive(value: false);
			goIntro.SetActive(value: false);
			goLeftSpineData.SetActive(value: false);
			goRightSpineData.SetActive(value: true);
		}
		else if (strEnableImage.Equals("Center"))
		{
			if (data.name == "1005000")
			{
				_nameText = RemoteObjectManager.instance.localUser.nickname;
			}
			else
			{
				_nameText = Localization.Get(data.name);
			}
			UISetter.SetLabel(lbCenterName, _nameText);
			goCenter.SetActive(value: true);
			goIntro.SetActive(value: false);
			goLeftSpineData.SetActive(value: false);
			goRightSpineData.SetActive(value: false);
		}
		else
		{
			_nameText = data.name;
			goCenter.SetActive(value: false);
			goIntro.SetActive(value: true);
			goLeftSpineData.SetActive(value: false);
			goRightSpineData.SetActive(value: false);
			if (string.IsNullOrEmpty(empty) && rootSpineData.target != null)
			{
				rootSpineData.target.SetActive(value: false);
			}
		}
		SetSpine(empty, text, strEnableImage, startPos, endPos, option_p, num, num2, startRot, endRot, option_r, num5, num6);
		if (shakeTime > 0f)
		{
			uiShake.BeginScenarioSceen(shakeTime, shakeMin, shakeMax);
			shakeTime = 0f;
			shakeMin = 0f;
			shakeMax = 0f;
		}
		rootSpineData.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
		_showDialogueBox = true;
		base.gameObject.SetActive(_showDialogueBox);
		_isBranchedText = data.windowType == DialoguerTextPhaseType.BranchedText;
		_branchedTextChoices = data.choices;
		_currentChoice = 0;
		if (data.theme != _theme)
		{
			resetWindowSize();
		}
		_theme = data.theme;
		voiceClip = data.VoiceClip;
		updateText();
		_windowReady = true;
		effInfoList.Clear();
	}

	private void onDialogueWindowCloseHandler()
	{
		startWindowTweenOut();
	}

	private void onDialoguerMessageEvent(string message, string metadata, float duration, float r, float g, float b, float a)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		metadata = StringCheck(metadata);
		switch (message)
		{
		case "battle":
		{
			BattleData battleData = BattleData.Create(EBattleType.ScenarioBattle);
			battleData.stageId = metadata;
			List<ScenarioBattleUnitDataRow> list = regulation.FindScenarioBattleUnitInfo(metadata, 3);
			List<RoLocalUser.ScenarioBattleInfo> list2 = ConvertScenarioBattleInfo(regulation.FindScenarioBattleUnitInfo(metadata, 1));
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					RoCommander roCommander = localUser.FindCommanderByUnitId(list[i].unitId);
					if (roCommander != null)
					{
						RoLocalUser.ScenarioBattleInfo item = default(RoLocalUser.ScenarioBattleInfo);
						item._battleIdx = list[i].battleIdx;
						item._unitId = list[i].unitId;
						item._unitGrade = roCommander.rank;
						item._unitClass = roCommander.cls;
						item._unitLevel = roCommander.level;
						item._skillLevel = roCommander.skillList;
						item._unitPosition = list[i].unitPosition;
						item._uType = list[i].uType;
						list2.Add(item);
					}
				}
			}
			RoUser roUser = RoUser.CreateScenarioUser(list2);
			roUser.nickname = localUser.nickname;
			roUser.level = localUser.level;
			roUser.guildName = localUser.guildName;
			battleData.attacker = roUser;
			List<RoLocalUser.ScenarioBattleInfo> list3 = ConvertScenarioBattleInfo(regulation.FindScenarioBattleUnitInfo(metadata, 2));
			if (list3 != null && list3.Count > 0)
			{
				RoUser defender = RoUser.CreateScenarioUser(list3);
				battleData.defender = defender;
			}
			localUser.currScenario.scenarioQuarterId = int.Parse(regulation.FindScenarioBattleInfo(metadata));
			RemoteObjectManager.instance.RequestScenarioBattle(battleData);
			break;
		}
		case "scenarioEnd":
			if (isScenario)
			{
				RemoteObjectManager.instance.RequestCompleteCommanderScenario(localUser.currScenario.commanderId, localUser.currScenario.scenarioId, int.Parse(metadata));
			}
			break;
		case "fadein":
		{
			UISetter.SetActive(ScenarioTitle, active: false);
			SetBackground(metadata);
			float duration2 = 0.8f;
			if (duration > 0f)
			{
				duration2 = duration;
			}
			UIFade.InScenario(duration2, r, g, b, a);
			break;
		}
		case "fadeout":
		{
			float duration3 = 0.8f;
			if (duration > 0f)
			{
				duration3 = duration;
			}
			UIFade.OutScenario(duration3, r, g, b, a);
			break;
		}
		case "bgmOff":
			SoundManager.StopMusic();
			break;
		case "fontSize":
			if (!string.IsNullOrEmpty(metadata))
			{
				isChangefontSize = true;
				fontSize = int.Parse(metadata);
			}
			break;
		case "camShake":
			shakeTime = duration;
			shakeMin = r;
			shakeMax = g;
			break;
		case "spark":
		{
			float time = 0.05f;
			int count = 1;
			Color color = ((r != 0f || g != 0f || b != 0f) ? new Color(r, g, b) : new Color(255f, 255f, 255f));
			if (duration > 0f)
			{
				time = duration;
			}
			if (a > 0f)
			{
				count = (int)a;
			}
			StartCoroutine(spark(color, time, count));
			break;
		}
		case "weddingEvent":
			UISetter.SetActive(WeddingEvent, active: true);
			isWeddingEvent = true;
			AutoClick_Off();
			break;
		case "flowerOn":
			UISetter.SetActive(WeddingFlower, active: true);
			break;
		case "flowerOff":
			UISetter.SetActive(WeddingFlower, active: false);
			break;
		}
	}

	private IEnumerator spark(Color _color, float _time, int _count)
	{
		sparkBG.color = _color;
		for (int count = _count; count > 0; count--)
		{
			isSpark = true;
			UISetter.SetActive(sparkBG, active: true);
			yield return new WaitForSeconds(_time * 0.5f);
			UISetter.SetActive(sparkBG, active: false);
			yield return new WaitForSeconds(_time * 0.5f);
		}
		isSpark = false;
		yield return null;
	}

	private List<RoLocalUser.ScenarioBattleInfo> ConvertScenarioBattleInfo(List<ScenarioBattleUnitDataRow> battleData)
	{
		if (battleData == null)
		{
			return null;
		}
		List<RoLocalUser.ScenarioBattleInfo> list = new List<RoLocalUser.ScenarioBattleInfo>();
		for (int i = 0; i < battleData.Count; i++)
		{
			RoLocalUser.ScenarioBattleInfo item = default(RoLocalUser.ScenarioBattleInfo);
			item._battleIdx = battleData[i].battleIdx;
			item._unitId = battleData[i].unitId;
			item._unitGrade = battleData[i].unitGrade;
			item._unitClass = battleData[i].unitClass;
			item._unitLevel = battleData[i].unitLevel;
			item._skillLevel = battleData[i].skillLevel;
			item._unitPosition = battleData[i].unitPosition;
			item._uType = battleData[i].uType;
			list.Add(item);
		}
		return list;
	}

	private void startWindowTweenIn()
	{
		_showDialogueBox = true;
		base.gameObject.SetActive(_showDialogueBox);
		iTween.ValueTo(base.gameObject, new Hashtable
		{
			{ "from", _windowTweenValue },
			{ "to", 1 },
			{ "onupdatetarget", base.gameObject },
			{ "onupdate", "updateWindowTweenValue" },
			{ "oncompletetarget", base.gameObject },
			{ "oncomplete", "windowInComplete" },
			{ "time", 0.5f },
			{
				"easetype",
				iTween.EaseType.easeOutBack
			}
		});
	}

	private void startWindowTweenOut()
	{
	}

	private void updateWindowTweenValue(float newValue)
	{
		_windowTweenValue = newValue;
	}

	private void windowInComplete()
	{
		_windowReady = true;
	}

	private void windowOutComplete()
	{
		_showDialogueBox = false;
		base.gameObject.SetActive(_showDialogueBox);
		resetWindowSize();
		_dialogue = false;
	}

	private void resetWindowSize()
	{
		_windowTweenValue = 0f;
		_windowReady = false;
	}

	private void Update()
	{
		if (isWeddingEvent)
		{
			return;
		}
		if (!_dialogue || isWindowHide)
		{
			if ((isScenario || isEvent || isInfinity) && !isWindowHide)
			{
			}
			return;
		}
		if (_windowReady && !scenarioLog.root.activeSelf && !SettingWindow.activeSelf)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				OnClick();
			}
			calculateText();
		}
		if (isScenarioStart)
		{
			runtime += Time.deltaTime;
			if (runtime >= 2f)
			{
				isScenarioStart = false;
				runtime = 0f;
				Dialoguer.ContinueDialogue(0);
			}
			calculateText();
		}
	}

	public void OnClick()
	{
		if (!_dialogue || isWindowHide || Input.GetMouseButtonUp(1) || uiShake.IsShake() || isWeddingEvent)
		{
			return;
		}
		if (!_isBranchedText)
		{
			if (_windowCurrentText == _windowTargetText)
			{
				DestroyEffect();
				Dialoguer.ContinueDialogue(0);
			}
			else
			{
				_windowCurrentText = _windowTargetText;
				updateText();
			}
		}
		else if (_windowCurrentText != _windowTargetText)
		{
			_windowCurrentText = _windowTargetText;
			updateText();
			if (_isBranchedText && _windowCurrentText == _windowTargetText)
			{
				CreateDialogueBox();
			}
		}
		if (_windowCurrentText == _windowTargetText)
		{
			ScenarioLogInfo item = default(ScenarioLogInfo);
			if (!string.IsNullOrEmpty(currSpeaker))
			{
				CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl.Find((CommanderDataRow row) => row.S_Idx == currSpeaker);
				if (commanderDataRow != null)
				{
					item.Id = commanderDataRow.id;
				}
				else
				{
					item.Id = "0";
				}
			}
			item.name = _nameText;
			item.scenarioContent = scenarioTextLocalKey;
			item.isScenario = isScenario;
			item.isEvent = isEvent;
			item.isInfinity = isInfinity;
			item.userNickName = ((!isContainUserNickname) ? null : RemoteObjectManager.instance.localUser.nickname);
			if (!logInfoList.Contains(item))
			{
				logInfoList.Add(item);
			}
			if (currEffSound != null && currVoice.isPlaying)
			{
				currEffSound.Stop();
				currEffSound = null;
			}
		}
		AutoClick_Off();
	}

	public void LogClick_On()
	{
		if (_windowCurrentText != _windowTargetText)
		{
			_windowCurrentText = _windowTargetText;
			updateText();
			ScenarioLogInfo item = default(ScenarioLogInfo);
			if (!string.IsNullOrEmpty(currSpeaker))
			{
				CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl.Find((CommanderDataRow row) => row.S_Idx == currSpeaker);
				if (commanderDataRow != null)
				{
					item.Id = commanderDataRow.id;
				}
				else
				{
					item.Id = "0";
				}
			}
			item.name = _nameText;
			item.scenarioContent = scenarioTextLocalKey;
			item.isScenario = isScenario;
			item.isEvent = isEvent;
			item.isInfinity = isInfinity;
			item.userNickName = ((!isContainUserNickname) ? null : RemoteObjectManager.instance.localUser.nickname);
			if (!logInfoList.Contains(item))
			{
				logInfoList.Add(item);
			}
		}
		AutoClick_Off();
		scenarioLog.InitAndOpen();
	}

	public void LogClick_Off()
	{
		scenarioLog.CloseLog();
	}

	private void AutoClick_OnOff()
	{
		if (!isAuto)
		{
			AutoClick_On();
		}
		else
		{
			AutoClick_Off();
		}
	}

	public void AutoClick_On()
	{
		isAuto = true;
		UISetter.SetActive(AutoOn, active: true);
		UISetter.SetActive(AutoOff, active: false);
	}

	public void AutoClick_Off()
	{
		isAuto = false;
		isGoingProgressing = false;
		UISetter.SetActive(AutoOn, active: false);
		UISetter.SetActive(AutoOff, active: true);
	}

	private void updateText()
	{
		if (strEnableImage.Equals("Left"))
		{
			lbLeftWndCurText.text = _windowCurrentText;
		}
		else if (strEnableImage.Equals("Right"))
		{
			lbRightWndCurText.text = _windowCurrentText;
		}
		else if (strEnableImage.Equals("Center"))
		{
			lbCenterWndCurText.text = _windowCurrentText;
		}
		else
		{
			lbIntroWndCurText.text = _windowCurrentText;
		}
	}

	private void calculateText()
	{
		if (uiShake.IsShake())
		{
			return;
		}
		if (!string.IsNullOrEmpty(voiceClip) && !isSpeedUp && currVoice == null)
		{
			currVoice = SoundManager.PlayVoice(voiceClip);
		}
		if ((_windowTargetText == string.Empty || _windowCurrentText == _windowTargetText) && !_isBranchedText)
		{
			if (!isTutorial && !isGoingProgressing && isAuto && !isSpark)
			{
				StartCoroutine(AutoNextDialogue());
			}
			return;
		}
		int num = 2;
		if (_textFrames < num)
		{
			_textFrames++;
			return;
		}
		_textFrames = 0;
		int num2 = 0;
		num2 = ((!isSpeedUp) ? 2 : 5);
		if (!(_windowCurrentText != _windowTargetText))
		{
			return;
		}
		for (int i = 0; i < num2; i++)
		{
			if (_windowTargetText.Length <= _windowCurrentText.Length)
			{
				break;
			}
			if (_windowTargetText[_windowCurrentText.Length] == '[')
			{
				do
				{
					_windowCurrentText += _windowTargetText[_windowCurrentText.Length];
				}
				while (_windowTargetText[_windowCurrentText.Length] != ']');
				_windowCurrentText += _windowTargetText[_windowCurrentText.Length];
			}
			else
			{
				_windowCurrentText += _windowTargetText[_windowCurrentText.Length];
			}
			SoundManager.PlaySFX("SE_KeyBoard_001");
			updateText();
		}
		if (_isBranchedText && _windowCurrentText == _windowTargetText)
		{
			CreateDialogueBox();
		}
		if (!(_windowCurrentText == _windowTargetText))
		{
			return;
		}
		ScenarioLogInfo item = default(ScenarioLogInfo);
		if (!string.IsNullOrEmpty(currSpeaker))
		{
			CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl.Find((CommanderDataRow row) => row.S_Idx == currSpeaker);
			if (commanderDataRow != null)
			{
				item.Id = commanderDataRow.id;
			}
			else
			{
				item.Id = "0";
			}
		}
		item.name = _nameText;
		item.scenarioContent = scenarioTextLocalKey;
		item.isScenario = isScenario;
		item.isEvent = isEvent;
		item.isInfinity = isInfinity;
		item.userNickName = ((!isContainUserNickname) ? null : RemoteObjectManager.instance.localUser.nickname);
		if (!logInfoList.Contains(item))
		{
			logInfoList.Add(item);
		}
	}

	private void CreateDialogueBox()
	{
		if (_branchedTextChoices.Length <= 0)
		{
			return;
		}
		box = UIDialogueBoxes._Create();
		if (!(box == null))
		{
			List<string> list = new List<string>();
			for (int i = 0; i < _branchedTextChoices.Length; i++)
			{
				list.Add(Localization.GetScenario(_branchedTextChoices[i]));
			}
			box.Init(list);
			if (strEnableImage.Equals("Left"))
			{
				box.transform.localPosition = new Vector3(290f, 200f, 0f);
			}
			else if (strEnableImage.Equals("Right"))
			{
				box.transform.localPosition = new Vector3(-290f, 200f, 0f);
			}
			else if (strEnableImage.Equals("Center"))
			{
				box.transform.localPosition = new Vector3(0f, 200f, 0f);
			}
		}
	}

	private void SetBackground(string bg_name)
	{
		Texture texture = Resources.Load(bgPrefix + bg_name) as Texture;
		UISetter.SetTexture(BG, texture);
	}

	private string StringCheck(string _str)
	{
		if (_str.Contains("\n"))
		{
			string[] array = _str.Split('\n');
			if (array.Length > 1)
			{
				return array[0];
			}
		}
		return _str;
	}

	private void SetTitleText()
	{
		if (string.IsNullOrEmpty(scenarioID))
		{
			return;
		}
		if (isScenario)
		{
			CommanderScenarioDataRow commanderScenarioDataRow = RemoteObjectManager.instance.regulation.FindScenarioInfo(int.Parse(scenarioID));
			if (commanderScenarioDataRow != null)
			{
				UISetter.SetLabel(text: (commanderScenarioDataRow.heart == 0) ? Localization.Get("20110") : ((commanderScenarioDataRow.heart == 2) ? Localization.Format("20111", commanderScenarioDataRow.order) : ((commanderScenarioDataRow.heart != 3) ? Localization.Format("20004", commanderScenarioDataRow.order) : Localization.Format("20112", commanderScenarioDataRow.order))), label: ScenarioTitle_Order);
				UISetter.SetLabel(ScenarioTitle_txt, Localization.Get(commanderScenarioDataRow.name));
			}
		}
		else if (isEvent)
		{
			EventBattleScenarioDataRow eventBattleScenarioDataRow = RemoteObjectManager.instance.regulation.eventBattleScenarioDtbl.Find((EventBattleScenarioDataRow row) => row.scenarioIdx == scenarioID);
			if (eventBattleScenarioDataRow != null)
			{
				UISetter.SetActive(ScenarioTitle_Order, active: true);
				UISetter.SetLabel(ScenarioTitle_Order, string.Format(Localization.Get("20004"), eventBattleScenarioDataRow.sort));
				UISetter.SetLabel(ScenarioTitle_txt, Localization.Get(eventBattleScenarioDataRow.title));
			}
		}
		else if (isInfinity)
		{
			InfinityFieldDataRow infinityFieldDataRow = RemoteObjectManager.instance.regulation.infinityFieldDtbl.Find((InfinityFieldDataRow row) => row.scenarioIdx == int.Parse(scenarioID));
			if (infinityFieldDataRow != null)
			{
				UISetter.SetActive(ScenarioTitle_Order, active: false);
				UISetter.SetLabel(ScenarioTitle_Order, string.Format(Localization.Get("20004"), infinityFieldDataRow.infinityFieldIdx));
				UISetter.SetLabel(ScenarioTitle_txt, Localization.Get(infinityFieldDataRow.name));
			}
		}
	}

	private void SetActiveGameObject_connectionScenario(bool isActive)
	{
		UISetter.SetActive(btnScenarioSkip, !isActive);
		UISetter.SetActive(bubbleSprite, !isActive);
		UISetter.SetActive(SpriteAni, !isActive);
		UISetter.SetActive(ScenarioTitle, isActive);
		UISetter.SetActive(btnAuto, !isActive);
		UISetter.SetActive(btnLog, !isActive);
		UISetter.SetActive(BtnWin, !isActive);
		UISetter.SetActive(BtnTextFast, !isActive);
		if (UIManager.instance.battle != null)
		{
			UISetter.SetActive(btnSetting, active: false);
		}
		else
		{
			UISetter.SetActive(btnSetting, !isActive);
		}
		if (isActive)
		{
			SetTitleText();
		}
	}

	private void SetFontSize(int fontSize)
	{
		lbLeftWndCurText.fontSize = fontSize;
		lbRightWndCurText.fontSize = fontSize;
		lbCenterWndCurText.fontSize = fontSize;
	}

	private void ResetFontSize()
	{
		lbLeftWndCurText.fontSize = 28;
		lbRightWndCurText.fontSize = 28;
		lbCenterWndCurText.fontSize = 28;
	}

	private void SetSpine(string strSpine, string strEmotion, string appearPos, Vector3 startPos, Vector3 endPos, string option_p, float duration_p, float startDelay_p, Vector3 startRot, Vector3 endRot, string option_r, float duration_r, float startDelay_r)
	{
		if (!string.IsNullOrEmpty(strSpine))
		{
			if (!dicSpineData.ContainsKey(strSpine))
			{
				SkeletonAnimation skeletonAnimation = null;
				skeletonAnimation = CacheManager.instance.SpineCache.Create<SkeletonAnimation>(strSpine, rootSpineData.transform);
				if (!(skeletonAnimation != null))
				{
					return;
				}
				dicSpineData.Add(strSpine, skeletonAnimation);
				if (strEmotion != spineEmotion || curSpine != strSpine)
				{
					spineEmotion = strEmotion;
					curSpine = strSpine;
					skeletonAnimation.state.SetAnimation(0, strEmotion, loop: true);
				}
				RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommanderResourceId(strSpine);
				if (roCommander != null)
				{
					if (roCommander.isBasicCostume)
					{
						skeletonAnimation.skeleton.SetSkin(roCommander.currentViewCostume);
					}
					else if (skeletonAnimation.skeleton.data.FindSkin(roCommander.getCurrentCostumeName()) != null)
					{
						skeletonAnimation.skeleton.SetSkin(roCommander.getCurrentCostumeName());
					}
					skeletonAnimation.skeleton.SetSlotsToSetupPose();
				}
				UIInteraction component = skeletonAnimation.GetComponent<UIInteraction>();
				if (component != null)
				{
					component.EnableInteration = false;
					component.enabled = false;
					if (component.spineEmoticon != null)
					{
						component.spineEmoticon.gameObject.SetActive(value: false);
					}
				}
				rootSpineData.gameObject.transform.localPosition = new Vector3(0f, -60f, 0f);
				rootSpineData.gameObject.transform.localScale = Vector3.one;
				rootSpineData.gameObject.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
				float y = skeletonAnimation.gameObject.transform.localPosition.y;
				switch (appearPos)
				{
				case "Left":
					skeletonAnimation.skeleton.FlipX = false;
					skeletonAnimation.gameObject.transform.localPosition = new Vector3(-400f, -250f + y, 0f);
					break;
				case "Right":
					skeletonAnimation.skeleton.FlipX = true;
					skeletonAnimation.gameObject.transform.localPosition = new Vector3(400f, -250f + y, 0f);
					break;
				case "Center":
					skeletonAnimation.skeleton.FlipX = false;
					skeletonAnimation.gameObject.transform.localPosition = new Vector3(0f, -250f + y, 0f);
					break;
				}
				rootSpineData.transform.localScale = Vector3.one;
				if (spineScale == "increase")
				{
					Vector3 zero = Vector3.zero;
					zero.x = skeletonAnimation.gameObject.transform.localScale.x * 1.5f;
					zero.y = skeletonAnimation.gameObject.transform.localScale.y * 1.5f;
					rootSpineData.transform.localPosition = new Vector3(0f, -240f, 0f);
					rootSpineData.transform.localScale = zero;
				}
				if (rootSpineData.target != null)
				{
					rootSpineData.target.SetActive(value: false);
				}
				skeletonAnimation.gameObject.SetActive(value: true);
				rootSpineData.target = skeletonAnimation.gameObject;
				TweenPosition tweenPosition = rootSpineData.gameObject.GetComponent<TweenPosition>();
				if (tweenPosition != null)
				{
					tweenPosition.ResetToBeginning();
					tweenPosition.enabled = false;
					rootSpineData.gameObject.transform.localPosition = new Vector3(0f, -60f, 0f);
				}
				switch (option_p)
				{
				case "none_p":
					rootSpineData.gameObject.transform.localPosition = startPos;
					break;
				case "once_p":
				case "ping_p":
					if (tweenPosition == null)
					{
						tweenPosition = rootSpineData.gameObject.AddComponent<TweenPosition>();
					}
					tweenPosition.enabled = true;
					tweenPosition.ResetToBeginning();
					tweenPosition.from = startPos;
					tweenPosition.to = endPos;
					if (option_p == "once_p")
					{
						tweenPosition.style = UITweener.Style.Once;
					}
					else
					{
						tweenPosition.style = UITweener.Style.PingPong;
					}
					tweenPosition.duration = duration_p;
					tweenPosition.delay = startDelay_p;
					break;
				}
				TweenRotation tweenRotation = rootSpineData.gameObject.GetComponent<TweenRotation>();
				if (tweenRotation != null)
				{
					tweenRotation.ResetToBeginning();
					tweenRotation.enabled = false;
					rootSpineData.gameObject.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
				}
				switch (option_r)
				{
				case "none_r":
					rootSpineData.gameObject.transform.localRotation = new Quaternion(startRot.x, startRot.y, startRot.z, 0f);
					break;
				case "once_r":
				case "ping_r":
					if (tweenRotation == null)
					{
						tweenRotation = rootSpineData.gameObject.AddComponent<TweenRotation>();
					}
					tweenRotation.enabled = true;
					tweenRotation.ResetToBeginning();
					tweenRotation.from = startRot;
					tweenRotation.to = endRot;
					if (option_r == "once_r")
					{
						tweenRotation.style = UITweener.Style.Once;
					}
					else
					{
						tweenRotation.style = UITweener.Style.PingPong;
					}
					tweenRotation.duration = duration_r;
					tweenRotation.delay = startDelay_r;
					break;
				}
				return;
			}
			SkeletonAnimation skeletonAnimation2 = null;
			skeletonAnimation2 = dicSpineData[strSpine];
			if (strEmotion != spineEmotion || curSpine != strSpine)
			{
				RoCommander roCommander2 = RemoteObjectManager.instance.localUser.FindCommanderResourceId(strSpine);
				if (roCommander2 != null)
				{
					if (roCommander2.isBasicCostume)
					{
						skeletonAnimation2.skeleton.SetSkin(roCommander2.currentViewCostume);
					}
					else if (skeletonAnimation2.skeleton.data.FindSkin(roCommander2.getCurrentCostumeName()) != null)
					{
						skeletonAnimation2.skeleton.SetSkin(roCommander2.getCurrentCostumeName());
					}
					skeletonAnimation2.skeleton.SetSlotsToSetupPose();
				}
				spineEmotion = strEmotion;
				curSpine = strSpine;
				skeletonAnimation2.state.SetAnimation(0, strEmotion, loop: true);
			}
			rootSpineData.gameObject.transform.localPosition = new Vector3(0f, -60f, 0f);
			rootSpineData.gameObject.transform.localScale = Vector3.one;
			rootSpineData.gameObject.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
			float y2 = skeletonAnimation2.gameObject.transform.localPosition.y;
			switch (appearPos)
			{
			case "Left":
				skeletonAnimation2.skeleton.FlipX = false;
				skeletonAnimation2.gameObject.transform.localPosition = new Vector3(-400f, y2, 0f);
				break;
			case "Right":
				skeletonAnimation2.skeleton.FlipX = true;
				skeletonAnimation2.gameObject.transform.localPosition = new Vector3(400f, y2, 0f);
				break;
			case "Center":
				skeletonAnimation2.skeleton.FlipX = false;
				skeletonAnimation2.gameObject.transform.localPosition = new Vector3(0f, y2, 0f);
				break;
			}
			rootSpineData.transform.localScale = Vector3.one;
			if (spineScale == "increase")
			{
				Vector3 zero2 = Vector3.zero;
				zero2.x = skeletonAnimation2.gameObject.transform.localScale.x * 1.5f;
				zero2.y = skeletonAnimation2.gameObject.transform.localScale.y * 1.5f;
				rootSpineData.transform.localPosition = new Vector3(0f, -240f, 0f);
				rootSpineData.transform.localScale = zero2;
			}
			if (rootSpineData.target != null)
			{
				rootSpineData.target.SetActive(value: false);
			}
			skeletonAnimation2.gameObject.SetActive(value: true);
			rootSpineData.target = skeletonAnimation2.gameObject;
			TweenPosition tweenPosition2 = rootSpineData.gameObject.GetComponent<TweenPosition>();
			if (tweenPosition2 != null)
			{
				tweenPosition2.ResetToBeginning();
				tweenPosition2.enabled = false;
				rootSpineData.gameObject.transform.localPosition = new Vector3(0f, -60f, 0f);
			}
			switch (option_p)
			{
			case "none_p":
				rootSpineData.gameObject.transform.localPosition = startPos;
				break;
			case "once_p":
			case "ping_p":
				if (tweenPosition2 == null)
				{
					tweenPosition2 = rootSpineData.gameObject.AddComponent<TweenPosition>();
				}
				tweenPosition2.enabled = true;
				tweenPosition2.ResetToBeginning();
				tweenPosition2.from = startPos;
				tweenPosition2.to = endPos;
				if (option_p == "once_p")
				{
					tweenPosition2.style = UITweener.Style.Once;
				}
				else
				{
					tweenPosition2.style = UITweener.Style.PingPong;
				}
				tweenPosition2.duration = duration_p;
				tweenPosition2.delay = startDelay_p;
				break;
			}
			TweenRotation tweenRotation2 = rootSpineData.gameObject.GetComponent<TweenRotation>();
			if (tweenRotation2 != null)
			{
				tweenRotation2.ResetToBeginning();
				tweenRotation2.enabled = false;
				rootSpineData.gameObject.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
			}
			switch (option_r)
			{
			case "none_r":
				rootSpineData.gameObject.transform.localRotation = new Quaternion(startRot.x, startRot.y, startRot.z, 0f);
				break;
			case "once_r":
			case "ping_r":
				if (tweenRotation2 == null)
				{
					tweenRotation2 = rootSpineData.gameObject.AddComponent<TweenRotation>();
				}
				tweenRotation2.enabled = true;
				tweenRotation2.ResetToBeginning();
				tweenRotation2.from = startRot;
				tweenRotation2.to = endRot;
				if (option_r == "once_r")
				{
					tweenRotation2.style = UITweener.Style.Once;
				}
				else
				{
					tweenRotation2.style = UITweener.Style.PingPong;
				}
				tweenRotation2.duration = duration_r;
				tweenRotation2.delay = startDelay_r;
				break;
			}
		}
		else if (rootSpineData.target != null)
		{
			rootSpineData.target.SetActive(value: false);
		}
	}

	private void InitEffectInfoList(string effName, bool isSprite, Vector3 startPos, Vector3 endPos, bool none_p, bool once_p, bool ping_p, float duration_p, float delay_p, Vector3 startRot, Vector3 endRot, bool none_r, bool once_r, bool ping_r, float duration_r, float delay_r, Vector3 startScale, Vector3 endScale, bool none_s, bool once_s, bool ping_s, float duration_s, float delay_s, float lifeTime, string soundClip)
	{
		EffectInfo item = default(EffectInfo);
		item.effName = effName;
		item.isSprite = isSprite;
		TweenType type_p = TweenType.None;
		if (none_p)
		{
			type_p = TweenType.None;
		}
		else if (once_p)
		{
			type_p = TweenType.Once;
		}
		else if (ping_p)
		{
			type_p = TweenType.PingPong;
		}
		TweenType type_r = TweenType.None;
		if (none_r)
		{
			type_r = TweenType.None;
		}
		else if (once_r)
		{
			type_r = TweenType.Once;
		}
		else if (ping_r)
		{
			type_r = TweenType.PingPong;
		}
		TweenType type_s = TweenType.None;
		if (none_s)
		{
			type_s = TweenType.None;
		}
		else if (once_s)
		{
			type_s = TweenType.Once;
		}
		else if (ping_s)
		{
			type_s = TweenType.PingPong;
		}
		item.type_p = type_p;
		item.type_r = type_r;
		item.type_s = type_s;
		item.startPos = startPos;
		item.endPos = endPos;
		item.duration_p = duration_p;
		item.delay_p = delay_p;
		item.startRot = startRot;
		item.endRot = endRot;
		item.duration_r = duration_r;
		item.delay_r = delay_r;
		item.startScale = startScale;
		item.endScale = endScale;
		item.duration_s = duration_s;
		item.delay_s = delay_s;
		item.lifeTime = lifeTime;
		item.effSound = soundClip;
		effInfoList.Add(item);
	}

	private void SetEffect(EffectInfo info)
	{
		if (!string.IsNullOrEmpty(info.effName))
		{
			GameObject gameObject = null;
			if (gameObject == null)
			{
				if (info.isSprite)
				{
					gameObject = Object.Instantiate(Resources.Load(EffPath + "SpriteEff") as GameObject);
					UISprite component = gameObject.GetComponent<UISprite>();
					if (component != null)
					{
						UISetter.SetSprite(component, info.effName);
					}
				}
				else
				{
					gameObject = Object.Instantiate(Resources.Load(EffPath + info.effName) as GameObject);
				}
				gameObject.transform.parent = EffectParent.transform;
			}
			TweenPosition tweenPosition = gameObject.GetComponent<TweenPosition>();
			if (tweenPosition == null)
			{
				tweenPosition = gameObject.AddComponent<TweenPosition>();
			}
			switch (info.type_p)
			{
			case TweenType.None:
				if (tweenPosition != null)
				{
					tweenPosition.enabled = false;
				}
				gameObject.transform.localPosition = info.startPos;
				break;
			case TweenType.Once:
			case TweenType.PingPong:
				tweenPosition.enabled = true;
				tweenPosition.ResetToBeginning();
				tweenPosition.from = info.startPos;
				tweenPosition.to = info.endPos;
				if (info.type_p == TweenType.Once)
				{
					tweenPosition.style = UITweener.Style.Once;
				}
				else
				{
					tweenPosition.style = UITweener.Style.PingPong;
				}
				tweenPosition.duration = info.duration_p;
				tweenPosition.delay = info.delay_p;
				break;
			}
			TweenScale tweenScale = gameObject.GetComponent<TweenScale>();
			if (tweenScale == null)
			{
				tweenScale = gameObject.AddComponent<TweenScale>();
			}
			switch (info.type_s)
			{
			case TweenType.None:
				if (tweenScale != null)
				{
					tweenScale.enabled = false;
				}
				if (info.startScale.x == 0f || info.startScale.y == 0f)
				{
					info.startScale = Vector3.one;
				}
				gameObject.transform.localScale = info.startScale;
				break;
			case TweenType.Once:
			case TweenType.PingPong:
				tweenScale.enabled = true;
				tweenScale.ResetToBeginning();
				tweenScale.from = info.startScale;
				tweenScale.to = info.endScale;
				if (info.type_s == TweenType.Once)
				{
					tweenScale.style = UITweener.Style.Once;
				}
				else
				{
					tweenScale.style = UITweener.Style.PingPong;
				}
				tweenScale.duration = info.duration_s;
				tweenScale.delay = info.delay_s;
				break;
			}
			TweenRotation tweenRotation = gameObject.GetComponent<TweenRotation>();
			if (tweenRotation == null)
			{
				tweenRotation = gameObject.AddComponent<TweenRotation>();
			}
			switch (info.type_r)
			{
			case TweenType.None:
				if (tweenRotation != null)
				{
					tweenRotation.enabled = false;
				}
				gameObject.transform.rotation = new Quaternion(info.startRot.x, info.startRot.y, info.startRot.z, 0f);
				break;
			case TweenType.Once:
			case TweenType.PingPong:
				tweenRotation.enabled = true;
				tweenRotation.ResetToBeginning();
				tweenRotation.from = info.startRot;
				tweenRotation.to = info.endRot;
				if (info.type_r == TweenType.Once)
				{
					tweenRotation.style = UITweener.Style.Once;
				}
				else
				{
					tweenRotation.style = UITweener.Style.PingPong;
				}
				tweenRotation.duration = info.duration_r;
				tweenRotation.delay = info.delay_r;
				break;
			}
			if (info.lifeTime > 0f)
			{
				StartCoroutine(EffectLifeTime(gameObject, info.lifeTime));
			}
		}
		if (!string.IsNullOrEmpty(info.effSound) && !isSpeedUp)
		{
			currEffSound = SoundManager.PlaySFX(info.effSound);
		}
	}

	private IEnumerator EffectLifeTime(GameObject obj, float lifeTime)
	{
		yield return new WaitForSeconds(lifeTime);
		Object.Destroy(obj);
	}

	private IEnumerator InitEffect(DialoguerTextData data)
	{
		InitEffectInfoList(data.createObjectPath, data.isSprite, new Vector3(data.EffstartPosX, data.EffstartPosY, 0f), new Vector3(data.EffEndPosX, data.EffEndPosY, 0f), data.EffNone_p, data.EffOnce_p, data.EffPingPong_p, data.EffDuration_p, data.EffStartDelay_p, new Vector3(data.EffStartRotationX, data.EffStartRotationY, data.EffStartRotationZ), new Vector3(data.EffEndRotationX, data.EffEndRotationY, data.EffEndRotationZ), data.EffNone_r, data.EffOnce_r, data.EffPingPong_r, data.EffDuration_r, data.EffStartDelay_r, new Vector3(data.EffStartScaleX, data.EffStartScaleY, 1f), new Vector3(data.EffEndScaleX, data.EffEndScaleY, 1f), data.EffNone_s, data.EffOnce_s, data.EffPingPong_s, data.EffDuration_s, data.EffStartDelay_s, data.EffLifeTime, data.EffSoundClip);
		yield return new WaitForSeconds(0.1f);
		Dialoguer.ContinueDialogue(0);
	}

	private IEnumerator AutoNextDialogue()
	{
		if (uiShake.IsShake() || (currVoice != null && currVoice.isPlaying && GameSetting.instance.voice) || (currEffSound != null && currEffSound.isPlaying && GameSetting.instance.effect) || isWindowHide)
		{
			yield return null;
			yield break;
		}
		isGoingProgressing = true;
		yield return new WaitForSeconds(2f);
		if (isWindowHide)
		{
			yield return new WaitUntil(() => !isWindowHide);
			yield return new WaitForSeconds(1f);
		}
		DestroyEffect();
		isGoingProgressing = false;
		Dialoguer.ContinueDialogue(0);
	}

	public List<ScenarioLogInfo> GetLogInfoList()
	{
		return logInfoList;
	}

	private void DestroyEffect()
	{
		if (EffectParent.transform.childCount > 0)
		{
			int childCount = EffectParent.transform.childCount;
			for (int num = childCount - 1; num >= 0; num--)
			{
				Object.Destroy(EffectParent.transform.GetChild(num).gameObject);
			}
		}
	}

	public void InitButton()
	{
		UISetter.SetActive(WindowOn, active: true);
		UISetter.SetActive(WindowOff, active: false);
		UISetter.SetActive(AutoOn, active: false);
		UISetter.SetActive(AutoOff, active: true);
		UISetter.SetActive(SpeedOn, active: false);
		UISetter.SetActive(SpeedOff, active: true);
	}

	public void WinClick_OnOff()
	{
		if (!isWindowHide)
		{
			WinClick_Hide();
		}
		else
		{
			WinClick_Show();
		}
	}

	public void WinClick_Hide()
	{
		WindowStatus(isOn: false);
		AutoClick_Off();
	}

	public void WinClick_Show()
	{
		WindowStatus(isOn: true);
	}

	public void WindowStatus(bool isOn)
	{
		if (_windowCurrentText != _windowTargetText)
		{
			OnClick();
		}
		if (strEnableImage.Equals("Left"))
		{
			UISetter.SetActive(lbCenterName, !isOn);
			UISetter.SetActive(lbCenterWndCurText, !isOn);
			UISetter.SetActive(lbLeftName, isOn);
			UISetter.SetActive(lbLeftWndCurText, isOn);
			UISetter.SetActive(lbRightName, !isOn);
			UISetter.SetActive(lbRightWndCurText, !isOn);
		}
		else if (strEnableImage.Equals("Right"))
		{
			UISetter.SetActive(lbCenterName, !isOn);
			UISetter.SetActive(lbCenterWndCurText, !isOn);
			UISetter.SetActive(lbLeftName, !isOn);
			UISetter.SetActive(lbLeftWndCurText, !isOn);
			UISetter.SetActive(lbRightName, isOn);
			UISetter.SetActive(lbRightWndCurText, isOn);
		}
		else if (strEnableImage.Equals("Center"))
		{
			UISetter.SetActive(lbCenterName, isOn);
			UISetter.SetActive(lbCenterWndCurText, isOn);
			UISetter.SetActive(lbLeftName, !isOn);
			UISetter.SetActive(lbLeftWndCurText, !isOn);
			UISetter.SetActive(lbRightName, !isOn);
			UISetter.SetActive(lbRightWndCurText, !isOn);
		}
		UISetter.SetActive(btnScenarioSkip, isOn);
		UISetter.SetActive(bubbleSprite, isOn);
		UISetter.SetActive(SpriteAni, isOn);
		UISetter.SetActive(btnAuto, isOn);
		UISetter.SetActive(btnLog, isOn);
		UISetter.SetActive(BtnTextFast, isOn);
		if (UIManager.instance.battle != null)
		{
			UISetter.SetActive(btnSetting, active: false);
		}
		else
		{
			UISetter.SetActive(btnSetting, isOn);
		}
		if (box != null)
		{
			UISetter.SetActive(box, isOn);
		}
		UISetter.SetActive(WindowOn, isOn);
		UISetter.SetActive(WindowOff, !isOn);
		isWindowHide = !isOn;
	}

	public void SpeedClick_On()
	{
		isSpeedUp = true;
		UISetter.SetActive(SpeedOn, active: true);
		UISetter.SetActive(SpeedOff, active: false);
		if (currVoice != null && currVoice.isPlaying)
		{
			currVoice.Stop();
			currVoice = null;
			voiceClip = null;
		}
		if (currEffSound != null && currEffSound.isPlaying)
		{
			currEffSound.Stop();
			currEffSound = null;
		}
	}

	public void SpeedClick_Off()
	{
		isSpeedUp = false;
		UISetter.SetActive(SpeedOn, active: false);
		UISetter.SetActive(SpeedOff, active: true);
	}

	public void OpenSetting()
	{
		UISetter.SetActive(SettingWindow, active: true);
		setting.Set(GameSetting.instance);
		preLanguage = PlayerPrefs.GetString("Language");
	}

	public void CloseSetting()
	{
		if (!setting.selLangPopup.isActive)
		{
			if (PlayerPrefs.GetString("Language") != preLanguage)
			{
				RemoteObjectManager.instance.RequestChangeLanguage();
			}
			UISetter.SetActive(SettingWindow, active: false);
		}
	}

	public void Log_ScrollUp()
	{
		if (scrollBar != null && scrollBar.value > 0f)
		{
			scrollBar.value -= 0.05f;
		}
	}

	public void Log_ScrollDown()
	{
		if (scrollBar != null && scrollBar.value < 1f)
		{
			scrollBar.value += 0.05f;
		}
	}

	public void EndWeddingEvent()
	{
		UISetter.SetActive(WeddingEvent, active: false);
		isWeddingEvent = false;
	}
}
