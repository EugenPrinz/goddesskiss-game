using System;
using System.Collections;
using System.Collections.Generic;
using Shared;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

public class UIBattleUnitStatus : UIItemBase
{
	[Serializable]
	public class UIView
	{
		public GameObject root;

		public virtual void SetEnable(bool enable)
		{
			UISetter.SetActive(root, enable);
		}
	}

	[Serializable]
	public class CommanderView : UIView
	{
		public UISprite imgSlot;

		public UISprite imgCommander;

		public UISprite imgUnit;

		public UILabel lbLevel;

		public UIGrid gridRank;

		public UISprite cardOutline_1;

		public UISprite cardCornerUp_1;

		public UISprite cardCornerUp_2;

		public UISprite cardCornerDown_1;

		public UISprite cardCornerDown_2;

		public UISprite cardOutlineTop_1;

		public UISprite cardOutlineTop_2;

		public UISprite cardOutlineBottom_1;

		public UISprite cardOutlineBottom_2;

		public UISprite thumbBackground;

		public void SetCardFrame(int classGroup, int subClass)
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
	}

	[Serializable]
	public class HpSpStatusView : UIView
	{
		public UIProgressBar hpProgress;

		public UIProgressBar spProgress;
	}

	[Serializable]
	public class ExpStatusView : UIView
	{
		public UIProgressBar expProgress;

		public UILabel lbExp;

		public GameObject onLevelup;
	}

	public static readonly string commanderSlotPrefix = "ig-character-bg-";

	public CommanderView commanderView;

	public HpSpStatusView statusView;

	public ExpStatusView expView;

	[SerializeField]
	private GameObject mercenaryIcon;

	public GameObject helperRoot;

	public GameObject conquestDieRoot;

	protected AudioSource expUpSound;

	private void OnDisable()
	{
		StopAllCoroutines();
		if (expUpSound != null)
		{
			SoundManager.StopSFXObject(expUpSound);
			expUpSound = null;
		}
	}

	public void Set(Troop.Slot slot)
	{
		CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[slot.cid];
		UISetter.SetSprite(commanderView.imgCommander, commanderDataRow.thumbnailId);
		UISetter.SetRank(commanderView.gridRank, slot.rank);
		UISetter.SetLabel(commanderView.lbLevel, string.Format(Localization.Get("1021"), slot.level));
		UISetter.SetSprite(commanderView.thumbBackground, (slot.marry != 1) ? "ig-character-bg" : "ig-character-bg2");
		commanderView.SetCardFrame(UIUnit.GetClassGroup(slot.cls), UIUnit.GetSubClass(slot.cls));
		UnitDataRow unitDataRow = RemoteObjectManager.instance.regulation.unitDtbl[slot.id];
		UISetter.SetSprite(commanderView.imgUnit, unitDataRow.unitSmallIconName);
		UISetter.SetProgress(expView.expProgress, 0f);
		UISetter.SetLabel(expView.lbExp, 0);
		UISetter.SetActive(expView.onLevelup, active: false);
	}

	public void Set(RoTroop.Slot roTroopSlot)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		CommanderDataRow commanderDataRow = regulation.commanderDtbl[roTroopSlot.commanderId];
		RoCommander roCommander = null;
		roCommander = ((roTroopSlot.charType != ECharacterType.Mercenary && roTroopSlot.charType != ECharacterType.SuperMercenary && roTroopSlot.charType != ECharacterType.NPCMercenary && roTroopSlot.charType != ECharacterType.SuperNPCMercenary) ? RemoteObjectManager.instance.localUser.FindCommander(roTroopSlot.commanderId) : RemoteObjectManager.instance.localUser.FindMercenaryCommander(roTroopSlot.commanderId, roTroopSlot.userIdx, roTroopSlot.charType));
		if (roCommander != null)
		{
			if (roCommander.isBasicCostume)
			{
				UISetter.SetSprite(commanderView.imgCommander, commanderDataRow.resourceId + "_" + roCommander.currentViewCostume);
			}
			else
			{
				UISetter.SetSprite(commanderView.imgCommander, commanderDataRow.resourceId + "_" + RemoteObjectManager.instance.regulation.GetCostumeName(roTroopSlot.unitCostume));
			}
		}
		else
		{
			UISetter.SetSprite(commanderView.imgCommander, commanderDataRow.resourceId + "_" + RemoteObjectManager.instance.regulation.GetCostumeName(roTroopSlot.unitCostume));
		}
		UISetter.SetRank(commanderView.gridRank, roTroopSlot.unitRank);
		UISetter.SetLabel(commanderView.lbLevel, string.Format(Localization.Get("1021"), roTroopSlot.unitLevel));
		UISetter.SetSprite(commanderView.thumbBackground, (roTroopSlot.marry != 1) ? "ig-character-bg" : "ig-character-bg2");
		commanderView.SetCardFrame(UIUnit.GetClassGroup(roTroopSlot.unitCls), UIUnit.GetSubClass(roTroopSlot.unitCls));
		UnitDataRow unitDataRow = regulation.unitDtbl[roTroopSlot.unitId];
		UISetter.SetSprite(commanderView.imgUnit, unitDataRow.unitSmallIconName);
		UISetter.SetActive(expView.onLevelup, active: false);
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		RoUnit roUnit = RoUnit.Create(roTroopSlot.unitId, roTroopSlot.unitLevel, roTroopSlot.unitRank, roTroopSlot.unitCls, roTroopSlot.unitCostume, roTroopSlot.commanderId, roTroopSlot.favorRewardStep, roTroopSlot.marry, roTroopSlot.transcendence);
		switch (roTroopSlot.charType)
		{
		case ECharacterType.Mercenary:
		case ECharacterType.SuperMercenary:
			UISetter.SetActive(mercenaryIcon, active: true);
			UISetter.SetActive(helperRoot, active: false);
			UISetter.SetLabel(expView.lbExp, 0);
			UISetter.SetProgress(expView.expProgress, 0f);
			UISetter.SetProgress(statusView.hpProgress, (float)roTroopSlot.health / (float)roUnit.currLevelReg.maxHealth);
			break;
		case ECharacterType.Helper:
		case ECharacterType.NPCMercenary:
		case ECharacterType.SuperNPCMercenary:
			UISetter.SetActive(helperRoot, active: true);
			UISetter.SetActive(mercenaryIcon, active: false);
			UISetter.SetLabel(expView.lbExp, 0);
			UISetter.SetProgress(expView.expProgress, 0f);
			UISetter.SetProgress(statusView.hpProgress, (float)roTroopSlot.health / (float)roUnit.currLevelReg.maxHealth);
			break;
		case ECharacterType.Commander:
			UISetter.SetActive(helperRoot, active: false);
			UISetter.SetActive(mercenaryIcon, active: false);
			UISetter.SetLabel(expView.lbExp, roCommander.exp);
			UISetter.SetProgress(expView.expProgress, (float)(int)roCommander.exp / (float)(int)roCommander.maxExp);
			UISetter.SetProgress(statusView.hpProgress, (float)roTroopSlot.health / (float)roUnit.currLevelReg.maxHealth);
			break;
		default:
			UISetter.SetActive(helperRoot, active: false);
			UISetter.SetActive(mercenaryIcon, active: false);
			break;
		}
	}

	public void Set(Unit unit)
	{
		UISetter.SetProgress(statusView.hpProgress, (float)unit.health / (float)unit.maxHealth);
		UISetter.SetProgress(statusView.spProgress, 0f);
		UISetter.SetActive(statusView.hpProgress.gameObject, active: true);
		if (unit._cdri >= 0)
		{
			CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[unit._cdri];
			RoCommander roCommander = null;
			if (unit._charType != ECharacterType.Mercenary && unit._charType != ECharacterType.SuperMercenary && unit._charType != ECharacterType.NPCMercenary && unit._charType != ECharacterType.SuperNPCMercenary)
			{
				roCommander = RemoteObjectManager.instance.localUser.FindCommander(unit.cid);
			}
			if (roCommander != null)
			{
				if (roCommander.isBasicCostume)
				{
					UISetter.SetSprite(commanderView.imgCommander, commanderDataRow.resourceId + "_" + roCommander.currentViewCostume);
				}
				else
				{
					UISetter.SetSprite(commanderView.imgCommander, commanderDataRow.resourceId + "_" + RemoteObjectManager.instance.regulation.GetCostumeName(unit.ctid));
				}
			}
			else
			{
				UISetter.SetSprite(commanderView.imgCommander, commanderDataRow.resourceId + "_" + RemoteObjectManager.instance.regulation.GetCostumeName(unit.ctid));
			}
		}
		UISetter.SetRank(commanderView.gridRank, unit._rank);
		UISetter.SetLabel(commanderView.lbLevel, string.Format(Localization.Get("1021"), unit.level));
		UISetter.SetSprite(commanderView.thumbBackground, (unit.marry != 1) ? "ig-character-bg" : "ig-character-bg2");
		commanderView.SetCardFrame(UIUnit.GetClassGroup(unit._cls), UIUnit.GetSubClass(unit._cls));
		UnitDataRow unitDataRow = RemoteObjectManager.instance.regulation.unitDtbl[unit.dri];
		UISetter.SetSprite(commanderView.imgUnit, unitDataRow.unitSmallIconName);
		switch (unit._charType)
		{
		case ECharacterType.Mercenary:
		case ECharacterType.SuperMercenary:
			UISetter.SetActive(helperRoot, active: false);
			UISetter.SetActive(mercenaryIcon, active: true);
			break;
		case ECharacterType.Helper:
		case ECharacterType.NPCMercenary:
		case ECharacterType.SuperNPCMercenary:
			UISetter.SetActive(helperRoot, active: true);
			UISetter.SetActive(mercenaryIcon, active: false);
			break;
		default:
			UISetter.SetActive(helperRoot, active: false);
			UISetter.SetActive(mercenaryIcon, active: false);
			break;
		}
	}

	public void Set(RoCommander roCommander, int currentHp, int maxHp)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetSprite(commanderView.imgCommander, roCommander.reg.resourceId + "_" + RemoteObjectManager.instance.regulation.GetCostumeName(roCommander.currentCostume));
		UISetter.SetRank(commanderView.gridRank, roCommander.rank);
		UISetter.SetLabel(commanderView.lbLevel, string.Format(Localization.Get("1021"), roCommander.level));
		UISetter.SetSprite(commanderView.thumbBackground, ((int)roCommander.marry != 1) ? "ig-character-bg" : "ig-character-bg2");
		commanderView.SetCardFrame(UIUnit.GetClassGroup(roCommander.cls), UIUnit.GetSubClass(roCommander.cls));
		UnitDataRow unitDataRow = regulation.unitDtbl[roCommander.reg.unitId];
		UISetter.SetSprite(commanderView.imgUnit, unitDataRow.unitSmallIconName);
		UISetter.SetProgress(statusView.hpProgress, (float)currentHp / (float)maxHp);
		UISetter.SetActive(conquestDieRoot, currentHp == 0);
	}

	public void SetUpdateData(Protocols.UserInformationResponse.Commander result)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		RoCommander roCommander = localUser.FindCommander(result.id);
		RoCommander roCommander2 = RoCommander.Create(roCommander.id, roCommander.level, 1, 1, 0, 0, 0, new List<int>());
		roCommander2.aExp = roCommander.aExp;
		StopAllCoroutines();
		StartCoroutine(expAnimation(roCommander2, result.level, result.exp));
		roCommander.level = result.level;
		roCommander.aExp = result.exp;
	}

	private IEnumerator expAnimation(RoCommander commander, int resultLevel, int resultExp)
	{
		if ((int)commander.aExp < resultExp)
		{
			expUpSound = SoundManager.PlaySFX("SE_GageUp_001", looping: true);
		}
		bool levelUp = (int)commander.level != resultLevel;
		int level = commander.level;
		while ((int)commander.aExp < resultExp)
		{
			++commander.aExp;
			UISetter.SetLabel(expView.lbExp, commander.exp);
			UISetter.SetProgress(expView.expProgress, (float)(int)commander.exp / (float)(int)commander.maxExp);
			UISetter.SetLabel(commanderView.lbLevel, string.Format(Localization.Get("1021"), commander.level));
			if (levelUp && (int)commander.level != level)
			{
				UISetter.SetActive(expView.onLevelup, active: true);
			}
			yield return null;
		}
		if (expUpSound != null)
		{
			SoundManager.StopSFXObject(expUpSound);
			expUpSound = null;
		}
		yield return null;
	}

	public void StartConquestBattleResultHpAnimation(int resultHp, int maxHp)
	{
		ConquestBattleResultHpAnimation(resultHp, maxHp);
	}

	public void ConquestBattleResultHpAnimation(int resultHp, int maxHp)
	{
		if (statusView.hpProgress.value != 0f)
		{
			iTween.ValueTo(base.gameObject, iTween.Hash("from", statusView.hpProgress.value, "to", (float)resultHp / (float)maxHp, "onupdatetarget", base.gameObject, "onupdate", "tweenOnUpdateCallBack", "time", 0.5f, "easetype", iTween.EaseType.linear));
			UISetter.SetActive(conquestDieRoot, resultHp == 0);
		}
	}

	private void tweenOnUpdateCallBack(float newValue)
	{
		UISetter.SetProgress(statusView.hpProgress, newValue);
	}

	public void Skip(int resultHp, int maxHp)
	{
		if (statusView.hpProgress.value != 0f)
		{
			UISetter.SetProgress(statusView.hpProgress, (float)resultHp / (float)maxHp);
			UISetter.SetActive(conquestDieRoot, resultHp == 0);
		}
	}
}
