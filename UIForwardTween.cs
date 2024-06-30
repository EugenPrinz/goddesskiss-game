using System.Collections.Generic;
using UnityEngine;

public class UIForwardTween : MonoBehaviour
{
	private static HashSet<GameObject> _currentTargetSet = new HashSet<GameObject>();

	public bool beginFromTargetPos;

	public bool setWorldPos;

	public UIPlayTween playTween;

	public UISprite sprite;

	public GameObject target;

	private bool _isFinished;

	private EventDelegate.Callback finishCallback;

	private Transform _cachedTransform;

	public bool isFinished => _isFinished;

	public static bool IsTweenTarget(GameObject target)
	{
		if (target == null)
		{
			return false;
		}
		return _currentTargetSet.Contains(target);
	}

	private void Start()
	{
	}

	private void Awake()
	{
		playTween.resetOnPlay = true;
		playTween.includeChildren = true;
		playTween.onFinished.Add(new EventDelegate(OnFinished));
	}

	private void Update()
	{
		if (target == null)
		{
			return;
		}
		if (setWorldPos)
		{
			Vector3 position = target.transform.position;
			position.x = _cachedTransform.position.x;
			position.y = _cachedTransform.position.y;
		}
		else
		{
			Vector3 localPosition = playTween.transform.InverseTransformPoint(_cachedTransform.position);
			localPosition.z = target.transform.localPosition.z;
			target.transform.localPosition = localPosition;
		}
		target.transform.rotation = _cachedTransform.rotation;
		target.transform.localScale = _GetScale();
		if (sprite != null)
		{
			UIWidget component = target.GetComponent<UIWidget>();
			if (component != null)
			{
				component.color = sprite.color;
			}
			else
			{
				UIRect component2 = target.GetComponent<UIRect>();
				if (component2 != null)
				{
					component2.alpha = sprite.alpha;
				}
			}
		}
		if (_isFinished)
		{
			if (_currentTargetSet.Contains(target))
			{
				_currentTargetSet.Remove(target);
			}
			if (finishCallback != null)
			{
				finishCallback();
			}
			target = null;
			playTween.gameObject.SetActive(value: false);
			Object.Destroy(playTween.gameObject);
		}
	}

	private Vector3 _GetScale()
	{
		Vector3 vector = Vector3.one;
		Transform transform = _cachedTransform;
		Transform transform2 = playTween.transform;
		while (true)
		{
			vector = Vector3.Scale(vector, transform.localScale);
			if (transform == transform2)
			{
				break;
			}
			transform = transform.parent;
		}
		return vector;
	}

	public UIForwardTween Play(GameObject target, bool forward, EventDelegate.Callback onFinished)
	{
		if (_currentTargetSet.Contains(target))
		{
			return null;
		}
		GameObject gameObject = playTween.gameObject;
		GameObject gameObject2 = NGUITools.AddChild(gameObject.transform.parent.gameObject, gameObject);
		gameObject2.SetActive(value: true);
		UIForwardTween componentInChildren = gameObject2.GetComponentInChildren<UIForwardTween>();
		_currentTargetSet.Add(target);
		componentInChildren._Play(target, forward, onFinished);
		return componentInChildren;
	}

	public UIForwardTween Play(GameObject target, bool active)
	{
		EventDelegate.Callback onFinished = null;
		if (active)
		{
			target.SetActive(active);
		}
		else
		{
			onFinished = delegate
			{
				target.SetActive(active);
			};
		}
		return Play(target, active, onFinished);
	}

	private void OnFinished()
	{
		_isFinished = true;
	}

	private void _Play(GameObject target, bool forward, EventDelegate.Callback onFinished)
	{
		this.target = target;
		_isFinished = false;
		finishCallback = onFinished;
		playTween.gameObject.SetActive(value: true);
		playTween.enabled = true;
		if (beginFromTargetPos)
		{
			playTween.transform.position = target.transform.position;
		}
		playTween.Play(forward);
		_cachedTransform = base.transform;
		Update();
	}
}
