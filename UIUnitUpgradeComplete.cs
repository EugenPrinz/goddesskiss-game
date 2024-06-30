using System.Collections;
using Shared.Regulation;
using UnityEngine;

public class UIUnitUpgradeComplete : UIPopup
{
	public GEAnimNGUI AnimRight;

	public GEAnimNGUI AnimBlock;

	public UIStatus unitInfo;

	public UIUnit uiUnit;

	public UILabel increseHealth;

	public UILabel increseAttack;

	public UILabel increseDefence;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public void Init(RoUnit unit)
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			UnitDataRow prevClsReg = unit.prevClsReg;
			UISetter.SetLabel(increseHealth, "+" + (unit.currClsReg.maxHealth - prevClsReg.maxHealth));
			UISetter.SetLabel(increseAttack, "+" + (unit.currClsReg.attackDamage - prevClsReg.attackDamage));
			UISetter.SetLabel(increseDefence, "+" + (unit.currClsReg.defense - prevClsReg.defense));
			unitInfo.Set(unit);
			uiUnit.Set(unit);
			OpenPopupShow();
		}
	}

	public void Init(RoUnit unit, RoCommander commander)
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			UnitDataRow prevClsReg = unit.prevClsReg;
			UISetter.SetLabel(increseHealth, "+" + (unit.currClsReg.maxHealth - prevClsReg.maxHealth));
			UISetter.SetLabel(increseAttack, "+" + (unit.currClsReg.attackDamage - prevClsReg.attackDamage));
			UISetter.SetLabel(increseDefence, "+" + (unit.currClsReg.defense - prevClsReg.defense));
			unitInfo.Set(unit, commander);
			uiUnit.Set(unit);
			OpenPopupShow();
		}
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
		HidePopup();
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		bBackKeyEnable = true;
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
		AnimRight.Reset();
		AnimBlock.Reset();
		AnimRight.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
		base.Close();
	}

	private void OnAnimClose()
	{
		AnimRight.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
