using System;
using UnityEngine;

namespace Cache
{
	[Serializable]
	public class CacheItem : MonoBehaviour, ICacheItem
	{
		public virtual int CacheID { get; set; }

		public GameObject CacheObj => base.gameObject;
	}
}
