using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public static class Utility
{
	private static StringBuilder _sb = new StringBuilder();

	public static readonly HashSet<Type> _TimedObjectTypeSet = new HashSet<Type>
	{
		typeof(Animation),
		typeof(Animator),
		typeof(ParticleSystem),
		typeof(Rigidbody),
		typeof(Rigidbody2D),
		typeof(AudioSource)
	};

	public static string GetGameObjectPath(GameObject go)
	{
		string text = "/" + go.name;
		while (go.transform.parent != null)
		{
			go = go.transform.parent.gameObject;
			text = "/" + go.name + text;
		}
		return text;
	}

	public static string ReadTextAsset(string textAssetPath)
	{
		TextAsset textAsset = Resources.Load<TextAsset>(textAssetPath);
		if (textAsset == null)
		{
			return string.Empty;
		}
		return textAsset.text;
	}

	public static T DeserializeJson<T>(string textAssetPath)
	{
		return JsonConvert.DeserializeObject<T>(ReadTextAsset(textAssetPath));
	}

	public static string[][] ReadCSV(string textAssetPath)
	{
		return ReadCSV(Resources.Load<TextAsset>(textAssetPath));
	}

	public static string[][] ReadCSV(TextAsset asset)
	{
		if (asset == null)
		{
			return null;
		}
		try
		{
			string[] array = asset.text.Trim().Split('\n');
			string[][] array2 = new string[array.Length][];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].Split(',');
			}
			return array2;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static bool WriteString(string path, string str)
	{
		try
		{
			using StreamWriter streamWriter = new StreamWriter(path, append: false, Encoding.UTF8);
			streamWriter.Write(str);
			streamWriter.Flush();
			streamWriter.Close();
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static Texture LoadTexture(string path)
	{
		return Resources.Load<Texture>(path);
	}

	public static GameObject LoadAndInstantiateGameObject(string path, Transform parent = null)
	{
		GameObject gameObject = Resources.Load<GameObject>(path);
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		gameObject2.name = gameObject.name;
		Transform transform = gameObject.transform;
		Transform transform2 = gameObject2.transform;
		transform2.parent = parent;
		transform2.localScale = transform.localScale;
		transform2.localPosition = transform.localPosition;
		transform2.localRotation = transform.localRotation;
		if (parent != null)
		{
			SetLayer(gameObject2, parent.gameObject.layer);
		}
		return gameObject2;
	}

	public static T LoadAndInstantiateGameObject<T>(string path, Transform parent = null) where T : MonoBehaviour
	{
		GameObject gameObject = LoadAndInstantiateGameObject(path, parent);
		if (gameObject == null)
		{
			return (T)null;
		}
		return gameObject.GetComponent<T>();
	}

	public static T FindOrCreateGameObject<T>(string path, Transform parent = null) where T : MonoBehaviour
	{
		T val = UnityEngine.Object.FindObjectOfType<T>();
		if (val != null)
		{
			return val;
		}
		return LoadAndInstantiateGameObject<T>(path, parent);
	}

	public static GameObject LoadAndInstantiateUnitRenderer(string path, Transform parent = null)
	{
		AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(path + ".assetbundle");
		GameObject gameObject = assetBundle.LoadAsset(path + ".prefab") as GameObject;
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		gameObject2.name = gameObject.name;
		Transform transform = gameObject.transform;
		Transform transform2 = gameObject2.transform;
		transform2.parent = parent;
		transform2.localScale = transform.localScale;
		transform2.localPosition = transform.localPosition;
		transform2.localRotation = transform.localRotation;
		if (parent != null)
		{
			SetLayer(gameObject2, parent.gameObject.layer);
		}
		return gameObject2;
	}

	public static void SetLayer(GameObject go, int layer)
	{
		go.layer = layer;
		Transform transform = go.transform;
		int i = 0;
		for (int childCount = transform.childCount; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			SetLayer(child.gameObject, layer);
		}
	}

	public static string GetTimeString(double seconds, bool summary = false)
	{
		if (seconds <= 0.0)
		{
			return string.Empty;
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		_sb.Length = 0;
		if (summary)
		{
			if (timeSpan.Days > 0)
			{
				_sb.AppendFormat("{0}일", timeSpan.Days);
			}
			else if (timeSpan.Hours > 1)
			{
				_sb.AppendFormat("{0}시간", timeSpan.Hours);
			}
			else if (timeSpan.Minutes > 0)
			{
				_sb.AppendFormat("{0}분{1}초", timeSpan.Minutes, timeSpan.Seconds);
			}
			else if (timeSpan.Seconds > 0)
			{
				_sb.AppendFormat("{0}초", timeSpan.Seconds);
			}
		}
		else
		{
			if (timeSpan.Days > 0)
			{
				_sb.Append(Localization.Format("5768", timeSpan.Days));
			}
			if ((float)(int)timeSpan.TotalHours > 0f)
			{
				_sb.Append(Localization.Format("5769", timeSpan.Hours));
			}
			if ((float)(int)timeSpan.TotalMinutes > 0f)
			{
				_sb.Append(Localization.Format("5770", timeSpan.Minutes));
			}
			if (timeSpan.Days <= 0 && timeSpan.TotalSeconds > 0.0)
			{
				_sb.Append(Localization.Format("5771", timeSpan.Seconds));
			}
		}
		return _sb.ToString();
	}

	public static string GetTimeSimpleString(double seconds, bool summary = false)
	{
		if (seconds <= 0.0)
		{
			return string.Empty;
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		_sb.Length = 0;
		if (summary)
		{
			if (timeSpan.Days > 0)
			{
				_sb.AppendFormat("{0}일", timeSpan.Days);
			}
			else if (timeSpan.Hours > 1)
			{
				_sb.AppendFormat("{0}시간", timeSpan.Hours);
			}
			else if (timeSpan.Minutes > 0)
			{
				_sb.AppendFormat("{0}분{1}초", timeSpan.Minutes, timeSpan.Seconds);
			}
			else if (timeSpan.Minutes <= 0)
			{
				_sb.Append("1분 미만");
			}
		}
		else
		{
			if (timeSpan.Days > 0)
			{
				_sb.Append(Localization.Format("5768", timeSpan.Days));
			}
			if ((float)(int)timeSpan.TotalHours > 0f)
			{
				_sb.Append(Localization.Format("5769", timeSpan.Hours));
			}
			if ((float)(int)timeSpan.TotalMinutes > 0f)
			{
				_sb.Append(Localization.Format("5770", timeSpan.Minutes));
			}
			if (timeSpan.Days <= 0 && (float)(int)timeSpan.TotalMinutes <= 0f)
			{
				_sb.Append(Localization.Format("5771", timeSpan.Seconds));
			}
		}
		return _sb.ToString();
	}

	public static string GetTimeHM_MSString(double seconds, bool summary = false)
	{
		if (seconds <= 0.0)
		{
			return string.Empty;
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		_sb.Length = 0;
		if (summary)
		{
			if (timeSpan.Days > 0)
			{
				_sb.AppendFormat("{0}일", timeSpan.Days);
			}
			else if (timeSpan.Hours > 1)
			{
				_sb.AppendFormat("{0}시간", timeSpan.Hours);
			}
			else if (timeSpan.Minutes > 0)
			{
				_sb.AppendFormat("{0}분{1}초", timeSpan.Minutes, timeSpan.Seconds);
			}
			else if (timeSpan.Minutes <= 0)
			{
				_sb.Append("1분 미만");
			}
		}
		else
		{
			if (timeSpan.Days > 0)
			{
				_sb.Append(Localization.Format("5768", timeSpan.Days));
			}
			if ((float)(int)timeSpan.TotalHours > 0f)
			{
				_sb.Append(Localization.Format("5769", timeSpan.Hours));
			}
			if ((float)(int)timeSpan.TotalMinutes > 0f)
			{
				_sb.Append(Localization.Format("5770", timeSpan.Minutes));
			}
			if (timeSpan.Days < 1 && (float)(int)timeSpan.TotalHours < 1f)
			{
				_sb.Append(Localization.Format("5771", timeSpan.Seconds));
			}
		}
		return _sb.ToString();
	}

	public static string GetTimeStringColonFormat(double seconds)
	{
		if (seconds <= 0.0)
		{
			return "00:00";
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		_sb.Length = 0;
		if (timeSpan.TotalHours >= 1.0)
		{
			_sb.AppendFormat("{0:00}:", (int)timeSpan.TotalHours);
		}
		_sb.AppendFormat("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
		return _sb.ToString();
	}

	public static string GetTimeStringSimpleColonFormat(double seconds)
	{
		if (seconds <= 0.0)
		{
			return "00";
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		_sb.Length = 0;
		if (timeSpan.Days > 0)
		{
			_sb.Append(Localization.Format("5768", timeSpan.Days));
			_sb.Append(Localization.Format("5769", timeSpan.Hours));
		}
		else if (timeSpan.TotalMinutes >= 1.0)
		{
			_sb.AppendFormat("{0:00}:{1:00}", timeSpan.Hours, timeSpan.Minutes);
		}
		else
		{
			_sb.Append(Localization.Format("5771", timeSpan.Seconds));
		}
		return _sb.ToString();
	}

	public static string GetTimeStringSimpleStringColonFormat(double seconds)
	{
		if (seconds <= 0.0)
		{
			return "00";
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		_sb.Length = 0;
		if (timeSpan.TotalMinutes >= 1.0)
		{
			_sb.AppendFormat("{0:00}:{1}", timeSpan.Hours, Localization.Format("5770", $"{timeSpan.Minutes:00}"));
		}
		else
		{
			_sb.Append(Localization.Format("5771", timeSpan.Seconds));
		}
		return _sb.ToString();
	}

	public static string GetTimeEventRemainFormat(double seconds, bool summary = false)
	{
		if (seconds <= 0.0)
		{
			return string.Empty;
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		_sb.Length = 0;
		if (timeSpan.Days > 0)
		{
			_sb.Append(Localization.Format("5768", timeSpan.Days));
		}
		if ((float)(int)timeSpan.TotalHours > 0f)
		{
			_sb.Append(Localization.Format("5769", timeSpan.Hours));
		}
		if ((float)(int)timeSpan.TotalMinutes > 0f)
		{
			_sb.Append(Localization.Format("5770", timeSpan.Minutes));
		}
		if (timeSpan.Minutes <= 0)
		{
			_sb.Append(Localization.Format("5771", timeSpan.Seconds));
		}
		return _sb.ToString();
	}

	public static string GetTimeStringFromMS(int ms, bool summary = false)
	{
		return GetTimeString((double)ms / 1000.0, summary);
	}

	public static double ToSeconds(double days, double hours, double minutes, double seconds)
	{
		return days * 86400.0 + hours * 3600.0 + minutes * 60.0 + seconds;
	}

	public static int GetRandomNumberByRate(List<int> rateList)
	{
		if (rateList == null || rateList.Count <= 0)
		{
			return 0;
		}
		List<int> list = new List<int>(rateList);
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			num = (list[i] = num + list[i]);
		}
		int num3 = UnityEngine.Random.Range(0, num);
		for (int j = 0; j < list.Count; j++)
		{
			if (num3 < list[j])
			{
				return j;
			}
		}
		rateList.ForEach(delegate
		{
		});
		return -1;
	}

	public static ReturnType GetPropertyValue<ReturnType>(object obj, string propName, object[] index)
	{
		Type type = obj.GetType();
		PropertyInfo property = type.GetProperty(propName);
		if (property == null)
		{
			return default(ReturnType);
		}
		object obj2 = property.GetGetMethod().Invoke(obj, index);
		return (ReturnType)obj2;
	}

	public static void SetPropertyValue(object obj, string propName, object value, object[] index)
	{
		Type type = obj.GetType();
		PropertyInfo property = type.GetProperty(propName);
		if (property != null && property.GetSetMethod(nonPublic: true) != null)
		{
			property.SetValue(obj, value, index);
		}
	}

	public static Vector3 ConvertPosWorldToUI(Vector3 worldPos)
	{
		Camera main = Camera.main;
		Camera mainCamera = UICamera.mainCamera;
		Vector3 position = main.WorldToViewportPoint(worldPos);
		position.z = 0f;
		return mainCamera.ViewportToWorldPoint(position);
	}

	public static void CopyToSameNameProperties(object src, object dest)
	{
		if (src == null || dest == null)
		{
			return;
		}
		PropertyInfo[] properties = src.GetType().GetProperties();
		Type type = dest.GetType();
		foreach (PropertyInfo propertyInfo in properties)
		{
			PropertyInfo property = type.GetProperty(propertyInfo.Name);
			if (property != null && propertyInfo.PropertyType == property.PropertyType)
			{
				property.SetValue(dest, propertyInfo.GetGetMethod().Invoke(src, null), null);
			}
		}
	}

	public static DateTime ConvertToDateTime(string yyyymmddFormatString)
	{
		if (string.IsNullOrEmpty(yyyymmddFormatString) || yyyymmddFormatString.Length != 8)
		{
			return default(DateTime);
		}
		string s = yyyymmddFormatString.Substring(0, 4);
		string s2 = yyyymmddFormatString.Substring(4, 2);
		string s3 = yyyymmddFormatString.Substring(6, 2);
		int result = 0;
		if (!int.TryParse(s, out result))
		{
			return default(DateTime);
		}
		int result2 = 0;
		if (!int.TryParse(s2, out result2))
		{
			return default(DateTime);
		}
		int result3 = 0;
		if (!int.TryParse(s3, out result3))
		{
			return default(DateTime);
		}
		return new DateTime(result, result2, result3);
	}

	public static void SampleAnimationEnd(Animation ani, string clipName)
	{
		ani.Play(clipName);
		ani.Stop();
		AnimationClip clip = ani.GetClip(clipName);
		clip.SampleAnimation(ani.gameObject, clip.length);
	}

	public static void SampleAnimationStart(Animation ani, string clipName)
	{
		ani.Play(clipName);
		ani.Stop();
		AnimationClip clip = ani.GetClip(clipName);
		clip.SampleAnimation(ani.gameObject, 0f);
	}

	public static T Find<T>(string path)
	{
		GameObject gameObject = GameObject.Find(path);
		if (gameObject == null)
		{
			return default(T);
		}
		return gameObject.GetComponent<T>();
	}

	public static string GetPureId(string itemId, string itemPrefix)
	{
		if (string.IsNullOrEmpty(itemId))
		{
			return null;
		}
		if (!itemId.StartsWith(itemPrefix))
		{
			return itemId;
		}
		return itemId.Substring(itemPrefix.Length);
	}

	public static UnityEngine.Object[] FindTimedObjects(Transform tf)
	{
		List<UnityEngine.Object> list = new List<UnityEngine.Object>();
		_FindTimedObjects(tf, list);
		return list.ToArray();
	}

	public static void _FindTimedObjects(Transform t, List<UnityEngine.Object> dst)
	{
		Component[] components = t.GetComponents<Component>();
		Component[] array = components;
		foreach (Component component in array)
		{
			if (_TimedObjectTypeSet.Contains(component.GetType()))
			{
				dst.Add(component);
			}
		}
		for (int j = 0; j < t.childCount; j++)
		{
			_FindTimedObjects(t.GetChild(j), dst);
		}
	}

	public static void SetBlindNotice(int idx)
	{
		string @string = PlayerPrefs.GetString("Notice");
		if (!ContainsBlineNoticeIdx(idx.ToString()))
		{
			@string = $"{@string}/{idx}";
			PlayerPrefs.SetString("Notice", @string);
		}
	}

	public static void SetBlindNoticeResetCheck()
	{
		string @string = PlayerPrefs.GetString("Notice");
		if (string.IsNullOrEmpty(@string))
		{
			@string = $"{DateTime.Now.Day}-";
			PlayerPrefs.SetString("Notice", @string);
			return;
		}
		int day = DateTime.Now.Day;
		string[] array = @string.Split('-');
		int num = int.Parse(array[0].ToString());
		if (day != num)
		{
			PlayerPrefs.DeleteKey("Notice");
			SetBlindNoticeResetCheck();
		}
	}

	public static string[] GetBlineNoticeIdxList()
	{
		string @string = PlayerPrefs.GetString("Notice");
		string[] array = @string.Split('-');
		return array[1].Split('/');
	}

	private static bool ContainsBlineNoticeIdx(string idx)
	{
		bool result = false;
		string[] blineNoticeIdxList = GetBlineNoticeIdxList();
		for (int i = 0; i < blineNoticeIdxList.Length; i++)
		{
			if (!string.IsNullOrEmpty(blineNoticeIdxList[i]) && blineNoticeIdxList[i] == idx)
			{
				result = true;
			}
		}
		return result;
	}

	public static bool PossibleChar(string str)
	{
		char[] array = str.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			bool flag = false;
			int num = Convert.ToInt32(array[i]);
			if (num >= 48 && num <= 57)
			{
				flag = true;
			}
			else if (num >= 65 && num <= 90)
			{
				flag = true;
			}
			else if (num >= 97 && num <= 122)
			{
				flag = true;
			}
			else if (num >= 44032 && num <= 55215)
			{
				flag = true;
			}
			else if (num >= 19968 && num <= 40959)
			{
				flag = true;
			}
			else if (num >= 12352 && num <= 12447)
			{
				flag = true;
			}
			else if (num >= 12448 && num <= 12543)
			{
				flag = true;
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	public static string GetStringToDay(double time)
	{
		if (time == 0.0)
		{
			return string.Empty;
		}
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(time).ToLocalTime();
		string language = Localization.language;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Length = 0;
		switch (language)
		{
		case "S_Kr":
		case "S_Beon":
		case "S_Gan":
			stringBuilder.Append(Localization.Format("5134", dateTime.Year));
			stringBuilder.Append("/");
			stringBuilder.Append(Localization.Format("5135", dateTime.Month));
			stringBuilder.Append("/");
			stringBuilder.Append(Localization.Format("5768", dateTime.Day));
			break;
		case "S_En":
			stringBuilder.Append(Localization.Format("5135", dateTime.Month));
			stringBuilder.Append("/");
			stringBuilder.Append(Localization.Format("5768", dateTime.Day));
			stringBuilder.Append("/");
			stringBuilder.Append(Localization.Format("5134", dateTime.Year));
			break;
		default:
			stringBuilder.Append(Localization.Format("5134", dateTime.Year));
			stringBuilder.Append("/");
			stringBuilder.Append(Localization.Format("5135", dateTime.Month));
			stringBuilder.Append("/");
			stringBuilder.Append(Localization.Format("5768", dateTime.Day));
			break;
		}
		stringBuilder.Append(" ");
		stringBuilder.Append(dateTime.Hour);
		stringBuilder.Append(":");
		stringBuilder.Append(dateTime.Minute.ToString("D2"));
		stringBuilder.Append(":");
		stringBuilder.Append(dateTime.Second.ToString("D2"));
		return stringBuilder.ToString();
	}

	public static string GetStringToDay(DateTime dt)
	{
		string language = Localization.language;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Length = 0;
		switch (language)
		{
		case "S_Kr":
		case "S_Beon":
		case "S_Gan":
			stringBuilder.Append(Localization.Format("5134", dt.Year));
			stringBuilder.Append("/");
			stringBuilder.Append(Localization.Format("5135", dt.Month));
			stringBuilder.Append("/");
			stringBuilder.Append(Localization.Format("5768", dt.Day));
			break;
		case "S_En":
			stringBuilder.Append(Localization.Format("5135", dt.Month));
			stringBuilder.Append("/");
			stringBuilder.Append(Localization.Format("5768", dt.Day));
			stringBuilder.Append("/");
			stringBuilder.Append(Localization.Format("5134", dt.Year));
			break;
		default:
			stringBuilder.Append(Localization.Format("5134", dt.Year));
			stringBuilder.Append("/");
			stringBuilder.Append(Localization.Format("5135", dt.Month));
			stringBuilder.Append("/");
			stringBuilder.Append(Localization.Format("5768", dt.Day));
			break;
		}
		stringBuilder.Append(" ");
		stringBuilder.Append(dt.Hour);
		stringBuilder.Append(":");
		stringBuilder.Append(dt.Minute.ToString("D2"));
		stringBuilder.Append(":");
		stringBuilder.Append(dt.Second.ToString("D2"));
		return stringBuilder.ToString();
	}

	public static int GetEventBattleDeckIndex(int eventId)
	{
		int result = -1;
		for (int i = 0; i < ConstValue.eventBattleSaveDeckCount; i++)
		{
			string key = $"EventBattleDeck_{i}";
			if (PlayerPrefs.HasKey(key))
			{
				string[] array = PlayerPrefs.GetString(key).Split('_');
				if (int.Parse(array[0]) == eventId)
				{
					result = i;
					break;
				}
			}
		}
		return result;
	}

	public static void UpdateEventBattleDeck(List<int> idList)
	{
		for (int i = 0; i < ConstValue.eventBattleSaveDeckCount; i++)
		{
			string key = $"EventBattleDeck_{i}";
			if (PlayerPrefs.HasKey(key))
			{
				string[] array = PlayerPrefs.GetString(key).Split('_');
				if (!idList.Contains(int.Parse(array[0])))
				{
					PlayerPrefs.DeleteKey(key);
				}
			}
		}
	}

	public static void ClearEventBattleDeck()
	{
		for (int i = 0; i < ConstValue.eventBattleSaveDeckCount; i++)
		{
			string key = $"EventBattleDeck_{i}";
			if (PlayerPrefs.HasKey(key))
			{
				PlayerPrefs.DeleteKey(key);
			}
		}
	}

	public static string ReadableFileSize(long byteCount)
	{
		string result = "0 Bytes";
		if ((double)byteCount >= 1073741824.0)
		{
			result = $"{(double)byteCount / 1073741824.0:##.##}" + " GB";
		}
		else if ((double)byteCount >= 1048576.0)
		{
			result = $"{(double)byteCount / 1048576.0:##.##}" + " MB";
		}
		else if ((double)byteCount >= 1024.0)
		{
			result = $"{(double)byteCount / 1024.0:##.##}" + " KB";
		}
		else if (byteCount > 0 && (double)byteCount < 1024.0)
		{
			result = byteCount + " Bytes";
		}
		return result;
	}

	public static void SetAtlas(this UISprite sprite, string atlasName)
	{
		if (!(sprite.atlas.name == atlasName))
		{
			sprite.atlas = Resources.Load<UIAtlas>(ConstValue.AtlasPath + atlasName);
		}
	}

	public static void SetAtlasImage(this UISprite sprite, string atlasName, string spriteName)
	{
		sprite.SetAtlas(atlasName);
		sprite.spriteName = spriteName;
	}
}
