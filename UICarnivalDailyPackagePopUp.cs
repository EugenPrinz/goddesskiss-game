using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICarnivalDailyPackagePopUp : UIPopup
{
	public UITimer remainTimer;

	public UIDefaultListView dailyPackageList;

	private CarnivalTypeDataRow typeData;

	private readonly string enableBtnIdPrefix = "Enable-";

	private readonly string buyBtnIdPrefix = "Buy-";

	private readonly string rewardListBtnIdPrefix = "RewardList-";

	private bool state;

	public void Init(CarnivalTypeDataRow _typeData)
	{
		typeData = _typeData;
		state = true;
		UISetter.SetLabel(title, Localization.Get(typeData.name));
		List<CarnivalDataRow> list = new List<CarnivalDataRow>();
		if (base.localUser.carnivalList.carnivalProcessList.ContainsKey(typeData.idx))
		{
			List<string> list2 = new List<string>(base.localUser.carnivalList.carnivalProcessList[typeData.idx].Keys);
			for (int i = 0; i < list2.Count; i++)
			{
				string key = list2[i];
				CarnivalDataRow item = base.regulation.carnivalDtbl.Find((CarnivalDataRow row) => row.idx == key);
				list.Add(item);
			}
		}
		SetTime();
		dailyPackageList.InitDailyPackageList(list, typeData);
	}

	public void SetTime()
	{
		TimeData remainTimeData = base.localUser.carnivalList.carnivalList[typeData.idx].remainTimeData;
		if (!(remainTimer == null) && !(remainTimeData.GetRemain() <= 0.0))
		{
			remainTimer.SetLabelFormat(string.Format("{0}:", Localization.Get("4025")), string.Empty);
			remainTimer.RegisterOnFinished(delegate
			{
				state = false;
			});
			remainTimer.Set(remainTimeData);
		}
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text.StartsWith(enableBtnIdPrefix))
		{
			string idx = text.Substring(text.IndexOf("-") + 1);
			base.network.RequestCarnivalComplete(int.Parse(typeData.idx), idx);
		}
		else if (text.StartsWith(buyBtnIdPrefix))
		{
			string pid = text.Substring(text.IndexOf("-") + 1);
			RemoteObjectManager.instance.RequestMakeOrderId(pid);
		}
		else if (text.StartsWith(rewardListBtnIdPrefix))
		{
			string cId = text.Substring(text.IndexOf("-") + 1);
			UIPopup.Create<UICarnivalDailyPackagePreViewPopUp>("CarnivalDailyPackagePreViewPopUp").InitAndOpenItemList(cId);
		}
	}
}
