using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Shared.Regulation;
using UnityEngine;

public class UICarnivalBasicPopUp : UIPopup
{
	public UITimer remainTimer;

	public UILabel remainTime;

	public UILabel connectTimer;

	public UIDefaultListView carnivalListView;

	private readonly string carnivalItemIdPrefix = "carnival-";

	private readonly string moveBtnIdPrefix = "Move-";

	private readonly string enableBtnIdPrefix = "Enable-";

	private readonly string completeRootIdPrefix = "Complete-";

	private readonly string disableRootIdPrefix = "Disable-";

	private CarnivalTypeDataRow typeData;

	private bool state;

	private int eventIdx;

	private int maxConnectTime;

	public void Init(CarnivalTypeDataRow _typeData, int eidx = 0)
	{
		typeData = _typeData;
		state = true;
		eventIdx = eidx;
		UISetter.SetLabel(title, Localization.Get(typeData.name));
		RefreshData();
		SetTime();
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
		List<CarnivalDataRow> list = base.regulation.carnivalDtbl.FindAll((CarnivalDataRow row) => row.cTidx == typeData.idx);
		list.Sort(delegate(CarnivalDataRow a, CarnivalDataRow b)
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
			return (num == num2) ? int.Parse(a.idx).CompareTo(int.Parse(b.idx)) : num.CompareTo(num2);
		});
		carnivalListView.Init(list, typeData, carnivalItemIdPrefix);
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		RefreshData();
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (!state)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7045"));
		}
		else if (text.StartsWith(moveBtnIdPrefix))
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

	private void StartConnectTimeCount()
	{
		StartCoroutine("CountUp");
	}

	private IEnumerator CheckConnectTime()
	{
		while (true)
		{
			UISetter.SetLabel(connectTimer, $"{GetTimeLabel(base.localUser.connectTime)}/{ConstValue.maxConnectTime}");
			if (base.localUser.connectTime >= ConstValue.maxConnectTime)
			{
				break;
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private string GetTimeLabel(int seconds)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if ((double)seconds <= 0.0)
		{
			return string.Empty;
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		stringBuilder.Length = 0;
		if ((float)(int)timeSpan.TotalMinutes > 0f)
		{
			stringBuilder.Append(Localization.Format("5770", timeSpan.TotalMinutes));
		}
		if ((float)timeSpan.Minutes > 0f)
		{
			stringBuilder.Append(Localization.Format("5771", timeSpan.Seconds));
		}
		return stringBuilder.ToString();
	}
}
