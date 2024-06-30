using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITimeMachinePopup : UISimplePopup
{
	public UIDefaultListView resultListView;

	private TimeMachineResultListItem curruntItem;

	private void Start()
	{
		Open();
		SetAutoDestroy(autoDestory: true);
	}

	public void Init(List<List<Protocols.RewardInfo.RewardData>> list, ETimeMachineType type)
	{
		if (type == ETimeMachineType.Stage || type == ETimeMachineType.Sweep)
		{
			UISetter.SetLabel(title, Localization.Get("1284"));
		}
		else
		{
			UISetter.SetLabel(title, Localization.Get("80018"));
		}
		resultListView.InitTimeMachineResult(list, type);
		StartCoroutine("_PlayRewardAnimation");
	}

	protected override void OnDisable()
	{
		base.localUser.UserLevelUpCheck();
		base.OnDisable();
	}

	public IEnumerator _PlayRewardAnimation()
	{
		if (resultListView.itemList == null)
		{
			yield break;
		}
		for (int idx = 0; idx < resultListView.itemList.Count; idx++)
		{
			resultListView.scrollView.MoveRelative(-resultListView.scrollView.transform.localPosition);
			TimeMachineResultListItem reward = (curruntItem = resultListView.itemList[idx] as TimeMachineResultListItem);
			UISetter.SetActive(reward, active: true);
			if (idx >= 2)
			{
				Vector3 localPosition = reward.transform.localPosition;
				localPosition.y += reward.bg.height / 2 + reward.bg.height;
				resultListView.scrollView.MoveRelative(-localPosition);
			}
			yield return StartCoroutine(reward._PlayRewardAnimation());
			yield return new WaitForSeconds(0.3f);
		}
		if (base.localUser.statistics.vipShopisFloating)
		{
			RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.VipShop);
			NetworkAnimation.Instance.CreateFloatingText_OnlyUIToast(Localization.Get("22007"), roBuilding.reg.resourceId);
			base.localUser.statistics.vipShopisFloating = false;
		}
	}

	public void Skip()
	{
		StopCoroutine("_PlayRewardAnimation");
		for (int i = 0; i < resultListView.itemList.Count; i++)
		{
			TimeMachineResultListItem timeMachineResultListItem = resultListView.itemList[i] as TimeMachineResultListItem;
			UISetter.SetActive(timeMachineResultListItem, active: true);
			timeMachineResultListItem.Skip();
			if (i >= 2 && i == resultListView.itemList.Count - 1)
			{
				resultListView.scrollView.MoveRelative(-resultListView.scrollView.transform.localPosition);
				Vector3 localPosition = timeMachineResultListItem.transform.localPosition;
				localPosition.y += timeMachineResultListItem.bg.height / 2 + timeMachineResultListItem.bg.height;
				resultListView.scrollView.MoveRelative(-localPosition);
			}
		}
	}
}
