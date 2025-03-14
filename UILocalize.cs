using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/UI/Localize")]
public class UILocalize : MonoBehaviour
{
	public string key;

	private bool mStarted;

	public string value
	{
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			UIWidget component = GetComponent<UIWidget>();
			UILabel uILabel = component as UILabel;
			UISprite uISprite = component as UISprite;
			UITexture uITexture = component as UITexture;
			Font font = null;
			if (LocalizationFontMap.instance != null)
			{
				font = LocalizationFontMap.instance.GetCurrentFont();
			}
			if (uILabel != null)
			{
				UIInput uIInput = NGUITools.FindInParents<UIInput>(uILabel.gameObject);
				if (uIInput != null && uIInput.label == uILabel)
				{
					uIInput.defaultText = value;
					return;
				}
				if (font != null)
				{
					uILabel.trueTypeFont = font;
				}
				uILabel.text = value;
			}
			else if (uISprite != null)
			{
				UIButton uIButton = NGUITools.FindInParents<UIButton>(uISprite.gameObject);
				if (uIButton != null && uIButton.tweenTarget == uISprite.gameObject)
				{
					uIButton.normalSprite = value;
				}
				uISprite.spriteName = value;
				uISprite.MakePixelPerfect();
			}
			else if (uITexture != null)
			{
				UISetter.SetTexture(uITexture, Utility.LoadTexture(value));
				uITexture.MakePixelPerfect();
			}
		}
	}

	private void OnEnable()
	{
		if (mStarted)
		{
			OnLocalize();
		}
	}

	private void Start()
	{
		mStarted = true;
		OnLocalize();
	}

	private void OnLocalize()
	{
		if (string.IsNullOrEmpty(key))
		{
			UILabel component = GetComponent<UILabel>();
			if (component != null)
			{
				key = component.text;
			}
		}
		if (!string.IsNullOrEmpty(key))
		{
			value = Localization.Get(key);
		}
	}
}
