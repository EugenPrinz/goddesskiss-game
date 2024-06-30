using System;
using System.Collections;
using Cache;
using UnityEngine;

public class UIDropItem : CacheItem
{
	public enum EType
	{
		Gold,
		Item
	}

	[Serializable]
	public class Step1
	{
		public TweenPosition tween_x;

		public TweenPosition tween_y1;

		public TweenPosition tween_y2;

		public float duration;

		public int range_l;

		public int range_r;

		public int range_t;

		public int range_b;

		public int range_y_min;

		public int range_y_max;

		public float time;

		public void Play()
		{
			tween_x.ResetToBeginning();
			tween_y1.ResetToBeginning();
			tween_y2.ResetToBeginning();
			tween_x.Play(forward: true);
			tween_y1.Play(forward: true);
			tween_y2.Play(forward: true);
		}
	}

	[Serializable]
	public class Step2
	{
		public TweenPosition tween_y;

		public int range_y;

		public float duration;

		public float time;

		public void Play()
		{
			tween_y.ResetToBeginning();
			tween_y.Play(forward: true);
		}
	}

	[Serializable]
	public class Step3
	{
		public ITweenPath path;

		public Transform pathPointer;

		public float length = 500f;

		public float time;
	}

	public delegate void EndDelegate();

	public Step1 step1;

	public Step2 step2;

	public Step3 step3;

	public Transform target;

	public UISprite icon;

	[HideInInspector]
	public int value;

	public EndDelegate _End;

	public bool autoDestroy = true;

	public void SetType(EType type)
	{
		switch (type)
		{
		case EType.Gold:
			UISetter.SetSprite(icon, "Goods-gold");
			break;
		case EType.Item:
			UISetter.SetSprite(icon, "ig-random-box-icon");
			break;
		}
	}

	private IEnumerator _Play()
	{
		step1.tween_x.to = new Vector3(UnityEngine.Random.Range(step1.range_l, step1.range_r), UnityEngine.Random.Range(step1.range_b, step1.range_t), 0f);
		int range_y = UnityEngine.Random.Range(step1.range_y_min, step1.range_y_max);
		step1.tween_y1.to = new Vector3(0f, range_y, 0f);
		step1.tween_y2.from = new Vector3(0f, range_y, 0f);
		step1.tween_x.duration = step1.duration;
		step1.tween_y1.duration = step1.duration / 2f;
		step1.tween_y2.delay = step1.tween_y1.duration;
		step1.tween_y2.duration = step1.duration / 2f;
		step1.Play();
		yield return new WaitForSeconds(step1.time);
		step1.tween_x.enabled = false;
		step1.tween_y1.enabled = false;
		step1.tween_y2.enabled = false;
		step2.tween_y.to = new Vector3(0f, step2.range_y, 0f);
		step2.tween_y.delay = 0f;
		step2.tween_y.duration = step2.duration;
		step2.Play();
		yield return new WaitForSeconds(step2.time);
		step2.tween_y.enabled = false;
		step3.path.transform.localPosition = Vector3.zero;
		step3.path.transform.localRotation = Quaternion.identity;
		step3.path.transform.right = target.position - step3.path.transform.position;
		float magnitude = Vector3.Magnitude(icon.transform.InverseTransformPoint(target.position));
		float scale = magnitude / step3.length;
		step3.path.gameObject.transform.localScale = new Vector3(scale, 1f, 1f);
		step3.path.time = step3.time;
		step3.path.gameObject.SetActive(value: true);
		step3.path._End = delegate
		{
			if (_End != null)
			{
				_End();
			}
			if (autoDestroy)
			{
				CacheManager.instance.EtcEffectCache.Release(this);
			}
		};
	}

	private void OnEnable()
	{
		icon.transform.localPosition = Vector3.zero;
		step3.pathPointer.transform.localPosition = Vector3.zero;
		step3.path.gameObject.SetActive(value: false);
		StopAllCoroutines();
		StartCoroutine(_Play());
	}

	private void Update()
	{
		icon.transform.position = step3.pathPointer.position;
		icon.transform.localRotation = step3.pathPointer.localRotation;
		icon.transform.localScale = step3.pathPointer.localScale;
	}
}
