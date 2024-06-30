using System.Collections.Generic;
using UnityEngine;

public class IOSNativeSettings : ScriptableObject
{
	public const string VERSION_NUMBER = "8.3.1";

	public string AppleId = "XXXXXXXXX";

	public int ToolbarIndex;

	public bool SendFakeEventsInEditor = true;

	public List<string> DefaultStoreProductsView = new List<string>();

	public List<IOSProductTemplate> InAppProducts = new List<IOSProductTemplate>();

	public List<GK_Leaderboard> Leaderboards = new List<GK_Leaderboard>();

	public List<GK_AchievementTemplate> Achievements = new List<GK_AchievementTemplate>();

	public bool checkInternetBeforeLoadRequest;

	public bool ShowStoreKitProducts = true;

	public bool ShowLeaderboards = true;

	public bool ShowAchievementsParams = true;

	public bool ShowUsersParams;

	public bool ShowOtherParams;

	public bool ShowRPKParams;

	public bool ExpandAPISettings = true;

	public bool EnableGameCenterAPI = true;

	public bool EnableInAppsAPI = true;

	public bool EnableCameraAPI = true;

	public bool EnableSocialSharingAPI = true;

	public bool EnableMediaPlayerAPI = true;

	public bool EnableiAdAPI = true;

	public bool EnableReplayKit;

	public bool EnableCloudKit;

	public bool EnableSoomla;

	public bool EnablePushNotificationsAPI;

	public int EditorFillRateIndex = 4;

	public int EditorFillRate = 100;

	public bool IsEditorTestingEnabled = true;

	public bool DisablePluginLogs;

	public bool UseGCRequestCaching;

	public bool UsePPForAchievements;

	public bool AutoLoadUsersSmallImages = true;

	public bool AutoLoadUsersBigImages;

	public int MaxImageLoadSize = 512;

	public float JPegCompressionRate = 0.8f;

	public IOSGalleryLoadImageFormat GalleryImageFormat = IOSGalleryLoadImageFormat.JPEG;

	public int RPK_iPadViewType;

	public string SoomlaDownloadLink = "http://goo.gl/7LYwuj";

	public string SoomlaDocsLink = "https://goo.gl/JFkpNa";

	public string SoomlaGameKey = string.Empty;

	public string SoomlaEnvKey = string.Empty;

	public bool OneSignalEnabled;

	public string OneSignalDocsLink = "https://goo.gl/Royty6";

	private const string ISNSettingsAssetName = "IOSNativeSettings";

	private const string ISNSettingsAssetExtension = ".asset";

	private static IOSNativeSettings instance;

	public static IOSNativeSettings Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load("IOSNativeSettings") as IOSNativeSettings;
				if (instance == null)
				{
					instance = ScriptableObject.CreateInstance<IOSNativeSettings>();
				}
			}
			return instance;
		}
	}
}
