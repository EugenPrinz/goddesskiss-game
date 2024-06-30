using System.Collections;
using UnityEngine;

public class MyAnimation : MonoBehaviour
{
	private bool bCheck;

	public float fInterval;

	public float fDurationInterval;

	public float fStartInterval;

	public UISpriteAnimation anim;

	public void Init()
	{
		StopCoroutine("StartTime");
		StopCoroutine("countTime");
		StopCoroutine("DelayTime");
		anim.Pause();
		StartCoroutine("StartTime", fStartInterval);
	}

	private IEnumerator StartTime(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		anim.Play();
		StartCoroutine("countTime", fDurationInterval);
	}

	private IEnumerator countTime(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		anim.Pause();
		StartCoroutine("DelayTime", fInterval);
	}

	private IEnumerator DelayTime(float fInterval)
	{
		yield return new WaitForSeconds(fInterval);
		anim.Play();
		StartCoroutine("countTime", fDurationInterval);
	}
}
