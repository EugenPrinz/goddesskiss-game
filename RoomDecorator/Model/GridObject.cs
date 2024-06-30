using UnityEngine;

namespace RoomDecorator.Model
{
	public class GridObject : MonoBehaviour
	{
		public SpriteRenderer[] sprites;

		protected Transform _transform;

		protected int _sortingOrder = -999;

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

		public virtual int sortingOrder
		{
			get
			{
				return _sortingOrder;
			}
			set
			{
				if (_sortingOrder != value)
				{
					_sortingOrder = value;
					for (int i = 0; i < sprites.Length; i++)
					{
						sprites[i].sortingOrder = _sortingOrder;
					}
				}
			}
		}

		public void SetActive(bool value)
		{
			base.gameObject.SetActive(value);
		}

		public void SetColor(Color color)
		{
			for (int i = 0; i < sprites.Length; i++)
			{
				if (!(sprites[i] == null))
				{
					sprites[i].color = color;
				}
			}
		}
	}
}
