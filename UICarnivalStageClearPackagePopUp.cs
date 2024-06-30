using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICarnivalStageClearPackagePopUp : UIPopup
{
	public UITimer remainTimer;

	public UILabel description;

	public GameObject buyBtn;

	public GameObject completeRoot;

	public UISprite costIcon;

	public UILabel cost;

	public UIDefaultListView developmentListView;

	private CarnivalTypeDataRow typeData;

	private bool state;

	private readonly string buyBtnIdPrefix = "Buy-";

	private readonly string enableBtnIdPrefix = "Enable-";

	[SerializeField]
	private GameObject terms;

	public void Init(CarnivalTypeDataRow _typeData)
	{
		typeData = _typeData;
		state = true;
		UISetter.SetLabel(title, Localization.Get(typeData.name));
		developmentListView.ResetPosition();
		InAppProductDataRow inAppProductDataRow = base.regulation.inAppProductDtbl.Find((InAppProductDataRow item) => item.iapidx == 204);
		UISetter.SetActive(terms, active: true);
		UISetter.SetActive(buyBtn, active: true);
		UISetter.SetGameObjectName(buyBtn, $"{buyBtnIdPrefix}{inAppProductDataRow.googlePlayID}");
		if (Localization.language == "S_Kr")
		{
			UISetter.SetSprite(costIcon, "store_won", snap: true);
			UISetter.SetLabel(cost, int.Parse(inAppProductDataRow.wonPrice).ToString("N0"));
		}
		else
		{
			UISetter.SetSprite(costIcon, "store_dollar", snap: true);
			UISetter.SetLabel(cost, inAppProductDataRow.dollarPrice);
		}
		SetTime();
		RefreshData();
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
			return (num == num2) ? a.idx.CompareTo(b.idx) : num.CompareTo(num2);
		});
		developmentListView.InitDevelopmentList(list, typeData.idx);
		SetButton();
	}

	private void SetButton()
	{
		bool flag = false;
		Protocols.CarnivalList carnivalList = RemoteObjectManager.instance.localUser.carnivalList;
		if (carnivalList.carnivalProcessList != null && carnivalList.carnivalProcessList.ContainsKey(typeData.idx))
		{
			flag = true;
		}
		UISetter.SetActive(buyBtn, !flag);
		UISetter.SetActive(completeRoot, flag);
	}

	public void SetTime()
	{
		TimeData remainTimeData = base.localUser.carnivalList.carnivalList[typeData.idx].remainTimeData;
		if (!(remainTimer == null))
		{
			remainTimer.SetLabelFormat(string.Format("{0}:", Localization.Get("4025")), string.Empty);
			remainTimer.RegisterOnFinished(delegate
			{
				state = false;
			});
			remainTimer.Set(remainTimeData);
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		RefreshData();
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text.StartsWith(buyBtnIdPrefix))
		{
			string pid = text.Substring(text.IndexOf("-") + 1);
			RemoteObjectManager.instance.RequestMakeOrderId(pid);
		}
		else if (text.StartsWith(enableBtnIdPrefix))
		{
			string idx = text.Substring(text.IndexOf("-") + 1);
			base.network.RequestCarnivalComplete(int.Parse(typeData.idx), idx);
		}
	}
}
