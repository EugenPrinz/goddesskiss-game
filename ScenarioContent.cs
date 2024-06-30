using System;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class ScenarioContent : UIItemBase
{
	[Serializable]
	public class OpenContent
	{
		[SerializeField]
		private UILabel Desc;

		[SerializeField]
		private UILabel rate;

		[SerializeField]
		private UILabel reward;

		[SerializeField]
		private GameObject completeButton;

		[SerializeField]
		private GameObject getRewardButton;

		[SerializeField]
		private GameObject Lock;

		[SerializeField]
		private UILabel btnTitle;

		[SerializeField]
		private UIGoods Reward;

		[SerializeField]
		private UILabel completeButtonLabel;

		private int completeRate;

		private int commanderId = -1;

		private int scenarioId = -1;

		private bool isGetReward;

		private const int MAX_RATE = 100;

		public void SetOpenContent(string desc, int _rate, string cid, int sid, bool isGetCompleteReward)
		{
			UISetter.SetLabel(Desc, Localization.Get(desc));
			UISetter.SetActive(Lock, active: false);
			UISetter.SetLabel(rate, $"{_rate}%");
			completeRate = _rate;
			commanderId = int.Parse(cid);
			scenarioId = sid;
			isGetReward = isGetCompleteReward;
			SetReward(commanderId, scenarioId);
			SetButton();
		}

		public void OnClick(GameObject sender)
		{
			RoLocalUser localUser = RemoteObjectManager.instance.localUser;
			localUser.currScenario.scenarioId = scenarioId;
			localUser.currScenario.commanderId = commanderId;
			switch (sender.name)
			{
			case "startButton":
				Loading.Load(Loading.Scenario);
				break;
			case "getRewardButton":
				RemoteObjectManager.instance.RequestRecieveCommanderScenarioReward(commanderId, scenarioId);
				break;
			case "Complete":
				if (completeRate < 100)
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("20011"));
				}
				break;
			}
		}

		public void SetButton()
		{
			if (completeRate <= 0)
			{
				UISetter.SetLabel(btnTitle, Localization.Get("20007"));
			}
			else
			{
				UISetter.SetLabel(btnTitle, Localization.Get("20008"));
			}
			UISetter.SetActive(getRewardButton, !isGetReward && completeRate >= 100);
			UISetter.SetActive(completeButton, isGetReward || completeRate < 100);
			if (isGetReward)
			{
				UISetter.SetLabel(completeButtonLabel, Localization.Get("20010"));
			}
			else if (completeRate < 100)
			{
				UISetter.SetLabel(completeButtonLabel, Localization.Get("20009"));
			}
		}

		private void SetReward(int cid, int csid)
		{
			RewardDataRow rewardDataRow = RemoteObjectManager.instance.regulation.FindScenarioCompleteReward(cid, csid);
			if (rewardDataRow != null)
			{
				Reward.Set(rewardDataRow);
			}
		}
	}

	[Serializable]
	public class CloseContent
	{
		[SerializeField]
		private UILabel content;

		[SerializeField]
		private UILabel title;

		[SerializeField]
		private ScenarioContent sn_contents;

		[SerializeField]
		private List<UILabel> labelList;

		[SerializeField]
		private GameObject Lock;

		public void Init(List<ScenarioOpenCondition> conditionList)
		{
			ResetLabel();
			for (int i = 0; i < conditionList.Count; i++)
			{
				UISetter.SetActive(labelList[i], active: true);
				if (conditionList[i].isComplete)
				{
					labelList[i].color = new Color(0f, 0.6784314f, 79f / 85f);
					UISetter.SetLabel(labelList[i], conditionList[i].str_condition);
				}
				else
				{
					labelList[i].color = new Color(0.4627451f, 24f / 85f, 4f / 85f);
					UISetter.SetLabel(labelList[i], conditionList[i].str_condition);
				}
			}
			UISetter.SetActive(Lock, active: true);
		}

		private void ResetLabel()
		{
			for (int i = 0; i < labelList.Count; i++)
			{
				UISetter.SetActive(labelList[i], active: false);
			}
		}
	}

	[SerializeField]
	private UILabel Order;

	[SerializeField]
	private UILabel title;

	[SerializeField]
	private GameObject Open_obj;

	[SerializeField]
	private GameObject Close_obj;

	[SerializeField]
	private OpenContent open_content;

	[SerializeField]
	private CloseContent close_content;

	private CommanderScenarioDataRow scenarioDB;

	private bool isGetReward;

	public void Set(int csid, List<ScenarioOpenCondition> openConditionList)
	{
		scenarioDB = RemoteObjectManager.instance.regulation.FindScenarioInfo(csid);
		UISetter.SetLabel(text: (scenarioDB.heart == 0) ? Localization.Get("20110") : ((scenarioDB.heart == 2) ? Localization.Format("20111", scenarioDB.order) : ((scenarioDB.heart != 3) ? Localization.Format("20004", scenarioDB.order) : Localization.Format("20112", scenarioDB.order))), label: Order);
		UISetter.SetLabel(title, Localization.Get(scenarioDB.name));
		SetContents(openConditionList);
	}

	private void SetContents(List<ScenarioOpenCondition> openConditionList)
	{
		int num = 0;
		if (openConditionList.Count > 0)
		{
			UISetter.SetActive(Open_obj, active: false);
			UISetter.SetActive(Close_obj, active: true);
			for (int i = 0; i < openConditionList.Count; i++)
			{
				if (!openConditionList[i].isComplete)
				{
					close_content.Init(openConditionList);
					return;
				}
				num++;
			}
		}
		if (openConditionList.Count == num)
		{
			UISetter.SetActive(Close_obj, active: false);
			UISetter.SetActive(Open_obj, active: true);
			SetCompleteRate();
		}
	}

	public void SetCompleteRate()
	{
		int num = 0;
		float num2 = RemoteObjectManager.instance.regulation.GetCompleteScenarioQuarterCount(scenarioDB.csid.ToString());
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		bool isGetCompleteReward = false;
		List<string> list = new List<string>();
		if (localUser.sn_resultDictionary != null && localUser.sn_resultDictionary.ContainsKey(scenarioDB.cid))
		{
			Dictionary<string, Protocols.CommanderScenario> dictionary = localUser.sn_resultDictionary[scenarioDB.cid];
			if (dictionary.ContainsKey(scenarioDB.csid.ToString()))
			{
				list = dictionary[scenarioDB.csid.ToString()].complete;
				if (dictionary[scenarioDB.csid.ToString()].receive == 1)
				{
					isGetCompleteReward = true;
				}
			}
		}
		num = (int)((float)list.Count / num2 * 100f);
		open_content.SetOpenContent(scenarioDB.desc, num, scenarioDB.cid, scenarioDB.csid, isGetCompleteReward);
	}

	public void OnClick(GameObject sender)
	{
		open_content.OnClick(sender);
	}
}
