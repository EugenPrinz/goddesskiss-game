using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class DownloadTexture : MonoBehaviour
{
	public string url = "http://www.yourwebsite.com/logo.png";

	public bool pixelPerfect = true;

	private Texture2D mTex;

	public bool isTextureDownLoadDone { get; private set; }

	public void SetUrl(string _url)
	{
		url = $"{PatchManager.Instance.GetPatchUrl}{_url}";
		StartCoroutine(OpenEventPage());
	}

	public void MainBannerSetUrl(string _url)
	{
		url = _url;
		StartCoroutine(MainBannerOpenEventPage());
	}

	private IEnumerator MainBannerOpenEventPage()
	{
		WWW www = new WWW(url);
		yield return www;
		mTex = www.texture;
		if (mTex != null)
		{
			UITexture component = GetComponent<UITexture>();
			component.mainTexture = mTex;
			if (pixelPerfect)
			{
				component.MakePixelPerfect();
			}
			component.SetDimensions(520, 87);
		}
		www.Dispose();
	}

	private IEnumerator OpenEventPage()
	{
		WWW www = new WWW(url);
		yield return www;
		mTex = www.texture;
		if (mTex != null)
		{
			UITexture component = GetComponent<UITexture>();
			component.mainTexture = mTex;
			if (pixelPerfect)
			{
				component.MakePixelPerfect();
			}
		}
		www.Dispose();
	}

	public void MainBannerSetUrl(int urlIdx)
	{
		if (UIManager.instance.world.mainCommand.banner.bannerInfoList != null)
		{
			BannerInfo bannerInfo = UIManager.instance.world.mainCommand.banner.bannerInfoList[urlIdx];
			if (bannerInfo != null)
			{
				url = bannerInfo.url;
				StartCoroutine(MainBannerOpenEventPage(urlIdx));
			}
		}
	}

	private IEnumerator MainBannerOpenEventPage(int urlIdx)
	{
		isTextureDownLoadDone = false;
		WWW www = new WWW(url);
		yield return www;
		ReleaseTexture();
		mTex = www.texture;
		if (mTex != null)
		{
			Texture2D mainTexture = mTex;
			UIMainbanner banner = UIManager.instance.world.mainCommand.banner;
			if (banner.bannerInfoList != null)
			{
				BannerInfo bannerInfo = banner.bannerInfoList[urlIdx];
				if (bannerInfo != null)
				{
					bannerInfo.texture = mTex;
					RemoteObjectManager.instance.localUser.OriginBannerList.Add(bannerInfo);
					mTex = null;
				}
			}
			UITexture component = GetComponent<UITexture>();
			component.mainTexture = mainTexture;
			if (pixelPerfect)
			{
				component.MakePixelPerfect();
			}
			component.SetDimensions(529, 106);
			isTextureDownLoadDone = true;
		}
		www.Dispose();
	}

	private void ReleaseTexture()
	{
		if (!(mTex == null))
		{
			Object.DestroyImmediate(mTex);
			mTex = null;
		}
	}

	private void OnDestroy()
	{
		if (mTex != null)
		{
			Object.Destroy(mTex);
		}
	}
}
