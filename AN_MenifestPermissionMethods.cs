internal static class AN_MenifestPermissionMethods
{
	public static string GetFullName(this AN_ManifestPermission permission)
	{
		string text = "android.permission.";
		switch (permission)
		{
		case AN_ManifestPermission.SET_ALARM:
			text = "com.android.alarm.permission.";
			break;
		case AN_ManifestPermission.INSTALL_SHORTCUT:
		case AN_ManifestPermission.UNINSTALL_SHORTCUT:
			text = "com.android.launcher.permission.";
			break;
		case AN_ManifestPermission.ADD_VOICEMAIL:
			text = "com.android.voicemail.permission.";
			break;
		}
		return text + permission;
	}

	public static bool IsNormalPermission(this AN_ManifestPermission permission)
	{
		switch (permission)
		{
		case AN_ManifestPermission.ACCESS_LOCATION_EXTRA_COMMANDS:
		case AN_ManifestPermission.ACCESS_NETWORK_STATE:
		case AN_ManifestPermission.ACCESS_NOTIFICATION_POLICY:
		case AN_ManifestPermission.ACCESS_WIFI_STATE:
		case AN_ManifestPermission.ACCESS_WIMAX_STATE:
		case AN_ManifestPermission.BLUETOOTH:
		case AN_ManifestPermission.BLUETOOTH_ADMIN:
		case AN_ManifestPermission.BROADCAST_STICKY:
		case AN_ManifestPermission.CHANGE_NETWORK_STATE:
		case AN_ManifestPermission.CHANGE_WIFI_MULTICAST_STATE:
		case AN_ManifestPermission.CHANGE_WIFI_STATE:
		case AN_ManifestPermission.CHANGE_WIMAX_STATE:
		case AN_ManifestPermission.DISABLE_KEYGUARD:
		case AN_ManifestPermission.EXPAND_STATUS_BAR:
		case AN_ManifestPermission.FLASHLIGHT:
		case AN_ManifestPermission.GET_PACKAGE_SIZE:
		case AN_ManifestPermission.INTERNET:
		case AN_ManifestPermission.KILL_BACKGROUND_PROCESSES:
		case AN_ManifestPermission.MODIFY_AUDIO_SETTINGS:
		case AN_ManifestPermission.NFC:
		case AN_ManifestPermission.READ_SYNC_SETTINGS:
		case AN_ManifestPermission.READ_SYNC_STATS:
		case AN_ManifestPermission.RECEIVE_BOOT_COMPLETED:
		case AN_ManifestPermission.REORDER_TASKS:
		case AN_ManifestPermission.REQUEST_INSTALL_PACKAGES:
		case AN_ManifestPermission.SET_TIME_ZONE:
		case AN_ManifestPermission.SET_WALLPAPER:
		case AN_ManifestPermission.SET_WALLPAPER_HINTS:
		case AN_ManifestPermission.SUBSCRIBED_FEEDS_READ:
		case AN_ManifestPermission.TRANSMIT_IR:
		case AN_ManifestPermission.USE_FINGERPRINT:
		case AN_ManifestPermission.VIBRATE:
		case AN_ManifestPermission.WAKE_LOCK:
		case AN_ManifestPermission.WRITE_SYNC_SETTINGS:
		case AN_ManifestPermission.SET_ALARM:
		case AN_ManifestPermission.INSTALL_SHORTCUT:
		case AN_ManifestPermission.UNINSTALL_SHORTCUT:
			return true;
		default:
			return false;
		}
	}
}
