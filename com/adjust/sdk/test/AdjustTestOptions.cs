using UnityEngine;

namespace com.adjust.sdk.test
{
	public class AdjustTestOptions
	{
		public string BaseUrl { get; set; }

		public string GdprUrl { get; set; }

		public string BasePath { get; set; }

		public string GdprPath { get; set; }

		public bool? Teardown { get; set; }

		public bool? DeleteState { get; set; }

		public bool? UseTestConnectionOptions { get; set; }

		public bool? NoBackoffWait { get; set; }

		public long? TimerIntervalInMilliseconds { get; set; }

		public long? TimerStartInMilliseconds { get; set; }

		public long? SessionIntervalInMilliseconds { get; set; }

		public long? SubsessionIntervalInMilliseconds { get; set; }

		public AndroidJavaObject ToAndroidJavaObject(AndroidJavaObject ajoCurrentActivity)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.adjust.sdk.AdjustTestOptions");
			androidJavaObject.Set("baseUrl", BaseUrl);
			androidJavaObject.Set("gdprUrl", GdprUrl);
			if (!string.IsNullOrEmpty(BasePath))
			{
				androidJavaObject.Set("basePath", BasePath);
			}
			if (!string.IsNullOrEmpty(GdprPath))
			{
				androidJavaObject.Set("gdprPath", GdprPath);
			}
			if (DeleteState.GetValueOrDefault(false) && ajoCurrentActivity != null)
			{
				androidJavaObject.Set("context", ajoCurrentActivity);
			}
			if (UseTestConnectionOptions.HasValue)
			{
				AndroidJavaObject val = new AndroidJavaObject("java.lang.Boolean", UseTestConnectionOptions.Value);
				androidJavaObject.Set("useTestConnectionOptions", val);
			}
			if (TimerIntervalInMilliseconds.HasValue)
			{
				AndroidJavaObject val2 = new AndroidJavaObject("java.lang.Long", TimerIntervalInMilliseconds.Value);
				androidJavaObject.Set("timerIntervalInMilliseconds", val2);
			}
			if (TimerStartInMilliseconds.HasValue)
			{
				AndroidJavaObject val3 = new AndroidJavaObject("java.lang.Long", TimerStartInMilliseconds.Value);
				androidJavaObject.Set("timerStartInMilliseconds", val3);
			}
			if (SessionIntervalInMilliseconds.HasValue)
			{
				AndroidJavaObject val4 = new AndroidJavaObject("java.lang.Long", SessionIntervalInMilliseconds.Value);
				androidJavaObject.Set("sessionIntervalInMilliseconds", val4);
			}
			if (SubsessionIntervalInMilliseconds.HasValue)
			{
				AndroidJavaObject val5 = new AndroidJavaObject("java.lang.Long", SubsessionIntervalInMilliseconds.Value);
				androidJavaObject.Set("subsessionIntervalInMilliseconds", val5);
			}
			if (Teardown.HasValue)
			{
				AndroidJavaObject val6 = new AndroidJavaObject("java.lang.Boolean", Teardown.Value);
				androidJavaObject.Set("teardown", val6);
			}
			if (NoBackoffWait.HasValue)
			{
				AndroidJavaObject val7 = new AndroidJavaObject("java.lang.Boolean", NoBackoffWait.Value);
				androidJavaObject.Set("noBackoffWait", val7);
			}
			return androidJavaObject;
		}
	}
}
