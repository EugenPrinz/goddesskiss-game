using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGetCommander : UIPopup
{
	public GEAnimNGUI AnimCenter;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTopBg;

	public GEAnimNGUI AnimTopBgMid;

	public GEAnimNGUI AnimLine;

	public GEAnimNGUI AnimBlock;

	public UICommander commander;

	public UIUnit unit;

	public UILabel description;

	public GameObject getBtn;

	public GameObject replayBtn;

	private string worldMapId;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		AnimCenter.Reset();
		AnimNpc.Reset();
		AnimTopBg.Reset();
		AnimTopBgMid.Reset();
		AnimLine.Reset();
		AnimBlock.Reset();
	}

	public void Init(RoWorldMap world)
	{
		RoCommander roCommander = RoCommander.Create(world.dataRow.c_idx, 1, 1, 1, 0, 0, 0, new List<int>());
		RoUnit roUnit = RoUnit.Create(roCommander.unitId, 1, 1, 1, 0, world.dataRow.c_idx, 0, 0, new List<int>());
		bool flag = world.starCount == world.maxStarCount && !world.rwd;
		if (world.rwd)
		{
			UISetter.SetActive(getBtn, active: false);
			UISetter.SetActive(replayBtn, active: true);
			UISetter.SetLabel(description, Localization.Get("5467"));
		}
		else if (!flag)
		{
			UISetter.SetActive(getBtn, active: false);
			UISetter.SetActive(replayBtn, active: false);
			UISetter.SetLabel(description, Localization.Format("140001", world.maxStarCount));
		}
		else
		{
			UISetter.SetActive(getBtn, active: true);
			UISetter.SetActive(replayBtn, active: false);
			UISetter.SetLabel(description, Localization.Format("140002", Localization.Get(roCommander.nickname)));
		}
		worldMapId = world.id;
		StartCoroutine(SetCommander(roCommander));
		unit.Set(roUnit);
	}

	private IEnumerator SetCommander(RoCommander commanderData)
	{
		yield return StartCoroutine(commander.SetCommander(commanderData));
		if (commander.spine != null)
		{
			UISetter.SetActive(commander.spine, active: false);
			if (commander.spine.target != null)
			{
				commander.spine.SetAnimation("e_10_poisoning");
				UIInteraction component = commander.spine.target.GetComponent<UIInteraction>();
				if (component != null)
				{
					component.EnableInteration = false;
					component.enabled = false;
				}
			}
			UISetter.SetActive(commander.spine, active: true);
		}
		OpenPopupShow();
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Get")
		{
			base.network.RequestWorldMapReward(worldMapId);
		}
		else if (text == "Replay")
		{
			RoWorldMap roWorldMap = base.localUser.FindWorldMap(worldMapId);
			CommanderCompleteType type = CommanderCompleteType.KissReplay;
			RoCommander roCommander = base.localUser.FindCommander(roWorldMap.dataRow.c_idx);
			UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
			uICommanderComplete.Init(type, roCommander.id);
		}
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
		AnimCenter.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBg.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBgMid.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimLine.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
	}

	private void OnAnimClose()
	{
		AnimCenter.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBg.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTopBgMid.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimLine.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
