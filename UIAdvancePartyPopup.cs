using System.Collections.Generic;
using Shared;
using Shared.Regulation;
using UnityEngine;

public class UIAdvancePartyPopup : UISimplePopup
{
	public UIDefaultListView commanderListView;

	public UITroop advancePartyTroop;

	public UILabel empty;

	public UILabel possibleStage;

	public GameObject advanceBtn;

	private RoTroop _currTroop;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
	}

	public void SetData(AnnihilationMode mode)
	{
		if (_currTroop == null)
		{
			_currTroop = RoTroop.Create(base.localUser.id);
		}
		List<RoCommander> advancePossibleCommanderList = base.localUser.GetAdvancePossibleCommanderList(EJob.All);
		advancePossibleCommanderList.Sort((RoCommander row, RoCommander row2) => -row.currLevelUnitReg.speed.CompareTo(row2.currLevelUnitReg.speed));
		commanderListView.InitAdvancePossibleCommanderList(advancePossibleCommanderList, "Advance_");
		int num = 8900 + int.Parse(base.regulation.defineDtbl["ANNIHILATE_PILOT_CLASS_LIMIT"].value);
		UISetter.SetLabel(empty, Localization.Format("80025", Localization.Get(num.ToString())));
		UISetter.SetActive(empty, advancePossibleCommanderList.Count == 0);
		UISetter.SetLabel(possibleStage, Localization.Format("80033", PossibleStageId(_currTroop.GetTotalSpeed(), mode)));
		advancePartyTroop.Set(_currTroop);
		UISetter.SetButtonEnable(advanceBtn, !_currTroop.IsEmpty());
	}

	private string PossibleStageId(int speed, AnnihilationMode mode)
	{
		int num = 0;
		int LastStage = 0;
		switch (mode)
		{
		case AnnihilationMode.NORMAL:
			LastStage = 100;
			break;
		case AnnihilationMode.HARD:
			LastStage = 200;
			break;
		case AnnihilationMode.HELL:
			LastStage = 300;
			break;
		}
		List<AnnihilateBattleDataRow> list = base.regulation.annihilateBattleDtbl.FindAll((AnnihilateBattleDataRow row) => int.Parse(row.idx) > LastStage - 100 && int.Parse(row.idx) < LastStage);
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].needSpeed <= speed)
				{
					num = int.Parse(list[i].idx);
				}
			}
		}
		num -= LastStage - 100;
		if (num <= 0)
		{
			return "0";
		}
		return num.ToString();
	}

	public void SetStageDeck(Dictionary<string, int> preDeck)
	{
		base.localUser.ResetAdvancePossible();
		RoTroop.Slot[] slots = _currTroop.slots;
		foreach (RoTroop.Slot slot in slots)
		{
			slot.ResetSlot();
		}
		int num = 0;
		if (preDeck.Count > 0)
		{
			foreach (KeyValuePair<string, int> item in preDeck)
			{
				RoCommander roCommander = base.localUser.FindCommander(item.Value.ToString());
				if (!roCommander.isAdvancePossible || roCommander.isDie)
				{
					continue;
				}
				roCommander.isAdvance = true;
				RoTroop.Slot slot2 = _currTroop.slots[num];
				slot2.unitId = roCommander.unitId;
				slot2.unitLevel = roCommander.level;
				slot2.exp = roCommander.aExp;
				slot2.health = roCommander.hp;
				slot2.commanderId = roCommander.id;
				slot2.unitCls = roCommander.cls;
				slot2.unitCostume = roCommander.currentCostume;
				slot2.favorRewardStep = roCommander.favorRewardStep;
				slot2.marry = roCommander.marry;
				slot2.transcendence = roCommander.transcendence;
				slot2.unitRank = roCommander.rank;
				slot2.position = int.Parse(item.Key) - 1;
				slot2.equipItem = roCommander.GetEquipItemList();
				slot2.weaponItem = roCommander.WeaponItem;
				for (int j = 0; j < roCommander.unitReg.skillDrks.Count; j++)
				{
					Troop.Slot.Skill skill = new Troop.Slot.Skill();
					SkillDataRow skillDataRow = base.regulation.skillDtbl[roCommander.unitReg.skillDrks[j]];
					if (skillDataRow.isActiveSkillType)
					{
						skill.sp = roCommander.sp;
					}
					skill.id = roCommander.unitReg.skillDrks[j];
					skill.lv = roCommander.skillList[j];
					slot2.skills.Add(skill);
				}
				num++;
			}
		}
		OnRefresh();
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		switch (text)
		{
		case "Close":
			base.localUser.ResetAdvancePossible();
			ClosePopup();
			return;
		case "EditTroop":
		{
			BattleData battleData = BattleData.Create(EBattleType.Annihilation);
			UIPopup.Create<UISelectDeckPopup>("SelectDeckPopup").Init(battleData);
			return;
		}
		case "DispatchAdvancePartyBtn":
			AdvancePartyNotice();
			return;
		}
		if (text.StartsWith("DieCommander-"))
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("80012"));
		}
		else if (text.StartsWith("AddCommander-"))
		{
			string commanderId = text.Substring(text.IndexOf("-") + 1);
			AddCommander(commanderId);
		}
		else if (text.StartsWith("RemoveCommander-") || text.StartsWith("CommanderSlot-"))
		{
			string text2 = text.Substring(text.IndexOf("-") + 1);
			if (!string.IsNullOrEmpty(text2))
			{
				RemoveCommander(text2);
			}
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		AnnihilationMode data = AnnihilationMode.NONE;
		if (UIManager.instance.world.existAnnihilationMap && UIManager.instance.world.annihilationMap.isActive)
		{
			data = UIManager.instance.world.annihilationMap.GetCurSelectMode();
		}
		SetData(data);
	}

	private void AddCommander(string commanderId)
	{
		if (!_currTroop.IsAddSlotPossible())
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("1329"));
		}
		else if (_currTroop.GetSlotByCommanderId(commanderId) == null)
		{
			RoTroop.Slot nextEmptySlot = _currTroop.GetNextEmptySlot();
			RoCommander roCommander = base.localUser.FindCommander(commanderId);
			roCommander.isAdvance = true;
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
			for (int i = 0; i < roCommander.unitReg.skillDrks.Count; i++)
			{
				Troop.Slot.Skill skill = new Troop.Slot.Skill();
				skill.id = roCommander.unitReg.skillDrks[i];
				skill.lv = roCommander.skillList[i];
				nextEmptySlot.skills.Add(skill);
			}
			SoundManager.PlaySFX("BTN_Formation_001");
			OnRefresh();
		}
	}

	private void RemoveCommander(string commanderId)
	{
		RoTroop.Slot[] slots = _currTroop.slots;
		int slotIndexByCommanderId = _currTroop.GetSlotIndexByCommanderId(commanderId);
		int position = slots[slotIndexByCommanderId].position;
		slots[slotIndexByCommanderId].ResetSlot();
		RoCommander roCommander = base.localUser.FindCommander(commanderId);
		roCommander.isAdvance = false;
		SoundManager.PlaySFX("BTN_Norma_001");
		OnRefresh();
	}

	private void AdvancePartyNotice()
	{
		UISimplePopup.CreateBool(localization: true, "1303", "80008", "80009", "1304", "1305").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				FullUnitCheck();
			}
		};
	}

	private void FullUnitCheck()
	{
		if (FullUnitState())
		{
			ClosePopup();
			base.network.RequestDispatchAdvancedParty(_currTroop);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "1331", null, "1001", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				ClosePopup();
				base.network.RequestDispatchAdvancedParty(_currTroop);
			}
		};
	}

	private bool FullUnitState()
	{
		int num = 0;
		for (int i = 0; i < _currTroop.slots.Length; i++)
		{
			RoTroop.Slot slot = _currTroop.slots[i];
			if (slot.IsValidId())
			{
				num++;
			}
		}
		return num == 5 || num == base.localUser.GetCommanderCount();
	}
}
