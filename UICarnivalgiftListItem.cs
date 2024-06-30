using UnityEngine;

public class UICarnivalgiftListItem : UIItemBase
{
	public UISprite icon;

	public GameObject selectRoot;

	public void Set(bool isComplete)
	{
		UISetter.SetSprite(icon, (!isComplete) ? "ng-event-icon" : "ng-event-icon02");
	}

	public override void SetSelection(bool selected)
	{
		base.SetSelection(selected);
		UISetter.SetActive(selectRoot, selected);
	}
}
