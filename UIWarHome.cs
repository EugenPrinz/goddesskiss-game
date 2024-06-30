using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIWarHome : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimInfo;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIDefaultListView rewardListView;

	public UIProgressBar completeProgress;

	public UILabel completeProgressValue;

	public GameObject receiveButton;

	public GameObject infoRoot;

	public UIBattleRecord battleRecord;

	public UIFlipSwitch mission;

	public UIFlipSwitch achievement;

	public UIFlipSwitch completeAchievement;

	public UIFlipSwitch battleRecordTab;

	public UISpineAnimation spineAnimation;

	public UIButton goalReceive;

	public UISprite goalReceiveIcon;

	public UISprite missionBadge;

	public UISprite achievementBadge;

	public UIButton receiveAllButton;

	private EReward _lastEvent = EReward.Mail;

	public static readonly string itemIdPrefix = "RewardItem-";

	public UITexture goBG;

	private void Start()
	{
		UISetter.SetSpine(spineAnimation, "n_007");
	}

	private void OnDestroy()
	{
		if (goBG != null)
		{
			goBG = null;
		}
		if (AnimBlock != null)
		{
			AnimBlock = null;
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		string text2 = null;
		RoMission roMission = null;
		if (text.Contains("-"))
		{
			text2 = text.Substring(text.IndexOf("-") + 1);
			if (!string.IsNullOrEmpty(text2))
			{
				if (_lastEvent == EReward.DailyMission)
				{
					roMission = base.localUser.FindMission(text2);
				}
				else if (_lastEvent == EReward.Achievement)
				{
					int num = 1;
					roMission = base.localUser.FindAchievement(text2, num);
					while (!roMission.bListShow)
					{
						num++;
						roMission = base.localUser.FindAchievement(text2, num);
					}
				}
			}
		}
		switch (text)
		{
		case "Close":
			if (!bBackKeyEnable)
			{
				Close();
			}
			return;
		case "Mission":
			Set(EReward.DailyMission);
			return;
		case "Achievement":
			Set(EReward.Achievement);
			return;
		case "CompleteAchievement":
			Set(EReward.CompleteAchievement);
			return;
		case "BattleRecord":
			Set(EReward.Undefined);
			return;
		}
		if (text.StartsWith("Receive-"))
		{
			if (roMission == null)
			{
				return;
			}
			if (_lastEvent == EReward.DailyMission)
			{
				if (!base.localUser.GetMissionItemCheck(roMission))
				{
					ReceiveMissionRewardNotice(roMission.idx);
				}
				else
				{
					ReceiveMissionReward(roMission.idx);
				}
			}
			else if (_lastEvent == EReward.Achievement)
			{
				if (!base.localUser.GetMissionItemCheck(roMission))
				{
					ReceiveAchievementRewardNotice(roMission.idx, roMission.sort);
				}
				else
				{
					ReceiveAchievementReward(roMission.idx, roMission.sort);
				}
			}
			return;
		}
		if (text.StartsWith("ReceivePerfect-"))
		{
			EReward eReward = (EReward)Enum.Parse(typeof(EReward), text2);
			if (base.localUser.eventPefectRewardReceiveDict.ContainsKey(eReward))
			{
				base.localUser.eventPefectRewardReceiveDict[eReward] = true;
			}
			EReward type = (EReward)Enum.Parse(typeof(EReward), "Perfect" + text2);
			List<RoReward> rewardList = base.localUser.GetRewardList(type);
			if (rewardList.Count <= 0 || rewardList.Count > 1)
			{
				UISimplePopup.CreateDebugOK("보상을 획득하게 될 겁니다... 앞으로", null, "확인");
				return;
			}
			RoReward roReward = rewardList[0];
			base.localUser.ConfirmReward(roReward);
			UISimplePopup.CreateDebugOK($"소식함으로 보상이 도착하였습니다.\nCode : {roReward.rewardId}\nCount : {roReward.rewardCount}", null, "확인");
			Set(eReward);
			return;
		}
		if (text.StartsWith("Link-"))
		{
			if (!roMission.link.StartsWith("World-") && roMission.link != "LastStage")
			{
				Close();
			}
			base.uiWorld.camp.GoNavigation(roMission.link);
			return;
		}
		if (text == "BtnGoalReceive")
		{
			if (_lastEvent == EReward.DailyMission)
			{
				base.network.RequestCompleteMissionGoal(1, base.localUser.missionGoal);
			}
			else if (_lastEvent == EReward.Achievement)
			{
				base.network.RequestCompleteMissionGoal(2, base.localUser.achievementGoal);
			}
			return;
		}
		if (!(text == "ReceiveAll"))
		{
			return;
		}
		if (_lastEvent == EReward.DailyMission)
		{
			if (base.localUser.badgeMissionCount > 0)
			{
				base.network.RequestAllMissionReward();
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7066"));
			}
		}
		else if (_lastEvent == EReward.Achievement)
		{
			if (base.localUser.badgeAchievementCount > 0)
			{
				base.network.RequestAllAchievementReward();
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7066"));
			}
		}
	}

	private void ReceiveAchievementReward(string id, int sort)
	{
		base.network.RequestAchievementReward(id, sort);
	}

	private void ReceiveAchievementRewardNotice(string id, int sort)
	{
		UISimplePopup.CreateBool(localization: true, "1303", "5476", "5477", "1304", "1305").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				ReceiveAchievementReward(id, sort);
			}
		};
	}

	private void ReceiveMissionReward(string id)
	{
		base.network.RequestMissionReward(id);
	}

	private void ReceiveMissionRewardNotice(string id)
	{
		UISimplePopup.CreateBool(localization: true, "1303", "5476", "5477", "1304", "1305").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				ReceiveMissionReward(id);
			}
		};
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		SetBadge();
	}

	private void SetBadge()
	{
		UISetter.SetActive(missionBadge, base.localUser.badgeMissionCount > 0);
		UISetter.SetActive(achievementBadge, base.localUser.badgeAchievementCount > 0);
	}

	public void SetSelectedType(EReward type)
	{
		UISetter.SetFlipSwitch(mission, type == EReward.DailyMission);
		UISetter.SetFlipSwitch(achievement, type == EReward.Achievement);
		UISetter.SetFlipSwitch(completeAchievement, type == EReward.CompleteAchievement);
		UISetter.SetFlipSwitch(battleRecordTab, type == EReward.Undefined);
		UISetter.SetActive(battleRecord, type == EReward.Undefined);
		UISetter.SetActive(rewardListView, type != EReward.Undefined);
	}

	public void SetInfo(EReward type)
	{
		bool flag = false;
		float num = 0f;
		RewardDataRow rewardDataRow = null;
		RewardDataRow rewardDataRow2 = null;
		int num2 = 0;
		switch (type)
		{
		case EReward.DailyMission:
			flag = true;
			UISetter.SetLabel(message, Localization.Get("14002"));
			rewardDataRow2 = RemoteObjectManager.instance.regulation.rewardDtbl.Find((RewardDataRow row) => row.category == ERewardCategory.MissionComplete && row.type == base.localUser.missionGoal - 1);
			if (rewardDataRow2 != null)
			{
				num2 = rewardDataRow2.typeIndex;
			}
			rewardDataRow = RemoteObjectManager.instance.regulation.rewardDtbl.Find((RewardDataRow row) => row.category == ERewardCategory.MissionComplete && row.type == base.localUser.missionGoal);
			if (rewardDataRow != null)
			{
				num = (float)(base.localUser.missionCompleteCount - num2) / (float)(rewardDataRow.typeIndex - num2);
			}
			break;
		case EReward.Achievement:
			flag = true;
			UISetter.SetLabel(message, Localization.Get("14003"));
			rewardDataRow2 = RemoteObjectManager.instance.regulation.rewardDtbl.Find((RewardDataRow row) => row.category == ERewardCategory.AchievementComplete && row.type == base.localUser.achievementGoal - 1);
			if (rewardDataRow2 != null)
			{
				num2 = rewardDataRow2.typeIndex;
			}
			rewardDataRow = RemoteObjectManager.instance.regulation.rewardDtbl.Find((RewardDataRow row) => row.category == ERewardCategory.AchievementComplete && row.type == base.localUser.achievementGoal);
			if (rewardDataRow != null)
			{
				num = (float)(base.localUser.achievementCompleteCount - num2) / (float)(rewardDataRow.typeIndex - num2);
			}
			break;
		}
		UISetter.SetActive(goalReceiveIcon, rewardDataRow != null);
		if (rewardDataRow != null)
		{
			CommanderDataRow commanderDataRow = base.regulation.commanderDtbl[rewardDataRow.rewardIdx.ToString()];
			UISetter.SetSprite(goalReceiveIcon, commanderDataRow.thumbnailId);
		}
		UISetter.SetLabel(completeProgressValue, Localization.Format("5900", num * 100f));
		UISetter.SetProgress(completeProgress, num);
		UISetter.SetActive(goalReceive, num >= 1f && rewardDataRow != null);
		UISetter.SetActive(completeProgressValue, num < 1f && rewardDataRow != null);
		UISetter.SetActive(completeProgress, num < 1f && rewardDataRow != null);
		UISetter.SetActive(infoRoot, flag && rewardDataRow != null);
	}

	public void Set(EReward type, List<RoMission> rewardList)
	{
		if (rewardList == null || rewardList.Count <= 0)
		{
			return;
		}
		SetSelectedType(type);
		SetInfo(type);
		if (rewardListView != null)
		{
			if (type == EReward.CompleteAchievement)
			{
				rewardListView.Init(rewardList, itemIdPrefix, bComplete: true);
			}
			else
			{
				rewardListView.Init(rewardList, itemIdPrefix);
			}
		}
		rewardListView.scrollView.ResetPosition();
		_lastEvent = type;
	}

	public void Set(EReward type)
	{
		switch (type)
		{
		case EReward.DailyMission:
			Set(type, base.localUser.missionList);
			break;
		case EReward.Achievement:
			Set(type, base.localUser.achievementList);
			break;
		case EReward.CompleteAchievement:
			Set(type, base.localUser.achievementList);
			break;
		case EReward.Undefined:
			battleRecord.Set(base.localUser);
			SetSelectedType(type);
			SetInfo(type);
			break;
		}
		if (type == EReward.Achievement || type == EReward.DailyMission)
		{
			UISetter.SetActive(receiveAllButton, active: true);
			rewardListView.GetComponent<UIPanel>().baseClipRegion = new Vector4(30f, 850f, 762f, 470f);
		}
		else
		{
			UISetter.SetActive(receiveAllButton, active: false);
			rewardListView.GetComponent<UIPanel>().baseClipRegion = new Vector4(30f, 810f, 762f, 550f);
		}
		SetBadge();
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
		AnimBG.Reset();
		AnimNpc.Reset();
		AnimInfo.Reset();
		AnimTitle.Reset();
		AnimBlock.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimInfo.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimInfo.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public override void Open()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			base.Open();
			if (rewardListView != null)
			{
				rewardListView.scrollView.SetDragAmount(0f, 0f, updateScrollbars: false);
			}
			OnAnimOpen();
		}
	}

	public override void Close()
	{
		bBackKeyEnable = true;
		HidePopup();
	}

	public void AnimOpenFinish()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
	}

	protected override void OnEnablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: true);
	}

	protected override void OnDisablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: false);
	}
}
