using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class GetUserLevel : StateAction
	{
		[Shared]
		public FsmInt store;

		public bool everyFrame;

		public override void OnEnter()
		{
			store.Value = RemoteObjectManager.instance.localUser.level;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			store.Value = RemoteObjectManager.instance.localUser.level;
		}
	}
}
