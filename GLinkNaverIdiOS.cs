using AOT;

public class GLinkNaverIdiOS : IGLinkNaverId
{
	private delegate void NaverIdLoginDelelgate();

	private delegate void NaverIdGetProfileDelegate(string result);

	[MonoPInvokeCallback(typeof(NaverIdLoginDelelgate))]
	public static void _NaverIdLoginCallback()
	{
	}

	[MonoPInvokeCallback(typeof(NaverIdGetProfileDelegate))]
	public static void _NaverIdGetProfileCallback(string result)
	{
	}

	public void init(string clientId, string clientSecret)
	{
	}

	public void login()
	{
	}

	public void logout()
	{
	}

	public bool isLogin()
	{
		return false;
	}

	public void getProfile()
	{
	}
}
