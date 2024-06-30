using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIConquestHistoryPopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public UIFlipSwitch rankingTab;

	public UIFlipSwitch currentRankingTab;

	public UIFlipSwitch prevConquestTab;

	public UIFlipSwitch prevJoinTab;

	public GameObject rankingRoot;

	public GameObject resultRoot;

	public GameObject stateRoot;

	public UIDefaultListView userListView;

	public UIDefaultListView rankingListView;

	public UIDefaultListView resultListView;

	public int selectPoint;

	private Dictionary<string, int> userList;

	private Protocols.ConquestInfo.PrevState.Point prevPointData;

	private List<int> pointList;

	private List<int> standbyList;

	private List<Protocols.GuildRankingInfo> highList;

	private List<Protocols.GuildRankingInfo> currentList;

	public UIConquestBattleResultPopup popup;

	private bool bBackKeyEnable;

	public void Start()
	{
		SetAutoDestroy(autoDestory: true);
		OpenPopup();
	}

	public void Init(int point = 0)
	{
		if (userList == null)
		{
			userList = new Dictionary<string, int>();
		}
		if (prevPointData == null)
		{
			prevPointData = new Protocols.ConquestInfo.PrevState.Point();
			prevPointData.win = new List<int>();
			prevPointData.lose = new List<int>();
		}
		if (base.uiWorld.guild.conquestInfo.prev != null)
		{
			userList = base.uiWorld.guild.conquestInfo.prev.userList;
			prevPointData = base.uiWorld.guild.conquestInfo.prev.pointData;
			standbyList = base.uiWorld.guild.conquestInfo.prev.standbyList;
		}
		if (pointList == null)
		{
			pointList = new List<int>();
			base.regulation.guildOccupyDtbl.ForEach(delegate(GuildOccupyDataRow row)
			{
				if (int.Parse(row.idx) != 1 && int.Parse(row.idx) != base.regulation.guildOccupyDtbl.length)
				{
					pointList.Add(int.Parse(row.idx));
				}
			});
		}
		if (point != 0)
		{
			OnPrevConquestTabClicked(point);
		}
	}

	public void SetRankingList(int type, List<Protocols.GuildRankingInfo> list)
	{
		switch (type)
		{
		case 1:
			highList = list;
			OnRankingTabClicked();
			break;
		case 2:
			currentList = list;
			OnCurrentRankingTabClicked();
			break;
		}
	}

	private void OnRankingTabClicked()
	{
		_SetTab(rankingState: true, currentRankingState: false, prevConquestState: false, prevJoinState: false);
		rankingListView.ResetPosition();
		rankingListView.InitGuildRankingList(highList);
	}

	private void OnCurrentRankingTabClicked()
	{
		_SetTab(rankingState: false, currentRankingState: true, prevConquestState: false, prevJoinState: false);
		rankingListView.scrollView.MoveRelative(-rankingListView.scrollView.transform.localPosition);
		rankingListView.InitGuildRankingList(currentList);
		int index = 0;
		if (currentList.Count <= 8)
		{
			return;
		}
		for (int i = 0; i < currentList.Count; i++)
		{
			if (currentList[i].idx == base.localUser.guildInfo.idx && i != 0)
			{
				index = i - 1;
			}
		}
		Vector3 localPosition = rankingListView.itemList[index].transform.localPosition;
		rankingListView.scrollView.MoveRelative(-localPosition);
	}

	private void OnPrevJoinTabClicked()
	{
		_SetTab(rankingState: false, currentRankingState: false, prevConquestState: false, prevJoinState: true);
		userListView.ResetPosition();
		userListView.Init(base.uiWorld.guild.memberList, userList);
	}

	private void OnPrevConquestTabClicked(int point = 0)
	{
		_SetTab(rankingState: false, currentRankingState: false, prevConquestState: true, prevJoinState: false);
		resultListView.ResetPosition();
		resultListView.InitConquestResultStageList(pointList, prevPointData, standbyList, "Battle-");
		if (point != 0)
		{
			selectPoint = point;
			base.network.RequestGetConquestBattle(point, 1);
		}
	}

	public override void OnClick(GameObject sender)
	{
		SoundManager.PlaySFX("EFM_SelectButton_001");
		string text = sender.name;
		UIGuild guild = UIManager.instance.world.guild;
		switch (text)
		{
		case "Close":
			if (!bBackKeyEnable)
			{
				ClosePopUp();
			}
			return;
		case "RankingTab":
			if (highList == null)
			{
				base.network.RequestGetGuildRanking(1);
			}
			else
			{
				OnRankingTabClicked();
			}
			return;
		case "CurrentRankingTab":
			if (currentList == null)
			{
				base.network.RequestGetGuildRanking(2);
			}
			else
			{
				OnCurrentRankingTabClicked();
			}
			return;
		case "PrevConquestTab":
			if (guild.conquestInfo.prev != null)
			{
				OnPrevConquestTabClicked();
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110312"));
			}
			return;
		case "PrevJoinTab":
			if (guild.conquestInfo.prev != null)
			{
				OnPrevJoinTabClicked();
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110312"));
			}
			return;
		}
		if (text.StartsWith("Battle-"))
		{
			int point = (selectPoint = int.Parse(text.Substring(text.IndexOf("-") + 1)));
			base.network.RequestGetConquestBattle(point);
		}
		else if (text.StartsWith("Empty-"))
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110368"));
		}
	}

	private void _SetTab(bool rankingState, bool currentRankingState, bool prevConquestState, bool prevJoinState)
	{
		int num = 0;
		num += (rankingState ? 1 : 0);
		num += (currentRankingState ? 1 : 0);
		num += (prevConquestState ? 1 : 0);
		num += (prevJoinState ? 1 : 0);
		if (num > 1 || num == 0)
		{
			rankingState = true;
			currentRankingState = false;
			prevConquestState = false;
			prevJoinState = false;
		}
		selectPoint = 0;
		UISetter.SetActive(rankingRoot, rankingState || currentRankingState);
		UISetter.SetActive(resultRoot, prevConquestState);
		UISetter.SetActive(stateRoot, prevJoinState);
		UISetter.SetFlipSwitch(rankingTab, rankingState);
		UISetter.SetFlipSwitch(currentRankingTab, currentRankingState);
		UISetter.SetFlipSwitch(prevConquestTab, prevConquestState);
		UISetter.SetFlipSwitch(prevJoinTab, prevJoinState);
	}

	public void CreateBattleResultPopup(Protocols.GetConquestBattle battle, int skip = 0)
	{
		popup = UIPopup.Create<UIConquestBattleResultPopup>("ConquestBattleResultPopup");
		popup.Init(battle, skip);
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopUp()
	{
		bBackKeyEnable = true;
		if (popup != null)
		{
			popup.Close();
		}
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
