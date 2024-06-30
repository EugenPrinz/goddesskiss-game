using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICarnivalDailyPackagePreViewPopUp : UIPopup
{
	public UIDefaultListView itemListView;

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public void InitAndOpenItemList(string cId)
	{
		CarnivalDataRow row = base.regulation.carnivalDtbl.Find((CarnivalDataRow item) => item.idx == cId);
		List<RewardDataRow> list = base.regulation.rewardDtbl.FindAll((RewardDataRow item) => item.category == ERewardCategory.Carnival && item.type == int.Parse(cId) && item.typeIndex != 1);
		RewardDataRow rewardDataRow = base.regulation.rewardDtbl.Find((RewardDataRow item) => item.category == ERewardCategory.Carnival && item.type == int.Parse(cId) && item.typeIndex == 1);
		InAppProductDataRow inAppProductDataRow = base.regulation.inAppProductDtbl.Find((InAppProductDataRow item) => item.iapidx == row.checkCount);
		itemListView.InitRewardList(list, "Reward-");
		if (inAppProductDataRow != null)
		{
			UISetter.SetLabel(title, Localization.Get(inAppProductDataRow.stringidx));
		}
		UISetter.SetLabel(message, Localization.Format("70013", rewardDataRow.minCount));
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
		yield return new WaitForSeconds(0f);
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
