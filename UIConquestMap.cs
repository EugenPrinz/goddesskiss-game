using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIConquestMap : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	public GEAnimNGUI AnimBottom;

	public UIScrollView mapScrollView;

	public GameObject badgeMail;

	public UILabel mailCount;

	public UILabel goldLabel;

	public UILabel cashLabel;

	public UITimer contentsTimer;

	public GameObject linePrefab;

	public GameObject troopPrefab;

	private Dictionary<int, List<int>> lineDic = new Dictionary<int, List<int>>();

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIDefaultListView stageListView;

	public GameObject lineRoot;

	public GameObject troopRoot;

	public GameObject radarCheck;

	public List<UIConquestStage> stageList;

	public GameObject radarBtn;

	public GameObject currentStateBtn;

	public GameObject PreviousStateBtn;

	public GameObject radarStateRoot;

	public GameObject radarStartBtn;

	public UITimer radarRefreshTimer;

	private TimeData radarDelayTime;

	private TimeData radarRefreshTime;

	public UILabel radarCost;

	public UILabel radarStateLabel;

	public GameObject radarEffect;

	[HideInInspector]
	public Protocols.ConquestTroopInfo.Enemy eGuild;

	[HideInInspector]
	public List<UIConquestTroop> troopList;

	private List<GameObject> lineList;

	private UIConquestDeckPopup deckPopup;

	private UIConquestTroopInformation troopInfoPopup;

	public UIConquestCurrentStatePopup currentPopup;

	private UIWebviewPopup infoPopUp;

	[HideInInspector]
	public UIConquestStageInfoPopup stagePopup;

	[HideInInspector]
	public int mainStageId;

	[HideInInspector]
	public int enemyStageId;

	[HideInInspector]
	public int selectPoint;

	private bool radarState;

	private bool radarRefresh;

	private int GUILD_OCCUPY_RADAR_REFRESH_DELAY;

	private int GUILD_OCCUPY_RADAR_PRICE;

	private UIConquestEnemyGuildInformation eGuildPopup;

	private EConquestState conquestType;

	private Protocols.GetRadarData.Radar radarData;

	private int TimeOverCount;

	private int radarStartTime;

	private string infoUrl
	{
		get
		{
			string language = Localization.language;
			language = language.Substring(2, language.Length - 2).ToLower();
			return $"http://gkcdn.flerogames.com/guide/android/guide_guildoccupy.html#{language}";
		}
	}

	public void InitAndOpenConquestMap()
	{
		if (!bEnterKeyEnable)
		{
			selectPoint = 0;
			radarRefresh = false;
			bEnterKeyEnable = true;
			OpenPopupShow();
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		switch (text)
		{
		case "Close":
			if (!bBackKeyEnable)
			{
				ClosePopUp();
			}
			return;
		case "BtnRadar":
			if (!(radarRefreshTime.GetRemain() > 0.0))
			{
				radarState = !radarState;
				if (radarState)
				{
					base.network.RequestGetConquestRadar();
				}
				else
				{
					SetRadar();
				}
			}
			return;
		case "StartRadar":
			if (radarDelayTime.GetRemain() > 0.0)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Format("110347", Utility.GetTimeSimpleString(radarDelayTime.GetRemain())));
			}
			else
			{
				StartRadar();
			}
			return;
		case "BtnCurrentState":
			base.network.RequestGetConquestCurrentStateInfo();
			return;
		case "BtnPreviousResult":
			base.network.RequestGetGuildRanking(1);
			return;
		case "BtnNotice":
			base.network.RequestGetConquestNotice();
			return;
		case "Mail":
			base.network.RequestMailList();
			return;
		case "Link-CashShop":
			base.uiWorld.mainCommand.OpenDiamonShop();
			return;
		case "Link-GoldShop":
			base.uiWorld.camp.GoNavigation("MetroBank");
			return;
		case "InfoBtn":
			if (infoPopUp == null)
			{
				infoPopUp = UIPopup.Create<UIWebviewPopup>("UIHelpWebView");
				infoPopUp.Init(infoUrl);
			}
			return;
		}
		if (text.StartsWith("ConquestTroop-"))
		{
			string s = text.Substring(text.IndexOf("-") + 1);
			CreateConquestTroopInformation(int.Parse(s));
		}
		else if (stageListView.Contains(text))
		{
			string pureId = stageListView.GetPureId(text);
			selectPoint = int.Parse(pureId);
			OnStageClicked();
		}
	}

	public void _Set(Protocols.ConquestTroopInfo info)
	{
		base.localUser.conquestDeckSlotState = info.slot;
		eGuild = info.eGuild;
		foreach (KeyValuePair<int, Protocols.ConquestTroopInfo.Troop> item in info.squard)
		{
			base.localUser.conquestDeck[item.Key] = item.Value;
			if (base.localUser.conquestDeck[item.Key].remainData == null)
			{
				base.localUser.conquestDeck[item.Key].remainData = new TimeData();
			}
			item.Value.remainData.SetByDuration(item.Value.remain);
			foreach (string value in item.Value.deck.Values)
			{
				RoCommander roCommander = base.localUser.FindCommander(value);
				roCommander.conquestDeckId = item.Key;
			}
		}
		InitData();
		ConquestTroopCheck();
		radarState = false;
		SetRadar();
	}

	private void SetResource()
	{
		UISetter.SetActive(badgeMail, base.localUser.badgeNewMailCount > 0);
		UISetter.SetLabel(mailCount, base.localUser.badgeNewMailCount);
		UISetter.SetLabel(goldLabel, base.localUser.gold.ToString("N0"));
		UISetter.SetLabel(cashLabel, base.localUser.cash.ToString("N0"));
	}

	private void InitData()
	{
		if (stageList == null)
		{
			stageList = new List<UIConquestStage>();
		}
		else
		{
			stageList.Clear();
		}
		if (troopList == null)
		{
			troopList = new List<UIConquestTroop>();
		}
		if (lineList == null)
		{
			lineList = new List<GameObject>();
		}
		List<GuildOccupyDataRow> conquestList = new List<GuildOccupyDataRow>();
		base.regulation.guildOccupyDtbl.ForEach(delegate(GuildOccupyDataRow row)
		{
			conquestList.Add(row);
		});
		mainStageId = ((base.localUser.conquestTeam != EConquestTeam.Red) ? 1 : conquestList.Count);
		enemyStageId = ((base.localUser.conquestTeam == EConquestTeam.Red) ? 1 : conquestList.Count);
		stageListView.Init(conquestList, stageList, "conquestStage-");
		if (lineList.Count == 0)
		{
			radarDelayTime = new TimeData();
			radarRefreshTime = new TimeData();
			GUILD_OCCUPY_RADAR_REFRESH_DELAY = int.Parse(base.regulation.defineDtbl["GUILD_OCCUPY_RADAR_REFRESH_DELAY"].value);
			GUILD_OCCUPY_RADAR_PRICE = int.Parse(base.regulation.defineDtbl["GUILD_OCCUPY_RADAR_PRICE"].value);
			SetStageLine();
			SetResource();
		}
		for (int num = troopList.Count - 1; num >= 0; num--)
		{
			UIConquestTroop uIConquestTroop = troopList[num];
			uIConquestTroop.DeleteTroop();
		}
		troopList.Clear();
		conquestType = base.uiWorld.guild.conquestInfo.state;
		UISetter.SetTimer(contentsTimer, base.uiWorld.guild.conquestTime);
		UISetter.SetLabel(radarCost, GUILD_OCCUPY_RADAR_PRICE);
		UISetter.SetActive(radarBtn, conquestType == EConquestState.Setting);
		UISetter.SetActive(currentStateBtn, conquestType == EConquestState.Setting);
		mapScrollView.MoveRelative(-mapScrollView.transform.localPosition);
		if (base.localUser.conquestTeam == EConquestTeam.Red)
		{
			mapScrollView.MoveRelative(new Vector3(-620f, 0f, 0f));
		}
	}

	private void SetStageLine()
	{
		int cnt = 0;
		int dbCnt = base.regulation.guildOccupyDtbl.length;
		base.regulation.guildOccupyDtbl.ForEach(delegate(GuildOccupyDataRow row)
		{
			if (cnt < dbCnt)
			{
				for (int i = 0; i < row.next.Count; i++)
				{
					if (row.next[i] != 0 && !existLine(cnt, row.next[i] - 1))
					{
						if (!lineDic.ContainsKey(cnt))
						{
							lineDic.Add(cnt, new List<int>());
						}
						lineDic[cnt].Add(row.next[i] - 1);
						CreateStageLine(cnt, row.next[i] - 1);
					}
				}
				cnt++;
			}
		});
	}

	private void CreateStageLine(int idx, int next)
	{
		Vector3 localPosition = (stageList[idx].transform.localPosition + stageList[next].transform.localPosition) * 0.5f;
		Vector3 localPosition2 = stageList[next].transform.localPosition;
		GameObject gameObject = Object.Instantiate(linePrefab);
		UISetter.SetActive(gameObject, active: true);
		gameObject.transform.parent = lineRoot.transform;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.transform.localPosition = localPosition;
		gameObject.GetComponent<UISprite>().width = (int)Vector3.Distance(stageList[idx].transform.localPosition, stageList[next].transform.localPosition);
		lineList.Add(gameObject);
		float y = localPosition.y - localPosition2.y;
		float x = localPosition.x - localPosition2.x;
		float z = Mathf.Atan2(y, x) * 57.29578f;
		gameObject.transform.rotation = Quaternion.Euler(0f, 0f, z);
	}

	private bool existLine(int idx, int next)
	{
		if (lineDic.ContainsKey(next))
		{
			for (int i = 0; i < lineDic[next].Count; i++)
			{
				if (lineDic[next][i] == idx)
				{
					return true;
				}
			}
		}
		return false;
	}

	public GameObject CreateConquestTroop()
	{
		GameObject gameObject = Object.Instantiate(troopPrefab);
		UISetter.SetActive(gameObject, active: true);
		gameObject.transform.parent = troopRoot.transform;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		return gameObject;
	}

	public void CreateConquestTroopInformation(int id)
	{
		if (troopInfoPopup == null)
		{
			Protocols.ConquestTroopInfo.Troop troop = base.localUser.conquestDeck[id];
			troopInfoPopup = UIPopup.Create<UIConquestTroopInformation>("ConquestTroopInformation");
			troopInfoPopup.Init(id, troop);
		}
	}

	public void CreateConquestCurrentPopup(List<Protocols.ConquestStageInfo.User> list)
	{
		if (currentPopup == null)
		{
			currentPopup = UIPopup.Create<UIConquestCurrentStatePopup>("ConquestCurrentStatePopup");
			currentPopup.Init(list);
		}
	}

	public void CreateConquestEnemyGuildPopup()
	{
		if (eGuildPopup == null)
		{
			eGuildPopup = UIPopup.Create<UIConquestEnemyGuildInformation>("ConquestEnemyGuildInformation");
			eGuildPopup.Init(eGuild);
		}
	}

	public void OnStageClicked()
	{
		if (conquestType == EConquestState.Join)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110348"));
		}
		else if (selectPoint == enemyStageId)
		{
			CreateConquestEnemyGuildPopup();
		}
		else if (selectPoint == mainStageId)
		{
			if (deckPopup == null)
			{
				deckPopup = UIPopup.Create<UIConquestDeckPopup>("ConquestDeckPopup");
				deckPopup.Init(EConquestStageInfoType.Main);
			}
		}
		else if (stagePopup == null)
		{
			base.network.RequestGetConquestStageInfo(selectPoint);
		}
	}

	public void CreateStageInfoPopup(Protocols.ConquestStageInfo info)
	{
		if (stagePopup == null)
		{
			stagePopup = UIPopup.Create<UIConquestStageInfoPopup>("ConquestStageInfoPopup");
		}
		stagePopup.Init(selectPoint, info);
	}

	public void StartRadar()
	{
		if (base.localUser.cash >= GUILD_OCCUPY_RADAR_PRICE)
		{
			base.network.RequestStartConquestRadar();
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				base.uiWorld.mainCommand.OpenDiamonShop();
			}
		};
	}

	public void SetRadar()
	{
		UISetter.SetActive(radarCheck, radarState);
		for (int i = 0; i < stageList.Count; i++)
		{
			UIConquestStage uIConquestStage = stageList[i];
			if (i + 1 == mainStageId || i + 1 == enemyStageId)
			{
				uIConquestStage.RadarState(state: false);
			}
			else
			{
				uIConquestStage.RadarState(radarState);
			}
		}
		if (!radarState)
		{
			radarRefreshTime.SetByDuration(GUILD_OCCUPY_RADAR_REFRESH_DELAY);
			UISetter.SetActive(radarRefreshTimer, active: true);
			UISetter.SetTimer(radarRefreshTimer, radarRefreshTime);
			radarRefreshTimer.RegisterOnFinished(delegate
			{
				UISetter.SetActive(radarRefreshTimer, active: false);
			});
			CancelInvoke();
			UISetter.SetLabel(radarStateLabel, string.Empty);
		}
		UISetter.SetActive(radarStateRoot, radarState);
		UISetter.SetActive(radarStartBtn, radarState);
	}

	public void StartRadar(Protocols.GetRadarData.Radar data)
	{
		StartCoroutine(SetRadarData(data));
	}

	public IEnumerator SetRadarData(Protocols.GetRadarData.Radar data)
	{
		radarData = data;
		if (data.remain > 0)
		{
			radarDelayTime.SetByDuration(data.remain);
		}
		if (radarStartTime < data.startTime)
		{
			radarStartTime = data.startTime;
			SoundManager.PlaySFX("EFM_StartExploration_001");
			UISetter.SetActive(radarEffect, active: true);
			yield return new WaitForSeconds(1f);
			UISetter.SetActive(radarEffect, active: false);
			SoundManager.PlaySFX("EFM_StartExploration_001");
			UISetter.SetActive(radarEffect, active: true);
			yield return new WaitForSeconds(1f);
			UISetter.SetActive(radarEffect, active: false);
		}
		UISetter.SetActive(radarStateRoot, data.overTime > 0);
		if (data.overTime > 0)
		{
			TimeOverCount = data.overTime;
			CancelInvoke();
			InvokeRepeating("CountUp", 1f, 1f);
		}
		for (int i = 0; i < stageList.Count; i++)
		{
			UIConquestStage uIConquestStage = stageList[i];
			if (data.info != null && data.info.ContainsKey(i + 1))
			{
				uIConquestStage.SetRadar(data.info[i + 1]);
			}
			else
			{
				uIConquestStage.SetRadar(null);
			}
		}
	}

	private void CountUp()
	{
		TimeOverCount++;
		UISetter.SetLabel(radarStateLabel, Localization.Format("110340", Utility.GetTimeStringSimpleStringColonFormat(TimeOverCount), radarData.uName));
	}

	private void ConquestTroopCheck()
	{
		foreach (KeyValuePair<int, Protocols.ConquestTroopInfo.Troop> item in base.localUser.conquestDeck)
		{
			if (base.localUser.conquestDeck[item.Key] == null)
			{
				continue;
			}
			bool flag = false;
			for (int i = 0; i < troopList.Count; i++)
			{
				UIConquestTroop uIConquestTroop = troopList[i];
				if (item.Key == uIConquestTroop.number)
				{
					flag = true;
				}
			}
			if (base.localUser.conquestDeck[item.Key].remainData != null && base.localUser.conquestDeck[item.Key].remainData.GetRemain() > 0.0 && !flag)
			{
				UIConquestTroop component = CreateConquestTroop().GetComponent<UIConquestTroop>();
				component.Set(item.Key, base.localUser.conquestDeck[item.Key].path, (int)base.localUser.conquestDeck[item.Key].remainData.GetRemain(), base.localUser.conquestDeck[item.Key].mvtm, base.localUser.conquestDeck[item.Key].ucash);
				troopList.Add(component);
			}
		}
	}

	private void ConquestStageArrowCheck()
	{
		if (stageList != null)
		{
			for (int i = 0; i < stageList.Count; i++)
			{
				UIConquestStage uIConquestStage = stageList[i];
				uIConquestStage.SetArrowCheck();
			}
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		if (deckPopup != null)
		{
			deckPopup.Init((selectPoint != mainStageId) ? EConquestStageInfoType.Move : EConquestStageInfoType.Main);
		}
		else if (stagePopup != null)
		{
			stagePopup.OnRefresh();
		}
		ConquestStageArrowCheck();
		ConquestTroopCheck();
		SetResource();
	}

	private void OpenPopupShow()
	{
		Open();
		OnAnimOpen();
	}

	public override void Open()
	{
		UIPanel component = GetComponent<UIPanel>();
		if (component != null)
		{
			UIManager.instance.world.mainCommand.SetChatPanelDepth(component.depth + 1);
		}
		base.Open();
	}

	public override void Close()
	{
		UIManager.instance.world.mainCommand.ResetChatPanelDepth();
		UIManager.instance.world.mainCommand.chat.CloseChat();
		UISetter.SetActive(radarEffect, active: false);
		base.Close();
	}

	public void ClosePopUp()
	{
		bBackKeyEnable = true;
		CancelInvoke();
		if (deckPopup != null)
		{
			deckPopup.ClosePopUp();
		}
		if (troopInfoPopup != null)
		{
			troopInfoPopup.Close();
		}
		if (currentPopup != null)
		{
			currentPopup.Close();
		}
		if (stagePopup != null)
		{
			stagePopup.Close();
		}
		if (infoPopUp != null)
		{
			infoPopUp.Close();
		}
		HidePopup();
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
	}

	private void OnAnimClose()
	{
	}
}
