using UnityEngine;

public class MarketExample : BaseIOSFeaturePreview
{
	private void Awake()
	{
	}

	private void OnGUI()
	{
		UpdateToStartPos();
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40f), "In-App Purchases", style);
		StartY += YLableStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Init"))
		{
			PaymentManagerExample.init();
		}
		if (ISN_Singleton<IOSInAppPurchaseManager>.Instance.IsStoreLoaded)
		{
			GUI.enabled = true;
		}
		else
		{
			GUI.enabled = false;
		}
		StartX = XStartPos;
		StartY += YButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Perform Buy #1"))
		{
			PaymentManagerExample.buyItem("your.product.id1.here");
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Perform Buy #2"))
		{
			PaymentManagerExample.buyItem("your.product.id2.here");
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Restore Purchases"))
		{
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.RestorePurchases();
		}
		StartX = XStartPos;
		StartY += YButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Verify Last Purchases"))
		{
			ISN_Singleton<IOSInAppPurchaseManager>.Instance.VerifyLastPurchase("https://sandbox.itunes.apple.com/verifyReceipt");
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Load Product View"))
		{
			IOSStoreProductView iOSStoreProductView = new IOSStoreProductView("333700869");
			iOSStoreProductView.Dismissed += StoreProductViewDisnissed;
			iOSStoreProductView.Load();
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Is Payments Enabled On device"))
		{
			IOSNativePopUpManager.showMessage("Payments Settings State", "Is Payments Enabled: " + ISN_Singleton<IOSInAppPurchaseManager>.Instance.IsInAppPurchasesEnabled);
		}
		StartX = XStartPos;
		StartY += YButtonStep;
		StartY += YLableStep;
		GUI.enabled = true;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40f), "Local Receipt Validation", style);
		StartY += YLableStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth + 10, buttonHeight), "Load Receipt"))
		{
			ISN_Security.OnReceiptLoaded += OnReceiptLoaded;
			ISN_Singleton<ISN_Security>.Instance.RetrieveLocalReceipt();
		}
	}

	private void StoreProductViewDisnissed()
	{
		ISN_Logger.Log("Store Product View was dismissed");
	}

	private void OnReceiptLoaded(ISN_LocalReceiptResult result)
	{
		ISN_Logger.Log("OnReceiptLoaded");
		ISN_Security.OnReceiptLoaded -= OnReceiptLoaded;
		if (result.Receipt != null)
		{
			IOSNativePopUpManager.showMessage("Success", "Receipt loaded, byte array length: " + result.Receipt.Length);
			return;
		}
		IOSDialog iOSDialog = IOSDialog.Create("Failed", "No Receipt found on the device. Would you like to refresh local Receipt?");
		iOSDialog.OnComplete += OnComplete;
	}

	private void OnComplete(IOSDialogResult res)
	{
		if (res == IOSDialogResult.YES)
		{
			ISN_Security.OnReceiptRefreshComplete += OnReceiptRefreshComplete;
			ISN_Singleton<ISN_Security>.Instance.StartReceiptRefreshRequest();
		}
	}

	private void OnReceiptRefreshComplete(ISN_Result res)
	{
		if (res.IsSucceeded)
		{
			IOSDialog iOSDialog = IOSDialog.Create("Success", "Receipt Refreshed, would you like to check it again?");
			iOSDialog.OnComplete += Dialog_RetrieveLocalReceipt;
		}
		else
		{
			IOSNativePopUpManager.showMessage("Fail", "Receipt Refresh Failed");
		}
	}

	private void Dialog_RetrieveLocalReceipt(IOSDialogResult res)
	{
		if (res == IOSDialogResult.YES)
		{
			ISN_Security.OnReceiptLoaded += OnReceiptLoaded;
			ISN_Singleton<ISN_Security>.Instance.RetrieveLocalReceipt();
		}
	}
}
