using System.Collections;
using UnityEngine;

public class UISelectChallengePopup : UIPopup
{
	public SelectChallengeListItem duel;

	public SelectChallengeListItem waveDuel;

	public SelectChallengeListItem realTimeDuel;

	[HideInInspector]
	public string listItemPrefix = "ChallengeItem";

	private bool _open;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
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
			StartCoroutine(OnEventHidePopup());
		}
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return null;
		Close();
	}

	public override void OnClick(GameObject sender)
	{
		if (!_open)
		{
			return;
		}
		string text = sender.name;
		if (text == "Close")
		{
			ClosePopup();
		}
		else if (text.StartsWith("ChallengeItem-"))
		{
			switch (text.Substring("ChallengeItem-".Length))
			{
			case "1":
				UIManager.instance.EnableCameraTouchEvent(isEnable: false);
				base.network.DuelRankingList();
				break;
			case "2":
				UIManager.instance.EnableCameraTouchEvent(isEnable: false);
				base.network.WaveDuelRankingList();
				break;
			}
			ClosePopup();
		}
	}
}
