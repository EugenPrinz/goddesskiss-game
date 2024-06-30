using System.Collections.Generic;
using UnityEngine;

public class UISelectSever : UIPopup
{
	public UIFlipSwitch server_10;

	public UIFlipSwitch server_20;

	public UIFlipSwitch server_30;

	public UIFlipSwitch server_40;

	public UIFlipSwitch server_50;

	public UIFlipSwitch server_60;

	public UIFlipSwitch server_Exist;

	public UILabel labelLast;

	public UILabel labelRecommend;

	public UILabel labelNew;

	private int lastConnectServer;

	private int recommendServer;

	private int newServer;

	private List<Protocols.ServerData.ServerInfo> serverInfoList;

	private List<Protocols.ServerData.ServerInfo> serverGroupList;

	public UIDefaultListView ServerListView;

	public UIServerListItem serverItem;

	private string GroupName = "10";

	private int selectIdx;

	private void Start()
	{
	}

	public void Init(Protocols.ServerData serverInfo)
	{
		serverInfoList = serverInfo.serverInfoList;
		selectIdx = RemoteObjectManager.instance.localUser.world;
		SetSelectedGroup(GroupName);
		lastConnectServer = selectIdx;
		recommendServer = serverInfo.recommandServer;
		newServer = serverInfo.newServer;
		UISetter.SetActive(labelRecommend, recommendServer > 0);
		UISetter.SetActive(labelNew, newServer > 0);
		UISetter.SetLabel(labelLast, Localization.Format("19067", lastConnectServer));
		if (recommendServer > 0)
		{
			UISetter.SetLabel(labelRecommend, Localization.Format("19067", recommendServer));
		}
		if (newServer > 0)
		{
			UISetter.SetLabel(labelNew, Localization.Format("19067", newServer));
		}
		base.gameObject.SetActive(value: true);
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		switch (text)
		{
		case "Close":
			SoundManager.PlaySFX("BTN_Negative_001");
			RemoteObjectManager.instance.localUser.world = selectIdx;
			CloseAnimation();
			return;
		case "10":
			GroupName = text;
			SetSelectedGroup(text);
			return;
		case "20":
			GroupName = text;
			SetSelectedGroup(text);
			return;
		case "30":
			GroupName = text;
			SetSelectedGroup(text);
			return;
		case "40":
			GroupName = text;
			SetSelectedGroup(text);
			return;
		case "50":
			GroupName = text;
			SetSelectedGroup(text);
			return;
		case "60":
			GroupName = text;
			SetSelectedGroup(text);
			return;
		case "Exist":
			GroupName = text;
			SetSelectedGroup(text);
			return;
		}
		if (ServerListView.Contains(text))
		{
			string id = ServerListView.GetPureId(text);
			Protocols.ServerData.ServerInfo serverInfo = serverGroupList.Find((Protocols.ServerData.ServerInfo row) => row.idx.ToString() == id);
			if (serverInfo.level == 0 && serverInfo.status >= 3)
			{
				UISimplePopup.CreateOK(localization: true, "19052", "19053", null, "1001");
				return;
			}
			ServerListView.SetSelection(id, selected: true);
			selectIdx = int.Parse(id);
		}
	}

	public void SetSelectedGroup(string key)
	{
		SoundManager.PlaySFX("BTN_Norma_001");
		server_10.Set((key == "10") ? SwitchStatus.ON : SwitchStatus.OFF);
		server_20.Set((key == "20") ? SwitchStatus.ON : SwitchStatus.OFF);
		server_30.Set((key == "30") ? SwitchStatus.ON : SwitchStatus.OFF);
		server_40.Set((key == "40") ? SwitchStatus.ON : SwitchStatus.OFF);
		server_50.Set((key == "50") ? SwitchStatus.ON : SwitchStatus.OFF);
		server_60.Set((key == "60") ? SwitchStatus.ON : SwitchStatus.OFF);
		server_Exist.Set((key == "Exist") ? SwitchStatus.ON : SwitchStatus.OFF);
		switch (key)
		{
		case "10":
			serverGroupList = serverInfoList.FindAll((Protocols.ServerData.ServerInfo row) => row.idx <= 10);
			break;
		case "20":
			serverGroupList = serverInfoList.FindAll((Protocols.ServerData.ServerInfo row) => row.idx > 10 && row.idx <= 20);
			break;
		case "30":
			serverGroupList = serverInfoList.FindAll((Protocols.ServerData.ServerInfo row) => row.idx > 20 && row.idx <= 30);
			break;
		case "40":
			serverGroupList = serverInfoList.FindAll((Protocols.ServerData.ServerInfo row) => row.idx > 30 && row.idx <= 40);
			break;
		case "50":
			serverGroupList = serverInfoList.FindAll((Protocols.ServerData.ServerInfo row) => row.idx > 40 && row.idx <= 50);
			break;
		case "60":
			serverGroupList = serverInfoList.FindAll((Protocols.ServerData.ServerInfo row) => row.idx > 50 && row.idx <= 60);
			break;
		case "Exist":
			serverGroupList = serverInfoList.FindAll((Protocols.ServerData.ServerInfo row) => row.level > 0);
			break;
		}
		serverGroupList.Sort((Protocols.ServerData.ServerInfo row, Protocols.ServerData.ServerInfo row1) => row.idx.CompareTo(row1.idx));
		ServerListView.Init(serverGroupList, "Server_");
		ServerListView.SetSelection(selectIdx.ToString(), selected: true);
	}

	public void OpenAnimation()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", 0, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeOutBack));
	}

	public void CloseAnimation()
	{
		base.gameObject.SetActive(value: false);
		M01_Title m01_Title = Object.FindObjectOfType(typeof(M01_Title)) as M01_Title;
		m01_Title.root_GameStart.gameObject.SetActive(value: true);
		m01_Title.touchMark.gameObject.SetActive(value: true);
		UISetter.SetLabel(m01_Title.serverName, Localization.Format("19067", RemoteObjectManager.instance.localUser.world));
	}

	public override void Open()
	{
		base.Open();
	}

	public override void Close()
	{
		base.Close();
	}
}
