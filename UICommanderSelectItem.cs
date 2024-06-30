using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICommanderSelectItem : UICommander
{
	public GameObject useRoot;

	public GameObject conquestRoot;

	public UILabel conquestLabel;

	public GameObject addButton;

	public GameObject removeButton;

	public GameObject dieButton;

	public UILabel dieLabel;

	public GameObject notEnoughClassRoot;

	public GameObject incompatible;

	public GameObject commanderHpRoot;

	public UIProgressBar commanderHpProgress;

	public UILabel commanderHpLabel;

	public GameObject commanderSpRoot;

	public UIProgressBar commanderSpProgress;

	public UILabel commanderSpLabel;

	public GameObject mercenaryRoot;

	public GameObject npcRoot;

	[SerializeField]
	private UILabel reason;

	[SerializeField]
	private GameObject notAvailable;

	private RoCommander commander;

	public GameObject waveObj;

	public UILabel wave;

	public UILabel slotNum;

	public MercenaryChecker checker;

	public void Set(RoCommander _commander, List<RoTroop> troops, BattleData battleData, ECharacterType charType = ECharacterType.None, int idx = -1)
	{
		int troopIndexContainCommander = RoTroop.GetTroopIndexContainCommander(troops, _commander.id);
		if (troopIndexContainCommander >= 0)
		{
			Set(_commander, troops[troopIndexContainCommander], battleData, charType, idx);
			if (troops.Count > 1)
			{
				UISetter.SetActive(waveObj, active: true);
				UISetter.SetLabel(wave, Localization.Format("5628", troopIndexContainCommander + 1));
			}
			else
			{
				UISetter.SetActive(waveObj, active: false);
			}
		}
		else
		{
			Set(_commander, troops[0], battleData, charType, idx);
			UISetter.SetActive(waveObj, active: false);
		}
	}

	public void Set(RoCommander _commander, RoTroop troop, BattleData battleData, ECharacterType charType = ECharacterType.None, int idx = -1)
	{
		UISetter.SetActive(empty, _commander == null);
		UISetter.SetActive(validSlotRoot, _commander != null);
		if (_commander == null)
		{
			return;
		}
		EBattleType eBattleType = battleData?.type ?? EBattleType.Undefined;
		commander = _commander;
		Set(commander);
		UISetter.SetActive(npcRoot, charType == ECharacterType.Helper || charType == ECharacterType.NPCMercenary);
		UISetter.SetActive(mercenaryRoot, charType == ECharacterType.Mercenary);
		if (charType == ECharacterType.Mercenary)
		{
			UISetter.SetGameObjectName(addButton, $"EngageAddButton-{idx}");
		}
		else
		{
			UISetter.SetGameObjectName(addButton, $"AddButton-{commander.id}");
		}
		UISetter.SetGameObjectName(removeButton, $"RemoveButton-{commander.id}");
		UISetter.SetGameObjectName(validSlotRoot, $"CommanderSelect-{commander.id}");
		UISetter.SetGameObjectName(dieButton, $"DieButton-{commander.id}");
		UISetter.SetActive(incompatible, active: false);
		UISetter.SetActive(commanderHpRoot, eBattleType == EBattleType.Annihilation);
		UISetter.SetActive(commanderSpRoot, eBattleType == EBattleType.Annihilation);
		UISetter.SetActive(conquestRoot, active: false);
		bool active = false;
		if (troop != null)
		{
			RoTroop.Slot slotByCommanderId = troop.GetSlotByCommanderId(commander.id);
			if (slotByCommanderId != null)
			{
				UISetter.SetLabel(slotNum, slotByCommanderId.slotNum);
				UISetter.SetLabel(position, slotByCommanderId.position + 1);
			}
			switch (eBattleType)
			{
			case EBattleType.Plunder:
			case EBattleType.Annihilation:
			case EBattleType.Raid:
			case EBattleType.WaveBattle:
			case EBattleType.EventBattle:
				active = slotByCommanderId != null && troop.GetSelectedTapByCommanderId(commander.id) == charType && CheckSameUser(troop);
				break;
			case EBattleType.Conquest:
				if (slotByCommanderId != null)
				{
					active = true;
				}
				else if (_commander.conquestDeckId != 0 && battleData.conquestDeckId != _commander.conquestDeckId)
				{
					UISetter.SetActive(conquestRoot, active: true);
					UISetter.SetLabel(conquestLabel, Localization.Format("110318", _commander.conquestDeckId));
				}
				break;
			default:
				active = slotByCommanderId != null;
				break;
			}
		}
		UISetter.SetActive(useRoot, active);
		UISetter.SetActive(slotNum, active);
		UISetter.SetActive(position, active);
		if (eBattleType == EBattleType.Annihilation)
		{
			UISetter.SetActive(dieButton, commander.isDie);
			SetHp(commander.hp);
			SkillDataRow skillDataRow = RemoteObjectManager.instance.regulation.skillDtbl[commander.currLevelUnitReg.skillDrks[1]];
			UISetter.SetProgress(commanderSpProgress, (float)(int)commander.sp / (float)skillDataRow.maxSp);
			UISetter.SetLabel(commanderSpLabel, $"{(int)commander.sp / skillDataRow.maxSp * 100}%");
		}
		else
		{
			UISetter.SetActive(dieButton, active: false);
		}
	}

	private void SetHp(int hp)
	{
		UISetter.SetProgress(commanderHpProgress, (float)hp / (float)commander.currLevelUnitReg.maxHealth);
		UISetter.SetLabel(commanderHpLabel, $"{hp / commander.currLevelUnitReg.maxHealth * 100}%");
	}

	private void SetDiableReson(EBattleType battleType)
	{
		if (battleType == EBattleType.Annihilation && commander.mercenaryHp <= 0 && (int)commander.dmgHp > 0)
		{
			UISetter.SetActive(dieButton, active: true);
			UISetter.SetActive(useRoot, active: false);
			SkillDataRow skillDataRow = RemoteObjectManager.instance.regulation.skillDtbl[commander.currLevelUnitReg.skillDrks[1]];
			UISetter.SetProgress(commanderSpProgress, 0f / (float)skillDataRow.maxSp);
			UISetter.SetLabel(commanderSpLabel, $"{0 / skillDataRow.maxSp * 100}%");
			return;
		}
		UISetter.SetActive(dieButton, active: false);
		if (commander.isEngaged == 0 && commander.existEngaged != 1)
		{
			UISetter.SetActive(notAvailable, active: true);
			UISetter.SetActive(useRoot, active: false);
			UISetter.SetLabel(reason, Localization.Get("1363"));
		}
		else
		{
			UISetter.SetActive(notAvailable, active: false);
		}
	}

	private bool CheckSameUser(RoTroop troop)
	{
		if (troop == null)
		{
			return false;
		}
		for (int i = 0; i < troop.slots.Length; i++)
		{
			if (troop.slots[i].userIdx == commander.userIdx)
			{
				return true;
			}
		}
		return false;
	}

	public void OnClick(GameObject sender)
	{
		if (sender.name == "NotAvailable")
		{
			int num = RemoteObjectManager.instance.localUser.level;
			int num2 = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["GUILD_EMPLOY_LIMIT"].value);
			if (commander.isEngaged == 0)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("1368"));
			}
		}
	}

	public void SelectCommander()
	{
		checker.SelectMercenary(this);
	}

	public void CheckRoot(bool isActive)
	{
		UISetter.SetActive(useRoot, isActive);
	}

	public RoCommander GetCommander()
	{
		return commander;
	}
}
