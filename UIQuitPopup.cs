using System.Collections;
using UnityEngine;

public class UIQuitPopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "Close")
		{
			SoundManager.PlaySFX("BTN_Negative_001");
			ClosePopup();
		}
		else if (text == "OK")
		{
			Application.Quit();
		}
		base.OnClick(sender);
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
		yield return new WaitForSeconds(0.4f);
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
