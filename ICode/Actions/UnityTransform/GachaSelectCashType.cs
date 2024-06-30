using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class GachaSelectCashType : StateAction
	{
		public override void OnEnter()
		{
			UIManager.instance.world.gacha.GachaSelectCashType();
			Finish();
		}
	}
}
