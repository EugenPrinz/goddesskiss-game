using UnityEngine;

public class UIFlipSwitch : MonoBehaviour
{
	public GameObject on;

	public GameObject off;

	public GameObject fLock;

	private SwitchStatus state;

	public void On()
	{
		Set(SwitchStatus.ON);
	}

	public void Off()
	{
		Set(SwitchStatus.OFF);
	}

	public void Lock(bool _isLock)
	{
		if (_isLock)
		{
			state = SwitchStatus.LOCK;
			on.SetActive(!_isLock);
		}
		else
		{
			state = SwitchStatus.OFF;
			on.SetActive(state == SwitchStatus.ON);
			off.SetActive(state == SwitchStatus.OFF);
		}
		fLock.SetActive(_isLock);
	}

	public void Set(SwitchStatus _state)
	{
		if (state != SwitchStatus.LOCK)
		{
			state = _state;
			if (on != null)
			{
				on.SetActive(state == SwitchStatus.ON);
			}
			if (off != null)
			{
				off.SetActive(state == SwitchStatus.OFF);
			}
			if (fLock != null)
			{
				fLock.SetActive(value: false);
			}
		}
	}

	public SwitchStatus GetState()
	{
		return state;
	}
}
