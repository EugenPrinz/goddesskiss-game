using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICarnivalExchangePopup : UIPopup
{
	public UITimer remainTimer;

	public UIDefaultListView carnivalListView;

	private readonly string carnivalItemIdPrefix = "carnival-";

	private readonly string moveBtnIdPrefix = "Move-";

	private readonly string enableBtnIdPrefix = "Enable-";

	private readonly string completeRootIdPrefix = "Complete-";

	private readonly string disableRootIdPrefix = "Disable-";

	public CarnivalExchangePopup exchangePopup;

	private CarnivalTypeDataRow typeData;

	private bool state;

	private int eventIdx;

	public void Init(CarnivalTypeDataRow _typeData, int eidx = 0)
	{
		typeData = _typeData;
		state = true;
		eventIdx = eidx;
		UISetter.SetLabel(title, Localization.Get(typeData.name));
		SetTime();
		RefreshData();
		carnivalListView.ResetPosition();
	}

	public void SetTime()
	{
		TimeData remainTimeData = base.localUser.carnivalList.carnivalList[typeData.idx].remainTimeData;
		if (remainTimer == null)
		{
			return;
		}
		if (UIManager.instance.world.existEventBattle && UIManager.instance.world.eventBattle.isActive && UIManager.instance.world.eventBattle.GetEventType() == 0)
		{
			UISetter.SetActive(remainTimer, active: false);
			return;
		}
		UISetter.SetActive(remainTimer, active: true);
		remainTimer.SetLabelFormat(string.Format("{0}:\n", Localization.Get("4025")), string.Empty);
		remainTimer.RegisterOnFinished(delegate
		{
			state = false;
		});
		remainTimer.Set(remainTimeData);
	}

	private void RefreshData()
	{
		if (base.localUser.carnivalList != null && base.localUser.carnivalList.carnivalProcessList != null && base.localUser.carnivalList.carnivalProcessList.Count > 0)
		{
			Dictionary<string, Dictionary<string, Protocols.CarnivalList.ProcessData>> carnivalProcessList = base.localUser.carnivalList.carnivalProcessList;
			if (carnivalProcessList.ContainsKey(typeData.idx))
			{
				foreach (KeyValuePair<string, Protocols.CarnivalList.ProcessData> item in carnivalProcessList[typeData.idx])
				{
					if (item.Value.receive == 1)
					{
						item.Value.complete = 1;
					}
					else
					{
						item.Value.complete = (base.localUser.IsCompleteExchangeCarnival(item.Key) ? 1 : 0);
					}
				}
			}
		}
		List<CarnivalDataRow> carnivalList = new List<CarnivalDataRow>();
		string prevIdx = string.Empty;
		base.regulation.carnivalDtbl.ForEach(delegate(CarnivalDataRow row)
		{
			if (row.cTidx == typeData.idx && prevIdx != row.idx)
			{
				carnivalList.Add(row);
				prevIdx = row.idx;
			}
		});
		carnivalList.Sort(delegate(CarnivalDataRow a, CarnivalDataRow b)
		{
			int num = 1;
			int num2 = 1;
			if (base.localUser.carnivalList.carnivalProcessList.ContainsKey(a.cTidx) && base.localUser.carnivalList.carnivalProcessList[a.cTidx].ContainsKey(a.idx))
			{
				int complete = base.localUser.carnivalList.carnivalProcessList[a.cTidx][a.idx].complete;
				int receive = base.localUser.carnivalList.carnivalProcessList[a.cTidx][a.idx].receive;
				num = ((receive == 1) ? 2 : ((complete != 1) ? 1 : 0));
			}
			if (base.localUser.carnivalList.carnivalProcessList.ContainsKey(b.cTidx) && base.localUser.carnivalList.carnivalProcessList[b.cTidx].ContainsKey(b.idx))
			{
				int complete2 = base.localUser.carnivalList.carnivalProcessList[b.cTidx][b.idx].complete;
				int receive2 = base.localUser.carnivalList.carnivalProcessList[b.cTidx][b.idx].receive;
				num2 = ((receive2 == 1) ? 2 : ((complete2 != 1) ? 1 : 0));
			}
			return (num == num2) ? a.idx.CompareTo(b.idx) : num.CompareTo(num2);
		});
		carnivalListView.Init(carnivalList, typeData, carnivalItemIdPrefix);
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		RefreshData();
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text.StartsWith(moveBtnIdPrefix))
		{
			string text2 = text.Substring(text.IndexOf("-") + 1);
			if (text2 != "Shop_Bullet" && text2 != "Shop_Diamond")
			{
				UIManager.instance.world.carnival.ClosePopup();
			}
			UIManager.instance.world.camp.GoNavigation(text2);
		}
		else if (text.StartsWith(enableBtnIdPrefix))
		{
			string idx = text.Substring(text.IndexOf("-") + 1);
			base.network.RequestCarnivalComplete(int.Parse(typeData.idx), idx, eventIdx);
		}
	}

	public int GetEventID()
	{
		return eventIdx;
	}
}
