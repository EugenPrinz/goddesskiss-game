using UnityEngine;

public class UIDamage : MonoBehaviour
{
	private static UIDamage _singleton;

	public GameObject panel;

	public GameObject prefab;

	public UILabel label;

	public bool isManager;

	private void Awake()
	{
		if (isManager)
		{
			_singleton = this;
		}
	}

	private void OnDestroy()
	{
		if (_singleton == this)
		{
			Object.Destroy(this);
		}
	}
}
