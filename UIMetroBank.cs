using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMetroBank : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimInfo;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIFakeRoulette roulette;

	public UILabel gettingGold;

	public UILabel munitionsCount;

	public UILabel gold;

	public UILabel multipleDescription;

	public UITimer munitionsTimer;

	public GameObject frontBlock;

	public UILabel startCash_1;

	public UILabel startCash_2;

	public GameObject beforeRoot;

	public GameObject afterRoot;

	public GameObject goldRoot;

	public GameObject tenGoldRoot;

	private int lastTarget;

	public UILabel level;

	public UITimer timer;

	public UILabel upgrade;

	public UISprite upgradeComplete;

	public UISprite chipIcon;

	public UISprite upgradeBadge;

	public UISprite receiveBadge;

	public UISprite exchangeBadge;

	public GameObject startBtn;

	public GameObject selectRoot;

	public RoulletListItem resultItem;

	public UIDefaultListView rewardListView;

	private RoBuilding building;

	private int cash_1;

	private int cash_2;

	private int bankGold;

	public UISpineAnimation spineAnimation;

	public GameObject goldBtn;

	public UIUser user;

	public GameObject badgeMail;

	public UILabel mailCount;

	private int rechargeCount;

	private int rechargeMaxCount;

	private int enableRechargeCount;

	private int oneRoulletCash;

	private int tenRoulletCash;

	private string RewardAnimationName = "BattleResultPopUp_Win_ItemOpen";

	public UITexture goBG;

	private new void Awake()
	{
		UISetter.SetSpine(spineAnimation, "n_006");
		beforeRoot.transform.localPosition = new Vector3(beforeRoot.transform.localPosition.x, 165f, beforeRoot.transform.localPosition.z);
		afterRoot.transform.localPosition = new Vector3(afterRoot.transform.localPosition.x, 165f, afterRoot.transform.localPosition.z);
		base.Awake();
	}

	private void OnDestroy()
	{
		if (goBG != null)
		{
			goBG = null;
		}
		if (AnimBlock != null)
		{
			AnimBlock = null;
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close")
		{
			if (!bBackKeyEnable)
			{
				UIManager.instance.RefreshOpenedUI();
				Close();
			}
			return;
		}
		if (text.StartsWith("Play-"))
		{
			SoundManager.PlaySFX("BTN_Exchange_001");
			int cnt = int.Parse(text.Substring("Play-".Length));
			NetWorkBankRoulletState(cnt);
			return;
		}
		switch (text)
		{
		case "Receive":
			break;
		case "Btn_Upgrade":
			break;
		case "Link-CashShop":
			base.uiWorld.mainCommand.OpenDiamonShop();
			break;
		case "Mail":
			base.network.RequestMailList();
			break;
		}
	}

	private void NetWorkBankRoulletState(int cnt, bool confirm = false)
	{
		if (cnt == 1)
		{
			if (enableRechargeCount < cnt)
			{
				goCashShop(MultiplePopUpType.NOTENOUGH_METROBANK);
				return;
			}
			if (base.localUser.cash < oneRoulletCash)
			{
				goCashShop(MultiplePopUpType.NOTENOUGH_CASH);
				return;
			}
		}
		else
		{
			if (!confirm && cnt == 10)
			{
				UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), Localization.Format("13025", tenRoulletCash), null, Localization.Get("1304"), Localization.Get("1305")).onClick = delegate(GameObject sender)
				{
					string text = sender.name;
					if (text == "OK")
					{
						NetWorkBankRoulletState(cnt, confirm: true);
					}
				};
				return;
			}
			if (enableRechargeCount < cnt)
			{
				goCashShop(MultiplePopUpType.NOTENOUGH_METROBANK);
				return;
			}
			if (base.localUser.cash < tenRoulletCash)
			{
				goCashShop(MultiplePopUpType.NOTENOUGH_CASH);
				return;
			}
		}
		base.network.RequestBankRoulletStart(cnt, rechargeCount);
	}

	private void goCashShop(MultiplePopUpType type)
	{
		if (type == MultiplePopUpType.NOTENOUGH_CASH)
		{
			UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject sender)
			{
				string text2 = sender.name;
				if (text2 == "OK")
				{
					base.uiWorld.mainCommand.OpenDiamonShop();
				}
				else if (!(text2 == "Cancel"))
				{
				}
			};
		}
		else
		{
			if (base.localUser.vipLevel >= 15)
			{
				return;
			}
			UISimplePopup.CreateBool(localization: true, "1303", "12006", null, "5348", "1000").onClick = delegate(GameObject sender)
			{
				string text = sender.name;
				if (text == "OK")
				{
					base.uiWorld.mainCommand.OpenDiamonShop();
				}
				else if (!(text == "Cancel"))
				{
				}
			};
		}
	}

	public void RoulletPlay(List<int> multiple)
	{
		if (multiple.Count == 1)
		{
			int multiple2 = multiple[0] % 100;
			StartCoroutine(Play(bankGold, multiple2));
		}
		else
		{
			SetRewardList(multiple);
		}
	}

	private void SetRewardList(List<int> list)
	{
		SoundManager.PlaySFX("BTN_TenExchange_001");
		UpAnimation(beforeRoot);
		rewardListView.InitRoulletRewardList(list, bankGold, "reward-");
		StartCoroutine(_PlayRewardAnimation());
		UISetter.SetActive(goldRoot, active: false);
		UISetter.SetActive(tenGoldRoot, active: true);
	}

	public IEnumerator Play(int gold, int multiple)
	{
		UpAnimation(beforeRoot);
		UISetter.SetActive(frontBlock, active: true);
		yield return new WaitForSeconds(0.5f);
		UISetter.SetLabel(gettingGold, null);
		UISetter.SetActive(goldRoot, active: false);
		UISetter.SetActive(tenGoldRoot, active: false);
		SoundManager.PlaySFX("SE_Roulette_001");
		yield return StartCoroutine(roulette.Play(multiple, lastTarget));
		RewardView(gold, multiple);
	}

	public void RewardView(int gold, int multiple)
	{
		UISetter.SetActive(receiveBadge, active: true);
		UISetter.SetLabel(gettingGold, Localization.Format("12013", multiple, multiple * gold));
		UISetter.SetActive(goldRoot, active: true);
		if (gettingGold != null)
		{
			gettingGold.transform.localScale = Vector3.one * 2f;
			TweenScale.Begin(gettingGold.gameObject, 0.2f, Vector3.one);
		}
		UISetter.SetActive(frontBlock, active: false);
		UISetter.SetActive(selectRoot, active: true);
		lastTarget = multiple;
		resultItem.Set(multiple);
		DownAnimation(beforeRoot);
		UIManager.instance.RefreshOpenedUI();
	}

	public void Set()
	{
		lastTarget = 1;
		bankGold = base.regulation.GetUserLevelDataRow(base.localUser.level).bankGold;
		List<int> list = base.regulation.FindMetroBankLuckList();
		roulette.SetList(list);
		InitUI();
		SetLabel();
	}

	private void SetLabel()
	{
		RefreshValue();
		UISetter.SetLabel(gold, bankGold.ToString("N0"));
		UISetter.SetLabel(startCash_1, oneRoulletCash);
		UISetter.SetLabel(startCash_2, tenRoulletCash);
		UISetter.SetLabel(munitionsCount, Localization.Format("12003", $"{rechargeMaxCount - rechargeCount}/{rechargeMaxCount}"));
		user.Set(base.localUser);
		UISetter.SetActive(badgeMail, base.localUser.badgeNewMailCount > 0);
		UISetter.SetLabel(mailCount, base.localUser.badgeNewMailCount);
	}

	private void RefreshValue()
	{
		rechargeCount = 0;
		if (base.localUser.resourceRechargeList.ContainsKey(601.ToString()))
		{
			rechargeCount = base.localUser.resourceRechargeList[601.ToString()];
		}
		rechargeMaxCount = base.regulation.GetMetroBankMaxRecharge(base.localUser.vipLevel);
		oneRoulletCash = base.regulation.GetMetroBankCost(rechargeCount);
		tenRoulletCash = base.regulation.GetMetroBankCost(rechargeCount, 10);
		enableRechargeCount = rechargeMaxCount - rechargeCount;
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		SetLabel();
		base.uiWorld.mainCommand.SetEnableButton(EGoods.FreeGold, state: false);
	}

	public void InitAndOpenMetroBank()
	{
		Set();
		Open();
		UISetter.SetActive(root, active: true);
	}

	public void InitUI()
	{
		UISetter.SetLabel(gettingGold, null);
		UISetter.SetActive(goldRoot, active: false);
		UISetter.SetActive(tenGoldRoot, active: false);
		UISetter.SetActive(frontBlock, active: false);
		UISetter.SetActive(receiveBadge, active: false);
		UISetter.SetActive(selectRoot, active: false);
		DownAnimation(beforeRoot);
		UpAnimation(afterRoot);
	}

	public void SetBuildingUpgradeButton()
	{
		if (building == null)
		{
			return;
		}
		if (building.state == EBuildingState.Undefined)
		{
			UISetter.SetActive(upgrade, active: true);
			UISetter.SetActive(upgradeComplete, active: false);
			UISetter.SetActive(timer, active: false);
			if (!building.isMaxLevel)
			{
				UISetter.SetLabel(upgrade, Localization.Get("1007"));
			}
			else
			{
				UISetter.SetLabel(upgrade, Localization.Get("5800"));
			}
		}
		else if (building.state == EBuildingState.UpgradeComplete)
		{
			UISetter.SetActive(upgrade, active: false);
			UISetter.SetActive(upgradeComplete, active: true);
			UISetter.SetActive(timer, active: false);
			UIMultiplePopup uIMultiplePopup = Object.FindObjectOfType(typeof(UIMultiplePopup)) as UIMultiplePopup;
			if (uIMultiplePopup != null)
			{
				uIMultiplePopup.Close();
			}
		}
		else if (building.state == EBuildingState.Upgrading)
		{
			UISetter.SetActive(upgrade, active: true);
			UISetter.SetActive(upgradeComplete, active: false);
			UISetter.SetActive(timer, active: true);
			UISetter.SetLabel(upgrade, Localization.Get("5647"));
			if ((bool)timer)
			{
				UISetter.SetActive(timer, building.upgradeTime.IsValid());
				if (building.upgradeTime.IsValid())
				{
					UISetter.SetTimer(timer, building.upgradeTime);
				}
			}
		}
		UISetter.SetLabel(level, "Lv" + building.level);
	}

	public IEnumerator _PlayRewardAnimation()
	{
		UISetter.SetActive(frontBlock, active: true);
		if (rewardListView.itemList != null)
		{
			for (int idx = 0; idx < rewardListView.itemList.Count; idx++)
			{
				RoulletListItem reward = rewardListView.itemList[idx] as RoulletListItem;
				UISetter.SetActive(reward, active: true);
				yield return new WaitForSeconds(0.1f);
			}
		}
		DownAnimation(beforeRoot);
		UISetter.SetActive(frontBlock, active: false);
		UIManager.instance.RefreshOpenedUI();
	}

	public void UpAnimation(GameObject ui)
	{
		iTween.MoveTo(ui, iTween.Hash("y", 165, "islocal", true, "time", 0.2, "delay", 0, "easeType", iTween.EaseType.linear));
	}

	public void DownAnimation(GameObject ui)
	{
		iTween.MoveTo(ui, iTween.Hash("y", 42, "islocal", true, "time", 0.2, "delay", 0, "easeType", iTween.EaseType.linear));
	}

	public override void Open()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			base.uiWorld.mainCommand.SetEnableButton(EGoods.FreeGold, state: false);
			base.Open();
			OnAnimOpen();
		}
	}

	public override void Close()
	{
		bBackKeyEnable = true;
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
		base.uiWorld.mainCommand.SetEnableButton(EGoods.FreeGold, state: true);
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
