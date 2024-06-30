using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICarnivalListItem : UIItemBase
{
	public UILabel title;

	public UILabel process;

	public UIDefaultListView rewardListView;

	public UISprite bg;

	public GameObject moveBtn;

	public GameObject enableBtn;

	public GameObject completeRoot;

	public GameObject disableRoot;

	public GameObject SelectRoot;

	private readonly string moveBtnIdPrefix = "Move-";

	private readonly string enableBtnIdPrefix = "Enable-";

	private readonly string completeRootIdPrefix = "Complete-";

	private readonly string disableRootIdPrefix = "Disable-";

	[SerializeField]
	private UIDefaultListView CurItemListView;

	private List<CurCarnivalItemInfo> carnivalItem = new List<CurCarnivalItemInfo>();

	private List<RewardDataRow> rewardList = new List<RewardDataRow>();

	private CarnivalDataRow carnivalDB;

	private ECarnivalType type;

	private int receiveCount;

	public void Set(string cTidx, CarnivalDataRow row)
	{
		Protocols.CarnivalList carnivalList = RemoteObjectManager.instance.localUser.carnivalList;
		rewardList.Clear();
		carnivalDB = row;
		type = RemoteObjectManager.instance.regulation.FindCarnivalType(cTidx);
		rewardList = RemoteObjectManager.instance.regulation.rewardDtbl.FindAll((RewardDataRow item) => item.category == ERewardCategory.Carnival && item.type == int.Parse(row.idx));
		List<CarnivalDataRow> list = RemoteObjectManager.instance.regulation.carnivalDtbl.FindAll((CarnivalDataRow item) => item.idx == row.idx);
		carnivalItem.Clear();
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		if (carnivalList.carnivalProcessList != null && carnivalList.carnivalProcessList.ContainsKey(cTidx) && carnivalList.carnivalProcessList[cTidx].ContainsKey(row.idx))
		{
			num = carnivalList.carnivalProcessList[cTidx][row.idx].count;
			flag = ((carnivalList.carnivalProcessList[cTidx][row.idx].complete != 0) ? true : false);
			flag2 = ((carnivalList.carnivalProcessList[cTidx][row.idx].receive != 0) ? true : false);
		}
		receiveCount = num;
		rewardListView.InitRewardList(rewardList);
		switch (type)
		{
		case ECarnivalType.NewUserExchangeEvent_Reward:
		case ECarnivalType.NewUserExchangeEvent_Mission:
		case ECarnivalType.ExchangeEvent_Reward:
		case ECarnivalType.ExchangeEvent_Mission:
		case ECarnivalType.EventBattle_Exchange:
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					CurCarnivalItemInfo item2 = default(CurCarnivalItemInfo);
					item2.idx = list[i].commodityIdx;
					item2.type = list[i].commodityType;
					item2.needCount = list[i].commodityCount;
					carnivalItem.Add(item2);
				}
				CurItemListView.InitCarnivalRewardList(carnivalItem);
			}
			break;
		}
		Regulation regulation = RemoteObjectManager.instance.regulation;
		switch (row.check2Type)
		{
		case EventCarnivalCheckType.None:
			UISetter.SetLabel(title, Localization.Format(row.explanation, row.checkCount));
			break;
		case EventCarnivalCheckType.Goods:
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl.Find((GoodsDataRow data) => data.type == row.check2);
			if (goodsDataRow != null)
			{
				UISetter.SetLabel(title, Localization.Format(row.explanation, row.checkCount, Localization.Get(goodsDataRow.name)));
			}
			break;
		}
		case EventCarnivalCheckType.Enemy:
		{
			EnemyCommanderDataRow enemyCommanderDataRow = regulation.enemyCommanderDtbl.Find((EnemyCommanderDataRow data) => data.id == row.check2);
			if (enemyCommanderDataRow != null)
			{
				UISetter.SetLabel(title, Localization.Format(row.explanation, row.checkCount, Localization.Get(enemyCommanderDataRow.name)));
			}
			break;
		}
		case EventCarnivalCheckType.Commander:
		{
			RoCommander roCommander2 = localUser.FindCommander(row.check2);
			if (roCommander2 != null)
			{
				if (row.checkType == 502)
				{
					int num2 = 8900 + row.checkCount;
					UISetter.SetLabel(title, Localization.Format(row.explanation, Localization.Get(num2.ToString()), roCommander2.nickname));
				}
				else
				{
					UISetter.SetLabel(title, Localization.Format(row.explanation, row.checkCount, roCommander2.nickname));
				}
			}
			break;
		}
		case EventCarnivalCheckType.Scenario:
		{
			RoCommander roCommander = localUser.FindCommander(row.check2);
			if (roCommander != null)
			{
				CommanderScenarioDataRow commanderScenarioDataRow = regulation.FindCommanderScenario(row.checkCount);
				if (commanderScenarioDataRow != null)
				{
					UISetter.SetLabel(title, Localization.Format(row.explanation, commanderScenarioDataRow.order, roCommander.nickname));
				}
			}
			break;
		}
		}
		UISetter.SetGameObjectName(moveBtn, $"{moveBtnIdPrefix}{row.link}");
		UISetter.SetGameObjectName(enableBtn, $"{enableBtnIdPrefix}{row.idx}");
		UISetter.SetActive(moveBtn, row.link != "0" && !flag);
		UISetter.SetActive(disableRoot, row.link == "0" && !flag);
		UISetter.SetActive(completeRoot, flag && flag2);
		UISetter.SetActive(enableBtn, flag && !flag2);
		UISetter.SetActive(process, row.checkType != 1 && row.checkType != 11 && row.checkType != 31 && row.checkType != 203 && row.checkType != 205 && row.checkType != 206 && row.checkType != 207);
		if (row.checkType == 506)
		{
			UISetter.SetLabel(process, $"{GetScenarioCompleteRate(row.checkCount)}/{100}");
		}
		else
		{
			UISetter.SetLabel(process, $"{num}/{row.checkCount}");
		}
		if (moveBtn.activeSelf || disableRoot.activeSelf)
		{
			UISetter.SetSprite(bg, "com_bg_popup_inside");
		}
		else if (enableBtn.activeSelf)
		{
			UISetter.SetSprite(bg, "login_bg_sever_select");
		}
		else if (completeRoot.activeSelf)
		{
			UISetter.SetSprite(bg, "com_bg_popup_inside4");
		}
	}

	private int GetScenarioCompleteRate(int csid)
	{
		CommanderScenarioDataRow commanderScenarioDataRow = RemoteObjectManager.instance.regulation.FindScenarioInfo(csid);
		int num = 0;
		float num2 = RemoteObjectManager.instance.regulation.GetCompleteScenarioQuarterCount(commanderScenarioDataRow.csid.ToString());
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		List<string> list = new List<string>();
		if (localUser.sn_resultDictionary != null && localUser.sn_resultDictionary.ContainsKey(commanderScenarioDataRow.cid))
		{
			Dictionary<string, Protocols.CommanderScenario> dictionary = localUser.sn_resultDictionary[commanderScenarioDataRow.cid];
			if (dictionary.ContainsKey(commanderScenarioDataRow.csid.ToString()))
			{
				list = dictionary[commanderScenarioDataRow.csid.ToString()].complete;
			}
		}
		return num = (int)((float)list.Count / num2 * 100f);
	}

	public void SetExchangePopup()
	{
		if (UIManager.instance.world.existCarnival)
		{
			UICarnival carnival = UIManager.instance.world.carnival;
			if (carnival.isActive && carnival.exchange.isActive && !carnival.exchange.exchangePopup.gameObject.activeSelf)
			{
				UISetter.SetActive(carnival.exchange.exchangePopup, active: true);
				carnival.exchange.exchangePopup.InitPopup(carnivalItem, rewardList, carnivalDB.checkCount - receiveCount, int.Parse(carnivalDB.cTidx), carnivalDB.idx);
			}
		}
	}
}
