using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMachineResultListItem : UIItemBase
{
	public UISprite bg;

	public UIDefaultListView rewardListView;

	public UILabel title;

	public GameObject emptyRoot;

	public GameObject completeAnimation;

	[HideInInspector]
	public bool playAnimation;

	private bool bonus;

	public void Set(List<Protocols.RewardInfo.RewardData> list, bool _bonus = false)
	{
		bonus = _bonus;
		int num = Mathf.Max(1, (int)Mathf.Ceil((float)list.Count / (float)rewardListView.grid.maxPerLine));
		bg.height += 110 * (num - 1);
		if (bonus)
		{
			UISetter.SetLabel(title, Localization.Get("1334"));
		}
		else
		{
			UISetter.SetLabel(title, Localization.Format("1285", base.gameObject.name));
		}
		UISetter.SetActive(emptyRoot, list.Count == 0);
		rewardListView.InitTimeMachineRewardItem(list);
	}

	public void Set(List<Protocols.RewardInfo.RewardData> list, ETimeMachineType type)
	{
		bonus = false;
		int num = Mathf.Max(1, (int)Mathf.Ceil((float)list.Count / (float)rewardListView.grid.maxPerLine));
		bg.height += 110 * (num - 1);
		switch (type)
		{
		case ETimeMachineType.Sweep:
			UISetter.SetLabel(title, Localization.Format("1285", base.gameObject.name));
			break;
		case ETimeMachineType.AdvanceParty:
			UISetter.SetLabel(title, Localization.Format("80019", base.gameObject.name));
			break;
		}
		UISetter.SetActive(emptyRoot, list.Count == 0);
		rewardListView.InitTimeMachineRewardItem(list);
	}

	public IEnumerator _PlayRewardAnimation()
	{
		if (rewardListView.itemList != null)
		{
			for (int idx = 0; idx < rewardListView.itemList.Count; idx++)
			{
				UIItemBase reward = rewardListView.itemList[idx];
				UISetter.SetActive(reward.gameObject, active: true);
				yield return null;
			}
		}
		if (bonus)
		{
			yield return new WaitForSeconds(0.1f);
			UISetter.SetActive(completeAnimation, active: true);
		}
	}

	public void Skip()
	{
		for (int i = 0; i < rewardListView.itemList.Count; i++)
		{
			UIItemBase uIItemBase = rewardListView.itemList[i];
			UISetter.SetActive(uIItemBase.gameObject, active: true);
		}
		if (bonus)
		{
			UISetter.SetActive(completeAnimation, active: true);
		}
	}
}
