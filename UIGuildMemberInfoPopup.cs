using UnityEngine;

public class UIGuildMemberInfoPopup : UISimplePopup
{
	public new UILabel name;

	public UISprite icon;

	public UILabel level;

	public UILabel time;

	public UILabel world;

	public UIButton btn1;

	public UIButton btn2;

	public UIButton btn3;

	public UILabel btnName1;

	public UILabel btnName2;

	public UILabel btnName3;

	public GameObject btnCancel;

	public GameObject btnOK;

	public GameObject btnExtra;

	private void Start()
	{
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
		SetRecyclable(recyclable: false);
	}

	public void SetInfo(string memberName, int memberWorld, int mamberIcon, int memberLevel, string lastTime, string buttonName1, string buttonName2, string buttonName3)
	{
		SetButton();
		UISetter.SetLabel(name, memberName);
		UISetter.SetLabel(world, Localization.Format("19067", memberWorld));
		UISetter.SetSpriteWithSnap(icon, RemoteObjectManager.instance.regulation.GetCostumeThumbnailName(mamberIcon), pixelPerfect: false);
		UISetter.SetLabel(level, "Lv" + memberLevel);
		UISetter.SetLabel(time, Localization.Format("80028", lastTime));
		UISetter.SetActive(btn1, !string.IsNullOrEmpty(buttonName1));
		UISetter.SetActive(btn2, !string.IsNullOrEmpty(buttonName2));
		UISetter.SetActive(btn3, !string.IsNullOrEmpty(buttonName3));
		UISetter.SetLabel(btnName1, buttonName1);
		UISetter.SetLabel(btnName2, buttonName2);
		UISetter.SetLabel(btnName3, buttonName3);
	}

	private void SetButton()
	{
		UISetter.SetActive(btnOK, !string.IsNullOrEmpty(okName.text));
		UISetter.SetActive(btnExtra, !string.IsNullOrEmpty(extraName.text));
		UISetter.SetActive(btnCancel, !string.IsNullOrEmpty(cancelName.text));
	}
}
