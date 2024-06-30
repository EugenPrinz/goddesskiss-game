using UnityEngine;

public class AppEventHandlerExample : MonoBehaviour
{
	private void Awake()
	{
		ISN_Singleton<IOSNativeAppEvents>.Instance.Subscribe();
		IOSNativeAppEvents.OnApplicationDidReceiveMemoryWarning += OnApplicationDidReceiveMemoryWarning;
		IOSNativeAppEvents.OnApplicationDidBecomeActive += HandleOnApplicationDidBecomeActive;
	}

	private void HandleOnApplicationDidBecomeActive()
	{
		ISN_Logger.Log("Caught OnApplicationDidBecomeActive event");
	}

	private void OnApplicationDidReceiveMemoryWarning()
	{
		ISN_Logger.Log("Caught OnApplicationDidReceiveMemoryWarning event");
	}
}
