using UnityEngine;

namespace RoomDecorator.Model
{
	public class AttachCharacterPoint : MonoBehaviour
	{
		[SerializeField]
		private int _sortingOrder;

		public string headAnimationName;

		public string bodyAnimationName;

		private Character _character;

		public int sortingOrder
		{
			get
			{
				return _sortingOrder;
			}
			set
			{
				_sortingOrder = value;
				if (_character != null)
				{
					_character.sortingOrder = _sortingOrder;
				}
			}
		}

		public bool isAttached => _character != null;

		public void Attach(Furniture target, Character character)
		{
			_character = character;
			_character.sortingOrder = _sortingOrder;
			_character.OnAttachPoint(target, this);
		}

		public void Detach()
		{
			if (!(_character == null))
			{
				_character.OnDetachPoint();
				_character = null;
			}
		}

		public bool IsAttached(Character character)
		{
			return _character == character;
		}
	}
}
