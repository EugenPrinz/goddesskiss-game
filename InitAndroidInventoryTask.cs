using System;
using UnityEngine;

public class InitAndroidInventoryTask : MonoBehaviour
{
	public event Action ActionComplete = delegate
	{
	};

	public event Action ActionFailed = delegate
	{
	};

	public static InitAndroidInventoryTask Create()
	{
		return new GameObject("InitAndroidInventoryTask").AddComponent<InitAndroidInventoryTask>();
	}

	public void Run()
	{
		if (AndroidInAppPurchaseManager.Client.IsConnected)
		{
			OnBillingConnected(null);
			return;
		}
		AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnBillingConnected;
		if (!AndroidInAppPurchaseManager.Client.IsConnectingToServiceInProcess)
		{
			AndroidInAppPurchaseManager.Client.Connect();
		}
	}

	private void OnBillingConnected(BillingResult result)
	{
		if (result == null)
		{
			OnBillingConnectFinished();
			return;
		}
		AndroidInAppPurchaseManager.ActionBillingSetupFinished -= OnBillingConnected;
		if (result.isSuccess)
		{
			OnBillingConnectFinished();
		}
		else
		{
			this.ActionFailed();
		}
	}

	private void OnBillingConnectFinished()
	{
		if (AndroidInAppPurchaseManager.Client.IsInventoryLoaded)
		{
			this.ActionComplete();
			return;
		}
		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinised;
		if (!AndroidInAppPurchaseManager.Client.IsProductRetrievingInProcess)
		{
			AndroidInAppPurchaseManager.Client.RetrieveProducDetails();
		}
	}

	private void OnRetrieveProductsFinised(BillingResult result)
	{
		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinised;
		if (result.isSuccess)
		{
			this.ActionComplete();
		}
		else
		{
			this.ActionFailed();
		}
	}
}
