using IapError;
using IapResponse;
using UnityEngine;

public class OneStoreIapManager : MonoBehaviour
{
	private AndroidJavaObject unityPlayerClass;

	private AndroidJavaObject currentActivity;

	private AndroidJavaObject onestoreIapManager;

	private string strLabelPayment = string.Empty;

	private string strLabelQuery = string.Empty;

	private const string appId = "OA00704793";

	private bool DEBUG_MODE;

	private static OneStoreIapManager _instance;

	public static OneStoreIapManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(OneStoreIapManager)) as OneStoreIapManager;
				if (_instance == null)
				{
					_instance = new GameObject("OneStoreIapManager").AddComponent<OneStoreIapManager>();
				}
			}
			return _instance;
		}
	}

	public void Init()
	{
		unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
		if (currentActivity != null)
		{
			onestoreIapManager = new AndroidJavaObject("com.onestore.iap.unity.RequestAdapter", "OneStoreIapManager", currentActivity, DEBUG_MODE);
		}
	}

	private void Destroy()
	{
		if (unityPlayerClass != null)
		{
			unityPlayerClass.Dispose();
		}
		if (currentActivity != null)
		{
			currentActivity.Dispose();
		}
		if (onestoreIapManager != null)
		{
			onestoreIapManager.Dispose();
		}
	}

	public void RequestPaymenet(string pId)
	{
		onestoreIapManager.Call("requestPayment", "OA00704793", pId, string.Empty, string.Empty, string.Empty);
	}

	public void PaymentResponse(string response)
	{
		Response response2 = JsonUtility.FromJson<Response>(response);
		if (!string.IsNullOrEmpty(response2.result.txid) && !string.IsNullOrEmpty(response2.result.receipt))
		{
			RemoteObjectManager.instance.RequestCheckPaymentOneStore(response2.result.txid, response2.result.receipt);
		}
	}

	public void PaymentError(string message)
	{
		Error error = JsonUtility.FromJson<Error>(message);
	}
}
