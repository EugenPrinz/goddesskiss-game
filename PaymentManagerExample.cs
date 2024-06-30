public class PaymentManagerExample
{
	public const string SMALL_PACK = "your.product.id1.here";

	public const string NC_PACK = "your.product.id2.here";

	private static bool IsInitialized;

	public static void init()
	{
		if (!IsInitialized)
		{
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("your.product.id1.here");
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.AddProductId("your.product.id2.here");
			IOSInAppPurchaseManager.OnVerificationComplete += HandleOnVerificationComplete;
			IOSInAppPurchaseManager.OnStoreKitInitComplete += OnStoreKitInitComplete;
			IOSInAppPurchaseManager.OnTransactionComplete += OnTransactionComplete;
			IOSInAppPurchaseManager.OnRestoreComplete += OnRestoreComplete;
			IsInitialized = true;
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.LoadStore();
		}
	}

	public static void buyItem(string productId)
	{
		ISN_Singleton<IOSInAppPurchaseManager>.Instance.BuyProduct(productId);
	}

	private static void UnlockProducts(string productIdentifier)
	{
		switch (productIdentifier)
		{
		case "your.product.id1.here":
			break;
		case "your.product.id2.here":
			break;
		}
	}

	private static void OnTransactionComplete(IOSStoreKitResult result)
	{
		ISN_Logger.Log("OnTransactionComplete: " + result.ProductIdentifier);
		ISN_Logger.Log("OnTransactionComplete: state: " + result.State);
		switch (result.State)
		{
		case InAppPurchaseState.Purchased:
		case InAppPurchaseState.Restored:
			UnlockProducts(result.ProductIdentifier);
			break;
		case InAppPurchaseState.Failed:
			ISN_Logger.Log("Transaction failed with error, code: " + result.Error.Code);
			ISN_Logger.Log("Transaction failed with error, description: " + result.Error.Description);
			break;
		}
		if (result.State == InAppPurchaseState.Failed)
		{
			IOSNativePopUpManager.showMessage("Transaction Failed", "Error code: " + result.Error.Code + "\nError description:" + result.Error.Description);
		}
		else
		{
			IOSNativePopUpManager.showMessage("Store Kit Response", "product " + result.ProductIdentifier + " state: " + result.State);
		}
	}

	private static void OnRestoreComplete(IOSStoreKitRestoreResult res)
	{
		if (res.IsSucceeded)
		{
			IOSNativePopUpManager.showMessage("Success", "Restore Compleated");
		}
		else
		{
			IOSNativePopUpManager.showMessage("Error: " + res.Error.Code, res.Error.Description);
		}
	}

	private static void HandleOnVerificationComplete(IOSStoreKitVerificationResponse response)
	{
		IOSNativePopUpManager.showMessage("Verification", "Transaction verification status: " + response.status);
		ISN_Logger.Log("ORIGINAL JSON: " + response.originalJSON);
	}

	private static void OnStoreKitInitComplete(ISN_Result result)
	{
		if (result.IsSucceeded)
		{
			int num = 0;
			foreach (IOSProductTemplate product in ISN_Singleton<IOSInAppPurchaseManager>.instance.Products)
			{
				if (product.IsAvaliable)
				{
					num++;
				}
			}
			IOSNativePopUpManager.showMessage("StoreKit Init Succeeded", "Available products count: " + num);
			ISN_Logger.Log("StoreKit Init Succeeded Available products count: " + num);
		}
		else
		{
			IOSNativePopUpManager.showMessage("StoreKit Init Failed", "Error code: " + result.Error.Code + "\nError description:" + result.Error.Description);
		}
	}
}
