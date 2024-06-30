using System;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIExplorationDetailPopup : UIPopup
{
	[Serializable]
	public class TimeType
	{
		public GameObject root;

		public UIFlipSwitch flip;

		public UILabel label1;

		public UILabel label2;
	}

	public GameObject readyView;

	public GameObject exploringView;

	public UITimer remainTimer;

	public GameObject completeView;

	public UIDefaultListView commanderTargetListView;

	public UIDefaultListView commanderSelectListView;

	public UIDefaultListView rewardListView;

	public GameObject timeTypeView;

	public List<TimeType> timeTypes;

	public GameObject selectedTimeView;

	public UILabel selectedTimeVal;

	public UILabel exp;

	public GameObject btnMovePrev;

	public GameObject btnMoveNext;

	protected EJob _selectJob;

	protected RoExploration _exploration;

	protected List<RoCommander> _commanders = new List<RoCommander>();

	public EExplorationState state => _exploration.state;

	public string mapID => _exploration.mapID;

	public int mapIdx => _exploration.mapIdx;

	protected int idx
	{
		get
		{
			return _exploration.idx;
		}
		set
		{
			_exploration.idx = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		SetRecyclable(recyclable: false);
		_selectJob = EJob.All;
	}

	private void Start()
	{
		Open();
	}

	public void Set(RoExploration exploration)
	{
		_exploration = exploration;
		List<RoCommander> list = new List<RoCommander>();
		for (int i = 0; i < _exploration.commanders.Count; i++)
		{
			if (state == EExplorationState.Idle)
			{
				if (!_exploration.commanders[i].isExploration && !_exploration.commanders[i].isDispatch)
				{
					list.Add(_exploration.commanders[i]);
				}
			}
			else
			{
				list.Add(_exploration.commanders[i]);
			}
		}
		_commanders = list;
		for (int j = 0; j < timeTypes.Count; j++)
		{
			if (j < _exploration.types.Count)
			{
				UISetter.SetLabel(timeTypes[j].label1, string.Format(Localization.Get("5769"), _exploration.types[j].searchTime));
				UISetter.SetLabel(timeTypes[j].label2, timeTypes[j].label1.text);
				UISetter.SetFlipSwitch(timeTypes[j].flip, j == idx);
			}
			else
			{
				UISetter.SetFlipSwitch(timeTypes[j].flip, state: false);
			}
		}
		UISetter.SetLabel(title, string.Format(Localization.Get("5080005"), Localization.Get(_exploration.mapName)));
		UISetter.SetLabel(exp, _exploration.Dr.searchExp);
		UISetter.SetLabel(selectedTimeVal, string.Format(Localization.Get("5769"), _exploration.Dr.searchTime));
		ResetMoveButtons();
		ResetRewardInformation();
		ResetTargetCommanders();
		ResetSelectCommanders();
		commanderSelectListView.ResetPosition();
		OnRefresh();
	}

	public void ResetRewardInformation()
	{
		List<RewardDataRow> list = base.regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == ERewardCategory.Exploration && row.type == _exploration.Dr.idx && row.typeIndex == 1);
		Dictionary<int, RewardDataRow> dictionary = new Dictionary<int, RewardDataRow>();
		List<RewardDataRow> list2 = new List<RewardDataRow>();
		for (int i = 0; i < list.Count; i++)
		{
			if (!dictionary.ContainsKey(list[i].rewardIdx))
			{
				dictionary.Add(list[i].rewardIdx, list[i]);
				list2.Add(list[i]);
			}
		}
		rewardListView.InitRewardList(list2);
	}

	public void ResetMoveButtons()
	{
		UISetter.SetActive(btnMovePrev, _exploration.mapIdx > 0);
		if (_exploration.mapIdx < base.localUser.explorationDtbl.length - 1)
		{
			UISetter.SetActive(btnMoveNext, base.localUser.explorationDtbl[_exploration.mapIdx + 1].isOpen);
		}
		else
		{
			UISetter.SetActive(btnMoveNext, active: false);
		}
	}

	public void ResetTargetCommanders()
	{
		commanderTargetListView.ResizeItemList(5);
		for (int i = 0; i < commanderTargetListView.itemList.Count; i++)
		{
			UICommanderSelectItem uICommanderSelectItem = commanderTargetListView.itemList[i] as UICommanderSelectItem;
			if (!(uICommanderSelectItem == null))
			{
				if (i < _commanders.Count)
				{
					uICommanderSelectItem.Set(_commanders[i]);
					UISetter.SetActive(uICommanderSelectItem.removeButton, !_commanders[i].isExploration);
					UISetter.SetGameObjectName(uICommanderSelectItem.removeButton, $"RemoveButton-{_commanders[i].id}");
					UISetter.SetActive(uICommanderSelectItem.validSlotRoot, active: true);
				}
				else
				{
					UISetter.SetActive(uICommanderSelectItem.validSlotRoot, active: false);
				}
				uICommanderSelectItem.SetSelection(selected: false);
			}
		}
	}

	protected List<RoCommander> GetCommanderList()
	{
		List<RoCommander> list = new List<RoCommander>();
		base.localUser.commanderList.ForEach(delegate(RoCommander commander)
		{
			if ((commander.branch == EBranch.Army || commander.branch == EBranch.Undefined) && (commander.job == _selectJob || _selectJob == EJob.All) && commander.state == ECommanderState.Nomal && (int)commander.cls >= _exploration.Dr.minClass)
			{
				list.Add(commander);
			}
		});
		return list;
	}

	public void ResetSelectCommanders()
	{
		List<RoCommander> commanderList = GetCommanderList();
		commanderList.Sort(delegate(RoCommander row, RoCommander row1)
		{
			if (row.currLevelUnitReg.GetTotalPower() > row1.currLevelUnitReg.GetTotalPower())
			{
				return -1;
			}
			return (row.currLevelUnitReg.GetTotalPower() < row1.currLevelUnitReg.GetTotalPower()) ? 1 : 0;
		});
		for (int i = 0; i < commanderList.Count; i++)
		{
			commanderList[i].charType = ECharacterType.Commander;
			commanderList[i].userIdx = int.Parse(base.localUser.uno);
		}
		commanderSelectListView.ResizeItemList(commanderList.Count);
		for (int j = 0; j < commanderList.Count; j++)
		{
			RoCommander roCommander = commanderList[j];
			UICommanderSelectItem uICommanderSelectItem = commanderSelectListView.itemList[j] as UICommanderSelectItem;
			if (!(uICommanderSelectItem == null))
			{
				UISetter.SetGameObjectName(uICommanderSelectItem.gameObject, "Commander_" + roCommander.id);
				uICommanderSelectItem.SetSelection(selected: false);
				uICommanderSelectItem.Set(roCommander);
				UISetter.SetGameObjectName(uICommanderSelectItem.addButton, $"AddButton-{roCommander.id}");
				UISetter.SetGameObjectName(uICommanderSelectItem.removeButton, $"RemoveButton-{roCommander.id}");
				UISetter.SetGameObjectName(uICommanderSelectItem.dieButton, $"DieButton-{roCommander.id}");
				UISetter.SetActive(uICommanderSelectItem.incompatible, active: false);
				UISetter.SetActive(uICommanderSelectItem.commanderHpRoot, active: false);
				UISetter.SetActive(uICommanderSelectItem.commanderSpRoot, active: false);
				UISetter.SetActive(uICommanderSelectItem.conquestRoot, active: false);
				if (roCommander.isExploration || roCommander.isDispatch)
				{
					UISetter.SetActive(uICommanderSelectItem.useRoot, active: true);
					UISetter.SetActive(uICommanderSelectItem.dieButton, active: true);
					UISetter.SetLabel(uICommanderSelectItem.dieLabel, (!roCommander.isExploration) ? Localization.Get("110075") : Localization.Get("5080003"));
				}
				else
				{
					UISetter.SetActive(uICommanderSelectItem.dieButton, active: false);
					UISetter.SetActive(uICommanderSelectItem.useRoot, state == EExplorationState.Idle && _commanders.Contains(roCommander));
				}
			}
		}
	}

	public new void OnRefresh()
	{
		switch (state)
		{
		case EExplorationState.Idle:
			UISetter.SetActive(readyView, active: true);
			UISetter.SetActive(exploringView, active: false);
			UISetter.SetActive(completeView, active: false);
			UISetter.SetActive(timeTypeView, active: true);
			UISetter.SetActive(selectedTimeView, active: false);
			remainTimer.Stop();
			break;
		case EExplorationState.Exploring:
		{
			UISetter.SetActive(readyView, active: false);
			UISetter.SetActive(exploringView, active: true);
			UISetter.SetActive(completeView, active: false);
			UISetter.SetActive(timeTypeView, active: false);
			UISetter.SetActive(selectedTimeView, active: true);
			TimeData timeData = TimeData.Create();
			timeData.SetByDuration(_exploration.data.remainTime);
			UISetter.SetTimer(remainTimer, timeData, "1048");
			remainTimer.RegisterOnFinished(delegate
			{
				OnRefresh();
			});
			break;
		}
		case EExplorationState.Complete:
			UISetter.SetActive(readyView, active: false);
			UISetter.SetActive(exploringView, active: false);
			UISetter.SetActive(completeView, active: true);
			UISetter.SetActive(timeTypeView, active: false);
			UISetter.SetActive(selectedTimeView, active: true);
			break;
		}
	}

	public void SetTimeType(GameObject sender)
	{
		if (state == EExplorationState.Idle)
		{
			idx = int.Parse(sender.name);
			UISetter.SetLabel(exp, _exploration.Dr.searchExp);
			UISetter.SetLabel(selectedTimeVal, string.Format(Localization.Get("5769"), _exploration.Dr.searchTime));
			SoundManager.PlaySFX("BTN_Norma_001");
		}
	}

	protected void MoveNext()
	{
		Set(base.localUser.explorationDtbl[mapIdx + 1]);
	}

	protected void MovePrev()
	{
		Set(base.localUser.explorationDtbl[mapIdx - 1]);
	}

	protected void AddCommander(string commanderId)
	{
		if (state != 0)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5080014"));
			return;
		}
		if (_commanders.Count >= 5)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5080015"));
			return;
		}
		RoCommander item = base.localUser.FindCommander(commanderId);
		_commanders.Add(item);
		ResetTargetCommanders();
		ResetSelectCommanders();
	}

	protected void RemoveCommander(string commanderId)
	{
		if (state != 0)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5080014"));
			return;
		}
		RoCommander item = base.localUser.FindCommander(commanderId);
		_commanders.Remove(item);
		ResetTargetCommanders();
		ResetSelectCommanders();
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		switch (text)
		{
		case "Exploration":
			if (_commanders.Count > 0)
			{
				UISimplePopup.CreateBool(localization: false, Localization.Get("5080008"), string.Format(Localization.Get("5080009"), _exploration.Dr.searchTime), null, Localization.Get("1001"), Localization.Get("1000")).onClick = delegate(GameObject obj)
				{
					string text2 = obj.name;
					if (text2 == "OK")
					{
						for (int i = 0; i < base.localUser.explorationDtbl.length; i++)
						{
							RoExploration roExploration = base.localUser.explorationDtbl[i];
							if (roExploration.mapIdx != _exploration.mapIdx && roExploration.state == EExplorationState.Idle)
							{
								base.localUser.explorationDtbl[i].RemoveCommanders(_commanders);
							}
						}
						_exploration.commanders = _commanders;
						List<string> list = new List<string>();
						for (int j = 0; j < _commanders.Count; j++)
						{
							list.Add(_commanders[j].id);
						}
						RemoteObjectManager.instance.RequestExplorationStart(_exploration.Dr.idx, list);
					}
				};
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5080016"));
			}
			return;
		case "Complete":
			RemoteObjectManager.instance.RequestExplorationComplete(_exploration.Dr.idx);
			return;
		case "Cancel":
			UISimplePopup.CreateBool(localization: true, "5080010", "5080011", null, "5080013", "5080012").onClick = delegate(GameObject obj)
			{
				string text3 = obj.name;
				if (text3 == "OK")
				{
					RemoteObjectManager.instance.RequestExplorationCancel(_exploration.Dr.idx);
				}
			};
			return;
		case "Close":
			Close();
			return;
		}
		if (text.StartsWith("AddButton-"))
		{
			string commanderId = text.Substring(text.IndexOf("-") + 1);
			AddCommander(commanderId);
			return;
		}
		if (text.StartsWith("RemoveButton-"))
		{
			string commanderId2 = text.Substring(text.IndexOf("-") + 1);
			RemoveCommander(commanderId2);
			return;
		}
		switch (text)
		{
		case "PrevMove":
			MovePrev();
			break;
		case "NextMove":
			MoveNext();
			break;
		case "AllTab":
			_selectJob = EJob.All;
			ResetSelectCommanders();
			commanderSelectListView.ResetPosition();
			break;
		case "AttackTab":
			_selectJob = EJob.Attack;
			ResetSelectCommanders();
			commanderSelectListView.ResetPosition();
			break;
		case "DefenseTab":
			_selectJob = EJob.Defense;
			ResetSelectCommanders();
			commanderSelectListView.ResetPosition();
			break;
		case "SupportTab":
			_selectJob = EJob.Support;
			ResetSelectCommanders();
			commanderSelectListView.ResetPosition();
			break;
		}
	}
}
