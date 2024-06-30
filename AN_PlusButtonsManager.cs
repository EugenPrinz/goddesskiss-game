using System;
using System.Collections.Generic;
using UnityEngine;

public class AN_PlusButtonsManager : SA_Singleton<AN_PlusButtonsManager>
{
	public List<AN_PlusButton> buttons = new List<AN_PlusButton>();

	private void Awake()
	{
		buttons = new List<AN_PlusButton>();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void RegisterButton(AN_PlusButton b)
	{
		buttons.Add(b);
	}

	private void OnApplicationPause(bool IsPaused)
	{
		if (IsPaused)
		{
			return;
		}
		foreach (AN_PlusButton button in buttons)
		{
			if (button != null && button.IsShowed)
			{
				button.Refresh();
			}
		}
	}

	private void OnPlusClicked(string data)
	{
		int obj = Convert.ToInt32(data);
		foreach (AN_PlusButton button in buttons)
		{
			if (button != null && button.ButtonId.Equals(obj))
			{
				button.FireClickAction();
			}
		}
	}
}
