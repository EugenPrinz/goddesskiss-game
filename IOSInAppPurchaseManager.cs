using System;
using System.Collections.Generic;
using UnityEngine;

public class IOSInAppPurchaseManager : ISN_Singleton<IOSInAppPurchaseManager>
{
	public const string APPLE_VERIFICATION_SERVER = "https://buy.itunes.apple.com/verifyReceipt";

	public const string SANDBOX_VERIFICATION_SERVER = "https://sandbox.itunes.apple.com/verifyReceipt";

	private bool _IsStoreLoaded;

	private bool _IsWaitingLoadResult;

	private static int _nextId;

	private Dictionary<int, IOSStoreProductView> _productsView = new Dictionary<int, IOSStoreProductView>();

	private static string lastPurchasedProduct;

	[Obsolete("products is deprecated, please use Products instead.")]
	public List<IOSProductTemplate> products => Products;

	public List<IOSProductTemplate> Products => IOSNativeSettings.Instance.InAppProducts;

	public bool IsStoreLoaded => _IsStoreLoaded;

	public bool IsInAppPurchasesEnabled => IOSNativeMarketBridge.ISN_InAppSettingState();

	public bool IsWaitingLoadResult => _IsWaitingLoadResult;

	private static int NextId
	{
		get
		{
			_nextId++;
			return _nextId;
		}
	}

	public static event Action OnRestoreStarted;

	public static event Action<string> OnTransactionStarted;

	public static event Action<IOSStoreKitResult> OnTransactionComplete;

	public static event Action<IOSStoreKitRestoreResult> OnRestoreComplete;

	public static event Action<ISN_Result> OnStoreKitInitComplete;

	public static event Action<bool> OnPurchasesStateSettingsLoaded;

	public static event Action<IOSStoreKitVerificationResponse> OnVerificationComplete;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	[Obsolete("loadStore is deprecated, please use LoadStore instead.")]
	public void loadStore()
	{
		LoadStore();
	}

	public void LoadStore()
	{
		if (_IsStoreLoaded)
		{
			Invoke("FireSuccessInitEvent", 1f);
		}
		else
		{
			if (_IsWaitingLoadResult)
			{
				return;
			}
			_IsWaitingLoadResult = true;
			string text = string.Empty;
			int count = Products.Count;
			for (int i = 0; i < count; i++)
			{
				if (i != 0)
				{
					text += ",";
				}
				text += Products[i].Id;
			}
			ISN_SoomlaGrow.Init();
			IOSNativeMarketBridge.loadStore(text);
		}
	}

	[Obsolete("buyProduct is deprecated, please use BuyProduct instead.")]
	public void buyProduct(string productId)
	{
		BuyProduct(productId);
	}

	public void BuyProduct(string productId)
	{
		IOSInAppPurchaseManager.OnTransactionStarted(productId);
		if (!_IsStoreLoaded)
		{
			if (!IOSNativeSettings.Instance.DisablePluginLogs)
			{
				ISN_Logger.Log("buyProduct shouldn't be called before StoreKit is initialized");
			}
			SendTransactionFailEvent(productId, "StoreKit not yet initialized", IOSTransactionErrorCode.SKErrorPaymentNotAllowed);
		}
		else
		{
			IOSNativeMarketBridge.buyProduct(productId);
		}
	}

	[Obsolete("addProductId is deprecated, please use AddProductId instead.")]
	public void addProductId(string productId)
	{
		AddProductId(productId);
	}

	public void AddProductId(string productId)
	{
		IOSProductTemplate iOSProductTemplate = new IOSProductTemplate();
		iOSProductTemplate.Id = productId;
		AddProductId(iOSProductTemplate);
	}

	public void AddProductId(IOSProductTemplate product)
	{
		bool flag = false;
		int index = 0;
		foreach (IOSProductTemplate product2 in Products)
		{
			if (product2.Id.Equals(product.Id))
			{
				flag = true;
				index = Products.IndexOf(product2);
				break;
			}
		}
		if (flag)
		{
			Products[index] = product;
		}
		else
		{
			Products.Add(product);
		}
	}

	public IOSProductTemplate GetProductById(string prodcutId)
	{
		foreach (IOSProductTemplate product in Products)
		{
			if (product.Id.Equals(prodcutId))
			{
				return product;
			}
		}
		IOSProductTemplate iOSProductTemplate = new IOSProductTemplate();
		iOSProductTemplate.Id = prodcutId;
		Products.Add(iOSProductTemplate);
		return iOSProductTemplate;
	}

	[Obsolete("restorePurchases is deprecated, please use RestorePurchases instead.")]
	public void restorePurchases()
	{
		RestorePurchases();
	}

	public void RestorePurchases()
	{
		if (!_IsStoreLoaded)
		{
			ISN_Error e = new ISN_Error(7, "Store Kit Initilizations required");
			IOSStoreKitRestoreResult obj = new IOSStoreKitRestoreResult(e);
			IOSInAppPurchaseManager.OnRestoreComplete(obj);
		}
		else
		{
			IOSInAppPurchaseManager.OnRestoreStarted();
			IOSNativeMarketBridge.restorePurchases();
		}
	}

	[Obsolete("verifyLastPurchase is deprecated, please use VerifyLastPurchase instead.")]
	public void verifyLastPurchase(string url)
	{
		VerifyLastPurchase(url);
	}

	public void VerifyLastPurchase(string url)
	{
		IOSNativeMarketBridge.verifyLastPurchase(url);
	}

	public void RegisterProductView(IOSStoreProductView view)
	{
		view.SetId(NextId);
		_productsView.Add(view.id, view);
	}

	private void OnStoreKitInitFailed(string data)
	{
		ISN_Error iSN_Error = new ISN_Error(data);
		_IsStoreLoaded = false;
		_IsWaitingLoadResult = false;
		ISN_Result iSN_Result = new ISN_Result(IsResultSucceeded: false);
		iSN_Result.SetError(iSN_Error);
		IOSInAppPurchaseManager.OnStoreKitInitComplete(iSN_Result);
		if (!IOSNativeSettings.Instance.DisablePluginLogs)
		{
			ISN_Logger.Log("STORE_KIT_INIT_FAILED Error: " + iSN_Error.Description);
		}
	}

	private void onStoreDataReceived(string data)
	{
		if (data.Equals(string.Empty))
		{
			ISN_Logger.Log("InAppPurchaseManager, no products avaiable");
			ISN_Result obj = new ISN_Result(IsResultSucceeded: true);
			IOSInAppPurchaseManager.OnStoreKitInitComplete(obj);
			return;
		}
		string[] array = data.Split('|');
		for (int i = 0; i < array.Length; i += 7)
		{
			string prodcutId = array[i];
			IOSProductTemplate productById = GetProductById(prodcutId);
			productById.DisplayName = array[i + 1];
			productById.Description = array[i + 2];
			productById.LocalizedPrice = array[i + 3];
			productById.Price = Convert.ToSingle(array[i + 4]);
			productById.CurrencyCode = array[i + 5];
			productById.CurrencySymbol = array[i + 6];
			productById.IsAvaliable = true;
		}
		ISN_Logger.Log("InAppPurchaseManager, total products in settings: " + Products.Count);
		int num = 0;
		foreach (IOSProductTemplate product in Products)
		{
			if (product.IsAvaliable)
			{
				num++;
			}
		}
		ISN_Logger.Log("InAppPurchaseManager, total avaliable products" + num);
		FireSuccessInitEvent();
	}

	private void onProductBought(string array)
	{
		string[] array2 = array.Split("|"[0]);
		bool isRestored = false;
		if (array2[1].Equals("0"))
		{
			isRestored = true;
		}
		string productIdentifier = array2[0];
		FireProductBoughtEvent(productIdentifier, array2[2], array2[3], array2[4], isRestored);
	}

	private void onProductStateDeferred(string productIdentifier)
	{
		IOSStoreKitResult obj = new IOSStoreKitResult(productIdentifier, InAppPurchaseState.Deferred, string.Empty, string.Empty, string.Empty);
		IOSInAppPurchaseManager.OnTransactionComplete(obj);
	}

	private void onTransactionFailed(string array)
	{
		string[] array2 = array.Split("|"[0]);
		string productIdentifier = array2[0];
		int num = Convert.ToInt32(array2[2]);
		IOSTransactionErrorCode errorCode = (IOSTransactionErrorCode)num;
		SendTransactionFailEvent(productIdentifier, array2[1], errorCode);
	}

	private void onVerificationResult(string array)
	{
		string[] array2 = array.Split("|"[0]);
		IOSStoreKitVerificationResponse iOSStoreKitVerificationResponse = new IOSStoreKitVerificationResponse();
		iOSStoreKitVerificationResponse.status = Convert.ToInt32(array2[0]);
		iOSStoreKitVerificationResponse.originalJSON = array2[1];
		iOSStoreKitVerificationResponse.receipt = array2[2];
		iOSStoreKitVerificationResponse.productIdentifier = lastPurchasedProduct;
		IOSInAppPurchaseManager.OnVerificationComplete(iOSStoreKitVerificationResponse);
	}

	public void onRestoreTransactionFailed(string array)
	{
		ISN_Error e = new ISN_Error(array);
		IOSStoreKitRestoreResult obj = new IOSStoreKitRestoreResult(e);
		IOSInAppPurchaseManager.OnRestoreComplete(obj);
	}

	public void onRestoreTransactionComplete(string array)
	{
		FireRestoreCompleteEvent();
	}

	private void OnProductViewLoaded(string viewId)
	{
		int key = Convert.ToInt32(viewId);
		if (_productsView.ContainsKey(key))
		{
			_productsView[key].OnContentLoaded();
		}
	}

	private void OnProductViewLoadedFailed(string viewId)
	{
		int key = Convert.ToInt32(viewId);
		if (_productsView.ContainsKey(key))
		{
			_productsView[key].OnContentLoadFailed();
		}
	}

	private void OnProductViewDismissed(string viewId)
	{
		int key = Convert.ToInt32(viewId);
		if (_productsView.ContainsKey(key))
		{
			_productsView[key].OnProductViewDismissed();
		}
	}

	private void FireSuccessInitEvent()
	{
		_IsStoreLoaded = true;
		_IsWaitingLoadResult = false;
		ISN_Result obj = new ISN_Result(IsResultSucceeded: true);
		IOSInAppPurchaseManager.OnStoreKitInitComplete(obj);
	}

	private void FireRestoreCompleteEvent()
	{
		IOSStoreKitRestoreResult obj = new IOSStoreKitRestoreResult(IsResultSucceeded: true);
		IOSInAppPurchaseManager.OnRestoreComplete(obj);
	}

	private void FireProductBoughtEvent(string productIdentifier, string applicationUsername, string receipt, string transactionIdentifier, bool IsRestored)
	{
		InAppPurchaseState state = (IsRestored ? InAppPurchaseState.Restored : InAppPurchaseState.Purchased);
		IOSStoreKitResult iOSStoreKitResult = new IOSStoreKitResult(productIdentifier, state, applicationUsername, receipt, transactionIdentifier);
		lastPurchasedProduct = iOSStoreKitResult.ProductIdentifier;
		IOSInAppPurchaseManager.OnTransactionComplete(iOSStoreKitResult);
	}

	private void SendTransactionFailEvent(string productIdentifier, string errorDescribtion, IOSTransactionErrorCode errorCode)
	{
		IOSStoreKitResult obj = new IOSStoreKitResult(productIdentifier, new ISN_Error((int)errorCode, errorDescribtion));
		IOSInAppPurchaseManager.OnTransactionComplete(obj);
	}

	private void EditorFakeInitEvent()
	{
		FireSuccessInitEvent();
	}

	static IOSInAppPurchaseManager()
	{
		IOSInAppPurchaseManager.OnRestoreStarted = delegate
		{
		};
		IOSInAppPurchaseManager.OnTransactionStarted = delegate
		{
		};
		IOSInAppPurchaseManager.OnTransactionComplete = delegate
		{
		};
		IOSInAppPurchaseManager.OnRestoreComplete = delegate
		{
		};
		IOSInAppPurchaseManager.OnStoreKitInitComplete = delegate
		{
		};
		IOSInAppPurchaseManager.OnPurchasesStateSettingsLoaded = delegate
		{
		};
		IOSInAppPurchaseManager.OnVerificationComplete = delegate
		{
		};
		_nextId = 1;
	}
}
