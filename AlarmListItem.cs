using Shared.Regulation;
using UnityEngine;

public class AlarmListItem : UIItemBase
{
	public UISprite icon;

	public UILabel title;

	public UILabel description;

	public GameObject moveBtn;

	public void SetMissonCount(AlarmDataRow row, int count)
	{
		UISetter.SetSprite(icon, "thum_WarMemorial");
		UISetter.SetLabel(title, Localization.Get(row.title));
		UISetter.SetLabel(description, Localization.Format(row.description, count));
	}

	public void SetCommanderHold(AlarmDataRow row, int time, int count)
	{
		UISetter.SetSprite(icon, "thum_Academy");
		UISetter.SetLabel(title, Localization.Get(row.title));
		UISetter.SetLabel(description, Localization.Format(row.description, Utility.GetTimeString(time)));
	}

	public void SetCommanderRefresh(AlarmDataRow row, int time)
	{
		UISetter.SetSprite(icon, "thum_Academy");
		UISetter.SetLabel(title, Localization.Get(row.title));
		UISetter.SetLabel(description, Localization.Format(row.description, Utility.GetTimeString(time)));
	}

	public void SetShopRefresh(AlarmDataRow row, int time)
	{
		UISetter.SetSprite(icon, "thum_BlackMarket");
		UISetter.SetLabel(title, Localization.Get(row.title));
		UISetter.SetLabel(description, Localization.Get(row.description));
	}

	public void SetExplosionEnd(AlarmDataRow row, int time)
	{
		UISetter.SetSprite(icon, "thum_SituationRoom");
		UISetter.SetLabel(title, Localization.Get(row.title));
		UISetter.SetLabel(description, Localization.Format(row.description, Utility.GetTimeString(time)));
	}

	public void SetSweepEnd(AlarmDataRow row, int time)
	{
		UISetter.SetSprite(icon, "thum_SituationRoom");
		UISetter.SetLabel(title, Localization.Get(row.title));
		UISetter.SetLabel(description, Localization.Format(row.description, Utility.GetTimeString(time)));
	}

	public void SetScrambleStart(AlarmDataRow row, int time)
	{
		UISetter.SetSprite(icon, "thum_Guild");
		UISetter.SetLabel(title, Localization.Get(row.title));
		UISetter.SetLabel(description, Localization.Format(row.description, Utility.GetTimeString(time)));
	}

	public void SetScrambleEnd(AlarmDataRow row, int time)
	{
		UISetter.SetSprite(icon, "thum_Guild");
		UISetter.SetLabel(title, Localization.Get(row.title));
		UISetter.SetLabel(description, Localization.Format(row.description, Utility.GetTimeString(time)));
	}

	public void SetRaidEnd(AlarmDataRow row, int time)
	{
		UISetter.SetSprite(icon, "thum_Raid");
		UISetter.SetLabel(title, Localization.Get(row.title));
		UISetter.SetLabel(description, Localization.Format(row.description, Utility.GetTimeString(time)));
	}

	public void SetArenaEnd(AlarmDataRow row, int time)
	{
		UISetter.SetSprite(icon, "thum_Challenge");
		UISetter.SetLabel(title, Localization.Get(row.title));
		UISetter.SetLabel(description, Localization.Format(row.description, Utility.GetTimeString(time)));
	}
}
