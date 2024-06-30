using UnityEngine;

namespace RoomDecorator.Model
{
	public class MoveToPosition : State
	{
		private Tiles _tiles;

		private Sorter _sorter;

		private Vector3 _destination;

		private float _speed = 1f;

		public MoveToPosition(Character owner)
			: base(owner)
		{
			_tiles = SingletonMonoBehaviour<RoomDecorator>.Instance.GetTiles();
			_sorter = SingletonMonoBehaviour<Sorter>.Instance;
		}

		public override void OnEnter()
		{
			_owner.view.HeadAnimation("walk", loop: true);
			_owner.view.BodyAnimation("walk", loop: true);
			SetDestination(_owner.destinationPosition);
		}

		private void SetDestination(Vector3 destination)
		{
			_destination = _owner.destinationPosition;
			_owner.SetDirection(_destination - _owner.transform.position);
		}

		public override void OnUpdate()
		{
			_owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _destination, _speed * Time.fixedDeltaTime);
			if (!(_destination == _owner.transform.position))
			{
				return;
			}
			bool flag = false;
			if (_owner.moveType == EMoveType.Attach && _owner.destinationTile != null && _owner.destinationTile.aboveObject == _owner.destinationObject)
			{
				Furniture furniture = (Furniture)_owner.destinationObject;
				flag = furniture.AttachCharacter(_owner);
			}
			if (!flag)
			{
				_sorter.SortUnit(_owner);
				int num = Random.Range(0, 10);
				if (_owner.moveType == EMoveType.Detach && num < 2)
				{
					_owner.ChangeState(EState.Wink);
				}
				else if (num < 7)
				{
					_owner.ChangeState(EState.Idle);
				}
				else
				{
					_owner.ChangeState(EState.Move);
				}
			}
		}
	}
}
