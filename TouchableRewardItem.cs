using System;
using System.Collections;
using UnityEngine;

public class TouchableRewardItem : MonoBehaviour
{
	public TouchableReward.Data data;

	public AnimationCurve curveYAxis;

	public AnimationCurve curveXAxis;

	public Action OnFinished;

	private bool _completePostProcess;

	private void OnCloseScene()
	{
		StopAllCoroutines();
		PostProcess();
	}

	private void OnClosePanel()
	{
		StopAllCoroutines();
		PostProcess();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Awake()
	{
		_completePostProcess = false;
	}

	private void Start()
	{
		_completePostProcess = false;
		StartCoroutine(PopAnimation());
	}

	private IEnumerator PopAnimation()
	{
		GetComponent<Collider>().enabled = data.clickable;
		float time = 0f;
		float duration = UnityEngine.Random.Range(0.8f, 1.5f);
		float mul = 1f / duration;
		Vector3 beginPos4 = base.transform.localPosition;
		Vector3 move = new Vector3(UnityEngine.Random.value * 120f * ((!(UnityEngine.Random.value > 0.5f)) ? (-1f) : 1f), UnityEngine.Random.Range(120f, 200f), 0f);
		Transform t = base.transform;
		Camera _mainCam = Camera.main;
		Camera _uiCam = UICamera.mainCamera;
		while (time < duration || data.clickable)
		{
			time += Time.deltaTime;
			if (data.isWorldObject)
			{
				if (data.from != null)
				{
					beginPos4 = _mainCam.WorldToViewportPoint(data.from.position);
					beginPos4 = _uiCam.ViewportToWorldPoint(beginPos4);
					beginPos4 = t.parent.InverseTransformPoint(beginPos4);
				}
			}
			else
			{
				beginPos4 = data.from.position;
				beginPos4 = t.parent.InverseTransformPoint(beginPos4);
			}
			Vector3 pos = Vector3.zero;
			pos.x = curveXAxis.Evaluate(time * mul) * move.x;
			pos.y = curveYAxis.Evaluate(time * mul) * move.y;
			t.localPosition = pos + beginPos4;
			yield return null;
		}
		if (!data.clickable)
		{
			StartCoroutine(MoveTo());
		}
	}

	private IEnumerator MoveTo()
	{
		float tScale = 5f;
		GetComponent<Collider>().enabled = false;
		Vector3 endPos = data.to.position;
		Transform t = base.transform;
		while (Vector3.Distance(endPos, t.position) > 0.1f)
		{
			t.position = Vector3.Lerp(t.position, endPos, Time.deltaTime * tScale);
			yield return null;
		}
		if (OnFinished != null)
		{
			OnFinished();
			_completePostProcess = true;
		}
		else
		{
			PostProcess();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnClick()
	{
		StopCoroutine("PopAnimation");
		StartCoroutine(MoveTo());
	}

	private void PostProcess()
	{
		if (!_completePostProcess)
		{
			if (data.onFinished != null)
			{
				data.onFinished(data);
			}
			_completePostProcess = true;
		}
	}
}
