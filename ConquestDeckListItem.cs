using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class ConquestDeckListItem : UIItemBase
{
	public UILabel title;

	public UITimer timer;

	public UILabel positionLabel;

	public GameObject addSlot;

	public GameObject deleteSlot;

	public GameObject settingTroop;

	public GameObject movingTroop;

	public GameObject standByTroop;

	public GameObject emptySlot;

	public GameObject standByPosition;

	public UICommander[] commanderItem;

	public void Set(int number, EConquestStageInfoType type, Protocols.ConquestTroopInfo.Troop troop)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UIConquestMap conquestMap = UIManager.instance.world.conquestMap;
		UISetter.SetGameObjectName(addSlot, $"AddSlot-{number}");
		UISetter.SetGameObjectName(deleteSlot, $"DeleteSlot-{number}");
		if (type == EConquestStageInfoType.Main || troop == null)
		{
			UISetter.SetGameObjectName(settingTroop, $"SettingTroop-{number}");
		}
		else
		{
			UISetter.SetGameObjectName(settingTroop, $"MoveTroop-{number}");
		}
		UISetter.SetLabel(title, Localization.Format("110318", number));
		int num = int.Parse(regulation.defineDtbl["GUILD_OCCUPY_TEAM"].value);
		int num2 = int.Parse(regulation.defineDtbl["GUILD_OCCUPY_PREMIUM_TEAM"].value);
		bool flag = number <= num;
		if (number > num)
		{
			bool flag2 = false;
			bool flag3 = localUser.conquestDeckSlotState.Contains(number);
			for (int i = 0; i < localUser.conquestDeckSlotState.Count; i++)
			{
				if (number == localUser.conquestDeckSlotState[i] + 1)
				{
					flag2 = true;
				}
			}
			UISetter.SetActive(base.gameObject, flag2 || number == num + 1);
			UISetter.SetActive(addSlot, !flag3);
		}
		else
		{
			UISetter.SetActive(addSlot, active: false);
		}
		UISetter.SetActive(emptySlot, troop == null && !addSlot.activeSelf);
		UISetter.SetActive(timer, active: false);
		UISetter.SetActive(positionLabel, active: false);
		UISetter.SetActive(settingTroop, active: true);
		UISetter.SetActive(movingTroop, active: false);
		UISetter.SetActive(standByTroop, active: false);
		UISetter.SetActive(deleteSlot, active: false);
		UISetter.SetActive(standByPosition, active: false);
		UICommander[] array = commanderItem;
		foreach (UICommander mb in array)
		{
			UISetter.SetActive(mb, active: false);
		}
		if (troop == null)
		{
			return;
		}
		int num3 = 0;
		int num4 = 0;
		num4 = ((troop.point != 0) ? troop.point : UIManager.instance.world.conquestMap.mainStageId);
		GuildOccupyDataRow guildOccupyDataRow = regulation.guildOccupyDtbl[num4.ToString()];
		foreach (KeyValuePair<string, string> item in troop.deck)
		{
			RoCommander roCommander = localUser.FindCommander(item.Value.ToString());
			if (roCommander != null)
			{
				UISetter.SetActive(commanderItem[num3], active: true);
				commanderItem[num3].Set(roCommander);
			}
			num3++;
		}
		if (troop.status == "S")
		{
			UISetter.SetActive(deleteSlot, troop.point == 0);
			UISetter.SetActive(positionLabel, troop.point != 0);
			UISetter.SetActive(timer, active: false);
			UISetter.SetActive(settingTroop, (troop.point == 0 || (type == EConquestStageInfoType.Move && conquestMap.selectPoint != troop.point)) ? true : false);
			UISetter.SetActive(movingTroop, active: false);
			UISetter.SetActive(standByTroop, (troop.point != 0 && type == EConquestStageInfoType.Main) ? true : false);
			UISetter.SetActive(standByPosition, (troop.point != 0 && conquestMap.selectPoint == troop.point) ? true : false);
		}
		else if (troop.status == "M")
		{
			UISetter.SetActive(timer, active: true);
			UISetter.SetActive(positionLabel, active: false);
			UISetter.SetActive(settingTroop, active: false);
			UISetter.SetActive(movingTroop, active: true);
			UISetter.SetActive(standByTroop, active: false);
			timer.Set(troop.remainData);
			timer.SetLabelFormat(string.Empty, Localization.Format("110281", Localization.Get(guildOccupyDataRow.s_idx)));
			timer.RegisterOnFinished(delegate
			{
				UISetter.SetActive(timer, active: false);
				UISetter.SetActive(positionLabel, active: true);
			});
		}
		UISetter.SetLabel(positionLabel, Localization.Format("110280", Localization.Get(guildOccupyDataRow.s_idx)));
	}
}
