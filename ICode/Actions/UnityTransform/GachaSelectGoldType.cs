using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class GachaSelectGoldType : StateAction
	{
		public override void OnEnter()
		{
			UIManager.instance.world.gacha.GachaSelectGoldType();
			Finish();
		}
	}
}
