public class GLinkStatistics
{
	private static IGLinkStatistics glinkStatistics;

	public const string kCurrencyNone = "NONE";

	public const string kCurrencyWon = "WON";

	public const string kCurrencyDollar = "DOLLAR";

	public const string kMarketNone = "NONE";

	public const string kMarketOne = "ONE";

	public const string kMarketGoogle = "GOOGLE";

	public const string kMarketApple = "APPLE";

	public static IGLinkStatistics sharedInstance()
	{
		if (glinkStatistics == null)
		{
			glinkStatistics = new GLinkStatisticsAndroid();
		}
		return glinkStatistics;
	}
}
