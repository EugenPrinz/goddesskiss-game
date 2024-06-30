using Shared.Regulation;
using UnityEngine;

public class UIEventBattleItem : UIItemBase
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

	public EventBattleFieldDataRow row;

	private Color color = new Color(246f, 235f, 103f);

	private Color lock_color = new Color(180f, 193f, 193f);

	private bool isLock;

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(selectSprite, selected);
		_selected = selected;
	}

	public void Set(EventBattleFieldDataRow _row, string lastClearId, bool eventEnd)
	{
		row = _row;
		UISetter.SetSprite(stageSprite, row.thumbnail);
		UISetter.SetSprite(subIcon, $"situation_text_{row.idx:00}");
		UISetter.SetLabel(stageName, Localization.Get(row.name));
		UISetter.SetActive(lockRoot, IsStageLock(lastClearId, eventEnd));
	}

	private bool IsStageLock(string lastClearId, bool eventEnd)
	{
		if (eventEnd)
		{
			return true;
		}
		if (row.idx > int.Parse(lastClearId) + 1)
		{
			return true;
		}
		return false;
	}
}
