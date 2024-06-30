using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICarnivalBulletRewardPopUp : UIPopup
{
	public UIDefaultListView rewardListView;

	public GameObject receiveBtn;

	public GameObject receiveLabel;

	public UITimer remainTimer;

	public GameObject remainRoot;

	private CarnivalTypeDataRow typeData;

	private CarnivalDataRow carnivalData;

	private Protocols.CarnivalList.ProcessData process;

	private readonly string itemPrefix = "Item-";

	public void Init(CarnivalTypeDataRow _typeData)
	{
		typeData = _typeData;
		UISetter.SetLabel(title, Localization.Get(typeData.name));
		List<CarnivalDataRow> list = base.regulation.carnivalDtbl.FindAll((CarnivalDataRow row) => row.cTidx == typeData.idx);
		double num = -1.0;
		for (int i = 0; i < list.Count; i++)
		{
			CarnivalDataRow carnivalDataRow = list[i];
			Protocols.CarnivalList.ProcessData processData = base.localUser.carnivalList.carnivalProcessList[typeData.idx][carnivalDataRow.idx];
			if (processData.receive == 0 && (num == -1.0 || num > processData.startTimeData.GetRemain()))
			{
				num = processData.startTimeData.GetRemain();
				carnivalData = carnivalDataRow;
				process = processData;
			}
		}
		SetBtn(process);
	}

	private void SetBtn(Protocols.CarnivalList.ProcessData process)
	{
		UISetter.SetActive(remainRoot, process.startTimeData.GetRemain() > 0.0);
		UISetter.SetActive(receiveLabel, !remainRoot.activeSelf);
		if (process.startTimeData.GetRemain() > 0.0)
		{
			remainTimer.Set(process.startTimeData);
		}
		else
		{
			remainTimer.Set(process.endTimeData);
		}
		if (!(process.startTimeData.GetRemain() > 0.0) && !(process.endTimeData.GetRemain() > 0.0))
		{
			return;
		}
		remainTimer.RegisterOnFinished(delegate
		{
			if (process.endTimeData.GetRemain() <= 0.0)
			{
				base.network.RequestGetCarnivalList(typeData.categoryType);
			}
			else
			{
				UIManager.instance.RefreshOpenedUI();
			}
		});
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "ReceiveBtn" && process.startTimeData.GetRemain() <= 0.0)
		{
			base.network.RequestCarnivalComplete(int.Parse(typeData.idx), carnivalData.idx);
		}
	}
}
