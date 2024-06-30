using System;
using UnityEngine;

public class AN_PlusButton
{
	private int _ButtonId;

	private TextAnchor _anchor = TextAnchor.MiddleCenter;

	private int _x;

	private int _y;

	private bool _IsShowed = true;

	public Action ButtonClicked = delegate
	{
	};

	private static int _nextId;

	public int ButtonId => _ButtonId;

	public int x => _x;

	public int y => _y;

	public bool IsShowed => _IsShowed;

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

	private static int nextId
	{
		get
		{
			_nextId++;
			return _nextId;
		}
	}

	public AN_PlusButton(string url, AN_PlusBtnSize btnSize, AN_PlusBtnAnnotation annotation)
	{
		_ButtonId = nextId;
		AN_PlusButtonProxy.createPlusButton(_ButtonId, url, (int)btnSize, (int)annotation);
		SA_Singleton<AN_PlusButtonsManager>.instance.RegisterButton(this);
	}

	public void SetGravity(TextAnchor btnAnchor)
	{
		_anchor = btnAnchor;
		AN_PlusButtonProxy.setGravity(gravity, _ButtonId);
	}

	public void SetPosition(int btnX, int btnY)
	{
		_x = btnX;
		_y = btnY;
		AN_PlusButtonProxy.setPosition(_x, _y, _ButtonId);
	}

	public void Show()
	{
		_IsShowed = true;
		AN_PlusButtonProxy.show(_ButtonId);
	}

	public void Hide()
	{
		_IsShowed = false;
		AN_PlusButtonProxy.hide(_ButtonId);
	}

	public void Refresh()
	{
		AN_PlusButtonProxy.refresh(_ButtonId);
	}

	public void FireClickAction()
	{
		ButtonClicked();
	}
}
