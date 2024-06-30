using System;
using System.Collections.Generic;
using UnityEngine;

public class CutInEffectCache : MonoBehaviour
{
	[Serializable]
	public class Element
	{
		[SerializeField]
		private string _key;

		[SerializeField]
		private CutInEffect _lhsPrefab;

		[SerializeField]
		private CutInEffect _rhsPrefab;

		public string key => _key;

		public CutInEffect prefab => _lhsPrefab;

		public CutInEffect lhsPrefab => _lhsPrefab;

		public CutInEffect rhsPrefab => _rhsPrefab;

		public static Element Create(string key, CutInEffect lhs, CutInEffect rhs)
		{
			Element element = new Element();
			element._key = key;
			element._lhsPrefab = lhs;
			element._rhsPrefab = rhs;
			return element;
		}
	}

	public const string PrefabHome = "Assets/Prefabs/CutInEffects";

	[SerializeField]
	private List<Element> _elementList;

	private Dictionary<string, Element> _elementDict;

	public List<Element> elementList => _elementList;

	public string MakeEffectId(string path)
	{
		string text = path.Replace(Application.dataPath, "Assets");
		text = text.Replace("\\", "/");
		text = text.Replace("Assets/Prefabs/CutInEffects/", string.Empty);
		int num = text.LastIndexOf("/");
		if (num < 0)
		{
			return text;
		}
		return text.Remove(num);
	}

	public void RefreshElementDict()
	{
		_elementDict = new Dictionary<string, Element>();
		foreach (Element element in _elementList)
		{
			_elementDict.Add(element.key, element);
		}
	}

	public bool HasEffect(string key)
	{
		if (_elementDict == null)
		{
			RefreshElementDict();
		}
		return _elementDict.ContainsKey(key);
	}

	public CutInEffect Create(string key, CutInEffect.Side side, UnitRenderer unitRenderer)
	{
		if (_elementDict == null)
		{
			RefreshElementDict();
		}
		Element element = _elementDict[key];
		CutInEffect cutInEffect = element.prefab;
		if (side == CutInEffect.Side.Right && element.rhsPrefab != null)
		{
			cutInEffect = element.rhsPrefab;
		}
		CutInEffect component = UnityEngine.Object.Instantiate(cutInEffect.gameObject).GetComponent<CutInEffect>();
		component._side = side;
		component._unitRenderer = unitRenderer;
		component.RefreshDuration();
		return component;
	}
}
