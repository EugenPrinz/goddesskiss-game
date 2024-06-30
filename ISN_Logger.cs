using UnityEngine;

public class ISN_Logger : ISN_Singleton<ISN_Logger>
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Create()
	{
	}

	public static void Log(object message, LogType logType = LogType.Log)
	{
		ISN_Singleton<ISN_Logger>.Instance.Create();
		if (message != null && !IOSNativeSettings.Instance.DisablePluginLogs && Application.isEditor)
		{
			ISNEditorLog(logType, message);
		}
	}

	private static void ISNEditorLog(LogType logType, object message)
	{
		switch (logType)
		{
		case LogType.Error:
			break;
		case LogType.Exception:
			break;
		case LogType.Warning:
			break;
		case LogType.Assert:
		case LogType.Log:
			break;
		}
	}
}
