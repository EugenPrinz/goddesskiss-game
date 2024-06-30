using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIRankingBattle : UIPopup
{
	[Serializable]
	public class UpgradeBuff : UIInnerPartBase
	{
		public UILabel resource;

		public WorldDuelBuffItem buffItem;

		public WorldDuelBuffItem buffUpgradeItem;

		public WorldDuelBuffItem deBuffItem;

		public WorldDuelBuffItem deBuffUpgradeItem;

		private EWorldDuelBuff buffType;

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
		}

		public void Set(EWorldDuelBuff buffType)
		{
			this.buffType = buffType;
			UISetter.SetLabel(resource, $"{base.localUser.worldDuelUpgradeCoin}/{base.regulation.FindGoodsServerFieldName(8003).max}");
			buffItem.Set(buffType, upgrade: false);
			buffUpgradeItem.Set(buffType, upgrade: true);
			deBuffItem.Set(buffType, upgrade: false);
			deBuffUpgradeItem.Set(buffType, upgrade: true);
		}

		public override void OnRefresh()
		{
			Set(buffType);
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			string name = sender.name;
			if (name == "BuffSettingClose")
			{
				UISetter.SetActive(root, active: false);
			}
		}
	}

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

	public UILabel ranking;

	public UILabel score;

	public UILabel nextScore;

	public UILabel record;

	public UILabel winningStreak;

	public UILabel reward;

	public UILabel countDescription;

	public UISprite gradeIcon;

	public UILabel gradeNum;

	public GameObject shopBadge;

	public GameObject scoreBadge;

	public GameObject rewardBadge;

	public UIDefaultListView targetListView;

	public UITimer refreshTargetTimer;

	public GameObject refreshCash;

	public UILabel refreshCashLabel;

	public GameObject refresh;

	public UITroop defenderTroop;

	public RecordList recordList;

	public readonly string duelSelectTargetItemPrefix = "DuelTargetCommander-";

	public readonly string replayItemPrefix = "RePlay-";

	public BattleData battleData;

	public UISpineAnimation spineAnimation;

	private UIRankingReward rankingRewardPopUp;

	private UISecretShopPopup secretShop;

	private UIScoreRewardPopup rewardPopUp;

	private string selectIdx;

	public UITexture goBG;

	private bool rankingPopupState;

	public GameObject duelUserInfo;

	public GameObject waveDuelUserInfo;

	public GameObject worldDuelUserInfo;

	public GameObject worldDuelUserEmpty;

	public GameObject worldDuelReMatchBtn;

	public UILabel waveDuelRecord;

	public UILabel waveDuelWinningStreak;

	public GameObject scoreRewardBtn;

	public GameObject refreshCashBtn;

	public UILabel defenderTroopWave;

	public GameObject btnMoveNextDefenderTroop;

	private EBattleType battleType = EBattleType.Duel;

	private int _curDefenderTroopIndex;

	public GameObject userData;

	public GameObject placementRoot;

	public GameObject scoreLabel;

	public UILabel placementLabel;

	public GameObject infoBtn;

	private GameObject infoPopUp;

	public GameObject buffRoot;

	public GameObject targetListRoot;

	public GameObject rankIconRoot;

	public UILabel userName;

	public UILabel userGuildName;

	public UILabel userScore;

	public UILabel userServer1;

	public UILabel userServer2;

	public UILabel worldDuelScore;

	public List<WorldDuelBuffItem> itemList;

	public UpgradeBuff upgradeBuff;

	private string spineBundleName;

	private void Start()
	{
		battleData = BattleData.Create(EBattleType.Duel);
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
		rankingPopupState = false;
		Set();
		UISetter.SetActive(upgradeBuff, active: false);
		if (battleType != EBattleType.WorldDuel)
		{
			UISetter.SetSpine(spineAnimation, "n_011");
			InitAndOpenSelectTarget();
		}
		else if (base.localUser.worldDuelBestRank != null)
		{
			SetSpine(base.localUser.worldDuelBestRank.thumbnailId);
		}
		else
		{
			UISetter.SetSpine(spineAnimation, "n_011");
		}
	}

	public void Set()
	{
		if (base.localUser.currentSeasonDuelTime.GetRemain() <= 0.0)
		{
			timer.SetLabelFormat(Localization.Get("5050014"), null);
			timer.SetFinishString(null);
			UISetter.SetTimer(timer, base.localUser.currentSeasonOpenRemainDuelTime);
		}
		else
		{
			timer.SetLabelFormat(Localization.Get("17091"), null);
			timer.SetFinishString(Localization.Get("5050013"));
			UISetter.SetTimer(timer, base.localUser.currentSeasonDuelTime);
		}
		UISetter.SetActive(userData, active: true);
		UISetter.SetActive(placementRoot, active: false);
		UISetter.SetActive(rankIconRoot, active: true);
		UISetter.SetActive(scoreLabel, battleType != EBattleType.Duel);
		UISetter.SetActive(infoBtn, battleType == EBattleType.Duel || battleType == EBattleType.WorldDuel);
		UISetter.SetActive(buffRoot, battleType == EBattleType.WorldDuel);
		UISetter.SetActive(targetListRoot, battleType != EBattleType.WorldDuel);
		defenderTroop.Set(base.localUser.defenderTroops[_curDefenderTroopIndex]);
		if (battleType == EBattleType.WaveDuel)
		{
			DefineDataRow defineDataRow = base.regulation.defineDtbl["ARENA_NONPERCENT"];
			if (base.localUser.duelRanking <= int.Parse(defineDataRow.value))
			{
				if (base.localUser.duelRanking <= 0)
				{
					UISetter.SetLabel(ranking, Localization.Get("14912"));
				}
				else
				{
					UISetter.SetLabel(ranking, Localization.Format("18010", base.localUser.duelRanking));
				}
			}
			else
			{
				UISetter.SetLabel(ranking, string.Format("{0} ({1}%)", Localization.Format("18010", base.localUser.duelRanking), base.localUser.duelRankingRate));
			}
			UISetter.SetActive(duelUserInfo, active: false);
			UISetter.SetActive(scoreRewardBtn, active: false);
			UISetter.SetActive(refreshCashBtn, active: false);
			UISetter.SetActive(waveDuelUserInfo, active: true);
			UISetter.SetActive(worldDuelUserInfo, active: false);
			UISetter.SetActive(gradeNum, active: false);
			UISetter.SetLabel(title, Localization.Get("5040008"));
			UISetter.SetLabel(waveDuelRecord, Localization.Format("5196", base.localUser.duelWinCount, base.localUser.duelLoseCount));
			UISetter.SetLabel(waveDuelWinningStreak, Localization.Format("18016", base.localUser.duelWinningStreak));
			UISetter.SetLabel(countDescription, Localization.Format("17091", 1, 1, 1));
			UISetter.SetSprite(gradeIcon, base.localUser.GetDuelGradeIcon());
		}
		else if (battleType == EBattleType.Duel)
		{
			DefineDataRow defineDataRow2 = base.regulation.defineDtbl["ARENA_PLACEMENT_MATCH"];
			if (int.Parse(defineDataRow2.value) > base.localUser.duelWinCount + base.localUser.duelLoseCount)
			{
				UISetter.SetActive(userData, active: false);
				UISetter.SetActive(placementRoot, active: true);
			}
			UISetter.SetLabel(placementLabel, Localization.Format("17102", int.Parse(defineDataRow2.value)));
			UISetter.SetLabel(ranking, Localization.Format("17105", base.localUser.duelScore, base.localUser.duelRanking));
			UISetter.SetActive(duelUserInfo, active: true);
			UISetter.SetActive(scoreRewardBtn, active: true);
			UISetter.SetActive(refreshCashBtn, active: true);
			UISetter.SetActive(waveDuelUserInfo, active: false);
			UISetter.SetActive(worldDuelUserInfo, active: false);
			UISetter.SetActive(gradeNum, active: true);
			SetGradeLabel(base.localUser.duelGradeIdx);
			UISetter.SetLabel(title, Localization.Get("5040007"));
			UISetter.SetLabel(score, Localization.Format("17014", base.localUser.duelScore));
			UISetter.SetLabel(record, Localization.Format("17016", base.localUser.duelWinCount, base.localUser.duelLoseCount));
			UISetter.SetLabel(winningStreak, Localization.Format("18016", base.localUser.duelWinningStreak));
			UISetter.SetLabel(countDescription, Localization.Format("17091", 1, 1, 1));
			if (base.localUser.duelRanking == 1)
			{
				UISetter.SetLabel(nextScore, Localization.Get("17103"));
			}
			else
			{
				UISetter.SetLabel(nextScore, Localization.Format("17014", base.localUser.duelNextScore));
			}
			UISetter.SetSprite(gradeIcon, base.localUser.GetDuelGradeIcon());
			int num = int.Parse(base.regulation.defineDtbl["MATCHING_REFRESH_CASH"].value);
			UISetter.SetLabel(refreshCashLabel, num);
		}
		else
		{
			UISetter.SetLabel(title, Localization.Get("400001"));
			UISetter.SetActive(duelUserInfo, active: false);
			UISetter.SetActive(scoreRewardBtn, active: false);
			UISetter.SetActive(refreshCashBtn, active: false);
			UISetter.SetActive(waveDuelUserInfo, active: false);
			UISetter.SetActive(worldDuelUserInfo, active: true);
			UISetter.SetActive(gradeNum, active: true);
			UISetter.SetActive(rankIconRoot, base.localUser.worldDuelBestRank != null);
			UISetter.SetActive(worldDuelReMatchBtn, base.localUser.worldDuelReMatchTarget != null);
			UISetter.SetLabel(ranking, Localization.Get("400002"));
			UISetter.SetLabel(worldDuelScore, Localization.Format("17014", base.localUser.worldDuelScore));
			if (base.localUser.worldDuelBestRank != null)
			{
				UISetter.SetLabel(userName, base.localUser.worldDuelBestRank.nickname);
				UISetter.SetLabel(userScore, Localization.Format("17014", base.localUser.worldDuelBestRank.worldDuelScore));
				UISetter.SetLabel(userServer1, Localization.Format("19067", base.localUser.worldDuelBestRank.world));
				if (base.localUser.worldDuelBestRank.guildWorld != 0)
				{
					UISetter.SetLabel(userGuildName, base.localUser.worldDuelBestRank.guildName);
				}
				else
				{
					UISetter.SetLabel(userGuildName, Localization.Get("400027"));
				}
				SetGradeLabel(base.localUser.worldDuelBestRank.duelGradeIdx);
				UISetter.SetSprite(gradeIcon, base.localUser.worldDuelBestRank.GetDuelGradeIcon());
			}
			else
			{
				UISetter.SetLabel(userServer1, string.Empty);
				UISetter.SetLabel(userName, Localization.Get("400018"));
				UISetter.SetLabel(userScore, Localization.Format("17014", 0));
				UISetter.SetLabel(userGuildName, Localization.Get("400027"));
			}
			for (int i = 0; i < itemList.Count; i++)
			{
				WorldDuelBuffItem worldDuelBuffItem = itemList[i];
				EWorldDuelBuff eWorldDuelBuff = (EWorldDuelBuff)(i + 1);
				worldDuelBuffItem.buffType = base.localUser.activeBuff[eWorldDuelBuff];
				worldDuelBuffItem.Set(eWorldDuelBuff, upgrade: false);
			}
		}
		if (base.localUser.defenderTroops.Count > 1)
		{
			UISetter.SetActive(btnMoveNextDefenderTroop, active: true);
			UISetter.SetLabel(defenderTroopWave, Localization.Format("5040009", _curDefenderTroopIndex + 1));
		}
		else
		{
			UISetter.SetActive(btnMoveNextDefenderTroop, active: false);
			UISetter.SetLabel(defenderTroopWave, Localization.Get("5982"));
		}
		SetBadge();
	}

	private void SetGradeLabel(int duelGradeIdx)
	{
		if (base.regulation.rankingDtbl.ContainsKey(duelGradeIdx.ToString()))
		{
			RankingDataRow data = base.regulation.rankingDtbl[duelGradeIdx.ToString()];
			List<RankingDataRow> list = base.regulation.rankingDtbl.FindAll((RankingDataRow row) => row.type == data.type && row.icon == data.icon);
			UISetter.SetActive(gradeNum, list.Count > 1);
			int num = list.IndexOf(data);
			UISetter.SetLabel(gradeNum, num + 1);
		}
	}

	public void MoveNextDefenderTroop()
	{
		_curDefenderTroopIndex++;
		if (_curDefenderTroopIndex >= base.localUser.defenderTroops.Count)
		{
			_curDefenderTroopIndex = 0;
		}
		defenderTroop.Set(base.localUser.defenderTroops[_curDefenderTroopIndex]);
		UISetter.SetLabel(defenderTroopWave, Localization.Format("5040009", _curDefenderTroopIndex + 1));
	}

	private void SetBadge()
	{
		UISetter.SetActive(scoreBadge, base.localUser.badgeChallenge);
		UISetter.SetActive(shopBadge, base.localUser.badgeChallengeShop);
		if (battleType == EBattleType.WaveDuel)
		{
			UISetter.SetActive(rewardBadge, active: false);
			UISetter.SetActive(shopBadge, base.localUser.badgeWaveDuelShop);
		}
		else if (battleType == EBattleType.Duel)
		{
			UISetter.SetActive(scoreBadge, base.localUser.badgeChallenge);
			UISetter.SetActive(shopBadge, base.localUser.badgeChallengeShop);
			UISetter.SetActive(rewardBadge, base.localUser.winRank != base.localUser.winRankIdx);
		}
		else
		{
			UISetter.SetActive(shopBadge, active: false);
			UISetter.SetActive(rewardBadge, base.localUser.worldWinRank != base.localUser.worldWinRankIdx);
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close")
		{
			if (!bBackKeyEnable)
			{
				Close();
			}
			return;
		}
		if (text.StartsWith(replayItemPrefix))
		{
			string pureId = Utility.GetPureId(text, replayItemPrefix);
			ERePlayType type = ERePlayType.Challenge;
			if (battleType == EBattleType.WaveDuel)
			{
				type = ERePlayType.WaveDuel;
			}
			else if (battleType == EBattleType.Duel)
			{
				type = ERePlayType.Challenge;
			}
			else if (battleType == EBattleType.WorldDuel)
			{
				type = ERePlayType.WorldDuel;
			}
			base.network.RequestGetRecordInfo(pureId, type);
			return;
		}
		if (targetListView.Contains(text))
		{
			if (base.localUser.defenderTroops[0].IsEmpty())
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("17074"));
				return;
			}
			if (battleType == EBattleType.WaveDuel)
			{
				if (base.localUser.waveDuelTicket <= 0)
				{
					base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.waveDuelTicket);
					return;
				}
				if (base.localUser.currentSeasonDuelTime.GetRemain() <= 0.0)
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5050013"));
					return;
				}
			}
			else if (base.localUser.challenge <= 0)
			{
				base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Challenge);
				return;
			}
			string pureId2 = targetListView.GetPureId(text);
			RoUser defender = base.localUser.duelTargetList[int.Parse(pureId2)];
			base.localUser.duelTargetIdx = int.Parse(pureId2);
			BattleData battleData = this.battleData;
			battleData.type = battleType;
			battleData.defender = defender;
			UIManager.instance.world.readyBattle.InitAndOpenReadyBattle(battleData);
			return;
		}
		switch (text)
		{
		case "CommanderEditBtn":
		{
			BattleData battleData2 = this.battleData;
			if (battleType == EBattleType.WaveDuel)
			{
				battleData2.type = EBattleType.WaveDuelDefender;
			}
			else if (battleType == EBattleType.Duel)
			{
				battleData2.type = EBattleType.Defender;
			}
			else if (battleType == EBattleType.WorldDuel)
			{
				battleData2.type = EBattleType.WorldDuelDefender;
			}
			RoTroop item = RoTroop.Create(base.localUser.id);
			battleData2.defender = base.localUser.CreateForBattle(new List<RoTroop> { item });
			battleData2.attacker = base.localUser.CreateForBattle(base.localUser.defenderTroops);
			UIManager.instance.world.readyBattle.InitAndOpenReadyBattle(battleData2);
			break;
		}
		case "FindTarget":
			base.network.RequestPvPDuelList();
			break;
		case "RewardBtn":
			base.network.RequestReceivePvPReward();
			break;
		case "RewardInfoBtn":
			if (rankingRewardPopUp == null)
			{
				rankingRewardPopUp = UIPopup.Create("RankingReward").GetComponent<UIRankingReward>();
				ERankingContentsType contentsType = ERankingContentsType.ChallengeGrade;
				if (battleType == EBattleType.WaveDuel)
				{
					contentsType = ERankingContentsType.WaveDuel;
				}
				else if (battleType == EBattleType.Duel)
				{
					contentsType = ERankingContentsType.ChallengeGrade;
				}
				else if (battleType == EBattleType.WorldDuel)
				{
					contentsType = ERankingContentsType.WorldDuelGrade;
				}
				rankingRewardPopUp.Init(contentsType);
			}
			break;
		case "RefreshDuelTarget":
			if (battleType == EBattleType.WaveDuel)
			{
				if (base.localUser.duelTargetRefreshTime.GetRemain() <= 0.0)
				{
					base.network.RequestRefreshPvPWaveDuelList();
				}
			}
			else if (base.localUser.duelTargetRefreshTime.GetRemain() <= 0.0)
			{
				base.network.RequestRefreshPvPDuelList();
			}
			else
			{
				RefreshDuelList();
			}
			break;
		case "RankingBtn":
			if (battleType == EBattleType.WorldDuel)
			{
				UIPopup.Create<RankingList>("RankingList").Set(EBattleType.WorldDuel, RemoteObjectManager.instance.duelRankingList);
			}
			else if (!rankingPopupState)
			{
				rankingPopupState = true;
				if (battleType == EBattleType.WaveDuel)
				{
					base.network.RequestPvPWaveDuelRankingList();
				}
				else
				{
					base.network.RequestPvPRankingList();
				}
			}
			else
			{
				UIPopup.Create<RankingList>("RankingList").Set(battleType, RemoteObjectManager.instance.duelRankingList);
			}
			break;
		case "ScoreRewardBtn":
			if (rewardPopUp == null)
			{
				rewardPopUp = UIPopup.Create<UIScoreRewardPopup>("ScoreRewardPopup");
				rewardPopUp.Init();
			}
			break;
		case "RecordBtn":
		{
			if (recordList == null)
			{
				recordList = UIPopup.Create<RecordList>("RecordList");
			}
			ERePlayType type2 = ERePlayType.Challenge;
			if (battleType == EBattleType.WaveDuel)
			{
				type2 = ERePlayType.WaveDuel;
			}
			else if (battleType == EBattleType.Duel)
			{
				type2 = ERePlayType.Challenge;
			}
			else if (battleType == EBattleType.WorldDuel)
			{
				type2 = ERePlayType.WorldDuel;
			}
			recordList.Init(type2);
			break;
		}
		case "ShopBtn":
			if (secretShop == null)
			{
				secretShop = UIPopup.Create<UISecretShopPopup>("SecretShopPopup");
				EShopType type3 = EShopType.ChallengeShop;
				if (battleType == EBattleType.WaveDuel)
				{
					type3 = EShopType.WaveDuelShop;
				}
				else if (battleType == EBattleType.Duel)
				{
					type3 = EShopType.ChallengeShop;
				}
				else if (battleType == EBattleType.WorldDuel)
				{
					type3 = EShopType.WorldDuelShop;
				}
				secretShop.Init(type3);
			}
			break;
		case "MoveNextDefenderTroopBtn":
			MoveNextDefenderTroop();
			break;
		case "InfoBtn":
			if (infoPopUp == null)
			{
				UISimplePopup uISimplePopup = UISimplePopup.CreateOK("InformationPopup");
				if (battleType == EBattleType.Duel)
				{
					uISimplePopup.Set(localization: true, "5040007", "17100", string.Empty, "1001", string.Empty, string.Empty);
				}
				else if (battleType == EBattleType.WorldDuel)
				{
					uISimplePopup.Set(localization: true, "400001", "400025", string.Empty, "1001", string.Empty, string.Empty);
				}
				infoPopUp = uISimplePopup.gameObject;
			}
			break;
		case "WorldPvPStartBtn":
			if (!base.localUser.worldDuelBattleEnable)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("400014"));
			}
			else if (base.localUser.defenderTroops[0].IsEmpty())
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("17074"));
			}
			else if (base.localUser.currentSeasonDuelTime.GetRemain() <= 0.0)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5050013"));
			}
			else if (base.localUser.worldDuelTicket <= 0)
			{
				base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.WorldDuelTicket);
			}
			else
			{
				base.network.RequestWorldDuelEnemyInfo();
			}
			break;
		case "WorldPvPReMatchBtn":
			if (!base.localUser.worldDuelBattleEnable)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("400014"));
			}
			else if (base.localUser.defenderTroops[0].IsEmpty())
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("17074"));
			}
			else if (base.localUser.currentSeasonDuelTime.GetRemain() <= 0.0)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5050013"));
			}
			else if (base.localUser.worldDuelTicket <= 0)
			{
				base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.WorldDuelTicket);
			}
			else
			{
				OpenWorldDuelReMatchReadyBattle();
			}
			break;
		default:
			if (text.StartsWith("Change-"))
			{
				string value = text.Substring(text.IndexOf("-") + 1);
				UISetter.SetActive(upgradeBuff.root, active: true);
				EWorldDuelBuff buffType = (EWorldDuelBuff)Enum.Parse(typeof(EWorldDuelBuff), value);
				upgradeBuff.Set(buffType);
			}
			else if (text.StartsWith("Upgrade-"))
			{
				string text2 = text.Substring(text.IndexOf("-") + 1);
				SetUpgradeBuff(text2);
			}
			else if (text.StartsWith("Setting-"))
			{
				string selectBuff = text.Substring(text.IndexOf("-") + 1);
				CloseUpgradeBuff();
				SetSelectBuff(selectBuff);
			}
			else
			{
				SendOnClickToInnerParts(sender);
			}
			break;
		}
	}

	public void OpenWorldDuelReadyBattle()
	{
		BattleData battleData = this.battleData;
		battleData.type = battleType;
		battleData.worldDuelReMatch = false;
		battleData.defender = base.localUser.worldDuelTarget;
		UIManager.instance.world.readyBattle.InitAndOpenReadyBattle(battleData);
	}

	public void OpenWorldDuelReMatchReadyBattle()
	{
		BattleData battleData = this.battleData;
		battleData.type = battleType;
		battleData.worldDuelReMatch = true;
		battleData.defender = base.localUser.worldDuelReMatchTarget;
		UIManager.instance.world.readyBattle.InitAndOpenReadyBattle(battleData);
	}

	private void SetUpgradeBuff(string buff)
	{
		string value = buff.Substring(0, 3);
		string value2 = buff.Substring(3);
		EWorldDuelBuff EbuffType = (EWorldDuelBuff)Enum.Parse(typeof(EWorldDuelBuff), value);
		EWorldDuelBuffEffect EbuffEffect = (EWorldDuelBuffEffect)Enum.Parse(typeof(EWorldDuelBuffEffect), value2);
		int level = Mathf.Max(base.localUser.worldDuelBuff[buff], 1);
		StrongestBuffBattleDataRow strongestBuffBattleDataRow = base.regulation.strongestBuffBattleDtbl.Find((StrongestBuffBattleDataRow row) => row.buffTarget == EbuffType && row.buffEffectType == EbuffEffect && row.buffLevel == level);
		if (base.localUser.worldDuelUpgradeCoin >= strongestBuffBattleDataRow.upgradeCoin)
		{
			base.network.RequestWorldDuelBuffUpgrade(buff);
		}
	}

	private void SetSelectBuff(string buff)
	{
		base.network.RequesWorldDuelBuffSetting(buff);
	}

	private void RefreshDuelList()
	{
		int num = int.Parse(base.regulation.defineDtbl["MATCHING_REFRESH_CASH"].value);
		if (base.localUser.cash >= num)
		{
			UISimplePopup.CreateBool(localization: true, "1014", "17032", string.Empty, "1013", "1000").onClick = delegate(GameObject sender)
			{
				string text2 = sender.name;
				if (text2 == "OK")
				{
					base.network.RequestRefreshPvPDuelList();
				}
				else if (!(text2 == "Cancel"))
				{
				}
			};
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				UIManager.instance.world.mainCommand.OpenDiamonShop();
			}
		};
	}

	public void SetAttacker(RoUser user)
	{
		battleData.attacker = user;
		base.network.RequestPvPDuelInfo(selectIdx);
	}

	public override void OnRefresh()
	{
		SetSelectTarget();
		Set();
		if (secretShop != null)
		{
			secretShop.OnRefresh();
		}
		if (rewardPopUp != null)
		{
			rewardPopUp.OnRefresh();
		}
		if (recordList != null)
		{
			recordList.OnRefresh();
		}
		if (rankingRewardPopUp != null)
		{
			rankingRewardPopUp.OnRefresh();
		}
		if (upgradeBuff.isActive)
		{
			upgradeBuff.OnRefresh();
		}
	}

	public void InitAndOpen(EBattleType type)
	{
		if (!bEnterKeyEnable)
		{
			base.localUser.isUpdateAtkRecord = true;
			bEnterKeyEnable = true;
			battleType = type;
			_curDefenderTroopIndex = 0;
			Open();
			OpenPopupShow();
			Init();
		}
	}

	public void CloseUpgradeBuff()
	{
		UISetter.SetActive(upgradeBuff, active: false);
	}

	public void InitAndOpenSelectTarget()
	{
		SetSelectTarget();
	}

	public void SetSelectTarget()
	{
		int num = (int)base.localUser.duelTargetRefreshTime.GetRemain();
		UISetter.SetTimer(refreshTargetTimer, base.localUser.duelTargetRefreshTime);
		refreshTargetTimer.RegisterOnFinished(delegate
		{
			RefreshEndTime();
		});
		UISetter.SetActive(refresh, num <= 0);
		UISetter.SetActive(refreshCash, num > 0);
		if (targetListView.scrollView != null)
		{
			targetListView.scrollView.SetDragAmount(0f, 0f, updateScrollbars: false);
		}
		targetListView.InitDuelList(battleType, base.localUser.duelTargetList, duelSelectTargetItemPrefix);
		targetListView.SetSelection(null, selected: false);
	}

	public void RefreshEndTime()
	{
		UISetter.SetActive(refresh, active: true);
		UISetter.SetActive(refreshCash, active: false);
	}

	public void SetSpine(string thumbnailId)
	{
		string costumeThumbnailName = base.regulation.GetCostumeThumbnailName(int.Parse(thumbnailId));
		string[] array = costumeThumbnailName.Split('_');
		string resourceId = array[0] + "_" + array[1];
		StartCoroutine(CreateSpineFromCache(resourceId, array[2]));
	}

	private IEnumerator CreateSpineFromCache(string resourceId, string costumeName)
	{
		if (spineAnimation != null)
		{
			if (!string.IsNullOrEmpty(spineBundleName) && spineBundleName != resourceId + ".assetbundle")
			{
				AssetBundleManager.DeleteAssetBundle(spineBundleName);
			}
			if (!AssetBundleManager.HasAssetBundle(resourceId + ".assetbundle"))
			{
				yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(resourceId + ".assetbundle"));
			}
			if (base.gameObject.activeSelf)
			{
				UISetter.SetSpine(spineAnimation, resourceId, costumeName);
			}
			spineAnimation.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
			spineBundleName = resourceId + ".assetbundle";
		}
		yield return null;
	}

	private void OpenPopupShow()
	{
		if (bEnterKeyEnable)
		{
			base.Open();
			OnAnimOpen();
		}
	}

	public override void Open()
	{
		EGoods resourceView = EGoods.Challenge;
		if (battleType == EBattleType.WaveDuel)
		{
			resourceView = EGoods.WaveDuelTicket;
		}
		else if (battleType == EBattleType.Duel)
		{
			resourceView = EGoods.Challenge;
		}
		else if (battleType == EBattleType.WorldDuel)
		{
			resourceView = EGoods.WorldDuelTicket;
		}
		base.uiWorld.mainCommand.SetResourceView(resourceView);
	}

	public override void Close()
	{
		bBackKeyEnable = true;
		HidePopup();
		base.uiWorld.mainCommand.SetResourceView(EGoods.Bullet);
		base.localUser.duelScore = 0;
		base.localUser.worldDuelScore = 0;
		base.localUser.duelRanking = 0;
		base.localUser.worldDuelRanking = 0;
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
		if (upgradeBuff.isActive)
		{
			CloseUpgradeBuff();
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
