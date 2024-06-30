using RoomDecorator.Data;
using RoomDecorator.Event;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIRoom : MonoBehaviour
	{
		public UISprite pointIcon;

		public UILabel pointValue;

		public UISprite cashIcon;

		public UILabel cashValue;

		public UILabel userName;

		public UILabel roomName;

		public GameObject masterView;

		public GameObject playView;

		public GameObject editView;

		public GameObject guestView;

		public UIFlipSwitch favorUserState;

		public UIRoomSideMenu sideMenu;

		public UIInventory inventory;

		public UIRoomVisit visit;

		public UIDormitoryCharacterList characterList;

		public UICostume costume;

		private DormitoryData _data;

		private void Start()
		{
			InvalidRoom();
		}

		private void OnEnable()
		{
			Message.AddListener("Shop.Open.CashShop", OpenCashShop);
			Message.AddListener("Room.Update.Mode", InvalidMode);
			Message.AddListener("Update.Goods", InvalidGoods);
			Message.AddListener("Room.Update.Name", InvalidRoomName);
			Message.AddListener("User.Update.FavorState", InvalidFavorState);
			Message.AddListener("Chr.Get", OnGetDormitoryCommanderInfo);
		}

		private void OnDisable()
		{
			Message.RemoveListener("Shop.Open.CashShop", OpenCashShop);
			Message.RemoveListener("Room.Update.Mode", InvalidMode);
			Message.RemoveListener("Update.Goods", InvalidGoods);
			Message.RemoveListener("Room.Update.Name", InvalidRoomName);
			Message.RemoveListener("User.Update.FavorState", InvalidFavorState);
			Message.RemoveListener("Chr.Get", OnGetDormitoryCommanderInfo);
		}

		private void OpenCashShop()
		{
			UIDiamondShop uIDiamondShop = UIPopup.Create<UIDiamondShop>("DiamondShop");
			uIDiamondShop.initTabIdx = 1;
			uIDiamondShop.InitAndOpenCashShop();
		}

		private void OnGetDormitoryCommanderInfo()
		{
			characterList.Open();
		}

		private void InvalidRoom()
		{
			_data = SingletonMonoBehaviour<DormitoryData>.Instance;
			userName.text = Localization.Format("81011", _data.user.name);
			masterView.SetActive(_data.user.isMaster);
			guestView.SetActive(!_data.user.isMaster);
			InvalidGoods();
			InvalidRoomName();
			InvalidMode();
			InvalidFavorState();
		}

		private void InvalidFavorState()
		{
			favorUserState.Set(_data.favorState ? SwitchStatus.ON : SwitchStatus.OFF);
		}

		private void InvalidGoods()
		{
			pointValue.text = _data.localUser.dormitoryPoint.ToString("N0");
			cashValue.text = _data.localUser.cash.ToString("N0");
		}

		private void InvalidRoomName()
		{
			roomName.text = _data.room.name;
			if (string.IsNullOrEmpty(_data.room.name))
			{
				roomName.text = Localization.Format("81002", _data.room.fno);
			}
		}

		private void InvalidMode()
		{
			playView.SetActive(!_data.isEditMode);
			editView.SetActive(_data.isEditMode);
		}

		public void OnClick(GameObject sender)
		{
			SoundManager.PlaySFX("BTN_Positive_001");
			switch (sender.name)
			{
			case "Manage":
				if (_data.user.isMaster)
				{
					RemoteObjectManager.instance.RequestGetDormitoryFloorInfo();
				}
				else
				{
					RemoteObjectManager.instance.RequestGetDormitoryUserFloorInfo(_data.user.uno);
				}
				break;
			case "Visit":
				visit.Open();
				break;
			case "Close":
				DormitoryInitData.Instance = null;
				Loading.Load(Loading.WorldMap);
				break;
			case "RoomName":
			{
				if (!_data.user.isMaster)
				{
					break;
				}
				UIReceiveUserString popup = UIPopup.Create<UIReceiveUserString>("InputUserString");
				popup.SetDefault(roomName.text);
				popup.SetLimitLength(10);
				popup.Set(localization: false, Localization.Get("81012"), (_data.config.changeRoomNameCache <= 0) ? Localization.Get("81013") : Localization.Format("81014", _data.config.changeRoomNameCache), null, Localization.Get("1006"), Localization.Get("1000"), null);
				popup.onClick = delegate(GameObject obj)
				{
					if (obj.name == "OK")
					{
						string text = popup.inputLabel.text;
						if (text == _data.room.name)
						{
							return;
						}
						if (text.Length < 2 || text.Length > 10 || string.IsNullOrEmpty(text))
						{
							NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81015"));
							return;
						}
						if (!Utility.PossibleChar(text))
						{
							NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7144"));
							return;
						}
						if (_data.localUser.cash < _data.config.changeRoomNameCache)
						{
							UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject t)
							{
								if (t.name == "OK")
								{
									RemoteObjectManager.instance.RequestGetCashShopList();
								}
							};
							return;
						}
						RemoteObjectManager.instance.RequestChangeDormitoryFloorName(_data.room.fno, popup.input.value);
					}
					popup.Close();
				};
				break;
			}
			case "ShowInventory":
				inventory.Open();
				break;
			case "CloseEditMode":
				_data.isEditMode = false;
				break;
			case "MyRoom":
			{
				RoDormitory.User user = new RoDormitory.User();
				user.name = _data.localUser.nickname;
				user.level = _data.localUser.level;
				user.uno = _data.localUser.uno;
				user.isMaster = true;
				DormitoryInitData.Instance.Set(user);
				RemoteObjectManager.instance.RequestGetDormitoryFloorDetailInfo("1");
				break;
			}
			case "AddFavorUser":
				RemoteObjectManager.instance.RequestAddDormitoryFavorUser(_data.user.uno);
				break;
			case "RemoveFavorUser":
				UISimplePopup.CreateBool(localization: true, "1303", "81054", null, "1304", "1305").onClick = delegate(GameObject obj)
				{
					if (obj.name == "OK")
					{
						RemoteObjectManager.instance.RequestRemoveDormitoryFavorUser(_data.user.uno);
					}
				};
				break;
			case "Costume":
				if (_data.room.characters.Count <= 0)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81070"));
				}
				else
				{
					costume.Open();
				}
				break;
			case "CashShop":
				RemoteObjectManager.instance.RequestGetCashShopList();
				break;
			case "InfoBtn":
				UISimplePopup.CreateOK("InformationPopup").Set(localization: true, "82002", "81102", string.Empty, "1001", string.Empty, string.Empty);
				break;
			}
		}
	}
}
