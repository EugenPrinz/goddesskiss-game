using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICarnivalSelectRewardPopUp : UIPopup
{
	public UIDefaultListView rewardListView;

	public UIGoods rewardItem;

	public GameObject receiveBtn;

	public GameObject selectBtn;

	public GameObject selectCompleteRoot;

	public UITimer remainTimer;

	private CarnivalTypeDataRow typeData;

	private readonly string itemPrefix = "Item-";

	private int selectId;

	public void Init(CarnivalTypeDataRow _typeData)
	{
		selectId = 0;
		typeData = _typeData;
		UISetter.SetLabel(title, Localization.Get(typeData.name));
		CarnivalDataRow carnivalData = base.regulation.carnivalDtbl.Find((CarnivalDataRow row) => row.cTidx == typeData.idx);
		List<RewardDataRow> list = RemoteObjectManager.instance.regulation.rewardDtbl.FindAll((RewardDataRow item) => item.category == ERewardCategory.Carnival && item.type == int.Parse(carnivalData.idx));
		rewardListView.InitCarivalSelectRewardList(list, itemPrefix);
		Protocols.CarnivalList.ProcessData processData = null;
		if (base.localUser.carnivalList.carnivalProcessList.ContainsKey(typeData.idx) && base.localUser.carnivalList.carnivalProcessList[typeData.idx].ContainsKey(carnivalData.idx))
		{
			processData = base.localUser.carnivalList.carnivalProcessList[typeData.idx][carnivalData.idx];
		}
		UISetter.SetActive(rewardListView, processData.count == 0);
		UISetter.SetActive(rewardItem, processData.count != 0);
		if (processData.count != 0)
		{
			rewardItem.Set(list[processData.count - 1]);
		}
		SetBtn(processData);
		SetTime(processData);
	}

	private void SetBtn(Protocols.CarnivalList.ProcessData process)
	{
		selectId = Mathf.Max(1, process.count);
		rewardListView.SetSelection(selectId.ToString(), selected: true);
		UISetter.SetActive(selectBtn, process.count == 0 && process.startTimeData.GetRemain() <= 0.0);
		UISetter.SetActive(selectCompleteRoot, process.count != 0 && process.startTimeData.GetRemain() > 0.0);
		UISetter.SetActive(receiveBtn, process.count != 0 && process.startTimeData.GetRemain() <= 0.0);
	}

	private void SetTime(Protocols.CarnivalList.ProcessData process)
	{
		if (process.count == 0 || (process.count > 0 && process.startTimeData.GetRemain() > 0.0))
		{
			remainTimer.SetLabelFormat(Localization.Get("11200001"), string.Empty);
		}
		else
		{
			remainTimer.SetLabelFormat(Localization.Get("11200002"), string.Empty);
		}
		remainTimer.Set(process.remainTimeData);
		remainTimer.RegisterOnFinished(delegate
		{
			base.network.RequestGetCarnivalList(typeData.categoryType);
		});
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (rewardListView.Contains(text))
		{
			if (!selectBtn.GetComponent<UIButton>().isGray)
			{
				selectId = int.Parse(rewardListView.GetPureId(text));
				rewardListView.SetSelection(selectId.ToString(), selected: true);
			}
		}
		else if (text == "SelectBtn")
		{
			if (!selectBtn.GetComponent<UIButton>().isGray)
			{
				CreateSelectRewardPopup();
			}
		}
		else if (text == "ReceiveBtn")
		{
			CarnivalDataRow carnivalDataRow = base.regulation.carnivalDtbl.Find((CarnivalDataRow row) => row.cTidx == typeData.idx);
			base.network.RequestCarnivalComplete(int.Parse(typeData.idx), carnivalDataRow.idx);
		}
	}

	private void CreateSelectRewardPopup()
	{
		UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: true, "1303", "11200004", string.Empty, "1001", "1000");
		uISimplePopup.onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				CarnivalDataRow carnivalDataRow = base.regulation.carnivalDtbl.Find((CarnivalDataRow row) => row.cTidx == typeData.idx);
				base.network.RequestCarnivalSelectItem(int.Parse(typeData.idx), int.Parse(carnivalDataRow.idx), selectId);
			}
		};
	}
}
