using System.Collections;
using UnityEngine;

public class UIShake : MonoBehaviour
{
	public Transform target;

	public float minShakeRange = 1f;

	public float maxShakeRange = 2f;

	public float shakeTime = 0.2f;

	public float term = 0.02f;

	public float beginDelay;

	private Vector3 _targetOriPos;

	private float _remainShakeTime;

	private bool _isShaking;

	public bool shakeOnStart;

	public bool shakeOnEnable;

	private void Start()
	{
		if (target == null)
		{
			target = base.transform;
		}
		_targetOriPos = target.localPosition;
		_isShaking = false;
		if (shakeOnStart)
		{
			Begin();
		}
	}

	private void OnEnable()
	{
		if (shakeOnEnable)
		{
			if (target == null)
			{
				target = base.transform;
			}
			_targetOriPos = target.localPosition;
			_isShaking = false;
			Begin();
		}
	}

	private void Update()
	{
	}

	public void ForceSetOriginalPos(Vector3 pos)
	{
		if (!_isShaking)
		{
			_targetOriPos = pos;
		}
	}

	public void Begin()
	{
		Begin(shakeTime, beginDelay);
	}

	public void Begin(float time, float min, float max, float delay = 0f)
	{
		minShakeRange = min;
		maxShakeRange = max;
		Begin(time);
	}

	public void Begin(float time, float delay = 0f)
	{
		_remainShakeTime = time;
		if (!_isShaking)
		{
			StartCoroutine(Shake());
		}
	}

	public void BeginScenarioSceen(float time, float min = 1f, float max = 2f)
	{
		_remainShakeTime = time;
		if (!_isShaking)
		{
			StartCoroutine(ShakeScenarioSceen(min, max));
		}
	}

	public void Stop()
	{
		StopAllCoroutines();
		target.transform.localPosition = _targetOriPos;
	}

	public void StopShakeScenarioSceen()
	{
		_isShaking = false;
		StopAllCoroutines();
		target.transform.localPosition = _targetOriPos;
	}

	private IEnumerator Shake()
	{
		if (target == null)
		{
			target = base.transform;
		}
		_isShaking = true;
		if (beginDelay > 0f)
		{
			yield return new WaitForSeconds(beginDelay);
		}
		while (_remainShakeTime > 0f)
		{
			_remainShakeTime -= term;
			Vector2 rand = Random.insideUnitCircle * Random.Range(minShakeRange, maxShakeRange);
			target.localPosition = _targetOriPos + new Vector3(rand.x, rand.y);
			yield return new WaitForSeconds(term);
		}
		_isShaking = false;
		target.localPosition = _targetOriPos;
		_remainShakeTime = 0f;
	}

	private IEnumerator ShakeScenarioSceen(float min = 1f, float max = 2f)
	{
		if (target == null)
		{
			target = base.transform;
		}
		_isShaking = true;
		while (_remainShakeTime > 0f)
		{
			_remainShakeTime -= Time.deltaTime;
			Vector2 rand = Random.insideUnitCircle * Random.Range(min, max);
			target.localPosition = _targetOriPos + new Vector3(rand.x, rand.y);
			yield return null;
		}
		_isShaking = false;
		target.localPosition = _targetOriPos;
		_remainShakeTime = 0f;
	}

	public bool IsShake()
	{
		return _isShaking;
	}
}
