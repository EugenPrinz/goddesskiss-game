using UnityEngine;

public class FollowObject : MonoBehaviour
{
	public GameObject target;

	public Vector3 offset;

	private void LateUpdate()
	{
		if (!(target == null))
		{
			Camera camera = NGUITools.FindCameraForLayer(target.layer);
			Camera camera2 = NGUITools.FindCameraForLayer(base.gameObject.layer);
			Vector3 vector = camera2.ViewportToWorldPoint(camera.WorldToViewportPoint(target.transform.position));
			vector.z = 0f;
			base.transform.position = vector + offset;
		}
	}
}
