using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScrambleRanking : UIPopup
{
	public UIDefaultListView rankingListView;

	public UISprite guildIcon;

	public UILabel guildName;

	public UILabel guildPoint;

	public UILabel guildUserCnt;

	public UIFlipSwitch nowTab;

	public UIFlipSwitch prevTab;

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private List<Protocols.ScrambleRankingData> nRankingList;

	private List<Protocols.ScrambleRankingData> pRankingList;

	private int nTotalDamage;

	private int pTotalDamage;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
	}

	public void Init(Dictionary<string, List<Protocols.ScrambleRankingData>> list)
	{
		UISetter.SetLabel(guildName, base.localUser.guildInfo.name);
		nTotalDamage = 0;
		pTotalDamage = 0;
		nRankingList = list["now"];
		for (int i = 0; i < nRankingList.Count; i++)
		{
			nTotalDamage += nRankingList[i].score;
		}
		pRankingList = list["prev"];
		for (int j = 0; j < pRankingList.Count; j++)
		{
			pTotalDamage += pRankingList[j].score;
		}
		InitAndNowTab();
	}

	public override void OnClick(GameObject sender)
	{
		switch (sender.name)
		{
		case "Close":
			SoundManager.PlaySFX("BTN_Negative_001");
			ClosePopup();
			break;
		case "ArmyTab":
			InitAndNowTab();
			break;
		case "NavyTab":
			InitAndPrevTab();
			break;
		}
	}

	public void InitAndNowTab()
	{
		_SetPage(nowState: true, prevState: false);
		SetNowRankingList();
	}

	public void InitAndPrevTab()
	{
		_SetPage(nowState: false, prevState: true);
		SetPrevRankingList();
	}

	public void SetNowRankingList()
	{
		rankingListView.Init(nRankingList, nTotalDamage);
	}

	public void SetPrevRankingList()
	{
		rankingListView.Init(pRankingList, pTotalDamage);
	}

	private void _SetPage(bool nowState, bool prevState)
	{
		int num = 0;
		num += (nowState ? 1 : 0);
		num += (prevState ? 1 : 0);
		if (num > 1 || num == 0)
		{
			nowState = true;
			prevState = false;
		}
		UISetter.SetFlipSwitch(nowTab, nowState);
		UISetter.SetFlipSwitch(prevTab, prevState);
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
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
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
