using System.Collections;
using System.Collections.Generic;
using Cache;
using Shared.Regulation;
using UnityEngine;

public class UIWorldMap : UIPopup
{
	public UITexture background1;

	public UITexture background2;

	public UIDefaultListView stageListView;

	private string strDefWorld = "Chapter";

	private string strDefWorldPrefix = string.Empty;

	private string strDefType = string.Empty;

	private string fmt = "00";

	private string strTypeResult = "01";

	private string strWorldResult = string.Empty;

	private string strStageResult = string.Empty;

	private string strResult;

	public UIScrollView stageScrollView;

	public GameObject previousMapButton;

	public GameObject previousMapBadge;

	public GameObject nextMapButton;

	public GameObject nextMapBadge;

	public GameObject nextMapLock;

	public UILabel worldName;

	public string selectStageId;

	public UILabel commanderRate;

	public UILabel commanderName;

	public UISprite commanderSprite;

	public GameObject commanderRewardBtn;

	public GameObject commanderRewardStarRoot;

	public GameObject commanderRewardClearRoot;

	public TweenScale tweenScale;

	public UIPlayTween tween;

	public TweenAlpha inTweener;

	public TweenAlpha outTweener;

	private const string tutorialWorldMapId = "0";

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	[HideInInspector]
	public UIGetCommander getCommander;

	private UIExplorationPopup _explorationPopup;

	public GameObject explorationBadge;

	public string currentWorldMapId { get; private set; }

	public RoWorldMap currentWorldMap => base.localUser.FindWorldMap(currentWorldMapId);

	private void Start()
	{
	}

	private void OnDestroy()
	{
		if (commanderSprite != null)
		{
			commanderSprite = null;
		}
		if (background1 != null)
		{
			background1 = null;
		}
		if (background2 != null)
		{
			background2 = null;
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (stageListView.Contains(text))
		{
			selectStageId = stageListView.GetPureId(text);
			RoWorldMap.Stage stage = currentWorldMap.FindStage(selectStageId);
			StartStage(stage, sender);
			return;
		}
		switch (text)
		{
		case "PreviousMap":
		{
			RoWorldMap roWorldMap = base.localUser.FindWorldMapByOffset(currentWorldMapId, -1);
			currentWorldMapId = roWorldMap.id;
			selectStageId = null;
			base.network.RequestWorldMapInformation(roWorldMap.id);
			break;
		}
		case "NextMap":
		{
			RoWorldMap roWorldMap2 = currentWorldMap;
			RoWorldMap roWorldMap3 = base.localUser.FindWorldMapByOffset(currentWorldMapId, 1);
			if (roWorldMap3 == null)
			{
				UISimplePopup.CreateOK(localization: true, "5386", "6042", "6037", "1001");
			}
			else if (roWorldMap2.canMoveNextMap)
			{
				currentWorldMapId = roWorldMap3.id;
				selectStageId = null;
				base.network.RequestWorldMapInformation(roWorldMap3.id);
			}
			else if (roWorldMap2.stageCount > currentWorldMap.clearStageCount)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Format("6039"));
			}
			else if (roWorldMap3.dataRow.unlockUserLevel > base.localUser.level)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Format("99999", roWorldMap3.dataRow.unlockUserLevel));
			}
			break;
		}
		case "CommanderReward":
			GetCommanderReward();
			break;
		case "Exploration":
			if (_explorationPopup == null)
			{
				UISetter.SetActive(explorationBadge, active: false);
				_explorationPopup = UIPopup.Create<UIExplorationPopup>("ExplorationPopup");
			}
			break;
		}
	}

	public void GetCommanderReward()
	{
		if (getCommander == null)
		{
			getCommander = UIPopup.Create<UIGetCommander>("GetCommander");
			RoWorldMap.Stage stage = currentWorldMap.stageList[currentWorldMap.stageList.Count - 1];
			getCommander.Init(currentWorldMap);
		}
	}

	public void StartStage(RoWorldMap.Stage _stage, GameObject sender = null)
	{
		if (_stage.isOpen && _stage.data.type != EStageTypeIdRange.PowerPlant)
		{
			if ((_stage.data.type != EStageTypeIdRange.GuardPost || _stage.canBattle) && (_stage.data.type != EStageTypeIdRange.Storage || _stage.canBattle))
			{
				BattleData battleData = BattleData.Create(EBattleType.Plunder);
				battleData.defender = _stage.GetEnemy();
				battleData.stageId = _stage.id;
				battleData.worldId = currentWorldMapId;
				battleData.IsCleared = int.Parse(_stage.id) <= base.localUser.lastClearStage;
				base.uiWorld.readyBattle.InitAndOpenReadyBattle(battleData);
			}
		}
		else if (!_stage.canBattle && sender != null)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5351"));
		}
		else if (!_stage.isOpen && sender != null)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("5767"));
		}
	}

	private IEnumerator DialogEndCheck(RoWorldMap.Stage _stage)
	{
		while (base.uiWorld.dialogMrg.gameObject.activeSelf)
		{
			yield return new WaitForSeconds(0.5f);
		}
		BattleData data = BattleData.Create(EBattleType.Plunder);
		data.defender = _stage.GetEnemy();
		data.stageId = _stage.id;
		data.worldId = currentWorldMapId;
		base.uiWorld.readyBattle.InitAndOpenReadyBattle(data);
	}

	public void NextStage(string _id)
	{
		RoWorldMap.Stage stage = currentWorldMap.FindNextStage(_id);
		StartStage(stage);
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		RefreshWorldMapReward();
	}

	public void _Set(RoWorldMap worldMap)
	{
		if (worldMap == null)
		{
			return;
		}
		tweenScale.enabled = false;
		currentWorldMapId = worldMap.id;
		UISetter.SetLabel(title, worldMap.name);
		UISetter.SetActive(previousMapButton, worldMap.canMovePreviousMap);
		bool canMoveNextMap = worldMap.canMoveNextMap;
		UISetter.SetActive(nextMapButton, !worldMap.id.Equals("0"));
		UISetter.SetActive(nextMapLock, !canMoveNextMap);
		UISetter.SetActive(explorationBadge, base.localUser.explorationDtbl.hasCompleteState);
		List<RoWorldMap.Stage> stageList = worldMap.stageList;
		UISetter.SetTexture(background1, Utility.LoadTexture(worldMap.dataRow.backgroundImageId));
		UISetter.SetTexture(background2, Utility.LoadTexture(worldMap.dataRow.backgroundImageId));
		stageScrollView.MoveRelative(-stageScrollView.transform.localPosition);
		int num = ((base.localUser.lastPlayStage == -1) ? (base.localUser.lastClearStage + 1) : base.localUser.lastPlayStage);
		if (base.localUser.lastPlayStage != -1 || (base.localUser.FindWorldMapByStage((num + 1).ToString()) != null && int.Parse(currentWorldMap.id) == int.Parse(base.localUser.FindLastOpenedWorldMap().id)))
		{
			if ((num - 4) % 20 >= 10)
			{
				stageScrollView.MoveRelative(new Vector3(-1280f, 0f, 0f));
			}
		}
		else if (int.Parse(currentWorldMap.id) != 0)
		{
			stageScrollView.MoveRelative(new Vector3(-1280f, 0f, 0f));
		}
		stageScrollView.UpdatePosition();
		base.localUser.lastPlayStage = -1;
		stageListView.Init(stageList, "Stage-");
		stageList.ForEach(delegate(RoWorldMap.Stage stage)
		{
			if (_CheckPosition(stage.data.position))
			{
			}
		});
		if (worldMap.starCount >= worldMap.maxStarCount && !worldMap.rwd)
		{
			tweenScale.PlayForward();
		}
		UISetter.SetLabel(UIManager.instance.world.mainCommand.worldName, Localization.Get(worldMap.name));
		int lastClearStage = base.localUser.lastClearStage;
		if (base.localUser.FindWorldMapByStage((base.localUser.lastClearStage + 1).ToString()) != null)
		{
			stageListView.SetSelection((lastClearStage + 1).ToString(), selected: true);
		}
		stageScrollView.enabled = !worldMap.id.Equals("0");
		RefreshWorldMapReward();
	}

	public void RefreshWorldMapReward()
	{
		CommanderDataRow commanderDataRow = base.regulation.commanderDtbl[currentWorldMap.dataRow.c_idx];
		UISetter.SetLabel(commanderRate, $"{currentWorldMap.starCount}/{currentWorldMap.maxStarCount}");
		UISetter.SetLabel(commanderName, commanderDataRow.nickname);
		UISetter.SetSprite(commanderSprite, commanderDataRow.worldMapRewardId);
		UISetter.SetActive(commanderRewardStarRoot, !currentWorldMap.rwd);
		UISetter.SetActive(commanderRewardClearRoot, currentWorldMap.rwd);
		UISetter.SetScale(commanderRewardStarRoot, new Vector3(1f, 1f, 1f));
	}

	public void StartRewardAnimation()
	{
		tween.resetOnPlay = true;
		tween.tweenGroup = 1;
		tween.Play(forward: true);
	}

	public void EndRewardAnimation()
	{
		tween.resetOnPlay = true;
		tween.tweenGroup = 2;
		tween.Play(forward: true);
	}

	private bool _CheckPosition(Vector3 pos)
	{
		return true;
	}

	public void InitAndOpenWorldMap(string world = null, string stage = null)
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			if (base.localUser.tutorialStep < 12)
			{
				RoWorldMap roWorldMap = base.localUser.FindWorldMap("0");
				base.network.RequestWorldMapInformation(roWorldMap.id);
			}
			else
			{
				RoWorldMap roWorldMap = ((!string.IsNullOrEmpty(world)) ? base.localUser.FindWorldMap(world) : base.localUser.FindLastOpenedWorldMap());
				selectStageId = stage;
				base.network.RequestWorldMapInformation(roWorldMap.id);
			}
		}
	}

	public void MoveWorldMap(string world)
	{
		currentWorldMapId = world;
		selectStageId = null;
		base.network.RequestWorldMapInformation(world);
	}

	public void InitAndOpenLastStageWorldMap()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			if (base.localUser.tutorialStep < 12)
			{
				RoWorldMap roWorldMap = base.localUser.FindWorldMap("0");
				base.network.RequestWorldMapInformation(roWorldMap.id);
			}
			else
			{
				RoWorldMap roWorldMap = base.localUser.FindWorldMapByStage(base.localUser.lastPlayStage.ToString());
				base.network.RequestWorldMapInformation(roWorldMap.id);
			}
		}
	}

	public void InitAndOpenTutorialWorldMap()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			RoWorldMap roWorldMap = base.localUser.FindWorldMap("0");
			base.network.RequestWorldMapInformation(roWorldMap.id);
		}
	}

	public void StartStageNavigation()
	{
		if (!string.IsNullOrEmpty(selectStageId))
		{
			RoWorldMap.Stage stage = currentWorldMap.FindStage(selectStageId);
			StartStage(stage);
			selectStageId = null;
		}
	}

	public void SetBGM()
	{
		if (currentWorldMap.dataRow.bgm.StartsWith("Pocket_"))
		{
			CacheManager.instance.SoundPocketCache.Create(currentWorldMap.dataRow.bgm);
		}
		else
		{
			CacheManager.instance.SoundCache.PlayBGM(Loading.WorldMap, currentWorldMap.dataRow.bgm);
		}
	}

	public void CloseWorldMap()
	{
		base.uiWorld.worldMap.Close();
		base.uiWorld.camp.OpenCamp();
	}

	public override void Open()
	{
		base.Open();
		base.uiWorld.mainCommand.OnWorldMap();
		In();
	}

	public override void Close()
	{
		if (!bBackKeyEnable)
		{
			bBackKeyEnable = true;
			Out();
			selectStageId = null;
		}
	}

	private void In()
	{
		_Play(inTweener);
	}

	private void Out()
	{
		_Play(outTweener);
	}

	private void _Play(UITweener tweener)
	{
		tweener.enabled = true;
		tweener.ResetToBeginning();
		tweener.PlayForward();
	}

	private void OpenEnd()
	{
	}

	public void CloseEnd()
	{
		base.Close();
		base.uiWorld.camp.OpenCamp();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
	}
}
