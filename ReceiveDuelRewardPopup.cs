using System.Collections;
using UnityEngine;

public class ReceiveDuelRewardPopup : UIPopup
{
	public GEAnimNGUI AnimEffect;

	public GEAnimNGUI AnimCenter;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTopBg;

	public GEAnimNGUI AnimTopBgMid;

	public GEAnimNGUI AnimLine;

	public GEAnimNGUI AnimBlock;

	public UILabel score;

	public UILabel ranking;

	public UILabel percent;

	public GameObject PvPRoot;

	public GameObject RaidRoot;

	public UIDefaultListView rewardListView;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		AnimEffect.Reset();
		AnimCenter.Reset();
		AnimNpc.Reset();
		AnimTopBg.Reset();
		AnimTopBgMid.Reset();
		AnimLine.Reset();
		AnimBlock.Reset();
		OpenPopupShow();
	}

	public void Set(PvPRewardType type)
	{
		UISetter.SetActive(PvPRoot, type == PvPRewardType.PvP);
		UISetter.SetActive(RaidRoot, type == PvPRewardType.Raid);
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close")
		{
			Close();
		}
	}

	public void OpenPopupShow()
	{
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
		AnimEffect.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimCenter.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBg.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBgMid.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimLine.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
	}

	private void OnAnimClose()
	{
		AnimEffect.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimCenter.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBg.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBgMid.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimLine.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			OnAnimOpen();
		}
		else if (Input.GetKeyDown(KeyCode.Q))
		{
			OnAnimClose();
		}
	}
}
