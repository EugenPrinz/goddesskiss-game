using System;
using System.Collections.Generic;
using Shared.Regulation;

namespace RoomDecorator.Data
{
	public class RoDormitory
	{
		public class User
		{
			public string uno;

			public string name;

			public int level;

			public bool isMaster;
		}

		public class Config
		{
			public int openLevel;

			public int changeRoomNameCache;

			public int maxFloorNum;

			public int maxRoomCharacter;

			public int inventoryLimit;

			public int itemAmountLimit;

			public int favorUserLimit;

			public void Init()
			{
				openLevel = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["SUPPLEMENT_LAND_OPEN_LEVEL"].value);
				changeRoomNameCache = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["DORMITORY_NAME_CHANGE_CASH"].value);
				maxFloorNum = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["DORMITORY_UPGRADE_MAX"].value);
				maxRoomCharacter = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["DORMITORY_PEOPLE_MAX"].value);
				inventoryLimit = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["DORMITORY_INVENTORY_LIMIT"].value);
				itemAmountLimit = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["DORMITORY_ITEM_LIMIT"].value);
				favorUserLimit = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["DORMITORY_MARK_MAX"].value);
			}
		}

		public class Item
		{
			public EDormitoryItemType type = EDormitoryItemType.Normal;

			protected string _id;

			private DataRow _data;

			public virtual string id
			{
				get
				{
					return _id;
				}
				set
				{
					if (!(_id == value))
					{
						_id = value;
						_data = null;
					}
				}
			}

			public DataRow data
			{
				get
				{
					if (_data == null)
					{
						_data = ItemDBLoader.Load(type, _id);
					}
					return _data;
				}
			}

			public Item(EDormitoryItemType type)
			{
				this.type = type;
			}

			public Item(EDormitoryItemType type, string id)
			{
				this.type = type;
				this.id = id;
			}

			public Item Clone()
			{
				Item item = new Item(type, id);
				item._data = _data;
				return item;
			}
		}

		public class InvenSlot
		{
			public Item item;

			public int amount;

			public InvenSlot(Item item, int amount)
			{
				this.item = item;
				this.amount = amount;
			}

			public InvenSlot(EDormitoryItemType type, string id, int amount)
			{
				item = new Item(type, id);
				this.amount = amount;
			}
		}

		public class InventoryData
		{
			public Dictionary<string, InvenSlot> itemNormal;

			public Dictionary<string, InvenSlot> itemAdvanced;

			public Dictionary<string, InvenSlot> itemWallpaper;

			public Dictionary<string, InvenSlot> costumeBody;

			public Dictionary<string, Dictionary<string, Item>> costumeHead;

			private Action<Dictionary<string, int>>[] _SetData;

			private Action<Dictionary<string, int>>[] _UpdateData;

			public InventoryData()
			{
				itemNormal = new Dictionary<string, InvenSlot>();
				itemAdvanced = new Dictionary<string, InvenSlot>();
				itemWallpaper = new Dictionary<string, InvenSlot>();
				costumeBody = new Dictionary<string, InvenSlot>();
				costumeHead = new Dictionary<string, Dictionary<string, Item>>();
				_SetData = new Action<Dictionary<string, int>>[6]
				{
					null,
					delegate(Dictionary<string, int> data)
					{
						SetData(itemNormal, data, (string id) => new Item(EDormitoryItemType.Normal, id));
					},
					delegate(Dictionary<string, int> data)
					{
						SetData(itemAdvanced, data, (string id) => new Item(EDormitoryItemType.Advanced, id));
					},
					delegate(Dictionary<string, int> data)
					{
						SetData(itemWallpaper, data, (string id) => new Item(EDormitoryItemType.Wallpaper, id));
					},
					null,
					delegate(Dictionary<string, int> data)
					{
						SetData(costumeBody, data, (string id) => new Item(EDormitoryItemType.CostumeBody, id));
					}
				};
				_UpdateData = new Action<Dictionary<string, int>>[6]
				{
					null,
					delegate(Dictionary<string, int> data)
					{
						UpdateData(itemNormal, data, (string id) => new Item(EDormitoryItemType.Normal, id));
					},
					delegate(Dictionary<string, int> data)
					{
						UpdateData(itemAdvanced, data, (string id) => new Item(EDormitoryItemType.Advanced, id));
					},
					delegate(Dictionary<string, int> data)
					{
						UpdateData(itemWallpaper, data, (string id) => new Item(EDormitoryItemType.Wallpaper, id));
					},
					null,
					delegate(Dictionary<string, int> data)
					{
						UpdateData(costumeBody, data, (string id) => new Item(EDormitoryItemType.CostumeBody, id));
					}
				};
			}

			public void SetHead(Dictionary<string, List<string>> data)
			{
				costumeHead.Clear();
				UpdateHead(data);
			}

			public void UpdateHead(Dictionary<string, List<string>> data)
			{
				if (data == null)
				{
					return;
				}
				Dictionary<string, List<string>>.Enumerator enumerator = data.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (!costumeHead.ContainsKey(enumerator.Current.Key))
					{
						costumeHead.Add(enumerator.Current.Key, new Dictionary<string, Item>());
					}
					List<string> value = enumerator.Current.Value;
					for (int i = 0; i < value.Count; i++)
					{
						costumeHead[enumerator.Current.Key].Add(value[i], new Item(EDormitoryItemType.CostumeHead, value[i]));
					}
				}
			}

			public void SetData(EDormitoryItemType type, Dictionary<string, int> data)
			{
				_SetData[(int)type](data);
			}

			public void UpdateData(EDormitoryItemType type, Dictionary<string, int> data)
			{
				if (data != null)
				{
					_UpdateData[(int)type](data);
				}
			}

			private void SetData(Dictionary<string, InvenSlot> item, Dictionary<string, int> data, Func<string, Item> creater)
			{
				item.Clear();
				Dictionary<string, int>.Enumerator enumerator = data.GetEnumerator();
				while (enumerator.MoveNext())
				{
					item.Add(enumerator.Current.Key, new InvenSlot(creater(enumerator.Current.Key), enumerator.Current.Value));
				}
			}

			private void UpdateData(Dictionary<string, InvenSlot> item, Dictionary<string, int> data, Func<string, Item> creater)
			{
				if (item == null || data == null)
				{
					return;
				}
				Dictionary<string, int>.Enumerator enumerator = data.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string key = enumerator.Current.Key;
					int value = enumerator.Current.Value;
					if (item.ContainsKey(enumerator.Current.Key))
					{
						if (value > 0)
						{
							item[key].amount = value;
						}
						else
						{
							item.Remove(key);
						}
					}
					else if (value > 0)
					{
						item.Add(key, new InvenSlot(creater(key), value));
					}
				}
			}
		}

		public Config config;

		public InventoryData invenData;

		public Dictionary<string, RoCharacter> characters;

		public void Init()
		{
			config = new Config();
			invenData = new InventoryData();
			characters = new Dictionary<string, RoCharacter>();
			config.Init();
		}

		public void InitCommanders()
		{
			characters = new Dictionary<string, RoCharacter>();
			Regulation regulation = RemoteObjectManager.instance.regulation;
			List<RoCommander> commanderList = RemoteObjectManager.instance.localUser.commanderList;
			for (int i = 0; i < commanderList.Count; i++)
			{
				if (regulation.dormitoryHeadCostumeMap.ContainsKey(commanderList[i].id))
				{
					characters.Add(commanderList[i].id, RoCharacter.Create(commanderList[i]));
				}
			}
		}

		public void Set(Protocols.Dormitory.Info data)
		{
			config.inventoryLimit = data.info["inven"];
			SetInvenData(data);
		}

		public void Set(Dictionary<string, Protocols.Dormitory.CommanderInfo> data)
		{
			Dictionary<string, RoCharacter>.Enumerator enumerator = characters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!data.ContainsKey(enumerator.Current.Key))
				{
					enumerator.Current.Value.fno = "0";
				}
				else
				{
					enumerator.Current.Value.fno = data[enumerator.Current.Key].fno;
				}
			}
		}

		public void SetInvenData(Protocols.Dormitory.InventoryData data)
		{
			invenData.SetData(EDormitoryItemType.Normal, data.itemNormal);
			invenData.SetData(EDormitoryItemType.Advanced, data.itemAdvanced);
			invenData.SetData(EDormitoryItemType.Wallpaper, data.itemWallpaper);
			invenData.SetData(EDormitoryItemType.CostumeBody, data.costumeBody);
			invenData.SetHead(data.costumeHead);
		}

		public void UpdateInvenData(Protocols.Dormitory.InventoryData data)
		{
			UpdateInvenData(EDormitoryItemType.Normal, data.itemNormal);
			UpdateInvenData(EDormitoryItemType.Advanced, data.itemAdvanced);
			UpdateInvenData(EDormitoryItemType.Wallpaper, data.itemWallpaper);
			UpdateInvenData(EDormitoryItemType.CostumeBody, data.costumeBody);
			UpdateHeadData(data.costumeHead);
		}

		public void UpdateHeadData(Dictionary<string, List<string>> data)
		{
			invenData.UpdateHead(data);
		}

		public void UpdateInvenData(EDormitoryItemType type, Dictionary<string, int> data)
		{
			invenData.UpdateData(type, data);
		}
	}
}
