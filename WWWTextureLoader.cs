using System;
using System.Collections;
using UnityEngine;

public class WWWTextureLoader : MonoBehaviour
{
	private string _url;

	public event Action<Texture2D> OnLoad = delegate
	{
	};

	public static WWWTextureLoader Create()
	{
		return new GameObject("WWWTextureLoader").AddComponent<WWWTextureLoader>();
	}

	public void LoadTexture(string url)
	{
		_url = url;
		StartCoroutine(LoadCoroutin());
	}

	private IEnumerator LoadCoroutin()
	{
		WWW www = new WWW(_url);
		yield return www;
		if (www.error == null)
		{
			this.OnLoad(www.texture);
		}
		else
		{
			this.OnLoad(null);
		}
	}
}
