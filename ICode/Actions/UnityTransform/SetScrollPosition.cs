using System;
using UnityEngine;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class SetScrollPosition : StateAction
	{
		public FsmGameObject scrollView;

		public FsmVector3 position;

		public Space space;

		public bool everyFrame;

		protected UIScrollView uiScroll;

		public override void OnEnter()
		{
			uiScroll = scrollView.Value.GetComponent<UIScrollView>();
			uiScroll.MoveRelative(-uiScroll.transform.localPosition);
			if (space == Space.World)
			{
				uiScroll.MoveAbsolute(position.Value);
			}
			else
			{
				uiScroll.MoveRelative(position.Value);
			}
			uiScroll.UpdateScrollbars();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			uiScroll.MoveRelative(-uiScroll.transform.localPosition);
			if (space == Space.World)
			{
				uiScroll.MoveAbsolute(position.Value);
			}
			else
			{
				uiScroll.MoveRelative(position.Value);
			}
			uiScroll.UpdateScrollbars();
		}
	}
}
