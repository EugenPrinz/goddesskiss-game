using System;
using System.Globalization;
using UnityEngine;

namespace Unitycoding
{
	public static class UnityUtility
	{
		private static AudioSource audioSource;

		public static void PlaySound(AudioClip clip, float volume)
		{
			if (clip == null)
			{
				return;
			}
			if (audioSource == null)
			{
				AudioListener audioListener = UnityEngine.Object.FindObjectOfType<AudioListener>();
				if (audioListener != null)
				{
					audioSource = audioListener.GetComponent<AudioSource>();
					if (audioSource == null)
					{
						audioSource = audioListener.gameObject.AddComponent<AudioSource>();
					}
				}
			}
			if (audioSource != null)
			{
				audioSource.PlayOneShot(clip, volume);
			}
		}

		public static string ColorToHex(Color32 color)
		{
			return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		}

		public static Color HexToColor(string hex)
		{
			hex = hex.Replace("0x", string.Empty);
			hex = hex.Replace("#", string.Empty);
			byte a = byte.MaxValue;
			byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
			if (hex.Length == 8)
			{
				a = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
			}
			return new Color32(r, g, b, a);
		}

		public static string ColorString(string value, Color color)
		{
			return "<color=#" + ColorToHex(color) + ">" + value + "</color>";
		}

		public static string Replace(string source, string oldString, string newString)
		{
			int num = source.IndexOf(oldString, StringComparison.CurrentCultureIgnoreCase);
			if (num >= 0)
			{
				source = source.Remove(num, oldString.Length);
				source = source.Insert(num, newString);
			}
			return source;
		}

		public static bool IsNumeric(object expression)
		{
			if (expression == null)
			{
				return false;
			}
			double result;
			return double.TryParse(Convert.ToString(expression, CultureInfo.InvariantCulture), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out result);
		}

		public static GameObject FindChild(this GameObject target, string name, bool includeInactive)
		{
			if (target != null)
			{
				if ((target.name == name && includeInactive) || (target.name == name && !includeInactive && target.activeInHierarchy))
				{
					return target;
				}
				for (int i = 0; i < target.transform.childCount; i++)
				{
					GameObject gameObject = target.transform.GetChild(i).gameObject.FindChild(name, includeInactive);
					if (gameObject != null)
					{
						return gameObject;
					}
				}
			}
			return null;
		}

		public static void Stretch(this RectTransform rectTransform, RectOffset offset)
		{
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.sizeDelta = new Vector2(-(offset.right + offset.left), -(offset.bottom + offset.top));
			rectTransform.anchoredPosition = new Vector2((float)offset.left + rectTransform.sizeDelta.x * rectTransform.pivot.x, (float)(-offset.top) - rectTransform.sizeDelta.y * (1f - rectTransform.pivot.y));
		}

		public static void Stretch(this RectTransform rectTransform)
		{
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.anchoredPosition = Vector2.zero;
		}
	}
}
