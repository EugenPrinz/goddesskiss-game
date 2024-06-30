using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIWeaponListPopup : UIPopup
{
	[Serializable]
	public class UIWeaponSkillInfo : UIInnerPartBase
	{
		public UISprite icon;

		public UISprite skillAttackRange;

		public List<UILabel> effectDescription;

		public List<UILabel> effectActive;

		public void Set(string commanderId, int slotType, string weaponId = "")
		{
			RoCommander roCommander = base.localUser.FindCommander(commanderId);
			if (string.IsNullOrEmpty(weaponId))
			{
				string weaponSkillId = roCommander.GetWeaponSkillId(slotType);
				if (base.regulation.skillDtbl.ContainsKey(weaponSkillId))
				{
					SkillDataRow skillDataRow = base.regulation.skillDtbl[weaponSkillId];
					UISetter.SetSprite(skillAttackRange, skillDataRow.rangeicon);
					UISetter.SetSprite(icon, skillDataRow.thumbnail);
				}
				else
				{
					UISetter.SetActive(skillAttackRange, active: false);
					UISetter.SetActive(icon, active: false);
				}
				isEmptySkill();
				return;
			}
			RoWeapon roWeapon = base.localUser.FindWeapon(weaponId);
			if (roWeapon.data.privateWeapon != 0 && roWeapon.data.skillIdx != "0")
			{
				SkillDataRow skillDataRow2 = base.regulation.skillDtbl[roWeapon.data.skillIdx];
				UISetter.SetSprite(skillAttackRange, skillDataRow2.rangeicon);
				UISetter.SetSprite(icon, skillDataRow2.thumbnail);
				for (int i = 0; i < 2; i++)
				{
					UISetter.SetLabel(effectDescription[i], (!(roWeapon.data.explanation[i] != "0")) ? Localization.Get("70012") : Localization.Get(roWeapon.data.explanation[i]));
					WeaponDataRow data = roWeapon.data;
					ESkillTargetType eSkillTargetType = ((skillDataRow2.targetType == ESkillTargetType.Own && skillDataRow2.targetType == ESkillTargetType.Friend) ? ESkillTargetType.Friend : ESkillTargetType.Enemy);
					UISetter.SetActive(effectActive[i], data.targetType != eSkillTargetType && roWeapon.data.explanation[i] != "0");
					if (data.targetType != eSkillTargetType)
					{
						UISetter.SetLabel(effectActive[i], (data.targetType != ESkillTargetType.Friend) ? Localization.Get("70013") : Localization.Get("70014"));
					}
				}
				return;
			}
			string weaponOriginalSkillId = roCommander.GetWeaponOriginalSkillId(slotType);
			if (string.IsNullOrEmpty(weaponOriginalSkillId))
			{
				UISetter.SetActive(icon, active: false);
				UISetter.SetActive(skillAttackRange, active: false);
				isEmptySkill();
				return;
			}
			SkillDataRow skillDataRow3 = base.regulation.skillDtbl[weaponOriginalSkillId];
			UISetter.SetSprite(skillAttackRange, skillDataRow3.rangeicon);
			UISetter.SetSprite(icon, skillDataRow3.thumbnail);
			for (int j = 0; j < 2; j++)
			{
				UISetter.SetLabel(effectDescription[j], (!(roWeapon.data.explanation[j] != "0")) ? Localization.Get("70012") : Localization.Get(roWeapon.data.explanation[j]));
				WeaponDataRow data2 = roWeapon.data;
				ESkillTargetType eSkillTargetType2 = ((skillDataRow3.targetType == ESkillTargetType.Own && skillDataRow3.targetType == ESkillTargetType.Friend) ? ESkillTargetType.Friend : ESkillTargetType.Enemy);
				UISetter.SetActive(effectActive[j], data2.targetType != eSkillTargetType2 && roWeapon.data.explanation[j] != "0");
				if (data2.targetType != eSkillTargetType2)
				{
					UISetter.SetLabel(effectActive[j], (data2.targetType != ESkillTargetType.Friend) ? Localization.Get("70013") : Localization.Get("70014"));
				}
			}
		}

		private void isEmptySkill()
		{
			UISetter.SetLabel(effectDescription[0], Localization.Get("70012"));
			UISetter.SetLabel(effectDescription[1], Localization.Get("70012"));
			UISetter.SetActive(effectActive[0], active: false);
			UISetter.SetActive(effectActive[1], active: false);
		}
	}

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public UIFlipSwitch basicTab;

	public UIFlipSwitch privateTab;

	public UIDefaultListView weaponListView;

	public UILabel useCommander;

	public UILabel attack;

	public UILabel defence;

	public UILabel accuracy;

	public UILabel luck;

	public UILabel leadership;

	public UILabel criticalDamage;

	public UILabel attackIncrease;

	public UILabel defenceIncrease;

	public UILabel accuracyIncrease;

	public UILabel luckIncrease;

	public UILabel speedIncrease;

	public UILabel criticalDamageIncrease;

	public UISprite sortBtn;

	public GameObject sortRoot;

	public GUIAnimNGUI sortBtnAnim;

	private EWeaponListSortType sortType;

	public UILabel emptyLabel;

	public GameObject bottomRoot;

	public GameObject weaponListRoot;

	public GameObject detailRoot;

	public GameObject equipBtn;

	public GameObject compareBtn;

	public UIWeaponSkillInfo baseSkillInfo;

	public UIWeaponSkillInfo weaponSkillInfo;

	public UILabel compareBtnLabel;

	private TabType tabType;

	private int selectWeaponId;

	private string commanderId;

	private int slotType;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		OpenPopup();
	}

	public void Set(int slotType, int privateType, string commanderId)
	{
		sortType = EWeaponListSortType.Undefined;
		tabType = (TabType)privateType;
		this.slotType = slotType;
		this.commanderId = commanderId;
		UISetter.SetLabel(compareBtnLabel, Localization.Get("70022"));
		InitList();
		SetEmptyLabel();
	}

	public void InitList()
	{
		RoCommander roCommander = base.localUser.FindCommander(commanderId);
		List<RoWeapon> weaponList = base.localUser.GetWeaponList(slotType, (int)tabType, roCommander.unitId);
		weaponList.Sort(delegate(RoWeapon row, RoWeapon row1)
		{
			int num;
			int num2;
			if (sortType == EWeaponListSortType.TotalStatus)
			{
				num = row.GetTotalAddStatPoint();
				num2 = row1.GetTotalAddStatPoint();
			}
			else if (sortType == EWeaponListSortType.Attack)
			{
				num = row.GetAddStatPoint(EItemStatType.ATK);
				num2 = row1.GetAddStatPoint(EItemStatType.ATK);
			}
			else if (sortType == EWeaponListSortType.Defence)
			{
				num = row.GetAddStatPoint(EItemStatType.DEF);
				num2 = row1.GetAddStatPoint(EItemStatType.DEF);
			}
			else if (sortType == EWeaponListSortType.Accuracy)
			{
				num = row.GetAddStatPoint(EItemStatType.ACCUR);
				num2 = row1.GetAddStatPoint(EItemStatType.ACCUR);
			}
			else if (sortType == EWeaponListSortType.Luck)
			{
				num = row.GetAddStatPoint(EItemStatType.LUCK);
				num2 = row1.GetAddStatPoint(EItemStatType.LUCK);
			}
			else if (sortType == EWeaponListSortType.LeaderShip)
			{
				num = row.GetAddStatPoint(EItemStatType.MOB);
				num2 = row1.GetAddStatPoint(EItemStatType.MOB);
			}
			else if (sortType == EWeaponListSortType.CriticalDamage)
			{
				num = row.GetAddStatPoint(EItemStatType.CRITDMG);
				num2 = row1.GetAddStatPoint(EItemStatType.CRITDMG);
			}
			else
			{
				num2 = 0;
				num = 0;
			}
			if (num2 < num)
			{
				return -1;
			}
			if (num2 > num)
			{
				return 1;
			}
			if (int.Parse(row.data.idx) < int.Parse(row.data.idx))
			{
				return -1;
			}
			return (int.Parse(row.data.idx) > int.Parse(row.data.idx)) ? 1 : 0;
		});
		CommanderDataRow commanderDataRow = base.regulation.commanderDtbl[commanderId];
		weaponListView.InitWeapon(weaponList, commanderDataRow.unitId, "Weapon-");
		weaponListView.ResetPosition();
		if (weaponList.Count > 0)
		{
			selectWeaponId = int.Parse(weaponList[0].idx);
			weaponListView.SetSelection(selectWeaponId.ToString(), selected: true);
			ChangeStatusLabel();
		}
		UISetter.SetActive(bottomRoot, weaponList.Count > 0);
		UISetter.SetActive(emptyLabel, weaponList.Count == 0);
		UISetter.SetActive(compareBtn, weaponList.Count > 0);
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		switch (text)
		{
		case "Close":
			ClosePopup();
			return;
		case "BasicTab":
			tabType = TabType.Basic;
			sortType = EWeaponListSortType.Undefined;
			InitList();
			return;
		case "PrivateTab":
			tabType = TabType.Private;
			sortType = EWeaponListSortType.Undefined;
			InitList();
			return;
		}
		if (text.StartsWith("Weapon-"))
		{
			selectWeaponId = int.Parse(text.Substring(text.IndexOf("-") + 1));
			weaponListView.SetSelection(selectWeaponId.ToString(), selected: true);
			ChangeStatusLabel();
			return;
		}
		switch (text)
		{
		case "EquipWeaponBtn":
			if (!equipBtn.GetComponent<UIButton>().isGray)
			{
				EquipCheck();
			}
			return;
		case "CompareBtn":
			OpenSkillCompare(!detailRoot.activeSelf);
			return;
		case "SortBtn":
			OpenSlotTypeContents();
			return;
		}
		if (text.StartsWith("SortType-"))
		{
			sortType = (EWeaponListSortType)int.Parse(text.Substring(text.IndexOf("-") + 1));
			InitList();
		}
	}

	private void OpenSlotTypeContents()
	{
		UISetter.SetActive(sortRoot, !sortRoot.activeSelf);
		UISetter.SetSprite(sortBtn, (!sortRoot.activeSelf) ? "qm-up-button" : "qm-down-button");
		if (sortRoot.activeSelf)
		{
			sortBtnAnim.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
		}
		else
		{
			sortBtnAnim.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
		}
	}

	private void EquipCheck()
	{
		RoWeapon roWeapon = base.localUser.FindWeapon(selectWeaponId.ToString());
		if (roWeapon.currEquipCommanderId > 0)
		{
			UISimplePopup.CreateBool(localization: true, "1303", "70036", string.Empty, "1304", "1305").onClick = delegate(GameObject obj)
			{
				string text = obj.name;
				if (text == "OK")
				{
					base.network.RequestEquipWeapon(int.Parse(commanderId), selectWeaponId);
					ClosePopup();
				}
			};
		}
		else
		{
			base.network.RequestEquipWeapon(int.Parse(commanderId), selectWeaponId);
			ClosePopup();
		}
	}

	private void OpenSkillCompare(bool open)
	{
		UISetter.SetActive(weaponListRoot, !open);
		UISetter.SetActive(detailRoot, open);
		UISetter.SetLabel(compareBtnLabel, (!open) ? Localization.Get("70022") : Localization.Get("70035"));
		RoCommander roCommander = base.localUser.FindCommander(commanderId);
		RoWeapon roWeapon = roCommander.FindWeaponItem(slotType);
		baseSkillInfo.Set(commanderId, slotType, (roWeapon != null) ? roWeapon.idx : string.Empty);
		weaponSkillInfo.Set(commanderId, slotType, selectWeaponId.ToString());
	}

	private void ChangeStatusLabel()
	{
		RoWeapon roWeapon = base.localUser.FindWeapon(selectWeaponId.ToString());
		UISetter.SetActive(useCommander, roWeapon.currEquipCommanderId > 0);
		if (roWeapon.currEquipCommanderId > 0)
		{
			CommanderDataRow commanderDataRow = base.regulation.commanderDtbl[roWeapon.currEquipCommanderId.ToString()];
			UISetter.SetLabel(useCommander, Localization.Format("70024", Localization.Get(commanderDataRow.nickname)));
		}
		RoCommander roCommander = base.localUser.FindCommander(commanderId);
		UISetter.SetButtonGray(equipBtn, roCommander.FindWeaponItem(slotType) == null || roWeapon.idx != roCommander.FindWeaponItem(slotType).idx);
		UISetter.SetButtonGray(compareBtn, roCommander.FindWeaponItem(slotType) == null || roWeapon.idx != roCommander.FindWeaponItem(slotType).idx);
		UnitDataRow unitDataRow = RoCommander.InvokeLevel(roCommander.unitReg, roCommander.rank, roCommander.level, roCommander.cls, roCommander.currentCostume, roCommander.id, roCommander.favorRewardStep, roCommander.marry, roCommander.transcendence, roCommander.GetEquipItemList(), roCommander.completeSetItemEquip, roCommander.WeaponItem, isWeaponSet: false);
		Dictionary<int, RoWeapon> dictionary = new Dictionary<int, RoWeapon>();
		dictionary.Add(roWeapon.data.slotType, roWeapon);
		foreach (KeyValuePair<int, RoWeapon> item in roCommander.WeaponItem)
		{
			if (!dictionary.ContainsKey(item.Key))
			{
				dictionary.Add(item.Key, item.Value);
			}
		}
		UnitDataRow unitDataRow2 = RoCommander.InvokeLevel(roCommander.unitReg, roCommander.rank, roCommander.level, roCommander.cls, roCommander.currentCostume, roCommander.id, roCommander.favorRewardStep, roCommander.marry, roCommander.transcendence, roCommander.GetEquipItemList(), roCommander.completeSetItemEquip, dictionary, isWeaponSet: false);
		UISetter.SetLabel(attack, unitDataRow.attackDamage);
		UISetter.SetLabel(defence, unitDataRow.defense);
		UISetter.SetLabel(accuracy, unitDataRow.accuracy);
		UISetter.SetLabel(luck, unitDataRow.luck);
		UISetter.SetLabel(leadership, unitDataRow.speed);
		UISetter.SetLabel(criticalDamage, unitDataRow.criticalDamageBonus);
		int num = unitDataRow2.attackDamage - unitDataRow.attackDamage;
		int num2 = unitDataRow2.defense - unitDataRow.defense;
		int num3 = unitDataRow2.accuracy - unitDataRow.accuracy;
		int num4 = unitDataRow2.luck - unitDataRow.luck;
		int num5 = unitDataRow2.speed - unitDataRow.speed;
		int num6 = unitDataRow2.criticalDamageBonus - unitDataRow.criticalDamageBonus;
		string arg = ((num >= 0) ? "+" : string.Empty);
		string arg2 = ((num2 >= 0) ? "+" : string.Empty);
		string arg3 = ((num3 >= 0) ? "+" : string.Empty);
		string arg4 = ((num4 >= 0) ? "+" : string.Empty);
		string arg5 = ((num5 >= 0) ? "+" : string.Empty);
		string arg6 = ((num6 >= 0) ? "+" : string.Empty);
		UISetter.SetColor(attackIncrease, (num >= 0) ? new Color(0.41568628f, 0.6509804f, 0.20784314f) : new Color(0.972549f, 0.1254902f, 8f / 51f));
		UISetter.SetColor(defenceIncrease, (num2 >= 0) ? new Color(0.41568628f, 0.6509804f, 0.20784314f) : new Color(0.972549f, 0.1254902f, 8f / 51f));
		UISetter.SetColor(accuracyIncrease, (num3 >= 0) ? new Color(0.41568628f, 0.6509804f, 0.20784314f) : new Color(0.972549f, 0.1254902f, 8f / 51f));
		UISetter.SetColor(luckIncrease, (num4 >= 0) ? new Color(0.41568628f, 0.6509804f, 0.20784314f) : new Color(0.972549f, 0.1254902f, 8f / 51f));
		UISetter.SetColor(speedIncrease, (num5 >= 0) ? new Color(0.41568628f, 0.6509804f, 0.20784314f) : new Color(0.972549f, 0.1254902f, 8f / 51f));
		UISetter.SetColor(criticalDamageIncrease, (num6 >= 0) ? new Color(0.41568628f, 0.6509804f, 0.20784314f) : new Color(0.972549f, 0.1254902f, 8f / 51f));
		UISetter.SetLabel(attackIncrease, $"({arg}{num})");
		UISetter.SetLabel(defenceIncrease, $"({arg2}{num2})");
		UISetter.SetLabel(accuracyIncrease, $"({arg3}{num3})");
		UISetter.SetLabel(luckIncrease, $"({arg4}{num4})");
		UISetter.SetLabel(speedIncrease, $"({arg5}{num5})");
		UISetter.SetLabel(criticalDamageIncrease, $"({arg6}{num6})");
	}

	private void SetEmptyLabel()
	{
		string key = string.Empty;
		switch (slotType)
		{
		case 2:
			key = "70903";
			break;
		case 3:
			key = "70904";
			break;
		case 4:
			key = "70905";
			break;
		case 1:
			key = "70906";
			break;
		case 5:
			key = "70907";
			break;
		}
		UISetter.SetLabel(emptyLabel, Localization.Get(key));
	}

	public void OpenPopup()
	{
		base.Open();
		AnimBG.Reset();
		AnimBlock.Reset();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
