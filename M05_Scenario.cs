using System.Collections;
using DialoguerCore;
using UnityEngine;

public class M05_Scenario : MonoBehaviour
{
	private ClassicRpgManager dialogMgr;

	private string commanderID;

	private string scenarioID;

	private GameObject loading;

	private bool bStart;

	public RoLocalUser localUser => RemoteObjectManager.instance.localUser;

	private void Awake()
	{
		loading = GameObject.Find("Loading");
		commanderID = $"{localUser.currScenario.commanderId:D5}";
		scenarioID = localUser.currScenario.scenarioId.ToString();
	}

	private IEnumerator Start()
	{
		if (loading != null)
		{
			loading.transform.parent = UIRoot.list[0].transform;
			loading.transform.localPosition = Vector3.zero;
			yield return null;
		}
		yield return StartCoroutine(UIManager.instance.scenario.InitCoroutine());
		if (loading != null)
		{
			Object.DestroyImmediate(loading);
		}
		dialogMgr = UIManager.instance.DialogMrg;
		if (localUser.currScenario.commanderId != 0)
		{
			dialogMgr.StartScenario();
			dialogMgr.InitScenarioDialogue(scenarioID, DialogueType.Scenario);
		}
		else
		{
			dialogMgr.StartInfinityScenario();
			dialogMgr.InitScenarioDialogue(scenarioID, DialogueType.Infinity);
		}
		UIFade.In(0f);
		bStart = true;
		yield return null;
	}

	private void Update()
	{
		if (bStart && !RemoteObjectManager.instance.waitingScenarioComplete && (dialogMgr == null || !dialogMgr.gameObject.activeSelf))
		{
			BattleData battleData = BattleData.Create(EBattleType.Undefined);
			if (localUser.currScenario.commanderId != 0)
			{
				battleData.move = EBattleResultMove.CommanderDetailScenarioInfo;
			}
			else
			{
				battleData.move = EBattleResultMove.InfinityBattle;
			}
			BattleData.Set(battleData);
			Loading.Load(Loading.WorldMap);
		}
	}
}
