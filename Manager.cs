using UnityEngine;

public abstract class Manager<T> : MonoBehaviour where T : Manager<T>
{
	protected static T manager;

	protected bool initialised;

	public static T GetInstance()
	{
		if (manager == null)
		{
			Create();
		}
		return manager;
	}

	protected static void Create()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = typeof(T).Name;
		manager = (T)gameObject.AddComponent(typeof(T));
	}

	private void Awake()
	{
		if (manager == null)
		{
			if (!initialised)
			{
				Init();
			}
			manager = (T)this;
		}
		else if (manager != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	protected virtual void Init()
	{
		initialised = true;
	}

	private void OnDestroy()
	{
		if (manager == this)
		{
			manager = (T)null;
		}
	}
}
