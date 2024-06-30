using System;
using System.Collections.Generic;
using RoomDecorator.Event;
using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UIRoomListItem : UIItemBase
	{
		[Serializable]
		public class UILockItem
		{
			public GameObject root;

			public UILabel openLevel;

			public void Set(Protocols.Dormitory.RoomInfo data)
			{
				if (!(root == null))
				{
					if (data.state != "L" || RemoteObjectManager.instance.localUser.level >= data.upgradeInfo.userLevel)
					{
						root.SetActive(value: false);
						return;
					}
					root.SetActive(value: true);
					openLevel.text = Localization.Format("81021", data.upgradeInfo.userLevel.ToString());
				}
			}
		}

		[Serializable]
		public class UICloseItem
		{
			public GameObject root;

			public UIListViewBase goodsList;

			public void Set(Protocols.Dormitory.RoomInfo data)
			{
				if (root == null)
				{
					return;
				}
				if (data.state != "L" || RemoteObjectManager.instance.localUser.level < data.upgradeInfo.userLevel)
				{
					root.SetActive(value: false);
					return;
				}
				root.SetActive(value: true);
				RoLocalUser localUser = RemoteObjectManager.instance.localUser;
				List<string> list = new List<string>();
				List<int> list2 = new List<int>();
				List<bool> list3 = new List<bool>();
				for (int i = 0; i < data.upgradeInfo.goodsIdxs.Count; i++)
				{
					if (data.upgradeInfo.goodsIdxs[i] != "0")
					{
						list.Add(data.upgradeInfo.goodsIdxs[i]);
						list2.Add(data.upgradeInfo.goodsValue[i]);
						GoodsDataRow goodsDataRow = data.upgradeInfo.goods[i];
						list3.Add(localUser.resourceList[goodsDataRow.serverFieldName] >= data.upgradeInfo.goodsValue[i]);
					}
				}
				goodsList.ResizeItemList(list.Count);
				for (int j = 0; j < list.Count; j++)
				{
					UIGoods uIGoods = goodsList.itemList[j] as UIGoods;
					uIGoods.name = goodsList.itemIdPrefix + list[j];
					uIGoods.SetGoodsId(list[j], list2[j]);
					UISetter.SetColor(uIGoods.count, (!list3[j]) ? Color.red : Color.white);
				}
			}
		}

		[Serializable]
		public class UIUnderConstructionItem
		{
			public GameObject root;

			public UITimer timer;

			public UILabel immediateCash;

			public void Set(Protocols.Dormitory.RoomInfo data)
			{
				if (root == null)
				{
					return;
				}
				if (data.state != "C" || data.remain <= 0.0)
				{
					root.SetActive(value: false);
					return;
				}
				immediateCash.text = data.upgradeInfo.immediateCash.ToString();
				timer.Stop();
				TimeData timeData = new TimeData();
				timeData.SetByDuration(data.remain);
				UISetter.SetTimer(timer, timeData, "81005");
				timer.RegisterOnFinished(delegate
				{
					Message.Send("Room.Update.List");
				});
				root.SetActive(value: true);
			}
		}

		[Serializable]
		public class UICompleteItem
		{
			public GameObject root;

			public void Set(Protocols.Dormitory.RoomInfo data)
			{
				if (!(root == null))
				{
					if (data.state != "C" || data.remain > 0.0)
					{
						root.SetActive(value: false);
					}
					else
					{
						root.SetActive(value: true);
					}
				}
			}
		}

		[Serializable]
		public class UIReadyItem
		{
			public GameObject root;

			public UIListViewBase characterList;

			public void Set(Protocols.Dormitory.RoomInfo data, bool isMasterUser)
			{
				if (root == null)
				{
					return;
				}
				if (data.state != "N")
				{
					root.SetActive(value: false);
					return;
				}
				root.SetActive(value: true);
				if (isMasterUser)
				{
					RoLocalUser localUser = RemoteObjectManager.instance.localUser;
					characterList.ResizeItemList(data.commanders.Count);
					for (int i = 0; i < data.commanders.Count; i++)
					{
						UIDormitoryCharacter uIDormitoryCharacter = characterList.itemList[i] as UIDormitoryCharacter;
						uIDormitoryCharacter.gameObject.name = characterList.itemIdPrefix + data.commanders[i];
						uIDormitoryCharacter.Set(localUser.dormitory.characters[data.commanders[i]]);
					}
				}
				else
				{
					characterList.ResizeItemList(data.commanderInfos.Count);
					for (int j = 0; j < data.commanderInfos.Count; j++)
					{
						UIDormitoryCharacter uIDormitoryCharacter2 = characterList.itemList[j] as UIDormitoryCharacter;
						uIDormitoryCharacter2.gameObject.name = characterList.itemIdPrefix + data.commanderInfos[j].id;
						uIDormitoryCharacter2.Set(data.commanderInfos[j]);
					}
				}
			}
		}

		public UILabel roomName;

		public UILockItem lockItem;

		public UICloseItem closeItem;

		public UIUnderConstructionItem underConstructionItem;

		public UICompleteItem completeItem;

		public UIReadyItem readyItem;

		public void Set(Protocols.Dormitory.RoomInfo data, bool isMasterUser)
		{
			roomName.text = data.name;
			if (string.IsNullOrEmpty(data.name))
			{
				roomName.text = Localization.Format("81002", data.fno);
			}
			lockItem.Set(data);
			closeItem.Set(data);
			underConstructionItem.Set(data);
			completeItem.Set(data);
			readyItem.Set(data, isMasterUser);
		}
	}
}
