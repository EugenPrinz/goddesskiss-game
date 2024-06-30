using System;
using Shared;
using Shared.Regulation;
using UnityEngine;

public class UIBattleStatisticRecord : UIItemBase
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

	public static readonly string commanderSlotPrefix = "ig-character-bg-";

	public CommanderView commanderView;

	[SerializeField]
	private GameObject mercenaryIcon;

	public GameObject helperRoot;

	protected AudioSource expUpSound;

	[SerializeField]
	private UILabel commanderName;

	[SerializeField]
	private UILabel record;

	[SerializeField]
	private UILabel topRankTitle;

	private void OnDisable()
	{
		StopAllCoroutines();
		if (expUpSound != null)
		{
			SoundManager.StopSFXObject(expUpSound);
			expUpSound = null;
		}
	}

	public void TopRankSet(Troop.Slot roTroopSlot, EBattleRecordType type)
	{
		string text = string.Empty;
		switch (type)
		{
		case EBattleRecordType.Attack:
			text = Localization.Get("301005");
			break;
		case EBattleRecordType.Avoid:
			text = Localization.Get("301006");
			break;
		case EBattleRecordType.Recover:
			text = Localization.Get("301007");
			break;
		}
		UISetter.SetLabel(topRankTitle, text);
		Set(roTroopSlot, type);
	}

	public void Set(Troop.Slot TroopSlot, EBattleRecordType type)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		Troop troop = UIManager.instance.battle.Simulator.record.result.lhsTroops[0];
		CommanderDataRow commanderDataRow = regulation.commanderDtbl[TroopSlot.cid];
		RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommander(TroopSlot.cid);
		if (roCommander != null && roCommander.isBasicCostume)
		{
			UISetter.SetSprite(commanderView.imgCommander, commanderDataRow.resourceId + "_" + roCommander.currentViewCostume);
		}
		else
		{
			UISetter.SetSprite(commanderView.imgCommander, commanderDataRow.resourceId + "_" + RemoteObjectManager.instance.regulation.GetCostumeName(TroopSlot.costume));
		}
		UISetter.SetRank(commanderView.gridRank, TroopSlot.rank);
		UISetter.SetLabel(commanderView.lbLevel, string.Format(Localization.Get("1021"), TroopSlot.level));
		UISetter.SetSprite(commanderView.thumbBackground, (TroopSlot.marry != 1) ? "ig-character-bg" : "ig-character-bg2");
		commanderView.SetCardFrame(UIUnit.GetClassGroup(TroopSlot.cls), UIUnit.GetSubClass(TroopSlot.cls));
		UnitDataRow unitDataRow = regulation.unitDtbl[TroopSlot.id];
		UISetter.SetSprite(commanderView.imgUnit, unitDataRow.unitSmallIconName);
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		RoCommander roCommander2 = localUser.FindCommander(TroopSlot.cid);
		UISetter.SetLabel(commanderName, roCommander2.nickname);
		RoUnit roUnit = RoUnit.Create(TroopSlot.id, TroopSlot.level, TroopSlot.rank, TroopSlot.cls, TroopSlot.costume, TroopSlot.cid, TroopSlot.favorRewardStep, TroopSlot.marry, TroopSlot.transcendence);
		switch ((ECharacterType)TroopSlot.charType)
		{
		case ECharacterType.Mercenary:
		case ECharacterType.SuperMercenary:
			UISetter.SetActive(mercenaryIcon, active: true);
			UISetter.SetActive(helperRoot, active: false);
			break;
		case ECharacterType.Helper:
			UISetter.SetActive(helperRoot, active: true);
			UISetter.SetActive(mercenaryIcon, active: false);
			break;
		case ECharacterType.Commander:
			UISetter.SetActive(helperRoot, active: false);
			UISetter.SetActive(mercenaryIcon, active: false);
			break;
		default:
			UISetter.SetActive(helperRoot, active: false);
			UISetter.SetActive(mercenaryIcon, active: false);
			break;
		}
		float num = 0f;
		switch (type)
		{
		case EBattleRecordType.Attack:
			num = ((troop._statsAttack != 0) ? ((float)TroopSlot.statsAttack / (float)troop._statsAttack) : 0f);
			UISetter.SetLabel(record, $"{TroopSlot.statsAttack}/{num * 100f:f2}%");
			break;
		case EBattleRecordType.Avoid:
			num = ((troop._statsDefense != 0) ? ((float)TroopSlot.statsDefense / (float)troop._statsDefense) : 0f);
			UISetter.SetLabel(record, $"{TroopSlot.statsDefense}/{num * 100f:f2}%");
			break;
		case EBattleRecordType.Recover:
			num = ((troop._statsHealing != 0) ? ((float)TroopSlot.statsHealing / (float)troop._statsHealing) : 0f);
			UISetter.SetLabel(record, $"{TroopSlot.statsHealing}/{num * 100f:f2}%");
			break;
		}
	}
}
