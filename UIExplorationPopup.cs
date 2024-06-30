using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Regulation;
using UnityEngine;

public class UIExplorationPopup : UIPopup
{
	private static UIExplorationPopup _inst;

	public UIGrid grid;

	public ExplorationListItem sourceItem;

	public UIFlipSwitch flipSend;

	public UIFlipSwitch flipComplete;

	[HideInInspector]
	public UIExplorationDetailPopup detailPopup;

	private Dictionary<string, ExplorationListItem> items;

	private bool _open;

	protected override void Awake()
	{
		base.Awake();
		SetRecyclable(recyclable: false);
		items = new Dictionary<string, ExplorationListItem>();
	}

	protected override void OnEnable()
	{
		_inst = this;
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		_inst = null;
		base.OnDisable();
	}

	public void OpenPopup()
	{
		if (!_open)
		{
			_open = true;
			Open();
			OpenAnimation();
		}
	}

	public void ClosePopup()
	{
		if (_open)
		{
			_open = false;
			CloseAnimation();
		}
	}

	private void Start()
	{
		for (int i = 0; i < base.localUser.explorationDtbl.length; i++)
		{
			RoExploration roExploration = base.localUser.explorationDtbl[i];
			roExploration.OnComplete = RefreshCompleteBtn;
			ExplorationListItem explorationListItem = UnityEngine.Object.Instantiate(sourceItem);
			explorationListItem.transform.parent = grid.transform;
			explorationListItem.transform.localPosition = Vector3.zero;
			explorationListItem.transform.localScale = Vector3.one;
			explorationListItem.name = roExploration.mapID;
			explorationListItem.Set(roExploration);
			explorationListItem.gameObject.SetActive(value: true);
			items.Add(roExploration.mapID, explorationListItem);
		}
		ResetBtns();
		grid.Reposition();
		OpenPopup();
	}

	private void OnDestroy()
	{
		for (int i = 0; i < base.localUser.explorationDtbl.length; i++)
		{
			RoExploration roExploration = base.localUser.explorationDtbl[i];
			roExploration.OnComplete = null;
		}
	}

	public void ResetBtns()
	{
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < base.localUser.explorationDtbl.length; i++)
		{
			RoExploration roExploration = base.localUser.explorationDtbl[i];
			if (!flag2 && roExploration.state == EExplorationState.Complete)
			{
				flag2 = true;
			}
			if (!flag && roExploration.state == EExplorationState.Idle)
			{
				for (int j = 0; j < roExploration.commanders.Count; j++)
				{
					RoCommander roCommander = roExploration.commanders[j];
					if (!roCommander.isDispatch)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag && flag2)
			{
				break;
			}
		}
		UISetter.SetFlipSwitch(flipSend, flag);
		UISetter.SetFlipSwitch(flipComplete, flag2);
	}

	public void RefreshCompleteBtn()
	{
		if (flipComplete.GetState() == SwitchStatus.ON)
		{
			return;
		}
		bool state = false;
		for (int i = 0; i < base.localUser.explorationDtbl.length; i++)
		{
			RoExploration roExploration = base.localUser.explorationDtbl[i];
			if (roExploration.state == EExplorationState.Complete)
			{
				state = true;
				break;
			}
		}
		UISetter.SetFlipSwitch(flipComplete, state);
	}

	public void Refresh(string mapID)
	{
		ResetBtns();
		if (items.ContainsKey(mapID))
		{
			items[mapID].OnRefresh();
		}
		if (detailPopup != null && detailPopup.mapID == mapID)
		{
			detailPopup.ResetTargetCommanders();
			detailPopup.ResetSelectCommanders();
			detailPopup.OnRefresh();
		}
	}

	public static void UIRefresh(string mapID)
	{
		if (!(_inst == null))
		{
			_inst.Refresh(mapID);
		}
	}

	public override void OnClick(GameObject sender)
	{
		if (!_open)
		{
			return;
		}
		base.OnClick(sender);
		switch (sender.name)
		{
		case "Close":
			ClosePopup();
			return;
		case "StartAll":
			ExplorationStartAll();
			return;
		case "CompleteAll":
			ExplorationCompleteAll();
			return;
		case "CompleteAllLock":
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7066"));
			return;
		}
		if (!(detailPopup != null))
		{
			RoExploration roExploration = base.localUser.explorationDtbl[sender.name];
			if (roExploration.isOpen)
			{
				detailPopup = UIPopup.Create<UIExplorationDetailPopup>("ExplorationDetailPopup");
				detailPopup.transform.parent = base.transform;
				detailPopup.transform.localPosition = Vector3.zero;
				detailPopup.transform.localScale = Vector3.one;
				detailPopup.Set(roExploration);
				detailPopup.gameObject.SetActive(value: true);
			}
		}
	}

	public void OpenAnimation()
	{
		base.transform.localPosition += new Vector3(0f, -1000f, 0f);
		iTween.MoveTo(base.gameObject, iTween.Hash("y", 0, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeOutBack));
	}

	public void CloseAnimation()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", -1000, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeInBack, "oncomplete", "Close", "oncompletetarget", base.gameObject));
	}

	public void ExplorationStartAll()
	{
		bool flag = false;
		int num = 0;
		Dictionary<string, RoCommander> dictionary = new Dictionary<string, RoCommander>();
		List<Protocols.ExplorationStartInfo> explorationData = new List<Protocols.ExplorationStartInfo>();
		for (int i = 0; i < base.localUser.explorationDtbl.length; i++)
		{
			RoExploration roExploration = base.localUser.explorationDtbl[i];
			if (roExploration.state != 0)
			{
				for (int j = 0; j < roExploration.commanders.Count; j++)
				{
					RoCommander roCommander = roExploration.commanders[j];
					dictionary.Add(roCommander.id, roCommander);
				}
				continue;
			}
			Protocols.ExplorationStartInfo explorationStartInfo = new Protocols.ExplorationStartInfo();
			explorationStartInfo.idx = roExploration.Dr.idx;
			explorationStartInfo.cids = new List<string>();
			for (int k = 0; k < roExploration.commanders.Count; k++)
			{
				RoCommander roCommander2 = roExploration.commanders[k];
				if (!roCommander2.isDispatch && !roCommander2.isExploration)
				{
					explorationStartInfo.cids.Add(roCommander2.id);
					try
					{
						dictionary.Add(roCommander2.id, roCommander2);
					}
					catch (Exception)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				break;
			}
			if (explorationStartInfo.cids.Count > 0)
			{
				explorationData.Add(explorationStartInfo);
				if (explorationStartInfo.cids.Count < 5)
				{
					num++;
				}
			}
		}
		if (flag)
		{
			UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), string.Empty, Localization.Get("5080022"), Localization.Get("1001"));
		}
		else
		{
			if (explorationData.Count <= 0)
			{
				return;
			}
			UISimplePopup uISimplePopup = null;
			uISimplePopup = ((num <= 0) ? UISimplePopup.CreateBool(localization: false, Localization.Get("5080018"), string.Format(Localization.Get("5080019"), explorationData.Count), null, Localization.Get("20007"), Localization.Get("1000")) : UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), string.Format(Localization.Get("5080021"), num), null, Localization.Get("20007"), Localization.Get("1000")));
			uISimplePopup.onClick = delegate(GameObject sender)
			{
				string text = sender.name;
				if (text == "OK")
				{
					Formatting formatting = Formatting.None;
					JsonSerializerSettings serializerSettings = Regulation.SerializerSettings;
					string jsonData = JsonConvert.SerializeObject(explorationData, formatting, serializerSettings);
					RemoteObjectManager.instance.RequestExplorationStartAll(jsonData);
				}
			};
		}
	}

	public void ExplorationCompleteAll()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < base.localUser.explorationDtbl.length; i++)
		{
			RoExploration roExploration = base.localUser.explorationDtbl[i];
			if (roExploration.state == EExplorationState.Complete)
			{
				list.Add(roExploration.Dr.idx);
			}
		}
		if (list.Count > 0)
		{
			RemoteObjectManager.instance.RequestExplorationCompleteAll(list);
		}
	}
}
