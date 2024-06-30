using System;

namespace ICode.Actions
{
	[Serializable]
	[Category(Category.Debug)]
	[Tooltip("Prints a message to the console.")]
	[HelpUrl("http://docs.unity3d.com/Documentation/ScriptReference/Debug.Log.html")]
	public class Log : StateAction
	{
		[Tooltip("Message to print.")]
		public FsmString message;

		[Tooltip("Execute the action every frame.")]
		public bool everyFrame;

		public override void OnEnter()
		{
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
		}
	}
}
