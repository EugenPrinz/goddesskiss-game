using UnityEngine;

namespace Step
{
	public abstract class AbstractStep : MonoBehaviour
	{
		protected bool _enable;

		protected int _stepNum;

		protected bool _isFinish;

		public virtual int StepNum
		{
			get
			{
				return _stepNum;
			}
			set
			{
				_stepNum = value;
			}
		}

		public virtual bool IsPriority
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public virtual bool IsLock
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		public virtual bool IsFinish
		{
			get
			{
				return _isFinish;
			}
			set
			{
				_isFinish = value;
			}
		}

		public virtual bool IsEveryFrameUpdate
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public virtual bool Enter()
		{
			_enable = true;
			if (!IsEveryFrameUpdate)
			{
				_isFinish = true;
			}
			OnEnter();
			return true;
		}

		public virtual bool Exit()
		{
			OnExit();
			Object.DestroyImmediate(this);
			return true;
		}

		public virtual void UpdateStep()
		{
			if (CanUpdate())
			{
				OnUpdate();
			}
		}

		protected virtual bool CanUpdate()
		{
			if (!_enable)
			{
				return false;
			}
			if (_isFinish)
			{
				return false;
			}
			if (!IsEveryFrameUpdate)
			{
				return false;
			}
			return true;
		}

		protected virtual void OnEnter()
		{
		}

		protected virtual void OnExit()
		{
		}

		protected virtual void OnUpdate()
		{
		}
	}
}
