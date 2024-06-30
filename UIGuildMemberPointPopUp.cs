using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildMemberPointPopUp : UIPopup
{
	public UIDefaultListView guildMemberListView;

	private List<Protocols.GuildMember.MemberData> memberList;

	public UILabel sortName;

	private ESortType sortType;

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public void InitAndOpenMemberList(List<Protocols.GuildMember.MemberData> list)
	{
		sortType = (ESortType)PlayerPrefs.GetInt("GuildPointSortType", 6);
		memberList = list;
		SetList();
		OpenPopup();
		SetAutoDestroy(autoDestory: true);
	}

	public void SetList()
	{
		if (sortType == ESortType.TotalPoint)
		{
			memberList.Sort(delegate(Protocols.GuildMember.MemberData row, Protocols.GuildMember.MemberData row1)
			{
				if (row.totalPoint < row1.totalPoint)
				{
					return 1;
				}
				return (row.totalPoint > row1.totalPoint) ? (-1) : row1.level.CompareTo(row.level);
			});
			UISetter.SetLabel(sortName, Localization.Get("1054"));
		}
		else if (sortType == ESortType.TodayPoint)
		{
			memberList.Sort(delegate(Protocols.GuildMember.MemberData row, Protocols.GuildMember.MemberData row1)
			{
				if (row.todayPoint < row1.todayPoint)
				{
					return 1;
				}
				return (row.todayPoint > row1.todayPoint) ? (-1) : row1.level.CompareTo(row.level);
			});
			UISetter.SetLabel(sortName, Localization.Get("1055"));
		}
		guildMemberListView.Init(memberList, "Member-");
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text.Contains("-"))
		{
		}
		if (text == "Close")
		{
			ClosePopup();
		}
		else if (text == "BtnSort")
		{
			if (sortType == ESortType.TotalPoint)
			{
				sortType = ESortType.TodayPoint;
			}
			else if (sortType == ESortType.TodayPoint)
			{
				sortType = ESortType.TotalPoint;
			}
			PlayerPrefs.SetInt("GuildPointSortType", (int)sortType);
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
