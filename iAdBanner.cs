using System;
using UnityEngine;

public class iAdBanner
{
	private bool _IsLoaded;

	private bool _IsOnScreen;

	private bool firstLoad = true;

	private bool _ShowOnLoad = true;

	private int _id;

	private TextAnchor _anchor;

	public int id => _id;

	public bool IsLoaded => _IsLoaded;

	public bool IsOnScreen => _IsOnScreen;

	public bool ShowOnLoad
	{
		get
		{
			return _ShowOnLoad;
		}
		set
		{
			_ShowOnLoad = value;
		}
	}

	public TextAnchor anchor => _anchor;

	public int gravity => _anchor switch
	{
		TextAnchor.LowerCenter => 81, 
		TextAnchor.LowerLeft => 83, 
		TextAnchor.LowerRight => 85, 
		TextAnchor.MiddleCenter => 17, 
		TextAnchor.MiddleLeft => 19, 
		TextAnchor.MiddleRight => 21, 
		TextAnchor.UpperCenter => 49, 
		TextAnchor.UpperLeft => 51, 
		TextAnchor.UpperRight => 53, 
		_ => 48, 
	};

	public event Action AdLoadedAction = delegate
	{
	};

	public event Action FailToReceiveAdAction = delegate
	{
	};

	public event Action AdWiewLoadedAction = delegate
	{
	};

	public event Action AdViewActionBeginAction = delegate
	{
	};

	public event Action AdViewFinishedAction = delegate
	{
	};

	public iAdBanner(TextAnchor anchor, int id)
	{
		_id = id;
		_anchor = anchor;
	}

	public iAdBanner(int x, int y, int id)
	{
		_id = id;
	}

	public void Hide()
	{
		if (_IsOnScreen)
		{
			_IsOnScreen = false;
		}
	}

	public void Show()
	{
		if (!_IsOnScreen)
		{
			_IsOnScreen = true;
		}
	}

	public void didFailToReceiveAdWithError()
	{
		this.FailToReceiveAdAction();
	}

	public void bannerViewDidLoadAd()
	{
		_IsLoaded = true;
		if (ShowOnLoad && firstLoad)
		{
			Show();
			firstLoad = false;
		}
		this.AdLoadedAction();
	}

	public void bannerViewWillLoadAd()
	{
		this.AdWiewLoadedAction();
	}

	public void bannerViewActionDidFinish()
	{
		this.AdViewFinishedAction();
	}

	public void bannerViewActionShouldBegin()
	{
		this.AdViewActionBeginAction();
	}
}
