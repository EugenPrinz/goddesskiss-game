using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIInfinityBattle : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	public UISpineAnimation spineAnimation;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UITexture goBG;

	public UIDefaultListView stageTabList;

	public GameObject scenarioRoot;

	public GameObject stageRoot;

	public UILabel scenarioTitle;

	public UILabel scenarioDescription;

	public UISprite bossThumb;

	public UILabel stageTitle;

	public UILabel stageDescription;

	public List<UIInfinityBattleItem> missionList;

	private string selectIdx;

	private string _currentIdx;

	private bool focus;

	private float itemHeight;

	private int maxStage;

	public string currentIdx
	{
		get
		{
			return _currentIdx;
		}
		set
		{
			_currentIdx = value;
		}
	}

	private void Start()
	{
		UISetter.SetSpine(spineAnimation, "n_004");
	}

	protected override void OnDisable()
	{
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
		base.OnDisable();
	}

	private void OnDestroy()
	{
		if (goBG != null)
		{
			goBG = null;
		}
		if (AnimBlock != null)
		{
			AnimBlock = null;
		}
	}

	public void Init()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			itemHeight = stageTabList.grid.cellHeight;
			maxStage = base.regulation.infinityFieldDtbl.length;
			OpenPopupShow();
		}
	}

	public void SetData(Protocols.InfinityTowerInformation data)
	{
		focus = true;
		currentIdx = data.infinityData.curField;
		selectIdx = currentIdx;
		bool flag = false;
		if (!string.IsNullOrEmpty(data.retryStage))
		{
			flag = true;
			selectIdx = data.retryStage;
		}
		stageTabList.InitInfinityList(base.localUser.infinityStageList, "Stage-");
		SetStageInfomation();
		if (flag)
		{
			SetReadyBattle();
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close")
		{
			if (!bBackKeyEnable)
			{
				Close();
			}
		}
		else if (text == "StartBtn")
		{
			StartScenario();
		}
		else if (text.StartsWith("ReadyBtn"))
		{
			SetReadyBattle();
		}
		else if (text.StartsWith("Stage-"))
		{
			string text2 = text.Substring(text.IndexOf("-") + 1);
			selectIdx = text2;
			SetStageInfomation();
		}
		else if (text.StartsWith("Mission-"))
		{
			string s = text.Substring(text.IndexOf("-") + 1);
			GetMissionReward(int.Parse(s));
		}
	}

	private void StartScenario()
	{
		Dictionary<int, int> dictionary = base.localUser.infinityStageList[selectIdx];
		if (dictionary == null || dictionary.Count == 0)
		{
			base.network.RequestStartInfinityBattleScenario(int.Parse(selectIdx));
			return;
		}
		InfinityFieldDataRow infinityFieldDataRow = base.regulation.infinityFieldDtbl[selectIdx];
		base.localUser.currScenario.scenarioId = infinityFieldDataRow.scenarioIdx;
		base.localUser.currScenario.commanderId = 0;
		Loading.Load(Loading.Scenario);
	}

	private void GetMissionReward(int idx)
	{
		InfinityFieldDataRow infinityFieldDataRow = base.regulation.infinityFieldDtbl[selectIdx];
		Dictionary<int, int> dictionary = base.localUser.infinityStageList[selectIdx];
		if (dictionary != null && dictionary.ContainsKey(idx) && dictionary[idx] == 1)
		{
			base.network.RequestInfinityBattleGetReward(int.Parse(selectIdx), idx);
		}
	}

	private void SetReadyBattle()
	{
		BattleData battleData = BattleData.Create(EBattleType.InfinityBattle);
		InfinityFieldDataRow infinityFieldDataRow = base.regulation.infinityFieldDtbl[selectIdx];
		battleData.defender = RoUser.CreateInfinityBattleUser(infinityFieldDataRow.enemy);
		battleData.stageId = selectIdx;
		base.uiWorld.readyBattle.InitAndOpenReadyBattle(battleData);
	}

	private void SetStageInfomation()
	{
		stageTabList.SetSelection(selectIdx, selected: true);
		if (focus)
		{
			if (int.Parse(selectIdx) > 3)
			{
				stageTabList.scrollView.MoveRelative(-stageTabList.scrollView.transform.localPosition);
				Vector3 zero = Vector3.zero;
				zero.y += itemHeight * (float)Mathf.Min(maxStage - 5, int.Parse(selectIdx) - 3);
				stageTabList.scrollView.MoveRelative(-zero);
			}
			focus = false;
		}
		InfinityFieldDataRow infinityData = base.regulation.infinityFieldDtbl[selectIdx];
		Dictionary<int, int> dictionary = base.localUser.infinityStageList[selectIdx];
		UISetter.SetActive(scenarioRoot, infinityData.type == EInfinityStageType.Scenario);
		UISetter.SetActive(stageRoot, infinityData.type != EInfinityStageType.Scenario);
		if (infinityData.type == EInfinityStageType.Scenario)
		{
			UISetter.SetLabel(scenarioTitle, Localization.Get(infinityData.name));
			UISetter.SetLabel(scenarioDescription, Localization.Get(infinityData.explanation));
			return;
		}
		EnemyCommanderDataRow enemyCommanderDataRow = base.regulation.enemyCommanderDtbl.Find((EnemyCommanderDataRow row) => row.id == infinityData.enemy);
		if (enemyCommanderDataRow != null)
		{
			CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[enemyCommanderDataRow.commanderId];
			UISetter.SetSprite(bossThumb, commanderDataRow.thumbnailId);
		}
		UISetter.SetLabel(stageTitle, Localization.Get(infinityData.name));
		UISetter.SetLabel(stageDescription, Localization.Get(infinityData.explanation));
		missionList[0].Set(infinityData.infinityFieldIdx, EBattleClearCondition.None, string.Empty, 1, dictionary.ContainsKey(1) ? dictionary[1] : 0);
		missionList[1].Set(infinityData.infinityFieldIdx, infinityData.clearMission01, infinityData.clearMission01Count, 2, dictionary.ContainsKey(2) ? dictionary[2] : 0);
		missionList[2].Set(infinityData.infinityFieldIdx, infinityData.clearMission02, infinityData.clearMission02Count, 3, dictionary.ContainsKey(3) ? dictionary[3] : 0);
	}

	public void UpdateInfinityBattleData(string currentIdx, Dictionary<string, Dictionary<int, int>> mission)
	{
		if (!string.IsNullOrEmpty(currentIdx))
		{
			this.currentIdx = currentIdx;
			selectIdx = currentIdx;
			focus = true;
		}
		foreach (KeyValuePair<string, Dictionary<int, int>> item in mission)
		{
			foreach (KeyValuePair<int, int> item2 in item.Value)
			{
				if (base.localUser.infinityStageList[item.Key].ContainsKey(item2.Key))
				{
					base.localUser.infinityStageList[item.Key][item2.Key] = item2.Value;
				}
				else
				{
					base.localUser.infinityStageList[item.Key].Add(item2.Key, item2.Value);
				}
			}
		}
		base.localUser.badgeInfinityBattleReward = RefreshBadge();
		RefreshStageTab();
		SetStageInfomation();
	}

	private void RefreshStageTab()
	{
		InfinityTabListItem infinityTabListItem = stageTabList.FindItem(selectIdx) as InfinityTabListItem;
		Dictionary<int, int> mission = base.localUser.infinityStageList[selectIdx];
		infinityTabListItem.Set(selectIdx, mission);
	}

	private bool RefreshBadge()
	{
		foreach (KeyValuePair<string, Dictionary<int, int>> infinityStage in base.localUser.infinityStageList)
		{
			foreach (KeyValuePair<int, int> item in infinityStage.Value)
			{
				if (item.Value == 1)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
	}

	public void OpenPopupShow()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Open()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Close()
	{
		bBackKeyEnable = true;
		HidePopup();
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
		AnimBG.Reset();
		AnimNpc.Reset();
		AnimTitle.Reset();
		AnimBlock.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public void AnimOpenFinish()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
	}

	protected override void OnEnablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: true);
	}

	protected override void OnDisablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: false);
	}
}
