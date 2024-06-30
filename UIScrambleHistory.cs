using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScrambleHistory : UIPopup
{
	public UIDefaultListView historyListView;

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
	}

	public void Init(List<Protocols.ScrambleMapHistory> list)
	{
		historyListView.Init(list);
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "Close")
		{
			SoundManager.PlaySFX("BTN_Negative_001");
			ClosePopup();
		}
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
		yield return new WaitForSeconds(0.8f);
		base.Close();
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
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
