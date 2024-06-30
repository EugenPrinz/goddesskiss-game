using UnityEngine;

public class ObjectView3D : MonoBehaviour
{
	private float rotationSpeed = 10f;

	private float lerpSpeed = 1f;

	private Vector3 theSpeed;

	private Vector3 avgSpeed;

	private bool isDragging;

	private Vector3 targetSpeedX;

	public GameObject pTarget;

	public Animation mAni;

	private string[] animationName;

	private int currentAnimation = 1;

	private Vector3 vUp = new Vector3(0f, 1f, 0f);

	private void OnDestroy()
	{
	}

	public void ChangeAnimation()
	{
		PlayAnimation();
	}

	private void Object3DSetActive()
	{
		pTarget.SetActive(value: true);
	}

	public void InitAnimationClipName()
	{
		Object3DSetActive();
		int clipCount = mAni.GetClipCount();
		animationName = new string[clipCount];
		int num = 0;
		foreach (AnimationState item in mAni)
		{
			animationName[num] = item.clip.name;
			num++;
		}
	}

	public void PlayAnimation()
	{
		mAni.Play(animationName[currentAnimation]);
		currentAnimation++;
		if (currentAnimation >= mAni.GetClipCount())
		{
			currentAnimation = 0;
		}
	}

	private void OnMouseDown1()
	{
		isDragging = true;
	}

	public void OnObjectPress()
	{
		OnMouseDown1();
	}

	private void Update()
	{
		if (Input.GetMouseButton(0) && isDragging)
		{
			theSpeed = new Vector3(0f - Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f);
			avgSpeed = Vector3.Lerp(avgSpeed, theSpeed, Time.deltaTime * 5f);
		}
		else
		{
			if (isDragging)
			{
				theSpeed = avgSpeed;
				isDragging = false;
			}
			float t = Time.deltaTime * lerpSpeed;
			theSpeed = Vector3.Lerp(theSpeed, Vector3.zero, t);
		}
		if (pTarget != null)
		{
			pTarget.transform.Rotate(vUp * theSpeed.x * rotationSpeed, Space.Self);
		}
	}
}
