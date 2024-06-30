using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIRaidScoreRewardPopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public UILabel description;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIDefaultListView rewardList;

	public GameObject GetRewardAllBtn;

	private void Start()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			SetAutoDestroy(autoDestory: true);
			Init();
			AnimBG.Reset();
			AnimBlock.Reset();
			OpenPopup();
		}
	}

	public void Init()
	{
		UISetter.SetButtonGray(GetRewardAllBtn, base.localUser.raidRank != base.localUser.raidRewardPoint);
		UISetter.SetLabel(description, Localization.Format("17076", base.localUser.raidScore, 12));
		Dictionary<int, List<RewardDataRow>> list = new Dictionary<int, List<RewardDataRow>>();
		base.regulation.rankingDtbl.ForEach(delegate(RankingDataRow row)
		{
			if (row.type == ERankingContentsType.RaidScore)
			{
				List<RewardDataRow> value = base.regulation.rewardDtbl.FindAll((RewardDataRow rRow) => rRow.category == ERewardCategory.Ranking && rRow.type == 4 && rRow.typeIndex == row.r_idx);
				list.Add(row.r_idx, value);
			}
		});
		rewardList.InitScoreRewardList(list, duel: false, "Score-");
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		Init();
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "Close")
		{
			if (!bBackKeyEnable)
			{
				ClosePopup();
			}
		}
		else if (rewardList.Contains(text))
		{
			SoundManager.PlaySFX("BTN_Point_001");
			UIScoreRewardItem uIScoreRewardItem = rewardList.FindItem(text) as UIScoreRewardItem;
			if (uIScoreRewardItem.state == ERewardState.Receptible)
			{
				if (!base.localUser.GetItemCheckList(uIScoreRewardItem.rewardList))
				{
					ReceiveRaidScoreRewardNotice(text);
				}
				else
				{
					ReceiveRaidScoreReward(text);
				}
			}
			else if (uIScoreRewardItem.state == ERewardState.NonReceptible)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("17088"));
			}
			return;
		}
		if (text == "GetRewardAllBtn")
		{
			if (GetRewardAllBtn.GetComponent<UIButton>().isGray)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6621"));
			}
			else
			{
				ReceiveRaidScoreRewardAll();
			}
		}
		else
		{
			base.OnClick(sender);
		}
	}

	private void ReceiveRaidScoreReward(string key)
	{
		string pureId = rewardList.GetPureId(key);
		base.network.RequestGetRankingReward(4, int.Parse(pureId));
	}

	private void ReceiveRaidScoreRewardAll()
	{
		if (!GetAllItemCheckList())
		{
			ReceiveRaidScoreRewardNotice(base.localUser.raidRank.ToString());
		}
		else
		{
			base.network.RequestGetRankingReward(4, base.localUser.raidRank);
		}
	}

	private void ReceiveRaidScoreRewardNotice(string key)
	{
		UISimplePopup.CreateBool(localization: true, "1303", "5476", "5477", "1304", "1305").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				ReceiveRaidScoreReward(key);
			}
		};
	}

	private bool GetAllItemCheckList()
	{
		List<string> idList = new List<string>();
		base.regulation.rankingDtbl.ForEach(delegate(RankingDataRow row)
		{
			if (row.type == ERankingContentsType.RaidScore && base.localUser.raidRewardPoint < row.r_idx && base.localUser.raidRank >= row.r_idx)
			{
				idList.Add(row.r_idx.ToString());
			}
		});
		List<RewardDataRow> list = new List<RewardDataRow>();
		for (int i = 0; i < idList.Count; i++)
		{
			string id = idList[i];
			UIScoreRewardItem uIScoreRewardItem = rewardList.FindItem(id) as UIScoreRewardItem;
			for (int j = 0; j < uIScoreRewardItem.rewardList.Count; j++)
			{
				RewardAdd(list, uIScoreRewardItem.rewardList[j]);
			}
		}
		return base.localUser.GetItemCheckList(list);
	}

	private void RewardAdd(List<RewardDataRow> list, RewardDataRow data)
	{
		bool flag = false;
		for (int i = 0; i < list.Count; i++)
		{
			RewardDataRow rewardDataRow = list[i];
			if (rewardDataRow.rewardType == data.rewardType && rewardDataRow.rewardIdx == data.rewardIdx)
			{
				rewardDataRow.AddCount(data.maxCount, data.minCount);
				flag = true;
			}
		}
		if (!flag)
		{
			RewardDataRow rewardDataRow2 = new RewardDataRow();
			rewardDataRow2.Clone(data);
			list.Add(rewardDataRow2);
		}
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		if (!bBackKeyEnable)
		{
			bBackKeyEnable = true;
			HidePopup();
		}
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
		base.Close();
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
