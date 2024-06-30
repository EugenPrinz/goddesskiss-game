using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPreDeckSetting : UIPopup
{
	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIFlipSwitch basicTab;

	public UIFlipSwitch attackTab;

	public UIFlipSwitch defenseTab;

	public UIFlipSwitch supportTab;

	public UISprite dragUnit;

	private int dragStartPosition = -1;

	private List<UIPositionPlate> positionList;

	public UIDefaultListView commanderListView;

	public UIDefaultListView preTroopList;

	private RoTroop[] _currTroop;

	private int selectTroopIdx;

	private EJob commanderJob;

	private int _selectedPositionId;

	private Transform unitFormation;

	private int maxPredeckCount = 5;

	private int basePredeckCount = 5;

	private int openCost = 1200;

	private int addOpenCost = 200;

	private int predeckCount = 5;

	private UITroop preTroop(int index)
	{
		return preTroopList.itemList[index] as UITroop;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		UICamera.onDragStart = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onDragStart, new UICamera.VoidDelegate(OnDragStart));
		UICamera.onDragEnd = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onDragEnd, new UICamera.VoidDelegate(OnDragEnd));
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Remove(UICamera.onDrag, new UICamera.VectorDelegate(OnDrag));
		UICamera.onDragOut = (UICamera.ObjectDelegate)Delegate.Remove(UICamera.onDragOut, new UICamera.ObjectDelegate(OnDragOver));
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		UICamera.onDragStart = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onDragStart, new UICamera.VoidDelegate(OnDragStart));
		UICamera.onDragEnd = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onDragEnd, new UICamera.VoidDelegate(OnDragEnd));
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Combine(UICamera.onDrag, new UICamera.VectorDelegate(OnDrag));
		UICamera.onDragOut = (UICamera.ObjectDelegate)Delegate.Combine(UICamera.onDragOut, new UICamera.ObjectDelegate(OnDragOver));
	}

	protected override void Awake()
	{
		base.Awake();
		maxPredeckCount = int.Parse(base.regulation.defineDtbl["PRE_DECK_COUNT"].value);
		basePredeckCount = int.Parse(base.regulation.defineDtbl["BASE_DECK_COUNT"].value);
		openCost = int.Parse(base.regulation.defineDtbl["DECK_PLUS_CASH"].value);
		addOpenCost = int.Parse(base.regulation.defineDtbl["DECK_PLUS_CASH_VALUE"].value);
	}

	public override void OnClick(GameObject sender)
	{
		SoundManager.PlaySFX("EFM_SelectButton_001");
		string text = sender.name;
		if (text == "Close")
		{
			CloseAnimation();
			return;
		}
		if (text.StartsWith("DeckItem-"))
		{
			int num = int.Parse(text.Substring(text.IndexOf("-") + 1));
			if (num + 1 > base.localUser.statistics.predeckCount)
			{
				OpenBuySlotPopup();
			}
			else if (selectTroopIdx != num)
			{
				selectTroopIdx = num;
				OnRefresh();
			}
			else
			{
				OnRefresh();
			}
			return;
		}
		if (text.StartsWith("Plate-"))
		{
			int num2 = int.Parse(text.Substring(text.IndexOf("-") + 1));
			RoTroop.Slot slotByPosition = _currTroop[selectTroopIdx].GetSlotByPosition(num2);
			if (slotByPosition != null && _selectedPositionId != num2)
			{
				_selectedPositionId = num2;
			}
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
		case "BasicTab":
			commanderJob = EJob.All;
			InitCommanderList();
			break;
		case "AttackTab":
			commanderJob = EJob.Attack;
			InitCommanderList();
			break;
		case "DefenseTab":
			commanderJob = EJob.Defense;
			InitCommanderList();
			break;
		case "SupportTab":
			commanderJob = EJob.Support;
			InitCommanderList();
			break;
		case "EditBtn":
		{
			if (sender.transform.GetComponentInParent<UITroop>() != preTroop(selectTroopIdx))
			{
				break;
			}
			UIReceiveUserString recv = UIPopup.Create<UIReceiveUserString>("InputUserString");
			recv.SetDefault(_currTroop[selectTroopIdx].nickname);
			recv.SetLimitLength(10);
			recv.Set(localization: false, Localization.Get("1357"), string.Empty, null, Localization.Get("1006"), Localization.Get("1000"), null);
			recv.onClick = delegate(GameObject popupSender)
			{
				string text2 = popupSender.name;
				if (text2 == "OK")
				{
					_currTroop[selectTroopIdx].nickname = recv.inputLabel.text;
					_SelectTroop(selectTroopIdx);
				}
			};
			break;
		}
		}
	}

	private void OnDragStart(GameObject go)
	{
		if (dragUnit == null)
		{
			return;
		}
		string text = go.name;
		if (text.StartsWith("Plate-") && !(go.transform.GetComponentInParent<UITroop>() != preTroop(selectTroopIdx)))
		{
			int num = int.Parse(text.Substring(text.IndexOf("-") + 1));
			UIPositionPlate uIPositionPlate = positionList[num];
			if (_currTroop[selectTroopIdx].GetSlotByPosition(num) == null)
			{
				dragStartPosition = -1;
				return;
			}
			uIPositionPlate.CopyThumbnail(dragUnit);
			UISetter.SetActive(dragUnit, active: true);
			uIPositionPlate.SetThumbnailAlpha(0.5f);
			dragStartPosition = num;
		}
	}

	private void OnDragEnd(GameObject go)
	{
		UISetter.SetActive(dragUnit, active: false);
		dragStartPosition = -1;
	}

	private void OnDrag(GameObject go, Vector2 delta)
	{
		if (dragStartPosition >= 0)
		{
			dragUnit.transform.position = UICamera.lastWorldPosition;
		}
	}

	private void OnDragOver(GameObject go, GameObject obj)
	{
		if (dragStartPosition < 0)
		{
			return;
		}
		string text = go.name;
		positionList[dragStartPosition].SetThumbnailAlpha(1f);
		if (!text.StartsWith("Plate-") || go.transform.GetComponentInParent<UITroop>() != preTroop(selectTroopIdx))
		{
			return;
		}
		int num = int.Parse(text.Substring(text.IndexOf("-") + 1));
		RoTroop.Slot slotByPosition = _currTroop[selectTroopIdx].GetSlotByPosition(dragStartPosition);
		RoTroop.Slot slotByPosition2 = _currTroop[selectTroopIdx].GetSlotByPosition(num);
		if (slotByPosition != null && slotByPosition2 != null)
		{
			if (slotByPosition.position == _selectedPositionId)
			{
				_selectedPositionId = slotByPosition2.position;
			}
			else if (slotByPosition2.position == _selectedPositionId)
			{
				_selectedPositionId = slotByPosition.position;
			}
			int position = slotByPosition.position;
			slotByPosition.position = slotByPosition2.position;
			slotByPosition2.position = position;
		}
		else if (slotByPosition2 == null)
		{
			if (slotByPosition.position == _selectedPositionId)
			{
				_selectedPositionId = num;
			}
			slotByPosition.position = num;
		}
		OnRefresh();
	}

	public void InitOpenPopup()
	{
		if (bEnterKeyEnable)
		{
			return;
		}
		bEnterKeyEnable = true;
		if (positionList == null)
		{
			positionList = new List<UIPositionPlate>();
		}
		if (_currTroop == null)
		{
			_currTroop = new RoTroop[maxPredeckCount];
		}
		for (int i = 0; i < maxPredeckCount; i++)
		{
			if (_currTroop[i] != null)
			{
				_currTroop[i] = null;
			}
			_currTroop[i] = RoTroop.Create(base.localUser.id);
		}
		predeckCount = base.localUser.statistics.predeckCount;
		OnResetPlate();
		SetPreDeck();
		InitTab();
		InitCommanderList();
		Open();
	}

	private void SetPreDeck()
	{
		for (int i = 0; i < predeckCount; i++)
		{
			Protocols.UserInformationResponse.PreDeck preDeck = base.localUser.preDeckList[i];
			int num = 0;
			foreach (KeyValuePair<string, int> deckDatum in preDeck.deckData)
			{
				RoTroop.Slot slot = _currTroop[preDeck.idx].slots[num];
				RoCommander roCommander = base.localUser.FindCommander(deckDatum.Value.ToString());
				slot.unitLevel = roCommander.level;
				slot.exp = roCommander.aExp;
				slot.health = 10000;
				slot.unitCls = roCommander.cls;
				slot.unitCostume = roCommander.currentCostume;
				slot.favorRewardStep = roCommander.favorRewardStep;
				slot.marry = roCommander.marry;
				slot.transcendence = roCommander.transcendence;
				slot.unitRank = roCommander.rank;
				slot.commanderId = roCommander.id;
				slot.unitId = roCommander.unitId;
				slot.equipItem = roCommander.GetEquipItemList();
				slot.weaponItem = roCommander.WeaponItem;
				slot.position = int.Parse(deckDatum.Key) - 1;
				num++;
			}
			_currTroop[preDeck.idx].nickname = preDeck.name;
		}
		int count = Mathf.Min(predeckCount + 1, maxPredeckCount);
		preTroopList.InitPredeckSlot(_currTroop, count, "DeckItem-");
		_SelectTroop(selectTroopIdx);
	}

	private void deleteSaveDeck(string key)
	{
		PlayerPrefs.DeleteKey(key);
		_currTroop[selectTroopIdx].ResetSlots();
	}

	private void AddCommander(string commanderId)
	{
		if (!_currTroop[selectTroopIdx].IsAddSlotPossible())
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("1329"));
		}
		else
		{
			if (_currTroop[selectTroopIdx].GetSlotByCommanderId(commanderId) != null)
			{
				return;
			}
			RoTroop.Slot nextEmptySlot = _currTroop[selectTroopIdx].GetNextEmptySlot();
			RoCommander roCommander = base.localUser.FindCommander(commanderId);
			nextEmptySlot.unitId = roCommander.unitId;
			nextEmptySlot.unitLevel = roCommander.level;
			nextEmptySlot.exp = roCommander.aExp;
			nextEmptySlot.health = 10000;
			nextEmptySlot.unitCls = roCommander.cls;
			nextEmptySlot.unitCostume = roCommander.currentCostume;
			nextEmptySlot.favorRewardStep = roCommander.favorRewardStep;
			nextEmptySlot.marry = roCommander.marry;
			nextEmptySlot.transcendence = roCommander.transcendence;
			nextEmptySlot.unitRank = roCommander.rank;
			nextEmptySlot.commanderId = roCommander.id;
			nextEmptySlot.equipItem = roCommander.GetEquipItemList();
			nextEmptySlot.weaponItem = roCommander.WeaponItem;
			for (int i = 0; i < _currTroop[selectTroopIdx].slots.Length; i++)
			{
				if (_currTroop[selectTroopIdx].GetSlotIndexByPosition(i) < 0)
				{
					nextEmptySlot.position = i;
					break;
				}
			}
			OnRefresh();
		}
	}

	private void RemoveCommander(string commanderId)
	{
		RoTroop.Slot[] slots = _currTroop[selectTroopIdx].slots;
		int slotIndexByCommanderId = _currTroop[selectTroopIdx].GetSlotIndexByCommanderId(commanderId);
		int position = slots[slotIndexByCommanderId].position;
		slots[slotIndexByCommanderId].ResetSlot();
		OnRefresh();
	}

	private void OnResetPlate()
	{
		for (int i = 0; i < positionList.Count; i++)
		{
			positionList[i].Reset();
		}
	}

	private void SelectTroopPosition()
	{
		for (int i = 0; i < preTroopList.itemList.Count; i++)
		{
			if (i == selectTroopIdx)
			{
				preTroop(i).gameObject.transform.localScale = new Vector3(1.1f, 1.1f);
				preTroop(i).gameObject.transform.localPosition = new Vector3(i * 320 + 13, 13f);
			}
			else
			{
				preTroop(i).gameObject.transform.localScale = Vector3.one;
				preTroop(i).gameObject.transform.localPosition = new Vector3(i * 320, 0f);
			}
		}
		preTroopList.SetSelection(selectTroopIdx.ToString(), selected: true);
	}

	private void _SelectTroop(int index)
	{
		preTroop(index).Set(_currTroop[index]);
		for (int i = 0; i < base.localUser.statistics.predeckCount; i++)
		{
			if (i == selectTroopIdx)
			{
				positionList.Clear();
				unitFormation = preTroop(i).gameObject.transform.Find("UnitFormation");
				positionList.Add(unitFormation.Find("00").GetComponent<UIPositionPlate>());
				positionList.Add(unitFormation.Find("01").GetComponent<UIPositionPlate>());
				positionList.Add(unitFormation.Find("02").GetComponent<UIPositionPlate>());
				positionList.Add(unitFormation.Find("03").GetComponent<UIPositionPlate>());
				positionList.Add(unitFormation.Find("04").GetComponent<UIPositionPlate>());
				positionList.Add(unitFormation.Find("05").GetComponent<UIPositionPlate>());
				positionList.Add(unitFormation.Find("06").GetComponent<UIPositionPlate>());
				positionList.Add(unitFormation.Find("07").GetComponent<UIPositionPlate>());
				positionList.Add(unitFormation.Find("08").GetComponent<UIPositionPlate>());
			}
		}
		SelectTroopPosition();
	}

	private void InitTab()
	{
		UISetter.SetFlipSwitch(basicTab, state: true);
		UISetter.SetFlipSwitch(attackTab, state: false);
		UISetter.SetFlipSwitch(defenseTab, state: false);
		UISetter.SetFlipSwitch(supportTab, state: false);
		commanderJob = EJob.All;
	}

	private void InitCommanderList()
	{
		commanderListView.ResetPosition();
		commanderListView.Init(base.localUser.GetCommanderList(commanderJob, have: true), _currTroop[selectTroopIdx], null, "Commander_");
	}

	private void OpenBuySlotPopup()
	{
		int cost = openCost + addOpenCost * (predeckCount - basePredeckCount);
		UISimplePopup.CreateBool(localization: false, Localization.Get("1002"), Localization.Format("27863", cost.ToString("N0")), null, Localization.Get("1002"), Localization.Get("1000")).onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (base.localUser.cash < cost)
				{
					UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject sSender)
					{
						string text2 = sSender.name;
						if (text2 == "OK")
						{
							UIManager.instance.world.mainCommand.OpenDiamonShop();
						}
					};
				}
				else
				{
					base.network.RequestBuyPredeckSlot();
				}
			}
		};
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		if (predeckCount != base.localUser.statistics.predeckCount)
		{
			predeckCount = base.localUser.statistics.predeckCount;
			int count = Mathf.Min(predeckCount + 1, maxPredeckCount);
			preTroopList.InitPredeckSlot(_currTroop, count, "DeckItem-");
		}
		if (_currTroop != null)
		{
			_SelectTroop(selectTroopIdx);
			commanderListView.Init(base.localUser.GetCommanderList(commanderJob, have: true), _currTroop[selectTroopIdx], null, "Commander_");
		}
	}

	public void OpenAnimation()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", 0, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeOutBack));
	}

	public void CloseAnimation()
	{
		List<Protocols.UserInformationResponse.PreDeck> list = new List<Protocols.UserInformationResponse.PreDeck>();
		for (int i = 0; i < base.localUser.statistics.predeckCount; i++)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			for (int j = 0; j < _currTroop[i].slots.Length; j++)
			{
				RoTroop.Slot slot = _currTroop[i].slots[j];
				if (slot.IsValid())
				{
					dictionary.Add((slot.position + 1).ToString(), int.Parse(slot.commanderId));
				}
			}
			Protocols.UserInformationResponse.PreDeck preDeck = new Protocols.UserInformationResponse.PreDeck();
			preDeck.idx = i;
			preDeck.name = _currTroop[i].nickname;
			preDeck.deckData = dictionary;
			list.Add(preDeck);
		}
		base.network.RequestPreDeckSetting(list);
		iTween.MoveTo(base.gameObject, iTween.Hash("y", -1000, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeInBack, "oncomplete", "Close", "oncompletetarget", base.gameObject));
	}

	public override void Open()
	{
		base.Open();
		OpenAnimation();
	}

	public override void Close()
	{
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
	}
}
