using UnityEngine;

public class MountPointsSub : AbstractMountPoints
{
	public AbstractMountPoints mountPoints;

	public override Transform GetPosition(string key)
	{
		if (mountPoints == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		return mountPoints.GetPosition(key);
	}
}
