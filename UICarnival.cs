using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICarnival : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIDefaultListView tabListView;

	public UICarnivalBasicPopUp basic;

	public UICarnivalChargePopUp charge;

	public UICarnivalEventPopUp carnivalEvent;

	public UICarnivalVipPackagePopUp vipPackage;

	public UICarnivalLevelPackagePopUp levelPackage;

	public UICarnivalDevelopmentPackagePopUp developmentPackage;

	public UICarnivalNewDevelopmentPackagePopUp newDevelopmentPackage;

	public UICarnivalStageClearPackagePopUp stageClearPackage;

	public Dictionary<string, TimeData> timeDataList;

	public UICarnivalExchangePopup exchange;

	public UICarnivalAttendancePopUp attendance;

	public UICarnivalSelectRewardPopUp selectReward;

	public UICarnivalBulletRewardPopUp bulletReward;

	public UICarnivalRandomRewardPopUp randomReward;

	public UICarnivalDailyPackagePopUp dailyPackage;

	private Protocols.CarnivalList carnivalList;

	private string selectIdx;

	[HideInInspector]
	public ECarnivalCategory categoryType;

	private readonly string tabidPrefix = "Tab-";

	[SerializeField]
	private UILabel mainTitle;

	private int eventIdx;

	public void Init(int eidx)
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			carnivalList = base.localUser.carnivalList;
			eventIdx = eidx;
			CreateCarnivalTabItem();
			OpenPopup();
			OpenContents();
			if (eidx == 0)
			{
				UISetter.SetLabel(mainTitle, Localization.Get("1918"));
			}
			else
			{
				UISetter.SetLabel(mainTitle, Localization.Get("6518"));
			}
		}
	}

	private void CreateCarnivalTabItem()
	{
		tabListView.Init(carnivalList.carnivalList, tabidPrefix);
		string id = tabListView.itemList[0].name.Substring(tabListView.itemList[0].name.IndexOf("-") + 1);
		tabListView.SetSelection(id, selected: true);
		tabListView.ResetPosition();
		selectIdx = id;
	}

	private void RefreshTabList()
	{
		tabListView.Init(carnivalList.carnivalList, tabidPrefix);
		tabListView.SetSelection(selectIdx, selected: true);
	}

	private void OpenContents(bool isRefresh = false)
	{
		CarnivalTypeDataRow carnivalTypeDataRow = base.regulation.carnivalTypeDtbl[selectIdx];
		categoryType = carnivalTypeDataRow.categoryType;
		if (carnivalTypeDataRow.Type == ECarnivalType.SelectReward && !base.localUser.carnivalList.carnivalProcessList.ContainsKey(selectIdx))
		{
			bool flag = false;
			foreach (KeyValuePair<string, Dictionary<string, Protocols.CarnivalList.ProcessData>> carnivalProcess in base.localUser.carnivalList.carnivalProcessList)
			{
				CarnivalTypeDataRow carnivalTypeDataRow2 = base.regulation.carnivalTypeDtbl[carnivalProcess.Key];
				if (carnivalTypeDataRow2.Type == ECarnivalType.SelectReward)
				{
					carnivalTypeDataRow = carnivalTypeDataRow2;
					selectIdx = carnivalTypeDataRow.idx;
					RefreshTabList();
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				carnivalList = base.localUser.carnivalList;
				CreateCarnivalTabItem();
				carnivalTypeDataRow = base.regulation.carnivalTypeDtbl[selectIdx];
			}
		}
		UISetter.SetActive(basic, carnivalTypeDataRow.Type == ECarnivalType.JoinMission || carnivalTypeDataRow.Type == ECarnivalType.Attend || carnivalTypeDataRow.Type == ECarnivalType.Mission || carnivalTypeDataRow.Type == ECarnivalType.EventBattle_Mission || carnivalTypeDataRow.Type == ECarnivalType.EventBattle_Reward);
		UISetter.SetActive(charge, carnivalTypeDataRow.Type == ECarnivalType.JoinCharge || carnivalTypeDataRow.Type == ECarnivalType.Charge);
		UISetter.SetActive(vipPackage, carnivalTypeDataRow.Type == ECarnivalType.VipPackage);
		UISetter.SetActive(carnivalEvent, carnivalTypeDataRow.Type == ECarnivalType.Event);
		UISetter.SetActive(levelPackage, carnivalTypeDataRow.Type == ECarnivalType.LevelPackage);
		UISetter.SetActive(developmentPackage, carnivalTypeDataRow.Type == ECarnivalType.DevelopmentPackage || carnivalTypeDataRow.Type == ECarnivalType.NewUserLevelPackage);
		UISetter.SetActive(exchange, carnivalTypeDataRow.Type == ECarnivalType.NewUserExchangeEvent_Reward || carnivalTypeDataRow.Type == ECarnivalType.NewUserExchangeEvent_Mission || carnivalTypeDataRow.Type == ECarnivalType.ExchangeEvent_Reward || carnivalTypeDataRow.Type == ECarnivalType.ExchangeEvent_Mission || carnivalTypeDataRow.Type == ECarnivalType.EventBattle_Exchange || carnivalTypeDataRow.Type == ECarnivalType.EventBattle_ExchangeOneDay);
		UISetter.SetActive(attendance, carnivalTypeDataRow.Type == ECarnivalType.JoinAttend || carnivalTypeDataRow.Type == ECarnivalType.ReturnUserAttend || carnivalTypeDataRow.Type == ECarnivalType.EventAttend);
		UISetter.SetActive(selectReward, carnivalTypeDataRow.Type == ECarnivalType.SelectReward);
		UISetter.SetActive(bulletReward, carnivalTypeDataRow.Type == ECarnivalType.BulletReward);
		UISetter.SetActive(newDevelopmentPackage, carnivalTypeDataRow.Type == ECarnivalType.NewDevelopmentPackage);
		UISetter.SetActive(stageClearPackage, carnivalTypeDataRow.Type == ECarnivalType.StageClearPackage);
		UISetter.SetActive(randomReward, carnivalTypeDataRow.Type == ECarnivalType.RandomReward);
		UISetter.SetActive(dailyPackage, carnivalTypeDataRow.Type == ECarnivalType.DailyPackage);
		switch (carnivalTypeDataRow.Type)
		{
		case ECarnivalType.JoinMission:
		case ECarnivalType.Mission:
		case ECarnivalType.Attend:
			if (!isRefresh)
			{
				basic.Init(carnivalTypeDataRow);
			}
			else
			{
				basic.OnRefresh();
			}
			break;
		case ECarnivalType.EventBattle_Mission:
		case ECarnivalType.EventBattle_Reward:
			if (!isRefresh)
			{
				basic.Init(carnivalTypeDataRow, eventIdx);
			}
			else
			{
				basic.OnRefresh();
			}
			break;
		case ECarnivalType.JoinCharge:
		case ECarnivalType.Charge:
			charge.Init(carnivalTypeDataRow);
			break;
		case ECarnivalType.Event:
			carnivalEvent.Init(carnivalTypeDataRow);
			break;
		case ECarnivalType.VipPackage:
			if (!isRefresh)
			{
				vipPackage.Init(carnivalTypeDataRow);
			}
			else
			{
				vipPackage.OnRefresh();
			}
			break;
		case ECarnivalType.LevelPackage:
			levelPackage.Init(carnivalTypeDataRow);
			break;
		case ECarnivalType.DevelopmentPackage:
		case ECarnivalType.NewUserLevelPackage:
			if (!isRefresh)
			{
				developmentPackage.Init(carnivalTypeDataRow);
			}
			else
			{
				developmentPackage.OnRefresh();
			}
			break;
		case ECarnivalType.NewUserExchangeEvent_Reward:
		case ECarnivalType.NewUserExchangeEvent_Mission:
		case ECarnivalType.ExchangeEvent_Reward:
		case ECarnivalType.ExchangeEvent_Mission:
			if (!isRefresh)
			{
				exchange.Init(carnivalTypeDataRow);
			}
			else
			{
				exchange.OnRefresh();
			}
			break;
		case ECarnivalType.EventBattle_Exchange:
		case ECarnivalType.EventBattle_ExchangeOneDay:
			if (!isRefresh)
			{
				exchange.Init(carnivalTypeDataRow, eventIdx);
			}
			else
			{
				exchange.OnRefresh();
			}
			break;
		case ECarnivalType.JoinAttend:
		case ECarnivalType.ReturnUserAttend:
		case ECarnivalType.EventAttend:
			if (!isRefresh)
			{
				attendance.Init(carnivalTypeDataRow);
			}
			else
			{
				attendance.OnRefresh();
			}
			break;
		case ECarnivalType.SelectReward:
			selectReward.Init(carnivalTypeDataRow);
			break;
		case ECarnivalType.BulletReward:
			bulletReward.Init(carnivalTypeDataRow);
			break;
		case ECarnivalType.NewDevelopmentPackage:
			newDevelopmentPackage.Init(carnivalTypeDataRow);
			break;
		case ECarnivalType.StageClearPackage:
			stageClearPackage.Init(carnivalTypeDataRow);
			break;
		case ECarnivalType.RandomReward:
			randomReward.Init(carnivalTypeDataRow);
			break;
		case ECarnivalType.DailyPackage:
			dailyPackage.Init(carnivalTypeDataRow);
			break;
		}
		if (isRefresh)
		{
			RefreshTabList();
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close")
		{
			ClosePopup();
		}
		else if (text.StartsWith(tabidPrefix))
		{
			string id = text.Substring(text.IndexOf("-") + 1);
			tabListView.SetSelection(id, selected: true);
			selectIdx = id;
			OpenContents();
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		OpenContents(isRefresh: true);
	}

	private void StartConnectTimeCount()
	{
	}

	private IEnumerator CountUp()
	{
		while (true)
		{
			base.localUser.connectTime++;
			if (base.localUser.connectTime >= ConstValue.maxConnectTime)
			{
				break;
			}
			yield return new WaitForSeconds(1f);
		}
		base.localUser.connectTime = ConstValue.maxConnectTime;
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
	}

	private void OnAnimClose()
	{
	}
}
