using System;
using System.Collections;
using UnityEngine;

public class UIAlarm : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public GameObject notice;

	public GameObject alarm;

	public UIFlipSwitch notiTab;

	public UIFlipSwitch alarmTab;

	public UIDefaultListView alarmListView;

	public UniWebView webviewPopup;

	public UISpineAnimation spineAnimation;

	private bool bLoadWebview;

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "Close")
		{
			if (!bBackKeyEnable)
			{
				Close();
			}
		}
		else if (alarmListView.Contains(text))
		{
			string pureId = alarmListView.GetPureId(text);
			OnMoveBtnClicked(pureId);
		}
		else if (text == "NotiTab")
		{
			InitAndNotiTab();
		}
		else if (text == "AlarmTab")
		{
			InitAndAlarmTab();
		}
	}

	public void InitAndOpenAlarm()
	{
		if (!bEnterKeyEnable)
		{
			OpenPopupShow();
			InitAndNotiTab();
			bEnterKeyEnable = true;
		}
	}

	public void InitAndNotiTab()
	{
		_SetPage(notiState: true, alarmState: false);
		if (!bLoadWebview)
		{
			ForwardBackKeyEvent.DTouchLock();
			NetworkAnimation.Instance.On();
			UniWebView uniWebView = webviewPopup;
			uniWebView.OnBackLoadEvent = (UniWebView.OnPopupBackLoadEvent)Delegate.Combine(uniWebView.OnBackLoadEvent, new UniWebView.OnPopupBackLoadEvent(OnPopupBackLoadEvent));
			webviewPopup.Load();
			bLoadWebview = true;
		}
		else
		{
			webviewPopup.Show();
		}
	}

	public void InitAndAlarmTab()
	{
		if (bLoadWebview)
		{
			webviewPopup.Hide();
		}
		_SetPage(notiState: false, alarmState: true);
		SetAlarmList();
	}

	public void SetAlarmList()
	{
		alarmListView.InitAlarm("Alarm_");
		alarmListView.grid.Reposition();
	}

	private void _SetPage(bool notiState, bool alarmState)
	{
		int num = 0;
		num += (notiState ? 1 : 0);
		num += (alarmState ? 1 : 0);
		if (num > 1 || num == 0)
		{
			notiState = true;
			alarmState = false;
		}
		UISetter.SetActive(notice, notiState);
		UISetter.SetActive(alarm, alarmState);
		UISetter.SetFlipSwitch(notiTab, notiState);
		UISetter.SetFlipSwitch(alarmTab, alarmState);
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
	}

	private void OpenPopupShow()
	{
		UISetter.SetSpine(spineAnimation, "n_001");
		base.Open();
		OnAnimOpen();
	}

	public override void Open()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Close()
	{
		bBackKeyEnable = true;
		HidePopup();
		webviewPopup.Hide();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
		UniWebView uniWebView = webviewPopup;
		uniWebView.OnBackLoadEvent = (UniWebView.OnPopupBackLoadEvent)Delegate.Remove(uniWebView.OnBackLoadEvent, new UniWebView.OnPopupBackLoadEvent(OnPopupBackLoadEvent));
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
		AnimBG.Reset();
		AnimNpc.Reset();
		AnimTitle.Reset();
		AnimBlock.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private IEnumerator MoveStart(string key)
	{
		Close();
		yield return new WaitForSeconds(0.5f);
		OnMoveBtnClicked(key);
	}

	private void OnMoveBtnClicked(string key)
	{
		Close();
		switch (key)
		{
		case "dcnt":
			UIManager.instance.world.camp.GoNavigation("WarMemorial");
			break;
		case "hold":
		case "cmdr":
			UIManager.instance.world.camp.GoNavigation("Academy");
			break;
		case "shop":
			UIManager.instance.world.camp.GoNavigation("BlackMarket");
			break;
		case "expd":
		case "mwdw":
			UIManager.instance.world.camp.GoNavigation("Situation");
			break;
		case "srgs":
		case "srge":
		case "ocps":
		case "ocpe":
			UIManager.instance.world.camp.GoNavigation("Guild");
			break;
		case "raid":
			UIManager.instance.world.camp.GoNavigation("Raid");
			break;
		case "arena":
			UIManager.instance.world.camp.GoNavigation("Duel");
			break;
		}
	}

	private void OnPopupBackLoadEvent(GameObject g)
	{
		NetworkAnimation.Instance.Off();
		ForwardBackKeyEvent.DTouchUnLock();
	}

	public void AnimOpenFinish()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
	}

	protected override void OnEnablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: true);
	}

	protected override void OnDisablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: false);
	}
}
