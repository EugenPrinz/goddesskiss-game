using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICarnivalChargePopUp : UIPopup
{
	public UITimer remainTimer;

	public UIDefaultListView giftListView;

	public UIDefaultListView rewardListView;

	public UIGoods reward;

	public GameObject enableBtn;

	public GameObject disableBtn;

	public GameObject completeRoot;

	public UILabel process;

	public UILabel description;

	private readonly string carnivalItemIdPrefix = "carnival-";

	private readonly string moveBtnIdPrefix = "Move-";

	private readonly string enableBtnIdPrefix = "Enable-";

	private readonly string completeRootIdPrefix = "Complete-";

	private readonly string disableRootIdPrefix = "Disable-";

	private CarnivalTypeDataRow typeData;

	private CarnivalDataRow processData;

	private bool state;

	public void Init(CarnivalTypeDataRow _typeData)
	{
		typeData = _typeData;
		state = true;
		UISetter.SetLabel(title, Localization.Get(typeData.name));
		List<CarnivalDataRow> list = base.regulation.carnivalDtbl.FindAll((CarnivalDataRow row) => row.cTidx == typeData.idx && row.checkType != 11);
		CarnivalDataRow finalCarnival = base.regulation.carnivalDtbl.Find((CarnivalDataRow row) => row.cTidx == typeData.idx && row.checkType == 11);
		RewardDataRow rewardDataRow = base.regulation.rewardDtbl.Find((RewardDataRow row) => row.category == ERewardCategory.Carnival && row.type == int.Parse(finalCarnival.idx));
		reward.Set(rewardDataRow);
		bool flag = SetData();
		UISetter.SetActive(process, processData.checkType != 11);
		if (processData.checkType != 11)
		{
			int num = ((!flag) ? (list.IndexOf(processData) - 1) : list.IndexOf(processData));
			SetRewardData(processData.idx);
			giftListView.InitGiftList(list, num, carnivalItemIdPrefix);
			giftListView.SetSelection(processData.idx, selected: true);
			giftListView.ResetPosition();
		}
		else
		{
			SetRewardData(list[list.Count - 1].idx);
			giftListView.InitGiftList(list, list.Count, carnivalItemIdPrefix);
			giftListView.SetSelection(list[list.Count - 1].idx, selected: true);
			giftListView.ResetPosition();
		}
		SetTime();
	}

	private bool SetData()
	{
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
		Protocols.CarnivalList carnivalList = roLocalUser.carnivalList;
		foreach (KeyValuePair<string, Protocols.CarnivalList.ProcessData> data in carnivalList.carnivalProcessList[typeData.idx])
		{
			CarnivalDataRow carnivalDataRow = base.regulation.carnivalDtbl.Find((CarnivalDataRow row) => row.idx == data.Key);
			if (data.Value.receive == 0 || carnivalList.carnivalProcessList[typeData.idx][data.Key].able == 1 || carnivalDataRow.checkType == 11)
			{
				processData = carnivalDataRow;
				num = carnivalList.carnivalProcessList[typeData.idx][data.Key].count;
				flag = ((carnivalList.carnivalProcessList[typeData.idx][data.Key].complete != 0) ? true : false);
				flag2 = ((carnivalList.carnivalProcessList[typeData.idx][data.Key].receive != 0) ? true : false);
				break;
			}
		}
		UISetter.SetGameObjectName(enableBtn, $"{enableBtnIdPrefix}{processData.idx}");
		UISetter.SetGameObjectName(disableBtn, $"{disableRootIdPrefix}{processData.idx}");
		UISetter.SetActive(disableBtn, !flag);
		UISetter.SetActive(completeRoot, flag2);
		UISetter.SetActive(enableBtn, flag && !flag2);
		UISetter.SetLabel(description, Localization.Format(processData.explanation, processData.checkCount));
		UISetter.SetLabel(process, $"{num}/{processData.checkCount}");
		return flag2;
	}

	private void SetRewardData(string idx)
	{
		List<RewardDataRow> list = base.regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == ERewardCategory.Carnival && row.type == int.Parse(idx));
		rewardListView.InitRewardList(list);
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
		else if (text.StartsWith(disableRootIdPrefix))
		{
			base.uiWorld.mainCommand.OpenDiamonShop();
		}
	}
}
