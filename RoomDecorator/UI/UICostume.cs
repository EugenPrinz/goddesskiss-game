using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RoomDecorator.Data;
using RoomDecorator.Event;
using Shared.Regulation;
using UnityEngine;

namespace RoomDecorator.UI
{
	public class UICostume : MonoBehaviour
	{
		public class SelectItem
		{
			public EDormitoryItemType type;

			public string id;

			public string itemName;

			public string costIconName;

			public string costValue;

			private bool _exists;

			public void Set(EDormitoryItemType type, string id)
			{
				this.type = type;
				this.id = id;
				_exists = true;
			}

			public void Set(RoDormitory.InvenSlot slot)
			{
				Set(slot.item.type, slot.item.id);
			}

			public void Clear()
			{
				id = string.Empty;
				_exists = false;
			}

			public bool Exsits()
			{
				return _exists;
			}
		}

		public GEAnimNGUI animBG;

		public GEAnimNGUI animBlock;

		public UIFlipSwitch tabBody;

		public UIFlipSwitch tabHead;

		public UIListViewBase costumeListView;

		public GameObject moveLeft;

		public GameObject moveRight;

		public UIButton applyButton;

		public UIButton sellButton;

		public UIButton buyButton;

		public UICharacter selectedCharacter;

		public UISellItem sellItem;

		private bool _isOpen;

		private bool _isClose = true;

		private RoDormitoryRoom _room;

		private EDormitoryItemType _curType;

		private int _characterIdx;

		private List<RoCharacter> _characters;

		private RoCharacter _character;

		private Dictionary<string, UICostumeListItem> _objs;

		private RoDormitory.InventoryData _inventoryData;

		private Dictionary<string, RoDormitory.InvenSlot> _bodyCostumes;

		private Dictionary<string, RoDormitory.Item> _headCostumes;

		private SelectItem _selectedItem;

		private void Awake()
		{
			_selectedItem = new SelectItem();
			_characterIdx = 0;
			_curType = EDormitoryItemType.CostumeBody;
			_room = SingletonMonoBehaviour<DormitoryData>.Instance.room;
			_inventoryData = SingletonMonoBehaviour<DormitoryData>.Instance.dormitory.invenData;
			_bodyCostumes = SingletonMonoBehaviour<DormitoryData>.Instance.dormitory.invenData.costumeBody;
			_objs = new Dictionary<string, UICostumeListItem>();
			costumeListView.itemIdPrefix = "Item-";
		}

		private void OnEnable()
		{
			Message.AddListener("Inven.Update", InvalidCharacter);
			Message.AddListener<string>("Chr.Update.Costume", InvalidCharacter);
		}

		private void OnDisable()
		{
			Message.RemoveListener("Inven.Update", InvalidCharacter);
			Message.RemoveListener<string>("Chr.Update.Costume", InvalidCharacter);
		}

		public void Open()
		{
			if (_isOpen)
			{
				return;
			}
			_isOpen = true;
			_isClose = false;
			base.gameObject.SetActive(value: true);
			costumeListView.ResetPosition();
			_curType = EDormitoryItemType.CostumeBody;
			_characters = new List<RoCharacter>();
			Dictionary<string, RoCharacter>.Enumerator enumerator = _room.characters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				_characters.Add(enumerator.Current.Value);
			}
			bool flag = true;
			if (_character != null && _characters.Count > _characterIdx && _character.id == _characters[_characterIdx].id)
			{
				flag = false;
			}
			if (flag)
			{
				_characterIdx = 0;
				_character = null;
				if (_characters.Count > _characterIdx)
				{
					_character = _characters[_characterIdx].Clone();
					_headCostumes = _inventoryData.costumeHead[_character.id];
				}
			}
			InvalidData();
			animBG.Reset();
			animBlock.Reset();
			animBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
			animBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
			SoundManager.PlaySFX("SE_MenuOpen_001");
		}

		private void InvalidData()
		{
			InvalidTab();
			InvalidCharacter();
		}

		private void Close()
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

		private void InvalidTab()
		{
			tabHead.Set((_curType == EDormitoryItemType.CostumeHead) ? SwitchStatus.ON : SwitchStatus.OFF);
			tabBody.Set((_curType == EDormitoryItemType.CostumeBody) ? SwitchStatus.ON : SwitchStatus.OFF);
		}

		private void InvalidButtons()
		{
			if (_curType == EDormitoryItemType.CostumeHead)
			{
				buyButton.gameObject.SetActive(value: true);
				sellButton.gameObject.SetActive(value: false);
				bool flag = false;
				if (_selectedItem.Exsits() && _headCostumes.ContainsKey(_selectedItem.id))
				{
					flag = true;
				}
				buyButton.isEnabled = ((_selectedItem.Exsits() && !flag) ? true : false);
				applyButton.isEnabled = ((_selectedItem.Exsits() && flag) ? true : false);
			}
			else
			{
				buyButton.gameObject.SetActive(value: false);
				sellButton.gameObject.SetActive(value: true);
				sellButton.isEnabled = (_selectedItem.Exsits() ? true : false);
				applyButton.isEnabled = (_selectedItem.Exsits() ? true : false);
			}
			moveLeft.gameObject.SetActive(_characterIdx > 0);
			moveRight.gameObject.SetActive((_characterIdx < _characters.Count - 1) ? true : false);
		}

		private void InvalidCharacter()
		{
			_selectedItem.Clear();
			_character = _characters[_characterIdx].Clone();
			InvalidButtons();
			InvalidCharacterImage();
			InvalidCostumeList();
		}

		private void InvalidCharacter(string cid)
		{
			if (_character != null && !(_character.id != cid))
			{
				_character = _characters[_characterIdx].Clone();
				_headCostumes = _inventoryData.costumeHead[_character.id];
				InvalidCharacter();
			}
		}

		private void InvalidCharacterImage()
		{
			selectedCharacter.Set(_character);
		}

		private void InvalidCostumeList()
		{
			_objs.Clear();
			if (_character == null)
			{
				costumeListView.ResizeItemList(0);
			}
			else if (_curType == EDormitoryItemType.CostumeHead)
			{
				ShowCostumeHeadList();
			}
			else if (_curType == EDormitoryItemType.CostumeBody)
			{
				ShowCostumeBodyList();
			}
			else
			{
				costumeListView.ResizeItemList(0);
			}
		}

		private void ShowCostumeHeadList()
		{
			Regulation regulation = RemoteObjectManager.instance.regulation;
			RoCharacter roCharacter = _characters[_characterIdx];
			Dictionary<string, RoDormitory.Item> hasHeadCostumes = ((_headCostumes == null) ? new Dictionary<string, RoDormitory.Item>() : _headCostumes);
			List<DormitoryHeadCostumeDataRow> list = regulation.dormitoryHeadCostumeMap[_character.id];
			list.Sort(delegate(DormitoryHeadCostumeDataRow arg1, DormitoryHeadCostumeDataRow arg2)
			{
				bool flag = hasHeadCostumes.ContainsKey(arg1.id);
				bool flag2 = hasHeadCostumes.ContainsKey(arg2.id);
				if (flag != flag2)
				{
					if (flag)
					{
						return -1;
					}
					if (flag2)
					{
						return 1;
					}
				}
				return arg1.sort.CompareTo(arg2.sort);
			});
			costumeListView.ResizeItemList(list.Count - 1);
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				DormitoryHeadCostumeDataRow dormitoryHeadCostumeDataRow = list[i];
				if (!(roCharacter.head.id == dormitoryHeadCostumeDataRow.id))
				{
					UICostumeListItem uICostumeListItem = costumeListView.itemList[num] as UICostumeListItem;
					uICostumeListItem.name = costumeListView.itemIdPrefix + dormitoryHeadCostumeDataRow.id;
					uICostumeListItem.Set(dormitoryHeadCostumeDataRow, hasHeadCostumes.ContainsKey(dormitoryHeadCostumeDataRow.id) ? 1 : 0);
					uICostumeListItem.SetSelection(selected: false);
					_objs.Add(dormitoryHeadCostumeDataRow.id, uICostumeListItem);
					num++;
				}
			}
		}

		private void ShowCostumeBodyList()
		{
			List<RoDormitory.InvenSlot> list = _bodyCostumes.Values.ToList();
			list.Sort((RoDormitory.InvenSlot arg1, RoDormitory.InvenSlot arg2) => ((DormitoryBodyCostumeDataRow)arg1.item.data).sort.CompareTo(((DormitoryBodyCostumeDataRow)arg2.item.data).sort));
			costumeListView.ResizeItemList(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				UICostumeListItem uICostumeListItem = costumeListView.itemList[i] as UICostumeListItem;
				uICostumeListItem.name = costumeListView.itemIdPrefix + list[i].item.id;
				uICostumeListItem.Set((DormitoryBodyCostumeDataRow)list[i].item.data, list[i].amount);
				uICostumeListItem.SetSelection(selected: false);
				_objs.Add(list[i].item.id, uICostumeListItem);
			}
		}

		private void ChangeCharacter(int targetIdx)
		{
			if (_characters.Count > targetIdx)
			{
				_characterIdx = targetIdx;
				_character = _characters[_characterIdx].Clone();
				_headCostumes = _inventoryData.costumeHead[_character.id];
				costumeListView.ResetPosition();
				InvalidCharacter();
				SoundManager.PlaySFX("BTN_Tap_001");
			}
		}

		private void ChangeTab(EDormitoryItemType type)
		{
			if (_curType != type)
			{
				_curType = type;
				costumeListView.ResetPosition();
				InvalidTab();
				InvalidCharacter();
				SoundManager.PlaySFX("BTN_Tap_001");
			}
		}

		public void OnClick(GameObject sender)
		{
			string text = sender.name;
			switch (text)
			{
			case "Apply":
				if (!_selectedItem.Exsits())
				{
					return;
				}
				if (_selectedItem.type == EDormitoryItemType.CostumeBody)
				{
					if (_characters[_characterIdx].body.id == _selectedItem.id)
					{
						return;
					}
					RemoteObjectManager.instance.RequestChangeDormitoryCommanderBody(_character.id, _selectedItem.id);
				}
				else if (_selectedItem.type == EDormitoryItemType.CostumeHead)
				{
					if (_characters[_characterIdx].head.id == _selectedItem.id)
					{
						return;
					}
					RemoteObjectManager.instance.RequestChangeDormitoryCommanderHead(_character.id, _selectedItem.id);
				}
				SoundManager.PlaySFX("BTN_Positive_001");
				return;
			case "Buy":
			{
				if (!_selectedItem.Exsits() || _selectedItem.type != EDormitoryItemType.CostumeHead)
				{
					return;
				}
				Regulation regulation = RemoteObjectManager.instance.regulation;
				DormitoryHeadCostumeDataRow dormitoryHeadCostumeDataRow = regulation.dormitoryHeadCostumeDtbl[_selectedItem.id];
				if (SingletonMonoBehaviour<DormitoryData>.Instance.localUser.resourceList[dormitoryHeadCostumeDataRow.goodsDr.serverFieldName] < dormitoryHeadCostumeDataRow.price)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("81071"));
					return;
				}
				UIBuyItem uIBuyItem = UIPopup.Create<UIBuyItem>("BuyItem");
				uIBuyItem.Set(Localization.Get("1303"), Localization.Format("70019", _selectedItem.itemName), dormitoryHeadCostumeDataRow.goodsDr.icon, dormitoryHeadCostumeDataRow.price.ToString(), Localization.Get("1304"), Localization.Get("1305"));
				uIBuyItem.onClick = delegate(GameObject obj)
				{
					if (obj.name == "Ok")
					{
						RemoteObjectManager.instance.RequestBuyDormitoryHeadCostume(_selectedItem.id);
					}
				};
				break;
			}
			case "Sell":
				if (_selectedItem.Exsits() && _selectedItem.type != EDormitoryItemType.CostumeHead)
				{
					sellItem.Set(_bodyCostumes[_selectedItem.id]);
					sellItem.Open();
				}
				return;
			case "BodyTab":
				ChangeTab(EDormitoryItemType.CostumeBody);
				return;
			case "HeadTab":
				ChangeTab(EDormitoryItemType.CostumeHead);
				return;
			case "Close":
				Close();
				return;
			case "Left":
				ChangeCharacter(_characterIdx - 1);
				break;
			case "Right":
				ChangeCharacter(_characterIdx + 1);
				break;
			}
			if (!text.StartsWith(costumeListView.itemIdPrefix))
			{
				return;
			}
			string text2 = text.Substring(text.IndexOf("-") + 1);
			if (!_selectedItem.Exsits() || !(_selectedItem.id == text2))
			{
				costumeListView.SetSelection(costumeListView.lastSelectedItemPureId, selected: false);
				costumeListView.SetSelection(text, selected: true);
				UICostumeListItem component = sender.GetComponent<UICostumeListItem>();
				if (_curType == EDormitoryItemType.CostumeHead)
				{
					_selectedItem.Set(_curType, text2);
					_selectedItem.itemName = component.itemName.text;
					_selectedItem.costIconName = component.costIcon.spriteName;
					_selectedItem.costValue = component.costValue.text;
					_character.head.id = text2;
				}
				else if (_curType == EDormitoryItemType.CostumeBody)
				{
					_selectedItem.Set(_bodyCostumes[text2]);
					_character.body.id = text2;
				}
				InvalidButtons();
				InvalidCharacterImage();
			}
		}
	}
}
