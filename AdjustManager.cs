using com.adjust.sdk;

public class AdjustManager
{
	private static AdjustManager _instance;

	public static AdjustManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AdjustManager();
			}
			return _instance;
		}
	}

	public void SimpleEvent(string token)
	{
		AdjustEvent adjustEvent = new AdjustEvent(token);
		Adjust.trackEvent(adjustEvent);
	}

	public void RevenueEvent(string token, double price)
	{
		AdjustEvent adjustEvent = new AdjustEvent(token);
		adjustEvent.setRevenue(price, "USD");
		Adjust.trackEvent(adjustEvent);
	}
}
