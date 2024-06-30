using System.Collections;
using UnityEngine;

public class UISelectPlatformPopup : UIPopup
{
	public GameObject BtnGuest;

	public GameObject BtnFacebook;

	public GameObject BtnGoogle;

	public GameObject BtnDbros;

	public UILabel label;

	private int loginType;

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		switch (sender.name)
		{
		case "Close":
			Close();
			break;
		case "Platform_Guest":
		{
			M01_Title m01_Title2 = Object.FindObjectOfType(typeof(M01_Title)) as M01_Title;
			m01_Title2.obj_Login.GetComponent<UILogin>().GuestSignUp();
			AdjustManager.Instance.SimpleEvent("75c7mx");
			break;
		}
		case "Platform_FB":
		{
			M01_Title m01_Title4 = Object.FindObjectOfType(typeof(M01_Title)) as M01_Title;
			m01_Title4.obj_Login.GetComponent<UILogin>().FaceBookBtn(loginType);
			AdjustManager.Instance.SimpleEvent("2ldxbw");
			break;
		}
		case "Platform_Google":
		{
			M01_Title m01_Title3 = Object.FindObjectOfType(typeof(M01_Title)) as M01_Title;
			m01_Title3.obj_Login.GetComponent<UILogin>().GoogleBtn(loginType);
			AdjustManager.Instance.SimpleEvent("fa2yiq");
			break;
		}
		case "Platform_Dbros":
			if (loginType != 0)
			{
				M01_Title m01_Title = Object.FindObjectOfType(typeof(M01_Title)) as M01_Title;
				m01_Title.obj_Login.GetComponent<UILogin>().CreateMember(loginType);
			}
			AdjustManager.Instance.SimpleEvent("qpbho9");
			break;
		}
	}

	public void InitAndOpen(Protocols.OSCode osType, Platform prePlatform)
	{
		SetAutoDestroy(autoDestory: true);
		UISetter.SetActive(BtnGuest, active: false);
		UISetter.SetActive(BtnFacebook, prePlatform != Platform.FaceBook);
		UISetter.SetActive(BtnGoogle, prePlatform != Platform.Google && osType == Protocols.OSCode.Android && RemoteObjectManager.instance.localUser.bEnableGoogleAccount);
		UISetter.SetActive(BtnDbros, prePlatform != Platform.Dbros);
		if (prePlatform == Platform.Guest)
		{
			UISetter.SetLabel(title, Localization.Get("19040"));
			UISetter.SetLabel(label, Localization.Get("19530"));
			loginType = 2;
		}
		else
		{
			UISetter.SetLabel(title, Localization.Get("19520"));
			UISetter.SetLabel(label, Localization.Get("19526"));
			loginType = 1;
		}
	}

	public void InitAndOpen()
	{
		SetAutoDestroy(autoDestory: true);
		Protocols.OSCode oSCode = Protocols.OSCode.Android;
		UISetter.SetActive(BtnGuest, active: true);
		UISetter.SetActive(BtnFacebook, active: true);
		UISetter.SetActive(BtnGoogle, oSCode == Protocols.OSCode.Android && RemoteObjectManager.instance.localUser.bEnableGoogleAccount);
		UISetter.SetActive(BtnDbros, active: true);
		UISetter.SetLabel(title, Localization.Get("19022"));
		UISetter.SetLabel(label, Localization.Get("19530"));
		loginType = 0;
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
	}

	private void OnAnimClose()
	{
	}
}
