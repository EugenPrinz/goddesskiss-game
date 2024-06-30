using System;
using UnityEngine;

public class UITimer : MonoBehaviour
{
	public enum StringFormat
	{
		Colon,
		Full,
		Simple,
		SimpleColon,
		HM_MS,
		EventRemain
	}

	public delegate void OnFinishedDelegate(UITimer timer);

	public StringFormat labelTextFormat = StringFormat.Full;

	public UILabel label;

	public UILabel percent;

	public UISprite sprite;

	public UIProgressBar progressBar;

	private double _startTime;

	private double _endTime;

	private double _duration;

	private TimeData _time = TimeData.Create();

	private string _labelFinishString;

	private bool _isLocalize;

	private string _localKey;

	private OnFinishedDelegate _onFinished;

	private bool _isEnd;

	public string stringPrefix { get; private set; }

	public string stringPostfix { get; private set; }

	public TimeData timeData => _time;

	private void OnDisable()
	{
		_onFinished = null;
	}

	public void Set(TimeData data)
	{
		_isLocalize = false;
		_isEnd = false;
		_time.Set(data);
	}

	public void Set(double startTime, double endTime)
	{
		_isLocalize = false;
		_isEnd = false;
		_time.Set(startTime, endTime);
	}

	public void SetByDuration(double startTime, double duration)
	{
		_isLocalize = false;
		_isEnd = false;
		_time.SetByDuration(startTime, duration);
	}

	public void Set(double endTime)
	{
		_isLocalize = false;
		_isEnd = false;
		_time.Set(RemoteObjectManager.instance.GetCurrentTime(), endTime);
	}

	public void Set(TimeData data, string localKey)
	{
		_isEnd = false;
		_isLocalize = true;
		_localKey = localKey;
		_time.Set(data);
	}

	public void Stop(bool resetUI = false)
	{
		_isEnd = true;
		_time.SetInvalidValue();
		if (resetUI)
		{
			UISetter.SetLabel(label, 0);
			UISetter.SetProgress(sprite, 0f);
			UISetter.SetProgress(progressBar, 0f);
		}
		if (!string.IsNullOrEmpty(_labelFinishString))
		{
			UISetter.SetLabel(label, _labelFinishString);
		}
		if (_onFinished != null)
		{
			_onFinished(this);
		}
	}

	public void SetLabelFormat(string prefix, string postfix)
	{
		stringPrefix = prefix;
		stringPostfix = postfix;
	}

	public void SetFinishString(string finishString)
	{
		_labelFinishString = finishString;
	}

	public void RegisterOnFinished(OnFinishedDelegate target)
	{
		_onFinished = null;
		_onFinished = (OnFinishedDelegate)Delegate.Combine(_onFinished, target);
	}

	public void UnregisterOnFinished(OnFinishedDelegate target)
	{
		_onFinished = (OnFinishedDelegate)Delegate.Remove(_onFinished, target);
	}

	public void AllReset()
	{
		_onFinished = null;
	}

	private void Update()
	{
		if (!_time.IsValid())
		{
			if (!_isEnd)
			{
				Stop();
			}
			return;
		}
		float currentProgress = _time.GetCurrentProgress();
		UISetter.SetProgress(sprite, currentProgress);
		UISetter.SetProgress(progressBar, currentProgress);
		if ((bool)percent)
		{
			UISetter.SetLabel(percent, Localization.Format("Common.PercentageFormat", (int)(currentProgress * 100f)));
		}
		string text = null;
		if (labelTextFormat == StringFormat.Full)
		{
			text = Utility.GetTimeString(_time.GetRemain());
		}
		else if (labelTextFormat == StringFormat.Colon)
		{
			text = Utility.GetTimeStringColonFormat(_time.GetRemain());
		}
		else if (labelTextFormat == StringFormat.Simple)
		{
			text = Utility.GetTimeSimpleString(_time.GetRemain());
		}
		else if (labelTextFormat == StringFormat.SimpleColon)
		{
			text = Utility.GetTimeStringSimpleColonFormat(_time.GetRemain());
		}
		else if (labelTextFormat == StringFormat.HM_MS)
		{
			text = Utility.GetTimeHM_MSString(_time.GetRemain());
		}
		else if (labelTextFormat == StringFormat.EventRemain)
		{
			text = Utility.GetTimeEventRemainFormat(_time.GetRemain());
		}
		if (_isLocalize)
		{
			UISetter.SetLabel(label, Localization.Format(_localKey, stringPrefix + text + stringPostfix));
		}
		else if (string.IsNullOrEmpty(text))
		{
			UISetter.SetLabel(label, null);
		}
		else
		{
			UISetter.SetLabel(label, stringPrefix + text + stringPostfix);
		}
		if (currentProgress > 1f)
		{
			Stop();
		}
	}
}
