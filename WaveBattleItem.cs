using Shared.Regulation;
using UnityEngine;

public class WaveBattleItem : UIItemBase
{
	[SerializeField]
	private UISprite img;

	[SerializeField]
	private GameObject btn;

	[SerializeField]
	private GameObject Lock;

	[SerializeField]
	private GameObject btnLock;

	[SerializeField]
	private UILabel Count;

	[SerializeField]
	private UITimer timer;

	[SerializeField]
	private GameObject badge;

	public void SetWaveItem(Protocols.WaveBattleInfoList.WaveBattleInfo info)
	{
		WaveBattleDataRow waveBattleDataRow = RemoteObjectManager.instance.regulation.FindWaveBattleData(info.battleIdx);
		UISetter.SetSprite(img, waveBattleDataRow.thumbnail);
		UISetter.SetActive(btn, info.closeTime == -1 || info.openTime == 0);
		UISetter.SetActive(Lock, info.openTime > 0);
		UISetter.SetLabel(Count, string.Format("{0} {1}/{2}", Localization.Get("4815"), info.maxCount - info.clearCount, info.maxCount));
		UISetter.SetActive(btnLock, info.clearCount == info.maxCount);
		UISetter.SetActive(badge, info.clearCount < info.maxCount);
		TimeData timeData = TimeData.Create();
		if (info.closeTime == -1)
		{
			UISetter.SetActive(timer, active: false);
			UISetter.SetActive(Count, active: true);
		}
		else if (info.closeTime > 0 && info.openTime == 0)
		{
			timeData.SetByDuration(info.closeTime);
			UISetter.SetTimer(timer, timeData, "4802");
			timer.RegisterOnFinished(delegate
			{
				RemoteObjectManager.instance.RequestWaveBattleList();
			});
			UISetter.SetActive(Count, active: true);
		}
		else if (info.openTime > 0)
		{
			timeData.SetByDuration(info.openTime);
			UISetter.SetTimer(timer, timeData, "4803");
			UISetter.SetActive(Count, active: false);
			timer.RegisterOnFinished(delegate
			{
				RemoteObjectManager.instance.RequestWaveBattleList();
			});
		}
	}
}
