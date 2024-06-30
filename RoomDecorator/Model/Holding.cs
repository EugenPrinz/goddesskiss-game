using UnityEngine;

namespace RoomDecorator.Model
{
	public class Holding : State
	{
		private Tiles _tiles;

		private Sorter _sorter;

		private bool _holding;

		private float _holdRemain = 0.3f;

		private Vector3 _offset;

		public Holding(Character owner)
			: base(owner)
		{
			_tiles = SingletonMonoBehaviour<RoomDecorator>.Instance.GetTiles();
			_sorter = SingletonMonoBehaviour<Sorter>.Instance;
		}

		public override void OnEnter()
		{
			_holding = false;
			_holdRemain = 0.3f;
			_owner.sortingLayerName = "Preview";
			_owner.view.HeadAnimation("hold");
			_owner.view.BodyAnimation("hold");
			_offset = _owner.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		public override void OnUpdate()
		{
			if (!_holding)
			{
				_holdRemain -= Time.deltaTime;
				if (_holdRemain < 0f)
				{
					_holding = true;
					_owner.view.HeadAnimation("holding", loop: true);
					_owner.view.BodyAnimation("holding", loop: true);
				}
			}
			_owner.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + _offset;
			if (_owner.isPressed)
			{
				return;
			}
			Tile tileByPoint = _tiles.GetTileByPoint(_owner.transform.position);
			if (tileByPoint == null)
			{
				Vector3 position = _owner.agent.gameObject.transform.position;
				_owner.transform.position = new Vector3(position.x, position.z / 2f, 0f);
				_owner.ChangeState(EState.Idle);
				return;
			}
			if (_owner.origin != tileByPoint)
			{
				_owner.UpdateTile(tileByPoint);
			}
			bool flag = false;
			if (tileByPoint.aboveObject != null && tileByPoint.aboveObject is Furniture)
			{
				Furniture furniture = (Furniture)tileByPoint.aboveObject;
				if (furniture.CanAttachCharacter(_owner.data))
				{
					AttachCharacterPoint attachAblePoint = furniture.GetAttachAblePoint();
					if (attachAblePoint != null)
					{
						flag = true;
						furniture.AttachCharacter(attachAblePoint, _owner);
					}
				}
			}
			if (!flag)
			{
				_owner.agent.Warp(new Vector3(_owner.transform.position.x, 0f, _owner.transform.position.y * 2f));
				_sorter.SortUnit(_owner);
				_owner.ChangeState(EState.Idle);
			}
		}

		public override void OnExit()
		{
			_owner.sortingLayerName = "Default";
		}
	}
}
