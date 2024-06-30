using System;
using System.Collections.Generic;
using UnityEngine;

public class ChannelListItem : UIItemBase
{
	public UILabel title;

	public UILabel day;

	public UILabel description;

	public GameObject button;

	public void Set(KeyValuePair<string, Protocols.ChannelData> row)
	{
		UISetter.SetLabel(title, Localization.Get((int.Parse(row.Key) + 19601).ToString()));
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(row.Value.openTime).ToLocalTime();
		UISetter.SetLabel(day, Localization.Format("19606", dateTime.Year, dateTime.Month, dateTime.Day));
		UISetter.SetLabel(description, string.Format("({0}{1},{2}{3},{4}{5},{6}{7})", Localization.Get("19607"), row.Value.maxLevel, Localization.Get("19608"), row.Value.maxStage, Localization.Get("19609"), row.Value.commanderCount, Localization.Get("19610"), row.Value.serverCount));
		UISetter.SetGameObjectName(button, $"{button.name}-{row.Key}");
	}
}
