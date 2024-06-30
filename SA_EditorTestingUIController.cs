using System;
using UnityEngine;
using UnityEngine.UI;

public class SA_EditorTestingUIController : MonoBehaviour
{
	public GameObject VideoPanel;

	public GameObject InterstitialPanel;

	public Image[] AppIcons;

	public Text[] AppNames;

	public event Action<bool> OnCloseVideo = delegate
	{
	};

	public event Action OnVideoLeftApplication = delegate
	{
	};

	public event Action<bool> OnCloseInterstitial = delegate
	{
	};

	public event Action OnInterstitialLeftApplication = delegate
	{
	};

	private void Start()
	{
	}

	public void InterstitialClick()
	{
		this.OnInterstitialLeftApplication();
	}

	public void VideoClick()
	{
		this.OnVideoLeftApplication();
	}

	public void ShowInterstitialAd()
	{
		base.gameObject.SetActive(value: true);
		InterstitialPanel.SetActive(value: true);
	}

	public void ShowVideoAd()
	{
		base.gameObject.SetActive(value: true);
		VideoPanel.SetActive(value: true);
	}

	public void CloseInterstitial()
	{
		base.gameObject.SetActive(value: false);
		InterstitialPanel.SetActive(value: false);
		this.OnCloseInterstitial(obj: true);
	}

	public void CloseVideo()
	{
		base.gameObject.SetActive(value: false);
		VideoPanel.SetActive(value: false);
		this.OnCloseVideo(obj: true);
	}
}
