using Shared.Regulation;

public class UIStatus : UIItemBase
{
	public UISprite unitThumbnail;

	public UILabel nickname;

	public UILabel level;

	public UILabel dotLevel;

	public UILabel spaceLevel;

	public UILabel levelNickname;

	public UILabel type;

	public UILabel totalPower;

	public UILabel attackSummary;

	public UILabel defenseSummary;

	public UILabel health;

	public UILabel attack;

	public UILabel reloadSpeed;

	public UILabel critical;

	public UILabel criticalDamage;

	public UILabel accuracy;

	public UILabel evasion;

	public UILabel defense;

	public UILabel leadership;

	public UILabel delay;

	public UILabel researchDuration;

	public UILabel speed;

	public UILabel increaseHealth;

	public UILabel increaseAttack;

	public UILabel increaseEvasion;

	public UILabel increaseDefense;

	public UILabel increaseAccuracy;

	public UILabel bonusDescription;

	public UILabel remainLeadership;

	public UIGrid rankGrid;

	public UISprite jobIcon;

	public UILabel originTotalPower;

	public UILabel originHealth;

	public UILabel originAttack;

	public UILabel originReloadSpeed;

	public UILabel originCritical;

	public UILabel originCriticalDamage;

	public UILabel originAccuracy;

	public UILabel originEvasion;

	public UILabel originDefense;

	public UILabel effectHealth;

	public UILabel effectAttack;

	public UILabel effectReloadSpeed;

	public UILabel effectCritical;

	public UILabel effectCriticalDamage;

	public UILabel effectAccuracy;

	public UILabel effectEvasion;

	public UILabel effectDefense;

	public UIDefaultItem[] attackTypeIconList;

	public UIDefaultItem[] skillIconList;

	public UIDefaultItem[] targetOrderPostionList;

	public void Set(RoUnit unit)
	{
		if (unit != null)
		{
			Set(unit.currLevelReg);
			UISetter.SetLabel(level, unit.level);
			UISetter.SetLabel(dotLevel, Localization.Format("1021", unit.level));
			UISetter.SetLabel(spaceLevel, Localization.Format("1021", unit.level));
			string nameKey = unit.unitReg.nameKey;
			if (Localization.Exists(nameKey))
			{
				UISetter.SetLabel(levelNickname, Localization.Format("1021", unit.level, Localization.Get(nameKey)));
			}
			else
			{
				UISetter.SetLabel(levelNickname, Localization.Format("1021", unit.level, nameKey));
			}
		}
	}

	public void Set(RoUnit unit, RoCommander commander)
	{
		if (unit != null)
		{
			Set(commander.currLevelUnitReg);
			UISetter.SetLabel(level, unit.level);
			UISetter.SetLabel(dotLevel, Localization.Format("1021", unit.level));
			UISetter.SetLabel(spaceLevel, Localization.Format("1021", unit.level));
			string nameKey = unit.unitReg.nameKey;
			if (Localization.Exists(nameKey))
			{
				UISetter.SetLabel(levelNickname, Localization.Format("1021", unit.level, Localization.Get(nameKey)));
			}
			else
			{
				UISetter.SetLabel(levelNickname, Localization.Format("1021", unit.level, nameKey));
			}
		}
	}

	public void Set(UnitDataRow unit)
	{
		if (unit != null)
		{
			UISetter.SetSprite(unitThumbnail, string.Format(unit.resourceName + UIUnit.thumbnailFrontIdPostfix));
			UISetter.SetLabel(type, unit.type);
			UISetter.SetLabel(health, unit.maxHealth);
			UISetter.SetLabel(attack, unit.attackDamage);
			UISetter.SetLabel(reloadSpeed, unit.reloadSpeed);
			UISetter.SetLabel(defense, unit.defense);
			UISetter.SetLabel(leadership, unit.leadership);
			UISetter.SetLabel(critical, Localization.Format("5781", unit.criticalChance));
			UISetter.SetLabel(criticalDamage, Localization.Format("5781", unit.criticalDamage));
			UISetter.SetLabel(evasion, unit.evasion);
			UISetter.SetLabel(accuracy, unit.accuracy);
			UISetter.SetLabel(totalPower, unit.GetTotalPower().ToString("N0"));
			UISetter.SetLabel(speed, unit.speed.ToString("N0"));
			UISetter.SetSprite(jobIcon, "com_icon_" + unit.job.ToString().ToLower());
			UISetter.SetLabel(originTotalPower, unit.GetOriginTotalPower().ToString("N0"));
			UISetter.SetLabel(originHealth, unit.originMaxHealth);
			UISetter.SetLabel(originAttack, unit.originAttackDamage);
			UISetter.SetLabel(originReloadSpeed, unit.originSpeed);
			UISetter.SetLabel(originDefense, unit.originDefense);
			UISetter.SetLabel(originCritical, Localization.Format("5781", unit.originCriticalChance));
			UISetter.SetLabel(originCriticalDamage, Localization.Format("5781", unit.originCriticalDamageBonus));
			UISetter.SetLabel(originEvasion, unit.originLuck);
			UISetter.SetLabel(originAccuracy, unit.originAccuracy);
			UISetter.SetLabel(effectHealth, "(+" + (unit.maxHealth - unit.originMaxHealth) + ")");
			UISetter.SetLabel(effectAttack, "(+" + (unit.attackDamage - unit.originAttackDamage) + ")");
			UISetter.SetLabel(effectReloadSpeed, "(+" + (unit.reloadSpeed - unit.originSpeed) + ")");
			UISetter.SetLabel(effectDefense, "(+" + (unit.defense - unit.originDefense) + ")");
			UISetter.SetLabel(effectCritical, "(+" + Localization.Format("5781", unit.criticalChance - unit.originCriticalChance) + ")");
			UISetter.SetLabel(effectCriticalDamage, "(+" + Localization.Format("5781", unit.criticalDamage - unit.originCriticalDamageBonus) + ")");
			UISetter.SetLabel(effectEvasion, "(+" + (unit.evasion - unit.originLuck) + ")");
			UISetter.SetLabel(effectAccuracy, "(+" + (unit.accuracy - unit.originAccuracy) + ")");
			if (unit.nameKey != null)
			{
				UISetter.SetLabelWithLocalization(nickname, Localization.Get(unit.nameKey));
			}
		}
	}

	public void Set(RoCommander commander)
	{
		UnitDataRow currLevelUnitReg = commander.currLevelUnitReg;
		Set(currLevelUnitReg);
		UISetter.SetRank(rankGrid, commander.rank);
		UISetter.SetLabel(level, commander.level);
		UISetter.SetLabel(dotLevel, Localization.Format("1021", commander.level));
		UISetter.SetLabel(spaceLevel, Localization.Format("1021", commander.level));
		string nameKey = currLevelUnitReg.nameKey;
		UISetter.SetLabel(levelNickname, Localization.Format("1021", commander.level, (!Localization.Exists(nameKey)) ? nameKey : Localization.Get(nameKey)));
		Regulation regulation = RemoteObjectManager.instance.regulation;
		string key = $"{(((int)commander.marry != 1) ? currLevelUnitReg.levelPattern : currLevelUnitReg.afterLevelPattern)}_{commander.rank}";
		if (regulation.levelPatternDtbl.ContainsKey(key))
		{
			LevelPatternDataRow levelPatternDataRow = regulation.levelPatternDtbl[key];
			UISetter.SetLabel(increaseHealth, $"+{levelPatternDataRow.hp}");
			UISetter.SetLabel(increaseAttack, $"+{levelPatternDataRow.atk}");
			UISetter.SetLabel(increaseDefense, $"+{levelPatternDataRow.def}");
			UISetter.SetLabel(increaseEvasion, $"+{levelPatternDataRow.luck}");
			UISetter.SetLabel(increaseAccuracy, $"+{levelPatternDataRow.aim}");
		}
	}

	public void Set(RoTroop troop)
	{
		UnitDataRow totalStatus = troop.GetTotalStatus();
		Set(totalStatus);
		UISetter.SetLabel(totalPower, troop.GetTotalPower());
		UISetter.SetLabel(speed, troop.GetTotalSpeed());
	}
}
