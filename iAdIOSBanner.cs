using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class iAdIOSBanner : MonoBehaviour
{
	public TextAnchor anchor = TextAnchor.LowerCenter;

	private static Dictionary<string, iAdBanner> _registeredBanners;

	public static Dictionary<string, iAdBanner> registeredBanners
	{
		get
		{
			if (_registeredBanners == null)
			{
				_registeredBanners = new Dictionary<string, iAdBanner>();
			}
			return _registeredBanners;
		}
	}

	public string sceneBannerId => SceneManager.GetActiveScene().name + "_" + base.gameObject.name;

	private void Start()
	{
		ShowBanner();
	}

	private void OnDestroy()
	{
		HideBanner();
	}

	public void ShowBanner()
	{
		iAdBanner iAdBanner2;
		if (registeredBanners.ContainsKey(sceneBannerId))
		{
			iAdBanner2 = registeredBanners[sceneBannerId];
		}
		else
		{
			iAdBanner2 = ISN_Singleton<iAdBannerController>.instance.CreateAdBanner(anchor);
			registeredBanners.Add(sceneBannerId, iAdBanner2);
		}
		if (iAdBanner2.IsLoaded && !iAdBanner2.IsOnScreen)
		{
			iAdBanner2.Show();
		}
	}

	public void HideBanner()
	{
		if (!registeredBanners.ContainsKey(sceneBannerId))
		{
			return;
		}
		iAdBanner iAdBanner2 = registeredBanners[sceneBannerId];
		if (iAdBanner2.IsLoaded)
		{
			if (iAdBanner2.IsOnScreen)
			{
				iAdBanner2.Hide();
			}
		}
		else
		{
			iAdBanner2.ShowOnLoad = false;
		}
	}
}
