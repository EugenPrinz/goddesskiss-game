using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVipGacha : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	public UISpineAnimation spineAnimation;

	public GameObject contentsRoot;

	public GameObject vipContentsPrefab;

	[HideInInspector]
	public UIVipGachaContents vipGachaContents;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UITexture goBG;

	public List<Protocols.VipGacha.VipGachaInfo> gachaInfoList;

	public int gachaCount;

	public static readonly string thumbnailGroupBackgroundPrefix = "ma_bg_icon_";

	[SerializeField]
	private UISprite JobBG;

	[SerializeField]
	private UICommander CommanerView;

	public void SetMainCommander()
	{
		int rewardIdx = base.localUser.gachaInfoList[0].rewardIdx;
		CommanerView.SetCommander_ForVipGacha(rewardIdx);
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

	public void Init()
	{
		if (vipGachaContents == null)
		{
			GameObject gameObject = Object.Instantiate(vipContentsPrefab);
			vipGachaContents = gameObject.GetComponent<UIVipGachaContents>();
			vipGachaContents.transform.parent = contentsRoot.transform;
			vipGachaContents.transform.localScale = new Vector3(1f, 1f, 1f);
			vipGachaContents.transform.localPosition = new Vector3(0f, -15f, 0f);
		}
		base.network.RequestVipGachaInfo();
		OpenPopupShow();
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close" && !bBackKeyEnable)
		{
			Close();
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		vipGachaContents.OnRefresh();
	}

	public void OpenPopupShow()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Open()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Close()
	{
		bBackKeyEnable = true;
		HidePopup();
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: false);
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

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
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
