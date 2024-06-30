using System.Collections.Generic;
using UnityEngine;

namespace RoomDecorator.Model
{
	public class BaseUnit : MonoBehaviour
	{
		[HideInInspector]
		public string id;

		[HideInInspector]
		public int width = 1;

		[HideInInspector]
		public int length = 1;

		protected Transform _transform;

		protected Renderer[] _renderers;

		protected int _sortingOrder;

		protected string _sortingLayerName;

		protected List<Tile> tiles = new List<Tile>();

		public new Transform transform
		{
			get
			{
				if (_transform == null)
				{
					_transform = base.gameObject.transform;
				}
				return _transform;
			}
		}

		public Renderer[] renderers
		{
			get
			{
				if (_renderers == null)
				{
					_renderers = transform.GetComponentsInChildren<Renderer>(includeInactive: true);
				}
				return _renderers;
			}
		}

		public virtual int sortingOrder
		{
			get
			{
				return _sortingOrder;
			}
			set
			{
				int num = _sortingOrder;
				_sortingOrder = value;
				for (int i = 0; i < renderers.Length; i++)
				{
					renderers[i].sortingOrder += _sortingOrder - num;
				}
			}
		}

		public virtual string sortingLayerName
		{
			get
			{
				return _sortingLayerName;
			}
			set
			{
				_sortingLayerName = value;
				for (int i = 0; i < renderers.Length; i++)
				{
					renderers[i].sortingLayerName = _sortingLayerName;
				}
			}
		}

		public Tile origin { get; protected set; }

		public virtual bool isAttached => false;

		public List<Tile> GetTiles()
		{
			return tiles;
		}

		public virtual bool OrderCompare(BaseUnit target)
		{
			if (target is Furniture)
			{
				return OrderCompare((Furniture)target);
			}
			if (target is Character)
			{
				return OrderCompare((Character)target);
			}
			return false;
		}

		public virtual bool OrderCompare(Furniture target)
		{
			return false;
		}

		public virtual bool OrderCompare(Character target)
		{
			return false;
		}
	}
}
