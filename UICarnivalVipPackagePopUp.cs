using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICarnivalVipPackagePopUp : UIPopup
{
	public UITimer remainTimer;

	public UIDefaultListView vipListView;

	public UIDefaultListView rewardListView;

	public UILabel description;

	public UILabel cost;

	public GameObject disableBtn;

	public GameObject enableBtn;

	public GameObject completeRoot;

	private readonly string enableBtnIdPrefix = "Enable-";

	private readonly string completeRootIdPrefix = "Complete-";

	private readonly string disableRootIdPrefix = "Disable-";

	private readonly string vipBtnIdPrefix = "Vip-";

	private CarnivalTypeDataRow typeData;

	private bool state;

	public void Init(CarnivalTypeDataRow _typeData, int selectIdx = 0)
	{
		typeData = _typeData;
		state = true;
		UISetter.SetLabel(title, Localization.Get(typeData.name));
		List<CarnivalDataRow> list = base.regulation.carnivalDtbl.FindAll((CarnivalDataRow row) => row.cTidx == typeData.idx);
		vipListView.InitVipList(list, vipBtnIdPrefix);
		vipListView.SetSelection(list[selectIdx].idx, selected: true);
		SetRewardData(list[selectIdx].idx);
		SetTime();
	}

	private void SetButton(CarnivalDataRow row)
	{
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
		Protocols.CarnivalList carnivalList = roLocalUser.carnivalList;
		if (carnivalList.carnivalProcessList != null && carnivalList.carnivalProcessList.ContainsKey(typeData.idx) && carnivalList.carnivalProcessList[typeData.idx].ContainsKey(row.idx))
		{
			num = carnivalList.carnivalProcessList[typeData.idx][row.idx].count;
			flag = ((carnivalList.carnivalProcessList[typeData.idx][row.idx].complete != 0) ? true : false);
			flag2 = ((carnivalList.carnivalProcessList[typeData.idx][row.idx].receive != 0) ? true : false);
		}
		UISetter.SetGameObjectName(enableBtn, $"{enableBtnIdPrefix}{row.idx}");
		UISetter.SetGameObjectName(disableBtn, $"{disableRootIdPrefix}{row.idx}");
		UISetter.SetActive(disableBtn, roLocalUser.vipLevel < row.userVip);
		UISetter.SetActive(completeRoot, flag2);
		UISetter.SetActive(enableBtn, roLocalUser.vipLevel >= row.userVip && !flag2);
	}

	private void SetRewardData(string idx)
	{
		CarnivalDataRow carnivalDataRow = base.regulation.carnivalDtbl.Find((CarnivalDataRow row) => row.idx == idx);
		List<RewardDataRow> list = base.regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == ERewardCategory.Carnival && row.type == int.Parse(idx));
		UISetter.SetLabel(description, Localization.Get(carnivalDataRow.explanation));
		UISetter.SetLabel(cost, carnivalDataRow.checkCount);
		rewardListView.InitRewardList(list);
		SetButton(carnivalDataRow);
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
		int selectIdx = vipListView.itemList.IndexOf(vipListView.lastSelectedItem);
		Init(typeData, selectIdx);
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text.StartsWith(vipBtnIdPrefix))
		{
			string text2 = text.Substring(text.IndexOf("-") + 1);
			vipListView.SetSelection(text2, selected: true);
			SetRewardData(text2);
		}
		else if (text.StartsWith(enableBtnIdPrefix))
		{
			string idx = text.Substring(text.IndexOf("-") + 1);
			CreateEnablePopUp(idx);
		}
		else if (text.StartsWith(disableRootIdPrefix))
		{
			string idx2 = text.Substring(text.IndexOf("-") + 1);
			CreateDisablePopUp(idx2);
		}
	}

	private void CreateEnablePopUp(string idx)
	{
		CarnivalDataRow carnivalData = base.regulation.carnivalDtbl.Find((CarnivalDataRow row) => row.idx == idx);
		UISimplePopup.CreateBool(localization: false, Localization.Get("23001"), Localization.Format("23002", carnivalData.checkCount, carnivalData.userVip), string.Empty, Localization.Get("1002"), Localization.Get("1000")).onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (base.localUser.cash < carnivalData.checkCount)
				{
					OpenPopup_GotoDiamondShop();
				}
				else
				{
					base.network.RequestCarnivalBuyPackage(int.Parse(typeData.idx), idx);
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	private void CreateDisablePopUp(string idx)
	{
		CarnivalDataRow carnivalDataRow = base.regulation.carnivalDtbl[idx];
		UISimplePopup.CreateBool(localization: false, Localization.Get("23003"), Localization.Format("23004", carnivalDataRow.checkCount, carnivalDataRow.userVip), string.Empty, Localization.Get("1003"), Localization.Get("1000")).onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				UIManager.instance.world.mainCommand.OpenDiamonShop();
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	private void OpenPopup_GotoDiamondShop()
	{
		UISimplePopup.CreateBool(localization: true, "5735", "5736", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				base.uiWorld.mainCommand.OpenDiamonShop();
			}
		};
	}
}
