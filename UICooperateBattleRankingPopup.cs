using System.Collections.Generic;
using UnityEngine;

public class UICooperateBattleRankingPopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public UIFlipSwitch rankingTab;

	public UIFlipSwitch stage1Tab;

	public UIFlipSwitch stage2Tab;

	public UIFlipSwitch stage3Tab;

	public GameObject guildRankingRoot;

	public GameObject stageRankingRoot;

	public UIDefaultListView guildRankingListView;

	public UIDefaultListView userRankingListView;

	private List<Protocols.CooperateBattlePointGuildRankingInfo> _pointGuildRankData;

	private List<Protocols.CooperateBattlePointUserRankingInfo> _pointUserRankData;

	private bool _open;

	private void Start()
	{
		SetRecyclable(recyclable: false);
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

	public void Set(List<Protocols.CooperateBattlePointGuildRankingInfo> data)
	{
		_pointGuildRankData = data;
		OnTabClicked(0);
	}

	public void Set(int step, List<Protocols.CooperateBattlePointUserRankingInfo> data)
	{
		_pointUserRankData = data;
		OnTabClicked(step);
	}

	private void OnTabClicked(int idx)
	{
		switch (idx)
		{
		case 0:
			_SetTab(guildRankStage: true, stage1State: false, stage2State: false, stage3State: false);
			guildRankingListView.ResetPosition();
			guildRankingListView.InitCoopPointGuildRankList(_pointGuildRankData);
			break;
		case 1:
			_SetTab(guildRankStage: false, stage1State: true, stage2State: false, stage3State: false);
			userRankingListView.ResetPosition();
			userRankingListView.InitCoopPointUserRankList(_pointUserRankData);
			break;
		case 2:
			_SetTab(guildRankStage: false, stage1State: false, stage2State: true, stage3State: false);
			userRankingListView.ResetPosition();
			userRankingListView.InitCoopPointUserRankList(_pointUserRankData);
			break;
		case 3:
			_SetTab(guildRankStage: false, stage1State: false, stage2State: false, stage3State: true);
			userRankingListView.ResetPosition();
			userRankingListView.InitCoopPointUserRankList(_pointUserRankData);
			break;
		default:
			_SetTab(guildRankStage: true, stage1State: false, stage2State: false, stage3State: false);
			userRankingListView.ResetPosition();
			userRankingListView.InitCoopPointUserRankList(_pointUserRankData);
			break;
		}
	}

	private void _SetTab(bool guildRankStage, bool stage1State, bool stage2State, bool stage3State)
	{
		int num = 0;
		num += (guildRankStage ? 1 : 0);
		num += (stage1State ? 1 : 0);
		num += (stage2State ? 1 : 0);
		num += (stage3State ? 1 : 0);
		if (num > 1 || num == 0)
		{
			guildRankStage = true;
			stage1State = false;
			stage2State = false;
			stage3State = false;
		}
		UISetter.SetActive(guildRankingRoot, guildRankStage);
		UISetter.SetActive(stageRankingRoot, stage1State || stage2State || stage3State);
		UISetter.SetFlipSwitch(rankingTab, guildRankStage);
		UISetter.SetFlipSwitch(stage1Tab, stage1State);
		UISetter.SetFlipSwitch(stage2Tab, stage2State);
		UISetter.SetFlipSwitch(stage3Tab, stage3State);
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
			else if (text == "GuildRanking")
			{
				SoundManager.PlaySFX("BTN_Tap_001");
				base.network.RequestCooperateBattlePointGuildRank();
			}
			else if (text.StartsWith("Stage-"))
			{
				SoundManager.PlaySFX("BTN_Tap_001");
				int stage = int.Parse(text.Substring(text.IndexOf("-") + 1));
				base.network.RequestCooperateBattlePointRank(stage);
			}
		}
	}
}
