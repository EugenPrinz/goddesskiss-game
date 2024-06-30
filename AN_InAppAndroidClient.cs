using System;
using UnityEngine;

public class AN_InAppAndroidClient : MonoBehaviour, AN_InAppClient
{
	private string _processedSKU;

	private AndroidInventory _inventory;

	private bool _IsConnectingToServiceInProcess;

	private bool _IsProductRetrievingInProcess;

	private bool _IsConnected;

	private bool _IsInventoryLoaded;

	public AndroidInventory Inventory => _inventory;

	public bool IsConnectingToServiceInProcess => _IsConnectingToServiceInProcess;

	public bool IsProductRetrievingInProcess => _IsProductRetrievingInProcess;

	public bool IsConnected => _IsConnected;

	public bool IsInventoryLoaded => _IsInventoryLoaded;

	public event Action<BillingResult> ActionProductPurchased = delegate
	{
	};

	public event Action<BillingResult> ActionProductConsumed = delegate
	{
	};

	public event Action<BillingResult> ActionBillingSetupFinished = delegate
	{
	};

	public event Action<BillingResult> ActionRetrieveProducsFinished = delegate
	{
	};

	private void Awake()
	{
		_inventory = new AndroidInventory();
	}

	public void AddProduct(string SKU)
	{
		GoogleProductTemplate googleProductTemplate = new GoogleProductTemplate();
		googleProductTemplate.SKU = SKU;
		GoogleProductTemplate template = googleProductTemplate;
		AddProduct(template);
	}

	public void AddProduct(GoogleProductTemplate template)
	{
		bool flag = false;
		int index = 0;
		foreach (GoogleProductTemplate product in _inventory.Products)
		{
			if (product.SKU.Equals(template.SKU))
			{
				flag = true;
				index = _inventory.Products.IndexOf(product);
				break;
			}
		}
		if (flag)
		{
			_inventory.Products[index] = template;
		}
		else
		{
			_inventory.Products.Add(template);
		}
	}

	public void RetrieveProducDetails()
	{
		_IsProductRetrievingInProcess = true;
		AN_BillingProxy.RetrieveProducDetails();
	}

	public void Purchase(string SKU)
	{
		Purchase(SKU, string.Empty);
	}

	public void Purchase(string SKU, string DeveloperPayload)
	{
		_processedSKU = SKU;
		AN_SoomlaGrow.PurchaseStarted(SKU);
		AN_BillingProxy.Purchase(SKU, DeveloperPayload);
	}

	public void Subscribe(string SKU)
	{
		Subscribe(SKU, string.Empty);
	}

	public void Subscribe(string SKU, string DeveloperPayload)
	{
		_processedSKU = SKU;
		AN_SoomlaGrow.PurchaseStarted(SKU);
		AN_BillingProxy.Subscribe(SKU, DeveloperPayload);
	}

	public void Consume(string SKU)
	{
		_processedSKU = SKU;
		AN_BillingProxy.Consume(SKU);
	}

	public void LoadStore()
	{
		Connect();
	}

	public void LoadStore(string base64EncodedPublicKey)
	{
		Connect(base64EncodedPublicKey);
	}

	public void Connect()
	{
		if (AndroidNativeSettings.Instance.IsBase64KeyWasReplaced)
		{
			Connect(AndroidNativeSettings.Instance.base64EncodedPublicKey);
			_IsConnectingToServiceInProcess = true;
		}
	}

	public void Connect(string base64EncodedPublicKey)
	{
		foreach (GoogleProductTemplate inAppProduct in AndroidNativeSettings.Instance.InAppProducts)
		{
			AddProduct(inAppProduct.SKU);
		}
		string text = string.Empty;
		int count = AndroidNativeSettings.Instance.InAppProducts.Count;
		for (int i = 0; i < count; i++)
		{
			if (i != 0)
			{
				text += ",";
			}
			text += AndroidNativeSettings.Instance.InAppProducts[i].SKU;
		}
		AN_BillingProxy.Connect(text, base64EncodedPublicKey);
	}

	public void OnPurchaseFinishedCallback(string data)
	{
		string[] array = data.Split("|"[0]);
		int num = Convert.ToInt32(array[0]);
		GooglePurchaseTemplate googlePurchaseTemplate = new GooglePurchaseTemplate();
		if (num == 0)
		{
			googlePurchaseTemplate.SKU = array[2];
			googlePurchaseTemplate.packageName = array[3];
			googlePurchaseTemplate.developerPayload = array[4];
			googlePurchaseTemplate.orderId = array[5];
			googlePurchaseTemplate.SetState(array[6]);
			googlePurchaseTemplate.token = array[7];
			googlePurchaseTemplate.signature = array[8];
			googlePurchaseTemplate.time = Convert.ToInt64(array[9]);
			googlePurchaseTemplate.originalJson = array[10];
			if (_inventory != null)
			{
				_inventory.addPurchase(googlePurchaseTemplate);
			}
		}
		else
		{
			googlePurchaseTemplate.SKU = _processedSKU;
		}
		switch (num)
		{
		case 0:
		{
			GoogleProductTemplate productDetails = Inventory.GetProductDetails(googlePurchaseTemplate.SKU);
			if (productDetails != null)
			{
				AN_SoomlaGrow.PurchaseFinished(productDetails.SKU, productDetails.PriceAmountMicros, productDetails.PriceCurrencyCode);
			}
			else
			{
				AN_SoomlaGrow.PurchaseFinished(googlePurchaseTemplate.SKU, 0L, "USD");
			}
			break;
		}
		case -1005:
			AN_SoomlaGrow.PurchaseCanceled(googlePurchaseTemplate.SKU);
			break;
		default:
			AN_SoomlaGrow.PurchaseError();
			break;
		}
		BillingResult obj = new BillingResult(num, array[1], googlePurchaseTemplate);
		this.ActionProductPurchased(obj);
	}

	public void OnConsumeFinishedCallBack(string data)
	{
		string[] array = data.Split("|"[0]);
		int num = Convert.ToInt32(array[0]);
		GooglePurchaseTemplate googlePurchaseTemplate = null;
		if (num == 0)
		{
			googlePurchaseTemplate = new GooglePurchaseTemplate();
			googlePurchaseTemplate.SKU = array[2];
			googlePurchaseTemplate.packageName = array[3];
			googlePurchaseTemplate.developerPayload = array[4];
			googlePurchaseTemplate.orderId = array[5];
			googlePurchaseTemplate.SetState(array[6]);
			googlePurchaseTemplate.token = array[7];
			googlePurchaseTemplate.signature = array[8];
			googlePurchaseTemplate.time = Convert.ToInt64(array[9]);
			googlePurchaseTemplate.originalJson = array[10];
			if (_inventory != null)
			{
				_inventory.removePurchase(googlePurchaseTemplate);
			}
		}
		BillingResult obj = new BillingResult(num, array[1], googlePurchaseTemplate);
		this.ActionProductConsumed(obj);
	}

	public void OnBillingSetupFinishedCallback(string data)
	{
		string[] array = data.Split("|"[0]);
		int code = Convert.ToInt32(array[0]);
		BillingResult billingResult = new BillingResult(code, array[1]);
		if (billingResult.isSuccess)
		{
			_IsConnected = true;
		}
		_IsConnectingToServiceInProcess = false;
		AN_SoomlaGrow.SetPurhsesSupportedState(billingResult.isSuccess);
		this.ActionBillingSetupFinished(billingResult);
	}

	public void OnQueryInventoryFinishedCallBack(string data)
	{
		string[] array = data.Split("|"[0]);
		int code = Convert.ToInt32(array[0]);
		BillingResult obj = new BillingResult(code, array[1]);
		_IsInventoryLoaded = true;
		_IsProductRetrievingInProcess = false;
		this.ActionRetrieveProducsFinished(obj);
	}

	public void OnPurchasesRecive(string data)
	{
		if (!data.Equals(string.Empty))
		{
			string[] array = data.Split("|"[0]);
			for (int i = 0; i < array.Length; i += 9)
			{
				GooglePurchaseTemplate googlePurchaseTemplate = new GooglePurchaseTemplate();
				googlePurchaseTemplate.SKU = array[i];
				googlePurchaseTemplate.packageName = array[i + 1];
				googlePurchaseTemplate.developerPayload = array[i + 2];
				googlePurchaseTemplate.orderId = array[i + 3];
				googlePurchaseTemplate.SetState(array[i + 4]);
				googlePurchaseTemplate.token = array[i + 5];
				googlePurchaseTemplate.signature = array[i + 6];
				googlePurchaseTemplate.time = Convert.ToInt64(array[i + 7]);
				googlePurchaseTemplate.originalJson = array[i + 8];
				_inventory.addPurchase(googlePurchaseTemplate);
			}
		}
	}

	public void OnProducttDetailsRecive(string data)
	{
		if (data.Equals(string.Empty))
		{
			return;
		}
		string[] array = data.Split("|"[0]);
		for (int i = 0; i < array.Length; i += 7)
		{
			GoogleProductTemplate googleProductTemplate = _inventory.GetProductDetails(array[i]);
			if (googleProductTemplate == null)
			{
				googleProductTemplate = new GoogleProductTemplate();
				googleProductTemplate.SKU = array[i];
				_inventory.Products.Add(googleProductTemplate);
			}
			googleProductTemplate.LocalizedPrice = array[i + 1];
			googleProductTemplate.Title = array[i + 2];
			googleProductTemplate.Description = array[i + 3];
			googleProductTemplate.PriceAmountMicros = Convert.ToInt64(array[i + 4]);
			googleProductTemplate.PriceCurrencyCode = array[i + 5];
			googleProductTemplate.OriginalJson = array[i + 6];
		}
	}
}
