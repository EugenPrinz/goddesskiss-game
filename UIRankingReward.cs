using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIRankingReward : UIPopup
{
	public UIDefaultListView rewardListView;

	public UIDefaultListView seasonRewardListView;

	public UIFlipSwitch achieveTab;

	public UIFlipSwitch weeklyTab;

	public UIFlipSwitch seasonTab;

	public GameObject seasonRoot;

	public GameObject basicRoot;

	private ERankingContentsType contentsType;

	public GameObject GetRewardAllBtn;

	public UITexture texture;

	public GameObject badge;

	private readonly string imagePrefix = "Texture/UI/";

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (rewardListView.Contains(text))
		{
			SoundManager.PlaySFX("BTN_Point_001");
			string id = text.Substring(text.IndexOf("-") + 1);
			UIRankingRewardItem uIRankingRewardItem = rewardListView.FindItem(id) as UIRankingRewardItem;
			if (uIRankingRewardItem.state == ERewardState.Receptible)
			{
				if (!base.localUser.GetItemCheckList(uIRankingRewardItem.rewardList))
				{
					ReceiveAchieveRewardNotice(text);
				}
				else
				{
					ReceiveAchieveReward(text);
				}
			}
			else if (uIRankingRewardItem.state == ERewardState.NonReceptible)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("17088"));
			}
			return;
		}
		switch (text)
		{
		case "GetRewardAllBtn":
			if (GetRewardAllBtn.GetComponent<UIButton>().isGray)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("6621"));
			}
			else
			{
				ReceiveAchieveRewardAll();
			}
			break;
		case "AchieveTab":
			if (contentsType == ERankingContentsType.WorldDuel || contentsType == ERankingContentsType.WorldDuelSeason)
			{
				contentsType = ERankingContentsType.WorldDuelGrade;
			}
			else
			{
				contentsType = ERankingContentsType.ChallengeGrade;
			}
			SetList();
			rewardListView.ResetPosition();
			break;
		case "WeeklyTab":
			if (contentsType == ERankingContentsType.WorldDuelSeason || contentsType == ERankingContentsType.WorldDuelGrade)
			{
				contentsType = ERankingContentsType.WorldDuel;
			}
			else
			{
				contentsType = ERankingContentsType.Challenge;
			}
			SetList();
			rewardListView.ResetPosition();
			break;
		case "SeasonTab":
			contentsType = ERankingContentsType.WorldDuelSeason;
			SetList();
			rewardListView.ResetPosition();
			break;
		default:
			base.OnClick(sender);
			break;
		}
	}

	public override void OnRefresh()
	{
		SetList();
	}

	private void SetList()
	{
		if (contentsType == ERankingContentsType.ChallengeGrade)
		{
			UISetter.SetActive(badge, base.localUser.winRank != base.localUser.winRankIdx);
			UISetter.SetButtonGray(GetRewardAllBtn, base.localUser.winRank != base.localUser.winRankIdx);
		}
		else if (contentsType == ERankingContentsType.WorldDuelGrade)
		{
			UISetter.SetActive(badge, base.localUser.worldWinRank != base.localUser.worldWinRankIdx);
			UISetter.SetButtonGray(GetRewardAllBtn, base.localUser.worldWinRank != base.localUser.worldWinRankIdx);
		}
		UISetter.SetActive(GetRewardAllBtn, contentsType == ERankingContentsType.ChallengeGrade || contentsType == ERankingContentsType.WorldDuelGrade);
		ERankingContentsType eRankingContentsType = ERankingContentsType.Challenge;
		eRankingContentsType = ((contentsType == ERankingContentsType.ChallengeGrade) ? ERankingContentsType.Challenge : contentsType);
		List<RankingDataRow> list = base.regulation.FindDuelRankingList(eRankingContentsType);
		UISetter.SetActive(basicRoot, eRankingContentsType != ERankingContentsType.WorldDuelSeason);
		UISetter.SetActive(seasonRoot, eRankingContentsType == ERankingContentsType.WorldDuelSeason);
		if (eRankingContentsType != ERankingContentsType.WorldDuelSeason)
		{
			rewardListView.InitRankingReward(list, contentsType, "Achieve-");
			return;
		}
		StartCoroutine(SetImage("ps-thumbnail01"));
		seasonRewardListView.InitRankingReward(list, contentsType, "Achieve-");
	}

	private void InitTab()
	{
		if (contentsType == ERankingContentsType.ChallengeGrade || contentsType == ERankingContentsType.Challenge)
		{
			UISetter.SetActive(achieveTab, active: true);
			UISetter.SetActive(weeklyTab, active: true);
			UISetter.SetActive(seasonTab, active: false);
			achieveTab.Set(SwitchStatus.ON);
			weeklyTab.Set(SwitchStatus.OFF);
			seasonTab.Set(SwitchStatus.OFF);
		}
		else if (contentsType == ERankingContentsType.WorldDuelGrade || contentsType == ERankingContentsType.WorldDuel || contentsType == ERankingContentsType.WorldDuelSeason)
		{
			UISetter.SetActive(achieveTab, active: true);
			UISetter.SetActive(weeklyTab, active: true);
			UISetter.SetActive(seasonTab, active: true);
			achieveTab.Set(SwitchStatus.ON);
			weeklyTab.Set(SwitchStatus.OFF);
			seasonTab.Set(SwitchStatus.OFF);
		}
		else
		{
			UISetter.SetActive(achieveTab, active: false);
			UISetter.SetActive(weeklyTab, active: true);
			UISetter.SetActive(seasonTab, active: false);
			achieveTab.Set(SwitchStatus.OFF);
			weeklyTab.Set(SwitchStatus.ON);
			seasonTab.Set(SwitchStatus.OFF);
		}
	}

	public void Init(ERankingContentsType contentsType)
	{
		this.contentsType = contentsType;
		InitTab();
		SetList();
	}

	private void ReceiveAchieveRewardNotice(string key)
	{
		UISimplePopup.CreateBool(localization: true, "1303", "5476", "5477", "1304", "1305").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				ReceiveAchieveReward(key);
			}
		};
	}

	private void ReceiveAchieveReward(string key)
	{
		string pureId = rewardListView.GetPureId(key);
		base.network.RequestGetRankingReward((int)contentsType, int.Parse(pureId));
	}

	private void ReceiveAchieveRewardAll()
	{
		if (!GetAllItemCheckList())
		{
			ReceiveAchieveRewardNotice(base.localUser.winRank.ToString());
		}
		else if (contentsType == ERankingContentsType.ChallengeGrade)
		{
			base.network.RequestGetRankingReward((int)contentsType, base.localUser.winRank);
		}
		else if (contentsType == ERankingContentsType.WorldDuelGrade)
		{
			base.network.RequestGetRankingReward((int)contentsType, base.localUser.worldWinRank);
		}
	}

	private bool GetAllItemCheckList()
	{
		List<string> idList = new List<string>();
		base.regulation.rankingDtbl.ForEach(delegate(RankingDataRow row)
		{
			if (contentsType == ERankingContentsType.ChallengeGrade)
			{
				if (row.type == ERankingContentsType.Challenge)
				{
					if (base.localUser.winRankIdx > 0)
					{
						if (base.localUser.winRankIdx > row.r_idx && base.localUser.winRank <= row.r_idx)
						{
							idList.Add(row.r_idx.ToString());
						}
					}
					else if (base.localUser.winRank <= row.r_idx)
					{
						idList.Add(row.r_idx.ToString());
					}
				}
			}
			else if (contentsType == ERankingContentsType.WorldDuelGrade && row.type == ERankingContentsType.WorldDuelGrade)
			{
				if (base.localUser.worldWinRankIdx > 0)
				{
					if (base.localUser.worldWinRankIdx > row.r_idx && base.localUser.worldWinRank <= row.r_idx)
					{
						idList.Add(row.r_idx.ToString());
					}
				}
				else if (base.localUser.worldWinRank <= row.r_idx)
				{
					idList.Add(row.r_idx.ToString());
				}
			}
		});
		List<RewardDataRow> list = new List<RewardDataRow>();
		for (int i = 0; i < idList.Count; i++)
		{
			string id = idList[i];
			UIRankingRewardItem uIRankingRewardItem = rewardListView.FindItem(id) as UIRankingRewardItem;
			for (int j = 0; j < uIRankingRewardItem.rewardList.Count; j++)
			{
				RewardAdd(list, uIRankingRewardItem.rewardList[j]);
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

	private IEnumerator SetImage(string texName)
	{
		if (!AssetBundleManager.HasAssetBundle(texName + ".assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(texName + ".assetbundle", ECacheType.Texture));
		}
		if (AssetBundleManager.HasAssetBundle(texName + ".assetbundle"))
		{
			Texture texture = AssetBundleManager.GetAssetBundle(texName + ".assetbundle").LoadAsset($"{texName}.png") as Texture;
			UISetter.SetTexture(this.texture, texture);
		}
		else
		{
			UISetter.SetTexture(this.texture, Utility.LoadTexture(string.Format(imagePrefix + "{0}", texName)));
		}
		yield return true;
	}
}
