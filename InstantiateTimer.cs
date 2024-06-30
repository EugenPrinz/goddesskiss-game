using System.Collections;
using UnityEngine;

public class InstantiateTimer : MonoBehaviour
{
	public string prefabPath;

	public int delayMS;

	public Transform targetPosition;

	private void OnEnable()
	{
		StartCoroutine(Play());
	}

	private IEnumerator Play()
	{
		yield return new WaitForSeconds((float)delayMS * 0.001f);
		GameObject go = Utility.LoadAndInstantiateGameObject(prefabPath);
		if (targetPosition == null)
		{
			targetPosition = base.transform;
		}
		go.transform.position = targetPosition.position;
	}
}
