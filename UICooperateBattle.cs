using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICooperateBattle : UIPopup
{
	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	public GEAnimNGUI AnimBottom;

	public UISpineAnimation spineAnimation;

	public GameObject mainView;

	public UITimer timer;

	public UILabel level;

	public UILabel stepTitle;

	public UILabel explanation;

	public UILabel endTurn;

	public UISprite targetUnit;

	public GameObject readyView;

	public UILabel damageInfo;

	public GameObject completeView;

	public GameObject receiveAllRewardBtn;

	public GameObject fxLevelUp;

	public List<CooperateBattleStepItem> stepItems;

	private UICooperateBattleRankingPopup rankingPopup;

	private UICooperateBattleRewardPopup rewardInfoPopup;

	private UISimplePopup _infoPopUp;

	private Protocols.CooperateBattleData _data;

	private bool _open;

	private int _maximumStep;

	private CooperateBattleDataRow _curDr;

	private int _curLevel
	{
		get
		{
			return base.localUser.coopStage;
		}
		set
		{
			base.localUser.coopStage = value;
		}
	}

	private int _curStep
	{
		get
		{
			return base.localUser.coopStep;
		}
		set
		{
			base.localUser.coopStep = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		UISetter.SetSpine(spineAnimation, "n_010");
		_maximumStep = int.Parse(base.regulation.defineDtbl["COOPERATE_BATTLE_STEP"].value);
	}

	public void InitAndOpen(Protocols.CooperateBattleData data)
	{
		UISetter.SetActive(mainView, active: false);
		Set(data);
		OpenPopup();
	}

	public void LevelUp()
	{
		UISetter.SetActive(fxLevelUp, active: true);
	}

	public void Set(Protocols.CooperateBattleData data)
	{
		_data = data;
		base.localUser.cooperateBattleTicket = data.coop.ticket;
		UIManager.instance.world.mainCommand.Set();
		TimeData timeData = TimeData.Create();
		timeData.SetByDuration(Mathf.Abs(data.coop.remain));
		if (data.coop.remain >= 0)
		{
			timer.SetLabelFormat(Localization.Get("17091"), null);
			timer.SetFinishString(Localization.Get("5090032"));
			UISetter.SetTimer(timer, timeData);
			base.localUser.currentCooperateRemainTime = timeData;
		}
		else
		{
			timer.SetLabelFormat(Localization.Get("5050014"), null);
			timer.SetFinishString(null);
			UISetter.SetTimer(timer, timeData);
			base.localUser.currentCooperateRemainTime = TimeData.Create();
		}
		int num = _data.recv.stage;
		if (num <= 0)
		{
			num = 1;
		}
		if (_data.recv.step == _maximumStep)
		{
			num++;
		}
		int num2 = _data.recv.step % _maximumStep + 1;
		_curLevel = _data.coop.stage;
		_curStep = _data.coop.step;
		if (num < _curLevel)
		{
			_curLevel = num;
			_curStep = _maximumStep;
		}
		else if (num == _curLevel)
		{
			if (num2 > _curStep)
			{
				num2 = _curStep;
			}
		}
		else
		{
			num = _curLevel;
			num2 = _curStep;
		}
		for (int i = 0; i < stepItems.Count; i++)
		{
			int num3 = i + 1;
			ECooperateStepState state = ECooperateStepState.Ready;
			if (num3 < num2)
			{
				state = ECooperateStepState.Complete;
			}
			else if (_curLevel == _data.coop.stage)
			{
				if (num3 < _data.coop.step)
				{
					state = ECooperateStepState.Clear;
				}
			}
			else
			{
				state = ECooperateStepState.Clear;
			}
			stepItems[i].Set(state);
		}
		ECooperateStepState state2 = stepItems[_curStep - 1].state;
		List<CooperateBattleDataRow> list = base.regulation.cooperateBattleStepDtbl[_curStep];
		int index = (_curLevel - 1) % list.Count;
		_curDr = list[index];
		UnitDataRow unitDataRow = base.regulation.unitDtbl[_curDr.enemy];
		UISetter.SetLabel(level, _curLevel);
		UISetter.SetLabel(stepTitle, Localization.Get(_curDr.name));
		UISetter.SetLabel(explanation, Localization.Get(_curDr.description));
		UISetter.SetLabel(endTurn, _curDr.endTurn);
		UISetter.SetSprite(targetUnit, $"{unitDataRow.resourceName}_Front");
		UISetter.SetActive(readyView, state2 == ECooperateStepState.Ready);
		UISetter.SetLabel(damageInfo, $"{data.coop.dmg} / {(ulong)(_curDr.goalPoint + (long)_curDr.goalPointIncrease * (long)(_curLevel - 1))}");
		UISetter.SetActive(completeView, state2 != ECooperateStepState.Ready);
		UISetter.SetActive(receiveAllRewardBtn, stepItems[_maximumStep - 1].state == ECooperateStepState.Clear);
	}

	public void OpenPopup()
	{
		if (!_open)
		{
			_open = true;
			Open();
			StartCoroutine(OpenAnimation());
		}
	}

	public void ClosePopup()
	{
		if (_open)
		{
			_open = false;
			if (rankingPopup != null)
			{
				rankingPopup.ClosePopup();
			}
			if (rewardInfoPopup != null)
			{
				rewardInfoPopup.ClosePopup();
			}
			if (_infoPopUp != null)
			{
				_infoPopUp.ClosePopup();
			}
			StartCoroutine(CloseAnimation());
		}
	}

	private IEnumerator OpenAnimation()
	{
		AnimNpc.Reset();
		AnimTitle.Reset();
		AnimBlock.Reset();
		AnimBottom.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBottom.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		yield return new WaitForSeconds(0.5f);
		UISetter.SetActive(mainView, active: true);
	}

	private IEnumerator CloseAnimation()
	{
		base.uiWorld.mainCommand.SetResourceView(EGoods.Bullet);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBottom.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		yield return new WaitForSeconds(0.4f);
		UISetter.SetActive(mainView, active: false);
		yield return new WaitForSeconds(0.4f);
		Close();
	}

	public override void Open()
	{
		base.uiWorld.mainCommand.SetResourceView(EGoods.CooperateBattleTicket);
		base.Open();
	}

	public void SetRankingData(List<Protocols.CooperateBattlePointGuildRankingInfo> data)
	{
		if (rankingPopup == null)
		{
			rankingPopup = UIPopup.Create<UICooperateBattleRankingPopup>("CooperateBattleRankingPopup");
		}
		rankingPopup.Set(data);
	}

	public void SetRankingData(int stage, List<Protocols.CooperateBattlePointUserRankingInfo> data)
	{
		if (!(rankingPopup == null))
		{
			rankingPopup.Set(stage, data);
		}
	}

	public override void OnClick(GameObject sender)
	{
		if (!_open)
		{
			return;
		}
		base.OnClick(sender);
		string text = sender.name;
		switch (text)
		{
		case "Close":
			ClosePopup();
			return;
		case "RewardInfoBtn":
			if (rewardInfoPopup == null)
			{
				rewardInfoPopup = UIPopup.Create<UICooperateBattleRewardPopup>("CooperateBattleRewardPopup");
			}
			return;
		case "RankingBtn":
			base.network.RequestCooperateBattlePointGuildRank();
			return;
		case "ReceiveAllRewardBtn":
			if (stepItems[_maximumStep - 1].state == ECooperateStepState.Clear)
			{
				if (base.localUser.currentCooperateRemainTime.GetRemain() <= 0.0)
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5090029"));
				}
				else
				{
					base.network.RequestCooperateBattleComplete(_curLevel, 0);
				}
			}
			return;
		case "ReadyBtn":
			if (base.localUser.currentCooperateRemainTime.GetRemain() <= 0.0)
			{
				UISimplePopup.CreateOK(localization: true, "1310", "5090010", null, "1001");
				return;
			}
			if (base.localUser.cooperateBattleTicket <= 0)
			{
				UISimplePopup.CreateOK(localization: true, "1310", "5090009", null, "1001");
				return;
			}
			switch (stepItems[_maximumStep - 1].state)
			{
			case ECooperateStepState.Clear:
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5090034"));
				break;
			case ECooperateStepState.Ready:
			{
				BattleData battleData = BattleData.Create(EBattleType.CooperateBattle);
				battleData.defender = GetEnemy();
				battleData.stageLevel = _curLevel;
				battleData.stageId = _curDr.idx;
				base.uiWorld.readyBattle.InitAndOpenReadyBattle(battleData);
				break;
			}
			}
			return;
		case "InfoBtn":
			if (_infoPopUp == null)
			{
				_infoPopUp = UISimplePopup.CreateOK("InformationPopup");
				_infoPopUp.Set(localization: true, "5090001", "5090035", string.Empty, "1001", string.Empty, string.Empty);
			}
			return;
		}
		if (!text.StartsWith("Stage-"))
		{
			return;
		}
		if (base.localUser.currentCooperateRemainTime.GetRemain() <= 0.0)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("5090029"));
			return;
		}
		int num = int.Parse(text.Substring(text.IndexOf("-") + 1));
		for (int i = 1; i < num; i++)
		{
			if (stepItems[i - 1].state != ECooperateStepState.Complete)
			{
				return;
			}
		}
		if (stepItems[num - 1].state == ECooperateStepState.Clear)
		{
			base.network.RequestCooperateBattleComplete(_curLevel, num);
		}
	}

	protected override void OnEnablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: true);
	}

	protected override void OnDisablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: false);
	}

	public RoUser GetEnemy()
	{
		return RoUser.CreateNPC("Enemy-" + 0, "NPC", RoTroop.CreateCooperateBattleEnemy(_curDr.idx));
	}
}
