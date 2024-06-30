using System;

public class AN_NotificationProxy
{
	private const string CLASS_NAME = "com.androidnative.features.notifications.LocalNotificationManager";

	private static void CallActivityFunction(string methodName, params object[] args)
	{
		AN_ProxyPool.CallStatic("com.androidnative.features.notifications.LocalNotificationManager", methodName, args);
	}

	public static void ShowToastNotification(string text, int duration)
	{
		CallActivityFunction("ShowToastNotification", text, duration.ToString());
	}

	public static void HideAllNotifications()
	{
		CallActivityFunction("HideAllNotifications");
	}

	public static void requestCurrentAppLaunchNotificationId()
	{
		CallActivityFunction("requestCurrentAppLaunchNotificationId");
	}

	public static void ScheduleLocalNotification(AndroidNotificationBuilder builder)
	{
		CallActivityFunction("ScheduleLocalNotification", builder.Title, builder.Message, builder.Time.ToString(), builder.Id.ToString(), builder.Icon, builder.Sound, builder.Vibration.ToString(), builder.ShowIfAppForeground.ToString(), builder.LargeIcon, (!(builder.BigPicture == null)) ? Convert.ToBase64String(builder.BigPicture.EncodeToPNG()) : string.Empty, (builder.Color != null) ? $"{255f * builder.Color.Value.a}|{255f * builder.Color.Value.r}|{255f * builder.Color.Value.g}|{255f * builder.Color.Value.b}" : string.Empty, builder.WakeLockTime);
	}

	public static void CanselLocalNotification(int id)
	{
		CallActivityFunction("canselLocalNotification", id.ToString());
	}

	public static void InitPushNotifications(string smallIcon, string largeIcon, string sound, bool vibration, bool showWhenAppForeground, bool replaceOldNotificationWithNew, string color)
	{
		CallActivityFunction("InitPushNotifications", smallIcon, largeIcon, sound, vibration.ToString(), showWhenAppForeground.ToString(), replaceOldNotificationWithNew.ToString(), color);
	}

	public static void GCMRgisterDevice(string senderId)
	{
		CallActivityFunction("GCMRgisterDevice", senderId);
	}

	public static void GCMLoadLastMessage()
	{
		CallActivityFunction("GCMLoadLastMessage");
	}

	public static void GCMRemoveLastMessageInfo()
	{
		CallActivityFunction("GCMRemoveLastMessageInfo");
	}
}
