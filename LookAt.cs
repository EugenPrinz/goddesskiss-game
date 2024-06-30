using UnityEngine;

[ExecuteInEditMode]
public class LookAt : MonoBehaviour
{
	public Transform target;

	public float distance = 10f;

	public bool reset;

	public Vector3 resetAngle = Vector3.zero;

	private void Update()
	{
		if (!(target == null))
		{
			if (reset)
			{
				Vector3 vector = Quaternion.Euler(resetAngle) * Vector3.forward;
				base.transform.position = target.position - vector * distance;
				base.transform.LookAt(target.position);
			}
			else
			{
				base.transform.LookAt(target.position);
				base.transform.position = target.position - base.transform.forward * distance;
			}
		}
	}
}
