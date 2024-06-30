using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UISeaRobberSweep : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIUser user;

	public UILabel btnLabel;

	public UILabel count;

	public UISprite btnIcon;

	public UIDefaultListView listView;

	public GameObject btn;

	private int type;

	public int selectedStageId { get; private set; }

	public void InitAndOpenSeaRobber(int _type)
	{
		if (!bEnterKeyEnable)
		{
			AnimBG.Reset();
			AnimBlock.Reset();
			bEnterKeyEnable = true;
			type = _type;
			selectedStageId = 0;
			List<SweepDataRow> list = base.regulation.FindSweepRow(_type);
			listView.Init(list, "Stage-");
			UISetter.SetLabel(title, Localization.Get("16011"));
			Set();
			OpenPopup();
		}
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "Close")
		{
			ClosePopup();
		}
		else if (text == "BuyTicket")
		{
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.SweepTicket);
		}
		else
		{
			if (!listView.Contains(text))
			{
				return;
			}
			_Select(text);
			SweepListItem sweepListItem = listView.lastSelectedItem as SweepListItem;
			if (sweepListItem.lockType != 0)
			{
				if (sweepListItem.lockType == ESweepLockType.MinLevel)
				{
					NetworkAnimation.Instance.CreateFloatingText(string.Format(Localization.Get("16049"), sweepListItem.row.minLevel));
				}
				else
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5802"));
				}
				_UnSelect(text);
			}
			else
			{
				StartReadyBattle();
				_UnSelect(text);
			}
		}
	}

	private void _Select(string id)
	{
		listView.SetSelection(id, selected: true);
		selectedStageId = int.Parse(listView.lastSelectedItemPureId);
	}

	private void _UnSelect(string id)
	{
		listView.SetSelection(id, selected: false);
		selectedStageId = 0;
	}

	public void StartReadyBattle()
	{
		if (selectedStageId != 0)
		{
			SweepListItem sweepListItem = listView.lastSelectedItem as SweepListItem;
			BattleData battleData = BattleData.Create(EBattleType.Guerrilla);
			battleData.sweepLevel = selectedStageId;
			battleData.sweepType = type;
			battleData.defender = RoUser.CreateNPC("Enemy-" + sweepListItem.row.uid, "NPC", RoTroop.CreateEnemy(sweepListItem.row.uid));
			UIManager.instance.world.readyBattle.InitAndOpenReadyBattle(battleData);
		}
	}

	public void Set()
	{
		if (user != null)
		{
			user.Set(base.localUser);
		}
		SetButton();
	}

	public void SetButton()
	{
		if (base.localUser.sweepTicket > 0)
		{
			btn.name = "StartSweep";
			UISetter.SetSprite(btnIcon, "com_attack_bar");
			UISetter.SetLabel(count, -1);
			UISetter.SetLabel(btnLabel, Localization.Get("5759"));
		}
		else
		{
			VipRechargeDataRow vipRechargeDataRow = base.regulation.vipRechargeDtbl[103.ToString()];
			UISetter.SetLabel(count, vipRechargeDataRow.startRecharge);
			btn.name = "BuyTicket";
			UISetter.SetLabel(btnLabel, Localization.Get("16014"));
			UISetter.SetSprite(btnIcon, "com_cash_bar");
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		Set();
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		if (!bBackKeyEnable)
		{
			bBackKeyEnable = true;
			HidePopup();
		}
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
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
