using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectDeckPopup : UIPopup
{
	public UIDefaultListView dectListView;

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private EBattleType type;

	public void Start()
	{
		SetAutoDestroy(autoDestory: true);
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
	}

	public void Init(BattleData battleData)
	{
		dectListView.Init(base.localUser.preDeckList, battleData, "Deck_");
	}

	public override void OnClick(GameObject sender)
	{
		SoundManager.PlaySFX("EFM_SelectButton_001");
		string text = sender.name;
		if (!dectListView.Contains(text))
		{
			return;
		}
		string id = dectListView.GetPureId(text);
		Protocols.UserInformationResponse.PreDeck preDeck = base.localUser.preDeckList.Find((Protocols.UserInformationResponse.PreDeck row) => row.idx == int.Parse(id));
		if (preDeck == null)
		{
			return;
		}
		Dictionary<string, int> deckData = preDeck.deckData;
		if (base.uiWorld.existReadyBattle && base.uiWorld.readyBattle.isActive)
		{
			if (!base.uiWorld.readyBattle.CanStageDeck(deckData))
			{
				UISimplePopup.CreateOK(localization: false, Localization.Get("1303"), Localization.Get("5040012"), null, "1001");
				return;
			}
			base.uiWorld.readyBattle.SetStageDeck(deckData);
		}
		else if (base.uiWorld.existAnnihilationMap && base.uiWorld.annihilationMap.isActive && base.uiWorld.annihilationMap.advancePopUp != null)
		{
			base.uiWorld.annihilationMap.advancePopUp.SetStageDeck(deckData);
		}
		ClosePopup();
	}

	public void OpenPopup()
	{
		base.Open();
		AnimBG.Reset();
		AnimBlock.Reset();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
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
