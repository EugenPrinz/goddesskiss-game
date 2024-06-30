using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICarnivalLevelPackagePopUp : UIPopup
{
	public UITimer remainTimer;

	public UILabel description;

	public UILabel cost;

	public GameObject enableBtn;

	public GameObject buyBtn;

	public GameObject completeRoot;

	public UISprite costIcon;

	public UIDefaultListView rewardListView;

	private readonly string enableBtnIdPrefix = "Enable-";

	private readonly string buyBtnIdPrefix = "Buy-";

	private CarnivalTypeDataRow typeData;

	private bool state;

	public void Init(CarnivalTypeDataRow _typeData)
	{
		typeData = _typeData;
		state = true;
		UISetter.SetLabel(title, Localization.Get(typeData.name));
		Protocols.CarnivalList carnivalList = RemoteObjectManager.instance.localUser.carnivalList;
		CarnivalDataRow carnivalData = base.regulation.carnivalDtbl.Find((CarnivalDataRow row) => row.cTidx == typeData.idx);
		List<RewardDataRow> list = base.regulation.rewardDtbl.FindAll((RewardDataRow item) => item.category == ERewardCategory.Carnival && item.type == int.Parse(carnivalData.idx));
		rewardListView.InitRewardList(list);
		InAppProductDataRow inAppProductDataRow = base.regulation.inAppProductDtbl.Find((InAppProductDataRow item) => item.iapidx == carnivalData.checkCount);
		UISetter.SetGameObjectName(enableBtn, $"{enableBtnIdPrefix}{carnivalData.idx}");
		UISetter.SetGameObjectName(buyBtn, $"{buyBtnIdPrefix}{inAppProductDataRow.googlePlayID}");
		UISetter.SetSprite(costIcon, (!(Localization.language == "S_Kr")) ? "store_dollar" : "store_won", snap: true);
		UISetter.SetLabel(cost, (!(Localization.language == "S_Kr")) ? inAppProductDataRow.dollarPrice : int.Parse(inAppProductDataRow.wonPrice).ToString("N0"));
		UISetter.SetLabel(description, Localization.Format(carnivalData.explanation, _typeData.endLevel));
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		if (carnivalList.carnivalProcessList != null && carnivalList.carnivalProcessList.ContainsKey(_typeData.idx) && carnivalList.carnivalProcessList[_typeData.idx].ContainsKey(carnivalData.idx))
		{
			num = carnivalList.carnivalProcessList[_typeData.idx][carnivalData.idx].count;
			flag = ((carnivalList.carnivalProcessList[_typeData.idx][carnivalData.idx].complete != 0) ? true : false);
			flag2 = ((carnivalList.carnivalProcessList[_typeData.idx][carnivalData.idx].receive != 0) ? true : false);
		}
		UISetter.SetActive(completeRoot, flag && flag2);
		UISetter.SetActive(enableBtn, flag && !flag2);
		UISetter.SetActive(buyBtn, !flag);
		SetTime();
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
		Init(typeData);
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
	}
}
