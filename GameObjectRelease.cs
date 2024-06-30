using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectRelease : MonoBehaviour
{
	public GameObjectPool pool;

	public float delay;

	public List<EventDelegate> onRelease = new List<EventDelegate>();

	private IEnumerator Release()
	{
		yield return new WaitForSeconds(delay);
		EventDelegate.Execute(onRelease);
		if (pool != null)
		{
			pool.Release(base.gameObject);
		}
		else
		{
			Object.DestroyImmediate(base.gameObject);
		}
	}

	private void OnEnable()
	{
		StopAllCoroutines();
		StartCoroutine("Release");
	}
}
