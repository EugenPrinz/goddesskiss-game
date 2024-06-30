using UnityEngine;

public class PlaySFXSound : MonoBehaviour
{
	public string strSFXName;

	public int delayMS;

	public bool loop;

	public SWP_TimeGroupController timeController;

	protected int _timeStack;

	protected bool enableUpdate;

	private void OnEnable()
	{
		_timeStack = 0;
		enableUpdate = true;
		TimeControllerManager instance = TimeControllerManager.instance;
		if (instance != null)
		{
			timeController = instance.GameMain;
		}
	}

	private void Update()
	{
		if (enableUpdate)
		{
			_timeStack += (int)(1000f * ((!(timeController != null)) ? Time.deltaTime : timeController.TimedDeltaTime()));
			if (_timeStack >= delayMS)
			{
				SoundManager.PlaySFX(strSFXName, loop);
				enableUpdate = false;
			}
		}
	}
}
