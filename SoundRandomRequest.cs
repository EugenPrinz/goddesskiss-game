using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRandomRequest : MonoBehaviour
{
	public List<AudioClip> clips;

	public int delayMS;

	public bool isBGM;

	private void OnEnable()
	{
		StartCoroutine(_Play());
	}

	private IEnumerator _Play()
	{
		yield return new WaitForSeconds((float)delayMS * 0.001f);
		AudioClip clip = clips[Random.Range(0, clips.Count)];
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
