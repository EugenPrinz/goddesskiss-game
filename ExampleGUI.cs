using System;
using com.adjust.sdk;
using UnityEngine;

public class ExampleGUI : MonoBehaviour
{
	private int numberOfButtons = 8;

	private bool isEnabled;

	private bool showPopUp;

	private string txtSetEnabled = "Disable SDK";

	private string txtManualLaunch = "Manual Launch";

	private string txtSetOfflineMode = "Turn Offline Mode ON";

	private void OnGUI()
	{
		if (showPopUp)
		{
			GUI.Window(0, new Rect(Screen.width / 2 - 150, Screen.height / 2 - 65, 300f, 130f), showGUI, "Is SDK enabled?");
		}
		if (GUI.Button(new Rect(0f, Screen.height * 0 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), txtManualLaunch) && !string.Equals(txtManualLaunch, "SDK Launched", StringComparison.OrdinalIgnoreCase))
		{
			AdjustConfig adjustConfig = new AdjustConfig("2fm9gkqubvpc", AdjustEnvironment.Sandbox);
			adjustConfig.setLogLevel(AdjustLogLevel.Verbose);
			adjustConfig.setLogDelegate(delegate
			{
			});
			adjustConfig.setSendInBackground(sendInBackground: true);
			adjustConfig.setLaunchDeferredDeeplink(launchDeferredDeeplink: true);
			adjustConfig.setEventSuccessDelegate(EventSuccessCallback);
			adjustConfig.setEventFailureDelegate(EventFailureCallback);
			adjustConfig.setSessionSuccessDelegate(SessionSuccessCallback);
			adjustConfig.setSessionFailureDelegate(SessionFailureCallback);
			adjustConfig.setDeferredDeeplinkDelegate(DeferredDeeplinkCallback);
			adjustConfig.setAttributionChangedDelegate(AttributionChangedCallback);
			Adjust.start(adjustConfig);
			isEnabled = true;
			txtManualLaunch = "SDK Launched";
		}
		if (GUI.Button(new Rect(0f, Screen.height / numberOfButtons, Screen.width, Screen.height / numberOfButtons), "Track Simple Event"))
		{
			AdjustEvent adjustEvent = new AdjustEvent("g3mfiw");
			Adjust.trackEvent(adjustEvent);
		}
		if (GUI.Button(new Rect(0f, Screen.height * 2 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), "Track Revenue Event"))
		{
			AdjustEvent adjustEvent2 = new AdjustEvent("a4fd35");
			adjustEvent2.setRevenue(0.25, "EUR");
			Adjust.trackEvent(adjustEvent2);
		}
		if (GUI.Button(new Rect(0f, Screen.height * 3 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), "Track Callback Event"))
		{
			AdjustEvent adjustEvent3 = new AdjustEvent("34vgg9");
			adjustEvent3.addCallbackParameter("key", "value");
			adjustEvent3.addCallbackParameter("foo", "bar");
			Adjust.trackEvent(adjustEvent3);
		}
		if (GUI.Button(new Rect(0f, Screen.height * 4 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), "Track Partner Event"))
		{
			AdjustEvent adjustEvent4 = new AdjustEvent("w788qs");
			adjustEvent4.addPartnerParameter("key", "value");
			adjustEvent4.addPartnerParameter("foo", "bar");
			Adjust.trackEvent(adjustEvent4);
		}
		if (GUI.Button(new Rect(0f, Screen.height * 5 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), txtSetOfflineMode))
		{
			if (string.Equals(txtSetOfflineMode, "Turn Offline Mode ON", StringComparison.OrdinalIgnoreCase))
			{
				Adjust.setOfflineMode(enabled: true);
				txtSetOfflineMode = "Turn Offline Mode OFF";
			}
			else
			{
				Adjust.setOfflineMode(enabled: false);
				txtSetOfflineMode = "Turn Offline Mode ON";
			}
		}
		if (GUI.Button(new Rect(0f, Screen.height * 6 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), txtSetEnabled))
		{
			if (string.Equals(txtSetEnabled, "Disable SDK", StringComparison.OrdinalIgnoreCase))
			{
				Adjust.setEnabled(enabled: false);
				txtSetEnabled = "Enable SDK";
			}
			else
			{
				Adjust.setEnabled(enabled: true);
				txtSetEnabled = "Disable SDK";
			}
		}
		if (GUI.Button(new Rect(0f, Screen.height * 7 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), "Is SDK Enabled?"))
		{
			isEnabled = Adjust.isEnabled();
			showPopUp = true;
		}
	}

	private void showGUI(int windowID)
	{
		if (isEnabled)
		{
			GUI.Label(new Rect(65f, 40f, 200f, 30f), "Adjust SDK is ENABLED!");
		}
		else
		{
			GUI.Label(new Rect(65f, 40f, 200f, 30f), "Adjust SDK is DISABLED!");
		}
		if (GUI.Button(new Rect(90f, 75f, 120f, 40f), "OK"))
		{
			showPopUp = false;
		}
	}

	public void handleGooglePlayId(string adId)
	{
	}

	public void AttributionChangedCallback(AdjustAttribution attributionData)
	{
		if (attributionData.trackerName != null)
		{
		}
		if (attributionData.trackerToken != null)
		{
		}
		if (attributionData.network != null)
		{
		}
		if (attributionData.campaign != null)
		{
		}
		if (attributionData.adgroup != null)
		{
		}
		if (attributionData.creative != null)
		{
		}
		if (attributionData.clickLabel != null)
		{
		}
		if (attributionData.adid == null)
		{
		}
	}

	public void EventSuccessCallback(AdjustEventSuccess eventSuccessData)
	{
		if (eventSuccessData.Message != null)
		{
		}
		if (eventSuccessData.Timestamp != null)
		{
		}
		if (eventSuccessData.Adid != null)
		{
		}
		if (eventSuccessData.EventToken != null)
		{
		}
		if (eventSuccessData.JsonResponse == null)
		{
		}
	}

	public void EventFailureCallback(AdjustEventFailure eventFailureData)
	{
		if (eventFailureData.Message != null)
		{
		}
		if (eventFailureData.Timestamp != null)
		{
		}
		if (eventFailureData.Adid != null)
		{
		}
		if (eventFailureData.EventToken != null)
		{
		}
		if (eventFailureData.JsonResponse == null)
		{
		}
	}

	public void SessionSuccessCallback(AdjustSessionSuccess sessionSuccessData)
	{
		if (sessionSuccessData.Message != null)
		{
		}
		if (sessionSuccessData.Timestamp != null)
		{
		}
		if (sessionSuccessData.Adid != null)
		{
		}
		if (sessionSuccessData.JsonResponse == null)
		{
		}
	}

	public void SessionFailureCallback(AdjustSessionFailure sessionFailureData)
	{
		if (sessionFailureData.Message != null)
		{
		}
		if (sessionFailureData.Timestamp != null)
		{
		}
		if (sessionFailureData.Adid != null)
		{
		}
		if (sessionFailureData.JsonResponse == null)
		{
		}
	}

	private void DeferredDeeplinkCallback(string deeplinkURL)
	{
		if (deeplinkURL == null)
		{
		}
	}
}
