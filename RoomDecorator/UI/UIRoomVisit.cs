using System.Collections;
using System.Collections.Generic;
using RoomDecorator.Data;
using RoomDecorator.Event;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIRoomVisit : MonoBehaviour
	{
		public GEAnimNGUI animBG;

		public GEAnimNGUI animBlock;

		public UIFlipSwitch tabSearch;

		public UIFlipSwitch tabGuild;

		public UIFlipSwitch tabFavor;

		public GameObject searchView;

		public UIListViewBase searchUserListView;

		public UIInput input;

		public GameObject guildView;

		public UIListViewBase guildUserListView;

		public GameObject favorView;

		public UILabel favorCount;

		public UIFlipSwitch editMode;

		public UIListViewBase favorUserListView;

		private bool _isOpen;

		private bool _isClose = true;

		private EVisitType _curTab;

		private UIListViewBase _curlistView;

		private Dictionary<EVisitType, List<Protocols.Dormitory.SearchUserInfo>> _data;

		private bool _favorEditMode;

		private void Awake()
		{
			_data = new Dictionary<EVisitType, List<Protocols.Dormitory.SearchUserInfo>>();
			_data.Add(EVisitType.Search, new List<Protocols.Dormitory.SearchUserInfo>());
			_data.Add(EVisitType.Guild, null);
			_data.Add(EVisitType.Favorites, null);
			input.defaultText = Localization.Get("81047");
		}

		private void OnEnable()
		{
			Message.AddListener<MessageEvent.Search.Data>("Search.Get.Users", OnSearchUsers);
			Message.AddListener<string>("Favor.Remove", OnRemoveFavorUser);
		}

		private void OnDisable()
		{
			Message.RemoveListener<MessageEvent.Search.Data>("Search.Get.Users", OnSearchUsers);
			Message.RemoveListener<string>("Favor.Remove", OnRemoveFavorUser);
		}

		public void Open()
		{
			if (!_isOpen)
			{
				_isOpen = true;
				_isClose = false;
				base.gameObject.SetActive(value: true);
				_data[EVisitType.Search] = new List<Protocols.Dormitory.SearchUserInfo>();
				_data[EVisitType.Guild] = null;
				_data[EVisitType.Favorites] = null;
				_curTab = EVisitType.Search;
				_favorEditMode = false;
				InvalidTab();
				InvalidData();
				animBG.Reset();
				animBlock.Reset();
				animBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
				animBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
				SoundManager.PlaySFX("SE_MenuOpen_001");
				input.value = string.Empty;
				RemoteObjectManager.instance.RequestGetRecommendUser();
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

		private void ChangeTab(EVisitType type)
		{
			if (_curTab != type)
			{
				_curTab = type;
				_favorEditMode = false;
				InvalidTab();
				InvalidData();
				SoundManager.PlaySFX("BTN_Tap_001");
			}
		}

		public void OnSearchUsers(MessageEvent.Search.Data data)
		{
			_data[data.type] = data.users;
			if (_curTab != data.type)
			{
				ChangeTab(data.type);
			}
			else
			{
				InvalidData();
			}
		}

		public void OnRemoveFavorUser(string uno)
		{
			int num = _data[EVisitType.Favorites].FindIndex((Protocols.Dormitory.SearchUserInfo x) => x.uno == uno);
			if (num != -1)
			{
				_data[EVisitType.Favorites].RemoveAt(num);
				InvalidData();
			}
		}

		private void InvalidTab()
		{
			switch (_curTab)
			{
			case EVisitType.Search:
				tabSearch.Set(SwitchStatus.ON);
				searchView.gameObject.SetActive(value: true);
				tabGuild.Set(SwitchStatus.OFF);
				guildView.gameObject.SetActive(value: false);
				tabFavor.Set(SwitchStatus.OFF);
				favorView.gameObject.SetActive(value: false);
				break;
			case EVisitType.Guild:
				tabSearch.Set(SwitchStatus.OFF);
				searchView.gameObject.SetActive(value: false);
				tabGuild.Set(SwitchStatus.ON);
				guildView.gameObject.SetActive(value: true);
				tabFavor.Set(SwitchStatus.OFF);
				favorView.gameObject.SetActive(value: false);
				break;
			case EVisitType.Favorites:
				tabSearch.Set(SwitchStatus.OFF);
				searchView.gameObject.SetActive(value: false);
				tabGuild.Set(SwitchStatus.OFF);
				guildView.gameObject.SetActive(value: false);
				tabFavor.Set(SwitchStatus.ON);
				favorView.gameObject.SetActive(value: true);
				break;
			}
		}

		private void InvalidData()
		{
			UIListViewBase uIListViewBase = null;
			switch (_curTab)
			{
			case EVisitType.Search:
				uIListViewBase = searchUserListView;
				break;
			case EVisitType.Guild:
				uIListViewBase = guildUserListView;
				break;
			case EVisitType.Favorites:
				uIListViewBase = favorUserListView;
				editMode.Set(_favorEditMode ? SwitchStatus.ON : SwitchStatus.OFF);
				favorCount.text = Localization.Format("81050", _data[_curTab].Count, SingletonMonoBehaviour<DormitoryData>.Instance.config.favorUserLimit);
				break;
			}
			List<Protocols.Dormitory.SearchUserInfo> list = _data[_curTab];
			uIListViewBase.ResizeItemList(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				UIRoomVisitUserListItem uIRoomVisitUserListItem = uIListViewBase.itemList[i] as UIRoomVisitUserListItem;
				uIRoomVisitUserListItem.gameObject.name = uIListViewBase.itemIdPrefix + list[i].uno;
				uIRoomVisitUserListItem.Set(list[i], _favorEditMode);
			}
		}

		public void OnMessage(GameObject arg0, UIRoomVisitUserListItem arg1)
		{
			string text = arg0.name;
			string id = ((!(arg1 == null)) ? arg1.name : string.Empty);
			switch (text)
			{
			case "Visit":
			{
				RoDormitory.User user = new RoDormitory.User();
				user.name = arg1.userName.text;
				user.level = int.Parse(arg1.level.text);
				user.uno = id;
				user.isMaster = false;
				DormitoryInitData.Instance.Set(user);
				RemoteObjectManager.instance.RequestGetDormitoryUserFloorDetailInfo(id, "1");
				break;
			}
			case "Delete":
				UISimplePopup.CreateBool(localization: true, "1303", "81054", null, "1304", "1305").onClick = delegate(GameObject sender)
				{
					if (sender.name == "OK")
					{
						RemoteObjectManager.instance.RequestRemoveDormitoryFavorUser(id);
					}
				};
				break;
			}
		}

		public void OnClick(GameObject sender)
		{
			switch (sender.name)
			{
			case "SearchTab":
				ChangeTab(EVisitType.Search);
				break;
			case "GuildTab":
				if (_data[EVisitType.Guild] == null)
				{
					RemoteObjectManager.instance.RequestGetDormitoryGuildUser();
				}
				else
				{
					ChangeTab(EVisitType.Guild);
				}
				break;
			case "FavoritesTab":
				if (_data[EVisitType.Favorites] == null)
				{
					RemoteObjectManager.instance.RequestGetDormitoryFavorUser();
				}
				else
				{
					ChangeTab(EVisitType.Favorites);
				}
				break;
			case "Search":
				if (!string.IsNullOrEmpty(input.value))
				{
					RemoteObjectManager.instance.RequestSearchDormitoryUser(input.value);
				}
				break;
			case "EditFavor":
				if (_favorEditMode)
				{
					_favorEditMode = false;
					InvalidData();
					SoundManager.PlaySFX("BTN_Positive_001");
				}
				break;
			case "ViewFavor":
				if (!_favorEditMode)
				{
					_favorEditMode = true;
					InvalidData();
					SoundManager.PlaySFX("BTN_Positive_001");
				}
				break;
			case "Close":
				Close();
				break;
			}
		}
	}
}
