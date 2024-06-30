using UnityEngine;

public class GLinkStatisticsAndroid : IGLinkStatistics
{
	public void sendNewUser(string gameUserId, string market)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.naver.glink.android.sdk.Statistics");
		androidJavaClass.CallStatic("sendNewUser", gameUserId, market);
	}

	public void sendPayUser(string gameUserId, double pay, string productCode, string currency, string market)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.naver.glink.android.sdk.Statistics");
		androidJavaClass.CallStatic("sendPayUser", gameUserId, pay, productCode, currency, market);
	}
}
