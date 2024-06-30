using System;
using System.Collections;
using UnityEngine;

public class UIWebviewPopup : UIPopup
{
	public UniWebView webview;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	private void Start()
	{
		if (!bEnterKeyEnable)
		{
			SetAutoDestroy(autoDestory: true);
			OpenPopup();
			bEnterKeyEnable = true;
		}
	}

	public void Init(string url)
	{
		webview.url = url;
		UniWebView uniWebView = webview;
		uniWebView.OnBackLoadEvent = (UniWebView.OnPopupBackLoadEvent)Delegate.Combine(uniWebView.OnBackLoadEvent, new UniWebView.OnPopupBackLoadEvent(OnPopupBackLoadEvent));
		webview.Load();
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if ((text == "Close" || text == "OK") && !bBackKeyEnable)
		{
			ClosePopup();
		}
		base.OnClick(sender);
	}

	private void OnPopupBackLoadEvent(GameObject g)
	{
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.3f);
		UIManager.instance.world.noticePopUp = null;
		yield return new WaitForSeconds(0.5f);
		UniWebView uniWebView = webview;
		uniWebView.OnBackLoadEvent = (UniWebView.OnPopupBackLoadEvent)Delegate.Remove(uniWebView.OnBackLoadEvent, new UniWebView.OnPopupBackLoadEvent(OnPopupBackLoadEvent));
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
		base.Close();
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		bBackKeyEnable = true;
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
	}

	private void OnAnimClose()
	{
	}
}
