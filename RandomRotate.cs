using UnityEngine;

public class RandomRotate : MonoBehaviour
{
	public bool holdX;

	public bool holdY;

	public bool holdZ;

	public Vector3 min = Vector3.zero;

	public Vector3 max = new Vector3(360f, 360f, 360f);

	private void Start()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (!holdX)
		{
			localEulerAngles.x = Random.Range(min.x, max.x);
		}
		if (!holdY)
		{
			localEulerAngles.y = Random.Range(min.y, max.y);
		}
		if (!holdZ)
		{
			localEulerAngles.z = Random.Range(min.z, max.z);
		}
		base.transform.localRotation = Quaternion.Euler(localEulerAngles);
		base.enabled = false;
	}
}
