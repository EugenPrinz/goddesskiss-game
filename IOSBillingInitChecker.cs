public class IOSBillingInitChecker
{
	public delegate void BillingInitListener();

	private BillingInitListener _listener;

	public IOSBillingInitChecker(BillingInitListener listener)
	{
		_listener = listener;
		if (ISN_Singleton<IOSInAppPurchaseManager>.Instance.IsStoreLoaded)
		{
			_listener();
			return;
		}
		IOSInAppPurchaseManager.OnStoreKitInitComplete += HandleOnStoreKitInitComplete;
		if (!ISN_Singleton<IOSInAppPurchaseManager>.Instance.IsWaitingLoadResult)
		{
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.LoadStore();
		}
	}

	private void HandleOnStoreKitInitComplete(ISN_Result obj)
	{
		IOSInAppPurchaseManager.OnStoreKitInitComplete -= HandleOnStoreKitInitComplete;
		_listener();
	}
}
