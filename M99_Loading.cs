using System.Collections;
using System.Collections.Generic;
using Cache;
using DialoguerCore;
using Shared;
using Shared.Regulation;
using UnityEngine;

public class M99_Loading : MonoBehaviour
{
	private static readonly string _loadingSceneName = "M99_Loading";

	public static readonly string loadingImagePrefix = "Texture/Loading/LOADING_";

	private static string _nextSceneName;

	private static string _prevSceneName;

	public static void Load(string nextSceneName)
	{
		if (!string.IsNullOrEmpty(nextSceneName))
		{
			if (nextSceneName == Loading.Init)
			{
				Application.LoadLevel(nextSceneName);
				return;
			}
			_nextSceneName = nextSceneName;
			Application.LoadLevel(_loadingSceneName);
		}
	}

	private void Start()
	{
		StartCoroutine(SelectBG());
	}

	private IEnumerator LoadDialogueData(BattleData battleData, EBattleDialogueEventType eventType, int eventValue = 0)
	{
		string strTag = "Chapter";
		int worldId = int.Parse(battleData.worldId);
		int stageId = int.Parse(battleData.stageId);
		List<string> spine = new List<string>();
		if (worldId > 0)
		{
			stageId = stageId - (worldId - 1) * 20 - ConstValue.tutorialMaximumStage;
		}
		string dialogueName = $"{strTag}-{worldId:00}-{stageId:00}-{(int)eventType:00}";
		if (eventType == EBattleDialogueEventType.WaveEvent)
		{
			if (eventValue < 0)
			{
				eventValue = 0;
			}
			dialogueName = $"{dialogueName}-{eventValue:00}";
		}
		if (!string.IsNullOrEmpty(dialogueName) && ClassicRpgManager.HasDialogue(dialogueName))
		{
			DialoguerDialogue dialogueByName = DialoguerDataManager.GetDialogueByName(dialogueName);
			int num = dialogueByName.startPhaseId;
			while (num != 0)
			{
				AbstractDialoguePhase abstractDialoguePhase = dialogueByName.phases[num];
				if (abstractDialoguePhase is EndPhase)
				{
					break;
				}
				if (abstractDialoguePhase is TextPhase || abstractDialoguePhase is BranchedTextPhase)
				{
					DialoguerTextData data = (abstractDialoguePhase as TextPhase).data;
					string portrait = data.portrait;
					if (!spine.Contains(portrait))
					{
						spine.Add(portrait);
					}
				}
				num = 0;
				if (abstractDialoguePhase.outs != null && abstractDialoguePhase.outs[0] >= 0)
				{
					num = abstractDialoguePhase.outs[0];
				}
			}
		}
		for (int i = 0; i < spine.Count; i++)
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(spine[i] + ".assetbundle", ECacheType.Spine));
		}
		yield return null;
	}

	private IEnumerator LoadSoundData(BattleData battleData)
	{
		Regulation rg = RemoteObjectManager.instance.regulation;
		string targetPocket = "Pocket_BGM_Stage_001";
		switch (battleData.type)
		{
		case EBattleType.Plunder:
		{
			WorldMapStageDataRow worldMapStageDataRow = rg.worldMapStageDtbl[battleData.stageId];
			WorldMapStageTypeDataRow worldMapStageTypeDataRow = rg.worldMapStageTypeDtbl[worldMapStageDataRow.typeId];
			if (!string.IsNullOrEmpty(worldMapStageTypeDataRow.bgm))
			{
				targetPocket = worldMapStageTypeDataRow.bgm;
			}
			break;
		}
		case EBattleType.Guerrilla:
		{
			SweepDataRow sweepDataRow = rg.sweepDtbl[$"{battleData.sweepType}_{battleData.sweepLevel}"];
			if (!string.IsNullOrEmpty(sweepDataRow.bgm))
			{
				targetPocket = sweepDataRow.bgm;
			}
			break;
		}
		case EBattleType.Raid:
			targetPocket = "Pocket_BGM_Raid";
			break;
		case EBattleType.Duel:
		case EBattleType.WaveDuel:
		case EBattleType.Conquest:
		case EBattleType.WorldDuel:
			targetPocket = "Pocket_BGM_Dual";
			break;
		case EBattleType.Annihilation:
		{
			AnnihilateBattleDataRow annihilateBattleDataRow = rg.annihilateBattleDtbl[battleData.stageId];
			if (!string.IsNullOrEmpty(annihilateBattleDataRow.bmg))
			{
				targetPocket = annihilateBattleDataRow.bmg;
			}
			break;
		}
		case EBattleType.EventBattle:
		{
			EventBattleFieldDataRow eventBattleFieldDataRow = rg.eventBattleFieldDtbl[$"{battleData.eventId}_{battleData.eventLevel}"];
			if (!string.IsNullOrEmpty(eventBattleFieldDataRow.bgm))
			{
				targetPocket = eventBattleFieldDataRow.bgm;
			}
			break;
		}
		case EBattleType.EventRaid:
			targetPocket = "Pocket_BGM_Raid";
			break;
		}
		if (targetPocket.StartsWith("Pocket_"))
		{
			CacheManager.instance.SoundPocketCache.Create(targetPocket);
		}
		else
		{
			CacheManager.instance.SoundCache.PlayBGM(Loading.Battle, targetPocket);
		}
		CacheManager.instance.SoundCache.Create("win");
		CacheManager.instance.SoundCache.Create("lose");
		yield return null;
	}

	private IEnumerator LoadCommanderData(string commanderId)
	{
		CommanderDataRow cmdDr = RemoteObjectManager.instance.regulation.commanderDtbl[commanderId];
		CacheItemPocket.instance.Create(CacheManager.instance.SpineCache, cmdDr.resourceId);
		if (!string.IsNullOrEmpty(cmdDr.unitId))
		{
			yield return StartCoroutine(LoadUnitData(cmdDr.unitId));
		}
	}

	private IEnumerator LoadUnitData(string unitId)
	{
		UnitDataRow unitDr = RemoteObjectManager.instance.regulation.unitDtbl[unitId];
		for (int i = 0; i < unitDr.skillDrks.Count; i++)
		{
			if (!string.IsNullOrEmpty(unitDr.skillDrks[i]) && unitDr.skillDrks[i] != "-" && unitDr.skillDrks[i] != "0")
			{
				yield return StartCoroutine(LoadSkillData(unitDr.skillDrks[i]));
			}
		}
		yield return null;
	}

	private IEnumerator LoadSkillData(string skillId)
	{
		SkillDataRow skillDr = RemoteObjectManager.instance.regulation.skillDtbl[skillId];
		if (string.IsNullOrEmpty(skillDr.cutInEffectId) || skillDr.cutInEffectId != "-")
		{
		}
		if (!string.IsNullOrEmpty(skillDr.actionEffSound) && skillDr.actionEffSound != "0")
		{
			CacheManager.instance.SoundCache.Create(skillDr.actionEffSound);
		}
		if (!string.IsNullOrEmpty(skillDr.actionSound) && skillDr.actionSound != "0")
		{
			CacheManager.instance.SoundCache.Create(skillDr.actionSound);
		}
		if (!string.IsNullOrEmpty(skillDr.fireSound) && skillDr.fireSound != "0")
		{
			CacheManager.instance.SoundCache.Create(skillDr.fireSound);
		}
		if (!string.IsNullOrEmpty(skillDr.hitSound) && skillDr.hitSound != "0")
		{
			CacheManager.instance.SoundCache.Create(skillDr.hitSound);
		}
		if (!string.IsNullOrEmpty(skillDr.beHitSound) && skillDr.beHitSound != "0")
		{
			CacheManager.instance.SoundCache.Create(skillDr.beHitSound);
		}
		if (!string.IsNullOrEmpty(skillDr.beMissSound) && skillDr.beMissSound != "0")
		{
			CacheManager.instance.SoundCache.Create(skillDr.beMissSound);
		}
		yield return null;
	}

	private IEnumerator LoadProjectileData(string projectileId)
	{
		ProjectileDataRow projectileDr = RemoteObjectManager.instance.regulation.projectileDtbl[projectileId];
		for (int i = 0; i < projectileDr.statusEffectDrks.Count; i++)
		{
			if (!string.IsNullOrEmpty(projectileDr.statusEffectDrks[i]) && projectileDr.statusEffectDrks[i] != "-" && projectileDr.statusEffectDrks[i] != "0")
			{
				yield return StartCoroutine(LoadStatusEffectData(projectileDr.statusEffectDrks[i]));
			}
		}
	}

	private IEnumerator LoadStatusEffectData(string statusEffectId)
	{
		StatusEffectDataRow statusEffDr = RemoteObjectManager.instance.regulation.statusEffectDtbl[statusEffectId];
		if (string.IsNullOrEmpty(statusEffDr.pfbName) || !(statusEffDr.pfbName != "-") || statusEffDr.pfbName != "0")
		{
		}
		yield return null;
	}

	private IEnumerator LoadTroopData(EBattleSide side, Troop troop)
	{
		for (int i = 0; i < troop.slots.Count; i++)
		{
			Troop.Slot slot = troop.slots[i];
			if (slot == null || slot.isEmpty)
			{
				continue;
			}
			if (side == EBattleSide.Left)
			{
				if (!string.IsNullOrEmpty(slot.cid))
				{
					yield return StartCoroutine(LoadCommanderData(slot.cid));
				}
			}
			else if (!string.IsNullOrEmpty(slot.id))
			{
				yield return StartCoroutine(LoadUnitData(slot.id));
			}
		}
	}

	private IEnumerator LoadTroopData(EBattleSide side, RoTroop troop)
	{
		for (int i = 0; i < troop.slots.Length; i++)
		{
			RoTroop.Slot slot = troop.slots[i];
			if (!slot.IsValid() || slot.position < 0)
			{
				continue;
			}
			if (side == EBattleSide.Left)
			{
				if (!string.IsNullOrEmpty(slot.commanderId))
				{
					yield return StartCoroutine(LoadCommanderData(slot.commanderId));
				}
			}
			else if (!string.IsNullOrEmpty(slot.unitId))
			{
				yield return StartCoroutine(LoadUnitData(slot.unitId));
			}
		}
	}

	private IEnumerator LoadTutorialData()
	{
		string[] tutorialUnits = new string[13]
		{
			"50001", "50002", "50003", "50004", "50005", "50006", "50007", "50008", "50009", "50010",
			"50011", "50012", "50013"
		};
		string[] tutorialSpine = new string[8] { "v_001", "v_002", "c_001", "c_018", "c_004", "n_005", "n_003", "n_006" };
		for (int j = 0; j < tutorialUnits.Length; j++)
		{
			yield return StartCoroutine(LoadUnitData(tutorialUnits[j]));
		}
		for (int i = 0; i < tutorialSpine.Length; i++)
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(tutorialSpine[i] + ".assetbundle", ECacheType.Spine));
			CacheItemPocket.instance.Create(CacheManager.instance.SpineCache, tutorialSpine[i]);
		}
		yield return null;
	}

	private IEnumerator ToNext()
	{
		yield return null;
		Time.timeScale = 1f;
		string next = _nextSceneName;
		if (next.CompareTo(Loading.Title) == 0)
		{
			SocketChatting.Relase();
		}
		if (CacheItemPocket.instance == null)
		{
			GameObject gameObject = new GameObject();
			CacheItemPocket.instance = gameObject.AddComponent<CacheItemPocket>();
			CacheItemPocket.instance.transform.position = new Vector3(999999f, 9999999f, 999999f);
			gameObject.name = typeof(CacheItemPocket).Name;
		}
		AssetBundleManager.DeleteAssetBundleAllWithoutBgm();
		if (next.CompareTo(Loading.WorldMap) == 0)
		{
		}
		if (next.CompareTo(Loading.Battle) == 0 || next.CompareTo(Loading.Tutorial) == 0)
		{
			if (next.CompareTo(Loading.Battle) == 0)
			{
				BattleData battleData = BattleData.Get();
				if (battleData != null)
				{
					yield return StartCoroutine(LoadSoundData(battleData));
					if (battleData.isReplayMode)
					{
						yield return StartCoroutine(LoadTroopData(EBattleSide.Left, battleData.record.initState.lhsTroops[0]));
						for (int i = 0; i < battleData.record.initState.rhsTroops.Count; i++)
						{
							yield return StartCoroutine(LoadTroopData(EBattleSide.Right, battleData.record.initState.rhsTroops[i]));
						}
					}
					else
					{
						if (battleData.type == EBattleType.Plunder)
						{
							for (int k = 0; k < 5; k++)
							{
							}
						}
						yield return StartCoroutine(LoadTroopData(EBattleSide.Left, battleData.attackerTroop));
						for (int j = 0; j < battleData.defender.battleTroopList.Count; j++)
						{
							yield return StartCoroutine(LoadTroopData(EBattleSide.Right, battleData.defender.battleTroopList[j]));
						}
					}
					BattleData.Set(battleData);
				}
			}
			else if (next.CompareTo(Loading.Tutorial) == 0)
			{
				yield return StartCoroutine(LoadTutorialData());
			}
		}
		else if (next.CompareTo(Loading.Prologue) == 0)
		{
			CacheManager.instance.SoundCache.PlayBGM(Loading.Prologue, "BGM_Prologue");
		}
		else if (next.CompareTo(Loading.Dormitory) == 0)
		{
			CacheManager.instance.SoundCache.PlayBGM(Loading.Dormitory, "BGM_Chapter_001");
		}
		_nextSceneName = null;
		_prevSceneName = next;
		AsyncOperation sync = Application.LoadLevelAsync(next);
		sync.allowSceneActivation = false;
		while (sync.progress < 0.9f)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.1f);
		sync.allowSceneActivation = true;
	}

	private IEnumerator SelectBG()
	{
		GameObject loading = Utility.LoadAndInstantiateGameObject("Prefabs/UI/Loading");
		loading.GetComponent<UILoading>().In();
		if (_nextSceneName != Loading.Prologue)
		{
			Object.DontDestroyOnLoad(loading);
		}
		yield return new WaitForSeconds(0.1f);
		yield return StartCoroutine(ToNext());
	}
}
