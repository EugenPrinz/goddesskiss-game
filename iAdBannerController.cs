using System;
using System.Collections.Generic;
using UnityEngine;

public class iAdBannerController : ISN_Singleton<iAdBannerController>
{
	private static int _nextId = 0;

	private static iAdBannerController _instance;

	private Dictionary<int, iAdBanner> _banners;

	private bool _IsInterstisialsAdReady;

	private bool _ShowOnLoad;

	public static int nextId
	{
		get
		{
			_nextId++;
			return _nextId;
		}
	}

	public bool IsInterstisialsAdReady => _IsInterstisialsAdReady;

	public bool IsEditorTestingEnabled => SA_EditorTesting.IsInsideEditor && IOSNativeSettings.Instance.IsEditorTestingEnabled;

	public List<iAdBanner> banners
	{
		get
		{
			List<iAdBanner> list = new List<iAdBanner>();
			if (_banners == null)
			{
				return list;
			}
			foreach (KeyValuePair<int, iAdBanner> banner in _banners)
			{
				list.Add(banner.Value);
			}
			return list;
		}
	}

	public static event Action InterstitialDidFailWithErrorAction;

	public static event Action InterstitialAdWillLoadAction;

	public static event Action InterstitialAdDidLoadAction;

	public static event Action InterstitialAdDidFinishAction;

	private void Awake()
	{
		_banners = new Dictionary<int, iAdBanner>();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (IsEditorTestingEnabled)
		{
			SA_Singleton<SA_EditorAd>.Instance.SetFillRate(IOSNativeSettings.Instance.EditorFillRate);
		}
	}

	public iAdBanner CreateAdBanner(TextAnchor anchor)
	{
		iAdBanner iAdBanner2 = new iAdBanner(anchor, nextId);
		_banners.Add(iAdBanner2.id, iAdBanner2);
		return iAdBanner2;
	}

	public iAdBanner CreateAdBanner(int x, int y)
	{
		iAdBanner iAdBanner2 = new iAdBanner(x, y, nextId);
		_banners.Add(iAdBanner2.id, iAdBanner2);
		return iAdBanner2;
	}

	public void DestroyBanner(int id)
	{
		if (_banners != null && _banners.ContainsKey(id))
		{
			_banners.Remove(id);
		}
	}

	public void StartInterstitialAd()
	{
		if (IsEditorTestingEnabled)
		{
			_ShowOnLoad = true;
			iAdBannerController.InterstitialAdWillLoadAction();
			SA_EditorAd.OnInterstitialLoadComplete += HandleOnInterstitialLoadComplete_Editor;
			SA_Singleton<SA_EditorAd>.Instance.LoadInterstitial();
		}
	}

	public void LoadInterstitialAd()
	{
		if (IsEditorTestingEnabled)
		{
			iAdBannerController.InterstitialAdWillLoadAction();
			SA_EditorAd.OnInterstitialLoadComplete += HandleOnInterstitialLoadComplete_Editor;
			SA_Singleton<SA_EditorAd>.Instance.LoadInterstitial();
		}
	}

	private void HandleOnInterstitialLoadComplete_Editor(bool success)
	{
		SA_EditorAd.OnInterstitialLoadComplete -= HandleOnInterstitialLoadComplete_Editor;
		if (success)
		{
			_IsInterstisialsAdReady = true;
			iAdBannerController.InterstitialAdDidLoadAction();
			if (_ShowOnLoad)
			{
				_ShowOnLoad = false;
				ShowInterstitialAd();
			}
		}
		else
		{
			iAdBannerController.InterstitialDidFailWithErrorAction();
		}
	}

	public void ShowInterstitialAd()
	{
		if (IsEditorTestingEnabled)
		{
			_IsInterstisialsAdReady = false;
			SA_EditorAd.OnInterstitialFinished += HandleOnInterstitialFinished_Editor;
			SA_Singleton<SA_EditorAd>.Instance.ShowInterstitial();
		}
		else if (_IsInterstisialsAdReady)
		{
			Invoke("interstitialAdActionDidFinish", 1f);
			_IsInterstisialsAdReady = false;
		}
	}

	private void HandleOnInterstitialFinished_Editor(bool isRewarded)
	{
		SA_EditorAd.OnInterstitialFinished -= HandleOnInterstitialFinished_Editor;
		iAdBannerController.InterstitialAdDidFinishAction();
	}

	public iAdBanner GetBanner(int id)
	{
		if (_banners.ContainsKey(id))
		{
			return _banners[id];
		}
		if (!IOSNativeSettings.Instance.DisablePluginLogs)
		{
			ISN_Logger.Log("Banner id: " + id + " not found", LogType.Warning);
		}
		return null;
	}

	private void didFailToReceiveAdWithError(string bannerID)
	{
		int id = Convert.ToInt32(bannerID);
		GetBanner(id)?.didFailToReceiveAdWithError();
	}

	private void bannerViewDidLoadAd(string bannerID)
	{
		int id = Convert.ToInt32(bannerID);
		GetBanner(id)?.bannerViewDidLoadAd();
	}

	private void bannerViewWillLoadAd(string bannerID)
	{
		int id = Convert.ToInt32(bannerID);
		GetBanner(id)?.bannerViewWillLoadAd();
	}

	private void bannerViewActionDidFinish(string bannerID)
	{
		int id = Convert.ToInt32(bannerID);
		GetBanner(id)?.bannerViewActionDidFinish();
	}

	private void bannerViewActionShouldBegin(string bannerID)
	{
		int id = Convert.ToInt32(bannerID);
		GetBanner(id)?.bannerViewActionShouldBegin();
	}

	private void interstitialdidFailWithError(string data)
	{
		iAdBannerController.InterstitialDidFailWithErrorAction();
		_IsInterstisialsAdReady = false;
	}

	private void interstitialAdWillLoad(string data)
	{
		iAdBannerController.InterstitialAdWillLoadAction();
		_IsInterstisialsAdReady = false;
	}

	private void interstitialAdDidLoad(string data)
	{
		iAdBannerController.InterstitialAdDidLoadAction();
		_IsInterstisialsAdReady = true;
	}

	private void interstitialAdActionDidFinish()
	{
		iAdBannerController.InterstitialAdDidFinishAction();
	}

	static iAdBannerController()
	{
		iAdBannerController.InterstitialDidFailWithErrorAction = delegate
		{
		};
		iAdBannerController.InterstitialAdWillLoadAction = delegate
		{
		};
		iAdBannerController.InterstitialAdDidLoadAction = delegate
		{
		};
		iAdBannerController.InterstitialAdDidFinishAction = delegate
		{
		};
	}
}
