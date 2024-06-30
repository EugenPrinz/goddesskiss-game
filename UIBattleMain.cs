using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

public class UIBattleMain : UIPopup
{
	[Serializable]
	public class Main : UIInnerPartBase
	{
		[Serializable]
		public class UIView
		{
			public GameObject root;

			protected UIPanelBase parentPanelBase;

			protected UIBattleMain uiBattleMain => parentPanelBase as UIBattleMain;

			protected EBattleType battleType => (uiBattleMain != null) ? uiBattleMain.battleType : EBattleType.Undefined;

			public virtual void Init(UIPanelBase parent)
			{
				parentPanelBase = parent;
			}

			public virtual void SetEnable(bool enable)
			{
				UISetter.SetActive(root, enable);
			}

			public virtual bool GetEnable()
			{
				return root != null && root.activeSelf;
			}
		}

		[Serializable]
		public class UITurnGaugeView : UIView
		{
			[Serializable]
			public class UIClearRankView : UIView
			{
				[Serializable]
				public struct Rank
				{
					public GameObject icon;

					public GameObject closeEff;
				}

				public List<Rank> rankObjects;

				private Coroutine _ctnUpdate;

				private int _rank;

				public int rank
				{
					get
					{
						return _rank;
					}
					set
					{
						if (!GetEnable() || _rank == value)
						{
							return;
						}
						_rank = value;
						if (_rank < 0 || _rank > rankObjects.Count)
						{
							for (int i = 0; i < rankObjects.Count; i++)
							{
								UISetter.SetActive(rankObjects[i].icon, active: false);
								UISetter.SetActive(rankObjects[i].closeEff, active: false);
							}
							return;
						}
						if (_ctnUpdate != null)
						{
							parentPanelBase.StopCoroutine(_ctnUpdate);
							_ctnUpdate = null;
						}
						_ctnUpdate = parentPanelBase.StartCoroutine(_UpdateRank());
					}
				}

				private IEnumerator _UpdateRank()
				{
					for (int i = 0; i < rankObjects.Count; i++)
					{
						if (i < _rank)
						{
							UISetter.SetActive(rankObjects[i].icon, active: true);
							UISetter.SetActive(rankObjects[i].closeEff, active: false);
						}
						else if (rankObjects[i].icon.activeSelf)
						{
							UISetter.SetActive(rankObjects[i].closeEff, active: true);
							yield return new WaitForSeconds(0.3f);
							UISetter.SetActive(rankObjects[i].icon, active: false);
							yield return new WaitForSeconds(0.6f);
							UISetter.SetActive(rankObjects[i].closeEff, active: false);
						}
						else
						{
							UISetter.SetActive(rankObjects[i].closeEff, active: false);
						}
					}
					yield return null;
				}
			}

			[Serializable]
			public class UITurnLabelView : UIView
			{
				public UILabel turn;

				private int _endTurn;

				public void InitTurn(int startTurn, int endTurn)
				{
					_endTurn = endTurn;
					SetTurn(startTurn);
				}

				public void SetTurn(int curTurn)
				{
					if (GetEnable())
					{
						if (_endTurn == 0)
						{
							turn.text = $"{curTurn}";
						}
						else
						{
							turn.text = $"{curTurn} / {_endTurn}";
						}
					}
				}
			}

			public UIClearRankView clearRankView;

			public UITurnLabelView turnLabelView;

			public UIProgressBar turnProgress;

			public GameObject startPos;

			public GameObject endPos;

			public UIDefaultListView itemListView;

			private Coroutine _ctnUpdate;

			private float _progressVal;

			private int _endTurn;

			private int _turn = -1;

			public int turn
			{
				get
				{
					return _turn;
				}
				set
				{
					if (GetEnable() && _turn != value)
					{
						_turn = value;
						_progressVal = (float)(_endTurn - _turn) / (float)_endTurn;
						if (_progressVal < 0f)
						{
							_progressVal = 0f;
						}
						if (_ctnUpdate != null)
						{
							parentPanelBase.StopCoroutine(_ctnUpdate);
							_ctnUpdate = null;
						}
						_ctnUpdate = parentPanelBase.StartCoroutine(TurnGaugeUpdate());
						turnLabelView.SetTurn(_turn);
					}
				}
			}

			public override void Init(UIPanelBase parent)
			{
				base.Init(parent);
				Type type = GetType();
				FieldInfo[] fields = type.GetFields();
				for (int i = 0; i < fields.Length; i++)
				{
					if (fields[i].FieldType.IsSubclassOf(typeof(UIView)))
					{
						UIView uIView = fields[i].GetValue(this) as UIView;
						uIView.Init(parent);
					}
				}
			}

			public void InitUI()
			{
				_endTurn = 0;
				_progressVal = 1f;
				turnProgress.value = 1f;
				turnProgress.foregroundWidget.color = Color.white;
				SetRank(3);
			}

			public void InitTurn(List<int> steps)
			{
				_endTurn = steps[steps.Count - 1];
				turnLabelView.InitTurn(steps[0], 0);
				itemListView.ResizeItemList(steps.Count);
				Vector3 vector = (endPos.transform.position - startPos.transform.position) / _endTurn;
				for (int i = 0; i < itemListView.itemList.Count; i++)
				{
					UITurnGagueItem uITurnGagueItem = itemListView.itemList[i] as UITurnGagueItem;
					uITurnGagueItem.transform.position = startPos.transform.position + vector * steps[i];
					uITurnGagueItem.label.text = steps[i].ToString();
					if (i == 0)
					{
						uITurnGagueItem.icon.spriteName = null;
					}
					else if (base.battleType == EBattleType.Raid)
					{
						if (i == itemListView.itemList.Count - 1)
						{
							uITurnGagueItem.icon.spriteName = "raid-wide02";
							uITurnGagueItem.icon.width = 114;
							uITurnGagueItem.icon.height = 60;
						}
						else
						{
							uITurnGagueItem.icon.spriteName = null;
						}
					}
					else
					{
						uITurnGagueItem.icon.spriteName = null;
					}
				}
			}

			public void SetRank(int rank)
			{
				clearRankView.rank = rank;
			}

			private IEnumerator TurnGaugeUpdate()
			{
				float direction = 0f;
				float speed = 1f;
				direction = ((!(_progressVal < turnProgress.value)) ? 1f : (-1f));
				UIWidget wg = turnProgress.foregroundWidget;
				while (_progressVal != turnProgress.value)
				{
					float time = Time.deltaTime;
					float dt = time * direction * speed;
					turnProgress.value += dt;
					if (direction < 0f)
					{
						if (_progressVal > turnProgress.value)
						{
							turnProgress.value = _progressVal;
						}
					}
					else if (_progressVal < turnProgress.value)
					{
						turnProgress.value = _progressVal;
					}
					wg.color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(1f, 0f, 0f, 1f), 1f - turnProgress.value);
					yield return null;
				}
			}
		}

		[Serializable]
		public class UIScoreView : UIView
		{
			public UILabel score;

			private long _scoreVal = -1L;

			public long scoreVal
			{
				get
				{
					return _scoreVal;
				}
				set
				{
					if (GetEnable() && _scoreVal != value)
					{
						_scoreVal = value;
						score.text = _scoreVal.ToString("N0");
					}
				}
			}

			public void InitUI()
			{
				scoreVal = 0L;
			}
		}

		[Serializable]
		public class UIGetBoxView : UIView
		{
			public TweenScale iconScale;

			public UILabel label;

			protected int _cnt = -1;

			protected int box
			{
				get
				{
					return _cnt;
				}
				set
				{
					if (GetEnable() && _cnt != value)
					{
						_cnt = value;
						UISetter.SetLabel(label, _cnt.ToString("N0"));
					}
				}
			}

			public int takenBox
			{
				get
				{
					return box;
				}
				set
				{
					if (GetEnable())
					{
						box = value;
						iconScale.ResetToBeginning();
						iconScale.Play(forward: true);
					}
				}
			}

			public void Clean()
			{
				iconScale.gameObject.transform.localScale = Vector3.one;
				box = 0;
			}
		}

		[Serializable]
		public class UIGetGoldView : UIView
		{
			public TweenScale iconScale;

			public UILabel label;

			private int _gold = -1;

			private int _takenGold;

			private Coroutine _ctnUpdate;

			protected int gold
			{
				get
				{
					return _gold;
				}
				set
				{
					if (GetEnable() && _gold != value)
					{
						_gold = value;
						label.text = _gold.ToString("N0");
					}
				}
			}

			public int takenGold
			{
				get
				{
					return _takenGold;
				}
				set
				{
					if (GetEnable() && _takenGold != value)
					{
						_takenGold = value;
						iconScale.ResetToBeginning();
						iconScale.Play(forward: true);
						if (_ctnUpdate != null)
						{
							parentPanelBase.StopCoroutine(_ctnUpdate);
							_ctnUpdate = null;
						}
						_ctnUpdate = parentPanelBase.StartCoroutine(UpdateGold());
					}
				}
			}

			public void Clean()
			{
				iconScale.gameObject.transform.localScale = Vector3.one;
				_takenGold = 0;
				gold = 0;
			}

			private IEnumerator UpdateGold()
			{
				while (takenGold != 0)
				{
					int value = 1;
					_takenGold -= value;
					if (takenGold < 0)
					{
						value += takenGold;
					}
					gold += value;
					yield return new WaitForSeconds(0.05f);
				}
			}
		}

		[Serializable]
		public class UIAutoView : UIView
		{
			public GameObject btnAuto;

			public GameObject btnManual;

			public GameObject iconLock;
		}

		[Serializable]
		public class UIWaveLabelView : UIView
		{
			public UILabel label;

			public Vector3 defaultPosition = new Vector3(-35f, -95f, 0f);

			public Vector3 guerrillaPosition = new Vector3(-35f, -48f, 0f);

			private int _totalWave;

			private int _wave;

			public int wave
			{
				get
				{
					return _wave;
				}
				set
				{
					if (GetEnable() && value != _wave)
					{
						if (base.battleType == EBattleType.WaveBattle)
						{
							_wave = value;
							label.text = $"{_wave.ToString()}";
						}
						else if (value <= _totalWave)
						{
							_wave = value;
							label.text = $"{_wave.ToString()}/{_totalWave.ToString()}";
						}
					}
				}
			}

			public void Init(EBattleType type)
			{
				if (type == EBattleType.Guerrilla)
				{
					root.transform.localPosition = guerrillaPosition;
				}
				else
				{
					root.transform.localPosition = defaultPosition;
				}
			}

			public void InitWave(int totalWave)
			{
				_totalWave = totalWave;
				wave = 1;
			}
		}

		[Serializable]
		public class UISpeedUpView : UIView
		{
			public List<GameObject> speedBtnList;

			public void SelectSpeed(string key)
			{
				if (GetEnable())
				{
					int num = speedBtnList.FindIndex((GameObject b) => b.name == key);
					num = (num + 1) % speedBtnList.Count;
					for (int i = 0; i < speedBtnList.Count; i++)
					{
						UISetter.SetActive(speedBtnList[i], i == num);
					}
				}
			}
		}

		[Serializable]
		public class UITimeView : UIView
		{
			public UILabel time;

			public float timeWarnThreshold = 10f;

			public GameObject timeWarnRoot;
		}

		[Serializable]
		public class UIGuildItemView : UIView
		{
			public UIDefaultListView items;

			public void Clean()
			{
				if (items != null)
				{
					items.ResizeItemList(0);
				}
			}

			public void Set(List<GuildSkillState> items)
			{
				if (GetEnable())
				{
					this.items.Init(items);
				}
			}
		}

		[Serializable]
		public class UIUserView : UIView
		{
			public UIUser uiUser;

			public UICommanderTag uiCommanderTag;

			public UIGuildItemView uiGuildItemView;

			protected int curCdri = -1;

			protected int curUdri = -1;

			public void Set(RoCommander commander)
			{
				if (!(uiCommanderTag == null))
				{
					uiCommanderTag.Set(commander);
				}
			}

			public void Set(Unit unit)
			{
				if (!(uiCommanderTag == null) && curUdri != unit.dri)
				{
					uiCommanderTag.SetRank(unit._rank);
					uiCommanderTag.SetLevel(unit.level);
					if (curCdri != unit._cdri)
					{
						CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[unit._cdri];
						uiCommanderTag.SetNickname(commanderDataRow.nickname);
						uiCommanderTag.SetCommander(commanderDataRow.resourceId);
						uiCommanderTag.EnableCommander(enable: true);
						curCdri = unit._cdri;
					}
					curUdri = unit.dri;
				}
			}

			public void SetGuildSkill(List<GuildSkillState> items)
			{
				if (items != null && GetEnable())
				{
					uiGuildItemView.Set(items);
				}
			}

			public void Shake(Unit unit)
			{
				if (!(uiCommanderTag == null) && unit._cdri == curCdri)
				{
					uiCommanderTag.Shake();
				}
			}

			public void SetState(Unit unit, string state)
			{
				if (!(uiCommanderTag == null) && curUdri == unit.dri)
				{
					uiCommanderTag.SetState(state);
				}
			}
		}

		[Serializable]
		public class UIWaveMarkerView : UIView
		{
			public UILabel label;

			public UIGrid grid;

			public GameObject item;

			private Transform gridTf;

			private int _totalWave;

			private int _wave;

			public int wave
			{
				get
				{
					return _wave;
				}
				set
				{
					if (GetEnable() && value > _wave && value <= _totalWave)
					{
						for (int i = _wave; i < value; i++)
						{
							GameObject gameObject = gridTf.GetChild(i).gameObject;
							parentPanelBase.StartCoroutine(_CloseMarker(gameObject));
						}
						_wave = value;
						UISetter.SetLabel(label, "WAVE" + _wave);
					}
				}
			}

			private IEnumerator _CloseMarker(GameObject obj)
			{
				GameObject fx = obj.transform.GetChild(0).gameObject;
				fx.SetActive(value: true);
				yield return new WaitForSeconds(0.1f);
				UISprite img = obj.GetComponent<UISprite>();
				img.spriteName = "pvp_wave_off";
				yield return new WaitForSeconds(0.3f);
				fx.SetActive(value: false);
			}

			public void Init(int totalWave)
			{
				if (GetEnable())
				{
					gridTf = grid.transform;
					for (int i = 0; i < totalWave; i++)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(item);
						gameObject.transform.parent = gridTf;
						gameObject.transform.localScale = Vector3.one;
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.SetActive(value: true);
					}
					_totalWave = totalWave;
					wave = 1;
					grid.Reposition();
				}
			}

			public void AllAnnihilated()
			{
				if (GetEnable())
				{
					for (int i = _wave; i <= _totalWave; i++)
					{
						GameObject gameObject = gridTf.GetChild(i).gameObject;
						parentPanelBase.StartCoroutine(_CloseMarker(gameObject));
					}
				}
			}
		}

		public UITimeView uiTimeView;

		public UITurnGaugeView uiTurnGaugeView;

		public UIScoreView uiScoreView;

		public UIGetBoxView uiGetBoxView;

		public UIGetGoldView uiGetGoldView;

		public UIView uiPauseView;

		public UIAutoView uiAutoView;

		public UISpeedUpView uiSpeedView;

		public UIUserView uiLhsUserView;

		public UIUserView uiRhsUserView;

		public UIWaveLabelView uiWaveView;

		public UIWaveMarkerView uiLhsWaveMarkerView;

		public UIWaveMarkerView uiRhsWaveMarkerView;

		public UIView uiBtnResultView;

		public UIDefaultListView activeSkillListView;

		public GameObject optionSetPanel;

		public UIOptionController uiOptionController;

		public UIBattleUnitController uiBattleUnitController;

		public GameObject replayPause;

		public List<UIGrid> grids;

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			Type type = GetType();
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].FieldType.IsSubclassOf(typeof(UIView)))
				{
					UIView uIView = fields[i].GetValue(this) as UIView;
					uIView.Init(parent);
				}
			}
		}
	}

	public GameObject cutInPanel;

	public UIBattleUnitPanel uiBattleUnitPanel;

	public UIBattleOpening opening;

	public UIStatusBubble damageBubble;

	public UIStatusBubble criticalDamageBubble;

	public UIStatusBubble missBubble;

	public UIStatusBubble criticalBubble;

	public UIStatusBubble counterBubble;

	public UIStatusBubble comboBubble;

	public UIStatusBubble slideCritical;

	public UIStatusBubble slideCounter;

	public UIStatusBubble slideCombo;

	public UIStatusSkillBubble slideSkill;

	public UICommanderBoardCut commanderBoardCut;

	public UISkillCut skillCut;

	public UIFatalCut fatalCut;

	public UIFlipSwitch flipRepeat;

	public GameObject dummySkip;

	public Main main;

	public EBattleType battleType;

	public BattleData currentBattleData { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		SendOnInitToInnerParts();
	}

	private new void OnEnable()
	{
		UISetter.SetActive(main, active: true);
		UISetter.SetActive(commanderBoardCut, active: false);
		if (uiBattleUnitPanel != null)
		{
			uiBattleUnitPanel.Clean();
			uiBattleUnitPanel.gameObject.SetActive(value: true);
		}
		main.uiGetBoxView.Clean();
		main.uiGetGoldView.Clean();
		main.uiScoreView.InitUI();
		if (main.uiBattleUnitController != null)
		{
			main.uiBattleUnitController.Clean();
			main.uiBattleUnitController.gameObject.SetActive(value: true);
		}
		main.uiLhsUserView.uiGuildItemView.Clean();
		main.uiRhsUserView.uiGuildItemView.Clean();
		if (main.uiLhsUserView.uiCommanderTag != null)
		{
			main.uiLhsUserView.uiCommanderTag.InitUI();
		}
		if (main.uiRhsUserView.uiCommanderTag != null)
		{
			main.uiRhsUserView.uiCommanderTag.InitUI();
		}
		main.uiTurnGaugeView.InitUI();
		SelectSpeed("Speed-x2");
		SelectAutoBattle(isAutoBattle: false);
	}

	public void Set(BattleData battleData)
	{
		currentBattleData = battleData;
		if (currentBattleData == null)
		{
			return;
		}
		Set(battleData.type);
		Set(battleData.attacker, battleData.defender);
		List<int> list = new List<int>();
		Regulation regulation = RemoteObjectManager.instance.regulation;
		EBattleType type = battleData.type;
		switch (type)
		{
		case EBattleType.Plunder:
		{
			WorldMapStageDataRow worldMapStageDataRow = regulation.worldMapStageDtbl[battleData.stageId];
			list.Add(0);
			list.Add(worldMapStageDataRow.turn1);
			list.Add(worldMapStageDataRow.turn2);
			list.Add(worldMapStageDataRow.turn3);
			break;
		}
		case EBattleType.Annihilation:
		{
			AnnihilateBattleDataRow annihilateBattleDataRow = regulation.annihilateBattleDtbl[battleData.stageId];
			list.Add(0);
			list.Add(annihilateBattleDataRow.endTurn);
			break;
		}
		case EBattleType.GuildScramble:
		{
			GuildStruggleDataRow guildStruggleDataRow = regulation.guildStruggleDtbl[battleData.stageId];
			break;
		}
		case EBattleType.Guerrilla:
		{
			SweepDataRow sweepDataRow = regulation.sweepDtbl[$"{battleData.sweepType}_{battleData.sweepLevel}"];
			list.Add(0);
			list.Add(sweepDataRow.endTurn);
			break;
		}
		case EBattleType.Raid:
		{
			RaidChallengeDataRow rcdr = regulation.raidChallengeDtbl[battleData.raidData.raidId.ToString()];
			List<RaidDataRow> list2 = regulation.raidDtbl.FindAll((RaidDataRow x) => x.key == battleData.raidData.raidId && x.pos == rcdr.commanderPos);
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(list2[i].phase);
			}
			break;
		}
		case EBattleType.Duel:
		case EBattleType.Conquest:
		case EBattleType.WorldDuel:
			list.Add(0);
			list.Add(int.Parse(regulation.defineDtbl["ARENA_END_TURN"].value));
			break;
		case EBattleType.WaveDuel:
			list.Add(0);
			list.Add(int.Parse(regulation.defineDtbl["ARENA_3WAVE_END_TURN"].value));
			break;
		case EBattleType.ScenarioBattle:
		{
			ScenarioBattleDataRow scenarioBattleDataRow = regulation.scenarioBattleDtbl[battleData.stageId];
			list.Add(0);
			list.Add(scenarioBattleDataRow.turn1);
			list.Add(scenarioBattleDataRow.turn2);
			list.Add(scenarioBattleDataRow.turn3);
			break;
		}
		case EBattleType.WaveBattle:
		{
			WaveBattleDataRow waveBattleDataRow = regulation.FindWaveBattleData(int.Parse(battleData.stageId));
			list.Add(0);
			list.Add(waveBattleDataRow.endTurn);
			break;
		}
		case EBattleType.CooperateBattle:
		{
			CooperateBattleDataRow cooperateBattleDataRow = regulation.cooperateBattleDtbl[battleData.stageId];
			list.Add(0);
			list.Add(cooperateBattleDataRow.endTurn);
			break;
		}
		case EBattleType.EventBattle:
		{
			EventBattleFieldDataRow eventBattleFieldDataRow = regulation.eventBattleFieldDtbl[$"{battleData.eventId}_{battleData.eventLevel}"];
			list.Add(0);
			if (eventBattleFieldDataRow.clearCondition1 == EBattleClearCondition.LimitedTurn)
			{
				list.Add(int.Parse(eventBattleFieldDataRow.clearCondition1_Value));
			}
			if (eventBattleFieldDataRow.clearCondition2 == EBattleClearCondition.LimitedTurn)
			{
				list.Add(int.Parse(eventBattleFieldDataRow.clearCondition2_Value));
			}
			list.Add(eventBattleFieldDataRow.endTurn);
			break;
		}
		case EBattleType.EventRaid:
		{
			EventRaidDataRow eventRaidDataRow = regulation.eventRaidDtbl[$"{battleData.eventId}_{battleData.eventRaidIdx}"];
			list.Add(0);
			list.Add(eventRaidDataRow.endTurn);
			break;
		}
		case EBattleType.InfinityBattle:
		{
			list.Add(0);
			InfinityFieldDataRow infinityFieldDataRow = regulation.infinityFieldDtbl[battleData.stageId];
			if (infinityFieldDataRow.clearMission01 == EBattleClearCondition.LimitedTurn)
			{
				list.Add(int.Parse(infinityFieldDataRow.clearMission01Count));
			}
			if (infinityFieldDataRow.clearMission02 == EBattleClearCondition.LimitedTurn)
			{
				list.Add(int.Parse(infinityFieldDataRow.clearMission02Count));
			}
			list.Add(infinityFieldDataRow.endTurn);
			break;
		}
		}
		InitTurn(list);
		if (battleData.isReplayMode)
		{
			UISetter.SetActive(main.replayPause, active: true);
		}
		else
		{
			UISetter.SetActive(main.replayPause, active: false);
		}
		currentBattleData = null;
	}

	public void Set(RoUser left, RoUser right)
	{
		if (left == null || right == null)
		{
			return;
		}
		UISetter.SetUser(main.uiLhsUserView.uiUser, left);
		if (main.uiRhsUserView.uiUser != null)
		{
			if (currentBattleData != null && (currentBattleData.type == EBattleType.WaveDuel || currentBattleData.type == EBattleType.Duel || currentBattleData.type == EBattleType.WorldDuel))
			{
				if (int.Parse(right.uno) < ConstValue.NpcStartUno && right.duelRanking != 0)
				{
					main.uiRhsUserView.uiUser.Set(right);
				}
				else
				{
					main.uiRhsUserView.uiUser.SetNpc(right);
				}
			}
			else
			{
				main.uiRhsUserView.uiUser.Set(right);
			}
		}
		if (opening != null)
		{
			opening.Set(left, right);
		}
	}

	public void CloseOpeninig()
	{
		if (opening != null)
		{
			opening.Close();
		}
	}

	public void StartBattle()
	{
		UISetter.SetActive(main.root, active: true);
	}

	public void Set(EBattleType battleType)
	{
		this.battleType = battleType;
		main.uiLhsUserView.SetEnable(enable: true);
		main.uiLhsUserView.uiGuildItemView.SetEnable(enable: true);
		switch (battleType)
		{
		case EBattleType.Plunder:
		case EBattleType.SeaRobber:
		case EBattleType.EventBattle:
		case EBattleType.InfinityBattle:
			main.uiScoreView.SetEnable(enable: false);
			main.uiTurnGaugeView.SetEnable(enable: true);
			main.uiTurnGaugeView.clearRankView.SetEnable(enable: true);
			main.uiTurnGaugeView.turnLabelView.SetEnable(enable: true);
			main.uiGetGoldView.SetEnable(enable: true);
			main.uiGetBoxView.SetEnable(enable: true);
			main.uiWaveView.Init(battleType);
			main.uiWaveView.SetEnable(enable: true);
			main.uiAutoView.SetEnable(enable: true);
			main.uiAutoView.iconLock.SetActive(value: false);
			main.uiPauseView.SetEnable(enable: true);
			main.uiRhsUserView.SetEnable(enable: false);
			break;
		case EBattleType.Annihilation:
			main.uiScoreView.SetEnable(enable: false);
			main.uiTurnGaugeView.SetEnable(enable: true);
			main.uiTurnGaugeView.clearRankView.SetEnable(enable: false);
			main.uiTurnGaugeView.turnLabelView.SetEnable(enable: true);
			main.uiGetGoldView.SetEnable(enable: true);
			main.uiGetBoxView.SetEnable(enable: true);
			main.uiWaveView.SetEnable(enable: true);
			main.uiAutoView.SetEnable(enable: true);
			main.uiAutoView.iconLock.SetActive(value: false);
			main.uiPauseView.SetEnable(enable: true);
			main.uiRhsUserView.SetEnable(enable: false);
			break;
		case EBattleType.Guerrilla:
			main.uiScoreView.SetEnable(enable: false);
			main.uiTurnGaugeView.SetEnable(enable: true);
			main.uiTurnGaugeView.clearRankView.SetEnable(enable: false);
			main.uiTurnGaugeView.turnLabelView.SetEnable(enable: true);
			main.uiGetGoldView.SetEnable(enable: true);
			main.uiGetBoxView.SetEnable(enable: true);
			main.uiWaveView.Init(battleType);
			main.uiWaveView.SetEnable(enable: true);
			main.uiAutoView.iconLock.SetActive(value: false);
			main.uiAutoView.SetEnable(enable: true);
			main.uiPauseView.SetEnable(enable: true);
			main.uiRhsUserView.SetEnable(enable: false);
			break;
		case EBattleType.Raid:
			main.uiScoreView.SetEnable(enable: true);
			main.uiTurnGaugeView.SetEnable(enable: true);
			main.uiTurnGaugeView.clearRankView.SetEnable(enable: false);
			main.uiTurnGaugeView.turnLabelView.SetEnable(enable: true);
			main.uiGetGoldView.SetEnable(enable: false);
			main.uiGetBoxView.SetEnable(enable: false);
			main.uiWaveView.SetEnable(enable: false);
			main.uiAutoView.iconLock.SetActive(value: false);
			main.uiAutoView.SetEnable(enable: true);
			main.uiPauseView.SetEnable(enable: true);
			main.uiRhsUserView.SetEnable(enable: false);
			break;
		case EBattleType.Duel:
		case EBattleType.GuildScramble:
		case EBattleType.Conquest:
		case EBattleType.WorldDuel:
			main.uiScoreView.SetEnable(enable: false);
			main.uiTurnGaugeView.SetEnable(enable: true);
			main.uiTurnGaugeView.clearRankView.SetEnable(enable: false);
			main.uiTurnGaugeView.turnLabelView.SetEnable(enable: true);
			main.uiGetGoldView.SetEnable(enable: false);
			main.uiGetBoxView.SetEnable(enable: false);
			main.uiWaveView.SetEnable(enable: false);
			main.uiAutoView.iconLock.SetActive(value: true);
			main.uiAutoView.SetEnable(enable: true);
			main.uiPauseView.SetEnable(enable: false);
			main.uiRhsUserView.SetEnable(enable: true);
			main.uiRhsUserView.uiGuildItemView.SetEnable(enable: true);
			main.uiBtnResultView.SetEnable(battleType == EBattleType.Duel || battleType == EBattleType.WorldDuel);
			break;
		case EBattleType.WaveDuel:
			main.uiScoreView.SetEnable(enable: false);
			main.uiTurnGaugeView.SetEnable(enable: true);
			main.uiTurnGaugeView.clearRankView.SetEnable(enable: false);
			main.uiTurnGaugeView.turnLabelView.SetEnable(enable: true);
			main.uiGetGoldView.SetEnable(enable: false);
			main.uiGetBoxView.SetEnable(enable: false);
			main.uiWaveView.SetEnable(enable: false);
			main.uiAutoView.iconLock.SetActive(value: true);
			main.uiAutoView.SetEnable(enable: true);
			main.uiPauseView.SetEnable(enable: false);
			main.uiRhsUserView.SetEnable(enable: true);
			main.uiRhsUserView.uiGuildItemView.SetEnable(enable: true);
			main.uiLhsWaveMarkerView.SetEnable(enable: true);
			main.uiRhsWaveMarkerView.SetEnable(enable: true);
			main.uiBtnResultView.SetEnable(enable: true);
			break;
		case EBattleType.ScenarioBattle:
			main.uiScoreView.SetEnable(enable: false);
			main.uiTurnGaugeView.SetEnable(enable: true);
			main.uiTurnGaugeView.clearRankView.SetEnable(enable: false);
			main.uiTurnGaugeView.turnLabelView.SetEnable(enable: true);
			main.uiGetGoldView.SetEnable(enable: false);
			main.uiGetBoxView.SetEnable(enable: false);
			main.uiWaveView.SetEnable(enable: false);
			main.uiAutoView.iconLock.SetActive(value: false);
			main.uiAutoView.SetEnable(enable: true);
			main.uiPauseView.SetEnable(enable: true);
			main.uiRhsUserView.SetEnable(enable: false);
			break;
		case EBattleType.WaveBattle:
			main.uiScoreView.SetEnable(enable: false);
			main.uiTurnGaugeView.SetEnable(enable: true);
			main.uiTurnGaugeView.clearRankView.SetEnable(enable: false);
			main.uiTurnGaugeView.turnLabelView.SetEnable(enable: true);
			main.uiGetGoldView.SetEnable(enable: false);
			main.uiGetBoxView.SetEnable(enable: false);
			main.uiWaveView.SetEnable(enable: true);
			main.uiAutoView.iconLock.SetActive(value: false);
			main.uiAutoView.SetEnable(enable: true);
			main.uiPauseView.SetEnable(enable: true);
			main.uiRhsUserView.SetEnable(enable: false);
			break;
		case EBattleType.CooperateBattle:
			main.uiScoreView.SetEnable(enable: true);
			main.uiTurnGaugeView.SetEnable(enable: true);
			main.uiTurnGaugeView.clearRankView.SetEnable(enable: false);
			main.uiTurnGaugeView.turnLabelView.SetEnable(enable: true);
			main.uiGetGoldView.SetEnable(enable: false);
			main.uiGetBoxView.SetEnable(enable: false);
			main.uiWaveView.SetEnable(enable: true);
			main.uiAutoView.iconLock.SetActive(value: false);
			main.uiAutoView.SetEnable(enable: false);
			main.uiPauseView.SetEnable(enable: true);
			main.uiRhsUserView.SetEnable(enable: false);
			main.uiBtnResultView.SetEnable(enable: false);
			break;
		case EBattleType.EventRaid:
			main.uiScoreView.SetEnable(enable: true);
			main.uiTurnGaugeView.SetEnable(enable: true);
			main.uiTurnGaugeView.clearRankView.SetEnable(enable: false);
			main.uiTurnGaugeView.turnLabelView.SetEnable(enable: true);
			main.uiGetGoldView.SetEnable(enable: false);
			main.uiGetBoxView.SetEnable(enable: false);
			main.uiWaveView.SetEnable(enable: true);
			main.uiAutoView.iconLock.SetActive(value: false);
			main.uiAutoView.SetEnable(enable: true);
			main.uiPauseView.SetEnable(enable: true);
			main.uiRhsUserView.SetEnable(enable: false);
			main.uiBtnResultView.SetEnable(enable: false);
			break;
		}
		if (GameSetting.instance.repeatBattle)
		{
			UISetter.SetActive(flipRepeat.gameObject, active: true);
			UISetter.SetFlipSwitch(flipRepeat, GameSetting.instance.repeatBattle);
		}
	}

	public void SetUICommander(Unit unit)
	{
		if (unit.side == EBattleSide.Left)
		{
			main.uiLhsUserView.Set(unit);
		}
		else
		{
			main.uiRhsUserView.Set(unit);
		}
	}

	public void UICommanderShake(Unit unit)
	{
		if (unit.side == EBattleSide.Left)
		{
			main.uiLhsUserView.Shake(unit);
		}
		else
		{
			main.uiRhsUserView.Shake(unit);
		}
	}

	public void UICommanderState(Unit unit, string state)
	{
		if (unit.side == EBattleSide.Left)
		{
			main.uiLhsUserView.SetState(unit, state);
		}
		else
		{
			main.uiRhsUserView.SetState(unit, state);
		}
	}

	public void UICommanderEnable(EBattleSide side)
	{
		if (side == EBattleSide.Left)
		{
			if (main.uiLhsUserView.uiCommanderTag != null)
			{
				main.uiLhsUserView.uiCommanderTag.EnableCommander(enable: true);
			}
		}
		else if (main.uiRhsUserView.uiCommanderTag != null)
		{
			main.uiRhsUserView.uiCommanderTag.EnableCommander(enable: true);
		}
	}

	public void SelectSpeed(string key)
	{
		main.uiSpeedView.SelectSpeed(key);
	}

	public void SetAutoEnable(bool enable)
	{
		main.uiAutoView.SetEnable(enable);
	}

	public void SetPauseEnable(bool enable)
	{
		main.uiPauseView.SetEnable(enable);
	}

	public void SelectAutoBattle(bool isAutoBattle)
	{
		if (main.uiAutoView.GetEnable())
		{
			UISetter.SetActive(main.uiAutoView.btnManual, isAutoBattle);
		}
	}

	public void SetRemainTime(float time)
	{
		if (main.uiTimeView.GetEnable())
		{
			UISetter.SetLabel(main.uiTimeView.time, Utility.GetTimeStringColonFormat(time));
			UISetter.SetActive(main.uiTimeView.timeWarnRoot, time <= main.uiTimeView.timeWarnThreshold);
		}
	}

	public void Set(EBattleSide side, RoCommander commander)
	{
		switch (side)
		{
		case EBattleSide.Left:
			main.uiLhsUserView.Set(commander);
			break;
		case EBattleSide.Right:
			main.uiRhsUserView.Set(commander);
			break;
		}
	}

	public void InitTurn(int step1, int step2)
	{
		List<int> list = new List<int>();
		list.Add(0);
		list.Add(step1);
		list.Add(step2);
		main.uiTurnGaugeView.InitTurn(list);
	}

	public void InitTurn(List<int> steps)
	{
		if (steps.Count > 0)
		{
			main.uiTurnGaugeView.InitTurn(steps);
		}
	}

	public void SetUITurn(int turn)
	{
		main.uiTurnGaugeView.turn = turn;
	}

	public void SetClearRank(int rank)
	{
		main.uiTurnGaugeView.SetRank(rank);
	}

	public void SetScore(long score)
	{
		main.uiScoreView.scoreVal = score;
	}

	public void SetLhsGuildSkills(List<GuildSkillState> items)
	{
		if (items != null)
		{
			main.uiLhsUserView.SetGuildSkill(items);
		}
	}

	public void SetRhsGuildSkills(List<GuildSkillState> items)
	{
		if (items != null)
		{
			main.uiRhsUserView.SetGuildSkill(items);
		}
	}

	public void SetUnitPanelEnable(bool enable)
	{
		UISetter.SetActive(uiBattleUnitPanel.gameObject, enable);
	}

	public void TakenGold(int takenGold)
	{
		SoundManager.PlaySFX("SE_Battle_ItemGet_coin_001");
		main.uiGetGoldView.takenGold += takenGold;
	}

	public void TakenItem(int takenItemCnt)
	{
		SoundManager.PlaySFX("SE_Battle_ItemGet_box_001");
		main.uiGetBoxView.takenBox += takenItemCnt;
	}

	public void SetLhsInitWave(int totalWave)
	{
		main.uiLhsWaveMarkerView.Init(totalWave);
	}

	public void SetLhsCurrWave(int currWave)
	{
		main.uiLhsWaveMarkerView.wave = currWave;
	}

	public void SetLhsAllAnnihilated()
	{
		main.uiLhsWaveMarkerView.AllAnnihilated();
	}

	public void SetRhsInitWave(int totalWave)
	{
		main.uiWaveView.InitWave(totalWave);
		main.uiRhsWaveMarkerView.Init(totalWave);
	}

	public void SetRhsCurrWave(int currWave)
	{
		main.uiWaveView.wave = currWave;
		main.uiRhsWaveMarkerView.wave = currWave;
	}

	public void SetRhsAllAnnihilated()
	{
		main.uiRhsWaveMarkerView.AllAnnihilated();
	}

	public void RepositionGrids()
	{
		for (int i = 0; i < main.grids.Count; i++)
		{
			if (!(main.grids[i] == null))
			{
				main.grids[i].Reposition();
			}
		}
	}
}
