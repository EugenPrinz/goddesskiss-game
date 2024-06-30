public interface IGLinkStatistics
{
	void sendNewUser(string gameUserId, string market);

	void sendPayUser(string gameUserId, double pay, string productCode, string currency, string market);
}
