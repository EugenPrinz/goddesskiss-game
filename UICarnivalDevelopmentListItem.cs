using Shared.Regulation;
using UnityEngine;

public class UICarnivalDevelopmentListItem : UIItemBase
{
	public UILabel title;

	public UIGoods reward;

	public GameObject enableBtn;

	public GameObject completeRoot;

	public GameObject disableRoot;

	private readonly string moveBtnIdPrefix = "Move-";

	private readonly string enableBtnIdPrefix = "Enable-";

	private readonly string completeRootIdPrefix = "Complete-";

	private readonly string disableRootIdPrefix = "Disable-";

	public void Set(string cTidx, CarnivalDataRow row)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		CarnivalTypeDataRow carnivalTypeDataRow = regulation.carnivalTypeDtbl[cTidx];
		Protocols.CarnivalList carnivalList = RemoteObjectManager.instance.localUser.carnivalList;
		RewardDataRow rewardDataRow = RemoteObjectManager.instance.regulation.rewardDtbl.Find((RewardDataRow item) => item.category == ERewardCategory.Carnival && item.type == int.Parse(row.idx));
		reward.Set(rewardDataRow);
		if (carnivalTypeDataRow.Type != ECarnivalType.StageClearPackage)
		{
			UISetter.SetLabel(title, Localization.Format(row.explanation, row.userLevel, rewardDataRow.minCount));
		}
		else
		{
			WorldMapStageDataRow worldMapStageDataRow = regulation.worldMapStageDtbl[row.checkCount.ToString()];
			UISetter.SetLabel(title, Localization.Format(row.explanation, worldMapStageDataRow.worldMapId, rewardDataRow.minCount));
		}
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		if (carnivalList.carnivalProcessList != null && carnivalList.carnivalProcessList.ContainsKey(cTidx.ToString()) && carnivalList.carnivalProcessList[cTidx].ContainsKey(row.idx))
		{
			num = carnivalList.carnivalProcessList[cTidx][row.idx].count;
			flag = ((carnivalList.carnivalProcessList[cTidx][row.idx].complete != 0) ? true : false);
			flag2 = ((carnivalList.carnivalProcessList[cTidx][row.idx].receive != 0) ? true : false);
		}
		UISetter.SetGameObjectName(enableBtn, $"{enableBtnIdPrefix}{row.idx}");
		UISetter.SetActive(disableRoot, !flag);
		UISetter.SetActive(completeRoot, flag && flag2);
		UISetter.SetActive(enableBtn, flag && !flag2);
	}
}
