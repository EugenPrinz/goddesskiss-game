using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIScoreRewardPopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public UILabel description;

	public UILabel score;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIDefaultListView rewardList;

	private void Start()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			SetAutoDestroy(autoDestory: true);
			AnimBG.Reset();
			AnimBlock.Reset();
			OpenPopup();
		}
	}

	public void Init()
	{
		UISetter.SetLabel(description, Localization.Format("17076", base.localUser.duelPoint, 12));
		UISetter.SetLabel(score, Localization.Format("17077", base.regulation.defineDtbl["ARENA_POINT_WIN"].value, base.regulation.defineDtbl["ARENA_POINT_LOSE"].value));
		Dictionary<int, List<RewardDataRow>> list = new Dictionary<int, List<RewardDataRow>>();
		base.regulation.rewardDtbl.ForEach(delegate(RewardDataRow row)
		{
			if (row.category == ERewardCategory.Score)
			{
				if (!list.ContainsKey(row.type))
				{
					list.Add(row.type, new List<RewardDataRow>());
				}
				list[row.type].Add(row);
			}
		});
		rewardList.InitScoreRewardList(list, duel: true, "Score-");
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
					ReceiveDuelPointRewardNotice(text);
				}
				else
				{
					ReceiveDuelPointReward(text);
				}
			}
			else if (uIScoreRewardItem.state == ERewardState.NonReceptible)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("17088"));
			}
			return;
		}
		base.OnClick(sender);
	}

	private void ReceiveDuelPointReward(string key)
	{
		string pureId = rewardList.GetPureId(key);
		base.network.RequestReceiveDuelPointReward(pureId);
	}

	private void ReceiveDuelPointRewardNotice(string key)
	{
		UISimplePopup.CreateBool(localization: true, "1303", "5476", "5477", "1304", "1305").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				ReceiveDuelPointReward(key);
			}
		};
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
