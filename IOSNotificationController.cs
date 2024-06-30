using System;
using System.Collections.Generic;
using UnityEngine;

public class IOSNotificationController : ISN_Singleton<IOSNotificationController>
{
	private static IOSNotificationController _instance;

	private static int _AllowedNotificationsType = -1;

	private const string PP_KEY = "IOSNotificationControllerKey";

	private const string PP_ID_KEY = "IOSNotificationControllerrKey_ID";

	private ISN_LocalNotification _LaunchNotification;

	public static int AllowedNotificationsType => _AllowedNotificationsType;

	public ISN_LocalNotification LaunchNotification => _LaunchNotification;

	public static event Action<IOSNotificationDeviceToken> OnDeviceTokenReceived;

	public static event Action<ISN_Result> OnNotificationScheduleResult;

	public static event Action<int> OnNotificationSettingsInfoResult;

	public static event Action<ISN_LocalNotification> OnLocalNotificationReceived;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void RequestNotificationPermissions()
	{
		if (ISN_Device.CurrentDevice.MajorSystemVersion >= 8)
		{
		}
	}

	public void ShowGmaeKitNotification(string title, string message)
	{
		GameCenterManager.ShowGmaeKitNotification(title, message);
	}

	[Obsolete("ShowNotificationBanner is deprecated, please use ShowGmaeKitNotification instead.")]
	public void ShowNotificationBanner(string title, string message)
	{
		ShowGmaeKitNotification(title, message);
	}

	[Obsolete("CancelNotifications is deprecated, please use CancelAllLocalNotifications instead.")]
	public void CancelNotifications()
	{
		CancelAllLocalNotifications();
	}

	public void CancelAllLocalNotifications()
	{
		SaveNotifications(new List<ISN_LocalNotification>());
	}

	public void CancelLocalNotification(ISN_LocalNotification notification)
	{
		CancelLocalNotificationById(notification.Id);
	}

	public void CancelLocalNotificationById(int notificationId)
	{
	}

	public void ScheduleNotification(ISN_LocalNotification notification)
	{
	}

	public List<ISN_LocalNotification> LoadPendingNotifications(bool includeAll = false)
	{
		return null;
	}

	public void ApplicationIconBadgeNumber(int badges)
	{
	}

	public void RequestNotificationSettings()
	{
	}

	public void OnDeviceTockeReceivedAction(IOSNotificationDeviceToken token)
	{
		IOSNotificationController.OnDeviceTokenReceived(token);
	}

	private void OnNotificationScheduleResultAction(string array)
	{
		string[] array2 = array.Split("|"[0]);
		ISN_Result iSN_Result = null;
		iSN_Result = ((!array2[0].Equals("0")) ? new ISN_Result(IsResultSucceeded: true) : new ISN_Result(IsResultSucceeded: false));
		_AllowedNotificationsType = Convert.ToInt32(array2[1]);
		IOSNotificationController.OnNotificationScheduleResult(iSN_Result);
	}

	private void OnNotificationSettingsInfoRetrived(string data)
	{
		int obj = Convert.ToInt32(data);
		IOSNotificationController.OnNotificationSettingsInfoResult(obj);
	}

	private void OnLocalNotificationReceived_Event(string array)
	{
		string[] array2 = array.Split("|"[0]);
		string message = array2[0];
		int id = Convert.ToInt32(array2[1]);
		string data = array2[2];
		int badgesNumber = Convert.ToInt32(array2[3]);
		ISN_LocalNotification iSN_LocalNotification = new ISN_LocalNotification(DateTime.Now, message);
		iSN_LocalNotification.SetData(data);
		iSN_LocalNotification.SetBadgesNumber(badgesNumber);
		iSN_LocalNotification.SetId(id);
		IOSNotificationController.OnLocalNotificationReceived(iSN_LocalNotification);
	}

	private void SaveNotifications(List<ISN_LocalNotification> notifications)
	{
		if (notifications.Count == 0)
		{
			PlayerPrefs.DeleteKey("IOSNotificationControllerKey");
			return;
		}
		string text = string.Empty;
		int count = notifications.Count;
		for (int i = 0; i < count; i++)
		{
			if (i != 0)
			{
				text += '|';
			}
			text += notifications[i].SerializedString;
		}
		PlayerPrefs.SetString("IOSNotificationControllerKey", text);
	}

	static IOSNotificationController()
	{
		IOSNotificationController.OnDeviceTokenReceived = delegate
		{
		};
		IOSNotificationController.OnNotificationScheduleResult = delegate
		{
		};
		IOSNotificationController.OnNotificationSettingsInfoResult = delegate
		{
		};
		IOSNotificationController.OnLocalNotificationReceived = delegate
		{
		};
	}
}
