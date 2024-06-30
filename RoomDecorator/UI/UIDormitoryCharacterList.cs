using System.Collections;
using System.Collections.Generic;
using RoomDecorator.Data;
using RoomDecorator.Event;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIDormitoryCharacterList : MonoBehaviour
	{
		public enum ETabType
		{
			All,
			Attacker,
			Defender,
			Supporter
		}

		public GEAnimNGUI animBG;

		public GEAnimNGUI animBlock;

		public UIFlipSwitch allTab;

		public UIFlipSwitch attackerTab;

		public UIFlipSwitch defenderTab;

		public UIFlipSwitch supporterTab;

		public UIFlipSwitch rankSortTab;

		public UIFlipSwitch levelSortTab;

		public UIFlipSwitch classSortTab;

		public UIListViewBase characterListView;

		private Dictionary<string, UIDormitoryCharacter> _items;

		private bool _isOpen;

		private bool _isClose = true;

		private ETabType _curTab;

		private ESortType _sortType;

		private ESortPowerType _sortPowerType;

		private List<RoCharacter> _characters;

		private DormitoryData _data;

		private RoDormitory _dormitory;

		private void Awake()
		{
			_data = SingletonMonoBehaviour<DormitoryData>.Instance;
			_dormitory = _data.dormitory;
			_items = new Dictionary<string, UIDormitoryCharacter>();
			_sortType = ESortType.Rank;
			_sortPowerType = ESortPowerType.Descending;
		}

		private void OnEnable()
		{
			Message.AddListener<string>("Chr.Update.Floor", InvalidCharacter);
		}

		private void OnDisable()
		{
			Message.RemoveListener<string>("Chr.Update.Floor", InvalidCharacter);
		}

		public void Open()
		{
			if (!_isOpen)
			{
				_isOpen = true;
				_isClose = false;
				base.gameObject.SetActive(value: true);
				characterListView.ResetPosition();
				InvalidTab();
				InvalidSortTab();
				InvalidCharacterList(EJob.All);
				animBG.Reset();
				animBlock.Reset();
				animBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
				animBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
				SoundManager.PlaySFX("SE_MenuOpen_001");
			}
		}

		public void Close()
		{
			if (!_isClose)
			{
				_isClose = true;
				animBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
				animBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
				StartCoroutine("WaitClose");
				SoundManager.PlaySFX("SE_MenuClose_001");
			}
		}

		private IEnumerator WaitClose()
		{
			yield return new WaitForSeconds(0.4f);
			base.gameObject.SetActive(value: false);
			_isOpen = false;
		}

		private void ChangeTab(ETabType type)
		{
			if (_curTab != type)
			{
				_curTab = type;
				characterListView.ResetPosition();
				InvalidTab();
				switch (_curTab)
				{
				case ETabType.All:
					InvalidCharacterList(EJob.All);
					break;
				case ETabType.Attacker:
					InvalidCharacterList(EJob.Attack);
					break;
				case ETabType.Defender:
					InvalidCharacterList(EJob.Defense);
					break;
				case ETabType.Supporter:
					InvalidCharacterList(EJob.Support);
					break;
				}
				SoundManager.PlaySFX("BTN_Tap_001");
			}
		}

		private void ChangeSortTab(ESortType type)
		{
			if (_sortType != type)
			{
				_sortType = type;
				characterListView.ResetPosition();
				InvalidSortTab();
				InvalidCharacterList();
				SoundManager.PlaySFX("BTN_Tap_001");
			}
		}

		private void ChangeSortOrder(ESortPowerType type)
		{
			if (_sortPowerType != type)
			{
				_sortPowerType = type;
				characterListView.ResetPosition();
				InvalidCharacterList();
			}
		}

		private void InvalidTab()
		{
			allTab.Set((_curTab == ETabType.All) ? SwitchStatus.ON : SwitchStatus.OFF);
			attackerTab.Set((_curTab == ETabType.Attacker) ? SwitchStatus.ON : SwitchStatus.OFF);
			defenderTab.Set((_curTab == ETabType.Defender) ? SwitchStatus.ON : SwitchStatus.OFF);
			supporterTab.Set((_curTab == ETabType.Supporter) ? SwitchStatus.ON : SwitchStatus.OFF);
		}

		private void InvalidSortTab()
		{
			rankSortTab.Set((_sortType == ESortType.Rank) ? SwitchStatus.ON : SwitchStatus.OFF);
			levelSortTab.Set((_sortType == ESortType.Level) ? SwitchStatus.ON : SwitchStatus.OFF);
			classSortTab.Set((_sortType == ESortType.Cls) ? SwitchStatus.ON : SwitchStatus.OFF);
		}

		private void InvalidCharacter(string cid)
		{
			if (_items.ContainsKey(cid))
			{
				UIDormitoryCharacter uIDormitoryCharacter = _items[cid];
				uIDormitoryCharacter.Set(_dormitory.characters[cid]);
			}
		}

		private void InvalidCharacterList()
		{
			int sortOrder = (int)_sortPowerType;
			_characters.Sort(delegate(RoCharacter arg1, RoCharacter arg2)
			{
				if (arg1.fno != arg2.fno)
				{
					if (arg1.fno == "0")
					{
						return 1;
					}
					if (arg2.fno == "0")
					{
						return -1;
					}
					return int.Parse(arg1.fno).CompareTo(int.Parse(arg2.fno));
				}
				if (arg1.commanderData.state != arg2.commanderData.state)
				{
					if (arg1.commanderData.state == ECommanderState.Nomal)
					{
						return -1;
					}
					if (arg2.commanderData.state == ECommanderState.Nomal)
					{
						return 1;
					}
				}
				int value = 0;
				int num = 0;
				switch (_sortType)
				{
				case ESortType.Rank:
					value = arg1.commanderData.rank;
					num = arg2.commanderData.rank;
					break;
				case ESortType.Cls:
					value = arg1.commanderData.cls;
					num = arg2.commanderData.cls;
					break;
				case ESortType.Level:
					value = arg1.commanderData.level;
					num = arg2.commanderData.level;
					break;
				}
				return num.CompareTo(value) * sortOrder;
			});
			_items.Clear();
			characterListView.ResizeItemList(_characters.Count);
			for (int i = 0; i < _characters.Count; i++)
			{
				UIDormitoryCharacter uIDormitoryCharacter = characterListView.itemList[i] as UIDormitoryCharacter;
				uIDormitoryCharacter.gameObject.name = characterListView.itemIdPrefix + _characters[i].id;
				uIDormitoryCharacter.Set(_characters[i]);
				_items.Add(_characters[i].id, uIDormitoryCharacter);
			}
		}

		private void InvalidCharacterList(EJob job)
		{
			_characters = new List<RoCharacter>();
			Dictionary<string, RoCharacter>.Enumerator enumerator = _dormitory.characters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				RoCharacter value = enumerator.Current.Value;
				if (job == EJob.All || job == value.commanderData.job)
				{
					_characters.Add(value);
				}
			}
			InvalidCharacterList();
		}

		public void OnMessage(GameObject arg0, UIDormitoryCharacter arg1)
		{
			string text = arg0.name;
			string cid = ((!(arg1 == null)) ? arg1.name : string.Empty);
			switch (text)
			{
			case "AddBtn":
				if (_data.room.characters.Count >= 5)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81030"));
				}
				else
				{
					RemoteObjectManager.instance.RequestArrangeDormitoryCommander(_data.room.fno, cid);
				}
				break;
			case "RemoveBtn":
				RemoteObjectManager.instance.RequestRemoveDormitoryCommander(cid);
				break;
			}
		}

		public void OnClick(GameObject sender)
		{
			switch (sender.name)
			{
			case "Close":
				Close();
				break;
			case "AllTab":
				ChangeTab(ETabType.All);
				break;
			case "AttackTab":
				ChangeTab(ETabType.Attacker);
				break;
			case "DefenseTab":
				ChangeTab(ETabType.Defender);
				break;
			case "SupportTab":
				ChangeTab(ETabType.Supporter);
				break;
			case "RankSortTab":
				ChangeSortTab(ESortType.Rank);
				break;
			case "LevelSortTab":
				ChangeSortTab(ESortType.Level);
				break;
			case "ClassSortTab":
				ChangeSortTab(ESortType.Cls);
				break;
			case "SortPowerBtn":
				ChangeSortOrder((_sortPowerType != ESortPowerType.Descending) ? ESortPowerType.Descending : ESortPowerType.Ascending);
				break;
			case "Lock":
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("82004"));
				break;
			}
		}
	}
}
