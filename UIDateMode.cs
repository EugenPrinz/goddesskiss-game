using System;
using System.Collections;
using Shared.Regulation;
using UnityEngine;

public class UIDateMode : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	private RoCommander commander;

	public UISpineAnimation spine;

	public GameObject giftPopup;

	public UILabel currentTime;

	public UILabel datingTime;

	public UILabel bulletCount;

	public UILabel skillPoint;

	public UITimer skillPointRechargeTimer;

	private TimeData skillRemainTime = new TimeData();

	private DateTime dateTime;

	private int datingSecond;

	private int remainGiftSecond;

	private int nextDatingGetGigtSecond;

	private bool bOpenGiftPopup;

	private int skillPointCount;

	public UILabel commanderFavorStep;

	public UIProgressBar commanderFavorProgress;

	public UILabel fommanderFavorValue;

	public UILabel nickName;

	public UILabel labelSecurityNumber;

	public UIInput inputNumber;

	private int securityNumber;

	private string spineBundleName;

	protected override void Awake()
	{
		remainGiftSecond = int.Parse(base.regulation.defineDtbl["DATE_PRESENT_COUNT"].value);
		base.Awake();
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close")
		{
			if (!bBackKeyEnable)
			{
				UISetter.SetActive(giftPopup, active: false);
				Close();
			}
		}
		else if (text == "GiftBox")
		{
			if (labelSecurityNumber.text == inputNumber.value)
			{
				RemoteObjectManager.instance.RequestDateModeGetGift();
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("300007"));
			}
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		if (focusStatus)
		{
			dateTime = DateTime.Now;
		}
	}

	public void InitAndOpenDateMode()
	{
		UISetter.SetActive(spine, active: false);
		UISetter.SetActive(giftPopup, active: false);
		bOpenGiftPopup = false;
		dateTime = DateTime.Now;
		datingSecond = 0;
		nextDatingGetGigtSecond = remainGiftSecond;
		commander = RemoteObjectManager.instance.localUser.FindCommanderResourceId(UIManager.instance.world.mainCommand.spineTest.spine.spinePrefabName);
		SetInfo();
		Open();
	}

	public void RefreshDateMode()
	{
		SkillPoingControl();
	}

	private void SetSpine()
	{
		if (base.gameObject.activeSelf && spine != null)
		{
			StartCoroutine(CreateSpineFromCache(UIManager.instance.world.mainCommand.spineTest.spine.spinePrefabName));
		}
	}

	private void SetInfo()
	{
		SkillPoingControl();
		UISetter.SetLabel(currentTime, dateTime.ToString("[tt] hh:mm:ss"));
		UISetter.SetLabel(datingTime, Localization.Format("300003", Utility.GetTimeStringColonFormat(datingSecond)));
		UISetter.SetLabel(bulletCount, UIManager.instance.world.mainCommand.bullet.text);
		UISetter.SetLabel(nickName, Localization.Format("300004", commander.nickname));
		SetFavorInfo();
	}

	private void SetFavorInfo()
	{
		UISetter.SetLabel(commanderFavorStep, commander.favorStep);
		FavorStepDataRow favorStepDataRow = RemoteObjectManager.instance.regulation.FindFavorStepData((int)commander.favorStep + 1);
		FavorDataRow favorData = commander.GetFavorData((int)commander.favorStep + 1);
		if ((!commander.possibleMarry) ? (favorData == null) : (((int)commander.marry == 1) ? (favorData == null) : ((int)commander.favorStep >= 13)))
		{
			UISetter.SetLabel(fommanderFavorValue, Localization.Get("1309"));
			UISetter.SetProgress(commanderFavorProgress, 0f);
		}
		else
		{
			UISetter.SetLabel(fommanderFavorValue, string.Concat(commander.favorPoint, "/", favorStepDataRow.favor));
			UISetter.SetProgress(commanderFavorProgress, (float)(int)commander.favorPoint / (float)favorStepDataRow.favor);
		}
	}

	public void SkillPoingControl()
	{
	}

	public void SkillPointCharge()
	{
	}

	public void GetDateGift(int remainSecond)
	{
		CloseGiftPopup();
		nextDatingGetGigtSecond = datingSecond + remainSecond;
		SetFavorInfo();
	}

	public void CloseGiftPopup()
	{
		UISetter.SetActive(giftPopup, active: false);
		bOpenGiftPopup = false;
	}

	private IEnumerator CreateSpineFromCache(string resourceId)
	{
		if (spine != null)
		{
			UISetter.SetActive(spine, active: true);
			if (base.gameObject.activeSelf)
			{
				UISetter.SetSpine(spine, resourceId);
			}
			spine.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
			spineBundleName = resourceId + ".assetbundle";
			spine.target.GetComponent<UIInteraction>().dateMode = true;
			SetInteraction();
		}
		yield return null;
	}

	public void SetInteraction(RoCommander commander = null)
	{
		if (commander == null)
		{
			Regulation regulation = RemoteObjectManager.instance.regulation;
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.FindCostumeData(int.Parse(RemoteObjectManager.instance.localUser.thumbnailId));
			if (commanderCostumeDataRow != null)
			{
				commander = RemoteObjectManager.instance.localUser.FindCommander(commanderCostumeDataRow.cid.ToString());
				if (commander != null)
				{
					spine.target.GetComponent<UIInteraction>().favorStep = commander.favorRewardStep;
					spine.target.GetComponent<UIInteraction>().marry = commander.marry;
				}
			}
		}
		else
		{
			spine.target.GetComponent<UIInteraction>().favorStep = commander.favorRewardStep;
			spine.target.GetComponent<UIInteraction>().marry = commander.marry;
		}
	}

	private IEnumerator CurrentTime()
	{
		dateTime = dateTime.AddSeconds(1.0);
		datingSecond++;
		SetInfo();
		if (!bOpenGiftPopup && datingSecond >= nextDatingGetGigtSecond)
		{
			UISetter.SetActive(giftPopup, active: true);
			SoundManager.PlaySFX("EFM_OpenPopup_001");
			securityNumber = UnityEngine.Random.Range(100, 999);
			UISetter.SetLabel(labelSecurityNumber, securityNumber);
			inputNumber.label.text = "???";
			inputNumber.value = string.Empty;
			bOpenGiftPopup = true;
		}
		yield return new WaitForSeconds(1f);
		StartCoroutine(CurrentTime());
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
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
		AnimBG.Reset();
		AnimNpc.Reset();
		AnimTitle.Reset();
		AnimBlock.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public override void Open()
	{
		UIPanel component = GetComponent<UIPanel>();
		if (component != null)
		{
			UIManager.instance.world.mainCommand.SetChatPanelDepth(component.depth + 1);
		}
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			base.Open();
			OnAnimOpen();
		}
	}

	public override void Close()
	{
		UIManager.instance.world.mainCommand.ResetChatPanelDepth();
		bBackKeyEnable = true;
		HidePopup();
	}

	public void AnimOpenFinish()
	{
		StartCoroutine(CurrentTime());
		SetSpine();
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
	}

	protected override void OnEnablePopup()
	{
		UISetter.SetVoice(spine, active: true);
	}

	protected override void OnDisablePopup()
	{
		UISetter.SetVoice(spine, active: false);
	}
}
