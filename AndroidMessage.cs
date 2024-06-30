using System;
using UnityEngine;

public class AndroidMessage : BaseAndroidPopup
{
	public string ok;

	public Action OnComplete = delegate
	{
	};

	public static AndroidMessage Create(string title, string message)
	{
		return Create(title, message, "Ok");
	}

	public static AndroidMessage Create(string title, string message, string ok)
	{
		AndroidMessage androidMessage = new GameObject("AndroidPopUp").AddComponent<AndroidMessage>();
		androidMessage.title = title;
		androidMessage.message = message;
		androidMessage.ok = ok;
		androidMessage.init();
		return androidMessage;
	}

	public void init()
	{
		AN_PoupsProxy.showMessage(title, message, ok);
	}

	public void onPopUpCallBack(string buttonIndex)
	{
		DispatchAction(AndroidDialogResult.CLOSED);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
