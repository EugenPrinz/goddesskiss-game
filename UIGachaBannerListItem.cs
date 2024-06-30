using Shared.Regulation;
using UnityEngine;

public class UIGachaBannerListItem : UIItemBase
{
	public UISprite bg;

	public UILabel description;

	public GameObject selectRoot;

	public GameObject badge;

	private string type;

	public void Set(GachaDataRow row)
	{
		type = row.type;
		UISetter.SetSprite(bg, Localization.Get(row.img));
		UISetter.SetActive(description, row.eventComment != "0");
		UISetter.SetLabel(description, Localization.Get(row.eventComment));
	}

	public void OnRefresh()
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		RoGacha roGacha = localUser.gacha[type];
		UISetter.SetActive(badge, roGacha.canOpenFreeBox);
	}

	public override void SetSelection(bool selected)
	{
		base.SetSelection(selected);
		UISetter.SetActive(selectRoot, selected);
	}
}
