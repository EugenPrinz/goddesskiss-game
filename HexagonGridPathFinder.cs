using System;
using System.Collections.Generic;

public class HexagonGridPathFinder
{
	public interface ExtraScoreGenerator
	{
		int onExtraScoreGenerate(int x, int y);
	}

	public class Node
	{
		private int _x;

		private int _y;

		private int _cost;

		private int _score;

		private Node _prev;

		private bool _isClosed;

		public int x => _x;

		public int y => _y;

		public int cost => _cost;

		public int score => _score;

		public Node prev => _prev;

		private Node()
		{
		}

		public virtual int getX()
		{
			return _x;
		}

		public virtual int getY()
		{
			return _y;
		}

		public virtual int getCost()
		{
			return _cost;
		}

		public virtual int getScore()
		{
			return _score;
		}

		public virtual Node getPrev()
		{
			return _prev;
		}

		public bool IsClosed()
		{
			return _isClosed;
		}

		public virtual bool isClosed()
		{
			return _isClosed;
		}

		public static Node _create(int x, int y)
		{
			Node node = new Node();
			node._x = x;
			node._y = y;
			node._cost = 0;
			node._score = 0;
			node._prev = null;
			node._isClosed = false;
			return node;
		}

		public static void _update(Node node, Node prev, int gx, int gy, ExtraScoreGenerator esg)
		{
			int num = node.getX();
			int num2 = node.getY();
			node._cost = prev.getCost() + 1;
			node._score = node._cost + getCost(num, num2, gx, gy);
			if (num == gx && num2 == gy)
			{
				node._score = 0;
			}
			node._prev = prev;
			node._isClosed = false;
			node._score += esg.onExtraScoreGenerate(num, num2);
		}

		public static int GetCost(int sx, int sy, int gx, int gy)
		{
			return getCost(sx, sy, gx, gy);
		}

		public static int getCost(int sx, int sy, int gx, int gy)
		{
			int value = gx - sx;
			int num = gy - sy;
			int num2 = Math.Abs(value);
			num2 = (num2 >> 1) + ((num >= 0) ? (num2 & 1) : 0);
			num2 = Math.Abs(num) - num2;
			return Math.Abs(value) + ((num2 >= 0) ? num2 : 0);
		}

		public static void _setPrev(Node node, Node prev)
		{
			node._prev = prev;
		}

		public static void _setIsClosed(Node node, bool isClosed)
		{
			node._isClosed = isClosed;
		}
	}

	private int _width;

	private int _height;

	private ExtraScoreGenerator _extraScoreGenerator;

	public int width => _width;

	public int height => _height;

	public HexagonGridPathFinder(int w, int h, ExtraScoreGenerator esg)
	{
		_width = w;
		_height = h;
		if (esg == null)
		{
			throw new ArgumentException("esg must be not null.");
		}
		_extraScoreGenerator = esg;
	}

	public virtual int getWidth()
	{
		return _width;
	}

	public virtual int getHeight()
	{
		return _height;
	}

	public IList<Node> FindPath(int sx, int sy, int gx, int gy)
	{
		return findPath(sx, sy, gx, gy);
	}

	internal virtual IList<Node> findPath(int sx, int sy, int gx, int gy)
	{
		ExtraScoreGenerator extraScoreGenerator = _extraScoreGenerator;
		Node[][] array = _createNodeGrid();
		IList<Node> list = new List<Node>();
		list.Add(array[sy][sx]);
		while (list.Count > 0)
		{
			Node node = list[0];
			list.RemoveAt(0);
			Node._setIsClosed(node, isClosed: true);
			if (node.getX() == gx && node.getY() == gy)
			{
				return _createPath(node);
			}
			foreach (Node item in _getNeighborList(node, array))
			{
				if (!item.isClosed())
				{
					int num = node.getCost() + 1;
					if (num < item.getCost())
					{
						list.Remove(item);
					}
					if (!_isInOpenedList(item, list))
					{
						Node._update(item, node, gx, gy, extraScoreGenerator);
						_addToOpenedList(item, list);
					}
				}
			}
		}
		return new List<Node>();
	}

	private Node[][] _createNodeGrid()
	{
		int num = getWidth();
		int num2 = getHeight();
		Node[][] array = RectangularArrays.ReturnRectangularNodeArray(num2, num);
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				array[i][j] = Node._create(j, i);
			}
		}
		return array;
	}

	private IList<Node> _createPath(Node goal)
	{
		IList<Node> list = new List<Node>();
		for (Node node = goal; node != null; node = node.getPrev())
		{
			list.Insert(0, node);
		}
		return list;
	}

	private void _addToOpenedList(Node node, IList<Node> openedList)
	{
		for (int i = 0; i < openedList.Count; i++)
		{
			if (node.getScore() < openedList[i].getScore())
			{
				openedList.Insert(i, node);
				return;
			}
		}
		openedList.Add(node);
	}

	private bool _isInOpenedList(Node node, IList<Node> openedList)
	{
		for (int i = 0; i < openedList.Count; i++)
		{
			if (object.ReferenceEquals(node, openedList[i]))
			{
				return true;
			}
		}
		return false;
	}

	private IList<Node> _getNeighborList(Node node, Node[][] grid)
	{
		int num = (1 - Math.Abs(node.getX())) & 1;
		int num2 = num - 1;
		int[] array = new int[6] { -1, 0, 1, -1, 1, 0 };
		int[] array2 = new int[6] { num, 1, num, num2, num2, -1 };
		int num3 = getWidth();
		int num4 = getHeight();
		IList<Node> list = new List<Node>();
		for (int i = 0; i < 6; i++)
		{
			int num5 = node.getX() + array[i];
			int num6 = node.getY() + array2[i];
			if (num5 >= 0 && num5 < num3 && num6 >= 0 && num6 < num4)
			{
				list.Add(grid[num6][num5]);
			}
		}
		return list;
	}
}
