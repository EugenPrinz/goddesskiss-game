using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIFirstPaymentPopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIDefaultListView rewardListView;

	public GameObject receiveBtn;

	public GameObject completeRoot;

	public GameObject moveBtn;

	public void Init()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			SetAutoDestroy(autoDestory: true);
			SetButton();
			SetReward();
			OpenPopup();
		}
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "ReceiveBtn")
		{
			base.network.RequestGetFirstPaymentReward();
		}
		else if (text == "MoveBtn")
		{
			base.uiWorld.camp.GoNavigation("Shop_Diamond");
		}
		ClosePopup();
	}

	private void SetButton()
	{
		UISetter.SetActive(receiveBtn, base.localUser.statistics.firstPayment == 1);
		UISetter.SetActive(completeRoot, base.localUser.statistics.firstPayment == 2);
		UISetter.SetActive(moveBtn, base.localUser.statistics.firstPayment == 0);
	}

	private void SetReward()
	{
		List<RewardDataRow> list = base.regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == ERewardCategory.FirstPayment);
		rewardListView.InitRewardList(list);
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		SetButton();
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		if (!bBackKeyEnable)
		{
			bBackKeyEnable = true;
			HidePopup();
		}
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0f);
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
		AnimBG.Reset();
		AnimBlock.Reset();
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
