using UnityEngine;

public class ClickEffectHandler : MonoBehaviour
{
	public Camera mianCamera;

	public GameObject goClickEffect;

	public bool bContinueMode;

	public float fTimeInterval;

	private const float fFrontOfCamera = -1f;

	private float beginTime;

	public Vector3 clickPosition;

	private void ShowClick()
	{
		GameObject gameObject = Object.Instantiate(goClickEffect);
		gameObject.transform.parent = base.transform;
		ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
		component.Play();
		Vector3 vector = mianCamera.ScreenToWorldPoint(Input.mousePosition);
		gameObject.transform.position = new Vector3(vector.x, vector.y, vector.z + -1f);
		Object.Destroy(gameObject, component.duration);
		clickPosition = gameObject.transform.position;
	}

	private void OnMouseDown()
	{
		ShowClick();
	}

	private void OnMouseDrag()
	{
		if (bContinueMode)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup - beginTime >= fTimeInterval)
			{
				ShowClick();
				beginTime = realtimeSinceStartup;
			}
		}
	}

	private void Start()
	{
		beginTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (Input.touchCount == 1)
			{
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Began && touch.tapCount == 1)
				{
					ShowClick();
				}
			}
		}
		else if (Input.GetMouseButtonDown(0))
		{
			ShowClick();
		}
	}
}
