using UnityEngine;

public class UIWeaponProgressListItem : UIItemBase
{
	public UILabel slot;

	public UISprite bg;

	public GameObject bgPattern;

	public GameObject lockRoot;

	public GameObject possibleRoot;

	public GameObject progressRoot;

	public GameObject finishRoot;

	public GameObject immediateBtn;

	public GameObject finishBtn;

	public GameObject progressBtn;

	public GameObject lockBtn;

	public UITimer progressTimer;

	private readonly string immediateBtnPrefix = "ImmediateBtn-";

	private readonly string finishBtnPrefix = "FinishBtn-";

	private readonly string progressBtnPrefix = "ProgressBtn-";

	private readonly string slotLockBtnPrefix = "SlotLockBtn-";

	public void Set(int slot, int remain)
	{
		bool flag = slot > RemoteObjectManager.instance.localUser.statistics.weaponMakeSlotCount;
		UISetter.SetLabel(this.slot, slot.ToString("00"));
		UISetter.SetGameObjectName(lockBtn, $"{slotLockBtnPrefix}{slot}");
		UISetter.SetGameObjectName(immediateBtn, $"{immediateBtnPrefix}{slot}");
		UISetter.SetGameObjectName(progressBtn, $"{progressBtnPrefix}{slot}");
		UISetter.SetGameObjectName(finishBtn, $"{finishBtnPrefix}{slot}");
		if (flag)
		{
			UISetter.SetActive(lockRoot, active: true);
			UISetter.SetActive(possibleRoot, active: false);
			UISetter.SetActive(finishRoot, active: false);
			UISetter.SetActive(progressRoot, active: false);
			UISetter.SetActive(bgPattern, active: false);
			UISetter.SetSprite(bg, "eq_img02_gray");
			return;
		}
		UISetter.SetActive(bgPattern, active: true);
		UISetter.SetActive(lockRoot, active: false);
		UISetter.SetActive(possibleRoot, remain < 0);
		UISetter.SetActive(finishRoot, remain == 0);
		UISetter.SetActive(progressRoot, remain > 0);
		if (remain < 0)
		{
			UISetter.SetSprite(bg, "eq_img01_brown");
		}
		else if (remain == 0)
		{
			UISetter.SetSprite(bg, "eq_img01_yellow");
		}
		else
		{
			UISetter.SetSprite(bg, "eq_img01_green");
		}
		if (remain > 0)
		{
			TimeData timeData = TimeData.Create();
			timeData.SetByDuration(remain);
			progressTimer.Set(timeData);
			progressTimer.RegisterOnFinished(delegate
			{
				isFinish();
			});
		}
		else if (remain == 0)
		{
			isFinish();
		}
	}

	private void isFinish()
	{
		UISetter.SetActive(lockRoot, active: false);
		UISetter.SetActive(possibleRoot, active: false);
		UISetter.SetActive(finishRoot, active: true);
		UISetter.SetActive(progressRoot, active: false);
		UISetter.SetSprite(bg, "eq_img01_yellow");
	}
}
