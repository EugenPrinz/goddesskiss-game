using UnityEngine;

public class GP_Quest
{
	public string Id;

	public string Name;

	public string Description;

	public string IconImageUrl;

	public string BannerImageUrl;

	public GP_QuestState state;

	public long LastUpdatedTimestamp;

	public long AcceptedTimestamp;

	public long EndTimestamp;

	public byte[] RewardData;

	public long CurrentProgress;

	public long TargetProgress;

	private Texture2D _icon;

	private Texture2D _banner;

	public Texture2D icon => _icon;

	public Texture2D banner => _banner;

	public void LoadIcon()
	{
		if (!(icon != null))
		{
			SA_WWWTextureLoader sA_WWWTextureLoader = SA_WWWTextureLoader.Create();
			sA_WWWTextureLoader.OnLoad += OnIconLoaded;
			sA_WWWTextureLoader.LoadTexture(IconImageUrl);
		}
	}

	public void LoadBanner()
	{
		if (!(icon != null))
		{
			SA_WWWTextureLoader sA_WWWTextureLoader = SA_WWWTextureLoader.Create();
			sA_WWWTextureLoader.OnLoad += OnBannerLoaded;
			sA_WWWTextureLoader.LoadTexture(BannerImageUrl);
		}
	}

	private void OnBannerLoaded(Texture2D tex)
	{
		_banner = tex;
	}

	private void OnIconLoaded(Texture2D tex)
	{
		_icon = tex;
	}
}
