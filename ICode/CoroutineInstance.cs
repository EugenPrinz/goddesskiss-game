using System.Collections;
using UnityEngine;

namespace ICode
{
	public class CoroutineInstance : MonoBehaviour
	{
		public Coroutine ProcessWork(IEnumerator routine)
		{
			return StartCoroutine(DestroyWhenComplete(routine));
		}

		public IEnumerator DestroyWhenComplete(IEnumerator routine)
		{
			yield return StartCoroutine(routine);
			Object.Destroy(base.gameObject);
		}
	}
}
