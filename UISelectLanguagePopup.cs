using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectLanguagePopup : UIPanelBase
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public List<UIFlipSwitch> languageBtnList;

	public UILabel languageName;

	private string _selLanguage;

	private bool _canChange;

	private bool _open;

	public void Open()
	{
		iTween.Stop(AnimBG.gameObject);
		iTween.Stop(AnimBlock.gameObject);
		UISetter.SetActive(this, active: true);
		_open = true;
		_canChange = false;
		_SetLanguage(Localization.language);
		AnimBG.Reset();
		AnimBlock.Reset();
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public void Close()
	{
		if (_open)
		{
			_open = false;
			AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
			AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
			SoundManager.PlaySFX("BTN_Positive_001");
			StartCoroutine(OnEventHidePopup());
		}
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		if (_canChange && Localization.language != _selLanguage)
		{
			Localization.language = _selLanguage;
			SetSelLanguage(_selLanguage);
		}
		UISetter.SetActive(this, active: false);
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text.StartsWith("Lang-"))
		{
			string key = text.Substring(text.IndexOf("-") + 1);
			_SetLanguage(key);
			return;
		}
		if (text == "OK")
		{
			_canChange = true;
		}
		base.OnClick(sender);
		Close();
	}

	private void _SetLanguage(string key)
	{
		for (int i = 0; i < Localization.knownLanguages.Length; i++)
		{
			if (string.Equals(Localization.knownLanguages[i], key))
			{
				languageBtnList[i].Set(SwitchStatus.ON);
			}
			else
			{
				languageBtnList[i].Set(SwitchStatus.OFF);
			}
		}
		_selLanguage = key;
	}

	public void SetSelLanguage(string key)
	{
		for (int i = 0; i < Localization.knownLanguages.Length; i++)
		{
			if (string.Equals(Localization.knownLanguages[i], key))
			{
				UILocalize componentInChildren = languageBtnList[i].on.GetComponentInChildren<UILocalize>();
				if (componentInChildren != null)
				{
					UISetter.SetLabel(languageName, Localization.Get(componentInChildren.key));
				}
			}
		}
	}
}
