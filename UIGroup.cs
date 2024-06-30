using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIGroup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIDefaultListView tabListView;

	public UIDefaultListView groupListView;

	private string selectIdx = "1";

	private readonly string tabidPrefix = "Tab-";

	public void InitAndOpenGroup()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			Set("1");
			OpenPopup();
		}
	}

	public void Set(string tabId)
	{
		selectIdx = tabId;
		InitGroupTap();
		InitGroup();
	}

	private void InitGroupTap()
	{
		List<GroupInfoDataRow> list = base.regulation.FindGroupInfoList();
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (GroupInfoDataRow item in list)
		{
			if (!dictionary.ContainsKey(item.tabidx))
			{
				dictionary.Add(item.tabidx, item.tabname);
			}
		}
		base.localUser.badgeGroupCount = 0;
		tabListView.InitGroupTab(dictionary, tabidPrefix);
		tabListView.SetSelection(selectIdx, selected: true);
		tabListView.ResetPosition();
		base.uiWorld.mainCommand.BadgeControl();
	}

	private void InitGroup()
	{
		List<GroupInfoDataRow> list = base.regulation.FindGroupInfoList();
		List<GroupInfoDataRow> list2 = list.FindAll((GroupInfoDataRow row) => row.tabidx == selectIdx);
		list2.Sort(delegate(GroupInfoDataRow a, GroupInfoDataRow b)
		{
			int num = 1;
			int num2 = 1;
			num = (base.localUser.completeRewardGroupList.Contains(int.Parse(a.groupIdx)) ? 2 : ((!base.localUser.isGetRewardGroup(a.groupIdx)) ? 1 : 0));
			num2 = (base.localUser.completeRewardGroupList.Contains(int.Parse(b.groupIdx)) ? 2 : ((!base.localUser.isGetRewardGroup(b.groupIdx)) ? 1 : 0));
			return (num == num2) ? int.Parse(a.groupIdx).CompareTo(int.Parse(b.groupIdx)) : num.CompareTo(num2);
		});
		groupListView.InitGroup(list2, "Group-");
		groupListView.ResetPosition();
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close")
		{
			ClosePopup();
		}
		else if (text.StartsWith(tabidPrefix))
		{
			string id = text.Substring(text.IndexOf("-") + 1);
			tabListView.SetSelection(id, selected: true);
			selectIdx = id;
			InitGroup();
		}
		else if (text.StartsWith("EnableBtn-"))
		{
			string s = text.Substring(text.IndexOf("-") + 1);
			base.network.RequestGetGroupReward(int.Parse(s));
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		Set(selectIdx);
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		if (!bBackKeyEnable)
		{
			bBackKeyEnable = true;
			HidePopup();
		}
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0f);
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
	}

	private void OnAnimClose()
	{
	}
}
