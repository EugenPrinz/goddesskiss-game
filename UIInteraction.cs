using System.Collections;
using System.Collections.Generic;
using Cache;
using Shared.Regulation;
using Spine.Unity;
using UnityEngine;

public class UIInteraction : MonoBehaviour
{
	private SkeletonAnimation character;

	private string characterName;

	public GameObject rootInteraction;

	public UISpineAnimation spineEmoticon;

	public GameObject talkBox;

	private UILabel talk;

	private string animation;

	private string lastAnimation;

	private float lastTouchTime;

	private int headTouchCount;

	private int bodyTouchCount;

	private int legTouchCount;

	private int MaxHeadTouchCount;

	private int MaxBodyTouchCount;

	private int MaxLegTouchCount;

	private List<string> ddabun;

	private List<string> ddabunVoice;

	private List<InteractionDataRow> giftData;

	private List<InteractionDataRow> commanderGiftData;

	public bool Enable = true;

	public static readonly int resetTime = 3;

	private ClickEffectHandler click;

	private AudioSource _curAudioSource;

	private List<string> _voices;

	private bool _enableVoice;

	private bool _pauseVoice;

	private bool _idleVoice = true;

	public static bool playVoice;

	private bool _mainDisplay;

	private bool _dateMode;

	private int _favorStep;

	private int _marry;

	public string commanderId { get; set; }

	public bool enableVoice
	{
		get
		{
			return _enableVoice;
		}
		set
		{
			_enableVoice = value;
		}
	}

	public bool pauseVoice
	{
		get
		{
			return _pauseVoice;
		}
		set
		{
			_pauseVoice = value;
			if (_pauseVoice && _curAudioSource != null)
			{
				_curAudioSource.Stop();
				_curAudioSource = null;
			}
		}
	}

	public bool idleVoice
	{
		get
		{
			return _idleVoice;
		}
		set
		{
			_idleVoice = value;
		}
	}

	public bool EnableInteration
	{
		get
		{
			if (rootInteraction != null)
			{
				return rootInteraction.activeSelf;
			}
			return false;
		}
		set
		{
			UISetter.SetActive(rootInteraction, value);
		}
	}

	public bool mainDisplay
	{
		get
		{
			return _mainDisplay;
		}
		set
		{
			_mainDisplay = value;
		}
	}

	public bool dateMode
	{
		get
		{
			return _dateMode;
		}
		set
		{
			_dateMode = value;
		}
	}

	public int favorStep
	{
		get
		{
			return _favorStep;
		}
		set
		{
			SetMaxTouchCount();
			_favorStep = value;
		}
	}

	public int marry
	{
		get
		{
			return _marry;
		}
		set
		{
			SetMaxTouchCount();
			SetDdabunText();
			_marry = value;
		}
	}

	private void Start()
	{
		UISetter.SetSpine(spineEmoticon, "npc_emoticon");
		characterName = base.gameObject.name.Replace("_SkeletonAnimation", string.Empty);
		characterName = characterName.Replace("(Clone)", string.Empty);
		character = base.gameObject.GetComponent<SkeletonAnimation>();
		talkBox = Utility.LoadAndInstantiateGameObject("Prefabs/UI/Talkbox", talkBox.transform);
		talk = talkBox.transform.Find("Label").GetComponent<UILabel>();
		if (character != null)
		{
			character.AnimationName = "a_01_idle1";
		}
		if (spineEmoticon != null && spineEmoticon.skeletonAnimation != null)
		{
			spineEmoticon.skeletonAnimation.GetComponent<MeshRenderer>().enabled = false;
		}
		talkBox.GetComponent<UISprite>().depth = base.gameObject.transform.parent.GetComponent<UISpineAnimation>().depth + 2;
		talk.depth = talkBox.GetComponent<UISprite>().depth + 1;
		spineEmoticon.depth = talkBox.GetComponent<UISprite>().depth;
		UISetter.SetActive(talkBox, active: false);
		Transform transform = base.gameObject.transform.Find("RootInteraction");
		if (transform != null)
		{
			transform.Find("Head").GetComponent<UISprite>().depth = talkBox.GetComponent<UISprite>().depth - 2;
			transform.Find("Body").GetComponent<UISprite>().depth = talkBox.GetComponent<UISprite>().depth - 2;
			transform.Find("Leg").GetComponent<UISprite>().depth = talkBox.GetComponent<UISprite>().depth - 2;
		}
		click = Object.FindObjectOfType(typeof(ClickEffectHandler)) as ClickEffectHandler;
		StartCoroutine("SetIdle", character.AnimationName);
		SetDdabunText();
		SetGfitData();
		SetMaxTouchCount();
	}

	private void OnDisable()
	{
		ResetAnimation();
		enableVoice = false;
		if (_voices != null && CacheManager.instance != null)
		{
			CacheManager.instance.SoundCache.CleanUp(_voices.ToArray());
			_voices.Clear();
		}
	}

	private void OnEnable()
	{
		if (character != null)
		{
			character.AnimationName = "a_01_idle1";
			StopCoroutine("StartIdle2");
			StartCoroutine("SetIdle", character.AnimationName);
		}
		enableVoice = true;
	}

	public void OnClick(GameObject sender)
	{
		if (Enable)
		{
			float time = Time.time;
			if ((float)resetTime < time - lastTouchTime)
			{
				headTouchCount = 0;
				bodyTouchCount = 0;
				legTouchCount = 0;
			}
			lastTouchTime = time;
			switch (sender.name)
			{
			case "Head":
				HeadTouch();
				break;
			case "Body":
				BodyTouch();
				break;
			case "Leg":
				LegTouch();
				break;
			}
		}
	}

	public void HeadTouch()
	{
		bodyTouchCount = 0;
		legTouchCount = 0;
		headTouchCount++;
		if (headTouchCount > MaxHeadTouchCount + 1)
		{
			headTouchCount = 1;
		}
		InteractionDataRow interactionDataRow = (mainDisplay ? FindInteractionFavorStepData(InteractionType.HEAD, headTouchCount) : ((!dateMode) ? RemoteObjectManager.instance.regulation.FindInteractionData(characterName, InteractionType.HEAD, headTouchCount) : FindInteractionFavorStepData(InteractionType.HEAD, headTouchCount, 1)));
		if (interactionDataRow != null)
		{
			animation = interactionDataRow.emotion;
			if (!(character.AnimationName == animation))
			{
				StopCoroutine("SetIdle");
				StopCoroutine("StartIdle2");
				character.AnimationName = animation;
				spineEmoticon.skeletonAnimation.AnimationName = interactionDataRow.emoticon;
				spineEmoticon.skeletonAnimation.GetComponent<MeshRenderer>().enabled = true;
				StartCoroutine("SetIdle", character.AnimationName);
				StopCoroutine("SetTalk");
				StartCoroutine("SetTalk", interactionDataRow.S_Idx);
				StopCoroutine("SetVoice");
				StartCoroutine("SetVoice", interactionDataRow.sound);
				ShowFavorUp(interactionDataRow.favorup);
			}
		}
	}

	public void BodyTouch()
	{
		headTouchCount = 0;
		legTouchCount = 0;
		bodyTouchCount++;
		if (bodyTouchCount > MaxBodyTouchCount + 1)
		{
			bodyTouchCount = 1;
		}
		InteractionDataRow interactionDataRow = (mainDisplay ? FindInteractionFavorStepData(InteractionType.BODY, bodyTouchCount) : ((!dateMode) ? RemoteObjectManager.instance.regulation.FindInteractionData(characterName, InteractionType.BODY, bodyTouchCount) : FindInteractionFavorStepData(InteractionType.BODY, bodyTouchCount, 1)));
		if (interactionDataRow != null)
		{
			animation = interactionDataRow.emotion;
			if (!(character.AnimationName == animation))
			{
				StopCoroutine("SetIdle");
				StopCoroutine("StartIdle2");
				character.AnimationName = animation;
				spineEmoticon.skeletonAnimation.AnimationName = interactionDataRow.emoticon;
				spineEmoticon.skeletonAnimation.GetComponent<MeshRenderer>().enabled = true;
				StartCoroutine("SetIdle", character.AnimationName);
				StopCoroutine("SetTalk");
				StartCoroutine("SetTalk", interactionDataRow.S_Idx);
				StopCoroutine("SetVoice");
				StartCoroutine("SetVoice", interactionDataRow.sound);
				ShowFavorUp(interactionDataRow.favorup);
			}
		}
	}

	public void LegTouch()
	{
		headTouchCount = 0;
		bodyTouchCount = 0;
		legTouchCount++;
		if (legTouchCount > MaxLegTouchCount + 1)
		{
			legTouchCount = 1;
		}
		InteractionDataRow interactionDataRow = (mainDisplay ? FindInteractionFavorStepData(InteractionType.LEG, legTouchCount) : ((!dateMode) ? RemoteObjectManager.instance.regulation.FindInteractionData(characterName, InteractionType.LEG, legTouchCount) : FindInteractionFavorStepData(InteractionType.LEG, legTouchCount, 1)));
		if (interactionDataRow != null)
		{
			animation = interactionDataRow.emotion;
			if (!(character.AnimationName == animation))
			{
				StopCoroutine("SetIdle");
				StopCoroutine("StartIdle2");
				character.AnimationName = animation;
				spineEmoticon.skeletonAnimation.AnimationName = interactionDataRow.emoticon;
				spineEmoticon.skeletonAnimation.GetComponent<MeshRenderer>().enabled = true;
				StartCoroutine("SetIdle", character.AnimationName);
				StopCoroutine("SetTalk");
				StartCoroutine("SetTalk", interactionDataRow.S_Idx);
				StopCoroutine("SetVoice");
				StartCoroutine("SetVoice", interactionDataRow.sound);
				ShowFavorUp(interactionDataRow.favorup);
			}
		}
	}

	private IEnumerator SetIdle(string name)
	{
		float duration2 = 1f;
		duration2 = character.skeletonDataAsset.GetSkeletonData(quiet: true).FindAnimation(name).duration;
		yield return new WaitForSeconds(duration2);
		character.AnimationName = "a_01_idle1";
		spineEmoticon.skeletonAnimation.GetComponent<MeshRenderer>().enabled = false;
		StopCoroutine("StartIdle2");
		StartCoroutine("StartIdle2");
	}

	public void ResetAnimation()
	{
		UISetter.SetActive(talkBox, active: false);
		if (character != null)
		{
			character.AnimationName = "a_01_idle1";
		}
		if (spineEmoticon != null && spineEmoticon.skeletonAnimation != null)
		{
			spineEmoticon.skeletonAnimation.GetComponent<MeshRenderer>().enabled = false;
		}
	}

	private IEnumerator SetTalk(string dialogue)
	{
		UISetter.SetActive(talkBox.gameObject.transform.parent.gameObject, active: true);
		UISetter.SetActive(talkBox, active: true);
		UISetter.SetLabel(talk, Localization.Get(dialogue));
		yield return new WaitForSeconds(2f);
		UISetter.SetActive(talkBox, active: false);
	}

	private IEnumerator SetVoice(string voice = "")
	{
		if (string.IsNullOrEmpty(voice))
		{
			yield break;
		}
		yield return null;
		if (!enableVoice || pauseVoice)
		{
			yield break;
		}
		if (_curAudioSource != null && _curAudioSource.isPlaying)
		{
			_curAudioSource.Stop();
		}
		_curAudioSource = SoundManager.PlayVoice(voice);
		if (_curAudioSource != null)
		{
			if (_voices == null)
			{
				_voices = new List<string>();
			}
			_voices.Add(voice);
		}
	}

	private void ShowFavorUp(int count)
	{
		if (!(click != null) || count <= 0)
		{
			return;
		}
		RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommander(commanderId);
		if (!string.IsNullOrEmpty(commanderId) && roCommander != null)
		{
			RoLocalUser localUser = RemoteObjectManager.instance.localUser;
			if ((int)roCommander.TempFaverUpCount < RemoteObjectManager.instance.regulation.FindVipExpData(localUser.vipLevel).favormax - localUser.todayFavorUpCount && (int)roCommander.TempFaverUpCount < roCommander.MaxFavorCount - (int)roCommander.FavorCount)
			{
				GameObject gameObject = Utility.LoadAndInstantiateGameObject("Prefabs/UI/FavorUp", base.gameObject.transform);
				gameObject.transform.position = click.clickPosition;
				gameObject.transform.Find("Count").GetComponent<UILabel>().text = "+" + count;
				TweenPosition component = gameObject.GetComponent<TweenPosition>();
				component.from = gameObject.transform.localPosition;
				component.to = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y + 50f);
				gameObject.GetComponent<UISprite>().depth = talkBox.GetComponent<UISprite>().depth - 1;
				gameObject.transform.Find("Count").GetComponent<UILabel>().depth = talkBox.GetComponent<UISprite>().depth - 1;
				Object.Destroy(gameObject, 1f);
				roCommander.TempFaverUpCount = (int)roCommander.TempFaverUpCount + count;
			}
		}
	}

	private void SetGfitData()
	{
		giftData = RemoteObjectManager.instance.regulation.FindInteractionData(characterName, InteractionType.GIFT);
		commanderGiftData = RemoteObjectManager.instance.regulation.FindInteractionData(characterName, InteractionType.COMMGIFT);
	}

	public void GiftInteraction(InteractionType interactionType)
	{
		InteractionDataRow interactionDataRow = ((interactionType != InteractionType.GIFT) ? commanderGiftData[Random.Range(0, commanderGiftData.Count)] : giftData[Random.Range(0, giftData.Count)]);
		if (interactionDataRow != null)
		{
			animation = interactionDataRow.emotion;
			if (!(character.AnimationName == animation))
			{
				StopCoroutine("SetIdle");
				StopCoroutine("StartIdle2");
				character.AnimationName = animation;
				spineEmoticon.skeletonAnimation.AnimationName = interactionDataRow.emoticon;
				spineEmoticon.skeletonAnimation.GetComponent<MeshRenderer>().enabled = true;
				StartCoroutine("SetIdle", character.AnimationName);
				StopCoroutine("SetTalk");
				StartCoroutine("SetTalk", interactionDataRow.S_Idx);
				StopCoroutine("SetVoice");
				StartCoroutine("SetVoice", interactionDataRow.sound);
			}
		}
	}

	public void VoiceBattle(CommanderVoiceDataRow voiceData)
	{
		if (voiceData != null)
		{
			if (voiceData.type == ECommanderVoiceEventType.ActiveSkill)
			{
				animation = "a_01_idle1";
			}
			else if (voiceData.type == ECommanderVoiceEventType.Passive1)
			{
				animation = "a_01_idle1";
			}
			else if (voiceData.type == ECommanderVoiceEventType.Passive2)
			{
				animation = "a_01_idle1";
			}
			else if (voiceData.type == ECommanderVoiceEventType.Fatal)
			{
				animation = "a_06_damage";
			}
			else if (voiceData.type == ECommanderVoiceEventType.Win)
			{
				animation = "e_04_pleasure";
			}
			else if (voiceData.type == ECommanderVoiceEventType.WinFatal)
			{
				animation = "a_05_lose";
			}
			else if (voiceData.type == ECommanderVoiceEventType.Lose)
			{
				animation = "a_05_lose";
			}
			else
			{
				animation = "a_01_idle1";
			}
			StopCoroutine("SetIdle");
			StopCoroutine("StartIdle2");
			character.AnimationName = animation;
			spineEmoticon.skeletonAnimation.GetComponent<MeshRenderer>().enabled = false;
			UISetter.SetActive(talkBox, active: false);
			StartCoroutine("SetIdle", character.AnimationName);
			StopCoroutine("SetTalk");
			StopCoroutine("SetVoice");
			StartCoroutine("SetVoice", voiceData.sound);
		}
	}

	public void VoiceInteraction(int idx)
	{
		InteractionDataRow interactionDataRow = RemoteObjectManager.instance.regulation.FindInteractionData(characterName, idx);
		if (interactionDataRow != null)
		{
			animation = interactionDataRow.emotion;
			StopCoroutine("SetIdle");
			StopCoroutine("StartIdle2");
			character.AnimationName = animation;
			spineEmoticon.skeletonAnimation.AnimationName = interactionDataRow.emoticon;
			spineEmoticon.skeletonAnimation.GetComponent<MeshRenderer>().enabled = true;
			StartCoroutine("SetIdle", character.AnimationName);
			StopCoroutine("SetTalk");
			StartCoroutine("SetTalk", interactionDataRow.S_Idx);
			StopCoroutine("SetVoice");
			StartCoroutine("SetVoice", interactionDataRow.sound);
		}
	}

	private void SetMaxTouchCount()
	{
		foreach (InteractionDataRow item in FindInteractionMaxTouchData(InteractionType.HEAD))
		{
			if (MaxHeadTouchCount < item.count)
			{
				MaxHeadTouchCount = item.count;
			}
		}
		foreach (InteractionDataRow item2 in FindInteractionMaxTouchData(InteractionType.BODY))
		{
			if (MaxBodyTouchCount < item2.count)
			{
				MaxBodyTouchCount = item2.count;
			}
		}
		foreach (InteractionDataRow item3 in FindInteractionMaxTouchData(InteractionType.LEG))
		{
			if (MaxLegTouchCount < item3.count)
			{
				MaxLegTouchCount = item3.count;
			}
		}
	}

	public void StartInteraction()
	{
		InteractionDataRow interactionDataRow = FindInteractionFavorStepData(InteractionType.START_FAVOR1, 0);
		if (interactionDataRow == null)
		{
			return;
		}
		animation = interactionDataRow.emotion;
		if (!(character.AnimationName == animation))
		{
			StopCoroutine("SetIdle");
			StopCoroutine("StartIdle2");
			character.AnimationName = animation;
			spineEmoticon.skeletonAnimation.AnimationName = interactionDataRow.emoticon;
			spineEmoticon.skeletonAnimation.GetComponent<MeshRenderer>().enabled = true;
			if (base.isActiveAndEnabled)
			{
				StartCoroutine("SetIdle", character.AnimationName);
				StopCoroutine("SetTalk");
				StartCoroutine("SetTalk", interactionDataRow.S_Idx);
				StopCoroutine("SetVoice");
				StartCoroutine("SetVoice", interactionDataRow.sound);
			}
		}
	}

	private void SetDdabunText()
	{
		if (ddabun == null)
		{
			ddabun = new List<string>();
		}
		else
		{
			ddabun.Clear();
		}
		if (ddabunVoice == null)
		{
			ddabunVoice = new List<string>();
		}
		else
		{
			ddabunVoice.Clear();
		}
		List<InteractionDataRow> list;
		if (dateMode)
		{
			list = RemoteObjectManager.instance.regulation.FindInteractionData(characterName, InteractionType.DATE_IDLE);
			if (marry == 1)
			{
				list?.AddRange(RemoteObjectManager.instance.regulation.FindInteractionData(characterName, InteractionType.DATE_IDLE_MARRY));
			}
		}
		else
		{
			list = RemoteObjectManager.instance.regulation.FindInteractionData(characterName, InteractionType.IDLE);
			if (marry == 1)
			{
				list?.AddRange(RemoteObjectManager.instance.regulation.FindInteractionData(characterName, InteractionType.IDLE_MARRY));
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			ddabun.Add(list[i].S_Idx);
			ddabunVoice.Add(list[i].sound);
		}
	}

	private IEnumerator StartIdle2()
	{
		float ddabunTime = character.skeletonDataAsset.GetSkeletonData(quiet: true).FindAnimation("a_01_idle1").duration * 3f;
		yield return new WaitForSeconds(ddabunTime);
		animation = "a_02_idle2";
		if (character.AnimationName == animation)
		{
			yield break;
		}
		StopCoroutine("SetIdle");
		character.AnimationName = animation;
		StartCoroutine("SetIdle", character.AnimationName);
		if (ddabun.Count > 0)
		{
			int index = Random.Range(0, ddabun.Count);
			StartCoroutine("SetTalk", ddabun[index]);
			if (idleVoice && playVoice)
			{
				StartCoroutine("SetVoice", ddabunVoice[index]);
			}
		}
	}

	private InteractionDataRow FindInteractionFavorStepData(InteractionType type, int count, int uiType = 0)
	{
		InteractionType type2 = InteractionType.IDLE;
		switch (uiType)
		{
		case 0:
			type2 = type switch
			{
				InteractionType.HEAD => (favorStep > 4) ? ((favorStep >= 5 && favorStep <= 8) ? InteractionType.HEAD_FAVOR2 : ((favorStep >= 9 && favorStep <= 12) ? InteractionType.HEAD_FAVOR3 : ((marry != 0) ? InteractionType.HEAD_FAVOR5 : InteractionType.HEAD_FAVOR4))) : InteractionType.HEAD_FAVOR1, 
				InteractionType.BODY => (favorStep > 4) ? ((favorStep >= 5 && favorStep <= 8) ? InteractionType.BODY_FAVOR2 : ((favorStep >= 9 && favorStep <= 12) ? InteractionType.BODY_FAVOR3 : ((marry != 0) ? InteractionType.BODY_FAVOR5 : InteractionType.BODY_FAVOR4))) : InteractionType.BODY_FAVOR1, 
				InteractionType.LEG => (favorStep > 4) ? ((favorStep >= 5 && favorStep <= 8) ? InteractionType.LEG_FAVOR2 : ((favorStep >= 9 && favorStep <= 12) ? InteractionType.LEG_FAVOR3 : ((marry != 0) ? InteractionType.LEG_FAVOR5 : InteractionType.LEG_FAVOR4))) : InteractionType.LEG_FAVOR1, 
				_ => (favorStep > 4) ? ((favorStep >= 5 && favorStep <= 8) ? InteractionType.START_FAVOR2 : ((favorStep >= 9 && favorStep <= 12) ? InteractionType.START_FAVOR3 : ((marry != 0) ? InteractionType.START_FAVOR5 : InteractionType.START_FAVOR4))) : InteractionType.START_FAVOR1, 
			};
			break;
		case 1:
			switch (type)
			{
			case InteractionType.HEAD:
				type2 = ((favorStep > 4) ? ((favorStep >= 5 && favorStep <= 8) ? InteractionType.DATE_HEAD_FAVOR2 : ((favorStep >= 9 && favorStep <= 12) ? InteractionType.DATE_HEAD_FAVOR3 : ((marry != 0) ? InteractionType.DATE_HEAD_FAVOR5 : InteractionType.DATE_HEAD_FAVOR4))) : InteractionType.DATE_HEAD_FAVOR1);
				break;
			case InteractionType.BODY:
				type2 = ((favorStep > 4) ? ((favorStep >= 5 && favorStep <= 8) ? InteractionType.DATE_BODY_FAVOR2 : ((favorStep >= 9 && favorStep <= 12) ? InteractionType.DATE_BODY_FAVOR3 : ((marry != 0) ? InteractionType.DATE_BODY_FAVOR5 : InteractionType.DATE_BODY_FAVOR4))) : InteractionType.DATE_BODY_FAVOR1);
				break;
			case InteractionType.LEG:
				type2 = ((favorStep > 4) ? ((favorStep >= 5 && favorStep <= 8) ? InteractionType.DATE_LEG_FAVOR2 : ((favorStep >= 9 && favorStep <= 12) ? InteractionType.DATE_LEG_FAVOR3 : ((marry != 0) ? InteractionType.DATE_LEG_FAVOR5 : InteractionType.DATE_LEG_FAVOR4))) : InteractionType.DATE_LEG_FAVOR1);
				break;
			}
			break;
		}
		return RemoteObjectManager.instance.regulation.FindInteractionData(characterName, type2, count);
	}

	private List<InteractionDataRow> FindInteractionMaxTouchData(InteractionType type)
	{
		if (mainDisplay)
		{
			InteractionType type2 = type switch
			{
				InteractionType.HEAD => (favorStep > 4) ? ((favorStep >= 5 && favorStep <= 8) ? InteractionType.HEAD_FAVOR2 : ((favorStep >= 9 && favorStep <= 12) ? InteractionType.HEAD_FAVOR3 : ((marry != 0) ? InteractionType.HEAD_FAVOR5 : InteractionType.HEAD_FAVOR4))) : InteractionType.HEAD_FAVOR1, 
				InteractionType.BODY => (favorStep > 4) ? ((favorStep >= 5 && favorStep <= 8) ? InteractionType.BODY_FAVOR2 : ((favorStep >= 9 && favorStep <= 12) ? InteractionType.BODY_FAVOR3 : ((marry != 0) ? InteractionType.BODY_FAVOR5 : InteractionType.BODY_FAVOR4))) : InteractionType.BODY_FAVOR1, 
				_ => (favorStep > 4) ? ((favorStep >= 5 && favorStep <= 8) ? InteractionType.LEG_FAVOR2 : ((favorStep >= 9 && favorStep <= 12) ? InteractionType.LEG_FAVOR3 : ((marry != 0) ? InteractionType.LEG_FAVOR5 : InteractionType.LEG_FAVOR4))) : InteractionType.LEG_FAVOR1, 
			};
			return RemoteObjectManager.instance.regulation.FindInteractionData(characterName, type2);
		}
		if (dateMode)
		{
			InteractionType type3 = type switch
			{
				InteractionType.HEAD => (favorStep > 4) ? ((favorStep >= 5 && favorStep <= 8) ? InteractionType.DATE_HEAD_FAVOR2 : ((favorStep >= 9 && favorStep <= 12) ? InteractionType.DATE_HEAD_FAVOR3 : ((marry != 0) ? InteractionType.DATE_HEAD_FAVOR5 : InteractionType.DATE_HEAD_FAVOR4))) : InteractionType.DATE_HEAD_FAVOR1, 
				InteractionType.BODY => (favorStep > 4) ? ((favorStep >= 5 && favorStep <= 8) ? InteractionType.DATE_BODY_FAVOR2 : ((favorStep >= 9 && favorStep <= 12) ? InteractionType.DATE_BODY_FAVOR3 : ((marry != 0) ? InteractionType.DATE_BODY_FAVOR5 : InteractionType.DATE_BODY_FAVOR4))) : InteractionType.DATE_BODY_FAVOR1, 
				_ => (favorStep > 4) ? ((favorStep >= 5 && favorStep <= 8) ? InteractionType.DATE_LEG_FAVOR2 : ((favorStep >= 9 && favorStep <= 12) ? InteractionType.DATE_LEG_FAVOR3 : ((marry != 0) ? InteractionType.DATE_LEG_FAVOR5 : InteractionType.DATE_LEG_FAVOR4))) : InteractionType.DATE_LEG_FAVOR1, 
			};
			return RemoteObjectManager.instance.regulation.FindInteractionData(characterName, type3);
		}
		return RemoteObjectManager.instance.regulation.FindInteractionData(characterName, type);
	}
}
