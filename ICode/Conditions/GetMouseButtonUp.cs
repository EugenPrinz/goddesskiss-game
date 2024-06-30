using System;
using UnityEngine;

namespace ICode.Conditions
{
	[Serializable]
	[Category(Category.Input)]
	[Tooltip("Returns true during the frame the user releases the given mouse button.")]
	[HelpUrl("http://docs.unity3d.com/ScriptReference/Input.GetMouseButtonUp.html")]
	public class GetMouseButtonUp : Condition
	{
		[Tooltip("Button values are 0 for left button, 1 for right button, 2 for the middle button.")]
		public FsmInt button;

		public override bool Validate()
		{
			return Input.GetMouseButtonUp(button.Value);
		}
	}
}
