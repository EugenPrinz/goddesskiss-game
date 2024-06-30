using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIHeadQuarters : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimInfo;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UITexture goBG;

	public UIFlipSwitch allTab;

	public UIFlipSwitch attackersTab;

	public UIFlipSwitch defendersTab;

	public UIFlipSwitch supportersTab;

	public UIScrollView armyScrollView;

	public UIDefaultListView armyHaveListView;

	public UIDefaultListView armyDontHaveListView;

	public GameObject armyDontHaveLine;

	private const int dontHaveListY = 104;

	private const int dontHaveLineY = 259;

	private ESortType sortType;

	private ESortPowerType sortPowerType;

	public UILabel sortName;

	public UISprite sortPower;

	public UILabel ringCount;

	private EJob currentJob;

	public UISpineAnimation spineAnimation;

	private List<RoCommander> haveList;

	private List<RoCommander> dontHaveList;

	public string selectedCommanderId { get; private set; }

	private void Start()
	{
		UISetter.SetSpine(spineAnimation, "n_001");
	}

	private void OnDestroy()
	{
		if (AnimBlock != null)
		{
			AnimBlock = null;
		}
		if (goBG != null)
		{
			goBG = null;
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		switch (text)
		{
		case "Close":
			if (!bBackKeyEnable)
			{
				ClosePopUp();
			}
			return;
		case "AllTab":
			InitAndAllTab();
			return;
		case "AttackersTab":
			InitAndAttackersTap();
			return;
		case "DefendersTab":
			InitAndDefendersTap();
			return;
		case "SupportersTab":
			InitAndSupportersTap();
			return;
		}
		if (armyHaveListView.Contains(text))
		{
			string pureId = armyHaveListView.GetPureId(text);
			RoCommander commander = base.localUser.FindCommander(pureId);
			if (commander == null)
			{
				return;
			}
			if (commander.state == ECommanderState.Nomal)
			{
				UIManager.instance.world.commanderDetail.InitOpenPopup(pureId, CommanderDetailType.Training, haveList);
				return;
			}
			CommanderRankDataRow rankRow = base.regulation.FindCommanderRankData(commander.rank);
			if (rankRow == null)
			{
				return;
			}
			UIMultiplePopup uIMultiplePopup = UIPopup.Create<UIMultiplePopup>("MultiplePopup");
			uIMultiplePopup.SetData(2, Localization.Get("1347"), string.Format(Localization.Get("8020"), commander.nickname), null, "Close", null, "CommanderRecruit", Localization.Get("1000"), null, Localization.Get("5034"), _otherState: false, _paymentState: true, EPaymentType.Gold, rankRow.gold);
			uIMultiplePopup.onClick = delegate(GameObject popupSender)
			{
				string text2 = popupSender.name;
				if (text2 == "CommanderRecruit")
				{
					if (rankRow.gold > base.localUser.gold)
					{
						UISimplePopup.CreateBool(localization: true, "1029", "1030", null, "1001", "1000").onClick = delegate(GameObject go)
						{
							string text3 = go.name;
							if (text3 == "OK")
							{
								base.uiWorld.camp.GoNavigation("MetroBank");
							}
						};
					}
					else
					{
						RemoteObjectManager.instance.RequestCommanderRankUp(commander.id);
					}
				}
			};
			return;
		}
		if (armyDontHaveListView.Contains(text))
		{
			string pureId2 = armyDontHaveListView.GetPureId(text);
			UIManager.instance.world.commanderDetail.InitOpenPopup(pureId2, CommanderDetailType.Recruit, dontHaveList);
			return;
		}
		switch (text)
		{
		case "PreDeck":
			UIManager.instance.world.preDeckSetting.InitOpenPopup();
			break;
		case "SortBtn":
			if (sortType == ESortType.Rank)
			{
				sortType = ESortType.Level;
			}
			else if (sortType == ESortType.Level)
			{
				sortType = ESortType.Cls;
			}
			else if (sortType == ESortType.Cls)
			{
				sortType = ESortType.Rank;
			}
			PlayerPrefs.SetInt("CommanderSortType", (int)sortType);
			SetCommanderList(currentJob);
			break;
		case "SortPowerBtn":
			if (sortPowerType == ESortPowerType.Descending)
			{
				sortPowerType = ESortPowerType.Ascending;
			}
			else if (sortPowerType == ESortPowerType.Ascending)
			{
				sortPowerType = ESortPowerType.Descending;
			}
			PlayerPrefs.SetInt("CommanderSortPowerType", (int)sortPowerType);
			SetCommanderList(currentJob);
			break;
		}
	}

	public void InitAndOpenHeadQuarters()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			InitAndAllTab();
			OpenPopupShow();
		}
	}

	public void OpenHeadQuarters()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			InitAndAllTab();
			Open();
		}
	}

	public void CommanderRefresh(bool _isCash)
	{
	}

	public void InitAndAllTab()
	{
		_SetPage(allState: true, attackerState: false, defendersState: false, supporterState: false);
		SetCommanderList(EJob.All);
	}

	public void InitAndAttackersTap()
	{
		_SetPage(allState: false, attackerState: true, defendersState: false, supporterState: false);
		SetCommanderList(EJob.Attack);
	}

	public void InitAndDefendersTap()
	{
		_SetPage(allState: false, attackerState: false, defendersState: true, supporterState: false);
		SetCommanderList(EJob.Defense);
	}

	public void InitAndSupportersTap()
	{
		_SetPage(allState: false, attackerState: false, defendersState: false, supporterState: true);
		SetCommanderList(EJob.Support);
	}

	private void _SetPage(bool allState, bool attackerState, bool defendersState, bool supporterState)
	{
		int num = 0;
		num += (allState ? 1 : 0);
		num += (attackerState ? 1 : 0);
		num += (defendersState ? 1 : 0);
		num += (supporterState ? 1 : 0);
		if (num > 1 || num == 0)
		{
			allState = true;
			attackerState = false;
			defendersState = false;
			supporterState = false;
		}
		UISetter.SetFlipSwitch(allTab, allState);
		UISetter.SetFlipSwitch(attackersTab, attackerState);
		UISetter.SetFlipSwitch(defendersTab, defendersState);
		UISetter.SetFlipSwitch(supportersTab, supporterState);
	}

	private void SetCommanderList(EJob job)
	{
		armyScrollView.transform.localPosition = new Vector3(0f, 0f, 0f);
		haveList = base.localUser.GetCommanderList(job, have: true, recruit: true);
		dontHaveList = base.localUser.GetCommanderList(job, have: false, recruit: true);
		currentJob = job;
		int num = (int)Mathf.Ceil((float)haveList.Count / 4f);
		int num2 = (int)armyHaveListView.grid.cellHeight * num;
		armyDontHaveListView.transform.localPosition = new Vector3(armyDontHaveListView.transform.localPosition.x, 104 - num2, armyDontHaveListView.transform.localPosition.z);
		armyDontHaveLine.transform.localPosition = new Vector3(armyDontHaveLine.transform.localPosition.x, 259 - num2, armyDontHaveLine.transform.localPosition.z);
		sortType = (ESortType)PlayerPrefs.GetInt("CommanderSortType", 3);
		if (sortType == ESortType.Level)
		{
			UISetter.SetLabel(sortName, Localization.Get("1034"));
		}
		else if (sortType == ESortType.Cls)
		{
			UISetter.SetLabel(sortName, Localization.Get("1035"));
		}
		else if (sortType == ESortType.Rank)
		{
			UISetter.SetLabel(sortName, Localization.Get("1050"));
		}
		sortPowerType = (ESortPowerType)PlayerPrefs.GetInt("CommanderSortPowerType", 1);
		int sortPowerFlag = (int)sortPowerType;
		if (sortPowerType == ESortPowerType.Descending)
		{
			sortPower.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
		else if (sortPowerType == ESortPowerType.Ascending)
		{
			sortPower.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
		}
		UISetter.SetLabel(ringCount, base.localUser.ring);
		haveList.Sort(delegate(RoCommander row, RoCommander row1)
		{
			int num3;
			int num4;
			if (sortType == ESortType.Level)
			{
				num3 = row.level;
				num4 = row1.level;
			}
			else if (sortType == ESortType.Cls)
			{
				num3 = row.cls;
				num4 = row1.cls;
			}
			else if (sortType == ESortType.Rank)
			{
				num3 = row.rank;
				num4 = row1.rank;
			}
			else
			{
				num4 = 0;
				num3 = 0;
			}
			if (row.state != ECommanderState.Nomal && row1.state == ECommanderState.Nomal)
			{
				return -1;
			}
			if (row.state == ECommanderState.Nomal && row1.state != ECommanderState.Nomal)
			{
				return 1;
			}
			if (num4 < num3)
			{
				return -1 * sortPowerFlag;
			}
			if (num4 > num3)
			{
				return sortPowerFlag;
			}
			if (int.Parse(row.id) < int.Parse(row1.id))
			{
				return -1;
			}
			return (int.Parse(row.id) > int.Parse(row1.id)) ? 1 : 0;
		});
		armyHaveListView.Init(haveList, "Commander_");
		armyDontHaveListView.Init(dontHaveList, "Commander_");
		armyHaveListView.ResetPosition();
		armyDontHaveListView.ResetPosition();
	}

	public void SetNavigationOpen(string id)
	{
		ItemExchangeDataRow itemExchangeDataRow = base.regulation.itemExchangeDtbl.Find((ItemExchangeDataRow row) => row.type == EStorageType.Medal && row.typeidx == id);
		if (itemExchangeDataRow != null)
		{
			UINavigationPopUp uINavigationPopUp = UIPopup.Create<UINavigationPopUp>("NavigationPopUp");
			uINavigationPopUp.Init(EStorageType.Medal, id, itemExchangeDataRow);
			uINavigationPopUp.title.text = Localization.Get("5608");
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		if (allTab.GetState() == SwitchStatus.ON)
		{
			InitAndAllTab();
		}
		if (attackersTab.GetState() == SwitchStatus.ON)
		{
			InitAndAttackersTap();
		}
		if (defendersTab.GetState() == SwitchStatus.ON)
		{
			InitAndDefendersTap();
		}
		if (supportersTab.GetState() == SwitchStatus.ON)
		{
			InitAndSupportersTap();
		}
	}

	private void OpenPopupShow()
	{
		OnAnimOpen();
		base.Open();
	}

	public void ClosePopUp()
	{
		bBackKeyEnable = true;
		HidePopup();
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
		AnimBG.Reset();
		AnimNpc.Reset();
		AnimInfo.Reset();
		AnimTitle.Reset();
		AnimBlock.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimInfo.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimInfo.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public void AnimOpenFinish()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
	}

	protected override void OnEnablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: true);
	}

	protected override void OnDisablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: false);
	}
}
