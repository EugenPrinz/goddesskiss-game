using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildMemberJoinPopUp : UIPopup
{
	public UIDefaultListView guildJoinListView;

	private List<Protocols.GuildMember.MemberData> memberList;

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public void InitAndOpenMemberList(List<Protocols.GuildMember.MemberData> list)
	{
		memberList = list;
		SetList();
		OpenPopup();
		SetAutoDestroy(autoDestory: true);
	}

	public void SetList()
	{
		guildJoinListView.Init(memberList, "Join-");
		UISetter.SetLabel(message, Localization.Format("110106", memberList.Count));
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		Protocols.GuildMember.MemberData memberData = null;
		string id = string.Empty;
		if (text.Contains("-"))
		{
			id = text.Substring(text.IndexOf("-") + 1);
			if (!string.IsNullOrEmpty(id))
			{
				memberData = memberList.Find((Protocols.GuildMember.MemberData list) => list.uno == int.Parse(id));
			}
		}
		if (text == "Close")
		{
			ClosePopup();
		}
		else if (text.StartsWith("BtnReject-"))
		{
			base.network.RequestRefuseGuildJoin(memberData.uno);
		}
		else if (text.StartsWith("BtnApprove-"))
		{
			base.network.RequestApproveGuildJoin(memberData.uno);
		}
	}

	public void RemoveJoinMember(int uno)
	{
		Protocols.GuildMember.MemberData memberData = memberList.Find((Protocols.GuildMember.MemberData list) => list.uno == uno);
		if (memberData != null)
		{
			memberList.Remove(memberData);
			SetList();
		}
	}

	public void AddGildMember(int uno)
	{
		Protocols.GuildMember.MemberData memberData = memberList.Find((Protocols.GuildMember.MemberData list) => list.uno == uno);
		if (memberData != null)
		{
			UIManager.instance.world.guild.AddMemberList(memberData);
			memberList.Remove(memberData);
			SetList();
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
