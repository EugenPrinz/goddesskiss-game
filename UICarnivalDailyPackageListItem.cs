using Shared.Regulation;
using UnityEngine;

public class UICarnivalDailyPackageListItem : UIItemBase
{
	public UILabel title;

	public UILabel process;

	public UILabel description;

	public UISprite bg;

	public GameObject enableBtn;

	public GameObject buyBtn;

	public GameObject rewardRoot;

	public GameObject completeRoot;

	public UISprite costIcon;

	public UILabel cost;

	private readonly string enableBtnIdPrefix = "Enable-";

	private readonly string buyBtnIdPrefix = "Buy-";

	private readonly string rewardListBtnIdPrefix = "RewardList-";

	private string cTidx;

	public void Set(string cTidx, CarnivalDataRow row)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		Protocols.CarnivalList carnivalList = localUser.carnivalList;
		CarnivalTypeDataRow carnivalTypeDataRow = regulation.carnivalTypeDtbl[cTidx];
		InAppProductDataRow inAppProductDataRow = regulation.inAppProductDtbl.Find((InAppProductDataRow item) => item.iapidx == row.checkCount);
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		if (carnivalList.carnivalProcessList != null && carnivalList.carnivalProcessList.ContainsKey(cTidx) && carnivalList.carnivalProcessList[cTidx].ContainsKey(row.idx))
		{
			num = carnivalList.carnivalProcessList[cTidx][row.idx].count;
			flag = ((carnivalList.carnivalProcessList[cTidx][row.idx].complete != 0) ? true : false);
			flag2 = ((carnivalList.carnivalProcessList[cTidx][row.idx].receive != 0) ? true : false);
		}
		UISetter.SetGameObjectName(enableBtn, $"{enableBtnIdPrefix}{row.idx}");
		UISetter.SetGameObjectName(rewardRoot, $"{rewardListBtnIdPrefix}{row.idx}");
		UISetter.SetActive(buyBtn, num == 0 && !flag && !flag2);
		UISetter.SetActive(completeRoot, flag && flag2);
		UISetter.SetActive(enableBtn, num > 0 && flag && !flag2);
		UISetter.SetActive(process, num > 0);
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
		if (inAppProductDataRow != null)
		{
			UISetter.SetGameObjectName(buyBtn, $"{buyBtnIdPrefix}{inAppProductDataRow.googlePlayID}");
			UISetter.SetLabel(cost, int.Parse(inAppProductDataRow.wonPrice).ToString("N0"));
			UISetter.SetLabel(title, Localization.Get(inAppProductDataRow.stringidx));
			UISetter.SetLabel(description, Localization.Get(inAppProductDataRow.explanation));
		}
		UISetter.SetLabel(process, $"{num}/{carnivalTypeDataRow.etc}");
	}
}
