using UnityEngine;

public class RandomPosition : MonoBehaviour
{
	public bool holdX;

	public bool holdY = true;

	public bool holdZ;

	public Vector3 min = new Vector3(-1f, -1f, -1f);

	public Vector3 max = new Vector3(1f, 1f, 1f);

	private void Start()
	{
		Vector3 localPosition = base.transform.localPosition;
		if (!holdX)
		{
			localPosition.x = Random.Range(min.x, max.x);
		}
		if (!holdY)
		{
			localPosition.y = Random.Range(min.y, max.y);
		}
		if (!holdZ)
		{
			localPosition.z = Random.Range(min.z, max.z);
		}
		base.transform.localPosition = localPosition;
		base.enabled = false;
	}
}
