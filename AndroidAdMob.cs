public static class AndroidAdMob
{
	private static GoogleMobileAdInterface _Client;

	public static GoogleMobileAdInterface Client
	{
		get
		{
			if (_Client == null)
			{
				_Client = SA_Singleton<AndroidAdMobController>.Instance;
				_Client.InitEditorTesting(AndroidNativeSettings.Instance.Is_Ad_EditorTestingEnabled, AndroidNativeSettings.Instance.Ad_EditorFillRate);
			}
			return _Client;
		}
	}
}
