using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class LocalStorage
{
	[Serializable]
	public class UserLoginData
	{
		public string id;

		public string pw;

		public int platform;
	}

	private static List<UserLoginData> loginData = new List<UserLoginData>();

	public static void SaveLoginData(string id, string pw, int platform)
	{
		if (platform == 2 || platform == 3)
		{
			UserLoginData userLoginData = FindLoginDataPlatform(platform);
			if (userLoginData == null)
			{
				loginData.Add(new UserLoginData
				{
					id = id,
					pw = pw,
					platform = platform
				});
			}
			else
			{
				userLoginData.id = id;
			}
		}
		else if (FindLoginData(id) == null)
		{
			loginData.Add(new UserLoginData
			{
				id = id,
				pw = pw,
				platform = platform
			});
		}
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, loginData);
		PlayerPrefs.SetString("LoginData", Convert.ToBase64String(memoryStream.GetBuffer()));
	}

	public static List<UserLoginData> LoadLoginData()
	{
		string @string = PlayerPrefs.GetString("LoginData");
		if (!string.IsNullOrEmpty(@string))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream serializationStream = new MemoryStream(Convert.FromBase64String(@string));
			loginData = (List<UserLoginData>)binaryFormatter.Deserialize(serializationStream);
		}
		return loginData;
	}

	public static void RemoveLoginData(string id)
	{
		loginData.Remove(FindLoginData(id));
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, loginData);
		PlayerPrefs.SetString("LoginData", Convert.ToBase64String(memoryStream.GetBuffer()));
	}

	private static UserLoginData FindLoginData(string id)
	{
		return loginData.Find((UserLoginData row) => row.id == id);
	}

	private static UserLoginData FindLoginDataPlatform(int platform)
	{
		return loginData.Find((UserLoginData row) => row.platform == platform);
	}

	public static void SetPref(string key, string value)
	{
		PlayerPrefs.SetString(key, value);
	}

	public static string GetPref(string key, string defaultValue)
	{
		return PlayerPrefs.GetString(key, defaultValue);
	}

	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}
}
