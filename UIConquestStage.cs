using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIConquestStage : UIItemBase
{
	public UISprite icon;

	public UILabel stageName;

	public UILabel symbol;

	public UILabel occCount;

	public UILabel enemyCount;

	public GameObject flagAnimation;

	public GameObject symbolRoot;

	public GameObject effectRoot;

	[HideInInspector]
	public bool isMainStage;

	[HideInInspector]
	public int point;

	public void Set(GuildOccupyDataRow row)
	{
		point = int.Parse(row.idx);
		UIConquestMap conquestMap = UIManager.instance.world.conquestMap;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		bool flag = point == conquestMap.enemyStageId;
		isMainStage = point == conquestMap.mainStageId;
		UISetter.SetSprite(icon, row.building);
		if (isMainStage)
		{
			UISetter.SetLabel(stageName, localUser.guildInfo.name);
		}
		else if (flag)
		{
			UISetter.SetLabel(stageName, conquestMap.eGuild.name);
		}
		else
		{
			UISetter.SetLabel(stageName, Localization.Get(row.s_idx));
		}
		UISetter.SetLabel(symbol, Localization.Get(row.symbol));
		SetArrowCheck();
	}

	public void SetArrowCheck()
	{
		if (UIManager.instance.world.guild.conquestInfo.state != EConquestState.Setting)
		{
			UISetter.SetActive(flagAnimation, active: false);
		}
		else if (isMainStage)
		{
			UISetter.SetActive(flagAnimation, SettingTroopEnable());
		}
		else
		{
			UISetter.SetActive(flagAnimation, StandByTroop());
		}
	}

	public void RadarState(bool state)
	{
		UISetter.SetActive(symbolRoot, state);
	}

	public void SetRadar(Protocols.GetRadarData.User user)
	{
		if (user == null)
		{
			UISetter.SetLabel(occCount, "?");
			UISetter.SetLabel(enemyCount, "?");
			return;
		}
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		string arg = ((user.alie.stand != -1) ? user.alie.stand.ToString() : "?");
		string arg2 = ((user.alie.move != -1) ? user.alie.move.ToString() : "?");
		string arg3 = ((user.enemy.stand != -1) ? user.enemy.stand.ToString() : "?");
		string arg4 = ((user.enemy.move != -1) ? user.enemy.move.ToString() : "?");
		if (localUser.conquestTeam == EConquestTeam.Blue)
		{
			UISetter.SetLabel(occCount, $"{arg}+{arg2}");
			UISetter.SetLabel(enemyCount, $"{arg3}+{arg4}");
		}
		else
		{
			UISetter.SetLabel(occCount, $"{arg3}+{arg4}");
			UISetter.SetLabel(enemyCount, $"{arg}+{arg2}");
		}
	}

	private bool SettingTroopEnable()
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		int num = int.Parse(regulation.defineDtbl["GUILD_OCCUPY_TEAM"].value);
		int num2 = int.Parse(regulation.defineDtbl["GUILD_OCCUPY_PREMIUM_TEAM"].value);
		int num3 = 0;
		int num4 = num;
		foreach (KeyValuePair<int, Protocols.ConquestTroopInfo.Troop> item in localUser.conquestDeck)
		{
			if (item.Value != null)
			{
				num3++;
			}
		}
		for (int i = 0; i < num2; i++)
		{
			int num5 = num + (i + 1);
			for (int j = 0; j < localUser.conquestDeckSlotState.Count; j++)
			{
				if (num5 == localUser.conquestDeckSlotState[j])
				{
					num4++;
				}
			}
		}
		List<RoCommander> list = localUser.commanderList.FindAll((RoCommander row) => row.conquestDeckId == 0);
		return num3 < num4 && list.Count >= 5;
	}

	private bool StandByTroop()
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		foreach (KeyValuePair<int, Protocols.ConquestTroopInfo.Troop> item in localUser.conquestDeck)
		{
			if (item.Value != null && item.Value.point == point && item.Value.status == "S")
			{
				return true;
			}
		}
		return false;
	}
}
