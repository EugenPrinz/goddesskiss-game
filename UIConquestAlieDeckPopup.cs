using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConquestAlieDeckPopup : UIPopup
{
	public UIDefaultListView deckListView;

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	public void Start()
	{
		SetAutoDestroy(autoDestory: true);
		OpenPopup();
	}

	public void Init(List<Protocols.ConquestStageUser> list, string userName)
	{
		UISetter.SetLabel(title, userName);
		deckListView.Init(list, "Deck_");
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopUp()
	{
		bBackKeyEnable = true;
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0f);
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
	}

	private void OnAnimClose()
	{
	}
}
