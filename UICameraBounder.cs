using UnityEngine;

public class UICameraBounder : MonoBehaviour
{
	public enum Side
	{
		Left,
		Right,
		Top,
		Bottom
	}

	public Side boundSide;

	public Transform target;

	public GameObject tweenAnchor;

	private Camera _cam;

	private void Start()
	{
		_cam = UICamera.mainCamera;
	}

	private void Update()
	{
		if (UIForwardTween.IsTweenTarget(tweenAnchor))
		{
			return;
		}
		float num = 10f * Time.deltaTime;
		Vector3 vector = _cam.WorldToViewportPoint(base.transform.position);
		switch (boundSide)
		{
		case Side.Left:
			if (vector.x < 0f)
			{
				target.transform.Translate(num, 0f, 0f, Space.Self);
			}
			break;
		case Side.Right:
			if (vector.x > 1f)
			{
				target.transform.Translate(0f - num, 0f, 0f, Space.Self);
			}
			break;
		case Side.Top:
			if (vector.y > 1f)
			{
				target.transform.Translate(0f, 0f - num, 0f, Space.Self);
			}
			break;
		case Side.Bottom:
			if (vector.y < 0f)
			{
				target.transform.Translate(0f, num, 0f, Space.Self);
			}
			break;
		}
	}
}
