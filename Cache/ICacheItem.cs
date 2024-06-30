using UnityEngine;

namespace Cache
{
	public interface ICacheItem
	{
		int CacheID { get; set; }

		GameObject CacheObj { get; }
	}
}
