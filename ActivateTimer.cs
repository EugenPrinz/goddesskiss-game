using System.Collections;
using UnityEngine;

public class ActivateTimer : MonoBehaviour
{
	public int delayMS = 500;

	public GameObject deactiveTarget;

	public GameObject activeTarget;

	private void OnEnable()
	{
		StartCoroutine(Play());
	}

	private IEnumerator Play()
	{
		if (deactiveTarget != null)
		{
			deactiveTarget.SetActive(value: true);
		}
		if (activeTarget != null)
		{
			activeTarget.SetActive(value: false);
		}
		yield return new WaitForSeconds((float)delayMS / 1000f);
		if (deactiveTarget != null)
		{
			deactiveTarget.SetActive(value: false);
		}
		if (activeTarget != null)
		{
			activeTarget.SetActive(value: true);
		}
	}
}
