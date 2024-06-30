using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteLayeredAnimation : MonoBehaviour
{
	public SpriteSheet spriteSheet;

	public string key;

	public string animationName;

	public string dir;

	public float fps = 1f;

	public float frame;

	private string preKey;

	private string preAnimationName;

	private string preDir;

	private float preFPS;

	private int _sortingOrderOffset;

	public bool loop = true;

	public List<SpriteAnimation> layerList;

	public Dictionary<string, List<string>> layerDict;

	public string GetCurrentPackageName(int layerIndex)
	{
		if (layerList == null || layerList.Count <= layerIndex)
		{
			return null;
		}
		return layerList[layerIndex].packageName;
	}

	private void Start()
	{
	}

	public void SetSortingOrderOffset(int offset)
	{
		if (_sortingOrderOffset == offset)
		{
			return;
		}
		_sortingOrderOffset = offset;
		if (layerList.Count > 0)
		{
			for (int i = 0; i < layerList.Count; i++)
			{
				layerList[i].sortingOrderOffset = offset;
			}
		}
	}

	public void ForceInit()
	{
		preKey = null;
		preAnimationName = null;
		preDir = null;
		preFPS = 0f;
		Update();
	}

	private void Update()
	{
		if (key != preKey)
		{
			preKey = key;
			CreateLayer();
		}
		if (preAnimationName != animationName || preDir != dir)
		{
			ChangeAnimation(animationName);
		}
		if (preFPS != fps)
		{
			for (int i = 0; i < layerList.Count; i++)
			{
				layerList[i].fps = fps;
			}
			preFPS = fps;
		}
	}

	private void OnDrawGizmos()
	{
	}

	public void PlayOnce(string aniName)
	{
		loop = false;
		ChangeAnimation(aniName);
	}

	public bool IsPlaying()
	{
		for (int i = 0; i < layerList.Count; i++)
		{
			if (layerList[i].isPlaying)
			{
				return true;
			}
		}
		return false;
	}

	private void ChangeAnimation(string nextAniName)
	{
		if (layerDict == null || layerDict.Count <= 0)
		{
			CreateLayer();
			if (!layerDict.ContainsKey(nextAniName))
			{
				spriteSheet.ClearMeshList();
				CreateLayer();
			}
		}
		if (!layerDict.ContainsKey(nextAniName))
		{
			animationName = ((!string.IsNullOrEmpty(preAnimationName)) ? preAnimationName : "Idle");
			dir = ((!string.IsNullOrEmpty(preDir)) ? preDir : "L");
			return;
		}
		preAnimationName = nextAniName;
		preDir = dir;
		List<string> list = layerDict[animationName];
		for (int i = 0; i < layerList.Count; i++)
		{
			SpriteAnimation spriteAnimation = layerList[i];
			if (i >= list.Count)
			{
				spriteAnimation.gameObject.SetActive(value: false);
				continue;
			}
			spriteAnimation.gameObject.SetActive(value: true);
			spriteAnimation.spriteSheet = spriteSheet;
			string text = ((!string.IsNullOrEmpty(list[i])) ? ("." + list[i]) : string.Empty);
			if (string.IsNullOrEmpty(dir))
			{
				spriteAnimation.packageName = $"{key}.{animationName}{text}";
				spriteAnimation.isLoop = loop;
				continue;
			}
			spriteAnimation.packageName = $"{key}.{animationName}.{dir}{text}";
			spriteAnimation.isLoop = loop;
		}
	}

	private void CreateLayer()
	{
		if (layerList != null && layerList.Count > 0)
		{
			for (int i = 0; i < layerList.Count; i++)
			{
				Object.DestroyImmediate(layerList[i].gameObject);
			}
			layerList.Clear();
		}
		if (layerList == null)
		{
			layerList = new List<SpriteAnimation>();
		}
		List<string> packageKeyList = spriteSheet.packageKeyList;
		Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
		for (int j = 0; j < packageKeyList.Count; j++)
		{
			string text = packageKeyList[j];
			if (text.StartsWith(key))
			{
				string[] array = text.Split('.');
				string text2 = array[1];
				if (!dictionary.ContainsKey(text2))
				{
					dictionary.Add(text2, new List<string>());
				}
				string item = string.Empty;
				if (array.Length > 3)
				{
					item = array[3];
				}
				if (!dictionary[text2].Contains(item))
				{
					dictionary[text2].Add(item);
				}
			}
		}
		int num = int.MinValue;
		foreach (List<string> value in dictionary.Values)
		{
			if (num < value.Count)
			{
				num = value.Count;
			}
		}
		layerDict = dictionary;
		for (int k = 0; k < num; k++)
		{
			string text3 = k.ToString("00");
			Transform transform = base.transform.Find(text3);
			GameObject gameObject = ((!(transform != null)) ? new GameObject(text3) : transform.gameObject);
			gameObject.layer = base.gameObject.layer;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			SpriteAnimation spriteAnimation = gameObject.AddComponent<SpriteAnimation>();
			spriteAnimation.spriteSheet = spriteSheet;
			spriteAnimation.fps = fps;
			spriteAnimation.sortingOrderOffset = _sortingOrderOffset;
			layerList.Add(spriteAnimation);
		}
		preFPS = fps;
		preAnimationName = null;
	}
}
