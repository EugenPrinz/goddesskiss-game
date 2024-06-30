using System.Collections;
using UnityEngine;

public class UISecretShop : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	public UISpineAnimation spineAnimation;

	public GameObject contentsRoot;

	public GameObject secretContentsPrefab;

	private UISecretShopContents secretContents;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UITexture goBG;

	private void Start()
	{
		UISetter.SetSpine(spineAnimation, "n_004");
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

	public void Init(EShopType type)
	{
		if (secretContents == null)
		{
			GameObject gameObject = Object.Instantiate(secretContentsPrefab);
			secretContents = gameObject.GetComponent<UISecretShopContents>();
			secretContents.transform.parent = contentsRoot.transform;
			secretContents.transform.localScale = new Vector3(1f, 1f, 1f);
			secretContents.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		secretContents.SetType(type, _popup: false);
		base.network.RequestGetSecretShopList(type);
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
		secretContents.OnRefresh();
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
