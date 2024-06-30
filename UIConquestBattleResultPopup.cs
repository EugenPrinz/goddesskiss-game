using System.Collections;
using UnityEngine;

public class UIConquestBattleResultPopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	public UILabel blueTeamName;

	public UILabel redTeamName;

	public UILabel blueTeamServer;

	public UILabel redTeamServer;

	public UIDefaultListView resultListView;

	public GameObject stateRoot;

	public UISprite blueState;

	public UISprite redState;

	private bool skip;

	private bool isPlaying;

	public void Init(Protocols.GetConquestBattle battle, int skipState = 0)
	{
		if (battle.eSide == "R")
		{
			UISetter.SetLabel(blueTeamName, base.localUser.guildInfo.name);
			UISetter.SetLabel(redTeamName, battle.enemyName);
			UISetter.SetLabel(blueTeamServer, Localization.Format("19067", base.localUser.world));
			UISetter.SetLabel(redTeamServer, Localization.Format("19067", battle.enemyWorld));
		}
		else
		{
			UISetter.SetLabel(redTeamName, base.localUser.guildInfo.name);
			UISetter.SetLabel(blueTeamName, battle.enemyName);
			UISetter.SetLabel(redTeamServer, Localization.Format("19067", base.localUser.world));
			UISetter.SetLabel(blueTeamServer, Localization.Format("19067", battle.enemyWorld));
		}
		resultListView.Init(battle);
		if (skipState == 1)
		{
			StartCoroutine("_Skip");
		}
		else
		{
			StartCoroutine("_PlayBattleAnimation");
		}
	}

	public void Start()
	{
		SetAutoDestroy(autoDestory: true);
		OpenPopup();
	}

	public IEnumerator _PlayBattleAnimation()
	{
		isPlaying = true;
		if (resultListView.itemList != null)
		{
			for (int idx = 0; idx < resultListView.itemList.Count; idx++)
			{
				UIConquestBattleResultUserListItem item = resultListView.itemList[idx] as UIConquestBattleResultUserListItem;
				if (!item.isActiveAndEnabled)
				{
					UISetter.SetActive(stateRoot, active: true);
					stateRoot.transform.localPosition = new Vector3(0f, resultListView.itemList[idx - 1].transform.localPosition.y - 173f, 0f);
					break;
				}
				resultListView.scrollView.MoveRelative(-resultListView.scrollView.transform.localPosition);
				if (idx >= 2)
				{
					Vector3 localPosition = item.transform.localPosition;
					localPosition.y += item.bg.height / 2 + item.bg.height * 2 - 188;
					resultListView.scrollView.MoveRelative(-localPosition);
				}
				yield return StartCoroutine(item.BattleAnimation());
				yield return new WaitForSeconds(0.5f);
			}
		}
		yield return new WaitForSeconds(0.5f);
		blueState.GetComponent<TweenScale>().PlayForward();
		redState.GetComponent<TweenScale>().PlayForward();
	}

	public IEnumerator _Skip()
	{
		yield return null;
		Skip();
	}

	public void Skip()
	{
		skip = true;
		isPlaying = false;
		StopCoroutine("_PlayBattleAnimation");
		for (int i = 0; i < resultListView.itemList.Count; i++)
		{
			UIConquestBattleResultUserListItem uIConquestBattleResultUserListItem = resultListView.itemList[i] as UIConquestBattleResultUserListItem;
			if (!uIConquestBattleResultUserListItem.isActiveAndEnabled)
			{
				UISetter.SetActive(stateRoot, active: true);
				stateRoot.transform.localPosition = new Vector3(0f, resultListView.itemList[i - 1].transform.localPosition.y - 173f, 0f);
				break;
			}
			uIConquestBattleResultUserListItem.Skip();
			resultListView.scrollView.MoveRelative(-resultListView.scrollView.transform.localPosition);
			if (i >= 2)
			{
				Vector3 localPosition = uIConquestBattleResultUserListItem.transform.localPosition;
				localPosition.y += uIConquestBattleResultUserListItem.bg.height / 2 + uIConquestBattleResultUserListItem.bg.height * 2 - 188;
				resultListView.scrollView.MoveRelative(-localPosition);
			}
		}
		blueState.GetComponent<TweenScale>().PlayForward();
		redState.GetComponent<TweenScale>().PlayForward();
	}

	public override void OnClick(GameObject sender)
	{
		SoundManager.PlaySFX("EFM_SelectButton_001");
		string text = sender.name;
		if (text == "Close")
		{
			if (!bBackKeyEnable)
			{
				ClosePopUp();
			}
		}
		else if (text == "Skip")
		{
			if (!skip)
			{
				Skip();
			}
		}
		else if (text.StartsWith("Replay-"))
		{
			if (isPlaying)
			{
				Skip();
			}
			string text2 = text.Substring(text.IndexOf("-") + 1);
			base.localUser.lastConquestReplayPoint = base.uiWorld.guild.historyPopup.selectPoint;
			if (!string.IsNullOrEmpty(text2))
			{
				base.network.RequestGetConquestReplay(text2);
			}
		}
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopUp()
	{
		bBackKeyEnable = true;
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0f);
		base.Close();
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
	}

	private void OnAnimClose()
	{
	}
}
