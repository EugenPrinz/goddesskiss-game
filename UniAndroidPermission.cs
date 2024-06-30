using System;
using UnityEngine;

public class UniAndroidPermission : MonoBehaviour
{
	private static Action permitCallBack;

	private static Action notPermitCallBack;

	private const string PackageClassName = "net.sanukin.PermissionManager";

	private AndroidJavaClass permissionManager;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public static bool IsPermitted(AndroidPermission permission)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("net.sanukin.PermissionManager");
		return androidJavaClass.CallStatic<bool>("hasPermission", new object[1] { GetPermittionStr(permission) });
	}

	public static void RequestPremission(AndroidPermission permission, Action onPermit = null, Action notPermit = null)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("net.sanukin.PermissionManager");
		androidJavaClass.CallStatic("requestPermission", GetPermittionStr(permission));
		permitCallBack = onPermit;
		notPermitCallBack = notPermit;
	}

	private static string GetPermittionStr(AndroidPermission permittion)
	{
		return "android.permission." + permittion;
	}

	private void OnPermit()
	{
		if (permitCallBack != null)
		{
			permitCallBack();
		}
		ResetCallBacks();
	}

	private void NotPermit()
	{
		if (notPermitCallBack != null)
		{
			notPermitCallBack();
		}
		ResetCallBacks();
	}

	private void ResetCallBacks()
	{
		notPermitCallBack = null;
		permitCallBack = null;
	}
}
