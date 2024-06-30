using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Crosstales.BWF.Util
{
	public static class Helper
	{
		private static readonly Regex lineEndingsRegex = new Regex("\\r\\n|\\r|\\n");

		private static readonly System.Random rd = new System.Random();

		private const string WINDOWS_PATH_DELIMITER = "\\";

		private const string UNIX_PATH_DELIMITER = "/";

		public static bool isInternetAvailable => Application.internetReachability != NetworkReachability.NotReachable;

		public static bool isWindowsPlatform => Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;

		public static bool isMacOSPlatform => Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor;

		public static bool isLinuxPlatform => Application.platform == RuntimePlatform.LinuxPlayer;

		public static bool isEditorMode => (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) && !Application.isPlaying;

		public static bool isSupportedPlatform => isWindowsPlatform || isMacOSPlatform || isLinuxPlatform;

		public static string ValidatePath(string path)
		{
			string text = null;
			if (isWindowsPlatform)
			{
				text = path.Replace('/', '\\');
				if (!text.EndsWith("\\"))
				{
					text += "\\";
				}
			}
			else
			{
				text = path.Replace('\\', '/');
				if (!text.EndsWith("/"))
				{
					text += "/";
				}
			}
			return text;
		}

		public static List<string> SplitStringToLines(string text, int skipHeaderLines = 0, int skipFooterLines = 0, char splitChar = '#')
		{
			List<string> list = new List<string>(200);
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = lineEndingsRegex.Split(text);
				for (int i = 0; i < array.Length; i++)
				{
					if (i + 1 > skipHeaderLines && i < array.Length - skipFooterLines && !string.IsNullOrEmpty(array[i]) && !array[i].StartsWith("#", StringComparison.OrdinalIgnoreCase))
					{
						list.Add(array[i].Split(splitChar)[0]);
					}
				}
			}
			return list;
		}

		public static string CreateReplaceString(string replaceChars, int stringLength)
		{
			if (replaceChars.Length > 1)
			{
				char[] array = new char[stringLength];
				for (int i = 0; i < stringLength; i++)
				{
					array[i] = replaceChars[rd.Next(0, replaceChars.Length)];
				}
				return new string(array);
			}
			if (replaceChars.Length == 1)
			{
				return new string(replaceChars[0], stringLength);
			}
			return string.Empty;
		}

		public static Color HSVToRGB(float h, float s, float v, float a = 1f)
		{
			if (s == 0f)
			{
				return new Color(v, v, v, a);
			}
			h /= 60f;
			int num = Mathf.FloorToInt(h);
			float num2 = h - (float)num;
			float num3 = v * (1f - s);
			float num4 = v * (1f - s * num2);
			float num5 = v * (1f - s * (1f - num2));
			return num switch
			{
				0 => new Color(v, num5, num3, a), 
				1 => new Color(num4, v, num3, a), 
				2 => new Color(num3, v, num5, a), 
				3 => new Color(num3, num4, v, a), 
				4 => new Color(num5, num3, v, a), 
				_ => new Color(v, num3, num4, a), 
			};
		}
	}
}
