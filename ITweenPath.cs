using System;
using System.Collections.Generic;
using UnityEngine;

public class ITweenPath : MonoBehaviour
{
	[Serializable]
	public class Path
	{
		public Transform[] path;
	}

	public delegate void EndDelegate();

	public GameObject target;

	public List<Path> paths;

	public iTween.EaseType easeType = iTween.EaseType.easeInOutSine;

	public float time;

	public int rootIdx;

	public EndDelegate _End;

	private void OnEnable()
	{
		iTween.Stop(target);
		target.transform.localPosition = Vector3.zero;
		target.transform.localRotation = Quaternion.identity;
		target.transform.localScale = Vector3.one;
		rootIdx = UnityEngine.Random.Range(0, paths.Count);
		Play();
	}

	private void Play()
	{
		iTween.MoveTo(target, iTween.Hash("path", paths[rootIdx].path, "time", time, "easetype", easeType, "oncomplete", "OnComplete", "oncompletetarget", base.gameObject));
	}

	public void OnComplete()
	{
		if (_End != null)
		{
			_End();
		}
	}
}
