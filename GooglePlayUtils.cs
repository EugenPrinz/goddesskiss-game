using System;
using UnityEngine;

public class GooglePlayUtils : SA_Singleton<GooglePlayUtils>
{
	public static Action<GP_AdvertisingIdLoadResult> ActionAdvertisingIdLoaded = delegate
	{
	};

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void GetAdvertisingId()
	{
		AN_GooglePlayUtilsProxy.GetAdvertisingId();
	}

	private void OnAdvertisingIdLoaded(string data)
	{
		string[] array = data.Split("|"[0]);
		string text = array[0];
		bool isLimitAdTrackingEnabled = Convert.ToBoolean(array[1]);
		GP_AdvertisingIdLoadResult gP_AdvertisingIdLoadResult;
		if (text != null && text.Length > 0)
		{
			gP_AdvertisingIdLoadResult = new GP_AdvertisingIdLoadResult(IsResultSucceeded: true);
			gP_AdvertisingIdLoadResult.id = text;
			gP_AdvertisingIdLoadResult.isLimitAdTrackingEnabled = isLimitAdTrackingEnabled;
		}
		else
		{
			gP_AdvertisingIdLoadResult = new GP_AdvertisingIdLoadResult(IsResultSucceeded: false);
		}
		ActionAdvertisingIdLoaded(gP_AdvertisingIdLoadResult);
	}
}
