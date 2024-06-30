using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICommander : UIUnit
{
	public UISpineAnimation spine;

	public GameObject recruitableRoot;

	public GameObject recruitableExistRoot;

	public GameObject recruitedRoot;

	public UILabel recruitHonor;

	public GameObject waitingRecruitedRoot;

	public GameObject waitOrganizationRoot;

	public GameObject standbyRoot;

	public GameObject recruitRoot;

	public GameObject lockBg;

	public GameObject recruitBg;

	public UISprite thumbGroupBackground;

	public UISprite thumbBackground;

	public UISprite marryIcon;

	public UILabel exp;

	public UIProgressBar expProgress;

	public UILabel medal;

	public UIProgressBar medalProgressBar;

	public UIProgressBar commonProgressBar;

	public GameObject badgeCommander;

	public GameObject badgeRecruit;

	public UILabel pilotLevel;

	public string commanderId;

	public bool isBicCard;

	private bool isEntry;

	public static readonly string thumbnailPrefix = "_1";

	public static readonly string cardPathPrefix = "Texture/Card/";

	public static readonly string cardBackPrefix = "card_frame_bg_";

	public static readonly string cardForePrefix = "card_frame_";

	public new static readonly string thumbnailBackgroundPrefix = "ma_bg_slot_";

	public static readonly string thumbnailGroupBackgroundPrefix = "ma_bg_icon_";

	private string spineBundleName;

	public override void SetSelection(bool selected)
	{
		base.SetSelection(selected);
	}

	public new void Set(string commanderId)
	{
		this.commanderId = commanderId.Trim();
		CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[this.commanderId];
		UISetter.SetSprite(thumbnail, commanderDataRow.thumbnailId);
	}

	public void Set(RoCommander commander)
	{
		if (commander != null)
		{
			SetCommanderData(commander);
			if (base.gameObject.activeSelf && spine != null)
			{
				StartCoroutine(CreateSpineFromCache(commander.resourceId));
			}
		}
	}

	public IEnumerator SetCommander(RoCommander commander)
	{
		if (commander != null)
		{
			SetCommanderData(commander);
			if (base.gameObject.activeSelf && spine != null)
			{
				yield return StartCoroutine(CreateSpineFromCache(commander.resourceId));
			}
		}
	}

	public void SetCommanderData(RoCommander commander)
	{
		commanderId = commander.id;
		UISetter.SetLabel(nickname, commander.reg.nickname);
		if (commander.isBasicCostume && commander.charType != ECharacterType.Mercenary)
		{
			UISetter.SetSprite(thumbnail, commander.resourceId + "_" + commander.currentViewCostume);
		}
		else
		{
			UISetter.SetSprite(thumbnail, commander.resourceId + "_" + commander.getCurrentCostumeName());
		}
		UISetter.SetSprite(smallUnitThumbnail, commander.unitReg.unitSmallIconName);
		UISetter.SetLabel(level, commander.level);
		UISetter.SetLabel(dotLevel, Localization.Format("1021", commander.level));
		UISetter.SetLabel(spaceLevel, Localization.Format("1021", commander.level));
		UISetter.SetLabel(levelNickname, Localization.Format("5784", commander.level, commander.reg.nickname));
		UISetter.SetLabel(pilotLevel, Localization.Format("5765", commander.level));
		SetCardFrame(UIUnit.GetClassGroup(commander.cls), UIUnit.GetSubClass(commander.cls));
		UISetter.SetSprite(thumbBackground, ((int)commander.marry != 1) ? "ig-character-bg" : "ig-character-bg2");
		UISetter.SetSprite(thumbGroupBackground, string.Format(thumbnailGroupBackgroundPrefix + "{0}", UIUnit.GetClassGroup(commander.cls)));
		UISetter.SetActive(marryIcon, commander.possibleMarry && (int)commander.favorRewardStep >= 13);
		UISetter.SetSprite(marryIcon, ((int)commander.marry != 1) ? "wd-ringbox" : "wd-ring");
		int num = 0;
		if (commander.state == ECommanderState.Nomal)
		{
			num = commander.rank;
		}
		UISetter.SetRank(rankGrid, commander.rank);
		UISetter.SetSprite(branchSlotBG, "com_img_slot_" + commander.unitReg.branch.ToString().ToLower());
		UISetter.SetSprite(jobIcon, "com_icon_" + commander.unitReg.job.ToString().ToLower());
		if (commander.unitReg.job == EJob.Attack)
		{
			UISetter.SetLabel(jobLabel, Localization.Get("1350"));
		}
		else if (commander.unitReg.job == EJob.Defense)
		{
			UISetter.SetLabel(jobLabel, Localization.Get("1351"));
		}
		else if (commander.unitReg.job == EJob.Support)
		{
			UISetter.SetLabel(jobLabel, Localization.Get("1352"));
		}
		else
		{
			UISetter.SetLabel(jobLabel, string.Empty);
		}
		if (trainingRoot != null)
		{
			UISetter.SetActive(trainingRoot, commander.rankUpTime.IsProgress() && !commander.rankUpTime.IsEnd());
			UISetter.SetTimer(timer, commander.rankUpTime);
			timer.SetLabelFormat(null, Localization.Get("5821"));
			if (commander.rankUpTime.IsProgress())
			{
				UISetter.SetLabel(stateLabel, Localization.Get("11005"));
			}
		}
		UISetter.SetLabel(leadership, commander.leadership);
		UISetter.SetLabel(exp, $"{commander.exp}/{commander.maxExp}");
		UISetter.SetProgress(expProgress, (float)(int)commander.exp / (float)(int)commander.maxExp);
		UISetter.SetStatus(status, commander);
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		CommanderRankDataRow commanderRankDataRow = regulation.FindCommanderRankData(num + 1);
		int num2 = commander.medal;
		UISetter.SetActive(badgeCommander, (commanderRankDataRow != null && commander.maxMedal <= num2) || (commanderRankDataRow == null && commander.IsTranscendenceSkillUp()) || (commander.IsClassUp(isGoldCheck: false) && commander.state == ECommanderState.Nomal));
		UISetter.SetActive(lockBg, active: false);
		UISetter.SetActive(commonProgressBar, active: false);
		if (commanderRankDataRow != null)
		{
			UISetter.SetProgress(medalProgressBar, (float)commander.medal / (float)commander.maxMedal);
			UISetter.SetProgress(commonProgressBar, (float)(commander.medal + localUser.medal) / (float)commander.maxMedal);
			UISetter.SetLabel(medal, $"{num2} / {commander.maxMedal}");
			UISetter.SetActive(recruitRoot, num2 > commander.maxMedal);
			UISetter.SetActive(lockBg, num2 >= commander.maxMedal && commander.state != ECommanderState.Nomal);
			UISetter.SetActive(recruitBg, num2 >= commander.maxMedal && commander.state != ECommanderState.Nomal);
		}
		else
		{
			UISetter.SetProgress(medalProgressBar, 1f);
			UISetter.SetProgress(commonProgressBar, 1f);
			UISetter.SetLabel(medal, string.Format("{0} / {1}", num2, Localization.Get("1309")));
			UISetter.SetActive(recruitBg, active: false);
		}
	}

	private IEnumerator CreateSpineFromCache(string resourceId)
	{
		if (spine != null)
		{
			if (!string.IsNullOrEmpty(spineBundleName) && spineBundleName != resourceId + ".assetbundle")
			{
				AssetBundleManager.DeleteAssetBundle(spineBundleName);
			}
			if (!AssetBundleManager.HasAssetBundle(resourceId + ".assetbundle"))
			{
				yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(resourceId + ".assetbundle"));
			}
			UISetter.SetActive(spine, active: true);
			if (base.gameObject.activeSelf)
			{
				UISetter.SetSpine(spine, resourceId);
			}
			spine.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
			spineBundleName = resourceId + ".assetbundle";
		}
		yield return null;
	}

	private IEnumerator CreateSpineFromCache(string resourceId, string costumeName)
	{
		if (spine != null)
		{
			if (!string.IsNullOrEmpty(spineBundleName) && spineBundleName != resourceId + ".assetbundle")
			{
				AssetBundleManager.DeleteAssetBundle(spineBundleName);
			}
			if (!AssetBundleManager.HasAssetBundle(resourceId + ".assetbundle"))
			{
				yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(resourceId + ".assetbundle"));
				UISetter.SetActive(spine, active: false);
			}
			UISetter.SetActive(spine, active: true);
			if (base.gameObject.activeSelf)
			{
				UISetter.SetSpine(spine, resourceId, costumeName);
			}
			spine.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
			spineBundleName = resourceId + ".assetbundle";
		}
		yield return null;
	}

	public void Set(RoTroop troop)
	{
		isEntry = false;
		if (troop != null)
		{
			Set(troop.commanderId);
			bool flag = troop.IsEmpty();
			UISetter.SetActive(waitOrganizationRoot, flag);
			UISetter.SetActive(standbyRoot, !flag);
			UISetter.SetStatus(status, troop);
			UISetter.SetLabel(position, troop.slots[0].position + 1);
		}
	}

	public void Set(RoRecruit.Entry entry)
	{
		isEntry = true;
		if (entry != null)
		{
			Set(entry.commander);
			RoLocalUser localUser = RemoteObjectManager.instance.localUser;
			UISetter.SetActive(recruitedRoot, entry.recruited);
			UISetter.SetActive(waitingRecruitedRoot, !entry.recruited && entry.delayTime.IsProgress());
			UISetter.SetActive(recruitableRoot, !entry.recruited && !entry.exist && !entry.delayTime.IsProgress());
			UISetter.SetActive(recruitableExistRoot, !entry.recruited && entry.exist && !entry.delayTime.IsProgress());
			UISetter.SetTimer(timer, entry.delayTime);
			UISetter.SetLabel(recruitHonor, entry.honor);
			UISetter.SetActive(badgeRecruit, localUser.honor >= entry.honor);
			timer.RegisterOnFinished(delegate
			{
				UIManager.instance.RefreshOpenedUI();
			});
		}
	}

	public void Set(Protocols.RecordInfo record)
	{
		UISetter.SetSprite(thumbnail, RemoteObjectManager.instance.regulation.GetCostumeThumbnailName(int.Parse(record.thumbnail)));
		UISetter.SetLabel(spaceLevel, string.Format(Localization.Get("1021"), record.level));
	}

	public void SetCommanderSlot(RoTroop.Slot slot)
	{
		UISetter.SetActive(validSlotRoot, !string.IsNullOrEmpty(slot.commanderId));
		if (!string.IsNullOrEmpty(slot.commanderId))
		{
			RoLocalUser localUser = RemoteObjectManager.instance.localUser;
			RoCommander commander = localUser.FindCommander(slot.commanderId);
			Set(commander);
		}
	}

	private void SetCardFrame(int classGroup, int subClass)
	{
		if (isBicCard)
		{
			UISetter.SetSprite(cardBack, "card_frame_bg_1");
			UISetter.SetSprite(cardOutline_1, "card_frame_bg2_" + classGroup);
			UISetter.SetSprite(cardOutline_2, "card_frame_bg2_" + classGroup);
			UISetter.SetSprite(cardOutline_3, "card_frame_bg2_" + classGroup);
			UISetter.SetSprite(cardOutline_4, "card_frame_bg2_" + classGroup);
			UISetter.SetActive(cardOutlineTop_1, subClass == 5);
			UISetter.SetActive(cardOutlineTop_2, subClass == 5);
			UISetter.SetActive(cardOutlineBottom_1, subClass == 5);
			UISetter.SetActive(cardOutlineBottom_2, active: false);
			if (subClass == 5)
			{
				UISetter.SetSprite(cardCornerUp_1, "card_frame" + (classGroup - 1) + "-4-up");
				UISetter.SetSprite(cardCornerUp_2, "card_frame" + (classGroup - 1) + "-4-up");
				UISetter.SetSprite(cardCornerDown_1, "card_frame" + (classGroup - 1) + "-4-down");
				UISetter.SetSprite(cardCornerDown_2, "card_frame" + (classGroup - 1) + "-4-down");
				UISetter.SetSprite(cardOutlineTop_1, "card_frame" + (classGroup - 1) + "-5-up");
				UISetter.SetSprite(cardOutlineTop_2, "card_frame" + (classGroup - 1) + "-5-up");
				UISetter.SetSprite(cardOutlineBottom_1, "card_frame3-5-down");
				return;
			}
			UISetter.SetSprite(cardCornerUp_1, "card_frame" + (classGroup - 1) + "-" + (subClass - 1) + "-up");
			UISetter.SetSprite(cardCornerUp_2, "card_frame" + (classGroup - 1) + "-" + (subClass - 1) + "-up");
			UISetter.SetSprite(cardCornerDown_1, "card_frame" + (classGroup - 1) + "-" + (subClass - 1) + "-down");
			UISetter.SetSprite(cardCornerDown_2, "card_frame" + (classGroup - 1) + "-" + (subClass - 1) + "-down");
		}
		else
		{
			UISetter.SetSprite(cardOutline_1, "ig-character-bg-line-0" + classGroup);
			UISetter.SetActive(cardOutlineTop_1, subClass == 5);
			UISetter.SetActive(cardOutlineTop_2, subClass == 5);
			UISetter.SetActive(cardOutlineBottom_1, subClass == 5);
			UISetter.SetActive(cardOutlineBottom_2, subClass == 5);
			if (subClass == 5)
			{
				UISetter.SetSprite(cardCornerUp_1, "ig-character-frame" + (classGroup - 1) + "-4-up");
				UISetter.SetSprite(cardCornerUp_2, "ig-character-frame" + (classGroup - 1) + "-4-up");
				UISetter.SetSprite(cardCornerDown_1, "ig-character-frame" + (classGroup - 1) + "-4-down");
				UISetter.SetSprite(cardCornerDown_2, "ig-character-frame" + (classGroup - 1) + "-4-down");
				UISetter.SetSprite(cardOutlineTop_1, "ig-character-frame" + (classGroup - 1) + "-5-up");
				UISetter.SetSprite(cardOutlineTop_2, "ig-character-frame" + (classGroup - 1) + "-5-up");
				return;
			}
			UISetter.SetSprite(cardCornerUp_1, "ig-character-frame" + (classGroup - 1) + "-" + (subClass - 1) + "-up");
			UISetter.SetSprite(cardCornerUp_2, "ig-character-frame" + (classGroup - 1) + "-" + (subClass - 1) + "-up");
			UISetter.SetSprite(cardCornerDown_1, "ig-character-frame" + (classGroup - 1) + "-" + (subClass - 1) + "-down");
			UISetter.SetSprite(cardCornerDown_2, "ig-character-frame" + (classGroup - 1) + "-" + (subClass - 1) + "-down");
		}
	}

	public void SetCommander_ForVipGacha(int commanderId)
	{
		CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[commanderId.ToString()];
		RoCommander roCommander = RoCommander.Create(commanderId.ToString(), 1, commanderDataRow.grade, 1, 0, 0, 0, new List<int>());
		if (roCommander == null)
		{
			return;
		}
		roCommander.SetBaseCostume();
		if (base.gameObject.activeSelf && spine != null)
		{
			StartCoroutine(CreateSpineFromCache(roCommander.resourceId, roCommander.getCurrentCostumeName()));
		}
		UISetter.SetLabel(nickname, roCommander.nickname);
		UISetter.SetSprite(thumbGroupBackground, string.Format(thumbnailGroupBackgroundPrefix + "{0}", UIUnit.GetClassGroup(roCommander.cls)));
		UISetter.SetRank(rankGrid, roCommander.rank);
		UnitDataRow unitDataRow = RemoteObjectManager.instance.regulation.unitDtbl[roCommander.unitId];
		if (unitDataRow != null)
		{
			UISetter.SetSprite(jobIcon, "com_icon_" + unitDataRow.job.ToString().ToLower());
			switch (unitDataRow.job)
			{
			case EJob.Attack:
				UISetter.SetLabel(jobLabel, Localization.Get("1350"));
				break;
			case EJob.Defense:
				UISetter.SetLabel(jobLabel, Localization.Get("1351"));
				break;
			case EJob.Support:
				UISetter.SetLabel(jobLabel, Localization.Get("1352"));
				break;
			default:
				UISetter.SetLabel(jobLabel, string.Empty);
				break;
			}
		}
	}

	public void SetCommander_ForEventBattle(int commanderId, int costume = 0)
	{
		CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[commanderId.ToString()];
		RoCommander roCommander = RoCommander.Create(commanderId.ToString(), 1, commanderDataRow.grade, 1, 0, 0, 0, new List<int>());
		if (roCommander == null)
		{
			return;
		}
		if (costume == 0)
		{
			roCommander.SetBaseCostume();
		}
		else
		{
			roCommander.SetCostume(costume);
		}
		if (base.gameObject.activeSelf && spine != null)
		{
			if (costume == 0)
			{
				StartCoroutine(CreateSpineFromCache(roCommander.resourceId, roCommander.getCurrentCostumeName()));
			}
			else
			{
				StartCoroutine(CreateSpineFromCache(roCommander.resourceId, costume.ToString()));
			}
		}
		UISetter.SetSprite(thumbGroupBackground, string.Format(thumbnailGroupBackgroundPrefix + "{0}", UIUnit.GetClassGroup(roCommander.cls)));
		UISetter.SetRank(rankGrid, roCommander.rank);
		UnitDataRow unitDataRow = RemoteObjectManager.instance.regulation.unitDtbl[roCommander.unitId];
		if (unitDataRow != null)
		{
			UISetter.SetSprite(jobIcon, "com_icon_" + unitDataRow.job.ToString().ToLower());
			switch (unitDataRow.job)
			{
			case EJob.Attack:
				UISetter.SetLabel(jobLabel, Localization.Get("1350"));
				break;
			case EJob.Defense:
				UISetter.SetLabel(jobLabel, Localization.Get("1351"));
				break;
			case EJob.Support:
				UISetter.SetLabel(jobLabel, Localization.Get("1352"));
				break;
			default:
				UISetter.SetLabel(jobLabel, string.Empty);
				break;
			}
		}
	}

	public void SetCommander_OnlyDispatch(RoCommander _commander)
	{
		if (_commander.isBasicCostume && _commander.charType != ECharacterType.Mercenary)
		{
			UISetter.SetSprite(thumbnail, _commander.resourceId + "_" + _commander.currentViewCostume);
		}
		else
		{
			UISetter.SetSprite(thumbnail, _commander.resourceId + "_" + _commander.getCurrentCostumeName());
		}
		UISetter.SetSprite(smallUnitThumbnail, _commander.unitReg.unitSmallIconName);
		UISetter.SetLabel(spaceLevel, Localization.Format("1021", _commander.level));
		SetCardFrame(UIUnit.GetClassGroup(_commander.cls), UIUnit.GetSubClass(_commander.cls));
		UISetter.SetSprite(thumbBackground, ((int)_commander.marry != 1) ? "ig-character-bg" : "ig-character-bg2");
		UISetter.SetSprite(thumbGroupBackground, string.Format(thumbnailGroupBackgroundPrefix + "{0}", UIUnit.GetClassGroup(_commander.cls)));
		UISetter.SetRank(rankGrid, _commander.rank);
		UISetter.SetSprite(thumbGroupBackground, string.Format(thumbnailGroupBackgroundPrefix + "{0}", UIUnit.GetClassGroup(_commander.cls)));
		UISetter.SetRank(rankGrid, _commander.rank);
		UnitDataRow unitDataRow = RemoteObjectManager.instance.regulation.unitDtbl[_commander.unitId];
		if (unitDataRow != null)
		{
			UISetter.SetSprite(jobIcon, "com_icon_" + unitDataRow.job.ToString().ToLower());
			switch (unitDataRow.job)
			{
			case EJob.Attack:
				UISetter.SetLabel(jobLabel, Localization.Get("1350"));
				break;
			case EJob.Defense:
				UISetter.SetLabel(jobLabel, Localization.Get("1351"));
				break;
			case EJob.Support:
				UISetter.SetLabel(jobLabel, Localization.Get("1352"));
				break;
			default:
				UISetter.SetLabel(jobLabel, string.Empty);
				break;
			}
		}
	}
}
