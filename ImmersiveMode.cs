using UnityEngine;

public class ImmersiveMode : SA_Singleton<ImmersiveMode>
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void EnableImmersiveMode()
	{
		AN_ImmersiveModeProxy.enableImmersiveMode();
	}
}
