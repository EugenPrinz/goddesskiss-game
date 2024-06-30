using System;
using System.Collections;
using System.Collections.Generic;
using ICode;
using Shared;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

public class UIBattleResult : UIPopup
{
	[Serializable]
	public class DuelResult : UIInnerPartBase
	{
		public GameObject winView;

		public GameObject loseView;

		public GameObject onInvalidationView;

		public Animation winTitleAnimation;

		public Animation loseTitleAnimation;

		public UISprite title;

		public UILabel battleTime;

		public UILabel rank;

		public UILabel rankPersent;

		public UILabel baseScore;

		public UILabel getScore;

		public UILabel stackScore;

		public UILabel curScore;

		public UILabel winScoreMark;

		public UILabel stackScoreMark;

		public UILabel winScoreName;

		public UILabel winScore;

		public UILabel loseStreakName;

		public UIGrid loseBtnGrid;

		public GameObject onInvalidation;

		public GameObject invalidationBtn;

		private UIBattleResult _parent;

		public void Init()
		{
			_parent = base.parentPanelBase as UIBattleResult;
			_parent.StartCoroutine(_PlayAnimation());
			if (_parent._isWin)
			{
				UISetter.SetActive(winView, active: true);
				UISetter.SetActive(loseView, active: false);
				UISetter.SetSprite(title, "result00_pvp_win");
				UISetter.SetColor(getScore, Color.white);
				UISetter.SetColor(curScore, Color.white);
				UISetter.SetColor(winScore, Color.white);
				UISetter.SetLabel(winScoreName, Localization.Get("5940"));
			}
			else
			{
				UISetter.SetActive(winView, active: false);
				UISetter.SetActive(loseView, active: true);
				UISetter.SetSprite(title, "result00_pvp_lose");
				UISetter.SetColor(getScore, new Color(0.97f, 0.25f, 0.25f));
				UISetter.SetColor(curScore, new Color(0.97f, 0.25f, 0.25f));
				UISetter.SetColor(winScore, new Color(0.97f, 0.25f, 0.25f));
				UISetter.SetLabel(winScoreName, Localization.Get("5943"));
			}
		}

		private IEnumerator _PlayAnimation()
		{
			UISetter.SetActive(root, active: false);
			UISetter.SetActive(_parent.commanderListView, active: false);
			Animation targetAni = ((!_parent._isWin) ? loseTitleAnimation : winTitleAnimation);
			if (targetAni != null)
			{
				while (targetAni.isPlaying)
				{
					yield return null;
				}
			}
			UISetter.SetActive(root, active: true);
			UISetter.SetActive(_parent.commanderListView, active: true);
			if (base.localUser.statistics.vipShopisFloating)
			{
				RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.VipShop);
				NetworkAnimation.Instance.CreateFloatingText_OnlyUIToast(Localization.Get("22007"), roBuilding.reg.resourceId);
				base.localUser.statistics.vipShopisFloating = false;
			}
		}

		public void SetBattleTime(float totalSec)
		{
			UISetter.SetLabel(battleTime, Utility.GetTimeStringColonFormat(totalSec));
		}

		public void SetStackScore(int score)
		{
			UISetter.SetLabel(stackScore, Mathf.Abs(score).ToString("N0"));
		}

		public void SetRank(int rank)
		{
			string text = string.Format(Localization.Get("5460"), rank.ToString("N0"));
			UISetter.SetLabel(this.rank, text);
		}

		public void SetRankPersent(float persent)
		{
			string text = $"({persent}%)";
			UISetter.SetLabel(rankPersent, text);
		}

		public void SetBaseScore(int baseScore)
		{
			UISetter.SetLabel(this.baseScore, Mathf.Abs(baseScore).ToString("N0"));
		}

		public void SetGetScore(int getScore)
		{
			UISetter.SetLabel(winScoreMark, (getScore >= 0) ? "+" : "-");
			UISetter.SetLabel(this.getScore, Mathf.Abs(getScore).ToString("N0"));
		}

		public void SetCurScore(int curScore)
		{
			UISetter.SetLabel(this.curScore, Mathf.Abs(curScore).ToString("N0"));
		}

		public void SetWinSocreName(int winStreak)
		{
		}

		public void SetWinScore(int winScore)
		{
			UISetter.SetLabel(stackScoreMark, (winScore >= 0) ? "+" : "-");
			UISetter.SetLabel(this.winScore, Mathf.Abs(winScore).ToString("N0"));
		}

		public void SetLoseStreakName(int winStreak)
		{
		}

		public void ResetLoseScore()
		{
			UISetter.SetActive(onInvalidation, active: true);
			UISetter.SetActive(invalidationBtn, active: false);
			loseBtnGrid.repositionNow = true;
		}
	}

	[Serializable]
	public class WaveDuelResult : UIInnerPartBase
	{
		public UISprite title;

		public Animation titleAnimation;

		public UILabel prevRank;

		public UILabel currRank;

		public GameObject chgRank;

		public GameObject notChgRank;

		private UIBattleResult _parent;

		public void Init()
		{
			_parent = base.parentPanelBase as UIBattleResult;
			_parent.StartCoroutine(_PlayAnimation());
			if (_parent._isWin)
			{
				UISetter.SetSprite(title, "result00_pvp_win");
			}
			else
			{
				UISetter.SetSprite(title, "result00_pvp_lose");
			}
		}

		private IEnumerator _PlayAnimation()
		{
			UISetter.SetActive(root, active: false);
			UISetter.SetActive(_parent.commanderListView, active: false);
			Animation targetAni = titleAnimation;
			if (targetAni != null)
			{
				while (targetAni.isPlaying)
				{
					yield return null;
				}
			}
			UISetter.SetActive(root, active: true);
			UISetter.SetActive(_parent.commanderListView, active: true);
			if (base.localUser.statistics.vipShopisFloating)
			{
				RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.VipShop);
				NetworkAnimation.Instance.CreateFloatingText_OnlyUIToast(Localization.Get("22007"), roBuilding.reg.resourceId);
				base.localUser.statistics.vipShopisFloating = false;
			}
		}

		public void SetRank(int prevRank, int currRank)
		{
			if (prevRank != currRank)
			{
				UISetter.SetActive(chgRank, active: true);
				UISetter.SetActive(notChgRank, active: false);
				UISetter.SetLabel(this.prevRank, Localization.Format("18010", prevRank));
				UISetter.SetLabel(this.currRank, Localization.Format("18010", currRank));
			}
			else
			{
				UISetter.SetActive(chgRank, active: false);
				UISetter.SetActive(notChgRank, active: true);
			}
		}
	}

	[Serializable]
	public class WorldDuelResult : UIInnerPartBase
	{
		public Animation winTitleAnimation;

		public Animation loseTitleAnimation;

		public UISprite title;

		public UILabel baseScore;

		public UILabel getScore;

		public UILabel curScore;

		public UILabel winScoreMark;

		private UIBattleResult _parent;

		public void Init()
		{
			_parent = base.parentPanelBase as UIBattleResult;
			_parent.StartCoroutine(_PlayAnimation());
			if (_parent._isWin)
			{
				UISetter.SetSprite(title, "result00_pvp_win");
				UISetter.SetColor(getScore, Color.white);
				UISetter.SetColor(curScore, Color.white);
			}
			else
			{
				UISetter.SetSprite(title, "result00_pvp_lose");
				UISetter.SetColor(getScore, new Color(0.97f, 0.25f, 0.25f));
				UISetter.SetColor(curScore, new Color(0.97f, 0.25f, 0.25f));
			}
		}

		private IEnumerator _PlayAnimation()
		{
			UISetter.SetActive(root, active: false);
			UISetter.SetActive(_parent.commanderListView, active: false);
			Animation targetAni = ((!_parent._isWin) ? loseTitleAnimation : winTitleAnimation);
			if (targetAni != null)
			{
				while (targetAni.isPlaying)
				{
					yield return null;
				}
			}
			UISetter.SetActive(root, active: true);
			UISetter.SetActive(_parent.commanderListView, active: true);
			if (base.localUser.statistics.vipShopisFloating)
			{
				RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.VipShop);
				NetworkAnimation.Instance.CreateFloatingText_OnlyUIToast(Localization.Get("22007"), roBuilding.reg.resourceId);
				base.localUser.statistics.vipShopisFloating = false;
			}
		}

		public void SetBaseScore(int baseScore)
		{
			UISetter.SetLabel(this.baseScore, Mathf.Abs(baseScore).ToString("N0"));
		}

		public void SetGetScore(int getScore)
		{
			UISetter.SetLabel(winScoreMark, (getScore >= 0) ? "+" : "-");
			UISetter.SetLabel(this.getScore, Mathf.Abs(getScore).ToString("N0"));
		}

		public void SetCurScore(int curScore)
		{
			UISetter.SetLabel(this.curScore, Mathf.Abs(curScore).ToString("N0"));
		}
	}

	[Serializable]
	public class PlunderResult : UIInnerPartBase
	{
		[Serializable]
		public class SubTypeData
		{
			public Animation winTitleAnimation;

			public Animation loseTitleAnimation;
		}

		public GameObject winView;

		public GameObject loseView;

		public GameObject winBattleInfoView;

		public GameObject expView;

		public GameObject goldView;

		public GameObject buttonsRoot;

		public SubTypeData plunder;

		public SubTypeData guerrilla;

		public UISprite title;

		public UILabel battleTime;

		public UILabel exp;

		public UILabel gold;

		public UIDefaultListView rewardList;

		public GameObject noRewardItems;

		public List<GameObject> clearRankStar;

		public UILabel commanderName;

		public UILabel favor;

		public GameObject btnWorldMap;

		public GameObject btnCamp;

		public GameObject btnSituation;

		public GameObject btnAnnihilation;

		public GameObject btnNextAnnihilation;

		public GameObject btnWaveBattle;

		public GameObject btnNextStage;

		public GameObject btnEventBattle;

		public GameObject btnEventBattleRetry;

		public GameObject repeatRoot;

		public UILabel repeatTime;

		private UIBattleResult _parent;

		private int rank;

		private Dictionary<string, Protocols.UserInformationResponse.Commander> updateCommanderList;

		private string RewardAnimationName = "BattleResultPopUp_Win_ItemOpen";

		private string LoseAnimationName = "BattleResultPopUp_Default_Title";

		private IEnumerator _PlayAnimation()
		{
			_parent._canClick = false;
			UISetter.SetActive(_parent.commanderListView, active: false);
			UISetter.SetActive(noRewardItems, active: false);
			UISetter.SetActive(_parent.clearRankView, active: false);
			UISetter.SetActive(buttonsRoot, active: false);
			UISetter.SetActive(winBattleInfoView, active: false);
			Animation targetAni = null;
			if (_parent._battleType == EBattleType.Plunder || _parent._battleType == EBattleType.EventBattle)
			{
				if (plunder.winTitleAnimation != null)
				{
					plunder.winTitleAnimation.gameObject.SetActive(value: true);
				}
				targetAni = ((!_parent._isWin) ? plunder.loseTitleAnimation : plunder.winTitleAnimation);
			}
			else if (_parent._battleType == EBattleType.WaveBattle)
			{
				if (plunder.winTitleAnimation != null)
				{
					plunder.winTitleAnimation.gameObject.SetActive(value: false);
				}
				if (_parent._battleData.isWin)
				{
					UISetter.SetLabel(_parent.WaveCount, string.Format(Localization.Get("4816"), _parent._battleData.WaveCount));
				}
				else
				{
					UISetter.SetLabel(_parent.WaveCount, string.Format(Localization.Get("4816"), _parent._battleData.WaveCount - 1));
				}
				targetAni = ((!_parent._isWin) ? guerrilla.loseTitleAnimation : guerrilla.winTitleAnimation);
			}
			else
			{
				if (plunder.winTitleAnimation != null)
				{
					plunder.winTitleAnimation.gameObject.SetActive(value: false);
				}
				targetAni = ((!_parent._isWin) ? guerrilla.loseTitleAnimation : guerrilla.winTitleAnimation);
			}
			yield return null;
			if (targetAni != null)
			{
				while (targetAni.isPlaying)
				{
					yield return null;
				}
			}
			if (GameSetting.instance.repeatBattle)
			{
				if (_parent._isWin)
				{
					if (_parent._battleType == EBattleType.Plunder)
					{
						WorldMapStageDataRow worldMapStageDataRow = base.regulation.worldMapStageDtbl[_parent._battleData.stageId];
						if (base.localUser.bullet < worldMapStageDataRow.bullet)
						{
							GameSetting.instance.repeatBattle = false;
							UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, string.Format(Localization.Get("10000009"), Localization.Get("4006")), Localization.Get("1001"));
						}
					}
					else if (_parent._battleType == EBattleType.Guerrilla)
					{
						if (base.localUser.sweepTicket < 1)
						{
							GameSetting.instance.repeatBattle = false;
							UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, string.Format(Localization.Get("10000009"), Localization.Get("4008")), Localization.Get("1001"));
						}
					}
					else if (_parent._battleType == EBattleType.EventBattle)
					{
						if (base.localUser.oil < 1)
						{
							GameSetting.instance.repeatBattle = false;
							UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, string.Format(Localization.Get("10000009"), Localization.Get("4381")), Localization.Get("1001"));
						}
					}
					else
					{
						GameSetting.instance.repeatBattle = false;
					}
				}
				else
				{
					GameSetting.instance.repeatBattle = false;
					UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, Localization.Get("10000013"), Localization.Get("1001"));
				}
			}
			UISetter.SetActive(repeatRoot, GameSetting.instance.repeatBattle);
			UISetter.SetLabel(repeatTime, 3);
			UISetter.SetActive(_parent.commanderListView, active: true);
			UISetter.SetActive(winBattleInfoView, active: true);
			UISetter.SetActive(buttonsRoot, active: true);
			if (_parent._isWin && (_parent._battleType == EBattleType.Plunder || _parent._battleType == EBattleType.EventBattle))
			{
				UISetter.SetActive(_parent.clearRankView, active: true);
				yield return _parent.StartCoroutine(ShowClearRank(rank));
			}
			UpdateCommander();
			_parent._canClick = true;
			if (rewardList.itemList == null || rewardList.itemList.Count <= 0)
			{
				UISetter.SetActive(noRewardItems, active: true);
			}
			else
			{
				yield return _parent.StartCoroutine(_PlayRewardAnimation());
			}
			if (base.localUser.statistics.vipShopisFloating)
			{
				RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.VipShop);
				NetworkAnimation.Instance.CreateFloatingText_OnlyUIToast(Localization.Get("22007"), roBuilding.reg.resourceId);
				base.localUser.statistics.vipShopisFloating = false;
			}
			if (GameSetting.instance.repeatBattle)
			{
				yield return new WaitForSeconds(1f);
				UISetter.SetLabel(repeatTime, 2);
				yield return new WaitForSeconds(1f);
				UISetter.SetLabel(repeatTime, 1);
				yield return new WaitForSeconds(1f);
				_parent._battleData.RefreshAttackerTroop(removeMercenary: true);
				if (_parent._battleType == EBattleType.Plunder)
				{
					base.network.RequestPlunder(_parent._battleData);
				}
				else if (_parent._battleType == EBattleType.Guerrilla)
				{
					base.network.RequestSituationSweepStart(_parent._battleData);
				}
				else if (_parent._battleType == EBattleType.EventBattle)
				{
					base.network.RequestEventBattle(_parent._battleData);
				}
				UISetter.SetActive(repeatRoot, active: false);
			}
		}

		public void Init()
		{
			_parent = base.parentPanelBase as UIBattleResult;
			switch (_parent._battleType)
			{
			case EBattleType.Plunder:
			{
				UISetter.SetActive(btnWorldMap, active: true);
				UISetter.SetActive(btnCamp, active: true);
				UISetter.SetActive(btnSituation, active: false);
				UISetter.SetActive(btnAnnihilation, active: false);
				UISetter.SetActive(btnNextAnnihilation, active: false);
				UISetter.SetActive(btnWaveBattle, active: false);
				bool active = false;
				int num = int.Parse(_parent._battleData.stageId);
				int num2 = int.Parse(_parent._battleData.worldId);
				if (num2 > 0)
				{
					if (num < base.localUser.lastClearStage)
					{
						active = true;
					}
					else if (num == base.localUser.lastClearStage)
					{
						active = (num - ConstValue.tutorialMaximumStage) % 20 != 0;
					}
				}
				UISetter.SetActive(btnNextStage, active);
				UISetter.SetActive(btnEventBattle, active: false);
				UISetter.SetActive(btnEventBattleRetry, active: false);
				UISetter.SetActive(expView, active: true);
				UISetter.SetActive(goldView, active: true);
				break;
			}
			case EBattleType.Guerrilla:
			case EBattleType.SeaRobber:
				UISetter.SetActive(btnWorldMap, active: false);
				UISetter.SetActive(btnCamp, active: false);
				UISetter.SetActive(btnSituation, active: true);
				UISetter.SetActive(btnAnnihilation, active: false);
				UISetter.SetActive(btnNextAnnihilation, active: false);
				UISetter.SetActive(btnWaveBattle, active: false);
				UISetter.SetActive(btnEventBattle, active: false);
				UISetter.SetActive(btnEventBattleRetry, active: false);
				UISetter.SetActive(expView, active: false);
				UISetter.SetActive(goldView, active: false);
				break;
			case EBattleType.Annihilation:
				UISetter.SetActive(btnWorldMap, active: false);
				UISetter.SetActive(btnCamp, active: false);
				UISetter.SetActive(btnSituation, active: false);
				UISetter.SetActive(btnAnnihilation, active: true);
				UISetter.SetActive(btnWaveBattle, active: false);
				UISetter.SetActive(btnEventBattle, active: false);
				UISetter.SetActive(btnEventBattleRetry, active: false);
				UISetter.SetActive(expView, active: true);
				UISetter.SetActive(goldView, active: true);
				break;
			case EBattleType.WaveBattle:
				UISetter.SetActive(btnWorldMap, active: false);
				UISetter.SetActive(btnCamp, active: false);
				UISetter.SetActive(btnSituation, active: false);
				UISetter.SetActive(btnAnnihilation, active: false);
				UISetter.SetActive(btnNextAnnihilation, active: false);
				UISetter.SetActive(btnWaveBattle, active: true);
				UISetter.SetActive(btnEventBattle, active: false);
				UISetter.SetActive(btnEventBattleRetry, active: false);
				UISetter.SetActive(expView, active: false);
				UISetter.SetActive(goldView, active: true);
				break;
			case EBattleType.EventBattle:
				UISetter.SetActive(btnWorldMap, active: false);
				UISetter.SetActive(btnCamp, active: true);
				UISetter.SetActive(btnSituation, active: false);
				UISetter.SetActive(btnAnnihilation, active: false);
				UISetter.SetActive(btnNextAnnihilation, active: false);
				UISetter.SetActive(btnWaveBattle, active: false);
				UISetter.SetActive(btnEventBattle, active: true);
				UISetter.SetActive(btnEventBattleRetry, !_parent._isWin);
				UISetter.SetActive(expView, active: true);
				UISetter.SetActive(goldView, active: true);
				break;
			}
			if (_parent._isWin)
			{
				UISetter.SetActive(winView, active: true);
				UISetter.SetActive(loseView, active: false);
				if (_parent._battleType == EBattleType.Guerrilla)
				{
					UISetter.SetSprite(title, "result00_s_win");
				}
				else if (_parent._battleType == EBattleType.Annihilation)
				{
					UISetter.SetSprite(title, "ab-result-clear");
				}
				else
				{
					UISetter.SetSprite(title, "result00_w_win01");
				}
			}
			else
			{
				UISetter.SetActive(winView, active: false);
				UISetter.SetActive(loseView, active: true);
				if (_parent._battleType == EBattleType.Guerrilla)
				{
					UISetter.SetSprite(title, "result00_s_lose");
				}
				else if (_parent._battleType == EBattleType.Annihilation)
				{
					UISetter.SetSprite(title, "ab-result-fail");
				}
				else
				{
					UISetter.SetSprite(title, "result00_w_lose");
				}
			}
			if (_parent._battleType == EBattleType.WaveBattle)
			{
				UISetter.SetActive(winView, active: true);
				UISetter.SetActive(loseView, active: false);
				UISetter.SetSprite(title, "battlefields-results");
			}
			_parent.StartCoroutine(_PlayAnimation());
		}

		public void SetClearRank(int _rank)
		{
			rank = _rank;
			for (int i = 0; i < clearRankStar.Count; i++)
			{
				UISetter.SetActive(clearRankStar[i], active: false);
			}
		}

		private IEnumerator ShowClearRank(int rank)
		{
			for (int i = 0; i < clearRankStar.Count; i++)
			{
				if (i < rank)
				{
					UISetter.SetActive(clearRankStar[i], active: true);
				}
				else
				{
					UISetter.SetActive(clearRankStar[i], active: false);
				}
				yield return new WaitForSeconds(0.25f);
			}
		}

		public void SetUpdataCommanderData(Dictionary<string, Protocols.UserInformationResponse.Commander> commanderList)
		{
			updateCommanderList = commanderList;
		}

		private void UpdateCommander()
		{
			if (updateCommanderList == null || updateCommanderList.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<string, Protocols.UserInformationResponse.Commander> updateCommander in updateCommanderList)
			{
				UIBattleUnitStatus uIBattleUnitStatus = _parent.commanderListView.FindItem(updateCommander.Key) as UIBattleUnitStatus;
				if (uIBattleUnitStatus != null)
				{
					uIBattleUnitStatus.SetUpdateData(updateCommander.Value);
				}
			}
		}

		public void SetBattleTime(float totalSec)
		{
			UISetter.SetLabel(battleTime, Utility.GetTimeStringColonFormat(totalSec));
		}

		public void SetGetExp(int exp)
		{
			UISetter.SetLabel(this.exp, (!base.localUser.isMaxLevel) ? exp.ToString("N0") : "MAX");
			UISetter.SetColor(this.exp, (!base.localUser.isMaxLevel) ? Color.white : Color.red);
		}

		public void SetFavor(List<Protocols.FavorUpData.CommanderFavor> commanderFavor, WorldMapStageDataRow dr)
		{
			if (commanderFavor == null)
			{
			}
		}

		public void SetRewardDataAndOpen(List<Protocols.RewardInfo.RewardData> rewardList)
		{
			if (rewardList != null && rewardList.Count != 0)
			{
				rewardList.RemoveAll((Protocols.RewardInfo.RewardData row) => row.rewardType == ERewardType.Favor);
				Protocols.RewardInfo.RewardData rewardData = rewardList.Find((Protocols.RewardInfo.RewardData row) => row.rewardType == ERewardType.Goods && row.rewardId == "4");
				if (rewardData != null)
				{
					UISetter.SetLabel(gold, rewardData.rewardCnt.ToString("N0"));
					rewardList.Remove(rewardData);
				}
				this.rewardList.InitRewardList(rewardList);
				for (int i = 0; i < this.rewardList.itemList.Count; i++)
				{
					UIGoods mb = this.rewardList.itemList[i] as UIGoods;
					UISetter.SetActive(mb, active: false);
				}
			}
		}

		public IEnumerator _PlayRewardAnimation()
		{
			if (rewardList.itemList == null)
			{
				yield break;
			}
			for (int idx = 0; idx < rewardList.itemList.Count; idx++)
			{
				UIGoods reward = rewardList.itemList[idx] as UIGoods;
				UISetter.SetActive(reward, active: true);
				reward.openAnimation.Play(RewardAnimationName);
				while (reward.openAnimation.IsPlaying(RewardAnimationName))
				{
					yield return null;
				}
			}
		}
	}

	[Serializable]
	public class RaidResult : UIInnerPartBase
	{
		public UISprite title;

		public Animation winTitleAnimation;

		public Animation loseTitleAnimation;

		public UILabel battleTime;

		public UILabel rank;

		public UILabel rankPersent;

		public UILabel baseScore;

		public UILabel getScore;

		public UILabel curScore;

		public GameObject baseInvalidation;

		public GameObject curInvalidation;

		private UIBattleResult _parent;

		public void Init()
		{
			_parent = base.parentPanelBase as UIBattleResult;
			_parent.StartCoroutine(_PlayAnimation());
			UISetter.SetSprite(title, "result00_rade_result");
		}

		private IEnumerator _PlayAnimation()
		{
			UISetter.SetActive(root, active: false);
			UISetter.SetActive(_parent.commanderListView, active: false);
			Animation targetAni = ((!_parent._isWin) ? loseTitleAnimation : winTitleAnimation);
			if (targetAni != null)
			{
				while (targetAni.isPlaying)
				{
					yield return null;
				}
			}
			UISetter.SetActive(root, active: true);
			UISetter.SetActive(_parent.commanderListView, active: true);
			if (base.localUser.statistics.vipShopisFloating)
			{
				RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.VipShop);
				NetworkAnimation.Instance.CreateFloatingText_OnlyUIToast(Localization.Get("22007"), roBuilding.reg.resourceId);
				base.localUser.statistics.vipShopisFloating = false;
			}
		}

		public void SetBattleTime(float totalSec)
		{
			UISetter.SetLabel(battleTime, Utility.GetTimeStringColonFormat(totalSec));
		}

		public void SetRank(int rank)
		{
			string text = string.Format(Localization.Get("5460"), rank.ToString("N0"));
			UISetter.SetLabel(this.rank, text);
		}

		public void SetRankPersent(float persent)
		{
			string text = $"({persent}%)";
			UISetter.SetLabel(rankPersent, text);
		}

		public void SetBaseScore(int baseScore)
		{
			string text = string.Format(Localization.Get("18015"), baseScore.ToString("N0"));
			UISetter.SetLabel(this.baseScore, text);
		}

		public void SetScore(int baseScore, int curScore)
		{
			if (baseScore > curScore)
			{
				UISetter.SetColor(this.curScore, new Color(0.97f, 0.25f, 0.25f));
				UISetter.SetActive(baseInvalidation, active: false);
				UISetter.SetActive(curInvalidation, active: true);
			}
			else
			{
				UISetter.SetColor(this.curScore, new Color(1f, 0.9f, 0f));
				UISetter.SetActive(baseInvalidation, active: true);
				UISetter.SetActive(curInvalidation, active: false);
			}
			UISetter.SetLabel(this.baseScore, baseScore);
			UISetter.SetLabel(this.curScore, curScore);
		}

		public void SetGetScore(int getSocre)
		{
			string text = string.Format(Localization.Get("18015"), getSocre.ToString("N0"));
			UISetter.SetLabel(getScore, text);
		}

		public void SetCurScore(int curScore)
		{
			string text = string.Format(Localization.Get("18015"), curScore.ToString("N0"));
			UISetter.SetLabel(this.curScore, text);
		}
	}

	[Serializable]
	public class ScrambleResult : UIInnerPartBase
	{
		public UISprite title;

		public UITroop myTroop;

		public UITroop enemyTroop;

		public UILabel battleTime;

		private UIBattleResult _parent;

		public void Init()
		{
			_parent = base.parentPanelBase as UIBattleResult;
			_parent._battleData.attackerTroop.FromBattleTroop(UIManager.instance.battle.Simulator.record.result.lhsTroops[0]);
			_parent._battleData.defenderTroop.FromBattleTroop(UIManager.instance.battle.Simulator.record.result.rhsTroops[0]);
			myTroop.Set(_parent._battleData.attackerTroop);
			enemyTroop.Set(_parent._battleData.defenderTroop);
			if (_parent._battleData.isWin)
			{
				UISetter.SetSprite(title, "result_text_victory");
			}
			else
			{
				UISetter.SetSprite(title, "result_text_defeat");
			}
		}

		public void SetBattleTime(float totalSec)
		{
			UISetter.SetLabel(battleTime, Utility.GetTimeStringColonFormat(totalSec));
		}
	}

	[Serializable]
	public class CooperateBattleResult : UIInnerPartBase
	{
		public UISprite title;

		public Animation winTitleAnimation;

		public Animation loseTitleAnimation;

		public UILabel damage;

		private UIBattleResult _parent;

		public void Init()
		{
			_parent = base.parentPanelBase as UIBattleResult;
			_parent.StartCoroutine(_PlayAnimation());
			UISetter.SetSprite(title, "result00_rade_result");
		}

		public void SetDamage(long dmg)
		{
			UISetter.SetLabel(damage, dmg);
		}

		private IEnumerator _PlayAnimation()
		{
			UISetter.SetActive(root, active: false);
			UISetter.SetActive(_parent.commanderListView, active: false);
			Animation targetAni = ((!_parent._isWin) ? loseTitleAnimation : winTitleAnimation);
			if (targetAni != null)
			{
				while (targetAni.isPlaying)
				{
					yield return null;
				}
			}
			UISetter.SetActive(root, active: true);
			UISetter.SetActive(_parent.commanderListView, active: true);
			if (base.localUser.statistics.vipShopisFloating)
			{
				RoBuilding roBuilding = base.localUser.FindBuilding(EBuilding.VipShop);
				NetworkAnimation.Instance.CreateFloatingText_OnlyUIToast(Localization.Get("22007"), roBuilding.reg.resourceId);
				base.localUser.statistics.vipShopisFloating = false;
			}
		}
	}

	[Serializable]
	public class EventRaidResult : UIInnerPartBase
	{
		public UISprite title;

		public Animation winTitleAnimation;

		public Animation loseTitleAnimation;

		public GameObject btnCamp;

		public GameObject btnEventRaid;

		public GameObject btnEventRaidRetry;

		public UIDefaultListView rewardList;

		public GameObject noRewardItems;

		public GameObject lastAttack;

		public UILabel baseDamage;

		public UILabel curDamage;

		public UILabel totalDamage;

		private UIBattleResult _parent;

		public void Init()
		{
			_parent = base.parentPanelBase as UIBattleResult;
			UISetter.SetActive(lastAttack, active: false);
			UISetter.SetSprite(title, (!_parent._isWin) ? "me-results-fail" : "me-results-success");
			UISetter.SetActive(btnCamp, active: true);
			UISetter.SetActive(btnEventRaid, active: true);
			UISetter.SetActive(btnEventRaidRetry, !_parent._isWin);
			_parent.StartCoroutine(_PlayAnimation());
		}

		private IEnumerator _PlayAnimation()
		{
			UISetter.SetActive(root, active: false);
			UISetter.SetActive(_parent.commanderListView, active: false);
			Animation targetAni = ((!_parent._isWin) ? loseTitleAnimation : winTitleAnimation);
			if (targetAni != null)
			{
				while (targetAni.isPlaying)
				{
					yield return null;
				}
			}
			UISetter.SetActive(root, active: true);
			UISetter.SetActive(_parent.commanderListView, active: true);
			if (rewardList.itemList == null || rewardList.itemList.Count <= 0)
			{
				UISetter.SetActive(noRewardItems, active: true);
			}
			else
			{
				yield return _parent.StartCoroutine(_PlayRewardAnimation());
			}
		}

		public IEnumerator _PlayRewardAnimation()
		{
			if (rewardList.itemList == null)
			{
				yield break;
			}
			for (int idx = 0; idx < rewardList.itemList.Count; idx++)
			{
				UIGoods reward = rewardList.itemList[idx] as UIGoods;
				UISetter.SetActive(reward, active: true);
				reward.openAnimation.Play("BattleResultPopUp_Win_ItemOpen");
				while (reward.openAnimation.IsPlaying("BattleResultPopUp_Win_ItemOpen"))
				{
					yield return null;
				}
			}
		}

		public void SetRewardData(List<Protocols.RewardInfo.RewardData> rewardList)
		{
			if (rewardList != null && rewardList.Count != 0)
			{
				rewardList.RemoveAll((Protocols.RewardInfo.RewardData row) => row.rewardType == ERewardType.Favor);
				Protocols.RewardInfo.RewardData rewardData = rewardList.Find((Protocols.RewardInfo.RewardData row) => row.rewardType == ERewardType.Goods && row.rewardId == "4");
				if (rewardData != null)
				{
					rewardList.Remove(rewardData);
				}
				this.rewardList.InitRewardList(rewardList);
				for (int i = 0; i < this.rewardList.itemList.Count; i++)
				{
					UIGoods mb = this.rewardList.itemList[i] as UIGoods;
					UISetter.SetActive(mb, active: false);
				}
			}
		}

		public void SetBaseDamage(long baseDamage)
		{
			UISetter.SetLabel(this.baseDamage, baseDamage);
		}

		public void SetCurDamage(long curDamage)
		{
			UISetter.SetLabel(this.curDamage, curDamage);
		}

		public void SetTotalDamage(long totalDamage)
		{
			UISetter.SetLabel(this.totalDamage, totalDamage);
		}
	}

	[Serializable]
	public class InfinityBattleResult : UIInnerPartBase
	{
		[Serializable]
		public class InfinityBattleMission
		{
			public UISprite icon;

			public UIClearConditionItem item;
		}

		private UIBattleResult _parent;

		public UISprite title;

		public Animation winTitleAnimation;

		public Animation loseTitleAnimation;

		public GameObject spineRoot;

		public GameObject battleInfoRoot;

		public GameObject btnRoot;

		public GameObject btnRetry;

		public GameObject btnCamp;

		public GameObject btnInfinityBattle;

		public List<GameObject> disableItems;

		public InfinityBattleMission mission1;

		public InfinityBattleMission mission2;

		public InfinityBattleMission mission3;

		public Protocols.InfinityTowerData data;

		public void Init()
		{
			_parent = base.parentPanelBase as UIBattleResult;
			_parent.StartCoroutine(_PlayAnimation());
			UISetter.SetSprite(title, (!_parent._isWin) ? "result00_w_lose" : "result00_w_win01");
			UISetter.SetActive(btnRetry, !_parent._isWin);
			UISetter.SetActive(btnCamp, active: true);
			UISetter.SetActive(btnInfinityBattle, active: true);
			for (int i = 0; i < disableItems.Count; i++)
			{
				UISetter.SetActive(disableItems[i], active: false);
			}
			InfinityFieldDataRow infinityFieldDataRow = base.regulation.infinityFieldDtbl[_parent._battleData.stageId];
			mission1.item.Set(EBattleClearCondition.None, string.Empty);
			mission2.item.Set(infinityFieldDataRow.clearMission01, infinityFieldDataRow.clearMission01Count);
			mission3.item.Set(infinityFieldDataRow.clearMission02, infinityFieldDataRow.clearMission02Count);
			if (data != null && data.fieldData != null && data.fieldData.ContainsKey(_parent._battleData.stageId))
			{
				if (data.fieldData[_parent._battleData.stageId].ContainsKey(1))
				{
					UISetter.SetSprite(mission1.icon, (data.fieldData[_parent._battleData.stageId][1] <= 0) ? "com_star02" : "com_star01");
				}
				if (data.fieldData[_parent._battleData.stageId].ContainsKey(2))
				{
					UISetter.SetSprite(mission2.icon, (data.fieldData[_parent._battleData.stageId][2] <= 0) ? "com_star02" : "com_star01");
				}
				if (data.fieldData[_parent._battleData.stageId].ContainsKey(3))
				{
					UISetter.SetSprite(mission3.icon, (data.fieldData[_parent._battleData.stageId][3] <= 0) ? "com_star02" : "com_star01");
				}
			}
		}

		private IEnumerator _PlayAnimation()
		{
			UISetter.SetActive(btnRoot, active: false);
			UISetter.SetActive(battleInfoRoot, active: false);
			UISetter.SetActive(_parent.commanderListView, active: false);
			Animation targetAni = ((!_parent._isWin) ? loseTitleAnimation : winTitleAnimation);
			if (targetAni != null)
			{
				targetAni.gameObject.SetActive(value: true);
				while (targetAni.isPlaying)
				{
					yield return null;
				}
			}
			UISetter.SetActive(btnRoot, active: true);
			UISetter.SetActive(battleInfoRoot, active: true);
		}
	}

	public UIDefaultListView commanderListView;

	public UISpineAnimation charSpine;

	public GameObject clearRankView;

	public GameObject titleView;

	public DuelResult duelResult;

	public WaveDuelResult waveDuelResult;

	public WorldDuelResult worldDuelResult;

	public PlunderResult plunderResult;

	public RaidResult raidResult;

	public ScrambleResult scrambleResult;

	public CooperateBattleResult coopBattleResult;

	public EventRaidResult eventRaidResult;

	public InfinityBattleResult infinityBattleResult;

	public ICodeBehaviour iCodeBehaviour;

	public Animation resultAnimation;

	public UISprite lastAttack;

	public GameObject winTitleEffect;

	public UILabel WaveCount;

	public int lhsTroopIdx;

	[Range(0f, 10f)]
	public float commanderVoiceDelay;

	private CommanderDataRow commanderDr;

	private BattleData _battleData;

	private bool _isWin;

	private EBattleType _battleType;

	private bool _isFatal;

	private Result _battleResult;

	[SerializeField]
	private GameObject battleStatBtn;

	[SerializeField]
	private GameObject battleStatLeftView;

	[SerializeField]
	private GameObject BattleStatRecord;

	[SerializeField]
	private UIDefaultListView RecordCommanderListView;

	[SerializeField]
	private UIBattleStatisticRecord firstRecord;

	[SerializeField]
	private UIFlipSwitch AttackTab;

	[SerializeField]
	private UIFlipSwitch AvoidTab;

	[SerializeField]
	private UIFlipSwitch RecoverTab;

	private bool _canClick = true;

	private IEnumerator Tutorial()
	{
		yield return null;
		iCodeBehaviour.SendEvent("Start", null);
	}

	private new void OnEnable()
	{
		iCodeBehaviour.gameObject.SetActive(value: true);
		UISetter.SetActive(titleView, active: false);
	}

	private void OnDestroy()
	{
		iCodeBehaviour = null;
	}

	public override void OnClick(GameObject sender)
	{
		if (_canClick)
		{
			if (GameSetting.instance.repeatBattle)
			{
				StopAllCoroutines();
				GameSetting.instance.repeatBattle = false;
			}
			base.OnClick(sender);
		}
	}

	public void Set(Simulator simul, Regulation reg)
	{
		StopAllCoroutines();
		_isWin = simul.result.IsWin;
		_battleType = simul.initState.battleType;
		UISetter.SetActive(clearRankView, active: false);
		UISetter.SetActive(duelResult, _battleType == EBattleType.Duel);
		UISetter.SetActive(plunderResult, _battleType == EBattleType.Plunder || _battleType == EBattleType.SeaRobber || _battleType == EBattleType.Guerrilla || _battleType == EBattleType.WaveBattle);
		UISetter.SetActive(infinityBattleResult, _battleType == EBattleType.InfinityBattle);
		UISetter.SetActive(raidResult, _battleType == EBattleType.Raid);
		UISetter.SetActive(scrambleResult, _battleType == EBattleType.GuildScramble);
		UISetter.SetActive(WaveCount, _battleType == EBattleType.WaveBattle);
		UISetter.SetActive(root.gameObject, active: true);
		SendOnInitToInnerParts();
		if (_isWin)
		{
			SoundManager.PlaySFX("SE_Win_001");
		}
		else
		{
			SoundManager.PlaySFX("SE_Loes_001");
		}
		if (charSpine != null)
		{
			string text = "a_01_idle1";
			text = ((!_isWin) ? "a_05_lose" : ((!_isFatal) ? "e_04_pleasure" : "a_05_lose"));
			charSpine.skeletonAnimation.state.SetAnimation(0, text, loop: true);
			SetLastAttackImg(_isWin);
			if (charSpine.target != null)
			{
				UIInteraction component = charSpine.target.GetComponent<UIInteraction>();
				if (component != null)
				{
					component.EnableInteration = false;
					component.enabled = false;
				}
			}
		}
		SetTroop(simul.lhsTroops[0]);
		switch (_battleType)
		{
		case EBattleType.Duel:
			duelResult.Init();
			break;
		case EBattleType.GuildScramble:
			scrambleResult.Init();
			break;
		case EBattleType.Raid:
			raidResult.Init();
			break;
		case EBattleType.Guerrilla:
		case EBattleType.WaveBattle:
			plunderResult.Init();
			break;
		case EBattleType.Plunder:
		case EBattleType.SeaRobber:
			plunderResult.SetClearRank(simul.record.result.clearRank);
			plunderResult.Init();
			break;
		case EBattleType.InfinityBattle:
			infinityBattleResult.Init();
			break;
		}
		StartCoroutine("Tutorial");
	}

	public void SetMainCommanderUnit(Unit unit)
	{
		if (unit == null)
		{
			return;
		}
		Regulation regulation = RemoteObjectManager.instance.regulation;
		commanderDr = regulation.commanderDtbl[unit._cdri];
		RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
		if (charSpine != null)
		{
			string skin = "1";
			if (unit._ctdri >= 0)
			{
				RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommander(unit.cid);
				if (roCommander != null && roCommander.isBasicCostume && (unit._charType != ECharacterType.Mercenary || unit._charType != ECharacterType.SuperMercenary))
				{
					skin = roCommander.currentViewCostume;
				}
				else
				{
					CommanderCostumeDataRow commanderCostumeDataRow = regulation.commanderCostumeDtbl[unit._ctdri];
					skin = commanderCostumeDataRow.skinName;
				}
			}
			charSpine.spinePrefabName = commanderDr.resourceId;
			charSpine.SetSkin(skin);
		}
		_isFatal = unit._bFatal;
	}

	public void SetTroop(Troop troop)
	{
		if (commanderListView == null)
		{
			return;
		}
		List<Troop.Slot> list = troop._slots.FindAll((Troop.Slot x) => x != null && !string.IsNullOrEmpty(x.id));
		commanderListView.ResizeItemList(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Troop.Slot slot = list[i];
			UIBattleUnitStatus uIBattleUnitStatus = commanderListView.itemList[i] as UIBattleUnitStatus;
			if (!(uIBattleUnitStatus == null))
			{
				uIBattleUnitStatus.Set(slot);
				UISetter.SetGameObjectName(uIBattleUnitStatus.gameObject, slot.cid);
				uIBattleUnitStatus.SetSelection(selected: false);
				uIBattleUnitStatus.statusView.SetEnable(enable: false);
				uIBattleUnitStatus.commanderView.SetEnable(enable: true);
				uIBattleUnitStatus.expView.SetEnable(enable: true);
			}
		}
	}

	public void SetTroop(RoTroop troop)
	{
		if (commanderListView == null)
		{
			return;
		}
		List<RoTroop.Slot> list = new List<RoTroop.Slot>();
		for (int i = 0; i < troop.slots.Length; i++)
		{
			RoTroop.Slot slot = troop.slots[i];
			if (slot.position >= 0)
			{
				int num = list.FindIndex((RoTroop.Slot x) => slot.position <= x.position);
				if (num < 0)
				{
					list.Add(slot);
				}
				else
				{
					list.Insert(num, slot);
				}
			}
		}
		commanderListView.ResizeItemList(list.Count);
		for (int j = 0; j < list.Count; j++)
		{
			RoTroop.Slot slot2 = list[j];
			UIBattleUnitStatus uIBattleUnitStatus = commanderListView.itemList[j] as UIBattleUnitStatus;
			if (!(uIBattleUnitStatus == null))
			{
				uIBattleUnitStatus.Set(slot2);
				UISetter.SetGameObjectName(uIBattleUnitStatus.gameObject, slot2.commanderId);
				uIBattleUnitStatus.SetSelection(selected: false);
				if (_battleType == EBattleType.Annihilation)
				{
					uIBattleUnitStatus.statusView.SetEnable(enable: true);
					uIBattleUnitStatus.commanderView.SetEnable(enable: true);
					uIBattleUnitStatus.expView.SetEnable(enable: false);
				}
				else
				{
					uIBattleUnitStatus.statusView.SetEnable(enable: false);
					uIBattleUnitStatus.commanderView.SetEnable(enable: true);
					uIBattleUnitStatus.expView.SetEnable(enable: true);
				}
			}
		}
	}

	public new void Open()
	{
		if (_battleData == null)
		{
			return;
		}
		StopAllCoroutines();
		_isWin = _battleData.isWin;
		_battleType = _battleData.type;
		UISetter.SetActive(clearRankView, active: false);
		UISetter.SetActive(duelResult, _battleType == EBattleType.Duel);
		UISetter.SetActive(waveDuelResult, _battleType == EBattleType.WaveDuel);
		UISetter.SetActive(worldDuelResult, _battleType == EBattleType.WorldDuel);
		UISetter.SetActive(plunderResult, _battleType == EBattleType.Plunder || _battleType == EBattleType.SeaRobber || _battleType == EBattleType.Guerrilla || _battleType == EBattleType.Annihilation || _battleType == EBattleType.EventBattle || _battleType == EBattleType.WaveBattle);
		UISetter.SetActive(infinityBattleResult, _battleType == EBattleType.InfinityBattle);
		UISetter.SetActive(raidResult, _battleType == EBattleType.Raid);
		UISetter.SetActive(scrambleResult, _battleType == EBattleType.GuildScramble);
		UISetter.SetActive(coopBattleResult, _battleType == EBattleType.CooperateBattle);
		UISetter.SetActive(WaveCount, _battleType == EBattleType.WaveBattle);
		UISetter.SetActive(battleStatBtn, _battleType == EBattleType.EventRaid || _battleType == EBattleType.Raid);
		UISetter.SetActive(battleStatLeftView, _battleType != EBattleType.EventRaid);
		UISetter.SetActive(BattleStatRecord, active: false);
		UISetter.SetActive(root.gameObject, active: true);
		SendOnInitToInnerParts();
		if (charSpine != null)
		{
			string text = "a_01_idle1";
			text = ((!_isWin) ? "a_05_lose" : ((!_isFatal) ? "e_04_pleasure" : "a_05_lose"));
			charSpine.skeletonAnimation.state.SetAnimation(0, text, loop: true);
			SetLastAttackImg(_isWin);
			if (charSpine.target != null)
			{
				UIInteraction component = charSpine.target.GetComponent<UIInteraction>();
				if (component != null)
				{
					component.EnableInteration = false;
					component.enabled = false;
				}
			}
		}
		if (_battleType == EBattleType.Annihilation)
		{
			_battleData.attackerTroop.FromBattleTroop(UIManager.instance.battle.Simulator.record.result.lhsTroops[0]);
		}
		if (_battleType == EBattleType.WaveDuel)
		{
			SetTroop(_battleData.attacker.battleTroopList[lhsTroopIdx]);
		}
		else
		{
			SetTroop(_battleData.attackerTroop);
		}
		switch (_battleType)
		{
		case EBattleType.Duel:
			UISetter.SetActive(titleView, active: true);
			duelResult.Init();
			break;
		case EBattleType.WaveDuel:
			UISetter.SetActive(titleView, active: true);
			waveDuelResult.Init();
			break;
		case EBattleType.WorldDuel:
			UISetter.SetActive(titleView, active: true);
			worldDuelResult.Init();
			break;
		case EBattleType.GuildScramble:
			UISetter.SetActive(titleView, active: true);
			scrambleResult.Init();
			break;
		case EBattleType.Raid:
			UISetter.SetActive(titleView, active: true);
			raidResult.Init();
			break;
		case EBattleType.Annihilation:
		case EBattleType.Guerrilla:
		case EBattleType.WaveBattle:
			UISetter.SetActive(titleView, active: true);
			plunderResult.Init();
			break;
		case EBattleType.Plunder:
		case EBattleType.SeaRobber:
		case EBattleType.EventBattle:
			if (!_isWin)
			{
				UISetter.SetActive(titleView, active: true);
			}
			plunderResult.SetClearRank(_battleData.clearRank);
			plunderResult.Init();
			break;
		case EBattleType.CooperateBattle:
			UISetter.SetActive(titleView, active: true);
			coopBattleResult.Init();
			break;
		case EBattleType.EventRaid:
			UISetter.SetActive(titleView, active: true);
			eventRaidResult.Init();
			break;
		case EBattleType.InfinityBattle:
			if (!_isWin)
			{
				UISetter.SetActive(titleView, active: true);
			}
			infinityBattleResult.Init();
			break;
		}
		if (_isWin)
		{
			SoundManager.PlaySFX("SE_Win_001");
			if (_battleType != EBattleType.Plunder)
			{
				UISetter.SetActive(winTitleEffect, active: true);
			}
		}
		else
		{
			SoundManager.PlaySFX("SE_Loes_001");
			UISetter.SetActive(winTitleEffect, active: false);
		}
		StartCoroutine("EffCommanderVoice");
		StartCoroutine("Tutorial");
	}

	public void Set(BattleData battleData)
	{
		if (battleData != null)
		{
			_battleData = battleData;
		}
	}

	public void SetBattleResult(Result battleResult)
	{
		_battleResult = battleResult;
	}

	private IEnumerator EffCommanderVoice()
	{
		yield return new WaitForSeconds(commanderVoiceDelay);
		if (_isWin)
		{
			if (_isFatal)
			{
				SoundManager.PlayVoiceEvent(commanderDr, ECommanderVoiceEventType.WinFatal);
			}
			else
			{
				SoundManager.PlayVoiceEvent(commanderDr, ECommanderVoiceEventType.Win);
			}
		}
		else
		{
			SoundManager.PlayVoiceEvent(commanderDr, ECommanderVoiceEventType.Lose);
		}
	}

	public void SetInvalidationClicked()
	{
		OpenVipRechargePopUp(EVipRechargeType.ResetStackScore);
	}

	private void SetLastAttackImg(bool isWin)
	{
		UISetter.SetSprite(lastAttack, (!isWin) ? "result00_last_destroyed_font" : "result00_last_attack_font");
	}

	public void OpenVipRechargePopUp(EVipRechargeType type)
	{
		int num = (int)type;
		VipRechargeDataRow vipRechargeDataRow = base.regulation.vipRechargeDtbl[num.ToString()];
		int num2 = 0;
		int vipLevel = base.localUser.vipLevel;
		int idx = base.regulation.vipExpDtbl[base.regulation.vipExpDtbl.length - 1].Idx;
		int maxRechargeCount = vipRechargeDataRow.GetMaxRechargeCount(vipLevel);
		if (base.localUser.resourceRechargeList.ContainsKey(num.ToString()))
		{
			num2 = base.localUser.resourceRechargeList[num.ToString()];
		}
		int num3 = maxRechargeCount - num2;
		if (base.localUser.vipLevel == idx && num3 == 0)
		{
			UISimplePopup.CreateOK(localization: true, "5635", "12014", null, "1001");
		}
		else if (base.localUser.vipLevel < vipRechargeDataRow.startVip || num3 == 0)
		{
			UISimplePopup.CreateOK(localization: true, "5635", "12015", null, "1001");
		}
		else
		{
			UIPopup.Create<UIResourcePurchasePopup>("ResourcePurchasePopup").initData(vipRechargeDataRow);
		}
	}

	public void SetBattleStatisticRecord(EBattleRecordType type)
	{
		Troop troop = UIManager.instance.battle.Simulator.record.result.lhsTroops[0];
		if (troop == null)
		{
			return;
		}
		List<Troop.Slot> list = new List<Troop.Slot>();
		for (int i = 0; i < troop.slots.Count; i++)
		{
			if (troop.slots[i] != null)
			{
				list.Add(troop.slots[i]);
			}
		}
		switch (type)
		{
		case EBattleRecordType.Attack:
			list.Sort(delegate(Troop.Slot sort_1, Troop.Slot sort_2)
			{
				if (sort_1.statsAttack > sort_2.statsAttack)
				{
					return -1;
				}
				return (sort_1.statsAttack != sort_2.statsAttack) ? 1 : 0;
			});
			break;
		case EBattleRecordType.Avoid:
			list.Sort(delegate(Troop.Slot sort_1, Troop.Slot sort_2)
			{
				if (sort_1.statsDefense > sort_2.statsDefense)
				{
					return -1;
				}
				return (sort_1.statsDefense != sort_2.statsDefense) ? 1 : 0;
			});
			break;
		case EBattleRecordType.Recover:
			list.Sort(delegate(Troop.Slot sort_1, Troop.Slot sort_2)
			{
				if (sort_1.statsHealing > sort_2.statsHealing)
				{
					return -1;
				}
				return (sort_1.statsHealing != sort_2.statsHealing) ? 1 : 0;
			});
			break;
		}
		firstRecord.TopRankSet(list[0], type);
		RecordCommanderListView.ResizeItemList(list.Count - 1);
		for (int j = 1; j < list.Count; j++)
		{
			UIBattleStatisticRecord uIBattleStatisticRecord = RecordCommanderListView.itemList[j - 1] as UIBattleStatisticRecord;
			if (!(uIBattleStatisticRecord == null))
			{
				uIBattleStatisticRecord.Set(list[j], type);
				UISetter.SetGameObjectName(uIBattleStatisticRecord.gameObject, list[j].cid);
				uIBattleStatisticRecord.SetSelection(selected: false);
			}
		}
	}

	public void OpenBattleStatRecord()
	{
		UISetter.SetActive(BattleStatRecord, active: true);
		UISetter.SetFlipSwitch(AttackTab, state: true);
		UISetter.SetFlipSwitch(AvoidTab, state: false);
		UISetter.SetFlipSwitch(RecoverTab, state: false);
		SetBattleStatisticRecord(EBattleRecordType.Attack);
	}

	public void CloseBattleStatRecord()
	{
		UISetter.SetActive(BattleStatRecord, active: false);
	}

	public void TabClick(GameObject sender)
	{
		EBattleRecordType battleStatisticRecord = EBattleRecordType.None;
		switch (sender.name)
		{
		case "Tab-1":
			battleStatisticRecord = EBattleRecordType.Attack;
			break;
		case "Tab-2":
			battleStatisticRecord = EBattleRecordType.Avoid;
			break;
		case "Tab-3":
			battleStatisticRecord = EBattleRecordType.Recover;
			break;
		}
		SetBattleStatisticRecord(battleStatisticRecord);
	}
}
