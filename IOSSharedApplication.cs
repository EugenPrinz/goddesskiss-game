using System;

public class IOSSharedApplication : ISN_Singleton<IOSSharedApplication>
{
	public const string URL_SCHEME_EXISTS = "url_scheme_exists";

	public const string URL_SCHEME_NOT_FOUND = "url_scheme_not_found";

	public static event Action<ISN_CheckUrlResult> OnUrlCheckResultAction;

	public static event Action<string> OnAdvertisingIdentifierLoadedAction;

	public void CheckUrl(string url)
	{
	}

	public void OpenUrl(string url)
	{
	}

	public void GetAdvertisingIdentifier()
	{
	}

	private void UrlCheckSuccess(string url)
	{
		IOSSharedApplication.OnUrlCheckResultAction(new ISN_CheckUrlResult(url, IsResultSucceeded: true));
	}

	private void UrlCheckFailed(string url)
	{
		IOSSharedApplication.OnUrlCheckResultAction(new ISN_CheckUrlResult(url, IsResultSucceeded: false));
	}

	private void OnAdvertisingIdentifierLoaded(string Identifier)
	{
		IOSSharedApplication.OnAdvertisingIdentifierLoadedAction(Identifier);
	}

	static IOSSharedApplication()
	{
		IOSSharedApplication.OnUrlCheckResultAction = delegate
		{
		};
		IOSSharedApplication.OnAdvertisingIdentifierLoadedAction = delegate
		{
		};
	}
}
