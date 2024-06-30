using UnityEngine;

public class MainBannerOrderIcon : UIItemBase
{
	[SerializeField]
	private GameObject OnIcon;

	[SerializeField]
	private GameObject OffIcon;

	public void SetActiveIcon(bool isActive)
	{
		UISetter.SetActive(OnIcon, isActive);
		UISetter.SetActive(OffIcon, !isActive);
	}

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(OnIcon, selected);
		UISetter.SetActive(OffIcon, !selected);
	}
}
