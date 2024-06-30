using System.Collections.Generic;
using Shared.Battle;
using UnityEngine;

public class UIBattleUnitController : Manager<UIBattleUnitController>
{
	public delegate void SelectDelegate(int unitIdx);

	public GameObjectPool pool;

	public Transform content;

	public UIGrid grid;

	public SelectDelegate _Click;

	[HideInInspector]
	public List<UIBattleUnitControllerItem> _items;

	protected override void Init()
	{
		base.Init();
		_items = new List<UIBattleUnitControllerItem>();
	}

	public UIBattleUnitControllerItem Create(Unit unit)
	{
		if (unit == null || unit.side == EBattleSide.Right)
		{
			return null;
		}
		UIBattleUnitControllerItem uIBattleUnitControllerItem = pool.Create<UIBattleUnitControllerItem>(content);
		if (uIBattleUnitControllerItem == null)
		{
			return null;
		}
		uIBattleUnitControllerItem.Set(unit);
		uIBattleUnitControllerItem._Click = delegate(int uIdx)
		{
			if (_Click != null)
			{
				_Click(uIdx);
			}
		};
		_items.Add(uIBattleUnitControllerItem);
		grid.Reposition();
		return uIBattleUnitControllerItem;
	}

	public void Clean()
	{
		while (content.childCount > 0)
		{
			Transform child = content.transform.GetChild(0);
			pool.Release(child.gameObject);
		}
		if (_items != null)
		{
			_items.Clear();
		}
	}

	public void UpdateUI()
	{
		for (int i = 0; i < _items.Count; i++)
		{
			_items[i].UpdateUI();
		}
	}

	public UIBattleUnitControllerItem FindItem(int unitIdx)
	{
		return _items.Find((UIBattleUnitControllerItem x) => x.unitIdx == unitIdx);
	}
}
