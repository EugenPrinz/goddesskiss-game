using UnityEngine;

public class UISelectAnnihilationModePopup : UIPopup
{
	[SerializeField]
	private GameObject hellLock;

	[SerializeField]
	private UILabel hellLockTxt;

	public void InitAndOpen()
	{
		UISetter.SetActive(this, active: true);
		bool flag = ((int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["ANNIHILATE_HELL_OPEN"].value) != 0) ? true : false);
		UISetter.SetActive(hellLock, !flag);
		if (!flag)
		{
			UISetter.SetLabel(hellLockTxt, Localization.Get("5040005"));
		}
		SetRecyclable(recyclable: false);
		SoundManager.PlaySFX("SE_MenuOpen_001");
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		string subMessage = string.Empty;
		AnnihilationMode selectMode = AnnihilationMode.NONE;
		switch (text)
		{
		case "normal_enterBtn":
			subMessage = "89507";
			selectMode = AnnihilationMode.NORMAL;
			break;
		case "hard_enterBtn":
			subMessage = "89508";
			selectMode = AnnihilationMode.HARD;
			break;
		case "hell_enterBtn":
			subMessage = "89509";
			selectMode = AnnihilationMode.HELL;
			break;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "89506", subMessage, "1001", "1000").onClick = delegate(GameObject popupSender)
		{
			string text2 = popupSender.name;
			if (text2 == "OK")
			{
				RemoteObjectManager.instance.RequestResetAnnihilationStage(selectMode);
				Close();
			}
		};
		base.OnClick(sender);
	}
}
