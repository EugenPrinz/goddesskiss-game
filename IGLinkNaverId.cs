public interface IGLinkNaverId
{
	void init(string clientId, string clientSecret);

	void login();

	void logout();

	bool isLogin();

	void getProfile();
}
