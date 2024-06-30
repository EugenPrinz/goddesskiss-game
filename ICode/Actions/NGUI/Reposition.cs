using System;

namespace ICode.Actions.NGUI
{
	[Serializable]
	[Category("NGUI")]
	[Tooltip("Reposition the elements in a UITable.")]
	public class Reposition : StateAction
	{
		[SharedPersistent]
		[Tooltip("The game object to use.")]
		public FsmGameObject gameObject;

		[Tooltip("Execute the action every frame.")]
		public bool everyFrame;

		private UITable table;

		public override void OnEnter()
		{
			table = gameObject.Value.GetComponent<UITable>();
			table.Reposition();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			table.Reposition();
		}
	}
}
