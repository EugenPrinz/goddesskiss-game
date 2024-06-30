using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIRewardListItem : UIItemBase
{
	public UILabel title;

	public UISprite conditionIcon;

	public UILabel message;

	public UILabel conditionCount;

	public List<UISprite> rewardIconList;

	public List<UILabel> rewardNameList;

	public List<UILabel> rewardCountList;

	public UISprite rewardIcon;

	public UISprite rewardCommanderIcon;

	public UILabel rewardName;

	public UILabel rewardCount;

	public UILabel completeTime;

	public GameObject completedButton;

	public GameObject receiveButton;

	public GameObject confirmButton;

	public GameObject linkButton;

	public UISprite rewardExpIcon;

	public UILabel rewardExpCount;

	public UISprite rewardExpCommanderIcon;

	public UISprite badge;

	public UITimer remainTimer;

	private TimeData remainTimeData;

	public UIGoods good;

	private const string rewardPrefix = "";

	private const string resourcePrefix = "Goods-";

	private const string namePrefix = "Item.Name.";

	public void Set(RoReward reward)
	{
		RoStatistics statistics = RemoteObjectManager.instance.localUser.statistics;
		UISetter.SetLabel(title, (!string.IsNullOrEmpty(reward.title)) ? Localization.Get(reward.title) : string.Empty);
		UISetter.SetLabel(message, reward.description);
		UISetter.SetLabel(completeTime, reward.completeTimeString);
		if (reward.conditionList != null)
		{
			if (!reward.IsExistCondition())
			{
				UISetter.SetLabel(conditionCount, string.Format("({0} / {0})", reward.GetTargetConditionCount()));
			}
			else
			{
				UISetter.SetLabel(conditionCount, $"({reward.GetCurrentConditionCount()} / {reward.GetTargetConditionCount()})");
			}
		}
		if (reward.rewardItem == null)
		{
			good.setMail(message: true);
			UISetter.SetActive(rewardCount, active: false);
		}
		else if (reward.rewardItem.Count == 1)
		{
			good.Set(reward.rewardItem[0]);
			UISetter.SetActive(rewardCount, active: true);
			UISetter.SetLabel(rewardCount, "x" + reward.rewardItem[0].rewardCnt);
		}
		else
		{
			good.setMail(message: false);
			UISetter.SetActive(rewardCount, active: false);
		}
		UISetter.SetSpriteWithSnap(conditionIcon, string.Empty + reward.resourceId);
		UISetter.SetSprite(rewardIcon, "Goods-" + reward.rewardResourceId);
		UISetter.SetLabel(rewardName, Localization.Get("Item.Name." + reward.rewardResourceId));
		if (remainTimer != null)
		{
			if (remainTimeData == null)
			{
				remainTimeData = TimeData.Create();
			}
			remainTimeData.SetByDuration(reward.completeTime);
			UISetter.SetTimer(remainTimer, remainTimeData);
		}
		bool received = reward.received;
		UISetter.SetActive(completedButton, active: false);
		UISetter.SetActive(linkButton, active: false);
		UISetter.SetActive(receiveButton, active: false);
		UISetter.SetActive(confirmButton, active: false);
		UISetter.SetActive(badge, active: false);
		if (received)
		{
			UISetter.SetActive(confirmButton, active: true);
		}
		else if (reward.IsCompleted())
		{
			if (!string.IsNullOrEmpty(reward.rewardId) || reward.rewardItem != null)
			{
				UISetter.SetActive(receiveButton, active: true);
			}
			else
			{
				UISetter.SetActive(confirmButton, active: true);
			}
			UISetter.SetActive(badge, active: true);
		}
		else if (!string.IsNullOrEmpty(reward.link))
		{
			UISetter.SetActive(linkButton, active: true);
		}
		else
		{
			UISetter.SetActive(linkButton, active: true);
		}
		UISetter.SetGameObjectName(linkButton, $"{_GetOriginalName(linkButton)}-{reward.id}");
		UISetter.SetGameObjectName(completedButton, $"{_GetOriginalName(completedButton)}-{reward.id}");
		UISetter.SetGameObjectName(receiveButton, $"{_GetOriginalName(receiveButton)}-{reward.id}");
		UISetter.SetGameObjectName(confirmButton, $"{_GetOriginalName(confirmButton)}-{reward.id}");
	}

	public void Set(Protocols.MailInfo.MailData reward)
	{
		UISetter.SetLabel(message, reward.message);
		int num = 0;
		foreach (Protocols.RewardInfo.RewardData item in reward.reward)
		{
			num++;
		}
		bool flag = reward.receive == 1;
	}

	public void Set(RoMission reward)
	{
		RoStatistics statistics = RemoteObjectManager.instance.localUser.statistics;
		bool flag = true;
		bool bAchieve = false;
		UISetter.SetLabel(title, Localization.Get(reward.title));
		if (reward.startTime != "0" && reward.startTime != null)
		{
			UISetter.SetLabel(message, string.Format(Localization.Get(reward.description), reward.startTime, reward.endTime));
			flag = false;
		}
		else if (reward.sort > 0)
		{
			bAchieve = true;
			if (int.Parse(reward.idx) == 9)
			{
				RoBuilding roBuilding = RemoteObjectManager.instance.localUser.buildingDict[(EBuilding)reward.count];
				UISetter.SetLabel(message, string.Format(Localization.Get(reward.description), Localization.Get(roBuilding.reg.locNameKey), reward.count1));
			}
			else if (int.Parse(reward.idx) == 4)
			{
				UISetter.SetLabel(message, string.Format(Localization.Get(reward.description), RemoteObjectManager.instance.localUser.FindWorldMapStage(reward.count.ToString()).data.worldMapId + " - " + RemoteObjectManager.instance.localUser.FindWorldMapStage(reward.count.ToString()).data.order));
				flag = false;
			}
			else if (int.Parse(reward.idx) == 5)
			{
				UISetter.SetLabel(message, string.Format(Localization.Get(reward.description), RemoteObjectManager.instance.localUser.FindCommander(reward.count.ToString()).nickname, reward.count1));
			}
			else
			{
				UISetter.SetLabel(message, string.Format(Localization.Get(reward.description), reward.count, reward.count1));
			}
		}
		else
		{
			UISetter.SetLabel(message, string.Format(Localization.Get(reward.description), reward.count, reward.count1));
			if (int.Parse(reward.idx) == 200 || int.Parse(reward.idx) == 201 || int.Parse(reward.idx) == 202)
			{
				reward.count1 = 30;
			}
			else if (int.Parse(reward.idx) >= 300 && int.Parse(reward.idx) < 401)
			{
				flag = false;
			}
		}
		UISetter.SetLabel(completeTime, reward.completeTimeString);
		UISetter.SetActive(conditionCount, flag);
		UISetter.SetLabel(conditionCount, $"({reward.conditionCount} / {((reward.count1 != 0) ? reward.count1 : reward.count)})");
		UISetter.SetSpriteWithSnap(conditionIcon, reward.icon, pixelPerfect: false);
		int num = 0;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		List<RewardDataRow> list2 = ((!RemoteObjectManager.instance.localUser.isMaxLevel) ? regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == (ERewardCategory)((!bAchieve) ? 4 : 3) && row.type == int.Parse(reward.idx) && row.typeIndex == reward.sort) : regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == (ERewardCategory)((!bAchieve) ? 13 : 3) && row.type == int.Parse(reward.idx) && row.typeIndex == reward.sort));
		if (list2.Count < 2)
		{
			UISetter.SetActive(rewardExpCommanderIcon, active: false);
			UISetter.SetSpriteWithSnap(rewardExpIcon, "Goods-exp", pixelPerfect: false);
			UISetter.SetLabel(rewardExpCount, "x" + 0);
			foreach (RewardDataRow row3 in list2)
			{
				UISetter.SetActive(rewardCommanderIcon, row3.rewardType == ERewardType.Medal && !reward.received);
				if (row3.rewardType == ERewardType.Medal)
				{
					UISetter.SetSprite(rewardCommanderIcon, $"{regulation.commanderDtbl.Find((CommanderDataRow list) => list.id == row3.rewardIdx.ToString()).resourceId}_1");
					UISetter.SetSpriteWithSnap(rewardIcon, "icon_c_line_2", pixelPerfect: false);
				}
				else if (row3.rewardIdx != 1000 && regulation.goodsDtbl.ContainsKey(row3.rewardIdx.ToString()))
				{
					UISetter.SetSpriteWithSnap(rewardIcon, regulation.goodsDtbl[row3.rewardIdx.ToString()].iconId, pixelPerfect: false);
				}
				UISetter.SetLabel(rewardCount, "x" + row3.minCount);
			}
		}
		else
		{
			foreach (RewardDataRow row2 in list2)
			{
				if (num == 0)
				{
					UISetter.SetActive(rewardExpCommanderIcon, row2.rewardType == ERewardType.Medal && !reward.received);
					if (row2.rewardType == ERewardType.Medal)
					{
						UISetter.SetSprite(rewardExpCommanderIcon, $"{regulation.commanderDtbl.Find((CommanderDataRow list) => list.id == row2.rewardIdx.ToString()).resourceId}_1");
						UISetter.SetSpriteWithSnap(rewardExpIcon, "icon_c_line_2", pixelPerfect: false);
					}
					else if (regulation.goodsDtbl.ContainsKey(row2.rewardIdx.ToString()))
					{
						UISetter.SetSpriteWithSnap(rewardExpIcon, regulation.goodsDtbl[row2.rewardIdx.ToString()].iconId, pixelPerfect: false);
					}
					UISetter.SetLabel(rewardExpCount, "x" + row2.minCount);
				}
				else
				{
					UISetter.SetActive(rewardCommanderIcon, row2.rewardType == ERewardType.Medal && !reward.received);
					if (row2.rewardType == ERewardType.Medal)
					{
						UISetter.SetSprite(rewardCommanderIcon, $"{regulation.commanderDtbl.Find((CommanderDataRow list) => list.id == row2.rewardIdx.ToString()).resourceId}_1");
						UISetter.SetSpriteWithSnap(rewardIcon, "icon_c_line_2", pixelPerfect: false);
					}
					else if (regulation.goodsDtbl.ContainsKey(row2.rewardIdx.ToString()))
					{
						UISetter.SetSpriteWithSnap(rewardIcon, regulation.goodsDtbl[row2.rewardIdx.ToString()].iconId, pixelPerfect: false);
					}
					UISetter.SetLabel(rewardCount, "x" + row2.minCount);
				}
				num++;
			}
		}
		bool received = reward.received;
		UISetter.SetActive(completedButton, active: false);
		UISetter.SetActive(linkButton, active: false);
		UISetter.SetActive(receiveButton, active: false);
		UISetter.SetActive(confirmButton, active: false);
		UISetter.SetActive(badge, active: false);
		UISetter.SetActive(completeTime, received);
		UISetter.SetActive(rewardExpIcon, !received);
		UISetter.SetActive(rewardExpCount, !received);
		UISetter.SetActive(rewardIcon, !received);
		UISetter.SetActive(rewardCount, !received);
		UISetter.SetActive(conditionCount, !received && flag);
		if (!received)
		{
			if (reward.combleted)
			{
				UISetter.SetActive(receiveButton, active: true);
				UISetter.SetActive(badge, active: true);
			}
			else if (!string.IsNullOrEmpty(reward.link) && reward.link != "-")
			{
				UISetter.SetActive(linkButton, active: true);
			}
			else
			{
				UISetter.SetActive(confirmButton, active: true);
			}
		}
		UISetter.SetGameObjectName(linkButton, $"{_GetOriginalName(linkButton)}-{reward.idx}");
		UISetter.SetGameObjectName(completedButton, $"{_GetOriginalName(completedButton)}-{reward.idx}");
		UISetter.SetGameObjectName(receiveButton, $"{_GetOriginalName(receiveButton)}-{reward.idx}");
		UISetter.SetGameObjectName(confirmButton, $"{_GetOriginalName(confirmButton)}-{reward.idx}");
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

	private void OnDestroy()
	{
		if (conditionIcon != null)
		{
			conditionIcon = null;
		}
	}
}
