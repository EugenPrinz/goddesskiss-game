using System;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UnitRendererCache : MonoBehaviour
{
	[Serializable]
	public class Element
	{
		[SerializeField]
		private string _key;

		[SerializeField]
		private UnitRenderer _prefab;

		public string key => _key;

		public UnitRenderer prefab => _prefab;

		public static Element Create(string key, UnitRenderer prefab)
		{
			Element element = new Element();
			element._key = key;
			element._prefab = prefab;
			return element;
		}
	}

	[SerializeField]
	private List<Element> _elementList;

	private Dictionary<string, Element> _elementDict;

	public List<Element> elementList => _elementList;

	private void Start()
	{
		SetAssetBundleUnit();
	}

	public string MakeRendererId(string path)
	{
		string text = path.Replace(Application.dataPath, "Assets");
		text = text.Replace("\\", "/");
		text = text.Replace(UnitDataRow.PrefabHome + "/", string.Empty);
		int num = text.LastIndexOf("/");
		if (num < 0)
		{
			return text;
		}
		return text.Remove(num);
	}

	public void RefreshElementDict()
	{
		if (_elementList == null || _elementList.Count == 0)
		{
			return;
		}
		_elementDict = new Dictionary<string, Element>();
		foreach (Element element in _elementList)
		{
			_elementDict.Add(element.key, element);
		}
	}

	public UnitRenderer Create(string id)
	{
		if (_elementDict == null)
		{
			RefreshElementDict();
		}
		UnitRenderer prefab = _elementDict[id].prefab;
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab.gameObject);
		return gameObject.GetComponent<UnitRenderer>();
	}

	private void SetAssetBundleUnit()
	{
		List<Element> list = new List<Element>();
		for (int i = 0; i < RemoteObjectManager.instance.regulation.unitDtbl.length; i++)
		{
			string text = RemoteObjectManager.instance.regulation.unitDtbl[i].resourceName + ".assetbundle";
			text = text.ToLower();
			string key = RemoteObjectManager.instance.regulation.unitDtbl[i].prefabId;
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(text);
			if (assetBundle == null)
			{
				continue;
			}
			GameObject gameObject = assetBundle.mainAsset as GameObject;
			if (list.Find((Element element) => element.key == key) == null)
			{
				SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
				{
					skinnedMeshRenderer.sharedMaterial.shader = Shader.Find(skinnedMeshRenderer.sharedMaterial.shader.name);
				}
				MeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
				foreach (MeshRenderer meshRenderer in componentsInChildren2)
				{
					meshRenderer.sharedMaterial.shader = Shader.Find(meshRenderer.sharedMaterial.shader.name);
				}
				list.Add(Element.Create(key, gameObject.GetComponent<UnitRenderer>()));
			}
		}
		_elementList.Clear();
		_elementList = list;
	}
}
