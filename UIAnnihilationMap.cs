using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnnihilationMap : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	public GEAnimNGUI AnimBottom;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public List<UIAnnihilationStage> stageList;

	public List<TweenAlpha> lineList;

	public UITimer resetTimer;

	public GameObject worldRoot;

	public GameObject timerRoot;

	public GameObject resetRoot;

	public GameObject resetBtn;

	public GameObject advancePartyBtn;

	public GameObject readyBtn;

	public TimeData resetTime;

	public UISpineAnimation spineAnimation;

	public TweenAlpha skullTween;

	public GameObject skullObject;

	public UISprite skull;

	public GameObject finishLabel;

	public UILabel stageName;

	public UILabel advanceLimit;

	[HideInInspector]
	public UIAdvancePartyPopup advancePopUp;

	private BattleData battleData;

	private UISecretShopPopup secretShop;

	private GameObject infoPopUp;

	private bool allClear;

	private bool goReady;

	private static int step = -1;

	public bool isPlayAdvanceParty;

	private int advanceStringIdx;

	[SerializeField]
	private UITexture BG_LevelColor;

	private AnnihilationMode curAnnihilationMode;

	[SerializeField]
	private GameObject NormalMakr;

	[SerializeField]
	private GameObject HardMakr;

	[SerializeField]
	private GameObject HellMakr;

	public bool isReset;

	public bool isPlay { get; set; }

	private void Start()
	{
	}

	protected override void Awake()
	{
		base.Awake();
		UISetter.SetSpine(spineAnimation, "n_002");
		int num = int.Parse(base.regulation.defineDtbl["ANNIHILATE_PILOT_CLASS_LIMIT"].value);
		advanceStringIdx = 8900 + num;
		advanceLimit.color = ((num != 1) ? Color.white : new Color(222f, 222f, 222f));
		advanceLimit.applyGradient = true;
		if (num < 2)
		{
			advanceLimit.gradientTop = Color.white;
			advanceLimit.gradientBottom = Color.white;
		}
		else if (num < 5)
		{
			advanceLimit.gradientTop = new Color(0.9843137f, 0.99607843f, 0.12156863f);
			advanceLimit.gradientBottom = new Color(0.35686275f, 74f / 85f, 0.015686275f);
		}
		else if (num < 9)
		{
			advanceLimit.gradientTop = new Color(0.12156863f, 0.99215686f, 0.99607843f);
			advanceLimit.gradientBottom = new Color(1f / 51f, 0.6509804f, 0.8745098f);
		}
		else if (num < 14)
		{
			advanceLimit.gradientTop = new Color(84f / 85f, 0.4745098f, 1f);
			advanceLimit.gradientBottom = new Color(0.78039217f, 0.2509804f, 1f);
		}
		else if (num < 19)
		{
			advanceLimit.gradientTop = new Color(1f, 0.9843137f, 0f);
			advanceLimit.gradientBottom = new Color(1f, 32f / 51f, 0f);
		}
		else
		{
			advanceLimit.gradientTop = new Color(1f, 39f / 85f, 14f / 85f);
			advanceLimit.gradientBottom = new Color(0.95686275f, 1f / 15f, 1f / 15f);
		}
	}

	private void SetAdvanceLimitLabel()
	{
		UISetter.SetLabel(advanceLimit, Localization.Get(advanceStringIdx.ToString()));
	}

	public void InitAndOpenAnnihilationMap(bool goReady)
	{
		if (bEnterKeyEnable && isReset)
		{
			step = GetLastClearStage();
			_Set();
			UISetter.SetActive(worldRoot, active: true);
		}
		else if (!bEnterKeyEnable)
		{
			this.goReady = goReady;
			bEnterKeyEnable = true;
			_Set();
			OpenPopupShow();
		}
		isReset = false;
	}

	public void SetEnemy(List<Dictionary<int, Protocols.AnnihilationMapInfo.CommanderData>> enemyList)
	{
		battleData = BattleData.Create(EBattleType.Annihilation);
		RoUser defender = RoUser.CreateAnnihilationEnemy(enemyList, base.localUser.lastClearAnnihilationStage);
		battleData.defender = defender;
	}

	public void SetTime(int duration, int clear)
	{
		if (resetTime == null)
		{
			resetTime = TimeData.Create();
		}
		resetTime.SetByDuration(duration);
		resetTimer.Set(resetTime);
		resetTimer.SetLabelFormat("(", ")");
		resetTimer.RegisterOnFinished(delegate
		{
			UISetter.SetActive(resetRoot, active: true);
			UISetter.SetActive(timerRoot, active: false);
		});
		UISetter.SetActive(resetRoot, duration <= 0);
		UISetter.SetActive(timerRoot, duration > 0);
		allClear = clear == 1;
		UISetter.SetButtonGray(resetBtn, duration <= 0);
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		switch (sender.name)
		{
		case "Close":
			if (!bBackKeyEnable)
			{
				ClosePopUp();
			}
			break;
		case "ReadyBtn":
			ReadyBtnConfirm();
			break;
		case "InfoBtn":
			if (infoPopUp == null)
			{
				UISimplePopup uISimplePopup = UISimplePopup.CreateOK("InformationPopup");
				uISimplePopup.Set(localization: true, "1913", "80032", string.Empty, "1001", string.Empty, string.Empty);
				infoPopUp = uISimplePopup.gameObject;
			}
			break;
		case "AdvancePartyBtn":
			if (!(advancePopUp == null))
			{
				break;
			}
			if (advancePartyBtn.GetComponent<UIButton>().isGray)
			{
				if (isPlayAdvanceParty)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("80026"));
				}
				else
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("80003"));
				}
			}
			else
			{
				advancePopUp = UIPopup.Create<UIAdvancePartyPopup>("AdvancePartyPopup");
				advancePopUp.SetData(curAnnihilationMode);
			}
			break;
		case "ResetBtn":
			StageAllClearCheck();
			break;
		case "ShopBtn":
			if (secretShop == null)
			{
				secretShop = UIPopup.Create<UISecretShopPopup>("SecretShopPopup");
				secretShop.Init(EShopType.AnnihilationShop);
			}
			break;
		}
	}

	public override void OnRefresh()
	{
		if (secretShop != null)
		{
			secretShop.OnRefresh();
		}
	}

	public void _Set()
	{
		if (step == -1)
		{
			step = GetLastClearStage();
		}
		for (int i = 0; i < stageList.Count; i++)
		{
			UIAnnihilationStage uIAnnihilationStage = stageList[i];
			if (!allClear)
			{
				if (step - 1 == i)
				{
					uIAnnihilationStage.Set(EAnnihilationStageType.Play);
				}
				else if (step - 1 > i)
				{
					uIAnnihilationStage.Set(EAnnihilationStageType.Clear);
				}
				else
				{
					uIAnnihilationStage.Set(EAnnihilationStageType.None);
				}
			}
			else
			{
				uIAnnihilationStage.Set(EAnnihilationStageType.Clear);
			}
			if (lineList[i] != null)
			{
				lineList[i].GetComponent<UISprite>().alpha = ((step - 1 <= i) ? 0.2f : 1f);
			}
		}
		ButtonControll();
		UISetter.SetActive(skullObject, !allClear);
		SetSkullIcon(step - 1);
		UISetter.SetActive(worldRoot, active: false);
		SetAdvanceLimitLabel();
		SetBg(curAnnihilationMode);
	}

	public void ButtonControll()
	{
		UISetter.SetButtonGray(advancePartyBtn, GetLastClearStage() <= 1 && !isPlay);
		UISetter.SetButtonGray(readyBtn, !allClear && base.localUser.GetAdvancePossibleCommanderCount() > 0);
	}

	private void SetSkullIcon(int idx)
	{
		if (!allClear && idx != -1)
		{
			UIAnnihilationStage uIAnnihilationStage = stageList[idx];
			skullObject.transform.position = uIAnnihilationStage.transform.position;
			UISetter.SetActive(skullObject, active: true);
			UISetter.SetActive(skull, (idx + 1) % 5 == 0);
			SetStageName(idx);
		}
	}

	private void SetStageName(int idx)
	{
		UISetter.SetLabel(stageName, Localization.Format("80021", idx + 1));
		UISetter.SetActive(finishLabel, idx != 19);
	}

	public void StageOpenAnimation()
	{
		if (step != GetLastClearStage())
		{
			StartCoroutine("StartStageOpenAnimation");
		}
	}

	private IEnumerator StartStageOpenAnimation()
	{
		GameObject uiRoot = UIRoot.list[0].gameObject;
		Transform timeMachine = uiRoot.transform.Find("TimeMachinePopup");
		while (timeMachine != null)
		{
			yield return null;
		}
		int move = base.localUser.lastClearAnnihilationStage - step;
		switch (curAnnihilationMode)
		{
		case AnnihilationMode.HARD:
			move -= 100;
			break;
		case AnnihilationMode.HELL:
			move -= 200;
			break;
		}
		UISetter.SetActive(skullObject, (move <= 0 && !allClear) ? true : false);
		stageList[step - 1].SetIcon(EAnnihilationStageType.Clear);
		stageList[step - 1].PlayFlagAnimation();
		for (int i = 0; i < move; i++)
		{
			int idx = step + i;
			lineList[idx - 1].Play(forward: true);
			yield return new WaitForSeconds(0.15f);
			if (idx + 1 == GetLastClearStage())
			{
				stageList[idx].SetIcon(EAnnihilationStageType.None);
				step = GetLastClearStage();
				SetSkullIcon(idx);
			}
			else
			{
				stageList[idx].SetIcon(EAnnihilationStageType.Clear);
				stageList[idx].PlayFlagAnimation();
			}
			yield return new WaitForSeconds(0.15f);
		}
		yield return null;
	}

	public void ResetAnnihilationMap()
	{
		step = base.localUser.lastClearAnnihilationStage;
		_Set();
		UISetter.SetActive(worldRoot, active: true);
	}

	private void StageAllClearCheck()
	{
		if (resetBtn.GetComponent<UIButton>().isGray)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("80017"));
			return;
		}
		if (allClear)
		{
			OpenModeSelectPopup();
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "80023", "80024", "1304", "1305").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				OpenModeSelectPopup();
			}
		};
	}

	public void ReadyBtnConfirm()
	{
		if (readyBtn.GetComponent<UIButton>().isGray)
		{
			if (base.localUser.GetAdvancePossibleCommanderCount() == 0)
			{
				int num = 8900 + int.Parse(base.regulation.defineDtbl["ANNIHILATE_PILOT_CLASS_LIMIT"].value);
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Format("80025", Localization.Get(num.ToString())));
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("80013"));
			}
		}
		else if (GetLastClearStage() == 1 && !isPlay)
		{
			UISimplePopup.CreateBool(localization: true, "1303", "80014", "80015", "1304", "1305").onClick = delegate(GameObject sender)
			{
				string text = sender.name;
				if (text == "OK")
				{
					battleData.stageId = base.localUser.lastClearAnnihilationStage.ToString();
					base.uiWorld.readyBattle.InitAndOpenReadyBattle(battleData);
				}
			};
		}
		else
		{
			battleData.stageId = base.localUser.lastClearAnnihilationStage.ToString();
			base.uiWorld.readyBattle.InitAndOpenReadyBattle(battleData);
		}
	}

	private void OpenPopupShow()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopUp()
	{
		bBackKeyEnable = true;
		step = GetLastClearStage();
		for (int i = 0; i < stageList.Count; i++)
		{
			UISetter.SetActive(stageList[i].flagAnimation, active: false);
		}
		StopCoroutine("StartStageOpenAnimation");
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.4f);
		UISetter.SetActive(worldRoot, active: false);
		yield return new WaitForSeconds(0.4f);
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
		AnimNpc.Reset();
		AnimTitle.Reset();
		AnimBlock.Reset();
		AnimBottom.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBottom.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		StartCoroutine("AnimOpenFinish");
	}

	private void OnAnimClose()
	{
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBottom.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private IEnumerator AnimOpenFinish()
	{
		yield return new WaitForSeconds(0.5f);
		UISetter.SetActive(worldRoot, active: true);
		if (step != base.localUser.lastClearAnnihilationStage)
		{
			yield return new WaitForSeconds(0.3f);
			yield return StartCoroutine("StartStageOpenAnimation");
		}
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
		if (goReady)
		{
			ReadyBtnConfirm();
		}
		yield return null;
	}

	protected override void OnEnablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: true);
	}

	protected override void OnDisablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: false);
	}

	public void SetMode(AnnihilationMode _mode)
	{
		curAnnihilationMode = _mode;
	}

	private void SetBg(AnnihilationMode mode)
	{
		switch (mode)
		{
		case AnnihilationMode.NORMAL:
			BG_LevelColor.color = new Color(0.38431373f, 0.6313726f, 0.47843137f);
			UISetter.SetActive(NormalMakr, active: true);
			UISetter.SetActive(HardMakr, active: false);
			UISetter.SetActive(HellMakr, active: false);
			break;
		case AnnihilationMode.HARD:
			BG_LevelColor.color = new Color(1f, 1f, 1f);
			UISetter.SetActive(NormalMakr, active: false);
			UISetter.SetActive(HardMakr, active: true);
			UISetter.SetActive(HellMakr, active: false);
			break;
		case AnnihilationMode.HELL:
			BG_LevelColor.color = new Color(1f, 0f, 0f);
			UISetter.SetActive(NormalMakr, active: false);
			UISetter.SetActive(HardMakr, active: false);
			UISetter.SetActive(HellMakr, active: true);
			break;
		}
	}

	public void OpenModeSelectPopup()
	{
		UIPopup.Create<UISelectAnnihilationModePopup>("ModeSelectPopup").InitAndOpen();
	}

	private int GetLastClearStage()
	{
		int lastClearAnnihilationStage = base.localUser.lastClearAnnihilationStage;
		return curAnnihilationMode switch
		{
			AnnihilationMode.HARD => lastClearAnnihilationStage - 100, 
			AnnihilationMode.HELL => lastClearAnnihilationStage - 200, 
			_ => lastClearAnnihilationStage, 
		};
	}

	public AnnihilationMode GetCurSelectMode()
	{
		return curAnnihilationMode;
	}
}
