using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class ScenarioListItem : UIItemBase
{
	[SerializeField]
	private UILabel ListOrder;

	[SerializeField]
	private UILabel Title;

	[SerializeField]
	private ScenarioContent SN_Content;

	[SerializeField]
	private GameObject selectRoot;

	[SerializeField]
	private GameObject Lock;

	private CommanderScenarioDataRow scenarioDB;

	[SerializeField]
	private UIDefaultListView ScenarioList;

	[SerializeField]
	private UISprite heart_r18;

	public List<ScenarioOpenCondition> openConditionList = new List<ScenarioOpenCondition>();

	public void Set(CommanderScenarioDataRow s_db)
	{
		if (s_db != null)
		{
			scenarioDB = s_db;
			if (scenarioDB.heart == 0)
			{
				UISetter.SetLabel(ListOrder, Localization.Get("20110"));
				UISetter.SetSprite(heart_r18, "wd-ringbox");
			}
			else if (scenarioDB.heart == 2)
			{
				UISetter.SetLabel(ListOrder, string.Format(Localization.Get("20111"), scenarioDB.order));
				UISetter.SetSprite(heart_r18, "r18-heart");
			}
			else if (scenarioDB.heart == 3)
			{
				UISetter.SetLabel(ListOrder, string.Format(Localization.Get("20112"), scenarioDB.order));
				UISetter.SetSprite(heart_r18, "wd-ring");
			}
			else
			{
				UISetter.SetLabel(ListOrder, string.Format(Localization.Get("20004"), scenarioDB.order));
			}
			UISetter.SetLabel(Title, Localization.Get(scenarioDB.name));
			SetCompleteMissionList();
			UISetter.SetActive(Lock, IsLockContents());
			UISetter.SetActive(heart_r18, s_db.heart != 1);
		}
	}

	public void OnClick()
	{
		SN_Content.Set(scenarioDB.csid, openConditionList);
		ScenarioList.SetSelection(scenarioDB.csid.ToString(), selected: true);
	}

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(selectRoot, selected);
	}

	private void SetCompleteMissionList()
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		if (localUser == null)
		{
			return;
		}
		RoCommander roCommander = localUser.FindCommander(scenarioDB.cid);
		Regulation regulation = RemoteObjectManager.instance.regulation;
		if (roCommander == null || regulation == null)
		{
			return;
		}
		if (openConditionList.Count > 0)
		{
			openConditionList.Clear();
		}
		if (scenarioDB.level > 0)
		{
			ListAdd(scenarioDB.level, roCommander.level, string.Format(Localization.Get("20013"), scenarioDB.level));
		}
		if (scenarioDB.grade > 0)
		{
			ListAdd(scenarioDB.grade, roCommander.rank, string.Format(Localization.Get("20014"), scenarioDB.grade));
		}
		if (scenarioDB.cls > 0)
		{
			ListAdd(scenarioDB.cls, roCommander.cls, string.Format(Localization.Get("20015"), scenarioDB.cls));
		}
		if (scenarioDB.favor > 0)
		{
			ListAdd(scenarioDB.favor, roCommander.favorStep, string.Format(Localization.Get("20016"), scenarioDB.favor));
		}
		if (scenarioDB.mapClear > 0)
		{
			WorldMapStageDataRow worldMapStageDataRow = regulation.worldMapStageDtbl[scenarioDB.mapClear];
			if (worldMapStageDataRow != null)
			{
				ScenarioOpenCondition item = default(ScenarioOpenCondition);
				item.isComplete = ((localUser.lastClearStage >= int.Parse(worldMapStageDataRow.id)) ? true : false);
				item.str_condition = string.Format(Localization.Get("20017"), worldMapStageDataRow.worldMapId, worldMapStageDataRow.order);
				openConditionList.Add(item);
			}
		}
		if (scenarioDB.commander > 0)
		{
			RoCommander roCommander2 = RemoteObjectManager.instance.localUser.FindCommander(scenarioDB.commander.ToString());
			if (roCommander2 != null)
			{
				ScenarioOpenCondition item2 = default(ScenarioOpenCondition);
				item2.isComplete = roCommander2.state == ECommanderState.Nomal;
				item2.str_condition = string.Format(Localization.Get("20018"), roCommander2.nickname);
				openConditionList.Add(item2);
			}
		}
		CommanderScenarioDataRow commanderScenarioDataRow = regulation.FindCommanderScenario(scenarioDB.scenarioClear);
		if (commanderScenarioDataRow != null)
		{
			ScenarioOpenCondition item3 = default(ScenarioOpenCondition);
			RoLocalUser.ScenarioCompleteInfo scenarioCompleteInfo = localUser.GetScenarioCompleteInfo(commanderScenarioDataRow.cid, commanderScenarioDataRow.csid.ToString());
			item3.str_condition = string.Format(Localization.Get("20019"), Localization.Get(commanderScenarioDataRow.name));
			if (scenarioCompleteInfo.completeQuarterIdx != null && scenarioCompleteInfo.completeQuarterIdx.Count > 0)
			{
				item3.isComplete = true;
			}
			else
			{
				item3.isComplete = false;
			}
			openConditionList.Add(item3);
		}
		if (scenarioDB.heart == 0)
		{
			ScenarioOpenCondition item4 = default(ScenarioOpenCondition);
			item4.isComplete = (int)roCommander.marry == 1;
			item4.str_condition = Localization.Get("20120");
			openConditionList.Add(item4);
		}
	}

	private void ListAdd(int scenarioCondition, int commanderCondition, string str_condition)
	{
		ScenarioOpenCondition item = default(ScenarioOpenCondition);
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		if (scenarioCondition > commanderCondition)
		{
			item.isComplete = false;
		}
		else
		{
			item.isComplete = true;
		}
		item.str_condition = str_condition;
		openConditionList.Add(item);
	}

	private bool IsLockContents()
	{
		int num = 0;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		if (openConditionList.Count > 0)
		{
			for (int i = 0; i < openConditionList.Count; i++)
			{
				if (!openConditionList[i].isComplete)
				{
					return true;
				}
				num++;
			}
		}
		return false;
	}
}
