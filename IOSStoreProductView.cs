using System;
using System.Collections.Generic;

public class IOSStoreProductView
{
	private List<string> _ids = new List<string>();

	private int _id;

	public int id => _id;

	public event Action Loaded = delegate
	{
	};

	public event Action LoadFailed = delegate
	{
	};

	public event Action Appeared = delegate
	{
	};

	public event Action Dismissed = delegate
	{
	};

	public IOSStoreProductView()
	{
		foreach (string item in IOSNativeSettings.Instance.DefaultStoreProductsView)
		{
			addProductId(item);
		}
		ISN_Singleton<IOSInAppPurchaseManager>.Instance.RegisterProductView(this);
	}

	public IOSStoreProductView(params string[] ids)
	{
		foreach (string productId in ids)
		{
			addProductId(productId);
		}
		ISN_Singleton<IOSInAppPurchaseManager>.Instance.RegisterProductView(this);
	}

	public void addProductId(string productId)
	{
		if (!_ids.Contains(productId))
		{
			_ids.Add(productId);
		}
	}

	public void Load()
	{
	}

	public void Show()
	{
	}

	public void OnProductViewAppeard()
	{
		this.Appeared();
	}

	public void OnProductViewDismissed()
	{
		this.Dismissed();
	}

	public void OnContentLoaded()
	{
		Show();
		this.Loaded();
	}

	public void OnContentLoadFailed()
	{
		this.LoadFailed();
	}

	public void SetId(int viewId)
	{
		_id = viewId;
	}
}
