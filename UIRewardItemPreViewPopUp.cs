using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIRewardItemPreViewPopUp : UIPopup
{
	public UIDefaultListView itemListView;

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public void InitAndOpenItemList(List<RewardDataRow> list, string titleName)
	{
		itemListView.InitRewardList(list, "Reward-");
		UISetter.SetLabel(title, titleName);
		OpenPopup();
		SetAutoDestroy(autoDestory: true);
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text.Contains("-"))
		{
		}
		if (text == "Close")
		{
			ClosePopup();
		}
	}

	public void OpenPopup()
	{
		base.Open();
		AnimBG.Reset();
		AnimBlock.Reset();
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
