using Shared.Regulation;
using UnityEngine;

public class SweepListItem : UIItemBase
{
	public UISprite icon;

	public UISprite subIcon;

	public UILabel stageName;

	public UISprite stageSprite;

	public GameObject selectSprite;

	public GameObject lockRoot;

	private bool _selected;

	private int minLevel;

	private int stageLevel;

	public SweepDataRow row;

	private Color color = new Color(246f, 235f, 103f);

	private Color lock_color = new Color(180f, 193f, 193f);

	public ESweepLockType lockType;

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(selectSprite, selected);
		_selected = selected;
	}

	public void Set(SweepDataRow _row)
	{
		row = _row;
		UISetter.SetSprite(stageSprite, $"situation_bg_boss_{row.level:00}");
		UISetter.SetSprite(icon, $"icon_pvp_{row.level - 1:00}");
		UISetter.SetSprite(subIcon, $"situation_text_{row.level:00}");
		string text = $"situation_text_{row.level + 1}";
		lockType = IsStageLock();
		if (lockType == ESweepLockType.MinLevel)
		{
			UISetter.SetActive(lockRoot, active: true);
			UISetter.SetLabel(stageName, Localization.Format("5178", row.minLevel));
		}
		else if (lockType == ESweepLockType.PrevStageClear)
		{
			UISetter.SetActive(lockRoot, active: true);
			UISetter.SetLabel(stageName, Localization.Get(row.name));
		}
		else
		{
			UISetter.SetActive(lockRoot, active: false);
			UISetter.SetLabel(stageName, Localization.Get(row.name));
		}
	}

	public ESweepLockType IsStageLock()
	{
		ESweepLockType result = ESweepLockType.None;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		bool flag = true;
		if (row.level > 1)
		{
			flag = localUser.GetSweepClearState(row.type, row.level - 1);
		}
		if (localUser.level < row.minLevel)
		{
			result = ESweepLockType.MinLevel;
		}
		else if (!flag)
		{
			result = ESweepLockType.PrevStageClear;
		}
		return result;
	}
}
