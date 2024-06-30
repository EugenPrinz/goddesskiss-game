using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConquestCurrentStatePopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public UIFlipSwitch currentStateTab;

	public GameObject stateRoot;

	public UIDefaultListView userListView;

	private Dictionary<string, int> userList;

	private bool bBackKeyEnable;

	public void Start()
	{
		SetAutoDestroy(autoDestory: true);
		OpenPopup();
	}

	public void Init(List<Protocols.ConquestStageInfo.User> list)
	{
		UISetter.SetActive(currentStateTab, active: true);
		list.Sort((Protocols.ConquestStageInfo.User row, Protocols.ConquestStageInfo.User row1) => (row.auth != row1.auth) ? row.auth.CompareTo(row1.auth) : row.level.CompareTo(row1.level));
		userListView.Init(base.uiWorld.guild.memberList, list);
	}

	public override void OnClick(GameObject sender)
	{
		SoundManager.PlaySFX("EFM_SelectButton_001");
		string text = sender.name;
		if (text == "Close" && !bBackKeyEnable)
		{
			ClosePopUp();
		}
	}

	private void _SetTab(bool currentState, bool guildState, bool resultState)
	{
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
