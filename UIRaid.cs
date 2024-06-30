using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared.Regulation;
using UnityEngine;

public class UIRaid : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimInfo;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UILabel summaryTitle;

	public UITimer timer;

	public UILabel rankingTitle;

	public UILabel rankingRate;

	public UILabel ranking;

	public UILabel score;

	public UILabel raidCnt;

	public UILabel nextScore;

	public UILabel bestScore;

	public UILabel description;

	public UISprite gradeIcon;

	public UISprite shopBadge;

	public UISprite rewardBadge;

	public UIDefaultListView targetListView;

	public UITimer battleTargetTimer;

	public TimeData raidTime;

	public UITroop attackerTroop;

	public UITroop raidTroop;

	public RoRaid raid;

	public UISpineAnimation spineAnimation;

	public UIDefaultListView skillListView;

	public List<int> raidIdList;

	public GameObject raidOpenTitle;

	private UIPopup rankingRewardPopUp;

	private UISecretShopPopup secretShop;

	private UIPopup rewardPopup;

	public UITexture goBG;

	private new void Awake()
	{
		UISetter.SetSpine(spineAnimation, "n_009");
		raidTime = TimeData.Create();
		base.Awake();
	}

	private void OnDestroy()
	{
		if (goBG != null)
		{
			goBG = null;
		}
		if (AnimBlock != null)
		{
			AnimBlock = null;
		}
	}

	public void Init()
	{
		DefineDataRow defineDataRow = base.regulation.defineDtbl["RAID_NONPERCENT"];
		if (base.localUser.raidRanking <= int.Parse(defineDataRow.value))
		{
			if (base.localUser.raidRanking <= 0)
			{
				UISetter.SetLabel(ranking, Localization.Get("14912"));
			}
			else
			{
				UISetter.SetLabel(ranking, Localization.Format("18010", base.localUser.raidRanking));
			}
		}
		else
		{
			UISetter.SetLabel(ranking, string.Format("{0} ({1}%)", Localization.Format("18010", base.localUser.raidRanking), base.localUser.raidRankingRate));
		}
		UISetter.SetLabel(score, Localization.Format("17014", base.localUser.raidScore));
		UISetter.SetLabel(nextScore, Localization.Format("17014", base.localUser.raidNextScore));
		UISetter.SetLabel(raidCnt, Localization.Format("18016", base.localUser.raidCount));
		UISetter.SetLabel(bestScore, Localization.Format("17014", base.localUser.raidBestScore));
		UISetter.SetSprite(gradeIcon, base.localUser.GetRaidGradeIcon());
		SetBadge();
	}

	public void SetRaidId(List<Dictionary<string, int>> raidIdList, int endTime)
	{
		raid = RoRaid.Create(int.Parse(raidIdList[0].Keys.ElementAt(0)), 0.0, 50.0, 0.0);
		SetRaidData(raidIdList);
		SetSkillData();
		UISetter.SetActive(raidOpenTitle, endTime > 0);
		raidTime.SetByDuration(endTime);
		if (endTime > 0)
		{
			battleTargetTimer.RegisterOnFinished(delegate
			{
				base.network.RaidRankingList();
			});
		}
		battleTargetTimer.SetFinishString(Localization.Get("18043"));
		UISetter.SetTimer(battleTargetTimer, raidTime);
	}

	public void SetRaidData(List<Dictionary<string, int>> raidIdList)
	{
		List<RoCommander> list = new List<RoCommander>();
		for (int i = 0; i < raidIdList.Count; i++)
		{
			RaidChallengeDataRow raidChallengeDataRow = base.regulation.raidChallengeDtbl[raidIdList[i].Keys.ElementAt(0)];
			RoCommander item = RoCommander.Create(raidChallengeDataRow.commanderId, 1, 1, 1, 0, 0, 0, new List<int>());
			list.Add(item);
		}
		targetListView.InitRaidList(list);
		for (int j = 0; j < targetListView.itemList.Count; j++)
		{
			RaidItem raidItem = targetListView.itemList[j] as RaidItem;
			raidItem.SetRemainTime(raidIdList[j].Values.ElementAt(0));
		}
		if (list.Count > 0)
		{
			UISetter.SetLabel(description, Localization.Get(list[0].unitReg.explanation));
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		switch (text)
		{
		case "Close":
			if (!bBackKeyEnable)
			{
				Close();
			}
			return;
		case "StartBattle":
		{
			BattleData battleData = BattleData.Create(EBattleType.Raid);
			battleData.defender = RoUser.CreateNPC("Enemy-" + 0, "NPC", RoTroop.CreateBoss(raid.commanderId));
			battleData.raidData = raid;
			base.uiWorld.readyBattle.InitAndOpenReadyBattle(battleData);
			return;
		}
		case "RewardBtn":
			OpenScoreReward();
			return;
		case "RankingBtn":
			UIPopup.Create<RankingList>("RankingList").Set(EBattleType.Raid, RemoteObjectManager.instance.raidRankingList);
			return;
		case "RewardInfoBtn":
			if (rankingRewardPopUp == null)
			{
				rankingRewardPopUp = UIPopup.Create("RankingReward");
				rankingRewardPopUp.GetComponent<UIRankingReward>().Init(ERankingContentsType.Raid);
			}
			return;
		}
		if (skillListView.Contains(text))
		{
			string pureId = skillListView.GetPureId(text);
			if (!string.Equals(pureId, "0"))
			{
				SetSkillInfo(pureId);
			}
		}
		else if (text == "ShopBtn" && secretShop == null)
		{
			secretShop = UIPopup.Create<UISecretShopPopup>("SecretShopPopup");
			secretShop.Init(EShopType.RaidShop);
		}
	}

	public void OpenScoreReward()
	{
		if (rewardPopup == null)
		{
			rewardPopup = UIPopup.Create<UIRaidScoreRewardPopup>("RaidScoreRewardPopUp");
		}
	}

	public override void OnRefresh()
	{
		if (secretShop != null)
		{
			secretShop.OnRefresh();
		}
		if (rewardPopup != null)
		{
			rewardPopup.OnRefresh();
		}
		SetBadge();
	}

	public void InitAndOpen()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			Open();
			OpenPopupShow();
		}
	}

	public void SetSkillData()
	{
		List<SkillDataRow> list = new List<SkillDataRow>();
		for (int i = 0; i < raid.commander.currLevelUnitReg.skillDrks.Count; i++)
		{
			if (!string.Equals(raid.commander.currLevelUnitReg.skillDrks[i], "0"))
			{
				SkillDataRow item = base.regulation.skillDtbl[raid.commander.currLevelUnitReg.skillDrks[i]];
				list.Add(item);
			}
		}
		RaidChallengeDataRow raidChallengeDataRow = base.regulation.raidChallengeDtbl[raid.raidId.ToString()];
		for (int j = 0; j < raidChallengeDataRow.parts.Count; j++)
		{
			if (string.IsNullOrEmpty(raidChallengeDataRow.parts[j]) || raidChallengeDataRow.parts[j] == "0")
			{
				continue;
			}
			UnitDataRow unitDataRow = base.regulation.unitDtbl[raidChallengeDataRow.parts[j]];
			for (int k = 0; k < unitDataRow.skillDrks.Count; k++)
			{
				if (!string.IsNullOrEmpty(unitDataRow.skillDrks[k]) && !(unitDataRow.skillDrks[k] == "0"))
				{
					SkillDataRow item2 = base.regulation.skillDtbl[unitDataRow.skillDrks[k]];
					list.Add(item2);
				}
			}
		}
		skillListView.InitSkillList(list, "Skill-");
	}

	private void SetSkillInfo(string id)
	{
		UISkillInfoPopup uISkillInfoPopup = UIPopup.Create<UISkillInfoPopup>("SkillInfoPopup");
		uISkillInfoPopup.Set(localization: true, "10056", null, null, null, "10048", null);
		uISkillInfoPopup.SetInfo(id);
	}

	private void SetBadge()
	{
		UISetter.SetActive(rewardBadge, base.localUser.raidRank > base.localUser.raidRewardPoint);
		UISetter.SetActive(shopBadge, base.localUser.badgeRaidShop);
	}

	public void OpenPopupShow()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Open()
	{
		base.uiWorld.mainCommand.SetResourceView(EGoods.Key);
	}

	public override void Close()
	{
		bBackKeyEnable = true;
		HidePopup();
		base.uiWorld.mainCommand.SetResourceView(EGoods.Bullet);
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
		if (secretShop != null)
		{
			secretShop.Close();
		}
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
		AnimBG.Reset();
		AnimNpc.Reset();
		AnimInfo.Reset();
		AnimTitle.Reset();
		AnimBlock.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimInfo.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimInfo.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public void AnimOpenFinish()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
	}

	protected override void OnEnablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: true);
	}

	protected override void OnDisablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: false);
	}
}
