using UnityEngine;

namespace RoomDecorator.Model
{
	public class Attach : State
	{
		public Furniture target;

		public AttachCharacterPoint point;

		private float _remain;

		public Attach(Character owner)
			: base(owner)
		{
		}

		public override void OnEnter()
		{
			_remain = Random.Range(10f, 20f);
			_owner.transform.position = point.transform.position;
			_owner.SetDirection(point.transform.forward);
			_owner.agent.Warp(new Vector3(_owner.transform.position.x, 0f, _owner.transform.position.y * 2f));
			_owner.view.HeadAnimation(null);
			_owner.view.BodyAnimation(null);
			_owner.view.SetToSetupPose();
			_owner.view.HeadAnimation(point.headAnimationName, loop: true);
			_owner.view.BodyAnimation(point.bodyAnimationName, loop: true);
			if (target.fixedSkin)
			{
				_owner.view.EnableAccessory(enable: false);
			}
		}

		public override void OnUpdate()
		{
			_remain -= Time.deltaTime;
			if (_remain < 0f)
			{
				Vector3 destinationPosition = new Vector3(_owner.agent.gameObject.transform.position.x, _owner.agent.gameObject.transform.position.z / 2f, _owner.transform.position.z);
				_owner.moveType = EMoveType.Detach;
				_owner.destinationPosition = destinationPosition;
				_owner.ChangeState(EState.MoveToPosition);
			}
		}

		public override void OnExit()
		{
			if (target != null)
			{
				if (target.fixedSkin)
				{
					_owner.view.EnableAccessory(enable: true);
				}
				Furniture furniture = target;
				target = null;
				point = null;
				furniture.DetachCharacter(_owner);
			}
			_owner.view.HeadAnimation(null);
			_owner.view.BodyAnimation(null);
			_owner.view.SetToSetupPose();
		}
	}
}
