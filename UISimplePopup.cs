using System;
using System.Collections;
using UnityEngine;

public class UISimplePopup : UIPopup
{
	public enum Result
	{
		OK,
		Cancel,
		Left,
		Middle,
		Right,
		Close,
		Unknown
	}

	public UILabel subMessage;

	public UILabel okName;

	public UILabel cancelName;

	public UILabel extraName;

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public static Result ParseResult(string r)
	{
		if (!Enum.IsDefined(typeof(Result), r))
		{
			return Result.Unknown;
		}
		return (Result)Enum.Parse(typeof(Result), r);
	}

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
		AnimBG.Reset();
		AnimBlock.Reset();
		OpenPopup();
	}

	public override void OnClick(GameObject sender)
	{
		if (onClick != null)
		{
			base.OnClick(sender);
			if (_autoDestory && _waitRoutineCount <= 0)
			{
				Close();
			}
		}
		else
		{
			base.OnClick(sender);
		}
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		SoundManager.PlaySFX("BTN_Positive_001");
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
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

	public UISimplePopup Set(bool localization, string title, string message, string subMessage, string ok, string cancel, string extra)
	{
		_Set(this, localization, title, message, subMessage, ok, cancel, extra);
		return this;
	}

	private static void _Set(UISimplePopup popup, bool localization, string title, string message, string subMessage, string ok, string cancel, string extra)
	{
		if (popup == null)
		{
			return;
		}
		if (localization)
		{
			if (!string.IsNullOrEmpty(title))
			{
				title = Localization.Get(title);
			}
			if (!string.IsNullOrEmpty(message))
			{
				message = Localization.Get(message);
			}
			if (!string.IsNullOrEmpty(subMessage))
			{
				subMessage = Localization.Get(subMessage);
			}
			if (!string.IsNullOrEmpty(ok))
			{
				ok = Localization.Get(ok);
			}
			if (!string.IsNullOrEmpty(cancel))
			{
				cancel = Localization.Get(cancel);
			}
			if (!string.IsNullOrEmpty(extra))
			{
				extra = Localization.Get(extra);
			}
		}
		UISetter.SetLabel(popup.title, title);
		UISetter.SetLabel(popup.message, message);
		UISetter.SetLabel(popup.subMessage, subMessage);
		UISetter.SetLabel(popup.okName, ok);
		UISetter.SetLabel(popup.cancelName, cancel);
		UISetter.SetLabel(popup.extraName, extra);
	}

	public static UISimplePopup CreateDebugOK(string message, string subMessage, string button)
	{
		return CreateOK(localization: false, "DEBUG", message, subMessage, button);
	}

	public static UISimplePopup CreateDebugBool(string message, string subMessage, string ok, string cancel)
	{
		UISimplePopup uISimplePopup = UIPopup.Create<UISimplePopup>("Twin");
		if (uISimplePopup == null)
		{
			return null;
		}
		_Set(uISimplePopup, localization: false, "DEBUG", message, subMessage, ok, cancel, null);
		return uISimplePopup;
	}

	public static UISimplePopup Toast(bool localization, string message, float duration = 2f)
	{
		UISimplePopup uISimplePopup = UIPopup.Create<UISimplePopup>("Toast");
		if (uISimplePopup == null)
		{
			return null;
		}
		_Set(uISimplePopup, localization, null, message, null, null, null, null);
		TweenAlpha componentInChildren = uISimplePopup.GetComponentInChildren<TweenAlpha>();
		componentInChildren.duration = duration;
		SendMessageTimer componentInChildren2 = uISimplePopup.GetComponentInChildren<SendMessageTimer>();
		componentInChildren2.delay = duration;
		return uISimplePopup;
	}

	public static UISimplePopup CreateOK(bool localization, string title, string message, string subMessage, string button)
	{
		UISimplePopup uISimplePopup = CreateOK<UISimplePopup>();
		_Set(uISimplePopup, localization, title, message, subMessage, button, null, null);
		return uISimplePopup;
	}

	public static UISimplePopup CreateOK(string prefabName = "Single")
	{
		return CreateOK<UISimplePopup>(prefabName);
	}

	public static T CreateOK<T>(string prefabName = "Single") where T : UISimplePopup
	{
		UISimplePopup uISimplePopup = UIPopup.Create<UISimplePopup>(prefabName);
		if (uISimplePopup == null)
		{
			return (T)null;
		}
		return uISimplePopup.GetComponent<T>();
	}

	public static UISimplePopup CreateBool(bool localization, string title, string message, string subMessage, string ok, string cancel)
	{
		UISimplePopup uISimplePopup = CreateBool<UISimplePopup>();
		if (uISimplePopup == null)
		{
			return null;
		}
		_Set(uISimplePopup, localization, title, message, subMessage, ok, cancel, null);
		return uISimplePopup;
	}

	public static T CreateBool<T>(string prefabName = "Twin") where T : UISimplePopup
	{
		UISimplePopup uISimplePopup = UIPopup.Create<UISimplePopup>(prefabName);
		if (uISimplePopup == null)
		{
			return (T)null;
		}
		return uISimplePopup.GetComponent<T>();
	}

	public static UISimplePopup CreateExtra(bool localization, string title, string message, string subMessage, string buttonLeft, string buttonMid, string buttonRight)
	{
		UISimplePopup uISimplePopup = CreateExtra<UISimplePopup>();
		if (uISimplePopup == null)
		{
			return null;
		}
		_Set(uISimplePopup, localization, title, message, subMessage, buttonLeft, buttonMid, buttonRight);
		return uISimplePopup;
	}

	public static T CreateExtra<T>(string prefabName = "Extra") where T : UISimplePopup
	{
		UISimplePopup uISimplePopup = UIPopup.Create<UISimplePopup>(prefabName);
		if (uISimplePopup == null)
		{
			return (T)null;
		}
		return uISimplePopup.GetComponent<T>();
	}
}
