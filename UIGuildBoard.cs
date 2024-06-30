using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class UIGuildBoard : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIDefaultListView itemListView;

	public UILabel page;

	public GameObject leftBtn;

	public GameObject rightBtn;

	public UILabel inputLabel;

	public UIInput input;

	private bool isSend;

	private bool isRefresh;

	private int sendDelay;

	private int refreshDelay;

	private int nowPage;

	private int maxPage;

	private int minPage;

	private int pageDelay;

	private int pageDelayCount;

	private int GUILD_BOARD_SEND_DELAY;

	private int GUILD_BOARD_REFRESH_DELAY;

	private int GUILD_BOARD_PAGE_DELAY;

	private int GUILD_BOARD_PAGE_COUNT;

	public void InitAndOpenGuildBoard()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			InitData();
			OpenPopup();
		}
	}

	private void InitData()
	{
		GUILD_BOARD_SEND_DELAY = int.Parse(base.regulation.defineDtbl["GUILD_BOARD_SEND_DELAY"].value);
		GUILD_BOARD_REFRESH_DELAY = int.Parse(base.regulation.defineDtbl["GUILD_BOARD_REFRESH_DELAY"].value);
		GUILD_BOARD_PAGE_DELAY = int.Parse(base.regulation.defineDtbl["GUILD_BOARD_PAGE_DELAY"].value);
		GUILD_BOARD_PAGE_COUNT = int.Parse(base.regulation.defineDtbl["GUILD_BOARD_PAGE_COUNT"].value);
		sendDelay = GUILD_BOARD_SEND_DELAY;
		refreshDelay = GUILD_BOARD_REFRESH_DELAY;
		pageDelay = 0;
		pageDelayCount = GUILD_BOARD_PAGE_COUNT;
		isRefresh = true;
		isSend = true;
		minPage = 1;
		nowPage = 0;
		UISetter.SetLabel(inputLabel, Localization.Get("110056"));
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close")
		{
			ClosePopup();
			return;
		}
		if (text.StartsWith("Remove-"))
		{
			string text2 = text.Replace("Remove-", string.Empty);
			string[] array = text2.Split('/');
			RemoveMessage(array[0], array[1]);
			return;
		}
		switch (text)
		{
		case "BtnRefresh":
			RefreshMessage();
			break;
		case "BtnSend":
			SendMessage();
			break;
		case "LeftArrow":
			OnLeftBtnClicked();
			break;
		case "RightArrow":
			OnRightBtnClicked();
			break;
		}
	}

	public void SetChattingData(int now, int max, List<Protocols.GuildBoardData> List)
	{
		nowPage = now;
		maxPage = max;
		itemListView.Init(List);
		itemListView.ResetPosition();
		SetPageControl();
		StopCoroutine("RefreshDelay");
		StartCoroutine("RefreshDelay");
	}

	private void SetPageControl()
	{
		UISetter.SetLabel(page, $"{nowPage} / {maxPage}");
	}

	public void SendMessage()
	{
		if (input.value.Length == 0)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110056"));
			return;
		}
		if (!isSend)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7055", sendDelay));
			return;
		}
		string msg = Regex.Replace(input.value, "(?<!\r)\n", " ");
		base.network.RequestGuildBoardWrite(msg, 1);
		input.value = string.Empty;
		StartCoroutine("SendDelay");
	}

	private void RefreshMessage()
	{
		if (!isRefresh)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7055", refreshDelay));
		}
		else
		{
			base.network.RequestGetGuildBoard(1);
		}
	}

	private void RemoveMessage(string idx, string name)
	{
		UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: false, Localization.Get("110059"), Localization.Format("110063", name), null, Localization.Get("110088"), Localization.Get("1000"));
		uISimplePopup.onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				base.network.RequestGuildBoardDelete(int.Parse(idx), nowPage);
			}
		};
	}

	public void OnRightBtnClicked()
	{
		if (nowPage >= maxPage)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110053"));
			return;
		}
		if (pageDelay == 0)
		{
			StartCoroutine("PageDelay");
		}
		else if (pageDelayCount == 0)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7055", pageDelay));
			return;
		}
		pageDelayCount--;
		nowPage++;
		base.network.RequestGetGuildBoard(nowPage);
	}

	public void OnLeftBtnClicked()
	{
		if (nowPage <= minPage)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110025"));
			return;
		}
		if (pageDelay == 0)
		{
			StartCoroutine("PageDelay");
		}
		else if (pageDelayCount == 0)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("7055", pageDelay));
			return;
		}
		pageDelayCount--;
		nowPage--;
		base.network.RequestGetGuildBoard(nowPage);
	}

	private IEnumerator SendDelay()
	{
		for (isSend = false; sendDelay > 0; sendDelay--)
		{
			yield return new WaitForSeconds(1f);
		}
		isSend = true;
		sendDelay = GUILD_BOARD_SEND_DELAY;
		yield return true;
	}

	private IEnumerator RefreshDelay()
	{
		for (isRefresh = false; refreshDelay > 0; refreshDelay--)
		{
			yield return new WaitForSeconds(1f);
		}
		isRefresh = true;
		refreshDelay = GUILD_BOARD_REFRESH_DELAY;
		yield return true;
	}

	private IEnumerator PageDelay()
	{
		pageDelayCount = GUILD_BOARD_PAGE_COUNT;
		for (pageDelay = GUILD_BOARD_PAGE_DELAY; pageDelay > 0; pageDelay--)
		{
			yield return new WaitForSeconds(1f);
		}
		yield return true;
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
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

	public override void Close()
	{
		StopCoroutine("SendDelay");
		StopCoroutine("RefreshDelay");
		StopCoroutine("PageDelay");
		SetAutoDestroy(autoDestory: true);
		base.Close();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		Close();
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
		AnimBlock.Reset();
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
