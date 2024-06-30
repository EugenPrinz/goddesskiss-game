using System.Collections;
using Shared.Regulation;
using UnityEngine;

public class UITranscendencePopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private RoCommander commander;

	private int nUseMedal;

	[SerializeField]
	private UISprite thumbnail;

	[SerializeField]
	private UILabel totalPower;

	[SerializeField]
	private UILabel useMedal;

	[SerializeField]
	private UILabel commanderMedal;

	[SerializeField]
	private UILabel totalMedal;

	[SerializeField]
	private UILabel currentStep;

	[SerializeField]
	private UILabel transcendenceCount;

	[SerializeField]
	private UILabel addHealth;

	[SerializeField]
	private UIProgressBar stepProgress;

	[SerializeField]
	private UILabel stepProgressLabel;

	[SerializeField]
	public UIDefaultListView slotListView;

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		switch (text)
		{
		case "Close":
			Close();
			return;
		case "ExchangeBtn":
			if (base.localUser.medal > 0)
			{
				UIPopup.Create<UIMedalExchangePopup>("MedalExchangePopup").Set(commander);
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("8029"));
			}
			return;
		case "InfoBtn":
			UISimplePopup.CreateOK("InformationPopup").Set(localization: true, "6600006", "6600013", string.Empty, "1001", string.Empty, string.Empty);
			return;
		}
		if (text.StartsWith("BtnUpgrade-"))
		{
			string slot = text.Replace("BtnUpgrade-", string.Empty);
			if (sender.GetComponent<UIButton>().isGray)
			{
				if (UIManager.instance.world.existCommanderDetail && UIManager.instance.world.commanderDetail.isActive)
				{
					UICommanderDetail commanderDetail = UIManager.instance.world.commanderDetail;
					commanderDetail.SetNavigationOpen(EStorageType.Medal, commander.id);
				}
				return;
			}
			UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), Localization.Format("6600011", nUseMedal), null, Localization.Get("1001"), Localization.Get("1000")).onClick = delegate(GameObject popupsender)
			{
				string text2 = popupsender.name;
				if (text2 == "OK")
				{
					RemoteObjectManager.instance.RequestTranscendenceSkillUp(int.Parse(commander.id), int.Parse(slot));
				}
			};
		}
		else if (text.StartsWith("SlotInfoBtn-"))
		{
			string s = text.Replace("SlotInfoBtn-", string.Empty);
			TranscendenceSlotDataRow transcendenceSlotDataRow = base.regulation.FindTranscendenceSlot(int.Parse(s));
			UISimplePopup.CreateOK("InformationPopup").Set(localization: true, transcendenceSlotDataRow.tipTitle, transcendenceSlotDataRow.tip, string.Empty, "1001", string.Empty, string.Empty);
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		Set();
	}

	public void InitAndOpenTranscendence(RoCommander comm)
	{
		commander = comm;
		Set();
		OpenPopup();
		SetRecyclable(recyclable: false);
	}

	public void Set(int slot = 0)
	{
		SetSlotList(slot);
		SetCommanderInfo();
		SetCommanderTranscendence();
	}

	public void SetCommanderInfo()
	{
		CommanderDataRow commanderDataRow = base.regulation.commanderDtbl[commander.id];
		UISetter.SetSprite(thumbnail, commander.reg.resourceId + "_" + RemoteObjectManager.instance.regulation.GetCostumeName(commander.currentCostume));
		UISetter.SetLabel(totalPower, commander.currLevelUnitReg.GetTotalPower().ToString("N0"));
		nUseMedal = ((commanderDataRow.vip != 1) ? int.Parse(base.regulation.defineDtbl["TRANSCRNDENCE_MEDALS_VALUE"].value) : int.Parse(base.regulation.defineDtbl["TRANSCRNDENCE_MEDALS_VALUE_VIP"].value));
		UISetter.SetLabel(useMedal, Localization.Format("6600004", nUseMedal));
		UISetter.SetLabel(commanderMedal, Localization.Get("6600005") + " : " + commander.medal);
		UISetter.SetLabel(totalMedal, base.localUser.medal);
	}

	private void SetCommanderTranscendence()
	{
		int num = commander.GetTranscendenceCount();
		int maxTranscendenceCount = commander.GetMaxTranscendenceCount();
		TranscendenceStepUpgradeDataRow transcendenceStepUpgradeDataRow = RemoteObjectManager.instance.regulation.FindTranscendenceStepUpgrade(commander.CurrentTranscendenceStep() + 1);
		UISetter.SetLabel(currentStep, string.Format("[FFCD38FF]{0}[-][FFFFFFFF] : {1}[-]", Localization.Get("6600006"), commander.CurrentTranscendenceStep()));
		if (transcendenceStepUpgradeDataRow != null)
		{
			UISetter.SetLabel(transcendenceCount, Localization.Format("6600008", transcendenceStepUpgradeDataRow.statAddMeasure, transcendenceStepUpgradeDataRow.statAddVolume));
		}
		else
		{
			UISetter.SetLabel(transcendenceCount, Localization.Get("6600026"));
		}
		UISetter.SetLabel(addHealth, Localization.Format("6600007", AddTranscendenceHealth().ToString("N0")));
		if (maxTranscendenceCount > 0)
		{
			UISetter.SetLabel(stepProgressLabel, num + " / " + maxTranscendenceCount);
			UISetter.SetProgress(stepProgress, (float)num / (float)maxTranscendenceCount);
		}
		else
		{
			UISetter.SetLabel(stepProgressLabel, num + " / " + Localization.Get("1309"));
			UISetter.SetProgress(stepProgress, 1f);
		}
	}

	private void SetSlotList(int slot)
	{
		if (slotListView != null)
		{
			slotListView.InitTranscendenceSlot(commander, slot, "Slot-");
		}
	}

	private int AddTranscendenceHealth()
	{
		int num = 0;
		int num2 = commander.GetTranscendenceCount();
		if (num2 > 0)
		{
			int num3 = 0;
			int num4 = num2;
			for (int i = 0; i < base.regulation.transcendenceStepUpgradeDtbl.length; i++)
			{
				if (num4 <= 0)
				{
					break;
				}
				TranscendenceStepUpgradeDataRow transcendenceStepUpgradeDataRow = base.regulation.transcendenceStepUpgradeDtbl[i];
				int num5 = transcendenceStepUpgradeDataRow.stepPoint - num3;
				int num6 = ((num4 >= num5) ? num5 : num4);
				int num7 = num6 / transcendenceStepUpgradeDataRow.statAddMeasure * transcendenceStepUpgradeDataRow.statAddVolume;
				num += num7;
				num4 -= num6;
				num3 = transcendenceStepUpgradeDataRow.stepPoint;
			}
		}
		return num;
	}

	public void OpenPopup()
	{
		base.Open();
		AnimBG.Reset();
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
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
