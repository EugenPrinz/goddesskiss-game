using System;
using Shared.Regulation;
using UnityEngine;

public class GuildBoardListItem : UIItemBase
{
	public GameObject removeBtn;

	public UISprite icon;

	public UILabel nickname;

	public UILabel contents;

	public UILabel date;

	private int uno;

	public void Set(Protocols.GuildBoardData data)
	{
		uno = data.uno;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		if (uno.ToString() == RemoteObjectManager.instance.localUser.uno)
		{
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.FindCostumeData(int.Parse(data.thumb));
			if (commanderCostumeDataRow != null)
			{
				RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommander(commanderCostumeDataRow.cid.ToString());
				if (roCommander != null && roCommander.isBasicCostume)
				{
					UISetter.SetSprite(icon, roCommander.resourceId + "_" + roCommander.currentViewCostume);
				}
				else
				{
					UISetter.SetSprite(icon, regulation.GetCostumeThumbnailName(int.Parse(data.thumb)));
				}
			}
		}
		else
		{
			UISetter.SetSprite(icon, regulation.GetCostumeThumbnailName(int.Parse(data.thumb)));
		}
		UISetter.SetLabel(nickname, data.unm);
		UISetter.SetLabel(contents, data.msg);
		DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(data.regdt).ToLocalTime();
		UISetter.SetLabel(date, Utility.GetStringToDay(dt));
		UISetter.SetActive(removeBtn, data.dauth == 1);
		UISetter.SetGameObjectName(removeBtn, string.Format("{0}{1}/{2}", "Remove-", data.idx, data.unm));
	}
}
