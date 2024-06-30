using UnityEngine;

namespace Cache
{
	public abstract class AbstractCacheWithoutPool : AbstractCache
	{
		public override GameObject Create(CacheElement elem, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			GameObject sourcePrefab = elem.sourcePrefab;
			if (!setting.enableCache)
			{
				elem.sourcePrefab = null;
			}
			GameObject gameObject = Object.Instantiate(sourcePrefab);
			Transform transform = gameObject.transform;
			transform.SetParent(parent, worldPositionStays: false);
			transform.localPosition = position;
			transform.localRotation = rotation;
			gameObject.SetActive(value: true);
			return gameObject;
		}
	}
}
