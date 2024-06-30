namespace GooglePlayGames
{
	public static class GameInfo
	{
		private const string UnescapedApplicationId = "APP_ID";

		private const string UnescapedIosClientId = "IOS_CLIENTID";

		private const string UnescapedWebClientId = "WEB_CLIENTID";

		private const string UnescapedNearbyServiceId = "NEARBY_SERVICE_ID";

		public const string ApplicationId = "224399829814";

		public const string IosClientId = "224399829814-vr27hlartjsrootggm0lomdld1b3q9ms.apps.googleusercontent.com";

		public const string WebClientId = "224399829814-ehqt3o6il0r5km3f0l408i26ug6pdira.apps.googleusercontent.com";

		public const string NearbyConnectionServiceId = "";

		public static bool ApplicationIdInitialized()
		{
			return !string.IsNullOrEmpty("224399829814") && !"224399829814".Equals(ToEscapedToken("APP_ID"));
		}

		public static bool IosClientIdInitialized()
		{
			return !string.IsNullOrEmpty("224399829814-vr27hlartjsrootggm0lomdld1b3q9ms.apps.googleusercontent.com") && !"224399829814-vr27hlartjsrootggm0lomdld1b3q9ms.apps.googleusercontent.com".Equals(ToEscapedToken("IOS_CLIENTID"));
		}

		public static bool WebClientIdInitialized()
		{
			return !string.IsNullOrEmpty("224399829814-ehqt3o6il0r5km3f0l408i26ug6pdira.apps.googleusercontent.com") && !"224399829814-ehqt3o6il0r5km3f0l408i26ug6pdira.apps.googleusercontent.com".Equals(ToEscapedToken("WEB_CLIENTID"));
		}

		public static bool NearbyConnectionsInitialized()
		{
			return !string.IsNullOrEmpty(string.Empty) && !string.Empty.Equals(ToEscapedToken("NEARBY_SERVICE_ID"));
		}

		private static string ToEscapedToken(string token)
		{
			return $"__{token}__";
		}
	}
}
