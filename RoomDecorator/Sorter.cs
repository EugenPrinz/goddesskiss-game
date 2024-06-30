using System.Collections.Generic;
using RoomDecorator.Model;
using UnityEngine;

namespace RoomDecorator
{
	public class Sorter : SingletonMonoBehaviour<Sorter>
	{
		private LinkedList<BaseUnit> sortList;

		private Dictionary<BaseUnit, LinkedListNode<BaseUnit>> _sortMap;

		private List<BaseUnit> sortUnits;

		public static bool OrderCompare(BaseUnit arg1, BaseUnit arg2)
		{
			List<Tile> tiles = arg2.GetTiles();
			if (tiles.Count == 0)
			{
				tiles.Add(arg2.origin);
			}
			for (int i = arg1.origin.x; i < arg1.origin.x + arg1.width; i++)
			{
				foreach (Tile item in tiles)
				{
					if (item.x == i && item.y == arg2.origin.y && arg1.origin.y < arg2.origin.y)
					{
						return true;
					}
				}
			}
			for (int j = arg1.origin.y; j < arg1.origin.y + arg1.length; j++)
			{
				foreach (Tile item2 in tiles)
				{
					if (item2.x == arg2.origin.x && item2.y == j && arg1.origin.x < arg2.origin.x)
					{
						return true;
					}
				}
			}
			if (arg1.origin.x + arg1.origin.y < arg2.origin.y + arg2.origin.x)
			{
				return true;
			}
			return false;
		}

		public void SortAll()
		{
			sortList = new LinkedList<BaseUnit>();
			sortUnits = new List<BaseUnit>();
			_sortMap = new Dictionary<BaseUnit, LinkedListNode<BaseUnit>>();
			foreach (Transform item in base.transform)
			{
				BaseUnit component = item.GetComponent<BaseUnit>();
				if (!(component == null))
				{
					Add(component);
				}
			}
			Order();
		}

		public void SortUpdate(BaseUnit unit)
		{
			if (sortList != null)
			{
				if (_sortMap.ContainsKey(unit))
				{
					LinkedListNode<BaseUnit> node = _sortMap[unit];
					sortList.Remove(node);
					_sortMap.Remove(unit);
				}
				Add(unit);
				Order();
			}
		}

		public void SortUnit(BaseUnit unit)
		{
			if (sortUnits != null && !sortUnits.Contains(unit))
			{
				sortUnits.Add(unit);
			}
		}

		private void Order()
		{
			LinkedListNode<BaseUnit> linkedListNode = sortList.Last;
			int num = 10;
			while (linkedListNode != null)
			{
				BaseUnit value = linkedListNode.Value;
				if (value.isAttached)
				{
					linkedListNode = linkedListNode.Previous;
					continue;
				}
				value.sortingOrder = num;
				linkedListNode = linkedListNode.Previous;
				num += 20;
			}
		}

		private void Add(BaseUnit unit)
		{
			if (unit.isAttached || unit.origin == null)
			{
				return;
			}
			if (sortList.Count == 0)
			{
				LinkedListNode<BaseUnit> value = sortList.AddFirst(unit);
				_sortMap.Add(unit, value);
				return;
			}
			bool flag = false;
			LinkedListNode<BaseUnit> linkedListNode = sortList.First;
			while (linkedListNode != null)
			{
				if (linkedListNode.Value.isAttached)
				{
					linkedListNode = linkedListNode.Next;
					continue;
				}
				if (unit.OrderCompare(linkedListNode.Value))
				{
					LinkedListNode<BaseUnit> value2 = sortList.AddBefore(linkedListNode, unit);
					_sortMap.Add(unit, value2);
					flag = true;
					break;
				}
				linkedListNode = linkedListNode.Next;
			}
			if (!flag)
			{
				LinkedListNode<BaseUnit> value3 = sortList.AddLast(unit);
				_sortMap.Add(unit, value3);
			}
		}

		private void LateUpdate()
		{
			SortUpdate();
		}

		private void SortUpdate()
		{
			if (sortUnits == null || sortUnits.Count == 0)
			{
				return;
			}
			for (int i = 0; i < sortUnits.Count; i++)
			{
				if (sortList == null)
				{
					return;
				}
				BaseUnit baseUnit = sortUnits[i];
				if (_sortMap.ContainsKey(baseUnit))
				{
					LinkedListNode<BaseUnit> node = _sortMap[baseUnit];
					sortList.Remove(node);
					_sortMap.Remove(baseUnit);
				}
				Add(baseUnit);
			}
			Order();
			sortUnits.Clear();
		}
	}
}
