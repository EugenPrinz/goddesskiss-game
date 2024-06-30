using UnityEngine;

public class SetMount : MonoBehaviour
{
	public AbstractMountPoints mountPoints;

	public string mountKey;

	protected Transform target;

	protected Transform tfOrg;

	private void Start()
	{
		if (mountPoints != null)
		{
			target = mountPoints.GetPosition(mountKey);
			if (target != null)
			{
				tfOrg = base.transform.parent;
				base.transform.parent = target;
			}
		}
	}
}
