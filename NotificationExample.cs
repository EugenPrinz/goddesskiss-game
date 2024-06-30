using System;
using UnityEngine;

public class NotificationExample : BaseIOSFeaturePreview
{
	private int lastNotificationId;

	private void Awake()
	{
		ISN_Singleton<IOSNotificationController>.instance.RequestNotificationPermissions();
		IOSNotificationController.OnLocalNotificationReceived += HandleOnLocalNotificationReceived;
		if (ISN_Singleton<IOSNotificationController>.Instance.LaunchNotification != null)
		{
			ISN_LocalNotification launchNotification = ISN_Singleton<IOSNotificationController>.Instance.LaunchNotification;
			IOSMessage.Create("Launch Notification", "Messgae: " + launchNotification.Message + "\nNotification Data: " + launchNotification.Data);
		}
	}

	private void OnGUI()
	{
		UpdateToStartPos();
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40f), "Local and Push Notifications", style);
		StartY += YLableStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Schedule Notification Silent"))
		{
			IOSNotificationController.OnNotificationScheduleResult += OnNotificationScheduleResult;
			ISN_LocalNotification iSN_LocalNotification = new ISN_LocalNotification(DateTime.Now.AddSeconds(5.0), "Your Notification Text No Sound", useSound: false);
			iSN_LocalNotification.SetData("some_test_data");
			iSN_LocalNotification.Schedule();
			lastNotificationId = iSN_LocalNotification.Id;
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Schedule Notification"))
		{
			IOSNotificationController.OnNotificationScheduleResult += OnNotificationScheduleResult;
			ISN_LocalNotification iSN_LocalNotification2 = new ISN_LocalNotification(DateTime.Now.AddSeconds(5.0), "Your Notification Text");
			iSN_LocalNotification2.SetData("some_test_data");
			iSN_LocalNotification2.SetSoundName("purchase_ok.wav");
			iSN_LocalNotification2.SetBadgesNumber(1);
			iSN_LocalNotification2.Schedule();
			lastNotificationId = iSN_LocalNotification2.Id;
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Cancel All Notifications"))
		{
			ISN_Singleton<IOSNotificationController>.instance.CancelAllLocalNotifications();
			IOSNativeUtility.SetApplicationBagesNumber(0);
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Cansel Last Notification"))
		{
			ISN_Singleton<IOSNotificationController>.instance.CancelLocalNotificationById(lastNotificationId);
		}
		StartX = XStartPos;
		StartY += YButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Reg Device For Push Notif. "))
		{
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Game Kit Notification"))
		{
			ISN_Singleton<IOSNotificationController>.instance.ShowGmaeKitNotification("Title", "Message");
		}
	}

	public void CheckNotificationSettings()
	{
		IOSNotificationController.OnNotificationSettingsInfoResult += HandleOnNotificationSettingsInfoResult;
		ISN_Singleton<IOSNotificationController>.Instance.RequestNotificationSettings();
	}

	private void HandleOnNotificationSettingsInfoResult(int avaliableTypes)
	{
		if ((avaliableTypes & 2) == 0)
		{
		}
	}

	private void OnDeviceTokenReceived(IOSNotificationDeviceToken token)
	{
		ISN_Logger.Log("OnTokenReceived");
		ISN_Logger.Log(token.tokenString);
		IOSDialog.Create("OnTokenReceived", token.tokenString);
		IOSNotificationController.OnDeviceTokenReceived -= OnDeviceTokenReceived;
	}

	private void HandleOnLocalNotificationReceived(ISN_LocalNotification notification)
	{
		IOSMessage.Create("Notification Received", "Messgae: " + notification.Message + "\nNotification Data: " + notification.Data);
	}

	private void OnNotificationScheduleResult(ISN_Result res)
	{
		IOSNotificationController.OnNotificationScheduleResult -= OnNotificationScheduleResult;
		string empty = string.Empty;
		if (res.IsSucceeded)
		{
			empty += "Notification was successfully scheduled\n allowed notifications types: \n";
			if (((uint)IOSNotificationController.AllowedNotificationsType & 4u) != 0)
			{
				empty += "Alert ";
			}
			if (((uint)IOSNotificationController.AllowedNotificationsType & 2u) != 0)
			{
				empty += "Sound ";
			}
			if (((uint)IOSNotificationController.AllowedNotificationsType & (true ? 1u : 0u)) != 0)
			{
				empty += "Badge ";
			}
		}
		else
		{
			empty += "Notification scheduling failed";
		}
		IOSMessage.Create("On Notification Schedule Result", empty);
	}
}
