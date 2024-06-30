using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine;

public sealed class SpriteSheet : MonoBehaviour
{
	[Serializable]
	public sealed class Element
	{
		public string name;

		public string fingerPrint;

		public Rect frameXyRect;

		public Rect atlasUvRect;
	}

	public static readonly Regex validDirectoryNamePattern = new Regex("(\\/[a-zA-Z0-9\\-]*)+\\/");

	public static readonly Regex validFileNamePattern = new Regex("[0-9]{3}\\-[01][01]\\-(000|090|180|270)\\-[0-9]\\.png\\z");

	public static readonly Regex validPathPattern = new Regex(validDirectoryNamePattern.ToString() + validFileNamePattern.ToString());

	public string spritesPath = "../ExternalAssets/Sprites";

	public string atlasPath = ".";

	public int maxAtlasSize = 1024;

	public int expanding = 2;

	public int padding = 2;

	public float pixelsToUnits = 100f;

	[SerializeField]
	private Material _atlasMaterial;

	[SerializeField]
	private float _unitySpritePixelsToUnits;

	[SerializeField]
	private List<Element> _elementList;

	private List<Mesh> _meshList = new List<Mesh>();

	private Dictionary<string, int> _indexDict = new Dictionary<string, int>();

	private Dictionary<string, int> _countDict = new Dictionary<string, int>();

	private List<Sprite> _unitySpriteList = new List<Sprite>();

	public Material atlasMaterial => _atlasMaterial;

	public float unitySpritePixelsToUnits => _unitySpritePixelsToUnits;

	public List<string> packageKeyList
	{
		get
		{
			if (_elementList.Count == 0)
			{
				return new List<string>();
			}
			if (_meshList.Count == 0)
			{
				_UpdateMeshList();
				_UpdateIndexDictAndCountDict();
			}
			return new List<string>(_countDict.Keys);
		}
	}

	public void Make()
	{
		Dictionary<string, Texture2D> dictionary = new Dictionary<string, Texture2D>();
		List<Element> elementList = _CreateElementList(dictionary);
		Texture2D texture2D = _CreateTexture(maxAtlasSize);
		Texture2D[] textures = _MakeTextures(elementList, dictionary);
		Rect[] array = texture2D.PackTextures(textures, 2 * expanding + padding, maxAtlasSize);
		if (array != null)
		{
			_ExpandSprites(texture2D, array, expanding);
			texture2D.Apply(updateMipmaps: false);
			_ApplyAtlasUvRects(elementList, array);
			_elementList = elementList;
			_UpdateAtlasMaterial(texture2D);
			ClearMeshList();
			dictionary.Clear();
		}
	}

	public void ClearMeshList()
	{
		_meshList.Clear();
		_unitySpriteList.Clear();
		_indexDict.Clear();
		_countDict.Clear();
	}

	public Mesh[] GetMeshes(string packageName)
	{
		if (_elementList.Count == 0)
		{
			return new Mesh[0];
		}
		if (_meshList.Count == 0)
		{
			_UpdateMeshList();
			_UpdateUnitySpriteList();
			_UpdateIndexDictAndCountDict();
		}
		int value = 0;
		if (!_countDict.TryGetValue(packageName, out value))
		{
			ClearMeshList();
			_UpdateMeshList();
			_UpdateUnitySpriteList();
			_UpdateIndexDictAndCountDict();
			if (!_countDict.TryGetValue(packageName, out value))
			{
				return new Mesh[0];
			}
		}
		int num = _indexDict[packageName];
		Mesh[] array = new Mesh[value];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = _meshList[num++];
		}
		return array;
	}

	public Sprite[] GetUnitySprites(string packageName)
	{
		if (_elementList.Count == 0)
		{
			return new Sprite[0];
		}
		if (_meshList.Count == 0)
		{
			_UpdateMeshList();
			_UpdateUnitySpriteList();
			_UpdateIndexDictAndCountDict();
		}
		int value = 0;
		if (!_countDict.TryGetValue(packageName, out value))
		{
			return new Sprite[0];
		}
		int num = _indexDict[packageName];
		Sprite[] array = new Sprite[value];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = _unitySpriteList[num++];
		}
		return array;
	}

	private List<Element> _CreateElementList(Dictionary<string, Texture2D> textureDict)
	{
		string[] files = Directory.GetFiles(Application.dataPath + "/" + spritesPath + "/./", "*.*", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			files[i] = files[i].Replace("\\", "/");
		}
		List<Element> list = new List<Element>();
		string[] array = files;
		foreach (string path in array)
		{
			Element element = _CreateElement(path, textureDict);
			if (element != null)
			{
				list.Add(element);
			}
		}
		list.Sort((Element a, Element b) => string.Compare(a.fingerPrint, b.fingerPrint));
		return list;
	}

	private static Texture2D _CreateTexture(int size)
	{
		return _CreateTexture(size, size);
	}

	private static Texture2D _CreateTexture(int width, int height)
	{
		return new Texture2D(width, height, TextureFormat.ARGB32, mipmap: false);
	}

	private static Texture2D[] _MakeTextures(List<Element> elementList, Dictionary<string, Texture2D> textureDict)
	{
		List<Texture2D> list = new List<Texture2D>();
		string text = string.Empty;
		foreach (Element element in elementList)
		{
			if (text != element.fingerPrint)
			{
				text = element.fingerPrint;
				Texture2D value = null;
				if (textureDict.TryGetValue(text, out value))
				{
					list.Add(value);
				}
			}
		}
		return list.ToArray();
	}

	private static void _ExpandSprites(Texture2D atlas, Rect[] uvRects, int expanding)
	{
		int width = atlas.width;
		int height = atlas.height;
		Color32[] pixels = atlas.GetPixels32();
		Rect[] array = _ConvertToXyRects(width, height, uvRects);
		Rect[] array2 = array;
		foreach (Rect xyRect in array2)
		{
			_ExpandSprite(width, height, pixels, xyRect, expanding);
		}
		atlas.SetPixels32(pixels);
	}

	private static void _ExpandSprite(int w, int h, Color32[] pixels, Rect xyRect, int expanding)
	{
		int num = (int)xyRect.xMin;
		int num2 = (int)xyRect.xMax;
		int num3 = (int)xyRect.yMin;
		int num4 = (int)xyRect.yMax;
		int num5 = Mathf.Max(0, num - expanding);
		int num6 = Mathf.Min(w, num2 + expanding) - 1;
		int num7 = Mathf.Max(0, num3 - expanding);
		int num8 = Mathf.Min(h, num4 + expanding) - 1;
		num2--;
		num4--;
		for (int num9 = num3; num9 >= num7; num9--)
		{
			int num10 = num3;
			Array.Copy(pixels, w * num10 + num, pixels, w * num9 + num, num2 - num + 1);
		}
		for (int i = num4; i <= num8; i++)
		{
			int num11 = num4;
			Array.Copy(pixels, w * num11 + num, pixels, w * i + num, num2 - num + 1);
		}
		for (int j = num7; j <= num8; j++)
		{
			int num12 = w * j;
			for (int num13 = num; num13 >= num5; num13--)
			{
				ref Color32 reference = ref pixels[num12 + num13];
				reference = pixels[num12 + num];
			}
			for (int k = num2; k <= num6; k++)
			{
				ref Color32 reference2 = ref pixels[num12 + k];
				reference2 = pixels[num12 + num2];
			}
		}
	}

	private static Rect[] _ConvertToXyRects(int w, int h, Rect[] uvRects)
	{
		Rect[] array = new Rect[uvRects.Length];
		for (int i = 0; i < uvRects.Length; i++)
		{
			array[i].xMin = Mathf.Round(uvRects[i].xMin * (float)w);
			array[i].xMax = Mathf.Round(uvRects[i].xMax * (float)w);
			array[i].yMin = Mathf.Round(uvRects[i].yMin * (float)h);
			array[i].yMax = Mathf.Round(uvRects[i].yMax * (float)h);
		}
		return array;
	}

	private static Rect[] _ConvertToUvRects(int w, int h, Rect[] xyRects)
	{
		Rect[] array = new Rect[xyRects.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i].xMin = xyRects[i].xMin / (float)w;
			array[i].xMax = xyRects[i].xMax / (float)w;
			array[i].yMin = xyRects[i].yMin / (float)h;
			array[i].yMax = xyRects[i].yMax / (float)h;
		}
		return array;
	}

	private static void _ApplyAtlasUvRects(List<Element> elementList, Rect[] atlasUvRects)
	{
		string text = string.Empty;
		int num = -1;
		foreach (Element element in elementList)
		{
			if (text != element.fingerPrint)
			{
				num++;
				text = element.fingerPrint;
			}
			element.atlasUvRect = atlasUvRects[num];
		}
	}

	private float _GetUnitySpritePixelsToUnits()
	{
		int width = _atlasMaterial.mainTexture.width;
		int height = _atlasMaterial.mainTexture.height;
		double num = 0.0;
		double num2 = 0.0;
		foreach (Element element in _elementList)
		{
			Rect rect = _ConvertToXyRects(width, height, new Rect[1] { element.atlasUvRect })[0];
			num += (double)rect.width;
			num += (double)rect.height;
			num2 += (double)element.frameXyRect.width;
			num2 += (double)element.frameXyRect.height;
		}
		return (float)(num / num2) * pixelsToUnits;
	}

	private void _UpdateAtlasMaterial(Texture2D atlas)
	{
		Shader shader = Shader.Find("Unlit/Transparent Colored");
		if (shader == null)
		{
			shader = Shader.Find("Sprites/Default");
		}
		atlas.filterMode = FilterMode.Bilinear;
		atlas.wrapMode = TextureWrapMode.Clamp;
		atlas.name = "_" + base.name;
		_atlasMaterial = new Material(shader);
		_atlasMaterial.mainTexture = atlas;
		_atlasMaterial.name = atlas.name;
		_unitySpritePixelsToUnits = _GetUnitySpritePixelsToUnits();
	}

	private void _UpdateMeshList()
	{
		List<Mesh> list = new List<Mesh>();
		foreach (Element element in _elementList)
		{
			Mesh item = _CreateMesh(element, pixelsToUnits);
			list.Add(item);
		}
		list.Sort((Mesh a, Mesh b) => string.Compare(a.name, b.name));
		int num = -1;
		string text = string.Empty;
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			Mesh mesh = list[i];
			int num2 = _GetSpriteFrameNum(mesh.name);
			string text2 = _GetSpritePackageName(mesh.name);
			if (text != string.Empty && text != text2)
			{
				num = -1;
			}
			for (int j = num + 1; j < num2; j++)
			{
				Mesh mesh2 = UnityEngine.Object.Instantiate((num >= 0) ? list[i - 1] : mesh);
				mesh2.name = _ReplaceSpriteFrameNum(mesh2.name, j);
				list.Add(mesh2);
			}
			num = num2;
			text = text2;
		}
		list.Sort((Mesh a, Mesh b) => string.Compare(a.name, b.name));
		list.ForEach(delegate(Mesh m)
		{
			m.hideFlags = HideFlags.HideAndDontSave;
		});
		_meshList = list;
	}

	private void _UpdateUnitySpriteList()
	{
		int width = _atlasMaterial.mainTexture.width;
		int height = _atlasMaterial.mainTexture.height;
		List<Sprite> list = new List<Sprite>();
		foreach (Element element in _elementList)
		{
			Rect rect = _ConvertToXyRects(width, height, new Rect[1] { element.atlasUvRect })[0];
			Vector2 pivot = -element.frameXyRect.position;
			pivot.x /= element.frameXyRect.width;
			pivot.y /= element.frameXyRect.height;
			Sprite sprite = Sprite.Create((Texture2D)_atlasMaterial.mainTexture, rect, pivot, _unitySpritePixelsToUnits);
			sprite.name = element.name;
			list.Add(sprite);
		}
		list.Sort((Sprite a, Sprite b) => string.Compare(a.name, b.name));
		int num = -1;
		string text = string.Empty;
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			Sprite sprite2 = list[i];
			int num2 = _GetSpriteFrameNum(sprite2.name);
			string text2 = _GetSpritePackageName(sprite2.name);
			if (text != string.Empty && text != text2)
			{
				num = -1;
			}
			for (int j = num + 1; j < num2; j++)
			{
				Sprite sprite3 = UnityEngine.Object.Instantiate((num >= 0) ? list[i - 1] : sprite2);
				sprite3.name = _ReplaceSpriteFrameNum(sprite3.name, j);
				list.Add(sprite3);
			}
			num = num2;
			text = text2;
		}
		list.Sort((Sprite a, Sprite b) => string.Compare(a.name, b.name));
		list.ForEach(delegate(Sprite s)
		{
			s.hideFlags = HideFlags.HideAndDontSave;
		});
		_unitySpriteList = list;
	}

	private void _UpdateIndexDictAndCountDict()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		int num = 0;
		int num2 = 1;
		string text = string.Empty;
		foreach (Mesh mesh in _meshList)
		{
			string text2 = _GetSpritePackageName(mesh.name);
			if (text != text2)
			{
				dictionary.Add(text2, num);
				dictionary2.Add(text, num2);
				num2 = 0;
			}
			num++;
			num2++;
			text = text2;
		}
		dictionary2.Add(text, num2);
		dictionary2.Remove(string.Empty);
		_indexDict = dictionary;
		_countDict = dictionary2;
	}

	private static int _GetSpriteFrameNum(string spriteName)
	{
		string[] array = spriteName.Split('.', '-');
		return int.Parse(array[array.Length - 4]);
	}

	private static string _GetSpritePackageName(string spriteName)
	{
		return spriteName.Remove(spriteName.LastIndexOf("."));
	}

	private static string _ReplaceSpriteFrameNum(string spriteName, int num)
	{
		string text = _GetSpritePackageName(spriteName);
		string text2 = spriteName.Replace(text + ".", string.Empty);
		int length = "000".Length;
		int length2 = "000-00-000-0".Length - length;
		text2 = text2.Substring(length, length2);
		text2 = $"{num:D3}{text2}";
		return text + "." + text2;
	}

	private static Element _CreateElement(string path, Dictionary<string, Texture2D> textureDict)
	{
		if (!validPathPattern.IsMatch(path))
		{
			return null;
		}
		Element element = new Element();
		element.name = _MakeElementName(path);
		Texture2D texture2D = _CreateTexture(0);
		try
		{
			texture2D.LoadImage(File.ReadAllBytes(path));
		}
		catch (Exception)
		{
			return null;
		}
		Rect rect = _GetTrimmedRect(texture2D);
		Color32[] array = _GetPixelsInRect(texture2D, rect);
		_ClearAlphaZeroPixels(array);
		string text = _GetFingerPrint(array);
		Vector2 vector = new Vector2(texture2D.width / 2, texture2D.height / 2);
		element.fingerPrint = text;
		element.frameXyRect = rect;
		element.frameXyRect.position -= vector;
		if (textureDict == null)
		{
			return element;
		}
		if (!textureDict.ContainsKey(element.fingerPrint))
		{
			Texture2D texture2D2 = _CreateTexture((int)rect.width, (int)rect.height);
			texture2D2.SetPixels32(array);
			texture2D2.Apply(updateMipmaps: false);
			textureDict.Add(text, texture2D2);
		}
		return element;
	}

	private static string _MakeElementName(string path)
	{
		string text = validPathPattern.Match(path).ToString();
		text = text.Substring(1).Replace(".png", string.Empty);
		return text.Replace("/", ".");
	}

	private static Rect _GetTrimmedRect(Texture2D texture)
	{
		int width = texture.width;
		int height = texture.height;
		int num = 0;
		int num2 = width - 1;
		int num3 = 0;
		int num4 = height - 1;
		Color32[] pixels = texture.GetPixels32();
		for (int i = num; i <= num2; i++)
		{
			int num5 = num3;
			while (num5 <= num4)
			{
				if (pixels[width * num5 + i].a == 0)
				{
					num5++;
					continue;
				}
				goto IL_004d;
			}
			continue;
			IL_004d:
			num = i;
			break;
		}
		int num6 = num2;
		while (num6 >= num)
		{
			int num7 = num4;
			while (num7 >= num3)
			{
				if (pixels[width * num7 + num6].a == 0)
				{
					num7--;
					continue;
				}
				goto IL_009b;
			}
			num6--;
			continue;
			IL_009b:
			num2 = num6;
			break;
		}
		for (int j = num3; j <= num4; j++)
		{
			int num8 = num;
			while (num8 <= num2)
			{
				if (pixels[width * j + num8].a == 0)
				{
					num8++;
					continue;
				}
				goto IL_00e9;
			}
			continue;
			IL_00e9:
			num3 = j;
			break;
		}
		int num9 = num4;
		while (num9 >= num3)
		{
			int num10 = num2;
			while (num10 >= num)
			{
				if (pixels[width * num9 + num10].a == 0)
				{
					num10--;
					continue;
				}
				goto IL_0138;
			}
			num9--;
			continue;
			IL_0138:
			num4 = num9;
			break;
		}
		num2++;
		num4++;
		return new Rect(num, num3, Mathf.Max(1, num2 - num), Mathf.Max(1, num4 - num3));
	}

	private static Color32[] _GetPixelsInRect(Texture2D texture, Rect rect)
	{
		int width = texture.width;
		int num = (int)rect.xMin;
		int num2 = (int)rect.xMax;
		int num3 = (int)rect.yMin;
		int num4 = (int)rect.yMax;
		Color32[] pixels = texture.GetPixels32();
		Color32[] array = new Color32[(num2 - num) * (num4 - num3)];
		int num5 = 0;
		for (int i = num3; i < num4; i++)
		{
			for (int j = num; j < num2; j++)
			{
				ref Color32 reference = ref array[num5++];
				reference = pixels[width * i + j];
			}
		}
		return array;
	}

	private static void _ClearAlphaZeroPixels(Color32[] pixels)
	{
		for (int i = 0; i < pixels.Length; i++)
		{
			if (pixels[i].a == 0)
			{
				pixels[i].r = 0;
				pixels[i].g = 0;
				pixels[i].b = 0;
			}
		}
	}

	private static string _GetFingerPrint(Color32[] pixels)
	{
		byte[] array = new byte[pixels.Length * 4];
		int num = 0;
		for (int i = 0; i < pixels.Length; i++)
		{
			Color32 color = pixels[i];
			array[num++] = color.a;
			array[num++] = color.r;
			array[num++] = color.g;
			array[num++] = color.b;
		}
		MD5 mD = MD5.Create();
		byte[] array2 = mD.ComputeHash(array);
		return BitConverter.ToString(array2);
	}

	private static Mesh _CreateMesh(Element element, float pixelsToUnits)
	{
		Mesh mesh = new Mesh();
		mesh.name = element.name;
		string[] array = mesh.name.Split('.', '-');
		int num = int.Parse(array[array.Length - 3]);
		int num2 = int.Parse(array[array.Length - 2]);
		Vector2[] array2 = new Vector2[4]
		{
			new Vector2(element.atlasUvRect.xMin, element.atlasUvRect.yMax),
			new Vector2(element.atlasUvRect.xMax, element.atlasUvRect.yMax),
			new Vector2(element.atlasUvRect.xMin, element.atlasUvRect.yMin),
			new Vector2(element.atlasUvRect.xMax, element.atlasUvRect.yMin)
		};
		Vector3[] array3 = new Vector3[4]
		{
			new Vector3(element.frameXyRect.xMin, element.frameXyRect.yMax),
			new Vector3(element.frameXyRect.xMax, element.frameXyRect.yMax),
			new Vector3(element.frameXyRect.xMin, element.frameXyRect.yMin),
			new Vector3(element.frameXyRect.xMax, element.frameXyRect.yMin)
		};
		Vector3 vector = new Vector3((num / 10 == 0) ? 1 : (-1), (num % 10 == 0) ? 1 : (-1));
		bool flag = false;
		switch (num2)
		{
		case 90:
			flag = true;
			break;
		case 180:
			vector.x = 0f - vector.x;
			vector.y = 0f - vector.y;
			break;
		case 270:
			vector.x = 0f - vector.x;
			flag = true;
			break;
		}
		for (int i = 0; i < array3.Length; i++)
		{
			float num3 = vector.x * array3[i].x / pixelsToUnits;
			float num4 = vector.y * array3[i].y / pixelsToUnits;
			ref Vector3 reference = ref array3[i];
			reference = ((!flag) ? new Vector3(num3, num4) : new Vector3(num4, num3));
		}
		int[] array4 = new int[4] { 0, 1, 2, 3 };
		if (vector.x < 0f)
		{
			array4 = new int[4]
			{
				array4[1],
				array4[0],
				array4[3],
				array4[2]
			};
		}
		if (vector.y < 0f)
		{
			array4 = new int[4]
			{
				array4[2],
				array4[3],
				array4[0],
				array4[1]
			};
		}
		if (flag)
		{
			array4 = new int[4]
			{
				array4[3],
				array4[1],
				array4[2],
				array4[0]
			};
		}
		mesh.vertices = new Vector3[4]
		{
			array3[array4[0]],
			array3[array4[1]],
			array3[array4[2]],
			array3[array4[3]]
		};
		mesh.uv = new Vector2[4]
		{
			array2[array4[0]],
			array2[array4[1]],
			array2[array4[2]],
			array2[array4[3]]
		};
		mesh.triangles = new int[6] { 0, 1, 2, 1, 3, 2 };
		mesh.RecalculateNormals();
		return mesh;
	}
}
