using System;
using UnityEngine;

public abstract class ISN_Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	private static bool applicationIsQuitting;

	[Obsolete("instance is deprecated, please use Instance instead.")]
	public static T instance => Instance;

	public static T Instance
	{
		get
		{
			if (applicationIsQuitting)
			{
				if (!IOSNativeSettings.Instance.DisablePluginLogs)
				{
					ISN_Logger.Log(string.Concat(typeof(T), " [Mog.Singleton] is already destroyed. Returning null. Please check HasInstance first before accessing instance in destructor."));
				}
				return (T)null;
			}
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
				if (_instance == null)
				{
					_instance = new GameObject().AddComponent<T>();
					_instance.gameObject.name = _instance.GetType().Name;
				}
			}
			return _instance;
		}
	}

	public static bool HasInstance => !IsDestroyed;

	public static bool IsDestroyed
	{
		get
		{
			if (_instance == null)
			{
				return true;
			}
			return false;
		}
	}

	protected void OnDestroy()
	{
		_instance = (T)null;
		applicationIsQuitting = true;
	}

	protected virtual void OnApplicationQuit()
	{
		_instance = (T)null;
		applicationIsQuitting = true;
	}
}
