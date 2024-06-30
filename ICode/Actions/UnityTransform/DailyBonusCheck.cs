using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class DailyBonusCheck : StateAction
	{
		public override void OnEnter()
		{
			RemoteObjectManager.instance.RequestDailyBonusCheck();
			Finish();
		}
	}
}
