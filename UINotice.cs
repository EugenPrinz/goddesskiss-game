using System;
using System.Collections;
using UnityEngine;

public class UINotice : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	private bool bLoadWebview;

	public DownloadTexture downloadTexture;

	public GameObject checkBoxRoot;

	public UIToggle checkBox;

	public UITimer eventTimer;

	private int idx;

	private void Start()
	{
		AnimBG.Reset();
		AnimBlock.Reset();
		SetAutoDestroy(autoDestory: true);
		OpenPopupShow();
	}

	public void Init(Protocols.NoticeData data)
	{
		idx = data.idx;
		UISetter.SetActive(checkBoxRoot, data.notiFixed == 0);
		eventTimer.SetLabelFormat(Localization.Get("7156"), string.Empty);
		eventTimer.Set(data.eventEndDate);
		downloadTexture.SetUrl(data.img);
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "Close" && !bBackKeyEnable && !bEnterKeyEnable)
		{
			Close();
		}
	}

	public void OnCheckBoxClicked()
	{
		if (checkBox.value)
		{
			Utility.SetBlindNotice(idx);
		}
		Close();
	}

	private string SecondsToDateTime(double time)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(time).ToLocalTime().ToString("yyyy-MM-dd   HH:mm:ss");
	}

	private void OpenPopupShow()
	{
		base.Open();
		StartCoroutine(ShowPopup());
		OnAnimOpen();
	}

	public override void Close()
	{
		bBackKeyEnable = true;
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.3f);
		UIManager.instance.world.noticePopUp = null;
		yield return new WaitForSeconds(0.5f);
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
		base.Close();
	}

	private IEnumerator ShowPopup()
	{
		bEnterKeyEnable = true;
		yield return new WaitForSeconds(0.8f);
		bEnterKeyEnable = false;
	}

	private void HidePopup()
	{
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
		AnimBG.Reset();
		AnimBlock.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public void AnimOpenFinish()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
	}
}
