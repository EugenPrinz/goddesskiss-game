using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIWeaponProgressHistoryPopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public UIDefaultListView historyListView;

	public UISprite slotTypeBtn;

	public GameObject slotTypeRoot;

	public GUIAnimNGUI slotTypeBtnAnim;

	private GradeType tabType = GradeType.Four;

	private SlotType slotType;

	private bool bBackKeyEnable;

	public void Start()
	{
		SetAutoDestroy(autoDestory: true);
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
	}

	private void Set()
	{
		List<List<string>> list = base.localUser.weaponHistory[(int)slotType].FindAll(delegate(List<string> row)
		{
			string key = row[3];
			WeaponDataRow weaponDataRow = base.regulation.weaponDtbl[key];
			return (tabType == (GradeType)weaponDataRow.weaponGrade) ? true : false;
		});
		historyListView.InitWeaponHistoryList(list, "History-");
		historyListView.ResetPosition();
	}

	public override void OnClick(GameObject sender)
	{
		SoundManager.PlaySFX("EFM_SelectButton_001");
		string text = sender.name;
		if (text == "Close")
		{
			if (!bBackKeyEnable)
			{
				ClosePopUp();
			}
			return;
		}
		if (text.StartsWith("History-"))
		{
			string material = text.Substring(text.IndexOf("-") + 1);
			CreateHistoryConfirmPopup(material);
			return;
		}
		switch (text)
		{
		case "GradeBtn-4":
			tabType = GradeType.Four;
			Set();
			return;
		case "GradeBtn-5":
			tabType = GradeType.Five;
			Set();
			return;
		case "SlotTypeBtn":
			OpenSlotTypeContents();
			return;
		}
		if (text.StartsWith("SlotType-"))
		{
			slotType = (SlotType)int.Parse(text.Substring(text.IndexOf("-") + 1));
			if (!base.localUser.weaponHistory.ContainsKey((int)slotType) || base.localUser.weaponHistory[(int)slotType].Count == 0)
			{
				base.network.RequestGetWeaponProgressHistory((int)slotType);
			}
			else
			{
				Set();
			}
		}
	}

	private void CreateHistoryConfirmPopup(string material)
	{
		string[] list = material.Split('_');
		UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), Localization.Get("70086"), string.Empty, Localization.Get("1304"), Localization.Get("1305")).onClick = delegate(GameObject obj)
		{
			string text = obj.name;
			if (text == "OK")
			{
				UIManager.instance.world.weaponResearch.SetWeaponRecipe(int.Parse(list[0]), int.Parse(list[1]), int.Parse(list[2]), int.Parse(list[3]));
				ClosePopUp();
			}
		};
	}

	private void OpenSlotTypeContents()
	{
		UISetter.SetActive(slotTypeRoot, !slotTypeRoot.activeSelf);
		UISetter.SetSprite(slotTypeBtn, (!slotTypeRoot.activeSelf) ? "qm-up-button" : "qm-down-button");
		if (slotTypeRoot.activeSelf)
		{
			slotTypeBtnAnim.MoveIn(GUIAnimSystemNGUI.eGUIMove.Self);
		}
		else
		{
			slotTypeBtnAnim.MoveOut(GUIAnimSystemNGUI.eGUIMove.Self);
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		Set();
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
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
