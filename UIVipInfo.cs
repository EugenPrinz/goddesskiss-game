using System.Collections;
using Shared.Regulation;
using UnityEngine;

public class UIVipInfo : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private const int DefVip = 7083;

	public UILabel nextBtnLabel;

	public UILabel beforeBtnLabel;

	public UILabel vipLevel;

	public UILabel lbVipContent;

	public UILabel userVipLevel;

	public UILabel userVipExp;

	public UIProgressBar userVipProgress;

	public GameObject nextBtn;

	public GameObject beforeBtn;

	private int vip;

	private int maxVip;

	private int minVip;

	private bool goCashShop;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		goCashShop = false;
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
	}

	public void Init(bool isShop)
	{
		goCashShop = isShop;
		vip = base.localUser.vipLevel;
		UISetter.SetLabel(userVipLevel, vip);
		maxVip = base.regulation.vipExpDtbl[base.regulation.vipExpDtbl.length - 1].Idx;
		minVip = base.regulation.vipExpDtbl[0].Idx;
		if (base.regulation.vipExpDtbl.ContainsKey((vip + 1).ToString()))
		{
			UISetter.SetActive(message, active: true);
			VipExpDataRow vipExpDataRow = base.regulation.vipExpDtbl[(vip + 1).ToString()];
			int exp = base.regulation.vipExpDtbl[vip.ToString()].exp;
			UISetter.SetLabel(userVipExp, $"{base.localUser.vipExp} / {exp}");
			UISetter.SetProgress(userVipProgress, (float)base.localUser.vipExp / (float)exp);
			UISetter.SetLabel(message, Localization.Format("7080", exp - base.localUser.vipExp, vip + 1));
		}
		else
		{
			UISetter.SetActive(message, active: false);
			UISetter.SetLabel(userVipExp, base.localUser.vipExp);
			UISetter.SetProgress(userVipProgress, 1f);
		}
		SetBtnControl();
		SetContentsControl();
	}

	public override void OnClick(GameObject sender)
	{
		switch (sender.name)
		{
		case "Close":
			ClosePopup();
			break;
		case "BeforeBtn":
			OnBeforeBtnClicked();
			break;
		case "NextBtn":
			OnNextBtnClicked();
			break;
		case "CashShopBtn":
			goCashShop = true;
			ClosePopup();
			break;
		}
	}

	public void OnCashShopOpen()
	{
		if (UIManager.instance.world != null)
		{
			UIManager.instance.world.mainCommand.OpenDiamonShop();
		}
		else
		{
			RemoteObjectManager.instance.RequestGetCashShopList();
		}
	}

	public void OnNextBtnClicked()
	{
		if (vip != maxVip)
		{
			vip++;
			SetBtnControl();
			SetContentsControl();
		}
	}

	public void OnBeforeBtnClicked()
	{
		if (vip != minVip)
		{
			vip--;
			SetBtnControl();
			SetContentsControl();
		}
	}

	public void SetBtnControl()
	{
		if (vip == minVip)
		{
			UISetter.SetActive(nextBtn, active: true);
			UISetter.SetActive(beforeBtn, active: false);
		}
		else if (vip == maxVip)
		{
			UISetter.SetActive(beforeBtn, active: true);
			UISetter.SetActive(nextBtn, active: false);
		}
		else
		{
			UISetter.SetActive(nextBtn, active: true);
			UISetter.SetActive(beforeBtn, active: true);
		}
		UISetter.SetLabel(vipLevel, $"VIP {vip}");
		UISetter.SetLabel(nextBtnLabel, $"VIP {vip + 1}");
		UISetter.SetLabel(beforeBtnLabel, $"VIP {vip - 1}");
	}

	public void SetContentsControl()
	{
		string key = $"{7083 + vip}";
		string text = Localization.Get(key);
		lbVipContent.text = text;
	}

	public void End()
	{
		ClosePopup();
	}

	private void OnAnimOpen()
	{
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public override void Open()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Close()
	{
		base.Close();
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.4f);
		if (goCashShop)
		{
			OnCashShopOpen();
		}
		yield return new WaitForSeconds(0.4f);
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
}
