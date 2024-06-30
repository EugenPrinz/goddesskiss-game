using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UISituation : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public GameObject[] explorationTidcket;

	public UIButton rechargeExploration;

	public UILabel rechargeExplorationCount;

	public UILabel explorationCount;

	public GameObject[] sweepTidcket;

	public UIButton rechargeSweep;

	public UILabel rechargSweepCount;

	public UILabel sweepCount;

	public List<GameObject> buttons;

	public List<GameObject> blocks;

	public List<UILabel> names;

	private UITimer timer = new UITimer();

	public UISpineAnimation spineAnimation;

	public UITexture goBG;

	private void Start()
	{
		UISetter.SetSpine(spineAnimation, "n_008");
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

	public void InitAndOpenSituation()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			OpenPopupShow();
		}
	}

	public void SetSweepEnable(int id, int remain)
	{
		DailyEventDataRow dailyEventDataRow = base.regulation.dailyEventDtbl[id.ToString()];
		int type = dailyEventDataRow.type;
		for (int i = 0; i < blocks.Count; i++)
		{
			if (type != 6)
			{
				UISetter.SetActive(blocks[i], i + 1 != type);
			}
			else
			{
				UISetter.SetActive(blocks[i], active: false);
			}
		}
		SetSweepNameLabel();
		if (remain > 0)
		{
			TimeData timeData = TimeData.Create();
			timeData.SetByDuration(remain);
			timer.RegisterOnFinished(delegate
			{
				base.network.RequestSituationInformation();
			});
			timer.Set(timeData);
		}
	}

	private void SetSweepNameLabel()
	{
		int i;
		for (i = 0; i < names.Count; i++)
		{
			DailyEventDataRow dailyEventDataRow = base.regulation.dailyEventDtbl.Find((DailyEventDataRow row) => row.week == (EWeekType)(i + 1));
			UISetter.SetLabel(names[i], Localization.Get(dailyEventDataRow.name));
		}
	}

	public void SetTidcket()
	{
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
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
			return;
		}
		if (text.StartsWith("BtnWeek_"))
		{
			SoundManager.PlaySFX("BTN_Difficulty_001");
			int type = int.Parse(text.Replace("BtnWeek_", string.Empty));
			base.uiWorld.seaRobberSweep.InitAndOpenSeaRobber(type);
			return;
		}
		switch (text)
		{
		case "BtnCloseInfo":
			break;
		case "BtnInfo":
			break;
		case "BtnEmptyEx":
			break;
		case "BtnEmptySw":
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.SweepTicket);
			break;
		}
	}

	public override void Open()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Close()
	{
		bBackKeyEnable = true;
		base.uiWorld.mainCommand.SetResourceView(EGoods.Bullet);
		HidePopup();
	}

	private void OpenPopupShow()
	{
		base.Open();
		base.uiWorld.mainCommand.SetResourceView(EGoods.SweepTicket);
		OnAnimOpen();
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
