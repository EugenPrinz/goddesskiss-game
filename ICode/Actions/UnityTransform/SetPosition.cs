using System;
using UnityEngine;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Transform)]
	[Tooltip("Set the position of the transform in world space.")]
	[HelpUrl("http://docs.unity3d.com/Documentation/ScriptReference/Transform-position.html")]
	public class SetPosition : TransformAction
	{
		[Tooltip("Position to set.")]
		public FsmVector3 position;

		[Tooltip("Smooth multiplier.")]
		public FsmFloat smooth;

		[Tooltip("Execute the action every frame.")]
		public bool everyFrame;

		public override void OnEnter()
		{
			base.OnEnter();
			DoSetPosition();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetPosition();
		}

		private void DoSetPosition()
		{
			if (smooth.Value == 0f)
			{
				transform.position = position.Value;
			}
			else
			{
				transform.position = Vector3.Lerp(transform.position, position.Value, Time.deltaTime * smooth.Value);
			}
		}
	}
}
