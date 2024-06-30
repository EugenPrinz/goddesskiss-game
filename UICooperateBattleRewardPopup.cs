using UnityEngine;

public class UICooperateBattleRewardPopup : UIPopup
{
	private bool _open;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		OpenPopup();
	}

	public void OpenPopup()
	{
		if (!_open)
		{
			_open = true;
			Open();
		}
	}

	public void ClosePopup()
	{
		if (_open)
		{
			_open = false;
			Close();
		}
	}

	public override void OnClick(GameObject sender)
	{
		if (_open)
		{
			base.OnClick(sender);
			string text = sender.name;
			if (text == "Close")
			{
				ClosePopup();
			}
		}
	}
}
