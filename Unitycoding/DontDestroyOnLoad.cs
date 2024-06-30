using UnityEngine;

namespace Unitycoding
{
	public class DontDestroyOnLoad : MonoBehaviour
	{
		private void Awake()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
