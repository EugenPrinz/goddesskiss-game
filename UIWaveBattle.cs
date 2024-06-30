using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIWaveBattle : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIDefaultListView targetListView;

	public UISpineAnimation spineAnimation;

	public UITexture goBG;

	private new void Awake()
	{
		UISetter.SetSpine(spineAnimation, "n_013");
		base.Awake();
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

	public void SetWaveData(List<Protocols.WaveBattleInfoList.WaveBattleInfo> waveInfoList)
	{
		if (waveInfoList != null)
		{
			targetListView.InitWaveBattleList(waveInfoList, "wave-");
			base.localUser.CommanderStatusReset();
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
		else if (text.StartsWith("wave-"))
		{
			int battleIdx = int.Parse(text.Substring(text.IndexOf("-") + 1));
			StartReadyBattle(battleIdx);
		}
	}

	public void StartReadyBattle(int battleIdx)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		BattleData battleData = BattleData.Create(EBattleType.WaveBattle);
		battleData.stageId = battleIdx.ToString();
		List<EnemyUnitDataRow> userBattleInfo = regulation.FindNextWaveBattleEnemy(regulation.FindWaveBattleData(battleIdx).enemy.ToString(), 1);
		battleData.defender = RoUser.CreateWaveBattleUser(userBattleInfo);
		UIManager.instance.world.readyBattle.InitAndOpenReadyBattle(battleData);
	}

	public override void OnRefresh()
	{
	}

	public void InitAndOpen()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			Open();
			OpenPopupShow();
		}
	}

	public void OpenPopupShow()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Open()
	{
	}

	public override void Close()
	{
		bBackKeyEnable = true;
		HidePopup();
		base.uiWorld.mainCommand.SetResourceView(EGoods.Bullet);
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
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
