using System.Collections;
using UnityEngine;

public class UISecretShopPopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public GameObject contentsRoot;

	public GameObject secretContentsPrefab;

	private UISecretShopContents secretContents;

	private void Start()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			SetAutoDestroy(autoDestory: true);
			AnimBG.Reset();
			AnimBlock.Reset();
			OpenPopup();
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
		secretContents.SetType(type, _popup: true);
		base.network.RequestGetSecretShopList(type);
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "Close")
		{
			SoundManager.PlaySFX("BTN_Negative_001");
			ClosePopup();
		}
	}

	public override void OnRefresh()
	{
		secretContents.OnRefresh();
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		if (!bBackKeyEnable)
		{
			bBackKeyEnable = true;
			HidePopup();
		}
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
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
