using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimInfo;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIDefaultListView guildListView;

	public UIDefaultListView guildMemberListView;

	public UIFlipSwitch join;

	public UIFlipSwitch create;

	public GameObject joinGuild;

	public GameObject createGuild;

	public GameObject guildMain;

	public GameObject guildJoinCreate;

	public UIInput searchGuildName;

	public UILabel guildNotice;

	public UILabel guildCount;

	public UILabel guildName;

	public UILabel guildWorld;

	public UISprite guildEmblem;

	public UILabel guildLevel;

	public UILabel guildPoint;

	public GameObject btnManage;

	public GameObject btnSkill;

	public GameObject btnApprove;

	public GameObject btnModify;

	public UILabel sortName;

	public UIInput createGuildName;

	public UIToggle toggleFree;

	public UIToggle toggleApprove;

	public UILabel minLevelLabel;

	public UISprite selectEmblem;

	public UILabel createCostLabel;

	public GameObject guildNoticeRoot;

	private UIGuildBoard guildBoard;

	public GameObject guildBoardBtn;

	public GameObject guildBoardBadge;

	private int minLevel;

	private int emblemIdx;

	private ESortType sortType;

	private int uiGuildState;

	private string guildMasterName;

	public UISpineAnimation spineAnimation;

	public static readonly string guildItemIdPrefix = "GuildItem-";

	public static readonly string memberItemIdPrefix = "MemberItem-";

	private int guildCreateCost;

	public GameObject conquestJoinBtn;

	public GameObject conquestBtn;

	public Protocols.ConquestInfo conquestInfo;

	private EConquestState conquestType;

	public UITimer conquestTimer;

	public TimeData conquestTime;

	public UILabel conquestStateLabel;

	public GameObject conquestBadge;

	public UIConquestHistoryPopup historyPopup;

	private List<RoGuild> guildList;

	public List<Protocols.GuildMember.MemberData> memberList;

	[HideInInspector]
	public UIGuildTroopDispatchPopup dispatch;

	private UISecretShopPopup secretShop;

	[SerializeField]
	private GameObject contentsRoot;

	[SerializeField]
	private GameObject dispatchPrefab;

	private void Start()
	{
		UISetter.SetSpine(spineAnimation, "n_010");
		guildCreateCost = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["GUILD_CREATION_PRICE"].value);
	}

	private void GoWhisperChatting(int uno, string nickName)
	{
		base.uiWorld.mainCommand.chat.AddNickName(uno, nickName);
		base.uiWorld.mainCommand.chat.OnChat(state: true);
		base.uiWorld.mainCommand.chat.InitOpenAndWhisperData();
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string key = sender.name;
		RoGuild guild = null;
		if (key.StartsWith(memberItemIdPrefix))
		{
			int nOkType = 0;
			if (base.localUser.guildInfo.memberGrade != 1)
			{
				return;
			}
			Protocols.GuildMember.MemberData member = memberList.Find((Protocols.GuildMember.MemberData list) => list.uno.ToString() == key.Replace(memberItemIdPrefix, string.Empty));
			if (member.uno.ToString() == base.localUser.uno)
			{
				return;
			}
			UIGuildMemberInfoPopup popup = UIPopup.Create<UIGuildMemberInfoPopup>("GuildMemberInfoPopup");
			popup.Set(localization: true, Localization.Get("110052"), null, null, null, null, Localization.Get("1001"));
			popup.SetInfo(member.name, member.world, member.thumnail, member.level, Utility.GetTimeString(member.lastTime), Localization.Get("110061"), Localization.Get("110054"), (member.memberGrade != 0) ? Localization.Get("110100") : Localization.Get("110092"));
			popup.onClick = delegate(GameObject popupSender)
			{
				switch (popupSender.name)
				{
				case "OK":
					popup.Close();
					if (nOkType == 1)
					{
						base.network.RequestDeportGuildMember(member.uno);
					}
					else if (nOkType == 2)
					{
						base.network.RequestDelegatingGuild(member.uno);
					}
					else if (nOkType == 3)
					{
						base.network.RequestAppointSubMaster(member.uno);
					}
					else if (nOkType == 4)
					{
						base.network.RequestFireSubMaster(member.uno);
					}
					break;
				case "Cancel":
					popup.Set(localization: true, Localization.Get("110052"), null, null, null, null, Localization.Get("1001"));
					popup.SetInfo(member.name, member.world, member.thumnail, member.level, Utility.GetTimeString(member.lastTime), Localization.Get("110061"), Localization.Get("110054"), (member.memberGrade != 0) ? Localization.Get("110100") : Localization.Get("110092"));
					break;
				case "Btn-1":
					popup.Set(localization: true, Localization.Get("110061"), Localization.Get("110062"), null, Localization.Get("110061"), Localization.Get("1000"), null);
					popup.SetInfo(member.name, member.world, member.thumnail, member.level, Utility.GetTimeString(member.lastTime), null, null, null);
					nOkType = 1;
					break;
				case "Btn-2":
					popup.Set(localization: true, Localization.Get("110057"), Localization.Get("110058"), null, Localization.Get("110060"), Localization.Get("1000"), null);
					popup.SetInfo(member.name, member.world, member.thumnail, member.level, Utility.GetTimeString(member.lastTime), null, null, null);
					nOkType = 2;
					break;
				case "Btn-3":
					if (member.memberGrade == 0)
					{
						popup.Set(localization: true, "110093", "110097", null, "110060", "1000", null);
						popup.SetInfo(member.name, member.world, member.thumnail, member.level, Utility.GetTimeString(member.lastTime), null, null, null);
						nOkType = 3;
					}
					else
					{
						popup.Set(localization: true, "110101", "110104", null, "110111", "1000", null);
						popup.SetInfo(member.name, member.world, member.thumnail, member.level, Utility.GetTimeString(member.lastTime), null, null, null);
						nOkType = 4;
					}
					break;
				}
			};
		}
		else if (key.Contains("-"))
		{
			string id = key.Substring(key.IndexOf("-") + 1);
			if (!string.IsNullOrEmpty(id))
			{
				guild = guildList.Find((RoGuild list) => list.gidx == int.Parse(id));
			}
		}
		if (key == "Close")
		{
			if (!bBackKeyEnable)
			{
				Close();
			}
		}
		else if (key == "Join")
		{
			uiGuildState = 0;
			SetTap();
		}
		else if (key == "Create")
		{
			uiGuildState = 1;
			SetTap();
		}
		else if (key == "Search")
		{
			if (!string.IsNullOrEmpty(searchGuildName.value))
			{
				base.network.RequestSearchGuild(searchGuildName.value);
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110217"));
			}
		}
		else if (key == "Refresh")
		{
			base.network.RequestGuildList();
		}
		else if (key.StartsWith("BtnJoin-"))
		{
			base.network.RequestFreeJoinGuild(guild.gidx);
		}
		else if (key.StartsWith("BtnApply-"))
		{
			base.network.RequestApplyGuildJoin(guild.gidx);
		}
		else if (key.StartsWith("BtnCancel-"))
		{
			UISimplePopup.CreateBool(localization: true, Localization.Get("110224"), guild.gnm, Localization.Get("110225"), Localization.Get("1304"), Localization.Get("1305")).onClick = delegate(GameObject popupSender)
			{
				string text3 = popupSender.name;
				if (text3 == "OK")
				{
					base.network.RequestCancelGuildJoin(guild.gidx);
				}
				else if (!(text3 == "Cancel"))
				{
				}
			};
		}
		else if (key == "BtnSort")
		{
			if (sortType == ESortType.Level)
			{
				sortType = ESortType.Time;
			}
			else if (sortType == ESortType.Time)
			{
				sortType = ESortType.PBPoint;
			}
			else if (sortType == ESortType.PBPoint)
			{
				sortType = ESortType.Level;
			}
			PlayerPrefs.SetInt("GuildMemberSortType", (int)sortType);
			SetMemberList();
		}
		else if (key == "BtnDispatch")
		{
			OpenDispatchPopup();
		}
		else if (key == "BtnConquest")
		{
			OnConquestBtnClicked();
		}
		else if (key == "EnterConquest")
		{
			OnConquestJoinBtnClicked();
		}
		else if (key == "BtnManage")
		{
			UIPopup.Create<UIGuildManagePopup>("GuildManagePopup").InitAndOpenGuildManage();
		}
		else if (key == "BtnSkill")
		{
			UIPopup.Create<UIGuildManagePopup>("GuildManagePopup").InitAndOpenGuildManage();
		}
		else if (key == "BtnApprove")
		{
			base.network.RequestManageGuildJoinMember();
		}
		else if (key == "BtnPoint")
		{
			UIPopup.Create<UIGuildMemberPointPopUp>("GuildMemberPointPopUp").InitAndOpenMemberList(memberList);
		}
		else if (key == "BtnEmblem")
		{
			UIImageListPopup uIImageListPopup = UIPopup.Create<UIImageListPopup>("ImageListPopup");
			uIImageListPopup.Set(localization: true, Localization.Get("110020"), null, null, null, Localization.Get("1000"), null);
			uIImageListPopup.SetItemGuildEmblem();
			uIImageListPopup.onClick = delegate(GameObject popupSender)
			{
				string text2 = popupSender.name;
				if (text2.StartsWith("Emblem_"))
				{
					emblemIdx = int.Parse(text2.Replace("Emblem_", string.Empty));
					SetCreateGuild();
				}
			};
		}
		else if (key == "BtnPlus")
		{
			if (minLevel < base.localUser.level)
			{
				minLevel++;
				SetCreateGuild();
			}
		}
		else if (key == "BtnMinus")
		{
			if (minLevel > base.localUser.FindBuilding(EBuilding.Guild).firstLevelReg.userLevel)
			{
				minLevel--;
				SetCreateGuild();
			}
		}
		else if (key == "BtnCreate")
		{
			CreateGuild();
		}
		else if (key == "BtnModify")
		{
			UIReceiveUserString recv = UIPopup.Create<UIReceiveUserString>("InputUserStringBig");
			recv.SetDefault(base.localUser.guildInfo.notice);
			recv.Set(localization: true, Localization.Get("110027"), null, null, Localization.Get("110040"), Localization.Get("1000"), null);
			recv.SetLimitLength(60);
			recv.onClick = delegate(GameObject popupSender)
			{
				string text = popupSender.name;
				if (text == "OK")
				{
					base.network.RequestUpdateGuildInfo(4, recv.input.value);
					recv.ClosePopup();
				}
				else if (text == "Close")
				{
					recv.ClosePopup();
				}
			};
		}
		else if (key == "ShopBtn")
		{
			if (secretShop == null)
			{
				secretShop = UIPopup.Create<UISecretShopPopup>("SecretShopPopup");
				secretShop.Init(EShopType.GuildShop);
			}
		}
		else if (key == "GuildBoard")
		{
			base.network.RequestGetGuildBoard(0);
		}
		else if (key == "BtnCooperateBattle")
		{
			int num = int.Parse(base.regulation.defineDtbl["COOPERATE_BATTLE_OPEN_GUILD_LEVEL"].value);
			if (base.localUser.guildInfo.level < num)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Format("110089", num));
			}
			else
			{
				base.network.RequestCooperateBattleInfo();
			}
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		UISetter.SetActive(guildBoardBtn, base.localUser.IsExistGuild());
		if (base.localUser.IsExistGuild())
		{
			InitAndOpenGuildInfo(memberList);
		}
		else if (uiGuildState == 0)
		{
			InitAndOpenGuildList(guildList);
		}
		if (dispatch != null)
		{
			dispatch.Set();
		}
		if (secretShop != null)
		{
			secretShop.OnRefresh();
		}
	}

	public void InitAndOpenGuildList(List<RoGuild> list)
	{
		uiGuildState = 0;
		UISetter.SetActive(guildNoticeRoot, active: false);
		UISetter.SetActive(guildJoinCreate, active: true);
		UISetter.SetActive(guildMain, active: false);
		InitGuildList(list);
		if (guildListView != null)
		{
			guildListView.scrollView.SetDragAmount(0f, 0f, updateScrollbars: false);
		}
		minLevel = base.localUser.FindBuilding(EBuilding.Guild).firstLevelReg.userLevel;
		emblemIdx = 0;
		Open();
	}

	public void RomoveGuildList(int gidx)
	{
		RoGuild roGuild = guildList.Find((RoGuild row) => row.gidx == gidx);
		if (roGuild != null)
		{
			guildList.Remove(roGuild);
			OnRefresh();
		}
	}

	public void InitGuildList(List<RoGuild> list)
	{
		if (list != null)
		{
			guildList = list;
			SetGuildList();
		}
	}

	public void SetGuildList()
	{
		if (uiGuildState == 0 && guildListView != null)
		{
			for (int i = 0; i < guildList.Count; i++)
			{
				if (guildList[i].list == "req")
				{
					RoGuild item = guildList[i];
					guildList.RemoveAt(i);
					guildList.Insert(0, item);
					break;
				}
			}
			guildListView.Init(guildList, guildItemIdPrefix);
		}
		SetTap();
	}

	public void ChangeGuildItemState(int idx, string listType)
	{
		RoGuild roGuild = guildList.Find((RoGuild list) => list.gidx == idx);
		if (roGuild != null)
		{
			roGuild.list = listType;
			SetGuildList();
		}
	}

	public void ChangeGuildItemType(int idx, int type)
	{
		RoGuild roGuild = guildList.Find((RoGuild list) => list.gidx == idx);
		if (roGuild != null)
		{
			roGuild.gtyp = type;
			SetGuildList();
		}
	}

	private void SetTap()
	{
		UISetter.SetFlipSwitch(join, uiGuildState == 0);
		UISetter.SetFlipSwitch(create, uiGuildState == 1);
		UISetter.SetActive(joinGuild, uiGuildState == 0);
		UISetter.SetActive(createGuild, uiGuildState == 1);
		if (uiGuildState == 1)
		{
			SetCreateGuild();
		}
	}

	private void SetCreateGuild()
	{
		UISetter.SetLabel(minLevelLabel, "Lv " + minLevel);
		UISetter.SetSpriteWithSnap(selectEmblem, "union_mark_" + $"{emblemIdx:00}");
		UISetter.SetLabel(createCostLabel, guildCreateCost);
	}

	private void CreateGuild()
	{
		if (base.localUser.cash < guildCreateCost)
		{
			NotEnough(MultiplePopUpType.NOTENOUGH_CASH);
			return;
		}
		if (createGuildName.value.Length < 2 || createGuildName.value.Length > 10 || string.IsNullOrEmpty(createGuildName.value))
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("80030"));
			return;
		}
		if (!Utility.PossibleChar(createGuildName.value))
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7144"));
			return;
		}
		UIGuildCreateConfirmPopup recv = UIPopup.Create<UIGuildCreateConfirmPopup>("GuildCreateConfirmPopup");
		recv.Set(localization: true, Localization.Get("110023"), string.Format(Localization.Get("110024"), guildCreateCost), null, Localization.Get("110026"), Localization.Get("1000"), null);
		recv.SetInfo(createGuildName.value, emblemIdx, minLevel, toggleFree.value ? 1 : 2, guildCreateCost);
		recv.onClick = delegate(GameObject popupSender)
		{
			string text = popupSender.name;
			if (text == "OK")
			{
				base.network.RequestCreateGuild(createGuildName.value, toggleFree.value ? 1 : 2, minLevel, emblemIdx);
				recv.ClosePopup();
			}
			else if (text == "Close")
			{
				recv.ClosePopup();
			}
		};
	}

	public void OpenGuildBoard(int page, int maxPage, List<Protocols.GuildBoardData> List)
	{
		if (guildBoard == null)
		{
			guildBoard = UIPopup.Create<UIGuildBoard>("GuildBoard");
			guildBoard.InitAndOpenGuildBoard();
		}
		guildBoard.SetChattingData(page, maxPage, List);
		base.localUser.badgeGuild = false;
		UISetter.SetActive(guildBoardBadge, active: false);
		UIManager.instance.RefreshOpenedUI();
	}

	public void CloseGuildBoard()
	{
		if (guildBoard != null)
		{
			guildBoard.Close();
		}
	}

	public void OpenDispatchPopup()
	{
		if (!(dispatch != null) || !dispatch.isActive)
		{
			if (dispatch == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(dispatchPrefab);
				dispatch = gameObject.GetComponent<UIGuildTroopDispatchPopup>();
				dispatch.transform.parent = contentsRoot.transform;
				dispatch.transform.localScale = new Vector3(1f, 1f, 1f);
				dispatch.transform.localPosition = new Vector3(0f, 0f, 0f);
			}
			dispatch.Init();
		}
	}

	public void CloseDispatchPopup()
	{
		if (dispatch != null)
		{
			dispatch.ClosePopup();
		}
	}

	public void SetConquestState(Protocols.ConquestInfo info)
	{
		conquestInfo = info;
		int num = int.Parse(base.regulation.defineDtbl["GUILD_OCCUPY_LEVEL"].value);
		base.localUser._conquestTeam = info.side;
		UISetter.SetActive(conquestJoinBtn, base.localUser.guildInfo.occupy == 1);
		UISetter.SetActive(conquestBtn, base.localUser.guildInfo.occupy == 1);
		SetConquestStateLabel();
		if (base.localUser.guildInfo.occupy == 0 || base.localUser.guildInfo.level < num)
		{
			return;
		}
		if (conquestJoinBtn.transform.localPosition.x == -800f)
		{
			iTween.MoveTo(conquestJoinBtn, iTween.Hash("x", -465, "islocal", true, "time", 0.2f, "delay", 1f, "easeType", iTween.EaseType.linear));
		}
		if (conquestInfo.remain > 0)
		{
			conquestTime = TimeData.Create();
			conquestTime.SetByDuration(conquestInfo.remain);
			UISetter.SetTimer(conquestTimer, conquestTime);
			conquestTimer.SetLabelFormat(string.Empty, Localization.Get("5821"));
			conquestTimer.RegisterOnFinished(delegate
			{
				if (UIManager.instance.world.existConquestMap && UIManager.instance.world.conquestMap.isActive)
				{
					if (conquestInfo.state == EConquestState.Join)
					{
						NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110349"));
					}
					else if (conquestInfo.state == EConquestState.Setting)
					{
						NetworkAnimation.Instance.CreateFloatingText(Localization.Format("110350"));
					}
					UIManager.instance.world.conquestMap.ClosePopUp();
				}
				base.network.RequestGetConquestInfo();
			});
		}
		if (base.localUser.lastConquestReplayPoint != 0)
		{
			CreateConquestHistoryPopup(base.localUser.lastConquestReplayPoint);
		}
	}

	public void SetConquestStateLabel()
	{
		UISetter.SetActive(conquestBadge, active: false);
		switch (conquestInfo.state)
		{
		case EConquestState.None:
			UISetter.SetLabel(conquestStateLabel, Localization.Get("110359"));
			break;
		case EConquestState.Join:
			if (conquestInfo.sign == 0)
			{
				if (base.localUser.guildInfo.memberGrade == 0)
				{
					UISetter.SetLabel(conquestStateLabel, Localization.Get("110354"));
					break;
				}
				UISetter.SetActive(conquestBadge, active: true);
				UISetter.SetLabel(conquestStateLabel, Localization.Get("110309"));
			}
			else
			{
				UISetter.SetLabel(conquestStateLabel, Localization.Get("110355"));
			}
			break;
		case EConquestState.Match:
			if (conquestInfo.sign == 0)
			{
				UISetter.SetLabel(conquestStateLabel, Localization.Get("110116"));
			}
			else
			{
				UISetter.SetLabel(conquestStateLabel, Localization.Get("110356"));
			}
			break;
		case EConquestState.Setting:
			if (conquestInfo.sign == 0)
			{
				UISetter.SetLabel(conquestStateLabel, Localization.Get("110116"));
			}
			else
			{
				UISetter.SetLabel(conquestStateLabel, Localization.Get("110357"));
			}
			break;
		case EConquestState.Battle:
			UISetter.SetLabel(conquestStateLabel, Localization.Get("110358"));
			break;
		}
	}

	private void OnConquestJoinBtnClicked()
	{
		EConquestState state = conquestInfo.state;
		if (state == EConquestState.Join && conquestInfo.sign == 0)
		{
			if (base.localUser.guildInfo.memberGrade == 0)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110360"));
			}
			else
			{
				base.network.RequestConquestJoin();
			}
		}
	}

	private void OnConquestBtnClicked()
	{
		int num = int.Parse(base.regulation.defineDtbl["GUILD_OCCUPY_LEVEL"].value);
		if (base.localUser.guildInfo.level < num)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("110344", num));
			return;
		}
		if (conquestInfo == null)
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110366"));
			Close();
			return;
		}
		switch (conquestInfo.state)
		{
		case EConquestState.None:
		case EConquestState.Join:
		case EConquestState.Match:
			base.network.RequestGetGuildRanking(1);
			break;
		case EConquestState.Setting:
			if (conquestInfo.join == 0)
			{
				base.network.RequestGetGuildRanking(1);
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110314"));
			}
			else if (conquestInfo.sign == 0 && conquestInfo.join == 1)
			{
				base.network.RequestGetGuildRanking(1);
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110315"));
			}
			else if (conquestInfo.sign == 1 && conquestInfo.join == 1)
			{
				base.network.RequestGetConquestTroop();
			}
			break;
		case EConquestState.Battle:
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110313"));
			break;
		}
	}

	public void SetConquestError()
	{
		UISetter.SetActive(conquestBtn, active: false);
		UISetter.SetActive(conquestJoinBtn, active: false);
	}

	public void CreateConquestHistoryPopup(int type, List<Protocols.GuildRankingInfo> list)
	{
		if (historyPopup == null)
		{
			historyPopup = UIPopup.Create<UIConquestHistoryPopup>("ConquestHistoryPopup");
			historyPopup.Init();
		}
		historyPopup.SetRankingList(type, list);
	}

	public void CreateConquestHistoryPopup(int point)
	{
		base.localUser.lastConquestReplayPoint = 0;
		if (historyPopup == null)
		{
			historyPopup = UIPopup.Create<UIConquestHistoryPopup>("ConquestHistoryPopup");
			historyPopup.Init(point);
		}
	}

	public void InitAndOpenGuildInfo(List<Protocols.GuildMember.MemberData> list)
	{
		uiGuildState = 2;
		UISetter.SetActive(guildNoticeRoot, active: true);
		UISetter.SetActive(guildJoinCreate, active: false);
		UISetter.SetActive(guildMain, active: true);
		InitGuildMemberList(list);
		SetGuildInfo();
		Open();
	}

	public void SetGuildInfo()
	{
		if (base.localUser.guildInfo.state == 1)
		{
			UISetter.SetLabel(guildNotice, Localization.Get("110242") + "\n" + Localization.Get("110243") + closeTimeString(base.localUser.guildInfo.closeTime));
		}
		else if (string.IsNullOrEmpty(base.localUser.guildInfo.notice))
		{
			UISetter.SetLabel(guildNotice, Localization.Get("112013"));
		}
		else
		{
			UISetter.SetLabel(guildNotice, base.localUser.guildInfo.notice);
		}
		UISetter.SetLabel(guildCount, $"[FF6000FF]{base.localUser.guildInfo.count}[-][4E1F00FF]/{base.localUser.guildInfo.maxCount}[-]");
		UISetter.SetLabel(guildName, base.localUser.guildInfo.name);
		UISetter.SetLabel(guildWorld, Localization.Format("19067", base.localUser.guildInfo.world));
		UISetter.SetLabel(guildLevel, "Lv " + base.localUser.guildInfo.level);
		UISetter.SetLabel(guildPoint, base.localUser.guildInfo.point);
		UISetter.SetSpriteWithSnap(guildEmblem, "union_mark_" + $"{base.localUser.guildInfo.emblem:00}");
		UISetter.SetActive(btnManage, base.localUser.guildInfo.memberGrade == 1);
		UISetter.SetActive(btnSkill, base.localUser.guildInfo.memberGrade != 1);
		UISetter.SetActive(btnApprove, base.localUser.guildInfo.memberGrade != 0);
		UISetter.SetActive(btnModify, base.localUser.guildInfo.memberGrade != 0);
	}

	public void InitGuildMemberList(List<Protocols.GuildMember.MemberData> list)
	{
		if (list != null)
		{
			sortType = (ESortType)PlayerPrefs.GetInt("GuildMemberSortType", 3);
			memberList = list;
			SetMemberList();
		}
	}

	private void SetMemberList()
	{
		if (uiGuildState != 2 || !(guildMemberListView != null))
		{
			return;
		}
		memberList.Sort(delegate(Protocols.GuildMember.MemberData row, Protocols.GuildMember.MemberData row1)
		{
			if (sortType == ESortType.Time)
			{
				return row.lastTime.CompareTo(row1.lastTime);
			}
			if (sortType == ESortType.Level)
			{
				return row1.level.CompareTo(row.level);
			}
			return (sortType == ESortType.PBPoint) ? row1.paymentBonusPoint.CompareTo(row.paymentBonusPoint) : row1.level.CompareTo(row.level);
		});
		for (int i = 0; i < memberList.Count; i++)
		{
			if (memberList[i].memberGrade == 1)
			{
				Protocols.GuildMember.MemberData memberData = memberList[i];
				memberList.RemoveAt(i);
				memberList.Insert(0, memberData);
				guildMasterName = memberData.name;
				break;
			}
		}
		int num = 1;
		for (int j = 0; j < memberList.Count; j++)
		{
			if (memberList[j].memberGrade == 2)
			{
				Protocols.GuildMember.MemberData memberData = memberList[j];
				memberList.RemoveAt(j);
				memberList.Insert(num, memberData);
				num++;
			}
		}
		guildMemberListView.Init(memberList, memberItemIdPrefix);
		if (sortType == ESortType.Time)
		{
			UISetter.SetLabel(sortName, Localization.Get("1053"));
		}
		else if (sortType == ESortType.Level)
		{
			UISetter.SetLabel(sortName, Localization.Get("1034"));
		}
		else if (sortType == ESortType.PBPoint)
		{
			UISetter.SetLabel(sortName, Localization.Get("21011"));
		}
	}

	public void AddMemberList(Protocols.GuildMember.MemberData member)
	{
		memberList.Add(member);
		UISetter.SetLabel(guildCount, $"[FF6000FF]{memberList.Count}[-][4E1F00FF]/{base.localUser.guildInfo.maxCount}[-]");
		SetMemberList();
	}

	public void RemoveMemberList(int uno)
	{
		Protocols.GuildMember.MemberData memberData = memberList.Find((Protocols.GuildMember.MemberData list) => list.uno == uno);
		if (memberData != null)
		{
			memberList.Remove(memberData);
			UISetter.SetLabel(guildCount, $"[FF6000FF]{memberList.Count}[-][4E1F00FF]/{base.localUser.guildInfo.maxCount}[-]");
			SetMemberList();
		}
	}

	public void DelegationGildMaster(int uno)
	{
		Protocols.GuildMember.MemberData memberData = memberList.Find((Protocols.GuildMember.MemberData list) => list.uno.ToString() == base.localUser.uno);
		if (memberData != null)
		{
			memberData.memberGrade = 0;
		}
		Protocols.GuildMember.MemberData memberData2 = memberList.Find((Protocols.GuildMember.MemberData list) => list.uno == uno);
		if (memberData2 != null)
		{
			memberData2.memberGrade = 1;
		}
	}

	public void AppointSubMaster(bool bAppoint, int uno)
	{
		Protocols.GuildMember.MemberData memberData = memberList.Find((Protocols.GuildMember.MemberData list) => list.uno == uno);
		if (memberData != null)
		{
			if (bAppoint)
			{
				memberData.memberGrade = 2;
			}
			else
			{
				memberData.memberGrade = 0;
			}
		}
	}

	private void NotEnough(MultiplePopUpType type)
	{
		switch (type)
		{
		case MultiplePopUpType.NOTENOUGH_CASH:
			UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject sender)
			{
				string text = sender.name;
				if (text == "OK")
				{
					UIManager.instance.world.mainCommand.OpenDiamonShop();
				}
			};
			break;
		case MultiplePopUpType.NOTENOUGH_GOLD:
			UISimplePopup.CreateBool(localization: true, "1029", "1030", null, "1001", "1000").onClick = delegate(GameObject sender)
			{
				string text2 = sender.name;
				if (text2 == "OK")
				{
					UIManager.instance.world.camp.GoNavigation("MetroBank");
				}
			};
			break;
		}
	}

	public string closeTimeString(double dTime)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddHours(9.0).AddSeconds((long)dTime).ToString("yyyy-MM-dd HH:mm:ss");
	}

	private string _GetOriginalName(GameObject go)
	{
		if (go == null)
		{
			return string.Empty;
		}
		string text = go.name;
		if (!text.Contains("-"))
		{
			return text;
		}
		return text.Remove(text.IndexOf("-"));
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
		conquestJoinBtn.transform.localPosition = new Vector3(-800f, -147f, 0f);
		if (secretShop != null)
		{
			secretShop.Close();
		}
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
		AnimInfo.Reset();
		AnimTitle.Reset();
		AnimBlock.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimInfo.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimInfo.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public override void Open()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			base.Open();
			if (guildListView != null)
			{
				guildListView.scrollView.SetDragAmount(0f, 0f, updateScrollbars: false);
			}
			if (guildMemberListView != null)
			{
				guildMemberListView.scrollView.SetDragAmount(0f, 0f, updateScrollbars: false);
			}
			OnAnimOpen();
		}
	}

	public override void Close()
	{
		bBackKeyEnable = true;
		if (UIManager.instance.world.existConquestMap && UIManager.instance.world.conquestMap.isActive)
		{
			UIManager.instance.world.conquestMap.ClosePopUp();
		}
		if (UIManager.instance.world.existCooperateBattle && UIManager.instance.world.cooperateBattle.isActive)
		{
			UIManager.instance.world.cooperateBattle.ClosePopup();
		}
		CloseGuildBoard();
		HidePopup();
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
