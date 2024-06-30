using System;
using UnityEngine;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Transform)]
	[Tooltip("Parents the GameObject to target.")]
	[HelpUrl("")]
	public class Parent : TransformAction
	{
		[NotRequired]
		[SharedPersistent]
		public FsmGameObject target;

		public FsmBool worldPositionSaves;

		[Tooltip("Execute the action every frame.")]
		public bool everyFrame;

		private Transform mTarget;

		public override void OnEnter()
		{
			base.OnEnter();
			mTarget = ((!(target.Value != null)) ? null : target.Value.transform);
			DoParent();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoParent();
		}

		private void DoParent()
		{
			transform.SetParent(mTarget, worldPositionSaves.Value);
		}
	}
}
