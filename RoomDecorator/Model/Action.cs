using UnityEngine;

namespace RoomDecorator.Model
{
	public class Action : State
	{
		private float _remain;

		private float _duration;

		private string _animationName;

		private bool _loop;

		private EState[] _nextState;

		public Action(Character owner)
			: base(owner)
		{
		}

		public Action(Character owner, string animationName, float duration, EState[] nextState, bool loop = false)
			: base(owner)
		{
			Set(animationName, duration, nextState, loop);
		}

		public void Set(string animationName, float duration, EState[] nextState, bool loop = false)
		{
			_duration = duration;
			_animationName = animationName;
			_loop = loop;
			_nextState = nextState;
		}

		public override void OnEnter()
		{
			_remain = _duration;
			_owner.view.HeadAnimation(_animationName, _loop);
			_owner.view.BodyAnimation(_animationName, _loop);
		}

		public override void OnUpdate()
		{
			_remain -= Time.deltaTime;
			if (_remain < 0f)
			{
				_owner.ChangeState(_nextState[Random.Range(0, _nextState.Length)]);
			}
		}
	}
}
