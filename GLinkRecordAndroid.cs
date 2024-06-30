using UnityEngine;

public class GLinkRecordAndroid : IGLinkRecord
{
	private class OnRecordManagerListener : AndroidJavaProxy
	{
		public OnRecordManagerListener()
			: base("com.naver.glink.android.sdk.PlugRecordManager$OnRecordManagerListener")
		{
		}

		private void onStartRecord()
		{
			showToast("start record");
		}

		private void onErrorRecord()
		{
			showToast("record error");
		}

		private void onFinishRecord(string uri)
		{
		}
	}

	private AndroidJavaClass delegateClass;

	private AndroidJavaObject currentActivity;

	public GLinkRecordAndroid()
	{
		delegateClass = new AndroidJavaClass("com.naver.glink.android.sdk.PlugRecordManager");
		currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		delegateClass.CallStatic("setOnRecordManagerListener", new OnRecordManagerListener());
	}

	private static void showToast(string message)
	{
		AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaClass("android.widget.Toast").CallStatic<AndroidJavaObject>("makeText", new object[3] { activity, message, 1 });
			androidJavaObject.Call("show");
		});
	}

	public void startRecord()
	{
		GLink.sharedInstance();
		delegateClass.CallStatic("startRecord", currentActivity);
	}

	public void stopRecord()
	{
		delegateClass.CallStatic("stopRecord");
	}
}
