using UnityEngine;

public class GLinkNaverIdAndroid : IGLinkNaverId
{
	private class OnLoggedInListener : AndroidJavaProxy
	{
		public OnLoggedInListener()
			: base("com.naver.glink.android.sdk.Glink$OnLoggedInListener")
		{
		}

		private void onLoggedIn(bool success)
		{
		}
	}

	private class OnGetProfileListener : AndroidJavaProxy
	{
		public OnGetProfileListener()
			: base("com.naver.glink.android.sdk.NaverIdLogin$OnGetProfileListener")
		{
		}

		private void onResult(string jsonString)
		{
		}
	}

	private AndroidJavaClass delegateClass;

	private AndroidJavaObject currentActivity;

	public GLinkNaverIdAndroid()
	{
		delegateClass = new AndroidJavaClass("com.naver.glink.android.sdk.NaverIdLogin");
		currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
	}

	public void init(string clientId, string clientSecret)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.naver.glink.android.sdk.Glink");
		androidJavaClass.CallStatic("init", currentActivity, clientId, clientSecret, -1);
	}

	public void login()
	{
		delegateClass.CallStatic("login", currentActivity, new OnLoggedInListener());
	}

	public void logout()
	{
		delegateClass.CallStatic("logout", currentActivity);
	}

	public bool isLogin()
	{
		return delegateClass.CallStatic<bool>("isLogin", new object[1] { currentActivity });
	}

	public void getProfile()
	{
		delegateClass.CallStatic("getProfile", currentActivity, new OnGetProfileListener());
	}
}
