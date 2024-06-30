using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGetItem : UIPopup
{
	public GEAnimNGUI AnimEffect;

	public GEAnimNGUI AnimCenter;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTopBg;

	public GEAnimNGUI AnimTopBgMid;

	public GEAnimNGUI AnimTopBgSub;

	public GEAnimNGUI AnimLine;

	public GEAnimNGUI AnimBlock;

	public UIDefaultListView listView;

	[SerializeField]
	private GameObject subMessage;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public static readonly string itemIdPrefix = "GetItem-";

	public void Set(List<Protocols.RewardInfo.RewardData> rewardList, string strMessage = "")
	{
		if (listView != null)
		{
			if (rewardList.Count == 1 && rewardList[0].rewardType >= ERewardType.GroupEff_1 && rewardList[0].rewardType <= ERewardType.GroupEff_8)
			{
				UISetter.SetLabel(title, Localization.Get("10082"));
				UISetter.SetLabel(message, Localization.Get(strMessage));
				UISetter.SetActive(subMessage, active: false);
			}
			else
			{
				UISetter.SetLabel(title, Localization.Get("7063"));
				UISetter.SetLabel(message, Localization.Get("5730"));
				UISetter.SetActive(subMessage, active: true);
			}
			listView.InitRewardList(rewardList, itemIdPrefix);
			listView.scrollView.ResetPosition();
			OpenPopupShow();
			SetAutoDestroy(autoDestory: true);
		}
	}

	public void Set(List<Protocols.RewardInfo.RewardData> rewardList, Protocols.EventRaidReward.EventRaidRewardData data)
	{
		if (!(listView != null))
		{
			return;
		}
		UISetter.SetLabel(title, Localization.Get("7063"));
		string text = string.Empty;
		if (data.attend > 0)
		{
			text += Localization.Format("6616", data.attend);
			if (data.own > 0 || data.mvp > 0)
			{
				text += ", ";
			}
		}
		if (data.own > 0)
		{
			text += Localization.Format("6617", data.own);
			if (data.mvp > 0)
			{
				text += ", ";
			}
		}
		if (data.mvp > 0)
		{
			text += Localization.Format("6618", data.mvp);
		}
		UISetter.SetLabel(message, text);
		UISetter.SetActive(subMessage, active: true);
		listView.InitRewardList(rewardList, itemIdPrefix);
		listView.scrollView.ResetPosition();
		OpenPopupShow();
		SetAutoDestroy(autoDestory: true);
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close" && !bBackKeyEnable)
		{
			Close();
		}
	}

	public void OpenPopupShow()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Close()
	{
		bBackKeyEnable = true;
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
		AnimEffect.Reset();
		AnimCenter.Reset();
		AnimNpc.Reset();
		AnimTopBg.Reset();
		AnimTopBgMid.Reset();
		AnimTopBgSub.Reset();
		AnimLine.Reset();
		AnimBlock.Reset();
		AnimEffect.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimCenter.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBg.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBgMid.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBgSub.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimLine.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private IEnumerator OnEventHidePopup()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: false);
		yield return new WaitForSeconds(0.3f);
		UISetter.SetActive(AnimCenter.gameObject, active: false);
		yield return new WaitForSeconds(0.5f);
		base.localUser.UserLevelUpCheck();
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
		base.Close();
	}

	private void OnAnimClose()
	{
		AnimEffect.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimCenter.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBg.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBgMid.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBgSub.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimLine.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
