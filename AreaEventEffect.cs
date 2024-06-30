using UnityEngine;

public class AreaEventEffect : MonoBehaviour
{
	public float duration = 3f;

	public float scale = 3f;

	public float angulerSpeed = 90f;

	private AnimationCurve[] _scaleCurves;

	private float _elapsedTime;

	private void Start()
	{
		float[] array = new float[4]
		{
			0f * duration,
			0.25f * duration,
			0.75f * duration,
			1f * duration
		};
		float num = scale;
		_scaleCurves = new AnimationCurve[3]
		{
			AnimationCurve.EaseInOut(array[0], 0f, array[1], num),
			AnimationCurve.Linear(array[1], num, array[2], num),
			AnimationCurve.EaseInOut(array[2], num, array[3], 0f)
		};
	}

	private void Update()
	{
		_elapsedTime += Time.deltaTime;
		if (_elapsedTime > duration)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		for (int i = 0; i < _scaleCurves.Length; i++)
		{
			AnimationCurve animationCurve = _scaleCurves[i];
			float time = animationCurve[animationCurve.length - 1].time;
			if (_elapsedTime <= time)
			{
				float num = animationCurve.Evaluate(_elapsedTime);
				base.transform.localScale = new Vector3(num, num, num);
				base.transform.Rotate(0f, angulerSpeed * Time.deltaTime, 0f);
				break;
			}
		}
	}
}
