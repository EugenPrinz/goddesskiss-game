using UnityEngine;

public class iAdUseExample : MonoBehaviour
{
	private GUIStyle style;

	private GUIStyle style2;

	private iAdBanner banner1;

	private iAdBanner banner2;

	public GameObject Quad;

	private void Start()
	{
		Quad.SetActive(value: false);
		iAdBannerController.InterstitialAdDidLoadAction += HandleInterstitialAdDidLoadAction;
		iAdBannerController.InterstitialAdDidFinishAction += HandleInterstitialAdDidFinishAction;
		InitStyles();
	}

	private void InitStyles()
	{
		style = new GUIStyle();
		style.normal.textColor = Color.white;
		style.fontSize = 16;
		style.fontStyle = FontStyle.BoldAndItalic;
		style.alignment = TextAnchor.UpperLeft;
		style.wordWrap = true;
		style2 = new GUIStyle();
		style2.normal.textColor = Color.white;
		style2.fontSize = 12;
		style2.fontStyle = FontStyle.Italic;
		style2.alignment = TextAnchor.UpperLeft;
		style2.wordWrap = true;
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		ISN_Logger.Log(pauseStatus);
	}

	private void OnGUI()
	{
		float num = 20f;
		float num2 = 10f;
		GUI.Label(new Rect(num2, num, Screen.width, 40f), "Interstitial Example", style);
		num += 40f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Start Interstitial Ad"))
		{
			ISN_Singleton<iAdBannerController>.instance.StartInterstitialAd();
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Load Interstitial Ad"))
		{
			ISN_Singleton<iAdBannerController>.instance.LoadInterstitialAd();
		}
		num2 += 170f;
		GUI.enabled = ISN_Singleton<iAdBannerController>.Instance.IsInterstisialsAdReady;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Show Interstitial Ad"))
		{
			ISN_Singleton<iAdBannerController>.instance.ShowInterstitialAd();
		}
		GUI.enabled = true;
		num += 80f;
		num2 = 10f;
		GUI.Label(new Rect(num2, num, Screen.width, 40f), "Banners Example", style);
		GUI.enabled = false;
		if (banner1 == null)
		{
			GUI.enabled = true;
		}
		num += 40f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Custom Pos"))
		{
			banner1 = ISN_Singleton<iAdBannerController>.instance.CreateAdBanner(300, 100);
		}
		num += 80f;
		num2 = 10f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Top Left"))
		{
			banner1 = ISN_Singleton<iAdBannerController>.instance.CreateAdBanner(TextAnchor.UpperLeft);
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Top Center"))
		{
			banner1 = ISN_Singleton<iAdBannerController>.instance.CreateAdBanner(TextAnchor.UpperCenter);
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Top Right"))
		{
			banner1 = ISN_Singleton<iAdBannerController>.instance.CreateAdBanner(TextAnchor.UpperRight);
		}
		num += 80f;
		num2 = 10f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Bottom Left"))
		{
			banner1 = ISN_Singleton<iAdBannerController>.instance.CreateAdBanner(TextAnchor.LowerLeft);
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Bottom Center"))
		{
			banner1 = ISN_Singleton<iAdBannerController>.instance.CreateAdBanner(TextAnchor.LowerCenter);
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Bottom Right"))
		{
			banner1 = ISN_Singleton<iAdBannerController>.instance.CreateAdBanner(TextAnchor.LowerRight);
		}
		num += 80f;
		num2 = 10f;
		GUI.enabled = false;
		if (banner1 != null && banner1.IsLoaded && banner1.IsOnScreen)
		{
			GUI.enabled = true;
		}
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Hide"))
		{
			banner1.Hide();
		}
		GUI.enabled = false;
		if (banner1 != null && banner1.IsLoaded && !banner1.IsOnScreen)
		{
			GUI.enabled = true;
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Show"))
		{
			banner1.Show();
		}
		GUI.enabled = false;
		if (banner1 != null)
		{
			GUI.enabled = true;
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Destroy"))
		{
			ISN_Logger.Log("Destroy banner with ID: " + banner1.id);
			ISN_Singleton<iAdBannerController>.instance.DestroyBanner(banner1.id);
			banner1 = null;
		}
		GUI.enabled = true;
	}

	private void HandleInterstitialAdDidFinishAction()
	{
		ISN_Logger.Log("OnInterstitialFinish action fired");
		IOSMessage.Create("Ad Event", "Ad Did Finish");
	}

	private void HandleInterstitialAdDidLoadAction()
	{
		ISN_Logger.Log("HandleInterstitialAdDidLoadAction event fired");
	}
}
