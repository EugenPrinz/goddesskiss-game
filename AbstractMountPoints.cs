using UnityEngine;

public abstract class AbstractMountPoints : MonoBehaviour
{
	public virtual Transform GetPosition(string key)
	{
		return null;
	}
}
