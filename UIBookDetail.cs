using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIBookDetail : UIPopup
{
	public GEAnimNGUI AnimLeft;

	public GEAnimNGUI AnimRight;

	public GEAnimNGUI AnimBlock;

	public GameObject card;

	public GameObject info;

	public GameObject sceanario;

	public GameObject favor;

	public UIFlipSwitch cardTab;

	public UIFlipSwitch sceanarioTab;

	public UIFlipSwitch infoTab;

	public UIFlipSwitch favorTap;

	[HideInInspector]
	public RoCommander commander;

	public UICommander uiCommanderLeft;

	public GameObject Tab;

	private bool isZoom;

	public GameObject btnZoomExit;

	private float device_width;

	private float device_height;

	private float device_aspectX = 1f;

	private float device_aspectY = 2f;

	private bool bTouchEnable;

	private bool bScaleCard;

	public UIDefaultListView favorRewardListView;

	public UIProgressBar favorRewardProgress;

	public UILabel favorRewarValue;

	public UILabel favorStep;

	public static readonly string favorRewardItemIdPrefix = "RewardItem-";

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		AnimLeft.Reset();
		AnimBlock.Reset();
		OnAnimOpen();
	}

	public override void OnClick(GameObject sender)
	{
		SoundManager.PlaySFX("EFM_SelectButton_001");
		string text = sender.name;
		switch (text)
		{
		case "Close":
			ClosePopup();
			break;
		case "CardBtn":
			if (cardTab.GetState() == SwitchStatus.ON)
			{
				device_width = Screen.width;
				device_height = Screen.height;
				device_aspectX = 1.6f;
				device_aspectY = 2f;
				float num = (float)Screen.height / ((float)Screen.width / 16f) / 9f;
				InitAndOpenCard();
				CardScaleUpSize();
			}
			else
			{
				InitAndOpenCard();
			}
			break;
		case "InfomationBtn":
			InitAndOpenInformation();
			break;
		case "ScenarioBtn":
			InitAndOpenSceanario();
			break;
		case "BtnZoomExit":
			CardScaleDownSize();
			break;
		default:
			if (!favorRewardListView.Contains(text))
			{
			}
			break;
		}
	}

	public void ZoomExit()
	{
		if (isZoom)
		{
			isZoom = false;
			CardScaleDownSize();
		}
	}

	public void Set(string _id)
	{
		SetBookDetail(_id);
		InitTap();
	}

	public void ResetUIActive()
	{
		isZoom = false;
	}

	private void InitTap()
	{
		UISetter.SetFlipSwitch(cardTab, state: true);
		UISetter.SetFlipSwitch(infoTab, state: false);
		UISetter.SetFlipSwitch(favorTap, state: false);
		sceanarioTab.Lock(_isLock: true);
	}

	private void SetBookDetail(string _id)
	{
		ResetUIActive();
		commander = base.localUser.FindCommander(_id);
		uiCommanderLeft.Set(commander);
		if ((bool)uiCommanderLeft.spine.GetComponent<UISpineAnimation>().target.GetComponent<UIInteraction>())
		{
			uiCommanderLeft.spine.GetComponent<UISpineAnimation>().target.GetComponent<UIInteraction>().commanderId = commander.id;
		}
		InitFavorRewardData();
	}

	public void InitAndOpenCard()
	{
		_SetPage(cardState: true, infoState: false, sceanarioState: false, favorState: false);
	}

	public void InitAndOpenInformation()
	{
		_SetPage(cardState: false, infoState: true, sceanarioState: false, favorState: false);
	}

	public void InitAndOpenSceanario()
	{
		_SetPage(cardState: false, infoState: false, sceanarioState: true, favorState: false);
	}

	public void InitAndOpenFavor()
	{
		_SetPage(cardState: false, infoState: false, sceanarioState: false, favorState: true);
	}

	private void _SetPage(bool cardState, bool infoState, bool sceanarioState, bool favorState)
	{
		UISetter.SetActive(card, cardState);
		UISetter.SetActive(info, infoState);
		UISetter.SetActive(sceanario, sceanarioState);
		UISetter.SetActive(favor, favorState);
		UISetter.SetFlipSwitch(cardTab, cardState);
		UISetter.SetFlipSwitch(infoTab, infoState);
		UISetter.SetFlipSwitch(sceanarioTab, sceanarioState);
		UISetter.SetFlipSwitch(favorTap, favorState);
	}

	public void InitFavorRewardData()
	{
		FavorStepDataRow favorStepDataRow = RemoteObjectManager.instance.regulation.FindFavorStepData(commander.FavorStep);
		FavorStepDataRow favorStepDataRow2 = RemoteObjectManager.instance.regulation.FindFavorStepData((int)commander.FavorStep + 1);
		List<RewardDataRow> list = RemoteObjectManager.instance.regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == ERewardCategory.FavorComplete && row.type.ToString() == commander.id);
		int num = 0;
		float value = 1f;
		if (favorStepDataRow != null)
		{
			num = favorStepDataRow.favor;
		}
		if (favorStepDataRow2 == null || (int)commander.FavorStep >= list.Count)
		{
			UISetter.SetLabel(favorStep, string.Format(Localization.Get("10072"), commander.FavorStep));
		}
		else
		{
			value = (float)((int)commander.FavorCount - num) / (float)(favorStepDataRow2.favor - num);
			UISetter.SetLabel(favorStep, string.Format(Localization.Get("10072"), (int)commander.FavorStep + 1));
		}
		favorRewardProgress.value = value;
		UISetter.SetLabel(favorRewarValue, favorRewardProgress.value * 100f + "%");
		if (!(favorRewardListView != null))
		{
			return;
		}
		list.Sort(delegate(RewardDataRow row, RewardDataRow row1)
		{
			if (row.typeIndex < row1.typeIndex)
			{
				return -1;
			}
			return (row.typeIndex > row1.typeIndex) ? 1 : 0;
		});
		favorRewardListView.InitFavorItem(list, favorRewardItemIdPrefix);
	}

	public void GetCommanderStage()
	{
		base.uiWorld.worldMap.InitAndOpenWorldMap("1", "1");
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		SetBookDetail(commander.id);
	}

	public void OpenPopupShow()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		HidePopup();
	}

	public override void Open()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Close()
	{
		HidePopup();
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
		AnimLeft.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
	}

	private void OnAnimClose()
	{
		AnimLeft.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public void OpenAnimation()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", 0, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeOutBack));
	}

	public void CloseAnimation()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", -1000, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeInBack, "oncomplete", "Close", "oncompletetarget", base.gameObject));
	}

	public void OnCardScaleEvent()
	{
		if (!bTouchEnable)
		{
			bScaleCard = !bScaleCard;
			if (bScaleCard)
			{
				CardScaleUpSize();
			}
			else
			{
				CardScaleDownSize();
			}
		}
	}

	public void CardScaleUpSize()
	{
		ForwardBackKeyEvent.DTouchLock();
		bTouchEnable = true;
		UISetter.SetActive(Tab, active: false);
		iTween.MoveTo(card.gameObject, iTween.Hash("position", new Vector3(0f, 0f, -100f), "islocal", true, "time", 0.2f));
		iTween.ScaleTo(card.gameObject, iTween.Hash("scale", new Vector3(device_aspectX, device_aspectY, 1f), "islocal", true, "time", 0.2f));
		iTween.RotateTo(card.gameObject, iTween.Hash("z", 90, "islocal", true, "oncomplete", "OnCompScaleUp", "onCompleteTarget", base.gameObject, "time", 0.2f));
	}

	private void OnCompScaleUp()
	{
		bTouchEnable = false;
		isZoom = true;
		btnZoomExit.SetActive(value: true);
	}

	private void OnCompScaleDown()
	{
		bTouchEnable = false;
		btnZoomExit.SetActive(value: false);
		ForwardBackKeyEvent.DTouchUnLock();
		UISetter.SetActive(Tab, active: true);
	}

	public void CardScaleDownSize()
	{
		bTouchEnable = true;
		iTween.MoveTo(card.gameObject, iTween.Hash("position", new Vector3(0f, -30f, 0f), "islocal", true, "time", 0.2f));
		iTween.ScaleTo(card.gameObject, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "islocal", true, "time", 0.2f));
		iTween.RotateTo(card.gameObject, iTween.Hash("z", 0, "islocal", true, "oncomplete", "OnCompScaleDown", "onCompleteTarget", base.gameObject, "time", 0.2f));
	}

	private void Update()
	{
		if (isZoom && Input.GetKeyDown(KeyCode.Escape))
		{
			CardScaleDownSize();
		}
	}
}
