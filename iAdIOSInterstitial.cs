using UnityEngine;
using UnityEngine.SceneManagement;

public class iAdIOSInterstitial : MonoBehaviour
{
	public string sceneBannerId => SceneManager.GetActiveScene().name + "_" + base.gameObject.name;

	private void Start()
	{
		ShowBanner();
	}

	public void ShowBanner()
	{
		ISN_Singleton<iAdBannerController>.instance.StartInterstitialAd();
	}
}
