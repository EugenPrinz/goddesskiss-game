using UnityEngine;

public class MessageTextUp2D : MonoBehaviour
{
	private UILabel __label;

	private Vector3 __pos;

	private Vector3 __initPos;

	public Camera guiCamera;

	public Camera worldCamera;

	public GameObject targetObject;

	public float tweenTime = 1f;

	public int tweenDistance = 40;

	public TweenPosition tweenPosition
	{
		get
		{
			TweenPosition tweenPosition = GetComponent<TweenPosition>();
			if (tweenPosition == null)
			{
				tweenPosition = base.gameObject.AddComponent<TweenPosition>();
			}
			return tweenPosition;
		}
	}

	public TweenAlpha tweenAlpha
	{
		get
		{
			TweenAlpha tweenAlpha = GetComponent<TweenAlpha>();
			if (tweenAlpha == null)
			{
				tweenAlpha = base.gameObject.AddComponent<TweenAlpha>();
			}
			return tweenAlpha;
		}
	}

	public void destroySelf()
	{
		TweenAlpha component = GetComponent<TweenAlpha>();
		component.ResetToBeginning();
		TweenPosition component2 = GetComponent<TweenPosition>();
		component2.ResetToBeginning();
		base.gameObject.SetActive(value: false);
	}

	public void initAndStartMoving(Vector3 _position, string _text)
	{
		TweenAlpha component = GetComponent<TweenAlpha>();
		if (component != null)
		{
			destroySelf();
		}
		base.gameObject.SetActive(value: true);
		__label.text = _text;
		Vector3 initPos_ = new Vector3(_position.x, _position.y + 0.1f, _position.z);
		startMoveUp(initPos_, tweenTime, tweenDistance);
	}

	public void initAndStartMoving(string _text, Vector3 wldPos)
	{
		TweenAlpha component = GetComponent<TweenAlpha>();
		if (component != null)
		{
			destroySelf();
		}
		base.gameObject.SetActive(value: true);
		__label.text = _text;
		startMoveUp(wldPos, tweenTime, tweenDistance);
	}

	public void startMoveUp(Vector3 initPos_, float tweenDuration_, int tweenEnd_)
	{
		__initPos = initPos_;
		calculatePosition(initPos_);
		TweenPosition tweenPosition = this.tweenPosition;
		tweenPosition.duration = tweenDuration_;
		tweenPosition.from = base.transform.localPosition;
		tweenPosition.to = tweenPosition.from + Vector3.up * tweenEnd_;
		tweenPosition.eventReceiver = base.gameObject;
		tweenPosition.callWhenFinished = "destroySelf";
		tweenPosition.PlayForward();
		TweenAlpha tweenAlpha = this.tweenAlpha;
		tweenAlpha.duration = tweenDuration_;
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0.3f;
		tweenAlpha.PlayForward();
	}

	public void calculatePosition(Vector3 pos_)
	{
		__pos = pos_;
		base.transform.position = __pos;
	}

	public void Awake()
	{
		__label = GetComponent<UILabel>();
	}

	private void Start()
	{
		guiCamera = UICamera.mainCamera;
		__label.MakePixelPerfect();
	}
}
