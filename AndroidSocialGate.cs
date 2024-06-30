using System;
using UnityEngine;

public class AndroidSocialGate : MonoBehaviour
{
	private static AndroidSocialGate _Instance = null;

	public static event Action<bool, string> OnShareIntentCallback;

	public static void StartGooglePlusShare(string text, Texture2D texture = null)
	{
		CheckAndCreateInstance();
		AN_SocialSharingProxy.StartGooglePlusShareIntent(text, (!(texture == null)) ? Convert.ToBase64String(texture.EncodeToPNG()) : string.Empty);
	}

	public static void StartShareIntent(string caption, string message, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		StartShareIntentWithSubject(caption, message, string.Empty, packageNamePattern);
	}

	public static void StartShareIntent(string caption, string message, Texture2D texture, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		StartShareIntentWithSubject(caption, message, string.Empty, texture, packageNamePattern);
	}

	public static void StartShareIntentWithSubject(string caption, string message, string subject, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		AN_SocialSharingProxy.StartShareIntent(caption, message, subject, packageNamePattern);
	}

	public static void StartShareIntentWithSubject(string caption, string message, string subject, Texture2D texture, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		byte[] inArray = texture.EncodeToPNG();
		string media = Convert.ToBase64String(inArray);
		AN_SocialSharingProxy.StartShareIntent(caption, message, subject, media, packageNamePattern);
	}

	public static void SendMail(string caption, string message, string subject, string recipients, Texture2D texture = null)
	{
		CheckAndCreateInstance();
		if (texture != null)
		{
			byte[] inArray = texture.EncodeToPNG();
			string media = Convert.ToBase64String(inArray);
			AN_SocialSharingProxy.SendMailWithImage(caption, message, subject, recipients, media);
		}
		else
		{
			AN_SocialSharingProxy.SendMail(caption, message, subject, recipients);
		}
	}

	private static void CheckAndCreateInstance()
	{
		if (_Instance == null)
		{
			_Instance = UnityEngine.Object.FindObjectOfType(typeof(AndroidSocialGate)) as AndroidSocialGate;
			if (_Instance == null)
			{
				_Instance = new GameObject().AddComponent<AndroidSocialGate>();
				_Instance.gameObject.name = _Instance.GetType().Name;
			}
		}
	}

	private void ShareCallback(string data)
	{
		string[] array = data.Split(new string[1] { "|" }, StringSplitOptions.None);
		bool arg = int.Parse(array[1]) == -1;
		AndroidSocialGate.OnShareIntentCallback(arg, array[0]);
	}

	static AndroidSocialGate()
	{
		AndroidSocialGate.OnShareIntentCallback = delegate
		{
		};
	}
}
