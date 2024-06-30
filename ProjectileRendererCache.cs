using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRendererCache : MonoBehaviour
{
	[Serializable]
	public class Element
	{
		[SerializeField]
		private string _key;

		[SerializeField]
		private int _index;

		[SerializeField]
		private ProjectileMotionPhase _prefab;

		public string key => _key;

		public int index => _index;

		public ProjectileMotionPhase prefab => _prefab;

		public static Element Create(string key, int index, ProjectileMotionPhase prefab)
		{
			Element element = new Element();
			element._key = key;
			element._index = index;
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
		SetAssetBundleProjectile();
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

	private ProjectileMotionPhase _CreateMotionPhase(int index)
	{
		Element element = _elementList[index];
		return _CreateMotionPhase(element);
	}

	private ProjectileMotionPhase _CreateMotionPhase(Element element)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(element.prefab.gameObject);
		return gameObject.GetComponent<ProjectileMotionPhase>();
	}

	public ProjectileRenderer Create(int id, Vector3 firePosition, Vector3 targetPosition)
	{
		if (_elementDict == null)
		{
			RefreshElementDict();
		}
		ProjectileRenderer projectileRenderer = null;
		if (projectileRenderer != null)
		{
			ProjectileMotionPhase[] componentsInChildren = projectileRenderer.transform.GetComponentsInChildren<ProjectileMotionPhase>(includeInactive: true);
			foreach (ProjectileMotionPhase projectileMotionPhase in componentsInChildren)
			{
				projectileMotionPhase.elapsedTime = 0;
				if (projectileMotionPhase.transform.GetSiblingIndex() == 0)
				{
					projectileMotionPhase.gameObject.SetActive(value: true);
					projectileMotionPhase.transform.position = firePosition;
				}
				else
				{
					projectileMotionPhase.gameObject.SetActive(value: false);
					projectileMotionPhase.transform.position = targetPosition;
				}
			}
			projectileRenderer.gameObject.SetActive(value: true);
			return projectileRenderer;
		}
		GameObject gameObject = new GameObject();
		ProjectileRenderer projectileRenderer2 = gameObject.AddComponent<ProjectileRenderer>();
		ProjectileMotionPhase projectileMotionPhase2 = _CreateMotionPhase(id / 100000);
		ProjectileMotionPhase projectileMotionPhase3 = _CreateMotionPhase(id % 10000);
		projectileRenderer2.name = $"Projectile-{id:D9}";
		projectileRenderer2.id = id;
		projectileMotionPhase2.transform.parent = projectileRenderer2.transform;
		projectileMotionPhase3.transform.parent = projectileRenderer2.transform;
		projectileMotionPhase2.transform.position = firePosition;
		projectileMotionPhase3.transform.position = targetPosition;
		projectileMotionPhase2.gameObject.SetActive(value: true);
		projectileMotionPhase3.gameObject.SetActive(value: false);
		return projectileRenderer2;
	}

	private void SetAssetBundleProjectile()
	{
		List<Element> list = new List<Element>();
		for (int i = 0; i < RemoteObjectManager.instance.regulation.projectileMotionPhaseDtbl.length; i++)
		{
			string key = RemoteObjectManager.instance.regulation.projectileMotionPhaseDtbl[i].GetKey();
			if (RemoteObjectManager.instance.regulation.projectileMotionPhaseDtbl[i].isHeader)
			{
				list.Add(Element.Create(key, i, null));
				continue;
			}
			string[] array = key.Split('/');
			string url = array[array.Length - 1].ToLower() + ".assetbundle";
			ProjectileMotionPhase prefab = null;
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(url);
			if (assetBundle != null)
			{
				GameObject gameObject = assetBundle.mainAsset as GameObject;
				prefab = gameObject.GetComponent<ProjectileMotionPhase>();
			}
			list.Add(Element.Create(key, i, prefab));
		}
		_elementList.Clear();
		_elementList = list;
	}
}
