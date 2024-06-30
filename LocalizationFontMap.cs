using System;
using UnityEngine;

public class LocalizationFontMap : MonoBehaviour
{
	public Font[] fontList;

	private static LocalizationFontMap _singleton;

	public static LocalizationFontMap instance
	{
		get
		{
			if (_singleton == null)
			{
				_singleton = UnityEngine.Object.FindObjectOfType<LocalizationFontMap>();
				GameObject gameObject = GameObject.Find("LocalizationFontMap");
				if (_singleton == null)
				{
					GameObject gameObject2 = new GameObject("LocalizationFontMap");
					_singleton = gameObject2.AddComponent<LocalizationFontMap>();
					if (_singleton == null)
					{
						throw new NullReferenceException();
					}
				}
				_singleton._Init();
			}
			return _singleton;
		}
	}

	private void _Init()
	{
		UnityEngine.Object.DontDestroyOnLoad(instance);
	}

	public Font GetCurrentFont()
	{
		string language = Localization.language;
		if (language.Equals("S_Kr"))
		{
			return fontList[0];
		}
		if (language.Equals("S_En"))
		{
			return fontList[1];
		}
		if (language.Equals("S_Beon"))
		{
			return fontList[2];
		}
		return fontList[0];
	}
}
