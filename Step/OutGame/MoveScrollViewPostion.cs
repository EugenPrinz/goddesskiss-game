using UnityEngine;

namespace Step.OutGame
{
	public class MoveScrollViewPostion : AbstractStepAction
	{
		public GameObjectData scrollView;

		public Vector3 targetPostion;

		protected UIScrollView _scrollView;

		public override bool IsLock
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		public override bool IsEveryFrameUpdate => true;

		public override bool Enter()
		{
			if (scrollView == null || scrollView.value == null)
			{
				return false;
			}
			_scrollView = scrollView.value.GetComponent<UIScrollView>();
			if (_scrollView == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			_scrollView.transform.localPosition = targetPostion;
			_isFinish = true;
		}
	}
}
