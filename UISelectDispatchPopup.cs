using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectDispatchPopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	[SerializeField]
	private UIDefaultListView commanderListView;

	private List<RoCommander> haveList = new List<RoCommander>();

	[HideInInspector]
	public int curSlotNum;

	public void InitListView(int slotNum)
	{
		InitList();
		commanderListView.InitDispatchPossibleCommanderList(haveList, "Commander_");
		OpenPopup();
		curSlotNum = slotNum;
	}

	private void InitList()
	{
		haveList = RemoteObjectManager.instance.localUser.GetCommanderList(EJob.All, have: true);
		haveList.Sort(delegate(RoCommander row, RoCommander row1)
		{
			if (row.state != ECommanderState.Nomal && row1.state == ECommanderState.Nomal)
			{
				return -1;
			}
			if (row.state == ECommanderState.Nomal && row1.state != ECommanderState.Nomal)
			{
				return 1;
			}
			if (int.Parse(row.id) < int.Parse(row1.id))
			{
				return -1;
			}
			return (int.Parse(row.id) > int.Parse(row1.id)) ? 1 : 0;
		});
	}

	public override void OnClick(GameObject sender)
	{
		if (commanderListView.Contains(sender.name))
		{
			string pureId = commanderListView.GetPureId(sender.name);
			commanderListView.SetSelection(pureId, selected: true);
		}
		if (sender.name.Contains("RemoveCommander-"))
		{
			string text = "RemoveCommander-";
			string id = sender.name.Substring(text.Length);
			commanderListView.SetSelection(id, selected: false);
		}
	}

	public void OpenPopup()
	{
		base.Open();
		AnimBG.Reset();
		AnimBlock.Reset();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
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
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
