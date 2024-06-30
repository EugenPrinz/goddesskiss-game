using Shared.Regulation;
using UnityEngine;

public class UIUnit : UIItemBase
{
	public static readonly string thumbnailBackIdPostfix = "_Front";

	public static readonly string thumbnailFrontIdPostfix = "_Front";

	public static readonly string thumbnailBackgroundPrefix = "ma_bg_slot_";

	public static readonly string aliveIdPostfix = "alliance_img_light";

	public static readonly string dieIdPostfix = "alliance_img_die";

	public UILabel nickname;

	public UISprite thumbnail;

	public UISprite thumbnailBack;

	public UISprite smallUnitThumbnail;

	public UILabel level;

	public UILabel levelRoot;

	public UILabel dotLevel;

	public UILabel spaceLevel;

	public UILabel levelNickname;

	public UILabel jobLabel;

	public UITimer timer;

	public UIGrid rankGrid;

	public UISprite branchSlotBG;

	public UISprite jobIcon;

	public UISprite backGround;

	public UIStatus status;

	public UILabel leadership;

	public UILabel stateLabel;

	public GameObject empty;

	public UISprite cardBack;

	public UISprite cardOutline_1;

	public UISprite cardOutline_2;

	public UISprite cardOutline_3;

	public UISprite cardOutline_4;

	public UISprite cardCornerUp_1;

	public UISprite cardCornerUp_2;

	public UISprite cardCornerDown_1;

	public UISprite cardCornerDown_2;

	public UISprite cardOutlineTop_1;

	public UISprite cardOutlineTop_2;

	public UISprite cardOutlineBottom_1;

	public UISprite cardOutlineBottom_2;

	public UIProgressBar remainHealthBar;

	public UIProgressBar remainHealthPercentBar;

	public UILabel remainHealthPercentLabel;

	public UISprite hpStausSprite;

	public GameObject selectedRoot;

	public GameObject lockRoot;

	public GameObject trainingRoot;

	public UILabel position;

	public GameObject positionRoot;

	public GameObject validSlotRoot;

	public UISprite upgradeBadge;

	public RoUnit target;

	[SerializeField]
	private GameObject CharacterTypeIcon;

	public GameObject helperIcon;

	[SerializeField]
	private GameObject PositionPlateObject;

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(selectedRoot, selected);
	}

	public override void SetLock(bool locked)
	{
		UISetter.SetActive(lockRoot, locked);
	}

	public void Set(string id)
	{
		UnitDataRow unitDataRow = RemoteObjectManager.instance.regulation.unitDtbl[id];
		UISetter.SetSprite(thumbnail, unitDataRow.resourceName + thumbnailFrontIdPostfix);
	}

	public void Set(RoUnit unit)
	{
		if (unit == null)
		{
			UISetter.SetActive(positionRoot, active: true);
			UISetter.SetActive(empty, active: true);
			UISetter.SetActive(validSlotRoot, active: false);
			return;
		}
		target = unit;
		UISetter.SetActive(level, active: true);
		UISetter.SetActive(dotLevel, active: true);
		UISetter.SetActive(spaceLevel, active: true);
		UISetter.SetActive(levelRoot, active: true);
		UISetter.SetActive(positionRoot, active: false);
		if (nickname != null)
		{
			if (GetClassGroup(unit.cls) == 1)
			{
				nickname.gradientTop = new Color(1f, 74f / 85f, 74f / 85f);
				nickname.gradientBottom = new Color(1f, 74f / 85f, 74f / 85f);
			}
			else if (GetClassGroup(unit.cls) == 2)
			{
				nickname.gradientTop = new Color(0.9843137f, 0.99607843f, 0.12156863f);
				nickname.gradientBottom = new Color(0.35686275f, 74f / 85f, 0.015686275f);
			}
			else if (GetClassGroup(unit.cls) == 3)
			{
				nickname.gradientTop = new Color(0.12252964f, 0.99215686f, 0.99607843f);
				nickname.gradientBottom = new Color(1f / 51f, 0.6509804f, 0.8745098f);
			}
			else if (GetClassGroup(unit.cls) == 4)
			{
				nickname.gradientTop = new Color(84f / 85f, 0.4745098f, 1f);
				nickname.gradientBottom = new Color(0.78039217f, 0.2509804f, 1f);
			}
			else if (GetClassGroup(unit.cls) == 5)
			{
				nickname.gradientTop = new Color(1f, 0.9843137f, 0f);
				nickname.gradientBottom = new Color(1f, 32f / 51f, 0f);
			}
			else
			{
				nickname.gradientTop = new Color(1f, 39f / 85f, 14f / 85f);
				nickname.gradientBottom = new Color(0.95686275f, 1f / 15f, 1f / 15f);
			}
		}
		string text = Localization.Get(unit.unitReg.nameKey);
		if (GetSubClass(unit.cls) > 1)
		{
			text = string.Format(text + " +{0}", GetSubClass(unit.cls) - 1);
		}
		UISetter.SetLabel(nickname, text);
		UISetter.SetSprite(thumbnail, unit.currLevelReg.resourceName + thumbnailFrontIdPostfix);
		UISetter.SetSprite(thumbnailBack, unit.currLevelReg.resourceName + thumbnailBackIdPostfix);
		SetCardFrame(GetClassGroup(unit.cls), GetSubClass(unit.cls));
		UISetter.SetLabel(level, unit.level);
		UISetter.SetLabel(dotLevel, Localization.Format("1021", unit.level));
		UISetter.SetLabel(spaceLevel, Localization.Format("1021", unit.level));
		UISetter.SetLabel(levelNickname, Localization.Format("1021", string.Concat(unit.level, "  ", Localization.Get(unit.unitReg.nameKey))));
		UISetter.SetSprite(branchSlotBG, "ma_bg_icon_" + GetClassGroup(unit.cls));
		UISetter.SetSprite(jobIcon, "com_icon_" + unit.unitReg.job.ToString().ToLower());
		UISetter.SetActive(lockRoot, !unit.unlocked);
		if (trainingRoot != null)
		{
			UISetter.SetActive(trainingRoot, unit.trainingTime.IsProgress());
			UISetter.SetTimer(timer, unit.trainingTime);
			if (unit.trainingTime.IsProgress())
			{
				UISetter.SetLabel(stateLabel, Localization.Get("11005"));
			}
		}
		UISetter.SetStatus(status, unit);
		UISetter.SetLabel(leadership, unit.currLevelReg.leadership);
	}

	public void Set(RoTroop.Slot slot)
	{
		target = null;
		UISetter.SetActive(CharacterTypeIcon, active: false);
		UISetter.SetActive(helperIcon, active: false);
		UISetter.SetActive(positionRoot, active: true);
		UISetter.SetActive(empty, active: true);
		UISetter.SetActive(validSlotRoot, active: false);
		if (slot != null)
		{
			bool flag = !slot.IsEmptyUnitId();
			UISetter.SetActive(validSlotRoot, flag);
			if (flag)
			{
				UISetter.SetActive(positionRoot, active: false);
				UISetter.SetActive(empty, active: false);
				UISetter.SetActive(validSlotRoot, active: true);
				RoUnit roUnit = null;
				roUnit = ((slot.charType == ECharacterType.Annihilation) ? RoUnit.Create(slot.unitId, slot.unitLevel, slot.unitRank, slot.unitCls, slot.unitCostume, slot.commanderId, slot.favorRewardStep, slot.marry, slot.transcendence, EBattleType.Annihilation) : ((slot.charType != ECharacterType.Enemy) ? RoUnit.Create(slot.unitId, slot.unitLevel, slot.unitRank, slot.unitCls, slot.unitCostume, slot.commanderId, slot.favorRewardStep, slot.marry, slot.transcendence) : RoUnit.Create(slot.unitId, slot.unitLevel, slot.unitRank, slot.unitCls, slot.unitCostume, slot.commanderId, slot.favorRewardStep, slot.marry, slot.transcendence, EBattleType.Plunder)));
				Set(roUnit);
				UISetter.SetLabel(position, slot.position + 1);
				UISetter.SetProgress(remainHealthBar, (float)slot.health / (float)roUnit.currLevelReg.maxHealth);
				UISetter.SetProgress(remainHealthPercentBar, (float)slot.health / 100f);
				UISetter.SetLabel(remainHealthPercentLabel, $"{slot.health}%");
				UISetter.SetSprite(hpStausSprite, (!((float)slot.health > 0f)) ? dieIdPostfix : aliveIdPostfix);
				UISetter.SetActive(CharacterTypeIcon, (slot.charType == ECharacterType.Mercenary || slot.charType == ECharacterType.NPCMercenary) ? true : false);
				UISetter.SetActive(helperIcon, slot.charType == ECharacterType.Helper);
			}
		}
	}

	private void SetCardFrame(int classGroup, int subClass)
	{
		UISetter.SetSprite(cardOutline_1, "ig-character-bg-line-0" + classGroup);
		UISetter.SetActive(cardOutlineTop_1, subClass == 5);
		UISetter.SetActive(cardOutlineTop_2, subClass == 5);
		UISetter.SetActive(cardOutlineBottom_1, subClass == 5);
		UISetter.SetActive(cardOutlineBottom_2, subClass == 5);
		if (subClass == 5)
		{
			UISetter.SetSprite(cardCornerUp_1, "ig-character-frame" + (classGroup - 1) + "-4-up");
			UISetter.SetSprite(cardCornerUp_2, "ig-character-frame" + (classGroup - 1) + "-4-up");
			UISetter.SetSprite(cardCornerDown_1, "ig-character-frame" + (classGroup - 1) + "-4-down");
			UISetter.SetSprite(cardCornerDown_2, "ig-character-frame" + (classGroup - 1) + "-4-down");
			UISetter.SetSprite(cardOutlineTop_1, "ig-character-frame" + (classGroup - 1) + "-5-up");
			UISetter.SetSprite(cardOutlineTop_2, "ig-character-frame" + (classGroup - 1) + "-5-up");
			return;
		}
		UISetter.SetSprite(cardCornerUp_1, "ig-character-frame" + (classGroup - 1) + "-" + (subClass - 1) + "-up");
		UISetter.SetSprite(cardCornerUp_2, "ig-character-frame" + (classGroup - 1) + "-" + (subClass - 1) + "-up");
		UISetter.SetSprite(cardCornerDown_1, "ig-character-frame" + (classGroup - 1) + "-" + (subClass - 1) + "-down");
		UISetter.SetSprite(cardCornerDown_2, "ig-character-frame" + (classGroup - 1) + "-" + (subClass - 1) + "-down");
	}

	public static int GetClassGroup(int tier)
	{
		int num = 1;
		switch (tier)
		{
		case 1:
			return 1;
		case 2:
		case 3:
		case 4:
			return 2;
		default:
			if (tier >= 5 && tier <= 8)
			{
				return 3;
			}
			if (tier >= 9 && tier <= 13)
			{
				return 4;
			}
			if (tier >= 14 && tier <= 18)
			{
				return 5;
			}
			return 6;
		}
	}

	public static int GetSubClass(int tier)
	{
		int num = 1;
		switch (tier)
		{
		case 1:
			return tier;
		case 2:
		case 3:
		case 4:
			return tier - 1;
		default:
			if (tier >= 5 && tier <= 8)
			{
				return tier - 4;
			}
			if (tier >= 9 && tier <= 13)
			{
				return tier - 8;
			}
			if (tier >= 14 && tier <= 18)
			{
				return tier - 13;
			}
			return tier - 18;
		}
	}

	public void SetPositionPlate(bool isActive)
	{
		if (PositionPlateObject != null)
		{
			UISetter.SetActive(PositionPlateObject, isActive);
		}
	}
}
