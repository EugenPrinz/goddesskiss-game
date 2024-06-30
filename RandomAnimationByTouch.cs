using System.Collections.Generic;
using UnityEngine;

public class RandomAnimationByTouch : MonoBehaviour
{
	public Animation ani;

	public List<string> aniList = new List<string>();

	public void OnEnable()
	{
		if (!(ani == null) && aniList.Count > 0)
		{
			ani.Play(aniList[0]);
		}
	}

	public void OnClick()
	{
		if (!(ani == null) && aniList.Count > 0)
		{
			ani.Play(aniList[Random.Range(1, aniList.Count)]);
		}
	}

	private void Update()
	{
		if (!(ani == null) && !ani.isPlaying)
		{
			ani.Play(aniList[0]);
		}
	}
}
