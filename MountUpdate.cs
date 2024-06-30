using UnityEngine;

public class MountUpdate : MonoBehaviour
{
	public AbstractMountPoints mountPoints;

	public string mountKey;

	protected Transform target;

	private void Start()
	{
		if (mountPoints != null)
		{
			target = mountPoints.GetPosition(mountKey);
		}
	}

	private void Update()
	{
		if (!(target == null))
		{
			base.transform.position = target.position;
			base.transform.rotation = target.rotation;
		}
	}
}
