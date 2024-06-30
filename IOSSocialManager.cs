using System;
using UnityEngine;

public class IOSSocialManager : ISN_Singleton<IOSSocialManager>
{
	public static event Action OnFacebookPostStart;

	public static event Action OnTwitterPostStart;

	public static event Action OnInstagramPostStart;

	public static event Action<ISN_Result> OnFacebookPostResult;

	public static event Action<ISN_Result> OnTwitterPostResult;

	public static event Action<ISN_Result> OnInstagramPostResult;

	public static event Action<ISN_Result> OnMailResult;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void ShareMedia(string text)
	{
		ShareMedia(text, null);
	}

	public void ShareMedia(string text, Texture2D texture)
	{
	}

	public void TwitterPost(string text, string url = null, Texture2D texture = null)
	{
		IOSSocialManager.OnTwitterPostStart();
	}

	public void FacebookPost(string text, string url = null, Texture2D texture = null)
	{
		IOSSocialManager.OnFacebookPostStart();
	}

	public void InstagramPost(Texture2D texture)
	{
		InstagramPost(texture, string.Empty);
	}

	public void InstagramPost(Texture2D texture, string message)
	{
		IOSSocialManager.OnInstagramPostStart();
	}

	public void WhatsAppShareText(string message)
	{
	}

	public void WhatsAppShareImage(Texture2D texture)
	{
	}

	public void SendMail(string subject, string body, string recipients)
	{
		SendMail(subject, body, recipients, null);
	}

	public void SendMail(string subject, string body, string recipients, Texture2D texture)
	{
		if (!(texture == null))
		{
		}
	}

	private void OnTwitterPostFailed()
	{
		ISN_Result obj = new ISN_Result(IsResultSucceeded: false);
		IOSSocialManager.OnTwitterPostResult(obj);
	}

	private void OnTwitterPostSuccess()
	{
		ISN_Result obj = new ISN_Result(IsResultSucceeded: true);
		IOSSocialManager.OnTwitterPostResult(obj);
	}

	private void OnFacebookPostFailed()
	{
		ISN_Result obj = new ISN_Result(IsResultSucceeded: false);
		IOSSocialManager.OnFacebookPostResult(obj);
	}

	private void OnFacebookPostSuccess()
	{
		ISN_Result obj = new ISN_Result(IsResultSucceeded: true);
		IOSSocialManager.OnFacebookPostResult(obj);
	}

	private void OnMailFailed()
	{
		ISN_Result obj = new ISN_Result(IsResultSucceeded: false);
		IOSSocialManager.OnMailResult(obj);
	}

	private void OnMailSuccess()
	{
		ISN_Result obj = new ISN_Result(IsResultSucceeded: true);
		IOSSocialManager.OnMailResult(obj);
	}

	private void OnInstaPostSuccess()
	{
		ISN_Result obj = new ISN_Result(IsResultSucceeded: true);
		IOSSocialManager.OnInstagramPostResult(obj);
	}

	private void OnInstaPostFailed(string data)
	{
		int code = Convert.ToInt32(data);
		ISN_Error e = new ISN_Error(code, "Posting Failed");
		ISN_Result obj = new ISN_Result(e);
		IOSSocialManager.OnInstagramPostResult(obj);
	}

	static IOSSocialManager()
	{
		IOSSocialManager.OnFacebookPostStart = delegate
		{
		};
		IOSSocialManager.OnTwitterPostStart = delegate
		{
		};
		IOSSocialManager.OnInstagramPostStart = delegate
		{
		};
		IOSSocialManager.OnFacebookPostResult = delegate
		{
		};
		IOSSocialManager.OnTwitterPostResult = delegate
		{
		};
		IOSSocialManager.OnInstagramPostResult = delegate
		{
		};
		IOSSocialManager.OnMailResult = delegate
		{
		};
	}
}
