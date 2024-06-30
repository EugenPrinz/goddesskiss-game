using System.Collections;
using System.Collections.Generic;
using RoomDecorator.Data;
using RoomDecorator.Event;
using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIRoomList : UIPopup
	{
		public GEAnimNGUI animBG;

		public GEAnimNGUI animBlock;

		public UIFlipSwitch receivePoint;

		public UILabel ston;

		public UILabel elec;

		public UILabel wood;

		public UIListViewBase roomListView;

		private RoDormitory.Config _config;

		private Protocols.Dormitory.FloorInfo _data;

		protected override void Awake()
		{
			base.Awake();
			SetRecyclable(recyclable: false);
		}

		private void Start()
		{
			Open();
		}

		protected override void OnEnable()
		{
			Message.AddListener("Update.Goods", InvalidResource);
			Message.AddListener("Room.Update.List", InvalidRoomList);
			Message.AddListener<Protocols.Dormitory.ConstructFloorResponse>("Room.Update.Build", UpdateFloor);
			Message.AddListener<Protocols.Dormitory.FinishConstructFloorResponse>("Room.Update.Build", UpdateFloor);
			Message.AddListener<bool>("Room.Update.PointState", UpdatePointState);
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			Message.RemoveListener("Update.Goods", InvalidResource);
			Message.RemoveListener("Room.Update.List", InvalidRoomList);
			Message.RemoveListener<Protocols.Dormitory.ConstructFloorResponse>("Room.Update.Build", UpdateFloor);
			Message.RemoveListener<Protocols.Dormitory.FinishConstructFloorResponse>("Room.Update.Build", UpdateFloor);
			Message.RemoveListener<bool>("Room.Update.PointState", UpdatePointState);
			base.OnDisable();
		}

		public override void Open()
		{
			base.Open();
			animBG.Reset();
			animBlock.Reset();
			animBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
			animBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
		}

		public override void Close()
		{
			animBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
			animBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
			StartCoroutine("WaitClose");
		}

		private IEnumerator WaitClose()
		{
			yield return new WaitForSeconds(0.4f);
			base.Close();
		}

		private void UpdatePointState(bool state)
		{
			if (_data != null)
			{
				_data.pointState = state;
				InvalidPointState();
			}
		}

		public void Set(Protocols.Dormitory.FloorInfo data)
		{
			_data = data;
			_config = base.localUser.dormitory.config;
			InvalidPointState();
			InvalidRoomList();
			InvalidResource();
		}

		public void UpdateFloor(Protocols.Dormitory.ConstructFloorResponse data)
		{
			Dictionary<string, Protocols.Dormitory.RoomInfo>.Enumerator enumerator = data.floors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				_data.floors[enumerator.Current.Key] = enumerator.Current.Value;
			}
			InvalidRoomList();
		}

		public void UpdateFloor(Protocols.Dormitory.FinishConstructFloorResponse data)
		{
			Dictionary<string, Protocols.Dormitory.RoomInfo>.Enumerator enumerator = data.floors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				_data.floors[enumerator.Current.Key] = enumerator.Current.Value;
			}
			InvalidRoomList();
		}

		private void InvalidPointState()
		{
			receivePoint.Set(_data.pointState ? SwitchStatus.ON : SwitchStatus.OFF);
			receivePoint.gameObject.SetActive(_data.isMasterUser);
		}

		private void InvalidResource()
		{
			ston.text = base.localUser.ston.ToString("N0");
			elec.text = base.localUser.elec.ToString("N0");
			wood.text = base.localUser.wood.ToString("N0");
		}

		private void InvalidRoomList()
		{
			if (_data != null)
			{
				if (_data.isMasterUser && _data.floors.Count < _config.maxFloorNum && (_data.floors.Count == 0 || _data.floors[_data.floors.Count.ToString()].state == "N"))
				{
					Protocols.Dormitory.RoomInfo roomInfo = new Protocols.Dormitory.RoomInfo();
					roomInfo.fno = (_data.floors.Count + 1).ToString();
					roomInfo.state = "L";
					_data.floors.Add(roomInfo.fno, roomInfo);
				}
				roomListView.ResizeItemList(_data.floors.Count);
				int num = 0;
				Dictionary<string, Protocols.Dormitory.RoomInfo>.Enumerator enumerator = _data.floors.GetEnumerator();
				while (enumerator.MoveNext())
				{
					UIRoomListItem uIRoomListItem = roomListView.itemList[num] as UIRoomListItem;
					uIRoomListItem.gameObject.name = roomListView.itemIdPrefix + enumerator.Current.Key;
					uIRoomListItem.Set(enumerator.Current.Value, _data.isMasterUser);
					num++;
				}
			}
		}

		public override void OnClick(GameObject sender)
		{
			switch (sender.name)
			{
			case "ReceivePoint":
				base.network.RequestGetDormitoryPointAll((!SingletonMonoBehaviour<DormitoryData>.exist) ? "0" : SingletonMonoBehaviour<DormitoryData>.Instance.room.fno);
				break;
			case "Close":
				Close();
				break;
			}
			base.OnClick(sender);
		}

		public void OnMessage(GameObject arg0, GameObject arg1)
		{
			string text = arg0.name;
			string id = ((!(arg1 == null)) ? arg1.name : string.Empty);
			switch (text)
			{
			case "Build":
			{
				bool flag = true;
				Protocols.Dormitory.RoomInfo roomInfo = _data.floors[id];
				for (int i = 0; i < roomInfo.upgradeInfo.goods.Count; i++)
				{
					GoodsDataRow goodsDataRow = roomInfo.upgradeInfo.goods[i];
					if (goodsDataRow != null && base.localUser.resourceList[goodsDataRow.serverFieldName] < roomInfo.upgradeInfo.goodsValue[i])
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81066"));
					return;
				}
				UISimplePopup.CreateBool(localization: true, "1303", "81022", null, "1304", "1305").onClick = delegate(GameObject obj)
				{
					if (obj.name == "OK")
					{
						RemoteObjectManager.instance.RequestConstructDormitoryFloor(id);
					}
				};
				break;
			}
			case "ImmediatelyComplete":
				if (base.localUser.cash < _data.floors[id].upgradeInfo.immediateCash)
				{
					UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject obj)
					{
						if (obj.name == "OK")
						{
							RemoteObjectManager.instance.RequestGetCashShopList();
						}
					};
					return;
				}
				UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), Localization.Format("81023", _data.floors[id].upgradeInfo.immediateCash), null, Localization.Get("1304"), Localization.Get("1305")).onClick = delegate(GameObject obj)
				{
					if (obj.name == "OK")
					{
						RemoteObjectManager.instance.RequestFinishConstructDormitoryFloor(id, 1);
					}
				};
				break;
			case "Open":
				RemoteObjectManager.instance.RequestFinishConstructDormitoryFloor(id, 0);
				break;
			case "Move":
				if (_data.isMasterUser)
				{
					RemoteObjectManager.instance.RequestGetDormitoryFloorDetailInfo(id);
				}
				else
				{
					RemoteObjectManager.instance.RequestGetDormitoryUserFloorDetailInfo(SingletonMonoBehaviour<DormitoryData>.Instance.user.uno, id);
				}
				break;
			}
			base.OnClick(arg0);
		}
	}
}
