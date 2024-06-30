using UnityEngine;

public class SwipeScript : MonoBehaviour
{
	private float fingerStartTime;

	private Vector2 fingerStartPos = Vector2.zero;

	private bool isSwipe;

	private float minSwipeDist = 200f;

	private float maxSwipeTime = 2f;

	private SwipeType currSwipeType;

	private void Update()
	{
		if (Input.touchCount <= 0)
		{
			return;
		}
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			switch (touch.phase)
			{
			case TouchPhase.Began:
				isSwipe = true;
				fingerStartTime = Time.time;
				fingerStartPos = touch.position;
				currSwipeType = SwipeType.NONE;
				break;
			case TouchPhase.Canceled:
				isSwipe = false;
				currSwipeType = SwipeType.NONE;
				break;
			case TouchPhase.Ended:
			{
				float num = Time.time - fingerStartTime;
				float magnitude = (touch.position - fingerStartPos).magnitude;
				if (!isSwipe || !(num < maxSwipeTime) || !(magnitude > minSwipeDist))
				{
					break;
				}
				Vector2 vector = touch.position - fingerStartPos;
				Vector2 zero = Vector2.zero;
				zero = ((!(Mathf.Abs(vector.x) > Mathf.Abs(vector.y))) ? (Vector2.up * Mathf.Sign(vector.y)) : (Vector2.right * Mathf.Sign(vector.x)));
				if (zero.x != 0f)
				{
					if (zero.x > 0f)
					{
						currSwipeType = SwipeType.RIGHT;
					}
					else
					{
						currSwipeType = SwipeType.LEFT;
					}
				}
				if (zero.y != 0f)
				{
					if (zero.y > 0f)
					{
						currSwipeType = SwipeType.UP;
					}
					else
					{
						currSwipeType = SwipeType.DOWN;
					}
				}
				break;
			}
			}
		}
	}

	public SwipeType GetCurrentSwipeType()
	{
		return currSwipeType;
	}

	public void SetSwipteStateNone()
	{
		currSwipeType = SwipeType.NONE;
	}
}
