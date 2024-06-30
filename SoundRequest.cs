using System.Collections;
using UnityEngine;

public class SoundRequest : MonoBehaviour
{
	public AudioClip clip;

	public int delayMS;

	public bool isBGM;

	private void OnEnable()
	{
		StartCoroutine(_Play());
	}

	private IEnumerator _Play()
	{
		yield return new WaitForSeconds((float)delayMS * 0.001f);
		if (!isBGM)
		{
			SoundPlayer.PlaySE(clip);
		}
		else
		{
			SoundPlayer.PlayBGM(clip);
		}
	}
}
