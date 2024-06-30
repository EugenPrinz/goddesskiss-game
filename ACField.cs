using UnityEngine;

[ExecuteInEditMode]
public class ACField : MonoBehaviour
{
	public float lookTargetZ;

	public float distance = 10f;

	public float angleX = 10f;

	public float angleY = 135f;

	public SplitScreenDrawSide drawSide;

	public Camera cam;

	public Transform camTransform;

	public Transform camLookTarget;

	private void Update()
	{
		camLookTarget.transform.localPosition = new Vector3(0f, 0f, lookTargetZ);
		Vector3 vector = Quaternion.Euler(angleX, angleY, 0f) * Vector3.forward;
		camTransform.position = vector * distance + camLookTarget.transform.position;
		camTransform.LookAt(camLookTarget);
	}
}
