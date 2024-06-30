using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIReward : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIDefaultListView rewardListView;

	public UIProgressBar completeProgress;

	public UILabel completeProgressValue;

	public GameObject receiveButton;

	public UIFlipSwitch daily;

	public UIFlipSwitch weekly;

	public UIFlipSwitch monthly;

	private List<RoReward> rewardList;

	private EReward _lastEvent = EReward.Mail;

	public static readonly string itemIdPrefix = "RewardItem-";

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		string text2 = null;
		RoReward roReward = null;
		if (text.Contains("-"))
		{
			text2 = text.Substring(text.IndexOf("-") + 1);
			if (!string.IsNullOrEmpty(text2))
			{
				roReward = base.localUser.FindReward(text2);
			}
		}
		switch (text)
		{
		case "Close":
			Close();
			return;
		case "Day":
			Set(EReward.DailyMission);
			return;
		case "Week":
			Set(EReward.WeeklyMission);
			return;
		case "Month":
			Set(EReward.MonthlyMission);
			return;
		case "ReceiveAll":
			if (!base.localUser.GetMailItemCheckList(rewardList))
			{
				ReceiveAllMailRewardNotice();
			}
			else
			{
				ReceiveAllMailReward();
			}
			return;
		}
		if (text.StartsWith("Receive-"))
		{
			if (roReward == null)
			{
				return;
			}
			if (roReward.type == EReward.Mail)
			{
				if (!base.localUser.GetMailItemCheck(roReward))
				{
					ReceiveMailRewardNotice(roReward.id, roReward.subType);
				}
				else
				{
					ReceiveMailReward(roReward.id, roReward.subType);
				}
			}
			else
			{
				base.localUser.ConfirmReward(roReward);
				Set(roReward.type);
				UISimplePopup.CreateDebugOK($"소식함으로 보상이 도착하였습니다.\nCode : {roReward.rewardId}\nCount : {roReward.rewardCount}", null, "확인");
			}
		}
		else if (text.StartsWith("ReceivePerfect-"))
		{
			EReward eReward = (EReward)Enum.Parse(typeof(EReward), text2);
			if (base.localUser.eventPefectRewardReceiveDict.ContainsKey(eReward))
			{
				base.localUser.eventPefectRewardReceiveDict[eReward] = true;
			}
			EReward type = (EReward)Enum.Parse(typeof(EReward), "Perfect" + text2);
			List<RoReward> list = base.localUser.GetRewardList(type);
			if (list.Count <= 0 || list.Count > 1)
			{
				UISimplePopup.CreateDebugOK("보상을 획득하게 될 겁니다... 앞으로", null, "확인");
				return;
			}
			RoReward roReward2 = list[0];
			base.localUser.ConfirmReward(roReward2);
			UISimplePopup.CreateDebugOK($"소식함으로 보상이 도착하였습니다.\nCode : {roReward2.rewardId}\nCount : {roReward2.rewardCount}", null, "확인");
			Set(eReward);
		}
		else if (text.StartsWith("Link-"))
		{
			Close();
			UIManager.instance.OpenLink(roReward.link);
		}
		else if (text.StartsWith("Confirm-") && roReward != null && roReward.type == EReward.Mail)
		{
			if (!roReward.received)
			{
				base.network.RequestReadMail(roReward.id);
			}
			UISimplePopup.CreateOK(localization: false, roReward.title, roReward.description, null, Localization.Get("1001"));
		}
	}

	private void ReceiveMailReward(string id, int type)
	{
		base.network.RequestGetReward(id, type);
	}

	private void ReceiveMailRewardNotice(string id, int type)
	{
		UISimplePopup.CreateBool(localization: true, "1303", "5476", "5477", "1304", "1305").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				ReceiveMailReward(id, type);
			}
		};
	}

	private void ReceiveAllMailReward()
	{
		base.network.RequestGetRewardAll();
	}

	private void ReceiveAllMailRewardNotice()
	{
		UISimplePopup.CreateBool(localization: true, "1303", "5476", "5477", "1304", "1305").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				ReceiveAllMailReward();
			}
		};
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		Set(EReward.Mail);
	}

	public void SetSelectedType(EReward type)
	{
		if (daily != null)
		{
			UISetter.SetFlipSwitch(daily, type == EReward.DailyMission);
		}
		if (weekly != null)
		{
			UISetter.SetFlipSwitch(weekly, type == EReward.WeeklyMission);
		}
		if (monthly != null)
		{
			UISetter.SetFlipSwitch(monthly, type == EReward.MonthlyMission);
		}
	}

	public void Set(EReward type, List<RoReward> rewardList)
	{
		if (rewardList == null || rewardList.Count <= 0)
		{
		}
		this.rewardList = rewardList;
		int num = 0;
		for (int i = 0; i < rewardList.Count; i++)
		{
			RoReward roReward = rewardList[i];
			if (roReward.type != type)
			{
				return;
			}
			if (roReward.received)
			{
				num++;
			}
		}
		SetSelectedType(type);
		if (type != EReward.Mail)
		{
			bool flag = false;
			if (base.localUser.eventPefectRewardReceiveDict.ContainsKey(type))
			{
				flag = !base.localUser.eventPefectRewardReceiveDict[type];
			}
			UISetter.SetButtonEnable(receiveButton, flag && num >= rewardList.Count);
			UISetter.SetGameObjectName(receiveButton, $"{_GetOriginalName(receiveButton)}-{type.ToString()}");
		}
		float num2 = (float)num / (float)rewardList.Count;
		UISetter.SetLabel(completeProgressValue, Localization.Format("5900", num2 * 100f));
		UISetter.SetProgress(completeProgress, num2);
		if (rewardListView != null)
		{
			rewardListView.Init(rewardList, itemIdPrefix);
		}
		if (_lastEvent != type)
		{
			rewardListView.scrollView.ResetPosition();
		}
		_lastEvent = type;
	}

	public void Set(EReward type, List<Protocols.MailInfo.MailData> rewardList)
	{
		if (rewardList != null && rewardList.Count > 0)
		{
			if (type == EReward.Mail)
			{
				UISetter.SetButtonEnable(receiveButton, rewardList.Count > 0 && base.localUser.newMailCount > 0);
			}
			if (rewardListView != null)
			{
				rewardListView.Init(rewardList, itemIdPrefix);
			}
		}
	}

	public void Set(EReward type)
	{
		Set(type, base.localUser.GetRewardList(type));
	}

	private string _GetOriginalName(GameObject go)
	{
		if (go == null)
		{
			return string.Empty;
		}
		string text = go.name;
		if (!text.Contains("-"))
		{
			return text;
		}
		return text.Remove(text.IndexOf("-"));
	}

	public void OpenPopup()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			base.Open();
			AnimBG.Reset();
			AnimBlock.Reset();
			if (rewardListView != null)
			{
				rewardListView.scrollView.SetDragAmount(0f, 0f, updateScrollbars: false);
			}
			OnAnimOpen();
		}
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
