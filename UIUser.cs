using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIUser : UIItemBase
{
	public UILabel nickname;

	public UILabel uno;

	public UISprite duelRankingMark;

	public UISprite duelRankingGrade;

	public UILabel duelGradeNum;

	public UISprite raidRankingMark;

	public UISprite raidRankingGrade;

	public GameObject duelRoot;

	public UILabel duelRanking;

	public UILabel bestDuelRanking;

	public UILabel duelScore;

	public GameObject raidRoot;

	public UILabel raidRanking;

	public UILabel raidScore;

	public UISprite picture;

	public UILabel vipLevel;

	public UILabel level;

	public UILabel dotLevel;

	public UILabel exp;

	public UIProgressBar expProgress;

	public UILabel gold;

	public UILabel cash;

	public UICommander mainCommander;

	public UITroop mainTroop;

	public UILabel explorationTicket;

	public UILabel sweepTicket;

	public UICommanderTag commanderTag;

	public UILabel power;

	public UILabel guildName;

	private RoUser _currUser;

	private RoLocalUser _currLocalUser;

	public UISprite result;

	public override void Refresh()
	{
		if (_currLocalUser != null)
		{
			Set(_currLocalUser);
		}
		else
		{
			Set(_currUser);
		}
	}

	public void Set(string name)
	{
		UISetter.SetLabel(nickname, name);
	}

	public void Set(RoLocalUser user)
	{
		if (user != null)
		{
			_currLocalUser = user;
			Set((RoUser)user);
			Regulation regulation = RemoteObjectManager.instance.regulation;
			UserLevelDataRow userLevelDataRow = regulation.GetUserLevelDataRow(user.level);
			if (userLevelDataRow != null)
			{
				UserLevelDataRow userLevelDataRow2 = regulation.GetUserLevelDataRow(user.level + 1);
				int num = userLevelDataRow.exp;
				int num2 = 0;
				num2 = userLevelDataRow2?.exp ?? num;
				int num3 = user.exp - userLevelDataRow.uExp;
				UISetter.SetLabel(exp, (!user.isMaxLevel) ? $"{num3} / {num2}" : "M A X");
				UISetter.SetColor(exp, (!user.isMaxLevel) ? Color.white : Color.red);
				UISetter.SetProgress(expProgress, (!user.isMaxLevel) ? ((float)num3 / (float)num2) : 1f);
			}
			UISetter.SetActive(uno, active: false);
			UISetter.SetLabel(gold, user.gold.ToString("N0"));
			UISetter.SetLabel(cash, user.cash.ToString("N0"));
			UISetter.SetLabel(explorationTicket, user.explorationTicket.ToString());
			UISetter.SetLabel(sweepTicket, user.sweepTicket.ToString());
			UISetter.SetLabel(vipLevel, user.vipLevel);
			UISetter.SetLabel(guildName, (!GameSetting.instance.guildName) ? string.Empty : (string.IsNullOrEmpty(user.guildName) ? Localization.Get("7180") : user.guildName));
		}
	}

	public virtual void Set(RoUser user)
	{
		if (user == null)
		{
			return;
		}
		_currUser = user;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetLabel(nickname, user.nickname);
		UISetter.SetLabel(level, user.level);
		UISetter.SetLabel(dotLevel, Localization.Format("1021", user.level));
		UISetter.SetLabel(guildName, (!GameSetting.instance.guildName) ? string.Empty : (string.IsNullOrEmpty(user.guildName) ? Localization.Get("7180") : user.guildName));
		UISetter.SetActive(duelRoot, user.duelScore != 0 || user.worldDuelScore != 0);
		string text = "-";
		if (user.duelRanking != 0)
		{
			text = user.duelRanking.ToString();
		}
		else if (user.worldDuelRanking != 0)
		{
			text = user.worldDuelRanking.ToString();
		}
		else
		{
			UISetter.SetLabel(duelRanking, Localization.Format("5460", text));
			if (user.duelScore != 0)
			{
				UISetter.SetLabel(duelScore, Localization.Format("18015", user.duelScore));
			}
			else if (user.worldDuelScore != 0)
			{
				UISetter.SetLabel(duelScore, Localization.Format("18015", user.worldDuelScore));
			}
		}
		UISetter.SetActive(raidRoot, user.raidScore != 0);
		UISetter.SetLabel(raidRanking, $"{user.raidRanking:N0}");
		UISetter.SetLabel(raidScore, Localization.Format("18015", user.raidScore));
		if (_currUser.thumbnailId != null)
		{
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.FindCostumeData(int.Parse(_currUser.thumbnailId));
			if (commanderCostumeDataRow != null)
			{
				RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommander(commanderCostumeDataRow.cid.ToString());
				if (roCommander != null && roCommander.isBasicCostume)
				{
					UISetter.SetSprite(picture, roCommander.resourceId + "_" + roCommander.currentViewCostume);
				}
				else
				{
					UISetter.SetSprite(picture, regulation.GetCostumeThumbnailName(int.Parse(_currUser.thumbnailId)));
				}
			}
			else
			{
				UISetter.SetSprite(picture, regulation.GetCostumeThumbnailName(int.Parse(_currUser.thumbnailId)));
			}
		}
		if (user.duelRanking != 0)
		{
			if (user.duelRanking < 4)
			{
				UISetter.SetActive(duelRankingMark, active: true);
				UISetter.SetActive(duelRanking, !(duelRankingMark != null));
				UISetter.SetSprite(duelRankingMark, "pvp_ranking_" + user.duelRanking);
			}
			else
			{
				UISetter.SetActive(duelRanking, active: true);
				UISetter.SetActive(duelRankingMark, active: false);
			}
			UISetter.SetSprite(duelRankingGrade, user.GetDuelGradeIcon());
			UISetter.SetLabel(duelRanking, Localization.Format("5460", text));
			UISetter.SetLabel(bestDuelRanking, $"{user.duelRanking:N0}");
			UISetter.SetLabel(duelScore, Localization.Format("18015", user.duelScore));
		}
		if (user.raidRanking != 0)
		{
			if (user.raidRanking < 4)
			{
				UISetter.SetActive(raidRankingMark, active: true);
				UISetter.SetActive(raidRanking, !(raidRankingMark != null));
				UISetter.SetSprite(raidRankingMark, "pvp_ranking_" + user.raidRanking);
			}
			else
			{
				UISetter.SetActive(raidRanking, active: true);
				UISetter.SetActive(raidRankingMark, active: false);
			}
			UISetter.SetSprite(raidRankingGrade, user.GetRaidGradeIcon());
		}
		if (user.worldDuelRanking != 0)
		{
			if (user.worldDuelRanking < 4)
			{
				UISetter.SetActive(duelRankingMark, active: true);
				UISetter.SetActive(duelRanking, !(duelRankingMark != null));
				UISetter.SetSprite(duelRankingMark, "pvp_ranking_" + user.worldDuelRanking);
			}
			else
			{
				UISetter.SetActive(duelRanking, active: true);
				UISetter.SetActive(duelRankingMark, active: false);
			}
			UISetter.SetSprite(duelRankingGrade, user.GetDuelGradeIcon());
			UISetter.SetLabel(duelRanking, Localization.Format("5460", text));
			UISetter.SetLabel(bestDuelRanking, $"{user.worldDuelRanking:N0}");
			UISetter.SetLabel(duelScore, Localization.Format("18015", user.worldDuelScore));
		}
		if (mainTroop != null)
		{
			mainTroop.Set(user.mainTroop);
		}
	}

	public void Set(RoUser user, EBattleType type)
	{
		if (user == null)
		{
			return;
		}
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(duelGradeNum, active: false);
		Set(user);
		if ((type == EBattleType.Duel || type == EBattleType.WorldDuel) && regulation.rankingDtbl.ContainsKey(user.duelGradeIdx.ToString()))
		{
			RankingDataRow data = regulation.rankingDtbl[user.duelGradeIdx.ToString()];
			List<RankingDataRow> list = regulation.rankingDtbl.FindAll((RankingDataRow row) => row.type == data.type && row.icon == data.icon);
			UISetter.SetActive(duelGradeNum, list.Count > 1);
			int num = list.IndexOf(data);
			UISetter.SetLabel(duelGradeNum, num + 1);
		}
	}

	public void SetNpc(RoUser user)
	{
		_currUser = user;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetLabel(nickname, user.nickname);
		UISetter.SetLabel(level, user.level);
		UISetter.SetLabel(dotLevel, Localization.Format("1021", user.level));
		UISetter.SetActive(duelRoot, user.duelScore != 0);
		string text = ((user.duelRanking == 0) ? Localization.Get("17096") : user.duelRanking.ToString());
		UISetter.SetLabel(duelRanking, text);
		UISetter.SetLabel(duelScore, Localization.Format("18015", user.duelScore));
		UISetter.SetActive(raidRoot, user.raidScore != 0);
		UISetter.SetLabel(raidRanking, $"{user.raidRanking:N0}");
		UISetter.SetLabel(raidScore, Localization.Format("18015", user.raidScore));
		UISetter.SetLabel(power, Localization.Format("18004", user.mainTotalPower));
		UISetter.SetLabel(guildName, (!GameSetting.instance.guildName) ? string.Empty : (string.IsNullOrEmpty(user.guildName) ? Localization.Get("7180") : user.guildName));
		if (_currUser.thumbnailId != null)
		{
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.FindCostumeData(int.Parse(_currUser.thumbnailId));
			if (commanderCostumeDataRow != null)
			{
				RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommander(commanderCostumeDataRow.cid.ToString());
				if (roCommander != null && roCommander.isBasicCostume)
				{
					UISetter.SetSprite(picture, roCommander.resourceId + "_" + roCommander.currentViewCostume);
				}
				else
				{
					UISetter.SetSprite(picture, regulation.GetCostumeThumbnailName(int.Parse(_currUser.thumbnailId)));
				}
			}
			else
			{
				UISetter.SetSprite(picture, regulation.GetCostumeThumbnailName(int.Parse(_currUser.thumbnailId)));
			}
		}
		if (user.duelRanking != 0)
		{
			if (user.duelRanking < 4)
			{
				UISetter.SetActive(duelRankingMark, active: true);
				UISetter.SetActive(duelRanking, !(duelRankingMark != null));
				UISetter.SetSprite(duelRankingMark, "pvp_ranking_" + user.duelRanking);
			}
			else
			{
				UISetter.SetActive(duelRanking, active: true);
				UISetter.SetActive(duelRankingMark, active: false);
			}
			UISetter.SetSprite(duelRankingGrade, user.GetDuelGradeIcon());
		}
		if (user.raidRanking != 0)
		{
			if (user.raidRanking < 4)
			{
				UISetter.SetActive(raidRankingMark, active: true);
				UISetter.SetActive(raidRanking, !(raidRankingMark != null));
				UISetter.SetSprite(raidRankingMark, "pvp_ranking_" + user.raidRanking);
			}
			else
			{
				UISetter.SetActive(raidRanking, active: true);
				UISetter.SetActive(raidRankingMark, active: false);
			}
			UISetter.SetSprite(raidRankingGrade, user.GetRaidGradeIcon());
		}
		if (mainTroop != null)
		{
			mainTroop.Set(user.mainTroop);
		}
	}

	public void Set(Protocols.ScrambleStageInfo.UserInfo data)
	{
		UISetter.SetLabel(dotLevel, Localization.Format("1021", data.level));
		UISetter.SetLabel(nickname, data.name);
	}

	public void Set(Protocols.RecordInfo record)
	{
		UISetter.SetLabel(nickname, record.userName);
		UISetter.SetLabel(guildName, (!GameSetting.instance.guildName) ? string.Empty : (string.IsNullOrEmpty(record.guildName) ? Localization.Get("7180") : record.guildName));
		if (mainCommander != null)
		{
			mainCommander.Set(record);
		}
	}
}
