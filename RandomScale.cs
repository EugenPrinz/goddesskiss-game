using UnityEngine;

public class RandomScale : MonoBehaviour
{
	public bool holdX;

	public bool holdY;

	public bool holdZ;

	public bool same;

	public Vector3 min = Vector3.one;

	public Vector3 max = Vector3.one;

	private void Start()
	{
		Vector3 localScale = base.transform.localScale;
		if (same)
		{
			float num = Random.Range(min.x, max.x);
			if (!holdX)
			{
				localScale.x = num;
			}
			if (!holdY)
			{
				localScale.y = num;
			}
			if (!holdZ)
			{
				localScale.z = num;
			}
		}
		else
		{
			if (!holdX)
			{
				localScale.x = Random.Range(min.x, max.x);
			}
			if (!holdY)
			{
				localScale.y = Random.Range(min.y, max.y);
			}
			if (!holdZ)
			{
				localScale.z = Random.Range(min.z, max.z);
			}
		}
		base.transform.localScale = localScale;
		base.enabled = false;
	}
}
