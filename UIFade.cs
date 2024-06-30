using UnityEngine;

public class UIFade : UIPanelBase
{
	private static UIFade _singleton;

	public UISprite texture;

	public TweenAlpha inTweener;

	public TweenAlpha outTweener;

	private int _playCount;

	private bool isIn;

	protected override void Awake()
	{
		base.Awake();
		if (_singleton != null && _singleton != this)
		{
			Object.DestroyImmediate(this);
			return;
		}
		_singleton = this;
		SetActive(active: false);
	}

	private void OnDestroy()
	{
		if (_singleton == this)
		{
			_singleton = null;
		}
	}

	private void Start()
	{
		inTweener.enabled = false;
		outTweener.enabled = false;
		inTweener.AddOnFinished(_OnFinished);
		outTweener.AddOnFinished(_OnFinished);
	}

	public static void SetActive(bool active)
	{
		_singleton.gameObject.SetActive(active);
		_singleton.texture.enabled = active;
	}

	public static void In(float duration)
	{
		_singleton._Play(_singleton.inTweener, duration);
	}

	public static void Out(float duration)
	{
		_singleton._Play(_singleton.outTweener, duration);
	}

	private void _Play(UITweener tweener, float duration)
	{
		_playCount++;
		SetActive(active: true);
		tweener.enabled = true;
		tweener.duration = duration;
		tweener.ResetToBeginning();
		tweener.PlayForward();
	}

	private void _OnFinished()
	{
		_playCount--;
		if (_playCount <= 0)
		{
			SetActive(active: false);
		}
	}

	public static void InScenario(float duration, float r = 0f, float g = 0f, float b = 0f, float a = 0f)
	{
		_singleton.isIn = true;
		_singleton._PlayScenarioFade(_singleton.inTweener, duration, r, g, b, a);
	}

	public static void OutScenario(float duration, float r = 0f, float g = 0f, float b = 0f, float a = 0f)
	{
		_singleton.isIn = false;
		_singleton._PlayScenarioFade(_singleton.outTweener, duration, r, g, b, a);
	}

	private void _PlayScenarioFade(TweenAlpha tweener, float duration, float r = 0f, float g = 0f, float b = 0f, float a = 0f)
	{
		_playCount++;
		SetActive(active: true);
		if (_singleton.isIn)
		{
			AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 0.5f), new Keyframe(0.5f, 0.75f), new Keyframe(0.8f, 0.96f), new Keyframe(1f, 1f));
			tweener.animationCurve = animationCurve;
			tweener.from = 1f;
			tweener.to = 0f;
		}
		else
		{
			AnimationCurve animationCurve2 = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 0.1f), new Keyframe(0.5f, 0.25f), new Keyframe(0.8f, 0.64f), new Keyframe(1f, 1f));
			tweener.animationCurve = animationCurve2;
			tweener.from = 0f;
			tweener.to = 1f;
		}
		_singleton.texture.color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
		tweener.enabled = true;
		tweener.duration = duration;
		tweener.ResetToBeginning();
		tweener.PlayForward();
	}
}
