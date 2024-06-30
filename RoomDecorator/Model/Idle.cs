using UnityEngine;

namespace RoomDecorator.Model
{
	public class Idle : State
	{
		private float _remain;

		public Idle(Character owner)
			: base(owner)
		{
		}

		public override void OnEnter()
		{
			_remain = Random.Range(1f, 5f);
			_owner.view.HeadAnimation("idle", loop: true);
			_owner.view.BodyAnimation("idle", loop: true);
		}

		public override void OnUpdate()
		{
			_remain -= Time.deltaTime;
			if (_remain < 0f)
			{
				Vector3 vector = new Vector3(_owner.agent.gameObject.transform.position.x, _owner.agent.gameObject.transform.position.z / 2f, _owner.transform.position.z);
				if (Vector2.Distance(_owner.transform.position, vector) > 0.1f)
				{
					_owner.moveType = EMoveType.None;
					_owner.destinationPosition = vector;
					_owner.ChangeState(EState.MoveToPosition);
				}
				else
				{
					_owner.ChangeState(EState.Move);
				}
			}
		}
	}
}
