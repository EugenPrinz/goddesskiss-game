using System;
using UnityEngine;

public class IOSNativeAppEvents : ISN_Singleton<IOSNativeAppEvents>
{
	public static event Action OnApplicationDidEnterBackground;

	public static event Action OnApplicationDidBecomeActive;

	public static event Action OnApplicationDidReceiveMemoryWarning;

	public static event Action OnApplicationWillResignActive;

	public static event Action OnApplicationWillTerminate;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Subscribe()
	{
	}

	private void applicationDidEnterBackground()
	{
		IOSNativeAppEvents.OnApplicationDidEnterBackground();
	}

	private void applicationDidBecomeActive()
	{
		IOSNativeAppEvents.OnApplicationDidBecomeActive();
	}

	private void applicationDidReceiveMemoryWarning()
	{
		IOSNativeAppEvents.OnApplicationDidReceiveMemoryWarning();
	}

	private void applicationWillResignActive()
	{
		IOSNativeAppEvents.OnApplicationWillResignActive();
	}

	private void applicationWillTerminate()
	{
		IOSNativeAppEvents.OnApplicationWillTerminate();
	}

	static IOSNativeAppEvents()
	{
		IOSNativeAppEvents.OnApplicationDidEnterBackground = delegate
		{
		};
		IOSNativeAppEvents.OnApplicationDidBecomeActive = delegate
		{
		};
		IOSNativeAppEvents.OnApplicationDidReceiveMemoryWarning = delegate
		{
		};
		IOSNativeAppEvents.OnApplicationWillResignActive = delegate
		{
		};
		IOSNativeAppEvents.OnApplicationWillTerminate = delegate
		{
		};
	}
}
