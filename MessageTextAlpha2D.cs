using UnityEngine;

public class MessageTextAlpha2D : MonoBehaviour
{
	public UILabel __label;

	public UIPlayTween tween;

	public void destroySelf()
	{
		base.gameObject.SetActive(value: false);
	}

	public void initAndStart(string _text)
	{
		SoundManager.PlaySFX("SE_Message_001");
		TweenAlpha component = GetComponent<TweenAlpha>();
		if (component != null)
		{
			destroySelf();
		}
		base.gameObject.SetActive(value: true);
		__label.text = _text;
		startAnimation();
	}

	public void startAnimation()
	{
		tween.tweenGroup = 1;
		tween.resetOnPlay = true;
		tween.Play(forward: true);
	}

	public void endAnimation()
	{
		tween.tweenGroup = 2;
		tween.resetOnPlay = true;
		tween.Play(forward: true);
	}

	private void Start()
	{
		__label.MakePixelPerfect();
	}
}
