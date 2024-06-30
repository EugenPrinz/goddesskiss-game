using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIConquestStageInfoPopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public UIDefaultListView blueListView;

	public UIDefaultListView redListView;

	public UILabel blueEmptyLabel;

	public UILabel redEmptyLabel;

	public GameObject blueEmptyRoot;

	public GameObject redEmptyRoot;

	public GameObject moveBtn;

	public GameObject refreshBtn;

	public GameObject leftArrow;

	public GameObject rightArrow;

	public UILabel blueTeamName;

	public UILabel redTeamName;

	public UILabel blueTeamWorld;

	public UILabel redTeamWorld;

	public UILabel refreshLabel;

	public UILabel rewardLabel;

	public UIGoods rewardItem;

	private int point;

	private int maxPoint;

	private int minPoint;

	private bool isRefresh;

	private int refreshDelay;

	public UIConquestDeckPopup deckPopup;

	private UIConquestAlieDeckPopup alieDeckPopup;

	public UIConquestMoveInfoPopup moveInfoPopup;

	private Protocols.ConquestStageInfo info;

	private int selectAlieUno;

	private int CONQUEST_STAGEINFO_REFRESH_DELAY;

	private bool bBackKeyEnable;

	public void Start()
	{
		SetAutoDestroy(autoDestory: true);
		OpenPopup();
	}

	public void Init(int point, Protocols.ConquestStageInfo info)
	{
		this.info = info;
		this.point = point;
		if (minPoint == 0)
		{
			UIConquestMap conquestMap = UIManager.instance.world.conquestMap;
			minPoint = 2;
			maxPoint = conquestMap.stageList.Count - 1;
		}
		GuildOccupyDataRow guildOccupyDataRow = base.regulation.guildOccupyDtbl[point.ToString()];
		CONQUEST_STAGEINFO_REFRESH_DELAY = int.Parse(base.regulation.defineDtbl["GUILD_OCCUPY_REFRESH_DELAY"].value);
		Protocols.RewardInfo.RewardData rewardData = new Protocols.RewardInfo.RewardData();
		rewardData.rewardType = (ERewardType)guildOccupyDataRow.rewardType;
		rewardData.rewardId = guildOccupyDataRow.rewardIdx;
		rewardData.rewardCnt = guildOccupyDataRow.rewardCount;
		rewardItem.Set(rewardData);
		UISetter.SetLabel(rewardLabel, string.Format("{0} {1}", Localization.Get(guildOccupyDataRow.s_idx), Localization.Get("4015")));
		blueListView.ResetPosition();
		redListView.ResetPosition();
		if (base.localUser.conquestTeam == EConquestTeam.Blue)
		{
			blueListView.Init(info.alieList, isAlie: true, "AlieUser-");
			redListView.Init(info.enemyList, isAlie: false, "EnemyUser-");
			UISetter.SetActive(blueEmptyRoot, info.alieList.Count == 0);
			UISetter.SetActive(redEmptyRoot, info.enemyList.Count == 0);
			UISetter.SetLabel(blueEmptyLabel, Localization.Get("110351"));
			if (existStandbyTroop(info.alieList))
			{
				UISetter.SetLabel(redEmptyLabel, Localization.Get("110353"));
			}
			else
			{
				UISetter.SetLabel(redEmptyLabel, Localization.Get("110352"));
			}
		}
		else
		{
			blueListView.Init(info.enemyList, isAlie: false, "EnemyUser-");
			redListView.Init(info.alieList, isAlie: true, "AlieUser-");
			UISetter.SetActive(blueEmptyRoot, info.enemyList.Count == 0);
			UISetter.SetActive(redEmptyRoot, info.alieList.Count == 0);
			UISetter.SetLabel(redEmptyLabel, Localization.Get("110351"));
			if (existStandbyTroop(info.alieList))
			{
				UISetter.SetLabel(blueEmptyLabel, Localization.Get("110353"));
			}
			else
			{
				UISetter.SetLabel(blueEmptyLabel, Localization.Get("110352"));
			}
		}
		UISetter.SetActive(rightArrow, point < maxPoint);
		UISetter.SetActive(leftArrow, point > minPoint);
		SetLabel(info.enemyInfo);
		StopCoroutine("RefreshDelay");
		StartCoroutine("RefreshDelay");
	}

	private bool existStandbyTroop(List<Protocols.ConquestStageInfo.User> list)
	{
		bool result = false;
		for (int i = 0; i < list.Count; i++)
		{
			Protocols.ConquestStageInfo.User user = list[i];
			if (user.standby > 0)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private void SetLabel(Protocols.ConquestStageInfo.EnemyInfo enemy)
	{
		UIConquestMap conquestMap = UIManager.instance.world.conquestMap;
		if (base.localUser.conquestTeam == EConquestTeam.Blue)
		{
			UISetter.SetLabel(blueTeamName, base.localUser.guildInfo.name);
			UISetter.SetLabel(redTeamName, conquestMap.eGuild.name);
			UISetter.SetLabel(blueTeamWorld, Localization.Format("19067", base.localUser.world));
			UISetter.SetLabel(redTeamWorld, Localization.Format("19067", conquestMap.eGuild.world));
		}
		else
		{
			UISetter.SetLabel(redTeamName, base.localUser.guildInfo.name);
			UISetter.SetLabel(blueTeamName, conquestMap.eGuild.name);
			UISetter.SetLabel(redTeamWorld, Localization.Format("19067", base.localUser.world));
			UISetter.SetLabel(blueTeamWorld, Localization.Format("19067", conquestMap.eGuild.world));
		}
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
			return;
		}
		if (text.StartsWith("EnemyUser-"))
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110329"));
			return;
		}
		if (text.StartsWith("AlieUser-"))
		{
			string text2 = text.Substring(text.IndexOf("-") + 1);
			if (text2 != base.localUser.uno)
			{
				OnAlieUserInfoPopupClicked(int.Parse(text2));
			}
			return;
		}
		switch (text)
		{
		case "LeftArrow":
			OnLeftBtnClicked();
			break;
		case "RightArrow":
			OnRightBtnClicked();
			break;
		case "RefreshBtn":
			OnRefreshBtnClicked();
			break;
		case "MoveBtn":
			if (deckPopup == null)
			{
				deckPopup = UIPopup.Create<UIConquestDeckPopup>("ConquestDeckPopup");
				deckPopup.Init(EConquestStageInfoType.Move);
			}
			break;
		}
	}

	private void OnAlieUserInfoPopupClicked(int uno)
	{
		if (alieDeckPopup == null)
		{
			selectAlieUno = uno;
			base.network.RequestGetConquestStageUserInfo(uno, point);
		}
	}

	public void CreateAlieUserDeckPopup(List<Protocols.ConquestStageUser> list)
	{
		if (alieDeckPopup == null)
		{
			Protocols.ConquestStageInfo.User user = info.alieList.Find((Protocols.ConquestStageInfo.User row) => int.Parse(row.uno) == selectAlieUno);
			alieDeckPopup = UIPopup.Create<UIConquestAlieDeckPopup>("ConquestAlieDeckPopup");
			alieDeckPopup.Init(list, user.name);
		}
	}

	private void OnRightBtnClicked()
	{
		point++;
		UIManager.instance.world.conquestMap.selectPoint = point;
		base.network.RequestGetConquestStageInfo(point);
	}

	private void OnLeftBtnClicked()
	{
		point--;
		UIManager.instance.world.conquestMap.selectPoint = point;
		base.network.RequestGetConquestStageInfo(point);
	}

	private void OnRefreshBtnClicked()
	{
		if (!isRefresh)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7055", refreshDelay));
		}
		else
		{
			base.network.RequestGetConquestStageInfo(point);
		}
	}

	public void OnOpenMoveInfoPopup(int point, int slot, int remain)
	{
		UISetter.SetActive(moveInfoPopup, active: true);
		moveInfoPopup.Init(point, slot, remain);
	}

	private IEnumerator RefreshDelay()
	{
		isRefresh = false;
		for (refreshDelay = CONQUEST_STAGEINFO_REFRESH_DELAY; refreshDelay > 0; refreshDelay--)
		{
			UISetter.SetLabel(refreshLabel, Utility.GetTimeStringColonFormat(refreshDelay));
			yield return new WaitForSeconds(1f);
		}
		isRefresh = true;
		UISetter.SetLabel(refreshLabel, Localization.Get("1013"));
		yield return true;
	}

	public override void OnRefresh()
	{
		if (deckPopup != null)
		{
			deckPopup.Init(EConquestStageInfoType.Move);
		}
	}

	public void OpenPopup()
	{
		base.Open();
		AnimBG.Reset();
		AnimBlock.Reset();
		OnAnimOpen();
	}

	public void ClosePopUp()
	{
		UISetter.SetActive(moveInfoPopup, active: false);
		bBackKeyEnable = true;
		StopCoroutine("RefreshDelay");
		HidePopup();
	}

	public override void Close()
	{
		if (deckPopup != null)
		{
			deckPopup.Close();
		}
		if (alieDeckPopup != null)
		{
			alieDeckPopup.Close();
		}
		base.Close();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		Close();
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
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
