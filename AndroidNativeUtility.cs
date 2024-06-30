using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AndroidNativeUtility : SA_Singleton<AndroidNativeUtility>
{
	public static int SDKLevel
	{
		get
		{
			IntPtr clazz = AndroidJNI.FindClass("android.os.Build$VERSION");
			IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
			return AndroidJNI.GetStaticIntField(clazz, staticFieldID);
		}
	}

	public static event Action<AN_PackageCheckResult> OnPackageCheckResult;

	public static event Action<string> OnAndroidIdLoaded;

	public static event Action<string> InternalStoragePathLoaded;

	public static event Action<string> ExternalStoragePathLoaded;

	public static event Action<AN_Locale> LocaleInfoLoaded;

	public static event Action<string[]> ActionDevicePackagesListLoaded;

	public static event Action<AN_NetworkInfo> ActionNetworkInfoLoaded;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void CheckIsPackageInstalled(string packageName)
	{
		AndroidNative.isPackageInstalled(packageName);
	}

	public void StartApplication(string bundle)
	{
		AndroidNative.runPackage(bundle);
	}

	public void StartApplication(string packageName, Dictionary<string, string> extras)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> extra in extras)
		{
			stringBuilder.AppendFormat("{0}{1}{2}", extra.Key, "|", extra.Value);
			stringBuilder.Append("|%|");
		}
		stringBuilder.Append("endofline");
		AndroidNative.LaunchApplication(packageName, stringBuilder.ToString());
	}

	public void LoadAndroidId()
	{
		AndroidNative.LoadAndroidId();
	}

	public void GetInternalStoragePath()
	{
		AndroidNative.GetInternalStoragePath();
	}

	public void GetExternalStoragePath()
	{
		AndroidNative.GetExternalStoragePath();
	}

	public string GetExternalStoragePublicDirectory(AN_ExternalStorageType type)
	{
		return AndroidNative.GetExternalStoragePublicDirectory(type.ToString());
	}

	public void LoadLocaleInfo()
	{
		AndroidNative.LoadLocaleInfo();
	}

	public void LoadPackagesList()
	{
		AndroidNative.LoadPackagesList();
	}

	public void LoadNetworkInfo()
	{
		AndroidNative.LoadNetworkInfo();
	}

	public static void OpenSettingsPage(string action)
	{
		AndroidNative.OpenSettingsPage(action);
	}

	public static void ShowPreloader(string title, string message)
	{
		AN_PoupsProxy.ShowPreloader(title, message);
	}

	public static void HidePreloader()
	{
		AN_PoupsProxy.HidePreloader();
	}

	public static void OpenAppRatingPage(string url)
	{
		AN_PoupsProxy.OpenAppRatePage(url);
	}

	public static void RedirectToGooglePlayRatingPage(string url)
	{
		OpenAppRatingPage(url);
	}

	public static void HideCurrentPopup()
	{
		AN_PoupsProxy.HideCurrentPopup();
	}

	public static void InvitePlusFriends()
	{
		AndroidNative.InvitePlusFriends();
	}

	private void OnAndroidIdLoadedEvent(string id)
	{
		AndroidNativeUtility.OnAndroidIdLoaded(id);
	}

	private void OnPacakgeFound(string packageName)
	{
		AN_PackageCheckResult obj = new AN_PackageCheckResult(packageName, IsResultSucceeded: true);
		AndroidNativeUtility.OnPackageCheckResult(obj);
	}

	private void OnPacakgeNotFound(string packageName)
	{
		AN_PackageCheckResult obj = new AN_PackageCheckResult(packageName, IsResultSucceeded: false);
		AndroidNativeUtility.OnPackageCheckResult(obj);
	}

	private void OnExternalStoragePathLoaded(string path)
	{
		AndroidNativeUtility.ExternalStoragePathLoaded(path);
	}

	private void OnInternalStoragePathLoaded(string path)
	{
		AndroidNativeUtility.InternalStoragePathLoaded(path);
	}

	private void OnLocaleInfoLoaded(string data)
	{
		string[] array = data.Split("|"[0]);
		AN_Locale aN_Locale = new AN_Locale();
		aN_Locale.CountryCode = array[0];
		aN_Locale.DisplayCountry = array[1];
		aN_Locale.LanguageCode = array[2];
		aN_Locale.DisplayLanguage = array[3];
		AndroidNativeUtility.LocaleInfoLoaded(aN_Locale);
	}

	private void OnPackagesListLoaded(string data)
	{
		string[] obj = data.Split("|"[0]);
		AndroidNativeUtility.ActionDevicePackagesListLoaded(obj);
	}

	private void OnNetworkInfoLoaded(string data)
	{
		string[] array = data.Split("|"[0]);
		AN_NetworkInfo aN_NetworkInfo = new AN_NetworkInfo();
		aN_NetworkInfo.SubnetMask = array[0];
		aN_NetworkInfo.IpAddress = array[1];
		aN_NetworkInfo.MacAddress = array[2];
		aN_NetworkInfo.SSID = array[3];
		aN_NetworkInfo.BSSID = array[4];
		aN_NetworkInfo.LinkSpeed = Convert.ToInt32(array[5]);
		aN_NetworkInfo.NetworkId = Convert.ToInt32(array[6]);
		AndroidNativeUtility.ActionNetworkInfoLoaded(aN_NetworkInfo);
	}

	static AndroidNativeUtility()
	{
		AndroidNativeUtility.OnPackageCheckResult = delegate
		{
		};
		AndroidNativeUtility.OnAndroidIdLoaded = delegate
		{
		};
		AndroidNativeUtility.InternalStoragePathLoaded = delegate
		{
		};
		AndroidNativeUtility.ExternalStoragePathLoaded = delegate
		{
		};
		AndroidNativeUtility.LocaleInfoLoaded = delegate
		{
		};
		AndroidNativeUtility.ActionDevicePackagesListLoaded = delegate
		{
		};
		AndroidNativeUtility.ActionNetworkInfoLoaded = delegate
		{
		};
	}
}
