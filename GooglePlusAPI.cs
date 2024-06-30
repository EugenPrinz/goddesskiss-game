using System;
using UnityEngine;

public class GooglePlusAPI : SA_Singleton<GooglePlusAPI>
{
	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	[Obsolete("clearDefaultAccount is deprecated, please use ClearDefaultAccount instead.")]
	public void clearDefaultAccount()
	{
		ClearDefaultAccount();
	}

	public void ClearDefaultAccount()
	{
		if (GooglePlayConnection.CheckState())
		{
			AN_GMSGeneralProxy.clearDefaultAccount();
			SA_Singleton<GooglePlayConnection>.instance.Disconnect();
		}
	}
}
