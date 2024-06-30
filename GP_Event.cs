using UnityEngine;

public class GP_Event
{
	public string Id;

	public string Description;

	public string IconImageUrl;

	public string FormattedValue;

	public long Value;

	private Texture2D _icon;

	public Texture2D icon => _icon;

	public void LoadIcon()
	{
		if (!(icon != null))
		{
			SA_WWWTextureLoader sA_WWWTextureLoader = SA_WWWTextureLoader.Create();
			sA_WWWTextureLoader.OnLoad += OnTextureLoaded;
			sA_WWWTextureLoader.LoadTexture(IconImageUrl);
		}
	}

	private void OnTextureLoaded(Texture2D tex)
	{
		_icon = tex;
	}
}
