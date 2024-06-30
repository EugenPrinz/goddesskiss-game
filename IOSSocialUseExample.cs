using System.Collections;
using UnityEngine;

public class IOSSocialUseExample : MonoBehaviour
{
	private GUIStyle style;

	private GUIStyle style2;

	public Texture2D drawTexture;

	public Texture2D textureForPost;

	private void Awake()
	{
		IOSSocialManager.OnFacebookPostResult += HandleOnFacebookPostResult;
		IOSSocialManager.OnTwitterPostResult += HandleOnTwitterPostResult;
		IOSSocialManager.OnInstagramPostResult += HandleOnInstagramPostResult;
		IOSSocialManager.OnMailResult += OnMailResult;
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

	private void OnGUI()
	{
		float num = 20f;
		float num2 = 10f;
		GUI.Label(new Rect(num2, num, Screen.width, 40f), "Twitter", style);
		num += 40f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Post"))
		{
			ISN_Singleton<IOSSocialManager>.instance.TwitterPost("Twitter posting test");
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Post Screenshot"))
		{
			StartCoroutine(PostTwitterScreenshot());
		}
		num += 80f;
		num2 = 10f;
		GUI.Label(new Rect(num2, num, Screen.width, 40f), "Facebook", style);
		num += 40f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Post"))
		{
			ISN_Singleton<IOSSocialManager>.instance.FacebookPost("Facebook posting test");
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Post Screenshot"))
		{
			StartCoroutine(PostFBScreenshot());
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Post Image"))
		{
			ISN_Singleton<IOSSocialManager>.instance.FacebookPost("Hello world", "https://www.assetstore.unity3d.com/en/#!/publisher/2256", textureForPost);
		}
		num += 80f;
		num2 = 10f;
		GUI.Label(new Rect(num2, num, Screen.width, 40f), "Native Sharing", style);
		num += 40f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Text"))
		{
			ISN_Singleton<IOSSocialManager>.instance.ShareMedia("Some text to share");
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Screenshot"))
		{
			StartCoroutine(PostScreenshot());
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Send Mail"))
		{
			ISN_Singleton<IOSSocialManager>.instance.SendMail("Mail Subject", "Mail Body  <strong> text html </strong> ", "mail1@gmail.com, mail2@gmail.com", textureForPost);
		}
		num += 80f;
		num2 = 10f;
		GUI.Label(new Rect(num2, num, Screen.width, 40f), "Instagram", style);
		num += 40f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Post image from camera"))
		{
			IOSCamera.OnImagePicked += OnPostImageInstagram;
			ISN_Singleton<IOSCamera>.Instance.PickImage(ISN_ImageSource.Camera);
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Post Screenshot"))
		{
			StartCoroutine(PostScreenshotInstagram());
		}
		num += 80f;
		num2 = 10f;
		GUI.Label(new Rect(num2, num, Screen.width, 40f), "WhatsApp", style);
		num += 40f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Share Text"))
		{
			ISN_Singleton<IOSSocialManager>.Instance.WhatsAppShareText("Some text");
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Share Image"))
		{
			ISN_Singleton<IOSSocialManager>.Instance.WhatsAppShareImage(textureForPost);
		}
	}

	private void OnPostImageInstagram(IOSImagePickResult result)
	{
		if (result.IsSucceeded)
		{
			Object.Destroy(drawTexture);
			drawTexture = result.Image;
		}
		else
		{
			IOSMessage.Create("ERROR", "Image Load Failed");
		}
		ISN_Singleton<IOSSocialManager>.instance.InstagramPost(drawTexture, "Some text to share");
		IOSCamera.OnImagePicked -= OnPostImageInstagram;
	}

	private IEnumerator PostScreenshotInstagram()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, mipmap: false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		ISN_Singleton<IOSSocialManager>.instance.InstagramPost(tex, "Some text to share");
		Object.Destroy(tex);
	}

	private IEnumerator PostScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, mipmap: false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		ISN_Singleton<IOSSocialManager>.instance.ShareMedia("Some text to share", tex);
		Object.Destroy(tex);
	}

	private IEnumerator PostTwitterScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, mipmap: false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		ISN_Singleton<IOSSocialManager>.instance.TwitterPost("My app Screenshot", null, tex);
		Object.Destroy(tex);
	}

	private IEnumerator PostFBScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, mipmap: false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		ISN_Singleton<IOSSocialManager>.instance.FacebookPost("My app Screenshot", null, tex);
		Object.Destroy(tex);
	}

	private void HandleOnInstagramPostResult(ISN_Result res)
	{
		if (res.IsSucceeded)
		{
			IOSNativePopUpManager.showMessage("Posting example", "Post Success!");
		}
		else
		{
			IOSNativePopUpManager.showMessage("Posting example", "Post Failed :(");
		}
	}

	private void HandleOnTwitterPostResult(ISN_Result res)
	{
		if (res.IsSucceeded)
		{
			IOSNativePopUpManager.showMessage("Posting example", "Post Success!");
		}
		else
		{
			IOSNativePopUpManager.showMessage("Posting example", "Post Failed :(");
		}
	}

	private void HandleOnFacebookPostResult(ISN_Result res)
	{
		if (res.IsSucceeded)
		{
			IOSNativePopUpManager.showMessage("Posting example", "Post Success!");
		}
		else
		{
			IOSNativePopUpManager.showMessage("Posting example", "Post Failed :(");
		}
	}

	private void OnMailResult(ISN_Result result)
	{
		if (result.IsSucceeded)
		{
			IOSNativePopUpManager.showMessage("Posting example", "Mail Sent");
		}
		else
		{
			IOSNativePopUpManager.showMessage("Posting example", "Mail Failed :(");
		}
	}
}
