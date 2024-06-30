using UnityEngine;

public class AndroidManager : MonoBehaviour
{
	private AndroidJavaObject curActivity;

	private static AndroidManager _instance;

	public static AndroidManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(AndroidManager)) as AndroidManager;
				if (_instance == null)
				{
					_instance = new GameObject("AndroidManager").AddComponent<AndroidManager>();
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
	}

	public string GetRegistrationId()
	{
		if (curActivity == null)
		{
			return null;
		}
		return curActivity.Call<string>("GetRegistrationId", new object[0]);
	}

	public void CallJavaFunc(string strFuncName, string strTemp)
	{
		if (curActivity != null)
		{
			curActivity.Call(strFuncName, strTemp);
		}
	}
}
