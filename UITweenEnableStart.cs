using System.Collections.Generic;
using UnityEngine;

public class UITweenEnableStart : MonoBehaviour
{
	public int tweenGroup;

	public List<UITweener> tweens;

	private void OnEnable()
	{
		Play(forward: true);
	}

	public void Play(bool forward)
	{
		GameObject go = base.gameObject;
		if (tweens.Count == 0)
		{
			NGUITools.SetActive(go, state: false);
			return;
		}
		if (!NGUITools.GetActive(go))
		{
			NGUITools.SetActive(go, state: true);
		}
		int i = 0;
		for (int count = tweens.Count; i < count; i++)
		{
			UITweener uITweener = tweens[i];
			if (uITweener.tweenGroup == tweenGroup)
			{
				uITweener.Play(forward);
				uITweener.ResetToBeginning();
				uITweener.Play(forward);
			}
		}
	}
}
