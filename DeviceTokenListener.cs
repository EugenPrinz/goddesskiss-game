using UnityEngine;

public class DeviceTokenListener : MonoBehaviour
{
	public static void Create()
	{
		new GameObject("DeviceTokenListener").AddComponent<DeviceTokenListener>();
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
