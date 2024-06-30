using System.Collections.Generic;
using UnityEngine;

public class CameraLikeEffect : MonoBehaviour
{
	public delegate void MyEventHandler();

	[HideInInspector]
	public Dictionary<string, Transform> arLookAt;

	public float LookAtScale;

	public float LookAtTime;

	public Transform tParent;

	public Vector3 vPos;

	public Vector3 ShakeAmount;

	public float ShakeTime;

	private Vector3 originalPosition;

	private float originalScale;

	public event MyEventHandler openEvent;

	public void LookInto(string nPos)
	{
		UIManager.instance.world.camp.mapScrollView.enabled = false;
		float num = arLookAt[nPos].position.x - tParent.position.x;
		float num2 = arLookAt[nPos].position.y - tParent.position.y;
		Vector3 vector = new Vector3(num * -1f, num2 * -1f, 0f);
		iTween.ScaleTo(base.gameObject, iTween.Hash("scale", new Vector3(LookAtScale, LookAtScale, 0f), "islocal", true, "time", LookAtTime, "delay", 0, "oncomplete", "ZoomInEnd", "oncompletetarget", base.gameObject));
		iTween.MoveTo(base.gameObject, vector * LookAtScale, LookAtTime);
	}

	public void LookOut()
	{
		UIManager.instance.world.camp.mapScrollView.enabled = false;
		originalPosition = tParent.position;
		iTween.MoveTo(base.gameObject, originalPosition, LookAtTime);
		iTween.ScaleTo(base.gameObject, iTween.Hash("scale", new Vector3(originalScale, originalScale, 0f), "islocal", true, "time", LookAtTime, "delay", 0, "oncomplete", "ZoomOutEnd", "oncompletetarget", base.gameObject));
	}

	public void ZoomInEnd()
	{
		this.openEvent();
		openEvent -= this.openEvent;
		UIManager.instance.world.camp.mapScrollView.enabled = true;
	}

	public void ZoomOutEnd()
	{
		UIManager.instance.world.camp.mapScrollView.enabled = true;
	}

	public void Shake()
	{
		iTween.ShakeScale(base.gameObject, ShakeAmount, ShakeTime);
	}

	private void Awake()
	{
		originalPosition = base.gameObject.transform.position;
		originalScale = base.gameObject.transform.localScale.x;
		if (arLookAt == null)
		{
			arLookAt = new Dictionary<string, Transform>();
		}
	}
}
