using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIDailyBonus : UIPopup
{
	public UIDefaultListView dayList;

	public UILabel attendanceCount;

	public UILabel timeLimit;

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	private UIGoods lastItem;

	public GameObject btnClose;

	private bool openCarnival;

	public override void OnClick(GameObject sender)
	{
		if (base.localUser.dailyBonus.isReceived && !AnimBG.IsAnimating() && !AnimBlock.IsAnimating())
		{
			string text = sender.name;
			if (text == "Close")
			{
				ClosePopup();
			}
			base.OnClick(sender);
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		_Set();
	}

	private void _Set()
	{
		RoDailyBonus dailyBonus = base.localUser.dailyBonus;
		List<DailyBonusDataRow> targetList = dailyBonus.GetCurrentVersionDataList();
		targetList.Sort((DailyBonusDataRow a, DailyBonusDataRow b) => a.day - b.day);
		string itemPrefix = "Day-";
		dayList.CreateItem(dailyBonus.totalDayCount, "Day-", delegate(UIItemBase item, int idx)
		{
			UIGoods uIGoods = item as UIGoods;
			DailyBonusDataRow dailyBonusDataRow = targetList[idx];
			UISetter.SetGameObjectName(item.gameObject, itemPrefix + dailyBonusDataRow.day);
			uIGoods.Set(dailyBonusDataRow);
			uIGoods.SetReceive(dailyBonusDataRow.day <= dailyBonus.lastCompleteDay);
			uIGoods.SetReceiveBlock(dailyBonusDataRow.day <= dailyBonus.lastCompleteDay);
			if (dailyBonusDataRow.day == dailyBonus.lastCompleteDay + 1)
			{
				lastItem = uIGoods;
			}
		});
		UISetter.SetLabel(timeLimit, Localization.Format("7069", dailyBonus.timeLimit.Year, dailyBonus.timeLimit.Month, dailyBonus.timeLimit.Day, dailyBonus.timeLimit.Day - DateTime.Now.Day));
		UISetter.SetLabel(attendanceCount, Localization.Format("7070", dailyBonus.lastCompleteDay, dailyBonus.totalDayCount));
	}

	private IEnumerator BonusReceive()
	{
		yield return new WaitForSeconds(1f);
		if (lastItem == null)
		{
			base.localUser.dailyBonus.isReceived = true;
			yield break;
		}
		lastItem.SetReceive(received: true);
		lastItem.GetComponent<Animator>().enabled = true;
		yield return new WaitForSeconds(1f);
		base.network.RequestDailyBonusReceive();
	}

	public void InitAndOpenDailyBonus()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			_Set();
			Open();
			AnimBG.Reset();
			AnimBlock.Reset();
			openCarnival = !base.localUser.dailyBonus.isReceived;
			if (!base.localUser.dailyBonus.isReceived)
			{
				StartCoroutine(BonusReceive());
			}
		}
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
		yield return new WaitForSeconds(0.8f);
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
		if (openCarnival)
		{
			base.network.RequestGetCarnivalList(ECarnivalCategory.Basic);
		}
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

	public override void Open()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Close()
	{
		HidePopup();
	}
}
