using UnityEngine;

public class RaidItem : UICommander
{
	public GameObject btn;

	public void Set(RoCommander commander, int index)
	{
		Set(commander);
	}

	public void SetRemainTime(int remain)
	{
		SetLock(remain > 0);
		UISetter.SetActive(btn, remain == 0);
		UISetter.SetActive(timer, remain > 0);
		if (remain > 0)
		{
			TimeData timeData = TimeData.Create();
			timeData.SetByDuration(remain);
			timer.Set(timeData);
			timer.SetLabelFormat(string.Empty, Localization.Get("18044"));
		}
	}
}
