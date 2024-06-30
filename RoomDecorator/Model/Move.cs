using UnityEngine;

namespace RoomDecorator.Model
{
	public class Move : State
	{
		private Tiles _tiles;

		private Sorter _sorter;

		public Move(Character owner)
			: base(owner)
		{
			_tiles = SingletonMonoBehaviour<RoomDecorator>.Instance.GetTiles();
			_sorter = SingletonMonoBehaviour<Sorter>.Instance;
		}

		public override void OnEnter()
		{
			Tile randomTile = _tiles.GetRandomTile();
			_owner.moveType = EMoveType.None;
			_owner.destinationTile = randomTile;
			_owner.destinationObject = randomTile.aboveObject;
			_owner.destinationPosition = randomTile.transform.position;
			if (randomTile.aboveObject != null && randomTile.aboveObject is Furniture)
			{
				Furniture furniture = (Furniture)randomTile.aboveObject;
				if (furniture.CanAttachCharacter(_owner.data) && Random.Range(0, 100) < furniture.data.actionRate)
				{
					AttachCharacterPoint attachAblePoint = furniture.GetAttachAblePoint();
					if (attachAblePoint != null)
					{
						_owner.moveType = EMoveType.Attach;
						_owner.destinationPosition = new Vector3(attachAblePoint.transform.position.x, attachAblePoint.transform.position.y, 0f);
					}
				}
			}
			_owner.view.HeadAnimation("walk", loop: true);
			_owner.view.BodyAnimation("walk", loop: true);
			_owner.agent.SetDestination(new Vector3(_owner.destinationPosition.x, 0f, _owner.destinationPosition.y * 2f));
		}

		public override void OnUpdate()
		{
			Vector3 position = _owner.agent.gameObject.transform.position;
			_owner.transform.position = new Vector3(position.x, position.z / 2f, _owner.transform.position.z);
			Tile tileByPoint = _tiles.GetTileByPoint(_owner.transform.position);
			if (_owner.origin != tileByPoint)
			{
				_owner.UpdateTile(tileByPoint);
			}
			_sorter.SortUnit(_owner);
			_owner.SetDirection(_owner.agent.velocity);
			if (_owner.agent.pathPending || !(_owner.agent.remainingDistance <= _owner.agent.stoppingDistance) || (_owner.agent.hasPath && _owner.agent.velocity.sqrMagnitude != 0f))
			{
				return;
			}
			bool flag = false;
			if (_owner.moveType == EMoveType.Attach && _owner.destinationTile.aboveObject != null && _owner.destinationTile.aboveObject is Furniture)
			{
				Furniture furniture = (Furniture)_owner.destinationTile.aboveObject;
				if (_owner.OrderCompare(furniture))
				{
					AttachCharacterPoint attachAblePoint = furniture.GetAttachAblePoint();
					if (attachAblePoint != null)
					{
						flag = true;
						_owner.destinationPosition = new Vector3(attachAblePoint.transform.position.x, attachAblePoint.transform.position.y, _owner.transform.position.z);
						_owner.ChangeState(EState.MoveToPosition);
					}
				}
			}
			if (!flag)
			{
				_owner.ChangeState(EState.Idle);
			}
		}

		public override void OnExit()
		{
			if (_owner.agent.isActiveAndEnabled)
			{
				_owner.agent.ResetPath();
			}
		}
	}
}
